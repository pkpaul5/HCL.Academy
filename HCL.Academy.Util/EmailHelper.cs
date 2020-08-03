﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Drawing;
using System.IO;

namespace HCLAcademy.Util
{
    public class EmailHelper
    {
        public static void SendEmail(SendMailRequest objtb, string templateCode)
        {
            var sendEmailViaSendGrid = ConfigurationManager.AppSettings["SendEmailViaSendGrid"].ToString();
            if(sendEmailViaSendGrid.ToLower() == "yes")
            {
                SendEmailSendgrid(objtb, templateCode).Wait();
            }
            else
            {
                try
                {
                    O365Credential cred = new O365Credential();
                    cred.ClientID = "c5a64d1a-ee90-4d57-b64e-6d31e7f51a8a";
                    cred.ClientSecert = "h4KUXZpUK2ZnpeIeU7OlSjAAcKo4G05Ewol8dHm+Ckg=";


                    CompositeObject obj = new CompositeObject();
                    obj.Credentials = cred;
                    obj.MailRequest = objtb;

                    string apiUrl = "https://hclmailapi.azurewebsites.net/api";

                    string inputJson = (new JavaScriptSerializer()).Serialize(obj);
                    System.Net.WebClient client = new WebClient();
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    string json = client.UploadString(apiUrl + "/email", inputJson);
                }
                catch (Exception ex)
                {
                    // Log exception
                }
            }
            
        }

        public static async Task SendEmailSendgrid(SendMailRequest objt, string templateCode)
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridAPIKey"].ToString();
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(objt.SenderEmailId, objt.SenderName);
            var subject = objt.Subject;
            var to = new EmailAddress(objt.To);
            var plainTextContent = "";
            var htmlContent = objt.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            if (templateCode == "EmployeeOnboardMail")
            {
                string attHCLLogo = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Email/HCL-Logo.png");
                string base64String = String.Empty;
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(attHCLLogo))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }                
                var attachmentHCLLogo = new Attachment();
                attachmentHCLLogo.Type = "image/png";
                attachmentHCLLogo.ContentId = "hcllogo";
                attachmentHCLLogo.Filename = "HCL-Logo.png";
                attachmentHCLLogo.Content = base64String;
                attachmentHCLLogo.Disposition = "inline";                
                msg.AddAttachment(attachmentHCLLogo);
                var clientlogo = ConfigurationManager.AppSettings["ClientLogo"].ToString();
                string attClientLogo = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Email/"+clientlogo);
                base64String = String.Empty;
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(attClientLogo))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }             
                var attachmentClientLogo = new Attachment();
                attachmentClientLogo.Type = "image/png";
                attachmentClientLogo.ContentId = "clientlogo";
                attachmentClientLogo.Filename = "OLIN-Logo.png";
                attachmentClientLogo.Content = base64String;
                attachmentClientLogo.Disposition = "inline";             
                msg.AddAttachment(attachmentClientLogo);
            }

            if (!string.IsNullOrEmpty(objt.Cc))
                msg.AddCc(objt.Cc);
            var response = await client.SendEmailAsync(msg);
        }
    }

    public class CompositeObject
    {
        public O365Credential Credentials { get; set; }
        public SendMailRequest MailRequest { get; set; }
    }

    public class O365Credential
    {

        public string ClientID { get; set; }
        public string ClientSecert { get; set; }

    }

    public class SendMailRequest
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string SenderEmailId { get; set; }
    }
}