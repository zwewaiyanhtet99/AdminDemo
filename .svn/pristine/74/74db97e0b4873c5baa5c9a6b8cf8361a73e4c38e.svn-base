using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class RM_TL_InfoController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "RM_TL_Info";
        // GET: RM_TL_Info
        public ActionResult Index()
        {
            try
            {
                var data = db.RM_TL_Info.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.RM_TL_Info.ToList());
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
            var name = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var description = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.RM_TL_Info.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new RM_TL_Info{

                ID = u.ID,
                Name = u.Name,
                Description = u.Description,
                PhoneNo = u.PhoneNo,
                Email = u.Email
            });

            //SEARCHING...

            if (!string.IsNullOrEmpty(name.Trim()))
            {
                user = user.Where(a => a.Name.ToLower().Contains(name.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(description.Trim()))
            {
                user = user.Where(a => a.Description.ToLower().Contains(description.Trim().ToLower())).ToList();
            }

            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        // GET: RM_TL_Info/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_TL_Info rM_TL_Info = db.RM_TL_Info.Find(id);
            if (rM_TL_Info == null)
            {
                return HttpNotFound();
            }
            return View(rM_TL_Info);
        }

        // GET: RM_TL_Info/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RM_TL_Info/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email,PhoneNo,Description,CREATEDDATE,CREATEDUSERID,UpdatedDate,UPDATEDUSERID,DEL_FLAG")] RM_TL_Info Info)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.RM_TL_Info.Where(c => c.Name.ToLower().Trim().Replace(" ", "") == Info.Name.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate RM TL Name!");
                        return View(Info);
                    }
                    #endregion
                    #region DataPreparation
                    RM_TL_Info dbModel = new RM_TL_Info();
                    dbModel.CREATEDDATE = DateTime.Now;
                    dbModel.CREATEDUSERID = User.Identity.GetUserId();
                    dbModel.Name = Info.Name;
                    dbModel.Description = Info.Description;
                    dbModel.Email = Info.Email;
                    dbModel.PhoneNo = Info.PhoneNo;
                    #endregion
                    db.RM_TL_Info.Add(dbModel);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    
                    ViewBag.RMInfo = "\"" + dbModel.Name + "\"";

                }
                
                return View(Info);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: RM_TL_Info/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_TL_Info rM_TL_Info = db.RM_TL_Info.Find(id);
            if (rM_TL_Info == null)
            {
                return HttpNotFound();
            }
            return View(rM_TL_Info);
        }

        // POST: RM_TL_Info/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email,PhoneNo,Description,CREATEDDATE,CREATEDUSERID,UpdatedDate,UPDATEDUSERID,DEL_FLAG")] RM_TL_Info rM_TL_Info)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            try
            {

                if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.RM_TL_Info.Where(c => c.Name.ToLower().Trim().Replace(" ", "") == rM_TL_Info.Name.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false && c.ID != rM_TL_Info.ID).Count() != 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate RM TL Name!");
                        return View(rM_TL_Info);
                    }
                    #endregion
                    #region DataPreparation

                    RM_TL_Info dbModel = new RM_TL_Info();
                    var dataFromDB = from s in db.RM_TL_Info
                                     where s.DEL_FLAG == false
                                     select s;
                    dbModel = dataFromDB.SingleOrDefault(m => m.ID == rM_TL_Info.ID);

                    dbModel.UpdatedDate = DateTime.Now;
                    dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                    dbModel.Name = rM_TL_Info.Name;
                    dbModel.Description = rM_TL_Info.Description;
                    dbModel.Email = rM_TL_Info.Email;
                    dbModel.PhoneNo = rM_TL_Info.PhoneNo;
                    dbModel.ID = rM_TL_Info.ID;
                    #endregion
                    db.Entry(dbModel).State = EntityState.Modified;
                db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{dbModel.Name} is updated.");
                    ViewBag.EditRMInfo = "\"" + dbModel.Name + "\"";
                }
                return View(rM_TL_Info);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            
        }

        // GET: RM_TL_Info/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_TL_Info rM_TL_Info = db.RM_TL_Info.Find(id);
            if (rM_TL_Info == null)
            {
                return HttpNotFound();
            }
            return View(rM_TL_Info);
        }

        // POST: RM_TL_Info/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RM_TL_Info rM_TL_Info = db.RM_TL_Info.Find(id);
            if (db.RM_Info.Where(x => x.RM_TL_ID == rM_TL_Info.ID).Count() != 0)
            {
                ViewBag.Error = rM_TL_Info.ID;
                return View("Delete");
            }
            db.RM_TL_Info.Remove(rM_TL_Info);
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
