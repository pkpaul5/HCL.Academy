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
    /// This service exposes all the methods related to GEOs.
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
    public class GeoController : ApiController
    {
        /// <summary>
        /// Returns a list of all GEOs.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetAllGEOs")]
        public List<GEO> GetAllGEOs(RequestBase request)
        {
            List<GEO> response = new List<GEO>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetAllGEOs();
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("GEOController", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);                
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
    }
}
