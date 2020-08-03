using HCL.Academy.Model;
using System;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class SkillHeatMapController : BaseController
    {
      
        // GET: SkillHeatMap/Details/5
        /// <summary>
        /// Fetch the project Details of the selected project ID
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int projectID)
        {
            InitializeServiceClient();
            try
            {
                
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetHeatMapProjectDetailByProjectID?projectID=" + projectID, req);
                HeatMapProjectDetail heatMapProjectDetail = await response.Content.ReadAsAsync<HeatMapProjectDetail>();
                return View(heatMapProjectDetail);
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SkillHeatMapController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                return View();
            }
        }
        /// <summary>
        /// Displays the Project Details
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Details(HeatMapProjectDetail p)
        {  
            return View(p);
        }
    }
}
