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
    /// This service houses all the method related to AssessmentQuestion table master data 
    /// </summary>
    public class AssessmentQuestionController : ApiController
    {
        /// <summary>
        /// This method returns all the AssessmentQuestions records
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllAssessmentQuestion")]
        public List<AssessmentQuestion> GetAllAssessmentQuestion(RequestBase request)
        {
            List<AssessmentQuestion> response = new List<AssessmentQuestion>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllAssessmentQuestion();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method returns an Assessmentquestion by id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAssessmentQuestionById")]
        public AssessmentQuestion GetAssessmentQuestionById(RequestBase req, int id)
        {
            AssessmentQuestion response = new AssessmentQuestion();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetAssessmentQuestionById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method updates an AssessmentQuestion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateAssessmentQuestion")]
        public bool UpdateAssessmentQuestion(AssessmentQuestionRequest request)
        {
            AssessmentQuestion AQ = new AssessmentQuestion();
            AQ.ID = request.ID;
            AQ.SelectedAssessmentId = request.SelectedAssessmentId;
            AQ.Question = request.Question;
            AQ.Option1 = request.Option1;
            AQ.Option2 = request.Option2;
            AQ.Option3 = request.Option3;
            AQ.Option4 = request.Option4;
            AQ.Option5 = request.Option5;
            AQ.Marks = request.Marks;
            AQ.CorrectOption = request.CorrectOption;
            AQ.CorrectOptionSequence = request.CorrectOptionSequence;
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.UpdateAssessmentQuestion(AQ);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method adds a new Assessmentqusetion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddAssessmentQuestion")]
        public bool AddAssessmentQuestion(AssessmentQuestionRequest request)
        {
            AssessmentQuestion AQ = new AssessmentQuestion();
            AQ.SelectedAssessmentId = request.SelectedAssessmentId;
            AQ.Question = request.Question;
            AQ.Option1 = request.Option1;
            AQ.Option2 = request.Option2;
            AQ.Option3 = request.Option3;
            AQ.Option4 = request.Option4;
            AQ.Option5 = request.Option5;
            AQ.Marks = request.Marks;
            AQ.CorrectOption = request.CorrectOption;
            AQ.CorrectOptionSequence = request.CorrectOptionSequence;
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                dal.AddAssessmentQuestion(AQ);
                response = true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        ///  This method deletes an AssessmentQuestion record from database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteAssessmentQuestion")]
        public void DeleteAssessmentQuestion(int id, RequestBase req)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.DeleteAssessmentQuestion(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }
    }
}