using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Configuration;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class CandidateSkillMapController : BaseController
    {
        // GET: CandidateSkillMap
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();            
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            ViewBag.Projects = projects;            
            return View();
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
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            ViewBag.Projects = projects;
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            //request.Status = "OnBoarded";
            request.RoleId = 0;
            request.GEOId = 0;
            request.ProjectId =Convert.ToInt32(projectid);
            HttpResponseMessage userskillResponse = await client.PostAsJsonAsync("Onboarding/GetOnBoardingSkillReport", request);
            List<UserOnBoarding> skills = await userskillResponse.Content.ReadAsAsync<List<UserOnBoarding>>();
            return PartialView("CandidateSkillMapReport",skills);
        }
        /// <summary>
        /// Gets the report in Excel format.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(string projectId)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            
            InitializeServiceClient();

            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            List<Skill> lstSkills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.Status = "OnBoarded";
            request.RoleId = 0;
            request.GEOId = 0;
            request.ProjectId = Convert.ToInt32(projectId);
            HttpResponseMessage userskillResponse = await client.PostAsJsonAsync("Onboarding/GetOnBoardingSkillReport", request);
            List<UserOnBoarding> skills = await userskillResponse.Content.ReadAsAsync<List<UserOnBoarding>>();

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
            workSheet.Cells[1, 1].Value = "Name";
            int skillCount = lstSkills.Count;
            for(int i=0;i<skillCount;i++)
            {
                workSheet.Cells[1, i + 2].Value = lstSkills[i].SkillName;
                workSheet.Cells[1, i + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
            }
            int j = 0;            
            foreach (var item in skills)
            {
                int k = 0;
                while (j < skills.Count && k < skillCount)
                {
                    workSheet.Cells[j + 2, 1].Value = item.Name;
                    workSheet.Cells[j + 2, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                    foreach (UserSkill u in item.UserSkills)
                    {                      
                        workSheet.Cells[j + 2, k + 2].Value = u.Competence;                                                
                        string comp = u.Competence.ToUpper();
                        switch (comp)
                        {
                            case "NOVICE":
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType= ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                            case "ADVANCED BEGINNER":
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16765772));
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);                                
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                            case "COMPETENT":
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                            case "PROFICIENT":
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(14125913));
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                            case "EXPERT":
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(5031812));
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                            default:
                                workSheet.Cells[j + 2, k + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[j + 2, k + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                workSheet.Cells[j + 2, k + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                                break;
                        }
                        k++;
                    }
                    j++;
                }
            }
            for(int x=0;x<skillCount;x++)
            {
                workSheet.Column(x + 1).Width = 30;
            }
            
            
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            string excelName = clientName + "_CandidateSkillMapReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
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
        // GET: CandidateSkillMap/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CandidateSkillMap/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CandidateSkillMap/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CandidateSkillMap/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CandidateSkillMap/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CandidateSkillMap/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CandidateSkillMap/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
