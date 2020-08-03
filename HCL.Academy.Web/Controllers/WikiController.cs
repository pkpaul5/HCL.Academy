using HCL.Academy.Model;
using HCLAcademy.Utility;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class WikiController : Controller
    {
        // GET: Wiki
        [Authorize]
        [SessionExpire]
        public ActionResult Wiki(string Selected)
        {
            ViewBag.Selected = (string.IsNullOrEmpty(Selected)) ? Selected : Selected.Replace(" ", "");
            return View();
        }

        /// <summary>
        /// Fetches all the Policy Documents.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]

        public ActionResult WikiPolicy()
        {
            try
            {  
                SPAuthUtility spUtil = new SPAuthUtility();
                WikiPolicyDocuments poldocs = spUtil.GetWikiPolicyDocuments();
                return View(poldocs);
            }
            catch (Exception ex)
            {
                // UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("WikiController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
                throw;
            }
        }

        /// <summary>
        /// Download the Wiki file from path specified.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public FileResult DownloadWikiFile(string filePath)
        {
            string decryptFileName = EncryptionHelper.Decrypt(filePath);        //Decrypt the File Name
            string fileName = decryptFileName.Substring(decryptFileName.LastIndexOf('/') + 1);
            
            SPAuthUtility spUtil = new SPAuthUtility();
            System.IO.Stream fileBytes = spUtil.DownloadDocument(decryptFileName);     //Download the selected document.

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        /// <summary>
        /// Fetches the list of Wiki Documents.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public ActionResult WikiDocumentTree()
        {
            
            SPAuthUtility spUtil = new SPAuthUtility();
            List<WikiDocuments> listOfWikiDoc = spUtil.GetWikiDocumentTree(Server);
            return PartialView("_WikiDocumentTree", listOfWikiDoc);
        }

    }
}