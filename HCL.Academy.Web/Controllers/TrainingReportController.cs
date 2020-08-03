using System.Configuration;
using HCL.Academy.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using System.IO;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class TrainingReportController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> TrainingReport()
        {
            InitializeServiceClient();
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();

            List<Skill> skills = new List<Skill>();
            Skill skAll = new Skill();
            skAll.SkillId = 0;
            skAll.SkillName = "All Skills";
            skills.Add(skAll);

            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            List<Skill> academySkills = await response.Content.ReadAsAsync<List<Skill>>();
            //objUserOnBoarding.Skills = await response.Content.ReadAsAsync<List<Skill>>();
            for (int i = 0; i < academySkills.Count; i++)
            {
                skills.Add(academySkills[i]);
            }
            objUserOnBoarding.Skills = skills;
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            objUserOnBoarding.Projects = projects;
            return View(objUserOnBoarding);
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
        public async Task<JsonResult> GetTrainingsReport(string skillid, string competencyid,string projectid)
        {   
            InitializeServiceClient();
            UserTrainingsRequest preRquisiteInfo = new UserTrainingsRequest();
            preRquisiteInfo.ClientInfo = req.ClientInfo;
            preRquisiteInfo.SkillId = Convert.ToInt32(skillid);
            preRquisiteInfo.ProjectId = Convert.ToInt32(projectid);
            preRquisiteInfo.CompetenceId = Convert.ToInt32(competencyid);
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingsReport", preRquisiteInfo);
            TrainingReport usersList = await trainingResponse.Content.ReadAsAsync<TrainingReport>();          
            return new JsonResult { Data = usersList };
        }

        /// <summary>
        /// Gets the report in Excel format. 
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="competency"></param>
        /// <returns></returns>
        [Authorize]       
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(string skill, string competency,string projectId)
        {   
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            InitializeServiceClient();
            UserTrainingsRequest trainingPreReq = new UserTrainingsRequest();
            trainingPreReq.ClientInfo = req.ClientInfo;
            trainingPreReq.CompetenceId = Convert.ToInt32(competency);
            trainingPreReq.SkillId = Convert.ToInt32(skill);
            trainingPreReq.ProjectId = Convert.ToInt32(projectId);
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainings", trainingPreReq);
            List<Training> SkillBasedTrainings = await trainingResponse.Content.ReadAsAsync<List<Training>>();
            
            int k = 3;
            try
            {
                if (SkillBasedTrainings.Count > 0)
                {
                    workSheet.Row(1).Height = 40;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    workSheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(3).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Cells[1, 1].Value = "Training Name";
                    workSheet.Cells[2, 1].Value = "Training Completed";
                    workSheet.Cells[2, 1].Style.Font.Bold = true;
                    workSheet.Cells[3, 1].Value = "Training Not Completed";
                    workSheet.Cells[3, 1].Style.Font.Bold = true;
                    workSheet.Column(1).Width = 28;

                    for (int i = 0; i < SkillBasedTrainings.Count; i++)
                    {
                        workSheet.Column(i + 2).Width = 50;
                        workSheet.Column(i + 2).Style.WrapText = true;
                        workSheet.Cells[1, i + 2].Value = SkillBasedTrainings[i].TrainingName;
                        k = 2;
                        InitializeServiceClient();
                        HttpResponseMessage userResponse = await client.PostAsJsonAsync("Training/GetUserTrainingsByTrainingID?trainingId=" + SkillBasedTrainings[i].TrainingId+"&projectId="+ projectId, req);
                        List<UserTraining> userTrainings = await userResponse.Content.ReadAsAsync<List<UserTraining>>();                        

                        if (userTrainings != null & userTrainings.Count > 0)
                        {
                            StringBuilder completedUserList = new StringBuilder();
                            StringBuilder wipUserList = new StringBuilder();

                            foreach (UserTraining userTraining in userTrainings)
                            {
                                if (userTraining.IsTrainingCompleted)
                                {
                                    completedUserList.Append(userTraining.Employee);
                                    completedUserList.Append(";");
                                }
                                else
                                {
                                    wipUserList.Append(userTraining.Employee);
                                    wipUserList.Append(";");
                                }
                            }
                            workSheet.Cells[k, i + 2].Value = completedUserList.ToString();
                            k = k + 1;
                            workSheet.Cells[k, i + 2].Value = wipUserList.ToString();
                        }
                    }
                }
                ////////////Get Assessments for the skill and competency level//////////
                SkillwiseAssessmentsRequest request = new SkillwiseAssessmentsRequest();
                request.ClientInfo = req.ClientInfo;
                request.SkillId = Convert.ToInt32(skill);
                request.CompetenceId = Convert.ToInt32(competency);

                HttpResponseMessage response = await client.PostAsJsonAsync("Assessment/GetAssessments", request);
                List<Assessment> SkillBasedAssessments = await response.Content.ReadAsAsync<List<Assessment>>();

                if (SkillBasedAssessments.Count > 0)
                {
                    k = k + 3;
                    workSheet.Row(k).Height = 40;
                    workSheet.Row(k).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(k).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Row(k).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    workSheet.Row(k).Style.Font.Bold = true;
                    workSheet.Row(k).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(k + 1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(k + 2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Cells[k, 1].Value = "Assessment Name";
                    workSheet.Cells[k + 1, 1].Value = "Assessment Completed";
                    workSheet.Cells[k + 1, 1].Style.Font.Bold = true;
                    workSheet.Cells[k + 2, 1].Value = "Assessment Not Completed";
                    workSheet.Cells[k + 2, 1].Style.Font.Bold = true;

                    for (int i = 0; i < SkillBasedAssessments.Count; i++)
                    {
                        workSheet.Cells[k, i + 2].Value = SkillBasedAssessments[i].AssessmentName;
                        HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/GetUserAssessmentsByAssessmentId?assessmentId=" + SkillBasedAssessments[i].AssessmentId+"&projectId="+projectId, request);
                        List<UserAssessment> userAssessments = await assessmentResponse.Content.ReadAsAsync<List<UserAssessment>>();

                        string userlist = String.Empty;
                        if (userAssessments != null & userAssessments.Count > 0)
                        {
                            StringBuilder completedAssmtUserList = new StringBuilder();
                            StringBuilder wipAssmtUserList = new StringBuilder();
                            foreach (UserAssessment userAssessment in userAssessments)
                            {
                                if (userAssessment.IsAssessmentComplete)
                                {
                                    completedAssmtUserList.Append(userAssessment.Employee);
                                    completedAssmtUserList.Append(";");
                                }
                                else
                                {
                                    wipAssmtUserList.Append(userAssessment.Employee);
                                    wipAssmtUserList.Append(";");
                                }
                            }
                            workSheet.Cells[k + 1, i + 2].Value = completedAssmtUserList.ToString();
                            workSheet.Cells[k + 2, i + 2].Value = wipAssmtUserList.ToString();
                        }
                    }
                }
                string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
                string excelName = clientName+ "_TrainingReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
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
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingReportController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return null;
            }
        }
    }
}