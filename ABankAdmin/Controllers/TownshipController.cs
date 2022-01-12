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
using PagedList;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class TownshipController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "TownshipController";
        EventLogController log = new EventLogController();
        // GET: Township
        public ActionResult Index()
        {
            IEnumerable<SelectListItem> items = db.Cities.Where(c=>c.DEL_FLAG=="0").Select(c => new SelectListItem
            {
                Value = c.Description,//c.Code,
                Text = c.Description

            });
            ViewBag.City = items;
            return View();            
        }
        
        //Post :Township/Index
        [HttpPost]
        public ActionResult Index(int? arg1) //adding parameter is in order to use Action name 'Index' same with View action name
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
            var search_city = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Code = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var township = (from s in db.Townships
                        where s.DEL_FLAG == "0"
                        select s);
           
            foreach (var c in township.ToList())
            {
                var city = db.Cities.Where(t => t.Code == c.CITY_Code && t.DEL_FLAG == "0").FirstOrDefault();
                c.CITY_Code = city == null ? "" : city.Description;

            }

            //update township for City Name changes for querying
            township = township.ToList().AsQueryable();

            //SEARCHING...

            if (!string.IsNullOrEmpty(search_city))
            {
                township = township.Where(a => a.CITY_Code == search_city);
                
            }
            if (!string.IsNullOrEmpty(Code))
            {
                township = township.Where(a => a.Code.Contains(Code.Trim()));
            }
            if (!string.IsNullOrEmpty(Desc))
            {
                township = township.Where(a => a.Description.Contains(Desc.Trim()));
            }

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                township = township.OrderBy(sortColumn+" "+sortColumnDir);
            }

           
            recordsTotal = township.Count();
            var data = township.Skip(skip).Take(pageSize).ToList();
           

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Township/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Townships
                             where s.DEL_FLAG == "0"
                             select s;
                Township town = result.SingleOrDefault(m => m.ID == id);
                var city = db.Cities.Where(t => t.Code == town.CITY_Code).FirstOrDefault();
                town.CITY_Code = city == null ? "" : city.Description;
                //Township township = db.Townships.Find(id);
                if (town == null)
                {
                    return HttpNotFound();
                }
                return View(town);
            }
            catch (Exception ex)
            {                
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Township/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                IEnumerable<SelectListItem> items = db.Cities.Where(c=> c.DEL_FLAG == "0").Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Description

                });
                ViewBag.City = items;
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Township/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Description,CITY_Code")] Township township)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Regex rgx = new Regex("^[A-Z]{1}[0-9]{3}$");
                    bool reg = true;
                    if (!rgx.IsMatch(township.Code))
                    {
                        //ModelState.AddModelError("Code", "Invalid Township Code!");
                        ModelState.AddModelError("Code", "Township Code must start 'T' and Maximum length is 4.");
                        //return View(township);
                        goto dropdown;
                    }
                    //code duplicate
                    if (db.Townships.Where(c => c.Code == township.Code && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Code", "Duplicate Township Code!");
                        reg = false;
                    }
                    if (db.Townships.Where(c => c.Description == township.Description && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Description", "Duplicate Description!");
                        reg = false;
                    }
                    if (reg == false)
                    {
                        //return View(township);
                        goto dropdown;
                    }
                    township.DEL_FLAG = "0";
                    township.CreatedDate = DateTime.Now;
                    township.CreatedUserId = User.Identity.GetUserId();
                    db.Townships.Add(township);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, township.ToString());
                    return RedirectToAction("Index");
                }

            dropdown:
                //dropdown
                IEnumerable<SelectListItem> items = db.Cities.Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Description

                });
                ViewBag.City = items;
                return View(township);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Township/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Townships
                             where s.DEL_FLAG == "0"
                             select s;
                Township town = result.SingleOrDefault(m => m.ID == id);
                //Township township = db.Townships.Find(id);
                if (town == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<SelectListItem> items = db.Cities.Where(c => c.DEL_FLAG == "0").Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Description
                });
                ViewBag.City = items;
                return View(town);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Township/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Description,CITY_Code")] Township township)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Regex rgx = new Regex("^[A-Z]{1}[0-9]{3}$");
                    bool reg = true;
                    if (!rgx.IsMatch(township.Code))
                    {
                        //ModelState.AddModelError("Code", "Invalid Township Code!");
                        ModelState.AddModelError("Code", "Township Code must start 'T' and Maximum length is 4.");
                        //return View(township);
                        goto dropdown;
                    }
                    //code duplicate
                    if (db.Townships.Where(c => c.Code == township.Code && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Code", "Duplicate Township Code!");
                        reg = false;
                    }
                    if (db.Townships.Where(c => c.Description == township.Description && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Description", "Duplicate Description!");
                        reg = false;
                    }
                    if (reg == false)
                    {
                        //return View(township);
                        goto dropdown;
                    }
                    var result = from s in db.Townships
                                 where s.DEL_FLAG == "0"
                                 select s;
                    var town = result.SingleOrDefault(m => m.ID == township.ID);
                    town.Code = township.Code;
                    town.Description = township.Description;
                    town.CITY_Code = township.CITY_Code;
                    town.UpdatedDate = DateTime.Now;
                    town.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(town).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
            dropdown:
                //dropdown
                IEnumerable<SelectListItem> items = db.Cities.Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Description

                });
                ViewBag.City = items;
                return View(township);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Township/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Townships
                             where s.DEL_FLAG == "0"
                             select s;
                Township township = result.SingleOrDefault(m => m.ID == id);
                var city = db.Cities.Where(t => t.Code == township.CITY_Code).FirstOrDefault();
                township.CITY_Code = city == null ? "" : city.Description;
                //Township township = db.Townships.Find(id);
                if (township == null)
                {
                    return HttpNotFound();
                }
                return View(township);
            }
            catch (Exception ex)
            {                
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Township/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Township township = db.Townships.Find(id);
                //check using at branch
                if (db.Branches.Where(b => b.TOWNSHIP == township.Code).Count() > 0)
                {
                    ModelState.AddModelError("", "You can't delete this Township. This is using at Branch!");
                    return View(township);
                }                
                township.DEL_FLAG = "1";
                township.UpdatedDate = DateTime.Now;
                township.UpdatedUserId = User.Identity.GetUserId();
                //db.Townships.Remove(township);
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
