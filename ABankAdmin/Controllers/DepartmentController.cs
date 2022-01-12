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
    public class DepartmentController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "DepartmentController";

        // GET: Department
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //ViewBag.Corporate = GetCorporateList();
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName,actionName,ex);
                return View("Error");
            }
            //return View(db.Departments.ToList());
        }

        //Post Department
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
            //var dept = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var lstdept = (from d in db.C_Departments
                           where d.DEL_FLAG==false
                              select d);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                lstdept = lstdept.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = lstdept.Count();
            var data = lstdept.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Department/Details/5
        //public ActionResult Details(int? id)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Department department = db.Departments.Find(id);
        //        if (department == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(department);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        // GET: Department/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
               // ViewBag.Corporate = GetCorporateList();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME")] C_Department department)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //bool reg = true;
                    #region duplicate department name
                    if (db.C_Departments.Where(d => d.NAME.ToLower().Trim().Replace(" ", "") == department.NAME.ToLower().Trim().Replace(" ", "") && d.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Department Name!");
                        return View(department);
                        //reg = false;
                    }
                    //if (reg == false)
                    //{
                    //    goto dropdown;
                    //}
                    #endregion

                    department.CreatedDateTime = DateTime.Now;
                    department.CreatedUserID = User.Identity.GetUserId();
                    department.DEL_FLAG = false;
                    db.C_Departments.Add(department);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    ViewBag.Department = "\""+department.NAME+"\"";
                    return View(department);
                }
            //dropdown:
            //    ViewBag.Corporate = GetCorporateList();

                return View(department);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Department department = db.C_Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME")] C_Department department)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //bool reg = true;
                    #region duplicate department name
                    if (db.C_Departments.Where(d => d.NAME.ToLower().Trim().Replace(" ", "") == department.NAME.ToLower().Trim().Replace(" ", "") && d.DEL_FLAG == false && d.ID!=department.ID).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Department Name!");
                        return View(department);
                        //reg = false;
                    }
                    //if (reg == false)
                    //{
                    //    goto dropdown;
                    //}
                    #endregion
                    var result = from d in db.C_Departments
                                 where d.DEL_FLAG == false
                                 select d;
                    var deptdb = result.SingleOrDefault(m => m.ID == department.ID);
                    //Department dept_data = db.Departments.Find(department.ID);
                    //deptdb.Corporate_ID = department.Corporate_ID;
                    deptdb .NAME= department.NAME;
                    deptdb.UpdatedDateTime = DateTime.Now;
                    deptdb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(deptdb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    //for success pop up
                    ViewBag.Department = "\"" + department.NAME + "\"";
                    return View(department);
                }
            //dropdown:
            //    //dropdown
            //    ViewBag.Corporate = GetCorporateList();
                return View(department);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Department department = db.C_Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Department department = db.C_Departments.Find(id);
                department.DEL_FLAG = true;
                department.UpdatedDateTime = DateTime.Now;
                department.UpdatedUserId = User.Identity.GetUserId();
                //db.Departments.Remove(department);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3);
                ViewBag.Department ="\""+department.NAME+"\"";
                return View(department);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        private IEnumerable<SelectListItem> GetCorporateList()
        {
            try
            {
                List<C_Corporate> CorporateList = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();

                IEnumerable<SelectListItem> lstCorporate = CorporateList.Select(co => new SelectListItem
                {
                    Value = co.ID.ToString(),
                    Text = co.COMPANY_NAME
                });
                return lstCorporate;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
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
