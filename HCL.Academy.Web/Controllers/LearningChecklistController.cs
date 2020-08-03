using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using System.Net.Http;

namespace HCLAcademy.Controllers
{
    public class LearningChecklistController : BaseController
    {
        // GET: LearningChecklist
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            LearningChecklist learning = new LearningChecklist();

            try
            {

                UserManager user = (UserManager)Session["CurrentUser"];
                UserTrainingDetail req = new UserTrainingDetail();
                req.UserId = user.DBUserId;
                HttpResponseMessage SkillBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetSkillBasedTrainingsUserView", req);
                List<UserTrainingDetail> SkillBasedTrainings = await SkillBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.SkillBasedTrainings = SkillBasedTrainings;

                HttpResponseMessage RoleBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetRoleBasedTrainingsUserView", req);
                List<UserTrainingDetail> RoleBasedTrainings = await RoleBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.RoleBasedTrainings = RoleBasedTrainings;

                client.Dispose();
            }
            catch (Exception ex)
            {
                //  UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(learning);
        }


        [Authorize]
        [SessionExpire]
        public async Task<PartialViewResult> GetSkillbasedTrainingUserview()
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                UserManager user = (UserManager)Session["CurrentUser"];
                req.UserId = user.DBUserId;

                HttpResponseMessage SkillBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetSkillBasedTrainingsUserView", req);
                List<UserTrainingDetail> SkillBasedTrainings = await SkillBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.SkillBasedTrainings = SkillBasedTrainings;
                client.Dispose();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("SkillbasedTrainingUserview", learning.SkillBasedTrainings);
            
        }

        [Authorize]
        [SessionExpire]
        public async Task<PartialViewResult> GetRolebasedTrainingUserview()
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                UserManager user = (UserManager)Session["CurrentUser"];
                req.UserId = user.DBUserId;

                HttpResponseMessage RoleBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetRoleBasedTrainingsUserView", req);
                List<UserTrainingDetail> RoleBasedTrainings = await RoleBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.RoleBasedTrainings = RoleBasedTrainings;
                client.Dispose();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("RolebasedTrainingUserview", learning.RoleBasedTrainings);

        }


        [Authorize]
        [SessionExpire]
        public async Task<bool> SkillTrainingCompletionRequest(int trainingid, int Projectid, string UserName,string progress)
        //public async Task<PartialViewResult> SkillTrainingCompletionRequest(int trainingid)
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                UserManager user = (UserManager)Session["CurrentUser"];
                req.TrainingId = trainingid;
            //    req.TrainingName = traingname;
                req.UserId = user.DBUserId;
                req.TrainingFlag = "SkillTraining";
                req.UserName = UserName;
                req.ProjectId = Projectid;
                req.EmailAddress = user.EmailID;
                req.Progress = progress;
                HttpResponseMessage SkillTrainingCompletionResponse = await client.PostAsJsonAsync("Training/TrainingCompletionRequest", req);
                bool result = await SkillTrainingCompletionResponse.Content.ReadAsAsync<bool>();
                //if (result == false)
                //{
                //    TempData["Message"] = "Unable to send Training for approval. ";
                //    TempData.Keep();
                //}
                //else if (result == true)
                //{
                //    TempData["Message"] = "Training sent for approval.";
                //    TempData.Keep();
                //}
                return result;

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }

        }

        [Authorize]
        [SessionExpire]
        public async Task<bool> RoleTrainingCompletionRequest(int trainingid, int Projectid, string UserName,string progress)
        // public async Task<PartialViewResult> RoleTrainingCompletionRequest(int trainingid)
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                UserManager user = (UserManager)Session["CurrentUser"];
                req.TrainingId = trainingid;
              //  req.TrainingName = traingname;
                req.UserId = user.DBUserId;
                req.TrainingFlag = "RoleTraining";
                req.UserName = UserName;
                req.ProjectId = Projectid;
                req.EmailAddress = user.EmailID;
                req.Progress = progress;
                HttpResponseMessage SkillTrainingCompletionResponse = await client.PostAsJsonAsync("Training/TrainingCompletionRequest", req);
                bool result = await SkillTrainingCompletionResponse.Content.ReadAsAsync<bool>();
                //if (result == false)
                //{
                //    TempData["Message"] = "Unable to send Training for approval. ";
                //    TempData.Keep();
                //}
                //else if (result == true)
                //{
                //    TempData["Message"] = "Training sent for approval.";
                //    TempData.Keep();
                //}
                return result;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
          }
            
        }

        // GET: LearningChecklist
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> AdminApproval()
        {
            InitializeServiceClient();
            LearningChecklist learning = new LearningChecklist();

            try
            {
                UserManager user = (UserManager)Session["CurrentUser"];
                UserTrainingDetail req = new UserTrainingDetail();
                if ((user.GroupPermission > 2) || (user.Admininfo.IsFirstLevelAdmin))
                {
                    req.IsAcademyAdmin = true;
                    req.IsProjectAdmin = false;
                }
                    
                else if ((user.Admininfo.IsSecondLevelAdmin) || (user.Admininfo.IsThirdLevelAdmin))
                {
                    req.IsAcademyAdmin = false;
                    req.IsProjectAdmin = true;
                    req.UserId = user.DBUserId;
                }
                    
                

                HttpResponseMessage SkillBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetSkillBasedTrainingsAdminView", req);
                List<UserTrainingDetail> SkillBasedTrainings = await SkillBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.SkillBasedTrainings = SkillBasedTrainings;

                HttpResponseMessage RoleBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetRoleBasedTrainingsAdminView", req);
                List<UserTrainingDetail> RoleBasedTrainings = await RoleBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.RoleBasedTrainings = RoleBasedTrainings;

                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(learning);
        }

        [Authorize]
        [SessionExpire]
        public async Task<bool> SkillTrainingApproval(int trainingid , int UserId, string ApprovalStatus, string AdminName, string TrainingName, string UserName, string UserEmail)
        {
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                req.TrainingId = trainingid;
                req.UserId = UserId;
                req.AdminApprovalStatus = ApprovalStatus;
                req.TrainingFlag = "SkillTraining";

                req.AdminName = AdminName;
                req.TrainingName = TrainingName;
                req.UserName = UserName;
                req.EmailAddress = UserEmail;

                HttpResponseMessage SkillTrainingCompletionResponse = await client.PostAsJsonAsync("Training/TrainingCompletionApproval", req);
                bool result = await SkillTrainingCompletionResponse.Content.ReadAsAsync<bool>();
                return result;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }
            
        }

        [Authorize]
        [SessionExpire]
        public async Task<bool> ChangeStatusofAllTrainings(string ApproveTrainings, string ApprovalStatus)
        {
            try
            {
                InitializeServiceClient();
                TrainingCompletionRequest req = new TrainingCompletionRequest();
                req.AdminApprovalStatus = ApprovalStatus;               
                req.trainingDetails = new List<string>();
                List<string> allTrainings = new List<string>();
                string []trainings=ApproveTrainings.Split('$');
                foreach(var train in trainings)
                {
                    if (!train.Equals(""))
                    {
                        allTrainings.Add(train);
                    }
                }
                allTrainings = allTrainings.Distinct().ToList();
                req.trainingDetails = allTrainings;
                
                HttpResponseMessage SkillTrainingCompletionResponse = await client.PostAsJsonAsync("Training/ChangeStatusofAllTrainings",req);
                bool result = await SkillTrainingCompletionResponse.Content.ReadAsAsync<bool>();
                return result;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }

        }

        [Authorize]
        [SessionExpire]
        public async Task<bool> RoleTrainingApproval(int trainingid, int UserId, string ApprovalStatus, string AdminName, string TrainingName, string UserName, string UserEmail)
        {
            try
            {
                InitializeServiceClient();
                UserTrainingDetail req = new UserTrainingDetail();
                req.TrainingId = trainingid;
                req.UserId = UserId;
                req.AdminApprovalStatus = ApprovalStatus;
                req.TrainingFlag = "RoleTraining";

                req.AdminName = AdminName;
                req.TrainingName = TrainingName;
                req.UserName = UserName;
                req.EmailAddress = UserEmail;

                HttpResponseMessage SkillTrainingCompletionResponse = await client.PostAsJsonAsync("Training/TrainingCompletionApproval", req);
                bool result = await SkillTrainingCompletionResponse.Content.ReadAsAsync<bool>();
                return result;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;
            }
           
        }

        [Authorize]
        [SessionExpire]
        public async Task<PartialViewResult> GetSkillbasedTrainingAdminview()
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();

                UserManager user = (UserManager)Session["CurrentUser"];
                UserTrainingDetail req = new UserTrainingDetail();
                if ((user.GroupPermission > 2) || (user.Admininfo.IsFirstLevelAdmin))
                {
                    req.IsAcademyAdmin = true;
                    req.IsProjectAdmin = false;
                }

                else if ((user.Admininfo.IsSecondLevelAdmin) || (user.Admininfo.IsThirdLevelAdmin))
                {
                    req.IsAcademyAdmin = false;
                    req.IsProjectAdmin = true;
                    req.UserId = user.DBUserId;
                }

                HttpResponseMessage SkillBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetSkillBasedTrainingsAdminView", req);
                List<UserTrainingDetail> SkillBasedTrainings = await SkillBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.SkillBasedTrainings = SkillBasedTrainings;
                client.Dispose();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("SkillbasedTrainingAdminview", learning.SkillBasedTrainings);

        }

        [Authorize]
        [SessionExpire]
        public async Task<PartialViewResult> GetRolebasedTrainingAdminview()
        {
            LearningChecklist learning = new LearningChecklist();
            try
            {
                InitializeServiceClient();

                UserManager user = (UserManager)Session["CurrentUser"];
                UserTrainingDetail req = new UserTrainingDetail();
                if ((user.GroupPermission > 2) || (user.Admininfo.IsFirstLevelAdmin))
                {
                    req.IsAcademyAdmin = true;
                    req.IsProjectAdmin = false;
                }

                else if ((user.Admininfo.IsSecondLevelAdmin) || (user.Admininfo.IsThirdLevelAdmin))
                {
                    req.IsAcademyAdmin = false;
                    req.IsProjectAdmin = true;
                    req.UserId = user.DBUserId;
                }
                HttpResponseMessage RoleBasedTrainingsResponse = await client.PostAsJsonAsync("Training/GetRoleBasedTrainingsAdminView", req);
                List<UserTrainingDetail> RoleBasedTrainings = await RoleBasedTrainingsResponse.Content.ReadAsAsync<List<UserTrainingDetail>>();
                learning.RoleBasedTrainings = RoleBasedTrainings;
                client.Dispose();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("LearningChecklistController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("RolebasedTrainingAdminview", learning.RoleBasedTrainings);

        }

    }

}