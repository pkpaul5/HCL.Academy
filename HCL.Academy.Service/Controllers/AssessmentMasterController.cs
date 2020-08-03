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
    /// This service provides all the Assessment master related methods
    /// </summary>
    public class AssessmentMasterController : ApiController
    {   /// <summary>
        /// This method returns all the Assessments in the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllAssessments")]
        public List<AssessmentMaster> GetAllAssessments(RequestBase request)
        {
            List<AssessmentMaster> response = new List<AssessmentMaster>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllAssessmentForMaster();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method returns AssessmentMaster
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAssessmentById")]
        public AssessmentMaster GetAssessmentById(RequestBase req, int id)
        {
            AssessmentMaster response = new AssessmentMaster();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetAssessmentById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method updates an existing Assessment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateAssessmentMaster")]
        public bool UpdateAssessmentMaster(AssessmentMasterRequest request)
        {
            AssessmentMaster AM = new AssessmentMaster();
            AM.AssessmentId = request.AssessmentId;
            AM.AssessmentName = request.AssessmentName;
            AM.Description = request.Description;
            AM.AssessmentLink = request.AssessmentLink;
            AM.AssessmentTimeInMins = request.AssessmentTimeInMins;
            AM.SelCompetencyId = request.SelCompetencyId;
            AM.SelSkillId = request.SelSkillId;
            AM.SelTrainingId = request.SelTrainingId;
            AM.PassingMarks = request.PassingMarks;
            AM.IsMandatory = request.IsMandatory;
            AM.Points = request.Points;

            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.UpdateAssessmentMaster(AM);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method saves a new Assessment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("SaveAssessmentMaster")]
        public bool SaveAssessmentMaster(AssessmentMasterRequest request)
        {
            AssessmentMaster AM = new AssessmentMaster();
            AM.AssessmentId = request.AssessmentId;
            AM.AssessmentName = request.AssessmentName;
            AM.Description = request.Description;
            AM.AssessmentLink = request.AssessmentLink;
            AM.AssessmentTimeInMins = request.AssessmentTimeInMins;
            AM.SelCompetencyId = request.SelCompetencyId;
            AM.SelSkillId = request.SelSkillId;
            AM.SelTrainingId = request.SelTrainingId;
            AM.PassingMarks = request.PassingMarks;
            AM.IsMandatory = request.IsMandatory;
            AM.Points = request.Points;
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                dal.AddAssessmentMaster(AM);
                response = true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method deletes Assessment from Academy database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteAssessment")]
        public void DeleteAssessment(int id, RequestBase req)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.DeleteAssessment(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }
    }
}
