using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class ACC_ServiceController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ACC_ServiceController";
        EventLogController log = new EventLogController();

        // GET: ACC_Service
        public ActionResult Index()
        {
            return View();
        }

        //Index Post
        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
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

            var acc_services = (from b in db.ACC_Services
                          where b.Del_Flag == false
                          select b);
            
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                acc_services = acc_services.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = acc_services.Count();
            var data = acc_services.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: ACC_Service/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ACC_Service aCC_Service = db.ACC_Service.Find(id);
        //    if (aCC_Service == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aCC_Service);
        //}

        // GET: ACC_Service/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: ACC_Service/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Account_Type,Acc_Description,Language")] ACC_Service aCC_Service)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    aCC_Service.CreatedDate = DateTime.Now;
                    aCC_Service.CreatedUserId = User.Identity.GetUserId();
                    aCC_Service.Del_Flag = false;
                    db.ACC_Services.Add(aCC_Service);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1);
                    return RedirectToAction("Index");
                }

                return View(aCC_Service);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
           
        }

        // GET: ACC_Service/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ACC_Service aCC_Service = db.ACC_Services.Find(id);
                if (aCC_Service == null)
                {
                    return HttpNotFound();
                }
                return View(aCC_Service);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
           
        }

        // POST: ACC_Service/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Account_Type,CreatedDate,CreatedUserId,UpdatedDate,UpdatedUserID,Del_Flag,Acc_Description,Language")] ACC_Service aCC_Service)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    ACC_Service acc_data = db.ACC_Services.Find(aCC_Service.ID);
                    acc_data.Account_Type = aCC_Service.Account_Type;
                    acc_data.Acc_Description = aCC_Service.Acc_Description;
                    acc_data.Language = aCC_Service.Language;
                    acc_data.UpdatedDate = DateTime.Now;
                    acc_data.UpdatedUserID = User.Identity.GetUserId();
                    db.Entry(acc_data).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(aCC_Service);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
            
        }

        // GET: ACC_Service/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ACC_Service aCC_Service = db.ACC_Services.Find(id);
                if (aCC_Service == null)
                {
                    return HttpNotFound();
                }
                return View(aCC_Service);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }

        }

        // POST: ACC_Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ACC_Service aCC_Service = db.ACC_Services.Find(id);
                aCC_Service.Del_Flag = true;
                aCC_Service.UpdatedDate = DateTime.Now;
                aCC_Service.UpdatedUserID = User.Identity.GetUserId();
                //db.ACC_Service.Remove(aCC_Service);
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid,controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
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
