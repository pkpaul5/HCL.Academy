 using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Configuration;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class ChecklistReportController : BaseController
    {
        // GET: ChecklistReport
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();

            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            objUserOnBoarding.Projects = projects;
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            objUserOnBoarding.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            objUserOnBoarding.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
            return View(objUserOnBoarding);
        }

        // GET: ChecklistReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        /// <summary>
        /// Fetches users of a particular status in a project
        /// </summary>
        /// <param name="status"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UserChecklistReport(int project, int roleId, int geoId,string option,string search)
        {
            InitializeServiceClient();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.IsExcelDownload = false;
            request.GEOId = geoId;
            request.RoleId = roleId;
            request.ProjectId = project;
            if(option==null)
            {
                option = "Name";
            }
            request.option = option;
            request.search = search;
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingChecklistReport", request);
            List<UserChecklistReportItem> lstUserChecklist = await response.Content.ReadAsAsync<List<UserChecklistReportItem>>();
            return PartialView("UserChecklistReport", lstUserChecklist);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="roleId"></param>
        /// <param name="geoId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> LastChecklistReport(int project, int roleId, int geoId,string option,string search)
        {
            InitializeServiceClient();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.IsExcelDownload = false;
            request.GEOId = geoId;
            request.RoleId = roleId;
            request.ProjectId = project;
            request.option = option;
            request.search = search;
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetLastChecklistReport", request);
            List<UserChecklistReportItem> lstUserChecklist = await response.Content.ReadAsAsync<List<UserChecklistReportItem>>();
            return PartialView("LastChecklistReport", lstUserChecklist);
        }

        /// <summary>
        /// Gets the report in Excel format. 
        /// </summary>        
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadChecklistReport(int project, int roleId, int geoId)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            InitializeServiceClient();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.IsExcelDownload = false;
            request.GEOId = geoId;
            request.RoleId = roleId;
            request.ProjectId = project;
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingChecklistReport", request);
            List<UserChecklistReportItem> lstUserChecklist = await response.Content.ReadAsAsync<List<UserChecklistReportItem>>();
            

            if (lstUserChecklist.Count > 0)
            {
                workSheet.Row(1).Height = 40;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                workSheet.Cells[1, 1].Value = "Name";
                workSheet.Cells[1, 2].Value = "EmployeeId";
                workSheet.Cells[1, 3].Value = "EmailAddress";
                workSheet.Cells[1, 4].Value = "CheckListItem";
                workSheet.Cells[1, 5].Value = "Completed";
                workSheet.Cells[1, 6].Value = "OnboardingDate";
                workSheet.Cells[1, 7].Value = "CompletionDate";

                workSheet.Column(1).Width = 28;
                workSheet.Column(2).Width = 28;
                workSheet.Column(3).Width = 28;
                workSheet.Column(4).Width = 28;

                for (int i = 0; i < lstUserChecklist.Count; i++)
                {

                    workSheet.Cells[i + 2, 1].Value = lstUserChecklist[i].Name;
                    workSheet.Cells[i + 2, 2].Value = lstUserChecklist[i].EmployeeID;
                    workSheet.Cells[i + 2, 3].Value = lstUserChecklist[i].EmailAddress;
                    workSheet.Cells[i + 2, 4].Value = lstUserChecklist[i].CheckListItem;
                    workSheet.Cells[i + 2, 5].Value = lstUserChecklist[i].CheckListStatus;
                    workSheet.Cells[i + 2, 6].Value = lstUserChecklist[i].OnboardingDate;
                    workSheet.Cells[i + 2, 7].Value = lstUserChecklist[i].CompletionDate;
                }


            }
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_ChecklistReport_" + DateTime.Now + ".xlsx";
            
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
            }

        }
        /// <summary>
        /// Gets the report in Excel format. 
        /// </summary>        
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadLastChecklistItemReport(int project, int roleId, int geoId)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            InitializeServiceClient();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.IsExcelDownload = false;
            request.GEOId = geoId;
            request.RoleId = roleId;
            request.ProjectId = project;
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetLastChecklistReport", request);
            List<UserChecklistReportItem> lstUserChecklist = await response.Content.ReadAsAsync<List<UserChecklistReportItem>>();


            if (lstUserChecklist.Count > 0)
            {
                workSheet.Row(1).Height = 40;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                workSheet.Cells[1, 1].Value = "Name";
                workSheet.Cells[1, 2].Value = "EmployeeId";
                workSheet.Cells[1, 3].Value = "EmailAddress";
                workSheet.Cells[1, 4].Value = "CheckListItem";            
                workSheet.Cells[1, 5].Value = "OnboardingDate";
                workSheet.Cells[1, 6].Value = "CompletionDate";

                workSheet.Column(1).Width = 28;
                workSheet.Column(2).Width = 28;
                workSheet.Column(3).Width = 28;
                workSheet.Column(4).Width = 28;

                for (int i = 0; i < lstUserChecklist.Count; i++)
                {

                    workSheet.Cells[i + 2, 1].Value = lstUserChecklist[i].Name;
                    workSheet.Cells[i + 2, 2].Value = lstUserChecklist[i].EmployeeID;
                    workSheet.Cells[i + 2, 3].Value = lstUserChecklist[i].EmailAddress;
                    workSheet.Cells[i + 2, 4].Value = lstUserChecklist[i].CheckListItem;                    
                    workSheet.Cells[i + 2, 5].Value = lstUserChecklist[i].OnboardingDate;
                    workSheet.Cells[i + 2, 6].Value = lstUserChecklist[i].CompletionDate;
                }


            }
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_LastChecklistItemReport_" + DateTime.Now + ".xlsx";
            
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
            }

        }

    }
}
