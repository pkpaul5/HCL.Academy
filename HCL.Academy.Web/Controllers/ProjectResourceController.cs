using HCL.Academy.Model;
using System;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ProjectResourceController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> ResourceDetails(int projectID)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //Resource prjRes = dal.GetResourceDetailsByProjectID(projectID);     //Get the resource details of all Projects

                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = projectID;
                userProjectInfo.ClientInfo = req.ClientInfo;
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Project/GetResourceDetailsByProjectID", userProjectInfo);
                Resource prjRes = await trainingResponse.Content.ReadAsAsync<Resource>();

                return View(prjRes);
            }
            catch (Exception ex)
            {
                //UserManager user = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ProjectResourceController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return View();
            }
        }
    }
}
