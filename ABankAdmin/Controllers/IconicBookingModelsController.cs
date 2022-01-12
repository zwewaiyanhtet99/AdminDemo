using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class IconicBookingModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "IconicBookingModels";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        // GET: IconicBookingModels
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
        // GET: IconicBookingModels/Details/5
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
        // GET: IconicBookingModels/Create
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

        // POST: IconicBookingModels/Create
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

        // GET: IconicBookingModels/Edit/5
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
            return View(editBooking);
        }

        // POST: IconicBookingModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IconicBookingVM reqBookingVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region duplicate Booking 
                    if (db.IconicBookingModels.Where(c => c.BOOKINGNUMBER.ToLower().Trim().Replace(" ", "") == reqBookingVm.BookingNo.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("BookingNo", "Duplicate Booking Number!");
                        return View(reqBookingVm);
                    }
                    #endregion

                    #region exist number in Blacklist 
                    if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == reqBookingVm.BookingNo.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                    {
                        ModelState.AddModelError("BookingNo", "This number is Black list number!");
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
                    dbModel.ID = reqBookingVm.ID;
                    #endregion
                    db.Entry(dbModel).State = EntityState.Modified;
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{dbModel.BOOKINGNUMBER} is updated.");
                    ViewBag.EditBooking = "\"" + dbModel.BOOKINGNUMBER + "\"";

                }
                return View(reqBookingVm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: IconicBookingModels/Delete/5
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

        // POST: IconicBookingModels/Delete/5
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
            //IconicBookingModel iconicBookingModel = db.IconicBookingModels.Find(id);
            //db.IconicBookingModels.Remove(iconicBookingModel);
            //db.SaveChanges();
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region bulkbooking
        public ActionResult BulkIconicBooking()
        {
            try
            {
                IconicBookingBulkViewModel iconicInfo = new IconicBookingBulkViewModel();
                ViewBag.UserType = GetUserTypeList();
                return View(iconicInfo);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult BulkIconicBookingValidation(IconicBookingBulkViewModel model)
        {
            List<IconicBookingBulkItemResponse> itemList = new List<IconicBookingBulkItemResponse>();
            IconicBookingBulkValidationViewModel viewModel = new IconicBookingBulkValidationViewModel();
            HttpPostedFileBase file = model.File;
            viewModel.MessageCount = 0;
            if (file.ContentLength <= 0 || string.IsNullOrEmpty(file.FileName))
            {
                return View();
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            try
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    if (noOfCol != 2)
                    {
                        ViewBag.ErrorMessage = "Your uploaded file is not valid.";
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value == model.USERTYPE);
                        return View("BulkIconicBooking");
                    }
                    string columntitle1 = workSheet.Cells[1, 1].Value.ToString();
                    string columntitle2 = workSheet.Cells[1, 2].Value.ToString();
                    if (!columntitle1.Equals("THE ICONIC Reserved Number")  || !columntitle2.Equals("Description") )
                    {
                        ViewBag.ErrorMessage = "Your uploaded file is not valid.";
                        ViewBag.UserType = GetUserTypeList().OrderByDescending(y => y.Value == model.USERTYPE);
                        return View("BulkIconicBooking");
                    }
                    
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var item = new IconicBookingBulkItemResponse();

                        item.BookingNo = (null != workSheet.Cells[rowIterator, 1].Value
                            ? workSheet.Cells[rowIterator, 1].Value.ToString()
                            : "Required");
                        
                        item.Description = (null != workSheet.Cells[rowIterator, 2].Value
                            ? workSheet.Cells[rowIterator, 2].Value.ToString()
                            : "");
                        item.USERTYPECODE = model.USERTYPECODE;
                        item.USERTYPE = model.USERTYPE;
                        item.Message = "Success";
                        if (item.Description.Length > 500)
                        {
                            item.Message = "Description length is too long";
                            viewModel.MessageCount++;
                        }
                        if (String.IsNullOrEmpty(item.BookingNo) || item.BookingNo == "Required")
                        {
                            item.Message = "THE ICONIC Reserved Number is Required";
                            viewModel.MessageCount++;
                        }
                        else if (!IsItOnlyNumbers(item.BookingNo) )
                        {
                            item.Message = "THE ICONIC Reserved Number Must be number";
                            viewModel.MessageCount++;
                        }
                        else
                        {
                            #region checkbookinglength
                            var bookinglimit = db.UserTypeModels.Where(x => x.USERTYPE == model.USERTYPE).OrderByDescending(y => y.ID)
                                      .Select(x => x.GENERATED_LIMIT)
                                      .ToList();
                            var limit = bookinglimit[0] - model.USERTYPECODE.Length;
                            if (item.BookingNo.Length > limit || item.BookingNo.Length < limit)
                            {
                                item.Message = "THE ICONIC Reserved Number length is invalid";
                                viewModel.MessageCount++;
                            }
                            #endregion

                            #region checkalreadybooking

                            var bindbooking = item.USERTYPECODE + item.BookingNo;
                            if (db.IconicBookingModels.Where(c => c.BOOKINGNUMBER.ToLower().Trim().Replace(" ", "") == bindbooking.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                            {
                                item.Message = "Duplicate THE ICONIC Reserved Number";
                                viewModel.MessageCount++;

                            }
                            #endregion

                            #region checkalreadyBlacklist 
                            if (db.IconicBlacklistModels.Where(c => c.BlackListNumber.ToLower().Trim().Replace(" ", "") == bindbooking.ToLower().Trim().Replace(" ", "") && c.DEL_FLAG == false).Count() != 0)
                            {
                                item.Message = "Duplicate THE ICONIC Reserved Number";
                                viewModel.MessageCount++;

                            }
                            #endregion

                            #region checkalreadyCIFinformation

                            if (db.CIFInfoModels.Where(c => c.USERTYPECODE.ToLower().Trim().Replace(" ", "") == bindbooking.ToLower().Trim().Replace(" ", "") && c.DELFLAG == false).Count() != 0)
                            {
                                item.Message = "This number is already used";
                                viewModel.MessageCount++;

                            }

                            #endregion
                           
                        }
                        itemList.Add(item);
                    }
                    if (itemList.Count > 0)
                    {
                        viewModel.BulkList = itemList;
                        TempData["IconicBulkInfo"] = JsonConvert.SerializeObject(viewModel);
                        return View(viewModel);
                    }
                }
                return View();

            }
            catch (Exception ex)
            {
                return View();
            }

        }

        private static bool IsItOnlyNumbers(string booking)
        {
            return !String.IsNullOrEmpty(booking) && booking.All(char.IsDigit);
        }

        [HttpPost]
        public ActionResult BulkIconicBookingConfirm()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            try
            {
                var data = new IconicBookingBulkValidationViewModel();
                data = JsonConvert.DeserializeObject<IconicBookingBulkValidationViewModel>(TempData["IconicBulkInfo"].ToString());


                if (data.BulkList.Count > 0)
                {
                    foreach (var item in data.BulkList)
                    {


                        #region DataPreparation

                        IconicBookingModel dbModel = new IconicBookingModel();
                        dbModel.CREATEDDATE = DateTime.Now;
                        dbModel.CREATEDUSERID = User.Identity.GetUserId();
                        dbModel.BOOKINGNUMBER = item.USERTYPECODE + item.BookingNo;
                        dbModel.Description = item.Description;

                        #endregion

                        db.IconicBookingModels.Add(dbModel);
                        db.SaveChanges();
                        //save log
                        log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    }
                }
                ViewBag.Booking = "Success";
            
                return View();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
