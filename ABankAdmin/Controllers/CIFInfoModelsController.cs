using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using CronNET;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CIFInfoModelsController : Controller
    {
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "CIFInfoModels";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        // GET: CIFInfoModels
        public ActionResult Index()
        {
            try
            {
                TempData["createReturn"] = string.Empty;
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.CIFInfoModels.ToList());
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
            var sortColumn =
                Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                       + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var cifid = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var username = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID); //Only show not pending register and reject status,deactivate not approved,upgrade not pending in listing
            var user =
                db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status == 1 || a.Status == 3 || a.Status == 4 || a.Status == 6) && a.DeactivateStatus != 2 & a.UpgradeStatus != 1 && a.DowngradeStatus != 1)
                    .ToList()
                    .ConvertAll(u => new CIFInfoVM
                    {
                        PHONENO = u.PHONENO,
                        NAME = u.NAME,
                        USERTYPE = u.USERTYPE,
                        USERTYPECODE = u.USERTYPECODE,
                        EXPIREDATE = u.EXPIREDATE,
                        strEXPIREDATE = String.Format("{0:MM/yy}", u.EXPIREDATE),
                        NRC = u.NRC,
                        Address = u.ADDRESS,
                        CIFID = u.CIFID,
                        ID = u.ID,
                        REMARK = u.REMARK,
                        UpdatedDate = u.UpdatedDate,
                        Createduserid = u.CREATEDUSERID,
                        Status = u.Status.ToString()
                    });

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                user = user.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(username.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                user = user.Where(a => a.NAME.ToLower().Contains(username.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //user = user.Where(a => a.USER_TYPE.Contains(usertype)).ToList();
                user = user.Where(a => a.USERTYPE == usertype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            foreach (var item in user)
            {
                if (item.Createduserid != null)
                {
                    item.Createduserid =
                        db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                }
                if (item.UpdatedDate != null)
                {
                    DateTime Updated = Convert.ToDateTime(item.UpdatedDate);
                    item.strUpdatedDate = Updated.ToString("dd-MM-yyyy");
                }
                else
                {
                    item.strUpdatedDate = null;
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

        // GET: CIFInfoModels/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result1 = from c in db.CIFInfoModels select c;
                CIFInfoModel cif = result1.SingleOrDefault(m => m.ID == id);
                string CIFID = cif.CIFID;

                CIFInfoVM cifvm = new CIFInfoVM();
                cifvm.CIFID = cif.CIFID;
                cifvm.NAME = cif.NAME;
                cifvm.CIFID = cif.CIFID;
                cifvm.Address = cif.ADDRESS;
                cifvm.USERTYPE = cif.USERTYPE;
                cifvm.USERTYPECODE = cif.USERTYPECODE;
                cifvm.PHONENO = cif.PHONENO;
                cifvm.REMARK = cif.REMARK;
                cifvm.EXPIREDATE = (DateTime)cif.EXPIREDATE;
                cifvm.NRC = cif.NRC;
                cifvm.strEXPIREDATE = cif.EXPIREDATE.ToString("dd-MM-yyyy");

                var AccInfos = (from a in db.Acct_InfoModels
                                where a.CIFID.Contains(CIFID) && a.ACCT_CLOSE_FLAG == false
                                orderby a.CreatedDateTime ascending
                                select new ViewModels.AccountVM
                                {
                                    CIFID = a.CIFID,
                                    ACCOUNTNO = a.ACCOUNTNO,
                                    SCHM_TYPE = a.SCHM_TYPE,
                                    SCHM_CODE = a.SCHM_CODE,
                                    AvailableAmt = a.AVAI_BALANCE.ToString(),
                                    CURRENCY = a.CURRENCY,
                                    BRANCHCODE = a.BRANCHCODE
                                });

                List<ViewModels.AccountVM> accvms = AccInfos.ToList();

                CIFInfo cIF = new CIFInfo();
                cIF.uservm = cifvm;


                if (cif == null)
                {
                    return HttpNotFound();
                }
                return View(cIF);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View(ex);
            }
        }

        //getting usertype list
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
                log.Error(User.Identity.GetUserId(), controllerName, "GetUserTypeList", ex);
                return null;
            }
        }

        //getting RM list
        public static IEnumerable<SelectListItem> GetRMList()
        {
            AdminDBContext db = new AdminDBContext();
            EventLogController log = new EventLogController();
            try
            {
                List<RM_Info> lstlist = db.RM_Info.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> rmlist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.Name

                });
                return rmlist;
            }
            catch (Exception ex)
            {
                log.Error("", "CIFInfoModels", "GetRMList", ex);
                return null;
            }
        }

        //getting Booking list
        private IEnumerable<SelectListItem> GetBookingList()
        {
            try
            {
                List<IconicBookingModel> lstlist =
                    db.IconicBookingModels.Where(b => b.DEL_FLAG == false && b.InUse == false).ToList();
                IEnumerable<SelectListItem> bookinglist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.BOOKINGNUMBER.ToString(),
                    Text = b.BOOKINGNUMBER

                });
                return bookinglist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBookingList", ex);
                return null;
            }
        }

        //getting Approver email list
        public static IEnumerable<SelectListItem> GetApproverEmailList(string userID)
        {
            AdminDBContext db = new AdminDBContext();
            EventLogController log = new EventLogController();
            try
            {

                var getBranch =
                    (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s).SingleOrDefault();
                int branchid = getBranch.BranchID;
                List<AdminUser> lstemaillist =
                    db.Users.Where(b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 4)
                        .ToList();
                IEnumerable<SelectListItem> bookinglist = lstemaillist.Select(b => new SelectListItem
                {
                    Value = b.Email.ToString(),
                    Text = b.Email

                });

                return bookinglist;
            }
            catch (Exception ex)
            {
                log.Error("", "CIFInfoModels", "GetApproverEmailList", ex);
                return null;
            }
        }

        private IEnumerable<SelectListItem> GetBookingListByIconic(string iconic)
        {
            try
            {
                List<IconicBookingModel> lstlist =
                    db.IconicBookingModels.Where(
                        b => b.DEL_FLAG == false && b.InUse == false && b.BOOKINGNUMBER.StartsWith(iconic)).ToList();
                IEnumerable<SelectListItem> bookinglist = lstlist.Select(b => new SelectListItem
                {
                    Value = b.BOOKINGNUMBER.ToString(),
                    Text = b.BOOKINGNUMBER

                });
                return bookinglist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }

        public JsonResult GetBookingListByIconicType(string iconic)
        {
            try
            {
                List<SelectListItem> item = new List<SelectListItem>();
                var getID =
                    (from s in db.UserTypeModels where s.USERTYPE == iconic && s.DEL_FLAG == false select s).SingleOrDefault();
                if (getID != null)
                {
                    List<IconicBookingModel> lstlist =
                    db.IconicBookingModels.Where(
                        b => b.DEL_FLAG == false && b.InUse == false && b.BOOKINGNUMBER.StartsWith(getID.USERTYPE_CODE_LIMIT.ToString()) && b.UserTypeID == getID.ID).ToList();

                    item.Add(new SelectListItem { Text = "Select Reserved Number", Value = "" });
                    if (lstlist.Count > 0)
                    {
                        foreach (var x in lstlist)
                        {
                            item.Add(new SelectListItem { Text = x.BOOKINGNUMBER, Value = x.BOOKINGNUMBER.ToString() });
                        }
                    }


                }
                return Json(new SelectList(item, "Value", "Text", JsonRequestBehavior.AllowGet));


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
        public JsonResult GetBookingListByIconicTypeForUpgrade(string iconic)
        {
            try
            {
                var new_usertypeInfo = (from s in db.UserTypeModels where s.USERTYPE != iconic && s.DEL_FLAG == false && s.Type == "Tier 1" select s).SingleOrDefault();

                List<SelectListItem> item = new List<SelectListItem>();
                List<IconicBookingModel> lstlist =
                    db.IconicBookingModels.Where(
                        b => b.DEL_FLAG == false && b.InUse == false && b.BOOKINGNUMBER.StartsWith(new_usertypeInfo.USERTYPE_CODE_LIMIT.ToString()) && b.UserTypeID == new_usertypeInfo.ID).ToList();

                item.Add(new SelectListItem { Text = "Select Reserved Number", Value = "" });
                if (lstlist.Count > 0)
                {
                    foreach (var x in lstlist)
                    {
                        item.Add(new SelectListItem { Text = x.BOOKINGNUMBER, Value = x.BOOKINGNUMBER.ToString() });
                    }
                }
                return Json(new SelectList(item, "Value", "Text", JsonRequestBehavior.AllowGet));



            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
        private RetailResponseInfo GetRetailResponse(string CIFID)
        {
            RetailResponseInfo rri = new RetailResponseInfo();
            try
            {
                CustomerInformationInquiry custInfoInquiry = new CustomerInformationInquiry();

                rri = custInfoInquiry.SelectCustomerInformation(CIFID, connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rri;
        }

        //Fetch with CIFID
        public JsonResult getRetail(string CIFID)
        {
            RetailResponseInfo rri = new RetailResponseInfo();
            RetailResponseInfo_VM rri_VM = new RetailResponseInfo_VM();
            try
            {
                if (CIFID != "")
                {
                    int count = db.CIFInfoModels.Where(u => u.CIFID == CIFID & u.DELFLAG == false).Count();
                    if (count > 0)
                        return Json(new { message = "Duplicate CIFID" }, JsonRequestBehavior.AllowGet);
                    rri = GetRetailResponse(CIFID);
                    rri_VM.Name = rri.Name;
                    rri_VM.Address = rri.Address;
                    rri_VM.Currency = rri.Currency;
                    rri_VM.Manager = rri.Manager;
                    rri_VM.LeadSource = rri.LeadSource;
                    rri_VM.PhoneNumber = rri.PhoneNumber;
                    rri_VM.Minor = rri.Minor;
                    rri_VM.Gender = rri.Gender;
                    rri_VM.NRC = rri.NRC;
                    rri_VM.lAcctInfo = rri.lAcctInfo;
                    rri_VM.PrimarySolID = rri.PrimarySolID;
                    if (!string.IsNullOrEmpty(rri.PrimarySolID))
                    {
                        var branch = db.Branches.Where(x => x.code == rri.PrimarySolID && x.DEL_FLAG == false).ToList();
                        rri_VM.BranchName = branch[0].name;
                    }

                    decimal totatamount = (decimal)0.0;
                    for (int i = 0; i < rri.lAcctInfo.Count; i++)
                    {
                        totatamount += decimal.Parse(rri.lAcctInfo[i].AvailableAmt.ToString());
                    }

                    rri_VM.TotalAmt = totatamount.ToString();
                    if (rri.NRC != null && rri.lAcctInfo.Count == 0)
                        return Json(new { message = "No individual account under this CIFID" },
                            JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(rri_VM, JsonRequestBehavior.AllowGet);
        }

        // GET: CIFInfoModels/Create
        public ActionResult Create()
        {
            try
            {
                CIFInfo cifinfo = new CIFInfo();
                if (TempData["createReturn"] == "1")
                {
                    if (Session["CIFVMCreateReturn"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVMCreateReturn"].ToString()))
                        {
                            cifinfo = JsonConvert.DeserializeObject<CIFInfo>(Session["CIFVMCreateReturn"].ToString());
                            if (cifinfo.uservm.Iconic == true)
                            {
                                if (cifinfo.uservm.IconicBooking != null)
                                {
                                    IEnumerable<SelectListItem> items =
                                        db.IconicBookingModels.Where(b => b.DEL_FLAG == false && b.InUse == false)
                                            .Select(c => new SelectListItem
                                            {
                                                Selected = (c.BOOKINGNUMBER == cifinfo.uservm.IconicBooking),
                                                Value = c.BOOKINGNUMBER.ToString(),
                                                Text = c.BOOKINGNUMBER
                                            }).ToList();
                                    ViewBag.Booking = items;
                                }


                            }
                            else
                            {
                                ViewBag.Booking = GetBookingList();
                            }
                            if (cifinfo.uservm.USERTYPE != null)
                            {
                                cifinfo.uservm.USERTYPECODE = null;
                                int usertypecodelimit = (from s in db.UserTypeModels
                                                         where s.USERTYPE == cifinfo.uservm.USERTYPE && s.DEL_FLAG == false
                                                         select s.USERTYPE_CODE_LIMIT).SingleOrDefault();

                                IEnumerable<SelectListItem> items1 = db.UserTypeModels.Where(b => b.DEL_FLAG == false)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.USERTYPE_CODE_LIMIT.ToString() == usertypecodelimit.ToString()),
                                        Value = c.USERTYPE_CODE_LIMIT.ToString(),
                                        Text = c.USERTYPE
                                    }).ToList();

                                ViewBag.UserType = items1;
                            }
                            else
                            {
                                ViewBag.UserType = GetUserTypeList();
                            }
                            if (cifinfo.uservm.RMID != null)
                            {
                                IEnumerable<SelectListItem> items2 = db.RM_Info.Where(b => b.DEL_FLAG == false)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.ID == cifinfo.uservm.RMID),
                                        Value = c.ID.ToString(),
                                        Text = c.Name
                                    }).ToList();

                                ViewBag.RMList = items2;

                            }
                            else
                            {
                                ViewBag.RMList = GetRMList();
                            }


                        }
                    }
                }
                else
                {

                    cifinfo.uservm = new CIFInfoVM();
                    ViewBag.UserType = GetUserTypeList();
                    ViewBag.RMList = GetRMList();
                    DateTime today = DateTime.Now;
                    DateTime addDate = today.AddDays(365);
                    DateTime addDate1 = today.AddDays(30);
                    cifinfo.uservm.strEXPIREDATE = addDate.ToString("MM/yy");
                    cifinfo.uservm.strEffectiveDate = addDate1.ToString("MM/yy");
                    ViewBag.Booking = GetBookingList();
                }

                return View(cifinfo);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: CIFInfoModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CIFInfo cifinfo)
        {
            TempData["create"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (cifinfo.uservm.Iconic == true)
                    {
                        if (cifinfo.uservm.IconicBooking == null)
                        {
                            ModelState.AddModelError("uservm.IconicBooking", "Please choose reserved number");
                            ViewBag.UserType = GetUserTypeList();
                            ViewBag.Booking = GetBookingListByIconic(cifinfo.uservm.USERTYPECODE);
                            return View(cifinfo);
                        }

                    }
                    CIFInfoVM cifinfovm = cifinfo.uservm;
                    //reget core data by CIFID
                    RetailResponseInfo rri = new RetailResponseInfo();
                    if (cifinfovm.CIFID != "")
                        rri = GetRetailResponse(cifinfovm.CIFID);
                    if (rri.NRC == null) //if wrong CIFID
                    {
                        ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                        ViewBag.UserType = GetUserTypeList();
                        ViewBag.Booking = GetBookingList();
                        return View(cifinfo);
                    }
                    //checking Validation

                    #region Validation

                    //CIFID duplicate
                    if (db.CIFInfoModels.Where(u => u.CIFID == cifinfovm.CIFID & u.DELFLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.CIFID", "Duplicate CIFID");
                        ViewBag.UserType = GetUserTypeList();
                        ViewBag.Booking = GetBookingList();
                        return View(cifinfo);
                    }

                    #endregion

                    #region Data preparation

                    //create new user
                    CIFInfoModel new_user = new CIFInfoModel();

                    new_user.CIFID = cifinfovm.CIFID;
                    new_user.NAME = cifinfovm.NAME;
                    new_user.NRC = cifinfovm.NRC;
                    new_user.USERTYPE = cifinfo.uservm.USERTYPE;
                    new_user.ADDRESS = cifinfovm.Address;
                    new_user.PHONENO = cifinfovm.PHONENO;
                    if (string.IsNullOrEmpty(cifinfovm.REMARK))
                    {
                        cifinfovm.REMARK = "-";
                    }
                    new_user.REMARK = cifinfovm.REMARK;
                    DateTime expdate = DateTime.Now;
                    DateTime effecitvedate = DateTime.Now;
                    var dd = cifinfovm.strEffectiveDate.Replace("/", "-");
                    var ddexp = cifinfovm.strEXPIREDATE.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    expdate = General.ConvertStrToDateForIconic(adddayexp);
                    effecitvedate = General.ConvertStrToDateForIconic(addday);
                    new_user.EXPIREDATE = expdate;
                    new_user.EFFECTIVEDATE = effecitvedate;
                    new_user.CREATEDDATE = DateTime.Now;
                    new_user.CREATEDUSERID = User.Identity.GetUserId();
                    new_user.BRANCHNAME = cifinfovm.BranchName;
                    new_user.RMID = cifinfovm.RMID;
                    new_user.RMName = cifinfovm.RMName;
                    new_user.Status = 0; //Request
                    if (cifinfo.uservm.IconicBooking == null)
                    {
                        new_user.USERTYPECODE = GenereateCodeForIconic(new_user.USERTYPE);
                        if (new_user.USERTYPECODE == "0")
                        {
                            cifinfo.uservm = new CIFInfoVM();
                            ViewBag.UserType = GetUserTypeList();
                            ViewBag.RMList = GetRMList();
                            DateTime today = DateTime.Now;
                            DateTime addDate = today.AddDays(365);
                            DateTime addDate1 = today.AddDays(30);
                            cifinfo.uservm.strEXPIREDATE = addDate.ToString("MM/yy");
                            cifinfo.uservm.strEffectiveDate = addDate1.ToString("MM/yy");
                            ViewBag.Booking = GetBookingList();
                            string getuserid = User.Identity.GetUserId();
                            log.Info(getuserid, controllerName, actionName, 1,
                                "Generated iconic type length is invalid.");
                            ViewBag.GetCode = "\"" + cifinfo.uservm.CIFID + "\"";

                            return View(cifinfo);
                        }

                    }
                    else
                    {
                        new_user.USERTYPECODE = cifinfo.uservm.IconicBooking;
                    }
                    cifinfo.uservm.USERTYPECODE = new_user.USERTYPECODE;
                    cifinfo.uservm.RMID = new_user.RMID;
                    cifinfo.uservm.RMName = new_user.RMName;



                    #endregion
                    //save log
                    Session["CIFVM"] = string.Empty; //To genereate PDF
                    Session["CIFVM"] = JsonConvert.SerializeObject(cifinfo.uservm);
                    Session["CIFVMCreateReturn"] = JsonConvert.SerializeObject(cifinfo);
                    return RedirectToAction("CIF_ConfirmPage", 1);
                }

                return View();


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        public static string GetDateTime(string l_Date, string l_SplitString)
        {
            string[] l_DateArr = l_Date.Split(new string[] { l_SplitString }, StringSplitOptions.None);
            return l_DateArr[2] + "-" + l_DateArr[1] + "-" + l_DateArr[0];

        }

        public ActionResult CIF_ConfirmPage()
        {
            TempData["createReturn"] = "1";
            CIFInfoVM reqmodel = new CIFInfoVM();
            try
            {
                if (TempData["create"] == "1")
                {
                    if (Session["CIFVM"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVM"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<CIFInfoVM>(Session["CIFVM"].ToString());
                        }
                    }
                }
                else if (TempData["edit"] == "2")
                {
                    if (Session["CIFEditVM"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFEditVM"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<CIFInfoVM>(Session["CIFEditVM"].ToString());
                        }
                    }
                }
                string userID = User.Identity.GetUserId();
                ViewBag.Emaillist = GetApproverEmailList(userID);
                reqmodel.UserEmail =
                    (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(reqmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CIF_ConfirmPage(CIFInfoVM reqVM)
        {
            try
            {
                Session["CIFVM"] = string.Empty;
                if (ModelState.IsValid)
                {
                    #region Data preparation

                    //create new user
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp.Status = 0;//Request register pending
                    DbInfoForTemp.CREATEDDATE = DateTime.Now;
                    DbInfoForTemp.CREATEDUSERID = User.Identity.GetUserId();
                    DateTime expdate = DateTime.Now;
                    DateTime effecitvedate = DateTime.Now;
                    var dd = reqVM.strEffectiveDate.Replace("/", "-");
                    var ddexp = reqVM.strEXPIREDATE.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    expdate = General.ConvertStrToDateForIconic(adddayexp);
                    effecitvedate = General.ConvertStrToDateForIconic(addday);
                    DbInfoForTemp.EXPIREDATE = expdate;
                    DbInfoForTemp.EFFECTIVEDATE = effecitvedate;
                    DbInfoForTemp.UpgradeStatus = 0;
                    DbInfoForTemp.DeactivateStatus = 0;
                    DbInfoForTemp.DowngradeStatus = 0;
                    DbInfoForTemp.DELFLAG = false;
                    DbInfoForTemp.REMARK = reqVM.REMARK;
                    DbInfoForTemp.RMName = reqVM.RMName;
                    DbInfoForTemp.RMID = reqVM.RMID;
                    DbInfoForTemp.CIFID = reqVM.CIFID;
                    DbInfoForTemp.USERTYPE = reqVM.USERTYPE;
                    DbInfoForTemp.USERTYPECODE = reqVM.USERTYPECODE;
                    DbInfoForTemp.PHONENO = reqVM.PHONENO;
                    DbInfoForTemp.NRC = reqVM.NRC;
                    DbInfoForTemp.ADDRESS = reqVM.Address;
                    DbInfoForTemp.NAME = reqVM.NAME;
                    DbInfoForTemp.BRANCHNAME = reqVM.BranchName;
                    DbInfoForTemp.ApproverEmail = reqVM.ApproverEmail;
                    DbInfoForTemp.UserEmail = reqVM.UserEmail;
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();


                    #endregion
                    if (reqVM.IconicBooking != null)
                    {
                        #region DataPreparationToupdateBooking

                        IconicBookingModel dbModel = new IconicBookingModel();
                        var dataFromDB = from s in db.IconicBookingModels
                                         where s.DEL_FLAG == false
                                         select s;
                        dbModel = dataFromDB.SingleOrDefault(m => m.BOOKINGNUMBER == reqVM.IconicBooking);
                        dbModel.UpdatedDate = DateTime.Now;
                        dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                        dbModel.InUse = true;

                        #endregion

                        db.Entry(dbModel).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, "CIF_ConfirmPage", 2, $"{reqVM.CIFID} Creation is pending.");
                    ViewBag.CifCreateInfo = "\"" + reqVM.CIFID + "\"";
                    var getBranch =
                        (from s in db.Users where s.Id == getuserid && s.DEL_FLAG == false select s).SingleOrDefault();
                    int branchid = getBranch.BranchID;
                    IEnumerable<SelectListItem> items2 =
                        db.Users.Where(
                            b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 3)
                            .Select(c => new SelectListItem
                            {
                                Selected = (c.Email == reqVM.ApproverEmail),
                                Value = c.Email.ToString(),
                                Text = c.Email
                            }).ToList();

                    ViewBag.Emaillist = items2;
                    List<string> mailList = new List<string>();
                    mailList.Add(reqVM.UserEmail);
                    mailList.Add(reqVM.ApproverEmail);

                    #region sendEmail

                    bool sendEmail = SendEmailForIconic(mailList, reqVM.CIFID, getuserid, controllerName,
                        "CIF_ConfirmPage");

                    #endregion

                    return View(reqVM);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        private void SaveAccounts(List<AccountVM> accvms, string CIFID, List<Models.Account_Info> core_accs)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                int saveCount = 0;
                foreach (AccountVM accvm in accvms)
                {
                    if (accvm.Active)
                        continue;
                    var core_acc = core_accs.Where(c => c.ACCOUNTNO == accvm.ACCOUNTNO).FirstOrDefault();
                    if (core_acc == null) //invalid acc no
                        continue;
                    saveCount++;
                    Models.Account_Info acc = new Models.Account_Info();
                    //core data
                    acc.ACCOUNTNO = core_acc.ACCOUNTNO;
                    acc.SCHM_TYPE = core_acc.SCHM_TYPE;
                    acc.SCHM_CODE = core_acc.SCHM_CODE;
                    acc.ACC_DESC = core_acc.ACC_DESC;
                    acc.BRANCHCODE = core_acc.BRANCHCODE;
                    acc.CURRENCY = core_acc.CURRENCY;
                    acc.AVAI_BALANCE = core_acc.AVAI_BALANCE;

                    //default values
                    acc.CIFID = CIFID;
                    acc.CreatedDateTime = DateTime.Now;
                    acc.CreatedUserID = User.Identity.GetUserId();
                    acc.ACCT_CLOSE_FLAG = false;

                    db.Acct_InfoModels.Add(acc);
                    db.SaveChanges();
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{saveCount} acounts saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }

        // GET: CIFInfoModels/Edit/5
        public ActionResult Edit(int? id)
        {
            CIFInfoEdit CifInfoVM = new CIFInfoEdit();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var info = db.CIFInfoModels.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            var CIFID = info.CIFID;
            var user = db.Acct_InfoModels.Where(a => a.CIFID == CIFID).ToList().ConvertAll(u => new AccountInfo()
            {
                AccountNumber = u.ACCOUNTNO,
                AccountType = u.SCHM_TYPE,
                BranchID = u.BRANCHCODE,
                Currency = u.CURRENCY,
                Schm_Code = u.SCHM_CODE,
                AvailableAmt = u.AVAI_BALANCE.ToString(),
            });
            double totalamt = 0;
            if (user.Count > 0)
            {
                foreach (var getTotalamt in user)
                {
                    totalamt += Convert.ToDouble(getTotalamt.AvailableAmt);
                }
            }

            CifInfoVM.uservm = JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(info));
            CifInfoVM.accountvms = user.ToList();
            string dateString = String.Format("{0:dd-MM-yyyy}", CifInfoVM.uservm.EXPIREDATE);
            DateTime effectivedateString = DateTime.Now;
            effectivedateString = CifInfoVM.uservm.EffectiveDate;
            DateTime expiredateString = DateTime.Now;
            expiredateString = CifInfoVM.uservm.EXPIREDATE;
            CifInfoVM.uservm.strEXPIREDATE = expiredateString.ToString("MM/yy");
            CifInfoVM.uservm.strEffectiveDate = effectivedateString.ToString("MM/yy");
            return View(CifInfoVM);
        }

        // POST: CIFInfoModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CIFInfoEdit cIFInfoModel)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            TempData["edit"] = "2";
            try
            {
                if (ModelState.IsValid)
                {
                    CIFInfoVM cifinfovm =
                        JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(cIFInfoModel.uservm));

                    #region Data preparation

                    //create new user
                    CIFInfoModel new_user = new CIFInfoModel();
                    new_user.CIFID = cifinfovm.CIFID;
                    new_user.NAME = cifinfovm.NAME;
                    new_user.NRC = cifinfovm.NRC;
                    new_user.USERTYPE = cifinfovm.USERTYPE;
                    new_user.ADDRESS = cifinfovm.Address;
                    new_user.PHONENO = cifinfovm.PHONENO;
                    new_user.REMARK = cifinfovm.REMARK;
                    new_user.USERTYPECODE = cifinfovm.USERTYPECODE;

                    new_user.DELFLAG = false;
                    DateTime expdate = DateTime.Now;
                    DateTime effecitvedate = DateTime.Now;
                    var dd = cifinfovm.strEffectiveDate.Replace("/", "-");
                    var ddexp = cifinfovm.strEXPIREDATE.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    expdate = General.ConvertStrToDateForIconic(adddayexp);
                    effecitvedate = General.ConvertStrToDateForIconic(addday);
                    new_user.EXPIREDATE = expdate;
                    new_user.CREATEDUSERID = User.Identity.GetUserId();
                    new_user.ID = cIFInfoModel.uservm.ID;
                    new_user.EFFECTIVEDATE = effecitvedate;
                    new_user.BRANCHNAME = cifinfovm.BranchName;
                    cIFInfoModel.uservm.USERTYPECODE = new_user.USERTYPECODE;
                    cIFInfoModel.uservm.EXPIREDATE = new_user.EXPIREDATE;

                    #endregion

                    new_user.UpdatedDate = DateTime.Now;
                    new_user.UPDATEDUSERID = User.Identity.GetUserId();
                    var localEntity = db.Set<CIFInfoModel>().Local.FirstOrDefault(f => f.ID == new_user.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    db.Entry(new_user).State = EntityState.Modified;
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{cIFInfoModel.uservm.NAME} is updated.");
                    Session["CIFVM"] = string.Empty; //To genereate PDF
                    Session["CIFVM"] = JsonConvert.SerializeObject(cIFInfoModel.uservm);

                    Session["CIFEditVM"] = JsonConvert.SerializeObject(cIFInfoModel.uservm);
                    return RedirectToAction("CIF_ConfirmPage");
                }

                return RedirectToAction("CIF_ConfirmPage");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: CIFInfoModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }

            return View(cIFInfoModel);
        }

        // POST: CIFInfoModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
                string CIFID = cIFInfoModel.CIFID.ToLower();
                cIFInfoModel.DELFLAG = true;
                //db.CIFInfoModels.Remove(cIFInfoModel);

                db.Acct_InfoModels
                    .Where(a => a.CIFID.Contains(CIFID))
                    .ToList()
                    .ForEach(a => a.ACCT_CLOSE_FLAG = true);

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

        public ActionResult PDF(bool flag)
        {
            List<string> fileList = new List<string>();
            var cifInfo = JsonConvert.DeserializeObject<CIFInfoVMForReport>(Session["InfoForReport"].ToString());
            var callcenter = (from a in db.Credentials
                              where a.Status == "1" & a.Name == "Iconic Call Center" & a.Key == "Iconic"
                              select a).ToList();
            string iconicCallCenter = callcenter[0].Value;
            string filename = GeneratePdf(cifInfo.CIFID, cifInfo.NAME, cifInfo.USERTYPECODE, cifInfo.strEXPIREDATE.ToString(), cifInfo.RMName, iconicCallCenter);
            string filename1 = GeneratePdfForNewCardNReceiving(iconicCallCenter);
            if (flag)
            {
                string filename2 = GeneratePdfForTermAndConditon(iconicCallCenter);
                fileList.Add(filename2);

            }

            fileList.Add(filename);
            fileList.Add(filename1);

            if (fileList.Count > 0)
            {
                return Download(fileList);
            }

            return null;
        }

        public ActionResult Download(List<string> filename)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string CifPdf_Path = ConstantValues.CifPdf_Path;
                string fullpath = "";
                using (var memoryStream = new MemoryStream())
                {
                    if (filename.Count > 0)
                    {
                        using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var item in filename)
                            {
                                if (item.Contains("cifinfo.pdf") || item.Contains("cifinfoNewCardAndReceiving.pdf") || item.Contains("cifinfoTermsAndConditions.pdf"))
                                {
                                    fullpath = $"{CifPdf_Path}\\Cif Information\\{item}";
                                }
                                ziparchive.CreateEntryFromFile(fullpath, item);
                                string getuserid = User.Identity.GetUserId();
                                log.Log(getuserid, controllerName, actionName, $"Downlod successfully.",
                                    $"File name is {item}.");
                            }
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");

                }

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [Obsolete]
        private string GeneratePdf(string cifid, string Fullname, string iconicno, string expdate, string RmName,
            string iconicCallCenter)
        {
            try
            {
                //string AdminPdf_Path = System.Configuration.ConfigurationManager.AppSettings["AdminPdfSavePath"];
                string CifPdf_Path = ConstantValues.CifPdf_Path;
                string filename = "";
                CifPdf_Path = $"{CifPdf_Path}\\Cif Information";
                //create if folder doesn't exist
                if (!Directory.Exists(CifPdf_Path))
                {
                    Directory.CreateDirectory(CifPdf_Path);
                }
                //create a new pdf document
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                // Create an empty page
                PdfSharp.Pdf.PdfPage page = document.AddPage();
                //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                //draw background image
                //DrawImage(gfx);
                //gfx.DrawRectangle(XBrushes.LightGray, new XRect(0, 0, page.Width.Point, page.Height.Point));
                //PDF Size
                page.Size = PdfSharp.PageSize.A4;
                // Put a logo in the header
                DrawTitleImage(gfx);
                // Create a font
                XFont font = new XFont("Times New Roman", 20, XFontStyle.Bold, options);
                XFont font1 = new XFont("Times New Roman", 8, XFontStyle.Bold, options);

                //Draw page border
                //XRect A4Rect = new XRect(50, 0, 500, 750);
                //gfx.DrawRectangle(XPens.Black,A4Rect);

                font = new XFont("Times New Roman", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                // Draw the text
                //gfx.DrawString("eBanking Customer Information", font, XBrushes.Black,
                //  new XRect(0, 0, page.Width, 190),
                //  XStringFormats.Center);

                //Draw Dotted Line
                XPen pen = new XPen(XColors.Indigo, 1.5);
                XPen pens = new XPen(XColors.Black, 0.5);
                //pen.DashStyle = XDashStyle.Dot;
                //gfx.DrawRectangle(pens, 330, 0, 219, 70);

                //gfx.DrawLine();
                // Create a font
                font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);


                ////for Datetime name
                //gfx.DrawString($"  Created Date", font, XBrushes.Black, new XRect(335, 15, page.Width, 10),
                //  XStringFormats.CenterLeft);
                //for Datetime value
                gfx.DrawString($"{DateTime.Now.ToString("dd/MM/yyyy")}", font, XBrushes.Gray,
                    new XRect(482, 50, page.Width, 10),
                    XStringFormats.CenterLeft);

                // For Staff ID name
                //gfx.DrawString($"  Staff_ID", font, XBrushes.Black, new XRect(335, 45, page.Width, 10),
                //  XStringFormats.CenterLeft);
                //// For Staff ID value
                //gfx.DrawString($": {StaffID}", font, XBrushes.Black, new XRect(420, 45, page.Width, 10),
                //  XStringFormats.CenterLeft);

                // Create a font
                font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);

                // For Customer name
                gfx.DrawString($"FULL NAME", font, XBrushes.Black, new XRect(60, 183, page.Width, 10),
                    XStringFormats.CenterLeft);
                // For Customer name value
                gfx.DrawString($"{Fullname}", font, XBrushes.Black, new XRect(210, 183, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawLine(pens, 210, 193, 440, 193);
                // For Customer Id
                gfx.DrawString($"CUSTOMER ID", font, XBrushes.Black, new XRect(60, 203, page.Width, 10),
                    XStringFormats.CenterLeft);
                // For Customer Id value
                gfx.DrawString($"{cifid}", font, XBrushes.Black, new XRect(210, 203, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawLine(pens, 210, 213, 440, 213);
                // For iconno name
                gfx.DrawString($"THE ICONIC NUMBER ", font, XBrushes.Black, new XRect(60, 223, page.Width, 10),
                    XStringFormats.CenterLeft);
                // For iconno value
                gfx.DrawString($"{iconicno}", font, XBrushes.Black, new XRect(210, 223, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawLine(pens, 210, 233, 440, 233);
                // For Expire Date  name
                gfx.DrawString($"EXPIRED DATE", font, XBrushes.Black, new XRect(60, 243, page.Width, 10),
                    XStringFormats.CenterLeft);
                // For Expire Date  value
                gfx.DrawString($"{expdate}", font, XBrushes.Black, new XRect(210, 243, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawLine(pens, 210, 253, 440, 253);
                // For RM name
                gfx.DrawString($"ASSIGNED RM", font, XBrushes.Black, new XRect(60, 263, page.Width, 10),
                    XStringFormats.CenterLeft);
                // For RM value
                gfx.DrawString($"{RmName}", font, XBrushes.Black, new XRect(210, 263, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawLine(pens, 210, 273, 440, 273);
                string timestamp = General.GetTimestamp(DateTime.Now);
                //footer
                gfx.DrawString($"THE ICONIC Call Center (+95) {iconicCallCenter}", font1, XBrushes.Gray,
                    new XRect(20, 750, page.Width, 10),
                    XStringFormats.CenterLeft);
                gfx.DrawString($"Standing with you", font, XBrushes.Black, new XRect(450, 750, 100, 0),
                    XStringFormats.CenterLeft);
                gfx.DrawString($"www.abank.com.mm", font1, XBrushes.Gray, new XRect(20, 760, page.Width, 10),
                    XStringFormats.CenterLeft);
                // Save the document...
                filename = $"{timestamp}_{cifid}_cifinfo.pdf";
                document.Save($"{CifPdf_Path}\\{filename}");


                return filename;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        [Obsolete]
        private string GeneratePdfForNewCardNReceiving(string iconicCallCenter)
        {
            try
            {
                //string AdminPdf_Path = System.Configuration.ConfigurationManager.AppSettings["AdminPdfSavePath"];
                string CifPdf_Path = ConstantValues.CifPdf_Path;
                string filename = "";
                CifPdf_Path = $"{CifPdf_Path}\\Cif Information";
                //create if folder doesn't exist
                if (!Directory.Exists(CifPdf_Path))
                {
                    Directory.CreateDirectory(CifPdf_Path);
                }
                //create a new pdf document
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                // Create an empty page
                PdfSharp.Pdf.PdfPage page = document.AddPage();
                //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                //PDF Size
                page.Size = PdfSharp.PageSize.A4;
                // Put a logo in the header
                DrawTitleImage(gfx);
                // Create a font
                XFont font = new XFont("Times New Roman", 20, XFontStyle.Bold, options);
                XFont font1 = new XFont("Times New Roman", 8, XFontStyle.Bold, options);
                XFont font3 = new XFont("Times New Roman", 20, XFontStyle.Bold, options);
                XFont font4 = new XFont("Times New Roman", 12, XFontStyle.Regular, options);

                font = new XFont("Times New Roman", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                //Draw Dotted Line
                XPen pen = new XPen(XColors.Indigo, 1.5);
                XPen pens = new XPen(XColors.Black, 0.5);
                // Create a font
                font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);
                //for Datetime value
                gfx.DrawString($"{DateTime.Now.ToString("dd/MM/yyyy")}", font, XBrushes.Gray,
                    new XRect(482, 50, page.Width, 10),
                    XStringFormats.CenterLeft);
                string text =
                    "A bank is pleased and honored to congratulate you on becoming THE ICONIC privilege banking customer. " + "THE ICONIC privilege banking is here to amplify your wealth and accelerate your goals." +
                    "All-inclusive services are well-designed with our tailored privileges offering to suit your needs and enjoy a world of curated indulgences. We are here to craft your journey as your wealth growing.";
                XRect rect = new XRect(50, 180, page.Width - 100, 200);
                XTextFormatter tf = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Justify;
                tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);

                gfx.DrawString("Dear \"THE ICONIC\" Member,", font4, XBrushes.Black,
                    new XRect(50, 150, page.Width, 10),
                    XStringFormats.CenterLeft);
                string text1 =
                    "We are delighted to introduce a privilege banking service by A bank named THE \"ICONIC\", " +
                    "which provides successful individuals access to exclusive privileges beyond possibilities." +
                    " Therefore, it is our honor to welcome you, THE ICONIC member, into our exclusive membership " +
                    "program and our exclusive campaign which includes special offers including a customized gold plate to magnify your wealth. ";
                XRect rect1 = new XRect(50, 250, page.Width - 100, 200);
                XTextFormatter tf1 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect1);
                tf1.Alignment = XParagraphAlignment.Justify;
                tf1.DrawString(text1, font, XBrushes.Black, rect1, XStringFormats.TopLeft);

                //for iconic card checkbox
                //Rectangle r1 = new Rectangle(20, 350, 550, 60);
                //gfx.DrawRectangle(pen, XBrushes.LightBlue, r1);
                gfx.DrawRectangle(pens, XBrushes.White, 50, 350, 15, 10);
                gfx.DrawString("THE ICONIC Card", font4, XBrushes.Black, new XRect(100, 350, 18, 10),
                    XStringFormats.CenterLeft);


                //for welcome pack
                //Rectangle r2 = new Rectangle(20, 430, 550, 60);
                //gfx.DrawRectangle(pen, XBrushes.LightBlue, r2);
                gfx.DrawRectangle(pens, XBrushes.White, 50, 380, 15, 10);
                gfx.DrawString("Welcome Pack and Privilege Book", font4, XBrushes.Black, new XRect(100, 380, 18, 10),
                    XStringFormats.CenterLeft);
                //for customer signature
                //Rectangle r3 = new Rectangle(20, 510, 550, 60);
                //gfx.DrawRectangle(pen, XBrushes.LightBlue, r3);
                gfx.DrawString("Customer Signature", font4, XBrushes.Black, new XRect(100, 410, page.Width, 50),
                    XStringFormats.CenterLeft);
                string timestamp = General.GetTimestamp(DateTime.Now);
                //footer
                gfx.DrawString($"THE ICONIC Call Center (+95) {iconicCallCenter}", font1, XBrushes.Gray,
                    new XRect(20, 750, page.Width, 50),
                    XStringFormats.CenterLeft);
                gfx.DrawString($"Standing with you", font, XBrushes.Black, new XRect(450, 750, 100, 0),
                    XStringFormats.CenterLeft);
                gfx.DrawString($"www.abank.com.mm", font1, XBrushes.Gray, new XRect(20, 760, page.Width, 10),
                    XStringFormats.CenterLeft);
                // Save the document...
                filename = $"{timestamp}_cifinfoNewCardAndReceiving.pdf";
                document.Save($"{CifPdf_Path}\\{filename}");


                return filename;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        [Obsolete]
        private string GeneratePdfForTermAndConditon(string iconicCallCenter)
        {
            try
            {
                //string AdminPdf_Path = System.Configuration.ConfigurationManager.AppSettings["AdminPdfSavePath"];
                string CifPdf_Path = ConstantValues.CifPdf_Path;
                string filename = "";
                CifPdf_Path = $"{CifPdf_Path}\\Cif Information";
                //create if folder doesn't exist
                if (!Directory.Exists(CifPdf_Path))
                {
                    Directory.CreateDirectory(CifPdf_Path);
                }
                //create a new pdf document
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                // Create an empty page
                PdfSharp.Pdf.PdfPage page = document.AddPage();
                //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Put a logo in the header
                DrawTitleImage(gfx);
                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                // Create a font
                XFont font = new XFont("Times New Roman", 20, XFontStyle.Bold, options);
                XFont font1 = new XFont("Times New Roman", 8, XFontStyle.Bold, options);
                XFont font4 = new XFont("Times New Roman", 14, XFontStyle.Regular, options);
                XFont fontforMM = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);


                // Create a font
                font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);
                XPen pens = new XPen(XColors.Black, 0.5);
                #region test For next page PDF

                PageSize[] pageSizes = (PageSize[])Enum.GetValues(typeof(PageSize));
                foreach (PageSize pageSize in pageSizes)
                {
                    if (pageSize == PageSize.Undefined)
                        continue;

                    if (pageSize == PageSize.A4)
                    {
                        // One page in Portrait...
                        PdfPage page2 = document.AddPage();
                        page2.Size = pageSize;
                        XGraphics gfx2 = XGraphics.FromPdfPage(page2);
                        XRect rect16 = new XRect(30, 60, page.Width - 70, 200);
                        XTextFormatter tf16 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect16);
                        tf16.Alignment = XParagraphAlignment.Justify;
                        tf16.DrawString(
                            "4.2 ထပ္မံ၍အပိုေခၚေဆာင္ေသာ ဧည့္သည္မ်ားအတြက္ တစ္ေယာက္လွ်င္ ဝန္ေဆာင္ခက်သင့္ေငြ ၃၂ေဒၚလာကို စာရင္းေသအပ္ေငြကာလၿပီးဆုံးခ်ိန္တြင္ ယခုဖြင့္လွစ္ထားေသာအပ္ေငြအေကာင့္မွ ႏႈတ္ယူသြားမည္ကို သိရွိၿပီး ထိုသို႔ျပဳလုပ္မည္ကို ခြင့္ျပဳပါသည္။  ",
                            fontforMM, XBrushes.Black, rect16, XStringFormats.TopLeft);
                        XRect rect17 = new XRect(30, 120, page.Width - 70, 200);
                        XTextFormatter tf17 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect17);
                        tf17.Alignment = XParagraphAlignment.Justify;
                        tf17.DrawString(
                            "Additional accompanying guests are subject to lounge service fees which will be charged USD 32 at the end of the Fixed Deposit term. I consent that A bank can deduct additional lounge service charge from my deposit account. ",
                            fontforMM, XBrushes.Black, rect17, XStringFormats.TopLeft);
                        XRect rect18 = new XRect(30, 160, page.Width - 70, 200);
                        XTextFormatter tf18 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect18);
                        tf18.Alignment = XParagraphAlignment.Justify;
                        tf18.DrawString(
                            "5. Fast Track Immigration at Suvarnabhumi Airport ဝန္ေဆာင္မႈအား memberသည္သာလွ်င္အသုံးျပဳခြင့္ရွိေၾကာင္းႏွင့္ အသုံးျပဳရာတြင္ လိုက္နာရမည့္ ေအာက္ပါ အခ်က္မ်ားကို သိရွိၿပီးျဖစ္ပါသည္။",
                            fontforMM, XBrushes.Black, rect18, XStringFormats.TopLeft);
                        XRect rect19 = new XRect(30, 200, page.Width - 70, 200);
                        XTextFormatter tf19 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect19);
                        tf19.Alignment = XParagraphAlignment.Justify;
                        tf19.DrawString(
                            "I acknowledge that Fast Track Immigration at Suvarnabhumi Airport service can be used by THE ICONIC member only.",
                            fontforMM, XBrushes.Black, rect19, XStringFormats.TopLeft);
                        XRect rect20 = new XRect(30, 240, page.Width - 70, 200);
                        XTextFormatter tf20 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect20);
                        tf20.Alignment = XParagraphAlignment.Justify;
                        tf20.DrawString(
                            "5.1 လူဝင္မႈႀကီးၾကပ္ေရးဌာန၏ ျပည္ဝင္ခြင့္စစ္ေဆးခ်က္မ်ားအား ေရွာင္ရန္ ဤဝန္ေဆာင္မႈကို ေပးျခင္းမဟုတ္ပါ။",
                            fontforMM, XBrushes.Black, rect20, XStringFormats.TopLeft);
                        XRect rect21 = new XRect(30, 280, page.Width - 70, 200);
                        XTextFormatter tf21 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect21);
                        tf21.Alignment = XParagraphAlignment.Justify;
                        tf21.DrawString("This service cannot use to avoid the immigration.", fontforMM, XBrushes.Black,
                            rect21, XStringFormats.TopLeft);
                        XRect rect22 = new XRect(30, 320, page.Width - 70, 200);
                        XTextFormatter tf22 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect22);
                        tf22.Alignment = XParagraphAlignment.Justify;
                        tf22.DrawString(
                            "5.2 လူဝင္မႈႀကီးၾကပ္ေရးဌာနမွ ျပည္ဝင္ခြင့္စစ္ေဆးရာတြင္ ျဖစ္ေပၚလာေသာ အခက္အခဲမ်ား ျပႆနာတစ္စုံတစ္ရာအတြက္ ဘဏ္၌တာဝန္မရွိပါ။",
                            fontforMM, XBrushes.Black, rect22, XStringFormats.TopLeft);
                        XRect rect23 = new XRect(30, 380, page.Width - 70, 200);
                        XTextFormatter tf23 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect23);
                        tf23.Alignment = XParagraphAlignment.Justify;
                        tf23.DrawString(
                            "A bank will not responsible for any incident occurs during the immigration checking.",
                            fontforMM, XBrushes.Black, rect23, XStringFormats.TopLeft);
                        XRect rect24 = new XRect(30, 420, page.Width - 70, 200);
                        XTextFormatter tf24 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect24);
                        tf24.Alignment = XParagraphAlignment.Justify;
                        tf24.DrawString(
                            "5.3 ယခုဝန္ေဆာင္မႈသည္ Bangladesh (BD), Cameroon (CM), Guinea (GN), India (IN), Iran (Islamic Republic Of) (IR), Kenya (KE), Nigeria (NG), Pakistan (PK), Sri Lanka (LK), Syrian Arab Republic (SY) ႏိုင္ငံကူးလက္မွတ္ကိုင္ေဆာင္ထားသူမ်ား အသုံးျပဴ၍မရပါ။",
                            fontforMM, XBrushes.Black, rect24, XStringFormats.TopLeft);
                        XRect rect25 = new XRect(30, 480, page.Width - 70, 200);
                        XTextFormatter tf25 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect25);
                        tf25.Alignment = XParagraphAlignment.Justify;
                        tf25.DrawString(
                            "This service is not available to passport holders of the following countries: Bangladesh (BD), Cameroon (CM), Guinea (GN), India (IN), Iran (Islamic Republic Of) (IR), Kenya (KE), Nigeria (NG), Pakistan (PK), Sri Lanka (LK), Syrian Arab Republic (SY) ",
                            fontforMM, XBrushes.Black, rect25, XStringFormats.TopLeft);
                        XRect rect26 = new XRect(30, 540, page.Width - 70, 200);
                        XTextFormatter tf26 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect26);
                        tf26.Alignment = XParagraphAlignment.Justify;
                        tf26.DrawString("5.4 ယခုဝန္ေဆာင္မႈအား လိုအပ္ပါက ဝန္ေဆာင္မႈေပးျခင္းကို ဘဏ္မွရပ္ဆိုင္းႏိုင္သည္။",
                            fontforMM, XBrushes.Black, rect26, XStringFormats.TopLeft);
                        XRect rect27 = new XRect(30, 580, page.Width - 70, 200);
                        XTextFormatter tf27 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect27);
                        tf27.Alignment = XParagraphAlignment.Justify;
                        tf27.DrawString("A bank reserves the right to cancel the booking if necessary.", fontforMM,
                            XBrushes.Black, rect27, XStringFormats.TopLeft);
                        XRect rect28 = new XRect(30, 620, page.Width - 70, 200);
                        XTextFormatter tf28 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect28);
                        tf28.Alignment = XParagraphAlignment.Justify;
                        tf28.DrawString(
                            "6. Cheque-no-bounce အား အသုံးျပဳမည္ဆိုပါက အျခားလိုအပ္သည့္ အခ်က္အလက္မ်ား ေလွ်ာက္လႊာမ်ားအား ျဖည့္ရမည္ကို သိရွိၿပီးျဖစ္ပါသည္။ ",
                            fontforMM, XBrushes.Black, rect28, XStringFormats.TopLeft);
                        XRect rect29 = new XRect(30, 660, page.Width - 70, 200);
                        XTextFormatter tf29 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect29);
                        tf29.Alignment = XParagraphAlignment.Justify;
                        tf29.DrawString(
                            "I acknowledge proceeding with additional application forms and providing more information to use Cheque-No Bounce service.",
                            fontforMM, XBrushes.Black, rect29, XStringFormats.TopLeft);
                        XRect rect30 = new XRect(30, 700, page.Width - 70, 200);
                        XTextFormatter tf30 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect30);
                        tf30.Alignment = XParagraphAlignment.Justify;
                        tf30.DrawString(
                            "6.1 Cheque-no-bounce အား အသုံးျပဳရာတြင္ ေပးေဆာင္ရမည့္ အတိုးႏွန္း ႏွင့္ အသုံးျပဳလိုက္ေသာ ပမာဏကို စာရင္းေသအပ္ေငြကာလၿပီးဆုံးခ်ိန္တြင္ ယခုဖြင့္လွစ္ထားေသာအပ္ေငြအေကာင့္မွ ႏႈတ္ယူသြားမည္ကို သိရွိၿပီး ထိုသို႔ျပဳလုပ္မည္ကို ခြင့္ျပဳပါသည္။  ",
                            fontforMM, XBrushes.Black, rect30, XStringFormats.TopLeft);
                        XRect rect31 = new XRect(30, 750, page.Width - 70, 200);
                        XTextFormatter tf31 = new XTextFormatter(gfx2);
                        gfx2.DrawRectangle(XBrushes.White, rect31);
                        tf31.Alignment = XParagraphAlignment.Justify;
                        tf31.DrawString(
                            "In case applying for cheque-no-bounce service, I consent to A bank to deduct the principle and interest rate according to the agreed rate at the matured date of the fixed deposit term. ",
                            fontforMM, XBrushes.Black, rect31, XStringFormats.TopLeft);
                        //Add third page
                        PdfPage page3 = document.AddPage();
                        page3.Size = pageSize;
                        XGraphics gfx3 = XGraphics.FromPdfPage(page3);
                        XRect rect32 = new XRect(30, 60, page.Width - 70, 200);
                        XTextFormatter tf32 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect32);
                        tf32.Alignment = XParagraphAlignment.Justify;
                        tf32.DrawString(
                            "7. ဘဏ္မွ သတ္မွတ္ထားေသာ အနည္းဆုံးအပ္ေငြပမာဏလိုအပ္ခ်က္အား မထားရွိပါက ဘဏ္မွ အသင္းဝင္အျဖစ္မွ ခ်က္ခ်င္းရပ္ဆိုင္းႏိုင္သည္။",
                            fontforMM, XBrushes.Black, rect32, XStringFormats.TopLeft);
                        XRect rect33 = new XRect(30, 100, page.Width - 70, 200);
                        XTextFormatter tf33 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect33);
                        tf33.Alignment = XParagraphAlignment.Justify;
                        tf33.DrawString(
                            "In the case where the customer fails to maintain the deposit to the conditions stipulated by the Bank, the Bank reserves the right to cancel THE ICONIC status of such customer and any privileges thereof, with effective immediately without any prior notice.",
                            fontforMM, XBrushes.Black, rect33, XStringFormats.TopLeft);
                        XRect rect34 = new XRect(30, 150, page.Width - 70, 200);
                        XTextFormatter tf34 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect34);
                        tf34.Alignment = XParagraphAlignment.Justify;
                        tf34.DrawString(
                            "8. ဘဏ္မွ သတ္မွတ္ထားေသာ စည္းကမ္းခ်က္မ်ားႏွင့္ ေပးအပ္ေသာ ဝန္ေဆာင္မႈမ်ားအား ႀကိဳတင္အၾကာင္းၾကားျခင္းမရွိပဲ ပယ္ဖ်က္ျခင္း (သို႔) ရပ္ဆိုင္းျခင္းမ်ား ျပဳလုပ္ႏိုင္သည္။ ",
                            fontforMM, XBrushes.Black, rect34, XStringFormats.TopLeft);
                        XRect rect35 = new XRect(30, 190, page.Width - 70, 200);
                        XTextFormatter tf35 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect35);
                        tf35.Alignment = XParagraphAlignment.Justify;
                        tf35.DrawString(
                            "The Bank reserves the right to modify terms and conditions of THE ICONIC privileges without prior notice.",
                            fontforMM, XBrushes.Black, rect35, XStringFormats.TopLeft);
                        XRect rect36 = new XRect(30, 230, page.Width - 70, 200);
                        XTextFormatter tf36 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect36);
                        tf36.Alignment = XParagraphAlignment.Justify;
                        tf36.DrawString(
                            "9. Privilege Bookတြင္ ေဖာ္ျပထားေသာ ဝန္ေဆာင္မႈမ်ား အသုံးျပဳရာတြင္ အဆင္မေျပမႈမ်ားေပၚေပါက္လာပါက A bankႏွင့္ သက္ဆိုင္မႈမရွိပဲ merchantနဲ႔သာ သက္ဆိုင္ပါသည္။",
                            fontforMM, XBrushes.Black, rect36, XStringFormats.TopLeft);
                        XRect rect37 = new XRect(30, 270, page.Width - 70, 200);
                        XTextFormatter tf37 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect37);
                        tf37.Alignment = XParagraphAlignment.Justify;
                        tf37.DrawString(
                            "A bank shall not be responsible and liable for the acts or defaults of the merchant or defects in the goods and service offered in the Privilege Book.",
                            fontforMM, XBrushes.Black, rect37, XStringFormats.TopLeft);
                        XRect rect38 = new XRect(30, 310, page.Width - 70, 200);
                        XTextFormatter tf38 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect38);
                        tf38.Alignment = XParagraphAlignment.Justify;
                        tf38.DrawString(
                            "10. A bankသည္ merchant၏ ကိုယ္စားလွယ္မဟုတ္ပါ။ ကုန္ပစၥည္း (သို႔) ဝန္ေဆာင္မႈ အရည္အေသြးမ်ား ခ်ိဳ႕ယြင္းခ်က္ရွိေနျခင္း မ်ားျဖစ္ေပၚလာပါက merchantနဲ႔သာ တိုက္႐ိုက္ေျဖရွင္းရပါမည္။",
                            fontforMM, XBrushes.Black, rect38, XStringFormats.TopLeft);
                        XRect rect39 = new XRect(30, 350, page.Width - 70, 200);
                        XTextFormatter tf39 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect39);
                        tf39.Alignment = XParagraphAlignment.Justify;
                        tf39.DrawString(
                            "A bank is not an agent of the merchant. Any dispute or defects in the goods and services standard must be resolved directly with the merchant.",
                            fontforMM, XBrushes.Black, rect39, XStringFormats.TopLeft);
                        XRect rect40 = new XRect(30, 390, page.Width - 70, 200);
                        XTextFormatter tf40 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect40);
                        tf40.Alignment = XParagraphAlignment.Justify;
                        tf40.DrawString(
                            "11.သတ္မွတ္ထားေသာ အထက္ပါအခ်က္အလက္မ်ားအား ဖတ္ၿပီးေၾကာင္းႏွင့္ အခ်က္အလက္မ်ားအား သေဘာတူပါသည္။",
                            fontforMM, XBrushes.Black, rect40, XStringFormats.TopLeft);
                        XRect rect41 = new XRect(30, 430, page.Width - 70, 200);
                        XTextFormatter tf41 = new XTextFormatter(gfx3);
                        gfx3.DrawRectangle(XBrushes.White, rect41);
                        tf41.Alignment = XParagraphAlignment.Justify;
                        tf41.DrawString("I have read and agreed all terms and conditions mentioned above. ", fontforMM,
                            XBrushes.Black, rect41, XStringFormats.TopLeft);
                        gfx3.DrawString($"Authorized Person's Signature", font, XBrushes.Black, new XRect(30, 480, page.Width, 10),
                   XStringFormats.CenterLeft);
                        gfx3.DrawString($"Applicant's Signature", font, XBrushes.Black, new XRect(350, 480, page.Width, 10),
                  XStringFormats.CenterLeft);
                        gfx3.DrawString($"Name:", font, XBrushes.Black, new XRect(30, 520, page.Width, 10),
                  XStringFormats.CenterLeft);
                        gfx3.DrawLine(pens, 80, 530, 230, 530);
                        gfx3.DrawString($"Position:", font, XBrushes.Black, new XRect(30, 560, page.Width, 10),
                  XStringFormats.CenterLeft);
                        gfx3.DrawLine(pens, 80, 570, 230, 570);
                        gfx3.DrawString($"Name:", font, XBrushes.Black, new XRect(350, 560, page.Width, 10),
                XStringFormats.CenterLeft);
                        gfx3.DrawLine(pens, 400, 570, 550, 570);
                        gfx3.DrawString($"Date:", font, XBrushes.Black, new XRect(30, 600, page.Width, 10),
              XStringFormats.CenterLeft);
                        gfx3.DrawLine(pens, 80, 610, 230, 610);
                        gfx3.DrawString($"Date:", font, XBrushes.Black, new XRect(350, 600, page.Width, 10),
              XStringFormats.CenterLeft);
                        gfx3.DrawLine(pens, 400, 610, 550, 610);
                        gfx3.DrawString($"THE ICONIC Call Center (+95) {iconicCallCenter}", font1, XBrushes.Gray,
                            new XRect(20, 750, page.Width, 50),
                            XStringFormats.CenterLeft);
                        gfx3.DrawString($"Standing with you", font, XBrushes.Black, new XRect(450, 750, 100, 0),
                            XStringFormats.CenterLeft);
                        gfx3.DrawString($"www.abank.com.mm", font1, XBrushes.Gray, new XRect(20, 760, page.Width, 10),
                            XStringFormats.CenterLeft);


                    }
                }
                #endregion test For next page PDF


                //for Datetime value
                gfx.DrawString($"{DateTime.Now.ToString("dd/MM/yyyy")}", font, XBrushes.Gray,
                    new XRect(482, 50, page.Width, 10),
                    XStringFormats.CenterLeft);
                // Create the starting point.

                //for textarea of letter

                gfx.DrawString("Terms and Conditions of THE ICONIC", font4, XBrushes.Black,
                    new XRect(20, 100, page.Width, 10),
                    XStringFormats.Center);
                XRect rect = new XRect(30, 120, page.Width - 70, 200);
                XTextFormatter tf = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Justify;
                tf.DrawString(
                    "1. သက္ဆိုင္ရာဥပေဒမ်ား၊ ၫႊန္ၾကားခ်က္မ်ား၊ အသစ္ (သို႔) ေျပာင္းလဲမႈေၾကာင့္ ထုတ္ျပန္သည့္ ဘဏ္၏ လုပ္ထုံးလုပ္နည္းမ်ားကို လိုက္နာေဆာင္႐ြက္သြားပါမည္။",
                    fontforMM, XBrushes.Black, rect, XStringFormats.TopLeft);
                XRect rect1 = new XRect(30, 160, page.Width - 70, 200);
                XTextFormatter tf1 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect1);
                tf1.Alignment = XParagraphAlignment.Justify;
                tf1.DrawString(
                    "I comply with updated bank’s procedures according to existing/ amended/ updated Laws and Directives.",
                    fontforMM, XBrushes.Black, rect1, XStringFormats.TopLeft);
                XRect rect2 = new XRect(30, 200, page.Width - 70, 200);
                XTextFormatter tf2 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect2);
                tf2.Alignment = XParagraphAlignment.Justify;
                tf2.DrawString(
                    "2. A bank၏ ဝန္ေဆာင္မႈမ်ားကို မည္သည့္ ဥပေဒျပစ္မႈက်ဳးလြန္ရန္ အလို႔ငွာ သုံးစြဲသြားမည္ မဟုတ္ပါေၾကာင္းကတိျပဳပါသည္။",
                    fontforMM, XBrushes.Black, rect2, XStringFormats.TopLeft);
                XRect rect3 = new XRect(30, 240, page.Width - 70, 200);
                XTextFormatter tf3 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect3);
                tf3.Alignment = XParagraphAlignment.Justify;
                tf3.DrawString(
                    "I declare that A bank’s services shall not be used for any clause of any criminal cases.",
                    fontforMM, XBrushes.Black, rect3, XStringFormats.TopLeft);
                XRect rect4 = new XRect(30, 280, page.Width - 70, 200);
                XTextFormatter tf4 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect4);
                tf4.Alignment = XParagraphAlignment.Justify;
                tf4.DrawString(
                    "3. THE ICONIC အသင္းဝင္ေလွ်ာက္ထားျခင္းဆိုင္ရာ ေအာက္ပါစည္းမ်ဥ္းစည္းကမ္းမ်ားကို သိရွိၿပီးျဖစ္ပါသည္။",
                    fontforMM, XBrushes.Black, rect4, XStringFormats.TopLeft);
                XRect rect5 = new XRect(30, 320, page.Width - 70, 200);
                XTextFormatter tf5 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect5);
                tf5.Alignment = XParagraphAlignment.Justify;
                tf5.DrawString("I acknowledge the following terms and conditions of being THE ICONIC member.",
                    fontforMM, XBrushes.Black, rect5, XStringFormats.TopLeft);
                XRect rect6 = new XRect(30, 360, page.Width - 70, 200);
                XTextFormatter tf6 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect6);
                tf6.Alignment = XParagraphAlignment.Justify;
                tf6.DrawString(
                    "3.1 THE ICONIC စာရင္းေသအပ္ေငြစာရင္းသည္ တစ္ဦးခ်င္းစာရင္းပုံစံျဖင့္သာ ဖြင့္ရမည္ျဖစ္သည္။ အျခားတစ္ဦးတစ္ေယာက္ႏွင့္ တြဲဖက္စာရင္းဖြင့္လွစ္၍မရပါ။",
                    fontforMM, XBrushes.Black, rect6, XStringFormats.TopLeft);
                XRect rect7 = new XRect(30, 400, page.Width - 70, 200);
                XTextFormatter tf7 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect7);
                tf7.Alignment = XParagraphAlignment.Justify;
                tf7.DrawString("Only an individual account type can open THE ICONIC Fixed Deposit account.",
                    fontforMM, XBrushes.Black, rect7, XStringFormats.TopLeft);
                XRect rect8 = new XRect(30, 440, page.Width - 70, 200);
                XTextFormatter tf8 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect8);
                tf8.Alignment = XParagraphAlignment.Justify;
                tf8.DrawString(
                    "3.2 စာရင္းပိုင္ရွင္တစ္ဦးတည္းမွ ဖြင့္လွစ္ထားေသာ စာရင္းေသအပ္ေငြအေကာင့္မ်ား၏ အပ္ေငြပမာဏသည္ အနည္းဆုံးအပ္ေငြလိုအပ္ခ်က္ (သိန္း ၁၄,၀၀၀ က်ပ္) ႏွင့္ ညီမွ်ပါက အသင္းဝင္အျဖစ္သတ္မွတ္ပါမည္။",
                    fontforMM, XBrushes.Black, rect8, XStringFormats.TopLeft);
                XRect rect9 = new XRect(30, 480, page.Width - 70, 200);
                XTextFormatter tf9 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect9);
                tf9.Alignment = XParagraphAlignment.Justify;
                tf9.DrawString(
                    "If the customer has several fixed deposit accounts and total amount is equivalent to minimum required amount of 1,400 Million MMK, the customer is considered as THE ICONIC member of A bank. ",
                    fontforMM, XBrushes.Black, rect9, XStringFormats.TopLeft);
                XRect rect10 = new XRect(30, 520, page.Width - 70, 200);
                XTextFormatter tf10 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect10);
                tf10.Alignment = XParagraphAlignment.Justify;
                tf10.DrawString(
                    "3.3 အသင္းဝင္ျဖစ္ရန္ စာရင္းေသအပ္ေငြပမာဏ သိန္း ၁၄,၀၀၀ က်ပ္ႏွင့္ အထက္ကို အနည္းဆုံး ၆လၾကာ ၌အပ္ႏွံထားရွိရမည္။",
                    fontforMM, XBrushes.Black, rect10, XStringFormats.TopLeft);
                XRect rect11 = new XRect(30, 560, page.Width - 70, 200);
                XTextFormatter tf11 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect11);
                tf11.Alignment = XParagraphAlignment.Justify;
                tf11.DrawString(
                    "In order to maintain membership status, the customer has to deposit minimum required amount of 1,400 Million MMK (or its equivalent) for duration of 6 months period.",
                    fontforMM, XBrushes.Black, rect11, XStringFormats.TopLeft);
                XRect rect12 = new XRect(30, 600, page.Width - 70, 200);
                XTextFormatter tf12 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect12);
                tf12.Alignment = XParagraphAlignment.Justify;
                tf12.DrawString(
                    "4. Priority Pass membership card အား member သည္သာလွ်င္အသုံးျပဳခြင့္ရွိေၾကာင္းႏွင့္ အသုံးျပဳရာတြင္ လိုက္နာရမည့္ ေအာက္ပါ အခ်က္မ်ားကို သိရွိၿပီးျဖစ္ပါသည္။ ",
                    fontforMM, XBrushes.Black, rect12, XStringFormats.TopLeft);
                XRect rect13 = new XRect(30, 640, page.Width - 70, 200);
                XTextFormatter tf13 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect13);
                tf13.Alignment = XParagraphAlignment.Justify;
                tf13.DrawString(
                    "I acknowledge that Priority Pass membership is for primary THE ICONIC member only.", fontforMM,
                    XBrushes.Black, rect13, XStringFormats.TopLeft);
                XRect rect14 = new XRect(30, 680, page.Width - 70, 200);
                XTextFormatter tf14 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect14);
                tf14.Alignment = XParagraphAlignment.Justify;
                tf14.DrawString(
                    "4.1	Priority Pass membership card တစ္ခုလွ်င္ Memberတစ္ေယာက္ႏွင့္ ဧည့္သည္အပိုႏွစ္ေယာက္အား ဝန္ေဆာင္ခ အခမဲ့အျဖစ္ ေခၚေဆာင္ခြင့္ရွိပါသည္။",
                    fontforMM, XBrushes.Black, rect14, XStringFormats.TopLeft);
                XRect rect15 = new XRect(30, 680, page.Width - 70, 200);
                XTextFormatter tf15 = new XTextFormatter(gfx);
                gfx.DrawRectangle(XBrushes.White, rect15);
                tf15.Alignment = XParagraphAlignment.Justify;
                tf15.DrawString(
                    "Priority Pass membership card holder is entitled to unlimited complimentary lounge visits with first two (2) accompanying guests free per visit.",
                    fontforMM, XBrushes.Black, rect15, XStringFormats.TopLeft);




                string timestamp = General.GetTimestamp(DateTime.Now);
                ////footer

                // Save the document...
                filename = $"{timestamp}_cifinfoTermsAndConditions.pdf";
                document.Save($"{CifPdf_Path}\\{filename}");


                return filename;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }
        private void DrawImage(XGraphics gfx)
        {

            //XImage image = XImage.FromFile(Server.MapPath("~/Images/new_pdf_bg.jpg"));

            /*  gfx.DrawImage(image, 50, 40, 500, 650);*/ //fixed image size => width 500, height 650
        }

        void DrawTitleImage(XGraphics gfxTitle)
        {
            XPen pen = new XPen(XColors.Gray, 0.5);
            gfxTitle.DrawLine(pen, 20, 25, 60, 25);
            gfxTitle.DrawLine(pen, 20, 27, 60, 27);
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            // Create a font
            XFont font = new XFont("Times New Roman", 5, XFontStyle.Bold, options);
            XFont font1 = new XFont("Times New Roman", 18, XFontStyle.Bold, options);
            gfxTitle.DrawString($"T H E", font, XBrushes.Gray, new XRect(63, 26, 60, 0),
                XStringFormats.CenterLeft);
            gfxTitle.DrawLine(pen, 78, 25, 117, 25);
            gfxTitle.DrawLine(pen, 78, 27, 117, 27);
            gfxTitle.DrawString($"I C O N I C", font1, XBrushes.Gray, new XRect(22, 38, 100, 0),
                XStringFormats.CenterLeft);
            gfxTitle.DrawLine(pen, 20, 48, 117, 48);
            gfxTitle.DrawLine(pen, 20, 50, 117, 50);
            //gfxTitle.DrawLine(pen, 100, 100, 100, 100);
            XImage image = XImage.FromFile(Server.MapPath("~/Images/pdftitle.jpg"));
            gfxTitle.DrawImage(image, 480, 20, 100, 30);
        }

        #region ReportCron

        public static void RunCronService()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";
            try
            {
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicCronlog && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var cronScheduleModel = new CRONScheudleModel();
                    cronScheduleModel.Schedule = cronServiceDt.SingleOrDefault().ToString();
                    var cronLog = new CronLog_Iconic();
                    cronLog.LOGMESSAGE = cronScheduleModel.Schedule;
                    cronLog.STARTDATE = DateTime.Now;
                    cronLog.RESPCODE = "001";
                    cronLog.RESPDESC = "SET TIMER";
                    cronLog.TASKNAME = ConstantValues.IconicCronlog;
                    cron_daemon.AddJob(cronScheduleModel.Schedule, CronTask);
                    cron_daemon.Start();
                    cronLog.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(cronLog);
                    db.SaveChanges();


                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                    Log2.RESPCODE = "999";
                    Log2.RESPDESC = "There is no row for Service name CRONLOG in TBL_SERVICEMANAGEMENT table.";
                    Log2.STARTDATE = DateTime.Now;
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                log.Error("0", controllerName, "RunCronService", ex);

            }

        }

        public static void CronTask()
        {
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";

            try
            {
                var datetime = DateTime.Now;
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicReportCron && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var timeFormat = cronServiceDt.SingleOrDefault();//every month every monday
                    if (datetime.Hour.ToString() == timeFormat) //+ ":" + datetime.Minute.ToString()

                    {
                        var dt = (db.CronLogs_Iconic.Where(a => a.STARTDATE.Value.Year == datetime.Year && a.STARTDATE.Value.Month == datetime.Month && a.STARTDATE.Value.Day == datetime.Day &&
                                                        a.RESPCODE == "002")).OrderByDescending(a => a.ID).ToList();

                        int logcount = dt.Count();
                        if (logcount == 0)
                        {
                            var Log2 = new CronLog_Iconic();
                            Log2.TASKNAME = ConstantValues.IconicReportCron;
                            Log2.RESPCODE = "002";
                            Log2.RESPDESC = "Starting";
                            Log2.STARTDATE = DateTime.Now;
                            Log2.ENDDATE = DateTime.Now;
                            db.CronLogs_Iconic.Add(Log2);
                            db.SaveChanges();
                            try
                            {
                                IconicReportTask();
                            }
                            catch (Exception ex)
                            {
                                var errLog = new CronLog_Iconic();
                                errLog.TASKNAME = ConstantValues.IconicReportCron;
                                errLog.RESPCODE = "999";
                                errLog.RESPDESC = ex.Message;
                                db.CronLogs_Iconic.Add(errLog);
                                db.SaveChanges();
                            }


                        }
                    }


                }

            }
            catch (Exception ex)
            {
                var errLog = new CronLog_Iconic();
                errLog.TASKNAME = ConstantValues.IconicReportCron;
                errLog.RESPCODE = "999";
                errLog.RESPDESC = ex.Message;
                db.CronLogs_Iconic.Add(errLog);
                db.SaveChanges();

            }

        }

        public static void IconicReportTask()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            var TotalCount = 0;
            var SuccessCount = 0;
            var FailCount = 0;
            try
            {
                DateTime oldestDate = DateTime.Now.Subtract(new TimeSpan(10, 0, 0, 0, 0));
                string filename = ExcelExportReport(oldestDate);
                List<string> mailList = new List<string>();
                var emaillistInfo = db.CIFInfoModels.Where(x => x.Status == 1 && (x.CREATEDDATE >= oldestDate && x.CREATEDDATE <= DateTime.Now)).Select(x => new { x.UserEmail, x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfo.Count > 0)
                {
                    foreach (var item in emaillistInfo)
                    {
                        TotalCount++;
                        mailList.Add(item.UserEmail);
                        mailList.Add(item.ApproverEmail);

                    }
                }

                #region sendEmail
                bool sendEmail = SendEmailForIconicReport(mailList, "", "CIFInfoModels", filename, "IconicReportTask");
                if (emaillistInfo.Count > 0)
                {
                    if (sendEmail)
                    {
                        var logMsg =
                            $"Total Count : {TotalCount}";
                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCron;
                        Log2.RESPCODE = "000";
                        Log2.RESPDESC = "Success";
                        Log2.LOGMESSAGE = logMsg;
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();

                    }
                    else
                    {
                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCron;
                        Log2.RESPCODE = "014";
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCron;
                    Log2.RESPCODE = "013";
                    Log2.RESPDESC = "No Data";
                    Log2.STARTDATE = DateTime.Now;
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();
                }
                #endregion


            }
            catch (Exception ex)
            {
                var Log2 = new CronLog_Iconic();
                Log2.TASKNAME = ConstantValues.IconicReportCron;
                Log2.RESPCODE = "014";
                Log2.RESPDESC = ex.Message;
                Log2.STARTDATE = DateTime.Now;
                Log2.ENDDATE = DateTime.Now;
                db.CronLogs_Iconic.Add(Log2);
                db.SaveChanges();
                log.Error("0", "CIFInfoModels", "IconicReportTask", ex);

            }
        }

        public static string ExcelExportReport(DateTime oldestDate)
        {
            string filename = string.Empty;
            string datestamp = General.GetDatestamp(DateTime.Now);
            string CSVFile_Path = ConstantValues.CSVIconicFile_Path;
            //create if folder doesn't exist
            if (!Directory.Exists(CSVFile_Path))
            {
                Directory.CreateDirectory(CSVFile_Path);
            }
            filename = $"{datestamp}_THEICONICNewRegistration" + ".xlsx";

            string path = $"{CSVFile_Path}";
            string filepath = $"{path}\\{filename}";

            AdminDBContext db = new AdminDBContext();
            var dbInfo = db.CIFInfoModels.Where(x => x.Status == 1 && (x.CREATEDDATE >= oldestDate && x.CREATEDDATE <= DateTime.Now)).ToList();
            List<CIFInfoModel> FileData = dbInfo.ToList();
            try
            {

                DataTable Dt = new DataTable();
                Dt.Columns.Add("CIFID", typeof(string));
                Dt.Columns.Add("THE ICONIC Tier", typeof(string));
                Dt.Columns.Add("THE ICONIC Number", typeof(string));
                Dt.Columns.Add("THE ICONIC Effective Date", typeof(string));
                Dt.Columns.Add("THE ICONIC Expire Date", typeof(string));
                Dt.Columns.Add("RM ID", typeof(string));
                Dt.Columns.Add("RM Name", typeof(string));
                Dt.Columns.Add("User's e-mail", typeof(string));
                Dt.Columns.Add("RM's e-mail", typeof(string));
                Dt.Columns.Add(" RM TL's e-mail", typeof(string));

                foreach (var data in FileData)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = data.CIFID;
                    row[1] = data.USERTYPE;
                    row[2] = data.USERTYPECODE;
                    row[3] = data.EFFECTIVEDATE.ToString("MM/yy");
                    row[4] = data.EXPIREDATE.ToString("MM/yy");
                    row[5] = data.RMID;
                    row[6] = data.RMName;
                    row[7] = data.UserEmail;
                    row[8] = (from s in db.Users where s.Id == data.RMID.ToString() && s.DEL_FLAG == false select s.Email).SingleOrDefault();
                    int RM_TL_ID = (from s in db.RM_Info where s.ID == data.RMID && s.DEL_FLAG == false select s.RM_TL_ID).SingleOrDefault();
                    row[9] = (from s in db.RM_TL_Info where s.ID == RM_TL_ID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
                    Dt.Rows.Add(row);

                }

                var memoryStream = new MemoryStream();
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;
                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Column(2).AutoFit();
                    //byte[] data = excelPackage.GetAsByteArray() as byte[];
                    // Write content to excel file  
                    System.IO.File.WriteAllBytes(filepath, excelPackage.GetAsByteArray());
                    //Close Excel package 
                    worksheet.Dispose();
                    return filename;
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public static void RunCronServiceForWeekday()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";

            try
            {
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicCronlogForWeekday && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var cronScheduleModel = new CRONScheudleModel();
                    cronScheduleModel.Schedule = cronServiceDt.SingleOrDefault().ToString();
                    var cronLog = new CronLog_Iconic();
                    cronLog.LOGMESSAGE = cronScheduleModel.Schedule;
                    cronLog.STARTDATE = DateTime.Now;
                    cronLog.RESPCODE = "003";
                    cronLog.RESPDESC = "SET TIMER";
                    cronLog.TASKNAME = ConstantValues.IconicCronlogForWeekday;
                    cron_daemon.AddJob(cronScheduleModel.Schedule, CronTaskForWeekday);
                    cron_daemon.Start();
                    cronLog.STARTDATE = DateTime.Now;
                    cronLog.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(cronLog);
                    db.SaveChanges();

                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                    Log2.RESPCODE = "999";
                    Log2.RESPDESC = "There is no row for Service name CRONLOG in TBL_SERVICEMANAGEMENT table.";
                    Log2.STARTDATE = DateTime.Now;
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                log.Error("0", controllerName, "RunCronServiceForWeekday", ex);

            }

        }

        public static void RunCronServiceForMonthly()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";
            try
            {
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicReportCron_Monthly && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var cronScheduleModel = new CRONScheudleModel();
                    cronScheduleModel.Schedule = cronServiceDt.SingleOrDefault().ToString();
                    var cronLog = new CronLog_Iconic();
                    cronLog.LOGMESSAGE = cronScheduleModel.Schedule;
                    cronLog.STARTDATE = DateTime.Now;
                    cronLog.RESPCODE = "005";
                    cronLog.RESPDESC = "SET TIMER";
                    cronLog.TASKNAME = ConstantValues.IconicReportCron_Monthly;
                    cron_daemon.AddJob(cronScheduleModel.Schedule, CronTaskForMonthly);
                    cron_daemon.Start();
                    cronLog.STARTDATE = DateTime.Now;
                    cronLog.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(cronLog);
                    db.SaveChanges();


                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                    Log2.RESPCODE = "999";
                    Log2.RESPDESC = "There is no row for Service name CRONLOG in TBL_SERVICEMANAGEMENT table.";
                    Log2.STARTDATE = DateTime.Now;
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                log.Error("0", controllerName, "RunCronServiceForMonthly", ex);

            }

        }
        public static void CronTaskForWeekday()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";
            try
            {

                var datetime = DateTime.Now;
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicReportCronForWeekday && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var timeFormat = cronServiceDt.SingleOrDefault();//weekday only
                    if (datetime.Hour.ToString() == timeFormat) //+ ":" + datetime.Minute.ToString()
                    {

                        var dt = (db.CronLogs_Iconic.Where(a => a.STARTDATE.Value.Year == datetime.Year && a.STARTDATE.Value.Month == datetime.Month && a.STARTDATE.Value.Day == datetime.Day &&
                                                        a.RESPCODE == "004")).OrderByDescending(a => a.ID).ToList();
                        int logcount = dt.Count();
                        if (logcount == 0)
                        {
                            var Log2 = new CronLog_Iconic();
                            Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                            Log2.RESPCODE = "004";
                            Log2.RESPDESC = "Starting";
                            Log2.STARTDATE = DateTime.Now;
                            Log2.ENDDATE = DateTime.Now;
                            db.CronLogs_Iconic.Add(Log2);
                            db.SaveChanges();
                            try
                            {
                                log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "IconicReportTaskForWeekday ");

                                IconicReportTaskForWeekday();
                            }
                            catch (Exception ex)
                            {
                                var errLog = new CronLog_Iconic();
                                errLog.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                                errLog.RESPCODE = "999";
                                errLog.RESPDESC = ex.Message;
                                errLog.STARTDATE = DateTime.Now;
                                errLog.ENDDATE = DateTime.Now;
                                db.CronLogs_Iconic.Add(errLog);
                                db.SaveChanges();
                            }


                        }
                    }


                }

            }
            catch (Exception ex)
            {
                var errLog = new CronLog_Iconic();
                errLog.TASKNAME = ConstantValues.IconicReportCron;
                errLog.RESPCODE = "999";
                errLog.RESPDESC = ex.Message;
                errLog.STARTDATE = DateTime.Now;
                errLog.ENDDATE = DateTime.Now;
                db.CronLogs_Iconic.Add(errLog);
                db.SaveChanges();

            }

        }

        public static void CronTaskForMonthly()
        {
            AdminDBContext db = new AdminDBContext();
            string controllerName = "CIFInfoModels";

            try
            {
                var datetime = DateTime.Now;
                var cronServiceDt =
                    db.ServiceManagements.Where(x => x.SERVICENAME == ConstantValues.IconicReportCronForMonthly && x.Active == true)
                        .Select(x => x.TimeFormat);
                int count = cronServiceDt.Count();
                if (count > 0)
                {
                    var timeFormat = cronServiceDt.SingleOrDefault();//every month first day
                    if (datetime.Hour.ToString() == timeFormat) //+ ":" + datetime.Minute.ToString()
                    {

                        var dt = (db.CronLogs_Iconic.Where(a => a.STARTDATE.Value.Year == datetime.Year && a.STARTDATE.Value.Month == datetime.Month && a.STARTDATE.Value.Day == datetime.Day &&
                                                        a.RESPCODE == "006")).OrderByDescending(a => a.ID).ToList();
                        int logcount = dt.Count();
                        if (logcount == 0)
                        {
                            var Log2 = new CronLog_Iconic();
                            Log2.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                            Log2.RESPCODE = "006";
                            Log2.RESPDESC = "Starting";
                            Log2.STARTDATE = DateTime.Now;
                            Log2.ENDDATE = DateTime.Now;
                            db.CronLogs_Iconic.Add(Log2);
                            db.SaveChanges();
                            try
                            {
                                IconicReportTaskForMonthly();
                            }
                            catch (Exception ex)
                            {
                                var errLog = new CronLog_Iconic();
                                errLog.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                                errLog.RESPCODE = "999";
                                errLog.RESPDESC = ex.Message;
                                errLog.STARTDATE = DateTime.Now;
                                errLog.ENDDATE = DateTime.Now;
                                db.CronLogs_Iconic.Add(errLog);
                                db.SaveChanges();
                            }


                        }
                    }


                }

            }
            catch (Exception ex)
            {
                var errLog = new CronLog_Iconic();
                errLog.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                errLog.RESPCODE = "999";
                errLog.RESPDESC = ex.Message;
                errLog.STARTDATE = DateTime.Now;
                errLog.ENDDATE = DateTime.Now;
                db.CronLogs_Iconic.Add(errLog);
                db.SaveChanges();

            }

        }
        public static void IconicReportTaskForWeekday()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            var TotalCount = 0;
            var SuccessCount = 0;
            var FailCount = 0;
            bool sendEmail = false;
            string status = String.Empty;
            try
            {
                log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "Inner ");

                #region Registration
                var emaillistInfo = db.CIFInfoForTemp.Where(x => x.Status == 0 && (x.UpgradeStatus == 0 || x.UpgradeStatus == 2) && x.DeactivateStatus == 0 && (x.DowngradeStatus == 0 || x.DowngradeStatus == 2) && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "Inner list1");

                if (emaillistInfo.Count > 0)
                {
                    log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "Inner list1 loop");

                    foreach (var item in emaillistInfo)
                    {
                        log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, item.ToString());

                        TotalCount++;
                        status = "registration";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportTask");


                    }
                }

                #endregion

                #region Edit
                var emaillistInfoForEdit = db.CIFInfoForTemp.Where(x => x.Status == 3 && (x.UpgradeStatus == 0 || x.UpgradeStatus == 2) && x.DeactivateStatus == 0 && (x.DowngradeStatus == 0 || x.DowngradeStatus == 2) && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfoForEdit.Count > 0)
                {
                    foreach (var item in emaillistInfoForEdit)
                    {
                        TotalCount++;
                        status = "edit";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportTask");


                    }
                }

                #endregion

                #region upgrade
                var emaillistInfoForUpgrade = db.CIFInfoForTemp.Where(x => x.Status == 1 && x.UpgradeStatus == 1 && x.DeactivateStatus == 0 && (x.DowngradeStatus == 0 || x.DowngradeStatus == 2) && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfoForUpgrade.Count > 0)
                {
                    foreach (var item in emaillistInfoForUpgrade)
                    {
                        TotalCount++;
                        status = "upgrade";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportTask");


                    }
                }

                #endregion

                #region deactivate
                var emaillistInfoForDeactive = db.CIFInfoForTemp.Where(x => x.Status == 1 && (x.UpgradeStatus == 0 || x.UpgradeStatus == 2) && x.DeactivateStatus == 1 && (x.DowngradeStatus == 0 || x.DowngradeStatus == 2) && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfoForDeactive.Count > 0)
                {
                    foreach (var item in emaillistInfoForDeactive)
                    {
                        TotalCount++;
                        status = "deactivate";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportTask");


                    }
                }

                #endregion
                log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, (emaillistInfo.Count > 0).ToString() + (emaillistInfoForEdit.Count > 0).ToString() + (emaillistInfoForUpgrade.Count > 0) + (emaillistInfoForDeactive.Count > 0));

                if (emaillistInfo.Count > 0 || emaillistInfoForEdit.Count > 0 || emaillistInfoForUpgrade.Count > 0 ||
                    emaillistInfoForDeactive.Count > 0)
                {
                    if (sendEmail)
                    {
                        log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "Sendemail success");

                        var logMsg =
                            $"Total Count : {TotalCount}";
                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                        Log2.RESPCODE = "000";
                        Log2.RESPDESC = "Success";
                        Log2.LOGMESSAGE = logMsg;
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();

                    }
                    else
                    {
                        log.Info("0", "CIFInfoModels", "IconicReportTaskForWeekday", 0, "Sendemail fail");

                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                        Log2.RESPCODE = "014";
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCron;
                    Log2.RESPCODE = "013";
                    Log2.RESPDESC = "No Data";
                    Log2.STARTDATE = DateTime.Now;
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                var Log2 = new CronLog_Iconic();
                Log2.TASKNAME = ConstantValues.IconicReportCronForWeekday;
                Log2.RESPCODE = "014";
                Log2.RESPDESC = ex.Message;
                Log2.STARTDATE = DateTime.Now;
                Log2.ENDDATE = DateTime.Now;
                db.CronLogs_Iconic.Add(Log2);
                db.SaveChanges();
                log.Error("0", "CIFInfoModels", "IconicReportTaskForWeekday", ex);

            }
        }

        public static void IconicReportTaskForMonthly()
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            var TotalCount = 0;
            var SuccessCount = 0;
            var FailCount = 0;
            bool sendEmail = false;
            string status = String.Empty;
            try
            {
                #region Renewal
                var emaillistInfoForRenewal = db.CIFInfoForTemp.Where(x => x.Status == 5 && (x.UpgradeStatus == 0 || x.UpgradeStatus == 2) && x.DeactivateStatus == 0 && (x.DowngradeStatus == 0 || x.DowngradeStatus == 2) && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfoForRenewal.Count > 0)
                {
                    foreach (var item in emaillistInfoForRenewal)
                    {
                        TotalCount++;
                        status = "Renewal";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportTask");


                    }
                }

                #endregion

                #region Downgrade
                var emaillistInfoForDowngrade = db.CIFInfoForTemp.Where(x => x.Status == 1 && x.DowngradeStatus == 1 && (x.UpgradeStatus == 0 || x.UpgradeStatus == 2) && x.DeactivateStatus == 0 && x.DELFLAG == false).Select(x => new { x.ApproverEmail }).Distinct().ToList();
                if (emaillistInfoForDowngrade.Count > 0)
                {
                    foreach (var item in emaillistInfoForDowngrade)
                    {
                        TotalCount++;
                        status = "Downgrade";
                        string filename = ExcelExportReportForWeekday(item.ApproverEmail, status);
                        sendEmail = SendEmailForIconicReportForWeekday(item.ApproverEmail, status, "", "CIFInfoModels", filename, "IconicReportCronForMonthly");


                    }
                }

                #endregion

                if (emaillistInfoForRenewal.Count > 0 || emaillistInfoForDowngrade.Count > 0)
                {
                    if (sendEmail)
                    {
                        var logMsg =
                            $"Total Count : {TotalCount}";
                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                        Log2.RESPCODE = "000";
                        Log2.RESPDESC = "Success";
                        Log2.LOGMESSAGE = logMsg;
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();

                    }
                    else
                    {
                        var Log2 = new CronLog_Iconic();
                        Log2.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                        Log2.RESPCODE = "014";
                        Log2.STARTDATE = DateTime.Now;
                        Log2.ENDDATE = DateTime.Now;
                        db.CronLogs_Iconic.Add(Log2);
                        db.SaveChanges();
                    }

                }
                else
                {
                    var Log2 = new CronLog_Iconic();
                    Log2.TASKNAME = ConstantValues.IconicReportCron;
                    Log2.RESPCODE = "013";
                    Log2.RESPDESC = "No Data";
                    Log2.ENDDATE = DateTime.Now;
                    db.CronLogs_Iconic.Add(Log2);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                var Log2 = new CronLog_Iconic();
                Log2.TASKNAME = ConstantValues.IconicReportCronForMonthly;
                Log2.RESPCODE = "014";
                Log2.RESPDESC = ex.Message;
                Log2.STARTDATE = DateTime.Now;
                Log2.ENDDATE = DateTime.Now;
                db.CronLogs_Iconic.Add(Log2);
                db.SaveChanges();
                log.Error("0", "CIFInfoModels", "IconicReportTaskForMonthly", ex);

            }
        }
        public static string ExcelExportReportForWeekday(string email, string status)
        {
            EventLogController log = new EventLogController();
            log.Info("0", "CIFInfoModels", "ExcelExportReportForWeekday", 0, "Reach excelreport");

            string filename = string.Empty;
            string datestamp = General.GetDatestamp(DateTime.Now);
            string CSVFile_Path = ConstantValues.CSVIconicFile_Path;
            //create if folder doesn't exist
            if (!Directory.Exists(CSVFile_Path))
            {
                Directory.CreateDirectory(CSVFile_Path);
            }
            filename = $"{datestamp}_THEICONICPendingCustomerInformation" + ".xlsx";

            string path = $"{CSVFile_Path}";
            string filepath = $"{path}\\{filename}";

            AdminDBContext db = new AdminDBContext();
            var dbInfo = new List<CIFInfoModelForTemp>();
            if (status.Equals("registration"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 0 && x.UpgradeStatus == 0 && x.DeactivateStatus == 0 && x.DowngradeStatus == 0 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            else if (status.Equals("edit"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 4 && x.UpgradeStatus != 0 && x.DeactivateStatus == 0 && x.DowngradeStatus != 0 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            else if (status.Equals("upgrade"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 1 && x.UpgradeStatus == 1 && x.DeactivateStatus == 0 && x.DowngradeStatus != 0 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            else if (status.Equals("deactivate"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 1 && x.UpgradeStatus != 1 && x.DeactivateStatus == 1 && x.DowngradeStatus != 0 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            else if (status.Equals("Renewal"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 5 && x.UpgradeStatus != 1 && x.DeactivateStatus == 0 && x.DowngradeStatus != 0 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            else if (status.Equals("Downgrade"))
            {
                dbInfo = db.CIFInfoForTemp.Where(x => x.Status == 1 && x.UpgradeStatus != 1 && x.DeactivateStatus == 0 && x.DowngradeStatus == 1 && x.ApproverEmail == email && x.DELFLAG == false).ToList();

            }
            List<CIFInfoModelForTemp> FileData = dbInfo.ToList();
            try
            {

                DataTable Dt = new DataTable();
                Dt.Columns.Add("CIFID", typeof(string));
                Dt.Columns.Add("NAME", typeof(string));

                foreach (var data in FileData)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = data.CIFID;
                    row[1] = data.NAME;
                    Dt.Rows.Add(row);

                }

                var memoryStream = new MemoryStream();
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;
                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Column(2).AutoFit();
                    //byte[] data = excelPackage.GetAsByteArray() as byte[];
                    // Write content to excel file  
                    System.IO.File.WriteAllBytes(filepath, excelPackage.GetAsByteArray());
                    //Close Excel package 
                    worksheet.Dispose();
                    return filename;
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        #endregion

        #region SendEmail
        //Get Email Template
        public static General.SMTPSetting GetEmailSetting()
        {
            AdminDBContext db = new AdminDBContext();

            General.SMTPSetting setting = new General.SMTPSetting();
            var mail = (from a in db.Credentials
                        where a.Status == "1" & a.Name == "IconicMailSetting"
                        select a).ToList();
            setting.SenderMail = mail.Where(a => a.Key == "sendermail").Select(n => n.Value).FirstOrDefault();
            setting.SenderName = mail.Where(a => a.Key == "sendername").Select(n => n.Value).FirstOrDefault();
            setting.Password = mail.Where(a => a.Key == "password").Select(n => n.Value).FirstOrDefault();
            setting.Host = mail.Where(a => a.Key == "host").Select(n => n.Value).FirstOrDefault();
            int port = Convert.ToInt32(mail.Where(m => m.Key == "port").Select(m => m.Value).FirstOrDefault());
            setting.Port = port;
            return setting;
        }
        //Validate EmailAddress
        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private EmailTemplate Template(string Type)
        {
            var template = db.EmailTemplates.Where(t => t.Type == Type).FirstOrDefault();
            return template;
        }

        public static Boolean SendEmailForIconic(List<string> mailList, string cifid, string userid, string controller, string action)
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();


            try
            {
                //Validate Email Allow
                var tranretail = "THE ICONIC User Management";
                //Get Data For Email
                General.SMTPSetting setting = GetEmailSetting();
                var template = from t in db.EmailTemplates
                               where t.Type == "iconic_pending_mail"
                               select t;
                var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
               
                
                if (mailList.Count > 0)
                {
                    foreach (var item in mailList)
                    {
                        if (IsEmailValid(item))
                        {
                            Body = Body.Replace("[ReceiverMail]", item);
                            bool sentmail = General.SendEmailForICONIC(item, Subject, Body, setting);
                            //log for email
                            if (sentmail)
                                log.Log(userid, controller, "SendEmailForIconic", $"Email Sent successfully.",
                                    $"Email Address is {item}.");
                            else
                            {
                                log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.",
                                    $"Email Address is {item}.");

                            }
                        }
                        else
                        {
                            log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.", $"This {item} Email Address is Invalid .");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(userid, controller, "SendEmailForIconic", ex);
                return false;
            }

            return true;
        }

        public static Boolean SendEmailForIconicReport(List<string> mailList, string userid, string controller, string filename, string action)
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            try
            {

                //Get Data For Email
                General.SMTPSetting setting = GetEmailSetting();
                var template = from t in db.EmailTemplates
                               where t.Type == "iconic_report"
                               select t;
                var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                if (mailList.Count > 0)
                {
                    foreach (var item in mailList)
                    {
                        if (IsEmailValid(item))
                        {
                            Body = Body.Replace("[SenderName]", (item.ToString()));
                            bool sentmail = General.SendEmailForICONICReport(item, Subject, Body, filename, setting);
                            //log for email
                            if (sentmail)
                                log.Log(userid, controller, "SendEmailForIconic", $"Email Sent successfully.",
                                    $"Email Address is {item}.");
                            else
                            {
                                log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.",
                                    $"Email Address is {item}.");

                            }
                        }
                        else
                        {
                            log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.", $"This {item} Email Address is Invalid .");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(userid, controller, "SendEmailForIconic", ex);
                return false;
            }

            return true;
        }

        public static Boolean SendEmailForIconicReportForWeekday(string mailList, string status, string userid, string controller, string filename, string action)
        {
            EventLogController log = new EventLogController();
            AdminDBContext db = new AdminDBContext();
            try
            {

                //Get Data For Email
                General.SMTPSetting setting = GetEmailSetting();
                var template = from t in db.EmailTemplates
                               where t.Type == "iconic_weekday_reminder"
                               select t;
                var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                Subject = Subject.Replace("[Status]", (status.ToString()));
                var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                if (IsEmailValid(mailList))
                {
                    Body = Body.Replace("[Status]", (status.ToString()));
                    Body = Body.Replace("[email]", (mailList.ToString()));
                    Body = Body.Replace("[Link]", (ConstantValues.IconicMailRoute.ToString()));
                    bool sentmail = General.SendEmailForICONICReport(mailList, Subject, Body, filename, setting);
                    //log for email
                    if (sentmail)
                        log.Log(userid, controller, "SendEmailForIconic", $"Email Sent successfully.",
                            $"Email Address is {mailList}.");
                    else
                    {
                        log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.",
                            $"Email Address is {mailList}.");

                    }
                }
                else
                {
                    log.Log(userid, controller, "SendEmailForIconic", $"Send Email failed.", $"This {mailList} Email Address is Invalid .");

                }

            }
            catch (Exception ex)
            {
                log.Error(userid, controller, "SendEmailForIconic", ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Iconic Approver
        public ActionResult IconicApprover()
        {
            try
            {
                TempData["Stage"] = string.Empty;
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.CIFInfoModels.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult IconicApprover(int? arg1)
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn =
                Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                       + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var cifid = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var username = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user =
                db.CIFInfoModels.Where(
                    a => a.DELFLAG == false && (a.Status == 1 || a.Status == 6) && (a.DowngradeStatus != 1) && (a.DeactivateStatus == 0))
                    .ToList()
                    .ConvertAll(u => new CIFInfoVM
                    {
                        PHONENO = u.PHONENO,
                        NAME = u.NAME,
                        USERTYPE = u.USERTYPE,
                        USERTYPECODE = u.USERTYPECODE,
                        EXPIREDATE = u.EXPIREDATE,
                        strEXPIREDATE = String.Format("{0:MM-yy}", u.EXPIREDATE),
                        NRC = u.NRC,
                        Address = u.ADDRESS,
                        CIFID = u.CIFID,
                        ID = u.ID,
                        REMARK = u.REMARK,
                        Createduserid = u.CREATEDUSERID,
                        Status = u.Status.ToString(),
                        UpgradeStatus = u.UpgradeStatus.ToString()
                    });


            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {

                var getTempData = (from s in db.CIFInfoForTemp
                                   where
                                       s.CIFID == item.CIFID && s.DELFLAG == false && (s.Status == 1 || s.Status == 6) && s.UpgradeStatus == 1 &&
                                       s.DeactivateStatus == 0 && s.DowngradeStatus != 1
                                   select s)
                    .SingleOrDefault();
                if (getTempData != null)
                {
                    item.UpgradeStatus = getTempData.UpgradeStatus.ToString();
                    item.REMARK = getTempData.REMARK.ToString();
                    item.UpdatedDate = getTempData.UpdatedDate;
                    item.RMID = getTempData.RMID;
                    item.RMName = getTempData.RMName;
                    item.ApproverEmail = getTempData.ApproverEmail;
                    item.UserEmail = getTempData.UserEmail;
                    item.EXPIREDATE = getTempData.EXPIREDATE;
                    item.strEXPIREDATE = String.Format("{0:MM-yy}", getTempData.EXPIREDATE);
                    item.EffectiveDate = getTempData.EFFECTIVEDATE;
                    item.strEffectiveDate = String.Format("{0:MM-yy}", getTempData.EFFECTIVEDATE);
                    item.USERTYPE = getTempData.USERTYPE;
                    item.USERTYPECODE = getTempData.USERTYPECODE;
                }
                if (item.UpgradeStatus != "0")
                {
                    if (item.Createduserid != null)
                    {
                        item.Createduserid =
                            db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.UpgradeStatus == "1")
                    {
                        item.Status = "Pending";
                    }
                    else if (item.UpgradeStatus == "2")
                    {
                        item.Status = "Already Approved";
                    }
                    else if (item.UpgradeStatus == "3")
                    {
                        item.Status = "Already Rejected";

                    }
                    userData.Add(item);
                }
                //if (item.UpgradeStatus == "0")
                //{
                //    user.Remove(Info);
                //}


            }
            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = userData.Count();
            var data = userData.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);

        }

        public ActionResult IconicApproverEdit(int? id, string stage)
        {
            if (stage != null)
            {
                ViewBag.FromStage = stage;
                TempData["Stage"] = stage;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string cifid = (from s in db.CIFInfoModels where s.ID == id && s.DELFLAG == false select s.CIFID).SingleOrDefault();

            CIFInfoModelForTemp cIFInfoModel = db.CIFInfoForTemp.Where(x => x.CIFID == cifid && x.DELFLAG == false).SingleOrDefault();
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            CIFInfoVMForApprover infoVm = new CIFInfoVMForApprover();
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForApprover>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            //infoVm.DeactivateDate = DateTime.Now;
            //if (infoVm.DeactivateDate == null)
            //{
            //    infoVm.strDeactivateDate = null;
            //}
            //else
            //{
            //    infoVm.strDeactivateDate = infoVm.DeactivateDate?.ToString("dd/MM/yyyy");
            //}
            infoVm.RMEmail = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            int RM_TL_ID = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.RM_TL_ID).SingleOrDefault();
            infoVm.RM_TL_Email = (from s in db.RM_TL_Info where s.ID == RM_TL_ID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IconicApproverEdit(CIFInfoVMForApprover infoVm)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    var DbInfoFortemp = (from s in db.CIFInfoForTemp where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s).SingleOrDefault();
                    DbInfo.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfo.UpdatedDate = DateTime.Now;
                    var approverEmail = DbInfo.ApproverEmail;
                    DbInfo.UserEmail = infoVm.UserEmail;
                    DbInfo.UpgradeStatus = DbInfoFortemp.UpgradeStatus;

                    if (TempData["Stage"].ToString() == "Approve")
                    {
                        if (DbInfo.UpgradeStatus == 1 && (DbInfo.Status == 1 || DbInfo.Status == 6) && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.UpgradeStatus = 2;
                            DbInfo.USERTYPE = DbInfoFortemp.USERTYPE;
                            DbInfo.USERTYPECODE = DbInfoFortemp.USERTYPECODE;
                            DbInfo.REMARK = DbInfoFortemp.REMARK;
                            DbInfo.RMID = DbInfoFortemp.RMID;
                            DbInfo.RMName = DbInfoFortemp.RMName;
                            DbInfo.EFFECTIVEDATE = DbInfoFortemp.EFFECTIVEDATE;
                            DbInfo.EXPIREDATE = DbInfoFortemp.EXPIREDATE;
                        }
                    }
                    else
                    {

                        if (DbInfo.UpgradeStatus == 1 && (DbInfo.Status == 1 || DbInfo.Status == 6) && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.UpgradeStatus = 3;
                        }

                    }
                    DbInfo.ApproverEmail = approverEmail;
                    #endregion
                    var localEntity = db.Set<CIFInfoModel>().Local.FirstOrDefault(f => f.ID == DbInfo.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    db.Entry(DbInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    #region DataPreparationToupdateCIFtemptable

                    DbInfoFortemp.UpdatedDate = DateTime.Now;
                    DbInfoFortemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoFortemp.DELFLAG = true;
                    db.Entry(DbInfoFortemp).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
                //save log
                ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, "Edit", 2, $"{infoVm.CIFID} Upgrade is approved.");
                List<string> mailList = new List<string>();
                if (infoVm.RMEmail != null)
                {
                    mailList.Add(infoVm.RMEmail);
                }

                mailList.Add(infoVm.UserEmail);
                mailList.Add(infoVm.RM_TL_Email);
                #region sendEmail
                bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "IconicApproverEdit");
                #endregion
                return View(infoVm);

            }
            catch (Exception ex)
            { throw ex; }
            return View();

        }


        #endregion

        #region Iconic Upgrade (Tier 2--> Tier 1) LOT 
        public ActionResult GetCifInfoList()
        {
            try
            {
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.CIFInfoModels.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetCifInfoList(int? arg1)
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
            var cifid = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var username = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            int currentYear = DateTime.Now.Year;
            var user = db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status == 1 || a.Status == 6) && a.UpgradeStatus != 1 && (a.DeactivateStatus == 0) && a.DowngradeStatus != 1).ToList().ConvertAll(u => new CIFInfoVM
            {
                PHONENO = u.PHONENO,
                NAME = u.NAME,
                USERTYPE = u.USERTYPE,
                USERTYPECODE = u.USERTYPECODE,
                EXPIREDATE = u.EXPIREDATE,
                strEXPIREDATE = String.Format("{0:MM-yy}", u.EXPIREDATE),
                NRC = u.NRC,
                Address = u.ADDRESS,
                CIFID = u.CIFID,
                ID = u.ID,
                REMARK = u.REMARK,
                Createduserid = u.CREATEDUSERID,
                UpgradeStatus = u.UpgradeStatus.ToString(),
                DeactivateStatus = u.DeactivateStatus.ToString(),
                Status = u.Status.ToString(),
            });
            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {
                var getType = (from s in db.UserTypeModels
                               where s.USERTYPE == item.USERTYPE && s.DEL_FLAG == false && (s.Type == "Tier 2" || s.Type == "Tier 2.1")
                               select s)
                                    .SingleOrDefault();
                if (getType != null)
                {
                    var getTempData = (from s in db.CIFInfoForTemp
                                       where
                                           s.CIFID == item.CIFID && s.DELFLAG == false && (s.Status == 3 || s.Status == 5 || s.UpgradeStatus == 1 ||
                                           s.DeactivateStatus == 1 || s.DowngradeStatus == 1)
                                       select s)
                        .SingleOrDefault();
                    if (getTempData == null)
                    {
                        Info = item;
                        if (item.Createduserid != null)
                        {
                            Info.Createduserid =
                                db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                        }

                        if (item.UpgradeStatus == "0")
                        {
                            Info.Status = "Normal";
                        }
                        else if (item.UpgradeStatus == "2")
                        {
                            Info.Status = "Already Approved";


                        }
                        else
                        {
                            Info.Status = "Already Rejected";
                        }
                        userData.Add(Info);
                    }
                }

            }


            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = userData.Count();
            var data = userData.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditCifInfoForUpgrade(int? id)
        {
            CIFInfoVM infoVm = new CIFInfoVM();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            cIFInfoModel.DeactivateDate = Convert.ToDateTime(cIFInfoModel.DeactivateDate);

            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            infoVm = JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            IEnumerable<SelectListItem> items2 = db.RM_Info.Where(b => b.DEL_FLAG == false)
                                   .Select(c => new SelectListItem
                                   {
                                       Selected = (c.ID == infoVm.RMID),
                                       Value = c.ID.ToString(),
                                       Text = c.Name
                                   }).ToList();

            ViewBag.RMList = items2;
            string userID = User.Identity.GetUserId();
            ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
            infoVm.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            Session["ApproverEmailList"] = ViewBag.Emaillist;
            Session["RMList"] = ViewBag.RMList;
            ViewBag.getOldDate = cIFInfoModel.EFFECTIVEDATE;
            DateTime today = DateTime.Now;
            DateTime addDate1 = today.AddMonths(1);
            infoVm.NewEffectiveDate = addDate1;
            infoVm.strNewEffectiveDate = addDate1.ToString("MM/yy");
            infoVm.strNewExpireDate = infoVm.NewEffectiveDate.AddDays(365).ToString("MM/yy");
            string new_usertype = String.Empty;
            var getInfo =
                (from s in db.UserTypeModels where s.USERTYPE == infoVm.USERTYPE && s.DEL_FLAG == false select s.Type)
                    .SingleOrDefault();
            if (getInfo == "Tier 2")
            {
                new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 1" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

            }
            if (getInfo == "Tier 2.1")
            {
                new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 1.1" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

            }
            infoVm.USERTYPE = new_usertype;
            infoVm.USERTYPECODE = GenereateCodeForIconic(infoVm.USERTYPE);
            if (infoVm.USERTYPECODE == "0")
            {
                ViewBag.GetCode = "Generated Iconic Number is over exceeded current Usertype Code";
                return View(infoVm);
            }
            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCifInfoForUpgrade(CIFInfoVM infoVm)
        {
            try
            {
                Session["CIFVMForEdit"] = string.Empty;
                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp = JsonConvert.DeserializeObject<CIFInfoModelForTemp>(JsonConvert.SerializeObject(DbInfo));
                    DbInfoForTemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoForTemp.UpdatedDate = DateTime.Now;
                    DbInfoForTemp.ApproverEmail = infoVm.ApproverEmail;
                    DbInfoForTemp.UserEmail = infoVm.UserEmail;
                    DbInfoForTemp.RMID = infoVm.RMID;
                    DbInfoForTemp.RMName = infoVm.RMName;
                    DbInfoForTemp.REMARK = infoVm.REMARK;
                    DbInfoForTemp.UpgradeStatus = 1;//Request upgrade pending
                    DbInfoForTemp.USERTYPECODE = infoVm.USERTYPECODE;
                    DbInfoForTemp.USERTYPE = infoVm.USERTYPE;
                    var dd = infoVm.strNewEffectiveDate.Replace("/", "-");
                    var ddexp = infoVm.strNewExpireDate.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    infoVm.EXPIREDATE = General.ConvertStrToDateForIconic(adddayexp);
                    infoVm.EffectiveDate = General.ConvertStrToDateForIconic(addday);
                    DbInfoForTemp.EFFECTIVEDATE = infoVm.EffectiveDate;
                    DbInfoForTemp.EXPIREDATE = infoVm.EXPIREDATE;
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();
                    #endregion

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, "CIF_Upgrade", 2, $"{infoVm.CIFID} Upgrade is pending.");
                    ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
                    var getBranch = (from s in db.Users where s.Id == getuserid && s.DEL_FLAG == false select s).SingleOrDefault();
                    int branchid = getBranch.BranchID;
                    IEnumerable<SelectListItem> items2 = db.Users.Where(b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 3)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.Email == infoVm.ApproverEmail),
                                        Value = c.Email.ToString(),
                                        Text = c.Email
                                    }).ToList();

                    ViewBag.Emaillist = items2;
                    List<string> mailList = new List<string>();
                    mailList.Add(infoVm.UserEmail);
                    mailList.Add(infoVm.ApproverEmail);
                    #region sendEmail
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "CIF_Upgrade");
                    #endregion
                    ViewBag.Emaillist = Session["ApproverEmailList"];
                    ViewBag.RMList = Session["RMList"];
                    return View(infoVm);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }

        #endregion

        #region Iconic Upgrade (Tier 2--> Tier 1) Before Expired Date

        public JsonResult getCIFIDInfo(string CIFID, string iconicno)
        {
            CIFInfoVMForDeactivator rri = new CIFInfoVMForDeactivator();
            CIFInfoModel info = new CIFInfoModel();
            try
            {
                if (CIFID != "" || iconicno != "")
                {
                    if (CIFID != "")
                    {
                        info =
                            (from s in db.CIFInfoModels where s.CIFID == CIFID && s.DELFLAG == false && s.Status == 1 && s.UpgradeStatus == 0 && s.DeactivateStatus == 0 select s)
                                .SingleOrDefault();

                    }
                    else
                    {
                        info =
                           (from s in db.CIFInfoModels where s.USERTYPECODE == iconicno && s.DELFLAG == false && s.Status == 1 && s.UpgradeStatus == 0 && s.DeactivateStatus == 0 select s)
                               .SingleOrDefault();
                    }
                    if (info == null)
                    {
                        return Json(new { message = "This CIFID is not registered" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        rri.PHONENO = info.PHONENO;
                        rri.NAME = info.NAME;
                        rri.USERTYPE = info.USERTYPE;
                        rri.USERTYPECODE = info.USERTYPECODE;
                        rri.EXPIREDATE = info.EXPIREDATE;
                        rri.EffectiveDate = info.EFFECTIVEDATE;
                        rri.strEXPIREDATE = String.Format("{0:MM/yy}", info.EXPIREDATE);
                        rri.strEffectiveDate = String.Format("{0:MM/yy}", info.EFFECTIVEDATE);
                        rri.NRC = info.NRC;
                        rri.Address = info.ADDRESS;
                        rri.CIFID = info.CIFID;
                        rri.ID = info.ID;
                        rri.RMName = info.RMName;
                        rri.RMID = info.RMID;
                        rri.BranchName = info.BRANCHNAME;

                    }
                    return Json(rri, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(rri, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateCifInfoForUpgrade()
        {
            try
            {
                CIFInfoVMForDeactivator infoVm = new CIFInfoVMForDeactivator();
                if (TempData["createReturn"] == "1")
                {
                    if (Session["CIFVMForUpgradeReturn"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVMForUpgradeReturn"].ToString()))
                        {
                            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForDeactivator>(Session["CIFVMForUpgradeReturn"].ToString());


                        }
                    }
                }


                DateTime today = DateTime.Now;
                DateTime addDate1 = today.AddMonths(1);
                infoVm.NewEffectiveDate = addDate1;
                infoVm.strNewEffectiveDate = addDate1.ToString("MM/yy");
                infoVm.strNewExpireDate = infoVm.NewEffectiveDate.AddDays(365).ToString("MM/yy");
                ViewBag.Booking = GetBookingList();


                return View(infoVm);
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
        public ActionResult CreateCifInfoForUpgrade(CIFInfoVMForDeactivator infoVm)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(infoVm.USERTYPECODE))
                    {
                        string usertypecode = (from s in db.CIFInfoModels where s.USERTYPE == infoVm.USERTYPECODE && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if (!usertypecode.Equals(infoVm.USERTYPECODE))
                        {
                            ModelState.AddModelError("USERTYPECODE", "THE ICONIC Number is Invalid");
                            return View(infoVm);
                        }


                    }
                    if (string.IsNullOrEmpty(infoVm.CIFID))
                    {
                        string cif = (from s in db.CIFInfoModels where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if (!cif.Equals(infoVm.CIFID))
                        {
                            ModelState.AddModelError("CIFID", "The CIFID is Invalid");
                            return View(infoVm);
                        }

                    }
                    string new_usertype = String.Empty;
                    var getInfo =
                        (from s in db.UserTypeModels where s.USERTYPE == infoVm.USERTYPE && s.DEL_FLAG == false select s.Type)
                            .SingleOrDefault();
                    if (getInfo == "Tier 2")
                    {
                        new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 1" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

                    }
                    if (getInfo == "Tier 2.1")
                    {
                        new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 1.1" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

                    }
                    infoVm.USERTYPE = new_usertype;
                    if (infoVm.IconicReservedNo == false)
                    {
                        if (!string.IsNullOrEmpty(new_usertype))
                        {
                            infoVm.NewIconicNo = GenereateCodeForIconic(new_usertype);
                            if (infoVm.NewIconicNo == "0")
                            {
                                ViewBag.GetCode = "Generated Iconic Number is over exceeded current Usertype Code";
                                DateTime today = DateTime.Now;
                                DateTime addDate1 = today.AddMonths(1);
                                infoVm.NewEffectiveDate = addDate1;
                                infoVm.strNewEffectiveDate = addDate1.ToString("MM/yy");
                                infoVm.strNewExpireDate = infoVm.NewEffectiveDate.AddDays(365).ToString("MM/yy");
                                ViewBag.Booking = GetBookingList();
                                return View();
                            }

                        }
                    }
                    else
                    {
                        infoVm.NewIconicNo = infoVm.IconicReservedlist;
                    }
                    //save log
                    Session["CIFVMForUpgrade"] = string.Empty;//To genereate PDF
                    Session["CIFVMForUpgrade"] = JsonConvert.SerializeObject(infoVm);
                    Session["CIFVMForUpgradeReturn"] = JsonConvert.SerializeObject(infoVm);
                    return RedirectToAction("ConfirmCifInfoForUpgrade");
                }

                return View();


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Fetch with CIFID
        public JsonResult getCIFIDInfoForUpgrade(string CIFID, string iconicno)
        {
            CIFInfoVMForDeactivator rri = new CIFInfoVMForDeactivator();
            CIFInfoModel info = new CIFInfoModel();
            try
            {
                if (CIFID != "" || iconicno != "")
                {
                    if (CIFID != "")
                    {
                        info =
                            (from s in db.CIFInfoModels where s.CIFID == CIFID && s.DELFLAG == false && (s.Status == 1 || s.Status == 6) && s.UpgradeStatus != 1 && s.DeactivateStatus == 0 && s.DowngradeStatus != 1 select s)
                                .SingleOrDefault();

                    }
                    else
                    {
                        info =
                           (from s in db.CIFInfoModels where s.USERTYPECODE == iconicno && s.DELFLAG == false && (s.Status == 1 || s.Status == 6) && s.UpgradeStatus != 1 && s.DeactivateStatus == 0 && s.DowngradeStatus != 1 select s)
                               .SingleOrDefault();
                    }
                    if (info == null)
                    {
                        return Json(new { message = "This CIFID is not registered" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(info.USERTYPE))
                        {
                            var getType =
                                (from s in db.UserTypeModels
                                 where s.USERTYPE == info.USERTYPE && s.DEL_FLAG == false && (s.Type == "Tier 2" || s.Type == "Tier 2.1")
                                 select s)
                                    .SingleOrDefault();
                            if (getType != null)
                            {
                                var getTempData = (from s in db.CIFInfoForTemp
                                                   where
                                                       s.CIFID == CIFID && s.DELFLAG == false &&
                                                       (s.Status == 3 || s.Status == 5 || s.UpgradeStatus == 1 ||
                                                        s.DeactivateStatus == 1 || s.DowngradeStatus == 1)
                                                   select s)
                                    .SingleOrDefault();
                                if (getTempData == null)
                                {
                                    rri.PHONENO = info.PHONENO;
                                    rri.NAME = info.NAME;
                                    rri.USERTYPE = info.USERTYPE;
                                    rri.USERTYPECODE = info.USERTYPECODE;
                                    rri.EXPIREDATE = info.EXPIREDATE;
                                    rri.EffectiveDate = info.EFFECTIVEDATE;
                                    rri.strEXPIREDATE = String.Format("{0:MM/yy}", info.EXPIREDATE);
                                    rri.strEffectiveDate = String.Format("{0:MM/yy}", info.EFFECTIVEDATE);
                                    rri.NRC = info.NRC;
                                    rri.Address = info.ADDRESS;
                                    rri.CIFID = info.CIFID;
                                    rri.ID = info.ID;
                                    rri.RMName = info.RMName;
                                    rri.RMID = info.RMID;
                                    rri.BranchName = info.BRANCHNAME;
                                }
                                else
                                {
                                    return Json(new { message = "This CIFID is in pending state" },
                                        JsonRequestBehavior.AllowGet);

                                }
                            }
                            else
                            {
                                return Json(new { message = "This CIFID can't upgrade from Tier 2 to Tier 1" },
                                    JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    return Json(rri, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(rri, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConfirmCifInfoForUpgrade()
        {
            TempData["createReturn"] = "1";
            CIFInfoVMForDeactivator reqmodel = new CIFInfoVMForDeactivator();
            try
            {
                if (TempData["createD"] == "1")
                {
                    if (Session["CIFVMForUpgrade"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVMForUpgrade"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<CIFInfoVMForDeactivator>(Session["CIFVMForUpgrade"].ToString());
                        }
                    }
                }


                string userID = User.Identity.GetUserId();
                ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
                reqmodel.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();



            }
            catch (Exception ex)
            { throw ex; }
            return View(reqmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCifInfoForUpgrade(CIFInfoVMForDeactivator reqVM)
        {
            try
            {
                Session["CIFVMForUpgrade"] = string.Empty;
                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == reqVM.ID && s.DELFLAG == false select s).SingleOrDefault();
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp = JsonConvert.DeserializeObject<CIFInfoModelForTemp>(JsonConvert.SerializeObject(DbInfo));
                    DbInfoForTemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoForTemp.UpdatedDate = DateTime.Now;
                    DbInfoForTemp.ApproverEmail = reqVM.ApproverEmail;
                    DbInfoForTemp.UserEmail = reqVM.UserEmail;
                    DbInfoForTemp.UpgradeStatus = 1;//Request upgrade pending
                    DbInfoForTemp.USERTYPECODE = reqVM.NewIconicNo;
                    DbInfoForTemp.USERTYPE = reqVM.USERTYPE;

                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();
                    #endregion

                    // save log
                    string controllerName = "CIFInfoModels";
                    string getuserid = User.Identity.GetUserId();
                    db.Entry(DbInfo).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            log.Info(getuserid, controllerName, "ConfirmCifInfoForUpgrade", 2, eve.Entry.Entity.GetType().Name);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.Info(getuserid, controllerName, "ConfirmCifInfoForUpgrade", 2, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    if (reqVM.IconicReservedNo == true)
                    {
                        #region DataPreparationToupdateBooking

                        IconicBookingModel dbModel = new IconicBookingModel();
                        var dataFromDB = from s in db.IconicBookingModels
                                         where s.DEL_FLAG == false
                                         select s;
                        dbModel = dataFromDB.SingleOrDefault(m => m.BOOKINGNUMBER == reqVM.NewIconicNo);
                        dbModel.UpdatedDate = DateTime.Now;
                        dbModel.UPDATEDUSERID = User.Identity.GetUserId();
                        dbModel.InUse = true;

                        #endregion

                        db.Entry(dbModel).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //save log
                    ViewBag.CifCreateInfo = "\"" + reqVM.CIFID + "\"";
                    var getBranch = (from s in db.Users where s.Id == getuserid && s.DEL_FLAG == false select s).SingleOrDefault();
                    int branchid = getBranch.BranchID;
                    IEnumerable<SelectListItem> items2 = db.Users.Where(b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 3)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.Email == reqVM.ApproverEmail),
                                        Value = c.Email.ToString(),
                                        Text = c.Email
                                    }).ToList();

                    ViewBag.Emaillist = items2;
                    List<string> mailList = new List<string>();
                    mailList.Add(reqVM.UserEmail);
                    mailList.Add(reqVM.ApproverEmail);
                    #region sendEmail
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, reqVM.CIFID, getuserid, controllerName, "ConfirmCifInfoForUpgrade");
                    #endregion
                    return View(reqVM);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }

        #endregion

        #region GenerateCode

        public static string GenereateCodeForIconic(string usertype)
        {
            AdminDBContext db = new AdminDBContext();
            string code = string.Empty;
            if (!string.IsNullOrEmpty(usertype))
            {
                bool IsContinue = true;
                int increasecount = 1;
                while (IsContinue)
                {
                    //prepare for UserTypeCode 
                    var GeneratedCount = new List<string>();
                    var prefix = string.Empty;
                    var Codelength = db.UserTypeModels.Where(a => a.USERTYPE == usertype).ToList();
                    int length = Codelength[0].GENERATED_LIMIT;
                    var count =
                        db.CIFInfoModels.Where(x => x.USERTYPE == usertype && x.DELFLAG == false)
                            .OrderByDescending(y => y.USERTYPECODE)
                            .Select(x => x.USERTYPECODE)
                            .ToList();
                    GeneratedCount = count;
                    //check count in Temp table
                    var countInTemp =
                        db.CIFInfoForTemp.Where(x => x.USERTYPE == usertype && x.DELFLAG == false)
                            .OrderByDescending(y => y.USERTYPECODE)
                            .Select(x => x.USERTYPECODE)
                            .ToList();
                    if (count.Count > 0 && countInTemp.Count > 0)
                    {
                        int first = Convert.ToInt32(count[0]);
                        int second = Convert.ToInt32(countInTemp[0]);
                        if (first > second)
                        {
                            GeneratedCount = count;
                        }
                        else
                        {
                            GeneratedCount = countInTemp;
                        }
                    }
                    int existno = 0;
                    int check = 0;
                    string takecount = string.Empty;
                    string UserTypecode = Codelength[0].USERTYPE_CODE_LIMIT.ToString();
                    int Usertypecodelength = UserTypecode.Length;
                    if (GeneratedCount.Count > 0)
                    {
                        takecount = GeneratedCount[0].Substring(GeneratedCount[0].LastIndexOf("-") + 1);
                        char[] ch = { takecount[0], takecount[1] };
                        String str = takecount.Substring(Usertypecodelength, takecount.Length - Usertypecodelength);
                        existno = Convert.ToInt32(str);
                        check = existno.ToString().Length;

                    }
                    else
                    {
                        check = 1;
                        existno = 0;
                    }


                    var newString = string.Empty;
                    var generatedCode = string.Empty;
                    int getLength = (length - Usertypecodelength - (existno.ToString().Length) - check);
                    if (getLength >= 1)
                    {
                        newString = prefix.PadLeft((length - Usertypecodelength - (existno.ToString().Length) - 1), '0');
                        generatedCode = UserTypecode + newString + (existno + increasecount);
                    }
                    else if (getLength <= 0)
                    {
                        int getspace = Convert.ToInt32(takecount.ToString().Substring(Usertypecodelength, 1));
                        if (getspace == 9)
                        {
                            generatedCode = (Convert.ToInt32(UserTypecode + existno) + increasecount).ToString();

                        }
                        else
                        {
                            generatedCode = UserTypecode + (existno + increasecount).ToString();

                        }


                    }
                    if (generatedCode.Length == length)
                    {
                        if (generatedCode.StartsWith(UserTypecode))
                        {
                            code = generatedCode;

                            #region checkSamebooking
                            var checkbooking =
                               db.IconicBookingModels.Where(x => x.BOOKINGNUMBER == UserTypecode).ToList();
                            var checkblacklist =
                                db.IconicBlacklistModels.Where(x => x.BlackListNumber == UserTypecode).ToList();
                            if (checkbooking.Count > 0 || checkblacklist.Count > 0)
                            {
                                increasecount += 1;
                                continue;
                            }
                            else
                            {
                                IsContinue = false;
                            }
                            #endregion

                        }
                        else
                        {
                            IsContinue = false;
                            code = "0";

                        }

                    }
                    else
                    {
                        increasecount += 1;
                        continue;
                    }

                }
            }
            return code;
        }
        #endregion
    }
}
