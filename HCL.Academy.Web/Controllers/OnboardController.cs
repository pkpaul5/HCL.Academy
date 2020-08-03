﻿using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class OnboardController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public ActionResult NLWorkPermit()
        {
            return View();
        }
        /// <summary>
        /// Fetches Onboarding details
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public async Task<ActionResult> OnBoarding()
        {
            InitializeServiceClient();
          //  IDAL dal = (new DALFactory()).GetInstance();
            ViewBag.showMarketRiskLink = false;
            ViewBag.MarketRiskLinkID = 0;
            OnBoardingViewModel boardingViewModel = new OnBoardingViewModel();
            bool sendEmail = false;           
            try
            {
               // List<OnBoarding> listOnboarding = dal.GetBoardingDataFromOnboarding(ref sendEmail);
              
                HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetBoardingDataFromOnboarding", req);
                List<OnBoarding> listOnboarding = await response.Content.ReadAsAsync<List<OnBoarding>>();

                if (listOnboarding.Count > 0)
                {
                    boardingViewModel.topRowList = listOnboarding.ToList().Where((c, i) => i % 2 == 0).ToList();
                    boardingViewModel.bottomRowList = listOnboarding.ToList().Where((c, i) => i % 2 != 0).ToList();
                    boardingViewModel.bgColorList = GetBgColor(listOnboarding);
                    boardingViewModel.sendEmail = sendEmail;                    
                    ViewBag.showMarketRiskLink = false;
                    //if (ViewBag.showMarketRiskLink)
                    //{
                    //    ViewBag.MarketRiskLinkID = dal.GetMarketRiskAssessmentID();
                    //}
                }
            }
            catch(Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("OnboardController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
            }
            return View(boardingViewModel);
        }
        /// <summary>
        /// Updates the status of the user being On boarded
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<JsonResult> OnBoardingStatusUpdate(List<OnBoardingTrainingStatus> obj)
        {
            //IDAL dal = (new DALFactory()).GetInstance();

            //List<object> objOnboard = dal.UpdateOnBoardingStatus(obj);

            InitializeServiceClient();
            TrainingStatusRequest trainingStatus = new TrainingStatusRequest();
            trainingStatus.ClientInfo = req.ClientInfo;
            trainingStatus.TrainingStatusItems = obj;
            HttpResponseMessage bannerResponse = await client.PostAsJsonAsync("Onboarding/UpdateOnBoardingStatus", trainingStatus);
            List<object> objOnboard = await bannerResponse.Content.ReadAsAsync<List<object>>();

            Response.RemoveOutputCacheItem("/onboard/onboarding");
            Response.RemoveOutputCacheItem("/home/getlearningjourney");
            Response.RemoveOutputCacheItem("/home/getassessments");
            Response.RemoveOutputCacheItem("/training/training");
            return Json(objOnboard, JsonRequestBehavior.AllowGet);
        }
      
        ///// <summary>
        ///// Sends an email regarding the on boarding status
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[SessionExpire]
        //[HttpPost]
        //public JsonResult OnBoardingStatusEmail()
        //{
        //    bool status = false;
        //    Response.RemoveOutputCacheItem("/onboard/onboarding");
        //    try
        //    {
        //        IDAL dal = (new DALFactory()).GetInstance();
        //        status = dal.EmilOnBoardingStatus();
        //    }
        //    catch (Exception ex)
        //    {
        //        UserManager user = (UserManager)Session["CurrentUser"];
        //        LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "OnBoard, OnBoardingStatusEmail", ex.Message, ex.StackTrace));

        //        status = false;
        //        Utilities.LogToEventVwr(ex.Message.ToString(), 0);
        //    }
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        //// Called from Onboarding "selected Profile"
        //[Authorize]
        //[SessionExpire]
        //public PartialViewResult GetUploadProfile()
        //{
        //    UserManager user = (UserManager)Session["CurrentUser"];
        //    try
        //    {
        //        IDAL dal = (new DALFactory()).GetInstance();
        //        dal.GetOnBoardingProfile();
              
        //    }
        //    catch(Exception ex)
        //    {               
        //        LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "OnBoard, GetUploadProfile", ex.Message, ex.StackTrace));
        //    }
        //    return PartialView("_UploadProfile", user);
        //}

        public async Task<PartialViewResult> GetInformation()
        {
            List<OnboardingHelp> onboardHelp = new List<OnboardingHelp>();
            try
            {
                //   IDAL dal = (new DALFactory()).GetInstance();
                // onboardHelp = dal.GetOnboardingHelp();     //Get the information about the Onboarding process.            

                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnboardingHelp", req);
                onboardHelp = await response.Content.ReadAsAsync<List<OnboardingHelp>>();

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("OnboardController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("_OnboardingHelp", onboardHelp);
        }
        ///// <summary>
        ///// Upload a profile from the selected file.
        ///// </summary>
        ///// <param name="uploadedFile"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult UploadProfile(HttpPostedFileBase uploadedFile)
        //{
        //    try
        //    {
        //        if (uploadedFile != null && uploadedFile.ContentLength > 0)
        //        {
        //            if (uploadedFile.ContentType == "application/doc" ||
        //                uploadedFile.ContentType == "application/docx" ||
        //                uploadedFile.ContentType == "application/octet-stream" ||
        //                uploadedFile.ContentType == "application/msword" ||
        //                uploadedFile.ContentType == "application/x-msw6" ||
        //                uploadedFile.ContentType == "application/x-msword" ||
        //                uploadedFile.ContentType == "application/pdf" ||
        //                uploadedFile.ContentType == "application/x-pdf" ||
        //                uploadedFile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        //            {
        //                byte[] fileByteArray = new byte[uploadedFile.ContentLength];

        //                uploadedFile.InputStream.Read(fileByteArray, 0, uploadedFile.ContentLength);

        //                //SharePointDAL dal = new SharePointDAL();

        //                UserManager user = (UserManager)Session["CurrentUser"];
        //                string fileName = user.UserName + "_Profile." + uploadedFile.FileName.Split('.').Last();
        //                dal.UploadProfile(fileName, fileByteArray);

        //                return Json(new
        //                {
        //                    statusCode = 200,
        //                    status = true,
        //                    message = "Profile Uploaded Successfully.",
        //                }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json(new
        //                {
        //                    statusCode = 400,
        //                    status = false,
        //                    message = "Please upload files having extensions: <strong>.doc , .docx, .pdf</strong> only.",
        //                }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

                
        //    }
        //    catch (Exception ex)
        //    {
        //        UserManager user = (UserManager)Session["CurrentUser"];
        //        LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "OnBoard, UploadProfile", ex.Message, ex.StackTrace));

        //    }

        //    return Json(new
        //    {
        //        statusCode = 400,
        //        status = false,
        //        message = "Bad Request! Upload Failed",
        //    }, JsonRequestBehavior.AllowGet);
        //}

        #region PRIVATE methods
        /// <summary>
        /// Get the background color of the cells in the Excel sheet based on the status of Onboarding
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<bgColor> GetBgColor(List<OnBoarding> list)
        {
            List<bgColor> bgColorList = new List<bgColor>();
            try
            {
                foreach (OnBoarding item in list)
                {
                    switch (item.boardingStatus)
                    {
                        case OnboardingStatus.NotStarted:
                            bgColorList.Add(bgColor.blue);
                            break;

                        case OnboardingStatus.OnGoing:
                            bgColorList.Add(bgColor.orange);
                            break;

                        case OnboardingStatus.Completed:
                            bgColorList.Add(bgColor.green);
                            break;

                        case OnboardingStatus.Rejected:
                            bgColorList.Add(bgColor.red);
                            break;

                        case OnboardingStatus.Failed: //Added Failed - Sudipta
                            bgColorList.Add(bgColor.red);
                            break;

                        case OnboardingStatus.OverDue: //Added Failed - Sudipta
                            bgColorList.Add(bgColor.red);
                            break;

                        default:
                            bgColorList.Add(bgColor.blue);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //    UserManager user = (UserManager)Session["CurrentUser"];
                //  LogHelper.AddLog("OnboardController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return bgColorList;
        }

        #endregion
    }
}