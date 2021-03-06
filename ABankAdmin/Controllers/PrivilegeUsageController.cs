using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class PrivilegeUsageController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "PrivilegeUsage";
        EventLogController log = new EventLogController();
        // GET: PrivilegeUsage
        public ActionResult CreatePrivilegeUsage()
        {
            try
            {

                ViewBag.PrivilegeList = GetPrivilegeName();
                return View();
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePrivilegeUsage(PrivilegeUsageCreateVM privilegeUsageVM)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(privilegeUsageVM.IconicNumber))
                    {
                        string usertypecode = (from s in db.CIFInfoModels where s.USERTYPE == privilegeUsageVM.IconicNumber && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if (!usertypecode.Equals(privilegeUsageVM.IconicNumber))
                        {
                            ModelState.AddModelError("USERTYPECODE", "THE ICONIC Number is Invalid");
                            return View(privilegeUsageVM);
                        }

                    }

                    //save log
                    Session["PrivilegeUsage"] = string.Empty;//To genereate PDF
                    Session["PrivilegeUsage"] = JsonConvert.SerializeObject(privilegeUsageVM);
                    return RedirectToAction("ConfirmPrivilegeUsage");
                }

                return View();


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        public ActionResult ConfirmPrivilegeUsage()
        {
            TempData["createReturn"] = "1";
            PrivilegeUsageVM reqmodel = new PrivilegeUsageVM();
            try
            {
                if (TempData["createD"] == "1")
                {
                    if (Session["PrivilegeUsage"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["PrivilegeUsage"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<PrivilegeUsageVM>(Session["PrivilegeUsage"].ToString());
                            var getinfo =
                                db.Privilege.Where(
                                    x => x.ID == reqmodel.PrivilegeId && x.IsDelete == false && x.InUse == false)
                                    .SingleOrDefault();
                            reqmodel.PrivilegeName = getinfo.PrivilegeName;
                            reqmodel.ServiceDate = getinfo.Date.ToString("dd/MM/yyyy");
                            reqmodel.Staff = getinfo.Staff.ToString();
                            reqmodel.ServiceLocation = getinfo.Location.ToString();
                        }
                    }
                }

               
            }
            catch (Exception ex)
            { throw ex; }
            return View(reqmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPrivilegeUsage(PrivilegeUsageVM privilegeUsage)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.PrivilegeUsage.Where(c => c.CustomerName.ToLower().Trim().Replace(" ", "") == privilegeUsage.CustomerName.ToLower().Trim().Replace(" ", "") && c.IsDelete == false).Count() != 0)
                    {
                        ModelState.AddModelError("CustomerName", "Duplicate Customer Name!");
                        return View(privilegeUsage);
                    }
                    Privilege_Usage privilege = new Privilege_Usage();
                    //core data
                    privilege.CustomerName = privilegeUsage.CustomerName;
                    privilege.CustomerNRC = privilegeUsage.CustomerNRC;
                    privilege.IconicNumber = privilegeUsage.IconicNumber;
                    privilege.IconicTier = privilegeUsage.IconicTier;
                    privilege.IconicExpireDate =Convert.ToDateTime(privilegeUsage.IconicExpireDate);
                    privilege.PrivilegeUsage = privilegeUsage.PrivilegeName;
                    privilege.ServiceDate = (from s in db.Privilege where s.ID == privilegeUsage.PrivilegeId && s.IsDelete == false && s.InUse==false select s.Time).SingleOrDefault();
                    privilege.ServiceLocation = privilegeUsage.ServiceLocation;
                    privilege.IsDelete = false;

                    //default values
                    privilege.CreatedDate = DateTime.Now;
                    privilege.CreatedUserID = User.Identity.GetUserId();

                    db.PrivilegeUsage.Add(privilege);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, "ConfirmPrivilegeUsage", 1);

                    #region Data preparation for Privilege In Use Update
                    var DbInfo = (from s in db.Privilege where s.PrivilegeName == privilegeUsage.PrivilegeName && s.IsDelete == false select s).SingleOrDefault();
                    DbInfo.InUse = true;
                    var localEntity = db.Set<Privilege>().Local.FirstOrDefault(f => f.ID == DbInfo.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    db.Entry(DbInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    ViewBag.PrivilegeUsage = "success";
                }
                return View(privilegeUsage);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "CreatePrivilege", ex);
                return View("Error");
            }
        }

        public JsonResult getCIFIDInfo(string iconicno)
        {
            PrivilegeUsageVM privilegeUsage = new PrivilegeUsageVM();
            CIFInfoModel info = new CIFInfoModel();
            try
            {
                if (iconicno != "")
                {
                    info =
                           (from s in db.CIFInfoModels where s.USERTYPECODE == iconicno && s.DELFLAG == false select s)
                               .SingleOrDefault();
                    if (info == null)
                    {
                        return Json(new { message = "This Iconic Number is not registered" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        privilegeUsage.IconicNumber = info.USERTYPECODE;
                        privilegeUsage.IconicTier = info.USERTYPE;
                        privilegeUsage.IconicExpireDate = info.EXPIREDATE.ToString();

                    }
                    return Json(privilegeUsage, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(privilegeUsage, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPrivilegeInfo(int privilegeID) { 
            PrivilegeUsageCreateVM privilegeUsage = new PrivilegeUsageCreateVM();
            Privilege info = new Privilege();
            try
            {
                info =(from s in db.Privilege where s.ID == privilegeID && s.IsDelete == false && s.InUse==false select s)
                               .SingleOrDefault();
                    if (info == null)
                    {
                        return Json(new { message = "There is no data in Privilege" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        privilegeUsage.DateAndTime = info.Date.ToString("dd/MM/yyyy");
                        privilegeUsage.Time = info.Time.ToString("HH:mm:ss tt");
                        privilegeUsage.Location = info.Location.ToString();
                        privilegeUsage.NoOfFollowers = info.NoOfFollowers.ToString();
                        privilegeUsage.Staff = info.Staff.ToString();

                    }
                    return Json(privilegeUsage, JsonRequestBehavior.AllowGet);

                
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(privilegeUsage, JsonRequestBehavior.AllowGet);
        }
        private IEnumerable<SelectListItem> GetPrivilegeName()
        {
            try
            {
                List<Privilege> lstlist = db.Privilege.Where(b => b.IsDelete == false && b.InUse==false).ToList();
                IEnumerable<SelectListItem> infolist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.PrivilegeName

                });
                return infolist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetPrivilegeName", ex);
                return null;
            }
        }
        public ActionResult Index()
        {
            try
            {
                var data = db.PrivilegeUsage.ToList();
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
            var user = db.PrivilegeUsage.Where(a => a.IsDelete == false).ToList().ConvertAll(u => new PrivilegeUsageVM
            {

                PrivilegeId = u.ID,
                CustomerName = u.CustomerName,
                CustomerNRC = u.CustomerNRC,
                IconicNumber = u.IconicNumber,
                IconicTier = u.IconicTier,
                IconicExpireDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", u.IconicExpireDate),
                ServiceDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", u.ServiceDate),
                PrivilegeUsage = u.PrivilegeUsage,
                ServiceLocation = u.ServiceLocation

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
                var result = from s in db.PrivilegeUsage
                             where s.IsDelete == false
                             select s;
                Privilege_Usage info = result.SingleOrDefault(m => m.ID == id);
                if (info == null)
                {
                    return HttpNotFound();
                }
                
                return View(info);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Privilege_Usage infoVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            if (ModelState.IsValid)
            {
                Privilege_Usage dbmodel = db.PrivilegeUsage.Find(infoVm.ID);
                if (dbmodel == null)
                {
                    return HttpNotFound();
                }
                dbmodel.CustomerName = infoVm.CustomerName;
                dbmodel.CustomerNRC = infoVm.CustomerNRC;
                dbmodel.UpdatedDate = DateTime.Now;
                dbmodel.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(dbmodel).State = EntityState.Modified;
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2);
                ViewBag.PrivilegeUsageEdit = "success";
            }
            return View(infoVm);
        }
        // GET: Tier_Management/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilege_Usage dbmodel = db.PrivilegeUsage.Find(id);
            if (dbmodel == null)
            {
                return HttpNotFound();
            }
            return View(dbmodel);
        }

        // POST: Tier_Management/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Privilege_Usage dbmodel = db.PrivilegeUsage.Find(id);
            db.PrivilegeUsage.Remove(dbmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}