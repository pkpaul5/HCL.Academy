using HCL.Academy.Model;
using System;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using HCLAcademy.Util;
using System.Configuration;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class NewsController : BaseController
    {
        // GET: News
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]

        public async Task<ActionResult> NewsEvents()
        {
            try
            {
                InitializeServiceClient();
                string newsSource = ConfigurationManager.AppSettings["NewsSource"].ToString();
                if (newsSource.Equals("Feed"))
                {
                    string noImagePath = Server.MapPath(Url.Content("~/Images/noimage.png"));
                    HttpResponseMessage newsResponse = await client.PostAsJsonAsync("News/GetNews?noImagePath=" + noImagePath, req);
                    ViewBag.annclst = await newsResponse.Content.ReadAsAsync<List<News>>();
                }
                else
                {
                    HttpResponseMessage newsResponse = await client.PostAsJsonAsync("News/GetNewsFromDB", req);
                    ViewBag.annclst = await newsResponse.Content.ReadAsAsync<List<News>>();
                }
                List<AcademyEvent> list = new List<AcademyEvent>();
                HttpResponseMessage response = await client.PostAsJsonAsync("Events/GetEvents", req);
                list = await response.Content.ReadAsAsync<List<AcademyEvent>>();
                ViewBag.EventsList = list;
                HttpResponseMessage userResponse = await client.PostAsJsonAsync("RSS/GetRSSFeeds", req);
                ViewBag.RssFeedVB = await userResponse.Content.ReadAsAsync<List<RSSFeed>>();
                //ViewBag.RssFeedVB = dal.GetRSSFeeds();     
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("NewsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View();
        }

        public async Task<ActionResult> Index()
        {
            List<News> newsRequests = new List<News>();
            try
            {
                InitializeServiceClient();
                string newsSource = ConfigurationManager.AppSettings["NewsSource"].ToString();
                
                if (newsSource.Equals("Feed"))
                {
                    string noImagePath = Server.MapPath(Url.Content("~/Images/noimage.png"));
                    HttpResponseMessage newsResponse = await client.PostAsJsonAsync("News/GetNews?noImagePath=" + noImagePath, req);
                    ViewBag.annclst = await newsResponse.Content.ReadAsAsync<List<News>>();
                }
                else
                {                   
                    HttpResponseMessage newsResponse = await client.PostAsJsonAsync("News/GetNewsFromDB", req);
                    newsRequests= await newsResponse.Content.ReadAsAsync<List<News>>();                    
                }
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("NewsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(newsRequests);
        }

        public ActionResult Create()
        {
            News news = new News();            
            return View(news);
        }

        // POST: News/Create
        [HttpPost]
        public async Task<ActionResult> Create(News news)
        {
            InitializeServiceClient();
            try
            {
                NewsRequest request = new NewsRequest();
                request.ClientInfo = req.ClientInfo;
                request.body = news.body.ToString();
                request.imageURL = news.imageURL.ToString();
                request.header = news.header.ToString();               
                HttpResponseMessage response = await client.PostAsJsonAsync("News/AddNewsEvents", request);
                bool result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(news);
                }
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("NewsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View(news);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            InitializeServiceClient();
            NewsRequest request = new NewsRequest();
            request.ClientInfo = req.ClientInfo;            
            HttpResponseMessage response = await client.PostAsJsonAsync("News/GetNewsEventByID?id=" + id, request);
            List<News> news = await response.Content.ReadAsAsync<List<News>>();
            News item = new News();
            item = news[0];
            Session["NewsID"] = id;
            return View(item);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(News news)
        {
            try
            {
                InitializeServiceClient();
                NewsRequest request = new NewsRequest();
                request.ClientInfo = req.ClientInfo;
                request.ID = Convert.ToInt32(Session["NewsID"]);
                request.imageURL = news.imageURL;                
                request.body = news.body;
                request.header = news.header;
                HttpResponseMessage response = await client.PostAsJsonAsync("News/UpdateNewsEvent", request);
                bool result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["EditNews"] = "Yes";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["EditNewsFailed"] = "No";
                    return View(news);
                }                
            }
            catch (Exception ex)
            {
                // UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("NewsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                TempData["EditNewsFailed"] = "No";
                return View(news);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {                
                InitializeServiceClient();
                bool result = false;
                NewsRequest request = new NewsRequest();
                request.ClientInfo = req.ClientInfo;
                request.ID = id;
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("News/DeleteNews", request);
                result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["DeleteNewsSuccess"] = "Yes";                    
                }
                else
                {
                    TempData["DeleteNewsFailed"] = "No";                   
                }
            }
            catch(Exception ex)
            {
                //       LogHelper.AddLog("NewsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}