using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class Number_ManagementController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "Number_Management";
        // GET: Number_Management
        public ActionResult Index()
        {
            try
            {
                var data = db.IconicBookingModels.ToList();
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
            //find search columns info
            var booking = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var description = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.IconicBookingModels.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new IconicBookingVM
            {

                ID = u.ID,
                BookingNo = u.BOOKINGNUMBER,
                Description = u.Description,
                InUse = u.InUse,
                Createduserid = u.CREATEDUSERID

            });

            //SEARCHING...

            if (!string.IsNullOrEmpty(booking.Trim()))
            {
                user = user.Where(a => a.BookingNo.ToLower().Contains(booking.Trim().ToLower())).ToList();
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
        // GET: Number_Management/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IconicBookingModel iconicBookingModel = db.IconicBookingModels.Find(id);
            if (iconicBookingModel == null)
            {
                return HttpNotFound();
            }
            return View(iconicBookingModel);
        }
        //getting usertype list
        [NonAction]
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
        [NonAction]
        private IEnumerable<SelectListItem> GetUserTypeListForEdit(int usertypeid)
        {
            try
            {
                List<UserTypeModel> lstlist = db.UserTypeModels.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> usertypelist = lstlist.Select(b => new SelectListItem
                {
                    Selected = (b.ID == usertypeid),
                    Value = b.USERTYPE_CODE_LIMIT.ToString(),
                    Text = b.USERTYPE

                });
                return usertypelist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetUserTypeListForEdit", ex);
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
                log.Error(User.Identity.GetUserId(), controllerName, "GetUserTypeLimit", ex);
                return null;
            }
        }
        // GET: Number_Management/Create
        public ActionResult Create()
        {
            try
            {
                IconicBookingVM iconicInfo = new IconicBookingVM();
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

        // POST: Number_Management/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IconicBookingVM reqBookingVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {

                    string takecountstr = reqBookingVm.PrefixBooking.Replace("-", "");
                    int takecount = takecountstr.Length;
                    int reqbookinglen = Convert.ToInt16(reqBookingVm.BookingNo.Length);
                    var count = db.UserTypeModels.Where(x => x.USERTYPE == reqBookingVm.USERTYPE).OrderByDescending(y => y.ID)
                                   .Select(x => x.GENERATED_LIMIT)
                                   .ToList();
                    if ((reqbookinglen + takecount) < count[0])
                    {
                        ViewBag.Error = "Error";
                        var needlen = count[0] - takecount;
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        ModelState.AddModelError("BookingNo", "length must be exactly " + needlen + " digit!");
                        return View(reqBookingVm);
                    }
                    var bindbookinglist = reqBookingVm.PrefixBooking + reqBookingVm.BookingNo;
                    #region duplicate Booking 
                    if (db.IconicBookingModels.Where(c => c.BOOKINGNUMBER.ToLower().Trim().Replace(" ", "") == bindbookinglist.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ViewBag.Error = "Error";
                        ModelState.AddModelError("BookingNo", "Duplicate Booking Number!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBookingVm);
                    }
                    #endregion

                    #region exist number in Blacklist 
                    if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == bindbookinglist.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ViewBag.Error = "Error";
                        ModelState.AddModelError("BookingNo", "This number is Black list number!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBookingVm);
                    }
                    #endregion
                    #region exist number in CIFinformation
                    if (db.CIFInfoModels.Where(c => c.USERTYPECODE.ToLower().Trim().Replace(" ", "") == bindbookinglist.ToLower().Trim().Replace(" ", "") && c.DELFLAG == false).Count() != 0)
                    {
                        ViewBag.Error = "Error";
                        ModelState.AddModelError("BookingNo", "This number is already used!");
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                        return View(reqBookingVm);
                    }
                    #endregion
                    #region DataPreparation
                    IconicBookingModel dbModel = new IconicBookingModel();
                    dbModel.CREATEDDATE = DateTime.Now;
                    dbModel.CREATEDUSERID = User.Identity.GetUserId();
                    dbModel.BOOKINGNUMBER = reqBookingVm.PrefixBooking + reqBookingVm.BookingNo;
                    dbModel.Description = reqBookingVm.Description;
                    int usertypecodelimit = Convert.ToInt32(reqBookingVm.USERTYPECODE);
                    dbModel.UserTypeID = (from s in db.UserTypeModels where s.USERTYPE == reqBookingVm.USERTYPE && s.DEL_FLAG == false && s.USERTYPE_CODE_LIMIT == usertypecodelimit select s.ID).SingleOrDefault();
                    #endregion
                    db.IconicBookingModels.Add(dbModel);
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    ViewBag.Booking = "\"" + reqBookingVm.BookingNo + "\"";
                    //ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Text);

                }
                ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value);
                return View(reqBookingVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

        }

        // GET: Number_Management/Edit/5
        public ActionResult Edit(int? id)
        {
            IconicBookingVM editBooking = new IconicBookingVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var info = db.IconicBookingModels.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }

            editBooking.BookingNo = info.BOOKINGNUMBER;
            editBooking.Description = info.Description;
            editBooking.ID = info.ID;
            editBooking.PrefixBooking = (from s in db.UserTypeModels where s.ID == info.UserTypeID && s.DEL_FLAG == false select s.USERTYPE_CODE_LIMIT).SingleOrDefault().ToString();
            editBooking.USERTYPE = (from s in db.UserTypeModels where s.ID == info.UserTypeID && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();
            ViewBag.UserType = GetUserTypeListForEdit(info.UserTypeID);
            return View(editBooking);
        }

        // POST: Number_Management/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IconicBookingVM reqBookingVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                int usertypecodelimit = Convert.ToInt32(reqBookingVm.USERTYPECODE);
                int UserTypeID = (from s in db.UserTypeModels where s.USERTYPE == reqBookingVm.USERTYPE && s.DEL_FLAG == false && s.USERTYPE_CODE_LIMIT == usertypecodelimit select s.ID).SingleOrDefault();

                if (ModelState.IsValid)
                {
                    //string bookingno = reqBookingVm.PrefixBooking + reqBookingVm.BookingNo;
                    //string n = reqBookingVm.BookingNo.Substring(reqBookingVm.PrefixBooking.Length,reqBookingVm.BookingNo.Length - reqBookingVm.PrefixBooking.Length);
                    #region duplicate Booking 
                    if (db.IconicBookingModels.Where(c => c.BOOKINGNUMBER.ToLower().Trim().Replace(" ", "") == reqBookingVm.BookingNo.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false && c.ID != reqBookingVm.ID).Count() != 0)
                    {
                        ViewBag.UserType = GetUserTypeListForEdit(UserTypeID);
                        ModelState.AddModelError("BookingNo", "Duplicate Booking Number!");
                        return View(reqBookingVm);
                    }
                    #endregion

                    #region exist number in Blacklist 
                    if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == reqBookingVm.BookingNo.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ViewBag.UserType = GetUserTypeListForEdit(UserTypeID);
                        ModelState.AddModelError("BookingNo", "This number is Black list number!");
                        return View(reqBookingVm);
                    }
                    #endregion
                    #region check number in Used 
                    if (db.IconicBookingModels.Where(c => c.ID== reqBookingVm.ID && c.DEL_FLAG == false && c.InUse==true).Count() != 0)
                    {
                        ViewBag.UserType = GetUserTypeListForEdit(UserTypeID);
                        ModelState.AddModelError("BookingNo", "This number can't be ediable!It is used.");
                        return View(reqBookingVm);
                    }
                    #endregion

                    #region DataPreparation

                    IconicBookingModel dbModel = new IconicBookingModel();
                    var dataFromDB = from s in db.IconicBookingModels
                                     where s.DEL_FLAG == false
                                     select s;
                    dbModel = dataFromDB.SingleOrDefault(m => m.ID == reqBookingVm.ID);

                    dbModel.UpdatedDate = DateTime.Now;
                    dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                    dbModel.BOOKINGNUMBER = reqBookingVm.BookingNo; 
                    dbModel.Description = reqBookingVm.Description;
                    int usertypelimit = Convert.ToInt32(reqBookingVm.USERTYPECODE);
                    dbModel.UserTypeID = (from s in db.UserTypeModels where s.USERTYPE == reqBookingVm.USERTYPE && s.DEL_FLAG == false && s.USERTYPE_CODE_LIMIT == usertypelimit select s.ID).SingleOrDefault();

                    dbModel.ID = reqBookingVm.ID;
                    #endregion
                    var localEntity = db.Set<IconicBookingModel>().Local.FirstOrDefault(f => f.ID == dbModel.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    // save log
                    string controllerName = "Number_Management";
                    string getuserid = User.Identity.GetUserId();
                    db.Entry(dbModel).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            log.Info(getuserid, controllerName, "Edit", 2, eve.Entry.Entity.GetType().Name);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.Info(getuserid, controllerName, "Edit", 2, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }


                    //save log
                   
                    log.Info(getuserid, controllerName, actionName, 2, $"{dbModel.BOOKINGNUMBER} is updated.");
                    ViewBag.EditBooking = "\"" + dbModel.BOOKINGNUMBER + "\"";

                }
                ViewBag.UserType = GetUserTypeListForEdit(UserTypeID);
                return View(reqBookingVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Number_Management/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IconicBookingModel iconicBookingModel = db.IconicBookingModels.Find(id);
            if (iconicBookingModel == null)
            {
                return HttpNotFound();
            }
            return View(iconicBookingModel);
        }

        // POST: Number_Management/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                IconicBookingModel bookingInfo = db.IconicBookingModels.Find(id);
                string Booking = bookingInfo.BOOKINGNUMBER.ToLower();
                bookingInfo.DEL_FLAG = true;
                //db.CIFInfoModels.Remove(cIFInfoModel);

                db.IconicBookingModels
                      .Where(a => a.BOOKINGNUMBER.Contains(Booking))
                      .ToList()
                      .ForEach(a => a.DEL_FLAG = true);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw (ex);
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
