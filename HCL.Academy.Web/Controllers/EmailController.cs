using HCL.Academy.Model;
using HCLAcademy.Controllers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class EmailController : BaseController
    {
        // GET: Email
        public async Task<ActionResult> Index()
        {
            List<EmailTemplate> templates = new List<EmailTemplate>();
            InitializeServiceClient();
            HttpResponseMessage emailResponse = await client.PostAsJsonAsync("Email/GetEmailTemplates", req);
            templates = await emailResponse.Content.ReadAsAsync<List<EmailTemplate>>();
            if (templates != null)
                Session["Email"] = templates;
            return View(templates);            
        }
       
        // GET: Email/Create
        public ActionResult Create()
        {
            EmailTemplate template = new EmailTemplate();
            return View(template);
        }

        // POST: Email/Create
        [HttpPost]
        public async Task<ActionResult> Create(EmailTemplate emailTemplate)
        {
            try
            {
                InitializeServiceClient();
                bool result = false;
                EmailTemplateRequest emailRequest = new EmailTemplateRequest();
                emailRequest.ClientInfo = req.ClientInfo;
                emailRequest.title = emailTemplate.title;
                emailRequest.id = emailTemplate.id;
                emailRequest.emailSubject = emailTemplate.emailSubject;
                emailRequest.emailBody = emailTemplate.emailBody;
                
                HttpResponseMessage response = await client.PostAsJsonAsync("Email/AddEmailTemplate", emailRequest);
                result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["Message"] = "Added successfully.";
                    TempData.Keep();
                }
                else
                {
                    TempData["Message"] = "Template could not be added due to an error.";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }

        // GET: Email/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            EmailTemplate template = new EmailTemplate();
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Email/GetEmailTemplateById/"+ id.ToString(), req);
            template = await response.Content.ReadAsAsync<EmailTemplate>();
            return View(template);
        }

        // POST: Email/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, EmailTemplate email)
        {
            try
            {
                bool result = false;
                InitializeServiceClient();
                EmailTemplateRequest request = new EmailTemplateRequest();
                request.ClientInfo = req.ClientInfo;
                request.emailBody = email.emailBody;
                request.emailSubject = email.emailSubject;
                request.title = email.title;
                request.id = id;
                HttpResponseMessage response = await client.PostAsJsonAsync("Email/UpdateEmailTemplate", request);
                result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    TempData["Message"] = "Updated successfully.";
                    TempData.Keep();
                }
                else
                {
                    TempData["Message"] = "Template could not be updated due to an error.";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("Email/DeleteEmailTemplate?id=" + id, req);
                bool result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result == false)
                {
                    TempData["Message"] = "Template cannot be deleted due to some issues";
                    TempData.Keep();
                }
                else if (result == true)
                {
                    TempData["Message"] = "Template deleted successfully";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return RedirectToAction("Index");
        }
    }
}
