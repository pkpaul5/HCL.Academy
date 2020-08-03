using HCL.Academy.Model;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using System.Threading.Tasks;
namespace HCLAcademy.Controllers
{
    //[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    //[OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = false)]
    public class BaseController : Controller
    {
        public UserManager user { get; set; }
        public HttpClient client { get; set; }
        public RequestBase req { get; set; }
        /// <summary>
        /// Get the Menu/Navigation bar of the website
        /// </summary>
        //public void GetMenu()
        //{
        //    IDAL dal = (new DALFactory()).GetInstance();
        //    List<SiteMenu> siteMenu = dal.GetMenu();
        //    System.Web.HttpContext.Current.Session["UserSiteMenu"] = siteMenu;
        //}

        public async Task<bool> InitializeServiceClient()
        {
            user = (UserManager)Session["CurrentUser"];
            client = new HttpClient();
            string serviceBaseURL = ConfigurationManager.AppSettings["AcademyServiceEndPoint"].ToString();
            client.BaseAddress = new Uri(serviceBaseURL);
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req = new RequestBase();
            req.ClientInfo = new ServiceConsumerInfo();
            if (user != null)
            {
                string token = Session["JWT"] as string;
                if (token == null)
                {
                    token = await GetToken(user.EmailID);
                    Session["JWT"] = token;
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                req.ClientInfo.emailId = user.EmailID;
                req.ClientInfo.id = user.DBUserId;
                req.ClientInfo.name = user.UserName;
                req.ClientInfo.spCredential = user.SPCredential;
                req.ClientInfo.spUserId = user.SPUserId;
                req.ClientInfo.Groups = user.Groups;
                req.ClientInfo.GroupPermission = user.GroupPermission;
            }
            return true;
        }
        public async Task<bool> InitializeServiceClient(string emailid)
        {
            client = new HttpClient();
            string serviceBaseURL = ConfigurationManager.AppSettings["AcademyServiceEndPoint"].ToString();
            client.BaseAddress = new Uri(serviceBaseURL);
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req = new RequestBase();
            req.ClientInfo = new ServiceConsumerInfo();
            req.ClientInfo.emailId = emailid;
            string token = Session["JWT"] as string;
            if (token == null)
            {
                token = await GetToken(emailid);
                Session["JWT"] = token;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         

            return true;
        }
        public async Task<string> GetToken(string emailid)
        {
            string token = "";
            HttpClient tokenclient = new HttpClient();
            string serviceBaseURL = ConfigurationManager.AppSettings["AcademyServiceEndPoint"].ToString();
            tokenclient.BaseAddress = new Uri(serviceBaseURL);
            tokenclient.DefaultRequestHeaders.Accept.Clear();
            tokenclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await tokenclient.GetAsync("Token/Get?emailid=" + emailid);
            token = await response.Content.ReadAsAsync<string>();
            return token;
        }
    }
}