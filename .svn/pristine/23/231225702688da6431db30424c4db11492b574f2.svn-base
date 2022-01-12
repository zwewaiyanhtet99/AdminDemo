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
using PagedList;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class Office_AccountController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "Office_AccountController";
        EventLogController log = new EventLogController();

        // GET: Office_Account
        public ActionResult Index()
        {
            
         int count = db.Office_Account.OrderBy(e => e.ID).Count();

         return View();
                     
           //return View(db.Office_Account.ToList());
        }
        //Post:Office_Account
        [HttpPost]
        public ActionResult Index(int? arg1) //adding parameter is in order to use Action name 'Index' same with View action name
        {
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
            var Code = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            //var city = db.Cities.ToList();
            var offAccount = (from s in db.Office_Account
                        where s.DEL_FLAG == false
                        select s);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                offAccount = offAccount.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = offAccount.Count();
            var data = offAccount.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Office_Account/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Office_Account office_Account = db.Office_Account.Find(id);
        //    if (office_Account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(office_Account);
        //}

        //GET: Office_Account/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //POST: Office_Account/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,Code,AcctNo,Description")] Office_Account office_Account)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            office_Account.CreatedDate = DateTime.Now;
        //            office_Account.CreatedUserId = User.Identity.GetUserId();
        //            office_Account.DEL_FLAG = false;
        //            db.Office_Account.Add(office_Account);
        //            db.SaveChanges();
        //            //save log
        //            string getuserid = User.Identity.GetUserId();
        //            log.Info(getuserid, controllerName, actionName, 1);
        //            return RedirectToAction("Index");
        //        }

        //        return View(office_Account);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //    //return View(office_Account);
        //}

        // GET: Office_Account/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Office_Account office_Account = db.Office_Account.Find(id);
                if (office_Account == null)
                {
                    return HttpNotFound();
                }
                return View(office_Account);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            } 
        }

        // POST: Office_Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,AcctNo,Description")] Office_Account office_Account)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Office_Account off_acct = db.Office_Account.Find(office_Account.ID);
                    //for string concating office account old data
                    string olddata = off_acct.ID + "," + off_acct.Code + "," + off_acct.AcctNo + "," + off_acct.Description + "," + off_acct.CreatedDate + "," + off_acct.CreatedUserId + "," + off_acct.UpdatedDate + "," + off_acct.UpdatedUserId + ","+ off_acct.DEL_FLAG;
                    off_acct.AcctNo = office_Account.AcctNo;
                    off_acct.Description = office_Account.Description;
                    off_acct.UpdatedDate = DateTime.Now;
                    off_acct.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(off_acct).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, olddata);
                    return RedirectToAction("Index");
                }
                return View(office_Account);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(office_Account);
        }

        // GET: Office_Account/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Office_Account office_Account = db.Office_Account.Find(id);
        //    if (office_Account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(office_Account);
        //}

        // POST: Office_Account/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Office_Account office_Account = db.Office_Account.Find(id);
        //    db.Office_Account.Remove(office_Account);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
