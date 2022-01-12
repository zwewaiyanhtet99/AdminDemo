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
using ABankAdmin.Core.Utils;

namespace ABankAdmin.Controllers
{
    public class UtilitiesDetailController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "UtilitiesDetailController";
        EventLogController log = new EventLogController();

        // GET: UtilitiesDetails
        public ActionResult Index()
        {
            bindDropDown();
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
            var Utility_ID = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Currency = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            
            var utilitiesdetail = (from s in db.UtilitiesDetails
                        where s.ACTIVE == "1"
                        select s);

            //SEARCHING...
            if (!string.IsNullOrEmpty(Utility_ID))
            {
                int utility = Convert.ToInt32(Utility_ID);
                utilitiesdetail = utilitiesdetail.Where(a => a.UTILITIES_ID == utility);
            }
            if (!string.IsNullOrEmpty(Currency))
            {
                utilitiesdetail = utilitiesdetail.Where(a => a.CURRENCY.Contains(Currency));
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                utilitiesdetail = utilitiesdetail.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = utilitiesdetail.Count();
            var data = utilitiesdetail.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
      
        [NonAction]
        public void bindDropDown()
        {
            var utility = db.Utilities.Where(u => u.Active == "1").ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Select Utility--", Value = "" });
            foreach (var m in utility)
            {
                li.Add(new SelectListItem { Text = m.Name, Value = m.ID.ToString() });
            }
            ViewBag.utility = li;

            var currency = db.Currencies.Where(c => c.DEL_FLAG == "0").ToList();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem { Text = "--Select Currency--", Value = "" });
            foreach (var m in currency)
            {
                item.Add(new SelectListItem { Text = m.Code, Value = m.Code });
            }
            ViewBag.Currency = item;
        }

        // GET: UtilitiesDetails/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                bindDropDown();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: UtilitiesDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UTILITIES_ID,DENO,CURRENCY")] UtilitiesDetail utilitiesDetail)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region amount overlap validation

                    //check DENO overlap
                    var DENO = utilitiesDetail.DENO.Replace(",", "");
                    if (db.UtilitiesDetails.Where(a => a.UTILITIES_ID == utilitiesDetail.UTILITIES_ID & a.DENO == DENO & a.ACTIVE == "1" & a.CURRENCY == utilitiesDetail.CURRENCY ).Count() > 0)
                    {
                        ModelState.AddModelError("DENO", "Duplicate DENO! This DENO is existing now. Please choose other DENO.");
                        bindDropDown();
                        return View();
                    }
                    
                    #endregion

                    utilitiesDetail.DENO = DENO;
                    utilitiesDetail.ACTIVE = "1";
                    utilitiesDetail.CreatedDateTime = DateTime.Now;
                    utilitiesDetail.CreatedUserID = User.Identity.GetUserId();
                    db.UtilitiesDetails.Add(utilitiesDetail);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, utilitiesDetail.ToString());
                    return RedirectToAction("Index");
                }
                bindDropDown();
                return View(utilitiesDetail);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: UtilitiesDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.UtilitiesDetails
                             where s.ACTIVE == "1"
                             select s;
                UtilitiesDetail utilitiesdetail = result.SingleOrDefault(m => m.ID == id);
                //Get Select Data From DDL             
                var currencylist = db.Currencies.Where( c => c.DEL_FLAG == "0").ToList();
                IEnumerable<SelectListItem> currency = currencylist.Select(c => new SelectListItem()
                {
                    Value = c.Code,
                    Text = c.Code

                });
                ViewBag.Currency = currency;   

                IEnumerable<SelectListItem> utility = db.Utilities.Where(g => g.Active == "1")
                           .Select(f => new SelectListItem()
                           {
                               Value = f.ID.ToString(),
                               Text = f.Name
                           });
                ViewBag.utility = utility;                          
               
                if (utilitiesdetail == null)
                {
                    return HttpNotFound();
                }
                return View(utilitiesdetail);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: UtilitiesDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UTILITIES_ID,DENO,ACTIVE,CURRENCY")] UtilitiesDetail utilitiesDetail)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region amount overlap validation

                    //check DENO overlap
                    var DENO = utilitiesDetail.DENO.Replace(",", "");
                    if (db.UtilitiesDetails.Where(a => a.ID != utilitiesDetail.ID && a.UTILITIES_ID == utilitiesDetail.UTILITIES_ID & a.DENO == DENO & a.ACTIVE == "1" & a.CURRENCY == utilitiesDetail.CURRENCY).Count() > 0)
                    {
                        ModelState.AddModelError("DENO", "Duplicate DENO! This DENO is existing now. Please choose other DENO.");
                        bindDropDown();
                        return View();
                    }

                    #endregion
                    var result = from s in db.UtilitiesDetails
                                 where s.ACTIVE == "1"
                                 select s;
                    var utilitiesdetail = result.SingleOrDefault(m => m.ID == utilitiesDetail.ID);
                    utilitiesdetail.UTILITIES_ID = utilitiesDetail.UTILITIES_ID;
                    utilitiesdetail.DENO = DENO;
                    utilitiesdetail.CURRENCY = utilitiesDetail.CURRENCY;
                    utilitiesdetail.UpdatedDateTime = DateTime.Now;
                    utilitiesdetail.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(utilitiesdetail).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                bindDropDown();
                return View(utilitiesDetail);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }           
        }

        // GET: UtilitiesDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UtilitiesDetail utilitiesDetail = db.UtilitiesDetails.Find(id);
                if (utilitiesDetail == null)
                {
                    return HttpNotFound();
                }
                return View(utilitiesDetail);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: UtilitiesDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                UtilitiesDetail utilitiesDetail = db.UtilitiesDetails.Find(id);
                utilitiesDetail.ACTIVE = "0";
                utilitiesDetail.UpdatedDateTime = DateTime.Now;
                utilitiesDetail.UpdatedUserId = User.Identity.GetUserId();
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
