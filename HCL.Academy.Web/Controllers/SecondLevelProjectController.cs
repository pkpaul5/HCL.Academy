using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Controllers;
using System.Threading.Tasks;
using System.Net.Http;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class SecondLevelProjectController : BaseController
    {
        // GET: SecondLevelProject
        public ActionResult Index()
        {
            return View();
        }
        [ActionName("ProjectAdmin")]      
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> ProjectAdmin(int parentProjectId)
        {
            Project project = new Project();
            InitializeServiceClient();
            HttpResponseMessage adminResponse = await client.PostAsJsonAsync("Project/GetProjectAdmin?projectid="+ parentProjectId.ToString(), req);
            List<ProjectAdmin> admins = await adminResponse.Content.ReadAsAsync<List<ProjectAdmin>>();
            HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
            ViewBag.Users = await userResponse.Content.ReadAsAsync<List<Users>>();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/EditProjectByID?projectID=" + parentProjectId, req);
            project = await response.Content.ReadAsAsync<Project>();
            ViewBag.ProjectName = project.projectName;
            ViewBag.ProjectId = parentProjectId;
            project.projectAdmins = admins;
            return View(project);
        }
        [HttpPost]
        [Authorize]
        [SessionExpire]
        public async Task<bool> AddProject(string projectname,int parentProjectId)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProjectDetails?name=" + projectname + "&parentprojectid="+ parentProjectId + "&projectlevel=2", req);
            return true;

        }
        [ActionName("ManageChildProject")]
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> ManageChildProject(int parentProjectId)
        {   
            InitializeServiceClient();
            UserManager user = (UserManager)Session["CurrentUser"];
            Project project = new Project();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/EditProjectByID?projectID=" + parentProjectId, req);
            project = await response.Content.ReadAsAsync<Project>();
            ViewBag.ProjectName = project.projectName;
            ViewBag.ProjectId = parentProjectId;
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetProjectByParent?projectid="+ parentProjectId.ToString(), req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            List<Project> childProjects = new List<Project>();
            if (user.GroupPermission > 2 || user.Admininfo.IsFirstLevelAdmin || user.Admininfo.IsSecondLevelAdmin)
                childProjects = projects;
            else if (user.Admininfo.IsThirdLevelAdmin)
            {
                foreach (Project p in projects)
                {
                    ProjectInfo selectedProject = user.Admininfo.ThirdLevelProjects.Find(x => x.ProjectId == p.id);
                    if (selectedProject != null)
                    {
                        childProjects.Add(p);
                    }
                }
            }
            return View(childProjects);
         
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<bool> AddAdmin(int userid,int projectid)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProjectAdmin?userid=" + userid+"&projectid="+projectid, req);            
            return true;
        }
        
        [HttpPost]
        // GET: SecondLevelProject/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SecondLevelProject/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SecondLevelProject/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
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
                    InitializeServiceClient();
                    UserProjectRequest userProjectInfo = new UserProjectRequest();
                    userProjectInfo.ProjectName = projectName;
                    userProjectInfo.ProjectId = project.id;
                    userProjectInfo.ClientInfo = req.ClientInfo;
                    HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/UpdateProject", userProjectInfo);
                    return RedirectToAction("Index","FirstLevelProject");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                // UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SecondLevelProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        // GET: SecondLevelProject/Delete/5
        public async Task<ActionResult> Delete(int id,int projectid)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/DeleteProjectAdmin?projectid="+projectid+"&userid=" + id, req);
            return RedirectToAction("ProjectAdmin", "SecondLevelProject",new { parentProjectId = projectid });
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
                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = projectID;
                userProjectInfo.ClientInfo = req.ClientInfo;
                HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/RemoveProject", userProjectInfo);
                return RedirectToAction("Index", "FirstLevelProject");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SecondLevelProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return PartialView(new Project());
            }
        }
        // POST: SecondLevelProject/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
