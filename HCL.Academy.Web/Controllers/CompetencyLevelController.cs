
using System.Collections.Generic;
using System.Web.Mvc;
using HCL.Academy.Model;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class CompetencyLevelController : BaseController
    {
        // GET: CompetencyLevel
        [Authorize]
        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();
            List<Competence> competence = new List<Competence>();
           // SqlSvrDAL dal = new SqlSvrDAL();
            HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
            competence = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
            //competence = dal.GetAllCompetencyLevels();
            Session["CompetencyLevels"] = competence;
            return View(competence);
        }

        //// GET: CompetencyLevel/Create
        //[Authorize]
        //[SessionExpire]
        //public async Task<ActionResult> Create()
        //{
        //    InitializeServiceClient();
        //    Competence competence = new Competence();
        //    List<Competence> allCompetences = new List<Competence>();
        //   // SqlSvrDAL dal = new SqlSvrDAL();
        //    HttpResponseMessage competencyResponse = await client.PostAsJsonAsync("Competency/GetAllCompetencyLevels", req);
        //    allCompetences = await competencyResponse.Content.ReadAsAsync<List<Competence>>();
        //    //allCompetences = dal.GetAllCompetencyLevels();
        //    Session["CompetencyLevels"] = allCompetences;
        //    return View(competence);
        //}

        //// POST: CompetencyLevel/Create
        //[Authorize]
        //[SessionExpire]
        //[HttpPost]
        //public ActionResult Create(Competence competences)
        //{
        //    try
        //    {
        //        InitializeServiceClient();
        //        Competence newCompetence = new Competence();
        //        IDAL dal = (new DALFactory()).GetInstance();
        //        bool result = false;
        //        dal.AddCompetencyLevels(competences.CompetenceName);
                
        //        if (result)
        //            return RedirectToAction("Create", newCompetence);
        //        else
        //            return View(competences);
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: CompetencyLevel/Edit/5
        //[Authorize]
        //[SessionExpire]
        //public ActionResult Edit(int id,string level)
        //{
        //    Competence competence = new Competence();
        //    competence.CompetenceId = id;
        //    competence.CompetenceName = level;
        //    return View(competence);
        //}

        //// POST: CompetencyLevel/Edit/5
        //[Authorize]
        //[SessionExpire]
        //[HttpPost]
        //public ActionResult Edit(int id, Competence collection)
        //{
        //    try
        //    {
        //        SqlSvrDAL dal = new SqlSvrDAL();
        //        bool result = false;
        //        result = dal.EditCompetenceByID(id,collection.CompetenceName);
        //        return RedirectToAction("Create",result);
        //    }
        //    catch(Exception ex)
        //    {
        //        return View();
        //    }
        //}
        //[Authorize]
        //[SessionExpire]
        ////// GET: CompetencyLevel/Delete/5
        //public ActionResult Delete(int id,string level)
        //{
        //    try
        //    {
        //        IDAL dal = (new DALFactory()).GetInstance();
        //        Competence competence = new Competence();
        //        competence.CompetenceId = id;
        //        competence.CompetenceName = level;
        //        bool result=dal.DeleteCompetencyLevel(id,level);
        //        if(result)
        //        return RedirectToAction("Create", new Competence());
        //        else
        //            return PartialView(competence);
        //    }
        //    catch (Exception ex)
        //    {
        //        UserManager user = (UserManager)Session["CurrentUser"];                
        //        return View(new Competence());
        //    }
        //}

    }
}
