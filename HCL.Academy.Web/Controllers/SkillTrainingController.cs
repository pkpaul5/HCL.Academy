using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class SkillTrainingController : BaseController
    {
        // GET: SkillTraining
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetSkillTrainings", req);
            List<SkillTraining> skillTrainings = await trainingResponse.Content.ReadAsAsync<List<SkillTraining>>();
            return View(skillTrainings);
        }

        // GET: SkillTraining/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SkillTraining/Create
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
            SkillTraining training = new SkillTraining();
            string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();

            if (dataStore == DataStore.SQLServer)
            {
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                training.competences = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                training.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            }
            else
            {
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                training.competences = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                training.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            }

            training.trainings = new List<SkillCompetencyLevelTraining>();
            HttpResponseMessage mastertrainingResponse = await client.PostAsJsonAsync("Training/GetMasterTrainings", req);
            List<SkillTraining> skillTrainings = await mastertrainingResponse.Content.ReadAsAsync<List<SkillTraining>>();
            foreach (var train in skillTrainings)
            {
                SkillCompetencyLevelTraining item = new SkillCompetencyLevelTraining();
                item.Title = train.trainingName;
                item.ID = train.id;
                training.trainings.Add(item);
            }

            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            training.skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
            return View(training);
        }

        // POST: SkillTraining/Create
        [HttpPost]
        public async Task<ActionResult> Create(SkillTraining skillTraining)
        {
            try
            {
                InitializeServiceClient();
                int? assessmentID = null;
                bool result = false;
                SkillwiseAssessmentsRequest assessmentRequest = new SkillwiseAssessmentsRequest();
                assessmentRequest.ClientInfo = req.ClientInfo;
                assessmentRequest.CompetenceId = Convert.ToInt32(skillTraining.selectedCompetence);
                assessmentRequest.SkillId = Convert.ToInt32(skillTraining.selectedSkill);
                HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/GetAssessments", assessmentRequest);
                List<Assessment> assessmentsList = await assessmentResponse.Content.ReadAsAsync<List<Assessment>>();

                if (assessmentsList.Count > 0)
                {
                    assessmentID = assessmentsList.FirstOrDefault().AssessmentId;
                }
                skillTraining.assessmentId = assessmentID;
                string trainingName = skillTraining.selectedTraining;
                skillTraining.trainingName = trainingName;
                List<SkillTraining> skills = new List<SkillTraining>();

                SkillTrainingRequest skillTrainigRequest = new SkillTrainingRequest();
                skillTrainigRequest.ClientInfo = req.ClientInfo;
                skillTrainigRequest.selectedSkill = skillTraining.selectedSkill;
                skillTrainigRequest.selectedCompetence = skillTraining.selectedCompetence;
                skillTrainigRequest.selectedTraining = skillTraining.selectedTraining;
                HttpResponseMessage skillTrainigResponse = await client.PostAsJsonAsync("Training/GetTrainingsByID", skillTrainigRequest);
                skills = await skillTrainigResponse.Content.ReadAsAsync<List<SkillTraining>>();

                if (skills != null)
                {
                    if (skills.Count > 0)
                    {
                        TempData["Message"] = "Combination already exists";
                        TempData.Keep();
                        return RedirectToAction("Index");
                    }
                }
                SkillTrainingRequest skilltrainingRequest = new SkillTrainingRequest();
                skilltrainingRequest.ClientInfo = req.ClientInfo;
                skilltrainingRequest.selectedTraining = skillTraining.selectedTraining;
                skilltrainingRequest.selectedCompetence = skillTraining.selectedCompetence;
                skilltrainingRequest.selectedGEO = skillTraining.selectedGEO;
                skilltrainingRequest.selectedSkill = skillTraining.selectedSkill;
                skilltrainingRequest.isMandatory = skillTraining.isMandatory;
                skilltrainingRequest.isAssessmentRequired = skillTraining.isAssessmentRequired;
                skilltrainingRequest.assessmentId = skillTraining.assessmentId;
                skilltrainingRequest.points = skillTraining.points;
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/AddSkillTraining", skilltrainingRequest);
                result = await trainingResponse.Content.ReadAsAsync<bool>();

                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillTrainingController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View(skillTraining);
            }
        }
        [Authorize]
        [SessionExpire]
        [HttpGet]
        // GET: SkillTraining/Edit/5
        public async Task<ActionResult> Edit(int id, string trainingId, int skillId, int competencyId, int points, int GEOId, bool isAssessmentRequired, bool isMandatory)
        {
            InitializeServiceClient();
            


            SkillTraining skillTraining = new SkillTraining();
            string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();

            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillId?skillId=" + skillId, req);
            skillTraining.competences = await competencyResponse.Content.ReadAsAsync<List<Competence>>();

        
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            skillTraining.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();

            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            skillTraining.skills = await response.Content.ReadAsAsync<List<Skill>>();
            List<SkillTraining> training = new List<SkillTraining>();
            skillTraining.trainings = new List<SkillCompetencyLevelTraining>();

            HttpResponseMessage mastertrainingResponse = await client.PostAsJsonAsync("Training/GetMasterTrainings", req);
            training = await mastertrainingResponse.Content.ReadAsAsync<List<SkillTraining>>();
            foreach (var train in training)
            {
                SkillCompetencyLevelTraining item = new SkillCompetencyLevelTraining();
                item.Title = train.trainingName;
                item.ID = train.id;
                skillTraining.trainings.Add(item);
            }
            skillTraining.id = id;
            //skillTraining.trainingName = trainingName;
            //skillTraining.skill = skill;
            skillTraining.skillId = skillId;
            //skillTraining.selectedSkill = skillId;
            // skillTraining.competency = competency;
            skillTraining.competencyLevelId = competencyId;
            //skillTraining.selectedCompetence = competencyId;
            //skillTraining.selectedGEO = GEO;
            //Session["GEO"] = GEO;
            //Session["Skill"] = skill;
            //Session["Competency"] = competency;
            //Session["Training"] = trainingName;
            skillTraining.trainingId = trainingId;
            skillTraining.points = points;
            // skillTraining.GEO = GEO;
            skillTraining.GEOId = GEOId;
            skillTraining.isAssessmentRequired = isAssessmentRequired;
            skillTraining.isMandatory = isMandatory;
            return View(skillTraining);
        }

        // POST: SkillTraining/Edit/5
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Edit(int id, SkillTraining skillTraining, string category, string skillType)
        {
            try
            {
                InitializeServiceClient();
                bool result = false;
                int? assessmentID = null;                

                SkillwiseAssessmentsRequest assessmentRequest = new SkillwiseAssessmentsRequest();
                assessmentRequest.ClientInfo = req.ClientInfo;
                assessmentRequest.CompetenceId = Convert.ToInt32(skillTraining.competencyLevelId);
                assessmentRequest.SkillId = Convert.ToInt32(skillTraining.skillId);
                HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/GetAssessments", assessmentRequest);
                List<Assessment> assessmentsList = await assessmentResponse.Content.ReadAsAsync<List<Assessment>>();

                if (assessmentsList.Count > 0)
                {
                    assessmentID = assessmentsList.FirstOrDefault().AssessmentId;
                }
                skillTraining.assessmentId = assessmentID;

                HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                skillTraining.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillId?skillId=" + skillTraining.skillId, req);
                skillTraining.competences = await competencyResponse.Content.ReadAsAsync<List<Competence>>();

                //        HttpResponseMessage competenceResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                //      skillTraining.competences = await competenceResponse.Content.ReadAsAsync<List<Competence>>();

                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                skillTraining.skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

                List<SkillTraining> training = new List<SkillTraining>();
                skillTraining.trainings = new List<SkillCompetencyLevelTraining>();
                
                HttpResponseMessage mastertrainingResponse = await client.PostAsJsonAsync("Training/GetMasterTrainings", req);
                training = await mastertrainingResponse.Content.ReadAsAsync<List<SkillTraining>>();
                foreach (var train in training)
                {
                    SkillCompetencyLevelTraining item = new SkillCompetencyLevelTraining();
                    item.Title = train.trainingName;
                    item.ID = train.id;
                    skillTraining.trainings.Add(item);
                }
                List<SkillTraining> skills = new List<SkillTraining>();
                
                SkillTrainingRequest skillTrainigRequest = new SkillTrainingRequest();
                skillTrainigRequest.ClientInfo = req.ClientInfo;
                skillTrainigRequest.selectedSkill = skillTraining.skillId.ToString();
                skillTrainigRequest.selectedCompetence = skillTraining.competencyLevelId.ToString();
                skillTrainigRequest.selectedTraining = skillTraining.trainingId.ToString();
                //HttpResponseMessage skillTrainigResponse = await client.PostAsJsonAsync("Training/GetTrainingsByID", skillTrainigRequest);
                //skills = await skillTrainigResponse.Content.ReadAsAsync<List<SkillTraining>>();

                //if (skills.Count > 0)
                //{
                //    TempData["Message"] = "Combination already exists";
                //    TempData.Keep();
                //    return RedirectToAction("Index");
                //}
                
                SkillTrainingRequest updateSkillTrainigRequest = new SkillTrainingRequest();
                updateSkillTrainigRequest.ClientInfo = req.ClientInfo;
                updateSkillTrainigRequest.selectedSkill = skillTraining.skillId.ToString();
                updateSkillTrainigRequest.selectedCompetence = skillTraining.competencyLevelId.ToString();
                updateSkillTrainigRequest.selectedTraining = skillTraining.trainingId.ToString();
                updateSkillTrainigRequest.id = skillTraining.id;
                updateSkillTrainigRequest.GEO = skillTraining.GEOId.ToString();
                updateSkillTrainigRequest.selectedGEO = skillTraining.GEOId.ToString();
                updateSkillTrainigRequest.isAssessmentRequired = skillTraining.isAssessmentRequired;
                updateSkillTrainigRequest.isMandatory = skillTraining.isMandatory;
                updateSkillTrainigRequest.assessmentId = skillTraining.assessmentId;
                updateSkillTrainigRequest.points = skillTraining.points;
                updateSkillTrainigRequest.selectedTraining = skillTraining.trainingId.ToString();
                HttpResponseMessage skillTrainigUpdateResponse = await client.PostAsJsonAsync("Training/UpdateSkillTraining", updateSkillTrainigRequest);
                result = await skillTrainigUpdateResponse.Content.ReadAsAsync<bool>();

                if (result)
                    return RedirectToAction("Index");
                else
                {
                    TempData["msg"]  = "Update unsuccessful";
                    return View(skillTraining);

                }
            }
            catch
            {
                return View(skillTraining);
            }
        }


        public async Task<ActionResult> Delete(int id, string skill, string competency)
        {
            try
            {
                int? check = null;
                InitializeServiceClient();
                DeleteSkillTrainingRequest deleteSkillTrainingRequest = new DeleteSkillTrainingRequest();
                deleteSkillTrainingRequest.ClientInfo = req.ClientInfo;
                deleteSkillTrainingRequest.id = id;
                deleteSkillTrainingRequest.skill = skill;
                deleteSkillTrainingRequest.competency = competency;
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("Training/DeleteSkillTraining", deleteSkillTrainingRequest);
                bool result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result == false)
                {
                    TempData["Message"] = "Training cannot be deleted due to either referential integrity or some other issue";
                    TempData.Keep();
                }
                else if (result == true)
                {
                    TempData["Message"] = "Training deleted successfully";
                    TempData.Keep();
                }

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillTrainingController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}
