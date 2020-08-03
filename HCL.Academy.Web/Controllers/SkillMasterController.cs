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
    public class SkillMasterController : BaseController
    {
        // GET: SkillMaster
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            List<SkillMaster> lstSkillMaster = new List<SkillMaster>();
            try
            {
                
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkillMaster", req);
                lstSkillMaster = await response.Content.ReadAsAsync<List<SkillMaster>>();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(lstSkillMaster);
        }
        
        // GET: SkillMaster/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SkillMaster/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            InitializeServiceClient();
            try
            {
                SkillRequest skillReq = new SkillRequest();
                skillReq.ClientInfo = req.ClientInfo;
                skillReq.SkillDetails = new SkillMaster();
                if (collection["IsDefault"] == "true")
                    skillReq.SkillDetails.IsDefault = true;
                else
                    skillReq.SkillDetails.IsDefault = false;
                skillReq.SkillDetails.Title = collection["Title"].ToString();

                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/AddSkillDetail", skillReq);
                bool result= await response.Content.ReadAsAsync<bool>();

                TempData["SkillCreateSuccess"] = true;
                TempData.Keep();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SkillMaster/Edit/5
        public async Task<ActionResult> Edit(int Id)
        {
            InitializeServiceClient();
            SkillMaster skillmaster = new SkillMaster();
            try
            {   
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetSkillById/"+Id.ToString(), req);
                skillmaster = await response.Content.ReadAsAsync<SkillMaster>();

            }
            catch (Exception ex)
            {
                // LogHelper.AddLog("SkillMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return View(skillmaster);
        }

        // POST: SkillMaster/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(SkillMaster skillmaster)
        {
            InitializeServiceClient();
            try
            {
                if (ModelState.IsValid)
                {
                    SkillRequest skillReq = new SkillRequest();
                    skillReq.ClientInfo = req.ClientInfo;
                    skillReq.SkillDetails = skillmaster;
                    HttpResponseMessage response = await client.PostAsJsonAsync("Skill/UpdateSkill", skillReq);
                    bool result = await response.Content.ReadAsAsync<bool>();
                    ViewBag.Success = true;
                }

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SkillMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(skillmaster);
        }

        // GET: SkillMaster/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/RemoveSkill/"+id.ToString(), req);
                int ErrorNumber = await response.Content.ReadAsAsync<int>();

                if (ErrorNumber == 50000)
                {
                    TempData["SkillDeleteFailMessage"] = "Record cannot be deleted because it has child record/records";
                    TempData.Keep("SkillDeleteFailMessage");
                    TempData.Remove("SkillDeleteSuccessMessage"); 
                }
                else
                {
                    TempData["SkillDeleteSuccessMessage"] = "Record deleted successfully";
                    TempData.Keep("SkillDeleteSuccessMessage");
                    TempData.Remove("SkillDeleteFailMessage");
                }
                
            }
            catch(Exception ex)
            {
                //LogHelper.AddLog("SkillMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            TempData.Remove("SkillCreateSuccess");
            return RedirectToAction("Index");
        }       
    }
}
