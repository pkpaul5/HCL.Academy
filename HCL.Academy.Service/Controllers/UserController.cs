using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service exposes methods related to user
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        /// <summary>
        /// This method returns all the roles
        /// </summary>
        /// <param name="req">Service client information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllRoles")]
        public List<Role> GetAllRoles(RequestBase req)
        {
            List<Role> result = new List<Role>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetAllRoles();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetAllRoles", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns all the roles
        /// </summary>
        /// <param name="req">Service client information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserRole")]
        public List<int> GetUserRole(RequestBase req)
        {
            List<int> result = new List<int>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetUserRole();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetUserRole", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method returns list of users
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUsers")]
        public List<Users> GetUsers(RequestBase req)
        {
            List<Users> result = new List<Users>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetUsers();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetUsers", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method resturns user's information like peers etc.
        ///// </summary>
        ///// <param name="req"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ActionName("GetCurrentUserCompleteUserProfile")]
        //public UserManager GetCurrentUserCompleteUserProfile(RequestBase req)
        //{
        //    UserManager result = new UserManager();
        //    try
        //    {
        //        SharePointDAL dal = new SharePointDAL(req.ClientInfo);
        //        result = dal.GetCurrentUserCompleteUserProfile();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.AddLog("UserController,GetCurrentUserCompleteUserProfile", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
        //    }
        //    return result;
        //}

        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRoleForOnboardedUser")]
        public List<UserRole> GetRoleForOnboardedUser(RequestBase req, int userId)
        {
            List<UserRole> result = new List<UserRole>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetRoleForOnboardedUser(userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetRoleForOnboardedUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="emailAddress">email Address</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserActiveStatus")]
        public string GetUserActiveStatus(RequestBase req, string emailAddress)
        {
            string result = String.Empty;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.CheckUserActive(emailAddress);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetUserActiveStatus", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method assigns role to a user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddRole")]
        public bool AddRole(RoleAssignmentRequest req)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.AddRole(req.Email, req.UserId, req.RoleId, req.Ismandatory, req.Lastdayofcompletion);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }

        /// <summary>
        /// /// This method removes role assigned to user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="roleId">role id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveUserRole")]
        public bool RemoveUserRole(RequestBase req,int roleId,string userId)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.RemoveUserRole(roleId,userId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,RemoveUserRole", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }
        /// <summary>
        /// /// This method adds skill for a role
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="roleId">role id</param>
        /// <param name="skillId">skill id</param>
        /// <param name="competencylevelId">competencylevel Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddRoleSkill")]
        public bool AddRoleSkill(RequestBase req, int roleId, int skillId,int competencylevelId)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.AddRoleSkill(roleId, skillId, competencylevelId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,AddRoleSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }
        /// <summary>
        /// /// This method gets skills for a role
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="roleId">role id</param>    
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRoleSkill")]
        public List<RoleSkill> GetRoleSkill(RequestBase req, int roleId)
        {
            List<RoleSkill> skills = new List<RoleSkill>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                skills = dal.GetAllRoleSkill(roleId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetRoleSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return skills;
        }
        /// <summary>
        /// /// This method deletes skill for a role
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="roleId">role id</param>
        /// <param name="skillId">skill id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteRoleSkill")]
        public bool DeleteRoleSkill(RequestBase req, int roleId, int skillId)
        {
            bool flag = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                flag = dal.DeleteRoleSkill(roleId, skillId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,DeleteRoleSkill", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }

        /// <summary>
        /// Get user id by email address
        /// </summary>
        /// <param name="req">Client information</param>
        /// <param name="emailId">Email address</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserId")]
        public int GetUserId(RequestBase req, string emailId)
        {
            int userId = 0;
            try
            {   
                 SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                 userId = sqlDAL.GetUserId(emailId);                
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetUserId", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return userId;
        }
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="req">client info</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRoles")]
        public List<Role> GetRoles(RequestBase req)
        {
            List<Role> roles = new List<Role>();
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                roles = sqlDAL.GetRoles();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return roles;
        }
        /// <summary>
        /// Gets a list of roles with skills assigned to them.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetRolesWithSkills")]
        public List<Role> GetRolesWithSkills(RequestBase req)
        {
            List<Role> roles = new List<Role>();
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                roles = sqlDAL.GetRolesWithSkills();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return roles;
        }
        /// <summary>
        /// Gets the Skill Gap Report.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetSkillGapReports")]
        public List<SkillGapReport> GetSkillGapReports(RequestBase req,int roleID)
        {
            List<SkillGapReport> roles = new List<SkillGapReport>();
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                roles = sqlDAL.GetSkillGapReports(roleID);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return roles;
        }

        /// <summary>
        /// Thisd method updates roletraining
        /// </summary>
        /// <param name="req">client infrormation</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateRoleTraining")]
        public bool UpdateRoleTraining(RoleTrainingRequest req)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.UpdateRoleTraining(req.ItemId,req.TrainingId,req.RoleId,req.IsMandatory);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,UpdateRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Add role training
        /// </summary>
        /// <param name="req">client info</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddRoleTraining")]
        public bool AddRoleTraining(RoleTrainingRequest req)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.AddRoleTraining(req.TrainingId, req.RoleId, req.IsMandatory);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,AddRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Remove role training
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="id">training id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveRoleTraining")]
        public bool RemoveRoleTraining(RequestBase req,int id)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.RemoveRoleTraining(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,RemoveRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Update role information
        /// </summary>
        /// <param name="req">Client information</param>
        /// <param name="roleId">Role id</param>
        /// <param name="roleName">role name</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateRole")]
        public bool UpdateRole(RequestBase req, int roleId, string roleName)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.UpdateRole(roleId,roleName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,UpdateRole", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }


        /// <summary>
        /// Remove role
        /// </summary>
        /// <param name="req">client information</param>
        /// <param name="roleId">role id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("RemoveRole")]
        public bool RemoveRole(RequestBase req, int roleId)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.RemoveRole(roleId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,RemoveRole", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Add role detail
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="roleName">role name</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddRoleDetail")]
        public bool AddRoleDetail(RequestBase req, string roleName)
        {
            bool response = false;
            try
            {
                SqlSvrDAL sqlDAL = new SqlSvrDAL(req.ClientInfo);
                response = sqlDAL.AddRole(roleName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("UserController,AddRoleDetail", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Add user to SharePoint membership group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        //[HttpPost]
        //[ActionName("AddUserToGroup")]
        //public int AddUserToGroup(RequestBase req, string emailId)
        //{
        //    int employeeId = 0;
        //    try
        //    {
        //        SharePointDAL dal = new SharePointDAL(req.ClientInfo);
        //        dal.AddUserToGroup(emailId,ref employeeId);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.AddLog("UserController,AddUserToGroup", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
        //    }
        //    return employeeId;
        //}

    }
}
