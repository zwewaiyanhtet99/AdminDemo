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
    public class SMS_AllowController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "SMS_AllowController";
        EventLogController log = new EventLogController();

        // GET: SMS_Allow
        public ActionResult Index()
        
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try 
            {
                //List<SMS_Allow> lstsmsallow = db.SMS_Allow.Where(s => s.Active == true).ToList();

                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.SMS_Allow.ToList());
        }

        //Post SMS_ALLOW
        [HttpPost]
        public ActionResult Index(int? arg1)//adding parameter is in order to use Action name 'Index' same with View action name
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
            var senallow = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var recallow = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var smsallow = (from s in db.SMS_Allows
                           where s.Active ==true
                           select s);

            //foreach (var c in utilities.ToList())
            //{
            //    var rule = db.Rules.Where(t => t.Code == c.DiscountCode).FirstOrDefault();
            //    c.DiscountCode = rule == null ? "" : rule.Description;

            //}
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                smsallow = smsallow.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = smsallow.Count();
            var data = smsallow.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: SMS_Allow/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SMS_Allow sMS_Allow = db.SMS_Allow.Find(id);
        //    if (sMS_Allow == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sMS_Allow);
        //}

        // GET: SMS_Allow/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: SMS_Allow/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,TransactionType,Sender_Allow,Receiver_Allow,CreatedUserId,CreatedDate,UpdatedUserId,UpdatedDate,Active")] SMS_Allow sMS_Allow)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.SMS_Allow.Add(sMS_Allow);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(sMS_Allow);
        //}

        // GET: SMS_Allow/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SMS_Allow sMS_Allow = db.SMS_Allows.Find(id);
                if (sMS_Allow == null)
                {
                    return HttpNotFound();
                }
                return View(sMS_Allow);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(sMS_Allow);
        }

        // POST: SMS_Allow/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Sender_Allow,Receiver_Allow,Sender_Email_Allow,Receiver_Email_Allow")] SMS_Allow sMS_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    SMS_Allow smsallowdb = db.SMS_Allows.Find(sMS_Allow.ID);
                    smsallowdb.Sender_Allow = sMS_Allow.Sender_Allow;
                    smsallowdb.Receiver_Allow = sMS_Allow.Receiver_Allow;
                    smsallowdb.Sender_Email_Allow = sMS_Allow.Sender_Email_Allow;
                    smsallowdb.Receiver_Email_Allow = sMS_Allow.Receiver_Email_Allow;
                    smsallowdb.UpdatedDate = DateTime.Now;
                    smsallowdb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(smsallowdb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Update successfully.");
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            return View(sMS_Allow);
        }

        // GET: SMS_Allow/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SMS_Allow sMS_Allow = db.SMS_Allow.Find(id);
        //    if (sMS_Allow == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sMS_Allow);
        //}

        // POST: SMS_Allow/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    SMS_Allow sMS_Allow = db.SMS_Allow.Find(id);
        //    db.SMS_Allow.Remove(sMS_Allow);
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
