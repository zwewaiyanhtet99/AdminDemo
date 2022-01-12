using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class FeedBackController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "FeedBackController";
        EventLogController log = new EventLogController();
        // GET: FeedBack          
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int? Test)
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
            var title = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

           
            var feedback = (from s in db.Feedbacks
                        where s.ACTIVE == "1"
                        select s);

            //SEARCHING...
            if (!string.IsNullOrEmpty(title))
            {
                feedback = feedback.Where(a => a.TITLE.Contains(title.Trim()));
            }
            if (!string.IsNullOrEmpty(Desc))
            {
                feedback = feedback.Where(a => a.DESCRIPTION.Contains(Desc.Trim()));
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                feedback = feedback.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = feedback.Count();
            var data = feedback.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: FeedBack/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: FeedBack/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TITLE,DESCRIPTION")] FeedBack feedback)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
     
                    if (db.Feedbacks.Where(c => c.TITLE == feedback.TITLE && c.ACTIVE == "1" /*&& c.ID == feedback.ID*/).Count() > 0)
                    {
                        ModelState.AddModelError("Title", "Duplicate Title!");
                        return View(feedback);
                    }
                    feedback.ACTIVE = "1";
                    feedback.CreatedDateTime = DateTime.Now;
                    feedback.CreatedUserID = User.Identity.GetUserId();
                    db.Feedbacks.Add(feedback);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, feedback.ToString());
                    return RedirectToAction("Index");
                }
                return View(feedback);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: FeedBack/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Feedbacks
                             where s.ACTIVE == "1"
                             select s;
                FeedBack feedback = result.SingleOrDefault(m => m.ID == id);
                if (feedback == null)
                {
                    return HttpNotFound();
                }
                return View(feedback);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: FeedBack/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TITLE,DESCRIPTION")] FeedBack feedback)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = from s in db.Feedbacks
                                 where s.ACTIVE == "1"
                                 select s;                    
                    var feedback1 = result.SingleOrDefault(m => m.ID == feedback.ID);
                    feedback1.DESCRIPTION = feedback.DESCRIPTION;
                    feedback1.UpdatedDateTime = DateTime.Now;
                    feedback1.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(feedback1).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(feedback);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: FeedBack/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedBack feedback = db.Feedbacks.Find(id);
                if (feedback == null)
                {
                    return HttpNotFound();
                }
                return View(feedback);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }



        // POST: FeedBack/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                FeedBack feedback = db.Feedbacks.Find(id);
                feedback.ACTIVE = "0";
                feedback.UpdatedDateTime = DateTime.Now;
                feedback.UpdatedUserId = User.Identity.GetUserId();
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
    }
}
