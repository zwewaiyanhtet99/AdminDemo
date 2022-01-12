using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using System.Linq.Dynamic;
using System.Net;
using System.Data.Entity;


namespace ABankAdmin.Controllers
{
    public class IB_HowToUseController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "IB_HowToUseController";
        EventLogController log = new EventLogController();
        // GET: IB_HowToUse
        public ActionResult Index()
        {
            ViewBag.MenuID = GetMenuList();
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? arg1)//adding parameter is in order to use Action name 'Index' same with View action name
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
            var Name = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Description = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var IB_Menu = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            //var city = db.Cities.ToList();
           var IB_HowToUse = (from i in db.IB_HowToUses
                        where i. Del_Flag == false
                        select i);

            //SEARCHING...
            if (!string.IsNullOrEmpty(Name))
            {
                IB_HowToUse = IB_HowToUse.Where(a => a.FormName.ToString().ToLower().Contains(Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(Description))
            {
                IB_HowToUse = IB_HowToUse.Where(a => a.Description.ToLower().Contains(Description.ToLower()));
            }
            if (!string.IsNullOrEmpty(IB_Menu))
            {
                int ib_menu = Convert.ToInt32(IB_Menu);
                IB_HowToUse = IB_HowToUse.Where(a => a.MenuID == ib_menu);
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IB_HowToUse = IB_HowToUse.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = IB_HowToUse.Count();
            var data = IB_HowToUse.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int? id) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.IBMenu = GetMenuList();
                var result = from r in db.IB_HowToUses
                             where r.Del_Flag == false
                             select r;
                IB_HowToUse IB_HowToUse = result.SingleOrDefault(i => i.ID == id);              

                if (IB_HowToUse == null)
                {
                    return HttpNotFound();
                }
                return View(IB_HowToUse);
            }
            catch (Exception ex) 
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FormName,Description")]IB_HowToUse IB_HowToUseNew) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                var IB_HTUList = from iB in db.IB_HowToUses
                                 where iB.Del_Flag == false
                                 select iB;
                IB_HowToUse IB_HowToUseOld = IB_HTUList.SingleOrDefault(i => i.ID == IB_HowToUseNew.ID);
                
                //Validation Duplicate
                if (db.IB_HowToUses.Where(ib => ib.FormName.ToLower().Trim() == IB_HowToUseNew.FormName.ToLower().Trim() && ib.Del_Flag == false && ib.ID != IB_HowToUseNew.ID).Count() != 0)
                {
                    ModelState.AddModelError("FormName", "Duplicate FormName!");
                    ViewBag.IBMenu = GetMenuList();
                    return View(IB_HowToUseNew);
                }
                IB_HowToUseOld.FormName = IB_HowToUseNew.FormName;
                IB_HowToUseOld.Description = IB_HowToUseNew.Description;
                //IB_HowToUseOld.MenuID = IB_HowToUseNew.MenuID;
                IB_HowToUseOld.UpdatedDate = DateTime.Now;
                IB_HowToUseOld.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(IB_HowToUseOld).State = EntityState.Modified;
                db.SaveChanges();
                //save log
                string getuserId = User.Identity.GetUserId();
                log.Info(getuserId, controllerName, actionName, 2);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        //public ActionResult Create()
        //{
        //    ViewBag.IBMenu = GetMenuList();
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,FormName,Description,MenuID")]IB_HowToUse IB_HowToUse) 
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //Code duplicate
        //            if(db.IB_HowToUses.Where(ib=>ib.FormName.ToLower().Trim()==IB_HowToUse.FormName.ToLower().Trim()&&ib.Del_Flag==false).Count()!=0)
        //            {
        //                ModelState.AddModelError("FormName", "Duplicate FormName!");
        //                ViewBag.IBMenu = GetMenuList();
        //                return View(IB_HowToUse);
        //            }
        //            IB_HowToUse.Del_Flag = false;
        //            IB_HowToUse.CreatedDate = DateTime.Now;
        //            IB_HowToUse.CreatedUserId = User.Identity.GetUserId();
        //            db.IB_HowToUses.Add(IB_HowToUse);
        //            db.SaveChanges();
        //            //save log
        //            string getuserid = User.Identity.GetUserId();
        //            log.Info(getuserid, controllerName, actionName, 1, IB_HowToUse.ToString());
        //            return RedirectToAction("Index");
        //        }
        //        return View(IB_HowToUse);
        //    }
        //    catch(Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}
        //public ActionResult Delete(int? id) 
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        IB_HowToUse IB_HowToUse = db.IB_HowToUses.Find(id);
        //        if (IB_HowToUse == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(IB_HowToUse);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id) 
        //{
        //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        IB_HowToUse IB_HowToUse = db.IB_HowToUses.Find(id);
        //        IB_HowToUse.Del_Flag= true;
        //        IB_HowToUse.UpdatedDate = DateTime.Now;
        //        IB_HowToUse.UpdatedUserId = User.Identity.GetUserId();
        //        db.SaveChanges();
        //        //save log
        //        string getuserid = User.Identity.GetUserId();
        //        log.Info(getuserid, controllerName, actionName, 3);
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}
        public ActionResult Details(int? id) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.IB_HowToUses
                             where s.Del_Flag == false
                             select s;
                IB_HowToUse IB_HowToUse = result.SingleOrDefault(m => m.ID == id);
                if (IB_HowToUse == null)
                {
                    return HttpNotFound();
                }
                return View(IB_HowToUse);
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
        private IEnumerable<SelectListItem> GetMenuList()
        {
            try
            {
                List<IB_Menu> IBMenuList = db.IB_Menus.Where(b => b.Active == true).ToList();

                IEnumerable<SelectListItem> IBMenu = IBMenuList.Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.Menu_Name
                });
                return IBMenu;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
    }
   

}