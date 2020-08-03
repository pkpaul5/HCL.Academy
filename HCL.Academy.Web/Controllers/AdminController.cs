using ExcelDataReader;
using HCLAcademy.Util;
using HCLAcademy.CustomFilters;
using HCL.Academy.Model;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCLAcademy.Controllers
{
    [CustomAuthorizationFilter]
    public class AdminController : BaseController
    {
        private ApplicationUserManager _userManager;
        #region PUBLIC METHODS

        // GET: Admin
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> OnBoardAdmin()
        {
            InitializeServiceClient();
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();

            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
            List<Skill> skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
            objUserOnBoarding.Skills = skills;

            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetAllProjects", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            objUserOnBoarding.Projects = projects;

            ViewBag.Skills = skills;

            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
            ViewBag.Competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();


            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            List<GEO> geos = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            objUserOnBoarding.GEOs = geos;

            HttpResponseMessage rolesResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            List<Role> roles = await rolesResponse.Content.ReadAsAsync<List<Role>>();
            objUserOnBoarding.Roles = roles;

            return View(objUserOnBoarding);
        }
        /// <summary>
        /// Upload the selected file of users
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [HttpPost]
        public async Task<JsonResult> UploadOnboardFile(HttpPostedFileBase uploadedFile)
        {
            InitializeServiceClient();
            //IDAL dal = (new DALFactory()).GetInstance();
            //SharePointDAL spDal = new SharePointDAL();
            SPAuthUtility util = new SPAuthUtility();

            StringBuilder logText = new StringBuilder();
            logText.Append("<table border = '1'> <tr><th>Result</th></tr>");

            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                #region Read file data
                if ((uploadedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || uploadedFile.ContentType == "application/octet-stream") &&
                    (uploadedFile.FileName.EndsWith(".xls") || uploadedFile.FileName.EndsWith(".xlsx")))
                {
                    //ExcelDataReader works on binary excel file
                    Stream stream = uploadedFile.InputStream;

                    IExcelDataReader reader = null;
                    if (uploadedFile.FileName.EndsWith(".xls"))
                    {
                        //reads the excel file with .xls extension
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else //if (uploadedFile.FileName.EndsWith(".xlsx"))
                    {
                        //reads excel file with .xlsx extension
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    //treats the first row of excel file as Coluymn Names
                    //   reader.IsFirstRowAsColumnNames = true;
                    //Adding reader data to DataSet()
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    reader.Close();

                    DataTable dt = result.Tables[0];

                    dt = dt.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();

                    int i = 0;
                    int j = 0;

                    //List<Skill> allSkill = dal.GetAllSkills();

                    HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                    List<Skill> allSkill = await skillResponse.Content.ReadAsAsync<List<Skill>>();


                    //List<Competence> allCompetence = dal.GetAllCompetenceList();
                    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                    List<Competence> allCompetence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                    HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);

                    //List<GEO> allGEO = dal.GetAllGEOs();
                    List<GEO> allGEO = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                    //List<Role> allRole = dal.GetAllRoles();

                    HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                    List<Role> allRole = await roleResponse.Content.ReadAsAsync<List<Role>>();


                    int length = dt.Rows.Count;

                    foreach (DataRow item in dt.Rows)
                    {
                        #region Parse XLS File Data

                        UserManager onboardedUser = null;
                        string emailId = Convert.ToString(dt.Rows[i][0]);
                        string skillName = Convert.ToString(dt.Rows[i][1]);
                        string competence = Convert.ToString(dt.Rows[i][2]);
                        string geo = Convert.ToString(dt.Rows[i][3]);
                        string role = Convert.ToString(dt.Rows[i][4]);

                        int skillId = 0;

                        if (allSkill.Exists(s => s.SkillName == skillName))
                        {
                            skillId = allSkill.Where(x => x.SkillName == skillName).FirstOrDefault().SkillId;

                            if (allCompetence.Exists(c => c.CompetenceName == competence && c.SkillId == skillId))
                            {
                                int competenceId = allCompetence.Where(c => c.CompetenceName == competence && c.SkillId == skillId).FirstOrDefault().CompetenceId;

                                if (allGEO.Exists(s => s.Title == geo))
                                {
                                    int geoId = allGEO.Where(x => x.Title == geo).FirstOrDefault().Id;

                                    if (allRole.Exists(r => r.Title == role))
                                    {
                                        int roleId = allRole.Where(x => x.Title == role).FirstOrDefault().Id;

                                        if (!string.IsNullOrWhiteSpace(emailId))
                                        {
                                            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetBoardingData", req);
                                            List<OnBoarding> listOnboarding = await response.Content.ReadAsAsync<List<OnBoarding>>();

                                            //   HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUserByEmail", req);
                                            //  onboardedUser = await userResponse.Content.ReadAsAsync<UserManager>();


                                            onboardedUser = util.GetUserByEmail(emailId);

                                            // onboardedUser = spDal.GetUserByEmail(emailId);
                                        }

                                        #region Valid user
                                        if (onboardedUser != null)
                                        {
                                            // Search user in AcademyOnboarding list
                                            //UserOnBoarding userOnboarding = dal.GetOnBoardingDetailsForUser(onboardedUser);
                                            RequestBase reqOnboardedUser = new RequestBase();
                                            reqOnboardedUser.ClientInfo = new ServiceConsumerInfo();
                                            reqOnboardedUser.ClientInfo.emailId = onboardedUser.EmailID;
                                            reqOnboardedUser.ClientInfo.Groups = onboardedUser.Groups;
                                            reqOnboardedUser.ClientInfo.id = onboardedUser.DBUserId;
                                            reqOnboardedUser.ClientInfo.name = onboardedUser.UserName;
                                            reqOnboardedUser.ClientInfo.spCredential = onboardedUser.SPCredential;
                                            reqOnboardedUser.ClientInfo.spUserId = onboardedUser.SPUserId;

                                            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsForUser", reqOnboardedUser);
                                            UserOnBoarding userOnboarding = await response.Content.ReadAsAsync<UserOnBoarding>();

                                            //if exist in Onboarding list
                                            if (!userOnboarding.IsPresentInOnBoard)
                                            {
                                                // Add user in ING Member group
                                                int employeeId = 0;
                                                if (util.AddUserToGroup(user.EmailID, ref employeeId))
                                                {
                                                    try
                                                    {
                                                        // Add User in AcademyOnboarding list 

                                                        string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();

                                                        int userId = 0;
                                                        if (dataStore == DataStore.SQLServer)
                                                        {
                                                            // dbUserId = sqlDAL.GetUserId(user.EmailID);
                                                            //userId = onboardedUser.DBUserId;
                                                            //SqlSvrDAL sqlDAL = new SqlSvrDAL();

                                                            HttpResponseMessage responseID = await client.PostAsJsonAsync("User/GetUserId?emailId=" + onboardedUser.EmailID, req);
                                                            userId = await responseID.Content.ReadAsAsync<int>();

                                                            //sqlDAL.OnBoardUser(competenceId.ToString(), skillId, user.SPUserId, geoId.ToString(), roleId, user.EmailID, user.UserName, employeeId);
                                                            //sqlDAL.OnboardEmail(user.EmailID, dbUserId, user.UserName);                                            
                                                        }
                                                        else
                                                        {
                                                            userId = onboardedUser.SPUserId;
                                                            //dal.OnBoardUser(competenceId.ToString(), skillId, user.SPUserId, geoId.ToString(), roleId, null, null, employeeId);
                                                            //dal.OnboardEmail(user.EmailID, user.SPUserId, user.UserName);
                                                        }
                                                        UserOnboardingRequest userInfo = new UserOnboardingRequest();
                                                        userInfo.ClientInfo = req.ClientInfo;
                                                        userInfo.CompetenceId = competenceId;
                                                        userInfo.EmailId = onboardedUser.EmailID;
                                                        userInfo.EmployeeId = onboardedUser.EmployeeId;
                                                        userInfo.Name = onboardedUser.UserName;
                                                        userInfo.SkillId = skillId;
                                                        userInfo.GeoId = geoId;
                                                        userInfo.RoleId = roleId;
                                                        HttpResponseMessage responseOnboarding = await client.PostAsJsonAsync("Onboarding/OnBoardUser", userInfo);
                                                        bool onboardingResult = await responseOnboarding.Content.ReadAsAsync<bool>();

                                                        OnboardEmailRequest emailRequest = new OnboardEmailRequest();
                                                        emailRequest.ClientInfo = req.ClientInfo;
                                                        emailRequest.Email = onboardedUser.EmailID;
                                                        emailRequest.UserId = userId;
                                                        emailRequest.UserName = onboardedUser.UserName;
                                                        HttpResponseMessage emailResponse = await client.PostAsJsonAsync("Onboarding/OnboardEmail", emailRequest);
                                                        bool emailResult = await emailResponse.Content.ReadAsAsync<bool>();


                                                        j = i + 1;
                                                        logText.Append("<tr><td>Member with email id " + emailId + "successfully onboarded<td><tr>");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        //UserManager loggeduser = (UserManager)Session["CurrentUser"];
                                                     //   LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "Admin, UploadOnboardFile", ex.Message, ex.StackTrace));
                                                        j = i + 1;
                                                        logText.Append("<tr><td>Exception occured while boarding user " + emailId + " at row no " + j + " Error: " + ex.Message + "<td><tr>");
                                                    }
                                                }
                                                else
                                                {
                                                    j = i + 1;
                                                    logText.Append("<tr><td>Error while adding User to the member group with email id " + emailId + " at row no " + j + "<td><tr>");
                                                }
                                            }
                                            else
                                            {
                                                j = i + 1;
                                                logText.Append("<tr><td>User already onboarded with email id " + emailId + " at row no " + j + "<td><tr>");
                                            }
                                        }
                                        else
                                        {
                                            j = i + 1;
                                            logText.Append("<tr><td>Invalid Email id " + emailId + " at row " + j + "<td><tr>");
                                        }
                                    }
                                }
                                #endregion Valid user
                            }
                            else
                            {
                                j = i + 1;
                                logText.Append("<tr><td>Invalid Competence " + emailId + " at row no " + j + "<td><tr>");
                            }
                        }
                        else
                        {
                            j = i + 1;
                            logText.Append("<tr><td>Invalid Skill " + emailId + " at row no " + j + "<td><tr>");
                        }

                        i++;

                        #endregion XLS file Parsing completed
                    }
                }
                #endregion Read file data
                else
                {
                    logText.Append("<tr><td>Please upload correct file format<td><tr>");
                }
            }
            else
            {
                logText.Append("<tr><td>Please upload correct file format<td><tr>");
            }

            logText.Append("</table>");

            return Json(new
            {
                statusCode = 200,
                status = true,
                message = logText.ToString(),
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Gets user details by seraching for a keyword in Emails
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public async Task<JsonResult> EmailSearch(string keyword)
        {
            UserManager onboardedUser = null;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ApplicationUser user = await IdentityUserManager.FindByEmailAsync(keyword);
                if (user != null)
                {
                    IdentityResult result = await IdentityUserManager.DeleteAsync(user);
                    UserManager ExtUser = new UserManager();
                    ExtUser.IsExternalUser = true;
                    ExtUser.EmailID = keyword;
                    ExtUser.UserName = user.UserName;
                    onboardedUser = ExtUser;
                }
                else
                {
                    SPAuthUtility util = new SPAuthUtility();
                    onboardedUser = util.GetUserByEmail(keyword);
                }
                                   
            }
            return Json(onboardedUser);
        }

        public ApplicationUserManager IdentityUserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /// <summary>
        /// Onboarding process for a particular user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UserOnBoarding(UserManager onboardedUser)
        {
            InitializeServiceClient();

            //IDAL dal = (new DALFactory()).GetInstance();
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();
            try
            {
                if (user != null)
                {
                    //objUserOnBoarding = dal.GetOnBoardingDetailsForUser(onboardedUser);      //Gets onboarding details

                    RequestBase reqOnboardedUser = new RequestBase();
                    reqOnboardedUser.ClientInfo = new ServiceConsumerInfo();
                    reqOnboardedUser.ClientInfo.emailId = onboardedUser.EmailID;
                    reqOnboardedUser.ClientInfo.Groups = onboardedUser.Groups;
                    reqOnboardedUser.ClientInfo.id = onboardedUser.DBUserId;
                    reqOnboardedUser.ClientInfo.name = onboardedUser.UserName;
                    reqOnboardedUser.ClientInfo.spCredential = onboardedUser.SPCredential;
                    reqOnboardedUser.ClientInfo.spUserId = onboardedUser.SPUserId;

                    HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsForUser", reqOnboardedUser);
                    objUserOnBoarding = await response.Content.ReadAsAsync<UserOnBoarding>();

                    // objUserOnBoarding.Skills = dal.GetAllSkills();                  //List of all Skills
                    HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                    objUserOnBoarding.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
                    ViewBag.Skills = objUserOnBoarding.Skills;
                    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                    objUserOnBoarding.Competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>(); //List of copetency levels
                    //objUserOnBoarding.Competence = dal.GetAllCompetenceList();
                    objUserOnBoarding.Status = Utilities.GetAllStatus();            //List of statuses
                    objUserOnBoarding.BGVStatus = Utilities.GetAllBGVStatus();      //Get the background verification status
                    objUserOnBoarding.ProfileSharingStatus = Utilities.GetAllProfileSharingStatus();    //Gets Profile sharing status
                    //objUserOnBoarding.GEOs = dal.GetAllGEOs();                     //List of all GEOs
                    //objUserOnBoarding.Roles = dal.GetAllRoles();                      //List of all Roles
                    HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                    objUserOnBoarding.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                    HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                    objUserOnBoarding.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();

                }
                else
                {
                    objUserOnBoarding = null;
                }

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return PartialView("_UserOnBoarding", objUserOnBoarding);
        }
        /// <summary>
        /// Gets all the Trainings and Assessments assigned to a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UserTrainingAndAssessment(UserManager user)
        {
            InitializeServiceClient();
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();
            try
            {
                // IDAL dal = (new DALFactory()).GetInstance();

                if (user != null)
                {
                    //objUserOnBoarding.Skills = dal.GetAllSkills();

                    HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                    objUserOnBoarding.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();
                    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                    objUserOnBoarding.Competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                    //objUserOnBoarding.Competence = dal.GetAllCompetenceList();

                    string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                    int userId = 0;

                    InitializeServiceClient();
                    if (dataStore.Equals(DataStore.SQLServer, StringComparison.InvariantCultureIgnoreCase))
                        userId = user.DBUserId;
                    else
                        userId = user.SPUserId;

                    HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingForUser?userId=" + userId, req);
                    objUserOnBoarding.UserTrainings = await trainingResponse.Content.ReadAsAsync<List<UserTraining>>();

                    UserwiseAssessmentsRequest request = new UserwiseAssessmentsRequest();
                    request.ClientInfo = req.ClientInfo;
                    request.Userid = userId;
                    request.OnlyOnBoardedTraining = false;

                    HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/GetAssessmentForUser", request);
                    objUserOnBoarding.UserAssessments = await assessmentResponse.Content.ReadAsAsync<List<UserAssessment>>();

                    //string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                    //if(dataStore == DataStore.SQLServer)
                    //{
                    //    objUserOnBoarding.UserTrainings = dal.GetTrainingForUser(user.DBUserId);    
                    //    objUserOnBoarding.UserAssessments = dal.GetAssessmentForUser(user.DBUserId);
                    //}
                    //else
                    //{
                    //    objUserOnBoarding.UserTrainings = dal.GetTrainingForUser(user.SPUserId);
                    //    objUserOnBoarding.UserAssessments = dal.GetAssessmentForUser(user.SPUserId);
                    //}
                }
                else
                {
                    objUserOnBoarding = null;
                }

            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("_UserTrainingAndAssessment", objUserOnBoarding);
        }
        /// <summary>
        /// Adds a skill to a particular user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="skillId"></param>
        /// <param name="competence"></param>
        /// <param name="ismandatory"></param>
        /// <param name="lastdayofcompletion"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<bool> AddSkill(string email, string userId, string skillId, string skillName, 
            string competenceid, string competence, bool ismandatory, DateTime lastdayofcompletion,string roleId)
        {
         
            InitializeServiceClient();
            bool flag = false;
            try
            {
                SkillAssignmentRequest skillReq = new SkillAssignmentRequest();
                skillReq.ClientInfo = req.ClientInfo;
                skillReq.CompetenceId = Convert.ToInt32(competenceid);
                skillReq.IsMandatory = ismandatory;
                skillReq.LastDayCompletion = lastdayofcompletion.ToString();
                skillReq.SkillId = Convert.ToInt32(skillId);
                skillReq.UserId = Convert.ToInt32(userId);
                skillReq.EmailAddress = email;
                skillReq.RoleId =Convert.ToInt32(roleId);
                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/AddSkill", skillReq);
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainingForUser?UserId=" + userId.ToString(), req);
                List<UserTraining> trainings = await trainingResponse.Content.ReadAsAsync<List<UserTraining>>();
                foreach (UserTraining item in trainings)
                {
                    if (item.SkillName == skillName)
                    {
                        OnboardEmailRequest emailRequest = new OnboardEmailRequest();
                        emailRequest.ClientInfo = req.ClientInfo;
                        emailRequest.Email = email;
                        emailRequest.UserId = Convert.ToInt32(userId);
                        //   emailRequest.UserName = user.UserName;
                        emailRequest.Skill = skillName;
                        emailRequest.SkillId = Convert.ToInt32(skillId);
                        emailRequest.CompetencyLevel = competence;
                        HttpResponseMessage emailResponse = await client.PostAsJsonAsync("Onboarding/AddSkillEmail", emailRequest);
                        bool emailResult = await emailResponse.Content.ReadAsAsync<bool>();
                        break;
                    }

                }

                flag = await response.Content.ReadAsAsync<bool>();

                #region KalibreAPI
                var KalibreClient = new HttpClient();
                string serviceBaseURL = ConfigurationManager.AppSettings["KalibreAPIBaseAddress"].ToString();
                string authCode = ConfigurationManager.AppSettings["KalibreAuthCode"].ToString();
                KalibreClient.BaseAddress = new Uri(serviceBaseURL);
                KalibreClient.DefaultRequestHeaders.Accept.Clear();
                KalibreClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string kalibreEndpoint = serviceBaseURL + "assignAssessments?authToken=" + authCode;

                KalibreAssmntAssgmnt kalibreassmntassgmnt = new KalibreAssmntAssgmnt();
                string kalibreAccountCode = ConfigurationManager.AppSettings["KalibreAccountCode"].ToString();
                KalibreAssignment kalibreassignment = new KalibreAssignment();
                kalibreassignment.technology = skillName;
                kalibreassignment.skillLevel = competence.ToUpper();
                kalibreassmntassgmnt.accountCode = kalibreAccountCode;
                kalibreassmntassgmnt.emailID = email;

                kalibreassmntassgmnt.assessments = new List<KalibreAssignment>();

                kalibreassmntassgmnt.assessments.Add(kalibreassignment);

                response = await KalibreClient.PostAsJsonAsync(kalibreEndpoint, kalibreassmntassgmnt);
                flag = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    Trace.TraceInformation("Kalibre assign assessment successful;"+ "Email id:" + email + ";Skill:" + skillName + ";Competence:" + competence.ToUpper());
                    //LogHelper.AddLog("AdminController", "Kalibre assign assessment successful;", "Email id:" + email + ";Skill:" + skillName + ";Competence:" + competence.ToUpper(), "HCL.Academy.Web", user.EmailID);
                }
                else
                {
                    Trace.TraceInformation("Kalibre assign assessment failed for " + "Email id:" + email + ";Skill:" + skillName + ";Competence:" + competence.ToUpper() + ";" + response.StatusCode.ToString()+response.Content.ToString());
                //    LogHelper.AddLog("AdminController", "Kalibre assign assessment failed for " + "Email id:" + email + ";Skill:" + skillName + ";Competence:" + competence.ToUpper() + ";" + response.StatusCode.ToString(), response.Content.ToString(), "HCL.Academy.Web", user.EmailID);

                }
                #endregion               
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return flag;
        }
        /// <summary>
        /// Adds a skill to a particular user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="rolelId"></param>        
        /// <param name="ismandatory"></param>
        /// <param name="lastdayofcompletion"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<bool> AddRole(string email, string userId, string roleId, bool ismandatory, DateTime lastdayofcompletion)
        {


            InitializeServiceClient();

            HttpResponseMessage rolesresponse = await client.PostAsJsonAsync("User/GetRoleForOnboardedUser?userId=" + userId.ToString(), req);
            List<UserRole> userRoles = await rolesresponse.Content.ReadAsAsync<List<UserRole>>();

            if (userRoles.Count == 0)
            {

                RoleAssignmentRequest roleReq = new RoleAssignmentRequest();
                roleReq.ClientInfo = req.ClientInfo;
                roleReq.Email = email;
                roleReq.UserId = userId;
                roleReq.RoleId = roleId;
                roleReq.Ismandatory = ismandatory;
                roleReq.Lastdayofcompletion = lastdayofcompletion;
                HttpResponseMessage response = await client.PostAsJsonAsync("User/AddRole", roleReq);
                bool flag = await response.Content.ReadAsAsync<bool>();
                return flag;
            }
            else
            {
                return false;
            }

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
        public async Task<ActionResult> UserOnBoardingReport(int project, int roleId, int geoId)
        {
            InitializeServiceClient();
            OnboardingReportRequest request = new OnboardingReportRequest();
            request.ClientInfo = req.ClientInfo;
            request.IsExcelDownload = false;
            request.GEOId = geoId;
            request.RoleId = roleId;
            request.ProjectId = project;
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsReport", request);
            List<UserOnBoarding> lstUserOnBoarding = await response.Content.ReadAsAsync<List<UserOnBoarding>>();

            try
            {
                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkills", req);
                ViewBag.Skills = await skillResponse.Content.ReadAsAsync<List<Skill>>();

                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetenceList", req);
                ViewBag.Competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();

                HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                ViewBag.Roles = await roleResponse.Content.ReadAsAsync<List<Role>>();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return PartialView("_UserOnBoardingReport", lstUserOnBoarding);

        }
        /// <summary>
        /// Get trainings assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetTrainingsForReport(int userId, string userEmail)
        {

            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Training/GetTrainingForUser?UserId=" + userId.ToString(), req);
            List<UserTraining> trainings = await response.Content.ReadAsAsync<List<UserTraining>>();

            return new JsonResult { Data = trainings };
        }
        /// <summary>
        /// Get Skills assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetSkillsForReport(int userId, string userEmail)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetSkillForOnboardedUser?userId=" + userId.ToString(), req);
            List<UserSkill> skills = await response.Content.ReadAsAsync<List<UserSkill>>();
            return new JsonResult { Data = skills };
        }
        /// <summary>
        /// Get Skills assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetSkillsByEmail(string userEmail)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Skill/GetUserSkillByEmail?emailAddress=" + userEmail, req);
            List<UserSkill> skills = await response.Content.ReadAsAsync<List<UserSkill>>();
            return new JsonResult { Data = skills };
        }
        /// <summary>
        /// Get Skills assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> ChangeActivation(int userId, int activate, string emailaddress)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("OnBoarding/ChangeUserActivation?userId=" + userId.ToString(), req);
            bool result = await response.Content.ReadAsAsync<bool>();
            string kalibreapplicable = ConfigurationManager.AppSettings["KalibreApplicable"].ToString();
            if (kalibreapplicable == "Y")
            {
                var KalibreClient = new HttpClient();
                string serviceBaseURL = ConfigurationManager.AppSettings["KalibreAPIBaseAddress"].ToString();
                string authCode = ConfigurationManager.AppSettings["KalibreAuthCode"].ToString();
                KalibreClient.BaseAddress = new Uri(serviceBaseURL);
                KalibreClient.DefaultRequestHeaders.Accept.Clear();
                KalibreClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (activate == 0)
                {
                    string kalibreEndpoint = serviceBaseURL + "deactivateAccess?authToken=" + authCode + "&emailID=" + emailaddress;
                    HttpResponseMessage kalibreResponse = KalibreClient.GetAsync(kalibreEndpoint).Result;
                    KalibreActivationResponse kalibreResult = await kalibreResponse.Content.ReadAsAsync<KalibreActivationResponse>();
                }
                else
                {
                    string kalibreEndpoint = serviceBaseURL + "activateAccess?authToken=" + authCode + "&emailID=" + emailaddress;
                    HttpResponseMessage kalibreResponse = KalibreClient.GetAsync(kalibreEndpoint).Result;
                    KalibreActivationResponse kalibreResult = await kalibreResponse.Content.ReadAsAsync<KalibreActivationResponse>();
                }


            }
            if (response.IsSuccessStatusCode)
            {
                var regCandidateResult = response.Content.ReadAsStringAsync().Result;
            }
            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Get Skills assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetRolesForReport(int userId, string userEmail)
        {
            InitializeServiceClient();
            //IDAL dal = (new DALFactory()).GetInstance();
            //List<UserRole> userRoles = dal.GetRoleForOnboardedUser(userId);

            HttpResponseMessage response = await client.PostAsJsonAsync("User/GetRoleForOnboardedUser?userId=" + userId.ToString(), req);
            List<UserRole> userRoles = await response.Content.ReadAsAsync<List<UserRole>>();

            return new JsonResult { Data = userRoles };
        }
        /// <summary>
        /// Get Assessments assigned for the selected user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetAssessmentsForReport(int userId, string userEmail)
        {
            string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
            InitializeServiceClient();

            //if (dataStore.Equals(DataStore.SQLServer, StringComparison.InvariantCultureIgnoreCase))
            //    userId = user.DBUserId;
            //else
            //    userId = user.SPUserId;

            //IDAL dal = (new DALFactory()).GetInstance();
            //List<UserAssessment> assessments = dal.GetAssessmentForUser(userId, false);
            UserwiseAssessmentsRequest request = new UserwiseAssessmentsRequest();
            request.ClientInfo = req.ClientInfo;
            request.Userid = userId;
            request.OnlyOnBoardedTraining = false;

            HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Assessment/GetAssessmentForUser", request);
            List<UserAssessment> assessments = await assessmentResponse.Content.ReadAsAsync<List<UserAssessment>>();
            return new JsonResult { Data = assessments };
        }
        /// <summary>
        /// Update the selected skill and assign new competence/completion date for a user
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="competence"></param>
        /// <param name="userId"></param>
        /// <param name="completiondate"></param>
        /// <param name="isCompetenceChanged"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> UpdateUserSkill(int itemId, int skillid, string competence, int userId, DateTime completiondate, bool isCompetenceChanged)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            try
            {
                //  dal.UpdateUserSkill(itemId, competence, userId, completiondate, isCompetenceChanged);
                InitializeServiceClient();
                SkillAssignmentRequest skillReq = new SkillAssignmentRequest();
                skillReq.ClientInfo = req.ClientInfo;
                skillReq.CompetenceId = Convert.ToInt32(competence);
                skillReq.Id = itemId;
                skillReq.CompetencyChanged = isCompetenceChanged;
                skillReq.LastDayCompletion = Convert.ToString(completiondate);
                skillReq.UserId = userId;
                HttpResponseMessage assessmentResponse = await client.PostAsJsonAsync("Skill/UpdateUserSkill", skillReq);
                OnboardEmailRequest emailRequest = new OnboardEmailRequest();
                emailRequest.ClientInfo = req.ClientInfo;
                emailRequest.Email = user.EmailID;
                emailRequest.UserId = Convert.ToInt32(userId);
                emailRequest.UserName = user.UserName;
                emailRequest.SkillId = skillid;
                HttpResponseMessage emailResponse = await client.PostAsJsonAsync("Onboarding/AddSkillEmail", emailRequest);
                bool emailResult = await emailResponse.Content.ReadAsAsync<bool>();
                bool flag = await assessmentResponse.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = flag };
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }
        }

        public ActionResult FakeAjaxCall()
        {
            return null;
        }
        /// <summary>
        /// Download a report for a particular status in Excel format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToExcel(int projectId, int roleId, int geoId)
        {
            try
            {
                InitializeServiceClient();
                OnboardingReportRequest request = new OnboardingReportRequest();
                request.ClientInfo = req.ClientInfo;
                request.IsExcelDownload = true;
                request.RoleId = roleId;
                request.GEOId = geoId;
                request.ProjectId = projectId;
                HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsReport", request);
                List<UserOnBoarding> lstUserOnBoarding = await response.Content.ReadAsAsync<List<UserOnBoarding>>();
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
                workSheet.Row(1).Style.Font.Bold = true;


                workSheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Row(2).Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Row(2).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                workSheet.Row(2).Style.Font.Bold = true;

                workSheet.Cells[1, 1].Value = "EmployeeId";
                workSheet.Cells[1, 2].Value = "Name";
                workSheet.Cells[1, 3].Value = "GEO";
                workSheet.Cells[1, 4].Value = "Role";
                workSheet.Cells[1, 5].Value = "Email";
                workSheet.Cells[1, 6].Value = "Skill";
                workSheet.Cells[1, 7].Value = "Competence";
                workSheet.Cells[1, 8].Value = "Trainings";
                workSheet.Cells[1, 8, 1, 10].Merge = true;
                // Sub Header for Training
                workSheet.Cells[2, 8].Value = "Name";
                workSheet.Cells[2, 9].Value = "Course Name";
                workSheet.Cells[2, 10].Value = "Status";

                workSheet.Cells[1, 11].Value = "Assessments";
                workSheet.Cells[1, 11, 1, 13].Merge = true;
                // Sub Header for Assessment
                workSheet.Cells[2, 11].Value = "Name";
                workSheet.Cells[2, 12].Value = "Course Name";
                workSheet.Cells[2, 13].Value = "Status";

                workSheet.Cells[1, 14].Value = "Active";


                // Applying Border to Header Cell
                System.Drawing.Color color = System.Drawing.Color.Black;

                workSheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 8, 1, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[1, 11, 1, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[2, 8, 2, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                workSheet.Cells[2, 11, 2, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);

                //Body of table  
                //  
                int recordIndex = 3;

                string strText = string.Empty;
                string itemStatus = string.Empty;
                foreach (var user in lstUserOnBoarding)
                {
                    workSheet.Row(recordIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    int trainingCount = user.UserTrainings.Count;
                    int assessmentCount = user.UserAssessments.Count;

                    workSheet.Cells[recordIndex, 1].Value = user.EmployeeId;
                    workSheet.Cells[recordIndex, 2].Value = user.Name;
                    workSheet.Cells[recordIndex, 3].Value = user.CurrentGEO;
                    if (user.CurrentRole != null)
                        workSheet.Cells[recordIndex, 4].Value = user.CurrentRole;
                    workSheet.Cells[recordIndex, 5].Value = user.Email;
                    if (user.Active)
                        workSheet.Cells[recordIndex, 14].Value = "Active";
                    else
                        workSheet.Cells[recordIndex, 14].Value = "Inactive/DeActivated";
                    int i = 0;
                    foreach (var item in user.UserTrainings)
                    {

                        itemStatus = item.IsTrainingCompleted == true ? "Completed" : "Not Completed";
                        strText += item.SkillName + " --> " + item.TrainingName + "; Status :" + itemStatus + "; Color :" + item.StatusColor + " \r\n";

                        System.Drawing.Color cellColor = System.Drawing.Color.FromName(item.StatusColor);

                        workSheet.Cells[recordIndex + i, 8].Value = item.TrainingName;
                        workSheet.Cells[recordIndex + i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + i, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + i, 8].Style.Fill.BackgroundColor.SetColor(cellColor);

                        workSheet.Cells[recordIndex + i, 9].Value = item.SkillName;
                        workSheet.Cells[recordIndex + i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + i, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + i, 9].Style.Fill.BackgroundColor.SetColor(cellColor);

                        workSheet.Cells[recordIndex + i, 10].Value = item.ItemStatus;
                        workSheet.Cells[recordIndex + i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + i, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + i, 10].Style.Fill.BackgroundColor.SetColor(cellColor);

                        i++;

                    }

                    strText = string.Empty;
                    itemStatus = string.Empty;
                    int j = 0;
                    foreach (var item in user.UserAssessments)
                    {
                        itemStatus = item.IsAssessmentComplete == true ? "Completed" : "Not Completed";
                        strText += item.TrainingAssessment + "; Status :" + itemStatus + "\r\n" + " (" + item.SkillName + " --> " + item.TrainingName + ") " + " \r\n";
                        System.Drawing.Color cellColor = System.Drawing.Color.FromName(item.StatusColor);

                        workSheet.Cells[recordIndex + j, 11].Value = item.TrainingAssessment;
                        workSheet.Cells[recordIndex + j, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + j, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + j, 11].Style.Fill.BackgroundColor.SetColor(cellColor);

                        workSheet.Cells[recordIndex + j, 12].Value = item.SkillName;
                        workSheet.Cells[recordIndex + j, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + j, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + j, 12].Style.Fill.BackgroundColor.SetColor(cellColor);

                        workSheet.Cells[recordIndex + j, 13].Value = item.ItemStatus;
                        workSheet.Cells[recordIndex + j, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + j, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + j, 13].Style.Fill.BackgroundColor.SetColor(cellColor);

                        j++;
                    }
                    int k = 0;
                    foreach (var item in user.UserSkills)
                    {
                        workSheet.Cells[recordIndex + k, 6].Value = item.Skill;
                        workSheet.Cells[recordIndex + k, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        workSheet.Cells[recordIndex + k, 7].Value = item.Competence;
                        workSheet.Cells[recordIndex + k, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + k, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        workSheet.Cells[recordIndex + k, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex + k, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        workSheet.Cells[recordIndex + k, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        k++;
                    }

                    // Applying Style to the cell
                    int rowCount = trainingCount > assessmentCount ? trainingCount : assessmentCount;

                    if (rowCount > 0)
                    {
                        workSheet.Cells[recordIndex, 1, (recordIndex + rowCount) - 1, 1].Merge = true;
                        workSheet.Cells[recordIndex, 2, (recordIndex + rowCount) - 1, 2].Merge = true;
                        workSheet.Cells[recordIndex, 3, (recordIndex + rowCount) - 1, 3].Merge = true;
                        workSheet.Cells[recordIndex, 4, (recordIndex + rowCount) - 1, 4].Merge = true;
                        workSheet.Cells[recordIndex, 5, (recordIndex + rowCount) - 1, 5].Merge = true;

                        workSheet.Cells[recordIndex, 1, (recordIndex + rowCount) - 1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 2, (recordIndex + rowCount) - 1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 3, (recordIndex + rowCount) - 1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 4, (recordIndex + rowCount) - 1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 5, (recordIndex + rowCount) - 1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 6, (recordIndex + rowCount) - 1, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 7, (recordIndex + rowCount) - 1, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 8, (recordIndex + rowCount) - 1, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 11, (recordIndex + rowCount) - 1, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                    }

                    if (assessmentCount > 0 || trainingCount > 0)
                        recordIndex = recordIndex + rowCount;
                    else
                    {
                        workSheet.Cells[recordIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 8, recordIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        workSheet.Cells[recordIndex, 11, recordIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, color);
                        recordIndex++;
                    }
                }
                workSheet.Column(4).Style.WrapText = true;
                workSheet.Column(5).Style.WrapText = true;
                workSheet.Column(6).Style.WrapText = true;
                workSheet.Column(7).Style.WrapText = true;

                workSheet.Column(1).Width = 30;
                workSheet.Column(2).Width = 30;
                workSheet.Column(3).Width = 30;
                workSheet.Column(4).Width = 30;
                workSheet.Column(5).Width = 30;
                workSheet.Column(6).Width = 30;
                workSheet.Column(7).Width = 30;
                workSheet.Column(8).Width = 40;
                workSheet.Column(9).Width = 15;
                workSheet.Column(10).Width = 17;
                workSheet.Column(11).Width = 40;
                workSheet.Column(12).Width = 15;
                workSheet.Column(13).Width = 19;
                workSheet.Column(14).Width = 30;
                string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
                string excelName = clientName + "_UserOnBoardingReport_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";

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
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return null;
            }
        }
        /// <summary>
        /// Download a report for a particular status in PDF format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [SessionExpire]
        public async Task<FileResult> DownloadReportToPDF(int projectId, int roleId, int geoId)
        {
            MemoryStream workStream = new MemoryStream();
            string strPDFFileName = string.Format("OnBoarded" + ".pdf");
            try
            {

                // Create a new MigraDoc document

                Document document = new Document();
                document.Info.Title = "A sample report";

                document.Info.Subject = "Report for user Onboarding.";

                document.Info.Author = "HCL";

                DefineStyles(document);

                bool flag = await CreatePage(document, projectId, roleId, geoId);

                // Create a renderer for the MigraDoc document.

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();

                // Associate the MigraDoc document with a renderer

                pdfRenderer.Document = document;

                // Layout and render document to PDF

                pdfRenderer.RenderDocument();

                // Save the document...
                pdfRenderer.PdfDocument.Save(workStream, false);

                // ...and start a viewer.
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }

            return File(workStream, "application/pdf", strPDFFileName);
        }
        /// <summary>
        /// On board a user with the selected skills, competence,email and GEO
        /// </summary>
        /// <param name="competence"></param>
        /// <param name="skillId"></param>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="geo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> OffBoardUser(string email)
        {
            string result = String.Empty;
            try
            {
                InitializeServiceClient();
                HttpResponseMessage responseOnboarding = await client.PostAsJsonAsync("Onboarding/OffBoardUser?emailid=" + email, req);
                bool onboardingResult = await responseOnboarding.Content.ReadAsAsync<bool>();
                SPAuthUtility util = new SPAuthUtility();
                util.RemoveUserFromGroup(email);
                result = "Offboarding successful";
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                result = "Offboarding failed";
            }
            return new JsonResult { Data = result };
        }
        /// <summary>
        /// On board a user with the selected skills, competence,email and GEO
        /// </summary>
        /// <param name="competence"></param>
        /// <param name="skillId"></param>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="geo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> OnBoardUser(string competence, string competenceName, int skillId, string skillName, int userId, string email, string geo, int roleId,int rolebasedskillcount,int employeeId)
        {
            InitializeServiceClient();
            string result = string.Empty;

            try
            {
                //int employeeId = 0;
                SPAuthUtility util = new SPAuthUtility();

                util.AddUserToGroup(email, ref employeeId);
                UserManager onboardedUser = util.GetUserByEmail(email);
                string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                int id = 0;
                if (dataStore == DataStore.SQLServer)
                {
                    id = onboardedUser.DBUserId;
                }
                else
                {
                    id = onboardedUser.SPUserId;
                }
                UserOnboardingRequest userInfo = new UserOnboardingRequest();
                userInfo.ClientInfo = req.ClientInfo;
                userInfo.CompetenceId = Convert.ToInt32(competence);
                userInfo.EmailId = onboardedUser.EmailID;
                userInfo.EmployeeId = employeeId;
                userInfo.Name = onboardedUser.UserName;
                userInfo.SkillId = skillId;
                userInfo.GeoId = Convert.ToInt32(geo);
                userInfo.RoleId = roleId;
                userInfo.RoleBasedSkillCount = rolebasedskillcount;

                HttpResponseMessage responseOnboarding = await client.PostAsJsonAsync("Onboarding/OnBoardUser", userInfo);
                bool onboardingResult = await responseOnboarding.Content.ReadAsAsync<bool>();
                if (onboardingResult)
                {
                    OnboardEmailRequest emailRequest = new OnboardEmailRequest();
                    emailRequest.ClientInfo = req.ClientInfo;
                    emailRequest.Email = onboardedUser.EmailID;
                    emailRequest.UserId = id;
                    emailRequest.UserName = onboardedUser.UserName;
                    HttpResponseMessage emailResponse = await client.PostAsJsonAsync("Onboarding/OnboardEmail", emailRequest);
                    bool emailResult = await emailResponse.Content.ReadAsAsync<bool>();

                    if (emailResult)
                    {
                        string kalibreapplicable = ConfigurationManager.AppSettings["KalibreApplicable"].ToString();
                        if (kalibreapplicable == "Y")
                        {
                            #region KalibreOnBoarding

                            string strConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                            String Firstname = String.Empty;
                            String Surname = String.Empty;
                            String[] arrUserName = null;

                            arrUserName = onboardedUser.UserName.Split(' ');
                            if (!String.IsNullOrEmpty(onboardedUser.UserName))
                            {
                                Firstname = arrUserName[0];
                                Surname = arrUserName[1];

                                if (Surname.Contains("("))
                                {
                                    string[] lastnameparts = Surname.Split("(".ToCharArray());
                                    Surname = lastnameparts[0];
                                }

                                if (Surname.Contains(","))
                                {
                                    string[] lastnameparts = Surname.Split(",".ToCharArray());
                                    Surname = lastnameparts[0];
                                }

                            }
                            string kalibreAccountCode = ConfigurationManager.AppSettings["KalibreAccountCode"].ToString();
                            var KalibreClient = new HttpClient();
                            string serviceBaseURL = ConfigurationManager.AppSettings["KalibreAPIBaseAddress"].ToString();
                            string kalibreAuthCode = ConfigurationManager.AppSettings["KalibreAuthCode"].ToString();
                            KalibreClient.BaseAddress = new Uri(serviceBaseURL);
                            KalibreClient.DefaultRequestHeaders.Accept.Clear();
                            KalibreClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            string kalibreEndpoint = serviceBaseURL + "registerCandidate?authToken=" + kalibreAuthCode + "&emailID=" + onboardedUser.EmailID + "&accountCode=" + kalibreAccountCode + "&firstName=" + Firstname + "&lastName=" + Surname;
                            HttpResponseMessage response = KalibreClient.GetAsync(kalibreEndpoint).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var regCandidateResult = response.Content.ReadAsStringAsync().Result;
                                RegisterCandidateResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterCandidateResponse>(regCandidateResult);
                                if (data.statusCode == 200)
                                {
                                    if (data.responseBody != null)
                                    {
                                        ResponseBody responsebody = data.responseBody;
                                        //Call Academy Service to store Kalibre UserName in Academy DB
                                        HttpResponseMessage serviceResponse = await client.PostAsJsonAsync("Onboarding/StoreKalibreUserName?emailid=" + onboardedUser.EmailID + "&kalibreUsrName=" + responsebody.username, req);
                                        //LogHelper.AddLog("AdminController", "User registered in Kalibre", responsebody.username, "HCL.Academy.Web", user.EmailID);
                                        Trace.TraceInformation("User registered in Kalibre"+ responsebody.username);

                                        #region KalibreAssignmentRegistration

                                        KalibreAssmntAssgmnt kalibreassmntassgmnt = new KalibreAssmntAssgmnt();

                                        KalibreAssignment kalibreassignment = new KalibreAssignment();
                                        kalibreassignment.technology = skillName;
                                        kalibreassignment.skillLevel = competenceName.ToUpper();
                                        kalibreassmntassgmnt.accountCode = "HCL";
                                        kalibreassmntassgmnt.emailID = onboardedUser.EmailID;

                                        kalibreassmntassgmnt.assessments = new List<KalibreAssignment>();

                                        kalibreassmntassgmnt.assessments.Add(kalibreassignment);

                                        kalibreEndpoint = serviceBaseURL + "assignAssessments?authToken=" + kalibreAuthCode;
                                        HttpResponseMessage assessmentresponse = await KalibreClient.PostAsJsonAsync(kalibreEndpoint, kalibreassmntassgmnt);
                                        if (assessmentresponse.IsSuccessStatusCode)
                                        {
                                            result = "Onboarding and skill assignment successful!!";
                                            //LogHelper.AddLog("AdminController", "Kalibre assign assessment successful;", "Email id:" + onboardedUser.EmailID + ";Skill:" + skillName + ";Competence:" + competenceName.ToUpper(), "HCL.Academy.Web", user.EmailID);
                                            Trace.TraceInformation("Kalibre assign assessment successful;"+ "Email id:" + onboardedUser.EmailID + ";Skill:" + skillName + ";Competence:" + competenceName.ToUpper());
                                        }
                                        else
                                        {
                                            result = "Onboarding and skill assignment unsuccessful!!";
                                            //LogHelper.AddLog("AdminController", "Kalibre assign assessment failed for " + "Email id:" + onboardedUser.EmailID + ";Skill:" + skillName + ";Competence:" + competenceName.ToUpper() + ";" + response.StatusCode.ToString(), response.Content.ToString(), "HCL.Academy.Web", user.EmailID);
                                            Trace.TraceInformation("Kalibre assign assessment failed for " + "Email id:" + onboardedUser.EmailID + ";Skill:" + skillName + ";Competence:" + competenceName.ToUpper() + ";" + response.StatusCode.ToString()+ response.Content.ToString());

                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                result = "Onboarding and skill assignment unsuccessful!!";
                             //   LogHelper.AddLog("AdminController", "User registration failed in Kalibre for " + onboardedUser.EmailID + "; " + response.StatusCode.ToString(), response.Content.ToString(), "HCL.Academy.Web", user.EmailID);
                                Trace.TraceInformation("User registration failed in Kalibre for " + onboardedUser.EmailID + "; " + response.StatusCode.ToString()+ response.Content.ToString());

                            }

                            #endregion

                        }
                        else
                        {
                            result = "Onboarding and skill assignment successful!!";
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //       LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                result = "Error while Onboarding..." + ex.Message;
            }

            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Edit details of an on boarded user
        /// </summary>
        /// <param name="objUserOnboard"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public JsonResult EditOnBoardUser(UserOnBoarding objUserOnboard)
        {
            bool result = false;
            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Get all the competences for a particular skill ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<JsonResult> FillCompetence(int Id)
        {
            try
            {
                //   IDAL dal = (new DALFactory()).GetInstance();

                InitializeServiceClient();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillId?skillId=" + Id, req);
                List<Competence> competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                //dal.GetCompetenciesBySkillId(Id);
                JsonResult j = Json(competencies, JsonRequestBehavior.AllowGet);
                return j;
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// Get all the competences for a skill using the name of the skill
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        public async Task<JsonResult> FillCompetenceBySkillName(string name)
        {
            try
            {
                InitializeServiceClient();
                // IDAL dal = (new DALFactory()).GetInstance();
                HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetCompetenciesBySkillName?skillName=" + name, req);
                List<Competence> competencies = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
                //List<Competence> competencies = dal.GetCompetenciesBySkillName(name);
                if (competencies.Count > 0)
                {
                    JsonResult j = Json(competencies, JsonRequestBehavior.AllowGet);
                    return j;
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //  LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Get the list of trainings based on the selected skill and competency
        /// </summary>
        /// <param name="competence"></param>
        /// <param name="competenceId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetTrainingOnSkillAndCompetence(string competence, int competenceId, int skillId)
        {
            // IDAL dal = (new DALFactory()).GetInstance();
            //List<Training> trainings = dal.GetTrainings(skillId, competenceId);

            InitializeServiceClient();
            UserTrainingsRequest trainingPreReq = new UserTrainingsRequest();
            trainingPreReq.ClientInfo = req.ClientInfo;
            trainingPreReq.CompetenceId = competenceId;
            trainingPreReq.SkillId = skillId;
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetTrainings", trainingPreReq);
            List<Training> trainings = await trainingResponse.Content.ReadAsAsync<List<Training>>();

            return new JsonResult { Data = trainings };
        }
        /// <summary>
        /// Assign a particular training to a user
        /// </summary>
        /// <param name="trainings"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> AssignTrainingsToUser(List<UserTraining> trainings, int userId)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            //bool result = dal.AssignTrainingsToUser(trainings, userId);
            InitializeServiceClient();
            AssignTrainingRequest Usertrainings = new AssignTrainingRequest();
            Usertrainings.ClientInfo = req.ClientInfo;
            Usertrainings.UserId = userId;
            Usertrainings.UserTraining = trainings;

            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/AssignTrainingsToUser", Usertrainings);
            bool result = await trainingResponse.Content.ReadAsAsync<bool>();
            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Get the list of Assessments based on the selected skill and competency
        /// </summary>
        /// <param name="competence"></param>
        /// <param name="competenceId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetAssessmentOnSkillAndCompetence(string competence, int competenceId, int skillId)
        {
            InitializeServiceClient();
            SkillwiseAssessmentsRequest request = new SkillwiseAssessmentsRequest();
            request.ClientInfo = req.ClientInfo;
            request.SkillId = skillId;
            request.CompetenceId = competenceId;

            HttpResponseMessage response = await client.PostAsJsonAsync("Assessment/GetAssessments", request);
            List<Assessment> assessment = await response.Content.ReadAsAsync<List<Assessment>>();

            //IDAL dal = (new DALFactory()).GetInstance();
            //List<Assessment> assessment = dal.GetAssessments(skillId, competenceId);
            return new JsonResult { Data = assessment };
        }
        ///// <summary>
        ///// Assign a particular assessment to a user
        ///// </summary>
        ///// <param name="assessments"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[SessionExpire]
        //public JsonResult AssignAssessmentsToUser(List<UserAssessment> assessments, int userId)
        //{
        //    IDAL dal = (new DALFactory()).GetInstance();

        //    bool result = dal.AssignAssessmentsToUser(assessments, userId);
        //    return new JsonResult { Data = result };
        //}
        /// <summary>
        /// Assign an assessment to all users
        /// </summary>
        /// <param name="assignedGroup"></param>
        /// <param name="assessment"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public JsonResult AssignAssessmentsToAllUser(int assignedGroup, UserAssessment assessment)
        {
            InitializeServiceClient();
            //IDAL dal = (new DALFactory()).GetInstance();
            bool result = false;
            //try
            //{
            //    SkillwiseUsersRequest request = new SkillwiseUsersRequest();
            //    request.SkillId = assignedGroup;
            //    request.ClientInfo = req.ClientInfo;

            //    HttpResponseMessage userResponse = await client.PostAsJsonAsync("Onboarding/GetAllOnBoardedUser", request);
            //    List<UserManager> lstUserOnBoarding = await userResponse.Content.ReadAsAsync<List<UserManager>>();

            //     //List<UserManager> lstUserOnBoarding = dal.GetAllOnBoardedUser(assignedGroup);
            //    List<UserAssessment> assessments = new List<UserAssessment>();
            //    assessments.Add(assessment);

            //    foreach (UserManager user in lstUserOnBoarding)
            //    {
            //        string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
            //        if (dataStore == DataStore.SQLServer)
            //        {

            //            result = dal.AssignAssessmentsToUser(assessments, user.DBUserId);

            //            HttpResponseMessage userResponse = await client.PostAsJsonAsync("Onboarding/AssignAssessmentsToUser", request);
            //            List<UserManager> lstUserOnBoarding = await userResponse.Content.ReadAsAsync<List<UserManager>>();

            //        }
            //        else
            //        {
            //            result = dal.AssignAssessmentsToUser(assessments, user.SPUserId);

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    UserManager user = (UserManager)Session["CurrentUser"];
            //    LogHelper.AddLog(new LogEntity(AppConstant.PartitionError, user.EmailID.ToString(), AppConstant.ApplicationName, "Admin, AssignAssessmentsToAllUser", ex.Message, ex.StackTrace));
            //    //return Json("", JsonRequestBehavior.AllowGet);
            //}

            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Assign a training to a particular group of users
        /// </summary>
        /// <param name="assignedGroup"></param>
        /// <param name="training"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> AssignTrainingsToAllUser(int assignedGroup, UserTraining training)
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            InitializeServiceClient();
            bool result = false;
            try
            {
                // List<UserManager> lstUserOnBoarding = dal.GetAllOnBoardedUser(assignedGroup);

                SkillwiseUsersRequest request = new SkillwiseUsersRequest();
                request.SkillId = assignedGroup;
                request.ClientInfo = req.ClientInfo;

                HttpResponseMessage userResponse = await client.PostAsJsonAsync("Onboarding/GetAllOnBoardedUser", request);
                List<UserManager> lstUserOnBoarding = await userResponse.Content.ReadAsAsync<List<UserManager>>();

                List<UserTraining> trainings = new List<UserTraining>();
                trainings.Add(training);
                string dataStore = ConfigurationManager.AppSettings["DATASTORE"].ToString();
                InitializeServiceClient();
                if (dataStore == DataStore.SQLServer)
                {
                    foreach (UserManager user in lstUserOnBoarding)
                    {
                        AssignTrainingRequest Usertrainings = new AssignTrainingRequest();
                        Usertrainings.ClientInfo = req.ClientInfo;
                        Usertrainings.UserId = user.DBUserId;
                        Usertrainings.UserTraining = trainings;
                        HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/AssignTrainingsToUser", Usertrainings);
                        result = await trainingResponse.Content.ReadAsAsync<bool>();

                        //result = dal.AssignTrainingsToUser(trainings, user.DBUserId);
                    }
                }
                else
                {
                    foreach (UserManager user in lstUserOnBoarding)
                    {
                        AssignTrainingRequest Usertrainings = new AssignTrainingRequest();
                        Usertrainings.ClientInfo = req.ClientInfo;
                        Usertrainings.UserId = user.DBUserId;
                        Usertrainings.UserTraining = trainings;
                        HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/AssignTrainingsToUser", Usertrainings);
                        result = await trainingResponse.Content.ReadAsAsync<bool>();
                        // result = dal.AssignTrainingsToUser(trainings, user.SPUserId);
                    }

                }

            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //  LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return new JsonResult { Data = result };
        }
        /// <summary>
        /// Remove a particular skill from a user's set of skills
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> RemoveUserSkill(int itemId, string userId, string emailId)
        {
            UserManager user = (UserManager)Session["CurrentUser"];
            try
            {
                InitializeServiceClient();
                List<UserSkill> skillresult = new List<UserSkill>();

                if (!String.IsNullOrEmpty(userId))
                {
                    RequestBase userreq = new RequestBase();
                    userreq.ClientInfo = new ServiceConsumerInfo();
                    userreq.ClientInfo.id = Convert.ToInt32(userId);
                    HttpResponseMessage skillresponse = await client.PostAsJsonAsync("Skill/GetSkillForUser", userreq);
                    skillresult = await skillresponse.Content.ReadAsAsync<List<UserSkill>>();
                }
                else
                {
                    HttpResponseMessage skillresponse = await client.PostAsJsonAsync("Skill/GetUserSkillByEmail?emailAddress=" + emailId, req);
                    skillresult = await skillresponse.Content.ReadAsAsync<List<UserSkill>>();
                }

                UserSkill skill = skillresult.Where(x => x.Id == itemId).FirstOrDefault();
                SkillAssignmentRequest skillReq = new SkillAssignmentRequest();
                skillReq.Id = itemId;

                if (!String.IsNullOrEmpty(userId))
                    skillReq.UserId = Convert.ToInt32(userId);
                if (!String.IsNullOrEmpty(emailId))
                    skillReq.EmailAddress = emailId;

                skillReq.ClientInfo = req.ClientInfo;

                HttpResponseMessage response = await client.PostAsJsonAsync("Skill/RemoveUserSkill", skillReq);
                bool result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    if (skill != null)
                    {
                        string kalibreapplicable = ConfigurationManager.AppSettings["KalibreApplicable"].ToString();
                        if (kalibreapplicable == "Y")
                        {
                            #region KalibreAPI
                            var KalibreClient = new HttpClient();
                            string serviceBaseURL = ConfigurationManager.AppSettings["KalibreAPIBaseAddress"].ToString();
                            string kalibreAccountCode = ConfigurationManager.AppSettings["KalibreAccountCode"].ToString();
                            string kalibreAuthCode = ConfigurationManager.AppSettings["KalibreAuthCode"].ToString();
                            KalibreClient.BaseAddress = new Uri(serviceBaseURL);
                            KalibreClient.DefaultRequestHeaders.Accept.Clear();
                            KalibreClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            string kalibreEndpoint = serviceBaseURL + "removeAssessments?authToken=" + kalibreAuthCode;

                            KalibreAssmntAssgmnt kalibreassmntassgmnt = new KalibreAssmntAssgmnt();
                            KalibreAssignment kalibreassignment = new KalibreAssignment();
                            kalibreassignment.technology = skill.Skill;
                            kalibreassignment.skillLevel = skill.Competence.ToUpper();
                            kalibreassmntassgmnt.accountCode = kalibreAccountCode;
                            kalibreassmntassgmnt.emailID = req.ClientInfo.emailId;
                            kalibreassmntassgmnt.assessments = new List<KalibreAssignment>();
                            kalibreassmntassgmnt.assessments.Add(kalibreassignment);

                            HttpResponseMessage removeskillresponse = await KalibreClient.PostAsJsonAsync(kalibreEndpoint, kalibreassmntassgmnt);
                            bool flag = removeskillresponse.IsSuccessStatusCode;
                            if (response.IsSuccessStatusCode)
                            {
                                Trace.TraceInformation("Kalibre remove assessment successful;"+ "Email id:" + req.ClientInfo.emailId + ";Skill:" + skill.Skill + ";Competence:" + skill.Competence.ToUpper());
                          //      LogHelper.AddLog("AdminController", "Kalibre remove assessment successful;", "Email id:" + req.ClientInfo.emailId + ";Skill:" + skill.Skill + ";Competence:" + skill.Competence.ToUpper(), "HCL.Academy.Web", user.EmailID);
                            }
                            else
                            {
                                //       LogHelper.AddLog("AdminController", "Kalibre remove assessment failed for " + "Email id:" + req.ClientInfo.emailId + ";Skill:" + skill.Skill + ";Competence:" + skill.Competence.ToUpper() + ";" + response.StatusCode.ToString(), response.Content.ToString(), "HCL.Academy.Web", user.EmailID);
                                Trace.TraceInformation("Kalibre remove assessment failed for " + "Email id:" + req.ClientInfo.emailId + ";Skill:" + skill.Skill + ";Competence:" + skill.Competence.ToUpper() + ";" + response.StatusCode.ToString()+ response.Content.ToString());

                            }
                            #endregion
                        }
                    }

                }

                return new JsonResult { Data = result };
            }
            catch (Exception ex)
            {

                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return new JsonResult { Data = false };
            }
        }
        /// <summary>
        /// Remove a particular skill from a user's set of skills
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> RemoveUserRole(int itemId, string userId)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //dal.RemoveUserRole(itemId, userId);
                InitializeServiceClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("User/RemoveUserRole?roleId=" + itemId + "&userId=" + userId, req);
                bool result = await response.Content.ReadAsAsync<bool>();
                return new JsonResult { Data = true };
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                // LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return new JsonResult { Data = false };
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Defines the style for a particular page
        /// </summary>
        /// <param name="document"></param>
        private void DefineStyles(Document document)
        {
            try
            {
                // Get the predefined style Normal.
                Style style = document.Styles["Normal"];
                // Because all styles are derived from Normal, the next line changes the 
                // font of the whole document. Or, more exactly, it changes the font of
                // all styles and paragraphs that do not redefine the font.
                style.Font.Name = "Verdana";

                style = document.Styles[StyleNames.Header];
                style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

                style = document.Styles[StyleNames.Footer];
                style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

                // Create a new style called Table based on style Normal
                style = document.Styles.AddStyle("Table", "Normal");
                style.Font.Name = "Verdana";
                style.Font.Name = "Times New Roman";
                style.Font.Size = 9;

                // Create a new style called Reference based on style Normal
                style = document.Styles.AddStyle("Reference", "Normal");
                style.ParagraphFormat.SpaceBefore = "5mm";
                style.ParagraphFormat.SpaceAfter = "5mm";
                style.ParagraphFormat.TabStops.AddTabStop("6cm", TabAlignment.Right);
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }
        /// <summary>
        /// Create all pages
        /// </summary>
        /// <param name="document"></param>
        /// <param name="status"></param>
        private async Task<bool> CreatePage(Document document, int projectId, int roleId, int geoId)
        {
            try
            {
                InitializeServiceClient();
                OnboardingReportRequest request = new OnboardingReportRequest();
                request.ClientInfo = req.ClientInfo;
                request.IsExcelDownload = true;
                request.ProjectId = projectId;
                request.RoleId = roleId;
                request.GEOId = geoId;
                HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsReport", request);
                List<UserOnBoarding> lstUserOnBoarding = await response.Content.ReadAsAsync<List<UserOnBoarding>>();

                // Add OnBoarding Report
                AddOnBoardingDetails(document, lstUserOnBoarding);

                // Add Training Details
                AddTrainingDetails(document, lstUserOnBoarding);

                //Add Assessment
                AddAssessmentDetails(document, lstUserOnBoarding);
                return true;
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return false;

            }

        }
        /// <summary>
        /// Create the section to add Onboarding details
        /// </summary>
        /// <param name="document"></param>
        /// <param name="lstUserOnBoarding"></param>
        private void AddOnBoardingDetails(Document document, List<UserOnBoarding> lstUserOnBoarding)
        {
            try
            {
                Section section = document.AddSection();

                section.AddParagraph("User OnBoarding Report");

                section.AddParagraph("\n");

                // Create the item table

                Table table = section.AddTable();
                table.Style = "Table";
                table.Borders.Width = 0.2;
                table.Borders.Left.Width = 0.5;
                table.Borders.Right.Width = 0.5;
                table.Rows.LeftIndent = 0;
                // Before you can add a row, you must define the columns

                MigraDoc.DocumentObjectModel.Tables.Column column = table.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("3.5cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2.5cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("3cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                //column = table.AddColumn("1.5cm");
                //column.Format.Alignment = ParagraphAlignment.Right;

                //column = table.AddColumn("1.5cm");
                //column.Format.Alignment = ParagraphAlignment.Right;

                // Create the header of the table

                Row row = table.AddRow();

                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

                //row.Shading.Color = TableBlue;

                row.Cells[0].AddParagraph("Id");
                row.Cells[1].AddParagraph("Name");
                row.Cells[2].AddParagraph("Role");
                row.Cells[3].AddParagraph("Email");
                row.Cells[4].AddParagraph("Primary Skill");
                row.Cells[5].AddParagraph("Competence");
                //   row.Cells[4].AddParagraph("BGV Status");
                //   row.Cells[5].AddParagraph("Profile Sharing");

                foreach (var item in lstUserOnBoarding)
                {
                    Row rowData = table.AddRow();
                    rowData.HeadingFormat = true;
                    rowData.Format.Alignment = ParagraphAlignment.Left;

                    rowData.Cells[0].AddParagraph(item.EmployeeId.ToString());
                    rowData.Cells[1].AddParagraph(item.Name);
                    rowData.Cells[2].AddParagraph(item.CurrentRole);
                    rowData.Cells[3].AddParagraph(item.Email);
                    rowData.Cells[4].AddParagraph(item.CurrentSkill);
                    rowData.Cells[5].AddParagraph(item.CurrentCompetance);
                    // rowData.Cells[4].AddParagraph(item.CurrentBGVStatus);
                    //   rowData.Cells[5].AddParagraph(item.CurrentProfileSharing);
                }
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
        }
        /// <summary>
        /// Add the training details section on the Home Page
        /// </summary>
        /// <param name="document"></param>
        /// <param name="lstUserOnBoarding"></param>
        private void AddTrainingDetails(Document document, List<UserOnBoarding> lstUserOnBoarding)
        {
            try
            {
                Section section = document.AddSection();

                section.AddParagraph("Details Training Report");

                section.AddParagraph("\n");

                // Create the item table

                Table table = section.AddTable();
                table.Style = "Table";
                //table.Borders.Color = MigraDoc.DocumentObjectModel.Color.FromRgb;
                table.Borders.Width = 0.2;
                table.Borders.Left.Width = 0.5;
                table.Borders.Right.Width = 0.5;
                table.Rows.LeftIndent = 0;

                // Before you can add a row, you must define the columns

                MigraDoc.DocumentObjectModel.Tables.Column column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                // Create the header of the table

                Row row = table.AddRow();

                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

                //row.Shading.Color = TableBlue;

                row.Cells[0].AddParagraph("Id");
                row.Cells[1].AddParagraph("Name");
                row.Cells[2].AddParagraph("Role");
                row.Cells[3].AddParagraph("Email");
                row.Cells[4].AddParagraph("Training Name");
                row.Cells[5].AddParagraph("Skill");
                row.Cells[6].AddParagraph("Status");

                foreach (var item in lstUserOnBoarding)
                {
                    Row rowData = table.AddRow();
                    rowData.HeadingFormat = true;
                    rowData.Format.Alignment = ParagraphAlignment.Left;

                    rowData.Cells[0].AddParagraph(item.EmployeeId.ToString());
                    rowData.Cells[0].MergeDown = item.UserTrainings.Count;
                    rowData.Cells[1].AddParagraph(item.Name);
                    rowData.Cells[1].MergeDown = item.UserTrainings.Count;
                    rowData.Cells[2].AddParagraph(item.CurrentRole);
                    rowData.Cells[2].MergeDown = item.UserTrainings.Count;
                    rowData.Cells[3].AddParagraph(item.Email);
                    rowData.Cells[3].MergeDown = item.UserTrainings.Count;

                    foreach (var training in item.UserTrainings)
                    {
                        Row rowTraining = table.AddRow();
                        rowTraining.HeadingFormat = true;
                        rowTraining.Format.Alignment = ParagraphAlignment.Left;

                        rowTraining.Cells[4].AddParagraph(training.TrainingName);
                        rowTraining.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(training.StatusColor);
                        rowTraining.Cells[5].AddParagraph(training.SkillName);
                        rowTraining.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(training.StatusColor);
                        rowTraining.Cells[6].AddParagraph(training.ItemStatus);
                        rowTraining.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(training.StatusColor);
                    }
                }
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }
        /// <summary>
        /// Add the Assessments section to the Home Page
        /// </summary>
        /// <param name="document"></param>
        /// <param name="lstUserOnBoarding"></param>
        private void AddAssessmentDetails(Document document, List<UserOnBoarding> lstUserOnBoarding)
        {
            try
            {
                Section section = document.AddSection();

                section.AddParagraph("Details Assessment Report");

                section.AddParagraph("\n");

                // Create the item table

                Table table = section.AddTable();
                table.Style = "Table";
                table.Borders.Width = 0.2;
                table.Borders.Left.Width = 0.5;
                table.Borders.Right.Width = 0.5;
                table.Rows.LeftIndent = 0;

                // Before you can add a row, you must define the columns

                MigraDoc.DocumentObjectModel.Tables.Column column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                column = table.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                // Create the header of the table

                Row row = table.AddRow();

                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

                //row.Shading.Color = TableBlue;

                row.Cells[0].AddParagraph("Id");
                row.Cells[1].AddParagraph("Name");
                row.Cells[2].AddParagraph("Role");
                row.Cells[3].AddParagraph("Email");
                row.Cells[4].AddParagraph("Assessment Name");
                row.Cells[5].AddParagraph("Skill");
                row.Cells[6].AddParagraph("Status");

                foreach (var item in lstUserOnBoarding)
                {
                    Row rowData = table.AddRow();
                    rowData.HeadingFormat = true;
                    rowData.Format.Alignment = ParagraphAlignment.Left;

                    rowData.Cells[0].AddParagraph(item.EmployeeId.ToString());
                    rowData.Cells[0].MergeDown = item.UserAssessments.Count;
                    rowData.Cells[1].AddParagraph(item.Name);
                    rowData.Cells[1].MergeDown = item.UserAssessments.Count;
                    rowData.Cells[2].AddParagraph(item.CurrentRole);
                    rowData.Cells[2].MergeDown = item.UserAssessments.Count;
                    rowData.Cells[3].AddParagraph(item.Email);
                    rowData.Cells[3].MergeDown = item.UserAssessments.Count;

                    foreach (UserAssessment assessment in item.UserAssessments)
                    {
                        Row rowTraining = table.AddRow();
                        rowTraining.HeadingFormat = true;
                        rowTraining.Format.Alignment = ParagraphAlignment.Left;

                        rowTraining.Cells[4].AddParagraph(assessment.TrainingAssessment);
                        rowTraining.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(assessment.StatusColor);
                        rowTraining.Cells[5].AddParagraph(assessment.SkillName);
                        rowTraining.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(assessment.StatusColor);
                        rowTraining.Cells[6].AddParagraph(assessment.ItemStatus);
                        rowTraining.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse(assessment.StatusColor);
                    }
                }
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
        }
        /// <summary>
        /// get the list of Onboarded users based on selected status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task<DataTable> GetTable(string status)
        {
            DataTable table = new DataTable();
            try
            {
                OnboardingReportRequest request = new OnboardingReportRequest();
                request.ClientInfo = req.ClientInfo;
                request.IsExcelDownload = true;
                request.Status = status;
                HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetOnBoardingDetailsReport", request);
                List<UserOnBoarding> lstUserOnBoarding = await response.Content.ReadAsAsync<List<UserOnBoarding>>();

                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Email", typeof(string));
                table.Columns.Add("Skill", typeof(string));
                table.Columns.Add("Competance", typeof(string));

                //
                // Here we add five DataRows.
                //
                foreach (UserOnBoarding item in lstUserOnBoarding)
                {
                    table.Rows.Add(item.Name, item.Email, item.CurrentSkill, item.CurrentCompetance);
                }
            }
            catch (Exception ex)
            {
                // UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("AdminController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            return table;
        }


        #endregion
    }
}