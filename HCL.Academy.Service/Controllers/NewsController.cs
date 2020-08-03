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
    /// This service exposes all the methods related to News.
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class NewsController : ApiController
    {
        /// <summary>
        /// This method fetches the news based on the noImagePath.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="noImagePath"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetNews")]
        public List<News> GetNews(RequestBase request, string noImagePath)
        {
            List<News> response = new List<News>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetNews(noImagePath);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("NewsController,GetNews", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// Gets news events from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetNewsFromDB")]
        public List<News> GetNewsFromDB(RequestBase request)
        {
            List<News> response = new List<News>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetNewsFromDB();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("NewsController,GetNews", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetNewsEventByID")]
        public List<News> GetNewsEventByID(RequestBase request,int id)
        {
            List<News> response = new List<News>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetNewsEventByID(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("NewsController,GetNewsEventByID", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Add News to the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddNewsEvents")]
        public bool AddNewsEvents(NewsRequest request)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                result = dal.AddNewsEvent(request.imageURL, request.header, request.body, request.trimmedBody);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("NewsController,AddNewsEvents", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// Delete News event.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteNews")]
        public bool DeleteNews(NewsRequest request)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                result = dal.DeleteNews(request.ID);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("NewsController,DeleteNews", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// Update a news event in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        public bool UpdateNewsEvent(NewsRequest request)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                result = dal.UpdateNewsEvent(request.ID,request.imageURL, request.header, request.body, request.trimmedBody);
            }
            catch (Exception ex)
            {
                //   LogHelper.AddLog("NewsController,UpdateNewsEvent", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
    }
}
