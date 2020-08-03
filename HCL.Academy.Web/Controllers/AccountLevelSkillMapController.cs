using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using System.Net.Http;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Configuration;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class AccountLevelSkillMapController : BaseController
    {
        // GET: AccountLevelSkillMap
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            ViewBag.Projects = projects;
            
            //HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkillResourceCount", req);
            //List<SkillCompetencyResource> result = await skillResponse.Content.ReadAsAsync<List<SkillCompetencyResource>>();
            AccountSkillHeatMapViewModel vm = new AccountSkillHeatMapViewModel();
           // vm.competencies = new SelectList( new List<Object>{
           //          new { value = "1", text = "Novice"  },new { value = "2" , text = "AdvancedBeginner" },new { value = "3" , text = "Competent"},
           //          new { value = "4" , text = "Proficient"},new { value = "5" , text = "Expert"}},
           //              "value","text");
           //// vm.resources = result;
           // vm.competencyLevel = 1;
            return View(vm);
        }

        /// <summary>
        /// Displays the Project Details
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection f)
        {
            InitializeServiceClient();
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            ViewBag.Projects = projects;
            HCL.Academy.Model.AccountSkillHeatMapViewModel vm = new AccountSkillHeatMapViewModel();
            //string compvalue = f["competencyLevel"];
            //string[] parts = compvalue.Split(",".ToCharArray());
            //vm.competencyLevel = Convert.ToInt32(parts[0]);
            vm.projectId= Convert.ToInt32(f["project"].ToString());
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkillResourceCount?projectId=" + vm.projectId, req);
            List<SkillCompetencyResource> result = await skillResponse.Content.ReadAsAsync<List<SkillCompetencyResource>>();
            vm.resources = result;
            //vm.competencies = new SelectList(new List<Object>{
            //         new { value = "1", text = "Novice"  },new { value = "2" , text = "AdvancedBeginner" },new { value = "3" , text = "Competent"},
            //         new { value = "4" , text = "Proficient"},new { value = "5" , text = "Expert"}},
            //            "value", "text");
            return View(vm);
        }
        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(string projectid)
        {
            InitializeServiceClient();
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkillResourceCount?projectId=" + projectid, req);
            List<SkillCompetencyResource> result = await skillResponse.Content.ReadAsAsync<List<SkillCompetencyResource>>();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;

            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);
            workSheet.Row(1).Style.Font.Bold = true;
            System.Drawing.Color color = System.Drawing.Color.Black;
            workSheet.Cells[1, 1].Value = "Skill";
            workSheet.Cells[1, 2].Value = "Novice Count";
            workSheet.Cells[1, 3].Value = "Advanced Beginner Count";
            workSheet.Cells[1, 4].Value = "Competent Count";
            workSheet.Cells[1, 5].Value = "Proficient Count";
            workSheet.Cells[1, 6].Value = "Expert Count";
            for (int i = 0; i < result.Count; i++)
            {
                int j = 0;
                var item = result[i];
                workSheet.Cells[i + 2, j + 1].Value = item.Skill;
                workSheet.Cells[i + 2, j + 2].Value = item.NoviceCount;
                workSheet.Cells[i + 2, j + 3].Value = item.AdvancedBeginnerCount;
                workSheet.Cells[i + 2, j + 4].Value = item.CompetentCount;
                workSheet.Cells[i + 2, j + 5].Value = item.ProficientCount;
                workSheet.Cells[i + 2, j + 6].Value = item.ExpertCount;
            }
            for (int x = 0; x < 6; x++)
            {
                workSheet.Column(x + 1).Width = 30;
            }
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_AccountSkillMapReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
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
        /// Fetches the related Training Report for the selected Skill and Competency
        /// </summary>
        /// <param name="skillid"></param>
        /// <param name="competencyid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<PartialViewResult> GetReport(string projectid)
        {
            InitializeServiceClient();
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkillResourceCount?projectId="+ projectid, req);
            List<SkillCompetencyResource> result = await skillResponse.Content.ReadAsAsync<List<SkillCompetencyResource>>();
            AccountSkillHeatMapViewModel vm = new AccountSkillHeatMapViewModel();            
            vm.resources = result;
            //int cid = 0;
            //if (competencylevelid != null)
            //{
            //    cid = Convert.ToInt32(competencylevelid);
            //}
            //else
            //    cid = 1;
            //vm.competencyLevel = cid;
            vm.projectId =Convert.ToInt32(projectid);
            //vm.competencies = new SelectList(new List<Object>{
            //         new { value = "1", text = "Novice"  },new { value = "2" , text = "AdvancedBeginner" },new { value = "3" , text = "Competent"},
            //         new { value = "4" , text = "Proficient"},new { value = "5" , text = "Expert"}},
            //          "value", "text");
            return PartialView("Report", vm);
        }
    }
}
