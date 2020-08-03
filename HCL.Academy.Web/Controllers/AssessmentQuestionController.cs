using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class AssessmentQuestionController : BaseController
    {
        // GET: AssessmentMaster
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage assessmentQuestionResponse = await client.PostAsJsonAsync("AssessmentQuestion/GetAllAssessmentQuestion", req);
            List<AssessmentQuestion> assessmentQuestion = await assessmentQuestionResponse.Content.ReadAsAsync<List<AssessmentQuestion>>();
            return View(assessmentQuestion);
        }

        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
            AssessmentQuestion aq = new AssessmentQuestion();
            List<AssessmentMaster> assessmentlist = new List<AssessmentMaster>();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentMaster/GetAllAssessments", req);
                assessmentlist = await response.Content.ReadAsAsync<List<AssessmentMaster>>();
                aq.Assessments = assessmentlist;              
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(aq);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Create(AssessmentQuestion AQ)
        {
            InitializeServiceClient();
            try
            {                
                if (ModelState.IsValid)
                {
                    AssessmentQuestionRequest AQR = new AssessmentQuestionRequest();
                    AQR.CorrectOption = AQ.CorrectOption;
                    AQR.CorrectOptionSequence = AQ.CorrectOptionSequence;
                    AQR.Marks = AQ.Marks;
                    AQR.Question = AQ.Question;
                    AQR.SelectedAssessmentId = AQ.SelectedAssessmentId;
                    AQR.Option1 = AQ.Option1;
                    AQR.Option2 = AQ.Option2;
                    AQR.Option3 = AQ.Option3;
                    AQR.Option4 = AQ.Option4;
                    AQR.Option5 = AQ.Option5;                    

                    HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentQuestion/AddAssessmentQuestion", AQR);

                    client.Dispose();
                    TempData["CreateSuccess"] = true;
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View();
        }

        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Edit(int Id)
        {
            InitializeServiceClient();
            AssessmentQuestion aq = new AssessmentQuestion();
            List<AssessmentMaster> assessmentlist = new List<AssessmentMaster>();
            List<Competence> competencelist = new List<Competence>();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentQuestion/GetAssessmentQuestionById?id=" + Id, req);
                aq = await response.Content.ReadAsAsync<AssessmentQuestion>();

                response = await client.PostAsJsonAsync("AssessmentMaster/GetAllAssessments", req);

                assessmentlist = await response.Content.ReadAsAsync<List<AssessmentMaster>>();
                aq.Assessments = assessmentlist;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(aq);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Edit(AssessmentQuestion AQ)
        {
            InitializeServiceClient();
            List<AssessmentMaster> assessmentlist = new List<AssessmentMaster>();
            try
            {
                if (ModelState.IsValid)
                {
                AssessmentQuestionRequest AQR = new AssessmentQuestionRequest();
                AQR.ID = AQ.ID;
                AQR.CorrectOption = AQ.CorrectOption;
                AQR.CorrectOptionSequence = AQ.CorrectOptionSequence;
                AQR.Marks = AQ.Marks;
                AQR.Question = AQ.Question;
                AQR.SelectedAssessmentId = AQ.SelectedAssessmentId;
                AQR.Option1 = AQ.Option1;
                AQR.Option2 = AQ.Option2;
                AQR.Option3 = AQ.Option3;
                AQR.Option4 = AQ.Option4;
                AQR.Option5 = AQ.Option5;
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentQuestion/UpdateAssessmentQuestion", AQR);
                bool result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    ViewBag.Success = true;
                }
                else
                    ViewBag.Success = false;
                response = await client.PostAsJsonAsync("AssessmentMaster/GetAllAssessments", req);
                assessmentlist = await response.Content.ReadAsAsync<List<AssessmentMaster>>();
                AQ.Assessments = assessmentlist;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(AQ);
        }
        
        public async Task<ActionResult> Delete(int id)
        {
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentQuestion/DeleteAssessmentQuestion?id=" + id, req);
                client.Dispose();
                TempData["DeleteMessage"] = "Record deleted successfully";
                TempData.Keep("DeleteMessage");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AssessmentQuestionController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}
