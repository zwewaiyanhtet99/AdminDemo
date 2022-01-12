using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class IconicBlacklistModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "IconicBlacklistModels";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        // GET: IconicBlacklistModels
        public ActionResult Index()
        {
            try
            {
                var data = db.IconicBlacklistModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.IconicBlacklistModels.ToList());
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
            //find search columns info
            var blacklist = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var description = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.IconicBlacklistModels.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new IconicBlacklistVM
            {

                ID = u.ID,
                BlacklistNo = u.BlackListNumber,
                Description = u.Description,
                Createduserid = u.CREATEDUSERID
            });

            //SEARCHING...

            if (!string.IsNullOrEmpty(blacklist.Trim()))
            {
                user = user.Where(a => a.BlacklistNo.ToLower().Contains(blacklist.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(description.Trim()))
            {
                user = user.Where(a => a.Description.ToLower().Contains(description.Trim().ToLower())).ToList();
            }

            foreach (var item in user)
            {
                if (item.Createduserid != null)
                {
                    item.Createduserid = db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
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

        // GET: IconicBlacklistModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IconicBlacklistModel iconicBlacklistModel = db.IconicBlacklistModels.Find(id);
            if (iconicBlacklistModel == null)
            {
                return HttpNotFound();
            }
            return View(iconicBlacklistModel);
        }

        // GET: IconicBlacklistModels/Create
        public ActionResult Create()
        {
            try
            {
                IconicBlacklistVM iconicInfo = new IconicBlacklistVM();
                ViewBag.UserType = GetUserTypeList();
                return View(iconicInfo);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: IconicBlacklistModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IconicBlacklistVM reqBlacklistVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    
                    string takecountstr = reqBlacklistVm.PrefixBooking;
                    int takecount = takecountstr.Length;
                    int reqbookinglen = Convert.ToInt16(reqBlacklistVm.BlacklistNo.Length);
                    var count = db.UserTypeModels.Where(x => x.USERTYPE == reqBlacklistVm.USERTYPE).OrderByDescending(y => y.ID)
                                   .Select(x => x.GENERATED_LIMIT)
                                   .ToList();
                    if ((reqbookinglen + takecount) < count[0])
                    {
                        ViewBag.Error = "Error";
                        var needlen = count[0] - takecount;
                        ModelState.AddModelError("BlacklistNo", "length must be exactly " + needlen + " digit!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBlacklistVm);
                    }
                    var bindblacklist = reqBlacklistVm.PrefixBooking + reqBlacklistVm.BlacklistNo;
                    #region duplicate Blacklist 
                    if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == bindblacklist.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ViewBag.Error = "Error";
                        ModelState.AddModelError("BlacklistNo", "Duplicate Black list Number!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBlacklistVm);
                    }
                    if (db.IconicBookingModels.Where(c => c.BOOKINGNUMBER.ToLower().Trim().Replace(" ", "") == bindblacklist.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ViewBag.Error = "Error";
                        ModelState.AddModelError("BlacklistNo", "This Number is already used as booking Number!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBlacklistVm);
                    }
                    #endregion
                    #region DataPreparation
                    IconicBlacklistModel dbModel = new IconicBlacklistModel();
                    dbModel.CREATEDDATE = DateTime.Now;
                    dbModel.CREATEDUSERID = User.Identity.GetUserId();
                    dbModel.BlackListNumber = reqBlacklistVm.PrefixBooking + reqBlacklistVm.BlacklistNo;
                    dbModel.Description = reqBlacklistVm.Description;
                    #endregion
                    db.IconicBlacklistModels.Add(dbModel);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    ViewBag.UserType = GetUserTypeList();
                    ViewBag.Booking = "\"" + dbModel.BlackListNumber + "\"";

                }
                ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                return View(reqBlacklistVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: IconicBlacklistModels/Edit/5
        public ActionResult Edit(int? id)
        {
            IconicBlacklistVM editBlacklist = new IconicBlacklistVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var info = db.IconicBlacklistModels.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            editBlacklist.BlacklistNo = info.BlackListNumber;
            editBlacklist.Description = info.Description;
            editBlacklist.ID = info.ID;
            return View(editBlacklist);
        }

        // POST: IconicBlacklistModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IconicBlacklistVM reqBlacklistVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == reqBlacklistVm.BlacklistNo.ToLower().Trim().Replace(" ", "" ) && c.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("BlacklistNo", "Duplicate Black list Number!");
                        return View(reqBlacklistVm);
                    }
                    #endregion
                    #region DataPreparation

                    IconicBlacklistModel dbModel = new IconicBlacklistModel();
                    var dataFromDB = from s in db.IconicBlacklistModels
                                     where s.DEL_FLAG == false
                                     select s;
                    dbModel = dataFromDB.SingleOrDefault(m => m.ID == reqBlacklistVm.ID);

                    dbModel.UpdatedDate = DateTime.Now;
                    dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                    dbModel.BlackListNumber = reqBlacklistVm.BlacklistNo;
                    dbModel.Description = reqBlacklistVm.Description;
                    dbModel.ID = reqBlacklistVm.ID;
                    #endregion
                    db.Entry(dbModel).State = EntityState.Modified;
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{dbModel.BlackListNumber} is updated.");
                    ViewBag.EditBooking = "\"" + dbModel.BlackListNumber + "\"";

                }
                return View(reqBlacklistVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: IconicBlacklistModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IconicBlacklistModel iconicBlacklistModel = db.IconicBlacklistModels.Find(id);
            if (iconicBlacklistModel == null)
            {
                return HttpNotFound();
            }
            return View(iconicBlacklistModel);
        }

        // POST: IconicBlacklistModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                IconicBlacklistModel blacklistInfo = db.IconicBlacklistModels.Find(id);
                string Blacklist = blacklistInfo.BlackListNumber.ToLower();
                blacklistInfo.DEL_FLAG = true;
                //db.CIFInfoModels.Remove(cIFInfoModel);

                db.IconicBookingModels
                      .Where(a => a.BOOKINGNUMBER.Contains(Blacklist))
                      .ToList()
                      .ForEach(a => a.DEL_FLAG = true);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(ex);
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

        private IEnumerable<SelectListItem> GetUserTypeList()
        {
            try
            {
                List<UserTypeModel> lstlist = db.UserTypeModels.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> usertypelist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.USERTYPE_CODE_LIMIT.ToString(),
                    Text = b.USERTYPE

                });
                return usertypelist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }

        public JsonResult GetUserTypeLimit(string usertype)
        {
            try
            {
                List<UserTypeModel> lstlist = db.UserTypeModels.Where(b => b.USERTYPE == usertype).ToList();

                UserTypeModel response = new UserTypeModel();
                response.USERTYPE_CODE_LIMIT = lstlist[0].USERTYPE_CODE_LIMIT;
                response.GENERATED_LIMIT = lstlist[0].GENERATED_LIMIT;


                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
    }
}
