using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;


namespace HCLAcademy.Controllers
{
    public class AcademyVideoController : BaseController
    {

        // GET: Get All Videos
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                InitializeServiceClient();
              //  SharePointDAL dal = new SharePointDAL();
                List<AcademyVideo> lstAcademyVideo = new List<AcademyVideo>();
                HttpResponseMessage videoResponse = await client.PostAsJsonAsync("Video/GetAllAcademyVideos", req);
                lstAcademyVideo = await videoResponse.Content.ReadAsAsync<List<AcademyVideo>>();
                //dal.GetAllAcademyVideos();
                return View(lstAcademyVideo);
            }
            catch (Exception ex)
            {
                //                UserManager user = (UserManager)Session["CurrentUser"];
                //              LogHelper.AddLog("AcademyVideoController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                throw;
            }
        }

        //POST: Ajax Call
        [HttpPost]
        [Authorize]
        [SessionExpire]
        public PartialViewResult GetVideoWindow(string url, string videoTitle)
        {
            AcademyVideo av = new AcademyVideo();
            av.url = url;
            av.title = videoTitle;
            return PartialView("_AcademyVideoModel", av);
        }

        ////url to stream
        //[Authorize]
        //[SessionExpire]
        //public FileStreamResult GetVideoStream(string url)
        //{
        //    SharePointDAL dal = new SharePointDAL();            
        //    return dal.GetVideoStream(url);
        //}

    }
}
