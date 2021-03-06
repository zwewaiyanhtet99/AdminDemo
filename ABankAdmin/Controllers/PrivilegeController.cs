using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class PrivilegeController : Controller
    {
        string controllerName = "Privilege";
        private AdminDBContext db = new AdminDBContext(); 
        EventLogController log = new EventLogController();
        // GET: Privilege
        public ActionResult CreatePrivilege()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePrivilege(PrivilegeVM prvilegeViewModel)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.Privilege.Where(c => c.PrivilegeName.ToLower().Trim().Replace(" ", "") == prvilegeViewModel.PrivilegeName.ToLower().Trim().Replace(" ", "") && c.IsDelete == false).Count() != 0)
                    {
                        ModelState.AddModelError("PrivilegeName", "Duplicate Privilege Name!");
                        return View(prvilegeViewModel);
                    }
                    Privilege privilege = new Privilege();
                    //core data
                    privilege.PrivilegeName = prvilegeViewModel.PrivilegeName;
                    privilege.Room = prvilegeViewModel.Room;
                    privilege.Location = prvilegeViewModel.Location;
                    privilege.NoOfFollowers = prvilegeViewModel.NoOfFollowers;
                    privilege.Staff = prvilegeViewModel.Staff;
                    privilege.IsDelete = false;
                    privilege.InUse = false;
                    privilege.Date = Convert.ToDateTime(prvilegeViewModel.DateAndTime).Date;
                    privilege.Time = Convert.ToDateTime(prvilegeViewModel.DateAndTime);
                    //default values
                    privilege.CreatedDate = DateTime.Now;
                    privilege.CreatedUserID = User.Identity.GetUserId();

                    db.Privilege.Add(privilege);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, "CreatePrivilege", 1);
                    ViewBag.Privilege = "success";
                }

                return View(prvilegeViewModel);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "CreatePrivilege", ex);
                return View("Error");
            }
        }
        public ActionResult Index()
        {
            try
            {
                var data = db.Privilege.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View();
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

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.Privilege.Where(a => a.IsDelete == false).ToList().ConvertAll(u => new PrivilegeVM
            {

                ID = u.ID,
                PrivilegeName = u.PrivilegeName,
                Location = u.Location,
                Room = u.Room,
                NoOfFollowers = u.NoOfFollowers,
                Staff = u.Staff,
                InUse = u.InUse,
                DateAndTime = u.Date.ToString("dd/MM/yyyy"),
                Time = u.Time.ToString("HH:mm:ss tt"),
               

            });

            //SEARCHING...

            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
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
                var result = from s in db.Privilege
                             where s.IsDelete == false
                             select s;
                Privilege info = result.SingleOrDefault(m => m.ID == id);
                if (info == null)
                {
                    return HttpNotFound();
                }
                PrivilegeVM infoVm = new PrivilegeVM();
                infoVm.PrivilegeName = info.PrivilegeName;
                infoVm.ID = info.ID;
                infoVm.InUse = info.InUse;
                infoVm.DateAndTime = info.Time.ToString();
                infoVm.Location = info.Location;
                infoVm.NoOfFollowers = info.NoOfFollowers;
                infoVm.Room = info.Room;
                infoVm.Staff = info.Staff;
                return View(infoVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PrivilegeVM infoVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            if (ModelState.IsValid)
            {
                Privilege dbmodel = db.Privilege.Find(infoVm.ID);
                if (dbmodel == null)
                {
                    return HttpNotFound();
                }
                
                dbmodel.InUse = infoVm.InUse;
                dbmodel.PrivilegeName = infoVm.PrivilegeName;
                dbmodel.Room = infoVm.Room;
                dbmodel.Location = infoVm.Location;
                dbmodel.NoOfFollowers = infoVm.NoOfFollowers;
                dbmodel.Staff = infoVm.Staff;
                dbmodel.Date = Convert.ToDateTime(infoVm.DateAndTime).Date;
                dbmodel.Time = Convert.ToDateTime(infoVm.DateAndTime);
                dbmodel.UpdatedDate = DateTime.Now;
                dbmodel.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(dbmodel).State = EntityState.Modified;
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2);
                ViewBag.PrivilegeEdit = "success";
            }
            return View(infoVm);
        }


    }
}