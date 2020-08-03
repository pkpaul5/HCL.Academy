using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCLAcademy.Controllers;
using System.Net;
using System.Net.Http;
using HCL.Academy.Model;
using System.Globalization;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace HCL.Academy.Web.Controllers
{
    public class AcademyEventsController : BaseController
    {
        // GET: AcademyEvents
        public async Task<ActionResult> Index()
        {
            List<AcademyEvent> list = new List<AcademyEvent>();
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Events/GetEvents", req);
            list =await response.Content.ReadAsAsync<List<AcademyEvent>>();
            return View("Index",list);
        }

        // GET: AcademyEvents/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AcademyEvents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademyEvents/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                InitializeServiceClient();
                EventRequest eventreq = new EventRequest();                
                eventreq.ClientInfo = req.ClientInfo;
                eventreq.eventinfo = new AcademyEvent();
                eventreq.eventinfo.id = 0;
                eventreq.eventinfo.title = collection["title"];
                eventreq.eventinfo.description = collection["description"];
                eventreq.eventinfo.location = collection["location"];
                eventreq.eventinfo.eventDate = Convert.ToDateTime(collection["eventDate"].ToString()); 
                eventreq.eventinfo.endDate = Convert.ToDateTime(collection["endDate"].ToString()); 

                HttpResponseMessage response = await client.PostAsJsonAsync("Events/SaveEvent", eventreq);
                bool result = await response.Content.ReadAsAsync<bool>();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();                
                telemetry.TrackException(ex);
                return View();
            }
        }

        // GET: AcademyEvents/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            List<AcademyEvent> list = new List<AcademyEvent>();
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Events/GetEvents", req);
            list = await response.Content.ReadAsAsync<List<AcademyEvent>>();
            AcademyEvent ae = list.Find(x => x.id == id);
            return View(ae);
        }

        // POST: AcademyEvents/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            try
            {
                InitializeServiceClient();
                EventRequest eventreq = new EventRequest();
                eventreq.ClientInfo = req.ClientInfo;
                eventreq.eventinfo = new AcademyEvent();
                eventreq.eventinfo.id = id;
                eventreq.eventinfo.title = collection["title"];
                eventreq.eventinfo.description = collection["description"];
                eventreq.eventinfo.location = collection["location"];
                eventreq.eventinfo.eventDate = Convert.ToDateTime(collection["eventDate"].ToString());
                eventreq.eventinfo.endDate = Convert.ToDateTime(collection["endDate"].ToString());

                HttpResponseMessage response = await client.PostAsJsonAsync("Events/SaveEvent", eventreq);
                bool result = await response.Content.ReadAsAsync<bool>();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }

        // GET: AcademyEvents/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            InitializeServiceClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("Events/DeleteEvent?id="+id, req);
            bool result = await response.Content.ReadAsAsync<bool>();
            return RedirectToAction("Index");
        }

        // POST: AcademyEvents/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                return View();
            }
        }
    }
}
