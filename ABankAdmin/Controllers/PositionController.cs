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
    public class PositionController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "PositionController";

        // GET: Positions
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.Positions.ToList());
        }

        //Post Position
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
            //var position = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var lstposition = (from p in db.C_Positions
                               where p.DEL_FLAG==false
                              select p);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                lstposition = lstposition.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = lstposition.Count();
            var data = lstposition.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Positions/Details/5
        //public ActionResult Details(int? id)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Position position = db.Positions.Find(id);
        //        if (position == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(position);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }          
        //}

        // GET: Positions/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //List<Corporate> corporate = db.Corporates.Where(c => c.DEL_FLAG == false).ToList();
                //IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                //{
                //    Value = a.ID.ToString(),
                //    Text = a.Company_Name

                //});
                //ViewBag.Corporate = items;
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Positions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME")] C_Position position)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate position name
                    if (db.C_Positions.Where(d => d.NAME.ToLower().Trim().Replace(" ", "") == position.NAME.ToLower().Trim().Replace(" ", "") && d.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Position Name!");
                        return View(position);
                    }
                    #endregion

                    position.CreatedUserID = User.Identity.GetUserId();
                    position.CreatedDateTime = DateTime.Now;
                    position.DEL_FLAG = false;
                    db.C_Positions.Add(position);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    ViewBag.position = "\""+position.NAME+"\"";
                    return View(position);
                }

                return View(position);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Positions/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Position position = db.C_Positions.Find(id);
                if (position == null)
                {
                    return HttpNotFound();
                }
                //List<Corporate> corporate = db.Corporates.Where(c => c.DEL_FLAG == false).ToList();
                //IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                //{
                //    Value = a.ID.ToString(),
                //    Text = a.Company_Name

                //});
                //ViewBag.Corporate = items;
                return View(position);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME")] C_Position position)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate position name
                    if (db.C_Positions.Where(p => p.NAME.ToLower().Trim().Replace(" ","") == position.NAME.ToLower().Trim().Replace(" ","") && p.DEL_FLAG == false && p.ID!=position.ID).Count() != 0)
                    {
                        ModelState.AddModelError("NAME", "Duplicate Position Name!");
                        return View(position);
                    }
                    #endregion

                    var result = from p in db.C_Positions
                                 where p.DEL_FLAG ==false
                                 select p;
                    var positiondb = result.SingleOrDefault(m => m.ID == position.ID);
                    //Position position_data = db.Positions.Find(position.ID);
                    //positiondb.CORPORATE_ID = position.CORPORATE_ID;
                    positiondb.NAME = position.NAME;
                    positiondb.UpdatedDateTime = DateTime.Now;
                    positiondb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(positiondb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    ViewBag.Position = "\""+position.NAME+"\"";
                    return View(position);
                }
                return View(position);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Positions/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Position position = db.C_Positions.Find(id);
                if (position == null)
                {
                    return HttpNotFound();
                }
                return View(position);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Position position = db.C_Positions.Find(id);
                position.UpdatedDateTime = DateTime.Now;
                position.UpdatedUserId = User.Identity.GetUserId();
                position.DEL_FLAG = true;
                //db.Positions.Remove(position);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3);
                ViewBag.Positon ="\""+position.NAME+"\"";
                return View(position);
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
