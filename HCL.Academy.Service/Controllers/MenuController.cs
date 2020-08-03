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
    /// This service exposes all the methods related to the Menu
    /// </summary>
    //[EnableCors(origins: "https://hclacademyhubnew.azurewebsites.net", headers: "*", methods: "*")]
   
    public class MenuController : ApiController
    {
        /// <summary>
        /// This method fetches the menu based on user role and authentication.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetMenu")]
        public List<SiteMenu> GetMenu(RequestBase request,int roleid)
        {
            LogHelper.AddLog("MenuController,GetMenu", "In GetMenu method","Starting", "HCL.Academy.Service", request.ClientInfo.emailId);
            List<SiteMenu> response = new List<SiteMenu>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(request.ClientInfo);
                response = dal.GetMenu(roleid);
            }
            catch (Exception ex)
            {
                //LogHelper.AddLog("MenuController,GetMenu", ex.Message, ex.StackTrace, "HCL.Academy.Service",request.ClientInfo.emailId);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            return response;
        }
        ///// <summary>
        ///// This method fetches all the banners.
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ActionName("GetBanners")]
        //public List<Banners> GetBanners(RequestBase request)
        //{
        //    List<Banners> response = new List<Banners>();
        //    try
        //    {
        //        SharePointDAL dal = new SharePointDAL(request.ClientInfo);
        //        //IDAL dal = (new DALFactory(request.ClientInfo)).GetInstance();
        //        response = dal.GetBanners();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.AddLog("MenuController,GetBanners", ex.Message, ex.StackTrace, "HCL.Academy.Service", request.ClientInfo.emailId);
        //    }
        //    return response;
        //}
    }
}
