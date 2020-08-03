using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service handles all the actions regarding emails.
    /// </summary>
    public class EmailController : ApiController
    {
        /// <summary>
        /// Get the list of email templates.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetEmailTemplates")]
        public List<EmailTemplate> GetEmailTemplates(RequestBase req)
        {
            List<EmailTemplate> emailTemplates = new List<EmailTemplate>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                emailTemplates = dal.GetEmailTemplates();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return emailTemplates;
        }
        /// <summary>
        /// Adds an email template
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("AddEmailTemplate")]
        public bool AddEmailTemplate(EmailTemplateRequest req)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.AddEmailTemplate(req.title, req.emailSubject, req.emailBody);
            }
            catch(Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// Gets details of the selected email template.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetEmailTemplateById")]
        public EmailTemplate GetEmailTemplateById(RequestBase req,int id)
        {
            EmailTemplate email = new EmailTemplate();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                email = dal.GetEmailTemplateById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return email;
        }

        /// <summary>
        /// This method will edit the selected email template.
        /// </summary>
        /// <param name="requestBase"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateEmailTemplate")]
        public bool UpdateEmailTemplate(EmailTemplateRequest requestBase)
        {
            bool result = false; ;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(requestBase.ClientInfo);
                result = dal.UpdateEmailTemplate(requestBase.id, requestBase.title, requestBase.emailSubject, requestBase.emailBody);
            }
            catch(Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Service", requestBase.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method will delete an email template
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteEmailTemplate")]
        public bool DeleteEmailTemplate(RequestBase req,int id)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.DeleteEmailTemplate(id);
            }
            catch(Exception ex)
            {
                //LogHelper.AddLog("EmailController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
    }
}
