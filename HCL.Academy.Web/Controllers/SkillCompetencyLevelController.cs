using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Dynamic;
using System.Threading.Tasks;
using System.Net.Http;
using HCLAcademy.Controllers;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class SkillCompetencyLevelController : BaseController
    {
        [Authorize]
        [SessionExpire]
        //[OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public async Task<ActionResult> Index()

        {
            try
            {
                
                InitializeServiceClient();
                List<SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();                
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("SkillCompetencyLevel/GetSkillCompetencyLevels", req);
                skillCompetencyLevels = await competencyResponse.Content.ReadAsAsync<List<SkillCompetencyLevel>>();
                
                return View(skillCompetencyLevels);
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Create()
        {   
            InitializeServiceClient();
            dynamic mymodel = new ExpandoObject();         
            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            mymodel.Skills = await response.Content.ReadAsAsync<List<Skill>>();
            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
            mymodel.Competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();            
            return View(mymodel);
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetCompetencies()
        {
            List<Competence> competencies = new List<Competence>();

            try
            {   
                InitializeServiceClient();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
                competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();             

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return new JsonResult { Data = competencies };
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetSkills()
        {
            List<Skill> skills = new List<Skill>();

            try
            {   
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                skills = await response.Content.ReadAsAsync<List<Skill>>();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return new JsonResult { Data = skills };
        }

        [Authorize]
        [HttpGet]
        [SessionExpire]
        public async Task<ActionResult> Edit(int id)
        {   
            InitializeServiceClient();
            List <SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();
            HttpResponseMessage skillCompetencyResponse = await client.PostAsJsonAsync("SkillCompetencyLevel/GetSkillCompetencyLevels", req);
            skillCompetencyLevels = await skillCompetencyResponse.Content.ReadAsAsync<List<SkillCompetencyLevel>>();
            SkillCompetencyLevel returnData = skillCompetencyLevels.Where(r => r.ItemID == id).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();         
            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            mymodel.Skills = await response.Content.ReadAsAsync<List<Skill>>();
            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
            mymodel.Competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();            
            mymodel.SkillCompetenceLevels = returnData;
            return View(mymodel);
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> PostSkillCompetencyLevel(int skill, int competence, string description, string professionalskill, string softskill, int comporder, int trainingcompletionpoints, int assessmentcompletionpoints)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                InitializeServiceClient();
                SkillCompetencyLevelRequest competencyLevelRequest = new SkillCompetencyLevelRequest();   
                competencyLevelRequest.ClientInfo = req.ClientInfo;
                competencyLevelRequest.SkillID = skill;
                competencyLevelRequest.CompetencyID = competence;
                competencyLevelRequest.Description = description;
                competencyLevelRequest.ProfessionalSkills = professionalskill;
                competencyLevelRequest.SoftSkills = softskill;
                competencyLevelRequest.CompetencyLevelOrder = comporder;
                competencyLevelRequest.TrainingCompletionPoints = trainingcompletionpoints;
                competencyLevelRequest.AssessmentCompletionPoints = assessmentcompletionpoints;                
                List<SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("SkillCompetencyLevel/GetSkillCompetencyLevels", req);
                skillCompetencyLevels = await competencyResponse.Content.ReadAsAsync<List<SkillCompetencyLevel>>();
                List<SkillCompetencyLevel> returnData = skillCompetencyLevels.Where(r => r.CompetencyID == competence && r.SkillID == skill).ToList();
                if (returnData.Count() > 0)
                {
                    return new JsonResult { Data = false };
                }
                else
                {
                    
                    HttpResponseMessage addCompetency= await client.PostAsJsonAsync("SkillCompetencyLevel/AddSkillCompetencyLevel", competencyLevelRequest);
                    bool status = await addCompetency.Content.ReadAsAsync<bool>();
                    return new JsonResult { Data = status };
                }
                
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> UpdateSkillCompetencyLevel(int itemid, int skill, int competence, string description, string professionalskill, string softskill, int comporder, int trainingcompletionpoints, int assessmentcompletionpoints)
        {
            try
            {
                
                InitializeServiceClient();
                List<SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("SkillCompetencyLevel/GetSkillCompetencyLevels", req);
                skillCompetencyLevels = await competencyResponse.Content.ReadAsAsync<List<SkillCompetencyLevel>>();
                List<SkillCompetencyLevel> returnData = skillCompetencyLevels.Where(r => r.ItemID != itemid && (r.SkillID == skill && r.CompetencyID == competence)).ToList();
                SkillCompetencyLevelRequest competencyLevelRequest = new SkillCompetencyLevelRequest();
                competencyLevelRequest.ClientInfo = req.ClientInfo;
                competencyLevelRequest.ItemID = itemid;
                competencyLevelRequest.SkillID = skill;
                competencyLevelRequest.CompetencyID = competence;
                competencyLevelRequest.Description = description;
                competencyLevelRequest.ProfessionalSkills = professionalskill;
                competencyLevelRequest.SoftSkills = softskill;
                competencyLevelRequest.CompetencyLevelOrder = comporder;
                competencyLevelRequest.TrainingCompletionPoints = trainingcompletionpoints;
                competencyLevelRequest.AssessmentCompletionPoints = assessmentcompletionpoints;
                if (returnData.Count() > 0)
                {
                    return new JsonResult { Data = false };
                }
                else
                {
                
                    HttpResponseMessage editCompetency = await client.PostAsJsonAsync("SkillCompetencyLevel/UpdateSkillCompetencyLevel", competencyLevelRequest);
                    bool status = await editCompetency.Content.ReadAsAsync<bool>();
                    return new JsonResult { Data = status };
                }

            }
            catch (Exception ex)
            {

                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }
        }



        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetSkillCompetencyLevels()
        {
            try
            {   
                InitializeServiceClient();
                List<SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("SkillCompetencyLevel/GetSkillCompetencyLevels", req);
                skillCompetencyLevels = await competencyResponse.Content.ReadAsAsync<List<SkillCompetencyLevel>>();
                return new JsonResult { Data = skillCompetencyLevels };
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }
        }
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> RemoveSkillCompetencyLevel(int itemId)
        {
            try
            {   
                InitializeServiceClient();
                HttpResponseMessage editCompetency = await client.PostAsJsonAsync("SkillCompetencyLevel/RemoveSkillCompetencyLevel?itemId=" + itemId, req);
                bool status = await editCompetency.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = status };
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillCompetencyLevelController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }

        }
    }
}