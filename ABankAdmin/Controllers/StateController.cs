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
    public class StateController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "StateController";
        EventLogController log = new EventLogController();

        // GET: State
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                var states = db.C_States.Include(s => s.Vrcountry);
                return View(states.ToList());
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post/State/Index
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
            var state = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var lststate = (from s in db.C_States
                              select s);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                lststate = lststate.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = lststate.Count();
            var data = lststate.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: State/Details/5
        //public ActionResult Details(int? id)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        State state = db.States.Find(id);
        //        if (state == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(state);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        // GET: State/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: State/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Country_ID")] C_State state)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate state name
                    if (db.C_States.Where(s => s.NAME.ToLower().Trim().Replace(" ", "") == state.NAME.ToLower().Trim().Replace(" ", "") && s.COUNTRY_ID==state.COUNTRY_ID).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate State Name!");
                        ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name");
                        return View(state);
                    }
                    #endregion

                    state.CreatedDateTime = DateTime.Now;
                    state.CreatedUserID = User.Identity.GetUserId();
                    db.C_States.Add(state);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    //For success box
                    ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name", state.COUNTRY_ID);
                    ViewBag.State = "\"" + state.NAME + "\"";
                    return View(state);
                }

                ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name", state.COUNTRY_ID);
                return View(state);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: State/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_State state = db.C_States.Find(id);
                if (state == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name", state.COUNTRY_ID);
                return View(state);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: State/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Country_ID")] C_State state)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate state name
                    if (db.C_States.Where(s => s.NAME.ToLower().Trim().Replace(" ", "") == state.NAME.ToLower().Trim().Replace(" ", "") && s.COUNTRY_ID == state.COUNTRY_ID && s.ID!=state.ID).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate State Name!");
                        ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name");
                        return View(state);
                    }
                    #endregion

                    C_State state_data = db.C_States.Find(state.ID);
                    state_data.COUNTRY_ID = state.COUNTRY_ID;
                    state_data.NAME = state.NAME;
                    state_data.UpdatedDateTime = DateTime.Now;
                    state_data.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(state_data).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name",state.COUNTRY_ID);
                    ViewBag.State = "\""+state.NAME+"\"";
                    return View(state);
                }
                ViewBag.Country_ID = new SelectList(db.C_Countries, "ID", "Name", state.COUNTRY_ID);
                return View(state);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: State/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_State state = db.C_States.Find(id);
                if (state == null)
                {
                    return HttpNotFound();
                }
                return View(state);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_State state = db.C_States.Find(id);
                ViewBag.Country = state.Vrcountry.NAME;
                state.UpdatedDateTime = DateTime.Now;
                state.UpdatedUserId = User.Identity.GetUserId();
                db.C_States.Remove(state);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3);
                ViewBag.State ="\""+state.NAME+"\"";
                return View(state);
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
