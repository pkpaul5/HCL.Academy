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
    /// This service exposes all the methods related to Competency Levels.
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class CompetencyController : ApiController
    {
        /// <summary>
        /// This method returns all the competency levels based on the skill ID.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetCompetenciesBySkillId")]
        public List<Competence> GetCompetenciesBySkillId(RequestBase request, int skillId)
        {
            List<Competence> response = new List<Competence>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetCompetenciesBySkillId(skillId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("CompetencyController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method returns all the competency levels based on skill name. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skillName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetCompetenciesBySkillName")]
        public List<Competence> GetCompetenciesBySkillName(RequestBase request, string skillName)
        {
            List<Competence> response = new List<Competence>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetCompetenciesBySkillName(skillName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("CompetencyController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// GEts the competency level of the user based on user ID passed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserCompetencyLevel")]
        public string GetUserCompetencyLabel(RequestBase request, int userId)
        {
            string response = String.Empty;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetUserCompetencyLabel(userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("CompetencyController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method returns a list of all competency levels.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllCompetenceList")]
        public List<Competence> GetAllCompetenceList(RequestBase request)
        {
            List<Competence> response = new List<Competence>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllCompetenceList();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("CompetencyController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method fetches all the Competency Levels
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllCompetencyLevels")]
        public List<Competence> GetAllCompetencyLevels(RequestBase request)
        {
            List<Competence> response = new List<Competence>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllCompetencyLevels();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("CompetencyController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
    }
}
