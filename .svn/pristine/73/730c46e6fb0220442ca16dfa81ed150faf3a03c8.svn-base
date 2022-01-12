using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class UserTypeModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: UserTypeModels
        public ActionResult Index()
        {
            try {
                var data = db.UserTypeModels;
                var count = db.UserTypeModels.Count();
                if (db.UserTypeModels.Count() > 0)
                {
                    List<UserTypeModel> lstUserType = db.UserTypeModels.Where(u => u.DEL_FLAG == false).ToList();
                    return View(lstUserType);
                }
            }
            catch(Exception ex)
            {
                throw ex;            }
                
            return View();
        }

        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            try {
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
                var rule = (from v in db.UserTypeModels
                            where v.DEL_FLAG == false
                            select v);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    rule = rule.OrderBy(sortColumn+" "+sortColumnDir);
                }
                recordsTotal = rule.Count();
                var data = rule.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            } 
            catch(Exception ex)
            { throw ex; }
        }

        // GET: UserTypeModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeModel userTypeModel = db.UserTypeModels.Find(id);
            if (userTypeModel == null)
            {
                return HttpNotFound();
            }
            return View(userTypeModel);
        }

        // GET: UserTypeModels/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            return View();
        }

        // POST: UserTypeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,USERTYPE,USERTYPE_CODE_LIMIT,GENERATED_LIMIT,DEL_FLAG,CreatedDateTime,CreatedUserID,UpdatedDateTime,UpdatedUserId")] UserTypeModel userTypeModel)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            if (ModelState.IsValid)
            {
                userTypeModel.CreatedDateTime = DateTime.Now;
                userTypeModel.CreatedUserID = User.Identity.GetUserId();
                db.UserTypeModels.Add(userTypeModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userTypeModel);
        }

        // GET: UserTypeModels/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeModel userTypeModel = db.UserTypeModels.Find(id);
            if (userTypeModel == null)
            {
                return HttpNotFound();
            }
            return View(userTypeModel);
        }

        // POST: UserTypeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,USERTYPE,USERTYPE_CODE_LIMIT,GENERATED_LIMIT,DEL_FLAG,CreatedDateTime,CreatedUserID,UpdatedDateTime,UpdatedUserId")] UserTypeModel userTypeModel)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            if (ModelState.IsValid)
            {
                UserTypeModel db_usertype = db.UserTypeModels.Find(userTypeModel.ID);
                if (db_usertype == null)
                {
                    return HttpNotFound();
                }
                db_usertype.USERTYPE = userTypeModel.USERTYPE;
                db_usertype.USERTYPE_CODE_LIMIT = userTypeModel.USERTYPE_CODE_LIMIT;
                db_usertype.GENERATED_LIMIT = userTypeModel.GENERATED_LIMIT;
                db_usertype.UpdatedDateTime = DateTime.Now;
                db_usertype.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(db_usertype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userTypeModel);
        }

        // GET: UserTypeModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeModel userTypeModel = db.UserTypeModels.Find(id);
            if (userTypeModel == null)
            {
                return HttpNotFound();
            }
            return View(userTypeModel);
        }

        // POST: UserTypeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTypeModel userTypeModel = db.UserTypeModels.Find(id);
            db.UserTypeModels.Remove(userTypeModel);
            db.SaveChanges();
            return RedirectToAction("Index");
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
