using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

namespace HCL.Academy.DAL
{
    public class SqlSvrDAL
    {
        private ServiceConsumerInfo currentUser;
        private string currentSiteUrl;
        private List<Role> roles;
        private List<CheckListItem> checkListItems;
        private string strConnectionString = string.Empty;
        private SqlConnection connection;

        //public async Task<string> GetToken(string authority, string resource, string scope)
        //{
        //    string CLIENTID = ConfigurationManager.AppSettings["ClientId"].ToString();
        //    string CLIENTSECRET = ConfigurationManager.AppSettings["Secret"].ToString();
        //    var authContext = new AuthenticationContext(authority);
        //    ClientCredential clientCred = new ClientCredential(CLIENTID, CLIENTSECRET);
        //    AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

        //    if (result == null)
        //        throw new InvalidOperationException("Failed to obtain the JWT token");

        //    return result.AccessToken;
        //}
        public SqlSvrDAL()
        {
            string keyvaultuse = ConfigurationManager.AppSettings["DBCONSTRFROMAZKEYVAULT"].ToString();
            if (keyvaultuse.ToUpper() == "TRUE")
            {
                string BASESECRETURI = ConfigurationManager.AppSettings["KeyVaultURL"].ToString();

                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                        {
                            Delay= TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 5,
                            Mode = RetryMode.Exponential
                         }
                };
                var client = new SecretClient(new Uri(BASESECRETURI), new DefaultAzureCredential(), options);
                KeyVaultSecret secret = client.GetSecret("academydbconstr");             
                strConnectionString = secret.Value;
            }
            else
                strConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            this.connection = new SqlConnection(strConnectionString);
        }
        public SqlSvrDAL(ServiceConsumerInfo user)
        {
            string keyvaultuse = ConfigurationManager.AppSettings["DBCONSTRFROMAZKEYVAULT"].ToString();
            if (keyvaultuse.ToUpper() == "TRUE")
            {
                string BASESECRETURI = ConfigurationManager.AppSettings["KeyVaultURL"].ToString();               

                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                        {
                            Delay= TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 5,
                            Mode = RetryMode.Exponential
                         }
                };
                var client = new SecretClient(new Uri(BASESECRETURI), new DefaultAzureCredential(), options);
                KeyVaultSecret secret = client.GetSecret("academydbconstr");            
                strConnectionString = secret.Value;
            }
            else
                strConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            connection = new SqlConnection(strConnectionString);
            currentSiteUrl = ConfigurationManager.AppSettings["URL"].ToString();
            currentUser = user;
        }



        public List<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }
        public List<CheckListItem> CheckListItems
        {
            get { return checkListItems; }
            set { checkListItems = value; }
        }
        public List<AcademyVideo> GetAllAcademyVideos()
        {
            List<AcademyVideo> lstAcademyVideo = new List<AcademyVideo>();
            return lstAcademyVideo;
        }
        public string CheckUserActive(string emailAddress)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters = { new SqlParameter("@useremail", SqlDbType.NVarChar) };
            parameters[0].Value = Convert.ToString(emailAddress);
            string active = dh.ExecuteScalar("[dbo].[proc_CheckUserActive]", CommandType.StoredProcedure, parameters).ToString();
            return active;

        }
        public KaliberUserAssessmentResults KaliberUpdateUserAssessment(KaliberUserAssessmentRequest kaliberrequest)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            KaliberUserAssessmentResults resp = new KaliberUserAssessmentResults();
            try
            {


                SqlParameter[] parameters =
                   {
                        new SqlParameter("@emailID",SqlDbType.NVarChar),
                        new SqlParameter("@accountCode",SqlDbType.NVarChar),
                        new SqlParameter("@skillLevel",SqlDbType.NVarChar),
                        new SqlParameter("@technology",SqlDbType.NVarChar),
                        new SqlParameter("@ragStatus",SqlDbType.NVarChar),
                        new SqlParameter("@mcqScore",SqlDbType.Decimal),
                        new SqlParameter("@mcqResult",SqlDbType.NVarChar),
                        new SqlParameter("@codeReviewExamScore",SqlDbType.Decimal),
                        new SqlParameter("@buildStatus",SqlDbType.NVarChar),
                        new SqlParameter("@totalTests",SqlDbType.Int),
                        new SqlParameter("@testStatus",SqlDbType.NVarChar),
                        new SqlParameter("@testCaseSuccessPercentage",SqlDbType.Decimal),
                        new SqlParameter("@testCoveragePercentage",SqlDbType.Decimal),
                        new SqlParameter("@assessmentName",SqlDbType.NVarChar),
                        new SqlParameter("@IsAssessmentComplete",SqlDbType.Bit),

                        new SqlParameter("@errorMessage",SqlDbType.NVarChar),
                    };
                parameters[0].Value = Convert.ToString(kaliberrequest.emailID);
                parameters[1].Value = Convert.ToString(kaliberrequest.accountCode);
                parameters[2].Value = Convert.ToString(kaliberrequest.skillLevel);
                parameters[3].Value = Convert.ToString(kaliberrequest.technology);
                parameters[4].Value = Convert.ToString(kaliberrequest.ragStatus);
                parameters[5].Value = Convert.ToDecimal(kaliberrequest.mcqScore);
                parameters[6].Value = Convert.ToString(kaliberrequest.mcqResult);
                parameters[7].Value = Convert.ToDecimal(kaliberrequest.codeReviewExamScore);
                parameters[8].Value = Convert.ToString(kaliberrequest.buildStatus);
                parameters[9].Value = Convert.ToInt32(kaliberrequest.totalTests);
                parameters[10].Value = Convert.ToString(kaliberrequest.testStatus);
                parameters[11].Value = Convert.ToDecimal(kaliberrequest.testCaseSuccessPercentage);
                parameters[12].Value = Convert.ToDecimal(kaliberrequest.testCoveragePercentage);
                parameters[13].Value = Convert.ToString(kaliberrequest.assessmentName);
                parameters[14].Value = Convert.ToString(kaliberrequest.ragStatus.ToLower()) == "pass" ? true : false; ;
                parameters[15].Size = 4000;
                parameters[15].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_kaliber_Update_UserAssessment]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    resp.Messege = Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value);
                    result = false;
                    LogHelper.AddLog(dh, "SqlSvrDAL.cs,KaliberUpdateUserAssessment", Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value), "", "KaliberAcademy", kaliberrequest.emailID);
                }
                else
                {
                    resp.Messege = "Record updated successfully.";
                    result = true;
                }

            }
            catch (Exception ex)
            {
                resp.Messege = Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value);
                LogHelper.AddLog(dh, "SqlSvrDAL.cs,KaliberUpdateUserAssessment", Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value), ex.InnerException.StackTrace.ToString(), "KaliberAcademy", kaliberrequest.emailID);
            }
            finally
            {
                // assign response object

                if (result)
                {
                    resp.status = "SUCCESS";
                }
                else
                {
                    resp.status = "FAILED";
                }
                resp.emailID = Convert.ToString(kaliberrequest.emailID);
                resp.assessmentName = Convert.ToString(kaliberrequest.assessmentName);
                resp.skillLevel = Convert.ToString(kaliberrequest.skillLevel);
                resp.technology = Convert.ToString(kaliberrequest.technology);

                // Log API request
                var json = new JavaScriptSerializer().Serialize(kaliberrequest);
                LogHelper.AddApiRequestLog(dh, Convert.ToString(json), result);

                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }


            }

            return resp;

        }
        public List<TrainingMaster> GetTrainingBySkill(int skillId)
        {
            List<TrainingMaster> trainings = new List<TrainingMaster>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@SkillID";
                sqlParameters[0].Value = skillId;
                sqlParameters[0].Direction = ParameterDirection.Input;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetTrainingBySkill]", CommandType.StoredProcedure, sqlParameters);

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        TrainingMaster t = new TrainingMaster();
                        t.Id = Convert.ToInt32(item["TrainingId"].ToString());
                        t.title = item["Title"].ToString();
                        trainings.Add(t);
                    }
                }
            }
            return trainings;
        }
        public List<QuestionDetail> GetEachQuestionDetails(string listName, int assessmentId)
        {
            List<QuestionDetail> allQuestions = new List<QuestionDetail>();
            try
            {
                QuestionDetail eachQuestionDetails = null;
                DataHelper dh = new DataHelper(strConnectionString);
                DataSet ds = new DataSet();

                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter();
                    sqlParameters[0].ParameterName = "@assessmentID";
                    sqlParameters[0].Value = assessmentId;
                    sqlParameters[0].Direction = ParameterDirection.Input;
                    ds = dh.ExecuteDataSet("[dbo].[proc_GetEachQuestionDetails]", CommandType.StoredProcedure, sqlParameters);

                }
                finally
                {
                    if (dh != null)
                    {
                        if (dh.DataConn != null)
                        {
                            dh.DataConn.Close();
                        }
                    }
                }
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            eachQuestionDetails = new QuestionDetail();
                            if (item["Question"] != null && !(item["Question"] is DBNull))
                            {
                                string multitextValue = item["Question"].ToString();
                                eachQuestionDetails.questionTitle = multitextValue;
                            }
                            if (item["Option1"] != null && !(item["Option1"] is DBNull))
                            {
                                eachQuestionDetails.option1 = Convert.ToString(item["Option1"]);
                            }
                            if (item["Option2"] != null && !(item["Option2"] is DBNull))
                            {
                                eachQuestionDetails.option2 = Convert.ToString(item["Option2"]);
                            }
                            if (item["Option3"] != null && !(item["Option3"] is DBNull))
                            {
                                eachQuestionDetails.option3 = Convert.ToString(item["Option3"]);
                            }
                            if (item["Option4"] != null && !(item["Option4"] is DBNull))
                            {
                                eachQuestionDetails.option4 = Convert.ToString(item["Option4"]);
                            }
                            if (item["Option5"] != null && !(item["Option5"] is DBNull))
                            {
                                if (item["Option5"].ToString().Length > 0)
                                    eachQuestionDetails.option5 = item["Option5"].ToString();
                            }
                            int CorrectOptionSequence = 0;
                            if (item["Option5"] != null && !(item["Option5"] is DBNull))
                            {
                                CorrectOptionSequence = Convert.ToInt32(item["CorrectOptionSequence"]);
                            }
                            if (CorrectOptionSequence == 1)
                            {
                                eachQuestionDetails.correctOption = eachQuestionDetails.option1;
                            }
                            else if (CorrectOptionSequence == 2)
                            {
                                eachQuestionDetails.correctOption = eachQuestionDetails.option2;
                            }
                            else if (CorrectOptionSequence == 3)
                            {
                                eachQuestionDetails.correctOption = eachQuestionDetails.option3;
                            }
                            else if (CorrectOptionSequence == 4)
                            {
                                eachQuestionDetails.correctOption = eachQuestionDetails.option4;
                            }
                            else
                            {
                                eachQuestionDetails.correctOption = Convert.ToString(item["CorrectOption"]);
                            }
                            eachQuestionDetails.marks = Convert.ToString(item["Marks"]);
                            eachQuestionDetails.questionID = Convert.ToString(item["ID"]);
                            allQuestions.Add(eachQuestionDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetEachQuestionDetails", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return allQuestions;
        }
        public List<SkillCompetencyResource> GetAllSkillResourceCount(int projectId)
        {
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter();
            sqlParameters[0].ParameterName = "@projectId";
            sqlParameters[0].Value = projectId;
            sqlParameters[0].Direction = ParameterDirection.Input;
            List<SkillCompetencyResource> result = new List<SkillCompetencyResource>();
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = dh.ExecuteDataSet("[dbo].[proc_GetAllSkillResourceCount]", CommandType.StoredProcedure, sqlParameters);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SkillCompetencyResource skillresource = new SkillCompetencyResource();
                        skillresource.Skill = ds.Tables[0].Rows[i]["Title"].ToString();
                        skillresource.SkillId = ds.Tables[0].Rows[i]["ID"].ToString();
                        skillresource.NoviceCount = Convert.ToInt32(ds.Tables[0].Rows[i]["NoviceCount"].ToString());
                        skillresource.AdvancedBeginnerCount = Convert.ToInt32(ds.Tables[0].Rows[i]["AdvancedBeginnerCount"].ToString());
                        skillresource.CompetentCount = Convert.ToInt32(ds.Tables[0].Rows[i]["CompetentCount"].ToString());
                        skillresource.ProficientCount = Convert.ToInt32(ds.Tables[0].Rows[i]["ProficientCount"].ToString());
                        skillresource.ExpertCount = Convert.ToInt32(ds.Tables[0].Rows[i]["ExpertCount"].ToString());
                        result.Add(skillresource);
                    }
                }
            }

            return result;

        }
        public bool RemoveUserSkill(int itemId, string userId, string email)
        {
            bool result = false;
            try
            {
                if (userId != null && userId != "0")
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId), new SqlParameter("@itemId", itemId),
                    new SqlParameter("@status",SqlDbType.Bit) {Direction=ParameterDirection.Output} };
                    DataHelper dh = new DataHelper(strConnectionString);
                    dh.ExecuteNonQuery("[dbo].[proc_RemoveUserSkill]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                }
                else if (email != null)
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@emailId", userId), new SqlParameter("@itemId", itemId),
                    new SqlParameter("@status",SqlDbType.Bit) {Direction=ParameterDirection.Output} };
                    DataHelper dh = new DataHelper(strConnectionString);
                    dh.ExecuteNonQuery("[dbo].[proc_RemoveUserSkillByEmail]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,RemoveUserSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                result = false;
            }
            return result;
        }
        public List<Role> GetAllRoles()
        {
            //if (roles == null)
            //{
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                roles = new List<Role>();
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllRoles]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            Role r = new Role();
                            r.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString());
                            r.Title = ds.Tables[0].Rows[i]["RoleName"].ToString();
                            roles.Add(r);
                        }
                    }
                }
                //if (HttpContext.Current != null)
                //    HttpContext.Current.Session[AppConstant.AllRoleData] = roles;

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllRoles", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            //}
            return roles;
        }
        public FileStreamResult GetVideoStream(string url)
        {
            Stream inputStream = null;
            return new FileStreamResult(inputStream, "video/mp4");
        }

        public Assessments GetAssessmentDetails(int assessmentId)
        {
            Assessments assessment = new Assessments();
            try
            {
                List<AcademyJoinersCompletion> assessmentDetails = GetCurrentUserAssessments(assessmentId, true);
                if (assessmentDetails.Count > 0)
                {
                    var objAssessment = assessmentDetails.SingleOrDefault();
                    if (objAssessment.attempts == objAssessment.maxAttempts || objAssessment.assessmentStatus)
                    {
                        assessment.assessmentId = objAssessment.trainingAssessmentLookUpId;
                        assessment.maxAttemptsExceeded = objAssessment.attempts == objAssessment.maxAttempts ? true : false;
                        assessment.assessmentCompletionStatus = objAssessment.assessmentStatus;
                        assessment.assessmentName = objAssessment.trainingAssessmentLookUpText;
                        return assessment;
                    }
                    assessment.assessmentId = objAssessment.trainingAssessmentLookUpId;
                    assessment.assessmentName = objAssessment.trainingAssessmentLookUpText;
                    DataHelper dh = new DataHelper(strConnectionString);
                    DataSet ds = new DataSet();
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter();
                    sqlParameters[0].ParameterName = "@assessmentID";
                    sqlParameters[0].Value = objAssessment.trainingAssessmentLookUpId;
                    sqlParameters[0].Direction = ParameterDirection.Input;
                    ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessment]", CommandType.StoredProcedure, sqlParameters);

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            assessment.passingPercentage = Convert.ToInt32(ds.Tables[0].Rows[0]["PassingMarks"].ToString());
                            assessmentDetails.SingleOrDefault().trainingAssessmentTimeInMins = Convert.ToInt32(ds.Tables[0].Rows[0]["AssessmentTimeInMins"].ToString());
                            //  assessment.assessmentName = ds.Tables[0].Rows[0]["Title"].ToString();
                        }

                    }
                    var questions = GetEachQuestionDetails(AppConstant.SPList_AcademyAssessment, objAssessment.trainingAssessmentLookUpId);
                    questions = questions.OrderBy(x => Guid.NewGuid()).Take(AppConstant.MaxQueForAssessment).ToList();
                    var totalMarks = questions.Sum(x => Convert.ToInt32(x.marks));

                    assessment.questionDetails = questions;
                    assessment.assessmentDetails = assessmentDetails;
                    assessment.totalMarks = totalMarks;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAssessmentDetails", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            return assessment;
        }

        public bool AssessmentResult(AssesmentResult result, List<QuestionDetail> questionDetails)
        {
            bool response = false;
            try
            {
                DataHelper dh = new DataHelper(strConnectionString);
                DataSet ds = new DataSet();

                DateTime CurrDate = DateTime.Now;

                foreach (var item in questionDetails)
                {
                    SqlParameter[] sqlParametersAssessmentAnswer = new SqlParameter[6];
                    sqlParametersAssessmentAnswer[0] = new SqlParameter();
                    sqlParametersAssessmentAnswer[0].ParameterName = "@assessmentId";
                    sqlParametersAssessmentAnswer[0].Value = result.assessmentId;
                    sqlParametersAssessmentAnswer[0].Direction = ParameterDirection.Input;
                    sqlParametersAssessmentAnswer[1] = new SqlParameter();
                    sqlParametersAssessmentAnswer[1].ParameterName = "@userId";
                    sqlParametersAssessmentAnswer[1].Value = currentUser.id;
                    sqlParametersAssessmentAnswer[1].Direction = ParameterDirection.Input;
                    sqlParametersAssessmentAnswer[2] = new SqlParameter();
                    sqlParametersAssessmentAnswer[2].ParameterName = "@question";
                    sqlParametersAssessmentAnswer[2].Value = item.questionTitle;
                    sqlParametersAssessmentAnswer[2].Direction = ParameterDirection.Input;
                    sqlParametersAssessmentAnswer[3] = new SqlParameter();
                    sqlParametersAssessmentAnswer[3].ParameterName = "@correctOption";
                    sqlParametersAssessmentAnswer[3].Value = item.correctOption;
                    sqlParametersAssessmentAnswer[3].Direction = ParameterDirection.Input;
                    sqlParametersAssessmentAnswer[4] = new SqlParameter();
                    sqlParametersAssessmentAnswer[4].ParameterName = "@selectedOption";
                    sqlParametersAssessmentAnswer[4].Value = Convert.ToString(item.selectedOption);
                    sqlParametersAssessmentAnswer[4].Direction = ParameterDirection.Input;
                    sqlParametersAssessmentAnswer[5] = new SqlParameter();
                    sqlParametersAssessmentAnswer[5].ParameterName = "@timestamp";
                    sqlParametersAssessmentAnswer[5].Value = CurrDate.ToString("dd/MM/yyyy HH:mm");
                    sqlParametersAssessmentAnswer[5].Direction = ParameterDirection.Input;
                    dh.ExecuteNonQuery("proc_AddUserAssessmentQuestionHistory", CommandType.StoredProcedure, sqlParametersAssessmentAnswer);
                }

                decimal percentage = (result.securedMarks * 100) / result.totalMarks;
                if (percentage >= Convert.ToDecimal(result.passingPercentage))
                {
                    response = true;

                    // Assessments AssessmentItem = (Assessments)HttpContext.Current.Session["StartAssessment"];
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("UserName", currentUser.name);
                    hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    hashtable.Add("Completed Date", DateTime.Now.ToString("dd.MMM.yyy"));
                    // hashtable.Add("AssessmentName", Convert.ToString(AssessmentItem.assessmentName));
                    hashtable.Add("AssessmentName", "");
                    hashtable.Add("MarksInPercentage", percentage);
                    bool Queue = AddToEmailQueue("SendAssessmentCertificates", hashtable, currentUser.emailId, null);
                }

                SqlParameter[] sqlParametersAssessmentStatus = new SqlParameter[5];
                sqlParametersAssessmentStatus[0] = new SqlParameter();
                sqlParametersAssessmentStatus[0].ParameterName = "@assessmentId";
                sqlParametersAssessmentStatus[0].Value = result.assessmentId;
                sqlParametersAssessmentStatus[0].Direction = ParameterDirection.Input;
                sqlParametersAssessmentStatus[1] = new SqlParameter();
                sqlParametersAssessmentStatus[1].ParameterName = "@userId";
                sqlParametersAssessmentStatus[1].Value = currentUser.id;
                sqlParametersAssessmentStatus[1].Direction = ParameterDirection.Input;
                sqlParametersAssessmentStatus[2] = new SqlParameter();
                sqlParametersAssessmentStatus[2].ParameterName = "@marksObtained";
                sqlParametersAssessmentStatus[2].Value = result.securedMarks;
                sqlParametersAssessmentStatus[2].Direction = ParameterDirection.Input;
                sqlParametersAssessmentStatus[3] = new SqlParameter();
                sqlParametersAssessmentStatus[3].ParameterName = "@marksInPercentage";
                sqlParametersAssessmentStatus[3].Value = percentage;
                sqlParametersAssessmentStatus[3].Direction = ParameterDirection.Input;
                sqlParametersAssessmentStatus[4] = new SqlParameter();
                sqlParametersAssessmentStatus[4].ParameterName = "@passed";
                if (response)
                    sqlParametersAssessmentStatus[4].Value = 1;
                else
                    sqlParametersAssessmentStatus[4].Value = 0;
                sqlParametersAssessmentStatus[4].Direction = ParameterDirection.Input;
                ds = dh.ExecuteDataSet("[dbo].[proc_UpdateUserAssessment]", CommandType.StoredProcedure, sqlParametersAssessmentStatus);

                /////Assign Points to User if assessment is completed///////////
                if (response)
                {
                    /////////////Get Assessment Details///////////////                        
                    SqlParameter[] sqlParametersAssessment = new SqlParameter[1];
                    sqlParametersAssessment[0] = new SqlParameter();
                    sqlParametersAssessment[0].ParameterName = "@assessmentID";
                    sqlParametersAssessment[0].Value = result.assessmentId;
                    sqlParametersAssessment[0].Direction = ParameterDirection.Input;
                    ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessment]", CommandType.StoredProcedure, sqlParametersAssessment);

                    List<Competence> allCompetencyLevelItems = GetAllCompetenceList();

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int points = 0;
                        if (ds.Tables[0].Rows[0]["Points"] != null && !(ds.Tables[0].Rows[0]["Points"] is DBNull))
                        {
                            points = Convert.ToInt32(ds.Tables[0].Rows[0]["Points"]);
                        }
                        int userSkillId = Convert.ToInt32(ds.Tables[0].Rows[0]["SkillId"].ToString());
                        int competenceLevelId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompetencyLevelId"].ToString());
                        List<Competence> querySkillCompetencyLevelItems = allCompetencyLevelItems.Where(c => c.CompetenceId == competenceLevelId).ToList();

                        ////////////////////Get UserPoint record///////////////////
                        SqlParameter[] sqlParametersUserPoint = new SqlParameter[3];
                        sqlParametersUserPoint[0] = new SqlParameter();
                        sqlParametersUserPoint[0].ParameterName = "@userId";
                        sqlParametersUserPoint[0].Value = currentUser.id;
                        sqlParametersUserPoint[0].Direction = ParameterDirection.Input;
                        sqlParametersUserPoint[1] = new SqlParameter();
                        sqlParametersUserPoint[1].ParameterName = "@skillId";
                        sqlParametersUserPoint[1].Value = userSkillId;
                        sqlParametersUserPoint[1].Direction = ParameterDirection.Input;
                        sqlParametersUserPoint[2] = new SqlParameter();
                        sqlParametersUserPoint[2].ParameterName = "@competencyId";
                        sqlParametersUserPoint[2].Value = competenceLevelId;
                        sqlParametersUserPoint[2].Direction = ParameterDirection.Input;
                        ds = dh.ExecuteDataSet("[dbo].[proc_GetUserPoint]", CommandType.StoredProcedure, sqlParametersUserPoint);

                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow userPointItem = ds.Tables[0].Rows[0];
                            if (userPointItem != null)
                            {
                                int currentTrainingPoint = 0;
                                int currentAssessmentPoint = 0;
                                SqlParameter[] sqlParametersAssessmentPoint = new SqlParameter[5];
                                sqlParametersAssessmentPoint[0] = new SqlParameter();
                                sqlParametersAssessmentPoint[0].ParameterName = "@userId";
                                sqlParametersAssessmentPoint[0].Value = currentUser.id;
                                sqlParametersAssessmentPoint[0].Direction = ParameterDirection.Input;
                                sqlParametersAssessmentPoint[1] = new SqlParameter();
                                sqlParametersAssessmentPoint[1].ParameterName = "@skillId";
                                sqlParametersAssessmentPoint[1].Value = userSkillId;
                                sqlParametersAssessmentPoint[1].Direction = ParameterDirection.Input;
                                sqlParametersAssessmentPoint[2] = new SqlParameter();
                                sqlParametersAssessmentPoint[2].ParameterName = "@competencyId";
                                sqlParametersAssessmentPoint[2].Value = competenceLevelId;
                                sqlParametersAssessmentPoint[2].Direction = ParameterDirection.Input;
                                sqlParametersAssessmentPoint[3] = new SqlParameter();
                                sqlParametersAssessmentPoint[3].ParameterName = "@points";
                                sqlParametersAssessmentPoint[3].Direction = ParameterDirection.Input;

                                if (userPointItem["AssessmentPoints"] != null)
                                {
                                    int currentPoints = 0;
                                    if (userPointItem["AssessmentPoints"] != null && !(userPointItem["AssessmentPoints"] is DBNull))
                                    {
                                        decimal d = decimal.Parse(userPointItem["AssessmentPoints"].ToString());
                                        currentPoints = (int)d;
                                        currentPoints = currentPoints + points;
                                        sqlParametersAssessmentPoint[3].Value = currentPoints;
                                        currentAssessmentPoint = currentPoints;
                                    }
                                    else
                                    {
                                        sqlParametersAssessmentPoint[3].Value = points;
                                        currentAssessmentPoint = points;
                                    }
                                }
                                else
                                {
                                    sqlParametersAssessmentPoint[3].Value = points;
                                    currentAssessmentPoint = points;
                                }

                                if (userPointItem["TrainingPoints"] != null && !(userPointItem["TrainingPoints"] is DBNull))
                                {
                                    currentTrainingPoint = Convert.ToInt32(userPointItem["TrainingPoints"]);
                                }

                                bool ifElevate = false;
                                int competencyLevelOrder = 0;
                                int completionStatus = 0;
                                if (userPointItem["CompletionStatus"] != null && !(userPointItem["CompletionStatus"] is DBNull))
                                {
                                    if (userPointItem["CompletionStatus"].ToString().ToUpper() == "TRUE")
                                        completionStatus = 1;
                                }
                                //////////Check if current competency level is complete////////////
                                if (querySkillCompetencyLevelItems != null && querySkillCompetencyLevelItems.Count > 0)
                                {
                                    int trainingCompletionPoints = 0;
                                    int assessmentCompletionPoints = 0;
                                    trainingCompletionPoints = querySkillCompetencyLevelItems[0].TrainingCompletionPoints;

                                    assessmentCompletionPoints = querySkillCompetencyLevelItems[0].AssessmentCompletionPoints;
                                    if (currentAssessmentPoint >= assessmentCompletionPoints && currentTrainingPoint >= trainingCompletionPoints)
                                    {
                                        completionStatus = 1;
                                        competencyLevelOrder = querySkillCompetencyLevelItems[0].CompetencyLevelOrder;
                                        ifElevate = true;
                                    }
                                }
                                sqlParametersAssessmentPoint[4] = new SqlParameter();
                                sqlParametersAssessmentPoint[4].ParameterName = "@completionStatus";
                                sqlParametersAssessmentPoint[4].Value = completionStatus;
                                sqlParametersAssessmentPoint[4].Direction = ParameterDirection.Input;
                                dh.ExecuteNonQuery("[dbo].[proc_UpdateAssessmentPoint]", CommandType.StoredProcedure, sqlParametersAssessmentPoint);

                                /////Elevate the user to next competency level/////
                                if (ifElevate)
                                {
                                    List<Competence> skillCompItems = allCompetencyLevelItems.Where(c => c.CompetencyLevelOrder == (competencyLevelOrder + 1) && c.SkillId == userSkillId).ToList();
                                    if (skillCompItems != null && skillCompItems.Count > 0)
                                    {
                                        ///////Get record for the skill from UserSkills list/////////
                                        SqlParameter[] sqlParametersUserCompetencyLevel = new SqlParameter[3];
                                        sqlParametersUserCompetencyLevel[0] = new SqlParameter();
                                        sqlParametersUserCompetencyLevel[0].ParameterName = "@userId";
                                        sqlParametersUserCompetencyLevel[0].Value = currentUser.id;
                                        sqlParametersUserCompetencyLevel[0].Direction = ParameterDirection.Input;
                                        sqlParametersUserCompetencyLevel[1] = new SqlParameter();
                                        sqlParametersUserCompetencyLevel[1].ParameterName = "@skillId";
                                        sqlParametersUserCompetencyLevel[1].Value = userSkillId;
                                        sqlParametersUserCompetencyLevel[1].Direction = ParameterDirection.Input;
                                        sqlParametersUserCompetencyLevel[2] = new SqlParameter();
                                        sqlParametersUserCompetencyLevel[2].ParameterName = "@competencyId";
                                        sqlParametersUserCompetencyLevel[2].Value = skillCompItems[0].CompetenceId.ToString();
                                        sqlParametersUserCompetencyLevel[2].Direction = ParameterDirection.Input;
                                        dh.ExecuteNonQuery("[dbo].[proc_UpdateUserCompetencyLevel]", CommandType.StoredProcedure, sqlParametersUserCompetencyLevel);

                                        ////////Assign training and assessment for the elevated competency level///////////////
                                        AddSkillBasedTrainingAssessment(skillCompItems[0].CompetenceId.ToString(), userSkillId, Convert.ToInt32(currentUser.id));

                                        ///////Reset UserPoints record for elevated competency level/////////////
                                        SqlParameter[] sqlParametersAddUserPoint = new SqlParameter[3];
                                        sqlParametersAddUserPoint[0] = new SqlParameter();
                                        sqlParametersAddUserPoint[0].ParameterName = "@userId";
                                        sqlParametersAddUserPoint[0].Value = currentUser.id;
                                        sqlParametersAddUserPoint[0].Direction = ParameterDirection.Input;
                                        sqlParametersAddUserPoint[1] = new SqlParameter();
                                        sqlParametersAddUserPoint[1].ParameterName = "@skillId";
                                        sqlParametersAddUserPoint[1].Value = userSkillId;
                                        sqlParametersAddUserPoint[1].Direction = ParameterDirection.Input;
                                        sqlParametersAddUserPoint[2] = new SqlParameter();
                                        sqlParametersAddUserPoint[2].ParameterName = "@competencyId";
                                        sqlParametersAddUserPoint[2].Value = skillCompItems[0].CompetenceId.ToString();
                                        sqlParametersAddUserPoint[2].Direction = ParameterDirection.Input;
                                        dh.ExecuteNonQuery("[dbo].[proc_AddUserPoint]", CommandType.StoredProcedure, sqlParametersAddUserPoint);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AssessmentResult", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return response;
        }
        private bool AddToEmailQueue(string templateCode, Hashtable dynamicKeyValues, string RecipientTo, string recipientCc)
        {
            string emailSub = string.Empty;
            string emailBody = string.Empty;
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = new DataSet();
            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateCode", SqlDbType.NVarChar) { Value = templateCode, Direction = ParameterDirection.Input  }
            };


            ds = dh.ExecuteDataSet("[dbo].[proc_GetEmailTemplate]", CommandType.StoredProcedure, parameters);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        emailSub = Convert.ToString(item["EmailSubject"]);
                        emailBody = Convert.ToString(item["EmailBody"]);

                        foreach (string key in dynamicKeyValues.Keys)
                        {
                            emailSub = emailSub.Replace("[##" + key + "##]", Convert.ToString(dynamicKeyValues[key]));
                            emailBody = emailBody.Replace("[##" + key + "##]", Convert.ToString(dynamicKeyValues[key]));
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.AddLog("SQLServerDAL,AddToEmailQueue", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                }
            }

            bool status = false;
            string clientName = ConfigurationManager.AppSettings["ClientName"].ToString();
            SendMailRequest objtb = new SendMailRequest();
            objtb.To = RecipientTo;
            objtb.Cc = recipientCc;
            objtb.SenderEmailId = "no-reply@hcl.com";
            objtb.SenderName = "HCL Academy";
            if (!string.IsNullOrEmpty(clientName))
            {
                objtb.SenderName = clientName + " Academy";
            }
            objtb.Subject = emailSub;
            objtb.Body = emailBody;
            Task.Factory.StartNew(() => EmailHelper.SendEmail(objtb, templateCode));
            status = true;
            return status;
        }

        public List<Users> GetUsers()
        {
            List<Users> lstUsers = new List<Users>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUsers]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUsers", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            dv.Sort = "UserName ASC";
            DataTable dt = dv.ToTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Users item = new Users();
                    item.userID = Convert.ToInt32(row["UserID"]);
                    item.userName = row["UserName"].ToString();
                    item.projectName = row["ProjectName"].ToString();
                    lstUsers.Add(item);
                }
            }

            return lstUsers;
        }

        public UserManager GetUsersByID(int id)
        {
            UserManager lstUsers = new UserManager();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUsersByID]", CommandType.StoredProcedure, new SqlParameter("@UserID", id));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUsers", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            DataTable dt = dv.ToTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    lstUsers.DBUserId = Convert.ToInt32(row["UserID"]);
                    lstUsers.UserName = row["UserName"].ToString();
                    lstUsers.EmailID = row["Email"].ToString();
                }
            }

            return lstUsers;
        }

        public bool UpdateProjectData(AssignUser objUserOnboard)
        {
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId",SqlDbType.Int),
                    new SqlParameter("@ProjectId",SqlDbType.Int ),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = objUserOnboard.selectedUser;
                parameters[1].Value = objUserOnboard.selectedProject;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Size = 4000;
                parameters[3].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateProjectData]", CommandType.StoredProcedure, parameters);
                result = true;
                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog("SqlSvrDAL, UpdateProject", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.Service", currentUser.id.ToString());

                    result = false;
                }

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public List<WikiPolicies> GetAllWikiPolicies()
        {
            List<WikiPolicies> wikiPol = new List<WikiPolicies>();
            return wikiPol;
        }

        public List<Skills> GetSkills()
        {
            List<Skills> allSkills = new List<Skills>();
            List<Skill> skills = GetAllSkills();
            List<Competence> competencies = GetAllCompetenceList();
            List<SkillCompetencies> trainings = new List<SkillCompetencies>();
            trainings = GetSkillCompetencyTraingings();
            if (competencies != null && competencies.Count() > 0)
            {
                foreach (Skill skill in skills)
                {
                    Skills objSkill = new Skills();
                    objSkill.skillName = skill.SkillName;

                    List<Competence> competenciesBySkill = competencies.Where(i => i.SkillId == skill.SkillId).ToList();
                    List<Competence> competenciesBySkillOrdered = competenciesBySkill.OrderBy(x => x.CompetencyLevelOrder).ToList();

                    if (competenciesBySkillOrdered != null && competenciesBySkillOrdered.Count() > 0)
                    {
                        SkillCompetencies objCompetencyDetail = null;
                        objSkill.competences = new List<SkillCompetencies>();
                        foreach (Competence competency in competenciesBySkillOrdered)
                        {
                            objCompetencyDetail = new SkillCompetencies();
                            List<SkillCompetencies> SelectedTraining = trainings.Where(m => m.SkillId == skill.SkillId && m.CompetenceId == competency.CompetenceId).ToList();
                            var ulHTML = new TagBuilder("ul");
                            StringBuilder output = new StringBuilder();
                            string trainingDescription = "";
                            foreach (SkillCompetencies lstitem in SelectedTraining)
                            {

                                var liHTML = new TagBuilder("li");
                                liHTML.MergeAttribute("style", "list-style-type:disc;margin-left:15px");
                                liHTML.SetInnerText(lstitem.TrainingName);
                                output.Append(liHTML.ToString());
                                trainingDescription += lstitem.TrainingDescription + System.Environment.NewLine;
                            }
                            ulHTML.InnerHtml = output.ToString();
                            objCompetencyDetail.CompetenceId = competency.CompetenceId;
                            objCompetencyDetail.CompetenceName = competency.CompetenceName;
                            objCompetencyDetail.Description = competency.Description;
                            objCompetencyDetail.SkillId = competency.SkillId;
                            objCompetencyDetail.TrainingName = ulHTML.ToString();
                            objCompetencyDetail.TrainingDescription = trainingDescription;
                            objSkill.competences.Add(objCompetencyDetail);
                        }
                    }
                    allSkills.Add(objSkill);
                }
            }
            return allSkills;
        }

        public List<Result> Search(string keyword)
        {
            List<Result> lstResult = null;
            return lstResult;
        }

        public HeatMapProjectDetail GetHeatMapProjectDetailByProjectID(int projectID)
        {
            DataSet ds = new DataSet();
            HeatMapProjectDetail heatMapProjectDetail = new HeatMapProjectDetail();
            heatMapProjectDetail.id = projectID;
            heatMapProjectDetail.competencyLevel = "Novice";
            String ProjectName = String.Empty;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetProjectById]", CommandType.StoredProcedure, new SqlParameter("@ProjectID", projectID));
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetHeatMapProjectDetailByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            DataView dv = new DataView();
            if (ds.Tables.Count > 0)
                dv = new DataView(ds.Tables[0]);
            DataTable dt = dv.ToTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                heatMapProjectDetail.projectName = dt.Rows[0]["Title"].ToString();

            }
            if (ProjectName != String.Empty)
            {
                heatMapProjectDetail.projectName = ProjectName;
            }
            return heatMapProjectDetail;
        }
        public List<int> GetUserRole()
        {
            List<int> userRoleList = new List<int>();
            try
            {
                DataSet dsRoles = new DataSet();
                DataView dvRoles = new DataView();
                DataHelper dhRoles = new DataHelper(strConnectionString);
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserID";
                sqlParameters[0].Value = currentUser.id;
                sqlParameters[0].Direction = ParameterDirection.Input;
                dsRoles = dhRoles.ExecuteDataSet("[dbo].[proc_GetOnBoardedUserRoles]", CommandType.StoredProcedure, sqlParameters);
                if (dsRoles.Tables.Count > 0)
                    dvRoles = new DataView(dsRoles.Tables[0]);
                DataTable dtRoles = dvRoles.ToTable();
                if (dtRoles != null && dtRoles.Rows.Count > 0)
                {
                    foreach (DataRow row in dtRoles.Rows)
                    {
                        var userRoleValue = row["RoleName"].ToString();
                        int userRoleId = Convert.ToInt32(row["RoleID"]);
                        userRoleList.Add(userRoleId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return userRoleList;
        }
        public List<SiteMenu> GetMenu(int roleid)
        {
            List<SiteMenu> siteMenu = null;
            try
            {
                siteMenu = GetMenuList(roleid);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetMenu", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            return siteMenu;
        }
        public int GetUserId(string emailId)
        {
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter();
            sqlParameters[0].ParameterName = "@emailAddress";
            sqlParameters[0].Value = emailId;
            sqlParameters[0].Direction = ParameterDirection.Input;
            object userId = dh.ExecuteScalar("[dbo].[proc_GetUserId]", CommandType.StoredProcedure, sqlParameters);
            return Convert.ToInt32(userId);
        }
        public bool SaveEvent(AcademyEvent ev)
        {
            List<AcademyEvent> list = new List<AcademyEvent>();
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = new DataSet();
            SqlParameter[] sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter();
            sqlParameters[0].ParameterName = "@id";
            sqlParameters[0].Value = ev.id;
            sqlParameters[0].Direction = ParameterDirection.Input;

            sqlParameters[1] = new SqlParameter();
            sqlParameters[1].ParameterName = "@title";
            sqlParameters[1].Value = ev.title;
            sqlParameters[1].Direction = ParameterDirection.Input;

            sqlParameters[2] = new SqlParameter();
            sqlParameters[2].ParameterName = "@location";
            sqlParameters[2].Value = ev.location;
            sqlParameters[2].Direction = ParameterDirection.Input;

            sqlParameters[3] = new SqlParameter();
            sqlParameters[3].ParameterName = "@starttime";
            sqlParameters[3].Value = ev.eventDate;
            sqlParameters[3].Direction = ParameterDirection.Input;

            sqlParameters[4] = new SqlParameter();
            sqlParameters[4].ParameterName = "@endtime";
            sqlParameters[4].Value = ev.endDate;
            sqlParameters[4].Direction = ParameterDirection.Input;

            sqlParameters[5] = new SqlParameter();
            sqlParameters[5].ParameterName = "@description";
            sqlParameters[5].Value = ev.description;
            sqlParameters[5].Direction = ParameterDirection.Input;

            int result = dh.ExecuteNonQuery("[dbo].[proc_SaveEvent]", CommandType.StoredProcedure, sqlParameters);

            if (result > 0)
                return true;
            else
                return false;
        }
        public bool DeleteEvent(int id)
        {

            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter();
            sqlParameters[0].ParameterName = "@id";
            sqlParameters[0].Value = id;
            sqlParameters[0].Direction = ParameterDirection.Input;
            int result = dh.ExecuteNonQuery("[dbo].[proc_DeleteEvent]", CommandType.StoredProcedure, sqlParameters);

            if (result > 0)
                return true;
            else
                return false;
        }
        public List<AcademyEvent> GetEvents()
        {
            List<AcademyEvent> list = new List<AcademyEvent>();
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = new DataSet();
            ds = dh.ExecuteDataSet("[dbo].[proc_GetAcademyEvents]", CommandType.StoredProcedure);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    AcademyEvent ae = new AcademyEvent();
                    ae.title = row["Title"].ToString();
                    ae.location = row["Location"].ToString();
                    ae.description = row["Description"].ToString();

                    ae.eventDate = Convert.ToDateTime(row["StartTime"].ToString());
                    ae.endDate = Convert.ToDateTime(row["EndTime"].ToString());
                    ae.id = Convert.ToInt32(row["ID"].ToString());
                    list.Add(ae);
                }
            }

            return list;
        }
        public List<SiteMenu> GetMenuList(int roleid)
        {
            List<SiteMenu> siteMenu = null;
            List<int> userRoleList = new List<int>();
            List<SiteMenu> roleBasedsiteMenu = new List<SiteMenu>();
            List<UserRole> userRoles = new List<UserRole>();
            DataHelper dhRoles = new DataHelper(strConnectionString);
            try
            {
                DataSet dsRoles = new DataSet();
                DataView dvRoles = new DataView();
                DataSet dsMenu = new DataSet();
                DataView dvMenu = new DataView();
                dsMenu = dhRoles.ExecuteDataSet("[dbo].[proc_GetMenu]", CommandType.StoredProcedure, new SqlParameter("@roleid", roleid));
                if (dsMenu.Tables.Count > 0)
                    dvMenu = new DataView(dsMenu.Tables[0]);
                if (siteMenu == null)
                {
                    siteMenu = new List<SiteMenu>();
                    DataTable dtMenu = dvMenu.ToTable();

                    if (dtMenu != null && dtMenu.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtMenu.Rows)
                        {
                            SiteMenu item = new SiteMenu();
                            item.ItemId = Int32.Parse(row["ID"].ToString());
                            item.ItemName = row["Title"].ToString();
                            item.ItemOrdering = Convert.ToInt32(row["Ordering"].ToString());
                            item.ParentItemId = Convert.ToInt32(row["ParentMenu"].ToString());
                            item.ItemTarget = row["Target"].ToString();
                            item.ItemHidden = row["Hidden"].ToString();
                            item.ItemURL = row["ControllerView"].ToString();
                            item.UserRole = new List<UserRole>();
                            UserRole userRole = new UserRole();
                            userRole.RoleId = Convert.ToInt32(row["RoleId"].ToString());
                            userRole.RoleName = row["RoleName"].ToString();
                            item.UserRole.Add(userRole);

                            if (item.ItemName.ToUpper() == "ADMIN" || item.ItemName.ToUpper() == "MASTER DATA")
                            {
                                //string spPMOGroup = ConfigurationManager.AppSettings["AcademyPMO"].ToString();
                                //if (currentUser.Groups.Exists(t => t == spPMOGroup))
                                if (currentUser.GroupPermission > 2)
                                {
                                    siteMenu.Add(item);
                                }
                            }
                            else
                            {
                                siteMenu.Add(item);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetMenuList", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhRoles != null)
                {
                    if (dhRoles.DataConn != null)
                    {
                        dhRoles.DataConn.Close();
                    }
                }
            }
            return siteMenu;
        }

        public UserManager GetCurrentUserCompleteUserProfile()
        {
            UserManager user = null;
            return user;
        }

        public List<Banners> GetBanners()
        {
            List<Banners> bannersList = new List<Banners>();
            return bannersList;
        }

        public List<Skill> GetAllSkills()
        {
            List<Skill> skills = null;
            DataHelper dh = new DataHelper(strConnectionString);
            if (skills == null)
            {
                skills = new List<Skill>();
                try
                {
                    DataSet ds = new DataSet();
                    DataView dv = new DataView();
                    ds = dh.ExecuteDataSet("[dbo].[proc_GetAllSkills]", CommandType.StoredProcedure);
                    if (ds.Tables.Count > 0)
                        dv = new DataView(ds.Tables[0]);
                    DataTable dt = dv.ToTable();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["IsDefault"].ToString().ToUpper() == "FALSE")
                            {
                                Skill item = new Skill();
                                item.SkillId = Int32.Parse(row["ID"].ToString());
                                item.SkillName = row["Title"].ToString();
                                //item.Skills = ??? 
                                skills.Add(item);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.AddLog("SQLServerDAL,GetAllSkills", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                }
                finally
                {
                    if (dh != null)
                    {
                        if (dh.DataConn != null)
                        {
                            dh.DataConn.Close();
                        }
                    }
                }
            }
            return skills;
        }

        public TrainingReport GetTrainingsReport(int skillid, int competencyid, int projectid)
        {
            TrainingReport trainingReport = new TrainingReport();
            try
            {
                List<UserDetails> currentUserList = new List<UserDetails>();
                DateTime today = new DateTime();
                today = DateTime.Now;
                trainingReport.userDetails = GetUserDetails(skillid, competencyid, projectid);
                trainingReport.counts = GetTrainingCounts(trainingReport.userDetails);
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetTrainingsReport", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            return trainingReport;
        }

        public List<RSSFeed> GetRSSFeeds()
        {
            List<RSSFeed> rSSFeeds = new List<RSSFeed>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetRssFeeds]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        XmlDocument rssXmlDoc = new XmlDocument();

                        rssXmlDoc.Load(Convert.ToString(item["RssFeedUrl"]));

                        // Parse the Items in the RSS file
                        XmlNodeList rssNodes = rssXmlDoc.SelectNodes(Convert.ToString(item["ItemNodePath"]));

                        // Iterate through the items in the RSS file
                        foreach (XmlNode rssNode in rssNodes)
                        {
                            RSSFeed rs = new RSSFeed();
                            XmlNode rssSubNode = rssNode.SelectSingleNode(Convert.ToString(item["TitleNodePath"])); //for Node Title text
                            rs.TitleNode = rssSubNode != null ? rssSubNode.InnerText : "";


                            rssSubNode = rssNode.SelectSingleNode(Convert.ToString(item["hrfTitleNodePath"]));  //for Node hyperlink on Node Title text
                            rs.LinkNode = rssSubNode != null ? rssSubNode.InnerText : "";

                            rssSubNode = rssNode.SelectSingleNode(Convert.ToString(item["DescNodePath"])); //for Node Description text
                            rs.DescriptionNode = rssSubNode != null ? rssSubNode.InnerText : "";

                            string TrimedDespt = Utilities.Truncate(HtmlToPlainText(Convert.ToString(item["DescNodePath"])), 25);
                            rssSubNode = rssNode.SelectSingleNode(TrimedDespt); //for Node Description text
                            rs.TrimedDescription = rssSubNode != null ? rssSubNode.InnerText : "";

                            rssSubNode = rssNode.SelectSingleNode(Convert.ToString(item["PubDateNodePath"])); //for Publish date value
                            rs.PubDateNode = rssSubNode != null ? rssSubNode.InnerText : "";

                            string strTitle = Convert.ToString(item["Title"]);    // for RssFeed Title
                            rs.Title = strTitle != null ? strTitle : "";
                            rSSFeeds.Add(rs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetRSSFeeds", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return rSSFeeds;
        }

        public List<RSSFeedMaster> GetAllRSSFeeds()
        {
            List<RSSFeedMaster> rSSFeeds = new List<RSSFeedMaster>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetRssFeeds]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RSSFeedMaster rSSFeed = new RSSFeedMaster();
                        rSSFeed.ID = Convert.ToInt32(dr["ID"]);
                        rSSFeed.Title = dr["Title"].ToString();
                        rSSFeed.TitleNode = dr["TitleNodePath"].ToString();
                        rSSFeed.DescriptionNode = dr["DescNodePath"].ToString();
                        rSSFeed.itemNodePath = dr["ItemNodePath"].ToString();
                        rSSFeed.RSSFeedUrl = dr["RssFeedUrl"].ToString();
                        rSSFeed.PubDateNode = dr["PubDateNodePath"].ToString();
                        rSSFeed.rssFeedOrder = Convert.ToInt32(dr["RssFeedOrder"]);
                        rSSFeed.hrfTitleNodePath = dr["hrfTitleNodePath"].ToString();
                        rSSFeeds.Add(rSSFeed);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllRSSFeeds", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return rSSFeeds;
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            return text;
        }

        public List<News> GetNews(string NoImagePath)
        {
            List<News> newsEvents = new List<News>();
            return newsEvents;
        }

        public List<News> GetNewsFromDB()
        {
            List<News> newsEvents = new List<News>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetNews]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        News news = new News();
                        news.ID = Convert.ToInt32(dr["ID"]);
                        news.header = dr["Header"].ToString();
                        news.imageURL = dr["imageURL"].ToString();
                        news.body = dr["Body"].ToString();
                        news.trimmedBody = dr["TrimmedBody"].ToString();
                        newsEvents.Add(news);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetNewsFromDB", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return newsEvents;

        }

        public List<News> GetNewsEventByID(int id)
        {
            List<News> newsEvents = new List<News>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                };

                ds = dh.ExecuteDataSet("[dbo].[proc_GetNewsByID]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        News news = new News();
                        news.header = dr["Header"].ToString();
                        news.imageURL = dr["imageURL"].ToString();
                        news.body = dr["Body"].ToString();
                        news.trimmedBody = dr["TrimmedBody"].ToString();
                        newsEvents.Add(news);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllRSSFeeds", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return newsEvents;

        }

        public List<TrainingPlan> GetTrainingPlans(int id)
        {
            List<TrainingPlan> trainingPlans = new List<TrainingPlan>();
            return trainingPlans;
        }

        public List<UserTraining> GetTrainingForUser(int userId, bool OnlyOnBoardedTraining = false)
        {
            List<UserTraining> trainings = new List<UserTraining>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userID", userId) };
                ds = dh.ExecuteDataSet("[dbo].[proc_GetTrainingDetails]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    UserTraining objTraining = null;
                    foreach (DataRow item in dt.Rows)
                    {
                        objTraining = new UserTraining();

                        if (item["CompletedDate"] != null && !(item["CompletedDate"] is DBNull))
                            objTraining.CompletedDate = Convert.ToString(item["CompletedDate"]);
                        else
                            objTraining.CompletedDate = "";
                        if (item["IsIncludeOnBoarding"] != null && !(item["IsIncludeOnBoarding"] is DBNull))
                            objTraining.IsIncludeOnBoarding = Convert.ToBoolean(item["IsIncludeOnBoarding"]);
                        if (item["IsMandatory"] != null && !(item["IsMandatory"] is DBNull))
                            objTraining.IsMandatory = Convert.ToBoolean(item["IsMandatory"]);
                        if (item["IsTrainingActive"] != null && !(item["IsTrainingActive"] is DBNull))
                            objTraining.IsTrainingActive = Convert.ToBoolean(item["IsTrainingActive"]);
                        if (item["IsTrainingCompleted"] != null && !(item["IsTrainingCompleted"] is DBNull))
                            objTraining.IsTrainingCompleted = Convert.ToBoolean(item["IsTrainingCompleted"]);
                        if (item["LastDayCompletion"] != null && !(item["LastDayCompletion"] is DBNull))
                            objTraining.LastDayCompletion = Convert.ToDateTime(item["LastDayCompletion"].ToString()).ToShortDateString();
                        if (item["Skill"] != null && !(item["Skill"] is DBNull))
                            objTraining.SkillName = item["Skill"] != null ? item["Skill"].ToString() : "";
                        if (item["SkillID"] != null && !(item["SkillID"] is DBNull))
                            objTraining.SkillId = item["SkillID"] != null ? (Convert.ToInt32(item["SkillID"])) : 0;
                        if (item["TrainingName"] != null && !(item["TrainingName"] is DBNull))
                            objTraining.TrainingName = item["TrainingName"] != null ? ((item["TrainingName"]).ToString()) : "";
                        if (item["TrainingID"] != null && !(item["TrainingID"] is DBNull))
                            objTraining.TrainingId = item["TrainingID"] != null ? (Convert.ToInt32(item["TrainingID"])) : 0;
                        if (objTraining.IsTrainingCompleted)
                        {
                            objTraining.StatusColor = AppConstant.Green;
                            objTraining.ItemStatus = AppConstant.Completed;
                        }
                        else if (objTraining.IsTrainingCompleted == false && Convert.ToDateTime(objTraining.LastDayCompletion) < DateTime.Now)
                        {
                            objTraining.StatusColor = AppConstant.Red;
                            objTraining.ItemStatus = AppConstant.OverDue;
                        }
                        else
                        {
                            objTraining.StatusColor = AppConstant.Amber;
                            objTraining.ItemStatus = AppConstant.InProgress;
                        }
                        trainings.Add(objTraining);
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetTrainingForUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public List<OnBoarding> GetBoardingData()
        {
            List<OnBoarding> onBoardings = new List<OnBoarding>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet dsUserTrainings = new DataSet();
                DataView dvUserTrainings = new DataView();
                DataSet dsRoleTrainings = new DataSet();
                DataView dvRoleTrainings = new DataView();
                SqlParameter[] sqlParameters = new SqlParameter[2];

                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@userID";
                sqlParameters[0].Value = currentUser.id;
                sqlParameters[0].Direction = ParameterDirection.Input;

                sqlParameters[1] = new SqlParameter();
                sqlParameters[1].ParameterName = "@roleBasedTraining";
                sqlParameters[1].Value = 0;
                sqlParameters[1].Direction = ParameterDirection.Input;
                dsUserTrainings = dh.ExecuteDataSet("[dbo].[proc_GetOnBoardingData]", CommandType.StoredProcedure, sqlParameters);
                if (dsUserTrainings.Tables.Count > 0)
                    dvUserTrainings = new DataView(dsUserTrainings.Tables[0]);

                SqlParameter[] sqlParametersRoleTraining = new SqlParameter[2];

                sqlParametersRoleTraining[0] = new SqlParameter();
                sqlParametersRoleTraining[0].ParameterName = "@userID";
                sqlParametersRoleTraining[0].Value = currentUser.id;
                sqlParametersRoleTraining[0].Direction = ParameterDirection.Input;

                sqlParametersRoleTraining[1] = new SqlParameter();
                sqlParametersRoleTraining[1].ParameterName = "@roleBasedTraining";
                sqlParametersRoleTraining[1].Value = 1;
                sqlParametersRoleTraining[1].Direction = ParameterDirection.Input;

                dsRoleTrainings = dh.ExecuteDataSet("[dbo].[proc_GetOnBoardingData]", CommandType.StoredProcedure, sqlParametersRoleTraining);
                if (dsRoleTrainings.Tables.Count > 0)
                    dvRoleTrainings = new DataView(dsRoleTrainings.Tables[0]);

                DataTable dtUserTrainings = dvUserTrainings.ToTable();
                DataTable dtRoleTrainings = dvRoleTrainings.ToTable();
                bool trainingStatus = false;
                if (dtUserTrainings != null && dtUserTrainings.Rows.Count > 0)
                {
                    foreach (DataRow row in dtUserTrainings.Rows)
                    {
                        trainingStatus = false;
                        if (!String.IsNullOrEmpty(row["IsTrainingCompleted"].ToString()) && !(row["IsTrainingCompleted"] is DBNull))
                        {
                            if (row["IsTrainingCompleted"].ToString().ToUpper() == "TRUE")
                            {
                                trainingStatus = true;
                            }
                        }

                        OnBoarding item = new OnBoarding();
                        item.boardingItemId = Convert.ToInt32(row["ID"]);
                        if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()))
                            item.boardingStatus = Utilities.GetOnBoardingStatus(trainingStatus, 0, Convert.ToDateTime(row["LastDayCompletion"]));
                        item.boardingType = OnboardingItemType.Training;
                        if (!String.IsNullOrEmpty(row["Training"].ToString()))
                        {
                            item.boardingItemName = row["Training"].ToString();
                        }
                        item.boardIngTrainingId = Convert.ToInt32(row["TrainingID"]);
                        onBoardings.Add(item);
                    }
                }
                if (dtRoleTrainings != null && dtRoleTrainings.Rows.Count > 0)
                {
                    foreach (DataRow row in dtRoleTrainings.Rows)
                    {
                        OnBoarding item = new OnBoarding();
                        item.boardingItemId = Convert.ToInt32(row["ID"]);
                        trainingStatus = false;
                        if (!String.IsNullOrEmpty(row["IsTrainingCompleted"].ToString()) && !(row["IsTrainingCompleted"] is DBNull))
                        {
                            if (row["IsTrainingCompleted"].ToString().ToUpper() == "TRUE")
                            {
                                trainingStatus = true;
                            }
                        }
                        if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()))
                        {
                            item.boardingStatus = Utilities.GetOnBoardingStatus(trainingStatus, 0, Convert.ToDateTime(row["LastDayCompletion"]));
                        }
                        if (!String.IsNullOrEmpty(row["Training"].ToString()))
                        {
                            item.boardingItemName = row["Training"].ToString();
                        }
                        item.boardingType = OnboardingItemType.RoleTraining;
                        item.boardIngTrainingId = Convert.ToInt32(row["TrainingID"]);
                        onBoardings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetBoardingData", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return onBoardings;
        }


        public List<UserSkill> GetUserSkillsOfCurrentUser()
        {
            List<UserSkill> lstSkills = new List<UserSkill>();

            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserSkillsForCurrentUser]", CommandType.StoredProcedure, new SqlParameter("@UserID", currentUser.id));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserSkill item = new UserSkill();
                        item.Id = Int32.Parse(row["ID"].ToString());
                        item.Competence = row["CompetencyLevel"].ToString().ToUpper();
                        item.SkillId = Int32.Parse(row["SkillID"].ToString());
                        item.Skill = row["SkillName"].ToString();
                        lstSkills.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserSkillsOfCurrentUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return lstSkills;
        }

        public List<Competence> GetAllCompetencyLevels()
        {
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            List<Competence> competenceList = new List<Competence>();
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllCompetencyLevels]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Competence item = new Competence();
                            if (row["ID"] != null && !(row["ID"] is DBNull))
                                item.CompetenceId = Int32.Parse(row["ID"].ToString());
                            if (row["Level"] != null && !(row["Level"] is DBNull))
                                item.CompetenceName = row["Level"].ToString();

                            competenceList.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllCompetencyLevels", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return competenceList;
        }

        public List<Competence> GetAllCompetenceList()
        {
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            List<Competence> competenceList = new List<Competence>();
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllCompetencies]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Competence item = new Competence();
                            if (row["ID"] != null && !(row["ID"] is DBNull))
                                item.CompetenceId = Int32.Parse(row["ID"].ToString());
                            if (row["CompetenceName"] != null && !(row["CompetenceName"] is DBNull))
                                item.CompetenceName = row["CompetenceName"].ToString();
                            if (row["SkillId"] != null && !(row["SkillId"] is DBNull))
                                item.SkillId = Int32.Parse(row["SkillId"].ToString());
                            if (row["SkillName"] != null && !(row["SkillName"] is DBNull))
                                item.SkillName = row["SkillName"].ToString();
                            if (row["Description"] != null && !(row["Description"] is DBNull))
                                item.Description = row["Description"].ToString();
                            if (row["CompetencyLevelOrder"] != null && !(row["CompetencyLevelOrder"] is DBNull))
                                item.CompetencyLevelOrder = Convert.ToInt32(row["CompetencyLevelOrder"].ToString());
                            competenceList.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllCompetenceList", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return competenceList;
        }

        public List<ProjectSkill> GetAllProjectSkills()
        {
            List<ProjectSkill> projectSkills = new List<ProjectSkill>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllProjectSkills]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProjectSkill item = new ProjectSkill();
                        item.itemId = Convert.ToInt32(row["ID"]);
                        item.project = row["Project"].ToString();
                        item.projectId = Convert.ToInt32(row["ProjectID"].ToString());
                        item.skill = row["Skill"].ToString();
                        item.skillId = Convert.ToInt32(row["SkillID"].ToString());
                        projectSkills.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllProjectSkills", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return projectSkills;
        }

        public bool AddProjectSkill(int ProjectID, int SkillID)
        {
            bool status = false;
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@ProjectId",SqlDbType.Int ) { Value = ProjectID},
                new SqlParameter("@SkillId",SqlDbType.Int ) { Value = SkillID},
                new SqlParameter("@Status",SqlDbType.Bit ) { Value = status,Direction=ParameterDirection.Output}
            };
            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_AddProjectSkill]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[2].Value);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (status)
                result = true;
            return result;
        }

        public List<ProjectSkillResource> GetAllProjectSkillResources()
        {
            List<ProjectSkillResource> projectSkillResources = new List<ProjectSkillResource>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllProjectSkillResources]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProjectSkillResource item = new ProjectSkillResource();
                        item.projectName = row["Project"].ToString();
                        item.projectId = Convert.ToInt32(row["ProjectId"]);
                        item.skill = row["Skill"].ToString();
                        item.skillId = Convert.ToInt32(row["SkillId"].ToString());
                        item.competencyLevel = row["CompetencyLevel"].ToString();
                        item.competencyLevelId = Convert.ToInt32(row["CompetencyLevelId"].ToString());
                        item.expectedResourceCount = Convert.ToInt32(row["ExpectedResourceCount"]);
                        item.availableResourceCount = Convert.ToInt32(row["AvailableResourceCount"]);
                        projectSkillResources.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllProjectSkillResources", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return projectSkillResources;
        }

        public bool AddProjectSkillResource(int ProjectID, ProjectSkillResource skillResource)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool status = false;
            bool result = false;
            SqlParameter[] parameters =
           {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = ProjectID },
                new SqlParameter("@SkillId", SqlDbType.Int) { Value = skillResource.skillId  },
                new SqlParameter("@CompetencyLevelId", SqlDbType.Int) { Value = skillResource.competencyLevelId },
                new SqlParameter("@ExpectedResourceCount", SqlDbType.Int) { Value = skillResource.expectedResourceCount },
                new SqlParameter("@AvailableResourceCount", SqlDbType.Int) { Value =skillResource.availableResourceCount },
                new SqlParameter("@Status", SqlDbType.Bit) { Value =status,Direction=ParameterDirection.Output }

            };
            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_AddProjectSkillResource]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[5].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }
        public List<ProjectSkillResource> GetAllProjectSkillResourcesByProjectID(int ProjectID)
        {
            List<ProjectSkillResource> projectSkillResources = new List<ProjectSkillResource>();
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                DataHelper dh = new DataHelper(strConnectionString);

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ProjectName",SqlDbType.VarChar)
                };
                parameters[0].Value = ProjectID;
                parameters[1].Size = 255;
                parameters[1].Direction = ParameterDirection.Output;
                try
                {
                    ds = dh.ExecuteDataSet("[dbo].[proc_GetAvailableSkillResourceCountByProjectId]", CommandType.StoredProcedure, parameters);
                    if (ds.Tables.Count > 0)
                        dv = new DataView(ds.Tables[0]);
                }
                finally
                {
                    if (dh != null)
                    {
                        if (dh.DataConn != null)
                        {
                            dh.DataConn.Close();
                        }
                    }
                }

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProjectSkillResource item = new ProjectSkillResource();
                        item.projectName = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
                        item.projectId = Convert.ToInt32(row["ProjectID"]);
                        item.skill = row["SkillName"].ToString();
                        item.skillId = Convert.ToInt32(row["SkillId"].ToString());
                        item.competencyLevel = row["CompetencyLevel"].ToString();
                        item.competencyLevelId = Convert.ToInt32(row["CompetencyLevelId"].ToString());
                        item.expectedResourceCount = Convert.ToInt32(row["ExpectedResourceCount"]);
                        item.availableResourceCount = Convert.ToInt32(row["AvailableResourceCount"]);
                        projectSkillResources.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllProjectSkillResourcesByProjectID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return projectSkillResources;
        }
        public List<TrainingAssignment> GetTrainingAssigments(int skillId, int trainingId, int projectId)
        {
            List<TrainingAssignment> trainingAssignments = new List<TrainingAssignment>();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
               {
                   new SqlParameter("@skillId",SqlDbType.Int),
                    new SqlParameter("@trainingId",SqlDbType.Int),
                    new SqlParameter("@projectId",SqlDbType.Int)
                };
            parameters[0].Value = skillId;
            parameters[1].Value = trainingId;
            parameters[2].Value = projectId;
            DataSet ds = dh.ExecuteDataSet("[dbo].[proc_gettrainingassignments]", CommandType.StoredProcedure, parameters);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            TrainingAssignment assignment = new TrainingAssignment();

                            if (ds.Tables[0].Rows[i]["Title"] != null)
                                assignment.TrainingName = ds.Tables[0].Rows[i]["Title"].ToString();
                            if (ds.Tables[0].Rows[i]["TrainingId"] != null)
                                assignment.TrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingId"].ToString());
                            if (ds.Tables[0].Rows[i]["EmployeeId"] != null && !(ds.Tables[0].Rows[i]["EmployeeId"] is DBNull))
                                assignment.EmployeeId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeId"].ToString());
                            if (ds.Tables[0].Rows[i]["EmailAddress"] != null)
                                assignment.EmailAddress = ds.Tables[0].Rows[i]["EmailAddress"].ToString();
                            if (ds.Tables[0].Rows[i]["Name"] != null)
                                assignment.UserName = ds.Tables[0].Rows[i]["Name"].ToString();
                            if (ds.Tables[0].Rows[i]["UserId"] != null)
                                assignment.UserId = Convert.ToInt32(ds.Tables[0].Rows[i]["UserId"].ToString());
                            trainingAssignments.Add(assignment);
                        }

                    }

                }

            }
            return trainingAssignments;

        }

        public List<OnBoarding> GetBoardingDataFromOnboarding()
        {
            List<OnBoarding> boardingList = new List<OnBoarding>();
            string userName = String.Empty;
            var showDefaultTraining = "NO";
            var showSkillBasedTraining = "NO";
            var showRoleBasedTraining = "NO";
            var showAssessments = "NO";

            try
            {
                DataSet dsConfig = new DataSet();
                DataView dvConfig = new DataView();
                DataHelper dh = new DataHelper(strConnectionString);
                dsConfig = dh.ExecuteDataSet("[dbo].[proc_GetConfigItems]", CommandType.StoredProcedure);
                if (dsConfig.Tables.Count > 0)
                    dvConfig = new DataView(dsConfig.Tables[0]);

                DataTable dt = dvConfig.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string Title = String.Empty;
                        string Value = String.Empty;
                        Title = row["Title"].ToString();
                        Value = row["Value"].ToString();
                        if (Title == "ShowUserAssessments")
                        {
                            showAssessments = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowDefaultTraining")
                        {
                            showDefaultTraining = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowSkillBasedTraining")
                        {
                            showSkillBasedTraining = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowRoleBasedTraining")
                        {
                            showRoleBasedTraining = Value.ToUpper().ToString();
                        }
                    }
                }

                DataSet dsOnBoarding = new DataSet();
                DataView dvOnBoarding = new DataView();

                dsOnBoarding = dh.ExecuteDataSet("dbo.proc_GetOnBoardingDataForUser", CommandType.StoredProcedure, new SqlParameter("@userId", currentUser.id));
                if (dsOnBoarding.Tables.Count > 0)
                {
                    dvOnBoarding = new DataView(dsOnBoarding.Tables[0]);
                }

                DataTable dtOnBoarding = new DataTable();
                dtOnBoarding = dvOnBoarding.ToTable();
                foreach (DataRow row in dtOnBoarding.Rows)
                {
                    //if (row["SendEmail"] != null)
                    //{
                    //    if (!String.IsNullOrEmpty(row["SendEmail"].ToString()))
                    //        sendEmail = Convert.ToBoolean(row["SendEmail"]) == true ? true : false;
                    //}


                    int locId = 0;
                    string locValue = "";
                    if (row["GEOID"] != null)
                    {
                        if (!String.IsNullOrEmpty(row["GEOID"].ToString()))
                            locId = Convert.ToInt32(row["GEOID"]);
                    }
                    if (row["GEOID"] != null)
                    {
                        if (!String.IsNullOrEmpty(row["GEO"].ToString()))
                            locValue = row["GEO"].ToString();
                    }
                    var checkList = GetOnBoardingCheckList(locValue, locId);
                    DataSet dsUser = new DataSet();
                    DataView dvUser = new DataView();
                    dsUser = dh.ExecuteDataSet("[dbo].[proc_GetUserCheckLists]", CommandType.StoredProcedure, new SqlParameter("@userId", currentUser.id));
                    dvUser = new DataView(dsUser.Tables[0]);
                    DataTable dtUsers = new DataTable();
                    dtUsers = dvUser.ToTable();
                    List<UserCheckList> lstUserCheckList = new List<UserCheckList>();
                    if (dtUsers != null && dtUsers.Rows.Count > 0)
                    {
                        foreach (DataRow data in dtUsers.Rows)
                        {
                            UserCheckList item = new UserCheckList();
                            item.Id = Convert.ToInt32(data["ID"].ToString());
                            if (data["CheckList"] != null)
                            {
                                if (!String.IsNullOrEmpty(data["CheckList"].ToString()))
                                    item.CheckList = data["CheckList"].ToString();
                            }
                            if (data["CheckListStatus"] != null)
                            {
                                if (!String.IsNullOrEmpty(data["CheckListStatus"].ToString()))
                                    item.CheckListStatus = data["CheckListStatus"].ToString();
                            }
                            lstUserCheckList.Add(item);
                        }
                    }

                    //////////Add Checklist items/////////////////
                    for (int i = 0; i < lstUserCheckList.Count; i++)
                    {
                        List<CheckListItem> itemCheckList = checkList.Where(c => c.id == Convert.ToInt32(lstUserCheckList[i].CheckList)).ToList();
                        if (itemCheckList.Count > 0)
                        {
                            boardingList.Add(new OnBoarding
                            {
                                boardingItemId = lstUserCheckList[i].Id,
                                boardingItemName = itemCheckList[0].name,
                                boardingItemDesc = itemCheckList[0].desc,
                                boardingStatus = GetStatus(lstUserCheckList[i].CheckListStatus),
                                boardingInternalName = itemCheckList[0].internalName
                            });
                        }
                    }

                    ///////Add items for default and skill based training assignments////////
                    if (showDefaultTraining == "YES" || showSkillBasedTraining == "YES")
                    {
                        List<UserTrainingDetail> userTrainings = GetTrainingDetails();
                        ArrayList defaultTrainingIds = new ArrayList();
                        if (showDefaultTraining == "YES")
                        {
                            DataSet dsDefaultTraining = GetDefaultTraining();
                            if (dsDefaultTraining.Tables.Count > 0)
                            {
                                if (dsDefaultTraining.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsDefaultTraining.Tables[0].Rows.Count; i++)
                                    {
                                        if (dsDefaultTraining.Tables[0].Rows[i]["TrainingId"] != null)
                                        {
                                            defaultTrainingIds.Add(dsDefaultTraining.Tables[0].Rows[i]["TrainingId"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < userTrainings.Count; i++)
                        {
                            if (showDefaultTraining == "YES" && defaultTrainingIds.Contains(userTrainings[i].TrainingId))
                            {
                                boardingList.Add(new OnBoarding
                                {
                                    boardingItemId = Convert.ToInt32(userTrainings[i].TrainingId),
                                    boardingItemName = Convert.ToString(userTrainings[i].TrainingName),
                                    boardingItemDesc = Convert.ToString(userTrainings[i].ModuleDesc),
                                    boardingItemLink = userTrainings[i].LinkUrl,
                                    boardingStatus = Utilities.GetOnBoardingStatus(userTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(userTrainings[i].LastDayToComplete)),
                                    boardingType = OnboardingItemType.Training,
                                    boardIngTrainingId = userTrainings[i].TrainingId,
                                    boardingIsMandatory = Convert.ToBoolean(userTrainings[i].Mandatory)
                                });
                            }
                            else if (showSkillBasedTraining == "YES" && !defaultTrainingIds.Contains(userTrainings[i].TrainingId))
                            {
                                boardingList.Add(new OnBoarding
                                {
                                    boardingItemId = Convert.ToInt32(userTrainings[i].TrainingId),
                                    boardingItemName = Convert.ToString(userTrainings[i].TrainingName),
                                    boardingItemDesc = Convert.ToString(userTrainings[i].ModuleDesc),
                                    boardingItemLink = userTrainings[i].LinkUrl,
                                    boardingStatus = Utilities.GetOnBoardingStatus(userTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(userTrainings[i].LastDayToComplete)),
                                    boardingType = OnboardingItemType.Training,
                                    boardIngTrainingId = userTrainings[i].TrainingId,
                                    boardingIsMandatory = Convert.ToBoolean(userTrainings[i].Mandatory)
                                });

                            }

                        }
                    }


                    //////////Add Assessment items/////////////////
                    if (showAssessments == "YES")
                    {
                        GetUserAssessments(boardingList);
                    }

                    //////////Add Role Based items/////////////////
                    if (showRoleBasedTraining == "YES")
                    {
                        List<UserTrainingDetail> roleTrainings = new List<UserTrainingDetail>();
                        GetUserRoleBasedTraining(ref roleTrainings, currentUser.id);
                        for (int i = 0; i < roleTrainings.Count; i++)
                        {
                            OnBoarding boardingItem = new OnBoarding();
                            boardingItem.boardingItemId = roleTrainings[i].Id;
                            boardingItem.boardingItemName = roleTrainings[i].TrainingName;
                            boardingItem.boardingItemDesc = roleTrainings[i].ModuleDesc;
                            boardingItem.boardingInternalName = roleTrainings[i].TrainingName;
                            boardingItem.boardingIsMandatory = roleTrainings[i].Mandatory;
                            if (roleTrainings[i].IsLink)
                                boardingItem.boardingItemLink = roleTrainings[i].LinkUrl;
                            else
                                boardingItem.boardingItemLink = roleTrainings[i].DocumentUrl;
                            boardingItem.boardingType = OnboardingItemType.RoleTraining;
                            boardingItem.boardingStatus = Utilities.GetOnBoardingStatus(roleTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(roleTrainings[i].CompletionDate));
                            boardingList.Add(boardingItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetBoardingDataFromOnboarding", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return boardingList;
        }

        private OnboardingStatus GetStatus(object value)
        {
            if (Convert.ToString(value).ToLower().Trim() == "completed" || Convert.ToString(value).ToLower().Trim() == "true" || Convert.ToString(value).Trim() == "1")
            {
                return OnboardingStatus.Completed;
            }
            else if (Convert.ToString(value).ToLower().Trim() == "initiated" || Convert.ToString(value).ToLower().Trim() == "false")
            {
                return OnboardingStatus.OnGoing;
            }
            else if (Convert.ToString(value).ToLower().Trim() == "rejected")
            {
                return OnboardingStatus.Rejected;
            }
            return OnboardingStatus.NotStarted;
        }



        public int GetMarketRiskAssessmentID()
        {
            int assessmentId = 0;
            DataHelper dh = new DataHelper(strConnectionString);
            var assesmentId = 0;
            try
            {
                assesmentId = Convert.ToInt32(dh.ExecuteScalar("[dbo].[proc_GetMarketRiskAssesmentId]", CommandType.StoredProcedure, new SqlParameter("@UserID", currentUser.id), new SqlParameter("@AssessmentTitle", "Market Risk")));
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetMarketRiskAssessmentID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return assessmentId;
        }

        public List<object> UpdateOnBoardingStatus(List<OnBoardingTrainingStatus> objs)
        {
            List<object> lstOnboarding = new List<object>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                if (objs != null && objs.Count > 0)
                {
                    foreach (OnBoardingTrainingStatus obj in objs)
                    {
                        if (obj.onboardingType == OnboardingItemType.Training)
                        {
                            SqlParameter[] parameters = new SqlParameter[] {
                                new SqlParameter("@userID",currentUser.id),
                                new SqlParameter("@userTrainingID", obj.id),
                                new SqlParameter("@CompletedDate", DateTime.Now),
                                new SqlParameter("@trainingCompletionStatus", obj.status),
                                new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}
                            };
                            DataHelper dhUpdate = new DataHelper(strConnectionString);
                            dhUpdate.ExecuteNonQuery("[dbo].[proc_UpdateOnboardingTrainingStatus]", CommandType.StoredProcedure, parameters);
                            if (dhUpdate.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                            {
                                lstOnboarding.Add(true);
                                lstOnboarding.Add(obj.id);
                            }
                        }
                        else if (obj.onboardingType == OnboardingItemType.RoleTraining)
                        {
                            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@id", obj.id),
                                new SqlParameter("@trainingCompletionStatus", obj.status),
                                new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                            DataHelper dhUpdate = new DataHelper(strConnectionString);
                            dhUpdate.ExecuteNonQuery("[dbo].[proc_UpdateRoleTrainingStatus]", CommandType.StoredProcedure, parameters);
                            if (dhUpdate.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                            {
                                lstOnboarding.Add(true);
                                lstOnboarding.Add(obj.id);
                            }
                        }
                        else if (obj.onboardingType == OnboardingItemType.Default)
                        {
                            DataHelper dhCheck = new DataHelper(strConnectionString);
                            SqlParameter[] parameters = {new SqlParameter("@UserID",currentUser.id),
                                                         new SqlParameter("@CheckList",obj.id),
                                                         new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}};
                            int ret = dhCheck.ExecuteNonQuery("[dbo].[proc_UpdateCheckListStatus]", CommandType.StoredProcedure, parameters);
                            if (dhCheck.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                            {
                                lstOnboarding.Add(true);
                                lstOnboarding.Add(obj.id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,UpdateOnBoardingStatus", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return lstOnboarding;
        }


        public List<object> UpdateOnBoardingStatus(OnBoardingTrainingStatus obj)
        {

            List<object> lstOnboarding = new List<object>();
            DataHelper dhUpdate = new DataHelper(strConnectionString);
            try
            {
                if (obj.onboardingType == OnboardingItemType.Training)
                {
                    SqlParameter[] parameters = new SqlParameter[] {
                                new SqlParameter("@userID",currentUser.id),
                                new SqlParameter("@userTrainingID", obj.id),
                                new SqlParameter("@CompletedDate", DateTime.Now),
                                new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}
                            };

                    dhUpdate.ExecuteNonQuery("[dbo].[proc_UpdateOnboardingTrainingStatus]", CommandType.StoredProcedure, parameters);
                    if (dhUpdate.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        lstOnboarding.Add(true);
                        lstOnboarding.Add(obj.id);
                    }
                }
                else if (obj.onboardingType == OnboardingItemType.Default)
                {
                    SqlParameter[] parameters = { new SqlParameter("@UserID", currentUser.id), new SqlParameter("@CheckList", obj.id) ,
                                new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}};
                    dhUpdate.ExecuteNonQuery("[dbo].[proc_UpdateCheckListStatus]", CommandType.StoredProcedure, parameters);
                    if (dhUpdate.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        lstOnboarding.Add(true);
                        lstOnboarding.Add(obj.id);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,UpdateOnBoardingStatus", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhUpdate != null)
                {
                    if (dhUpdate.DataConn != null)
                    {
                        dhUpdate.DataConn.Close();
                    }
                }
            }
            return lstOnboarding;
        }

        public bool EmilOnBoardingStatus()
        {
            return false;
        }

        public bool GetOnBoardingProfile()
        {
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                string onboardingid = string.Empty;
                SqlParameter[] parameters =
                {
                        new SqlParameter("@UserId", SqlDbType.Int) { Value =currentUser.id, Direction = ParameterDirection.Input },
                        new SqlParameter("@UserEmail", SqlDbType.NVarChar) { Value = currentUser.emailId, Direction = ParameterDirection.Input }
                    };
                onboardingid = dh.ExecuteScalar("[dbo].[proc_GetOnBoardingProfile]", CommandType.StoredProcedure, parameters).ToString();

                if (onboardingid != String.Empty && onboardingid != null)
                {
                    //currentUser.OnBoardingID = Convert.ToInt32(onboardingid);
                    //currentUser.HasAttachments = false;
                    result = true;
                }

            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetOnBoardingProfile", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public List<OnboardingHelp> GetOnboardingHelp()
        {
            DataHelper dh = new DataHelper(strConnectionString);
            List<OnboardingHelp> onboardHelp = new List<OnboardingHelp>();
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetOnBoardingHelp]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        OnboardingHelp item = new OnboardingHelp();
                        item.description = row["Description"].ToString();
                        item.title = row["Title"].ToString();
                        item.orderingId = Convert.ToInt32(row["OrderingId"]);
                        onboardHelp.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetOnboardingHelp", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return onboardHelp;
        }

        public List<Training> GetTrainings(int skillId, int competenceId)
        {
            List<Training> trainings = new List<Training>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@SkillID";
                sqlParameters[0].Value = skillId;
                sqlParameters[1] = new SqlParameter();
                sqlParameters[0].Direction = ParameterDirection.Input;
                sqlParameters[1].ParameterName = "@CompetenceID";
                sqlParameters[1].Value = competenceId;
                sqlParameters[1].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetTrainings]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Training item = new Training();
                        item.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        item.TrainingName = row["TrainingName"].ToString();
                        item.SkillId = Convert.ToInt32(row["SkillID"]);
                        item.CompetencyId = Convert.ToInt32(row["CompetencyID"]);
                        if (!(row["Mandatory"] is DBNull))
                            item.IsMandatory = Convert.ToBoolean(row["Mandatory"]);
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public List<UserTraining> GetUserTrainingsByTrainingID(int trainingId, int projectId)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@TrainingId";
                sqlParameters[0].Value = trainingId;
                sqlParameters[1] = new SqlParameter();
                sqlParameters[0].Direction = ParameterDirection.Input;
                sqlParameters[1].ParameterName = "@ProjectID";
                sqlParameters[1].Value = projectId;
                sqlParameters[1].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserTrainingsByTrainingID]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTraining userTraining = new UserTraining();
                        userTraining.Employee = row["Employee"].ToString();
                        if (!(row["IsTrainingCompleted"] is DBNull) && (row["IsTrainingCompleted"] != null))
                            userTraining.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);
                        userTraining.SkillName = row["Skill"].ToString();
                        userTraining.TrainingName = row["Training"].ToString();
                        userTrainings.Add(userTraining);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetUserTrainingsByTrainingID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userTrainings;
        }

        public List<Assessment> GetAssessments(int skillId, int competenceId)
        {
            List<Assessment> assessments = new List<Assessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@SkillID";
                sqlParameters[0].Value = skillId;
                sqlParameters[0].Direction = ParameterDirection.Input;
                sqlParameters[1] = new SqlParameter();
                sqlParameters[1].ParameterName = "@CompetenceID";
                sqlParameters[1].Value = competenceId;
                sqlParameters[1].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessments]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Assessment item = new Assessment();
                        if (row["AssessmentId"] != null && !(row["AssessmentId"] is DBNull))
                            item.AssessmentId = Convert.ToInt32(row["AssessmentId"]);
                        if (row["AssessmentName"] != null && !(row["AssessmentName"] is DBNull))
                            item.AssessmentName = row["AssessmentName"].ToString();
                        if (row["SkillID"] != null && !(row["SkillID"] is DBNull))
                            item.SkillId = Convert.ToInt32(row["SkillID"]);
                        if (row["CompetencyID"] != null && !(row["CompetencyID"] is DBNull))
                            item.CompetencyId = Convert.ToInt32(row["CompetencyID"]);
                        if (row["IsMandatory"] != null && !(row["IsMandatory"] is DBNull))
                            item.IsMandatory = Convert.ToBoolean(row["IsMandatory"]);
                        assessments.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return assessments;
        }
        public List<SkillTraining> GetTrainingsByID(int skillID, int competencyID, int trainingID)
        {
            DataSet dsTraining = new DataSet();
            DataView dvTraining = new DataView();
            DataTable dtTraining = new DataTable();
            DataHelper dhTraining = new DataHelper(strConnectionString);
            List<SkillTraining> trainings = new List<SkillTraining>();

            try
            {
                SqlParameter[] parameters =
            {
                new SqlParameter("@SkillID",SqlDbType.Int ) { Value = skillID},
                new SqlParameter("@CompetencyID",SqlDbType.Int){Value=competencyID},
                new SqlParameter("@TrainingID",SqlDbType.Int){Value=trainingID}
            };
                dsTraining = dhTraining.ExecuteDataSet("[dbo].[proc_GetTrainingById]", CommandType.StoredProcedure, parameters);
                dtTraining = dsTraining.Tables[0];
                if (dtTraining.Rows.Count > 0)
                {
                    foreach (DataRow row in dtTraining.Rows)
                    {
                        SkillTraining train = new SkillTraining();
                        train.trainingName = row["Title"].ToString();
                        trainings.Add(train);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetTrainingsByID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }
            return trainings;
        }
        public bool AddSkillTraining(SkillTraining training)
        {
            bool result = false;
            bool status = false;
            DataHelper dhTraining = new DataHelper(strConnectionString);
            int trainingID = Convert.ToInt32(training.selectedTraining);
            int competency = Convert.ToInt32(training.selectedCompetence);
            int GEO = Convert.ToInt32(training.selectedGEO);
            int skill = Convert.ToInt32(training.selectedSkill);
            bool mandatory = training.isMandatory;
            bool assessment = training.isAssessmentRequired;
            int? assessmentID = training.assessmentId;
            int points = training.points;
            int count;

            SqlParameter[] parameters =
            {
                new SqlParameter("@TrainingID",SqlDbType.Int ) { Value = trainingID},
                new SqlParameter("@GEOId",SqlDbType.Int ) { Value = GEO},
                new SqlParameter("@IsAssessmentRequired",SqlDbType.Bit){Value=assessment},
                new SqlParameter("@IsMandatory",SqlDbType.Bit){Value=mandatory},
                new SqlParameter("@SkillID",SqlDbType.Int ) { Value = skill},
                new SqlParameter("@CompetencyID",SqlDbType.Int ) { Value = competency},
                new SqlParameter("@AssessmentID",SqlDbType.Int ) { Value = assessmentID},
                new SqlParameter("@Points",SqlDbType.Int ) { Value = points},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };

            try
            {
                count = dhTraining.ExecuteNonQuery("[dbo].[proc_AddSkillTraining]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[8].Value);
                if (status)
                    result = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }

            return result;
        }
        public bool UpdateSkillTraining(SkillTraining training)
        {
            bool result = false;
            bool status = false;
            DataHelper dhTraining = new DataHelper(strConnectionString);
            int id = Convert.ToInt32(training.id);

            int competency = Convert.ToInt32(training.selectedCompetence);
            int GEO = Convert.ToInt32(training.selectedGEO);
            int skill = Convert.ToInt32(training.selectedSkill);
            bool mandatory = training.isMandatory;
            bool assessment = training.isAssessmentRequired;
            int? assessmentID = training.assessmentId;
            int points = training.points;
            int trainingId = Convert.ToInt32(training.selectedTraining);
            int count;
            SqlParameter nullParameter = new SqlParameter("@AssessmentID", SqlDbType.Int);
            nullParameter.Value = (object)assessmentID ?? DBNull.Value;

            SqlParameter[] parameters =
        {
                new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                new SqlParameter("@GEOId",SqlDbType.Int ) { Value = GEO},
                new SqlParameter("@IsAssessmentRequired",SqlDbType.Bit){Value=assessment},
                new SqlParameter("@IsMandatory",SqlDbType.Bit){Value=mandatory},
                new SqlParameter("@SkillID",SqlDbType.Int ) { Value = skill},
                new SqlParameter("@CompetencyID",SqlDbType.Int ) { Value = competency},
                new SqlParameter("@AssessmentID",SqlDbType.Int ) { Value = assessmentID},
                new SqlParameter("@Points",SqlDbType.Int ) { Value = points},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output },
                new SqlParameter("@TrainingId",SqlDbType.Int ) { Value =trainingId}
            };
            parameters[6] = nullParameter;
            try
            {
                count = dhTraining.ExecuteNonQuery("[dbo].[proc_UpdateSkillTraining]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[8].Value);
                if (status)
                    result = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,UpdateSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }

            return result;
        }
        public bool DeleteSkillTraining(int id, string skill, string competency)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhTraining = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                new SqlParameter("@Skill",SqlDbType.NVarChar ) { Value = skill},
                new SqlParameter("@Competency",SqlDbType.NVarChar ) { Value = competency},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };
            try
            {
                count = dhTraining.ExecuteNonQuery("[dbo].[proc_DeleteSkillTraining]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[3].Value);
                if (status)
                    result = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,DeleteSkillTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }
            return result;
        }
        public List<UserAssessment> GetUserAssessmentsByAssessmentId(int assessmentId, int projectid)
        {
            List<UserAssessment> userAssessments = new List<UserAssessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AssessmentId",SqlDbType.Int ) { Value = assessmentId},
                    new SqlParameter("@ProjectID",SqlDbType.NVarChar ) { Value =  projectid}
                };
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserAssessmentsByAssessmentID]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserAssessment userAssessment = new UserAssessment();
                        userAssessment.Employee = row["Employee"].ToString();
                        if ((row["IsAssessmentCompleted"] != null) && !(row["IsAssessmentCompleted"] is DBNull))
                            userAssessment.IsAssessmentComplete = Convert.ToBoolean(row["IsAssessmentCompleted"]);
                        userAssessment.TrainingAssessment = row["Assessment"].ToString();
                        userAssessments.Add(userAssessment);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetUserAssessmentsByAssessmentId", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userAssessments;
        }


        public bool AddSkillBasedTrainingAssessment(string competence, int skillId, int userId)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId), new SqlParameter("@skillId", skillId), new SqlParameter("@competencyLevelId", Convert.ToInt32(competence)) };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_AddSkillTrainingAssessment]", CommandType.StoredProcedure, parameters);
                if (count > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,AddSkillBasedTrainingAssessment", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public bool AddSkillBasedTrainingAssessment(string competence, int skillId, int userId, bool isTrainingMandatory, DateTime lastDayOfCompletion)
        {
            bool status = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId), new SqlParameter("@skillId", skillId), new SqlParameter("@competencyLevelId", Convert.ToInt32(competence)), new SqlParameter("@isMandatory", isTrainingMandatory), new SqlParameter("@lastdayofcompletion", lastDayOfCompletion) };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_AddSkillBasedTrainingAssessment]", CommandType.StoredProcedure, parameters);
                if (count > 0)
                    status = true;
                else
                    status = false;

            }
            catch (Exception ex)
            {
                status = false;
                LogHelper.AddLog("SQLServerDAL,AddSkillBasedTrainingAssessment", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            return status;

        }

        public List<GEO> GetAllGEOs()
        {
            List<GEO> allGEOs = new List<GEO>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllGEOs]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        GEO item = new GEO();
                        item.Id = Int32.Parse(row["ID"].ToString());
                        item.Title = row["Title"].ToString();
                        //item.Skills = ??? 
                        allGEOs.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllGEOs", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return allGEOs;
        }
        public DataSet GetDefaultTraining()
        {
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = dh.ExecuteDataSet("SELECT * FROM DefaultOnboardingTraining", CommandType.Text);
                return ds;

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetDefaultTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                return null;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }
        public bool OnBoardUser(string competence, int skillId, int userId, string geo, int roleId, string userEmail, string userName, int employeeId, int roleBasedSkillCount)
        {

            bool status = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                int lastDayCompletion = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LastDayCompletion"]);
                SqlParameter[] parameters =
                {
                        new SqlParameter("@competenceid", SqlDbType.Int) { Value = Convert.ToInt32(competence), Direction = ParameterDirection.Input },
                        new SqlParameter("@skillid", SqlDbType.Int) { Value = skillId, Direction = ParameterDirection.Input  },
                        new SqlParameter("@geoid", SqlDbType.Int) { Value = Convert.ToInt32(geo), Direction = ParameterDirection.Input },
                        new SqlParameter("@useremail", SqlDbType.NVarChar) { Value = userEmail, Direction = ParameterDirection.Input },
                        new SqlParameter("@username", SqlDbType.NVarChar) { Value = userName, Direction = ParameterDirection.Input },
                        new SqlParameter("@roleid", SqlDbType.Int) { Value = roleId, Direction = ParameterDirection.Input },
                        new SqlParameter("@lastdayofcompletion", SqlDbType.Int) { Value = lastDayCompletion, Direction = ParameterDirection.Input },
                        new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId, Direction = ParameterDirection.Input },
                        new SqlParameter("@rolebasedskillcount", SqlDbType.Int) { Value = roleBasedSkillCount, Direction = ParameterDirection.Input }

                };
                status = (bool)dh.ExecuteScalar("[dbo].[proc_OnBoardUser]", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                status = false;
                LogHelper.AddLog("SQLServerDAL,OnBoardUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return status;

        }

        public UserOnBoarding GetOnBoardingDetailsForUser(ServiceConsumerInfo user)
        {
            UserOnBoarding objUserOnBoarding = new UserOnBoarding();
            objUserOnBoarding.UserId = user.id;
            objUserOnBoarding.Name = user.name;
            objUserOnBoarding.Email = user.emailId;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                SqlParameter[] parameters =
                {  new SqlParameter("@UserEmail", SqlDbType.NVarChar) { Value = user.emailId, Direction = ParameterDirection.Input } };

                ds = dh.ExecuteDataSet("[dbo].[proc_GetOnBoardingDetailsForUser]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            var TempLength = ds.Tables[0].Rows.Count;
                            objUserOnBoarding.CurrentSkill = Convert.ToString(item["CurrentSkill"]);
                            objUserOnBoarding.CurrentCompetance = Convert.ToString(item["CurrentCompetance"]);
                            objUserOnBoarding.CurrentStatus = Convert.ToString(item["CurrentStatus"]);
                            objUserOnBoarding.CurrentBGVStatus = Convert.ToString(item["CurrentBGVStatus"]);
                            objUserOnBoarding.CurrentProfileSharing = Convert.ToString(item["CurrentProfileSharing"]);
                            objUserOnBoarding.CurrentGEO = Convert.ToString(item["CurrentGEO"]);
                            objUserOnBoarding.IsPresentInOnBoard = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetOnBoardingDetailsForUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return objUserOnBoarding;
        }

        public List<UserAssessment> GetAssessmentForUser(int userId, bool OnlyOnBoardedTraining = false)
        {
            List<UserAssessment> userAssessments = new List<UserAssessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                OnlyOnBoardedTraining = true;
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserId";
                sqlParameters[0].Value = userId;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessmentsForUser]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserAssessment userAssessment = new UserAssessment();
                        if (row["Assessment"] != null && !(row["Assessment"] is DBNull))
                            userAssessment.TrainingAssessment = row["Assessment"].ToString();
                        if (row["CompletedDate"] != null && !(row["CompletedDate"] is DBNull))
                            userAssessment.CompletedDate = Convert.ToString(row["CompletedDate"]);
                        if (row["IsIncludeOnBoarding"] != null && !(row["IsIncludeOnBoarding"] is DBNull))
                            userAssessment.IsIncludeOnBoarding = Convert.ToBoolean(row["IsIncludeOnBoarding"]);
                        if (row["IsMandatory"] != null && !(row["IsMandatory"] is DBNull))
                            userAssessment.IsMandatory = Convert.ToBoolean(row["IsMandatory"]);
                        if (row["IsAssessmentActive"] != null && !(row["IsAssessmentActive"] is DBNull))
                            userAssessment.IsAssessmentActive = Convert.ToBoolean(row["IsAssessmentActive"]);
                        if (row["IsAssessmentComplete"] != null && !(row["IsAssessmentComplete"] is DBNull))
                            userAssessment.IsAssessmentComplete = Convert.ToBoolean(row["IsAssessmentComplete"]);
                        if (row["LastDayCompletion"] != null && !(row["LastDayCompletion"] is DBNull))
                            userAssessment.LastDayCompletion = Convert.ToDateTime(row["LastDayCompletion"].ToString()).ToShortDateString();
                        if (row["Skill"] != null && !(row["Skill"] is DBNull))
                            userAssessment.SkillName = row["Skill"] != null ? row["Skill"].ToString() : "";
                        if (row["SkillId"] != null && !(row["SkillId"] is DBNull))
                            userAssessment.SkillId = row["SkillId"] != null ? Convert.ToInt32(row["SkillId"]) : 0;
                        if (row["Training"] != null && !(row["Training"] is DBNull))
                            userAssessment.TrainingName = row["Training"] != null ? row["Training"].ToString() : "";
                        if (row["TrainingId"] != null && !(row["TrainingId"] is DBNull))
                            userAssessment.TrainingId = row["TrainingId"] != null ? Convert.ToInt32(row["TrainingId"]) : 0;
                        if (row["AssessmentId"] != null && !(row["AssessmentId"] is DBNull))
                            userAssessment.TrainingAssessmentId = row["AssessmentId"] != null ? Convert.ToInt32(row["AssessmentId"]) : 0;
                        userAssessments.Add(userAssessment);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAssessmentForUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userAssessments;
        }

        public bool AssignAssessmentsToUser(List<UserAssessment> assessments, int userId, bool forDefault = false)
        {
            bool result = false;
            string spResult = string.Empty;
            DataTable table = new DataTable();
            table.Columns.Add("TrainingAssessmentId", typeof(int));//
            table.Columns.Add("TrainingCourseId", typeof(int));//
            table.Columns.Add("IsMandatory", typeof(bool));//
            table.Columns.Add("IsIncludeOnBoarding", typeof(bool));//
            table.Columns.Add("LastDayCompletion", typeof(string));//

            // add a single row for each item in the collection.
            foreach (UserAssessment assessment in assessments)
            {
                table.Rows.Add(
                    assessment.TrainingAssessmentId,
                    assessment.SkillId,
                    assessment.IsMandatory,
                    assessment.IsIncludeOnBoarding,
                    assessment.LastDayCompletion

                    );
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(strConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.proc_AssignAssessmentsToUser", sqlcon))
                    {
                        sqlcon.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ListAssessments", SqlDbType.Structured).Value = table;
                        cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@fordefault", SqlDbType.Bit).Value = 1;
                        cmd.Parameters.Add("@status", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        if (cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                        {
                            result = true;
                        }
                        sqlcon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AssignAssessmentsToUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                result = false;
            }
            return result;
        }

        public List<UserManager> GetAllOnBoardedUser(int assignedTo)
        {
            List<UserManager> lstUserManager = new List<UserManager>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@skillID";
                sqlParameters[0].Value = assignedTo;
                sqlParameters[0].Direction = ParameterDirection.Input;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllOnBoardedUser]", CommandType.StoredProcedure, sqlParameters);

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            UserManager objUser = null;
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        objUser = new UserManager()
                        {
                            EmailID = ds.Tables[0].Rows[i]["EmailAddress"].ToString(),
                            DBUserId = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString()),
                            UserName = ds.Tables[0].Rows[i]["Name"].ToString()
                        };
                        lstUserManager.Add(objUser);
                    }
                }
            }
            return lstUserManager;
        }

        public bool AssignTrainingsToUser(List<UserTraining> trainings, int userId, bool forDefault = false)
        {
            int ret = 0; bool result = false;
            string spResult = string.Empty;
            DataTable table = new DataTable();
            table.Columns.Add("TrainingCourseId", typeof(int));//
            table.Columns.Add("TrainingModuleId", typeof(int));//
            table.Columns.Add("IsMandatory", typeof(bool));//
            table.Columns.Add("IsIncludeOnBoarding", typeof(bool));//
            table.Columns.Add("LastDayCompletion", typeof(string));//


            // add a single row for each item in the collection.
            foreach (UserTraining training in trainings)
            {
                table.Rows.Add(
                    training.SkillId,
                    training.TrainingId,
                    training.IsMandatory,
                    training.IsIncludeOnBoarding,
                    training.LastDayCompletion
                    );
            }



            try
            {
                using (SqlConnection sqlcon = new SqlConnection(strConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.proc_AssignTrainingsToUser", sqlcon))
                    {

                        sqlcon.Open();
                        // add the table-valued-parameter. 
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ListTraining", SqlDbType.Structured).Value = table;
                        cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@fordefault", SqlDbType.Bit).Value = 1;
                        // execute sqlcon.Open();
                        ret = cmd.ExecuteNonQuery();
                        sqlcon.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,AssignTrainingsToUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            result = ret > 0 ? true : false;
            return result;

        }

        public void RemoveAssessmentHistory(int userId, int skillId, int competenceid)
        {
        }
        public bool AddSkill(string SkillName, bool isDefault)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {
                SqlParameter[] parameters =
                    {
                        new SqlParameter("@Title",SqlDbType.NVarChar),
                        new SqlParameter("@IsDefault",SqlDbType.Bit),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = SkillName;
                parameters[1].Value = isDefault == true ? 1 : 0;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Size = 4000;
                dh.ExecuteNonQuery("[dbo].[proc_AddSkill]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;

                    LogHelper.AddLog("SQLServerDAL,AddSkill", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,AddSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }
        public bool UpdateSkill(SkillMaster skillmaster)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ID",SqlDbType.Int),
                        new SqlParameter("@Title",SqlDbType.NVarChar),
                        new SqlParameter("@IsDefault",SqlDbType.Bit),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = skillmaster.ID;
                parameters[1].Value = skillmaster.Title;
                parameters[1].Size = 100;
                parameters[2].Value = skillmaster.IsDefault == true ? 1 : 0;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[4].Size = 4000;
                parameters[4].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_UpdateSkill]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,UpdateSkill", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,UpdateSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public TrainingMaster GetMasterTrainingById(int id)
        {
            TrainingMaster trainingMaster = new TrainingMaster();
            try
            {
                DataSet dsTrainingMaster = new DataSet();
                DataView dvTrainingMaster = new DataView();
                DataHelper dh = new DataHelper(strConnectionString);
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int)
                };
                parameters[0].Value = id;
                dsTrainingMaster = dh.ExecuteDataSet("[dbo].[proc_GetMasterTrainingById]", CommandType.StoredProcedure, parameters);
                if (dsTrainingMaster.Tables.Count > 0)
                    dvTrainingMaster = new DataView(dsTrainingMaster.Tables[0]);
                DataTable dtTrainingMaster = dvTrainingMaster.ToTable();
                if (dtTrainingMaster != null & dtTrainingMaster.Rows.Count == 1)
                {
                    trainingMaster = new TrainingMaster();
                    trainingMaster.Id = Convert.ToInt32(dtTrainingMaster.Rows[0]["ID"].ToString());
                    if (dtTrainingMaster.Rows[0]["Title"] != null && !(dtTrainingMaster.Rows[0]["Title"] is DBNull))
                        trainingMaster.title = Convert.ToString(dtTrainingMaster.Rows[0]["Title"]);
                    if (dtTrainingMaster.Rows[0]["SkillType"] != null && !(dtTrainingMaster.Rows[0]["SkillType"] is DBNull))
                        trainingMaster.skillType = Convert.ToString(dtTrainingMaster.Rows[0]["SkillType"]);
                    if (dtTrainingMaster.Rows[0]["TrainingCategory"] != null && !(dtTrainingMaster.Rows[0]["TrainingCategory"] is DBNull))
                        trainingMaster.trainingCategory = Convert.ToString(dtTrainingMaster.Rows[0]["TrainingCategory"]);
                    if (dtTrainingMaster.Rows[0]["TrainingLink"] != null && !(dtTrainingMaster.Rows[0]["TrainingLink"] is DBNull))
                        trainingMaster.trainingLink = Convert.ToString(dtTrainingMaster.Rows[0]["TrainingLink"]);
                    if (dtTrainingMaster.Rows[0]["TrainingContent"] != null && !(dtTrainingMaster.Rows[0]["TrainingContent"] is DBNull))
                        trainingMaster.selectedContent = (dtTrainingMaster.Rows[0]["TrainingContent"]).ToString();
                    if (dtTrainingMaster.Rows[0]["Description"] != null && !(dtTrainingMaster.Rows[0]["Description"] is DBNull))
                        trainingMaster.description = Convert.ToString(dtTrainingMaster.Rows[0]["Description"]);
                    if (dtTrainingMaster.Rows[0]["TrainingDocument"] != null && !(dtTrainingMaster.Rows[0]["TrainingDocument"] is DBNull))
                        trainingMaster.trainingDocument = Convert.ToString(dtTrainingMaster.Rows[0]["TrainingDocument"]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetMasterTrainingById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return trainingMaster;
        }

        public bool RemoveSkill(int Id, out int ErrNumber)
        {
            ErrNumber = 0;
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ID",SqlDbType.Int),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = Id;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteSkill]", CommandType.StoredProcedure, parameters);


                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("RemoveSkill", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.Service", currentUser.emailId.ToString());

                }
                else if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && Convert.ToInt32(dh.Cmd.Parameters["@ErrorNumber"].Value) == 50000)
                {
                    result = false;
                    ErrNumber = 50000;
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,RemoveSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;
        }
        public SkillMaster GetSkillById(int Id)
        {
            SkillMaster skillmaster = null;
            try
            {
                DataSet dsskillmaster = new DataSet();
                DataView dvskillmaster = new DataView();
                DataHelper dh = new DataHelper(strConnectionString);
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int)
                };
                parameters[0].Value = Id;
                dsskillmaster = dh.ExecuteDataSet("[dbo].[proc_GetSkillById]", CommandType.StoredProcedure, parameters);
                if (dsskillmaster.Tables.Count > 0)
                    dvskillmaster = new DataView(dsskillmaster.Tables[0]);
                DataTable dtacademyconfig = dvskillmaster.ToTable();
                if (dtacademyconfig != null & dtacademyconfig.Rows.Count == 1)
                {
                    skillmaster = new SkillMaster();
                    skillmaster.ID = Convert.ToInt32(dtacademyconfig.Rows[0]["ID"].ToString());
                    skillmaster.Title = Convert.ToString(dtacademyconfig.Rows[0]["Title"]);
                    skillmaster.IsDefault = Convert.ToString(dtacademyconfig.Rows[0]["IsDefault"]) == "true" ? true : false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetSkillById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return skillmaster;
        }
        public bool AddSkill(string email, string userId, string skillId, string competence, bool ismandatory, DateTime lastdayofcompletion, int roleId)
        {
            int id = 0;
            if (userId == null)
                id = GetUserId(email);
            else
                id = Convert.ToInt32(userId);
            bool result = false;
            try
            {
                if (id > 0)
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", id),
                    new SqlParameter("@skillId", Convert.ToInt32(skillId)),
                    new SqlParameter("@competenceId", Convert.ToInt32(competence)), new SqlParameter("@isMandatory", ismandatory),
                    new SqlParameter("@lastdayofcompletion", lastdayofcompletion),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output},
                    new SqlParameter("@roleid", roleId) };

                    DataHelper dh = new DataHelper(strConnectionString);
                    int count = dh.ExecuteNonQuery("[dbo].[proc_AddUserSkill]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                }
                else if (email != null)
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@email",email),
                    new SqlParameter("@skillId", Convert.ToInt32(skillId)),
                    new SqlParameter("@competenceId", Convert.ToInt32(competence)), new SqlParameter("@isMandatory", ismandatory),
                    new SqlParameter("@lastdayofcompletion", lastdayofcompletion),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output},
                    new SqlParameter("@roleid", roleId) };

                    DataHelper dh = new DataHelper(strConnectionString);
                    int count = dh.ExecuteNonQuery("[dbo].[proc_AddUserSkillByEmail]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,AddSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public List<UserSkill> GetSkillForUser(int userId)
        {
            return GetSkillForOnboardedUser(userId);
        }

        public List<UserSkill> GetSkillForOnboardedUser(int userId)
        {
            List<UserSkill> userSkills = new List<UserSkill>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillForOnboardedUser]", CommandType.StoredProcedure, new SqlParameter("@userId", userId));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UserSkill objSkill = new UserSkill();
                        objSkill.Id = Int32.Parse(row["id"].ToString());
                        objSkill.Skill = row["skillName"].ToString();
                        objSkill.SkillId = Int32.Parse(row["skillId"].ToString());
                        objSkill.CompetenceId = Int32.Parse(row["competenceId"].ToString());
                        objSkill.Competence = row["competenceName"].ToString();
                        objSkill.SkillwiseCompetencies = row["skillwiseCompetencies"].ToString();
                        objSkill.SkillwiseCompetencyIds = row["skillwiseCompetencyIds"].ToString();
                        objSkill.RoleId = Int32.Parse(row["roleId"].ToString());
                        userSkills.Add(objSkill);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetSkillForOnboardedUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userSkills;
        }
        public List<UserSkill> GetUserSkillByEmail(string emailAddress)
        {
            List<UserSkill> userSkills = new List<UserSkill>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillByEmailAddress]", CommandType.StoredProcedure, new SqlParameter("@email", emailAddress));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UserSkill objSkill = new UserSkill();
                        objSkill.Id = Int32.Parse(row["id"].ToString());
                        objSkill.Skill = row["skillName"].ToString();
                        objSkill.SkillId = Int32.Parse(row["skillId"].ToString());
                        objSkill.CompetenceId = Int32.Parse(row["competenceId"].ToString());
                        objSkill.Competence = row["competenceName"].ToString();
                        objSkill.SkillwiseCompetencies = row["skillwiseCompetencies"].ToString();
                        objSkill.SkillwiseCompetencyIds = row["skillwiseCompetencyIds"].ToString();
                        userSkills.Add(objSkill);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetSkillForOnboardedUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userSkills;
        }
        public bool UpdateUserSkill(int itemId, string competence, int userId, string emailId, DateTime completiondate, bool isCompetenceChanged)
        {
            bool result = false;
            try
            {
                if (userId > 0)
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userSkillId", Convert.ToInt32(itemId)),
                    new SqlParameter("@userId", userId), new SqlParameter("@competencyLevelId", Convert.ToInt32(competence)),
                    new SqlParameter("@completionDate", completiondate), new SqlParameter("@competencyChanged", isCompetenceChanged),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };

                    DataHelper dh = new DataHelper(strConnectionString);
                    int count = dh.ExecuteNonQuery("[dbo].[proc_UpdateUserSkill]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                }
                else if (!String.IsNullOrEmpty(emailId))
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userSkillId", Convert.ToInt32(itemId)),
                    new SqlParameter("@emailId", emailId), new SqlParameter("@competencyLevelId", Convert.ToInt32(competence)),
                    new SqlParameter("@completionDate", completiondate), new SqlParameter("@competencyChanged", isCompetenceChanged),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };

                    DataHelper dh = new DataHelper(strConnectionString);
                    int count = dh.ExecuteNonQuery("[dbo].[proc_UpdateUserSkillByEmail]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,UpdateUserSkill", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }
        public List<UserChecklistReportItem> GetOnBoardingChecklistReport(int roleId, int projectId, int geoId, string option, string search)
        {
            List<UserChecklistReportItem> checklistReport = new List<UserChecklistReportItem>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter nullParam = new SqlParameter();
            nullParam.Direction = ParameterDirection.Input;
            nullParam.Value = DBNull.Value;
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@projectID", projectId), new SqlParameter("@roleID", roleId), new SqlParameter("@geoID", geoId), new SqlParameter("@option", option), new SqlParameter("@search", search) };
            ds = dh.ExecuteDataSet("[dbo].[proc_AllUserGetChecklistData]", CommandType.StoredProcedure, parameters);
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    UserChecklistReportItem item = new UserChecklistReportItem();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.EmailAddress = ds.Tables[0].Rows[i]["EmailAddress"].ToString();
                    item.CheckListItem = ds.Tables[0].Rows[i]["Title"].ToString();
                    item.EmployeeID = ds.Tables[0].Rows[i]["EmployeeId"].ToString();
                    item.CompletionDate = ds.Tables[0].Rows[i]["CompletionDate"].ToString();
                    item.OnboardingDate = ds.Tables[0].Rows[i]["OnboardingDate"].ToString();
                    if (ds.Tables[0].Rows[i]["CheckListStatus"].ToString().ToUpper() == "TRUE")
                        item.CheckListStatus = "Yes";
                    else
                        item.CheckListStatus = "No";
                    checklistReport.Add(item);

                }
            }
            return checklistReport;
        }
        public List<UserChecklistReportItem> GetLastChecklistReport(int roleId, int projectId, int geoId, string option, string search)
        {
            List<UserChecklistReportItem> checklistReport = new List<UserChecklistReportItem>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@projectID", projectId), new SqlParameter("@roleID", roleId), new SqlParameter("@geoID", geoId), new SqlParameter("@option", option), new SqlParameter("@search", search) };

            ds = dh.ExecuteDataSet("[dbo].[proc_LastChecklistData]", CommandType.StoredProcedure, parameters);
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    UserChecklistReportItem item = new UserChecklistReportItem();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.EmailAddress = ds.Tables[0].Rows[i]["EmailAddress"].ToString();
                    item.CheckListItem = ds.Tables[0].Rows[i]["Title"].ToString();
                    item.EmployeeID = ds.Tables[0].Rows[i]["EmployeeId"].ToString();
                    item.CompletionDate = ds.Tables[0].Rows[i]["CompletionDate"].ToString();
                    item.OnboardingDate = ds.Tables[0].Rows[i]["OnboardingDate"].ToString();
                    checklistReport.Add(item);

                }
            }
            return checklistReport;
        }
        public List<UserOnBoarding> GetOnBoardingSkillReport(int roleId, int projectId, int geoId)
        {
            List<Skill> skills = GetAllSkills();
            List<UserOnBoarding> lstUserOnBoarding = new List<UserOnBoarding>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@projectId", projectId),
                new SqlParameter("@roleId", roleId),new SqlParameter("@geoId",geoId)};
            ds = dh.ExecuteDataSet("[dbo].[proc_GetAllUsers]", CommandType.StoredProcedure, parameters);
            if (ds.Tables.Count > 0)
            {
                DataSet dsUserSkills = new DataSet();
                dsUserSkills = dh.ExecuteDataSet("proc_GetAllUserSkills", CommandType.StoredProcedure);

                List<UserSkill> lstSkills = new List<UserSkill>();
                UserSkill objSkill = null;
                if (dsUserSkills.Tables.Count > 0 && dsUserSkills.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in dsUserSkills.Tables[0].Rows)
                    {
                        objSkill = new UserSkill();
                        objSkill.Id = Convert.ToInt32(item["id"].ToString());
                        objSkill.Skill = item["Skill"].ToString();
                        objSkill.SkillId = Convert.ToInt32(item["SkillId"].ToString());
                        objSkill.Competence = item["Competence"].ToString();
                        objSkill.CompetenceId = Convert.ToInt32(item["CompetencyLevelId"].ToString());
                        objSkill.UserId = Convert.ToInt32(item["UserId"].ToString());
                        lstSkills.Add(objSkill);
                    }
                }
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    if (item["Active"].ToString().ToUpper() == "TRUE")
                    {
                        UserOnBoarding objUserOnBoarding = new UserOnBoarding();
                        if (item["ID"] != null && !(item["ID"] is DBNull))
                            objUserOnBoarding.UserId = Convert.ToInt32(item["ID"].ToString());

                        if (item["Name"] != null && !(item["Name"] is DBNull))
                            objUserOnBoarding.Name = item["Name"].ToString();
                        objUserOnBoarding.UserSkills = new List<UserSkill>();
                        for (int i = 0; i < skills.Count; i++)
                        {
                            List<UserSkill> lstUserSkills = lstSkills.Where(s => s.UserId == objUserOnBoarding.UserId && s.SkillId == skills[i].SkillId).ToList();
                            if (lstUserSkills.Count > 0)
                                objUserOnBoarding.UserSkills.Add(lstUserSkills[0]);
                            else
                            {
                                UserSkill userskill = new UserSkill();
                                userskill.Skill = skills[i].SkillName;
                                userskill.SkillId = skills[i].SkillId;
                                userskill.Competence = "";
                                objUserOnBoarding.UserSkills.Add(userskill);
                            }

                        }

                        lstUserOnBoarding.Add(objUserOnBoarding);
                    }
                }
            }
            return lstUserOnBoarding;
        }
        public List<UserOnBoarding> GetOnBoardingDetailsReport(bool isExcelDownload, int roleId, int geoId, int projectId)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@projectId", projectId),
                new SqlParameter("@roleId", roleId),new SqlParameter("@geoId",geoId)};

            List<UserOnBoarding> lstUserOnBoarding = new List<UserOnBoarding>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            ds = dh.ExecuteDataSet("[dbo].[proc_GetAllUsers]", CommandType.StoredProcedure, parameters);
            if (ds.Tables.Count > 0)
            {
                DataSet dsUserSkills = new DataSet();
                dsUserSkills = dh.ExecuteDataSet("proc_GetAllUserSkills", CommandType.StoredProcedure);

                List<UserSkill> lstSkills = new List<UserSkill>();
                UserSkill objSkill = null;
                if (dsUserSkills.Tables.Count > 0 && dsUserSkills.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in dsUserSkills.Tables[0].Rows)
                    {
                        objSkill = new UserSkill();
                        objSkill.Id = Convert.ToInt32(item["id"].ToString());
                        objSkill.Skill = item["Skill"].ToString();
                        objSkill.SkillId = Convert.ToInt32(item["SkillId"].ToString());
                        objSkill.Competence = item["Competence"].ToString();
                        objSkill.CompetenceId = Convert.ToInt32(item["CompetencyLevelId"].ToString());
                        objSkill.UserId = Convert.ToInt32(item["UserId"].ToString());
                        lstSkills.Add(objSkill);
                    }
                }
                DataSet dsUserTrainings = new DataSet();
                dsUserTrainings = dh.ExecuteDataSet("proc_GetAllUserTrainings", CommandType.StoredProcedure);
                List<UserTraining> lstTrainings = new List<UserTraining>();
                if (dsUserTrainings.Tables.Count > 0 && dsUserTrainings.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in dsUserTrainings.Tables[0].Rows)
                    {
                        UserTraining objTraining = new UserTraining();
                        objTraining.CompletedDate = Convert.ToString(item["CompletedDate"]);
                        if (item["IsIncludeOnBoarding"] != null && !(item["IsIncludeOnBoarding"] is DBNull))
                            objTraining.IsIncludeOnBoarding = Convert.ToBoolean(item["IsIncludeOnBoarding"]);
                        if (item["IsMandatory"] != null && !(item["IsMandatory"] is DBNull))
                            objTraining.IsMandatory = Convert.ToBoolean(item["IsMandatory"]);
                        if (item["IsTrainingActive"] != null && !(item["IsTrainingActive"] is DBNull))
                            objTraining.IsTrainingActive = Convert.ToBoolean(item["IsTrainingActive"]);
                        if (item["IsTrainingCompleted"] != null && !(item["IsTrainingCompleted"] is DBNull))
                            objTraining.IsTrainingCompleted = Convert.ToBoolean(item["IsTrainingCompleted"]);
                        if (item["LastDayCompletion"] != null && !(item["LastDayCompletion"] is DBNull))
                            objTraining.LastDayCompletion = Convert.ToString(item["LastDayCompletion"]);
                        if (item["Skill"] != null && !(item["Skill"] is DBNull))
                            objTraining.SkillName = item["Skill"].ToString();
                        if (item["SkillId"] != null && !(item["SkillId"] is DBNull))
                            objTraining.SkillId = Convert.ToInt32(item["SkillId"].ToString());
                        if (item["Training"] != null && !(item["Training"] is DBNull))
                            objTraining.TrainingName = item["Training"].ToString();
                        if (item["TrainingId"] != null && !(item["TrainingId"] is DBNull))
                            objTraining.TrainingId = Convert.ToInt32(item["TrainingId"].ToString());
                        if (item["UserId"] != null && !(item["UserId"] is DBNull))
                            objTraining.UserId = Convert.ToInt32(item["UserId"].ToString());

                        if (objTraining.IsTrainingCompleted)
                        {
                            objTraining.StatusColor = AppConstant.Green;
                            objTraining.ItemStatus = AppConstant.Completed;
                        }
                        else if (objTraining.IsTrainingCompleted == false && Convert.ToDateTime(objTraining.LastDayCompletion) < DateTime.Now)
                        {
                            objTraining.StatusColor = AppConstant.Red;
                            objTraining.ItemStatus = AppConstant.OverDue;
                        }
                        else
                        {
                            objTraining.StatusColor = AppConstant.Amber;
                            objTraining.ItemStatus = AppConstant.InProgress;
                        }
                        lstTrainings.Add(objTraining);
                    }
                }
                DataSet dsUserAssessments = new DataSet();
                dsUserAssessments = dh.ExecuteDataSet("proc_GetAllUserAssessments", CommandType.StoredProcedure);

                List<UserAssessment> lstAssessments = new List<UserAssessment>();
                if (dsUserAssessments.Tables.Count > 0 && dsUserAssessments.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in dsUserAssessments.Tables[0].Rows)
                    {
                        UserAssessment objUserAssessment = new UserAssessment();
                        objUserAssessment.CompletedDate = Convert.ToString(item["CompletedDate"]);
                        if (item["IsIncludeOnBoarding"] != null && !(item["IsIncludeOnBoarding"] is DBNull))
                            objUserAssessment.IsIncludeOnBoarding = Convert.ToBoolean(item["IsIncludeOnBoarding"]);
                        if (item["IsMandatory"] != null && !(item["IsMandatory"] is DBNull))
                            objUserAssessment.IsMandatory = Convert.ToBoolean(item["IsMandatory"]);
                        if (item["IsAssessmentActive"] != null && !(item["IsAssessmentActive"] is DBNull))
                            objUserAssessment.IsAssessmentActive = Convert.ToBoolean(item["IsAssessmentActive"]);
                        if (item["IsAssessmentComplete"] != null && !(item["IsAssessmentComplete"] is DBNull))
                            objUserAssessment.IsAssessmentComplete = Convert.ToBoolean(item["IsAssessmentComplete"]);
                        if (item["LastDayCompletion"] != null && !(item["LastDayCompletion"] is DBNull))
                            objUserAssessment.LastDayCompletion = Convert.ToString(item["LastDayCompletion"]);
                        if (item["Skill"] != null && !(item["Skill"] is DBNull))
                            objUserAssessment.SkillName = item["Skill"].ToString();
                        if (item["SkillId"] != null && !(item["SkillId"] is DBNull))
                            objUserAssessment.SkillId = Convert.ToInt32(item["SkillId"].ToString());
                        if (item["Training"] != null && !(item["Training"] is DBNull))
                            objUserAssessment.TrainingName = item["Training"].ToString();
                        if (item["TrainingId"] != null && !(item["TrainingId"] is DBNull))
                            objUserAssessment.TrainingId = Convert.ToInt32(item["TrainingId"]);
                        if (item["Assessment"] != null && !(item["Assessment"] is DBNull))
                            objUserAssessment.TrainingAssessment = item["Assessment"].ToString();
                        if (item["AssessmentId"] != null && !(item["AssessmentId"] is DBNull))
                            objUserAssessment.TrainingAssessmentId = Convert.ToInt32(item["AssessmentId"].ToString());
                        if (item["UserId"] != null && !(item["UserId"] is DBNull))
                            objUserAssessment.UserId = Convert.ToInt32(item["UserId"].ToString());

                        if (objUserAssessment.IsAssessmentComplete)
                        {
                            objUserAssessment.StatusColor = AppConstant.Green;
                            objUserAssessment.ItemStatus = AppConstant.Completed;
                        }
                        else if (objUserAssessment.IsAssessmentComplete == false && Convert.ToDateTime(objUserAssessment.LastDayCompletion) < DateTime.Now)
                        {
                            objUserAssessment.StatusColor = AppConstant.Red;
                            objUserAssessment.ItemStatus = AppConstant.OverDue;
                        }
                        else
                        {
                            objUserAssessment.StatusColor = AppConstant.Amber;
                            objUserAssessment.ItemStatus = AppConstant.InProgress;
                        }
                        lstAssessments.Add(objUserAssessment);
                    }
                }

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    UserOnBoarding objUserOnBoarding = new UserOnBoarding();
                    if (item["Skill"] != null && !(item["Skill"] is DBNull))
                        objUserOnBoarding.CurrentSkill = item["Skill"].ToString();
                    if (item["GEO"] != null && !(item["GEO"] is DBNull))
                        objUserOnBoarding.CurrentGEO = item["GEO"].ToString();
                    if (item["CompetencyLevel"] != null && !(item["CompetencyLevel"] is DBNull))
                        objUserOnBoarding.CurrentCompetance = item["CompetencyLevel"].ToString();
                    if (item["Status"] != null && !(item["Status"] is DBNull))
                        objUserOnBoarding.CurrentStatus = item["Status"].ToString();
                    if (item["BGVStatus"] != null && !(item["BGVStatus"] is DBNull))
                        objUserOnBoarding.CurrentBGVStatus = item["BGVStatus"].ToString();
                    if (item["ProfileSharing"] != null && !(item["ProfileSharing"] is DBNull))
                        objUserOnBoarding.CurrentProfileSharing = item["ProfileSharing"].ToString();
                    if (item["EmailAddress"] != null && !(item["EmailAddress"] is DBNull))
                        objUserOnBoarding.Email = item["EmailAddress"].ToString();
                    if (item["ID"] != null && !(item["ID"] is DBNull))
                        objUserOnBoarding.UserId = Convert.ToInt32(item["ID"].ToString());

                    if (item["EmployeeId"] != null && !(item["EmployeeId"] is DBNull))
                        objUserOnBoarding.EmployeeId = Convert.ToInt32(item["EmployeeId"].ToString());

                    if (item["Active"] != null && !(item["Active"] is DBNull))
                        objUserOnBoarding.Active = Convert.ToBoolean(item["Active"].ToString());

                    DataSet dsUserRoles = new DataSet();
                    dsUserRoles = dh.ExecuteDataSet("proc_GetUserRoles", CommandType.StoredProcedure, new SqlParameter("@UserId", objUserOnBoarding.UserId));
                    if (dsUserRoles.Tables.Count > 0)
                    {
                        if (dsUserRoles.Tables[0].Rows.Count > 0)
                        {
                            objUserOnBoarding.CurrentRole = dsUserRoles.Tables[0].Rows[0]["TITLE"].ToString();
                        }
                    }

                    if (item["Name"] != null && !(item["Name"] is DBNull))
                        objUserOnBoarding.Name = item["Name"].ToString();
                    if (item["ProjectId"] != null && !(item["ProjectId"] is DBNull))
                        objUserOnBoarding.ProjectId = Convert.ToInt32(item["ProjectId"].ToString());
                    //if (item["Project"] != null && !(item["Project"] is DBNull))
                    //    objUserOnBoarding.ProjectName = item["Project"].ToString();

                    if (isExcelDownload)
                    {
                        List<UserSkill> lstUserSkills = lstSkills.Where(s => s.UserId == objUserOnBoarding.UserId).ToList();
                        objUserOnBoarding.UserSkills = lstUserSkills;

                        List<UserTraining> lstUserTrainings = lstTrainings.Where(s => s.UserId == objUserOnBoarding.UserId).ToList();
                        objUserOnBoarding.UserTrainings = lstUserTrainings;

                        List<UserAssessment> lstUserAssessments = lstAssessments.Where(s => s.UserId == objUserOnBoarding.UserId).ToList();
                        objUserOnBoarding.UserAssessments = lstUserAssessments;
                    }
                    lstUserOnBoarding.Add(objUserOnBoarding);
                }
            }
            return lstUserOnBoarding;
        }

        public List<Competence> GetCompetenciesBySkillId(int SkillID)
        {
            List<Competence> competencies = new List<Competence>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetCompetenciesBySkillId]", CommandType.StoredProcedure, new SqlParameter("@SkillID", SkillID));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Competence item = new Competence();
                        if (row["ID"] != null && !(row["ID"] is DBNull))
                            item.CompetenceId = Int32.Parse(row["ID"].ToString());
                        if (row["Title"] != null && !(row["Title"] is DBNull))
                            item.CompetenceName = row["Title"].ToString();

                        competencies.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetCompetenciesBySkillId", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return competencies;
        }
        public void ChangeUserActivation(int userid)
        {
            try
            {
                DataHelper dh = new DataHelper(strConnectionString);
                dh.ExecuteNonQuery("proc_ManageUserActivation", CommandType.StoredProcedure, new SqlParameter("@userId", userid));
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,ChangeUserActivation", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }

        }

        public List<Competence> GetCompetenciesBySkillName(string name)
        {
            List<Competence> competencies = new List<Competence>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetCompetenciesBySkillName]", CommandType.StoredProcedure, new SqlParameter("@SkillName", name));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Competence item = new Competence();
                        item.CompetenceId = Int32.Parse(row["ID"].ToString());
                        item.CompetenceName = row["Title"].ToString();
                        competencies.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetCompetenciesBySkillName", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return competencies;
        }

        private List<UserAssessment> GetUserAssessments(int userId)
        {
            List<UserAssessment> userAssessmentCollection = new List<UserAssessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] parameters =
                {
                        new SqlParameter("@UserID", SqlDbType.Int) { Value = userId, Direction = ParameterDirection.Input }
                  };

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessmentsForUser]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            UserAssessment userAssessment = new UserAssessment();
                            if (item["AssessmentId"] != null && !(item["AssessmentId"] is DBNull))
                                userAssessment.Id = Convert.ToInt32(item["AssessmentId"].ToString());
                            if (item["Assessment"] != null && !(item["Assessment"] is DBNull))
                                userAssessment.TrainingAssessment = item["Assessment"].ToString();
                            if (item["SkillId"] != null && !(item["SkillId"] is DBNull))
                                userAssessment.SkillId = Convert.ToInt32(item["SkillId"].ToString());
                            if (item["IsAssessmentComplete"] != null && !(item["IsAssessmentComplete"] is DBNull))
                                userAssessment.IsAssessmentComplete = Convert.ToBoolean(item["IsAssessmentComplete"]);

                            userAssessmentCollection.Add(userAssessment);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetUserAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userAssessmentCollection;
        }
        //public ListItemCollection CacheConfig()
        //{
        //    return null;
        //}
        public List<UserAssessment> GetUserAssessmentsByID(int ID)
        {
            List<UserAssessment> userAssessments = new List<UserAssessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserAssessmentsByID]", CommandType.StoredProcedure, new SqlParameter("@ID", ID));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserAssessment item = new UserAssessment();
                        item.TrainingAssessment = row["Assessment"].ToString();
                        if (!(row["CompletedDate"] is DBNull))
                            item.CompletedDate = Convert.ToDateTime(row["CompletedDate"]).ToString();
                        if (!(row["MarksInPercentage"] is DBNull))
                            item.MarksInPercentage = Convert.ToDecimal(row["MarksInPercentage"]);
                        userAssessments.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetUserAssessmentsByID", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userAssessments;
        }

        public string GetUserCompetencyLabel(int userid)
        {
            object competencyName = String.Empty;
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                competencyName = dh.ExecuteScalar("[dbo].[proc_GetUserCompetancyLabel]", CommandType.StoredProcedure, new SqlParameter("@userid", userid));
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserCompetencyLabel", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return competencyName.ToString();
        }

        public List<AcademyJoinersCompletion> GetCurrentUserAssessments(int Id, bool updateAttempt)
        {
            List<AcademyJoinersCompletion> academyJoiners = new List<AcademyJoinersCompletion>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetCurrentUserAssessments]", CommandType.StoredProcedure, new SqlParameter("@UserId", currentUser.id));

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (Id == Convert.ToInt32(row["AssessmentID"].ToString()))
                            {
                                AcademyJoinersCompletion item = new AcademyJoinersCompletion();
                                if (row["SkillID"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["SkillID"].ToString()))
                                        item.skillLookUpId = Int32.Parse(row["SkillID"].ToString());
                                }
                                if (row["Skill"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["Skill"].ToString()))
                                        item.skillLookUpText = row["Skill"].ToString();
                                }
                                if (row["LastDayCompletion"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()) && !(row["LastDayCompletion"] is DBNull))
                                        item.lastDayCompletion = Convert.ToDateTime(row["LastDayCompletion"]);
                                }
                                if (row["IsMandatory"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["IsMandatory"].ToString()))
                                        item.isMandatory = Convert.ToBoolean(row["IsMandatory"]);
                                }
                                if (row["IsAssessmentComplete"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["IsAssessmentComplete"].ToString()))
                                        item.assessmentStatus = Convert.ToBoolean(row["IsAssessmentComplete"]);
                                }
                                if (row["NoOfAttempt"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["NoOfAttempt"].ToString()))
                                        item.attempts = Convert.ToInt32(row["NoOfAttempt"].ToString());
                                }
                                if (row["MarksObtained"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["MarksObtained"].ToString()))
                                        item.marksSecured = Convert.ToInt32(row["MarksObtained"]);
                                }
                                item.maxAttempts = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxAttempts"]) == -1 ? int.MaxValue : Convert.ToInt32(ConfigurationManager.AppSettings["MaxAttempts"]);
                                if (row["AssessmentID"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["AssessmentID"].ToString()))
                                    {
                                        item.trainingAssessmentLookUpId = Convert.ToInt32(row["AssessmentID"]);
                                        item.id = Convert.ToInt32(row["AssessmentID"]);
                                    }
                                }
                                if (row["Assessment"] != null)
                                {
                                    if (!String.IsNullOrEmpty(row["Assessment"].ToString()))
                                        item.trainingAssessmentLookUpText = row["Assessment"].ToString();
                                }
                                academyJoiners.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetCurrentUserAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return academyJoiners;
        }

        public List<AcademyJoinersCompletion> GetCurrentUserAssessments(bool updateAttempt)
        {
            List<AcademyJoinersCompletion> academyJoiners = new List<AcademyJoinersCompletion>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetCurrentUserAssessments]", CommandType.StoredProcedure, new SqlParameter("@UserId", currentUser.id));

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            AcademyJoinersCompletion item = new AcademyJoinersCompletion();
                            if (row["SkillID"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["SkillID"].ToString()))
                                    item.skillLookUpId = Int32.Parse(row["SkillID"].ToString());
                            }
                            if (row["Skill"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["Skill"].ToString()))
                                    item.skillLookUpText = row["Skill"].ToString();
                            }
                            if (row["LastDayCompletion"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()) && !(row["LastDayCompletion"] is DBNull))
                                    item.lastDayCompletion = Convert.ToDateTime(row["LastDayCompletion"]);
                            }
                            if (row["IsMandatory"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["IsMandatory"].ToString()))
                                    item.isMandatory = Convert.ToBoolean(row["IsMandatory"]);
                            }
                            if (row["IsAssessmentComplete"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["IsAssessmentComplete"].ToString()))
                                    item.assessmentStatus = Convert.ToBoolean(row["IsAssessmentComplete"]);
                            }
                            if (row["NoOfAttempt"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["NoOfAttempt"].ToString()))
                                    item.attempts = Convert.ToInt32(row["NoOfAttempt"].ToString());
                            }
                            if (row["MarksObtained"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["MarksObtained"].ToString()))
                                    item.marksSecured = Convert.ToInt32(row["MarksObtained"]);
                            }
                            item.maxAttempts = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxAttempts"]) == -1 ? int.MaxValue : Convert.ToInt32(ConfigurationManager.AppSettings["MaxAttempts"]);
                            if (row["AssessmentID"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["AssessmentID"].ToString()))
                                {
                                    item.trainingAssessmentLookUpId = Convert.ToInt32(row["AssessmentID"]);
                                    item.id = Convert.ToInt32(row["AssessmentID"]);
                                }
                            }
                            if (row["Assessment"] != null)
                            {
                                if (!String.IsNullOrEmpty(row["Assessment"].ToString()))
                                    item.trainingAssessmentLookUpText = row["Assessment"].ToString();
                            }
                            academyJoiners.Add(item);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetCurrentUserAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return academyJoiners;
        }
        public List<UserTrainingDetail> GetTrainingItems()
        {
            List<UserTrainingDetail> userTrainingDetails = new List<UserTrainingDetail>();
            userTrainingDetails = GetTrainingDetails();
            return userTrainingDetails;

        }
        private List<UserTrainingDetail> GetTrainingDetails()
        {
            List<UserTrainingDetail> trainingDetails = new List<UserTrainingDetail>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetTrainingDetails]", CommandType.StoredProcedure, new SqlParameter("@userID", currentUser.id));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.AdminApprovalStatus = row["AdminApprovalStatus"].ToString();
                        item.SkillId = Int32.Parse(row["SkillId"].ToString());
                        item.SkillName = row["Skill"].ToString();
                        item.TrainingId = Int32.Parse(row["TrainingID"].ToString());
                        item.TrainingName = row["TrainingName"].ToString();
                        if (!((row["CompletedDate"]) is DBNull))
                            item.CompletionDate = Convert.ToDateTime(row["CompletedDate"]).ToShortDateString();

                        item.IsLink = true;
                        item.DocumentUrl = null;
                        if (row["IsMandatory"] != null)
                            item.Mandatory = Convert.ToBoolean(row["IsMandatory"]);
                        if (row["IsTrainingCompleted"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["IsTrainingCompleted"].ToString()))
                                item.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);
                        }
                        else
                            item.IsTrainingCompleted = false;
                        //if (item.IsTrainingCompleted)
                        //{

                        //}
                        if (row["LastDayCompletion"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()))
                            {
                                //  item.LastDayToComplete = DateTime.ParseExact(row["LastDayCompletion"].ToString(), "dd/MM/yy", CultureInfo.InvariantCulture);                                
                                item.LastDayToComplete = Convert.ToDateTime(row["LastDayCompletion"]);
                            }
                        }
                        item.status = Utilities.GetTraningStatus(item.IsTrainingCompleted, item.LastDayToComplete);
                        item.bgColor = Utilities.GetTrainingColor(item.status);
                        item.TrainingType = TrainingType.SkillTraining;
                        if (row["TrainingLink"] != null)
                            item.LinkUrl = row["TrainingLink"].ToString();
                        trainingDetails.Add(item);
                    }
                }


            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetTrainingDetails", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            GetUserRoleBasedTraining(ref trainingDetails, currentUser.id);
            return trainingDetails;
        }
        public void GetUserRoleBasedTraining(ref List<UserTrainingDetail> trainings, int userid)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserRoleBasedTraining]", CommandType.StoredProcedure, new SqlParameter("@userID", currentUser.id));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.SkillId = Int32.Parse(row["RoleId"].ToString());
                        item.SkillName = row["Role"].ToString();
                        item.TrainingId = Int32.Parse(row["TrainingID"].ToString());
                        item.TrainingName = row["TrainingName"].ToString();
                        if (!((row["CompletedDate"]) is DBNull))
                            item.CompletionDate = Convert.ToDateTime(row["CompletedDate"]).ToShortDateString();

                        item.IsLink = true;
                        item.DocumentUrl = null;
                        if (row["IsMandatory"] != null)
                            item.Mandatory = Convert.ToBoolean(row["IsMandatory"]);
                        if (row["IsTrainingCompleted"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["IsTrainingCompleted"].ToString()))
                                item.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);
                        }

                        if (item.IsTrainingCompleted)
                        {
                            item.status = Utilities.GetTraningStatus(item.IsTrainingCompleted, item.LastDayToComplete);
                        }
                        if (row["LastDayCompletion"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()))
                            {
                                item.LastDayToComplete = Convert.ToDateTime(row["LastDayCompletion"]);
                            }
                        }
                        item.bgColor = Utilities.GetTrainingColor(item.status);
                        item.TrainingType = TrainingType.RoleTraining;
                        if (row["URL"] != null)
                            item.LinkUrl = row["URL"].ToString();
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog("SQLServerDAL,GetUserRoleBasedTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        public List<UserTrainingDetail> GetRoleBasedTrainingsForUser(int userID)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserRoleBasedTraining]", CommandType.StoredProcedure, new SqlParameter("@userID", userID));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.SkillId = Int32.Parse(row["RoleId"].ToString());
                        item.SkillName = row["Role"].ToString();
                        item.TrainingId = Int32.Parse(row["TrainingID"].ToString());
                        item.TrainingName = row["TrainingName"].ToString();
                        if (!((row["CompletedDate"]) is DBNull))
                            item.CompletionDate = Convert.ToDateTime(row["CompletedDate"]).ToShortDateString();

                        item.IsLink = true;
                        item.DocumentUrl = null;
                        if (row["IsMandatory"] != null)
                            item.Mandatory = Convert.ToBoolean(row["IsMandatory"]);
                        if (row["IsTrainingCompleted"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["IsTrainingCompleted"].ToString()))
                                item.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);
                        }

                        if (item.IsTrainingCompleted)
                        {
                            item.status = Utilities.GetTraningStatus(item.IsTrainingCompleted, item.LastDayToComplete);
                        }
                        if (row["LastDayCompletion"] != null)
                        {
                            if (!String.IsNullOrEmpty(row["LastDayCompletion"].ToString()))
                            {
                                item.LastDayToComplete = Convert.ToDateTime(row["LastDayCompletion"]);
                            }
                        }
                        item.bgColor = Utilities.GetTrainingColor(item.status);
                        item.TrainingType = TrainingType.RoleTraining;
                        if (row["URL"] != null)
                            item.LinkUrl = row["URL"].ToString();
                        trainings.Add(item);
                    }
                }
                return trainings;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserRoleBasedTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                return trainings;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        public List<UserSkillDetail> GetUserTrainingsDetails(string SPlistName)
        {
            List<UserSkillDetail> trainingCourses = new List<UserSkillDetail>();
            try
            {
                List<UserTrainingDetail> userTrainings = GetTrainingDetails();
                if (userTrainings != null && userTrainings.Count > 0)
                {
                    trainingCourses = GetCourseWiseTrainingModules(userTrainings);
                    return trainingCourses.OrderBy(x => x.lastDayToComplete).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserTrainingsDetails", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            return trainingCourses;
        }

        public List<UserSkillDetail> GetTrainingJourneyDetails(int userId)
        {
            List<UserSkillDetail> userskillDetails = new List<UserSkillDetail>();
            List<UserSkill> userSkills = GetSkillForOnboardedUser(userId);

            List<UserTrainingDetail> userTrainings = GetSkillBasedTrainingsList(userId);
            List<UserAssessment> userAssessments = GetUserAssessments(userId);
            if (userSkills != null && userSkills.Count > 0)
            {
                foreach (var skill in userSkills)
                {
                    UserSkillDetail skdetail = new UserSkillDetail();
                    skdetail.id = skill.SkillId;
                    skdetail.skillName = skill.Skill;
                    skdetail.listOfTraining = userTrainings.FindAll(s => s.SkillId == skill.SkillId);
                    List<UserAssessment> skillwiseAssessments = userAssessments.Where(u => u.SkillId == skill.SkillId).ToList();
                    skdetail.skillStatus = Utilities.GetCourseStatus(skdetail.listOfTraining, skillwiseAssessments); // FailedOngoing, OverdueOngoing- RED|Blue
                    userskillDetails.Add(skdetail);
                }
            }
            return userskillDetails;
        }
        public List<SkillCompetencies> GetSkillCompetencyTraingings()
        {
            List<SkillCompetencies> trainings = new List<SkillCompetencies>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillCompetencyTrainings]", CommandType.StoredProcedure);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            SkillCompetencies skillCompetencies = new SkillCompetencies();
                            skillCompetencies.CompetenceId = Convert.ToInt32(row["CompetencyID"]);
                            skillCompetencies.CompetenceName = row["CompetencyLevel"].ToString();
                            skillCompetencies.TrainingDescription = row["Description"].ToString();
                            skillCompetencies.TrainingId = Convert.ToInt32(row["TrainingId"]);
                            skillCompetencies.TrainingName = row["TrainingName"].ToString();
                            skillCompetencies.SkillId = Convert.ToInt32(row["SkillId"].ToString());
                            trainings.Add(skillCompetencies);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetSkillCompetencyTraingings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public List<UserRole> GetRoleForOnboardedUser(int userId)
        {
            List<UserRole> lstUserRoles = new List<UserRole>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserRoles]", CommandType.StoredProcedure, new SqlParameter("@UserId", userId));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            UserRole u = new UserRole();
                            u.RoleId = Convert.ToInt32(row["ID"].ToString());
                            u.RoleName = row["TITLE"].ToString();
                            lstUserRoles.Add(u);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoleForOnboardedUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return lstUserRoles;
        }



        public bool AddRole(string email, string userId, string roleId, bool ismandatory, DateTime lastdayofcompletion)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId), new SqlParameter("@roleId", roleId), new SqlParameter("@isMandatory", ismandatory), new SqlParameter("@lastdayofcompletion", lastdayofcompletion),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_AddUserRole]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public bool RemoveUserRole(int roleId, string userId)
        {
            bool result = false;
            int count;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId), new SqlParameter("@roleId", roleId),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                count = dh.ExecuteNonQuery("[dbo].[proc_RemoveUserRole]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,RemoveUserRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public List<Project> GetAllProjects()
        {
            List<Project> projects = new List<Project>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllProjects]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Project item = new Project();
                        item.id = Int32.Parse(row["ID"].ToString());
                        item.projectName = row["Title"].ToString();
                        //item.Skills = ??? 
                        projects.Add(item);
                    }
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return projects;
        }

        public bool AddProject(string projectName)
        {
            bool status = false;
            int count;
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@Title",SqlDbType.VarChar ) { Value = projectName},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output },
            };
            try
            {
                count = dh.ExecuteNonQuery("[dbo].[proc_AddProject]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[1].Value);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }

        public bool UpdateProject(Project project)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@Title",SqlDbType.VarChar ),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = project.id;
                parameters[1].Value = project.projectName;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Size = 4000;
                parameters[3].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateProject]", CommandType.StoredProcedure, parameters);
                result = true;
                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog("SQLServerDAL,UpdateProject", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", currentUser.emailId.ToString());
                    result = false;
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public bool RemoveProject(int projectID)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = projectID;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteProject]", CommandType.StoredProcedure, parameters);
                result = true;
                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,RemoveProject", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", currentUser.emailId.ToString());

                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,RemoveProject", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public Project EditProjectByID(int projectID)
        {
            Project project = new Project();
            project.id = projectID;
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProjectID",SqlDbType.Int)
                };
                parameters[0].Value = project.id;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetProjectById]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    project.projectName = dt.Rows[0]["Title"].ToString();
                    project.parentProjectId = Convert.ToInt32(dt.Rows[0]["ParentProjectId"].ToString());
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return project;
        }


        public bool EditCompetenceByID(int competenceID, string level)
        {
            bool status = false;
            bool result = false;
            Competence competence = new Competence();
            competence.CompetenceId = competenceID;
            competence.CompetenceName = level;
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            DataView dv = new DataView();
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@Level",SqlDbType.NVarChar),
                    new SqlParameter("@Status",SqlDbType.Bit)
                };
                parameters[0].Value = competence.CompetenceId;
                parameters[1].Value = competence.CompetenceName;
                parameters[2].Value = status;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateCompetencyLevel]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[2].Value);
                if (status)
                {
                    result = true;
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public Resource GetResourceDetailsByProjectID(int projectID)
        {
            Resource resource = new Resource();
            resource.projectId = projectID;
            resource.allResources = new List<AllResources>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
               {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ProjectName",SqlDbType.VarChar)
                };
                parameters[0].Value = projectID;
                parameters[1].Size = 255;
                parameters[1].Direction = ParameterDirection.Output;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAvailableSkillResourceCountByProjectId]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                resource.projectName = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
                Hashtable objHashTable = new Hashtable();
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!objHashTable.ContainsKey(dr["SkillId"]))
                        {
                            objHashTable.Add(dr["SkillId"], dr["SkillId"]);
                            AllResources objAllResources = new AllResources();
                            objAllResources.skillId = Convert.ToInt32(dr["SkillId"]);
                            objAllResources.skill = dr["SkillName"].ToString();
                            resource.allResources.Add(objAllResources);
                        }
                    }
                }
                foreach (AllResources item in resource.allResources)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (item.skillId == Convert.ToInt32(dr["SkillId"]))
                        {
                            switch (dr["CompetencyLevel"].ToString().ToUpper())
                            {
                                case "NOVICE":
                                    item.expectedBeginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                    item.availableBeginnerCount = Convert.ToInt32(dr["AvailableResourceCount"].ToString() == String.Empty ? "0" : dr["AvailableResourceCount"].ToString());
                                    break;
                                case "ADVANCED BEGINNER":
                                    item.expectedAdvancedBeginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                    item.availableAdvancedBeginnerCount = Convert.ToInt32(dr["AvailableResourceCount"].ToString() == String.Empty ? "0" : dr["AvailableResourceCount"].ToString());
                                    break;

                                case "COMPETENT":
                                    item.expectedCompetentCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                    item.availableCompetentCount = Convert.ToInt32(dr["AvailableResourceCount"].ToString() == String.Empty ? "0" : dr["AvailableResourceCount"].ToString());
                                    break;

                                case "PROFICIENT":
                                    item.expectedProficientCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                    item.availableProficientCount = Convert.ToInt32(dr["AvailableResourceCount"].ToString() == String.Empty ? "0" : dr["AvailableResourceCount"].ToString());
                                    break;

                                case "EXPERT":
                                    item.expectedExpertCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                    item.availableExpertCount = Convert.ToInt32(dr["AvailableResourceCount"].ToString() == String.Empty ? "0" : dr["AvailableResourceCount"].ToString());
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return resource;
        }

        public ProjectDetails GetProjectSkillsByProjectID(string projectID)
        {
            ProjectDetails objProjectDetails = new ProjectDetails();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
               {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ProjectName",SqlDbType.VarChar)
                };
                parameters[0].Value = Int32.Parse(projectID);
                parameters[1].Size = 255;
                parameters[1].Direction = ParameterDirection.Output;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillsByProjectId]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                objProjectDetails.projectId = Int32.Parse(projectID);
                objProjectDetails.projectName = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
                objProjectDetails.projectSkill = new List<ProjectSkill>();
                ProjectSkill objProjectSkill = null;

                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        objProjectSkill = new ProjectSkill();

                        objProjectSkill.itemId = Int32.Parse(row["ItemId"].ToString());
                        objProjectSkill.project = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
                        objProjectSkill.projectId = Int32.Parse(row["ProjectId"].ToString());
                        objProjectSkill.skill = row["SkillName"].ToString();
                        objProjectSkill.skillId = Int32.Parse(row["SkillId"].ToString());
                        objProjectDetails.projectSkill.Add(objProjectSkill);
                    }
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return objProjectDetails;
        }

        public bool PostProjectSkill(string projectid, string skillid)
        {
            bool status = false;
            ProjectSkill objProjectSkill = new ProjectSkill();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@ProjectId",SqlDbType.Int ) { Value = Int32.Parse(projectid) },
                new SqlParameter("@SkillId",SqlDbType.Int){Value = Int32.Parse(skillid)},
                new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}
            };
            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_AddProjectSkill]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }

        public bool DeleteProjectSkill(int projectskillid, string projectid, string skillid)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool status = false;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ProjectSkillID",SqlDbType.Int),
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@SkillID",SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar)
                };
                parameters[0].Value = projectskillid;
                parameters[1].Value = projectid;
                parameters[2].Value = skillid;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[4].Size = 4000;
                parameters[4].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteProjectSkill]", CommandType.StoredProcedure, parameters);
                status = true;
                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog("SQLServerDAL,DeleteProjectSkill", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", currentUser.emailId.ToString());
                    status = false;
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }


        public bool DeleteCompetencyLevel(int deletedCompetence, string level)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool status = false;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@Level",SqlDbType.NVarChar),
                    new SqlParameter("@Status",SqlDbType.Bit)
                };
                parameters[0].Value = deletedCompetence;
                parameters[1].Value = level;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Value = status;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteCompetencyLevel]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[2].Value);

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }

        public bool AddProjectSkillResources(ProjectResources prjRes)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool status = false;
            try
            {
                foreach (var item in prjRes.skillResources)
                {
                    SqlParameter[] parameters =
                    {
                    new SqlParameter("@projectId", SqlDbType.Int ) { Value = prjRes.projectId },
                    new SqlParameter("@skillId", SqlDbType.Int) { Value = item.skillId},
                    new SqlParameter("@avlNoviceCount", SqlDbType.Int) { Value = item.beginnerCount },
                    new SqlParameter("@avlAdvancedBeginnerCount", SqlDbType.Int) { Value = item.advancedBeginnerCount },
                    new SqlParameter("@avlCompetentCount", SqlDbType.Int) { Value = item.competentCount },
                    new SqlParameter("@avlProficientCount", SqlDbType.Int) { Value = item.proficientCount },
                    new SqlParameter("@avlExpertCount", SqlDbType.Int) { Value = item.expertCount},
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}
                };

                    dh.ExecuteNonQuery("[dbo].[proc_AddAvlResourceCountByProjectID]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }


        public bool AddCompetencyLevels(string competencyLevel)
        {
            bool result = false;
            bool status = false;
            int count;
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@Title",SqlDbType.VarChar ) { Value = competencyLevel},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output },
            };
            try
            {
                count = dh.ExecuteNonQuery("[dbo].[proc_AddCompetencyLevel]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[1].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;

        }

        public bool AddExpectedProjectResourceCountByProjectId(ProjectResources prjRes)
        {
            bool status = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                foreach (var item in prjRes.skillResources)
                {
                    SqlParameter[] parameters =
                    {
                    new SqlParameter("@projectId", SqlDbType.Int) { Value = prjRes.projectId },
                    new SqlParameter("@skillId", SqlDbType.Int) { Value = item.skillId },
                    new SqlParameter("@exptdNoviceCount", SqlDbType.Int) { Value = item.beginnerCount },
                    new SqlParameter("@exptdAdvancedBeginnerCount", SqlDbType.Int) { Value = item.advancedBeginnerCount },
                    new SqlParameter("@exptdCompetentCount", SqlDbType.Int) { Value = item.competentCount },
                    new SqlParameter("@exptdProficientCount", SqlDbType.Int) { Value = item.proficientCount },
                    new SqlParameter("@exptdExpertCount", SqlDbType.Int) { Value = item.expertCount },
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output}
                    };

                    dh.ExecuteNonQuery("[dbo].[proc_AddExpecedResourceCountByProjectID]", CommandType.StoredProcedure, parameters);
                    if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }

        public bool AddNewsEvent(string imageURL, string header, string body, string trimBody)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@imageURL", imageURL), new SqlParameter("@Header", header), new SqlParameter("@Body", body),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_AddNews]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }


        public void GetExpectedProjectResourceCountByProjectId(ProjectResources prjRes)
        {
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
               {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ProjectName",SqlDbType.VarChar)
                };
                parameters[0].Value = prjRes.projectId;
                parameters[1].Size = 255;
                parameters[1].Direction = ParameterDirection.Output;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetExpectedSkillResourceCountByProjectId]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            prjRes.projectName = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
            List<SkillResource> lstSkillResource = new List<SkillResource>();
            Hashtable objHashTable = new Hashtable();
            DataTable dt = dv.ToTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!objHashTable.ContainsKey(dr["SkillId"]))
                    {
                        objHashTable.Add(dr["SkillId"], dr["SkillId"]);
                        SkillResource objSkillResource = new SkillResource();
                        objSkillResource.skillId = Convert.ToInt32(dr["SkillId"]);
                        objSkillResource.skill = dr["SkillName"].ToString();
                        lstSkillResource.Add(objSkillResource);
                    }
                }
            }
            prjRes.skillResources = lstSkillResource;

            foreach (SkillResource skr in prjRes.skillResources)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (skr.skillId == Convert.ToInt32(dr["SkillId"]))
                    {
                        switch (dr["CompetencyLevel"].ToString().ToUpper())
                        {
                            case "NOVICE":
                                skr.beginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;
                            case "ADVANCED BEGINNER":
                                skr.advancedBeginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "COMPETENT":
                                skr.competentCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "PROFICIENT":
                                skr.proficientCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "EXPERT":
                                skr.expertCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;
                            default:
                                break;
                        }
                    }

                }
            }

        }

        public bool OnboardEmail(string email, int UserId, string UserName)
        {
            bool result = false;
            try
            {
                if (UserId == 0)
                    UserId = GetUserId(email);
                Hashtable hashtable = new Hashtable();
                Hashtable Training_HT = new Hashtable();

                string strAssessmentname = string.Empty;

                List<UserAssessment> AssessmentLi = GetAssessmentForUser(UserId);
                foreach (UserAssessment itemAsses in AssessmentLi)
                {
                    DateTime dt = Convert.ToDateTime(itemAsses.LastDayCompletion);
                    strAssessmentname = strAssessmentname + itemAsses.TrainingAssessment + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
                }
                string strTrainingName = string.Empty;
                List<UserTraining> trainingLi = GetTrainingForUser(UserId, true);
                List<UserTrainingDetail> roleTrainings = new List<UserTrainingDetail>();
                roleTrainings = GetRoleBasedTrainingsForUser(UserId);
                foreach (UserTraining itemTraining in trainingLi)
                {
                    DateTime dt = Convert.ToDateTime(itemTraining.LastDayCompletion);
                    strTrainingName = strTrainingName + itemTraining.TrainingName + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
                }

                hashtable.Add("UserName", UserName);
                hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                hashtable.Add("WebUrl", "");
                hashtable.Add("Assessment", strAssessmentname);
                hashtable.Add("Training", strTrainingName);
                bool Queue1 = AddToEmailQueue("EmployeeOnboardMail", hashtable, email, null);

                string trainingTable = string.Empty;
                trainingTable += "<table border='1' cellspacing='0' cellpadding='0' style='border-collapse: collapse; border: none;'>";
                trainingTable += "<tbody>";
                trainingTable += "<tr>";
                trainingTable += "<td><b>Skill Based Training Name</b></td>";
                trainingTable += "<td><b>Last Date of Completion</b></td>";
                trainingTable += "<td><b>Mandatory?</b></td>";
                trainingTable += "</tr>";

                foreach (UserTraining item in trainingLi)
                {
                    trainingTable += "<tr>";
                    trainingTable += "<td>" + item.TrainingName + "</td>";
                    trainingTable += "<td>" + item.LastDayCompletion + "</td>";
                    trainingTable += "<td>" + item.IsMandatory + "</td>";
                    trainingTable += "</tr>";
                }

                trainingTable += "<tr>";
                trainingTable += "<td><b>Role Based Training Name</b></td>";
                trainingTable += "<td><b>Last Date of Completion</b></td>";
                trainingTable += "<td><b>Mandatory?</b></td>";
                trainingTable += "</tr>";

                foreach (UserTrainingDetail item in roleTrainings)
                {
                    trainingTable += "<tr>";
                    trainingTable += "<td>" + item.TrainingName + "</td>";
                    trainingTable += "<td>" + item.LastDayToComplete + "</td>";
                    trainingTable += "<td>" + item.Mandatory + "</td>";
                    trainingTable += "</tr>";
                }

                trainingTable += "</tbody>";
                trainingTable += "</table>";
                Hashtable data = new Hashtable();
                data.Add("TrainingTable", trainingTable);
                data.Add("UserName", UserName);
                data.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                data.Add("WebUrl", "");
                data.Add("Assessment", strAssessmentname);
                bool Queue2 = AddToEmailQueue("EmployeeOnboardTrainingMail", data, email, null);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,OnboardEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }


        public bool AddSkillEmail(string email, int UserId, string UserName, int skillId, string competence)
        {
            if (UserId == 0)
                UserId = GetUserId(email);
            bool result = false;
            string strAssessmentname = string.Empty;
            UserManager user = GetUsersByID(UserId);
            UserName = user.UserName;

            List<UserAssessment> AssessmentLi = GetAssessmentForUser(UserId);
            foreach (UserAssessment itemAsses in AssessmentLi)
            {
                DateTime dt = Convert.ToDateTime(itemAsses.LastDayCompletion);
                strAssessmentname = strAssessmentname + itemAsses.TrainingAssessment + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
            }
            string strTrainingName = string.Empty;
            List<UserTraining> trainingLi = GetTrainingForUser(UserId, true);
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();
            trainings = GetRoleBasedTrainingsForUser(UserId);
            foreach (UserTraining itemTraining in trainingLi)
            {
                if (!itemTraining.IsTrainingCompleted)
                {
                    DateTime dt = Convert.ToDateTime(itemTraining.LastDayCompletion);
                    strTrainingName = strTrainingName + itemTraining.TrainingName + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
                }
            }
            try
            {
                string skillTrainingTable = string.Empty;
                skillTrainingTable += "<table border='1' cellspacing='0' cellpadding='0' style='border-collapse: collapse; border: none;'>";
                skillTrainingTable += "<tbody>";
                skillTrainingTable += "<tr>";
                skillTrainingTable += "<td><b>Training Name</b></td>";
                skillTrainingTable += "<td><b>Last Date of Completion</b></td>";
                skillTrainingTable += "<td><b>Mandatory?</b></td>";
                skillTrainingTable += "</tr>";

                foreach (UserTraining item in trainingLi)
                {
                    if (item.SkillId == skillId)
                    {
                        if (!item.IsTrainingCompleted)
                        {
                            skillTrainingTable += "<tr>";
                            skillTrainingTable += "<td>" + item.TrainingName + "</td>";
                            skillTrainingTable += "<td>" + item.LastDayCompletion + "</td>";
                            skillTrainingTable += "<td>" + item.IsMandatory + "</td>";
                            skillTrainingTable += "</tr>";
                        }
                    }
                }
                skillTrainingTable += "</tbody>";
                skillTrainingTable += "</table>";
                Hashtable data = new Hashtable();
                data.Add("TrainingTable", skillTrainingTable);
                data.Add("UserName", UserName);
                data.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                data.Add("WebUrl", "");
                data.Add("Assessment", strAssessmentname);
                bool queue = AddToEmailQueue("EmployeeOnboardTrainingMail", data, user.EmailID, null);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //UserManager user = (UserManager)HttpContext.Current.Session["CurrentUser"];
                LogHelper.AddLog("SQLServerDAL,OnboardEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public bool ProcessRemiderEmail()
        {
            LogHelper.AddLog("SQLServerDAL,ProcessRemiderEmail", "In ProcessRemiderEmail", "In ProcessRemiderEmail", "HCL.Academy.DAL", currentUser.emailId.ToString());
            bool result = false;
            try
            {

                DataSet dsTraining = new DataSet();
                DataTable dtTraining = new DataTable();
                DataHelper data = new DataHelper(strConnectionString);
                dsTraining = data.ExecuteDataSet("[dbo].[proc_SendReminders]", CommandType.StoredProcedure);
                string strTrainingName = String.Empty;
                List<int> lstUserIDs = new List<int>();
                if (dsTraining != null)
                {
                    dtTraining = dsTraining.Tables[0];
                    if (dsTraining.Tables[0].Rows != null)
                    {
                        foreach (DataRow row in dtTraining.Rows)
                        {
                            int userID = Convert.ToInt32(row["UserId"]);
                            lstUserIDs.Add(userID);
                        }
                        lstUserIDs = lstUserIDs.Distinct().ToList();
                    }
                    result = SendReminderEmails(lstUserIDs);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,ProcessRemiderEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }



        public bool SendReminderEmails(List<int> userIDs)
        {
            bool status = false;
            //string strTrainingName = String.Empty;
            string UserName = String.Empty;
            string email = String.Empty;
            foreach (int id in userIDs)
            {
                List<UserTraining> training = new List<UserTraining>();
                training = GetTrainingForUser(id, false);
                UserManager users = GetUsersByID(id);
                UserName = users.UserName;
                email = users.EmailID;
                //foreach (UserTraining itemTraining in training)
                //{
                //    if (!itemTraining.IsTrainingCompleted)
                //    {
                //        DateTime dt = Convert.ToDateTime(itemTraining.LastDayCompletion);
                //        strTrainingName = strTrainingName + itemTraining.TrainingName + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
                //    }
                //}
                try
                {
                    string trainingTable = string.Empty;
                    trainingTable += "<table border='1' cellspacing='0' cellpadding='0' style='border-collapse: collapse; border: none;'>";
                    trainingTable += "<tbody>";
                    trainingTable += "<tr>";
                    trainingTable += "<td><b>Training Name</b></td>";
                    trainingTable += "<td><b>Last Date of Completion</b></td>";
                    trainingTable += "<td><b>Mandatory?</b></td>";
                    trainingTable += "</tr>";

                    foreach (UserTraining item in training)
                    {
                        if (!item.IsTrainingCompleted)
                        {
                            trainingTable += "<tr>";
                            trainingTable += "<td>" + item.TrainingName + "</td>";
                            trainingTable += "<td>" + item.LastDayCompletion + "</td>";
                            trainingTable += "<td>" + item.IsMandatory + "</td>";
                            trainingTable += "</tr>";
                        }
                    }
                    trainingTable += "</tbody>";
                    trainingTable += "</table>";
                    Hashtable data = new Hashtable();
                    data.Add("TrainingTable", trainingTable);
                    data.Add("UserName", UserName);
                    data.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    data.Add("WebUrl", "");

                    bool queue = AddToEmailQueue("ReminderEmail", data, email, null);
                    status = true;
                }
                catch (Exception ex)
                {
                    status = false;
                    LogHelper.AddLog("SQLServerDAL,OnboardEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                }
            }
            return status;
        }

        public List<Role> GetRoles()
        {
            List<Role> allRoles = new List<Role>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllRoles]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            Role r = new Role();
                            r.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString());
                            r.Title = ds.Tables[0].Rows[i]["RoleName"].ToString();
                            allRoles.Add(r);
                        }
                    }
                }
                // if (HttpContext.Current != null)
                // HttpContext.Current.Session[AppConstant.AllRoleData] = roles;

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            List<Role> returnRoles = allRoles.OrderBy(r => r.Id).ToList();
            return returnRoles;
        }

        public List<Role> GetRolesWithSkills()
        {
            List<Role> allRoles = new List<Role>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetRolesWithSkill]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            Role r = new Role();
                            r.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString());
                            r.Title = ds.Tables[0].Rows[i]["RoleName"].ToString();
                            allRoles.Add(r);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            List<Role> returnRoles = allRoles.OrderBy(r => r.Id).ToList();
            return returnRoles;
        }

        public List<SkillGapReport> GetSkillGapReports(int roleID)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            List<SkillGapReport> reports = new List<SkillGapReport>();
            try
            {
                DataSet ds = new DataSet();
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@RoleID";
                sqlParameters[0].Value = roleID;
                sqlParameters[0].Direction = ParameterDirection.Input;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillGapReport]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            SkillGapReport r = new SkillGapReport();
                            r.EmployeeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                            r.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                            r.EmailID = ds.Tables[0].Rows[i]["EmailID"].ToString();
                            r.ExpectedCompetencyLevel = ds.Tables[0].Rows[i]["ExpectedCompetency"].ToString();
                            r.ActualCompetencyLevel = ds.Tables[0].Rows[i]["ActualCompetency"].ToString();
                            r.Skill = ds.Tables[0].Rows[i]["Skill"].ToString();
                            reports.Add(r);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoles", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return reports;
        }

        public bool AddRole(string roleName)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@RoleTitle",SqlDbType.NVarChar),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = roleName;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_AddRole]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,AddRole", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", currentUser.emailId.ToString());

                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }
        public bool UpdateRole(int roleId, string roleName)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@RoleId",SqlDbType.Int),
                        new SqlParameter("@RoleTitle",SqlDbType.NVarChar),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = roleId;
                parameters[1].Value = roleName;
                parameters[1].Size = 100;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Size = 4000;
                parameters[3].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_UpdateRole]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,UpdateRole", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,UpdateRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }
        public List<RoleTraining> GetAllRoleTrainings()
        {
            List<RoleTraining> roleTrainings = new List<RoleTraining>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllRoleTrainings]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RoleTraining r = new RoleTraining();

                            r.RoleTrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"].ToString()); ;
                            r.RoleId = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString());
                            r.RoleName = ds.Tables[0].Rows[i]["RoleName"].ToString();
                            r.TrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingId"].ToString()); ;
                            r.TrainingName = ds.Tables[0].Rows[i]["TrainingName"].ToString();
                            r.IsMandatory = Convert.ToBoolean(ds.Tables[0].Rows[i]["RoleTrainingIsMandatory"].ToString());

                            roleTrainings.Add(r);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllRoleTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return roleTrainings;
        }
        public List<SkillTraining> GetMasterTrainings()
        {
            List<SkillTraining> trainings = new List<SkillTraining>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllTrainings]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            SkillTraining t = new SkillTraining();

                            t.id = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"].ToString());
                            t.trainingName = ds.Tables[0].Rows[i]["TrainingTitle"].ToString();
                            trainings.Add(t);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetMasterTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }
        public bool UpdateRoleTraining(int itemId, int trainingId, int roleId, bool isMandatory)
        {
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ItemId",SqlDbType.Int) { Value = itemId},
                        new SqlParameter("@TrainingId",SqlDbType.Int) { Value = trainingId},
                        new SqlParameter("@RoleId",SqlDbType.Int) { Value = roleId},
                        new SqlParameter("@IsMandatory",SqlDbType.Bit) { Value = isMandatory},
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[4].Direction = ParameterDirection.Output;
                parameters[5].Size = 4000;
                parameters[5].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_UpdateRoleTraining]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,UpdateRoleTraining", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,UpdateRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }
        public bool AddRoleTraining(int trainingId, int roleId, bool isMandatory)
        {
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@TrainingId",SqlDbType.Int) { Value = trainingId},
                        new SqlParameter("@RoleId",SqlDbType.Int) { Value = roleId},
                        new SqlParameter("@IsMandatory",SqlDbType.Bit) { Value = isMandatory},
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[3].Direction = ParameterDirection.Output;
                parameters[4].Size = 4000;
                parameters[4].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_AddRoleTraining]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog("SQLServerDAL,AddRoleTraining", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog("SQLServerDAL,AddRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;
        }
        public bool RemoveRoleTraining(int roleTrainingId)
        {
            bool status = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@RoleTrainingId",SqlDbType.Int),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = roleTrainingId;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteRoleTraining]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    status = false;
                    LogHelper.AddLog(dh, "SQLServerDAL,RemoveRoleTraining", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    status = true;
                }

            }
            catch (Exception ex)
            {
                status = false;
                LogHelper.AddLog(dh, "SQLServerDAL,RemoveRoleTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }
        public bool RemoveRole(int roleId)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@RoleId",SqlDbType.Int),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = roleId;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteRole]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog(dh, "SQLServerDAL,RemoveRole", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog(dh, "SQLServerDAL,RemoveRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public bool AddSkillCompetencyLevel(int skill, int competence, string description, string professionalSkill, string softSkill, int compOrder, int trainingCompletionPoints, int assessmentCompletionPoints)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@SkillId",SqlDbType.Int) { Value = skill},
                        new SqlParameter("@CompetenceId",SqlDbType.Int) { Value = competence},
                        new SqlParameter("@Description",SqlDbType.NVarChar) { Value = description},
                        new SqlParameter("@ProfessionalSkill",SqlDbType.VarChar) { Value = professionalSkill},
                        new SqlParameter("@SoftSkill",SqlDbType.VarChar) { Value = softSkill},
                        new SqlParameter("@CompOrder",SqlDbType.Int) { Value = compOrder},
                        new SqlParameter("@TrainingCompletionPoints",SqlDbType.Int) { Value = trainingCompletionPoints},
                        new SqlParameter("@AssessmentCompletionPoints",SqlDbType.Int) { Value = assessmentCompletionPoints},
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[8].Direction = ParameterDirection.Output;
                parameters[9].Size = 4000;
                parameters[9].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_AddSkillCompetencyLevel]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog(dh, "SQLServerDAL,AddSkillCompetencyLevel", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog(dh, "SQLServerDAL,AddSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }

        public bool UpdateSkillCompetencyLevel(int itemid, int skill, int competence, string description, string professionalSkill, string softSkill, int compOrder, int trainingCompletionPoints, int assessmentCompletionPoints)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ItemId",SqlDbType.Int) { Value = itemid},
                        new SqlParameter("@SkillId",SqlDbType.Int) { Value = skill},
                        new SqlParameter("@CompetenceId",SqlDbType.Int) { Value = competence},
                        new SqlParameter("@Description",SqlDbType.NVarChar) { Value = description},
                        new SqlParameter("@ProfessionalSkill",SqlDbType.VarChar) { Value = professionalSkill},
                        new SqlParameter("@SoftSkill",SqlDbType.VarChar) { Value = softSkill},
                        new SqlParameter("@CompOrder",SqlDbType.Int) { Value = compOrder},
                        new SqlParameter("@TrainingCompletionPoints",SqlDbType.Int) { Value = trainingCompletionPoints},
                        new SqlParameter("@AssessmentCompletionPoints",SqlDbType.Int) { Value = assessmentCompletionPoints},
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[9].Direction = ParameterDirection.Output;
                parameters[10].Size = 4000;
                parameters[10].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_UpdateSkillCompetencyLevel]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog(dh, "SQLServerDAL,UpdateSkillCompetencyLevel", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog(dh, "SQLServerDAL,UpdateSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }

        public bool RemoveSkillCompetencyLevel(int itemId)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool result = false;
            try
            {

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ItemId",SqlDbType.Int),
                        new SqlParameter("@ErrorNumber",SqlDbType.Int),
                        new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                    };
                parameters[0].Value = itemId;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteSkillCompetencyLevel]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;
                    LogHelper.AddLog(dh, "SQLServerDAL,RemoveSkillCompetencyLevel", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.AddLog(dh, "SQLServerDAL,RemoveSkillCompetencyLevel", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());

            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return result;

        }
        public List<SkillMaster> GetAllSkillMaster()
        {
            List<SkillMaster> allSkills = new List<SkillMaster>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllSkills]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            SkillMaster skillmaster = new SkillMaster();
                            skillmaster.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]);
                            skillmaster.Title = ds.Tables[0].Rows[i]["Title"].ToString();
                            skillmaster.IsDefault = ds.Tables[0].Rows[i]["IsDefault"].ToString() == "true" ? true : false;
                            allSkills.Add(skillmaster);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAllSkillMaster", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            return allSkills;
        }
        public List<SkillCompetencyLevel> GetSkillCompetencyLevels()
        {
            List<SkillCompetencyLevel> skillCompetencyLevels = new List<SkillCompetencyLevel>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillCompetencyLevels]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            SkillCompetencyLevel s = new SkillCompetencyLevel();
                            s.ItemID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemID"].ToString());
                            s.CompetencyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CompetencyID"].ToString());
                            s.CompetencyName = ds.Tables[0].Rows[i]["CompetencyName"].ToString();
                            s.SkillID = Convert.ToInt32(ds.Tables[0].Rows[i]["SkillID"].ToString());
                            s.SkillName = ds.Tables[0].Rows[i]["SkillName"].ToString();
                            s.CompetencyLevelOrder = Convert.ToInt32(ds.Tables[0].Rows[i]["CompetencyLevelOrder"].ToString());
                            s.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                            s.ProfessionalSkills = ds.Tables[0].Rows[i]["ProfessionalSkills"].ToString();
                            s.SoftSkills = ds.Tables[0].Rows[i]["SoftSkills"].ToString();
                            s.TrainingCompletionPoints = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingCompletionPoints"]);
                            s.AssessmentCompletionPoints = Convert.ToInt32(ds.Tables[0].Rows[i]["AssessmentCompletionPoints"]);

                            skillCompetencyLevels.Add(s);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetSkillCompetencyLevels", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            List<SkillCompetencyLevel> returnData = skillCompetencyLevels.OrderBy(r => r.ItemID).ToList();
            return returnData;
        }

        public List<SkillTraining> GetSkillTrainings()
        {
            List<SkillTraining> trainings = new List<SkillTraining>();
            DataSet dsTraining = new DataSet();
            DataView dvTraining = new DataView();
            DataTable dtTraining = new DataTable();
            DataHelper dhTraining = new DataHelper(strConnectionString);
            try
            {
                dsTraining = dhTraining.ExecuteDataSet("[dbo].[proc_GetAllSkillCompetencyTrainings]", CommandType.StoredProcedure);
                dtTraining = dsTraining.Tables[0];
                if (dtTraining.Rows.Count > 0)
                {
                    foreach (DataRow row in dtTraining.Rows)
                    {
                        SkillTraining train = new SkillTraining();
                        train.trainingName = row["TrainingName"].ToString();
                        train.trainingId = row["TrainingId"].ToString();
                        train.competences = GetAllCompetenceList();
                        train.skills = GetAllSkills();
                        train.skill = row["Skill"].ToString();
                        train.skillId = Convert.ToInt32(row["SkillId"].ToString());
                        train.competency = row["Competency"].ToString();
                        train.competencyLevelId = Convert.ToInt32(row["CompetencyLevelId"].ToString());
                        train.id = Convert.ToInt32(row["ID"]);
                        train.points = Convert.ToInt32(row["Points"]);
                        train.GEO = row["GEO"].ToString();
                        train.GEOId = Convert.ToInt32(row["GEOId"].ToString());
                        train.isMandatory = Convert.ToBoolean(row["IsMandatory"]);
                        train.isAssessmentRequired = Convert.ToBoolean(row["IsAssessmentRequired"]);
                        trainings.Add(train);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhTraining, "SQLServerDAL,GetSkillTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }
            return trainings;
        }



        public bool AddTraining(string title, string description, string skillType, string category, string link, string content, string document)
        {
            bool result = false;
            bool status = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Title", title),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@SkillType", skillType),
                    new SqlParameter("@TrainingCategory", category),
                    new SqlParameter("@Link", link),
                    new SqlParameter("@ContentId",Convert.ToInt32(content)),
                    new SqlParameter("@Document",document),
                    new SqlParameter("@Status",SqlDbType.Bit)};
                parameters[7].Direction = ParameterDirection.Output;
                parameters[7].Value = status;

                int count = dh.ExecuteNonQuery("[dbo].[proc_AddTraining]", CommandType.StoredProcedure, parameters);
                if (count > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,AddTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public bool UpdateTraining(int id, string title, string description, string skillType, string category, string link, string content, string document)
        {
            bool result = false;
            bool status = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@ID",id),
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@SkillType", skillType),
                    new SqlParameter("@TrainingCategory", category),
                    new SqlParameter("@Link", link),
                    new SqlParameter("@ContentId",Convert.ToInt32(content)),
                    new SqlParameter("@Document",document),
                    new SqlParameter("@Status",SqlDbType.Bit)};
                parameters[8].Direction = ParameterDirection.Output;
                parameters[8].Value = status;

                int count = dh.ExecuteNonQuery("[dbo].[proc_UpdateTraining]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[8].Value);
                if (count > 0 && status == true)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,UpdateTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public bool DeleteTraining(int id, out bool? check)
        {
            bool result = false;
            bool status = false;
            bool exists = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@ID",id),
                    new SqlParameter("@Exists",SqlDbType.Bit),
                    new SqlParameter("@Status",SqlDbType.Bit),
                };
                check = exists;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Value = check;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Value = status;
                int count = dh.ExecuteNonQuery("[dbo].[proc_DeleteTraining]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[2].Value);
                exists = Convert.ToBoolean(parameters[1].Value);
                check = exists;
                if (count > 0 && status == true)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,DeleteTraining", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            check = exists;
            return result;
        }

        public List<TrainingContent> GetTrainingContent()
        {
            List<TrainingContent> trainingContents = new List<TrainingContent>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetTrainingContent]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            TrainingContent content = new TrainingContent();
                            content.id = Convert.ToInt32(row["ID"]);
                            content.title = row["Title"].ToString();
                            content.contentBody = row["ContentBody"].ToString();
                            content.contentUrl = row["ContentUrl"].ToString();
                            content.orderby = Convert.ToInt32(row["OrderBy"]);
                            trainingContents.Add(content);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetTrainingContent", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainingContents;
        }

        public List<TrainingMaster> GetAllMasterTrainings()
        {
            List<TrainingMaster> trainings = new List<TrainingMaster>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllMasterTrainings]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            TrainingMaster t = new TrainingMaster();

                            t.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"].ToString());
                            if (ds.Tables[0].Rows[i]["TrainingTitle"] != null && !(ds.Tables[0].Rows[i]["TrainingTitle"] is DBNull))
                                t.title = ds.Tables[0].Rows[i]["TrainingTitle"].ToString();
                            if (ds.Tables[0].Rows[i]["SkillType"] != null && !(ds.Tables[0].Rows[i]["SkillType"] is DBNull))
                                t.skillType = ds.Tables[0].Rows[i]["SkillType"].ToString();
                            if (ds.Tables[0].Rows[i]["TrainingCategory"] != null && !(ds.Tables[0].Rows[i]["TrainingCategory"] is DBNull))
                                t.trainingCategory = ds.Tables[0].Rows[i]["TrainingCategory"].ToString();
                            if (ds.Tables[0].Rows[i]["TrainingLink"] != null && !(ds.Tables[0].Rows[i]["TrainingLink"] is DBNull))
                                t.trainingLink = ds.Tables[0].Rows[i]["TrainingLink"].ToString();
                            if (ds.Tables[0].Rows[i]["TrainingContent"] != null && !(ds.Tables[0].Rows[i]["TrainingContent"] is DBNull))
                                t.selectedContent = (ds.Tables[0].Rows[i]["TrainingContent"]).ToString();
                            if (ds.Tables[0].Rows[i]["TrainingDocument"] != null && !(ds.Tables[0].Rows[i]["TrainingDocument"] is DBNull))
                                t.trainingDocument = ds.Tables[0].Rows[i]["TrainingDocument"].ToString();
                            trainings.Add(t);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAllMasterTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public bool AddRssFeeds(RSSFeedMaster rSSFeed)
        {
            bool result = false;
            bool status = false;
            string title = rSSFeed.Title;
            string titleNode = rSSFeed.TitleNode;
            string descNode = rSSFeed.DescriptionNode;
            string rssFeedUrl = rSSFeed.RSSFeedUrl;
            string pubDateNode = rSSFeed.PubDateNode;
            string itemNodePath = rSSFeed.itemNodePath;
            int rssOrder = rSSFeed.rssFeedOrder;
            string hrfNodePath = rSSFeed.hrfTitleNodePath;
            int count;
            DataHelper dhRss = new DataHelper(strConnectionString);


            SqlParameter[] parameters =
            {
                new SqlParameter("@Title",SqlDbType.NVarChar ) { Value = title},
                new SqlParameter("@TitleNodePath",SqlDbType.NVarChar ) { Value = titleNode},
                new SqlParameter("@RssFeedUrl",SqlDbType.NVarChar){Value=rssFeedUrl},
                new SqlParameter("@ItemNodePath",SqlDbType.NVarChar ) { Value =itemNodePath },
                new SqlParameter("@DescNodePath",SqlDbType.NVarChar ) { Value = descNode},
                new SqlParameter("@PubDateNodePath",SqlDbType.NVarChar ) { Value = pubDateNode},
                new SqlParameter("@RssFeedOrder",SqlDbType.Int ) { Value =rssOrder },
                new SqlParameter("@hrfTitleNodePath",SqlDbType.NVarChar){ Value=hrfNodePath},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };

            try
            {
                count = dhRss.ExecuteNonQuery("[dbo].[proc_AddRssFeed]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[8].Value);
                if (status)
                    result = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhRss, "SQLServerDAL,AddRssFeeds", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dhRss != null)
                {
                    if (dhRss.DataConn != null)
                    {
                        dhRss.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public RSSFeedMaster GetRssFeedById(int id)
        {
            RSSFeedMaster rSSFeed = new RSSFeedMaster();
            DataHelper dhRss = new DataHelper(strConnectionString);
            SqlParameter[] sqlParameter =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value =id }
            };
            DataSet dsRss = new DataSet();
            DataTable dtRss = new DataTable();
            try
            {
                dsRss = dhRss.ExecuteDataSet("[dbo].[proc_GetRssFeedById]", CommandType.StoredProcedure, sqlParameter);
                dtRss = dsRss.Tables[0];
                if (dtRss != null && dtRss.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtRss.Rows)
                    {
                        rSSFeed.ID = Convert.ToInt32(dr["ID"]);
                        rSSFeed.Title = dr["Title"].ToString();
                        rSSFeed.TitleNode = dr["TitleNodePath"].ToString();
                        rSSFeed.DescriptionNode = dr["DescNodePath"].ToString();
                        rSSFeed.itemNodePath = dr["ItemNodePath"].ToString();
                        rSSFeed.RSSFeedUrl = dr["RssFeedUrl"].ToString();
                        rSSFeed.PubDateNode = dr["PubDateNodePath"].ToString();
                        rSSFeed.rssFeedOrder = Convert.ToInt32(dr["RssFeedOrder"]);
                        rSSFeed.hrfTitleNodePath = dr["hrfTitleNodePath"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhRss, "SQLServerDAL,GetRssFeedById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dhRss != null)
                {
                    if (dhRss.DataConn != null)
                    {
                        dhRss.DataConn.Close();
                    }
                }
            }
            return rSSFeed;
        }

        public bool UpdateRssFeeds(RSSFeedMaster rSSFeed)
        {
            bool result = false;
            bool status = false;
            int id = rSSFeed.ID;
            string title = rSSFeed.Title;
            string titleNode = rSSFeed.TitleNode;
            string descNode = rSSFeed.DescriptionNode;
            string rSSfeedUrl = rSSFeed.RSSFeedUrl;
            string pubDateNode = rSSFeed.PubDateNode;
            string itemNodePath = rSSFeed.itemNodePath;
            string hrfNodePath = rSSFeed.hrfTitleNodePath;
            int rssOrder = rSSFeed.rssFeedOrder;
            int count;
            DataHelper dhRss = new DataHelper(strConnectionString);


            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value =id },
                new SqlParameter("@Title",SqlDbType.NVarChar ) { Value = title},
                new SqlParameter("@TitleNodePath",SqlDbType.NVarChar ) { Value = titleNode},
                new SqlParameter("@RssFeedUrl",SqlDbType.NVarChar){Value=rSSfeedUrl},
                new SqlParameter("@ItemNodePath",SqlDbType.NVarChar ) { Value =itemNodePath },
                new SqlParameter("@DescNodePath",SqlDbType.NVarChar ) { Value = descNode},
                new SqlParameter("@PubDateNodePath",SqlDbType.NVarChar ) { Value = pubDateNode},
                new SqlParameter("@RssFeedOrder",SqlDbType.Int ) { Value =rssOrder },
                new SqlParameter("@hrfTitleNodePath",SqlDbType.NVarChar){ Value=hrfNodePath},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };

            try
            {
                count = dhRss.ExecuteNonQuery("[dbo].[proc_UpdateRssFeed]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[9].Value);
                if (status)
                    result = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhRss, "SQLServerDAL,UpdateRssFeeds", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dhRss != null)
                {
                    if (dhRss.DataConn != null)
                    {
                        dhRss.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public bool DeleteRssFeed(int id)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhRss = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };
            try
            {
                count = dhRss.ExecuteNonQuery("[dbo].[proc_DeleteRssFeed]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[1].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dhRss != null)
                {
                    if (dhRss.DataConn != null)
                    {
                        dhRss.DataConn.Close();
                    }
                }
            }

            return result;
        }


        public List<EmailTemplate> GetEmailTemplates()
        {
            List<EmailTemplate> templates = new List<EmailTemplate>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllEmailTemplates]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            EmailTemplate t = new EmailTemplate();
                            t.id = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                            t.title = ds.Tables[0].Rows[i]["Title"].ToString();
                            t.emailSubject = ds.Tables[0].Rows[i]["EmailSubject"].ToString();
                            t.emailBody = ds.Tables[0].Rows[i]["EmailBody"].ToString();

                            templates.Add(t);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetEmailTemplates", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return templates;
        }

        public bool AddEmailTemplate(string title, string emailSubject, string emailBody)
        {
            bool result = false;
            bool status = false;
            DataHelper dhEmail = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@Title",SqlDbType.NVarChar){Value=title},
                new SqlParameter("@EmailSubject",SqlDbType.NVarChar){Value=emailSubject},
                new SqlParameter("@EmailBody",SqlDbType.NVarChar){Value=emailBody},
                new SqlParameter("@Status",SqlDbType.NVarChar){Value=status}
            };
            try
            {
                int count = dhEmail.ExecuteNonQuery("[dbo].[proc_AddEmailTemplate]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[3].Value);
                if (status)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhEmail, "SQLServerDAL,AddEmailTemplate", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhEmail != null)
                {
                    if (dhEmail.DataConn != null)
                    {
                        dhEmail.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public EmailTemplate GetEmailTemplateById(int id)
        {
            EmailTemplate template = new EmailTemplate();
            DataHelper dhEmail = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.NVarChar){Value=id}
            };
            DataSet dsEmail = new DataSet();
            DataTable dtEmail = new DataTable();
            dsEmail = dhEmail.ExecuteDataSet("[dbo].[proc_GetEmailTemplateById]", CommandType.StoredProcedure, parameters);
            dtEmail = dsEmail.Tables[0];
            foreach (DataRow dr in dtEmail.Rows)
            {
                template.id = Convert.ToInt32(dr["ID"]);
                template.title = dr["Title"].ToString();
                template.emailSubject = dr["EmailSubject"].ToString();
                template.emailBody = dr["EmailBody"].ToString();
            }
            return template;
        }

        public bool UpdateEmailTemplate(int id, string title, string emailSubject, string emailBody)
        {
            bool result = false;
            bool status = false;
            DataHelper dhEmail = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.NVarChar){Value=id},
                new SqlParameter("@Title",SqlDbType.NVarChar){Value=title},
                new SqlParameter("@EmailSubject",SqlDbType.NVarChar){Value=emailSubject},
                new SqlParameter("@EmailBody",SqlDbType.NVarChar){Value=emailBody},
                new SqlParameter("@Status",SqlDbType.NVarChar){Value=status}
            };
            try
            {
                int count = dhEmail.ExecuteNonQuery("[dbo].[proc_UpdateEmailTemplate]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[4].Value);
                if (status)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhEmail, "SQLServerDAL,UpdateEmailTemplate", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhEmail != null)
                {
                    if (dhEmail.DataConn != null)
                    {
                        dhEmail.DataConn.Close();
                    }
                }
            }
            return result;
        }

        public bool DeleteEmailTemplate(int id)
        {
            bool result = false;
            bool status = false;
            DataHelper dhEmail = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.NVarChar){Value=id},
                new SqlParameter("@Status",SqlDbType.NVarChar){Value=status}
            };
            try
            {
                int count = dhEmail.ExecuteNonQuery("[dbo].[proc_DeleteEmailTemplate]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[1].Value);
                if (status)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dhEmail, "SQLServerDAL,DeleteEmailTemplate", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhEmail != null)
                {
                    if (dhEmail.DataConn != null)
                    {
                        dhEmail.DataConn.Close();
                    }
                }
            }
            return result;
        }
        public void StoreKalibreUserName(string emailid, string kalibreUsrName)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@EmailId",SqlDbType.NVarChar,100),
                                new SqlParameter("@KalibreUserName",SqlDbType.NVarChar,255),
                                new SqlParameter("@ErrorMessage",SqlDbType.NVarChar,4000)
                };
                parameters[0].Value = emailid;
                parameters[1].Value = kalibreUsrName;
                parameters[0].Direction = ParameterDirection.Input;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_StoreKalibreUsrName]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog("StoreKalibreUserName", dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.Service", currentUser.emailId.ToString());

                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        public List<AcademyConfig> GetAllAcademyConfig()
        {
            List<AcademyConfig> academyconfigs = new List<AcademyConfig>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet dsacademyconfigs = new DataSet();
                DataView dvacademyconfigs = new DataView();

                dsacademyconfigs = dh.ExecuteDataSet("[dbo].[proc_GetAllAcademyConfig]", CommandType.StoredProcedure);
                if (dsacademyconfigs.Tables.Count > 0)
                    dvacademyconfigs = new DataView(dsacademyconfigs.Tables[0]);
                DataTable dtacademyconfig = dvacademyconfigs.ToTable();
                if (dtacademyconfig != null & dtacademyconfig.Rows.Count > 0)
                {
                    AcademyConfig academyconfig = null;
                    foreach (DataRow item in dtacademyconfig.Rows)
                    {
                        academyconfig = new AcademyConfig();
                        academyconfig.ID = Convert.ToInt32(item["ID"].ToString());
                        academyconfig.Title = Convert.ToString(item["Title"]);
                        academyconfig.Value = Convert.ToString(item["Value"]);
                        academyconfigs.Add(academyconfig);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAllAcademyConfig", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return academyconfigs;
        }

        public AcademyConfig GetAcademyConfigById(int Id)
        {
            AcademyConfig academyconfig = null;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet dsacademyconfig = new DataSet();
                DataView dvacademyconfig = new DataView();

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int)
                };
                parameters[0].Value = Id;
                dsacademyconfig = dh.ExecuteDataSet("[dbo].[proc_GetAcademyConfigById]", CommandType.StoredProcedure, parameters);
                if (dsacademyconfig.Tables.Count > 0)
                    dvacademyconfig = new DataView(dsacademyconfig.Tables[0]);
                DataTable dtacademyconfig = dvacademyconfig.ToTable();
                if (dtacademyconfig != null & dtacademyconfig.Rows.Count == 1)
                {
                    academyconfig = new AcademyConfig();
                    academyconfig.ID = Convert.ToInt32(dtacademyconfig.Rows[0]["ID"].ToString());
                    academyconfig.Title = Convert.ToString(dtacademyconfig.Rows[0]["Title"]);
                    academyconfig.Value = Convert.ToString(dtacademyconfig.Rows[0]["Value"]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAcademyConfigById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return academyconfig;
        }

        public AcademyConfig EditAcademyConfigByID(int Id)
        {
            AcademyConfig academyConfig = new AcademyConfig();
            academyConfig.ID = Id;
            DataHelper dh = new DataHelper(strConnectionString);
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",SqlDbType.Int)
                };
                parameters[0].Value = Id;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAcademyConfigById]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    academyConfig.Title = dt.Rows[0]["Title"].ToString();
                    academyConfig.Value = dt.Rows[0]["Value"].ToString();
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return academyConfig;
        }

        public void UpdateAcademyConfig(AcademyConfig academyconfig)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@Title",SqlDbType.VarChar ),
                    new SqlParameter("@Value",SqlDbType.VarChar ),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = academyconfig.ID;
                parameters[1].Value = academyconfig.Title;
                parameters[2].Value = academyconfig.Value;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[4].Size = 4000;
                parameters[4].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateAcademyConfig]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog("UpdateAcademyConfig", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.Service", currentUser.emailId.ToString());

                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

        }

        public void AddAcademyConfig(AcademyConfig academyconfig)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@Title",SqlDbType.VarChar ),
                    new SqlParameter("@Value",SqlDbType.VarChar ),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = academyconfig.Title;
                parameters[1].Value = academyconfig.Value;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Size = 4000;
                parameters[3].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_AddAcademyConfig]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog(dh, "SQLServerDAL,AddAcademyConfig", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        public void DeleteAcademyConfig(int id)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = id;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_DeleteAcademyConfig]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog(dh, "SQLServerDAL,DeleteAcademyConfig", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        public bool AddChecklist(string checklist, string geo, string internalName, string description, bool choice, string role)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            int geoID = Convert.ToInt32(geo);
            int roleID = Convert.ToInt32(role);
            DataHelper dhChecklist = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@Title",SqlDbType.VarChar ) { Value = checklist},
                new SqlParameter("@GeoID",SqlDbType.Int ) { Value = geoID},
                new SqlParameter("@InternalName",SqlDbType.VarChar ) { Value = internalName},
                new SqlParameter("@Description",SqlDbType.VarChar ) { Value = description},
                new SqlParameter("@Choice",SqlDbType.Bit ) { Value = choice},
                new SqlParameter("@RoleID",SqlDbType.Int ) { Value = roleID},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }

            };
            try
            {
                count = dhChecklist.ExecuteNonQuery("[dbo].[proc_AddChecklist]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[6].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dhChecklist != null)
                {
                    if (dhChecklist.DataConn != null)
                    {
                        dhChecklist.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public bool UpdateChecklist(int id, string checklist, string geo, string internalName, string description, bool choice, string role)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhChecklist = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                new SqlParameter("@Title",SqlDbType.VarChar ) { Value = checklist},
                new SqlParameter("@GeoID",SqlDbType.Int ) { Value = geo},
                new SqlParameter("@InternalName",SqlDbType.VarChar ) { Value = internalName},
                new SqlParameter("@Description",SqlDbType.VarChar ) { Value = description},
                new SqlParameter("@Choice",SqlDbType.Bit ) { Value = choice},
                new SqlParameter("@RoleID",SqlDbType.Int ) { Value = role},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };
            try
            {
                count = dhChecklist.ExecuteNonQuery("[dbo].[proc_UpdateChecklist]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[7].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dhChecklist != null)
                {
                    if (dhChecklist.DataConn != null)
                    {
                        dhChecklist.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public bool DeleteChecklist(int id)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhChecklist = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.Int ) { Value = id},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };
            try
            {
                count = dhChecklist.ExecuteNonQuery("[dbo].[proc_DeleteChecklist]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[1].Value);
                if (status)
                    result = true;
            }
            finally
            {
                if (dhChecklist != null)
                {
                    if (dhChecklist.DataConn != null)
                    {
                        dhChecklist.DataConn.Close();
                    }
                }
            }

            return result;
        }
        public List<OnBoarding> GetBoardingDataFromOnboarding(ref bool sendEmail)
        {
            List<OnBoarding> boardingList = new List<OnBoarding>();
            string userName = String.Empty;
            var showDefaultTraining = "NO";
            var showSkillBasedTraining = "NO";
            var showRoleBasedTraining = "NO";
            var showAssessments = "NO";
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet dsConfig = new DataSet();
                DataView dvConfig = new DataView();

                dsConfig = dh.ExecuteDataSet("[dbo].[proc_GetConfigItems]", CommandType.StoredProcedure);
                if (dsConfig.Tables.Count > 0)
                    dvConfig = new DataView(dsConfig.Tables[0]);

                DataTable dt = dvConfig.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string Title = String.Empty;
                        string Value = String.Empty;
                        Title = row["Title"].ToString();
                        Value = row["Value"].ToString();
                        if (Title == "ShowUserAssessments")
                        {
                            showAssessments = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowDefaultTraining")
                        {
                            showDefaultTraining = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowSkillBasedTraining")
                        {
                            showSkillBasedTraining = Value.ToUpper().ToString();
                        }
                        else if (Title == "ShowRoleBasedTraining")
                        {
                            showRoleBasedTraining = Value.ToUpper().ToString();
                        }
                    }
                }

                DataSet dsOnBoarding = new DataSet();
                DataView dvOnBoarding = new DataView();

                dsOnBoarding = dh.ExecuteDataSet("dbo.proc_GetOnBoardingDataForUser", CommandType.StoredProcedure, new SqlParameter("@userId", currentUser.id));
                if (dsOnBoarding.Tables.Count > 0)
                {
                    dvOnBoarding = new DataView(dsOnBoarding.Tables[0]);
                }

                DataTable dtOnBoarding = new DataTable();
                dtOnBoarding = dvOnBoarding.ToTable();
                foreach (DataRow row in dtOnBoarding.Rows)
                {
                    if (row["SendEmail"] != null)
                    {
                        if (!String.IsNullOrEmpty(row["SendEmail"].ToString()))
                            sendEmail = Convert.ToBoolean(row["SendEmail"]) == true ? true : false;
                    }


                    int locId = 0;
                    string locValue = "";
                    if (row["GEOID"] != null)
                    {
                        if (!String.IsNullOrEmpty(row["GEOID"].ToString()))
                            locId = Convert.ToInt32(row["GEOID"]);
                    }
                    if (row["GEOID"] != null)
                    {
                        if (!String.IsNullOrEmpty(row["GEO"].ToString()))
                            locValue = row["GEO"].ToString();
                    }
                    var checkList = GetOnBoardingCheckList(locValue, locId);
                    DataSet dsUser = new DataSet();
                    DataView dvUser = new DataView();
                    dsUser = dh.ExecuteDataSet("[dbo].[proc_GetUserCheckLists]", CommandType.StoredProcedure, new SqlParameter("@userId", currentUser.id));
                    dvUser = new DataView(dsUser.Tables[0]);
                    DataTable dtUsers = new DataTable();
                    dtUsers = dvUser.ToTable();
                    List<UserCheckList> lstUserCheckList = new List<UserCheckList>();
                    if (dtUsers != null && dtUsers.Rows.Count > 0)
                    {
                        foreach (DataRow data in dtUsers.Rows)
                        {
                            UserCheckList item = new UserCheckList();
                            item.Id = Convert.ToInt32(data["ID"].ToString());
                            if (data["CheckList"] != null)
                            {
                                if (!String.IsNullOrEmpty(data["CheckList"].ToString()))
                                    item.CheckList = data["CheckList"].ToString();
                            }
                            if (data["CheckListStatus"] != null)
                            {
                                if (!String.IsNullOrEmpty(data["CheckListStatus"].ToString()))
                                    item.CheckListStatus = data["CheckListStatus"].ToString();
                            }
                            lstUserCheckList.Add(item);
                        }
                    }
                    ///////Add items for default and skill based training assignments////////
                    if (showDefaultTraining == "YES" || showSkillBasedTraining == "YES")
                    {
                        List<UserTrainingDetail> userTrainings = GetTrainingDetails();
                        ArrayList defaultTrainingIds = new ArrayList();
                        if (showDefaultTraining == "YES")
                        {
                            DataSet dsDefaultTraining = GetDefaultTraining();
                            if (dsDefaultTraining.Tables.Count > 0)
                            {
                                if (dsDefaultTraining.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsDefaultTraining.Tables[0].Rows.Count; i++)
                                    {
                                        if (dsDefaultTraining.Tables[0].Rows[i]["TrainingId"] != null)
                                        {
                                            defaultTrainingIds.Add(dsDefaultTraining.Tables[0].Rows[i]["TrainingId"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < userTrainings.Count; i++)
                        {
                            if (showDefaultTraining == "YES" && defaultTrainingIds.Contains(userTrainings[i].TrainingId))
                            {
                                boardingList.Add(new OnBoarding
                                {
                                    boardingItemId = Convert.ToInt32(userTrainings[i].TrainingId),
                                    boardingItemName = Convert.ToString(userTrainings[i].TrainingName),
                                    boardingItemDesc = Convert.ToString(userTrainings[i].ModuleDesc),
                                    boardingItemLink = userTrainings[i].LinkUrl,
                                    boardingStatus = Utilities.GetOnBoardingStatus(userTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(userTrainings[i].LastDayToComplete)),
                                    boardingType = OnboardingItemType.Training,
                                    boardIngTrainingId = userTrainings[i].TrainingId,
                                    boardingIsMandatory = Convert.ToBoolean(userTrainings[i].Mandatory)
                                });
                            }
                            else if (showSkillBasedTraining == "YES" && !defaultTrainingIds.Contains(userTrainings[i].TrainingId))
                            {
                                boardingList.Add(new OnBoarding
                                {
                                    boardingItemId = Convert.ToInt32(userTrainings[i].TrainingId),
                                    boardingItemName = Convert.ToString(userTrainings[i].TrainingName),
                                    boardingItemDesc = Convert.ToString(userTrainings[i].ModuleDesc),
                                    boardingItemLink = userTrainings[i].LinkUrl,
                                    boardingStatus = Utilities.GetOnBoardingStatus(userTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(userTrainings[i].LastDayToComplete)),
                                    boardingType = OnboardingItemType.Training,
                                    boardIngTrainingId = userTrainings[i].TrainingId,
                                    boardingIsMandatory = Convert.ToBoolean(userTrainings[i].Mandatory)
                                });

                            }

                        }
                    }
                    //////////Add Checklist items/////////////////
                    for (int i = 0; i < lstUserCheckList.Count; i++)
                    {
                        List<CheckListItem> itemCheckList = checkList.Where(c => c.id == Convert.ToInt32(lstUserCheckList[i].CheckList)).ToList();
                        if (itemCheckList.Count > 0)
                        {
                            boardingList.Add(new OnBoarding
                            {
                                boardingItemId = lstUserCheckList[i].Id,
                                boardingItemName = itemCheckList[0].name,
                                boardingItemDesc = itemCheckList[0].desc,
                                boardingStatus = GetStatus(lstUserCheckList[i].CheckListStatus),
                                boardingInternalName = itemCheckList[0].internalName
                            });
                        }
                    }

                    ///////Add Assessment Items////////
                    if (showAssessments == "YES")
                    {
                        GetUserAssessments(boardingList);
                    }

                    /////////////Add Role based training items///////////
                    if (showRoleBasedTraining == "YES")
                    {
                        List<UserTrainingDetail> roleTrainings = new List<UserTrainingDetail>();
                        GetUserRoleBasedTraining(ref roleTrainings, currentUser.id);
                        for (int i = 0; i < roleTrainings.Count; i++)
                        {
                            OnBoarding boardingItem = new OnBoarding();
                            boardingItem.boardingItemId = roleTrainings[i].Id;
                            boardingItem.boardingItemName = roleTrainings[i].TrainingName;
                            boardingItem.boardingItemDesc = roleTrainings[i].ModuleDesc;
                            boardingItem.boardingInternalName = roleTrainings[i].TrainingName;
                            boardingItem.boardingIsMandatory = roleTrainings[i].Mandatory;
                            if (roleTrainings[i].IsLink)
                                boardingItem.boardingItemLink = roleTrainings[i].LinkUrl;
                            else
                                boardingItem.boardingItemLink = roleTrainings[i].DocumentUrl;
                            boardingItem.boardingType = OnboardingItemType.RoleTraining;
                            boardingItem.boardingStatus = Utilities.GetOnBoardingStatus(roleTrainings[i].IsTrainingCompleted, 0, Convert.ToDateTime(roleTrainings[i].CompletionDate));
                            boardingList.Add(boardingItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetBoardingDataFromOnboarding", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return boardingList;
        }
        public void OffBoardUser(string EmailAddress)
        {
            DataHelper dhChecklist = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@useremail",SqlDbType.NVarChar,100 ) { Value = EmailAddress}
            };
            try
            {
                dhChecklist.ExecuteNonQuery("[dbo].[proc_OffBoardUser]", CommandType.StoredProcedure, parameters);
            }
            finally
            {
                if (dhChecklist != null)
                {
                    if (dhChecklist.DataConn != null)
                    {
                        dhChecklist.DataConn.Close();
                    }
                }
            }

        }
        public ProjectResources GetExpectedProjectResourceCountByProjectId(int ProjectId)
        {
            ProjectResources prjres = new ProjectResources();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
               {
                    new SqlParameter("@ProjectID",SqlDbType.Int),
                    new SqlParameter("@ProjectName",SqlDbType.VarChar)
                };
                parameters[0].Value = ProjectId;
                parameters[1].Size = 255;
                parameters[1].Direction = ParameterDirection.Output;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetExpectedSkillResourceCountByProjectId]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

            prjres.projectName = Convert.ToString(dh.Cmd.Parameters["@ProjectName"].Value);
            List<SkillResource> lstSkillResource = new List<SkillResource>();
            Hashtable objHashTable = new Hashtable();
            DataTable dt = dv.ToTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!objHashTable.ContainsKey(dr["SkillId"]))
                    {
                        objHashTable.Add(dr["SkillId"], dr["SkillId"]);
                        SkillResource objSkillResource = new SkillResource();
                        objSkillResource.skillId = Convert.ToInt32(dr["SkillId"]);
                        objSkillResource.skill = dr["SkillName"].ToString();
                        lstSkillResource.Add(objSkillResource);
                    }
                }
            }
            prjres.skillResources = lstSkillResource;

            foreach (SkillResource skr in prjres.skillResources)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (skr.skillId == Convert.ToInt32(dr["SkillId"]))
                    {
                        switch (dr["CompetencyLevel"].ToString().ToUpper())
                        {
                            case "NOVICE":
                                skr.beginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;
                            case "ADVANCED BEGINNER":
                                skr.advancedBeginnerCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "COMPETENT":
                                skr.competentCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "PROFICIENT":
                                skr.proficientCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;

                            case "EXPERT":
                                skr.expertCount = Convert.ToInt32(dr["ExpectedResourceCount"].ToString() == String.Empty ? "0" : dr["ExpectedResourceCount"].ToString());
                                break;
                            default:
                                break;
                        }
                    }

                }
            }

            return prjres;

        }
        public List<UserTraining> GetUserTrainingReport(int projectid, int roleid)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@projectid",SqlDbType.Int),new SqlParameter("@roleid",SqlDbType.Int)

                };
                parameters[0].Value = projectid;
                parameters[1].Value = roleid;

                ds = dh.ExecuteDataSet("[dbo].[proc_GenerateUserTrainingReport]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        UserTraining userTraining = new UserTraining();
                        userTraining.Employee = ds.Tables[0].Rows[i]["Name"].ToString();
                        userTraining.EmployeeId = ds.Tables[0].Rows[i]["EmployeeId"].ToString();
                        userTraining.EmailId = ds.Tables[0].Rows[i]["EmailAddress"].ToString();
                        userTraining.Role = ds.Tables[0].Rows[i]["Role"].ToString();
                        userTraining.ProjectName = ds.Tables[0].Rows[i]["Project"].ToString();
                        userTraining.TrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingId"].ToString());
                        userTraining.SkillName = ds.Tables[0].Rows[i]["Skill"].ToString();
                        userTraining.TrainingName = ds.Tables[0].Rows[i]["TrainingName"].ToString();
                        userTraining.IsTrainingCompleted = false;
                        if (ds.Tables[0].Rows[i]["IsTrainingCompleted"] != null && !(ds.Tables[0].Rows[i]["IsTrainingCompleted"] is DBNull))
                            userTraining.IsTrainingCompleted = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsTrainingCompleted"].ToString());

                        if (ds.Tables[0].Rows[i]["AdminApprovalStatus"] != null && !(ds.Tables[0].Rows[i]["AdminApprovalStatus"] is DBNull))
                            userTraining.AdminApprovalStatus = Convert.ToString(ds.Tables[0].Rows[i]["AdminApprovalStatus"].ToString());
                        else
                            userTraining.AdminApprovalStatus = "Not Started";
                        if (ds.Tables[0].Rows[i]["LastDayCompletion"] != null && !(ds.Tables[0].Rows[i]["LastDayCompletion"] is DBNull))
                            userTraining.LastDayCompletion = ds.Tables[0].Rows[i]["LastDayCompletion"].ToString();
                        if (ds.Tables[0].Rows[i]["CompletedDate"] != null && !(ds.Tables[0].Rows[i]["CompletedDate"] is DBNull))
                            userTraining.CompletedDate = ds.Tables[0].Rows[i]["CompletedDate"].ToString();
                        userTrainings.Add(userTraining);
                    }
                }
                DateTime theDate = DateTime.Now;
                int todayDay = theDate.Day;
                int todayMonth = theDate.Month;
                int todayYear = theDate.Year;
                string todaysdate = theDate.Day.ToString() + "/" + theDate.Month.ToString() + "/" + theDate.Year.ToString();
                string[] formats = { "MM/dd/yyyy", "dd/MM/yyyy", "M-d-yyyy", "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy", };
                string completedDate = String.Empty;
                string lastDates = String.Empty;
                DateTime today = DateTime.ParseExact(todaysdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                foreach (var train in userTrainings)
                {
                    DateTime compDate = new DateTime();
                    DateTime completeDay = new DateTime();
                    DateTime lastDay = new DateTime();

                    if (DateTime.TryParseExact(train.CompletedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out compDate))
                    {
                        completedDate = compDate.ToString("dd/MM/yyyy");
                        completeDay = DateTime.ParseExact(completedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    string[] lastdate = train.LastDayCompletion.Split('/');
                    var lstday = lastdate[0];
                    var lstmonth = lastdate[1];
                    var lstyear = lastdate[2];
                    string l = lstday + "/" + lstmonth + "/" + lstyear;
                    lastDay = DateTime.ParseExact(l, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if ((completedDate == null) || (completedDate.Equals(String.Empty)))
                    {
                        if (today > lastDay)
                        {
                            train.AdminApprovalStatus = "Overdue";
                        }
                    }
                    else
                    {

                        if (compDate > lastDay)
                        {
                            train.AdminApprovalStatus = "Overdue";
                        }
                    }

                    if (train.AdminApprovalStatus.Equals("Not Started"))
                    {
                        if (today > lastDay)
                        {
                            train.AdminApprovalStatus = "Overdue";
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserTrainingReport", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userTrainings;
        }

        #region AssessmentMaster
        public AssessmentMaster GetAssessmentById(int id)
        {
            AssessmentMaster am = new AssessmentMaster();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ID",SqlDbType.Int)
                    };
                parameters[0].Value = id;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessmentById]", CommandType.StoredProcedure, parameters[0]);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            am.AssessmentId = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                            if (ds.Tables[0].Rows[i]["Title"] != null && !(ds.Tables[0].Rows[i]["Title"] is DBNull))
                                am.AssessmentName = ds.Tables[0].Rows[i]["Title"].ToString();
                            if (ds.Tables[0].Rows[i]["AssessmentLink"] != null && !(ds.Tables[0].Rows[i]["AssessmentLink"] is DBNull))
                                am.AssessmentLink = ds.Tables[0].Rows[i]["AssessmentLink"].ToString();
                            if (ds.Tables[0].Rows[i]["IsMandatory"] != null && !(ds.Tables[0].Rows[i]["IsMandatory"] is DBNull))
                                am.IsMandatory = (bool)ds.Tables[0].Rows[i]["IsMandatory"];
                            if (ds.Tables[0].Rows[i]["Description"] != null && !(ds.Tables[0].Rows[i]["Description"] is DBNull))
                                am.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                            if (ds.Tables[0].Rows[i]["PassingMarks"] != null && !(ds.Tables[0].Rows[i]["PassingMarks"] is DBNull))
                                am.PassingMarks = Convert.ToInt32(ds.Tables[0].Rows[i]["PassingMarks"]);
                            if (ds.Tables[0].Rows[i]["AssessmentTimeInMins"] != null && !(ds.Tables[0].Rows[i]["AssessmentTimeInMins"] is DBNull))
                                am.AssessmentTimeInMins = Convert.ToInt32(ds.Tables[0].Rows[i]["AssessmentTimeInMins"]);

                            if (ds.Tables[0].Rows[i]["Training"] != null && !(ds.Tables[0].Rows[i]["Training"] is DBNull))
                                am.SelectedTraining = ds.Tables[0].Rows[i]["Training"].ToString();

                            //if (ds.Tables[0].Rows[i]["Training"] != null && !(ds.Tables[0].Rows[i]["Training"] is DBNull))
                            //    am.SelectedTraining = ds.Tables[0].Rows[i]["Training"].ToString();

                            if (ds.Tables[0].Rows[i]["Skill"] != null && !(ds.Tables[0].Rows[i]["Skill"] is DBNull))
                                am.SelectedSkill = ds.Tables[0].Rows[i]["Skill"].ToString();
                            //if (ds.Tables[0].Rows[i]["Skill"] != null && !(ds.Tables[0].Rows[i]["Skill"] is DBNull))
                            //    am.SelectedSkill = ds.Tables[0].Rows[i]["Skill"].ToString();

                            if (ds.Tables[0].Rows[i]["CompetencyLevel"] != null && !(ds.Tables[0].Rows[i]["CompetencyLevel"] is DBNull))
                                am.SelectedCompetency = ds.Tables[0].Rows[i]["CompetencyLevel"].ToString();

                            if (ds.Tables[0].Rows[i]["Points"] != null && !(ds.Tables[0].Rows[i]["Points"] is DBNull))
                                am.Points = Convert.ToInt32(ds.Tables[0].Rows[i]["Points"]);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAssessmentById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return am;
        }

        public List<AssessmentMaster> GetAllAssessmentForMaster()
        {
            List<AssessmentMaster> assesments = new List<AssessmentMaster>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllAssessmentForMaster]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AssessmentMaster am = new AssessmentMaster();

                            am.AssessmentId = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                            if (ds.Tables[0].Rows[i]["Title"] != null && !(ds.Tables[0].Rows[i]["Title"] is DBNull))
                                am.AssessmentName = ds.Tables[0].Rows[i]["Title"].ToString();
                            if (ds.Tables[0].Rows[i]["AssessmentLink"] != null && !(ds.Tables[0].Rows[i]["AssessmentLink"] is DBNull))
                                am.AssessmentLink = ds.Tables[0].Rows[i]["AssessmentLink"].ToString();
                            if (ds.Tables[0].Rows[i]["IsMandatory"] != null && !(ds.Tables[0].Rows[i]["IsMandatory"] is DBNull))
                                am.IsMandatory = (bool)ds.Tables[0].Rows[i]["IsMandatory"];
                            if (ds.Tables[0].Rows[i]["Description"] != null && !(ds.Tables[0].Rows[i]["Description"] is DBNull))
                                am.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                            if (ds.Tables[0].Rows[i]["PassingMarks"] != null && !(ds.Tables[0].Rows[i]["PassingMarks"] is DBNull))
                                am.PassingMarks = Convert.ToInt32(ds.Tables[0].Rows[i]["PassingMarks"]);
                            if (ds.Tables[0].Rows[i]["AssessmentTimeInMins"] != null && !(ds.Tables[0].Rows[i]["AssessmentTimeInMins"] is DBNull))
                                am.AssessmentTimeInMins = Convert.ToInt32(ds.Tables[0].Rows[i]["AssessmentTimeInMins"]);

                            if (ds.Tables[0].Rows[i]["TrainingId"] != null && !(ds.Tables[0].Rows[i]["TrainingId"] is DBNull))
                                am.SelTrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingId"]);

                            if (ds.Tables[0].Rows[i]["Training"] != null && !(ds.Tables[0].Rows[i]["Training"] is DBNull))
                                am.SelectedTraining = ds.Tables[0].Rows[i]["Training"].ToString();

                            if (ds.Tables[0].Rows[i]["SkillId"] != null && !(ds.Tables[0].Rows[i]["SkillId"] is DBNull))
                                am.SelSkillId = Convert.ToInt32(ds.Tables[0].Rows[i]["SkillId"]);

                            if (ds.Tables[0].Rows[i]["Skill"] != null && !(ds.Tables[0].Rows[i]["Skill"] is DBNull))
                                am.SelectedSkill = ds.Tables[0].Rows[i]["Skill"].ToString();

                            if (ds.Tables[0].Rows[i]["CompetencyLevelId"] != null && !(ds.Tables[0].Rows[i]["CompetencyLevelId"] is DBNull))
                                am.SelCompetencyId = Convert.ToInt32(ds.Tables[0].Rows[i]["CompetencyLevelId"]);

                            if (ds.Tables[0].Rows[i]["CompetencyLevel"] != null && !(ds.Tables[0].Rows[i]["CompetencyLevel"] is DBNull))
                                am.SelectedCompetency = ds.Tables[0].Rows[i]["CompetencyLevel"].ToString();

                            if (ds.Tables[0].Rows[i]["Points"] != null && !(ds.Tables[0].Rows[i]["Points"] is DBNull))
                                am.Points = Convert.ToInt32(ds.Tables[0].Rows[i]["Points"]);

                            assesments.Add(am);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAllAssessmentForMaster", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return assesments;
        }

        public bool UpdateAssessmentMaster(AssessmentMaster AM)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            bool status = true;
            try
            {
                SqlParameter[] parameters =
                   {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@Title",SqlDbType.VarChar ),
                    new SqlParameter("@AssessmentLink", SqlDbType.VarChar ),
                    new SqlParameter("@IsMandatory", SqlDbType.Bit),
                    new SqlParameter("@Description", SqlDbType.VarChar),
                    new SqlParameter("@PassingMarks", SqlDbType.Int),
                    new SqlParameter("@AssessmentTimeInMins", SqlDbType.Int),
                    new SqlParameter("@TrainingId", SqlDbType.Int),
                    new SqlParameter("@SkillId", SqlDbType.Int),
                    new SqlParameter("@CompetencyLevelId", SqlDbType.Int),
                    new SqlParameter("@Points", SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = AM.AssessmentId;
                parameters[1].Value = AM.AssessmentName;
                parameters[2].Value = AM.AssessmentLink;
                parameters[3].Value = AM.IsMandatory == true ? 1 : 0;
                parameters[4].Value = AM.Description;
                parameters[5].Value = AM.PassingMarks;
                parameters[6].Value = AM.AssessmentTimeInMins;
                parameters[7].Value = AM.SelTrainingId;
                parameters[8].Value = AM.SelSkillId;
                parameters[9].Value = AM.SelCompetencyId;
                parameters[10].Value = AM.Points;
                parameters[11].Direction = ParameterDirection.Output;
                parameters[12].Direction = ParameterDirection.Output;
                parameters[12].Size = 4000;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateAssessment]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog(dh, "SQLServerDAL,UpdateAssessmentMaster", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                    status = false;
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dh, "SQLServerDAL,GetAllAssessmentForMaster", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                status = false;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }

        public void AddAssessmentMaster(AssessmentMaster AM)
        {
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                SqlParameter[] parameters =
                   {
                    new SqlParameter("@Title",SqlDbType.VarChar ),
                    new SqlParameter("@AssessmentLink", SqlDbType.VarChar ),
                    new SqlParameter("@IsMandatory", SqlDbType.Bit),
                    new SqlParameter("@Description", SqlDbType.VarChar),
                    new SqlParameter("@PassingMarks", SqlDbType.Int),
                    new SqlParameter("@AssessmentTimeInMins", SqlDbType.Int),
                    new SqlParameter("@TrainingId", SqlDbType.Int),
                    new SqlParameter("@SkillId", SqlDbType.Int),
                    new SqlParameter("@CompetencyLevelId", SqlDbType.Int),
                    new SqlParameter("@Points", SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };

                parameters[0].Value = AM.AssessmentName;
                parameters[1].Value = AM.AssessmentLink;
                parameters[2].Value = AM.IsMandatory == true ? 1 : 0;
                parameters[3].Value = AM.Description;
                parameters[4].Value = AM.PassingMarks;
                parameters[5].Value = AM.AssessmentTimeInMins;
                parameters[6].Value = AM.SelTrainingId;
                parameters[7].Value = AM.SelSkillId;
                parameters[8].Value = AM.SelCompetencyId;
                parameters[9].Value = AM.Points;
                parameters[10].Direction = ParameterDirection.Output;
                parameters[11].Direction = ParameterDirection.Output;
                parameters[11].Size = 4000;
                dh.ExecuteNonQuery("[dbo].[proc_AddAssessmentMaster]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {

                    LogHelper.AddLog(dh, "SQLServerDAL,AddAssessmentMaster", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dh, "SQLServerDAL,AddAssessmentMaster", ex.Message, ex.StackTrace, "HCL.Academy.DAL", "");
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

        }

        public List<Training> GetAllTrainings()
        {
            List<Training> trainings = new List<Training>();
            DataSet dsTraining = new DataSet();
            DataView dvTraining = new DataView();
            DataTable dtTraining = new DataTable();
            DataHelper dhTraining = new DataHelper(strConnectionString);
            try
            {
                dsTraining = dhTraining.ExecuteDataSet("[dbo].[proc_GetAllSkillCompetencyTrainings]", CommandType.StoredProcedure);
                dtTraining = dsTraining.Tables[0];
                if (dtTraining.Rows.Count > 0)
                {
                    foreach (DataRow row in dtTraining.Rows)
                    {
                        Training train = new Training();
                        train.TrainingName = row["TrainingName"].ToString();
                        train.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        trainings.Add(train);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dhTraining, "SQLServerDAL,GetAllTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", "");
            }
            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public void DeleteAssessment(int id)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = id;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_DeleteAssessment]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {

                    LogHelper.AddLog(dh, "SQLServerDAL,DeleteAssessment", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }
        #endregion

        #region AssessmentQuestion

        public AssessmentQuestion GetAssessmentQuestionById(int id)
        {
            AssessmentQuestion aq = new AssessmentQuestion();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                SqlParameter[] parameters =
                    {
                        new SqlParameter("@ID",SqlDbType.Int)
                    };
                parameters[0].Value = id;
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessmentQuestionById]", CommandType.StoredProcedure, parameters[0]);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //AQ.ID
                            //,AQ.Question
                            //,AQ.CorrectOption
                            //,AQ.CorrectOptionSequence
                            //,AQ.Marks
                            //,AQ.Option1
                            //,AQ.Option2
                            //,AQ.Option3
                            //,AQ.Option4
                            //,AQ.Option5
                            //,AQ.AssessmentId
                            //,A.Title As AssessmentName

                            aq.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                            if (ds.Tables[0].Rows[i]["Question"] != null && !(ds.Tables[0].Rows[i]["Question"] is DBNull))
                                aq.Question = ds.Tables[0].Rows[i]["Question"].ToString();

                            if (ds.Tables[0].Rows[i]["CorrectOption"] != null && !(ds.Tables[0].Rows[i]["CorrectOption"] is DBNull))
                                aq.CorrectOption = ds.Tables[0].Rows[i]["CorrectOption"].ToString();

                            if (ds.Tables[0].Rows[i]["CorrectOptionSequence"] != null && !(ds.Tables[0].Rows[i]["CorrectOptionSequence"] is DBNull))
                                aq.CorrectOptionSequence = Convert.ToInt32(ds.Tables[0].Rows[i]["CorrectOptionSequence"]);

                            if (ds.Tables[0].Rows[i]["Marks"] != null && !(ds.Tables[0].Rows[i]["Marks"] is DBNull))
                                aq.Marks = Convert.ToInt32(ds.Tables[0].Rows[i]["Marks"]);

                            if (ds.Tables[0].Rows[i]["Option1"] != null && !(ds.Tables[0].Rows[i]["Option1"] is DBNull))
                                aq.Option1 = ds.Tables[0].Rows[i]["Option1"].ToString();


                            if (ds.Tables[0].Rows[i]["Option2"] != null && !(ds.Tables[0].Rows[i]["Option2"] is DBNull))
                                aq.Option2 = ds.Tables[0].Rows[i]["Option2"].ToString();

                            if (ds.Tables[0].Rows[i]["Option3"] != null && !(ds.Tables[0].Rows[i]["Option3"] is DBNull))
                                aq.Option3 = ds.Tables[0].Rows[i]["Option3"].ToString();

                            if (ds.Tables[0].Rows[i]["Option4"] != null && !(ds.Tables[0].Rows[i]["Option4"] is DBNull))
                                aq.Option4 = ds.Tables[0].Rows[i]["Option4"].ToString();

                            if (ds.Tables[0].Rows[i]["Option5"] != null && !(ds.Tables[0].Rows[i]["Option5"] is DBNull))
                                aq.Option5 = ds.Tables[0].Rows[i]["Option5"].ToString();

                            if (ds.Tables[0].Rows[i]["AssessmentId"] != null && !(ds.Tables[0].Rows[i]["AssessmentId"] is DBNull))
                                aq.SelectedAssessmentId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssessmentId"]);

                            if (ds.Tables[0].Rows[i]["AssessmentName"] != null && !(ds.Tables[0].Rows[i]["AssessmentName"] is DBNull))
                                aq.SelectedAssessment = ds.Tables[0].Rows[i]["AssessmentName"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAssessmentById", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return aq;
        }

        public List<AssessmentQuestion> GetAllAssessmentQuestion()
        {
            List<AssessmentQuestion> assesmentqas = new List<AssessmentQuestion>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllAssessmentQuestion]", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AssessmentQuestion am = new AssessmentQuestion();
                            am.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                            if (ds.Tables[0].Rows[i]["Question"] != null && !(ds.Tables[0].Rows[i]["Question"] is DBNull))
                                am.Question = ds.Tables[0].Rows[i]["Question"].ToString();
                            if (ds.Tables[0].Rows[i]["CorrectOption"] != null && !(ds.Tables[0].Rows[i]["CorrectOption"] is DBNull))
                                am.CorrectOption = ds.Tables[0].Rows[i]["CorrectOption"].ToString();

                            if (ds.Tables[0].Rows[i]["CorrectOptionSequence"] != null && !(ds.Tables[0].Rows[i]["CorrectOptionSequence"] is DBNull))
                                am.CorrectOptionSequence = Convert.ToInt32(ds.Tables[0].Rows[i]["CorrectOptionSequence"]);

                            if (ds.Tables[0].Rows[i]["Marks"] != null && !(ds.Tables[0].Rows[i]["Marks"] is DBNull))
                                am.Marks = Convert.ToInt32(ds.Tables[0].Rows[i]["Marks"]);

                            if (ds.Tables[0].Rows[i]["Option1"] != null && !(ds.Tables[0].Rows[i]["Option1"] is DBNull))
                                am.Option1 = ds.Tables[0].Rows[i]["Option1"].ToString();

                            if (ds.Tables[0].Rows[i]["Option2"] != null && !(ds.Tables[0].Rows[i]["Option2"] is DBNull))
                                am.Option2 = ds.Tables[0].Rows[i]["Option2"].ToString();

                            if (ds.Tables[0].Rows[i]["Option3"] != null && !(ds.Tables[0].Rows[i]["Option3"] is DBNull))
                                am.Option3 = ds.Tables[0].Rows[i]["Option3"].ToString();

                            if (ds.Tables[0].Rows[i]["Option4"] != null && !(ds.Tables[0].Rows[i]["Option4"] is DBNull))
                                am.Option4 = ds.Tables[0].Rows[i]["Option4"].ToString();

                            if (ds.Tables[0].Rows[i]["Option5"] != null && !(ds.Tables[0].Rows[i]["Option5"] is DBNull))
                                am.Option5 = ds.Tables[0].Rows[i]["Option5"].ToString();

                            if (ds.Tables[0].Rows[i]["Marks"] != null && !(ds.Tables[0].Rows[i]["Marks"] is DBNull))
                                am.Marks = Convert.ToInt32(ds.Tables[0].Rows[i]["Marks"]);

                            if (ds.Tables[0].Rows[i]["AssessmentName"] != null && !(ds.Tables[0].Rows[i]["AssessmentName"] is DBNull))
                                am.SelectedAssessment = ds.Tables[0].Rows[i]["AssessmentName"].ToString();

                            if (ds.Tables[0].Rows[i]["AssessmentId"] != null && !(ds.Tables[0].Rows[i]["AssessmentId"] is DBNull))
                                am.SelectedAssessmentId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssessmentId"]);

                            assesmentqas.Add(am);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dh, "SQLServerDAL,GetAllAssessmentForMaster", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return assesmentqas;
        }

        public bool UpdateAssessmentQuestion(AssessmentQuestion AQ)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            bool status = true;
            try
            {

                SqlParameter[] parameters =
                   {
                    new SqlParameter("@Id", SqlDbType.Int),
                    new SqlParameter("@SelAssessmentId", SqlDbType.Int ),
                    new SqlParameter("@Question", SqlDbType.NVarChar ),
                    new SqlParameter("@CorrectOption", SqlDbType.NVarChar),
                    new SqlParameter("@CorrectOptionSequence", SqlDbType.Int),
                    new SqlParameter("@Marks", SqlDbType.Int),
                    new SqlParameter("@Option1", SqlDbType.NVarChar),
                    new SqlParameter("@Option2", SqlDbType.NVarChar),
                    new SqlParameter("@Option3", SqlDbType.NVarChar),
                    new SqlParameter("@Option4", SqlDbType.NVarChar),
                    new SqlParameter("@Option5", SqlDbType.NVarChar),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = AQ.ID;
                parameters[1].Value = AQ.SelectedAssessmentId;
                parameters[2].Value = AQ.Question;
                parameters[3].Value = AQ.CorrectOption;
                parameters[4].Value = AQ.CorrectOptionSequence;
                parameters[5].Value = AQ.Marks;
                parameters[6].Value = AQ.Option1;
                parameters[7].Value = AQ.Option2;
                parameters[8].Value = AQ.Option3;
                parameters[9].Value = AQ.Option4;
                parameters[10].Value = AQ.Option5;
                parameters[11].Direction = ParameterDirection.Output;
                parameters[12].Direction = ParameterDirection.Output;
                parameters[12].Size = 4000;
                count = dh.ExecuteNonQuery("[dbo].[proc_UpdateAssessmentQuestion]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog(dh, "SQLServerDAL,UpdateAssessmentQuestion", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                    status = false;
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dh, "SQLServerDAL,UpdateAssessmentQuestion", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                status = false;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return status;
        }


        public void AddAssessmentQuestion(AssessmentQuestion AQ)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                   {
                    new SqlParameter("@SelAssessmentId",SqlDbType.Int ),
                    new SqlParameter("@Question", SqlDbType.NVarChar ),
                    new SqlParameter("@CorrectOption", SqlDbType.NVarChar),
                    new SqlParameter("@CorrectOptionSequence", SqlDbType.Int),
                    new SqlParameter("@Marks", SqlDbType.NVarChar),
                    new SqlParameter("@Option1", SqlDbType.NVarChar),
                    new SqlParameter("@Option2", SqlDbType.NVarChar),
                    new SqlParameter("@Option3", SqlDbType.NVarChar),
                    new SqlParameter("@Option4", SqlDbType.NVarChar),
                    new SqlParameter("@Option5", SqlDbType.NVarChar),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };

                parameters[0].Value = AQ.SelectedAssessmentId;
                parameters[1].Value = AQ.Question;
                parameters[2].Value = AQ.CorrectOption;
                parameters[3].Value = AQ.CorrectOptionSequence;
                parameters[4].Value = AQ.Marks;
                parameters[5].Value = AQ.Option1;
                parameters[6].Value = AQ.Option2;
                parameters[7].Value = AQ.Option3;
                parameters[8].Value = AQ.Option4;
                parameters[9].Value = AQ.Option5;
                parameters[10].Direction = ParameterDirection.Output;
                parameters[11].Direction = ParameterDirection.Output;
                parameters[11].Size = 4000;
                dh.ExecuteNonQuery("[dbo].[proc_AddAssessmentQuestion]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    LogHelper.AddLog(dh, "SQLServerDAL,AddAssessmentQuestion", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,AddAssessmentQuestion", ex.Message, ex.StackTrace, "HCL.Academy.DAL", "");
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }


        public void DeleteAssessmentQuestion(int id)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            int count;
            try
            {
                SqlParameter[] parameters =
                    {
                    new SqlParameter("@ID",SqlDbType.Int),
                    new SqlParameter("@ErrorNumber",SqlDbType.Int),
                    new SqlParameter("@ErrorMessage",SqlDbType.VarChar),
                };
                parameters[0].Value = id;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                count = dh.ExecuteNonQuery("[dbo].[proc_DeleteAssessmentQuestion]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorNumber"].Value != DBNull.Value && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {

                    LogHelper.AddLog(dh, "SQLServerDAL,DeleteAssessmentQuestion", dh.Cmd.Parameters["@ErrorNumber"].Value.ToString(), dh.Cmd.Parameters["@ErrorMessage"].Value.ToString(), "HCL.Academy.DAL", "");
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }

        #endregion

        #region PrivateMethods

        private List<SkillCompetencies> GetSkillCompetenciesByName(string Skill)
        {
            List<SkillCompetencies> skillCompetencies = new List<SkillCompetencies>();
            List<Competence> competences = GetCompetenciesBySkillName(Skill);
            var ulHTML = new TagBuilder("ul");
            StringBuilder output = new StringBuilder();
            return skillCompetencies;
        }

        private List<UserSkillDetail> GetCourseWiseTrainingModules(List<UserTrainingDetail> trainingModuleList)
        {

            List<UserSkillDetail> skills = new List<UserSkillDetail>();
            var courseList = trainingModuleList.Select(x => x.SkillName).Distinct();

            foreach (var courseItem in courseList)
            {
                UserSkillDetail skill = new UserSkillDetail();
                skill.skillName = courseItem;

                skill.listOfTraining = (List<UserTrainingDetail>)trainingModuleList.Where(x => x.SkillName == courseItem)
                    .Select(x => new UserTrainingDetail
                    {
                        SkillName = x.SkillName,
                        TrainingName = x.TrainingName,
                        NoOfAttempts = x.NoOfAttempts,
                        LastDayToComplete = x.LastDayToComplete,
                        IsTrainingCompleted = x.IsTrainingCompleted,
                        status = x.status,
                        bgColor = x.bgColor,
                        IsLink = x.IsLink,
                        IsWikiLink = x.IsWikiLink,
                        LinkUrl = x.LinkUrl,
                        DocumentUrl = x.DocumentUrl,
                        TrainingType = x.TrainingType
                    }).ToList();
                skill.id = skill.listOfTraining[0].SkillId;
                skill.trainingType = skill.listOfTraining[0].TrainingType;
                skills.Add(skill);
            }
            return skills;
        }

        private List<UserTrainingDetail> GetSkillBasedTrainingsList(int userId)
        {
            List<UserTrainingDetail> trainingsLst = new List<UserTrainingDetail>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillBasedTraining]", CommandType.StoredProcedure, new SqlParameter("@UserId", currentUser.id));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                ArrayList defaultTrainingIds = new ArrayList();
                ArrayList defaultAssessmentIds = new ArrayList();
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.IsLink = false;
                        item.DocumentUrl = null;
                        if (row["TrainingName"] != null && !(row["TrainingName"] is DBNull))
                            item.TrainingName = row["TrainingName"].ToString();
                        if (row["SkillID"] != null && !(row["SkillID"] is DBNull))
                            item.SkillId = Int32.Parse(row["SkillID"].ToString());
                        if (row["Skill"] != null && !(row["Skill"] is DBNull))
                            item.SkillName = row["Skill"].ToString();
                        if (row["CompletedDate"] != null && !(row["CompletedDate"] is DBNull))
                            item.CompletionDate = Convert.ToDateTime(row["CompletedDate"].ToString()).ToShortDateString();

                        if (row["IsMandatory"] != null && !(row["IsMandatory"] is DBNull))
                            item.Mandatory = Convert.ToBoolean(row["IsMandatory"]);

                        if (row["IsTrainingCompleted"] != null && !(row["IsTrainingCompleted"] is DBNull))
                            item.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);

                        if (item.IsTrainingCompleted)
                        {
                            item.status = Utilities.GetTraningStatus(item.IsTrainingCompleted, item.LastDayToComplete);
                        }
                        if (row["LastDayCompletion"] != null && !(row["LastDayCompletion"] is DBNull))
                            item.LastDayToComplete = Convert.ToDateTime(row["LastDayCompletion"]);

                        item.bgColor = Utilities.GetTrainingColor(item.status);
                        item.TrainingType = TrainingType.SkillTraining;
                        trainingsLst.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AddLog(dh, "SQLServerDAL,GetSkillBasedTrainingsList", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainingsLst;
        }
        private List<UserDetails> GetUserDetails(int skillId, int competenceId, int projectId)
        {
            List<Training> trainings = new List<Training>();
            List<Training> lstAllTrainings = new List<Training>();
            List<UserDetails> userDetails = new List<UserDetails>();
            List<UserDetails> userTrainings = new List<UserDetails>();
            List<UserDetails> allDetails = new List<UserDetails>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
               {
                    new SqlParameter("@SkillId",SqlDbType.Int),
                    new SqlParameter("@CompetencyId",SqlDbType.Int)
                };
                parameters[0].Value = skillId;
                parameters[1].Value = competenceId;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillCompetencyLevelTraining]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = new DataTable();
                dt = dv.ToTable();
                if (dt != null & dt.Rows.Count > 0)
                {
                    UserDetails allskill = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        allskill = new UserDetails();
                        allskill.TrainingId = Convert.ToInt32(dr["TrainingId"]);
                        allskill.TrainingName = Convert.ToString(dr["TrainingName"]);
                        if (!(dr["IsMandatory"] is DBNull))
                            allskill.IsMandatory = Convert.ToBoolean(dr["IsMandatory"]);
                        allskill.skillName = Convert.ToString(dr["SkillName"]);
                        allskill.competenceName = Convert.ToString(dr["CompetenceName"]);
                        // allskill.LastDayCompletion = Convert.ToString(dr["LastDayCompletion"]);
                        userDetails.Add(allskill);
                        Training training = new Training();
                        training.TrainingId = allskill.TrainingId;
                        training.TrainingName = allskill.TrainingName;
                        training.IsMandatory = allskill.IsMandatory;
                        lstAllTrainings.Add(training);
                    }
                    if (lstAllTrainings.Count > 0)
                    {
                        List<UserDetails> lstAllUsers = new List<UserDetails>();
                        List<UserDetails> lstTrainedUsers = new List<UserDetails>();
                        lstAllUsers = GetUsersForAllTrainings(projectId);             //Fetching user details corresponding to the Trainings
                        foreach (Training train in lstAllTrainings)
                        {
                            var trainUsers = lstAllUsers.Where(user => user.TrainingCourse == train.TrainingName).ToList();
                            lstTrainedUsers.AddRange(trainUsers);
                        }
                        foreach (UserDetails user in lstTrainedUsers)
                        {
                            if (!userTrainings.Contains(user))
                            {
                                userTrainings.AddRange(lstTrainedUsers);
                            }
                        }
                    }
                }
                if (userTrainings.Count > 0)                                            //Merging the Training and corresponding Users data.
                {
                    var lstDetails = (from s1 in userDetails
                                      join s2 in userTrainings
                 on s1.TrainingName equals s2.TrainingCourse
                                      select new UserDetails()
                                      {
                                          TrainingId = s1.TrainingId,
                                          TrainingName = s1.TrainingName,
                                          IsMandatory = s1.IsMandatory,
                                          Employee = s2.Employee,
                                          IsTrainingCompleted = s2.IsTrainingCompleted,
                                          AdminApprovalStatus = s2.AdminApprovalStatus,
                                          LastDayCompletion = s2.LastDayCompletion,
                                          TrainingCourse = s2.TrainingCourse,
                                          skillName = s1.skillName,
                                          competenceName = s1.competenceName
                                      }).ToList();
                    allDetails = lstDetails;
                    allDetails.Distinct().ToList();
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return allDetails;
        }

        private List<UserDetails> GetUsersForAllTrainings(int projectId)
        {
            List<UserDetails> lstUsersDetails = new List<UserDetails>();
            //Tables:UserTraining,SkillCompetencyLevelTraining,AcademyOnboarding
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@projectId",SqlDbType.Int)
                };
                parameters[0].Value = projectId;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetUsersForAllTrainings]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = new DataTable();
                dt = dv.ToTable();
                UserDetails objTraining = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        objTraining = new UserDetails();
                        objTraining.TrainingId = Convert.ToInt32(dr["TrainingId"]);
                        objTraining.IsTrainingCompleted = Convert.ToBoolean(dr["IsTrainingCompleted"]);
                        objTraining.TrainingCourse = Convert.ToString(dr["Training"]);
                        objTraining.Employee = Convert.ToString(dr["EmployeeName"]);
                        objTraining.skillName = Convert.ToString(dr["SkillName"]);
                        objTraining.AdminApprovalStatus = Convert.ToString(dr["AdminApprovalStatus"]);
                        objTraining.LastDayCompletion = Convert.ToString(dr["LastDayCompletion"]);
                        lstUsersDetails.Add(objTraining);
                    }
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return lstUsersDetails;
        }

        private List<TrainingCount> GetTrainingCounts(List<UserDetails> userTrainings)
        {
            List<TrainingCount> lstCount = new List<TrainingCount>();
            List<TrainingCount> trainingCounts = new List<TrainingCount>();
            var allCount = userTrainings.GroupBy(train => new { train.TrainingName, train.AdminApprovalStatus, train.competenceName }).Select(grp =>
            new
            {
                trainingDetail = grp.Key,
                total = grp.Count()
            }).ToList();            //Categorizing the Users based on Training Name and Completion status

            if (allCount.Count() > 0)           //If users exist for particular training
            {
                foreach (var train in allCount)
                {
                    int comp = 0;
                    int prog = 0;
                    if (train.trainingDetail.AdminApprovalStatus == "Approved")
                        comp = train.total;
                    else
                        prog = train.total;
                    lstCount.Add(new TrainingCount()
                    {
                        trainingName = train.trainingDetail.TrainingName,
                        competencyLevel = train.trainingDetail.competenceName,
                        completedCount = comp,
                        progressCount = prog
                    });
                }

                int i = 0;
                if (lstCount.Count > 1)     //For trainings which have Users who have Completed as well as Work in Progress status
                {
                    while ((i + 1) < lstCount.Count)                        //Normalizing count based on Training name to avoid multiple rows
                    {
                        if (lstCount[i].trainingName == lstCount[i + 1].trainingName)
                        {
                            lstCount[i].trainingName = lstCount[i].trainingName;
                            lstCount[i].competencyLevel = lstCount[i].competencyLevel;
                            lstCount[i].completedCount = lstCount[i].completedCount + lstCount[i + 1].completedCount;
                            lstCount[i].progressCount = lstCount[i].progressCount + lstCount[i + 1].progressCount;
                            lstCount.RemoveAt(i + 1);
                        }
                        i++;
                    }
                    trainingCounts = lstCount;
                }
                else                               //For trainings which have users with either Completed/Work in Progress status
                    return lstCount;
            }
            return trainingCounts;
        }

        private List<UserSkillDetail> GetSkillWiseTrainings(List<UserTrainingDetail> trainingModuleList)
        {
            List<UserSkillDetail> skills = new List<UserSkillDetail>();
            var courseList = trainingModuleList.Select(x => x.SkillName).Distinct();

            foreach (var courseItem in courseList)
            {
                UserSkillDetail skill = new UserSkillDetail();
                skill.skillName = courseItem;

                skill.listOfTraining = (List<UserTrainingDetail>)trainingModuleList.Where(x => x.SkillName == courseItem)
                    .Select(x => new UserTrainingDetail
                    {
                        SkillName = x.SkillName,
                        TrainingName = x.TrainingName,
                        NoOfAttempts = x.NoOfAttempts,
                        LastDayToComplete = x.LastDayToComplete,
                        IsTrainingCompleted = x.IsTrainingCompleted,
                        status = x.status,
                        bgColor = x.bgColor,
                        IsLink = x.IsLink,
                        IsWikiLink = x.IsWikiLink,
                        LinkUrl = x.LinkUrl,
                        DocumentUrl = x.DocumentUrl,
                        TrainingType = x.TrainingType
                    }).ToList();
                skill.id = skill.listOfTraining[0].SkillId;
                skill.trainingType = skill.listOfTraining[0].TrainingType;
                skills.Add(skill);
            }
            return skills;
        }
        private List<CheckListItem> GetOnBoardingCheckList(string locValue, int locId)
        {
            List<CheckListItem> checklistItems = GetAllChecklist();
            List<CheckListItem> filteredItems = checklistItems.Where(c => c.geoId == locId).ToList();
            return filteredItems;
        }
        public List<CheckListItem> GetAllChecklist()
        {
            checkListItems = new List<CheckListItem>();
            DataSet dsCheckList = new DataSet();
            DataView dvCheckList = new DataView();
            DataHelper dhCheckList = new DataHelper(strConnectionString);

            dsCheckList = dhCheckList.ExecuteDataSet("[dbo].[proc_GetAllChecklists]", CommandType.StoredProcedure);
            dvCheckList = new DataView(dsCheckList.Tables[0]);
            DataTable dtCheckList = dvCheckList.ToTable();
            if (dtCheckList != null && dtCheckList.Rows.Count != 0)
            {
                foreach (DataRow row in dtCheckList.Rows)
                {
                    CheckListItem item = new CheckListItem();
                    item.id = Convert.ToInt32(row["ID"].ToString());
                    if (Convert.ToBoolean(row["TypeChoice"]))
                        item.choice = "TRUE";
                    else
                        item.choice = "FALSE";
                    item.desc = row["Description"].ToString();
                    item.geoId = Convert.ToInt32(row["GEOID"]);
                    item.geoName = row["GEO"].ToString();
                    item.internalName = row["InternalName"].ToString();
                    item.roleId = Convert.ToInt32(row["RoleID"]);
                    item.roleName = row["Role"].ToString();
                    item.name = row["Title"].ToString();
                    checkListItems.Add(item);
                }
            }
            //if (HttpContext.Current != null)
            //    HttpContext.Current.Session[AppConstant.AllCheckListData] = checkListItems;

            return checkListItems;
        }
        private List<OnBoarding> GetUserAssessments(List<OnBoarding> boardingList)
        {
            List<int> ids = new List<int>();
            List<UserAssessment> userAssessments = new List<UserAssessment>();
            List<Assessment> allAssessments = new List<Assessment>();
            allAssessments = GetAllAssessments();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserId";
                sqlParameters[0].Value = currentUser.id;
                sqlParameters[0].Direction = ParameterDirection.Input;
                sqlParameters[1] = new SqlParameter();
                sqlParameters[1].ParameterName = "@OnlyOnBoardedTraining";
                sqlParameters[1].Value = true;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetAssessmentsForUser]", CommandType.StoredProcedure, sqlParameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserAssessment userAssessment = new UserAssessment();
                        Assessment assessmentItem = new Assessment();
                        bool assessmentStatus = false;
                        if (row["Assessment"] != null && !(row["Assessment"] is DBNull))
                            userAssessment.TrainingAssessment = row["Assessment"].ToString();
                        if (row["CompletedDate"] != null && !(row["CompletedDate"] is DBNull))
                            userAssessment.CompletedDate = Convert.ToString(row["CompletedDate"]);
                        if (row["IsAssessmentComplete"] != null && !(row["IsAssessmentComplete"] is DBNull))
                        {
                            assessmentStatus = true;
                            userAssessment.IsAssessmentComplete = Convert.ToBoolean(row["IsAssessmentComplete"]);
                        }
                        if (row["AssessmentId"] != null && !(row["AssessmentId"] is DBNull))
                            userAssessment.TrainingAssessmentId = row["AssessmentId"] != null ? Convert.ToInt32(row["AssessmentId"]) : 0;
                        userAssessments.Add(userAssessment);
                        int attempts = 0;
                        DateTime date = new DateTime();
                        if (row["NoOfAttempt"] != null && !(row["NoOfAttempt"] is DBNull))
                            attempts = Convert.ToInt32(row["NoOfAttempt"]);
                        if (row["LastDayCompletion"] != null && !(row["LastDayCompletion"] is DBNull))
                            date = Convert.ToDateTime(row["LastDayCompletion"]);
                        if (assessmentItem != null)
                        {
                            OnBoarding board = new OnBoarding();
                            board.boardingItemId = Convert.ToInt32(row["ID"]);
                            board.boardingItemName = assessmentItem.AssessmentName;
                            board.boardingItemDesc = assessmentItem.Description;
                            board.boardingStatus = (OnboardingStatus)Utilities.GetOnBoardingStatus
                                                    (
                                                        assessmentStatus,
                                                        attempts,
                                                        date
                                                    );

                            if (board.boardingStatus == OnboardingStatus.Completed || board.boardingStatus == OnboardingStatus.Failed ||
                                board.boardingStatus == OnboardingStatus.OverDue || board.boardingStatus == OnboardingStatus.Rejected)
                            {
                                board.boardingItemLink = "#";
                            }
                            else
                            {
                                board.boardingItemLink = assessmentItem.AssessmentLink;
                            }
                            board.boardingType = OnboardingItemType.Assessment;
                            board.boardIngAssessmentId = assessmentItem.AssessmentId;
                            boardingList.Add(board);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetUserAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return boardingList;
        }
        private List<Assessment> GetAllAssessments()
        {
            List<Assessment> assessments = new List<Assessment>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet dsAssessments = new DataSet();
                DataView dvAssessments = new DataView();

                dsAssessments = dh.ExecuteDataSet("[dbo].[proc_GetAllAssessments]", CommandType.StoredProcedure);
                if (dsAssessments.Tables.Count > 0)
                    dvAssessments = new DataView(dsAssessments.Tables[0]);
                DataTable dtAssessment = dvAssessments.ToTable();

                if (dtAssessment != null & dtAssessment.Rows.Count > 0)
                {
                    Assessment assessment = null;
                    foreach (DataRow item in dtAssessment.Rows)
                    {
                        string _AssessmentLink = null;
                        if (item["AssessmentLink"] != null)
                        {
                            _AssessmentLink = item["AssessmentLink"].ToString();
                        }
                        int _AssessmentTimeInMins = 0;
                        if (item["AssessmentTimeInMins"] != null)
                        {
                            _AssessmentTimeInMins = Convert.ToInt32(item["AssessmentTimeInMins"].ToString());
                        }

                        assessment = new Assessment();

                        assessment.AssessmentId = Convert.ToInt32(item["ID"].ToString());
                        assessment.AssessmentName = Convert.ToString(item["Title"]);
                        if (item["IsMandatory"] != null)
                            assessment.IsMandatory = Convert.ToBoolean(item["IsMandatory"]);
                        assessment.AssessmentLink = _AssessmentLink;
                        assessment.AssessmentTimeInMins = _AssessmentTimeInMins;
                        if (item["Description"] != null)
                            assessment.Description = Convert.ToString(item["Description"]);
                        assessments.Add(assessment);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,GetAllAssessments", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return assessments;
        }

        #endregion


        #region "External User Methods"
        public List<Organization> GetAllOrganizations()
        {
            List<Organization> items = new List<Organization>();

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllOrganizations]", CommandType.StoredProcedure);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Organization item = new Organization();
                        item.Id = Convert.ToInt32(row["ID"].ToString());
                        item.Name = row["Name"].ToString();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public List<UserGroup> GetAllUserGroups()
        {
            List<UserGroup> items = new List<UserGroup>();

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllUserGroups]", CommandType.StoredProcedure);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UserGroup item = new UserGroup();
                        item.Id = Convert.ToInt32(row["ID"].ToString());
                        item.Name = row["Name"].ToString();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public List<ExternalUser> GetAllExternalUsers()
        {
            List<ExternalUser> items = new List<ExternalUser>();

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllExternalUsers]", CommandType.StoredProcedure);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ExternalUser item = new ExternalUser();
                        item.ID = Convert.ToInt32(row["ID"].ToString());
                        item.UserId = row["UserID"].ToString();
                        item.UserName = row["UserName"].ToString();
                        //item.EncryptedPassword = row["EncryptedPassword"].ToString();
                        //item.PasswordSalt = row["PasswordSalt"].ToString();
                        item.OrganizationID = Convert.ToInt32(row["OrganizationID"].ToString());
                        item.Name = row["Name"].ToString();
                        item.Organization = row["OrganizationName"].ToString();
                        item.GEO = row["GEOName"].ToString();
                        item.Role = row["RoleName"].ToString();
                        item.Group = row["GroupName"].ToString();
                        item.EmployeeId = Convert.ToInt32(row["EmployeeId"].ToString());
                        item.RoleId = Convert.ToInt32(row["RoleId"].ToString());
                        item.GroupId = Convert.ToInt32(row["GroupId"].ToString());
                        item.CompetencyLevelId = Convert.ToInt32(row["CompetencyLevelId"].ToString());
                        item.SkillId = Convert.ToInt32(row["SkillId"].ToString());
                        item.GEOId = Convert.ToInt32(row["GEOId"].ToString());
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public ExternalUser GetExternalUserById(int ID)
        {
            ExternalUser item = null;

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@ID";
                sqlParameters[0].Value = ID;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetExternalUserById]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        item = new ExternalUser();
                        item.ID = Convert.ToInt32(row["ID"].ToString());
                        item.UserId = row["UserID"].ToString();
                        item.UserName = row["UserName"].ToString();
                        item.EncryptedPassword = row["EncryptedPassword"].ToString();
                        item.PasswordSalt = row["PasswordSalt"].ToString();
                        item.OrganizationID = Convert.ToInt32(row["OrganizationID"].ToString());
                    }
                }
            }
            return item;
        }
        public ExternalUser GetExternalUserByUserName(string UserName)
        {
            ExternalUser item = null;

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserName";
                sqlParameters[0].Value = UserName;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetExternalUserByUserName]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        item = new ExternalUser();
                        item.ID = Convert.ToInt32(row["ID"].ToString());
                        item.UserId = row["UserID"].ToString();
                        item.UserName = row["UserName"].ToString();
                        item.EncryptedPassword = row["EncryptedPassword"].ToString();
                        item.PasswordSalt = row["PasswordSalt"].ToString();
                        item.OrganizationID = Convert.ToInt32(row["OrganizationID"].ToString());
                        item.FirstPasswordChanged = Convert.ToBoolean(row["FirstPasswordChanged"].ToString());
                    }
                }
            }
            return item;
        }
        public ExternalUser GetExternalUserByUserId(string UserId)
        {
            ExternalUser item = null;

            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserId";
                sqlParameters[0].Value = UserId;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetExternalUserByUserId]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        item = new ExternalUser();
                        item.ID = Convert.ToInt32(row["ID"].ToString());
                        item.UserId = row["UserID"].ToString();
                        item.UserName = row["UserName"].ToString();
                        item.EncryptedPassword = row["EncryptedPassword"].ToString();
                        item.PasswordSalt = row["PasswordSalt"].ToString();
                        item.OrganizationID = Convert.ToInt32(row["OrganizationID"].ToString());
                    }
                }
            }
            return item;
        }

        public string SaveExternalUser(ExternalUserRequest User)
        {
            string exceptionMessage = null;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id",SqlDbType.Int),
                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                    new SqlParameter("@EncryptedPassword",SqlDbType.NVarChar),
                    new SqlParameter("@PasswordSalt",SqlDbType.NVarChar),
                    new SqlParameter("@OrganizationID",SqlDbType.Int),
                    new SqlParameter("@EmployeeId",SqlDbType.Int),
                    new SqlParameter("@RoleId",SqlDbType.Int),
                    new SqlParameter("@SkillId",SqlDbType.Int),
                    new SqlParameter("@CompetenceId",SqlDbType.Int),
                    new SqlParameter("@GEOId",SqlDbType.Int),
                    new SqlParameter("@Name",SqlDbType.NVarChar),
                    new SqlParameter("@GroupId",SqlDbType.Int),
                    new SqlParameter("@errorMessage",SqlDbType.NVarChar)
                };

                parameters[0].Value = User.Id;
                parameters[1].Value = User.UserName;
                parameters[2].Value = User.EncryptedPassword;
                parameters[3].Value = User.PasswordSalt;
                parameters[4].Value = User.OrganizationId;
                parameters[5].Value = User.EmployeeId;
                parameters[6].Value = User.RoleId;
                parameters[7].Value = User.SkillId;
                parameters[8].Value = User.CompetencyLevelId;
                parameters[9].Value = User.GEOId;
                parameters[10].Value = User.Name;
                parameters[11].Value = User.GroupId;
                parameters[12].Size = 4000;
                parameters[12].Direction = ParameterDirection.Output;
                dh.ExecuteNonQuery("[dbo].[proc_SaveExternalUser]", CommandType.StoredProcedure, parameters);
                exceptionMessage = Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value);
                if (dh.Cmd != null && String.IsNullOrEmpty(exceptionMessage) && User.Id == 0)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("UserName", User.Name);
                    hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    hashtable.Add("Password", User.Password);
                    bool passwordemailsent = AddToEmailQueue("ExternalUserOnboardMail", hashtable, User.UserName, null);
                    OnboardEmail(User.UserName, 0, User.Name);
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return exceptionMessage;
        }
        public bool UpdateExternalUserPasswordStatus(string useremail)
        {
            bool flag = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {

                SqlParameter[] parameters = { new SqlParameter("@useremail", SqlDbType.VarChar) };
                parameters[0].Value = useremail;
                dh.ExecuteNonQuery("[dbo].[proc_UpdateExternalUserPasswordStatus]", CommandType.StoredProcedure, parameters);
                flag = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,UpdateExternalUserPasswordStatus", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return flag;
        }

        public bool UpdateNewsEvent(int id, string imageURL, string header, string body, string trimBody)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", id),
                    new SqlParameter("@imageURL", imageURL),new SqlParameter("@Header", header), new SqlParameter("@Body", body),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_UpdateNews]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public bool SendExternalUserPassword(ExternalUserRequest User)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool passwordemailsent = false;
            try
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("UserName", User.Name);
                hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                hashtable.Add("Password", User.Password);
                passwordemailsent = AddToEmailQueue("ExternalUserOnboardMail", hashtable, User.UserName, null);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,SendExternalUserPassword", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return passwordemailsent;
        }
        public bool DeleteExternalUser(int id)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            bool flag = false;
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@Id", SqlDbType.Int) };
                parameters[0].Value = id;
                dh.ExecuteNonQuery("[dbo].[proc_DeleteExternalUser]", CommandType.StoredProcedure, parameters);
                flag = true;
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,DeleteExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null) { dh.DataConn.Close(); }
                }
            }
            return flag;
        }

        public bool DeleteNews(int id)
        {
            bool result = false;
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@Id", SqlDbType.Int) };
                parameters[0].Value = id;
                int count = dh.ExecuteNonQuery("[dbo].[proc_DeleteNews]", CommandType.StoredProcedure, parameters);
                if (count > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(dh, "SQLServerDAL,DeleteNews", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null) { dh.DataConn.Close(); }
                }
            }
            return result;
        }

        public List<UserGroupMemberShip> GetUserGroupMemberShip(ExternalUserRequest User)
        {
            List<UserGroupMemberShip> groups = new List<UserGroupMemberShip>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@UserName";
                sqlParameters[0].Value = User.UserName;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetUserGroupMemberShip]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UserGroupMemberShip group = new UserGroupMemberShip();
                        group.GroupId = Convert.ToInt32(row["GroupID"].ToString());
                        group.GroupName = row["Name"].ToString();
                        group.GroupPermission = Convert.ToInt32(row["GroupPermission"].ToString());
                        groups.Add(group);
                    }
                }
            }
            return groups;
        }
        public string ResetExternalUserPassword(ExternalUserRequest req)
        {
            string exceptionMessage = null;

            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id",SqlDbType.Int),
                    new SqlParameter("@EncryptedPassword",SqlDbType.NVarChar),
                    new SqlParameter("@PasswordSalt",SqlDbType.NVarChar),
                    new SqlParameter("@errorMessage",SqlDbType.NVarChar)
                };
                parameters[0].Value = req.Id;
                parameters[1].Value = req.EncryptedPassword;
                parameters[2].Value = req.PasswordSalt;
                parameters[3].Size = 4000;
                parameters[3].Direction = ParameterDirection.Output;

                dh.ExecuteNonQuery("[dbo].[proc_ResetExternalUserPassword]", CommandType.StoredProcedure, parameters);

                if (dh.Cmd != null && dh.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    exceptionMessage = Convert.ToString(dh.Cmd.Parameters["@ErrorMessage"].Value);
                }

                if (dh.Cmd != null && String.IsNullOrEmpty(exceptionMessage))
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("UserName", req.Name);
                    hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    hashtable.Add("Password", req.Password);
                    bool passwordemailsent = AddToEmailQueue("ChangePasswordMail", hashtable, req.UserName, null);

                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return exceptionMessage;
        }
        #endregion

        public void AddProjectAdmin(int userid, int projectid)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
           {
                new SqlParameter("@userid",SqlDbType.Int ) { Value = userid},
                new SqlParameter("@projectid",SqlDbType.Int ) { Value = projectid}
            };

            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_AddProjectAdmin]", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddProjectAdmin", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

        }

        public void DeleteProjectAdmin(int projectid, int userid)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
           {
                new SqlParameter("@adminid",SqlDbType.Int ) { Value = userid},
                new SqlParameter("@projectid",SqlDbType.Int ) { Value = projectid}
            };

            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_DeleteProjectAdmin]", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,DeleteProjectAdmin", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }

        }

        public List<ProjectAdmin> GetProjectAdmin(int projectid)
        {
            List<ProjectAdmin> admins = new List<ProjectAdmin>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@projectid";
                sqlParameters[0].Value = projectid;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetProjectAdmin]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ProjectAdmin admin = new ProjectAdmin();
                        admin.EmployeeId = Convert.ToInt32(row["EmployeeId"].ToString());
                        admin.Name = row["Name"].ToString();
                        admin.EmailAddress = row["EmailAddress"].ToString();
                        admin.UserId = Convert.ToInt32(row["AdminId"].ToString());
                        admins.Add(admin);
                    }
                }
            }
            return admins;

        }

        public List<Project> GetProjectByParent(int projectid)
        {
            List<Project> projects = new List<Project>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter();
                sqlParameters[0].ParameterName = "@parentprojectid";
                sqlParameters[0].Value = projectid;
                sqlParameters[0].Direction = ParameterDirection.Input;

                ds = dh.ExecuteDataSet("[dbo].[proc_GetProjectsByParent]", CommandType.StoredProcedure, sqlParameters);
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Project project = new Project();
                        project.parentProjectId = Convert.ToInt32(row["ParentProjectId"].ToString());
                        project.projectName = row["Title"].ToString();
                        project.projectLevel = Convert.ToInt32(row["ProjectLevel"].ToString());
                        project.id = Convert.ToInt32(row["ID"].ToString());
                        projects.Add(project);
                    }
                }
            }
            return projects;

        }
        public void AddProjectDetails(string name, int parentprojectid, int projectlevel)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
           {
                new SqlParameter("@name",SqlDbType.NVarChar) { Value = name},
                new SqlParameter("@parentprojectid",SqlDbType.Int ) { Value = parentprojectid},
                new SqlParameter("@projectlevel",SqlDbType.Int ) { Value = projectlevel}
            };

            try
            {
                dh.ExecuteNonQuery("[dbo].[proc_AddProjectDetails]", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddProjectDetails", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
        }
        public ProjectAdminInfo GetProjectAdminInfo(int userid)
        {
            ProjectAdminInfo projAdminInfo = new ProjectAdminInfo();
            projAdminInfo.SecondLevelProjects = new List<int>();
            projAdminInfo.ThirdLevelProjects = new List<ProjectInfo>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            SqlParameter[] parameters =
           {
                new SqlParameter("@userid",SqlDbType.Int) { Value = userid}
            };

            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetProjectAdminInfo]", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetProjectAdminInfo", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }

            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(row["ProjectLevel"].ToString()) == 0)
                        {
                            projAdminInfo.IsFirstLevelAdmin = true;

                        }
                        else if (Convert.ToInt32(row["ProjectLevel"].ToString()) == 1)
                        {
                            projAdminInfo.IsSecondLevelAdmin = true;
                            projAdminInfo.SecondLevelProjects.Add(Convert.ToInt32(row["ProjectId"].ToString()));
                        }
                        else if (Convert.ToInt32(row["ProjectLevel"].ToString()) == 2)
                        {
                            projAdminInfo.IsThirdLevelAdmin = true;
                            ProjectInfo p = new ProjectInfo();
                            p.ProjectId = Convert.ToInt32(row["ProjectId"].ToString());
                            p.ParentProjectId = Convert.ToInt32(row["ParentProjectId"].ToString());
                            projAdminInfo.ThirdLevelProjects.Add(p);
                        }
                    }
                }
            }
            return projAdminInfo;
        }
        public bool ProcessEscalationEmail()
        {
            LogHelper.AddLog("SQLServerDAL,ProcessEscalationEmail", "In ProcessEscalationEmail", "In ProcessEscalationEmail", "HCL.Academy.DAL", currentUser.emailId.ToString());
            bool result = false;
            try
            {

                DataSet dsTraining = new DataSet();
                DataTable dtTraining = new DataTable();
                DataHelper data = new DataHelper(strConnectionString);
                dsTraining = data.ExecuteDataSet("[dbo].[proc_EscalationMails]", CommandType.StoredProcedure, new SqlParameter("@CurrentDate", DateTime.Now));
                string strTrainingName = String.Empty;
                List<int> lstUserIDs = new List<int>();
                if (dsTraining != null)
                {
                    dtTraining = dsTraining.Tables[0];
                    if (dsTraining.Tables[0].Rows != null)
                    {
                        foreach (DataRow row in dtTraining.Rows)
                        {
                            int userID = Convert.ToInt32(row["UserId"]);
                            lstUserIDs.Add(userID);
                        }
                        lstUserIDs = lstUserIDs.Distinct().ToList();
                    }
                    result = SendEscalationEmails(lstUserIDs);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,ProcessRemiderEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }

        public bool SendEscalationEmails(List<int> userIDs)
        {
            bool status = false;
            //string strTrainingName = String.Empty;
            string UserName = String.Empty;
            string email = String.Empty;
            foreach (int id in userIDs)
            {
                List<UserTraining> training = new List<UserTraining>();
                training = GetTrainingForUser(id, false);
                UserManager users = GetUsersByID(id);
                UserName = users.UserName;
                email = users.EmailID;
                //foreach (UserTraining itemTraining in training)
                //{
                //    if (!itemTraining.IsTrainingCompleted)
                //    {
                //        DateTime dt = Convert.ToDateTime(itemTraining.LastDayCompletion);
                //        strTrainingName = strTrainingName + itemTraining.TrainingName + " Last Completion Date " + dt.ToString("MMMM dd") + ", ";
                //    }
                //}
                try
                {
                    string trainingTable = string.Empty;
                    trainingTable += "<table border='1' cellspacing='0' cellpadding='0' style='border-collapse: collapse; border: none;'>";
                    trainingTable += "<tbody>";
                    trainingTable += "<tr>";
                    trainingTable += "<td><b>Training Name</b></td>";
                    trainingTable += "<td><b>Last Date of Completion</b></td>";
                    trainingTable += "<td><b>Mandatory?</b></td>";
                    trainingTable += "</tr>";

                    foreach (UserTraining item in training)
                    {
                        if (!item.IsTrainingCompleted)
                        {
                            trainingTable += "<tr>";
                            trainingTable += "<td>" + item.TrainingName + "</td>";
                            trainingTable += "<td>" + item.LastDayCompletion + "</td>";
                            trainingTable += "<td>" + item.IsMandatory + "</td>";
                            trainingTable += "</tr>";
                        }
                    }
                    trainingTable += "</tbody>";
                    trainingTable += "</table>";
                    Hashtable data = new Hashtable();
                    data.Add("TrainingTable", trainingTable);
                    data.Add("UserName", UserName);
                    data.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    data.Add("WebUrl", "");

                    bool queue = AddToEmailQueue("EscalationEmail", data, email, null);
                    status = true;
                }
                catch (Exception ex)
                {
                    status = false;
                    LogHelper.AddLog("SQLServerDAL,OnboardEmail", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                }
            }
            return status;
        }

        public List<UserTraining> GetUserRoleBaseTrainingReport(int projectid, int roleid)
        {
            List<UserTraining> userTrainings = new List<UserTraining>();
            DataSet ds = new DataSet();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@projectid",SqlDbType.Int),new SqlParameter("@roleid",SqlDbType.Int)

                };
                parameters[0].Value = projectid;
                parameters[1].Value = roleid;

                ds = dh.ExecuteDataSet("[dbo].[proc_GenerateUserRoleBaseTrainingReport]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        UserTraining userTraining = new UserTraining();
                        userTraining.Employee = ds.Tables[0].Rows[i]["Name"].ToString();
                        userTraining.EmployeeId = ds.Tables[0].Rows[i]["EmployeeId"].ToString();
                        userTraining.EmailId = ds.Tables[0].Rows[i]["EmailAddress"].ToString();
                        userTraining.Role = ds.Tables[0].Rows[i]["RoleName"].ToString();
                        userTraining.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                        userTraining.TrainingId = Convert.ToInt32(ds.Tables[0].Rows[i]["TrainingId"].ToString());
                        //userTraining.SkillName = ds.Tables[0].Rows[i]["Skill"].ToString();
                        userTraining.TrainingName = ds.Tables[0].Rows[i]["TrainingName"].ToString();
                        userTraining.IsTrainingCompleted = false;
                        if (ds.Tables[0].Rows[i]["IsTrainingCompleted"] != null && !(ds.Tables[0].Rows[i]["IsTrainingCompleted"] is DBNull))
                            userTraining.IsTrainingCompleted = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsTrainingCompleted"].ToString());

                        if (ds.Tables[0].Rows[i]["AdminApprovalStatus"] != null && !(ds.Tables[0].Rows[i]["AdminApprovalStatus"] is DBNull))
                            userTraining.AdminApprovalStatus = Convert.ToString(ds.Tables[0].Rows[i]["AdminApprovalStatus"].ToString());
                        else
                            userTraining.AdminApprovalStatus = "Not Started";

                        userTraining.LastDayCompletion = ds.Tables[0].Rows[i]["LastDayCompletion"].ToString();
                        userTraining.CompletedDate = ds.Tables[0].Rows[i]["CompletedDate"].ToString();
                        userTrainings.Add(userTraining);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserRoleBaseTrainingReport", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return userTrainings;
        }

        public List<UserTrainingDetail> GetSkillBasedTrainingsUserView(int userid)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();

            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetSkillBasedTrainingsUserView]", CommandType.StoredProcedure, new SqlParameter("@UserId", userid));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.UserId = Convert.ToInt32(row["UserID"]);
                        item.EmailAddress = row["EmailAddress"].ToString().ToUpper();
                        item.UserName = Convert.ToString(row["UserName"]);
                        item.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        item.TrainingName = Convert.ToString(row["TrainingName"].ToString().ToUpper());
                        item.IsTrainingCompleted = row["IsTrainingCompleted"] == DBNull.Value ? false : Convert.ToBoolean(row["IsTrainingCompleted"]);

                        if (row["CompletedDate"] != null)
                        {
                            item.CompletionDate = Convert.ToString(row["CompletedDate"]);
                        }
                        else
                        {
                            item.CompletionDate = "";
                        }

                        item.AdminApprovalStatus = Convert.ToString(row["AdminApprovalStatus"]);
                        if (row["ProjectId"] != null && !(row["ProjectId"] is DBNull))
                            item.ProjectId = Convert.ToInt32(row["ProjectId"]);
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetSkillBasedTrainingsUserView", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public List<UserTrainingDetail> GetRoleBasedTrainingsUserView(int userid)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();

            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                ds = dh.ExecuteDataSet("[dbo].[proc_GetRoleBasedTrainingsUserView]", CommandType.StoredProcedure, new SqlParameter("@UserId", userid));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.UserId = Convert.ToInt32(row["UserID"]);
                        item.EmailAddress = row["EmailAddress"].ToString().ToUpper();
                        item.UserName = Convert.ToString(row["UserName"]);
                        item.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        item.TrainingName = Convert.ToString(row["TrainingName"].ToString().ToUpper());
                        item.IsTrainingCompleted = row["IsTrainingCompleted"] == DBNull.Value ? false : Convert.ToBoolean(row["IsTrainingCompleted"]);
                        item.IsLink = true;
                        item.LinkUrl = row["TrainingLink"].ToString();
                        if (row["CompletedDate"] != null)
                        {
                            item.CompletionDate = Convert.ToString(row["CompletedDate"]);
                        }
                        else
                        {
                            item.CompletionDate = "";
                        }

                        item.AdminApprovalStatus = Convert.ToString(row["AdminApprovalStatus"]);
                        if (row["ProjectId"] != null && !(row["ProjectId"] is DBNull))
                            item.ProjectId = Convert.ToInt32(row["ProjectId"]);
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoleBasedTrainingsUserView", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public bool TrainingCompletionRequest(UserTrainingDetail req)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhChecklist = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@TrainingId",SqlDbType.Int ) { Value = req.TrainingId},
                new SqlParameter("@UserId",SqlDbType.Int ) { Value = req.UserId},
                //new SqlParameter("@TrainingType",SqlDbType.Int ) { Value = req.TrainingType},
                new SqlParameter("@TrainingType",SqlDbType.NVarChar ) { Value = req.TrainingFlag},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output },
                new SqlParameter("@Progress",SqlDbType.NVarChar ) { Value = req.Progress}

            };
            try
            {
                count = dhChecklist.ExecuteNonQuery("[dbo].[proc_TrainingCompletionRequest]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[3].Value);
                if (status)
                {
                    result = true;

                    List<ProjectAdmin> projadmin = new List<ProjectAdmin>();
                    projadmin = GetProjectAdmin(req.ProjectId);
                    //foreach (ProjectAdmin item in projadmin)
                    //{
                    //    Hashtable hashtable = new Hashtable();
                    //    hashtable.Add("UserName", req.UserName);
                    //    hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    //    hashtable.Add("Training", req.TrainingName);
                    //    hashtable.Add("AdminName", item.Name);
                    //    bool ApprovalRequestemailsent = AddToEmailQueue("TrainingApprovalRequest", hashtable, item.EmailAddress, req.EmailAddress);
                    //}
                    string AdminUserEmailId = string.Empty;
                    foreach (ProjectAdmin item in projadmin)
                    {
                        AdminUserEmailId = AdminUserEmailId + item.EmailAddress + ";";
                    }

                    if (AdminUserEmailId != string.Empty || AdminUserEmailId != null)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("UserName", req.UserName);
                        hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                        hashtable.Add("Training", req.TrainingName);
                        hashtable.Add("AdminName", "ADMIN");
                        bool ApprovalRequestemailsent = AddToEmailQueue("TrainingApprovalRequest", hashtable, AdminUserEmailId, req.EmailAddress);
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoleBasedTrainingsAdminView", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dhChecklist != null)
                {
                    if (dhChecklist.DataConn != null)
                    {
                        dhChecklist.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public List<UserTrainingDetail> GetRoleBasedTrainingsAdminView(UserTrainingDetail trainingdetails)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();

            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                int Academyadminvalue = 0;
                int Projectadminvalue = 0;

                if (trainingdetails.IsAcademyAdmin)
                    Academyadminvalue = 1;
                if (trainingdetails.IsProjectAdmin)
                    Projectadminvalue = 1;

                SqlParameter[] parameters =
                {
                new SqlParameter("@ProjectAdminUserId",SqlDbType.Int) { Value = trainingdetails.UserId},
                new SqlParameter("@IsAcademyAdmin",SqlDbType.Bit ) { Value = Academyadminvalue},
                new SqlParameter("@IsProjectAdmin",SqlDbType.Bit ) { Value = Projectadminvalue},
                };

                ds = dh.ExecuteDataSet("[dbo].[proc_GetRoleBasedTrainingsAdminView]", CommandType.StoredProcedure, parameters);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.UserId = Convert.ToInt32(row["UserID"]);
                        item.EmailAddress = row["EmailAddress"].ToString().ToUpper();
                        item.UserName = Convert.ToString(row["UserName"]);
                        item.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        item.TrainingName = Convert.ToString(row["TrainingName"].ToString().ToUpper());
                        item.IsTrainingCompleted = row["IsTrainingCompleted"] == DBNull.Value ? false : Convert.ToBoolean(row["IsTrainingCompleted"]);

                        if (row["CompletedDate"] != null)
                        {
                            item.CompletionDate = Convert.ToString(row["CompletedDate"]);
                        }
                        else
                        {
                            item.CompletionDate = "";
                        }

                        item.AdminApprovalStatus = Convert.ToString(row["AdminApprovalStatus"]);
                        item.AdminName = Convert.ToString(row["AdminName"]);
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetRoleBasedTrainingsAdminView", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public List<UserTrainingDetail> GetSkillBasedTrainingsAdminView(UserTrainingDetail trainingdetails)
        {
            List<UserTrainingDetail> trainings = new List<UserTrainingDetail>();

            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                DataSet ds = new DataSet();
                DataView dv = new DataView();

                int Academyadminvalue = 0;
                int Projectadminvalue = 0;

                if (trainingdetails.IsAcademyAdmin)
                    Academyadminvalue = 1;
                if (trainingdetails.IsProjectAdmin)
                    Projectadminvalue = 1;

                SqlParameter[] parameters =
                {
                new SqlParameter("@ProjectAdminUserId",SqlDbType.Int) { Value = trainingdetails.UserId},
                new SqlParameter("@IsAcademyAdmin",SqlDbType.Bit ) { Value = Academyadminvalue},
                new SqlParameter("@IsProjectAdmin",SqlDbType.Bit ) { Value = Projectadminvalue},
                };


                ds = dh.ExecuteDataSet("proc_GetSkillBasedTrainingsAdminView", CommandType.StoredProcedure, parameters);

                //ds = dh.ExecuteDataSet("proc_GetSkillBasedTrainingsAdminView", CommandType.StoredProcedure);
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);

                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UserTrainingDetail item = new UserTrainingDetail();
                        item.UserId = Convert.ToInt32(row["UserID"]);
                        item.EmailAddress = row["EmailAddress"].ToString().ToUpper();
                        item.UserName = Convert.ToString(row["UserName"]);
                        item.TrainingId = Convert.ToInt32(row["TrainingId"]);
                        item.TrainingName = Convert.ToString(row["TrainingName"].ToString().ToUpper());
                        item.IsTrainingCompleted = row["IsTrainingCompleted"] == DBNull.Value ? false : Convert.ToBoolean(row["IsTrainingCompleted"]);

                        //if (row["IsTrainingCompleted"] != null)
                        //{
                        //    item.IsTrainingCompleted = Convert.ToBoolean(row["IsTrainingCompleted"]);
                        //}
                        //else
                        //{
                        //    item.IsTrainingCompleted = false;
                        //}

                        if (row["CompletedDate"] != null)
                        {
                            item.CompletionDate = Convert.ToString(row["CompletedDate"]);
                        }
                        else
                        {
                            item.CompletionDate = "";
                        }

                        item.AdminApprovalStatus = Convert.ToString(row["AdminApprovalStatus"]);
                        item.AdminName = Convert.ToString(row["AdminName"]);
                        trainings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetSkillBasedTrainingsAdminView", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return trainings;
        }

        public bool TrainingCompletionApproval(UserTrainingDetail req)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhTraining = new DataHelper(strConnectionString);

            SqlParameter[] parameters =
            {
                new SqlParameter("@TrainingId",SqlDbType.Int ) { Value = req.TrainingId},
                new SqlParameter("@UserId",SqlDbType.Int ) { Value = req.UserId},
                new SqlParameter("@TrainingType",SqlDbType.NVarChar ) { Value = req.TrainingFlag},
                new SqlParameter("@AdminApprovalStatus",SqlDbType.NVarChar ) { Value = req.AdminApprovalStatus},
                new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
            };
            try
            {
                count = dhTraining.ExecuteNonQuery("[dbo].[proc_TrainingCompletionApproval]", CommandType.StoredProcedure, parameters);
                status = Convert.ToBoolean(parameters[4].Value);
                if (status)
                {
                    result = true;

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("UserName", req.UserName);
                    hashtable.Add("ClientName", ConfigurationManager.AppSettings["ClientName"].ToString());
                    hashtable.Add("Training", req.TrainingName);
                    hashtable.Add("ApprovalStatus", req.AdminApprovalStatus);
                    if (req.AdminName != "")
                    {
                        hashtable.Add("AdminName", req.AdminName);
                    }
                    else
                    {
                        hashtable.Add("AdminName", "ADMIN");
                    }

                    bool ApprovalRequestemailsent = AddToEmailQueue("TrainingApproval", hashtable, req.EmailAddress, null);
                }

            }
            finally
            {
                if (dhTraining != null)
                {
                    if (dhTraining.DataConn != null)
                    {
                        dhTraining.DataConn.Close();
                    }
                }
            }

            return result;
        }

        public bool ChangeStatusOfAllTrainings(List<string> trainings, string approvalStatus)
        {
            bool result = false;
            bool status = false;
            int count = 0;
            DataHelper dhTraining = new DataHelper(strConnectionString);
            foreach (string req in trainings)
            {
                int id = Convert.ToInt32(req);
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId",SqlDbType.Int ) { Value = id},
                    new SqlParameter("@AdminApprovalStatus",SqlDbType.NVarChar ) { Value = approvalStatus},
                    new SqlParameter("@Status",SqlDbType.Bit){Value=status,Direction=ParameterDirection.Output }
                };

                try
                {
                    count = dhTraining.ExecuteNonQuery("[dbo].[proc_ChangeStatusofAllTrainings]", CommandType.StoredProcedure, parameters);
                    status = Convert.ToBoolean(parameters[4].Value);
                    if (status)
                    {
                        result = true;
                    }
                }

                catch (Exception ex)
                {
                    LogHelper.AddLog("SQLServerDAL,ChangeStatusOfAllTrainings", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
                }

                finally
                {
                    if (dhTraining != null)
                    {
                        if (dhTraining.DataConn != null)
                        {
                            dhTraining.DataConn.Close();
                        }
                    }
                }
            }

            return result;
        }

        public List<ProjectAdmin> GetAllProjectAdminInfo()
        {
            DataSet ds = new DataSet();
            List<ProjectAdmin> projAdminInfo = new List<ProjectAdmin>();
            DataHelper dh = new DataHelper(strConnectionString);
            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetAllProjectAdminInfo]", CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetAllProjectAdminInfo", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ProjectAdmin admin = new ProjectAdmin();
                        admin.EmailAddress = row["EmailAddress"].ToString();
                        admin.Name = row["Name"].ToString();
                        admin.ProjectId = Convert.ToInt32(row["ProjectId"].ToString());
                        admin.UserId = Convert.ToInt32(row["AdminId"].ToString());
                        projAdminInfo.Add(admin);
                    }
                }
            }
            return projAdminInfo;
        }
        public List<UserCheckList> GetUserChecklist(int userid)
        {
            DataHelper dh = new DataHelper(strConnectionString);
            List<UserCheckList> lstUserCheckList = new List<UserCheckList>();
            try
            {
                DataSet dsUser = new DataSet();
                DataView dvUser = new DataView();
                dsUser = dh.ExecuteDataSet("[dbo].[proc_GetUserCheckLists]", CommandType.StoredProcedure, new SqlParameter("@userId", userid));
                dvUser = new DataView(dsUser.Tables[0]);
                DataTable dtUsers = new DataTable();
                dtUsers = dvUser.ToTable();

                if (dtUsers != null && dtUsers.Rows.Count > 0)
                {
                    foreach (DataRow data in dtUsers.Rows)
                    {
                        UserCheckList item = new UserCheckList();
                        item.Id = Convert.ToInt32(data["ID"].ToString());
                        if (data["CheckList"] != null)
                        {
                            if (!String.IsNullOrEmpty(data["CheckList"].ToString()))
                                item.CheckList = data["CheckList"].ToString();
                        }
                        if (data["CheckListStatus"] != null)
                        {
                            if (!String.IsNullOrEmpty(data["CheckListStatus"].ToString()))
                                item.CheckListStatus = data["CheckListStatus"].ToString();
                        }
                        item.Title = data["Title"].ToString();
                        lstUserCheckList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,GetUserChecklist", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return lstUserCheckList;
        }
        public List<RoleSkill> GetAllRoleSkill(int roleid)
        {
            List<RoleSkill> roleSkills = new List<RoleSkill>();
            DataSet ds = new DataSet();
            DataView dv = new DataView();
            DataHelper dh = new DataHelper(strConnectionString);

            try
            {
                ds = dh.ExecuteDataSet("[dbo].[proc_GetRoleSkill]", CommandType.StoredProcedure, new SqlParameter("@roleId", roleid));
                if (ds.Tables.Count > 0)
                    dv = new DataView(ds.Tables[0]);
                DataTable dt = dv.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        RoleSkill item = new RoleSkill();
                        item.RoleId = Int32.Parse(row["RoleID"].ToString());
                        item.SkillId = Int32.Parse(row["SkillID"].ToString());
                        item.CompetencyLevelID = Int32.Parse(row["CompetencyLevelID"].ToString());
                        item.Role = row["Role"].ToString();
                        item.Skill = row["Skill"].ToString();
                        item.CompetencyLevel = row["CompetencyLevel"].ToString();
                        roleSkills.Add(item);
                    }
                }
            }
            finally
            {
                if (dh != null)
                {
                    if (dh.DataConn != null)
                    {
                        dh.DataConn.Close();
                    }
                }
            }
            return roleSkills;
        }
        public bool AddRoleSkill(int roleId, int skillId, int competencylevelId)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@roleId", roleId), new SqlParameter("@skillId", skillId),
                    new SqlParameter("@competencylevelId", competencylevelId),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_AddRoleSkill]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }
        public bool DeleteRoleSkill(int roleId, int skillId)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@roleId", roleId), new SqlParameter("@skillId", skillId),
                    new SqlParameter("@status", SqlDbType.Bit) { Direction=ParameterDirection.Output} };
                DataHelper dh = new DataHelper(strConnectionString);
                int count = dh.ExecuteNonQuery("[dbo].[proc_DeleteRoleSkill]", CommandType.StoredProcedure, parameters);
                if (dh.Cmd.Parameters["@status"].Value.ToString().ToUpper() == "TRUE")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("SQLServerDAL,AddRole", ex.Message, ex.StackTrace, "HCL.Academy.DAL", currentUser.emailId.ToString());
            }
            return result;
        }
    }
}