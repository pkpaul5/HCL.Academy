using System.Configuration;
using HCL.Academy.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class TrainingAssessmentStatusController : BaseController
    {
        // GET: TrainingAssessmentStatus
        public async Task<ActionResult> Index()
        {
            try
            {
                
                TrainingStatus objTrainingStatus = new TrainingStatus();
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                List<Skill> skills = new List<Skill>();
                Skill skAll = new Skill();
                skAll.SkillId = 0;
                skAll.SkillName = "All Skills";
                skills.Add(skAll);
                List<Skill> academySkills= await response.Content.ReadAsAsync<List<Skill>>();

                for(int i=0;i< academySkills.Count;i++)
                {
                    skills.Add(academySkills[i]);
                }
                objTrainingStatus.Skills = skills;
                HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
                objTrainingStatus.Projects = projects;
                return View(objTrainingStatus);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("TrainingAssessmentStatusController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }
        /// <summary>
        /// Get all the competences for a particular skill ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<JsonResult> FillTraining(int Id)
        {
            try
            {
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Training/GetTrainingBySkill?skillId=" + Id, req);
                List<TrainingMaster> trainings = await response.Content.ReadAsAsync<List<TrainingMaster>>();             
                JsonResult j = Json(trainings, JsonRequestBehavior.AllowGet);
                return j;
            }
            catch (Exception ex)
            {
                //  LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "Admin, OnBoardUser", ex.Message, ex.StackTrace));
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Gets the TrainingAssignment. 
        /// </summary>        
        /// <returns></returns>
        [Authorize]      
        [SessionExpire]
        public async Task<JsonResult> GetTrainingAssignment(int skillId,int trainingid,int projectid)
        {  
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingAssigments?skillId="+skillId+"&trainingId=" + trainingid + "&projectId=" + projectid, req);
            List<TrainingAssignment> trainingAssignments = await trainingResponse.Content.ReadAsAsync<List<TrainingAssignment>>();
            return new JsonResult { Data = trainingAssignments };

        }
        /// <summary>
        /// Gets the report in Excel format. 
        /// </summary>        
        /// <returns></returns>
        [Authorize]        
        [SessionExpire]
        public async Task<FileResult> DownloadTrainingAssignment(int skillId,int trainingid, int projectid)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingAssigments?skillId="+skillId+"&trainingId=" + trainingid + "&projectId=" + projectid, req);
            List<TrainingAssignment> trainingAssignments = await trainingResponse.Content.ReadAsAsync<List<TrainingAssignment>>();

            if (trainingAssignments.Count > 0)
            {
                workSheet.Row(1).Height = 40;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                workSheet.Cells[1, 1].Value = "Training Name";
                workSheet.Cells[1, 2].Value = "EmployeeId";
                workSheet.Cells[1, 3].Value = "EmailAddress";
                workSheet.Cells[1, 4].Value = "UserName";

                workSheet.Column(1).Width = 28;
                workSheet.Column(2).Width = 28;
                workSheet.Column(3).Width = 28;
                workSheet.Column(4).Width = 28;

                for (int i = 0; i < trainingAssignments.Count; i++)
                {
                    
                    workSheet.Cells[i+2, 1].Value = trainingAssignments[i].TrainingName;
                    workSheet.Cells[i + 2, 2].Value = trainingAssignments[i].EmployeeId;
                    workSheet.Cells[i + 2, 3].Value = trainingAssignments[i].EmailAddress;
                    workSheet.Cells[i +2, 4].Value = trainingAssignments[i].UserName;
                }
                
              
            }
                string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
                string excelName = clientName + "_TrainingAssignmentReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
           
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