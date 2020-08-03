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
    /// This service exposes all the methods related to assessment
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class AssessmentController : ApiController
    {
        /// <summary>
        /// This method returns assessments based on skill and competency level
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAssessments")]
        public List<Assessment> GetAssessments(SkillwiseAssessmentsRequest request)
        {
            List<Assessment> response = new List<Assessment>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAssessments(request.SkillId, request.CompetenceId);
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
            }
            return response;
        }

        /// <summary>
        /// This method returns assessment detail by id
        /// </summary>
        /// <param name="request">Client Information</param>
        /// <param name="assessmentId">Assessment id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAssessmentDetails")]
        public Assessments GetAssessmentDetails(RequestBase request,int assessmentId)
        {
            Assessments response = new Assessments();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAssessmentDetails(assessmentId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides list of assigned assessments for a user
        /// </summary>
        /// <param name="request">User id and client information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAssessmentForUser")]
        public List<UserAssessment> GetAssessmentForUser(UserwiseAssessmentsRequest request)
        {
            List<UserAssessment> response = new List<UserAssessment>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAssessmentForUser(request.Userid,request.OnlyOnBoardedTraining);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method gives the list of assessments by user id
        /// </summary>
        /// <param name="request">client info</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserAssessmentsByID")]
        public List<UserAssessment> GetUserAssessmentsByID(RequestBase request,int userId)
        {
            List<UserAssessment> response = new List<UserAssessment>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetUserAssessmentsByID(userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides assessment details for currently logged in user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id">Assessment id</param>
        /// <param name="updateAttempts">boolean value</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetCurrentUserAssessments")]
        public List<AcademyJoinersCompletion> GetCurrentUserAssessments(RequestBase request, int id, bool updateAttempts)
        {
            List<AcademyJoinersCompletion> response = new List<AcademyJoinersCompletion>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetCurrentUserAssessments(id, updateAttempts);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// Assessments for logged in user
        /// </summary>
        /// <param name="request">client info</param>
        /// <param name="updateAttempts"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetCurrentUserAssessments")]
        public List<AcademyJoinersCompletion> GetCurrentUserAssessments(RequestBase request,bool updateAttempts)
        {
            List<AcademyJoinersCompletion> response = new List<AcademyJoinersCompletion>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetCurrentUserAssessments(updateAttempts);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides details of assigned assessment based on assessment id
        /// </summary>
        /// <param name="request">Client info</param>
        /// <param name="assessmentId">Assessment id</param>
        /// /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserAssessmentsByAssessmentId")]
        public List<UserAssessment> GetUserAssessmentsByAssessmentId(RequestBase request, int assessmentId,int projectId)
        {
            List<UserAssessment> response = new List<UserAssessment>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetUserAssessmentsByAssessmentId(assessmentId,projectId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method updates assessment result
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AssessmentResult")]
        public bool AssessmentResult(AssessmentResultRequest request)
        {
            bool response = false; ;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.AssessmentResult(request.Result,request.QuestionDetails);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex); 
            }
            return response;
        }
    }
}
