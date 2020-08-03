using HCL.Academy.DAL;
using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This service exposes all the methods related to UserAssessment updation
    /// </summary>
    public class KaliberUserAssessmentController : ApiController
    {
        /// <summary>
        /// This service updates user scores in userassessment table.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("UpdateUserAssessment")]
        public List<KaliberUserAssessmentResults> UpdateUserAssessment(KaliberUserAssessmentCollectionRequest data)
        {
            List<KaliberUserAssessmentResults> Listresponse = new List<KaliberUserAssessmentResults>();
            try
            {
                SqlSvrDAL dal = new SqlSvrDAL(null);
                List<KaliberUserAssessmentRequest> Listrequest = data.kaliberuserassessmentrequest;

                //user email
                //Technology
                //Assessment
                //skill level
                foreach (KaliberUserAssessmentRequest assessment in Listrequest)
                {
                    KaliberUserAssessmentResults response = new KaliberUserAssessmentResults();
                    response = dal.KaliberUpdateUserAssessment(assessment);
                    Listresponse.Add(response);
                }

                


            }
            catch (Exception ex)
            {
                
            }

            return Listresponse;


        }

    }
}
