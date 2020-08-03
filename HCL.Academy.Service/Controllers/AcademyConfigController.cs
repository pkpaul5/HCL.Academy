using HCL.Academy.DAL;
using HCL.Academy.Model;
using HCLAcademy.Util;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service provides all the AcademyConfig related functionality in HCL Academy
    /// </summary>
    public class AcademyConfigController : ApiController
    {
        /// <summary>
        /// This method returns all the AcademyConfigs that exist in HCL Academy database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllAcademyConfig")]
        public List<AcademyConfig> GetAllAcademyConfig(RequestBase req)
        {
            List<AcademyConfig> lstAcademyConfig = new List<AcademyConfig>();
            try
            {
                ///  IDAL dal = (new DALFactory(req.ClientInfo)).GetInstance();
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                lstAcademyConfig = dal.GetAllAcademyConfig();

            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
            }
            return lstAcademyConfig;
        }

        /// <summary>
        /// This method returns AcademyConfig record for an ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAcademyConfigById")]
        public AcademyConfig GetAcademyConfigById(int id, RequestBase req)
        {
            AcademyConfig academyconfig = null;
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                academyconfig = dal.GetAcademyConfigById(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }

            return academyconfig;
        }

        /// <summary>
        /// This method adds a new AcademyConfig to HCL Academy Database
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Value"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("AddAcademyConfig")]
        public void AddAcademyConfig(string Title,string Value, RequestBase req)
        {
            AcademyConfig academyconfig = new AcademyConfig();
            try
            {
                academyconfig.Title = Title;
                academyconfig.Value = Value;
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.AddAcademyConfig(academyconfig);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// This method updates an existing AcademyConfig
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Title"></param>
        /// <param name="Value"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateAcademyConfig")]
        public void UpdateAcademyConfig(int Id,string Title,string Value, RequestBase req)
        {
            try
            {
                AcademyConfig academyConfig = new AcademyConfig();
                academyConfig.ID = Id;
                academyConfig.Title = Title;
                academyConfig.Value = Value;
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.UpdateAcademyConfig(academyConfig);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }            
        }

        /// <summary>
        /// This method deletes an AcademyConfig from HCL Academy Database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        [Authorize]
        [HttpPost]
        [ActionName("DeleteAcademyConfig")]
        public void DeleteAcademyConfig(int id, RequestBase req)
        {
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(req.ClientInfo);
                dal.DeleteAcademyConfig(id);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("AcademyConfigController", ex.Message, ex.StackTrace, "HCL.Academy.Service", req.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
        }


    }
}