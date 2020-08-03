using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCLAcademy.Controllers
{
    public class AuthorizeController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        //
        // GET: /Authorize/
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Login()
        {
            if (Request.IsAuthenticated)
            {
              
                var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                string email = userClaims?.FindFirst(System.IdentityModel.Claims.ClaimTypes.Name)?.Value;
                await InitializeServiceClient(email);
                string name = "";
                string serviceuserName = ConfigurationManager.AppSettings["SP_ServiceAccountID"].ToString();
                if (serviceuserName.ToUpper() == email.ToUpper())
                {
                    string[] parts = serviceuserName.Split("@".ToCharArray());
                    name = parts[0];
                }
                else
                {
                    name = userClaims?.FindFirst("name")?.Value;
                }
                
                HttpResponseMessage rolesresponse = await client.PostAsJsonAsync("User/GetUserActiveStatus?emailAddress=" + email, req);
                string activeStatus = await rolesresponse.Content.ReadAsAsync<string>();
                if (activeStatus.ToUpper() == "TRUE")
                {
                    SPAuthUtility objAuth = new SPAuthUtility();
                    UserManager user = await objAuth.AuthorizeServiceAccount(email, name);
                    
                    if (user != null && user.GroupPermission > 0)
                    {
                        Session.Add("IsOnline", user.IsOnline);

                        HttpResponseMessage admininforesponse = await client.PostAsJsonAsync("Project/GetProjectAdminInfo?userid=" + user.DBUserId, req);
                        ProjectAdminInfo admininfo = await admininforesponse.Content.ReadAsAsync<ProjectAdminInfo>();
                        user.Admininfo = admininfo;
                        //  FormsAuthentication.SetAuthCookie(objAuth.DisplayName, objAuth.RememberMe);

                        if (Session["CurrentUser"] == null)
                        {
                            Session.Add("CurrentUser", user);
                        }
                        else
                        {
                            Session.Remove("CurrentUser");
                            Session.Add("CurrentUser", user);
                        }
                        return RedirectToAction("Home", "Home");
                    }

                    if (user != null && user.GroupPermission == 0)
                    {
                        ModelState.AddModelError("", "You are not Authorized to Log in");
                    }

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Username and/or Password is incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your userid has been deactivated.Please contact administrator");

                }
            }
            FetchAppConfig();
            return View();
        }
        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        ///// <summary>
        ///// Send an OpenID Connect sign-out request.
        ///// </summary>
        //public void SignOut()
        //{
        //    HttpContext.GetOwinContext().Authentication.SignOut(
        //        OpenIdConnectAuthenticationDefaults.AuthenticationType,
        //        CookieAuthenticationDefaults.AuthenticationType);
        //}
        [HttpGet]
        public ActionResult SiteMaintanance()
        {
            return View();
        }
        /// <summary>
        /// Check whether a user is authorized to Login to the website
        /// </summary>
        /// <param name="objAuth"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(SPAuthUtility objAuth)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    InitializeServiceClient(objAuth.UserName);
                    HttpResponseMessage rolesresponse = await client.PostAsJsonAsync("User/GetUserActiveStatus?emailAddress=" + objAuth.UserName, req);
                    string activeStatus = await rolesresponse.Content.ReadAsAsync<string>();
                    if (activeStatus.ToUpper() == "TRUE")
                    {
                        
                        HttpResponseMessage response = await client.PostAsJsonAsync("ExternalUser/GetExternalUserByUserName?UserName=" + objAuth.UserName, req);
                        ExternalUser authResponse = await response.Content.ReadAsAsync<ExternalUser>();
                        //if (authResponse.UserName != null)
                        //{
                        //    string serviceuserName = ConfigurationManager.AppSettings["SP_ServiceAccountID"].ToString();
                        //    string name = authResponse.Name;
                        //}
                        
                        #region AspIdentity
                       // This doesn't count login failures towards account lockout
                       // To enable password failures to trigger account lockout, change to shouldLockout: true
                       var result = await SignInManager.PasswordSignInAsync(objAuth.UserName, objAuth.Password, false, true);
                       
                        #endregion
                        if (result.ToString().ToUpper()=="FAILURE")
                        {
                            ModelState.AddModelError("", "Username and/or Password is incorrect.");
                        }
                        else if (result.ToString().ToUpper() == "LOCKEDOUT")
                        {
                            ModelState.AddModelError("", "Your account is lockedout.Try after some time.");
                        }
                        else if (result.ToString().ToUpper() == "SUCCESS")
                        {
                            ExternalUserRequest extuser = new ExternalUserRequest();
                            extuser.ClientInfo = req.ClientInfo;
                            extuser.UserName = objAuth.UserName;
                            HttpResponseMessage groupresponse = await client.PostAsJsonAsync("ExternalUser/GetUserMemberShip", extuser);
                            List<UserGroupMemberShip> groupMemberships = await groupresponse.Content.ReadAsAsync<List<UserGroupMemberShip>>();
                            int groupPermission = 0;
                            List<string> groups = new List<string>();
                            if (groupMemberships.Count > 0)
                            {
                                for (int i = 0; i < groupMemberships.Count; i++)
                                {
                                    groups.Add(groupMemberships[i].GroupName);
                                    if (groupMemberships[i].GroupPermission > groupPermission)
                                        groupPermission = groupMemberships[i].GroupPermission;

                                }
                            }
                            UserManager user = new UserManager();
                            user.GroupPermission = groupPermission;
                            user.Groups = groups;
                            user.UserName = authResponse.Name;
                            user.DBUserId = Convert.ToInt32(authResponse.UserId);
                            user.EmailID = authResponse.UserName;
                            user.IsExternalUser = true;

                            if (user != null && user.GroupPermission > 0)
                            {
                                HttpResponseMessage admininforesponse = await client.PostAsJsonAsync("Project/GetProjectAdminInfo?userid=" + user.DBUserId, req);
                                ProjectAdminInfo admininfo = await admininforesponse.Content.ReadAsAsync<ProjectAdminInfo>();
                                user.Admininfo = admininfo;
                                if (Session["CurrentUser"] == null)
                                {
                                    Session.Add("CurrentUser", user);
                                }
                                else
                                {
                                    Session.Remove("CurrentUser");
                                    Session.Add("CurrentUser", user);
                                }
                                if(authResponse.FirstPasswordChanged)
                                    return RedirectToAction("Home", "Home");
                                else
                                    return RedirectToAction("ResetPassword", "Account");
                            }

                            if (user != null && user.GroupPermission == 0)
                            {
                                ModelState.AddModelError("", "You are not Authorized to Log in");
                            }

                            if (user == null)
                            {
                                ModelState.AddModelError("", "Username and/or Password is incorrect");
                            }
                        }


                    }
                    else
                    {
                        ModelState.AddModelError("", "Your userid has been deactivated.Please contact administrator");

                    }
                }
            }
            catch (Exception ex)
            {
                // Utilities.LogToEventVwr(ex.Message, 0);
                if (ex.Message.Contains("denied"))
                {
                    ModelState.AddModelError("", "You are not Authorized to Log in");
                }
                else
                {
                    ModelState.AddModelError("", "Username and/or Password is incorrect");
                }

            }
            return View(objAuth);
        }
        /// <summary>
        /// Logout from the application
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
           // FormsAuthentication.SignOut();

            Session.Clear();

            Response.RemoveOutputCacheItem("/home/getlearningjourney");
            Response.RemoveOutputCacheItem("/home/getassessments");
            Response.RemoveOutputCacheItem("/wiki/wikipolicy");
            Response.RemoveOutputCacheItem("/wiki/wikidocumenttree");
            Response.RemoveOutputCacheItem("/onboard/onboarding");
            Response.RemoveOutputCacheItem("/training/training");

            // Removing session variables
            System.Web.HttpContext.Current.Session.Remove("StartAssessment");
            System.Web.HttpContext.Current.Session.Remove("UserSiteMenu");
            System.Web.HttpContext.Current.Session.Remove("BannersList");
            System.Web.HttpContext.Current.Session.Remove("CurrentUser");
            System.Web.HttpContext.Current.Session.Remove("LogoBase64Image");
            System.Web.HttpContext.Current.Session.Remove("Projects");
            System.Web.HttpContext.Current.Session.Remove("EditProject");
            System.Web.HttpContext.Current.Session.Remove("SPCredential");
            System.Web.HttpContext.Current.Session.Remove("AcademyConfig");
            System.Web.HttpContext.Current.Session.Remove("IsOnline");
            System.Web.HttpContext.Current.Session.Remove("UserRole");

            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllVideoData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllWikiPolicyData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllSkillData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllEventsData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllCompetencyData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllTrainingData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.DefaultTrainingAssessmentData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllAssessmentData);
            System.Web.HttpContext.Current.Session.Remove(AppConstant.AllGEOData);
            HttpContext.GetOwinContext().Authentication.SignOut(
               OpenIdConnectAuthenticationDefaults.AuthenticationType,
               CookieAuthenticationDefaults.AuthenticationType);
            //return RedirectToAction("Login", "Authorize");
        }
        /// <summary>
        /// Fetches the configuration settings for the application
        /// </summary>
        public void FetchAppConfig()
        {
            Session["ClientName"] = null;
            if (ConfigurationManager.AppSettings["ClientName"] != null)
                Session["ClientName"] = ConfigurationManager.AppSettings["ClientName"].ToString();
        }
    }
}