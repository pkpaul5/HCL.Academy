using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class RoleTrainingController : BaseController
    {

        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            try
            {
                
                InitializeServiceClient();
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetAllRoleTrainings", req);
                List<RoleTraining> roleTrainings = await trainingResponse.Content.ReadAsAsync<List<RoleTraining>>();

                return View(roleTrainings);
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Create()
        {
           
            dynamic mymodel = new ExpandoObject();
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetMasterTrainings", req);
            List<SkillTraining> roleTrainings = await trainingResponse.Content.ReadAsAsync<List<SkillTraining>>();

            HttpResponseMessage rolesResponse = await client.PostAsJsonAsync("User/GetRoles", req);
            List<Role> roles = await rolesResponse.Content.ReadAsAsync<List<Role>>();

            mymodel.Roles = roles;
            mymodel.Trainings = roleTrainings;

            return View(mymodel);
        }

        [Authorize]
        [HttpGet]
        [SessionExpire]
        public async Task<ActionResult> Edit(int id)
        {
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetAllRoleTrainings", req);
            List<RoleTraining> roleTrainings = await trainingResponse.Content.ReadAsAsync<List<RoleTraining>>();

            HttpResponseMessage rolesResponse = await client.PostAsJsonAsync("User/GetRoles", req);
            List<Role> roles = await rolesResponse.Content.ReadAsAsync<List<Role>>();

            HttpResponseMessage mastertrainingResponse = await client.PostAsJsonAsync("Training/GetMasterTrainings", req);
            List<SkillTraining> skillTrainings = await mastertrainingResponse.Content.ReadAsAsync<List<SkillTraining>>();

            RoleTraining returnData = roleTrainings.Where(r => r.RoleTrainingId == id).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();
            mymodel.Roles = roles;
            mymodel.Trainings = skillTrainings;
            mymodel.RoleTraining = returnData;
            return View(mymodel);
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> UpdateRoleTraining(int itemId, int trainingId, int roleId, bool isMandatory)
        {   
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetAllRoleTrainings", req);
            List<RoleTraining> roleTrainings = await trainingResponse.Content.ReadAsAsync<List<RoleTraining>>();
            List<RoleTraining> returnData = roleTrainings.Where(r => r.RoleTrainingId != itemId && (r.TrainingId == trainingId && r.RoleId == roleId)).ToList();
            if (returnData.Count() > 0)
            {
                return new JsonResult { Data = false };
            }
            else
            {
                //bool status = dal.UpdateRoleTraining(itemId, trainingId, roleId, isMandatory);
                RoleTrainingRequest roleTrainingRequest = new RoleTrainingRequest();
                roleTrainingRequest.ItemId = itemId;
                roleTrainingRequest.TrainingId = trainingId;
                roleTrainingRequest.RoleId = roleId;
                roleTrainingRequest.IsMandatory = isMandatory;
                HttpResponseMessage updateResponse = await client.PostAsJsonAsync("User/UpdateRoleTraining", roleTrainingRequest);
                bool status = await updateResponse.Content.ReadAsAsync<bool>();
                
                return new JsonResult { Data = status };
            }
        }

        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                InitializeServiceClient();                
                HttpResponseMessage response = await client.PostAsJsonAsync("User/RemoveRoleTraining?id="+id, req);
                bool status = await response.Content.ReadAsAsync<bool>();               
                return RedirectToAction("Index");
            }
            catch
            {
                //  return View();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> DeleteRoleTraining(int id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/RemoveRoleTraining?id=" + id, req);
                bool status = await response.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = status };
            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }

        }

        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> PostRoleTraining(int trainingId, int roleId, bool isMandatory)
        {
            try
            {
               
                InitializeServiceClient();
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetAllRoleTrainings", req);
                List<RoleTraining> roleTrainings = await trainingResponse.Content.ReadAsAsync<List<RoleTraining>>();
                List<RoleTraining> returnData = roleTrainings.Where(r => r.TrainingId == trainingId && r.RoleId == roleId).ToList();
                if (returnData.Count() > 0)
                {
                    return new JsonResult { Data = false };
                }
                else
                {
                    RoleTrainingRequest trainingRequest = new RoleTrainingRequest();
                    trainingRequest.ClientInfo = req.ClientInfo;
                    trainingRequest.IsMandatory = isMandatory;
                    trainingRequest.TrainingId = trainingId;
                    trainingRequest.RoleId = roleId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("User/AddRoleTraining", trainingRequest);
                    bool status = await response.Content.ReadAsAsync<bool>();
                    return new JsonResult { Data = status };
                }

            }
            catch (Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = null };
            }

        }
    }
}