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
    /// This service exposes all the methods related to Skill Competency Levels. 
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class SkillCompetencyLevelController : ApiController
    {
        /// <summary>
        /// This method gets all the Skill Competency Levels
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillCompetencyLevels")]
        public List<SkillCompetencyLevel> GetSkillCompetencyLevels(RequestBase request)
        {
            List<SkillCompetencyLevel> response = new List<SkillCompetencyLevel>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetSkillCompetencyLevels();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController,GetSkillCompetencyLevels", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method adds a skill competency level.
        /// </summary>
        /// <param name="skillCompetencyLevel"></param>
        /// <returns></returns>  
        [Authorize]
        [HttpPost]
        [ActionName("AddSkillCompetencyLevel")]
        public bool AddSkillCompetencyLevel(SkillCompetencyLevelRequest skillCompetencyLevel)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(skillCompetencyLevel.ClientInfo);
                response = dal.AddSkillCompetencyLevel(skillCompetencyLevel.SkillID, skillCompetencyLevel.CompetencyID, skillCompetencyLevel.Description, skillCompetencyLevel.ProfessionalSkills, skillCompetencyLevel.SoftSkills, skillCompetencyLevel.CompetencyLevelOrder, skillCompetencyLevel.TrainingCompletionPoints, skillCompetencyLevel.AssessmentCompletionPoints);
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("SkillCompetencyLevelController,AddSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.Service", skillCompetencyLevel.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;           
        }
        /// <summary>
        /// Updates the desired skill competency level.
        /// </summary>
        /// <param name="skillCompetencyLevel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateSkillCompetencyLevel")]
        public bool UpdateSkillCompetencyLevel(SkillCompetencyLevelRequest skillCompetencyLevel)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(skillCompetencyLevel.ClientInfo);
                response = dal.UpdateSkillCompetencyLevel(skillCompetencyLevel.ItemID,skillCompetencyLevel.SkillID, skillCompetencyLevel.CompetencyID, skillCompetencyLevel.Description, skillCompetencyLevel.ProfessionalSkills, skillCompetencyLevel.SoftSkills, skillCompetencyLevel.CompetencyLevelOrder, skillCompetencyLevel.TrainingCompletionPoints, skillCompetencyLevel.AssessmentCompletionPoints);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController,UpdateSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.Service", skillCompetencyLevel.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method removes the selected skill competency level 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveSkillCompetencyLevel")]
        public bool RemoveSkillCompetencyLevel(RequestBase req,int itemId)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.RemoveSkillCompetencyLevel(itemId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController,RemoveSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

    }
}
