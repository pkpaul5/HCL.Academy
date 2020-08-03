using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net;
using System.Security;
using System.Web;
using HCL.Academy.Model;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Linq;
using System.IO;
using HCLAcademy.Util;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.SharePoint.Client.Utilities;
using OfficeDevPnP.Core;
namespace HCLAcademy
{
    public class SPAuthUtility
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public ClientContext ClientContext { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }


        Web web = null;
        string url = string.Empty;

        #region SP 2013 Online credential
        /// <summary>
        /// Get SP 2013 Online Credentials
        /// </summary>
        /// <returns></returns>
        private static SharePointOnlineCredentials GetSpOnlineCredential(Uri webUri, string userName, string password)
        {
            try
            {
                var securePassword = new SecureString();
                foreach (var ch in password) securePassword.AppendChar(ch);

                return new SharePointOnlineCredentials(userName, securePassword);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
  
        /// <summary>
        /// Get SP 2013 Online User Details
        /// </summary>
        /// <returns></returns>
        public async Task<UserManager> AuthorizeServiceAccount(string email,string name)
        {
            UserManager user = new UserManager();
            
            try
            {

                string url = ConfigurationManager.AppSettings["URL"].ToString();
                Uri uri = new Uri(url, UriKind.Absolute);
                var isOnline = false;
                
                string SharepointPlatform = ConfigurationManager.AppSettings["SharepointPlatform"].ToString();
              //  string serviceuserName = ConfigurationManager.AppSettings["SP_ServiceAccountID"].ToString();
                //string servicePassword = ConfigurationManager.AppSettings["SP_ServiceAccountPWD"].ToString();

                if (SharepointPlatform == "SPOnline")
                {
                    isOnline = true;
                }

                user.IsOnline = isOnline;

                try
                {
                    //Get User Groups
                    string spReaderGroup = ConfigurationManager.AppSettings["AcademyReaderGroup"].ToString();
                    string spMemberGroup = ConfigurationManager.AppSettings["AcademyMemberGroup"].ToString();
                    string spOwnerGroup = ConfigurationManager.AppSettings["AcademyOwnerGroup"].ToString();
                    string spPmoGroup = ConfigurationManager.AppSettings["AcademyPMO"].ToString();
                    string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                    string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();

                    using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                    {
                        cc.Load(cc.Web, w => w.Url,
                                                    w => w.SiteGroups,
                                                    w => w.CurrentUser,
                                                    w => w.CurrentUser.Groups);
                        cc.ExecuteQuery();
                        //Assign values to custom object
                        //user.SPUserId = web.CurrentUser.Id;
                        user.SPUserId = 0;
                        user.EmailID = email;
                        user.UserName = name;
                        //  DisplayName = web.CurrentUser.Title;
                        // user.isSiteAdmin = web.CurrentUser.IsSiteAdmin;
                        user.GroupPermission = 0;
                        user.IsExternalUser = false;
                        //Groups
                        List<string> grp = new List<string>();
                        GroupCollection spgroups = cc.Web.SiteGroups;
                        bool readergroupchecked = false;
                        bool ownergroupchecked = false;
                        bool membergroupchecked = false;
                        bool pmogroupchecked = false;
                        //if (serviceuserName.ToUpper() == email.ToUpper())
                        //{
                        //    user.GroupPermission = 4;
                        //    user.Groups = new List<string>();
                        //    user.Groups.Add(spOwnerGroup);
                        //    user.Groups.Add(spPmoGroup);
                        //}
                        //else
                        //{
                            foreach (Group g in spgroups)
                            {
                                if (g.Title.ToUpper() == spReaderGroup.ToUpper())
                                {
                                    cc.Load(g.Users);
                                    cc.ExecuteQuery();
                                    foreach (User usr in g.Users)
                                    {
                                        if (email.ToUpper() == usr.Email.ToUpper())
                                        {
                                            grp.Add(g.Title);
                                            user.GroupPermission = 1;
                                            break;
                                        }
                                    }
                                    readergroupchecked = true;
                                }
                                else if (g.Title.ToUpper() == spMemberGroup.ToUpper())
                                {
                                    cc.Load(g.Users);
                                    cc.ExecuteQuery();
                                    foreach (User usr in g.Users)
                                    {
                                        if (email.ToUpper() == usr.Email.ToUpper())
                                        {
                                            grp.Add(g.Title);
                                            user.GroupPermission = 2;
                                            break;
                                        }
                                    }
                                    membergroupchecked = true;
                                }
                                else if (g.Title.ToUpper() == spOwnerGroup.ToUpper())
                                {
                                    cc.Load(g.Users);
                                    cc.ExecuteQuery();
                                    foreach (User usr in g.Users)
                                    {
                                        if (email.ToUpper() == usr.Email.ToUpper())
                                        {
                                            grp.Add(g.Title);
                                            user.GroupPermission = 3;
                                            break;
                                        }
                                    }
                                    ownergroupchecked = true;
                                }
                                else if (g.Title.ToUpper() == spPmoGroup.ToUpper())
                                {
                                    cc.Load(g.Users);
                                    cc.ExecuteQuery();
                                    foreach (User usr in g.Users)
                                    {
                                        if (email.ToUpper() == usr.Email.ToUpper())
                                        {
                                            grp.Add(g.Title);
                                            user.GroupPermission = 4;
                                            break;
                                        }
                                    }
                                    pmogroupchecked = true;
                                }
                                if (readergroupchecked == true && membergroupchecked == true && ownergroupchecked == true && pmogroupchecked == true)
                                {
                                    break;
                                }
                            }
                            user.Groups = grp;                            
                       // }

                        //  ClientContext = cc;
                        //  user.SPCredential = cc.Credentials;
                        string dataStoreConfigValue = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                        if (dataStoreConfigValue == DataStore.SQLServer)
                        {
                            HttpClient client = new HttpClient();
                            string token =HttpContext.Current.Session["JWT"] as string;
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            string serviceBaseURL = ConfigurationManager.AppSettings["AcademyServiceEndPoint"].ToString();
                            client.BaseAddress = new Uri(serviceBaseURL);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            RequestBase req = new RequestBase();
                            req.ClientInfo = new ServiceConsumerInfo();
                            req.ClientInfo.emailId = user.EmailID;
                            req.ClientInfo.name = user.UserName;                            
                            req.ClientInfo.Groups = user.Groups;
                            HttpResponseMessage responseID = await client.PostAsJsonAsync("User/GetUserId?emailId=" + email, req);
                            user.DBUserId = await responseID.Content.ReadAsAsync<int>();
                        }
                    };
                    
                }
                catch (Exception ex)
                {
                    LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", email);
                    return user;
                }
                return user;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", email);
                user = null;
                return user;
            }
        }
        #region Authorize the User
        /// <summary>
        /// Get SP 2013 Online User Details
        /// </summary>
        /// <returns></returns>
        public async Task<UserManager> Authorize(string userName, string password)
        {
            UserManager user = new UserManager();

            try
            {

                string url = ConfigurationManager.AppSettings["URL"].ToString();
                Uri uri = new Uri(url, UriKind.Absolute);
                var isOnline = false;
                string SharepointPlatform = ConfigurationManager.AppSettings["SharepointPlatform"].ToString();

                if (SharepointPlatform == "SPOnline")
                {
                    isOnline = true;
                }

                user.IsOnline = isOnline;

                try
                {
                    //Get User Groups
                    string spReaderGroup = ConfigurationManager.AppSettings["AcademyReaderGroup"].ToString();
                    string spMemberGroup = ConfigurationManager.AppSettings["AcademyMemberGroup"].ToString();
                    string spOwnerGroup = ConfigurationManager.AppSettings["AcademyOwnerGroup"].ToString();
                    string spPmoGroup = ConfigurationManager.AppSettings["AcademyPMO"].ToString();

                    //Get Client Context
                    using (ClientContext clientContext = new ClientContext(url))
                    {
                        if (isOnline)
                        {
                            var credential = GetSpOnlineCredential(uri, userName, password);
                            clientContext.Credentials = credential;
                        }
                        else
                        {
                            NetworkCredential credential = new NetworkCredential(userName, password);
                            clientContext.Credentials = credential;
                        }

                        web = clientContext.Web;
                        clientContext.Load(web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                        clientContext.Load(web.CurrentUser);
                        clientContext.ExecuteQuery();

                        //Assign values to custom object
                        user.SPUserId = web.CurrentUser.Id;
                        user.EmailID = web.CurrentUser.Email;
                        user.UserName = web.CurrentUser.Title;
                        DisplayName = web.CurrentUser.Title;
                        user.isSiteAdmin = web.CurrentUser.IsSiteAdmin;

                        //Groups
                        List<string> grp = new List<string>();

                        foreach (Microsoft.SharePoint.Client.Group gp in web.CurrentUser.Groups)
                        {
                            grp.Add(gp.Title);
                        }

                        int groupValidation = 0;

                        //Reader Group
                        if (grp.Contains(spReaderGroup))
                        {
                            groupValidation = 1;
                            user.Groups = grp;
                            user.GroupPermission = groupValidation;
                        }

                        //Members Group
                        if (grp.Contains(spMemberGroup))
                        {
                            groupValidation = 2;
                            user.Groups = grp;
                            user.GroupPermission = groupValidation;
                        }

                        //Owners Group
                        if (grp.Contains(spOwnerGroup))
                        {
                            groupValidation = 3;
                            user.Groups = grp;
                            user.GroupPermission = groupValidation;
                        }

                        //Owners Group
                        if (grp.Contains(spPmoGroup))
                        {
                            groupValidation = 4;
                            user.Groups = grp;
                            user.GroupPermission = groupValidation;
                        }

                        if (groupValidation == 0)
                        {
                            user.Groups = null;
                            user.GroupPermission = groupValidation;
                        }
                        ClientContext = clientContext;
                        user.SPCredential = ClientContext.Credentials;
                        string dataStoreConfigValue = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                        if (dataStoreConfigValue == DataStore.SQLServer)
                        {
                            //SqlSvrDAL dal = new SqlSvrDAL();

                            //user.DBUserId = dal.GetUserId(web.CurrentUser.Email);
                            //UserManager user = (UserManager)Session["CurrentUser"];
                            HttpClient client = new HttpClient();
                            string serviceBaseURL = ConfigurationManager.AppSettings["AcademyServiceEndPoint"].ToString();
                            client.BaseAddress = new Uri(serviceBaseURL);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            RequestBase req = new RequestBase();
                            req.ClientInfo = new ServiceConsumerInfo();
                            req.ClientInfo.emailId = user.EmailID;
                            //req.ClientInfo.id = user.DBUserId;
                            req.ClientInfo.name = user.UserName;
                            req.ClientInfo.spCredential = user.SPCredential;
                            req.ClientInfo.spUserId = user.SPUserId;
                            req.ClientInfo.Groups = user.Groups;

                            //  HttpResponseMessage responseID = await client.PostAsJsonAsync("User/GetUserId?emailId=" + web.CurrentUser.Email, req);
                            HttpResponseMessage responseID = await client.PostAsJsonAsync("User/GetUserId?emailId=" + userName, req);
                            user.DBUserId = await responseID.Content.ReadAsAsync<int>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return user;
                }

                //HttpContext.Current.Session["SPCredential"] = ClientContext.Credentials;

                return user;
            }
            catch (Exception)
            {
                user = null;
                return user;
            }
        }
        #endregion
        public bool AddUserToGroup(string email, ref int employeeId)
        {
            bool result = false;
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();                
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    
                    string spAcademyMemberGroup = ConfigurationManager.AppSettings["AcademyMemberGroup"].ToString();
                    User oUser = null;
                    Microsoft.SharePoint.Client.Group spGrp = null;
                    spGrp = cc.Web.SiteGroups.GetByName(spAcademyMemberGroup);
                    cc.Load(spGrp);
                    oUser = cc.Web.EnsureUser("i:0#.f|membership|" + email);
                    cc.Load(oUser);

                    // Adding users to the Group                      
                    spGrp.Users.AddUser(oUser);
                    spGrp.Update();
                    cc.ExecuteQuery();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", email);
            }
            return result;
            
        }
        public bool RemoveUserFromGroup(string email)
        {
            bool result = false;
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();          
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    string spAcademyMemberGroup = ConfigurationManager.AppSettings["AcademyMemberGroup"].ToString();
                    User oUser = null;
                    Microsoft.SharePoint.Client.Group spGrp = null;
                    spGrp = cc.Web.SiteGroups.GetByName(spAcademyMemberGroup);
                    cc.Load(spGrp);
                    oUser = cc.Web.EnsureUser("i:0#.f|membership|" + email);
                    cc.Load(oUser);
                    spGrp.Users.Remove(oUser);
                    spGrp.Update();
                    cc.ExecuteQuery();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", currentUser.EmailID);
                
            }
            return result;            
            
        }
        public UserManager GetUserByEmail(string searchEmail)
        {
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            UserManager user = null;
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();                
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    if (!string.IsNullOrEmpty(searchEmail))
                    {
                        ClientResult<Microsoft.SharePoint.Client.Utilities.PrincipalInfo> persons =
                        Microsoft.SharePoint.Client.Utilities.Utility.ResolvePrincipal(
                            cc,
                            cc.Web,
                            searchEmail,
                            Microsoft.SharePoint.Client.Utilities.PrincipalType.User,
                            Microsoft.SharePoint.Client.Utilities.PrincipalSource.All,
                            null,
                            true
                        );
                        cc.ExecuteQuery();
                        Microsoft.SharePoint.Client.Utilities.PrincipalInfo person = persons.Value;
                        User userFound = cc.Web.EnsureUser(person.LoginName);
                        cc.Load(userFound);
                        cc.ExecuteQuery();
                        user = new UserManager();
                        user.Designation = person.JobTitle;
                        user.UserName = userFound.Title;
                        user.EmailID = userFound.Email;
                        user.SPUserId = userFound.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", searchEmail);                
            }
            return user;
        }
        public WikiPolicyDocuments GetWikiPolicyDocuments()
        {
            WikiPolicyDocuments poldocs = new WikiPolicyDocuments();
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            try
            {
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    List<WikiPolicies> wikiPol = new List<WikiPolicies>();
                    ListItemCollection lstItemsPolicy = GetPolicyDocumentItems(AppConstant.SPList_FAQs, cc, null, string.Empty);
                    if (lstItemsPolicy != null & lstItemsPolicy.Count > 0)
                    {
                        foreach (ListItem item in lstItemsPolicy)
                        {
                            WikiPolicies wiki = new WikiPolicies();
                            wiki.DocumentName = item.DisplayName + "." + item["EncodedAbsUrl"].ToString().Split('/').Last().Split('.').Last().ToString();
                            wiki.PolicyOwner = item["PolicyOwner"].ToString();
                            wiki.DocumentURL = item["EncodedAbsUrl"].ToString();
                            wikiPol.Add(wiki);
                        }
                    }
                    poldocs.ListOfWiki = wikiPol;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", currentUser.EmailID);
            }
            return poldocs;
        }
        private ListItemCollection GetPolicyDocumentItems(string lstGetName, ClientContext context, string webName, string camlQuery)
        {
            ListItemCollection itemsColl = null;
            try
            {
                CamlQuery query = new CamlQuery();

                if (!string.IsNullOrEmpty(webName))
                {
                    Microsoft.SharePoint.Client.Web web = context.Site.OpenWeb(webName);
                    context.Load(web, w => w.Lists);
                    context.ExecuteQuery();
                    List lst = web.Lists.GetByTitle(lstGetName);
                    context.Load(lst);
                    context.ExecuteQuery();
                    if (string.IsNullOrEmpty(camlQuery))
                    {
                        query = CamlQuery.CreateAllItemsQuery();
                    }
                    else
                    {
                        query.ViewXml = camlQuery;
                    }

                    itemsColl = lst.GetItems(query);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query).
                    context.Load(itemsColl, items => items.Include(item => item.Id, item => item.DisplayName, item => item["EncodedAbsUrl"], item => item["PolicyOwner"]));
                    context.ExecuteQuery();
                    return itemsColl;
                }
                else
                {
                    Microsoft.SharePoint.Client.Web web = context.Web;
                    context.Load(web, w => w.Lists);
                    context.ExecuteQuery();
                    List lst = web.Lists.GetByTitle(lstGetName);
                    context.Load(lst);
                    context.ExecuteQuery();
                    if (string.IsNullOrEmpty(camlQuery))
                    {
                        query = CamlQuery.CreateAllItemsQuery();
                    }
                    else
                    {
                        query.ViewXml = camlQuery;
                    }
                    itemsColl = lst.GetItems(query);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                    context.Load(itemsColl, items => items.Include(item => item.Id, item => item.DisplayName, item => item["EncodedAbsUrl"], item => item["PolicyOwner"]));
                    context.ExecuteQuery();

                }
            }
            catch (Exception ex)
            {
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetPolicyDocumentItems", ex.Message, ex.StackTrace, "Academy", user.EmailID);


            }
            return itemsColl;
            
            
        }
        public Stream DownloadDocument(string decryptFileName)
        {
            Stream fileBytes = null;
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
            string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
            //Using this principal in your application using the SharePoint PnP Sites Core library
            var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret);

            cc.Load(cc.Web, w => w.Url, w => w.ServerRelativeUrl);
            cc.ExecuteQuery();
            fileBytes = SharePointUtil.GetFile(decryptFileName, cc);
            return fileBytes;
          
        }
        private ListItemCollection GetFolderItems(string ListName, string folder)
        {
            ListItemCollection items = null;
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                //    using (ClientContext context = new ClientContext(url))
                //  {
                //   context.Credentials = (ICredentials)HttpContext.Current.Session["SPCredential"];
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    Microsoft.SharePoint.Client.Web web = cc.Web;
                    cc.Load(web, w => w.ServerRelativeUrl);
                    List lst = web.Lists.GetByTitle(ListName);
                    cc.ExecuteQuery();
                    cc.Load(lst, l => l.Fields);

                    var query1 = new CamlQuery();

                    query1.FolderServerRelativeUrl = web.ServerRelativeUrl + folder;

                    query1.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                        "<Query>" +
                        "<Where>" +
                                    "<Contains>" +
                                        "<FieldRef Name=\"FileDirRef\" />" +
                                        "<Value Type=\"Text\">" + folder + "</Value>" +
                                     "</Contains>" +
                        "</Where>" +
                        "</Query>" +
                        "</View>";

                    items = lst.GetItems(query1);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                    cc.Load(items);
                    cc.ExecuteQuery();

                }
            }
            catch (Exception ex)
            {
                UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", currentUser.EmailID);
            }
            return items;
            
        }
        public string GetBase64BitLogoImageStream()
        {
            string imageStream = null;
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                ListItemCollection lstlogoItems = GetFolderItems(
                ListConstant.SiteAssets, ListConstant.LogoFolder);

              
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    var serverUrl = new Uri(cc.Url).GetLeftPart(UriPartial.Authority);

                    if (lstlogoItems != null && lstlogoItems.Count > 0)
                    {
                        foreach (ListItem item in lstlogoItems)
                        {
                            Logos logos = new Logos();
                            string strurl = string.Empty;

                            #region Get images from SP Lib
                            strurl = serverUrl + Convert.ToString(item.FieldValues[FieldConstant.FileRef]);
                            Stream stream = (SharePointUtil.GetFile(strurl, cc));
                            #endregion

                            imageStream = Utilities.CreateBase64Image(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", currentUser.EmailID);
                

            }

            return imageStream;
           
           
        }

        private ListItemCollection GetWikiDocumentItems(string lstGetName, ClientContext context, string webName, string camlQuery)
        {
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            ListItemCollection itemsColl = null;
            try
            {
                CamlQuery query = new CamlQuery();

                if (!string.IsNullOrEmpty(webName))
                {
                    // Microsoft.SharePoint.Client.Web web = context.Site.OpenWeb(webName);
                    // context.Load(web, w => w.Lists);
                    List lst = context.Web.Lists.GetByTitle(lstGetName);
                    context.Load(lst);
                    context.ExecuteQuery();
                    if (string.IsNullOrEmpty(camlQuery))
                    {
                        query = CamlQuery.CreateAllItemsQuery();
                    }
                    else
                    {
                        query.ViewXml = camlQuery;
                    }

                    itemsColl = lst.GetItems(query);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query).

                    context.Load(itemsColl, items => items.Include(item => item.Id, item => item.EffectiveBasePermissions, item => item.DisplayName, item => item.Folder, item => item.FileSystemObjectType, item => item.Folder.ParentFolder.Name, item => item["EncodedAbsUrl"], item => item["FileRef"]));
                    context.ExecuteQuery();
                    return itemsColl;
                }
                else
                {
                    //Microsoft.SharePoint.Client.Web web = context.Web;
                    //context.Load(web, w => w.Lists);
                    List lst = context.Web.Lists.GetByTitle(lstGetName);
                    context.Load(lst);
                    context.ExecuteQuery();
                    if (string.IsNullOrEmpty(camlQuery))
                    {
                        query = CamlQuery.CreateAllItemsQuery();
                    }
                    else
                    {
                        query.ViewXml = camlQuery;
                    }
                    itemsColl = lst.GetItems(query);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                    context.Load(itemsColl, items => items.ListItemCollectionPosition);
                    context.Load(itemsColl, items => items.Include(item => item.Id, item => item.EffectiveBasePermissions, item => item.DisplayName, item => item.Folder, item => item.FileSystemObjectType, item => item.Folder.ParentFolder.Name, item => item["EncodedAbsUrl"], item => item["FileRef"]));

                    context.ExecuteQuery();

                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("HCL.Academy.Web", ex.Message, ex.StackTrace, "Academy", currentUser.EmailID);
            }
            return itemsColl;
           
            

            
        }
        private List<WikiDocuments> GetChild(List<WikiDocuments> wikiDoc)
        {
            //Get child items
            List<WikiDocuments> wikiDocchild = new List<WikiDocuments>();
            foreach (WikiDocuments item in wikiDoc)
            {
                var wikichilddoc = from c in wikiDoc where c.DocumentURL.Equals(item.ParentFolderURL) select c;
                foreach (WikiDocuments itemwiki in wikichilddoc.ToList())
                {
                    if (itemwiki.WikiChild == null)
                    {
                        itemwiki.WikiChild = new List<WikiDocuments>();
                    }

                    itemwiki.WikiChild.Add(item);
                }

            }
            var d = from c in wikiDoc where c.ParentFolder.Equals("TrainingDocuments") select c;
            return d.ToList();
            
        }
        public List<WikiDocuments> GetWikiDocumentTree(HttpServerUtilityBase Server)
        {
            List<WikiDocuments> listOfWikiDoc = new List<WikiDocuments>();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            try
            {

                string url = ConfigurationManager.AppSettings["URL"].ToString();
                //  using (ClientContext context = new ClientContext(url))
                // {
                //     context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web);
                    cc.Load(cc.Web.Lists);
                    cc.ExecuteQuery();
                    List sourceList = cc.Web.Lists.GetByTitle(AppConstant.SPList_WikiDocuments);
                    cc.Load(sourceList);
                    cc.ExecuteQuery();
                    cc.Load(sourceList.RootFolder);
                    cc.Load(sourceList.RootFolder.Folders);
                    CamlQuery camlQuery = new CamlQuery();

                    camlQuery.ViewXml = "@<View Scope='RecursiveAll'></View>";


                    ListItemCollection listItems = sourceList.GetItems(camlQuery);
                    // cc.Load(listItems);
                    cc.Load(listItems, items => items.Include(item => item.Id, item => item.EffectiveBasePermissions, item => item.DisplayName, item => item.Folder, item => item.FileSystemObjectType, item => item.Folder.ParentFolder.Name, item => item["EncodedAbsUrl"], item => item["FileRef"]));
                    cc.ExecuteQuery();

                    //ListCollection collList = cc.Web.Lists;
                    //cc.Load(collList);
                    //cc.ExecuteQuery();
                    // string camlQuery = "@<View Scope='RecursiveAll'></View>";
                    //  ListItemCollection lstItemsDoc = GetWikiDocumentItems(AppConstant.SPList_WikiDocuments, cc, null, camlQuery);
                    if (listItems != null & listItems.Count > 0)
                    {
                        List<WikiDocuments> TrainingDocFirst = new List<WikiDocuments>();
                        foreach (ListItem item in listItems)
                        {
                            var perms = item.EffectiveBasePermissions.Has(PermissionKind.OpenItems); //Permission Trimmed
                            if (perms)
                            {
                                WikiDocuments wiki = new WikiDocuments();
                                wiki.ID = item.Id;
                                wiki.ParentFolder = Server.UrlDecode(item["EncodedAbsUrl"].ToString().Split('/')[item["EncodedAbsUrl"].ToString().Split('/').Length - 2]);
                                wiki.DocumentURL = item["EncodedAbsUrl"].ToString();
                                wiki.ParentFolderURL = wiki.DocumentURL.Remove(wiki.DocumentURL.LastIndexOf("/"));
                                if (item.FileSystemObjectType == FileSystemObjectType.Folder)
                                {
                                    wiki.IsFolder = true;
                                    wiki.DocumentName = item.DisplayName;
                                }
                                else
                                {
                                    wiki.DocumentName = item.DisplayName + "." + item["EncodedAbsUrl"].ToString().Split('/').Last().Split('.').Last().ToString();
                                    wiki.IsFolder = false;
                                }

                                TrainingDocFirst.Add(wiki);
                            }
                        }
                        listOfWikiDoc = GetChild(TrainingDocFirst); //ReStructure Objects
                    }

                }
            }
            catch (Exception ex)
            {
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);
            }
            return listOfWikiDoc;
       
        }
        public List<Result> Search(string keyword)
        {
            List<Result> lstResult = null;
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            try
            {
                //using (ClientContext context = new ClientContext(url))
                //{
                //    context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    List<Result> lstResult_ = null;
                    lstResult_ = GetSearch(keyword);
                    if (lstResult_ != null && lstResult_.Count > 0)
                    {
                        lstResult = new List<Result>();
                        lstResult = lstResult_;
                    }
                    else
                    {
                        lstResult = new List<Result>();
                        lstResult = null;
                    }
                }
            }
            catch (Exception ex)
            {
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);                
            }

            return lstResult;
            
        }
        public List<Result> GetSearch(string searchItem)
        {
            List<Result> lstresult = new List<Result>();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            try
            {
                //using (ClientContext context = new ClientContext(url))
                //{
                //    context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    cc.Load(cc.Web, w => w.Url);
                    cc.ExecuteQuery();
                    if (!string.IsNullOrEmpty(searchItem))
                    {

                        var keywordQuery = new KeywordQuery(cc)
                        {
                            QueryText = "(FileExtension:doc OR FileExtension:docx OR FileExtension:xls OR FileExtension:xlsx OR FileExtension:ppt OR FileExtension:pptx OR FileExtension:pdf) (IsDocument:\"True\" OR contentclass:\"STS_ListItem\") (Path:" + cc.Web.Url + ") {" + searchItem + "}"
                        };
                        keywordQuery.TrimDuplicates = true;
                        SearchExecutor searchExecutor = new SearchExecutor(cc);
                        ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
                        cc.ExecuteQuery();

                        if (results != null)
                        {
                            foreach (var resultRow in results.Value[0].ResultRows)
                            {
                                if (resultRow != null && resultRow["Title"] != null)
                                {
                                    Result result = new Result();
                                    result.ResultName = resultRow["Title"].ToString() + "." + resultRow["FileType"].ToString();
                                    result.ResultAuthor = resultRow["Author"].ToString();
                                    result.ResultModified = Convert.ToDateTime(resultRow["LastModifiedTime"].ToString()).ToShortDateString();
                                    result.ResultHighlights = resultRow["HitHighlightedSummary"].ToString().Replace("<ddd/>", "......");
                                    result.ResultSource = resultRow["Path"].ToString();
                                    lstresult.Add(result);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);
                
            }

            return lstresult;
        }
        public WikiPolicyDocuments GetWikiDocumentTree(HttpServerUtilityBase Server, string folder)
        {
            WikiPolicyDocuments poldocs = new WikiPolicyDocuments();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            try
            {
                //using (ClientContext context = new ClientContext(url))
                //{
                //    context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    if (!string.IsNullOrEmpty(folder))
                    {
                        string folderURL = "/sites/Academy/STATE/OnBoarding/" + folder;

                        List<WikiDocuments> TrainingDocFirst = new List<WikiDocuments>();

                        string camlQuery = "@<View Scope='RecursiveAll'></View>";


                        ListItemCollection lstItemsDoc = GetWikiDocumentItems(AppConstant.SPList_WikiDocuments, cc, null, camlQuery);
                        if (lstItemsDoc != null & lstItemsDoc.Count > 0)
                        {

                            foreach (ListItem item in lstItemsDoc)
                            {
                                if (Convert.ToString(item["FileRef"]).Contains(folderURL))
                                {
                                    var perms = item.EffectiveBasePermissions.Has(PermissionKind.OpenItems); //Permission Trimmed
                                    if (perms)
                                    {
                                        WikiDocuments wiki = new WikiDocuments();
                                        wiki.ID = item.Id;
                                        wiki.ParentFolder = Server.UrlDecode(item["EncodedAbsUrl"].ToString().Split('/')[item["EncodedAbsUrl"].ToString().Split('/').Length - 2]);
                                        wiki.DocumentURL = item["EncodedAbsUrl"].ToString();
                                        wiki.ParentFolderURL = wiki.DocumentURL.Remove(wiki.DocumentURL.LastIndexOf("/"));
                                        if (item.FileSystemObjectType == FileSystemObjectType.Folder)
                                        {
                                            wiki.IsFolder = true;
                                            wiki.DocumentName = item.DisplayName;
                                        }
                                        else
                                        {
                                            wiki.DocumentName = item.DisplayName + "." + item["EncodedAbsUrl"].ToString().Split('/').Last().Split('.').Last().ToString();
                                            wiki.IsFolder = false;
                                        }

                                        TrainingDocFirst.Add(wiki);
                                    }
                                }
                            }
                            poldocs.ListOfWikiDoc = GetChild(TrainingDocFirst);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                // LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, currentUser.EmailID.ToString(), AppConstant.ApplicationName, "SharePointDAL, GetWikiDocumentTree", ex.Message, ex.StackTrace));
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);

            }
            return poldocs;
            
            
        }
        public List<WikiPolicies> GetAllWikiPolicies()
        {
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            List<WikiPolicies> wikiPol = new List<WikiPolicies>();
            try
            {
                //using (ClientContext context = new ClientContext(url))
                //{
                //    context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    ListItemCollection lstItemsPolicy = GetPolicyDocumentItems(AppConstant.SPList_FAQs, cc, null, string.Empty);

                    if (lstItemsPolicy != null & lstItemsPolicy.Count > 0)
                    {
                        foreach (ListItem item in lstItemsPolicy)
                        {
                            WikiPolicies wiki = new WikiPolicies();
                            wiki.DocumentName = item.DisplayName + "." + item["EncodedAbsUrl"].ToString().Split('/').Last().Split('.').Last().ToString();
                            wiki.PolicyOwner = Convert.ToString(item["PolicyOwner"]);
                            wiki.DocumentURL = Convert.ToString(item["EncodedAbsUrl"]);
                            wikiPol.Add(wiki);
                        }
                        //  HttpContext.Current.Session[AppConstant.AllWikiPolicyData] = wikiPol;
                    }
                }
            }
            catch (Exception ex)
            {  
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetAllWikiPolicies", ex.Message, ex.StackTrace, "Academy", user.EmailID);
            }
           
            return wikiPol;
            
        }
        private string GetManagerDetails(PersonProperties personProperties)
        {
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            string managerName = string.Empty;
            if (personProperties.ExtendedManagers != null && personProperties.ExtendedManagers.Count() > 0)
            {
                try
                {
                    
                    string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                    string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                    //Using this principal in your application using the SharePoint PnP Sites Core library

                    using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                    {
                        cc.Load(cc.Web, w => w.Url,
                                                    w => w.SiteGroups,
                                                    w => w.CurrentUser,
                                                    w => w.CurrentUser.Groups);
                        cc.ExecuteQuery();
                        PeopleManager myManager = new PeopleManager(cc);
                        PersonProperties myManagerpersonProperties = myManager.GetPropertiesFor(personProperties.ExtendedManagers.Last());
                        cc.Load(myManagerpersonProperties);
                        cc.ExecuteQuery();
                        managerName = myManagerpersonProperties.DisplayName;
                    }
                }
                catch (Exception ex)
                {
                    
                    UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                    LogHelper.AddLog("SPAuthUtility,GetManagerDetails", ex.Message, ex.StackTrace, "Academy", user.EmailID);
                }
            }
            return managerName;

            
        }
        private List<string> GetPeersDetails(PersonProperties personProperties)
        {
            List<string> peersname = new List<string>();

            if (personProperties.Peers != null && personProperties.Peers.Count() > 0)
            {
                UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
                try
                {

                    string url = ConfigurationManager.AppSettings["URL"].ToString();
                    //using (ClientContext context = new ClientContext(url))
                    //{
                    //    context.Credentials = currentUser.SPCredential;
                    string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                    string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                    //Using this principal in your application using the SharePoint PnP Sites Core library

                    using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                    {
                        cc.Load(cc.Web, w => w.Url,
                                                    w => w.SiteGroups,
                                                    w => w.CurrentUser,
                                                    w => w.CurrentUser.Groups);
                        cc.ExecuteQuery();
                        foreach (string peers in personProperties.Peers)//.Take(5))
                        {
                            string peer = peers;
                            PeopleManager peersDetails = new PeopleManager(cc);
                            PersonProperties peerDetailsProperties = peersDetails.GetPropertiesFor(peer);
                            cc.Load(peerDetailsProperties);
                            cc.ExecuteQuery();
                            peersname.Add(peerDetailsProperties.DisplayName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                    LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);
                    
                }
            }
            return peersname;
            
        }
        private List<string> GetReporteeDetails(PersonProperties personProperties)
        {
            List<string> reprteename = new List<string>();
            if (personProperties.DirectReports != null && personProperties.DirectReports.Count() > 0)
            {
                UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                try
                {
                    //using (ClientContext context = new ClientContext(url))
                    //{
                    //    context.Credentials = currentUser.SPCredential;
                    string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                    string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                    //Using this principal in your application using the SharePoint PnP Sites Core library

                    using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                    {
                        cc.Load(cc.Web, w => w.Url,
                                                    w => w.SiteGroups,
                                                    w => w.CurrentUser,
                                                    w => w.CurrentUser.Groups);
                        cc.ExecuteQuery();
                        foreach (string reportee in personProperties.DirectReports)
                        {
                            string reporteeemail = reportee;
                            PeopleManager reporteeDetails = new PeopleManager(cc);
                            PersonProperties reporteeDetailsProperties = reporteeDetails.GetPropertiesFor(reporteeemail);
                            cc.Load(reporteeDetailsProperties);
                            cc.ExecuteQuery();
                            reprteename.Add(reporteeDetailsProperties.DisplayName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                    LogHelper.AddLog("SPAuthUtility,GetWikiDocumentTree", ex.Message, ex.StackTrace, "Academy", user.EmailID);
                    
                }
            }
            return reprteename;

            
        }
        public UserManager GetCurrentUserCompleteUserProfile()
        {
            UserManager user = new UserManager();
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            try
            {
                //using (ClientContext context = new ClientContext(url))
                //{
                //    context.Credentials = currentUser.SPCredential;
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();
                    Web currentweb = cc.Web;
                    cc.Load(currentweb, w => w.SiteUsers);
                    cc.ExecuteQuery();

                    PeopleManager peopleManager = new PeopleManager(cc);
                    ClientResult<PrincipalInfo> principal = Microsoft.SharePoint.Client.Utilities.Utility.ResolvePrincipal(cc, currentweb, currentUser.EmailID, PrincipalType.User, PrincipalSource.All, currentweb.SiteUsers, true);
                    cc.ExecuteQuery();


                    //PersonProperties personProperties = peopleManager.GetMyProperties();
                    PersonProperties personProperties = peopleManager.GetPropertiesFor(principal.Value.LoginName);

                    cc.Load(personProperties, p => p.AccountName,
                                                          p => p.DisplayName,
                                                          p => p.UserProfileProperties,
                                                          p => p.DirectReports,
                                                          p => p.ExtendedManagers,
                                                          p => p.Peers,
                                                          p => p.Title);

                    cc.ExecuteQuery();

                    string managerName = string.Empty;
                    PersonProperties myManagerpersonProperties;
                    List<string> peersname = new List<string>();
                    List<string> reprteename = new List<string>();
                    if (personProperties.ExtendedManagers != null && personProperties.ExtendedManagers.Count() > 0)
                    {
                        PeopleManager myManager = new PeopleManager(cc);
                        myManagerpersonProperties = myManager.GetPropertiesFor(personProperties.ExtendedManagers.Last());
                        cc.Load(myManagerpersonProperties);
                        cc.ExecuteQuery();
                        if (myManagerpersonProperties != null) managerName = myManagerpersonProperties.DisplayName;


                    }
                    if (personProperties.Peers != null && personProperties.Peers.Count() > 0)
                    {
                        foreach (string peers in personProperties.Peers)//.Take(5))
                        {
                            string peer = peers;
                            PeopleManager peersDetails = new PeopleManager(cc);
                            string[] peerParts = peer.Split("|".ToCharArray());
                            peersname.Add(peerParts[peerParts.Length - 1]);
                        }
                    }




                    if (personProperties.DirectReports != null && personProperties.DirectReports.Count() > 0)
                    {
                        foreach (string reportee in personProperties.DirectReports)
                        {
                            string reporteeemail = reportee;
                            string[] reporteeParts = reporteeemail.Split("|".ToCharArray());
                            reprteename.Add(reporteeParts[reporteeParts.Length - 1]);
                        }
                    }
                    user.UserName = personProperties.DisplayName.ToString();
                    user.Designation = personProperties.Title;
                    user.Competency = user.Competency;
                    user.Manager = managerName;
                    user.Manager = GetManagerDetails(personProperties);
                    user.Peers = GetPeersDetails(personProperties);
                    user.Peers = peersname;
                    user.Reportees = GetReporteeDetails(personProperties);
                    user.Reportees = reprteename;
                }
            }
            catch (Exception ex)
            {   
                LogHelper.AddLog("SPAuthUtility,GetCurrentUserCompleteUserProfile", ex.Message, ex.StackTrace, "Academy", user.EmailID);             
            }
            return user;
        }
        private ListItemCollection CacheConfig()
        {
            
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
          
            string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
            string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
            //Using this principal in your application using the SharePoint PnP Sites Core library

            using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
            {
                cc.Load(cc.Web, w => w.Url,
                                            w => w.SiteGroups,
                                            w => w.CurrentUser,
                                            w => w.CurrentUser.Groups);
                cc.ExecuteQuery();
                ListItemCollection lstItemsConfig = SharePointUtil.GetListItems(AppConstant.SPList_AcademyConfig, cc, null, null);
                return lstItemsConfig;
            }


        }
        public List<AcademyEvent> GetEvents()
        {
            string url = ConfigurationManager.AppSettings["URL"].ToString();
            List<AcademyEvent> events = new List<AcademyEvent>();
            UserManager currentUser = (UserManager)HttpContext.Current.Session["CurrentUser"];
            try
            {
                string offsetDays = string.Empty;
                ListItemCollection items = CacheConfig();

                foreach (ListItem item in items)
                {
                    if (item["Title"].Equals("Events"))
                    {
                        offsetDays = item["Value1"].ToString();
                        break;
                    }
                }

                string camlQueryEvents = @"<View>
                                        <Query>
                                            <Where>
                                                <Geq>
                                                    <FieldRef Name='EndDate' />
                                                    <Value Type='DateTime' >
                                                        <Today OffsetDays='" + offsetDays + @"' />
                                                    </Value>
                                                </Geq>
                                            </Where>
                                        </Query>
                                        <OrderBy>
                                            <FieldRef Name='EventDate' Ascending='FALSE'/>
                                        </OrderBy>
                                    </View>";

             
                string spClientId = ConfigurationManager.AppSettings["SPClientId"].ToString();
                string spClientSecret = ConfigurationManager.AppSettings["SPClientSecret"].ToString();
                //Using this principal in your application using the SharePoint PnP Sites Core library

                using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(url, spClientId, spClientSecret))
                {
                    cc.Load(cc.Web, w => w.Url,
                                                w => w.SiteGroups,
                                                w => w.CurrentUser,
                                                w => w.CurrentUser.Groups);
                    cc.ExecuteQuery();

                    ListItemCollection lstItemsEvents = SharePointUtil.GetListItems(
                    AppConstant.SPList_AcademyEvents, cc, null, camlQueryEvents);

                    foreach (var item in lstItemsEvents)
                    {
                        string title = item["Title"].ToString();
                        DateTime eventDate = Convert.ToDateTime(item["EventDate"]);
                        string location = item["Location"].ToString();
                        string description = item["Description"].ToString();

                        AcademyEvent event_ = new AcademyEvent()
                        {
                            title = title,
                            eventDate = eventDate,
                            location = location,
                            description = description
                        };

                        events.Add(event_);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SPAuthUtility,GetEvents", ex.Message, ex.StackTrace, "Academy", user.EmailID);
            }
            return events;
        }
    }
}