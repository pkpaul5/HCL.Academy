using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Utility;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using System.Configuration;
using System.IO;
using HCLAcademy.CustomFilters;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    [CustomAuthorizationFilter]
    public class HomeController : BaseController
    {
        // GET: HomeNew
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Home()
        {
            await InitializeServiceClient();
            if (Session["UserSiteMenu"] == null)
            {
                HttpResponseMessage userRoleResponse = await client.PostAsJsonAsync("User/GetUserRole", req);
                List<int> userRoles = await userRoleResponse.Content.ReadAsAsync<List<int>>();
                System.Web.HttpContext.Current.Session["UserRole"] = userRoles;
                HttpResponseMessage menuResponse = await client.PostAsJsonAsync("Menu/GetMenu?roleid=" + userRoles[0], req);
                List<SiteMenu> siteMenu = await menuResponse.Content.ReadAsAsync<List<SiteMenu>>();
                Session["UserSiteMenu"] = siteMenu;
            }
            if (Session["LogoBase64Image"] == null)            
                Session["LogoBase64Image"] = ConfigurationManager.AppSettings["ClientLogo"].ToString();
            HomeViewModel vm = new HomeViewModel();
            UserTrainingDetail roletrainingreq = new UserTrainingDetail();
            roletrainingreq.ClientInfo = req.ClientInfo;
            UserManager user = (UserManager)Session["CurrentUser"];
            roletrainingreq.UserId = user.DBUserId;
            List<UserTrainingDetail> ListOfTrainings = new List<UserTrainingDetail>();
            
            try
            {
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingItems", req);
                ListOfTrainings = await trainingResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                List<UserTrainingDetail> skilltrainings =ListOfTrainings.Where(x => x.TrainingType.ToString().ToLower() == TrainingType.SkillTraining.ToString().ToLower()).ToList();
                vm.skillTrainings = skilltrainings;

                HttpResponseMessage roletrainingResponse = await client.PostAsJsonAsync("Training/GetRoleBasedTrainingsUserView", roletrainingreq);
                vm.roleTrainings= await roletrainingResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();

                HttpResponseMessage response = await client.PostAsJsonAsync("Checklist/GetUserChecklist", req);
               vm.checklist = await response.Content.ReadAsAsync<List<UserCheckList>>();

                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetUserSkillsOfCurrentUser", req);
                vm.skills = await skillResponse.Content.ReadAsAsync<List<UserSkill>>();

                HttpResponseMessage userResponse = await client.PostAsJsonAsync("Assessment/GetCurrentUserAssessments?updateAttempts=false", req);
                vm.assessments = await userResponse.Content.ReadAsAsync<List<AcademyJoinersCompletion>>();

                List<AcademyEvent> list = new List<AcademyEvent>();
                HttpResponseMessage responseevents = await client.PostAsJsonAsync("Events/GetEvents", req);
                list = await responseevents.Content.ReadAsAsync<List<AcademyEvent>>();
                vm.events = list;
                string newsSource = ConfigurationManager.AppSettings["NewsSource"].ToString();
                if (newsSource.Equals("Feed"))
                {
                    List<RSSFeed> postRSList = new List<RSSFeed>();
                    HttpResponseMessage rssResponse = await client.PostAsJsonAsync("RSS/GetRSSFeeds", req);
                    postRSList = await rssResponse.Content.ReadAsAsync<List<RSSFeed>>();
                    vm.rssFeed = postRSList;
                }
                else
                {
                    List<News> news = new List<News>();
                    HttpResponseMessage newsResponse = await client.PostAsJsonAsync("News/GetNewsFromDB", req);
                    news = await newsResponse.Content.ReadAsAsync<List<News>>();
                    vm.news = news;
                }                
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("HomeController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(vm);
        }
        [Authorize]
        public FileResult DownloadFile(string filePath)
        {
            string decryptFileName = EncryptionHelper.Decrypt(filePath);
            //SharePointDAL spDal = new SharePointDAL();
            SPAuthUtility spUtil = new SPAuthUtility();
            Stream fileBytes = spUtil.DownloadDocument(decryptFileName);
            try
            {
                string fileName = decryptFileName.Substring(decryptFileName.LastIndexOf('/') + 1);
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("HomeController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return null;
            }

        }
        // GET: HomeNew/Details/5
        public ActionResult Details(int id)
        {
           
            return View();
        }

        // GET: HomeNew/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeNew/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeNew/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeNew/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeNew/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeNew/Delete/5
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
    }
}
