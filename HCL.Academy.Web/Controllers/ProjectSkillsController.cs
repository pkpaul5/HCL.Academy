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
    public class ProjectSkillsController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Manage(int projectid)
        {
            InitializeServiceClient();
            try
            {
            
                UserOnBoarding objOnBoarding = new UserOnBoarding();            
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                objOnBoarding.Skills = await response.Content.ReadAsAsync<List<Skill>>();
                client.Dispose();
                return View("ProjectSkills", objOnBoarding);
            }
            catch (Exception ex)
            {
                // LogHelper.AddLog("ProjectSkillsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetProjectSkills(string projectid)
        {
            
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetProjectSkillsByProjectID?projectID=" + projectid, req);
            ProjectDetails objProjectDetails = await response.Content.ReadAsAsync<ProjectDetails>();
            return new JsonResult { Data = objProjectDetails };
        }
        /// <summary>
        /// Update skills related to a Project
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="skillid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> PostProjectSkill(string projectid, string skillid)
        {
            try
            {
                
                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = Convert.ToInt32(projectid);
                userProjectInfo.SkillId = Convert.ToInt32(skillid);
                userProjectInfo.ClientInfo = req.ClientInfo;
                userProjectInfo.SkillId =Convert.ToInt32(skillid);
                HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/PostProjectSkill", userProjectInfo);
                bool status = await ProjResponse.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = status };
            }            
            catch (Exception ex)
            {

                //   LogHelper.AddLog("ProjectSkillsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return new JsonResult { Data = null };
            }
        }        
        /// <summary>
        /// Delete the selected Skill associated to a project.
        /// </summary>
        /// <param name="projectskillid"></param>
        /// <param name="projectid"></param>
        /// <param name="skillid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> DeleteProjectSkill(int projectskillid, string projectid, string skillid)
        {
            InitializeServiceClient();
            try
            {
                
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/DeleteProjectSkill?projectskillid=" + projectskillid + "&projectid=" + projectid + "&skillid=" + skillid, req);
            }
            catch (Exception ex)
            {
                // UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectSkillsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return new JsonResult { Data = false };
            }
            return new JsonResult { Data = true };
        }

        

        
    }
}