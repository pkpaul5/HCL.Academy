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
    /// This service provides all the skill related functionality in HCL Academy
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class SkillController : ApiController
    {
        /// <summary>
        /// This method provides all the skills that exist in the Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllSkills")]
        public List<Skill> GetAllSkills(RequestBase req)
        {
            List<Skill> skills = new List<Skill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetAllSkills();
            }            
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetAllSkills", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }

        /// <summary>
        /// This method provides skills assigned to a onboarded user
        /// </summary>
        /// <param name="req">Client info</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillForOnboardedUser")]
        public List<UserSkill> GetSkillForOnboardedUser(RequestBase req, int userId)
        {
            List<UserSkill> skills = new List<UserSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetSkillForOnboardedUser(userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetSkillForOnboardedUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }
        /// <summary>
        /// This method provides skills assigned to a onboarded user
        /// </summary>
        /// <param name="req">Client info</param>
        /// <param name="emailAddress">emailAddress</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserSkillByEmail")]
        public List<UserSkill> GetUserSkillByEmail(RequestBase req, string emailAddress)
        {
            List<UserSkill> skills = new List<UserSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetUserSkillByEmail(emailAddress);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetUserSkillByEmail", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }
        /// <summary>
        /// This method provides skills assigned to a onboarded user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillForUser")]
        public List<UserSkill> GetSkillForUser(RequestBase req)
        {
            List<UserSkill> skills = new List<UserSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetSkillForUser(req.ClientInfo.id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetSkillForUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }

        /// <summary>
        /// This method provides skills assigned to a onboarded user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserSkillsOfCurrentUser")]
        public List<UserSkill> GetUserSkillsOfCurrentUser(RequestBase req)
        {
            List<UserSkill> skills = new List<UserSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetUserSkillsOfCurrentUser();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetUserSkillsOfCurrentUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }

        /// <summary>
        /// This method assigns a skill to a user along with corresponding trainings and assessments
        /// </summary>
        /// <param name="userSkill"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddSkill")]
        public bool AddSkill(SkillAssignmentRequest userSkill)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userSkill.ClientInfo);
                response = dal.AddSkill(userSkill.EmailAddress, userSkill.UserId.ToString(),userSkill.SkillId.ToString(), 
                    userSkill.CompetenceId.ToString(), userSkill.IsMandatory,Convert.ToDateTime( userSkill.LastDayCompletion),userSkill.RoleId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,AddSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", userSkill.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method removes a skill assigned to the user along with corresponding training and assessment
        /// </summary>
        /// <param name="userSkill"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveUserSkill")]
        public bool RemoveUserSkill(SkillAssignmentRequest userSkill)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userSkill.ClientInfo);
                response = dal.RemoveUserSkill(userSkill.Id, userSkill.UserId.ToString(),userSkill.EmailAddress);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,RemoveUserSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", userSkill.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method is used to update user skill details like competency,last date of completion
        /// </summary>
        /// <param name="userSkill"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateUserSkill")]
        public bool UpdateUserSkill(SkillAssignmentRequest userSkill)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userSkill.ClientInfo);
                response = dal.UpdateUserSkill(userSkill.Id,userSkill.CompetenceId.ToString(), userSkill.UserId, userSkill.EmailAddress,Convert.ToDateTime( userSkill.LastDayCompletion),userSkill.CompetencyChanged);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,UpdateUserSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", userSkill.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// Returns all skills
        /// </summary>
        /// <param name="req">Client Info</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllSkillMaster")]
        public List<SkillMaster> GetAllSkillMaster(RequestBase req)
        {
            List<SkillMaster> response = new List<SkillMaster>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetAllSkillMaster();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetAllSkillMaster", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method returns skill by id
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="id">skill id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillById")]
        public SkillMaster GetSkillById(RequestBase req,int id)
        {
            SkillMaster response = new SkillMaster();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetSkillById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetSkillById", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method updates skill details
        /// </summary>
        /// <param name="req">skill and client details</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateSkill")]
        public bool UpdateSkill(SkillRequest req)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.UpdateSkill(req.SkillDetails);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,UpdateSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Adds skill master data
        /// </summary>
        /// <param name="req">client info and skill details</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddSkillDetail")]
        public bool AddSkillDetail(SkillRequest req)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.AddSkill(req.SkillDetails.Title, req.SkillDetails.IsDefault);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,AddSkillDetail", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Remove skill master data
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="id">skill id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveSkill")]
        public int RemoveSkill(RequestBase req,int id)
        {
            //bool response = false;
            int errorCode = 0;
            try
            {

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.RemoveSkill(id,out errorCode);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,RemoveSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return errorCode;
        }

        /// <summary>
        /// Get skills
        /// </summary>
        /// <param name="req">client info</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkills")]
        public List<Skills> GetSkills(RequestBase req)
        {
            //bool response = false;
            List<Skills> skills = new List<Skills>();            
            try
            {

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills =dal.GetSkills();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetSkills", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }

        /// <summary>
        /// Get skills
        /// </summary>
        /// <param name="req">client info</param>
        /// /// <param name="projectId">projectId</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllSkillResourceCount")]
        public List<SkillCompetencyResource> GetAllSkillResourceCount(RequestBase req,string projectId)
        {
            //bool response = false;
            List<SkillCompetencyResource> skills = new List<SkillCompetencyResource>();
            try
            {

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetAllSkillResourceCount(Convert.ToInt32(projectId));
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillController,GetAllSkillResourceCount", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }

    }
}
