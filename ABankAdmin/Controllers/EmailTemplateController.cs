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
    public class EmailTemplateController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "EmailTemplateController";
        EventLogController log = new EventLogController();

        // GET: EmailTemplate
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View(db.EmailTemplates.ToList());
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post/EmailTemplate/Index
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
            var type = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var subject = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var body = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var email = (from e in db.EmailTemplates
                            select e);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                email = email.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = email.Count();
            var data = email.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: EmailTemplate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = db.EmailTemplates.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplate/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: EmailTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,Type,Subject,Body")] EmailTemplate emailTemplate)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.EmailTemplates.Add(emailTemplate);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(emailTemplate);
        //}

        // GET: EmailTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EmailTemplate emailTemplate = db.EmailTemplates.Find(id);
                if (emailTemplate == null)
                {
                    return HttpNotFound();
                }
                return View(emailTemplate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: EmailTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Type,Subject,Body")] EmailTemplate emailTemplate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    EmailTemplate email_temp = db.EmailTemplates.Find(emailTemplate.ID);
                    email_temp.Type = emailTemplate.Type;
                    email_temp.Subject = emailTemplate.Subject;
                    email_temp.Body = emailTemplate.Body;
                    db.Entry(email_temp).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(emailTemplate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: EmailTemplate/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EmailTemplate emailTemplate = db.EmailTemplates.Find(id);
        //    if (emailTemplate == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(emailTemplate);
        //}

        // POST: EmailTemplate/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    EmailTemplate emailTemplate = db.EmailTemplates.Find(id);
        //    db.EmailTemplates.Remove(emailTemplate);
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
