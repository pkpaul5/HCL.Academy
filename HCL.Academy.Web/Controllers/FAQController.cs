using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class FAQController : Controller
    {
        /// <summary>
        /// Get the FAQs page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public ActionResult FAQ()
        {
            try
            {
                //  IDAL dal = (new DALFactory()).GetInstance();
                SPAuthUtility spUtil = new SPAuthUtility();
                List<WikiPolicies> wikiPol = spUtil.GetAllWikiPolicies();      //Get all the Wiki policies
                WikiPolicyDocuments poldocs = new WikiPolicyDocuments();
                poldocs.ListOfWiki = wikiPol;
                return View(poldocs);
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                // LogHelper.AddLog("FAQController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                throw;
            }
        }
    }
}