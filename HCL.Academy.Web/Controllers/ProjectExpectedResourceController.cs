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
    public class ProjectExpectedResourceController : BaseController
    {
        // GET: ProjectExpectedResource
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProjectExpectedResource/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProjectExpectedResource/Create
        /// <summary>
        /// Sets the expected resources and their respective skills
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public async Task<ActionResult> Create(int projectID)
        {
            ProjectResources prjRes = new ProjectResources();
            //prjRes.projectId = projectID;
            //prjRes.skillResources = new List<SkillResource>();
            InitializeServiceClient();
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //prjRes = dal.GetExpectedProjectResourceCountByProjectId(projectID);
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/GetExpectedProjectResourceCountByProjectId?projectID=" + projectID, req);
                prjRes = await response.Content.ReadAsAsync<ProjectResources>();
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectExpectedResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(prjRes);
        }

        // POST: ProjectExpectedResource/Create
        [HttpPost]
        public async Task<ActionResult> Create(ProjectResources prjRes)
        {
            InitializeServiceClient();
            try
            {
                ProjectResourcesRequest req = new ProjectResourcesRequest();
                req.projectId = prjRes.projectId;
                req.projectName = prjRes.projectName;
                req.skillResources = prjRes.skillResources;
                HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddExpectedProjectResourceCountByProjectId", req);
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectExpectedResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            client.Dispose();

            return View(prjRes);
        }

        // GET: ProjectExpectedResource/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectExpectedResource/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, System.Web.Mvc.FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectExpectedResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View();
            }
        }

        // GET: ProjectExpectedResource/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectExpectedResource/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, System.Web.Mvc.FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectExpectedResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View();
            }
        }
    }
}
