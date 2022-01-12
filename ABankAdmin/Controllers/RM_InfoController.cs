using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class RM_InfoController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "RM_Info";
        // GET: RM_Info
        public ActionResult Index()
        {
            try
            {
                List<RM_InfoVM> lstvm = new List<RM_InfoVM>();
                RM_InfoVM vm = new RM_InfoVM();
                var data = db.RM_Info.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    foreach (var rmInfo in data)
                    {
                        vm.ID = rmInfo.ID;
                        vm.Name = rmInfo.Name;
                        var getTLName = (from s in db.RM_TL_Info where s.ID == rmInfo.RM_TL_ID select s).SingleOrDefault();
                        vm.RM_TL_Name = getTLName.Name;
                        lstvm.Add(vm);
                    }
                    return View(lstvm);
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
            
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.RM_Info.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new RM_InfoVM
            {

                ID = u.ID,
                Name = u.Name,
                RM_TL_ID = u.RM_TL_ID
                
            });
            foreach (var item in user)
            {
                if (item.RM_TL_ID != null)
                {
                    item.RM_TL_Name = db.RM_TL_Info.Where(x => x.ID == item.RM_TL_ID).Select(x => x.Name).FirstOrDefault();
                }
            }
            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        // GET: RM_Info/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_Info rM_Info = db.RM_Info.Find(id);
            RM_InfoVM vm = new RM_InfoVM();
            if (rM_Info == null)
            {
                return HttpNotFound();
            }
            vm.Name = rM_Info.Name;
            vm.RM_TL_ID = rM_Info.RM_TL_ID;
            var getTLName = (from s in db.RM_TL_Info where s.ID == rM_Info.RM_TL_ID select s).SingleOrDefault();
            vm.RM_TL_Name = getTLName.Name;
            
            return View(vm);
        }

        // GET: RM_Info/Create
        public ActionResult Create()
        {
            try
            {
               
                ViewBag.RM_TL_List = GetRM_TL_List();
                return View();
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        private IEnumerable<SelectListItem> GetRM_TL_List()
        {
            try
            {
                List<RM_TL_Info> lstlist = db.RM_TL_Info.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> usertypelist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.Name

                });
                return usertypelist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
        // POST: RM_Info/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RM_InfoVM rM_Info)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
               if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.RM_Info.Where(c => c.Name.ToLower().Trim().Replace(" ", "") == rM_Info.Name.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate RM Name!");
                        return View(rM_Info);
                    }
                    #endregion
                    #region DataPreparation
                    RM_Info dbModel = new RM_Info();
                    dbModel.CREATEDDATE = DateTime.Now;
                    dbModel.CREATEDUSERID = User.Identity.GetUserId();
                    dbModel.Name = rM_Info.Name;
                    dbModel.Email = rM_Info.Email;
                    dbModel.RM_TL_ID = rM_Info.RM_TL_ID;
                    
                    #endregion
                    db.RM_Info.Add(dbModel);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);

                    ViewBag.RMInfo = "\"" + dbModel.Name + "\"";

                }
                ViewBag.RM_TL_List = GetRM_TL_List().Where(x=>x.Text== rM_Info.Name).OrderByDescending(x=>x.Text);
                return View(rM_Info);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: RM_Info/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rM_Info = db.RM_Info.Find(id);
            if (rM_Info == null)
            {
                return HttpNotFound();
            }
            RM_InfoVM vm = new RM_InfoVM();
            vm.Name = rM_Info.Name;
            vm.RM_TL_ID = rM_Info.RM_TL_ID;
            vm.Email = rM_Info.Email;
            var getTLName = (from s in db.RM_TL_Info where s.ID == rM_Info.RM_TL_ID select s).SingleOrDefault();
            vm.RM_TL_Name = getTLName.Name;
            //RM TL info
            IEnumerable<SelectListItem> items = db.RM_TL_Info.Where(c => c.DEL_FLAG == false).Select(c => new SelectListItem
            {
                Selected = (c.ID == rM_Info.RM_TL_ID),
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            //items.Insert(0, new SelectListItem { Value = vm.RM_TL_ID.ToString(), Text = vm.RM_TL_Name });
            ViewBag.RM_TL_List = items;
            return View(vm);
        }

        // POST: RM_Info/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RM_InfoVM rM_Info)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            try
            {

                if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.RM_Info.Where(c => c.Name.ToLower().Trim().Replace(" ", "") == rM_Info.Name.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false && c.ID != rM_Info.ID).Count() != 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate RM TL Name!");
                        return View(rM_Info);
                    }
                    #endregion
                    #region DataPreparation

                    RM_Info dbModel = new RM_Info();
                    var dataFromDB = from s in db.RM_Info
                                     where s.DEL_FLAG == false
                                     select s;
                    dbModel = dataFromDB.SingleOrDefault(m => m.ID == rM_Info.ID);

                    dbModel.UpdatedDate = DateTime.Now;
                    dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                    dbModel.Name = rM_Info.Name;
                    dbModel.RM_TL_ID = rM_Info.RM_TL_ID;
                    dbModel.ID = rM_Info.ID;
                    dbModel.Email = rM_Info.Email;
                    #endregion
                    db.Entry(dbModel).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{dbModel.Name} is updated.");
                    ViewBag.EditRMInfo = "\"" + dbModel.Name + "\"";
                }
                ViewBag.RM_TL_List = GetRM_TL_List();
                return View(rM_Info);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: RM_Info/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_Info rM_Info = db.RM_Info.Find(id);
            if (rM_Info == null)
            {
                return HttpNotFound();
            }
            RM_InfoVM vm = new RM_InfoVM();
            vm.RM_TL_ID = rM_Info.RM_TL_ID;
            vm.Name = rM_Info.Name;
            var getTLName = (from s in db.RM_TL_Info where s.ID == rM_Info.RM_TL_ID select s).SingleOrDefault();
            vm.RM_TL_Name = getTLName.Name;
            return View(vm);
        }

        // POST: RM_Info/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RM_Info rM_Info = db.RM_Info.Find(id);
            db.RM_Info.Remove(rM_Info);
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
