using HCLAcademy.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class TrainingMasterController : BaseController
    {
        // GET: TrainingMaster
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("TrainingMaster/GetAllMasterTrainings", req);
            List<TrainingMaster> trainingMaster = await trainingResponse.Content.ReadAsAsync<List<TrainingMaster>>();
            return View(trainingMaster);
        }
        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync("TrainingMaster/GetTrainingContent", req);
            List<TrainingContent> trainingContent = await responseMessage.Content.ReadAsAsync<List<TrainingContent>>();
            TrainingMaster master = new TrainingMaster();
            master.contents = trainingContent;
            return View(master);
        }
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Create(TrainingMaster collection)
        {
            InitializeServiceClient();
            try
            {
                TrainingMasterRequest trainReq = new TrainingMasterRequest();
                trainReq.ClientInfo = req.ClientInfo;
                trainReq.selectedContent = collection.selectedContent;
                trainReq.description = collection.description;
                trainReq.document = collection.trainingDocument;
                if(trainReq.document == null)
                    trainReq.document = "";
                trainReq.skillType = collection.skillType;
                trainReq.title = collection.title;
                trainReq.trainingCategory = collection.trainingCategory;
                trainReq.trainingLink = collection.trainingLink;
                if (trainReq.trainingLink == null)
                    trainReq.trainingLink = "";
               HttpResponseMessage response = await client.PostAsJsonAsync("TrainingMaster/AddTraining", trainReq);
                bool result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    ViewBag.Success = true;
                }
                else
                    ViewBag.Success = false;

                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("TrainingMaster/GetTrainingContent", req);
                List<TrainingContent> trainingContent = await responseMessage.Content.ReadAsAsync<List<TrainingContent>>();
                collection.contents = trainingContent;
                return View(collection);                

            }
            catch
            {
                return View();
            }
        }
        [Authorize]
        [SessionExpire]
        [HttpGet]
        public async Task<ActionResult> Edit(int Id)
        {
            InitializeServiceClient();
            TrainingMaster trainingMaster = new TrainingMaster();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("TrainingMaster/GetMasterTrainingById?id=" + Id.ToString(), req);
                trainingMaster = await response.Content.ReadAsAsync<TrainingMaster>();
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("TrainingMaster/GetTrainingContent", req);
                List<TrainingContent> trainingContent = await responseMessage.Content.ReadAsAsync<List<TrainingContent>>();
                trainingMaster.contents = trainingContent;
            }
            catch (Exception ex)
            {
                // LogHelper.AddLog("TrainingMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(trainingMaster);
        }
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> Edit(TrainingMaster trainingMaster)
        {
            InitializeServiceClient();
            try
            {
                if (ModelState.IsValid)
                {
                    TrainingMasterRequest trainingReq = new TrainingMasterRequest();
                    trainingReq.ClientInfo = req.ClientInfo;
                    trainingReq.Id = trainingMaster.Id;
                    trainingReq.skillType = trainingMaster.skillType;
                    trainingReq.title = trainingMaster.title;
                    trainingReq.trainingCategory = trainingMaster.trainingCategory;
                    trainingReq.trainingLink = trainingMaster.trainingLink;
                    if (trainingReq.trainingLink == null)
                        trainingReq.trainingLink = "";
                    trainingReq.selectedContent = trainingMaster.selectedContent;
                    trainingReq.description = trainingMaster.description;
                    trainingReq.document = trainingMaster.trainingDocument;
                    if(trainingReq.document == null)
                        trainingReq.document = "";
                    HttpResponseMessage response = await client.PostAsJsonAsync("TrainingMaster/UpdateTraining", trainingReq);
                    bool result = await response.Content.ReadAsAsync<bool>();
                    if(result)
                    { 
                        ViewBag.Success = true;
                    }
                    else
                        ViewBag.Success = false;

                    HttpResponseMessage responseMessage = await client.PostAsJsonAsync("TrainingMaster/GetTrainingContent", req);
                    List<TrainingContent> trainingContent = await responseMessage.Content.ReadAsAsync<List<TrainingContent>>();
                    trainingMaster.contents = trainingContent;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return View(trainingMaster);
        }
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("TrainingMaster/DeleteTraining?id=" + id, req);
                bool result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result == false)
                {
                    TempData["Message"] = "Training cannot be deleted due to either referential integrity or some other issue";
                    TempData.Keep();
                }
                else if (result == true)
                {
                    TempData["Message"] = "Training deleted successfully";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingMasterController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}