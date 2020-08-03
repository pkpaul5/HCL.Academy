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
    /// This service exposes all the methods related to Events
    /// </summary>
    /// 
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class EventsController : ApiController
    {
        /// <summary>
        /// This method returns all the events.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetEvents")]
        public List<AcademyEvent> GetEvents(RequestBase request)
        {
            List<AcademyEvent> response = new List<AcademyEvent>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetEvents();
            }
            catch (Exception ex)
            {
                //           LogHelper.AddLog("EventsController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method adds or updates event data
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("SaveEvent")]
        public bool SaveEvent(EventRequest req)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.SaveEvent(req.eventinfo);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EventsController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method deletes event
        /// </summary>
        /// <param name="req"></param>
        /// /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteEvent")]
        public bool DeleteEvent(RequestBase req,int id)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.DeleteEvent(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EventsController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
    }
}
