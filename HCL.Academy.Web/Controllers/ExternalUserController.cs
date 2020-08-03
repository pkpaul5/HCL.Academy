using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCL.Academy;
using HCLAcademy.Util;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ExternalUserController : BaseController
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: ExternalUser
        public async Task<ActionResult> Index()
        {
            List<ExternalUser> externalUsers = new List<ExternalUser>();
            InitializeServiceClient();
            HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/GetAllExternalUsers", req);
            externalUsers = await externalResponse.Content.ReadAsAsync<List<ExternalUser>>();
            return View(externalUsers);
        }

        // GET: ExternalUser/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExternalUser/Create
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
            ExternalUser extUser = new ExternalUser();
            HttpResponseMessage orgResponse = await client.PostAsJsonAsync("ExternalUser/GetAllOrganizations", req);
            ViewBag.lstOrganizations = await orgResponse.Content.ReadAsAsync<List<Organization>>();
            HttpResponseMessage groupResponse = await client.PostAsJsonAsync("ExternalUser/GetAllUserGroups", req);
            ViewBag.lstGroups = await groupResponse.Content.ReadAsAsync<List<UserGroup>>();
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
            ViewBag.CompetenceList = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            ViewBag.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
            return View(extUser);
        }

        // POST: ExternalUser/Create
        [HttpPost]
        public async Task<ActionResult> Create(ExternalUser externalUser)
        {
            InitializeServiceClient();
            try
            {

                if (externalUser.GroupId == 0)
                {
                    ModelState.AddModelError("SelectedGroup", "Please select a Group");
                    return View(externalUser);
                }
                else if (externalUser.OrganizationID == 0)
                {
                    ModelState.AddModelError("SelectedOrganization", "Please select an Organization");
                    return View(externalUser);
                }
                else if (externalUser.UserName == "")
                {
                    ModelState.AddModelError("UserName", "Email is required");
                    return View(externalUser);
                }
                ExternalUserRequest user = new ExternalUserRequest();
                user.ClientInfo = req.ClientInfo;
                user.EmployeeId = externalUser.EmployeeId;
                user.CompetencyLevelId = externalUser.CompetencyLevelId;
                user.SkillId = externalUser.SkillId;
                user.RoleId = externalUser.RoleId;
                user.GEOId = externalUser.GEOId;
                user.GroupId = externalUser.GroupId;
                user.Name = externalUser.Name;
                user.OrganizationId = externalUser.OrganizationID;
                user.UserName = externalUser.UserName;
                string randompassword = PasswordHelper.GeneratePassword(true, true, true, true, false, 8);
                var keyNew = PasswordHelper.GenerateSalt(10);
                var hashedpassword = PasswordHelper.EncodePassword(randompassword, keyNew);
                user.EncryptedPassword = hashedpassword;
                user.Password = randompassword;
                user.PasswordSalt = keyNew;
                user.Id = 0;
                HttpResponseMessage response = await client.PostAsJsonAsync("ExternalUser/SaveExternalUser", user);
                string result = await response.Content.ReadAsAsync<string>();

                #region AspIdentity
                var Identityuser = new ApplicationUser { UserName = externalUser.UserName, Email = externalUser.UserName };
                var Identityresult = await IdentityUserManager.CreateAsync(Identityuser, user.Password);
                #endregion End ASPIdentity
                if (Identityresult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ExternalUser extUser = new ExternalUser();
                    HttpResponseMessage orgResponse = await client.PostAsJsonAsync("ExternalUser/GetAllOrganizations", req);
                    ViewBag.lstOrganizations = await orgResponse.Content.ReadAsAsync<List<Organization>>();
                    HttpResponseMessage groupResponse = await client.PostAsJsonAsync("ExternalUser/GetAllUserGroups", req);
                    ViewBag.lstGroups = await groupResponse.Content.ReadAsAsync<List<UserGroup>>();
                    HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                    ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

                    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                    ViewBag.CompetenceList = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
                    HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                    ViewBag.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                    HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                    ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
                    TempData["msg"] = "Addition unsuccessful";
                    return View(extUser);
                }
                    //return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ExternalUser extUser = new ExternalUser();
                HttpResponseMessage orgResponse = await client.PostAsJsonAsync("ExternalUser/GetAllOrganizations", req);
                ViewBag.lstOrganizations = await orgResponse.Content.ReadAsAsync<List<Organization>>();
                HttpResponseMessage groupResponse = await client.PostAsJsonAsync("ExternalUser/GetAllUserGroups", req);
                ViewBag.lstGroups = await groupResponse.Content.ReadAsAsync<List<UserGroup>>();
                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                ViewBag.CompetenceList = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
                HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                ViewBag.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
                TempData["msg"] = "Addition unsuccessful";
                return View(extUser);
            }
        }

        public ApplicationUserManager IdentityUserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: ExternalUser/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            InitializeServiceClient();
            HttpResponseMessage orgResponse = await client.PostAsJsonAsync("ExternalUser/GetAllOrganizations", req);
            ViewBag.lstOrganizations = await orgResponse.Content.ReadAsAsync<List<Organization>>();
            HttpResponseMessage groupResponse = await client.PostAsJsonAsync("ExternalUser/GetAllUserGroups", req);
            ViewBag.lstGroups = await groupResponse.Content.ReadAsAsync<List<UserGroup>>();
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
            ViewBag.CompetenceList = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            ViewBag.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();

            HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/GetAllExternalUsers", req);
            List<ExternalUser> externalUsers = await externalResponse.Content.ReadAsAsync<List<ExternalUser>>();
            ExternalUser user = externalUsers.Where(e => e.ID == id).FirstOrDefault();
            return View(user);
        }

        // POST: ExternalUser/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, ExternalUser externalUser, FormCollection collection)
        {
            InitializeServiceClient();
            try
            {
                if (externalUser.GroupId == 0)
                {
                    ModelState.AddModelError("SelectedGroup", "Please select a Group");
                    return View(externalUser);
                }
                else if (externalUser.OrganizationID == 0)
                {
                    ModelState.AddModelError("SelectedOrganization", "Please select an Organization");
                    return View(externalUser);
                }
                else if (externalUser.UserName == "")
                {
                    ModelState.AddModelError("UserName", "Email is required");
                    return View(externalUser);
                }
                ExternalUserRequest user = new ExternalUserRequest();
                user.Id = externalUser.ID;
                user.ClientInfo = req.ClientInfo;
                user.EmployeeId = externalUser.EmployeeId;
                user.CompetencyLevelId = externalUser.CompetencyLevelId;
                user.SkillId = externalUser.SkillId;
                // user.RoleId = externalUser.RoleId;
                user.GEOId = externalUser.GEOId;
                user.GroupId = externalUser.GroupId;
                user.Name = externalUser.Name;
                user.OrganizationId = externalUser.OrganizationID;
                user.UserName = externalUser.UserName;
                // string randompassword = Utilities.GeneratePassword(true, true, true, true, false, 8);
                //  var keyNew = Utilities.GenerateSalt(10);
                // var hashedpassword = Utilities.EncodePassword(randompassword, keyNew);
                user.EncryptedPassword = "";
                user.Password = "";
                user.PasswordSalt = "";
                HttpResponseMessage response = await client.PostAsJsonAsync("ExternalUser/SaveExternalUser", user);
                string result = await response.Content.ReadAsAsync<string>();
                return RedirectToAction("Index");
            }
            catch
            {
                HttpResponseMessage orgResponse = await client.PostAsJsonAsync("ExternalUser/GetAllOrganizations", req);
                ViewBag.lstOrganizations = await orgResponse.Content.ReadAsAsync<List<Organization>>();
                HttpResponseMessage groupResponse = await client.PostAsJsonAsync("ExternalUser/GetAllUserGroups", req);
                ViewBag.lstGroups = await groupResponse.Content.ReadAsAsync<List<UserGroup>>();
                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                ViewBag.CompetenceList = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
                HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                ViewBag.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
                TempData["msg"] = "Update unsuccessful";
                return View(externalUser);
            }
        }

        // GET: ExternalUser/Delete/5
        public async Task<ActionResult> Delete(int id,string email)
        {
            InitializeServiceClient();
            HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/DeleteExternalUser?id=" + id, req);
            bool flag = await externalResponse.Content.ReadAsAsync<bool>();
            UserManager curuser = (UserManager)Session["CurrentUser"];
            ApplicationUser user = await IdentityUserManager.FindByEmailAsync(email);
            if (user != null)
            { 
                IdentityResult result = await UserManager.DeleteAsync(user);

                if (result.Succeeded)
                {  
                    return RedirectToAction("Index");
                }
                else
                {

                    //LogHelper.AddLog("AcademyConfigController", "User Deletion failed", "User Deletion failed", "HCL.Academy.Web", curuser.EmailID);
                    Trace.TraceInformation("User Deletion failed");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Trace.TraceInformation("User Deletion failed");
                //LogHelper.AddLog("AcademyConfigController", "User Deletion failed", "User Deletion failed", "HCL.Academy.Web", curuser.EmailID);
                return RedirectToAction("Index");
            }

            //return View("Index", userManager.Users);

            //InitializeServiceClient();
            //HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/DeleteExternalUser?id="+id, req);
            //bool flag = await externalResponse.Content.ReadAsAsync<bool>();
            //return RedirectToAction("Index");
        }

        // POST: ExternalUser/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: ExternalUser/Create
        [HttpGet]
        public async Task<ActionResult> ChangePassword(int id)
        {
            InitializeServiceClient();
            HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/GetAllExternalUsers", req);
            List<ExternalUser> externalUsers = await externalResponse.Content.ReadAsAsync<List<ExternalUser>>();
            ExternalUser user = externalUsers.Where(e => e.ID == id).FirstOrDefault();
            return View(user);
        }
        // POST: ExternalUser/Create
        //[HttpPost]
        //[SubmitButtonSelector(Name = "UpdatePassword")]
        //public async Task<ActionResult> UpdatePassword(ExternalUser externalUser)
        //{
        //    InitializeServiceClient();
        //    ExternalUserRequest user = new ExternalUserRequest();
        //    user.ClientInfo = req.ClientInfo;
        //    var keyNew = PasswordHelper.GenerateSalt(10);
        //    var hashedpassword = PasswordHelper.EncodePassword(externalUser.Password, keyNew);
        //    user.EncryptedPassword = hashedpassword;
        //    user.Password = externalUser.Password;
        //    user.PasswordSalt = keyNew;
        //    user.Id = externalUser.ID;
        //    user.UserName = externalUser.UserName;
        //    user.Name = externalUser.Name;
        //    HttpResponseMessage externalResponse = await client.PostAsJsonAsync("ExternalUser/ResetExternalUserPassword", user);
        //    string result = await externalResponse.Content.ReadAsAsync<string>();
        //    return RedirectToAction("Index");
        //}

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        [SubmitButtonSelector(Name = "UpdatePassword")]
        public async Task<ActionResult> UpdatePassword(ExternalUser model)
        {
            InitializeServiceClient();
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("UpdatePassword", "Index");
            }
            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                TempData["msg"] = "Update Successful";
                ExternalUserRequest externalUser = new ExternalUserRequest();
                externalUser.ClientInfo = req.ClientInfo;
                externalUser.UserName = model.UserName;
                externalUser.Password = model.Password;
                HttpResponseMessage response = await client.PostAsJsonAsync("ExternalUser/SendExternalUserPassword", externalUser);
                bool resultsendmail = await response.Content.ReadAsAsync<bool>();

                return View(model);
            }
            AddErrors(result);
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

    }
}
