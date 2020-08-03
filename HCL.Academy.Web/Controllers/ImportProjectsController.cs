using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ImportProjectsController : BaseController
    {
        class MultiReturnValue
        {
            public bool Result { get; set; }
            public StringBuilder LogText { get; set; }
            public MultiReturnValue()
            {
                LogText = new StringBuilder();
            }
        }

        [Authorize]
        [SessionExpire]
        public ActionResult ImportProjects()
        {
            return View();
        }
        /// <summary>
        /// Upload the Project Data present in the file selected.
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<JsonResult> UploadProjectDataFile(HttpPostedFileBase uploadedFile)
        {
          //  IDAL dal = (new DALFactory()).GetInstance();
            bool isProjectInserted = false;
            StringBuilder logText = new StringBuilder();
            MultiReturnValue mrv = new MultiReturnValue();
            InitializeServiceClient();
            try
            {
                logText.Append("<table border = '1'> <tr><th>Result</th></tr>");
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    #region Read file data
                    if ((uploadedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || uploadedFile.ContentType == "application/octet-stream") &&
                        (uploadedFile.FileName.EndsWith(".xls") || uploadedFile.FileName.EndsWith(".xlsx")))
                    {

                        string url = ConfigurationManager.AppSettings["URL"].ToString();
                        List<Skill> allSkills = null;
                        List<Competence> allCompetencies = null;
                        List<Project> allProjects = null;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                        allSkills = await response.Content.ReadAsAsync<List<Skill>>();
                            //objUserOnBoarding.Skills = skills;
                            //allSkills = dal.GetAllSkills();
                            //allCompetencies = dal.GetAllCompetenceList();
                        response = null;

                        response = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                        allCompetencies = await response.Content.ReadAsAsync<List<Competence>>();
                            //allProjects = dal.GetAllProjects();
                        response = null;
                        response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                        allProjects = await response.Content.ReadAsAsync<List<Project>>();
                        }
                    catch (Exception ex)
                    {
                        UserManager user = (UserManager)Session["CurrentUser"];
                      //  LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "ImportProject,UploadProjectDataFile", ex.Message, ex.StackTrace));
                    }

                        List<DataTable> listDataTable = new List<DataTable>();
                        ProjectData objProjectData = new ProjectData();
                        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(uploadedFile.InputStream, false))
                        {
                            Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                            var listOfSheets = Utilities.GetAllWorksheets(doc);
                            foreach (Sheet sheetItem in listOfSheets)
                            {
                                string sheetName = sheetItem.Name;
                                string sheetId = sheetItem.Id.Value;
                                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheetId) as WorksheetPart).Worksheet;
                                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                                DataTable dt = new DataTable();
                                int rowcount = rows.Count();

                                foreach (Row row in rows)
                                {
                                    if (row != null)
                                    {
                                        if (row.RowIndex.Value == 1)
                                        {
                                            foreach (Cell cell in row.Descendants<Cell>())
                                            {
                                                dt.Columns.Add(Utilities.GetSpreadsheetCellValue(doc, cell));
                                            }
                                        }
                                        else
                                        {
                                            dt.Rows.Add();
                                            int i = 0;
                                            foreach (Cell cell in row.Descendants<Cell>())
                                            {
                                                dt.Rows[dt.Rows.Count - 1][i] = Utilities.GetSpreadsheetCellValue(doc, cell);
                                                i++;
                                            }
                                        }
                                    }
                                }
                                listDataTable.Add(dt);
                            }
                        }

                        objProjectData.projects = new List<Project>();
                        Project objProject = null;
                        foreach (DataRow item in listDataTable[0].Rows)
                        {
                            objProject = new Project();
                            objProject.projectName = Convert.ToString(item.ItemArray[0]);
                            List<Project> itemProject = allProjects.Where(project => (project.projectName).ToLower() == (objProject.projectName).ToLower()).ToList();
                            if (itemProject != null && itemProject.Count() > 0)
                            {
                                string duplicateProject = itemProject.FirstOrDefault().projectName;
                                logText.Append("<tr><td class='error'>The Project <span class='bold'>" + duplicateProject + "</span>already present.<td><tr>");
                            }
                            else
                            {
                                objProjectData.projects.Add(objProject);
                            }

                        }

                        objProjectData.projectSkills = new List<ProjectSkill>();
                        ProjectSkill objProjectSkill = null;
                        logText.Append("<tr><td>&nbsp;</td></tr>");
                        foreach (DataRow item in listDataTable[1].Rows)
                        {
                            objProjectSkill = new ProjectSkill();
                            objProjectSkill.project = item.ItemArray[0] != null ? Convert.ToString(item.ItemArray[0]) : "";
                            objProjectSkill.skill = item.ItemArray[1] != null ? Convert.ToString(item.ItemArray[1]) : "";

                            List<Skill> objSkill = allSkills.Where(i => i.SkillName == objProjectSkill.skill).ToList();
                            objProjectSkill.skillId = objSkill != null && objSkill.Count() > 0 ? Convert.ToInt32(objSkill.FirstOrDefault().SkillId) : -1;
                            if (objProjectSkill.skillId == -1)
                            {
                                logText.Append("<tr><td class='error'>Project :<span class='bold'>" + objProjectSkill.project + "</span>Skill : <span class='bold'>" + objProjectSkill.skill + "</span> is not valid<td><tr>");
                            }
                            else
                            {
                                objProjectData.projectSkills.Add(objProjectSkill);
                            }
                        }

                        objProjectData.projectSkillResources = new List<ProjectSkillResource>();
                        ProjectSkillResource objProjectSkillResource = null;
                        logText.Append("<tr><td>&nbsp;</td></tr>");
                        foreach (DataRow item in listDataTable[2].Rows)
                        {
                            objProjectSkillResource = new ProjectSkillResource();
                            objProjectSkillResource.projectName = Convert.ToString(item.ItemArray[0]);
                            objProjectSkillResource.skill = Convert.ToString(item.ItemArray[1]);
                            objProjectSkillResource.competencyLevel = Convert.ToString(item.ItemArray[2]);
                            objProjectSkillResource.expectedResourceCount = Convert.ToInt32(item.ItemArray[3]);
                            objProjectSkillResource.availableResourceCount = Convert.ToInt32(item.ItemArray[4]);

                            List<Competence> itemSkillResource = allCompetencies.Where(i => i.SkillName == objProjectSkillResource.skill && i.CompetenceName.ToUpper() == objProjectSkillResource.competencyLevel.ToUpper()).ToList();
                            if (itemSkillResource != null && itemSkillResource.Count() > 0)
                            {
                                objProjectSkillResource.competencyLevelId = itemSkillResource != null && itemSkillResource.Count() > 0 ? itemSkillResource.FirstOrDefault().CompetenceId : -1;
                                objProjectSkillResource.skillId = itemSkillResource != null && itemSkillResource.Count() > 0 ? itemSkillResource.FirstOrDefault().SkillId : -1;
                                objProjectData.projectSkillResources.Add(objProjectSkillResource);
                            }
                            else
                            {
                                logText.Append("<tr><td class='error'>" +
                                                    "For Project : <span class='bold'>" + objProjectSkillResource.projectName + "</span>" +
                                                    "Skill : <span class='bold'>" + objProjectSkillResource.skill + "</span>" +
                                                    ", Competency Level :  <span class='bold'>" + objProjectSkillResource.competencyLevel + "</span>" +
                                                    " Combination doesn't exist. So it can't be added to ProjectSkillResource list<td><tr>"

                                               );
                            }
                        }
                        mrv = await InsertProjectFromExcel(
                            objProjectData.projects, objProjectData.projectSkills, objProjectData.projectSkillResources);
                        isProjectInserted = mrv.Result;
                        logText.Append(mrv.LogText);
                    }
                    else
                    {
                        logText.Append("<tr><td>Please upload correct file format<td><tr>");
                    }
                    #endregion Read file data

                }
                else
                {
                    logText.Append("<tr><td>Please upload correct file format<td><tr>");
                }

                logText.Append("</table>");
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ImportsProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return Json(new
            {
                statusCode = 200,
                status = isProjectInserted,
                message = logText.ToString(),
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Checks whether the Project Details were uploaded successfully.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="projectskills"></param>
        /// <param name="projectskillresources"></param>
        /// <param name="logText"></param>
        /// <returns></returns>
        //private bool InsertProjectFromExcel(List<Project> projects, List<ProjectSkill> projectskills, 
        //    List<ProjectSkillResource> projectskillresources, ref StringBuilder logText)
        private async Task<MultiReturnValue> InsertProjectFromExcel(List<Project> projects, List<ProjectSkill> projectskills,
            List<ProjectSkillResource> projectskillresources)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            InitializeServiceClient();
            //bool result = false;
            MultiReturnValue mrv = new MultiReturnValue();
            try
            {
                List<Project> allProjects = null;
                List<ProjectSkill> allProjectSkills = null;
                List<ProjectSkillResource> allProjectSkillResource = null;
                /*Code for Service*/
                HttpResponseMessage response = null;
                foreach (Project project in projects)
                {
                    //dal.AddProject(project.projectName);
                    response = await client.PostAsJsonAsync("Project/AddProject?projectName=" + project.projectName, req);
                    mrv.LogText.Append("<tr><td class='success'>The Project : <span class='bold'>" + project.projectName + "</span> added successfully to the list Projects.</td></tr>");
                }

                //allProjects = dal.GetAllProjects();     //List of all Projects
                response = await client.PostAsJsonAsync("Project/GetAllProjects", req);
                allProjects = await response.Content.ReadAsAsync<List<Project>>();
                
                //allProjectSkills = dal.GetAllProjectSkills();       //List of all Project Skills
                response = await client.PostAsJsonAsync("Project/GetAllProjectSkills", req);
                allProjectSkills = await response.Content.ReadAsAsync<List<ProjectSkill>>();

                mrv.LogText.Append("<tr><td>&nbsp;</td></tr>");
                foreach (ProjectSkill proskill in projectskills)
                {
                    List<ProjectSkill> objProSkill = allProjectSkills.Where(item => item.project == proskill.project && item.skill == proskill.skill).ToList();
                    if (objProSkill != null && objProSkill.Count > 0)
                    {
                        // project & skill combination already exists
                        mrv.LogText.Append("<tr><td class='error'>Project :<span class='bold'>" + objProSkill.FirstOrDefault().project + "</span>Skill : <span class='bold'>" + objProSkill.FirstOrDefault().skill + "</span> is already present in ProjectSkill list.<td><tr>");
                    }
                    else // add project skill
                    {
                        List<Project> project = allProjects.Where(item => item.projectName == proskill.project).ToList();
                        if (project != null && project.Count > 0)
                        {
                            //dal.AddProjectSkill(
                            //    allProjects.Where(item => item.projectName == proskill.project).FirstOrDefault().id, 
                            //    proskill.skillId);

                            //AddProjectSkill(RequestBase req, int ProjectID, int SkillID)
                            response = await client.PostAsJsonAsync("Project/AddProjectSkill?ProjectID=" + project[0].id + "&SkillID=" + proskill.skillId, req);
                            mrv.LogText.Append("<tr><td class='success'>Project : <span class='bold'>" + proskill.project + "</span> and Skill : <span class='bold'>" + proskill.skill + "</span> added successfully to the list ProjectSkills.</td></tr>");
                        }
                        else
                        {
                            // project doesn't exist in Project list
                            // So it can't be added to project skill
                            mrv.LogText.Append("<tr><td class='error'>Project :<span class='bold'>" + proskill.project + "</span>is not available in Project list so It can't be added to ProjectSkill<td><tr>");
                        }
                    }
                }

                //allProjectSkillResource = dal.GetAllProjectSkillResources();
                response = await client.PostAsJsonAsync("Project/GetAllProjectSkillResources", req);
                allProjectSkillResource = await response.Content.ReadAsAsync<List<ProjectSkillResource>>();

                response = null;
                response = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                List<Skill> allSkills = await response.Content.ReadAsAsync<List<Skill>>();

                mrv.LogText.Append("<tr><td>&nbsp;</td></tr>");
                ProjectSkillResourceRequest req1 = new ProjectSkillResourceRequest();
                foreach (ProjectSkillResource skillResource in projectskillresources)
                {
                    List<Project> project = allProjects.Where(item => item.projectName == skillResource.projectName).ToList();
                    List<Skill> skill = allSkills.Where(x => x.SkillName == skillResource.skill).ToList();

                    if (project != null && project.Count > 0)
                    {
                        int projectid = project != null && project.Count() > 0 ? project.FirstOrDefault().id : -1;
                        int skillid = skill != null && skill.Count() > 0 ? skill.FirstOrDefault().SkillId : -1;
                        List<ProjectSkillResource> objProSkillResource = allProjectSkillResource.Where(item => item.projectId == projectid && item.skillId == skillResource.skillId && item.competencyLevelId == skillResource.competencyLevelId).ToList();

                        
                        //List<ProjectSkillResource> objProSkillResource = await response.Content.ReadAsAsync<List<ProjectSkillResource>>();

                        if (objProSkillResource != null && objProSkillResource.Count() > 0 && objProSkillResource[0].expectedResourceCount != 0 && objProSkillResource[0].availableResourceCount != 0)
                        {
                            // Item already exist
                            // Can't be added again
                            mrv.LogText.Append("<tr><td class='error'>Project :<span class='bold'>" + skillResource.projectName + "</span> Skill : <span class='bold'>" + skillResource.skill + "</span>, Competency Level : <span class='bold'>" + skillResource.competencyLevel + "</span> already exists in ProjectSkillResource list.<td><tr>");
                        }
                        else
                        {
                            //dal.AddProjectSkillResource(allProjects.Where(item => item.projectName == skillResource.projectName).FirstOrDefault().id,skillResource);

                            req1.skillId = skillid;
                            req1.projectId = projectid;
                            req1.competencyLevelId = skillResource.competencyLevelId;
                            req1.availableResourceCount = skillResource.availableResourceCount;
                            req1.expectedResourceCount = skillResource.expectedResourceCount;
                            response = await client.PostAsJsonAsync("Project/AddProjectSkillResource", req1);

                            mrv.LogText.Append("<tr><td class='success'>Project : " + skillResource.projectName + " Skill : " + skillResource.skill + " and Competency Level : " + skillResource.competencyLevel + " added successfully to the list ProjectSkillResource. </td></tr>");
                        }
                    }
                    else
                    {
                        // Can't be added
                        // Project doesn't exist
                        mrv.LogText.Append("<tr><td class='error'>Project is not available for combination, Project :<span class='bold'>" + skillResource.projectName + "</span> Skill : <span class='bold'>" + skillResource.skill + "</span>, Competency Level : <span class='bold'>" + skillResource.competencyLevel + "</span>. So it can't be added to ProjectSkillResource<td><tr>");
                    }
                }
                mrv.Result = true;
                //result = true;
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ImportsProjectController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                mrv.Result = false;
            }
            //return result;
            return mrv;
        }
    }
}