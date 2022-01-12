using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using NLog.Fluent;
using PagedList;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class CityController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CityController";
        EventLogController log = new EventLogController();
        // GET: City
        public ActionResult Index()
        {  
            return View();
        }

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
            var CityCode = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            //var city = db.Cities.ToList();
            var city = (from s in db.Cities
                        where s.DEL_FLAG == "0"
                        select s);           

            //SEARCHING...
            if (!string.IsNullOrEmpty(CityCode))
            {
                city = city.Where(a => a.Code.Contains(CityCode.Trim()));
            }
            if (!string.IsNullOrEmpty(Desc))
            {
                city = city.Where(a => a.Description.Contains(Desc.Trim()));
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                city = city.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = city.Count();
            var data = city.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: City/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Cities
                             where s.DEL_FLAG == "0"
                             select s;
                City city = result.SingleOrDefault(m => m.ID == id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: City/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Description")] City city)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Regex rgx = new Regex("^[A-Z]{1}[0-9]{3}$");
                    bool reg = true;
                    if (!rgx.IsMatch(city.Code))
                    {
                        //ModelState.AddModelError("Code", "Invalid Currency Code!");
                        ModelState.AddModelError("Code", "City Code must start with 'C' and Maximum length is 4.");
                        return View(city);
                    }
                    //code duplicate
                    if (db.Cities.Where(c => c.Code == city.Code && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Code", "Duplicate City Code!");
                        reg = false;
                    }
                    if (db.Cities.Where(c => c.Description == city.Description && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Description", "Duplicate Description!");
                        reg = false;
                    }
                    if (reg == false)
                    {
                        return View(city);
                    }
                    city.DEL_FLAG = "0";
                    city.CreatedDate = DateTime.Now;
                    city.CreatedUserId = User.Identity.GetUserId();
                    db.Cities.Add(city);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, city.ToString());
                    return RedirectToAction("Index");
                }
                return View(city);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
    // GET: City/Edit/5
    public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Cities
                             where s.DEL_FLAG == "0"
                             select s;
                City city = result.SingleOrDefault(m => m.ID == id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Description")] City city)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = from s in db.Cities
                                 where s.DEL_FLAG == "0"
                                 select s;
                    var city1 = result.SingleOrDefault(m => m.ID == city.ID);
                    city1.Description = city.Description;
                    city1.UpdatedDate = DateTime.Now;
                    city1.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(city1).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(city);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: City/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                City city = db.Cities.Find(id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                City city = db.Cities.Find(id);
                //check using at branch
                if (db.Branches.Where(b => b.CITY == city.Code).Count() > 0) 
                {
                    ModelState.AddModelError("", "You can't delete this City. This is using at Branch!");
                    return View(city);
                }
                city.DEL_FLAG = "1";
                city.UpdatedDate = DateTime.Now;
                city.UpdatedUserId = User.Identity.GetUserId();
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
