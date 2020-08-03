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
    /// This service performs all the actions for a checklist
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class ChecklistController : ApiController
    {
        /// <summary>
        /// User checklist
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserChecklist")]
        public List<UserCheckList> GetUserChecklist(RequestBase request)
        {
            List<UserCheckList> response = new List<UserCheckList>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetUserChecklist(request.ClientInfo.id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;

        }
        /// <summary>
        /// Gets a list of all checklists.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllChecklist")]
        public List<CheckListItem> GetAllChecklist(RequestBase request)
        {
            List<CheckListItem> response = new List<CheckListItem>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllChecklist();
            }
            catch (Exception ex)
            {
                //       LogHelper.AddLog("ChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method adds a Checklist.
        /// </summary>
        /// <param name="checklist"></param>
        /// <returns></returns> 
        [Authorize]
        [HttpPost]
        [ActionName("AddChecklist")]
        public bool AddChecklist(ChecklistRequest checklist)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(checklist.ClientInfo);
                response = dal.AddChecklist(checklist.name,checklist.selectedGEO,checklist.internalName,checklist.desc,checklist.choice,checklist.selectedRole);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Service", checklist.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// Updates the desired Checklist.
        /// </summary>
        /// <param name="checklist"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateChecklist")]
        public bool UpdateChecklist(ChecklistRequest checklist)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(checklist.ClientInfo);
                response = dal.UpdateChecklist(checklist.id,checklist.name, checklist.selectedGEO, checklist.internalName, checklist.desc, checklist.choice, checklist.selectedRole);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Service", checklist.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method deletes the selected checklist.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteChecklist")]
        public bool DeleteChecklist(RequestBase req, int itemId)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.DeleteChecklist(itemId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
    }
}
