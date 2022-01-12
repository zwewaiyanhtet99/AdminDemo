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
    public class CountryController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CountryController";
        EventLogController log = new EventLogController();

        // GET: Country
        public ActionResult Index()
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
            //return View(db.Countries.ToList());
        }

        //Post/Country/Index
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
            var country = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var countrylst = (from c in db.C_Countries
                              select c);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                countrylst = countrylst.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = countrylst.Count();
            var data = countrylst.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Country/Details/5
        //public ActionResult Details(int? id)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Country country = db.Countries.Find(id);
        //        if (country == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(country);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }

        //}

        // GET: Country/Create
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
            //return View();
        }

        // POST: Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME")] C_Country c_country)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate country name
                    if (db.C_Countries.Where(c => c.NAME.ToLower().Trim().Replace(" ", "") == c_country.NAME.ToLower().Trim().Replace(" ", "")).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Country Name!");
                        return View(c_country);
                    }
                    #endregion

                    c_country.CreatedDateTime = DateTime.Now;
                    c_country.CreatedUserID = User.Identity.GetUserId();
                    db.C_Countries.Add(c_country);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    ViewBag.Country ="\""+ c_country.NAME+"\"";
                    return View(c_country);
                }

                return View(c_country);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Country/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Country country = db.C_Countries.Find(id);
                if (country == null)
                {
                    return HttpNotFound();
                }
                return View(country);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

        }

        // POST: Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME")] C_Country country)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate country name
                    if (db.C_Countries.Where(c => c.NAME.ToLower().Trim().Replace(" ", "") == country.NAME.ToLower().Trim().Replace(" ", "")&& c.ID != country.ID).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Country Name!");
                        return View(country);
                    }
                    #endregion

                    C_Country country_data = db.C_Countries.Find(country.ID);
                    country_data.NAME = country.NAME;
                    country_data.UpdatedDateTime = DateTime.Now;
                    country_data.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(country_data).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    ViewBag.Country = "\"" + country.NAME + "\"";
                    return View(country);
                }
                return View(country);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

        }

        // GET: Country/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Country country = db.C_Countries.Find(id);
                if (country == null)
                {
                    return HttpNotFound();
                }
                return View(country);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Country country = db.C_Countries.Find(id);
                country.UpdatedUserId = User.Identity.GetUserId();
                country.UpdatedDateTime = DateTime.Now;
                db.C_Countries.Remove(country);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3);
                ViewBag.Country = "\""+country.NAME+"\"";
                return View(country);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
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
