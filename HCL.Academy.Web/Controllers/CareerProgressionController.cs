using HCL.Academy.Model;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class CareerProgressionController : BaseController
    {
        /// <summary>
        /// Get the skills of the logged in user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [SessionExpire]
        [OutputCache(Duration = 600, VaryByCustom = "User", VaryByParam = "", Location = OutputCacheLocation.Server, NoStore = false)]
        public async Task<ActionResult> Career()
        {
            // IDAL dal = (new DALFactory()).GetInstance();
            //List<UserSkill> lstSkills = dal.GetUserSkillsOfCurrentUser();            
            InitializeServiceClient();
            HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetUserSkillsOfCurrentUser", req);
            List<UserSkill> lstSkills = await skillResponse.Content.ReadAsAsync<List<UserSkill>>();

            return View(lstSkills);
        }
    }
}