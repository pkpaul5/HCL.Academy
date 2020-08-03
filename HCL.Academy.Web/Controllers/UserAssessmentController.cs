using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using HCL.Academy.Model;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class UserAssessmentController : BaseController
    {
        // GET: UserAssessment
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage userResponse = await client.PostAsJsonAsync("Assessment/GetCurrentUserAssessments?updateAttempts=false", req);
            List <AcademyJoinersCompletion> list = await userResponse.Content.ReadAsAsync<List<AcademyJoinersCompletion>>();
            return View(list);
        }

        // GET: UserAssessment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserAssessment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserAssessment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAssessment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserAssessment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAssessment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserAssessment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
