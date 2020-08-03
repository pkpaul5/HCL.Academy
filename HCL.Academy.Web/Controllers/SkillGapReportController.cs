using HCL.Academy.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class SkillGapReportController : BaseController
    {
        // GET: SkillGapReport
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            SkillGapReport gapReport = new SkillGapReport();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("User/GetRolesWithSkills", req);
            List<Role> roles = await trainingResponse.Content.ReadAsAsync<List<Role>>();
            ViewBag.Roles = roles;
            return View();
        }

        public async Task<PartialViewResult> GetSkillGapReport(int roleID)
        {
            InitializeServiceClient();
            SkillGapReport gapReport = new SkillGapReport();
            gapReport.RoleId = roleID;
            List<SkillGapReport> reports = new List<SkillGapReport>();
            HttpResponseMessage reportResponse = await client.PostAsJsonAsync("User/GetSkillGapReports?roleID=" + roleID, req);
            reports = await reportResponse.Content.ReadAsAsync<List<SkillGapReport>>();
            return PartialView("SkillGapReport", reports);
        }

        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(int roleID)
        {
            InitializeServiceClient();
            SkillGapReport gapReport = new SkillGapReport();
            gapReport.RoleId = roleID;
            List<SkillGapReport> reports = new List<SkillGapReport>();
            HttpResponseMessage reportResponse = await client.PostAsJsonAsync("User/GetSkillGapReports?roleID=" + roleID, req);
            reports = await reportResponse.Content.ReadAsAsync<List<SkillGapReport>>();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            System.Drawing.Color color = System.Drawing.Color.Black;

            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);

            for (int k = 1; k <= 6; k++)
            {
                workSheet.Cells[1, k].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);
            }
            workSheet.Cells[1, 1].Value = "EmployeeName";
            workSheet.Cells[1, 2].Value = "EmployeeId";
            workSheet.Cells[1, 3].Value = "EmailID";
            workSheet.Cells[1, 4].Value = "Skill";
            workSheet.Cells[1, 5].Value = "Expected Competency";
            workSheet.Cells[1, 6].Value = "Actual Competency";

            for (int i = 0; i < reports.Count; i++)
            {
                int j = 0;
                var item = reports[i];
                workSheet.Cells[i + 2, j + 1].Value = item.EmployeeName;
                workSheet.Cells[i + 2, j + 2].Value = item.EmployeeID;
                workSheet.Cells[i + 2, j + 3].Value = item.EmailID;
                workSheet.Cells[i + 2, j + 4].Value = item.Skill;
                workSheet.Cells[i + 2, j + 5].Value = item.ExpectedCompetencyLevel;
                workSheet.Cells[i + 2, j + 6].Value = item.ActualCompetencyLevel;

                workSheet.Row(i + 2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(i + 2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(i + 2).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);

            }
            
            workSheet.Column(1).Width = 30;
            workSheet.Column(1).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(2).Width = 15;
            workSheet.Column(2).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(3).Width = 30;
            workSheet.Column(3).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(4).Width = 10;
            workSheet.Column(4).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(5).Width = 40;
            workSheet.Column(5).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(6).Width = 30;
            workSheet.Column(6).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);

            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_SkillGapReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
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
