
using System.Collections.Generic;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using HCL.Academy.Model;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCL.Academy.Web.Controllers
{
    public class FirstLevelProjectController : BaseController
    {
        // GET: FirstLevelProject
        public async Task<ActionResult> Index()
        {
            InitializeServiceClient();

            UserManager user = (UserManager)Session["CurrentUser"];
            if (user.GroupPermission > 2)
                ViewBag.IsAcademyAdmin = true;
            else
                ViewBag.IsAcademyAdmin = false;
            if (user.Admininfo.IsFirstLevelAdmin)
                ViewBag.IsFirstLevelAdmin = true;
            else
                ViewBag.IsFirstLevelAdmin = false;

            HttpResponseMessage userResponse = await client.PostAsJsonAsync("User/GetUsers", req);
            ViewBag.Users = await userResponse.Content.ReadAsAsync<List<Users>>();
            HttpResponseMessage adminResponse = await client.PostAsJsonAsync("Project/GetProjectAdmin?projectid=0", req);
            List<ProjectAdmin> admins = await adminResponse.Content.ReadAsAsync<List<ProjectAdmin>>();
            Project accountlevelproject = new Project();
            accountlevelproject.projectAdmins = admins;
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetProjectByParent?projectid=0", req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            accountlevelproject.childProjects = new List<Project>();
            if (user.GroupPermission > 2 || user.Admininfo.IsFirstLevelAdmin)
                accountlevelproject.childProjects = projects;
            else if (user.Admininfo.IsSecondLevelAdmin)
            {
              
                foreach (Project p in projects)
                {
                    
                    if (user.Admininfo.SecondLevelProjects.Contains(p.id))
                    {
                        accountlevelproject.childProjects.Add(p);
                    }
                }
            }
            else if (user.Admininfo.IsThirdLevelAdmin)
            {
              
                foreach (Project p in projects)
                {
                    ProjectInfo selectedProject = user.Admininfo.ThirdLevelProjects.Find(x => x.ParentProjectId == p.id);
                    if (selectedProject != null)
                    {
                        accountlevelproject.childProjects.Add(p);
                    }
                }
            }
            return View(accountlevelproject);
        }
        [HttpPost]
        [Authorize]
        [SessionExpire]
        public async Task<PartialViewResult> GetChildProjects(int parentProjectId)
        {
            InitializeServiceClient();
            UserManager user = (UserManager)Session["CurrentUser"];
            HttpResponseMessage projectResponse = await client.PostAsJsonAsync("Project/GetProjectByParent?projectid=" + parentProjectId.ToString(), req);
            List<Project> projects = await projectResponse.Content.ReadAsAsync<List<Project>>();
            List<Project> childProjects = new List<Project>();
            if (user.GroupPermission > 2 || user.Admininfo.IsFirstLevelAdmin)
                childProjects = projects;
            else if (user.Admininfo.IsSecondLevelAdmin)
            {   
                foreach (Project p in projects)
                {
                    if (user.Admininfo.SecondLevelProjects.Contains(p.id))
                    {
                        childProjects.Add(p);
                    }
                }
            }
            else if (user.Admininfo.IsThirdLevelAdmin)
            {                
                foreach (Project p in projects)
                {
                    ProjectInfo selectedProject = user.Admininfo.ThirdLevelProjects.Find(x => x.ParentProjectId == p.id);
                    if (selectedProject != null)
                    {
                        childProjects.Add(p);
                    }
                }
            }

            return PartialView("ChildProjects", childProjects);
        }
        [HttpPost]
        [Authorize]
        [SessionExpire]
        public async Task<bool> AddProject(string projectname)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProjectDetails?name=" + projectname + "&parentprojectid=0&projectlevel=1", req);
            return true;

        }
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<bool> AddAdmin(int userid)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/AddProjectAdmin?userid=" + userid + "&projectid=0", req);
            return true;
        }
        // GET: FirstLevelProject/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FirstLevelProject/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FirstLevelProject/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FirstLevelProject/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FirstLevelProject/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FirstLevelProject/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Project/DeleteProjectAdmin?projectid=0&userid=" + id, req);
            return RedirectToAction("Index", "FirstLevelProject");
        }

        // POST: FirstLevelProject/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
