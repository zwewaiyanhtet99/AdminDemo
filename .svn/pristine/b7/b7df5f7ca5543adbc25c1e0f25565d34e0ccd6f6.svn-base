using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class Ver_sionController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "Ver_sionController";
        EventLogController log = new EventLogController();
        // GET: Ver_sion
        public ActionResult Index()
        {
            return View();
        }
        
        //Post: Version Index
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
            //find search columns info
            var townshipCode = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Code = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var version = (from v in db.Versions
                            select v);

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                version = version.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = version.Count();
            var data = version.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Ver_sion/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ver_sion ver_sion = db.Versions.Find(id);
                if (ver_sion == null)
                {
                    return HttpNotFound();
                }
                return View(ver_sion);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Ver_sion/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Ver_sion ver = new Ver_sion();
                ver.UserType = "MB";
                PlatformTypeDDL();
                UpdateStatusDDL();
                return View(ver);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Ver_sion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VersionNo,PlatForm,UserType,VersionName,UpdatedStatus")] Ver_sion ver_sion)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    ver_sion.CreatedDate = DateTime.Now;
                    ver_sion.CreatedUserId = User.Identity.GetUserId();
                    db.Versions.Add(ver_sion);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, ver_sion.ToString());
                    return RedirectToAction("Index");
                }

                return View(ver_sion);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Ver_sion/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                PlatformTypeDDL();
                UpdateStatusDDL();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ver_sion ver_sion = db.Versions.Find(id);
                if (ver_sion == null)
                {
                    return HttpNotFound();
                }
                return View(ver_sion);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Ver_sion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,VersionNo,PlatForm,UserType,VersionName,UpdatedStatus")] Ver_sion ver_sion)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //get data from db
                    Ver_sion db_ver_sion = db.Versions.Find(ver_sion.ID);
                    if (db_ver_sion == null)
                    {
                        return HttpNotFound();
                    }
                    //update field what u want to be allowed to edit  (from formdata)
                    db_ver_sion.VersionNo = ver_sion.VersionNo;
                    db_ver_sion.PlatForm = ver_sion.PlatForm;
                    db_ver_sion.UserType = ver_sion.UserType;
                    db_ver_sion.VersionName = ver_sion.VersionName;
                    db_ver_sion.UpdatedStatus = ver_sion.UpdatedStatus;
                    db_ver_sion.UpdatedDate = DateTime.Now;
                    db_ver_sion.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(db_ver_sion).State = EntityState.Modified;
                    db.SaveChanges();
                    // save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(ver_sion);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Ver_sion/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ver_sion ver_sion = db.Versions.Find(id);
                if (ver_sion == null)
                {
                    return HttpNotFound();
                }
                return View(ver_sion);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Ver_sion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
               
                Ver_sion ver_sion = db.Versions.Find(id);
                db.Versions.Remove(ver_sion);
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

        //get Platform type
        private void PlatformTypeDDL()
        {
            //create platform type list
            ViewBag.PlatformType = new List<SelectListItem>()
            {
                new SelectListItem{Text="ios",Value="ios"},
                new SelectListItem{Text="android",Value="android"}
            };
        }

        //get Update Status
        private void UpdateStatusDDL()
        {
            //create update status lidt
            ViewBag.UpdateStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Normal",Value="Normal"},
                new SelectListItem{Text="Force Change",Value="Force"}
            };
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
