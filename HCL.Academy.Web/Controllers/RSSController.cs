using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Controllers;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class RSSController : BaseController
    {
        // GET: RSS
        public async Task<ActionResult> Index()
        {
            List<RSSFeedMaster> rSSFeeds = new List<RSSFeedMaster>();
            InitializeServiceClient();
            HttpResponseMessage rssResponse = await client.PostAsJsonAsync("RSS/GetAllRSSFeeds", req);
            rSSFeeds = await rssResponse.Content.ReadAsAsync<List<RSSFeedMaster>>();
            if (rSSFeeds != null)
                Session["RSS"] = rSSFeeds;
            return View(rSSFeeds);
        }
        [HttpGet]
        public ActionResult Create()
        {
            RSSFeedMaster rSS = new RSSFeedMaster();
            return View(rSS);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RSSFeedMaster feed)
        {
            try
            {
                InitializeServiceClient();
                bool result = false;
                RssFeedRequest feedRequest = new RssFeedRequest();
                feedRequest.ClientInfo = req.ClientInfo;
                feedRequest.DescriptionNode = feed.DescriptionNode;
                feedRequest.itemNodePath = feed.itemNodePath;
                feedRequest.RSSFeedUrl = feed.RSSFeedUrl;
                feedRequest.PubDateNode = feed.PubDateNode;
                feedRequest.rssFeedOrder = feed.rssFeedOrder;
                feedRequest.Title = feed.Title;
                feedRequest.TitleNode = feed.TitleNode;
                feedRequest.hrfTitleNodePath = feed.hrfTitleNodePath;
                HttpResponseMessage response = await client.PostAsJsonAsync("RSS/AddRssFeeds", feedRequest);
                result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["Message"] = "Added successfully.";
                    TempData.Keep();
                }
                else
                {
                    TempData["Message"] = "Feed could not be added due to an error.";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            InitializeServiceClient();
            RSSFeedMaster rSS = new RSSFeedMaster();
            HttpResponseMessage response = await client.PostAsJsonAsync("RSS/GetRssFeedById/" + id.ToString(), req);
            rSS = await response.Content.ReadAsAsync<RSSFeedMaster>();
            return View(rSS);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(RSSFeedMaster feed)
        {
            try
            {
                InitializeServiceClient();
                bool result = false;
                RssFeedRequest rssFeed = new RssFeedRequest();
                rssFeed.ClientInfo = req.ClientInfo;
                rssFeed.ID = feed.ID;
                rssFeed.DescriptionNode = feed.DescriptionNode;
                rssFeed.itemNodePath = feed.itemNodePath;
                rssFeed.RSSFeedUrl = feed.RSSFeedUrl;
                rssFeed.PubDateNode = feed.PubDateNode;
                rssFeed.rssFeedOrder = feed.rssFeedOrder;
                rssFeed.Title = feed.Title;
                rssFeed.TitleNode = feed.TitleNode;
                rssFeed.hrfTitleNodePath = feed.hrfTitleNodePath;
                HttpResponseMessage response = await client.PostAsJsonAsync("RSS/UpdateRssFeeds", rssFeed);
                result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["Message"] = "Rss Feed updated successfully.";
                    TempData.Keep();
                }
                else
                {
                    TempData["Message"] = "Rss Feed was not updated due to an error.";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("RSS/DeleteRssFeed?id=" + id, req);
                bool result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result == false)
                {
                    TempData["Message"] = "Feed cannot be deleted due to some issues";
                    TempData.Keep();
                }
                else if (result == true)
                {
                    TempData["Message"] = "Feed deleted successfully";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}