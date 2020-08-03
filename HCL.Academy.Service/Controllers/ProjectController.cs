using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service provides all the Project related functionality in HCL Academy
    /// </summary>
    public class ProjectController : ApiController
    {
        /// <summary>
        /// This method returns all the projects that exist in Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllProjects")]
        public List<Project> GetAllProjects(RequestBase req)
        {
            List<Project> lstProj = new List<Project>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                lstProj = dal.GetAllProjects();       //Gets all projects

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetAllProjects", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return lstProj;
        }

        /// <summary>
        ///  This method returns all the ProjectsSkillResource that exist in Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllProjectSkillResources")]
        public List<ProjectSkillResource> GetAllProjectSkillResources(RequestBase req)
        {
            List<ProjectSkillResource> lstPsr = new List<ProjectSkillResource>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                lstPsr = dal.GetAllProjectSkillResources();       //Gets all ProjectsSkillResource

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetAllProjectSkillResources", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return lstPsr;
        }

        /// <summary>
        /// This method returns all the ProjectsSkillResource for a ProjectId that exist in Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllProjectSkillResourcesByProjectID")]
        public List<ProjectSkillResource> GetAllProjectSkillResourcesByProjectID(RequestBase req, int ProjectID)
        {
            List<ProjectSkillResource> lstPsr = new List<ProjectSkillResource>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                lstPsr = dal.GetAllProjectSkillResourcesByProjectID(ProjectID);       //Gets all ProjectsSkillResource for a particular Project

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetAllProjectSkillResourcesByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return lstPsr;
        }

        /// <summary>
        /// This method returns all the project and skill associations in HCL Academy
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllProjectSkills")]
        public List<ProjectSkill> GetAllProjectSkills(RequestBase req)
        {
            List<ProjectSkill> lstPs = new List<ProjectSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                lstPs = dal.GetAllProjectSkills();      //Gets all ProjectsSkillResource for a particular Project

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetAllProjectSkills", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return lstPs;
        }

        /// <summary>
        /// This method returns Expected Project Resouce Count for a particular Project
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetExpectedProjectResourceCountByProjectId")]
        public ProjectResources GetExpectedProjectResourceCountByProjectId(RequestBase req, int ProjectID)
        {
            ProjectResources pr = new ProjectResources();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                pr = dal.GetExpectedProjectResourceCountByProjectId(ProjectID);  //Gets Expected Project Resouce Count for a particular Project

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetExpectedProjectResourceCountByProjectId", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return pr;
        }

        /// <summary>
        /// This method returns HeatMapProjectDetails for a project
        /// </summary>
        /// <param name="req"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetHeatMapProjectDetailByProjectID")]
        public HeatMapProjectDetail GetHeatMapProjectDetailByProjectID(RequestBase req, int projectID)
        {
            HeatMapProjectDetail hmpd = new HeatMapProjectDetail();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                hmpd = dal.GetHeatMapProjectDetailByProjectID(projectID);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetHeatMapProjectDetailByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return hmpd;
        }

        /// <summary>
        /// This method returns Project Skill association for a project
        /// </summary>
        /// <param name="req"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetProjectSkillsByProjectID")]
        public ProjectDetails GetProjectSkillsByProjectID(RequestBase req, string projectID)
        {
            ProjectDetails pd = new ProjectDetails();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                pd = dal.GetProjectSkillsByProjectID(projectID);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetProjectSkillsByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return pd;
        }

        /// <summary>
        /// This method adds a project to HCL Academy DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProject")]
        public bool AddProject(RequestBase req, string projectName)
        {
            bool status = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                status = dal.AddProject(projectName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddProject", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }

        /// <summary>
        /// This method saves Project and associated Skill
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ProjectID"></param>
        /// <param name="SkillID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProjectSkill")]
        public bool AddProjectSkill(int ProjectID, int SkillID, RequestBase req)
        {
            bool status = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                status = dal.AddProjectSkill(ProjectID, SkillID);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddProjectSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }

        /// <summary>
        /// This method deletes relation between a project and skill
        /// </summary>
        /// <param name="req"></param>
        /// <param name="projectskillid"></param>
        /// <param name="projectid"></param>
        /// <param name="skillid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteProjectSkill")]
        public bool DeleteProjectSkill(RequestBase req, int projectskillid, string projectid, string skillid)
        {
            bool status = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                status = dal.DeleteProjectSkill(projectskillid, projectid, skillid);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,DeleteProjectSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }

        /// <summary>
        /// This method is used for editing a project in HCL Academy DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("EditProjectByID")]
        public Project EditProjectByID(RequestBase req, int projectID)
        {
            Project project = new Project();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                project = dal.EditProjectByID(projectID);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,EditProjectByID", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return project;
        }

        /// <summary>
        /// This method is used for saving overall resource count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProjectSkillResource")]
        public bool AddProjectSkillResource(ProjectSkillResourceRequest request)
        {
            bool status = false;
            try
            {
                ProjectSkillResource psr = new ProjectSkillResource();
                psr.projectId = request.projectId;
                psr.skillId = request.skillId;
                psr.competencyLevelId = request.competencyLevelId;
                psr.expectedResourceCount = request.expectedResourceCount;
                psr.availableResourceCount = request.availableResourceCount;
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                status = dal.AddProjectSkillResource(request.projectId, psr);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddProjectSkillResource", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }

        /// <summary>
        /// This method saves available resource count for various skills associated with a project
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProjectSkillResources")]
        public bool AddProjectSkillResources(ProjectResourcesRequest request)
        {
            bool status = false;
            try
            {
                ProjectResources prjRes = new ProjectResources();
                prjRes.projectId = request.projectId;
                prjRes.projectName = request.projectName;
                prjRes.skillResources = request.skillResources;
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                status = dal.AddProjectSkillResources(prjRes);
            }
            catch (Exception ex)
            {
                //     LogHelper.AddLog("ProjectController,AddProjectSkillResources", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }


        /// <summary>
        /// This method saves expected project resource count for a Project
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddExpectedProjectResourceCountByProjectId")]
        public bool AddExpectedProjectResourceCountByProjectId(ProjectResourcesRequest request)
        {
            bool status = false;
            try
            {
                ProjectResources prjRes = new ProjectResources();
                prjRes.projectId = request.projectId;
                prjRes.skillResources = request.skillResources;
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                status = dal.AddExpectedProjectResourceCountByProjectId(prjRes);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddExpectedProjectResourceCountByProjectId", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return status;
        }

        /// <summary>
        /// This method removes a project from Project table in Academy
        /// </summary>
        /// <param name="userProjectInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveProject")]
        public bool RemoveProject(UserProjectRequest userProjectInfo)
        {
            bool isDeleted = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userProjectInfo.ClientInfo);
                isDeleted = dal.RemoveProject(userProjectInfo.ProjectId);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,RemoveProject", ex.Message, ex.StackTrace, "HCL.Academy.Service", userProjectInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isDeleted;
        }

        /// <summary>
        /// This method update a project in Project table in Academy
        /// </summary>
        /// <param name="userProjectInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateProject")]
        public bool UpdateProject(UserProjectRequest userProjectInfo)
        {
            bool isUpdated = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userProjectInfo.ClientInfo);
                Project objProject = new Project();
                objProject.id = userProjectInfo.ProjectId;
                objProject.projectName = userProjectInfo.ProjectName;
                isUpdated = dal.UpdateProject(objProject);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,UpdateProject", ex.Message, ex.StackTrace, "HCL.Academy.Service", userProjectInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isUpdated;
        }


        /// <summary>
        /// This method provides resource count for  project table in Academy
        /// </summary>
        /// <param name="userProjectInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetResourceDetailsByProjectID")]
        public Resource GetResourceDetailsByProjectID(UserProjectRequest userProjectInfo)
        {
            Resource resource = new Resource();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userProjectInfo.ClientInfo);
                resource = dal.GetResourceDetailsByProjectID(userProjectInfo.ProjectId);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetResourceDetailsByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.Service", userProjectInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return resource;
        }

        /// <summary>
        /// This method is used to map skill against a project in Academy
        /// </summary>
        /// <param name="userProjectInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("PostProjectSkill")]
        public bool PostProjectSkill(UserProjectRequest userProjectInfo)
        {
            bool isAdded = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userProjectInfo.ClientInfo);
                isAdded = dal.PostProjectSkill(Convert.ToString(userProjectInfo.ProjectId), Convert.ToString(userProjectInfo.SkillId));

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,PostProjectSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", userProjectInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isAdded;
        }

        /// <summary>
        /// This method updates the project assigned to an onboarded user in Academy
        /// </summary>
        /// <param name="userProjectInfo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateProjectData")]
        public bool UpdateProjectData(UserProjectRequest userProjectInfo)
        {
            bool isAdded = false;
            AssignUser objUserOnboard = new AssignUser();
            objUserOnboard.selectedUser = Convert.ToString(userProjectInfo.UserId);
            objUserOnboard.selectedProject = Convert.ToString(userProjectInfo.ProjectId);
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(userProjectInfo.ClientInfo);
                isAdded = dal.UpdateProjectData(objUserOnboard);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,UpdateProjectData", ex.Message, ex.StackTrace, "HCL.Academy.Service", userProjectInfo.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isAdded;
        }
        /// <summary>
        /// This method adds project admin
        /// </summary>
        /// <param name="req">Request base</param>
        /// <param name="userid">user id</param>
        /// <param name="projectid">project id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProjectAdmin")]
        public bool AddProjectAdmin(RequestBase req, int userid, int projectid)
        {
            bool isAdded = false;

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.AddProjectAdmin(userid, projectid);
                isAdded = true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddFirstLevelProjectAdmin", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isAdded;
        }

        /// <summary>
        /// This method removes the project admin
        /// </summary>
        /// <param name="req">Request base</param>
        /// <param name="projectid">project id</param>
        /// <param name="userid">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteProjectAdmin")]
        public bool DeleteProjectAdmin(RequestBase req, int projectid, int userid)
        {
            bool flag = false;

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.DeleteProjectAdmin(projectid, userid);
                flag = true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,DeleteProjectAdmin", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }
        /// <summary>
        /// This method provides project admin for the project
        /// </summary>
        /// <param name="req">request base</param>
        /// <param name="projectid">project id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetProjectAdmin")]
        public List<ProjectAdmin> GetProjectAdmin(RequestBase req, int projectid)
        {
            List<ProjectAdmin> admins = new List<ProjectAdmin>();

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                admins = dal.GetProjectAdmin(projectid);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetProjectAdmin", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return admins;
        }

        /// <summary>
        /// Get child projects for parent project
        /// </summary>
        /// <param name="req">request base</param>
        /// <param name="projectid">parent project id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetProjectByParent")]
        public List<Project> GetProjectByParent(RequestBase req, int projectid)
        {
            List<Project> projects = new List<Project>();

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                projects = dal.GetProjectByParent(projectid);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetProjectByParent", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return projects;
        }

        /// <summary>
        /// This method adds project details
        /// </summary>
        /// <param name="req"></param>
        /// <param name="name"></param>
        /// <param name="parentprojectid"></param>
        /// <param name="projectlevel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddProjectDetails")]
        public bool AddProjectDetails(RequestBase req, string name, int parentprojectid, int projectlevel)
        {
            bool isAdded = false;

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.AddProjectDetails(name, parentprojectid, projectlevel);
                isAdded = true;
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,AddProjectDetails", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return isAdded;
        }

        /// <summary>
        /// This method return project admin info of the user
        /// </summary>
        /// <param name="req">request base</param>
        /// <param name="userid">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetProjectAdminInfo")]
        public ProjectAdminInfo GetProjectAdminInfo(RequestBase req, int userid)
        {
            ProjectAdminInfo adminInfo = new ProjectAdminInfo();

            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                adminInfo = dal.GetProjectAdminInfo(userid);

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetProjectAdminInfo", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return adminInfo;
        }
        /// <summary>
        /// This method returns all the project admins
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllProjectAdminInfo")]
        public List<ProjectAdmin> GetAllProjectAdminInfo(RequestBase req)
        {
            List<ProjectAdmin> projAdminInfo = new List<ProjectAdmin>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                projAdminInfo = dal.GetAllProjectAdminInfo();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ProjectController,GetAllProjectAdminInfo", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return projAdminInfo;
        }
    }
}
