using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service exposes all the methods related to RSS
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class RSSController : ApiController
    {
        /// <summary>
        /// This method returns all the RSS Feeds.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRSSFeeds")]
        public List<RSSFeed> GetRSSFeeds(RequestBase request)
        {
            List<RSSFeed> response = new List<RSSFeed>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetRSSFeeds();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController,GetRSSFeeds", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return response;
        }
        /// <summary>
        /// Fetches all RSS Feeds from DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllRSSFeeds")]
        public List<RSSFeedMaster> GetAllRSSFeeds(RequestBase request)
        {
            List<RSSFeedMaster> response = new List<RSSFeedMaster>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllRSSFeeds();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController,GetAllRSSFeeds", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Adds a new RssFeed
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddRssFeeds")]
        public bool AddRssFeeds(RssFeedRequest request)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                RSSFeedMaster rSSFeed = new RSSFeedMaster();
                rSSFeed.ID = request.ID;
                rSSFeed.DescriptionNode= request.DescriptionNode;
                rSSFeed.itemNodePath= request.itemNodePath;
                rSSFeed.RSSFeedUrl =request.RSSFeedUrl;
                rSSFeed.PubDateNode = request.PubDateNode;
                rSSFeed.rssFeedOrder = request.rssFeedOrder;                
                rSSFeed.TitleNode = request.TitleNode;
                rSSFeed.Title = request.Title;
                rSSFeed.hrfTitleNodePath = request.hrfTitleNodePath;
                result = dal.AddRssFeeds(rSSFeed);
            }
            catch(Exception ex)
            {
                //LogHelper.AddLog("RSSController,AddRssFeeds", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// Get details of the selected ID.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRssFeedById")]
        public RSSFeedMaster GetRssFeedById(RequestBase request,int id)
        {
            SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
            RSSFeedMaster rSSFeed = new RSSFeedMaster();
            rSSFeed = dal.GetRssFeedById(id);
            return rSSFeed;
        }

        /// <summary>
        /// This function will update the selected RssFeed
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateRssFeeds")]
        public bool UpdateRssFeeds(RssFeedRequest request)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                RSSFeedMaster rSSFeed = new RSSFeedMaster();
                rSSFeed.ID = request.ID;
                rSSFeed.DescriptionNode = request.DescriptionNode;
                rSSFeed.itemNodePath = request.itemNodePath;
                rSSFeed.RSSFeedUrl = request.RSSFeedUrl;
                rSSFeed.PubDateNode = request.PubDateNode;
                rSSFeed.rssFeedOrder = request.rssFeedOrder;               
                rSSFeed.TitleNode = request.TitleNode;
                rSSFeed.Title = request.Title;
                rSSFeed.hrfTitleNodePath = request.hrfTitleNodePath;
                result = dal.UpdateRssFeeds(rSSFeed);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController,UpdateRssFeeds", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method deletes an Rss Feed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteRssFeed")]
        public bool DeleteRssFeed(RequestBase request,int id)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                result = dal.DeleteRssFeed(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("RSSController,DeleteRssFeed", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
    }
}
