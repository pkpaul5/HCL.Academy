using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCLAcademy.Controllers
{
    public class AssessmentController : BaseController
    {
        private static int _assessmentId { get; set; }

        // GET: Assessment
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index(int id)
        {
            _assessmentId = id;
            TempData["assmentID"] = _assessmentId;
            InitializeServiceClient();
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //var objAssessmentDetails = dal.GetAssessmentDetails(_assessmentId);         //Get assessment details based on Assessment ID

                HttpResponseMessage response = await client.PostAsJsonAsync("Assessment/GetAssessmentDetails?assessmentId="+ _assessmentId, req);
                var objAssessmentDetails = await response.Content.ReadAsAsync<Assessments>();

                if (!objAssessmentDetails.assessmentCompletionStatus && !objAssessmentDetails.maxAttemptsExceeded)
                {
                    Session["StartAssessment"] = objAssessmentDetails;
                }
                else
                {
                    return RedirectToAction("Home", "Home");
                }
                
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //  LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return View("StartAssessment");
        }
        /// <summary>
        /// Get assessment questions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult AssessmentQuestions()
        {
            Assessments assessments = (Assessments)Session["StartAssessment"];
            return Json(assessments, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get the result of the assessment using the details of the question 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="QuestionDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AssessmentResult(AssesmentResult result, List<QuestionDetail> QuestionDetails)
        {
            bool response = false;
            try
            {
                InitializeServiceClient();

                AssessmentResultRequest assessmentResult = new AssessmentResultRequest();
                assessmentResult.ClientInfo =this.req.ClientInfo;
                assessmentResult.QuestionDetails = QuestionDetails;
                assessmentResult.Result = result;

                HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/AssessmentResult", assessmentResult);
                response = await assessmentResponse.Content.ReadAsAsync<bool>();

                //IDAL dal = (new DALFactory()).GetInstance();
                //response = dal.AssessmentResult(result, QuestionDetails);

                Response.RemoveOutputCacheItem("/onboard/onboarding");
                Response.RemoveOutputCacheItem("/home/getlearningjourney");
                Response.RemoveOutputCacheItem("/home/getassessments");
                Response.RemoveOutputCacheItem("/training/training");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AssessmentController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                //  Utilities.LogToEventVwr(ex.StackTrace, 0);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}