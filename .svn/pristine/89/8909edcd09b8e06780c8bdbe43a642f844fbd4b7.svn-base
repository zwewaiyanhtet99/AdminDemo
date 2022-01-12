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
    public class SMS_BodyController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "SMS_BodyController";
        EventLogController log = new EventLogController();

        // GET: SMS_Body
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.SMS_Body.ToList());
        }

        //Post/SMS_Body/Index
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
            var trantype = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var debmsg = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var crdmsg = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var smsbody = (from s in db.SMS_Bodys
                            where s.Active == true
                            select s);

            //foreach (var c in utilities.ToList())
            //{
            //    var rule = db.Rules.Where(t => t.Code == c.DiscountCode).FirstOrDefault();
            //    c.DiscountCode = rule == null ? "" : rule.Description;

            //}
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                smsbody = smsbody.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = smsbody.Count();
            var data = smsbody.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: SMS_Body/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SMS_Body sMS_Body = db.SMS_Bodys.Find(id);
                if (sMS_Body == null)
                {
                    return HttpNotFound();
                }
                return View(sMS_Body);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: SMS_Body/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: SMS_Body/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,TransactionType,DebitMessage,CreditMessage,CreatedUserId,CreatedDate,UpdatedUserId,UpdatedDate,Active")] SMS_Body sMS_Body)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.SMS_Bodys.Add(sMS_Body);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(sMS_Body);
        //}

        // GET: SMS_Body/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SMS_Body sMS_Body = db.SMS_Bodys.Find(id);
                if (sMS_Body == null)
                {
                    return HttpNotFound();
                }
                return View(sMS_Body);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: SMS_Body/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DebitMessage,CreditMessage")] SMS_Body sMS_Body)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    SMS_Body smsbodydb = db.SMS_Bodys.Find(sMS_Body.ID);
                    smsbodydb.DebitMessage = sMS_Body.DebitMessage;
                    smsbodydb.CreditMessage = sMS_Body.CreditMessage;
                    smsbodydb.UpdatedDate = DateTime.Now;
                    smsbodydb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(smsbodydb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Update successfully.");
                    return RedirectToAction("Index");
                }
                return View(sMS_Body);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: SMS_Body/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SMS_Body sMS_Body = db.SMS_Bodys.Find(id);
        //    if (sMS_Body == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sMS_Body);
        //}

        //// POST: SMS_Body/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    SMS_Body sMS_Body = db.SMS_Bodys.Find(id);
        //    db.SMS_Bodys.Remove(sMS_Body);
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
