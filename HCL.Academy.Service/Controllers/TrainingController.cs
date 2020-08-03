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
    /// This service provides all the training related functionality in HCL Academy
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class TrainingController : ApiController
    {



        /// <summary>
        /// This method provides all the assigned trainings for all users
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserTrainingReport")]
        public List<UserTraining> GetUserTrainingReport(RequestBase request,string projectid, string roleid)
        {
            List<UserTraining> trainings = new List<UserTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                trainings = dal.GetUserTrainingReport(Convert.ToInt32(projectid), Convert.ToInt32(roleid));
            }
            catch (Exception ex)
            {
               // LogHelper.AddLog("TrainingController,GetUserTrainingReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method provides all the assigned trainings for all users based on user role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserRoleBaseTrainingReport")]
        public List<UserTraining> GetUserRoleBaseTrainingReport(RequestBase request, string projectid, string roleid)
        {
            List<UserTraining> trainings = new List<UserTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                trainings = dal.GetUserRoleBaseTrainingReport(Convert.ToInt32(projectid), Convert.ToInt32(roleid));
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                //LogHelper.AddLog("TrainingController,GetUserRoleBaseTrainingReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
            }
            return trainings;
        }


        /// <summary>
        /// This method provides all the trainings for a particular skill and competency level combination in Academy
        /// </summary>
        /// <param name="trainingPreReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainings")]
        public List<Training> GetTrainings(UserTrainingsRequest trainingPreReq)
        {
            List<Training> trainings= new List<Training>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(trainingPreReq.ClientInfo);
                trainings = dal.GetTrainings(trainingPreReq.SkillId, trainingPreReq.CompetenceId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", trainingPreReq.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method provides all the trainings and their details assigned to the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingItems")]
        public List<UserTrainingDetail> GetTrainingItems(RequestBase req)
        {
             TelemetryClient telemetry=new TelemetryClient();         
             telemetry.TrackEvent("In GetTrainingItems");

             List<UserTrainingDetail> ListOfTrainings = new List<UserTrainingDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                ListOfTrainings = dal.GetTrainingItems();
                
                
            }
            catch (Exception ex)
            {
               // LogHelper.AddLog("TrainingController,GetTrainingItems", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                telemetry.TrackException(ex);
            }
            return ListOfTrainings;
        }

        /// <summary>
        /// This method provides all the trainings and their details assigned to the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// /// <param name="skillId"></param>
        /// <param name="trainingId"></param>
        ///  /// <param name="projectId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingAssigments")]
        public List<TrainingAssignment> GetTrainingAssigments(RequestBase req,int skillId,int trainingId,int projectId)
        {
            List<TrainingAssignment> ListOfTrainings = new List<TrainingAssignment>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                ListOfTrainings = dal.GetTrainingAssigments(skillId,trainingId, projectId);
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                //      LogHelper.AddLog("TrainingController,GetTrainingAssigments", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
            }
            return ListOfTrainings;
        }

        /// <summary>
        /// This method provides all the trainings and their details assigned to the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingForUser")]
        public List<UserTraining> GetTrainingForUser(RequestBase req, int UserId)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                userTrainings = dal.GetTrainingForUser(UserId, false);
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                //LogHelper.AddLog("TrainingController,GetTrainingForUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
            }
            return userTrainings;
        }

        /// <summary>
        /// This method all trainings of an user
        /// </summary>
        /// <param name="req"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllUserTrainings")]
        public List<UserTraining> GetAllUserTrainings(RequestBase req, int UserId)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                userTrainings = dal.GetTrainingForUser(UserId, false);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetTrainingForUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return userTrainings;
        }
        /// <summary>
        /// This method provides all the trainings and their details assigned to the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingBySkill")]
        public List<TrainingMaster> GetTrainingBySkill(RequestBase req, int skillId)
        {
            List<TrainingMaster> trainings = new List<TrainingMaster>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetTrainingBySkill(skillId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetTrainingBySkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }
        /// <summary>
        /// This method provides all the trainings and their details assigned to current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <param name="trainingId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserTrainingsByTrainingID")]
        public List<UserTraining> GetUserTrainingsByTrainingID(RequestBase req, int trainingId, int projectId)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                userTrainings = dal.GetUserTrainingsByTrainingID(trainingId, projectId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetUserTrainingsByTrainingID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return userTrainings;
        }

        /// <summary>
        /// This method provides trainings list and users list in Academy
        /// </summary>
        /// <param name="preRquisiteInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingsReport")]
        public TrainingReport GetTrainingsReport(UserTrainingsRequest preRquisiteInfo)
        {
            TrainingReport trainingReport = new TrainingReport();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(preRquisiteInfo.ClientInfo);
                trainingReport = dal.GetTrainingsReport(preRquisiteInfo.SkillId,preRquisiteInfo.CompetenceId, preRquisiteInfo.ProjectId);
                
            }
            catch (Exception ex)
            {
                // LogHelper.AddLog("TrainingController,GetTrainingsReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", preRquisiteInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingReport;
        }

        /// <summary>
        /// This method provides list of all trainings for the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserTrainingsDetails")]
        public List<UserSkillDetail> GetUserTrainingsDetails(RequestBase req)
        {
            List<UserSkillDetail> trainingCourses = new List<UserSkillDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetUserTrainingsDetails("");
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("TrainingController,GetUserTrainingsDetails", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method provides list of all skill and all associated trainings with it  for the current user in Academy
        /// </summary>
        /// <param name="req">Client information</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingJourneyDetails")]
        public List<UserSkillDetail> GetTrainingJourneyDetails(RequestBase req, int userId)
        {
            List<UserSkillDetail> trainingCourses = new List<UserSkillDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetTrainingJourneyDetails(userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetTrainingJourneyDetails", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method assigns training to the user
        /// </summary>
        /// <param name="req">Request parameter with training ,user id and client information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AssignTrainingsToUser")]
        public bool AssignTrainingsToUser(AssignTrainingRequest req)
        {
            bool isTrainingAssigned = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                isTrainingAssigned = dal.AssignTrainingsToUser(req.UserTraining, req.UserId, false);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,AssignTrainingsToUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isTrainingAssigned;
        }

        /// <summary>
        /// This method provides list of all trainings based on role for the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllRoleTrainings")]
        public List<RoleTraining> GetAllRoleTrainings(RequestBase req)
        {
            List<RoleTraining> trainingCourses = new List<RoleTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetAllRoleTrainings();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetAllRoleTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method provides list of all trainings for the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetMasterTrainings")]
        public List<SkillTraining> GetMasterTrainings(RequestBase req)
        {
            List<SkillTraining> trainingCourses = new List<SkillTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetMasterTrainings();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetMasterTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method provides list of all trainings based on skills for the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillTrainings")]
        public List<SkillTraining> GetSkillTrainings(RequestBase req)
        {
            List<SkillTraining> trainingCourses = new List<SkillTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetSkillTrainings();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetSkillTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method check for training of the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddSkillTraining")]
        public bool AddSkillTraining(SkillTrainingRequest req)
        {
            bool result = false;
            try
            {
                SkillTraining skillTraining = new SkillTraining();
                skillTraining.selectedTraining = req.selectedTraining;
                skillTraining.selectedCompetence = req.selectedCompetence;
                skillTraining.selectedGEO = req.selectedGEO;
                skillTraining.selectedSkill = req.selectedSkill;
                skillTraining.isMandatory = req.isMandatory;
                skillTraining.isAssessmentRequired = req.isAssessmentRequired;
                skillTraining.assessmentId = req.assessmentId;
                skillTraining.points = req.points;

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.AddSkillTraining(skillTraining);
               
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,AddSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method provides training by ID of the current user in Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetTrainingsByID")]
        public List<SkillTraining> GetTrainingsByID(SkillTrainingRequest req)
        {
            List<SkillTraining> trainingCourses = new List<SkillTraining>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainingCourses = dal.GetTrainingsByID(Convert.ToInt32(req.selectedSkill), Convert.ToInt32(req.selectedCompetence), Convert.ToInt32(req.selectedTraining));

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetTrainingsByID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainingCourses;
        }

        /// <summary>
        /// This method deletes skillcompentencytrainings of the current user in Academy
        /// </summary>
        /// <param name="deleteSkillTrainingRequest"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteSkillTraining")]
        public bool DeleteSkillTraining(DeleteSkillTrainingRequest deleteSkillTrainingRequest)
        {   
            bool result=false;
        
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(deleteSkillTrainingRequest.ClientInfo);
                result = dal.DeleteSkillTraining(deleteSkillTrainingRequest.id, deleteSkillTrainingRequest.skill, deleteSkillTrainingRequest.competency);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,DeleteSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", deleteSkillTrainingRequest.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method updates training details
        /// </summary>
        /// <param name="req">training update request</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateSkillTraining")]
        public bool UpdateSkillTraining(SkillTrainingRequest req)
        {
            bool result = false;
            try
            {
                SkillTraining skillTraining = new SkillTraining();
                skillTraining.selectedTraining = req.selectedTraining;
                skillTraining.selectedCompetence = req.selectedCompetence;
                skillTraining.selectedGEO = req.selectedGEO;
                skillTraining.selectedSkill = req.selectedSkill;
                skillTraining.isMandatory = req.isMandatory;
                skillTraining.isAssessmentRequired = req.isAssessmentRequired;
                skillTraining.assessmentId = req.assessmentId;
                skillTraining.points = req.points;
                skillTraining.id = req.id;

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.UpdateSkillTraining(skillTraining);

            }
            catch (Exception ex)
            {
                //  LogHelper.AddLog("TrainingController,UpdateSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns all Trainings from Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllTrainings")]
        public List<Training> GetAllTrainings(RequestBase req)
        {
            List<Training> trainings = new List<Training>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetAllTrainings();
            }
            catch (Exception ex)
            {
                //       LogHelper.AddLog("TrainingController,GetAllTrainings", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method returns all Trainings from Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillBasedTrainingsUserView")]
        public List<UserTrainingDetail> GetSkillBasedTrainingsUserView(UserTrainingDetail req)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetSkillBasedTrainingsUserView(req.UserId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetSkillBasedTrainingsUserView", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method returns all Trainings from Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRoleBasedTrainingsUserView")]
        public List<UserTrainingDetail> GetRoleBasedTrainingsUserView(UserTrainingDetail req)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetRoleBasedTrainingsUserView(req.UserId);
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("TrainingController,GetRoleBasedTrainingsUserView", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method updates training details
        /// </summary>
        /// <param name="req">training update request</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("TrainingCompletionRequest")]
        public bool TrainingCompletionRequest(UserTrainingDetail req)
        {
            bool result = false;
            try
            {
                UserTrainingDetail skillTraining = new UserTrainingDetail();
                skillTraining.TrainingId = req.TrainingId;
                skillTraining.UserName = req.UserName;
               // skillTraining.TrainingName = req.TrainingName;
                skillTraining.TrainingFlag = req.TrainingFlag;
                skillTraining.UserId = req.UserId;
                skillTraining.ProjectId = req.ProjectId;
                skillTraining.AdminName = req.AdminName;
                skillTraining.EmailAddress = req.EmailAddress;
                skillTraining.Progress = req.Progress;
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.TrainingCompletionRequest(skillTraining);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,TrainingCompletionRequest", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns all Trainings from Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRoleBasedTrainingsAdminView")]
        public List<UserTrainingDetail> GetRoleBasedTrainingsAdminView(UserTrainingDetail req)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetRoleBasedTrainingsAdminView(req);
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("TrainingController,GetRoleBasedTrainingsAdminView", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method returns all Trainings from Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillBasedTrainingsAdminView")]
        public List<UserTrainingDetail> GetSkillBasedTrainingsAdminView(UserTrainingDetail req)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                trainings = dal.GetSkillBasedTrainingsAdminView(req);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController,GetSkillBasedTrainingsAdminView", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return trainings;
        }

        /// <summary>
        /// This method updates training details
        /// </summary>
        /// <param name="req">training update request</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("TrainingCompletionApproval")]
        public bool TrainingCompletionApproval(UserTrainingDetail req)
        {
            bool result = false;
            try
            {
                UserTrainingDetail skillTraining = new UserTrainingDetail();
                skillTraining.TrainingId = req.TrainingId;
                skillTraining.TrainingFlag = req.TrainingFlag;
                skillTraining.AdminApprovalStatus = req.AdminApprovalStatus;
                skillTraining.UserId = req.UserId;

                skillTraining.UserName = req.UserName;
                skillTraining.TrainingName = req.TrainingName;
                skillTraining.AdminName = req.AdminName;
                skillTraining.EmailAddress = req.EmailAddress;

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.TrainingCompletionApproval(skillTraining);

            }
            catch (Exception ex)
            {
                //  LogHelper.AddLog("TrainingController,TrainingCompletionApproval", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// Approve/Reject all trainings at once
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("ChangeStatusofAllTrainings")]
        public bool ChangeStatusofAllTrainings(TrainingCompletionRequest req)
        {
            bool result = false;
            try
            {
                List<string> skillTraining = new List<string>();
                string trainingFlag = req.AdminApprovalStatus;
                skillTraining = req.trainingDetails;
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.ChangeStatusOfAllTrainings(skillTraining,req.AdminApprovalStatus);

            }
            catch (Exception ex)
            {
                //  LogHelper.AddLog("TrainingController,TrainingCompletionApproval", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }


    }

    
}
