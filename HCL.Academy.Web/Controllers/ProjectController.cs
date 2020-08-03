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
    public class ProjectController : BaseController
    {
        // GET: Project
        public async Task<PartialViewResult> Index()
        {
            List<Project> lstProj = new List<Project>();
            InitializeServiceClient();
            try
            {                
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                lstProj = await response.Content.ReadAsAsync<List<Project>>();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            client.Dispose();
            return PartialView(lstProj);
        }
        /// <summary>
        /// fetches a list of all projects
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> AddProject()
        {
            Project addProject = new Project();
            InitializeServiceClient();
            try
            {
                List<Project> lstAllProjects = new List<Project>();
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                lstAllProjects = await response.Content.ReadAsAsync<List<Project>>();
                addProject.projectName = String.Empty;
                Session["Projects"] = lstAllProjects;
                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(addProject);
        }
        /// <summary>
        /// Adds a new Project to the list of projects which is the input to this action.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [HttpPost]

        public async Task<ActionResult> AddProject(string projectName)
        {
            
            if (projectName.Equals(String.Empty))
            {
                ModelState.AddModelError("ProjectName", "Project Name is required");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    InitializeServiceClient();
                    HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProject?projectName=" + projectName, req);
                    return RedirectToAction("AddProject");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View();
            }
        }
        /// <summary>
        /// Edit the information of the selcted project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> EditProjects(int projectID)
        {
            Project project = new Project();
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/EditProjectByID?projectID=" + projectID, req);
                project = await response.Content.ReadAsAsync<Project>();
                Session["EditProject"] = project;
                
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }

            return View(project);
        }
        /// <summary>
        /// Edits/Updates the information of the selected project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> EditProjects(string projectName)
        {
            if (projectName.Equals(String.Empty))
            {
                ModelState.AddModelError("ProjectName", "Project Name is required");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Project project = (Project)Session["EditProject"];
                    //project.projectName = projectName;

                    //IDAL dal = (new DALFactory()).GetInstance();
                    //dal.UpdateProject(project);

                    InitializeServiceClient();
                    UserProjectRequest userProjectInfo = new UserProjectRequest();
                    userProjectInfo.ProjectName = projectName;
                    userProjectInfo.ProjectId = project.id;
                    userProjectInfo.ClientInfo = req.ClientInfo;
                    HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/UpdateProject", userProjectInfo);

                    return RedirectToAction("AddProject", new Project());
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View();
            }
        }
        /// <summary>
        /// Delete selected project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> DeleteProject(int projectID)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //dal.RemoveProject(projectID);

                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = projectID;
                userProjectInfo.ClientInfo = req.ClientInfo;
                HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/RemoveProject", userProjectInfo);

                return RedirectToAction("AddProject", new Project());
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return PartialView(new Project());
            }
        }


        public ActionResult ManageSkills(int projectID)
        {
            return View();
        }

    }
}