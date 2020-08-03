using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service exposes methods related to user
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class ExternalUserController : ApiController
    {
        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        /// <summary>
        /// This method returns all the Organizations
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllOrganizations")]
        public List<Organization> GetAllOrganizations(RequestBase req)
        {
            List<Organization> result = new List<Organization>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetAllOrganizations();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,GetAllOrganizations", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns all the User Groups
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllUserGroups")]
        public List<UserGroup> GetAllUserGroups(RequestBase req)
        {
            List<UserGroup> result = new List<UserGroup>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetAllUserGroups();
            }
            catch (Exception ex)
            {
                //         LogHelper.AddLog("ExternalUserController,GetAllUserGroups", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns all the External Users
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllExternalUsers")]
        public List<ExternalUser> GetAllExternalUsers(RequestBase req)
        {
            List<ExternalUser> result = new List<ExternalUser>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetAllExternalUsers();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,GetAllExternalUsers", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="UserId">user id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetExternalUserById")]
        public ExternalUser GetExternalUserById(RequestBase req, int UserId)
        {
            ExternalUser result = null;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetExternalUserById(UserId);
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("ExternalUserController,GetExternalUserById", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">Request base</param>            
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateExternalUserPasswordStatus")]
        public bool UpdateExternalUserPasswordStatus(RequestBase req)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.UpdateExternalUserPasswordStatus(req.ClientInfo.emailId);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,SaveExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">ExternalUser info</param>    
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("SaveExternalUser")]
        public string SaveExternalUser(ExternalUserRequest req)
        {
            string result = string.Empty;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.SaveExternalUser(req);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,SaveExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method sends password to external user
        /// </summary>
        /// <param name="req">ExternalUser info</param>    
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("SendExternalUserPassword")]
        public bool SendExternalUserPassword(ExternalUserRequest req)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.SendExternalUserPassword(req);
            }
            catch (Exception ex)
            {
                //        LogHelper.AddLog("ExternalUserController,SaveExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="UserName">UserName</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetExternalUserByUserName")]
        public ExternalUser GetExternalUserByUserName(RequestBase req, string UserName)
        {
            ExternalUser result = null;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetExternalUserByUserName(UserName);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,GetExternalUserByUserName", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// This method returns list of roles assigned to a user
        /// </summary>
        /// <param name="req">client info</param>
        /// <param name="UserName">UserName</param>
        /// <param name="Password">Password</param>
        /// <returns>blank if login is successful; otherwise it returns error message</returns>
        [Authorize]
        [HttpPost]
        [ActionName("AuthenticateExternalUser")]
        public ExternalUserAuthResponse AuthenticateExternalUser(RequestBase req, string UserName, string Password)
        {
            ExternalUserAuthResponse response = new ExternalUserAuthResponse();
            ExternalUser user = null;           
            bool comp = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                user = dal.GetExternalUserByUserName(UserName);
                string encryptedPassword = "";
                if (!string.IsNullOrEmpty(user.EncryptedPassword))
                {
                    //   encryptedPassword = CreateHash(Password, Encoding.ASCII.GetBytes(user.PasswordSalt));
                    encryptedPassword=PasswordHelper.EncodePassword(Password, user.PasswordSalt);
                }

                comp = (0 == string.Compare(user.EncryptedPassword, encryptedPassword, false));

                if(comp == false)
                {  
                    response.result = false;
                    response.errorMessage = "Userid & Password do not match";
                }
                else
                {
                    response.result = true;
                    response.user = user;                    
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.errorMessage = ex.Message;
                //       LogHelper.AddLog("ExternalUserController,AuthenticateExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }

        /// <summary>
        /// Request object
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("ResetExternalUserPassword")]
        public string ResetExternalUserPassword(ExternalUserRequest req)
        {
            string result = string.Empty;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);

                // string salt = GetCSPRNGSalt();
                // string encryptedPassword = CreateHash(NewPassword, Encoding.ASCII.GetBytes(salt));

                result = dal.ResetExternalUserPassword(req);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,ResetExternalUserPassword", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// Deletes external user
        /// </summary>
        /// <param name="req">Request base</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteExternalUser")]
        public bool DeleteExternalUser(RequestBase req,int id)
        {
            bool result = false;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.DeleteExternalUser(id);
            }
            catch (Exception ex)
            {
                //       LogHelper.AddLog("ExternalUserController,DeleteExternalUser", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }

        /// <summary>
        /// Get UserMemberShip
        /// </summary>
        /// <param name="req">ExternalUserRequest</param>        
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUserMemberShip")]
        public List<UserGroupMemberShip> GetUserMemberShip(ExternalUserRequest req)
        {
            List<UserGroupMemberShip> result = new List<UserGroupMemberShip>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                result = dal.GetUserGroupMemberShip(req);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("ExternalUserController,GetUserMemberShip", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return result;
        }
        //private string GetCSPRNGSalt()
        //{
        //    RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        //    byte[] tokenData = new byte[SALT_BYTE_SIZE];
        //    rng.GetBytes(tokenData);
        //    string token = Convert.ToBase64String(tokenData);
        //    return token;
        //}

        //private string CreateHash(string password, byte[] salt)
        //{
        //    // Hash the password and encode the parameters
        //    byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
        //    return PBKDF2_ITERATIONS + ":" +
        //        Convert.ToBase64String(salt) + ":" +
        //        Convert.ToBase64String(hash);
        //}

        ///// <summary>
        ///// Computes the PBKDF2-SHA1 hash of a password.
        ///// </summary>
        ///// <param name="password">The password to hash.</param>
        ///// <param name="salt">The salt.</param>
        ///// <param name="iterations">The PBKDF2 iteration count.</param>
        ///// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        ///// <returns>A hash of the password.</returns>
        //private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        //{
        //    Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
        //    pbkdf2.IterationCount = iterations;
        //    return pbkdf2.GetBytes(outputBytes);
        //}


    }
}
