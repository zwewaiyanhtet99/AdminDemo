using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;

namespace ABankAdmin.Controllers
{
    public class ServiceManagementController : Controller
    {
        private AdminDBContext db = new AdminDBContext(); 
        string controllerName = "ServiceManagementController";
        EventLogController log = new EventLogController();

        // GET: ServiceManagement
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            db.ServiceManagements.ToList(); //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();           

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            var service = (from s in db.ServiceManagements
                           select s);

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                service = service.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = service.Count();
            var data = service.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }   
  
        // GET: ServiceManagement/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ServiceManagement serviceManagement = db.ServiceManagements.Find(id);
                if (serviceManagement == null)
                {
                    return HttpNotFound();
                }
                @ViewBag.ServiceName = serviceManagement.SERVICENAME;
                return View(serviceManagement);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ServiceManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SERVICENAME,TimeFormat,Active")] ServiceManagement serviceManagement)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = from s in db.ServiceManagements
                                 select s;
                    var service = result.SingleOrDefault(m => m.ID == serviceManagement.ID);
                    if (service.SERVICENAME.Equals("CRONLOG"))
                    {
                        service.Active = serviceManagement.Active;
                    }
                    //else if (service.SERVICENAME.Equals("SCHEDULETRANSFER"))
                    //{
                    //    int a;
                    //    service.TimeFormat = serviceManagement.TimeFormat;
                    //    if (service.TimeFormat.All(char.IsDigit))
                    //    {
                    //        a = Convert.ToInt32(service.TimeFormat);
                    //        if (a < 0 || a > 23)
                    //        {
                    //            ModelState.AddModelError("TimeFormat", "Please enter a value less than or equal to 23.");
                    //            return View(serviceManagement);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ModelState.AddModelError("TimeFormat", "Please enter a valid number.");
                    //        return View(serviceManagement);
                    //    }
                    //}
                    service.SERVICENAME = serviceManagement.SERVICENAME;
                    service.TimeFormat = serviceManagement.TimeFormat;
                    service.UpdatedDate = DateTime.Now;
                    service.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(service).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(serviceManagement);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
