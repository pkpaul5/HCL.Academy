using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class SkillsController : BaseController
    {
        // GET: Skills
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public async Task<ActionResult> Skills()
        {
            try
            {
              
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetSkills", req);
                List<Skills> allSkills = await response.Content.ReadAsAsync<List<Skills>>();
                return View(allSkills);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("SkillsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }
    }
}