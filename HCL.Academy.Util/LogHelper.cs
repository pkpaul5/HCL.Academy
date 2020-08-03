using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
namespace HCLAcademy.Util
{
    public class LogHelper
    {
        /// <summary>
        /// This method adds any exception,information,warning to Azure storage
        /// </summary>
        /// <param name="e">Logentity</param>
        //public static void AddLog(LogEntity e)
        //{
        //    string flag = ConfigurationManager.AppSettings["LoggingEnabled"].ToString().ToUpper();
        //    if (flag == "YES")
        //    {
        //        AzureStorageTableOperations tblOps = new AzureStorageTableOperations(StorageOperations.Logging);
        //        tblOps.AddEntity(e);
        //    }
        //}
        public static void AddLog(string StrSource, string StrMessege, string StrDetailMessege, string StrApplicationName, string StrUserName)
        {
            string strConnectionString = "";// ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
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
            DataHelper d = new DataHelper(strConnectionString);
            string flag = ConfigurationManager.AppSettings["LoggingEnabled"].ToString().ToUpper();
            bool result = false;
            DataHelper dhelper = d;
            try
            {

                //if (flag == "YES")
                //{

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@Source",SqlDbType.NVarChar),
                        new SqlParameter("@Message",SqlDbType.NVarChar),
                        new SqlParameter("@DetailMessage",SqlDbType.NVarChar),
                        new SqlParameter("@ApplicationName",SqlDbType.NVarChar),
                        new SqlParameter("@UserName",SqlDbType.NVarChar),
                        new SqlParameter("@errorMessage",SqlDbType.NVarChar),
                    };
                parameters[0].Value = Convert.ToString(StrSource);
                parameters[1].Value = Convert.ToString(StrMessege);
                parameters[2].Value = Convert.ToString(StrDetailMessege);
                parameters[3].Value = Convert.ToString(StrApplicationName);
                parameters[4].Value = Convert.ToString(StrUserName);

                parameters[5].Size = 4000;
                parameters[5].Direction = ParameterDirection.Output;
                dhelper.ExecuteNonQuery("[dbo].[proc_AddLogs]", CommandType.StoredProcedure, parameters);

                if (dhelper.Cmd != null && dhelper.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;

                }
                else
                {
                    result = true;
                }


                // }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dhelper != null)
                {
                    if (dhelper.DataConn != null)
                    {
                        dhelper.DataConn.Close();
                    }
                }
            }
            //return result;

        }
        public static void AddLog(DataHelper d, string StrSource, string StrMessege, string StrDetailMessege, string StrApplicationName, string StrUserName)
        {
            string flag = ConfigurationManager.AppSettings["LoggingEnabled"].ToString().ToUpper();
            bool result = false;
            DataHelper dhelper = d;
            try
            {

                //if (flag == "YES")
                //{

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@Source",SqlDbType.NVarChar),
                        new SqlParameter("@Message",SqlDbType.NVarChar),
                        new SqlParameter("@DetailMessage",SqlDbType.NVarChar),
                        new SqlParameter("@ApplicationName",SqlDbType.NVarChar),
                        new SqlParameter("@UserName",SqlDbType.NVarChar),
                        new SqlParameter("@errorMessage",SqlDbType.NVarChar),
                    };
                parameters[0].Value = Convert.ToString(StrSource);
                parameters[1].Value = Convert.ToString(StrMessege);
                parameters[2].Value = Convert.ToString(StrDetailMessege);
                parameters[3].Value = Convert.ToString(StrApplicationName);
                parameters[4].Value = Convert.ToString(StrUserName);

                parameters[5].Size = 4000;
                parameters[5].Direction = ParameterDirection.Output;
                dhelper.ExecuteNonQuery("[dbo].[proc_AddLogs]", CommandType.StoredProcedure, parameters);

                if (dhelper.Cmd != null && dhelper.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;

                }
                else
                {
                    result = true;
                }


                // }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dhelper != null)
                {
                    if (dhelper.DataConn != null)
                    {
                        dhelper.DataConn.Close();
                    }
                }
            }
            //return result;

        }
        public static void AddApiRequestLog(DataHelper d, string JsonString, bool status)
        {
            //string flag = ConfigurationManager.AppSettings["LoggingEnabled"].ToString().ToUpper();
            bool result = false;
            DataHelper dhelper = d;
            try
            {

                //if (flag == "YES")
                //{

                SqlParameter[] parameters =
                    {
                        new SqlParameter("@JsonRequest",SqlDbType.NVarChar),
                        new SqlParameter("@Status",SqlDbType.Bit),
                        new SqlParameter("@errorMessage",SqlDbType.NVarChar),
                    };
                parameters[0].Value = Convert.ToString(JsonString);
                if (status)
                {
                    parameters[1].Value = 1;
                }
                else
                {
                    parameters[1].Value = 0;
                }

                parameters[2].Size = 4000;
                parameters[2].Direction = ParameterDirection.Output;
                dhelper.ExecuteNonQuery("[dbo].[proc_AddApiRequestLogs]", CommandType.StoredProcedure, parameters);

                if (dhelper.Cmd != null && dhelper.Cmd.Parameters["@ErrorMessage"].Value != DBNull.Value)
                {
                    result = false;

                }
                else
                {
                    result = true;
                }


                // }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dhelper != null)
                {
                    if (dhelper.DataConn != null)
                    {
                        dhelper.DataConn.Close();
                    }
                }
            }
            //return result;

        }
    }
}