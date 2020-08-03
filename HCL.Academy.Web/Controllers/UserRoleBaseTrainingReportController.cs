using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Controllers;
using HCL.Academy.Model;
using HCLAcademy.Util;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class UserRoleBaseTrainingReportController : BaseController
    {
        // GET: UserRoleBaseTrainingReport
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();

            try
            {
                HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
                ViewBag.Projects = projects;

                HttpResponseMessage rolesResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                List<Role> roles = await rolesResponse.Content.ReadAsAsync<List<Role>>();
                ViewBag.Roles = roles;

                client.Dispose();
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("UserRoleBaseTrainingReportController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
            }
            return View();
        }
        public async Task<PartialViewResult> GetTrainingsReport(string projectid, string roleid)
        {
            InitializeServiceClient();
            List<UserTraining> usertrainings = new List<UserTraining>();
            HttpResponseMessage response = await client.PostAsJsonAsync("Training/GetUserRoleBaseTrainingReport?projectid=" + projectid + "&roleid=" + roleid, req);
            usertrainings = await response.Content.ReadAsAsync<List<UserTraining>>();
            client.Dispose();
            return PartialView("UserRoleBaseTrainingReport", usertrainings);
        }
        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(string projectid, string roleid)
        {
            InitializeServiceClient();
            List<UserTraining> usertrainings = new List<UserTraining>();
            HttpResponseMessage response = await client.PostAsJsonAsync("Training/GetUserRoleBaseTrainingReport?projectid=" + projectid + "&roleid=" + roleid, req);
            usertrainings = await response.Content.ReadAsAsync<List<UserTraining>>();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            System.Drawing.Color color = System.Drawing.Color.Black;
            // workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            //workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            //workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);

            for (int k = 1; k <= 9; k++)
            {
                workSheet.Cells[1, k].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);
            }
            workSheet.Cells[1, 1].Value = "EmployeeName";
            workSheet.Cells[1, 2].Value = "EmployeeId";
            workSheet.Cells[1, 3].Value = "EmailAddress";
            workSheet.Cells[1, 4].Value = "Role";
            workSheet.Cells[1, 5].Value = "TrainingName";
            workSheet.Cells[1, 6].Value = "CompletionStatus";
            workSheet.Cells[1, 7].Value = "LastDayCompletion";
            workSheet.Cells[1, 8].Value = "CompletedDate";
            workSheet.Cells[1, 9].Value = "ProjectName";
            //workSheet.Cells[1, 10].Value = "Skill";

            for (int i = 0; i < usertrainings.Count; i++)
            {
                int j = 0;
                var item = usertrainings[i];
                workSheet.Cells[i + 2, j + 1].Value = item.Employee;
                workSheet.Cells[i + 2, j + 2].Value = item.EmployeeId;
                workSheet.Cells[i + 2, j + 3].Value = item.EmailId;
                workSheet.Cells[i + 2, j + 4].Value = item.Role;
                workSheet.Cells[i + 2, j + 5].Value = item.TrainingName;
                workSheet.Cells[i + 2, j + 6].Value = item.AdminApprovalStatus;
                workSheet.Cells[i + 2, j + 7].Value = item.LastDayCompletion;
                workSheet.Cells[i + 2, j + 8].Value = item.CompletedDate;
                workSheet.Cells[i + 2, j + 9].Value = item.ProjectName;
               // workSheet.Cells[i + 2, j + 10].Value = item.SkillName;
                workSheet.Row(i + 2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(i + 2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                //workSheet.Row(i + 2).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(i + 2).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                if (item.AdminApprovalStatus =="Approved")
                {
                    for (int k = 1; k <= 9; k++)
                    {
                        workSheet.Cells[i + 2, j + k].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[i + 2, j + k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    }
                }
                else
                {
                    DateTime dt = DateTime.ParseExact(item.LastDayCompletion, "dd/MM/yyyy", null);
                    if (DateTime.Now > dt)
                    {
                        for (int k = 1; k <= 9; k++)
                        {
                            workSheet.Cells[i + 2, j + k].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            workSheet.Cells[i + 2, j + k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                        }
                    }
                    else
                    {
                        for (int k = 1; k <= 9; k++)
                        {
                            workSheet.Cells[i + 2, j + k].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            workSheet.Cells[i + 2, j + k].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gold);
                        }
                    }

                }
            }
            //for (int x = 0; x < 9; x++)
            //{
            //    workSheet.Column(x + 1).Width = 30;
            //}
            workSheet.Column(1).Width = 30;
            workSheet.Column(1).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(2).Width = 15;
            workSheet.Column(2).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(3).Width = 30;
            workSheet.Column(3).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(4).Width = 10;
            workSheet.Column(4).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(5).Width = 10;
            workSheet.Column(5).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(6).Width = 30;
            workSheet.Column(6).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(7).Width = 15;
            workSheet.Column(7).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(8).Width = 20;
            workSheet.Column(8).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            workSheet.Column(9).Width = 20;
            workSheet.Column(9).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            //workSheet.Column(10).Width = 20;
            //workSheet.Column(10).Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_UserRoleBasedTrainingReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
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