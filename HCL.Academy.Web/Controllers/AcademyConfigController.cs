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
    public class AcademyConfigController : BaseController
    {
        // GET: AcademyConfig
        public async Task<ActionResult> Index()
        {
            List<AcademyConfig> lstAcademyConfig = new List<AcademyConfig>();
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/GetAllAcademyConfig", req);
                lstAcademyConfig = await response.Content.ReadAsAsync<List<AcademyConfig>>();
                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
            }
            return View(lstAcademyConfig);
        }

        // GET: AcademyConfig/Details/5
        public async  Task<ActionResult> Details(int id)
        {
            AcademyConfig academyconfig = null;
            InitializeServiceClient();
            try
            {                
                HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/GetAcademyConfigById?id=" + id, req);
                academyconfig = await response.Content.ReadAsAsync<AcademyConfig>();
                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(academyconfig);
        }

        // GET: AcademyConfig/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademyConfig/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            InitializeServiceClient();
            try
            {               
                HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/AddAcademyConfig?Title=" + collection["Title"] + "&Value=" + collection["Value"] , req);
                client.Dispose();
                TempData["CreateSuccess"] = true;
                TempData.Keep();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                // return View();
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View();
        }

        // GET: AcademyConfig/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            AcademyConfig academyconfig = new AcademyConfig();
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/GetAcademyConfigById?id=" + id, req);
                client.Dispose();
                academyconfig = await response.Content.ReadAsAsync<AcademyConfig>();
                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(academyconfig);
        }

        // POST: AcademyConfig/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            AcademyConfig academyConfig = new AcademyConfig();
            academyConfig.ID = Convert.ToInt32(collection["ID"]);
            academyConfig.Title = collection["Title"];
            academyConfig.Value = collection["Value"];
            InitializeServiceClient();
            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/UpdateAcademyConfig?Id=" + academyConfig.ID + "&Title=" + academyConfig.Title + "&Value=" + academyConfig.Value, req);
                    client.Dispose();
                    ViewBag.Success = true;
                }

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(academyConfig);
        }

        // GET: AcademyConfig/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            InitializeServiceClient();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("AcademyConfig/DeleteAcademyConfig?id=" + id, req);
                client.Dispose();
                TempData["DeleteMessage"] = "Record deleted successfully";
                TempData.Keep("DeleteMessage");
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
       
    }
}
