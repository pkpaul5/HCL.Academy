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
    /// This service contains methods related to onboarding
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class OnboardingController : ApiController
    {
        /// <summary>
        /// This method is used for onboarding new user
        /// </summary>
        /// <param name="userInfo">new user information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("OnBoardUser")]
        public bool OnBoardUser(UserOnboardingRequest userInfo)
        {
            bool response = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userInfo.ClientInfo);
                response = dal.OnBoardUser(userInfo.CompetenceId.ToString(), userInfo.SkillId, 0, userInfo.GeoId.ToString(), userInfo.RoleId, userInfo.EmailId, userInfo.Name, userInfo.EmployeeId, userInfo.RoleBasedSkillCount);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,OnBoardUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", userInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides checklist report
        /// </summary>
        /// <param name="req">request object specifying onboarding status and whether the data will be used for Excel download</param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetOnBoardingChecklistReport")]
        public List<UserChecklistReportItem> GetOnBoardingChecklistReport(OnboardingReportRequest req)
        {
            List<UserChecklistReportItem> response = new List<UserChecklistReportItem>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetOnBoardingChecklistReport(req.RoleId, req.ProjectId, req.GEOId,req.option,req.search);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetOnBoardingChecklistReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }


        /// <summary>
        /// This method provides last checklist item report
        /// </summary>
        /// <param name="req">request object specifying onboarding status and whether the data will be used for Excel download</param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetLastChecklistReport")]
        public List<UserChecklistReportItem> GetLastChecklistReport(OnboardingReportRequest req)
        {
            List<UserChecklistReportItem> response = new List<UserChecklistReportItem>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetLastChecklistReport(req.RoleId, req.ProjectId, req.GEOId,req.option,req.search);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetLastChecklistReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;


        }
        /// <summary>
        /// This method provides all the onboarding help text
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetOnboardingHelp")]
        public List<OnboardingHelp> GetOnboardingHelp(RequestBase req)
        {
            List<OnboardingHelp> response = new List<OnboardingHelp>();
            try
            {

                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetOnboardingHelp();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetOnboardingHelp", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides details of skill and role based training assigned to the user
        /// </summary>
        /// <param name="req">service client's information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetBoardingData")]
        public List<OnBoarding> GetBoardingData(RequestBase req)
        {
            List<OnBoarding> response = new List<OnBoarding>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetBoardingData();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetBoardingData", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Get list of users who have been assigned a specific skill as primary skill
        /// </summary>        
        /// <param name="req">skill Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllOnBoardedUser")]
        public List<UserManager> GetAllOnBoardedUser(SkillwiseUsersRequest req)
        {
            List<UserManager> response = new List<UserManager>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetAllOnBoardedUser(req.SkillId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetAllOnBoardedUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides details of all onboarded users including their skill,training,assessment and role details
        /// </summary>
        /// <param name="req">request object specifying onboarding status and whether the data will be used for Excel download</param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetOnBoardingDetailsReport")]
        public List<UserOnBoarding> GetOnBoardingDetailsReport(OnboardingReportRequest req)
        {
            List<UserOnBoarding> response = new List<UserOnBoarding>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetOnBoardingDetailsReport(req.IsExcelDownload, req.RoleId, req.GEOId, req.ProjectId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetOnBoardingDetailsReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// This method provides details of all onboarded users including their skill,training,assessment and role details
        /// </summary>
        /// <param name="req">request object specifying onboarding status and whether the data will be used for Excel download</param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetOnBoardingSkillReport")]
        public List<UserOnBoarding> GetOnBoardingSkillReport(OnboardingReportRequest req)
        {
            List<UserOnBoarding> response = new List<UserOnBoarding>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                response = dal.GetOnBoardingSkillReport(req.RoleId, req.ProjectId, req.GEOId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetOnBoardingSkillReport", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        /// <summary>
        /// This method updates the training completion status for more than one training for an employee
        /// </summary>
        /// <param name="trainingStatus"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateOnBoardingStatus")]
        public List<object> UpdateOnBoardingStatus(TrainingStatusRequest trainingStatus)
        {
            List<object> result = new List<object>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(trainingStatus.ClientInfo);
                result = dal.UpdateOnBoardingStatus(trainingStatus.TrainingStatusItems);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,UpdateOnBoardingStatus", ex.Message, ex.StackTrace, "HCL.Academy.Service", trainingStatus.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method provides details of a user including his skills,trainings,assessments and project assignment
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetOnBoardingDetailsForUser")]
        public UserOnBoarding GetOnBoardingDetailsForUser(RequestBase req)
        {
            UserOnBoarding result = new UserOnBoarding();
            try
            {
                UserManager userManager = new UserManager();
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetOnBoardingDetailsForUser(req.ClientInfo);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetOnBoardingDetailsForUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }


        /// <summary>
        /// This method provides details of checklist,training,assessment details of a user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetBoardingDataFromOnboarding")]
        public List<OnBoarding> GetBoardingDataFromOnboarding(RequestBase req)
        {
            List<OnBoarding> result = new List<OnBoarding>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetBoardingDataFromOnboarding();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,GetBoardingDataFromOnboarding", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method sends onboarding email to onboarded user
        /// </summary>
        /// <param name="req">Request parameter containing user information like email etc.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("OnboardEmail")]
        public bool OnboardEmail(OnboardEmailRequest req)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.OnboardEmail(req.Email, req.UserId, req.UserName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,OnboardEmail", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }

        /// <summary>
        /// This method sends an email to when a skill is added
        /// </summary>
        /// <param name="req">Request parameter containing user information like email etc.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddSkillEmail")]
        public bool AddSkillEmail(OnboardEmailRequest req)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.AddSkillEmail(req.Email, req.UserId, req.UserName,req.SkillId,req.CompetencyLevel);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,AddSkillEmail", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }

        /// <summary>
        /// This method stores Kalibre User Name
        /// </summary>
        /// <param name="emailid"></param>
        /// <param name="kalibreUsrName"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("StoreKalibreUserName")]
        public void StoreKalibreUserName(string emailid, string kalibreUsrName, RequestBase req)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.StoreKalibreUserName(emailid, kalibreUsrName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,StoreKalibreUserName", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }
        /// <summary>
        /// This method stores Kalibre User Name
        /// </summary>
        /// <param name="emailid"></param>        
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("OffBoardUser")]
        public bool OffBoardUser(RequestBase req, string emailid)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.OffBoardUser(emailid);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,OffBoardUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }
        }
        /// <summary>
        /// This method makes a user active or deactive
        /// </summary>
        /// <param name="userid"></param>        
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("ChangeUserActivation")]
        public bool ChangeUserActivation(RequestBase req, int userid)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.ChangeUserActivation(userid);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("OnBoardingController,ChangeUserActivation", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }
        }
    }
}
