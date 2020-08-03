using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ProjectAvailableResourceController : BaseController
    {
        // GET: ProjectAvailableResource
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create(int projectID,string projectName)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            ProjectResources prjRes = new ProjectResources();
            prjRes.projectId = projectID;
            prjRes.projectName = projectName;
            List<SkillResource> lstSkillResource = new List<SkillResource>();
            prjRes.skillResources = lstSkillResource;
            InitializeServiceClient();
            try
            {
                //List<ProjectSkillResource> projectSkillResources = dal.GetAllProjectSkillResourcesByProjectID(projectID);       //Get the resources and their skills assigned to the selcted Project.
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjectSkillResourcesByProjectID?projectID=" + projectID, req);
                //prjRes = await response.Content.ReadAsAsync<ProjectResources>();
                List<ProjectSkillResource> projectSkillResources = await response.Content.ReadAsAsync<List<ProjectSkillResource>>();
                Hashtable objHashTable = new Hashtable();
                if (projectSkillResources != null && projectSkillResources.Count > 0)
                {
                    foreach (var item in projectSkillResources)
                    {
                        if (!objHashTable.ContainsKey(item.skillId))
                        {
                            objHashTable.Add(item.skillId, item.skillId);
                            SkillResource objSkillResource = new SkillResource();
                            objSkillResource.skillId = item.skillId;
                            objSkillResource.skill = item.skill;
                            lstSkillResource.Add(objSkillResource);
                        }
                    }
                    prjRes.skillResources = lstSkillResource;

                    foreach (SkillResource skr in prjRes.skillResources)
                    {
                        foreach (var item in projectSkillResources)
                        {
                            if (skr.skillId == item.skillId)
                            {
                                switch (item.competencyLevel.ToUpper())
                                {
                                    case "NOVICE":
                                        skr.beginnerCount = item.availableResourceCount;
                                        break;
                                    case "ADVANCED BEGINNER":
                                        skr.advancedBeginnerCount =item.availableResourceCount;
                                        break;

                                    case "COMPETENT":
                                        skr.competentCount = item.availableResourceCount;
                                        break;

                                    case "PROFICIENT":
                                        skr.proficientCount = item.availableResourceCount;
                                        break;

                                    case "EXPERT":
                                        skr.expertCount = item.availableResourceCount;
                                        break;
                                    default:
                                        break;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectAvailableResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(prjRes);
        }

        // POST: ProjectExpectedResource/Create
        /// <summary>
        /// Adds/Updates the resources assigned to a Project
        /// </summary>
        /// <param name="prjRes"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(ProjectResources prjRes)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                if (ModelState.IsValid)
                {
                    //IDAL dal = (new DALFactory()).GetInstance();
                    //dal.AddProjectSkillResources(prjRes);
                    InitializeServiceClient();
                    ProjectResourcesRequest projectResourcesRequest = new ProjectResourcesRequest();
                    projectResourcesRequest.projectId = prjRes.projectId;
                    projectResourcesRequest.projectName = prjRes.projectName;
                    projectResourcesRequest.skillResources = prjRes.skillResources;
                    HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProjectSkillResources", projectResourcesRequest);
                }
                ViewBag.Message = "Data updated";
                return View(prjRes);
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectAvailableResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View(prjRes);
            }
        }
    }
}