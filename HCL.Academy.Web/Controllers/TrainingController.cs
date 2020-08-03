﻿using HCL.Academy.Model;
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
    public class TrainingController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public async Task<ViewResult> Training()
        {
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetUserTrainingsDetails", req);
            List<UserSkillDetail> traningModules = await trainingResponse.Content.ReadAsAsync<List<UserSkillDetail>>();
            return View(traningModules);
        }

        #region Static Trainings

        public ActionResult OPSTraining()
        {
            return View();
        }

        public ActionResult PolymerTraining()
        {
            return View();
        }

        public ActionResult ScalaTraining()
        {
            return View();
        }

        public ActionResult OracleApplicationExpressTraining()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TIBCOTraining()
        {
            return View();
        }

        public ActionResult FullStackDevelopmentTranning()
        {
            return View();
        }

        #endregion

        #region Training Materials 

        [Authorize]
        [SessionExpire]
        public ActionResult TrainingMaterials(string Selected)
        {
            ViewBag.Selected = (string.IsNullOrEmpty(Selected)) ? string.Empty : Selected.Replace(" ", "");
            ViewBag.Folder = (string.IsNullOrEmpty(Selected)) ? string.Empty : Selected.Replace(" ", "%20");
            return View();
        }

        [Authorize]
        [SessionExpire]
        public ActionResult TrainingDocumentTree(string folder)
        {
            WikiPolicyDocuments poldocs = new WikiPolicyDocuments();

            try
            {
                SPAuthUtility spUtil = new SPAuthUtility();
                poldocs = spUtil.GetWikiDocumentTree(Server, folder);
                return PartialView("_TrainingMaterials", poldocs.ListOfWikiDoc);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                throw;
            }
        }

        private List<WikiDocuments> GetChild(List<WikiDocuments> wikiDoc)
        {
            //Get child items
            List<WikiDocuments> wikiDocchild = new List<WikiDocuments>();
            foreach (WikiDocuments item in wikiDoc)
            {
                var wikichilddoc = from c in wikiDoc where c.DocumentURL.Equals(item.ParentFolderURL) select c;
                foreach (WikiDocuments itemwiki in wikichilddoc.ToList())
                {
                    if (itemwiki.WikiChild == null)
                    {
                        itemwiki.WikiChild = new List<WikiDocuments>();
                    }

                    itemwiki.WikiChild.Add(item);
                }
            }
            var d = from c in wikiDoc where c.ParentFolder.Equals("OnBoarding") select c;
            return d.ToList();
        }

        #endregion
    }
}