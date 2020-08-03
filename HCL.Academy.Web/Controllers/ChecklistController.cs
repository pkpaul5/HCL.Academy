using System;
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
    public class ChecklistController : BaseController
    {
        // GET: Checklist
        public async Task<ActionResult> Index()
        {
            //IDAL dal = (new DALFactory()).GetInstance();
            List<CheckListItem> checkLists = new List<CheckListItem>();
            //checkLists = dal.GetAllChecklist();
            InitializeServiceClient();
            HttpResponseMessage checklistResponse = await client.PostAsJsonAsync("Checklist/GetAllChecklist", req);
            checkLists = await checklistResponse.Content.ReadAsAsync<List<CheckListItem>>();
            if (checkLists != null)
                Session["Checklist"] = checkLists;
            return View(checkLists);
        }

        // GET: Checklist/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Checklist/Create
        public async Task<ActionResult> Create()
        {
            InitializeServiceClient();
          //  IDAL dal = (new DALFactory()).GetInstance();
            Checklist checklist = new Checklist();
            //  checklist.roles = dal.GetAllRoles();
            HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            checklist.roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
            //checklist.GEOs = dal.GetAllGEOs();
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
            checklist.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            return View(checklist);
        }

        // POST: Checklist/Create
        [HttpPost]
        public async Task<ActionResult> Create(Checklist newChecklist)
        {
            
            try
            {
                InitializeServiceClient();
                if (newChecklist.selectedGEO == null)
                {
                    ModelState.AddModelError("SelectedGEO", "Please select a GEO");
                    return View(newChecklist);
                }
                else if (newChecklist.selectedRole == null)
                {
                    ModelState.AddModelError("SelectedRole", "Please select a Role");
                    return View(newChecklist);
                }
                else if (ModelState.IsValid)
                {
                    bool result = false;
                    //string name = newChecklist.name;
                    //string geo = newChecklist.selectedGEO.ToString();
                    //string internalName = newChecklist.internalName;
                    //string description = newChecklist.desc;
                    //bool choice = newChecklist.choice;                    
                    //string role = newChecklist.selectedRole.ToString();
                    //IDAL dal = (new DALFactory()).GetInstance();
                    //result = dal.AddChecklist(name, geo, internalName, description, choice, role);

                    ChecklistRequest checkRequest = new ChecklistRequest();
                    checkRequest.ClientInfo = req.ClientInfo;
                    checkRequest.name = newChecklist.name;
                    checkRequest.selectedGEO = newChecklist.selectedGEO.ToString();
                    checkRequest.internalName = newChecklist.internalName;
                    checkRequest.desc = newChecklist.desc;
                    checkRequest.choice = newChecklist.choice;
                    checkRequest.selectedRole = newChecklist.selectedRole.ToString();
                    //IDAL dal = (new DALFactory()).GetInstance();
                    //result = dal.AddChecklist(name, geo, internalName, description, choice, role);

                    HttpResponseMessage addResponse = await client.PostAsJsonAsync("Checklist/AddChecklist", checkRequest);
                    result = await addResponse.Content.ReadAsAsync<bool>();

                    if (result)
                        return RedirectToAction("Index");
                    else
                        return View(newChecklist);
                }
                else
                    return View(newChecklist);
            }
            catch
            {
                return View();
            }
        }


        // GET: Checklist/Edit/5
        public async Task<ActionResult> Edit(int id, string checklist, string geo, string internalName, string description, bool textchoice, string role)
        {
            InitializeServiceClient();
            //IDAL dal = (new DALFactory()).GetInstance();
            Checklist editChecklist = new Checklist();
            //editChecklist.ClientInfo = req.ClientInfo;
            HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);            
            editChecklist.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
            //editChecklist.roles = dal.GetAllRoles();

            HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
            editChecklist.roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
            editChecklist.id = id;
            editChecklist.name = checklist;
            editChecklist.internalName = internalName;
            editChecklist.desc = description;
            editChecklist.choice = textchoice;
            return View(editChecklist);
        }

        // POST: Checklist/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Checklist editChecklist)
        {
            InitializeServiceClient();
            try
            {
                if (editChecklist.selectedGEO == null)
                {
                    ModelState.AddModelError("SelectedGEO", "Please select a GEO");
                    return View(editChecklist);
                }
                else if (editChecklist.selectedRole == null)
                {
                    ModelState.AddModelError("SelectedRole", "Please select a Role");
                    return View(editChecklist);
                }
                else if(ModelState.IsValid)
                {
                    bool result = false;
                    editChecklist.id = id;
                    string textchoice = String.Empty;
                    
                    //IDAL dal = (new DALFactory()).GetInstance();
                    //// editChecklist.GEOs = dal.GetAllGEOs();
                    HttpResponseMessage geoResponse = await client.PostAsJsonAsync("Geo/GetAllGEOs", req);
                    editChecklist.GEOs = await geoResponse.Content.ReadAsAsync<List<GEO>>();
                    //editChecklist.roles = dal.GetAllRoles();

                    HttpResponseMessage roleResponse = await client.PostAsJsonAsync("User/GetAllRoles", req);
                    editChecklist.roles = await roleResponse.Content.ReadAsAsync<List<Role>>();
                    ChecklistRequest checkRequest = new ChecklistRequest();
                    checkRequest.ClientInfo = req.ClientInfo;
                    checkRequest.id = id;
                    checkRequest.name = editChecklist.name;
                    checkRequest.selectedGEO = editChecklist.selectedGEO.ToString();
                    checkRequest.internalName = editChecklist.internalName;
                    checkRequest.desc = editChecklist.desc;
                    checkRequest.choice = editChecklist.choice;
                    checkRequest.selectedRole = editChecklist.selectedRole.ToString();
                    //result = dal.UpdateChecklist(id, checklist, geo, internalName, description, choice, role);
                    HttpResponseMessage editResponse = await client.PostAsJsonAsync("Checklist/UpdateChecklist", checkRequest);
                    result = await editResponse.Content.ReadAsAsync<bool>();
                    if (result)
                        return RedirectToAction("Index");
                    else
                        return View(editChecklist);

                }
                else
            {
                return View(editChecklist);
            }
        }
            catch
            {
                return View(editChecklist);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //Checklist deleteChecklist = new Checklist();
                InitializeServiceClient();
                bool result = false;
                HttpResponseMessage deleteResponse = await client.PostAsJsonAsync("Checklist/DeleteChecklist?itemId="+id, req);
                result = await deleteResponse.Content.ReadAsAsync<bool>();
                if (result)
                    return RedirectToAction("Index");
                else
                    return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
