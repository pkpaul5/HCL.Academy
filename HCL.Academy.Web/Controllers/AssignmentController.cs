using HCL.Academy.Model;
using System;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class AssignmentController : BaseController
    {
        // GET: Assignment
        public async Task<ActionResult> Index(int id)
        {
            AssignUser assignUser = new AssignUser();
            try
            {
                Project project = new Project();
                InitializeServiceClient();
                HttpResponseMessage projectresponse = await client.PostAsJsonAsync("Project/EditProjectByID?projectID=" + id, req);
                project = await projectresponse.Content.ReadAsAsync<Project>();
                ViewBag.ProjectName = project.projectName;
                ViewBag.ProjectId = id;
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                assignUser.lstProjects = await response.Content.ReadAsAsync<List<Project>>();
                HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
                assignUser.lstUsers = await userResponse.Content.ReadAsAsync<List<Users>>();
                assignUser.selectedProject = id.ToString();
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AssignmentController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(assignUser);
        }
        /// <summary>
        /// Assign a user to the selected project
        /// </summary>
        /// <param name="assignUser"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> Index(AssignUser assignUser)
        {
            AssignUser newUser = new AssignUser();           
            InitializeServiceClient();
            try
            {
                if (assignUser.selectedUser == null)        //Checking whether a user is selected
                {
                    ModelState.AddModelError("selectedUser", "Please select an Employee");
                    HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                    newUser.lstProjects = await response.Content.ReadAsAsync<List<Project>>();                    
                    HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
                    newUser.lstUsers = await userResponse.Content.ReadAsAsync<List<Users>>();
                    newUser.selectedProject = assignUser.selectedProject;
                    return View(newUser);
                }
                if (assignUser.selectedProject == null)     //Checking whether a project is selected
                {
                    ModelState.AddModelError("selectedProject", "Please select a Project");
                    HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                    newUser.lstProjects = await response.Content.ReadAsAsync<List<Project>>();                    
                    HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
                    newUser.lstUsers = await userResponse.Content.ReadAsAsync<List<Users>>();
                    newUser.selectedUser = assignUser.selectedUser;
                    return View(newUser);
                }

                if (assignUser.selectedProject == null && assignUser.selectedUser == null)      //Checking whether a user and project are selected or not
                {
                    ModelState.AddModelError("selectedUser", "Please select an Employee");
                    ModelState.AddModelError("selectedProject", "Please select a Project");                    
                    HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                    newUser.lstProjects = await response.Content.ReadAsAsync<List<Project>>();                    
                    HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
                    assignUser.lstUsers = await userResponse.Content.ReadAsAsync<List<Users>>();
                    return View(assignUser);
                }

                if (ModelState.IsValid)
                {
                    InitializeServiceClient();
                    UserProjectRequest userProjectInfo = new UserProjectRequest();
                    userProjectInfo.UserId = Convert.ToInt32(assignUser.selectedUser);
                    userProjectInfo.ProjectId = Convert.ToInt32(assignUser.selectedProject);
                    userProjectInfo.ClientInfo = req.ClientInfo;
                    HttpResponseMessage ProjResponse = await client.PostAsJsonAsync("Project/UpdateProjectData", userProjectInfo);
                }

                return RedirectToAction("Index", "Assignment");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AssignmentController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View(newUser);
            }
        }

    }
}