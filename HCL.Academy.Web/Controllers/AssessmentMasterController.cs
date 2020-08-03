using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HCL.Academy.Model;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class AssessmentMasterController : BaseController
    {
        // GET: AssessmentMaster
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("AssessmentMaster/GetAllAssessments", req);
            List<AssessmentMaster> assessmentMaster = await assessmentResponse.Content.ReadAsAsync<List<AssessmentMaster>>();
            return View(assessmentMaster);
        }
        
        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
            AssessmentMaster am = new AssessmentMaster();
            List<Skill> skilllist = new List<Skill>();
            List<Competence> competencelist = new List<Competence>();
            try
            {
                HttpResponseMessage  response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                skilllist = await response.Content.ReadAsAsync<List<Skill>>();

                am.Skills = skilllist;

                response = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels",  req);
                competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                am.Competencies = competencelist;

                response = await client.PostAsJsonAsync("Training/GetAllTrainings",req);
                List<Training> traininglist = await response.Content.ReadAsAsync<List<Training>>();
                am.Trainings = traininglist;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(am);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Create(AssessmentMaster AM)
        {
            InitializeServiceClient();
            List<Skill> skilllist = new List<Skill>();
            List<Competence> competencelist = new List<Competence>();
            string SelSkill = String.Empty;
            try
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    AssessmentMasterRequest AMR = new AssessmentMasterRequest();
                    
                    AMR.AssessmentLink = AM.AssessmentLink;
                    AMR.AssessmentName = AM.AssessmentName;
                    AMR.AssessmentTimeInMins = AM.AssessmentTimeInMins;
                    AMR.Description = AM.Description;
                    AMR.IsMandatory = AM.IsMandatory;
                    AMR.PassingMarks = AM.PassingMarks;
                    AMR.Points = AM.Points;
                    AMR.SelCompetencyId = AM.SelCompetencyId;
                    AMR.SelSkillId = AM.SelSkillId;
                    AMR.SelTrainingId = AM.SelTrainingId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentMaster/SaveAssessmentMaster", AMR);

                    client.Dispose();
                    TempData["CreateSuccess"] = true;
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
                else
                {
                    AssessmentMaster am = new AssessmentMaster();
                    try
                    {
                        HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                        skilllist = await response.Content.ReadAsAsync<List<Skill>>();
                        am.Skills = skilllist;

                        response = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
                        competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                        am.Competencies = competencelist;

                        response = await client.PostAsJsonAsync("Training/GetAllTrainings", req);
                        List<Training> traininglist = await response.Content.ReadAsAsync<List<Training>>();
                        am.Trainings = traininglist;
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                        TelemetryClient telemetry = new TelemetryClient();
                        //telemetry.TrackException(ex); TelemetryClient telemetry = new TelemetryClient();
                        telemetry.TrackException(ex);
                    }
                    return View(am);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View();
        }

        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Edit(int AssessmentId,int SelSkillId,string SelectedSkill,int SelCompetencyId,int? SelTrainingId)
        {
            InitializeServiceClient();
            AssessmentMaster am = new AssessmentMaster();
            am.SelSkillId = SelSkillId;
            am.SelTrainingId = SelTrainingId;
            am.SelTrainingId = SelTrainingId;
            List<Skill> skilllist = new List<Skill>();
            List<Competence> competencelist = new List<Competence>();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentMaster/GetAssessmentById?id=" + AssessmentId, req);
                am = await response.Content.ReadAsAsync<AssessmentMaster>();
                
                response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                skilllist = await response.Content.ReadAsAsync<List<Skill>>();

                am.Skills = skilllist;

                response = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillName?skillName=" + SelectedSkill, req);
                competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                am.Competencies = competencelist;

                UserTrainingsRequest utr = new UserTrainingsRequest();
                utr.SkillId = SelSkillId;                
                utr.CompetenceId = SelCompetencyId;
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("Training/GetTrainings", utr);
                List<Training> traininglist = await responseMessage.Content.ReadAsAsync<List<Training>>();
                am.Trainings = traininglist;

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(am);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Edit(AssessmentMaster AM)
        {
            InitializeServiceClient();
            List<Skill> skilllist = new List<Skill>();
            List<Competence> competencelist = new List<Competence>();
            string SelSkill = String.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    AssessmentMasterRequest AMR = new AssessmentMasterRequest();
                    AMR.AssessmentId = AM.AssessmentId;
                    AMR.AssessmentLink = AM.AssessmentLink;
                    AMR.AssessmentName = AM.AssessmentName;
                    AMR.AssessmentTimeInMins = AM.AssessmentTimeInMins;
                    AMR.Description = AM.Description;
                    AMR.IsMandatory = AM.IsMandatory;
                    AMR.PassingMarks = AM.PassingMarks;
                    AMR.Points = AM.Points;
                    //AMR.SelectedCompetency = AM.SelectedCompetency;
                    AMR.SelCompetencyId = AM.SelCompetencyId;
                    //AMR.SelectedSkill = AM.SelectedSkill;
                    AMR.SelSkillId = AM.SelSkillId;
                    //AMR.SelectedTraining = AM.SelectedTraining;
                    AMR.SelTrainingId = AM.SelTrainingId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentMaster/UpdateAssessmentMaster", AMR);
                    bool result = await response.Content.ReadAsAsync<bool>();
                    if (result)
                    {
                        ViewBag.Success = true;
                    }
                    else
                        ViewBag.Success = false;
                    response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                    skilllist = await response.Content.ReadAsAsync<List<Skill>>();
                    AM.Skills = skilllist;
                    SelSkill = skilllist.Where(x => x.SkillId == AM.SelSkillId).FirstOrDefault().SkillName;
                    response = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillName?skillName=" + SelSkill, req);
                    competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                    AM.Competencies = competencelist;

                    UserTrainingsRequest utr = new UserTrainingsRequest();
                    //utr.SkillId = skilllist.Where(x=>x.SkillName == SelectedSkill).FirstOrDefault().SkillId;
                    utr.SkillId = AM.SelSkillId;
                    //utr.CompetenceId = competencelist.Where(x=>x.CompetenceName == SelectedCompetency).FirstOrDefault().CompetenceId;
                    utr.CompetenceId = AM.SelCompetencyId;
                    HttpResponseMessage responseMessage = await client.PostAsJsonAsync("Training/GetTrainings", utr);
                    List<Training> traininglist = await responseMessage.Content.ReadAsAsync<List<Training>>();
                    AM.Trainings = traininglist;
                }
                else
                {
                    AssessmentMaster am = new AssessmentMaster();
                    try
                    {
                        HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                        skilllist = await response.Content.ReadAsAsync<List<Skill>>();
                        am.Skills = skilllist;

                        response = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
                        competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                        am.Competencies = competencelist;

                        response = await client.PostAsJsonAsync("Training/GetAllTrainings", req);
                        List<Training> traininglist = await response.Content.ReadAsAsync<List<Training>>();
                        am.Trainings = traininglist;
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                        TelemetryClient telemetry = new TelemetryClient();
                        telemetry.TrackException(ex);
                    }
                    return View(am);

                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(AM);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> GetCompetenciesBySkillId(int SkillId)
        {
            InitializeServiceClient();
            List<Competence> competencelist = new List<Competence>();
            List<SelectListItem> competencies = new List<SelectListItem>();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillId?skillId=" + SkillId, req);
                competencelist = await response.Content.ReadAsAsync<List<Competence>>();
                competencelist.ForEach(x =>
                {
                    competencies.Add(new SelectListItem { Text = x.CompetenceName, Value = x.CompetenceId.ToString() });
                }
                );
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return Json(competencies, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> GetTrainingBySkillCompetencyId(int SkillId,int CompetencyId)
        {
            InitializeServiceClient();
            List<SelectListItem> trainings = new List<SelectListItem>();
            try
            {
                UserTrainingsRequest utr = new UserTrainingsRequest();
                utr.SkillId = SkillId;
                utr.CompetenceId = CompetencyId;
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("Training/GetTrainings", utr);
                List<Training> traininglist = await responseMessage.Content.ReadAsAsync<List<Training>>();
                traininglist.ForEach(x =>
                {
                    trainings.Add(new SelectListItem { Text = x.TrainingName, Value = x.TrainingId.ToString() });
                }
                );
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return Json(trainings, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Delete(int id)
        {
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AssessmentMaster/DeleteAssessment?id=" + id, req);
                client.Dispose();
                TempData["DeleteMessage"] = "Record deleted successfully";
                TempData.Keep("DeleteMessage");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AssessmentMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }

    }
}
