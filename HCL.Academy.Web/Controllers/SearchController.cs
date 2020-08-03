using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class SearchController : Controller
    {
        private static List<Result> lstResult;
        /// <summary>
        /// Fetches results for a particular Keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [SessionExpire]
        public ActionResult Search(string keyword)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                SPAuthUtility spUtil = new SPAuthUtility();
                lstResult = spUtil.Search(keyword);
                return RedirectToAction("Search", "Search");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("SearchController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }
        /// <summary>
        /// Search for a particular keyword
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public ActionResult Search()
        {
            if (lstResult != null && lstResult.Count > 0)
            {
                ViewBag.lstResults = lstResult;
            }
            else
            {
                ViewBag.lstResults = null;
            }
            return View();
        }
    }
}