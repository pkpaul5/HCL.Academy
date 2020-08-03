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
    /// This service handles all requests related to the Master Trainings.
    /// </summary>
    public class TrainingMasterController : ApiController
    {
        /// <summary>
        /// This method gets all the master trainings
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllMasterTrainings")]
        public List<TrainingMaster> GetAllMasterTrainings(RequestBase request)
        {
            List<TrainingMaster> response = new List<TrainingMaster>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllMasterTrainings();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,GetAllMasterTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method adds a training
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddTraining")]
        public bool AddTraining(TrainingMasterRequest request)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.AddTraining(request.title,request.description,request.skillType,request.trainingCategory,request.trainingLink,request.selectedContent,request.document);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method fetches the details of the selected training.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetMasterTrainingById")]
        public TrainingMaster GetMasterTrainingById(RequestBase req, int id)
        {
            TrainingMaster response = new TrainingMaster();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetMasterTrainingById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method updates the selected training.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateTraining")]
        public bool UpdateTraining(TrainingMasterRequest request)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.UpdateTraining(request.Id,request.title, request.description, request.skillType, request.trainingCategory, request.trainingLink, request.selectedContent, request.document);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// this method deletes the selected training.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteTraining")]
        public bool DeleteTraining(RequestBase request,int id)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                bool? check = false;
                response = dal.DeleteTraining(id,out check);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method fetches the training content from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingContent")]
        public List<TrainingContent> GetTrainingContent(RequestBase request)
        {
            List<TrainingContent> response = new List<TrainingContent>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetTrainingContent();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
    }
}
