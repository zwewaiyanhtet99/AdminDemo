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
    public class Tier_ManagementController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "Tier_Management";
        // GET: Tier_Management
        public ActionResult Index()
        {
            try
            {
                if (db.UserTypeModels.Count() > 0)
            {
                List<UserTypeModel> lstUserType = db.UserTypeModels.Where(u => u.DEL_FLAG == false).ToList();
                return View(lstUserType);
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            try
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
                var rule = (from v in db.UserTypeModels
                            where v.DEL_FLAG == false
                            select v);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    rule = rule.OrderBy(sortColumn + " " + sortColumnDir);
                }
                recordsTotal = rule.Count();
                var data = rule.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { throw ex; }
        }
        // GET: Tier_Management/Details/5
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

        // GET: Tier_Management/Create
        public ActionResult Create()
        {
            List<SelectListItem> typeInfo = new List<SelectListItem>()
{
new SelectListItem{ Text="Tier 1", Value = "Tier 1" },
new SelectListItem{ Text="Tier 1.1", Value = "Tier 1.1" },
   new SelectListItem{ Text="Tier 2", Value = "Tier 2" },
   new SelectListItem{ Text="Tier 2.1", Value = "Tier 2.1" },
};
            ViewBag.TypeInfo = typeInfo;
            return View();
        }

        // POST: Tier_Management/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserTypeModel tierModel)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            if (ModelState.IsValid)
            {
                tierModel.CreatedDateTime = DateTime.Now;
                tierModel.CreatedUserID = User.Identity.GetUserId();
                db.UserTypeModels.Add(tierModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tierModel);
        }

        // GET: Tier_Management/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Tier_Management/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,USERTYPE,USERTYPE_CODE_LIMIT,GENERATED_LIMIT,DEL_FLAG,CreatedDateTime,CreatedUserID,UpdatedDateTime,UpdatedUserId")] UserTypeModel userTypeModel)
        {
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

        // GET: Tier_Management/Delete/5
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

        // POST: Tier_Management/Delete/5
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
