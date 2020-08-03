using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ManageRolesController : BaseController
    {

        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Roles()
        {
            InitializeServiceClient();
            try
            {
                //  IDAL dal = (new DALFactory()).GetInstance();
                //  List<Role> roles = dal.GetRoles();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/GetRoles", req);
                List<Role> roles = await response.Content.ReadAsAsync<List<Role>>();
                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                ViewBag.Competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();

                return View(roles);
            }
            catch (Exception ex)
            {
                //  UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetRoles(string roleName)
        {
            List<Role> roles = new List<Role>();
            InitializeServiceClient();
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //roles = dal.GetRoles();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/GetRoles", req);
                roles = await response.Content.ReadAsAsync<List<Role>>();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return new JsonResult { Data = roles };
        }
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetRoleSkill(string roleId)
        {
            InitializeServiceClient();
            List<RoleSkill> result = new List<RoleSkill>();
            try
            {
                HttpResponseMessage responsealldata = await client.PostAsJsonAsync("User/GetRoleSkill?roleId=" + roleId, req);
                result = await responsealldata.Content.ReadAsAsync<List<RoleSkill>>();

                for(int i=0;i<result.Count;i++)
                {   
                    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillId?skillId=" + result[i].SkillId, req);
                    List<Competence> competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                    result[i].ValidCompetencies = competencies;
                }

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return new JsonResult { Data = result };
        }
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> AddRoleSkill(string roleId, string skillId, string competencylevelId)
        {
            List<RoleSkill> result = new List<RoleSkill>();
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("User/AddRoleSkill?roleId=" + roleId + "&skillId=" + skillId + "&competencylevelId=" + competencylevelId, req);
                bool status = await response.Content.ReadAsAsync<bool>();
                HttpResponseMessage responsealldata = await client.PostAsJsonAsync("User/GetRoleSkill?roleId=" + roleId, req);
                result = await responsealldata.Content.ReadAsAsync<List<RoleSkill>>();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return new JsonResult { Data = result };
        }
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> PostRole(string roleName)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/AddRoleDetail?roleName=" + roleName, req);
                bool status = await response.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = status };
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> UpdateRole(int roleId, string roleName)
        {
            InitializeServiceClient();
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //List<Role> roles = dal.GetRoles();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/GetRoles", req);
                List<Role> roles = await response.Content.ReadAsAsync<List<Role>>();

                List<Role> returnData = roles.Where(r => r.Id != roleId && r.Title.ToLower() == roleName.ToLower()).ToList();
                if (returnData.Count() > 0)
                {
                    return new JsonResult { Data = false };
                }
                else
                {
                    //bool status = dal.UpdateRole(roleId, roleName);
                    HttpResponseMessage updateResponse = await client.PostAsJsonAsync("User/UpdateRole?roleId=" + roleId + "&roleName=" + roleName, req);
                    bool status = await updateResponse.Content.ReadAsAsync<bool>();
                    return new JsonResult { Data = status };
                }

            }
            catch (Exception ex)
            {
                //   UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> DeleteRole(int roleId)
        {
            InitializeServiceClient();
            try
            {
                // IDAL dal = (new DALFactory()).GetInstance();
                // bool status = dal.RemoveRole(roleId);

                HttpResponseMessage response = await client.PostAsJsonAsync("User/RemoveRole?roleId=" + roleId, req);
                bool status = await response.Content.ReadAsAsync<bool>();

                return new JsonResult { Data = status };
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }

        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> DeleteRoleSkill(int roleId,int skillId)
        {
            InitializeServiceClient();
            try
            {                
                HttpResponseMessage response = await client.PostAsJsonAsync("User/DeleteRoleSkill?roleId=" + roleId + "&skillId=" + skillId, req);
                bool status = await response.Content.ReadAsAsync<bool>();

                return new JsonResult { Data = status };
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ManageRolesController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }

        }

    }
}