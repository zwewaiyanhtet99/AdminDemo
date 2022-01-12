using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ABankAdmin.Core.Utils;
using System.Globalization;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class RequestController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "RequestController";
        EventLogController log = new EventLogController();
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        private const string RetailType = ConstantValues.Retail;
        private const string CorporateType = ConstantValues.Corporate;

        // GET: Request
        public ActionResult Index()
        {
            //Cretate list  
            UserTypeDDL();
            Getddl();
            return View();
        }
        private void Getddl()
        {
            ViewBag.ruTYPEList = new List<SelectListItem>()
            {
                new SelectListItem {Text="Register",Value="True"},
                new SelectListItem {Text="Update",Value="False"}
            };
            //Create list
            ViewBag.ruSTATUSList = new List<SelectListItem>()
            {
                new SelectListItem{Text="Requested",Value="0"},
                new SelectListItem{Text="Rejected",Value="2"},
                new SelectListItem{Text="All",Value=null}
            };
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
            var textCIFID = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var textUSERNAME = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var textPHONENO = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var textTYPE = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var textSTATUS = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[8][search][value]").FirstOrDefault();
            DateTime? fromDate = null, toDate = null;
            //if(Datefilter!="")
            //{ 
            //string[] strdate = Datefilter.Split('~');
            //if (!string.IsNullOrEmpty(strdate[0].Trim()))
            //{
            //    fromDate = General.ConvertStrToDate(strdate[0]);
            //}
            //if (!string.IsNullOrEmpty(strdate[1].Trim()))
            //{
            //    toDate = General.ConvertStrToDate(strdate[1]).AddDays(1);
            //}
            //}
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
            }


            //#region validation
            ////check toDate is greater than fromDate or not
            //if (toDate < fromDate)
            //{
            //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
            //    UserTypeDDL();
            //    Getddl();
            //    return View();
            //}
            //#endregion

            
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            
            //get requested list of logged in admin
            string userid = User.Identity.GetUserId();
            var requservms = db.ReqUsers.Where(u => u.USER_TYPE=="Retail" & u.MAKER == userid && u.STATUS != 1).ToList().ConvertAll(ru => new ReqUserVM
            {
                ID = ru.ID,
                MOBILENO = ru.MOBILENO,
                USERNAME = ru.USERNAME,
                FULLNAME = ru.FULLNAME,
                NRC = ru.NRC,
                EMAIL = ru.EMAIL,
                ADDRESS = ru.ADDRESS,
                CIFID = ru.CIFID,
                MINOR = ru.MINOR,
                GENDER = ru.GENDER,
                ALLOW_TRAN_COUNT = ru.ALLOW_TRAN_COUNT,
                DAILY_MAX_AMT = ru.DAILY_MAX_AMT,
                ISNEW = ru.ISNEW,
                STATUS = ru.STATUS,
                REQUESTEDDATE = ru.REQUESTEDDATE,
                REQUESTINFO = ru.REQUESTINFO,
                CHECKERREASON = ru.CHECKERREASON,
                IsVIP = ru.IsVIP,
                USER_TYPE = ru.USER_TYPE,
            });
           // SEARCHING...

            if (!string.IsNullOrEmpty(textCIFID))
            {
                requservms = requservms.Where(a => a.CIFID.ToLower().Contains(textCIFID.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(textUSERNAME))
            {
                requservms = requservms.Where(a => a.USERNAME.ToLower().Contains(textUSERNAME.ToLower().Trim())).ToList();
            }
            //if (!string.IsNullOrEmpty(Datefilter))
            //{
            //    DateTime rcdateTime = DateTime.Parse(Datefilter); 
            //    reqChanges = reqChanges.Where(a => a.REQUESTEDDATE.CompareTo(rcdateTime.Date)==0);
            //    //reqChanges = reqChanges.ToList().AsQueryable();
            //}
            if (!string.IsNullOrEmpty(textPHONENO))
            {
                requservms = requservms.Where(a => a.MOBILENO.ToLower().Contains( textPHONENO.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(textTYPE))
            {
                requservms = requservms.Where(a => a.ISNEW.ToString()== textTYPE).ToList();
            }
            //For initial state TestSTATUS
            if (textSTATUS == "")
                textSTATUS = "0";
            if (!string.IsNullOrEmpty(textSTATUS))
            {
                requservms = requservms.Where(a => a.STATUS.ToString()== textSTATUS).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                requservms = requservms.Where(a => a.USER_TYPE==usertype).ToList();
            }
            if (fromDate != null & toDate != null)
            {
                requservms = requservms.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate).ToList();
            }
            else if (fromDate != null)
            {
                requservms = requservms.Where(a => a.REQUESTEDDATE >= fromDate).ToList();
            }
            else if (toDate != null)
            {
                requservms = requservms.Where(a =>a.REQUESTEDDATE < toDate).ToList();
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                requservms = requservms.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }


            recordsTotal = requservms.Count();
            var data = requservms.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);        
    }
        // GET: Request/Details/5
        public ActionResult Detail(int id)
        {
            try
            {
                ReqUser requser = db.ReqUsers.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                User user = db._Users.Where(u => u.CIFID == requser.CIFID).FirstOrDefault();
                if (requser == null)
                {
                    return HttpNotFound();
                }
                ReqUserVM requservm = new ReqUserVM
                {
                    ID = requser.ID,
                    MOBILENO = requser.MOBILENO,
                    USERNAME = requser.USERNAME,
                    FULLNAME = requser.FULLNAME,
                    NRC = requser.NRC,
                    EMAIL = requser.EMAIL,
                    ADDRESS = requser.ADDRESS,
                    CIFID = requser.CIFID,
                    MINOR = requser.MINOR,
                    GENDER = requser.GENDER,
                    ALLOW_TRAN_COUNT = requser.ALLOW_TRAN_COUNT,
                    DAILY_MAX_AMT = requser.DAILY_MAX_AMT,
                    ISNEW = requser.ISNEW,
                    STATUS = requser.STATUS,
                    MAKER = requser.VrMaker.UserName,
                    REQUESTEDDATE = requser.REQUESTEDDATE,
                    REQUESTINFO = requser.REQUESTINFO,
                    CHECKER = (requser.VrChecker == null ? null : requser.VrChecker.UserName),
                    CHECKEDDATE = requser.CHECKEDDATE,
                    CHECKERREASON = requser.CHECKERREASON,
                    CREATEDDATE = (user == null ? null : user.CREATEDDATE),
                    IsVIP = requser.IsVIP,
                    USER_TYPE = requser.USER_TYPE,
                };
                
                //getting accounts by CIFID   
                List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == requser.CIFID && a.ISNEW==requser.ISNEW && a.REQUSER_ID==requser.ID).ToList().ConvertAll(x => new AccountVM
                {
                    ID = x.ID,
                    ACCOUNTNO = x.ACCOUNTNO,
                    ACC_TYPE = x.ACC_TYPE,
                    SCHM_CODE=x.SCHM_CODE,
                    BRANCHCODE = x.BRANCHCODE,
                    CURRENCY = x.CURRENCY,
                    QR_ALLOW = x.QR_ALLOW,
                    Active = true

                });  
                
                ReqUserAndAccVM userandaccvm = new ReqUserAndAccVM();
                userandaccvm.reqUservm = requservm;
                userandaccvm.accountvms = accvms;
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //To fetch data from core by CIFID
        public JsonResult getRetail(string CIFID)
        {
            RetailResponseInfo rri = new RetailResponseInfo();
            try
            {
                if (CIFID != "")
                {
                    int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    if (count > 0)
                        return Json(new { message = "Duplicate CIFID" }, JsonRequestBehavior.AllowGet);
                    if (db.ReqUsers.Where(u => u.CIFID == CIFID & u.STATUS == 0).Count() > 0)
                    {
                        return Json(new { message = "Already Requested!" }, JsonRequestBehavior.AllowGet);
                    }
                        rri = GetRetailResponse(CIFID);
                    if (rri.NRC != null && rri.lAcctInfo.Count == 0)
                        return Json(new { message = "No individual account under this CIFID" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(rri, JsonRequestBehavior.AllowGet);
        }
        public ActionResult register()
        {
            try
            {
                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = new UserVM();
                return View(userandaccvm);
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
        public ActionResult register(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    UserVM uservm = userandaccvm.uservm;
                    //reget core data by CIFID
                    RetailResponseInfo rri = new RetailResponseInfo();
                    if (uservm.CIFID != "")
                        rri = GetRetailResponse(uservm.CIFID);
                    if (rri.NRC == null) //if wrong CIFID
                    {
                        ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                        return View(userandaccvm);
                    }
                    //checking Validation
                    #region Validation
                    //CIFID duplicate
                    if (db._Users.Where(u => u.CIFID == uservm.CIFID & u.DEL_STATUS == false).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.CIFID", "Duplicate CIFID");
                        return View(userandaccvm);
                    }
                    //duplicate requested CIFID (ReqUsers)
                    if (db.ReqUsers.Where(u => u.CIFID == uservm.CIFID & u.ISNEW & u.STATUS==0).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.CIFID", "This CIFID is already requested.");
                        return View(userandaccvm);
                    }

                    #region username
                    //space and special char
                    Regex rgx = new Regex("^[a-zA-Z0-9]+?$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Username isn't allowed space and special character.");
                        return View(userandaccvm);
                    }
                    //length
                    rgx = new Regex("^([a-zA-Z0-9]){6,16}$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Minimum length is 6 and maximum length is 16.");
                        return View(userandaccvm);
                    }
                    //staring with number
                    rgx = new Regex("^[a-zA-Z]+([a-zA-Z0-9]){5,15}$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Invalid USERNAME, Username isn't allowed to start with number.");
                        return View(userandaccvm);
                    }
                    //username duplicate
                    if (db._Users.Where(u => u.USERNAME.ToLower() == uservm.USERNAME.ToLower() && u.DEL_STATUS == false).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Duplicate USERNAME");
                        return View(userandaccvm);
                    }
                    //duplicate requested username (ReqUsers)
                    if (db.ReqUsers.Where(u => u.USERNAME.ToLower() == uservm.USERNAME.ToLower() & u.STATUS==0).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Duplicate Requested USERNAME");
                        return View(userandaccvm);
                    }
                    #endregion
                    #region VIP
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    //convert tran_amt and max_amt from string to int
                    if (uservm.IsVIP)
                    {
                        try
                    {
                        allow_tran_count = General.ConvertStrToInt(uservm.ALLOW_TRAN_COUNT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "The field Allow Tran limit must be between 0 and 2,147,483,647.");
                        return View(userandaccvm);
                    }
                    try
                    {
                        daily_max_amount = General.ConvertStrToDecimal(uservm.DAILY_MAX_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9,999,999,999,999,999.99");
                        return View(userandaccvm);
                    }
                   
                        //allow tran count
                        if ( uservm.ALLOW_TRAN_COUNT == null || (allow_tran_count <= GetRule("R003")))
                        {
                            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count must be greater than Rule's count.");
                            return View(userandaccvm);
                        }
                        //allow tran count
                        if ( uservm.DAILY_MAX_AMT == null || (daily_max_amount <= GetDecimalRule("R005")))
                        {
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's Amount.");
                            return View(userandaccvm);
                        }
                    }
                    #endregion
                    //checking to choose at least one account
                    if (userandaccvm.accountvms == null)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }
                    List<AccountVM> accvms = userandaccvm.accountvms;
                    List<AccountVM> allowedaccvms = accvms.Where(a => a.Active == true).ToList();
                    if (allowedaccvms.Count == 0)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }

                    //check also at Request table


                    #endregion

                    string LoginUser = User.Identity.GetUserId();

                    //save into requested user table (temp)
                    ReqUser requser = new ReqUser();
                    //core data
                    requser.NRC = rri.NRC;
                    requser.FULLNAME = rri.Name;
                    requser.MINOR = rri.Minor;
                    requser.GENDER = rri.Gender;
                    //formdata
                    requser.CIFID = uservm.CIFID;
                    requser.USERNAME = uservm.USERNAME;
                    requser.MOBILENO = uservm.MOBILENO;
                    requser.EMAIL = uservm.EMAIL;
                    requser.ADDRESS = uservm.ADDRESS;
                    //Check VIP
                    requser.IsVIP = uservm.IsVIP;
                    requser.USER_TYPE = RetailType;//new User Type field                  
                    if (uservm.IsVIP)//save for only VIP users
                    {
                        //get form inputs
                        requser.ALLOW_TRAN_COUNT = allow_tran_count;
                        requser.DAILY_MAX_AMT = daily_max_amount;   
                    }
                    else
                    {
                        //get rule values for Normal User
                        requser.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }
                    //request Info
                    requser.REQUESTINFO = "";

                    //Default fields
                    requser.ISNEW = true;   //true for register
                    requser.STATUS = 0;     //status => 0   request status, 1  approve, 2 reject
                    requser.MAKER = LoginUser;
                    requser.REQUESTEDDATE = DateTime.Now;                  
                    //get BranchID from Logged In User
                    AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                    requser.BranchID = admin.BranchID;

                    db.ReqUsers.Add(requser);
                    db.SaveChanges();
                    //save log
                    log.Info(LoginUser, controllerName, actionName, 1, $"User ID is {requser.USERNAME}.");

                    //saving accounts
                    if (allowedaccvms.Count() > 0)//allowedaccvms is only selected accounts
                    {
                        List<AccountInfo> core_accs = rri.lAcctInfo;
                        SaveAccounts(allowedaccvms, requser.CIFID, core_accs, requser.ID);
                    }
                    //success
                    return RedirectToAction("index");
                }
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //save accounts for registration (new)
        private void SaveAccounts(List<AccountVM> accvms, string CIFID, List<AccountInfo> core_accs, int ReqUserID)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {               
                int saveCount = 0;
                foreach (AccountVM accvm in accvms)
                {
                    if (!accvm.Active)
                        continue;
                    var core_acc = core_accs.Where(c => c.AccountNumber == accvm.ACCOUNTNO).FirstOrDefault();
                    if (core_acc == null)//invalid acc no
                        continue;
                    saveCount++;
                    ReqAcc reqacc = new ReqAcc();
                    //core data
                    reqacc.ACCOUNTNO = core_acc.AccountNumber;
                    reqacc.ACC_TYPE = core_acc.AccountType;
                    reqacc.ACC_DESC = core_acc.AccountTypeDesc;
                    reqacc.SCHM_CODE = core_acc.Schm_Code;
                    reqacc.BRANCHCODE = core_acc.BranchID;
                    reqacc.CURRENCY = core_acc.Currency;
                    //form data
                    reqacc.ISNEW = true;
                    //QR allow false if Acc type is  ODA, LAA, TDA
                    if (reqacc.ACC_TYPE == "ODA" || reqacc.ACC_TYPE == "LAA" || reqacc.ACC_TYPE == "TDA" /*|| reqacc.CURRENCY != "MMK"*/)
                    {
                        reqacc.QR_ALLOW = false;
                    }
                    else
                        reqacc.QR_ALLOW = accvm.QR_ALLOW;
                    //default values
                    reqacc.CIFID = CIFID;
                    reqacc.REQUSER_ID = ReqUserID;

                    db.ReqAccs.Add(reqacc);
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

        //Edit
        public ActionResult Edit(int id) 
        {
            try
            {                
                ReqUser user = db.ReqUsers.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();

                //old code for Retail
                //rri = GetRetailResponse(user.CIFID);
                //UserVM uservm = new UserVM
                //{
                //    ID = user.ID,
                //    MOBILENO = user.MOBILENO,
                //    USERNAME = user.USERNAME,
                //    FULLNAME = rri.Name,
                //    NRC = rri.NRC,
                //    EMAIL = user.EMAIL,
                //    ADDRESS = user.ADDRESS,
                //    CIFID = user.CIFID,
                //    MINOR = rri.Minor,
                //    GENDER = rri.Gender,
                //    ALLOW_TRAN_COUNT = user.ALLOW_TRAN_COUNT,
                //    DAILY_MAX_AMT = user.DAILY_MAX_AMT,
                //    IsVIP = user.IsVIP,
                //};
                ////getting accounts by CIFID
                //List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == user.CIFID && a.ISNEW==user.ISNEW && a.REQUSER_ID == user.ID).ToList().ConvertAll(x => new AccountVM
                //{
                //    ID = x.ID,
                //    ACCOUNTNO = x.ACCOUNTNO,
                //    ACC_TYPE = x.ACC_TYPE,
                //    ACC_DESC = x.ACC_DESC,
                //    SCHM_CODE=x.SCHM_CODE,
                //    BRANCHCODE = x.BRANCHCODE,
                //    CURRENCY = x.CURRENCY,
                //    QR_ALLOW = x.QR_ALLOW,
                //    Active=true
                //});
                //List<AccountVM> closedaccvms = new List<AccountVM>();
                ////getting core accounts
                //if (rri.lAcctInfo.Count > 0)
                //{
                //    List<AccountInfo> core_accs = rri.lAcctInfo;
                //    //remove closed acc
                //    AccountVM[] tmp = new AccountVM[accvms.Count()];
                //    accvms.CopyTo(tmp);
                //    foreach (var dbacc in tmp)
                //    {
                //        if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                //        {
                //            accvms.Remove(dbacc);
                //            closedaccvms.Add(dbacc);
                //        }
                //    }

                //    //add new core acc
                //    foreach (var acc in core_accs)
                //    {
                //        if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                //        {
                //            accvms.Add(new AccountVM
                //            {
                //                ID = 0,
                //                ACCOUNTNO = acc.AccountNumber,
                //                ACC_TYPE = acc.AccountType,
                //                ACC_DESC = acc.AccountTypeDesc,
                //                SCHM_CODE=acc.Schm_Code,
                //                BRANCHCODE = acc.BranchID,
                //                CURRENCY = acc.Currency,
                //                Active = false,
                //                QR_ALLOW = false
                //            });
                //        }
                //    }
                //}

                //UserAndAccVM userandaccvm = new UserAndAccVM();
                //userandaccvm.uservm = uservm;
                //userandaccvm.accountvms = accvms;
                //userandaccvm.closedaccs = closedaccvms;

                //new code
                UserAndAccVM userandaccvm = new UserAndAccVM();

                if (user.USER_TYPE == RetailType)
                {
                    rri = GetRetailResponse(user.CIFID);
                    UserVM uservm = new UserVM
                    {
                        ID = user.ID,
                        MOBILENO = user.MOBILENO,
                        USERNAME = user.USERNAME,
                        FULLNAME = rri.Name,
                        NRC = rri.NRC,
                        EMAIL = user.EMAIL,
                        ADDRESS = user.ADDRESS,
                        CIFID = user.CIFID,
                        MINOR = rri.Minor,
                        GENDER = rri.Gender,
                        ALLOW_TRAN_COUNT = General.ConvertIntToStr(user.ALLOW_TRAN_COUNT),
                        DAILY_MAX_AMT = General.ConvertNDecimalToStr(user.DAILY_MAX_AMT),
                        IsVIP = user.IsVIP,
                        USER_TYPE=user.USER_TYPE,
                    };
                    //getting accounts by CIFID
                    List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == user.CIFID && a.ISNEW == user.ISNEW && a.REQUSER_ID == user.ID).ToList().ConvertAll(x => new AccountVM
                    {
                        ID = x.ID,
                        ACCOUNTNO = x.ACCOUNTNO,
                        ACC_TYPE = x.ACC_TYPE,
                        ACC_DESC = x.ACC_DESC,
                        SCHM_CODE = x.SCHM_CODE,
                        BRANCHCODE = x.BRANCHCODE,
                        CURRENCY = x.CURRENCY,
                        QR_ALLOW = x.QR_ALLOW,
                        Active = true
                    });
                    List<AccountVM> closedaccvms = new List<AccountVM>();
                    //getting core accounts
                    if (rri.lAcctInfo.Count > 0)
                    {
                        List<AccountInfo> core_accs = rri.lAcctInfo;
                        //remove closed acc
                        AccountVM[] tmp = new AccountVM[accvms.Count()];
                        accvms.CopyTo(tmp);
                        foreach (var dbacc in tmp)
                        {
                            if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                            {
                                accvms.Remove(dbacc);
                                closedaccvms.Add(dbacc);
                            }
                        }

                        //add new core acc
                        foreach (var acc in core_accs)
                        {
                            if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                            {
                                accvms.Add(new AccountVM
                                {
                                    ID = 0,
                                    ACCOUNTNO = acc.AccountNumber,
                                    ACC_TYPE = acc.AccountType,
                                    ACC_DESC = acc.AccountTypeDesc,
                                    SCHM_CODE = acc.Schm_Code,
                                    BRANCHCODE = acc.BranchID,
                                    CURRENCY = acc.Currency,
                                    Active = false,
                                    QR_ALLOW = false
                                });
                            }
                        }
                    }

                    userandaccvm.uservm = uservm;
                    userandaccvm.accountvms = accvms;
                    userandaccvm.closedaccs = closedaccvms;
                }
                else if (user.USER_TYPE == CorporateType)
                {
                    cri = GetCorporateResponse(user.CIFID);
                    UserVM uservm = new UserVM
                    {
                        ID = user.ID,
                        MOBILENO = user.MOBILENO,
                        USERNAME = user.USERNAME,
                        FULLNAME = cri.Name,
                        NRC = cri.AccountOpenDate,
                        EMAIL = user.EMAIL,
                        ADDRESS = user.ADDRESS,
                        CIFID = user.CIFID,
                        MINOR = user.MINOR,
                        GENDER = user.GENDER,
                        ALLOW_TRAN_COUNT = user.ALLOW_TRAN_COUNT.ToString(),
                        DAILY_MAX_AMT = user.DAILY_MAX_AMT.ToString(),
                        IsVIP = user.IsVIP,
                        USER_TYPE=user.USER_TYPE,
                    };
                    //getting accounts by CIFID
                    List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == user.CIFID && a.ISNEW == user.ISNEW && a.REQUSER_ID == user.ID).ToList().ConvertAll(x => new AccountVM
                    {
                        ID = x.ID,
                        ACCOUNTNO = x.ACCOUNTNO,
                        ACC_TYPE = x.ACC_TYPE,
                        ACC_DESC = x.ACC_DESC,
                        SCHM_CODE = x.SCHM_CODE,
                        BRANCHCODE = x.BRANCHCODE,
                        CURRENCY = x.CURRENCY,
                        QR_ALLOW = x.QR_ALLOW,
                        Active = true
                    });
                    List<AccountVM> closedaccvms = new List<AccountVM>();
                    //getting core accounts
                    if (cri.lstCorpAccountInfo.Count > 0)
                    {
                        List<AccountInfo> core_accs = cri.lstCorpAccountInfo.Select(ca => new AccountInfo
                        {
                            AccountNumber = ca.AccountNumber,
                            AccountType = ca.AccountType,
                            AccountTypeDesc = ca.AccountTypeDesc,
                            Schm_Code = ca.Schm_Code,
                            BranchID = ca.BranchID,
                            Currency = ca.Currency
                        }).ToList();
                        //remove closed acc
                        AccountVM[] tmp = new AccountVM[accvms.Count()];
                        accvms.CopyTo(tmp);
                        foreach (var dbacc in tmp)
                        {
                            if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                            {
                                accvms.Remove(dbacc);
                                closedaccvms.Add(dbacc);
                            }
                        }

                        //add new core acc
                        foreach (var acc in core_accs)
                        {
                            if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                            {
                                accvms.Add(new AccountVM
                                {
                                    ID = 0,
                                    ACCOUNTNO = acc.AccountNumber,
                                    ACC_TYPE = acc.AccountType,
                                    ACC_DESC = acc.AccountTypeDesc,
                                    SCHM_CODE = acc.Schm_Code,
                                    BRANCHCODE = acc.BranchID,
                                    CURRENCY = acc.Currency,
                                    Active = false,
                                    QR_ALLOW = false
                                });
                            }
                        }
                    }

                    userandaccvm.uservm = uservm;
                    userandaccvm.accountvms = accvms;
                    userandaccvm.closedaccs = closedaccvms;
                }

                //to send Request type to make UserName field read-only or not
                ViewBag.ISNEW = user.ISNEW;

                return View(userandaccvm);
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
        public ActionResult Edit(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                userandaccvm.closedaccs= new List<AccountVM>();
                if (ModelState.IsValid)
                {
                    UserVM uservm = userandaccvm.uservm;
                    List<AccountVM> accvms = userandaccvm.accountvms;
                    ReqUser requser = db.ReqUsers.Find(uservm.ID);
                    if (requser == null)
                    {
                        return HttpNotFound();
                    }
                    //to send Request type to make UserName field read-only or not
                    ViewBag.ISNEW = requser.ISNEW;
                    //not to allow editing approved requests
                    if (requser.STATUS == 1) //if approved
                    {
                        ModelState.AddModelError("", "You can't change the approved request!");
                        return View(userandaccvm);
                    }
                    //checking CIFID
                    RetailResponseInfo rri = new RetailResponseInfo();
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    if (requser.USER_TYPE == RetailType)
                    {
                        rri = GetRetailResponse(requser.CIFID);

                        if (rri.NRC == null) //if wrong CIFID
                        {
                            ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                            return View(userandaccvm);
                        }
                    }
                    else if (requser.USER_TYPE == CorporateType)
                    {
                        cri = GetCorporateResponse(requser.CIFID); 
                        
                        if (cri.Name == null) //check Name if corporate CIFID
                        {
                            ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                            return View(userandaccvm);
                        }
                    }
                    //checking Validation
                    //#region Validation
                    ////txn count
                    //if (uservm.ALLOW_TRAN_COUNT == null)
                    //{
                    //    ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count is required.");
                    //    return View(userandaccvm);
                    //}
                    ////max amt
                    //if (uservm.DAILY_MAX_AMT == null)
                    //{
                    //    ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount is required.");
                    //    return View(userandaccvm);
                    //}
                    #region VIP
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    //convert tran_amt and max_amt from string to int
                    try
                    {
                        allow_tran_count = General.ConvertStrToInt(uservm.ALLOW_TRAN_COUNT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "The field Allow Tran limit must be between 0 and 2,147,483,647.");
                        return View(userandaccvm);
                    }
                    try
                    {
                        daily_max_amount = General.ConvertStrToDecimal(uservm.DAILY_MAX_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9,999,999,999,999,999.99");
                        return View(userandaccvm);
                    }
                    if (uservm.IsVIP)
                    {
                        //allow tran count
                        if (uservm.ALLOW_TRAN_COUNT == null || (allow_tran_count <= GetRule("R003")))
                        {
                            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count must be greater than Rule's count.");
                            return View(userandaccvm);
                        }
                        //allow tran count
                        if (uservm.DAILY_MAX_AMT == null || (daily_max_amount <= GetDecimalRule("R005")))
                        {
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's Amount.");
                            return View(userandaccvm);
                        }
                    }
                    #endregion

                    //Check to choose at least one acc
                    if (accvms.Count == 0)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }
                    List<AccountVM> allowedaccvms = accvms.Where(a => a.Active == true).ToList();
                    if (allowedaccvms.Count == 0)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }

                    //#endregion

                    //core data
                    if (requser.USER_TYPE == RetailType)
                    {
                        requser.NRC = rri.NRC;
                        requser.FULLNAME = rri.Name;
                        requser.MINOR = rri.Minor;
                        requser.GENDER = rri.Gender;
                    }
                    else if (requser.USER_TYPE == CorporateType)
                    {
                        requser.NRC = cri.AccountOpenDate;
                        requser.FULLNAME = cri.Name;
                        requser.MINOR = uservm.MINOR;
                        requser.GENDER = uservm.GENDER;
                    }

                    //formdata
                    //get form data if request is only new (register) 
                    if (requser.ISNEW)
                    {
                        requser.USERNAME = uservm.USERNAME;
                    }
                    requser.MOBILENO = uservm.MOBILENO;
                    requser.EMAIL = uservm.EMAIL;
                    requser.ADDRESS = uservm.ADDRESS;
                    //requser.ALLOW_TRAN_COUNT = allow_tran_count;
                    //requser.DAILY_MAX_AMT = daily_max_amount;
                    //Check VIP
                    requser.IsVIP = uservm.IsVIP;//Update VIP flag
                    if (uservm.IsVIP)
                    {
                        //get form inputs
                        requser.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    else
                    {
                        //get rule values for Normal User
                        requser.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }
                    //default fields
                    if (requser.STATUS == 2) 
                    {
                        requser.STATUS = 0;     //change status back to requested status
                    }
                    requser.REQUESTEDDATE = DateTime.Now;
                    db.Entry(requser).State = EntityState.Modified;
                    db.SaveChanges();
                    // save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{uservm.USERNAME} is updated.");

                    //updating accounts
                    List<AccountInfo> core_accs = new List<AccountInfo>();
                    if (requser.USER_TYPE == RetailType)
                    {
                        core_accs = rri.lAcctInfo;
                    } 
                    else if (requser.USER_TYPE == CorporateType)
                    {
                        core_accs = cri.lstCorpAccountInfo.Select(c=> new AccountInfo 
                        {
                            AccountNumber = c.AccountNumber,
                            AccountType = c.AccountType,
                            AccountTypeDesc = c.AccountTypeDesc,
                            Schm_Code = c.Schm_Code,
                            BranchID = c.BranchID,
                            Currency = c.Currency
                        }).ToList();
                    }

                    UpdateAccounts(accvms, requser.CIFID, core_accs, requser.ISNEW, requser.ID);

                    return RedirectToAction("Index");
                }
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
   

        //update accounts
        private void UpdateAccounts(List<AccountVM> accvms, string CIFID, List<AccountInfo> core_accs, Boolean IsNew, int ReqUserID)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = User.Identity.GetUserId();
            try
            {
                //remove all reqaccs (resetting)
                db.ReqAccs.RemoveRange(db.ReqAccs.Where(ra => ra.CIFID==CIFID && ra.ISNEW== IsNew && ra.REQUSER_ID == ReqUserID));
                db.SaveChanges();

                //loop all shown accounts
                int saveCount = 0;
                foreach (var accvm in accvms)
                {
                    if (!accvm.Active)//uncheck accounts will not be saved. (skipping)
                        continue;
                    var core_acc = core_accs.Where(c => c.AccountNumber == accvm.ACCOUNTNO).FirstOrDefault();
                    if (core_acc == null)//invalid acc no   or  closed accs at core
                        continue;
                    saveCount++;    //increment saved count
                    ReqAcc acc = new ReqAcc();
                    //core data
                    acc.ACCOUNTNO = core_acc.AccountNumber;
                    acc.ACC_TYPE = core_acc.AccountType;
                    acc.ACC_DESC = core_acc.AccountTypeDesc;
                    acc.SCHM_CODE = core_acc.Schm_Code;
                    acc.BRANCHCODE = core_acc.BranchID;
                    acc.CURRENCY = core_acc.Currency;
                    acc.ISNEW = IsNew;
                    //form data
                    //QR allow false if Acc type is  ODA, LAA, TDA ,not MMK
                    if (accvm.ACC_TYPE == "ODA" || accvm.ACC_TYPE == "LAA" || accvm.ACC_TYPE == "TDA"/* || accvm.CURRENCY != "MMK"*/)
                    {
                        acc.QR_ALLOW = false;
                    }
                    else
                        acc.QR_ALLOW = accvm.QR_ALLOW;
                    //default values
                    acc.CIFID = CIFID;
                    acc.REQUSER_ID = ReqUserID;

                    db.ReqAccs.Add(acc);
                    db.SaveChanges();
                }

                if (saveCount > 0)
                    log.Info(getuserid, controllerName, actionName, 2, $"{saveCount} accounts saved.");
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
            }
        }

        // GET: Request/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                ReqUser requser = db.ReqUsers.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                if (requser == null)
                {
                    return HttpNotFound();
                }
                UserVM uservm = new UserVM
                {
                    ID = requser.ID,
                    MOBILENO = requser.MOBILENO,
                    USERNAME = requser.USERNAME,
                    FULLNAME = requser.FULLNAME,
                    NRC = requser.NRC,
                    EMAIL = requser.EMAIL,
                    ADDRESS = requser.ADDRESS,
                    CIFID = requser.CIFID,
                    MINOR = requser.MINOR,
                    GENDER = requser.GENDER,
                    ALLOW_TRAN_COUNT = General.ConvertIntToStr(requser.ALLOW_TRAN_COUNT),
                    DAILY_MAX_AMT = General.ConvertNDecimalToStr(requser.DAILY_MAX_AMT),
                    IsVIP = requser.IsVIP,
                    USER_TYPE=requser.USER_TYPE
                };
                //getting accounts by CIFID
                List<AccountVM> accvms = db.ReqAccs.Where(a =>  a.CIFID == requser.CIFID && a.ISNEW==requser.ISNEW && a.REQUSER_ID == requser.ID).ToList().ConvertAll(x => new AccountVM
                {
                    ID = x.ID,
                    ACCOUNTNO = x.ACCOUNTNO,
                    ACC_TYPE = x.ACC_TYPE,
                    SCHM_CODE=x.SCHM_CODE,
                    BRANCHCODE = x.BRANCHCODE,
                    CURRENCY = x.CURRENCY,
                    QR_ALLOW = x.QR_ALLOW,
                    Active=true
                });
                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = uservm;
                userandaccvm.accountvms = accvms;

                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ReqUser requser = db.ReqUsers.Find(id);
                //don't allow to delete approved request
                if (requser.STATUS == 1) 
                {
                    ModelState.AddModelError("", "You can't delete approved request.");
                    return RedirectToAction("Delete", new { id = id });
                }
                string deleteData = $"{db.ReqAccs.Where(a => a.CIFID == requser.CIFID && a.ISNEW==requser.ISNEW && a.REQUSER_ID == requser.ID).Count()} accounts deleted.";
                //delete accounts if needs      //with CIFID
                db.ReqAccs.RemoveRange(db.ReqAccs.Where(a => a.CIFID == requser.CIFID && a.ISNEW == requser.ISNEW && a.REQUSER_ID == requser.ID));
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 3, deleteData);
                //delete user
                db.ReqUsers.Remove(requser);
                db.SaveChanges();
                //save log
                deleteData = requser.ToString();
                log.Info(getuserid, controllerName, actionName, 3, deleteData);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }      
        public ActionResult UserList()
        {
            //int count = db._Users.Count(); 
            UserTypeDDL();
                return View();
        }
        [HttpPost]
        public ActionResult UserList(int? arg1) 
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
            var phoneNo = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
            int IntRule = Convert.ToInt32(rule.Value);
            int ruleValue = (rule == null ? 5 : IntRule);

            var user = db._Users.Where(a => a.USER_TYPE=="Retail" & a.DEL_STATUS == false).ToList().ConvertAll(u => new UserVM
            {
                ID = u.ID,
                MOBILENO = u.MOBILENO,
                USERNAME = u.USERNAME,
                FULLNAME = u.FULLNAME,
                NRC = u.NRC,
                EMAIL = u.EMAIL,
                ADDRESS = u.ADDRESS,
                CIFID = u.CIFID,
                MINOR = u.MINOR,
                GENDER = u.GENDER,
                ALLOW_TRAN_COUNT = u.ALLOW_TRAN_COUNT.ToString(),
                DAILY_MAX_AMT = u.DAILY_MAX_AMT.ToString(),
                ISLOCK_FLAG = u.ISLOCK_FLAG,
                TRAN_LOCK = (u.FAIL_TRAN_PWD_COUNT >= ruleValue),
                IsVIP = u.IsVIP,
                USER_TYPE=u.USER_TYPE,
            });
            //For Seraching
            //List<ReqUser> reqUser = db.ReqUsers.Where(r => r.STATUS == 0).ToList();

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid))
            {
                user = user.Where(a => a.CIFID.ToLower().Contains(cifid.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(username))
            {
                //int branchid = Convert.ToInt32(username);
                user = user.Where(a => a.USERNAME.ToLower().Contains(username.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(phoneNo))
            {
                //int roleid = Convert.ToInt32(role);
                user = user.Where(a => a.MOBILENO.Contains(phoneNo)).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //int roleid = Convert.ToInt32(role);
                user = user.Where(a => a.USER_TYPE==usertype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        //UserDetail
        public ActionResult UserDetails(int id)
        {
            try
            {
                User user = db._Users.Where(u => u.DEL_STATUS == false && u.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                UserVM uservm = new UserVM
                {
                    ID = user.ID,
                    MOBILENO = user.MOBILENO,
                    USERNAME = user.USERNAME,
                    FULLNAME = user.FULLNAME,
                    NRC = user.NRC,
                    EMAIL = user.EMAIL,
                    ADDRESS = user.ADDRESS,
                    CIFID = user.CIFID,
                    MINOR = user.MINOR,
                    GENDER = user.GENDER,
                    ALLOW_TRAN_COUNT = General.ConvertIntToStr(user.ALLOW_TRAN_COUNT),
                    DAILY_MAX_AMT = General.ConvertNDecimalToStr(user.DAILY_MAX_AMT),
                    CREATEDDATE= user.CREATEDDATE,
                    IsVIP = user.IsVIP,
                    USER_TYPE=user.USER_TYPE,
                };
                //getting accounts by CIFID
                List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                {
                    ID = x.ID,
                    ACCOUNTNO = x.ACCOUNTNO,
                    ACC_TYPE = x.ACC_TYPE,
                    SCHM_CODE=x.SCHM_CODE,
                    BRANCHCODE = x.BRANCHCODE,
                    CURRENCY = x.CURRENCY,
                    Active = x.Active,
                    QR_ALLOW = x.QR_ALLOW
                });
                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = uservm;
                userandaccvm.accountvms = accvms;
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Edit
        public ActionResult UserEdit(int id)
        {
            try
            {
                User user = db._Users.Where(u => u.DEL_STATUS == false && u.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();
                if (user.USER_TYPE == RetailType)
                {
                    rri = GetRetailResponse(user.CIFID);
                }
                else if (user.USER_TYPE == CorporateType)
                {
                    cri = GetCorporateResponse(user.CIFID);
                }
                UserVM uservm = new UserVM
                {
                    ID = user.ID,
                    MOBILENO = user.MOBILENO,
                    USERNAME = user.USERNAME,
                    FULLNAME = (user.USER_TYPE==RetailType? rri.Name:cri.Name),
                    NRC = (user.USER_TYPE == RetailType ? rri.NRC : cri.AccountOpenDate),
                    EMAIL = user.EMAIL,
                    ADDRESS = user.ADDRESS,
                    CIFID = user.CIFID,
                    MINOR = (user.USER_TYPE == RetailType ? rri.Minor : user.MINOR),
                    GENDER = (user.USER_TYPE == RetailType ? rri.Gender : user.GENDER),
                    ALLOW_TRAN_COUNT = General.ConvertIntToStr(user.ALLOW_TRAN_COUNT),
                    DAILY_MAX_AMT = General.ConvertNDecimalToStr(user.DAILY_MAX_AMT),
                    ISLOCK_FLAG = user.ISLOCK_FLAG,
                    IsVIP = user.IsVIP,
                    USER_TYPE=user.USER_TYPE
                };
                //getting accounts by CIFID
                List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                {
                    ID = x.ID,
                    ACCOUNTNO = x.ACCOUNTNO,
                    ACC_TYPE = x.ACC_TYPE,
                    ACC_DESC = x.ACC_DESC,
                    SCHM_CODE=x.SCHM_CODE,
                    BRANCHCODE = x.BRANCHCODE,
                    CURRENCY = x.CURRENCY,
                    Active = x.Active,
                    QR_ALLOW = x.QR_ALLOW
                });
                List<AccountVM> closedaccvms = new List<AccountVM>();
                //getting core accounts
                if (user.USER_TYPE == RetailType)
                {
                    if (rri.lAcctInfo.Count > 0)
                    {
                        List<AccountInfo> core_accs = rri.lAcctInfo;
                        //remove closed acc
                        AccountVM[] tmp = new AccountVM[accvms.Count()];
                        accvms.CopyTo(tmp);
                        foreach (var dbacc in tmp)
                        {
                            if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                            {
                                accvms.Remove(dbacc);
                                closedaccvms.Add(dbacc);
                            }
                        }

                        //add new core acc
                        foreach (var acc in core_accs)
                        {
                            if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                            {
                                accvms.Add(new AccountVM
                                {
                                    ID = 0,
                                    ACCOUNTNO = acc.AccountNumber,
                                    ACC_TYPE = acc.AccountType,
                                    ACC_DESC = acc.AccountTypeDesc,
                                    SCHM_CODE = acc.Schm_Code,
                                    BRANCHCODE = acc.BranchID,
                                    CURRENCY = acc.Currency,
                                    Active = false,
                                    QR_ALLOW = false
                                });
                            }
                        }
                    }
                }
                else if (user.USER_TYPE == CorporateType) 
                {
                    if (cri.lstCorpAccountInfo.Count > 0)
                    {
                        List<AccountInfo> core_accs = cri.lstCorpAccountInfo.Select(ca => new AccountInfo
                        {
                            AccountNumber = ca.AccountNumber,
                            AccountType = ca.AccountType,
                            AccountTypeDesc = ca.AccountTypeDesc,
                            Schm_Code = ca.Schm_Code,
                            BranchID = ca.BranchID,
                            Currency = ca.Currency
                        }).ToList();
                        //remove closed acc
                        AccountVM[] tmp = new AccountVM[accvms.Count()];
                        accvms.CopyTo(tmp);
                        foreach (var dbacc in tmp)
                        {
                            if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                            {
                                accvms.Remove(dbacc);
                                closedaccvms.Add(dbacc);
                            }
                        }

                        //add new core acc
                        foreach (var acc in core_accs)
                        {
                            if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                            {
                                accvms.Add(new AccountVM
                                {
                                    ID = 0,
                                    ACCOUNTNO = acc.AccountNumber,
                                    ACC_TYPE = acc.AccountType,
                                    ACC_DESC = acc.AccountTypeDesc,
                                    SCHM_CODE = acc.Schm_Code,
                                    BRANCHCODE = acc.BranchID,
                                    CURRENCY = acc.Currency,
                                    Active = false,
                                    QR_ALLOW = false
                                });
                            }
                        }
                    }
                }
                //if (rri.lAcctInfo.Count > 0)
                //{
                //    List<AccountInfo> core_accs = rri.lAcctInfo;
                //    //remove closed acc
                //    AccountVM[] tmp = new AccountVM[accvms.Count()];
                //    accvms.CopyTo(tmp);
                //    foreach (var dbacc in tmp)
                //    {
                //        if (core_accs.Where(a => a.AccountNumber == dbacc.ACCOUNTNO).Count() == 0)
                //        {
                //            accvms.Remove(dbacc);
                //            closedaccvms.Add(dbacc);
                //        }
                //    }

                //    //add new core acc
                //    foreach (var acc in core_accs)
                //    {
                //        if (accvms.Where(a => a.ACCOUNTNO == acc.AccountNumber).Count() == 0)
                //        {
                //            accvms.Add(new AccountVM
                //            {
                //                ID = 0,
                //                ACCOUNTNO = acc.AccountNumber,
                //                ACC_TYPE = acc.AccountType,
                //                ACC_DESC = acc.AccountTypeDesc,
                //                SCHM_CODE=acc.Schm_Code,
                //                BRANCHCODE = acc.BranchID,
                //                CURRENCY = acc.Currency,
                //                Active = false,
                //                QR_ALLOW = false
                //            });
                //        }
                //    }
                //}

                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = uservm;
                userandaccvm.accountvms = accvms;
                userandaccvm.closedaccs = closedaccvms;

                return View(userandaccvm);
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
        public ActionResult UserEdit(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                  
                    UserVM uservm = userandaccvm.uservm;
                    List<AccountVM> accvms = userandaccvm.accountvms;
                    userandaccvm.closedaccs = new List<AccountVM>();
                    ReqUser requser = new ReqUser();
                    var req = db.ReqUsers.Where(r => r.CIFID == uservm.CIFID && r.STATUS == 0 && r.ISNEW == false).Count();
                    //reget core data by CIFID
                    RetailResponseInfo rri = new RetailResponseInfo();
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    if (uservm.CIFID != "")
                    {
                        if (uservm.USER_TYPE == RetailType)
                        {
                            rri = GetRetailResponse(uservm.CIFID);
                        }
                        else if (uservm.USER_TYPE == CorporateType)
                        {
                            cri = GetCorporateResponse(uservm.CIFID);
                        }
                    }
                    //   rri = GetRetailResponse(uservm.CIFID);
                    #region VIP
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    //convert tran_amt and max_amt from string to int
                    try
                    {
                        allow_tran_count = General.ConvertStrToInt(uservm.ALLOW_TRAN_COUNT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "The field Allow Tran limit must be between 0 and 2,147,483,647.");
                        return View(userandaccvm);
                    }
                    try
                    {
                        daily_max_amount = General.ConvertStrToDecimal(uservm.DAILY_MAX_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9,999,999,999,999,999.99");
                        return View(userandaccvm);
                    }
                    if (uservm.IsVIP)
                    {
                        //allow tran count
                        if (uservm.ALLOW_TRAN_COUNT == null || (allow_tran_count <= GetRule("R003")))
                        {
                            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count must be greater than Rule's count.");
                            return View(userandaccvm);
                        }
                        //allow tran count
                        if (uservm.DAILY_MAX_AMT == null || (daily_max_amount <= GetDecimalRule("R005")))
                        {
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's Amount.");
                            return View(userandaccvm);
                        }
                    }
                    #endregion
                    List<AccountVM> allowedaccvms = accvms.Where(a => a.Active == true).ToList();
                    if (allowedaccvms.Count == 0)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }
                    if (req > 0)
                    {
                        ModelState.AddModelError("", "Already Requested!");
                        return View(userandaccvm);
                    }
                    string LoginUser = User.Identity.GetUserId();
                    //core data
                    if (uservm.USER_TYPE == RetailType)
                    {
                        requser.NRC = rri.NRC;
                        requser.FULLNAME = rri.Name;
                        requser.MINOR = rri.Minor;
                        requser.GENDER = rri.Gender;
                    }
                    else if(uservm.USER_TYPE == CorporateType)//"Corporate" == CorporateType)
                    {
                        requser.NRC = cri.AccountOpenDate;
                        requser.FULLNAME = cri.Name;
                        requser.MINOR = uservm.MINOR;
                        requser.GENDER = uservm.GENDER;
                    }

                    //requser.NRC = rri.NRC;
                    //requser.FULLNAME = rri.Name;
                    //requser.MINOR = rri.Minor;
                    //requser.GENDER = rri.Gender;
                    //formdata
                    requser.CIFID = uservm.CIFID;
                    requser.USERNAME = uservm.USERNAME;
                    requser.MOBILENO = uservm.MOBILENO;
                    requser.EMAIL = uservm.EMAIL;
                    requser.ADDRESS = uservm.ADDRESS;
                    requser.ALLOW_TRAN_COUNT = allow_tran_count;
                    requser.DAILY_MAX_AMT = daily_max_amount;
                    //Check VIP
                    requser.IsVIP = uservm.IsVIP;//Update VIP flag
                    if (uservm.IsVIP)
                    {
                        //get form inputs
                        requser.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    else
                    {
                        //get rule values for Normal User
                        requser.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }
                    //request Info
                    requser.REQUESTINFO = "";

                    //Default fields
                    requser.USER_TYPE = uservm.USER_TYPE;
                    requser.ISNEW = false;   //true for register
                    requser.STATUS = 0;     //status => 0   request status, 1  approve, 2 reject
                    requser.MAKER = LoginUser;
                    requser.REQUESTEDDATE = DateTime.Now;
                    //get BranchID from Logged In User
                    AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                    requser.BranchID = admin.BranchID;
                    
                    db.ReqUsers.Add(requser);
                    db.SaveChanges();
                    //save log
                    log.Info(LoginUser, controllerName, actionName, 1);

                    //saving accounts
                    if (allowedaccvms.Count() > 0)//allowedaccvms is only selected accounts
                    {
                        List<AccountInfo> core_accs = rri.lAcctInfo;
                        UpdateAccounts(accvms, requser.CIFID, core_accs, false, requser.ID);
                    }
                    //success
                    return RedirectToAction("index");
                }
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //try
            //{
            //    userandaccvm.closedaccs = new List<AccountVM>();
            //    if (ModelState.IsValid)
            //    {
            //        UserVM uservm = userandaccvm.uservm;
            //        List<AccountVM> accvms = userandaccvm.accountvms;
            //        User user = db._Users.Find(uservm.ID);
            //        if (user == null)
            //        {
            //            return HttpNotFound();
            //        }
            //        //checking Validation
            //        #region Validation
            //        //txn count
            //        if (uservm.ALLOW_TRAN_COUNT == null)
            //        {
            //            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count is required.");
            //            return View(userandaccvm);
            //        }
            //        //max amt
            //        if (uservm.DAILY_MAX_AMT == null)
            //        {
            //            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount is required.");
            //            return View(userandaccvm);
            //        }
            //        #endregion

            //        //user.USERNAME = uservm.USERNAME; //not editable username
            //        user.MOBILENO = uservm.MOBILENO;
            //        user.EMAIL = uservm.EMAIL;
            //        user.ADDRESS = uservm.ADDRESS;
            //        user.ALLOW_TRAN_COUNT = uservm.ALLOW_TRAN_COUNT;
            //        user.DAILY_MAX_AMT = uservm.DAILY_MAX_AMT;
            //        //default fields
            //        //requser.REQUESTEDDATE = DateTime.Now;
            //        //requser = User.Identity.GetUserId();
            //        db.Entry(user).State = EntityState.Modified;
            //        db.SaveChanges();
            //        // save log
            //        string getuserid = User.Identity.GetUserId();
            //        log.Info(getuserid, controllerName, actionName, 2, $"{uservm.USERNAME} is updated.");

            //        //updating accounts
            //        RetailResponseInfo rri = new RetailResponseInfo();
            //        rri = GetRetailResponse(uservm.CIFID);
            //        if (rri.NRC == null) //if wrong CIFID
            //        {
            //            ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
            //            return View(userandaccvm);
            //        }
            //        List<AccountInfo> core_accs = rri.lAcctInfo;

            //        UpdateAccounts(accvms, user.CIFID, core_accs);

            //        return RedirectToAction("Index");
            //    }
            //    return View(userandaccvm);
            //}
            //catch (Exception ex)
            //{
            //    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            //    return View("Error");
            //}
        }

        // GET: Request/UserDelete/5
        public ActionResult UserDelete(int id)
        {
            try
            {
                User user = db._Users.Where(u => u.DEL_STATUS == false && u.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                UserVM uservm = new UserVM
                {
                    ID = user.ID,
                    MOBILENO = user.MOBILENO,
                    USERNAME = user.USERNAME,
                    FULLNAME = user.FULLNAME,
                    NRC = user.NRC,
                    EMAIL = user.EMAIL,
                    ADDRESS = user.ADDRESS,
                    CIFID = user.CIFID,
                    MINOR = user.MINOR,
                    GENDER = user.GENDER,
                    ALLOW_TRAN_COUNT = General.ConvertIntToStr(user.ALLOW_TRAN_COUNT),
                    DAILY_MAX_AMT = General.ConvertNDecimalToStr(user.DAILY_MAX_AMT),
                    IsVIP = user.IsVIP,
                    USER_TYPE=user.USER_TYPE,
                };
                //getting accounts by CIFID
                List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                {
                    ID = x.ID,
                    ACCOUNTNO = x.ACCOUNTNO,
                    ACC_TYPE = x.ACC_TYPE,
                    SCHM_CODE=x.SCHM_CODE,
                    BRANCHCODE = x.BRANCHCODE,
                    CURRENCY = x.CURRENCY,
                    Active = x.Active,
                    QR_ALLOW = x.QR_ALLOW
                });
                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = uservm;
                userandaccvm.accountvms = accvms;

                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Request/UserDelete/5
        [HttpPost, ActionName("UserDelete")]
        public ActionResult UserDeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 7 && rc.STATUS == 0).Count();
                if (existingcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 7);//type 7 => Delete
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting delete successfully.", $"User id is {id}");
                    return Json("Requesting delete successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        public ActionResult Changes(FormCollection form)
        {
            //request list
            string userid = User.Identity.GetUserId();
            List<ReqChange> lstChanges = db.ReqChanges.Where(rc => rc.MAKER == userid && rc.STATUS!=1).ToList();
            int count = lstChanges.Count();
            string[] Username = new string[count];
            int i = 0;
            foreach (var item in lstChanges)
            {
                var name = db._Users.Where(u => u.ID == item.USERID).FirstOrDefault();
                Username[i++]= name.USERNAME;
            }
            //bind User Type List
            //UserTypeDDL();
            //create User Type List
            ViewBag.UserType = new List<SelectListItem>()
            {
                new SelectListItem{Text="Retail User",Value="Retail"},
                new SelectListItem{Text="Corporate User",Value="Corporate"}
            };
            //Create Status List
            ViewBag.rcTYPEList = new List<SelectListItem>
                {
                    new SelectListItem{Text="Reset Password",Value="2"},
                    new SelectListItem{Text="Lock",Value="3"},
                    new SelectListItem{Text="Unlock",Value="4"},
                    new SelectListItem{Text="Tran Lock",Value="5"},
                    new SelectListItem{Text="Tran Unlock",Value="6"},
                    new SelectListItem{Text="Delete",Value="7"}
                };

            //Create Status List
            ViewBag.rcSTATUSList = new List<SelectListItem>
                {
                    new SelectListItem{Text="All",Value=null},
                    new SelectListItem{Text="Requested",Value="0"},
                    new SelectListItem{Text="Approved",Value="1"},
                    new SelectListItem{Text="Rejected",Value="2"}
                    
                };
           // ViewBag.rcUSERNAME = form["rcUSERNAME"];
           // ViewBag.rcTYPE = form["rcTYPE"];
           // ViewBag.rcMAKER = form["rcMAKER"];
           // ViewBag.rcSTATUS = form["rcSTATUS"];
           // ViewBag.ReqDate = form["Datefilter"];

            //assign initial value for "RequestedDate" and Status
            if (/*form["Datefilter"] == null &&*/  form["aStatus"] == null)
            {
                form["aStatus"] = "0";
                //form["Datefilter"] = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.aStatus = form["aStatus"];
                //ViewBag.ReqDate = form["Datefilter"];

            }

            return View();
        }
        [HttpPost]
        public ActionResult Changes(int? arg1)
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
            var rcUSERNAME = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var rcTYPE = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var rcMAKER = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var rcSTATUS = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            DateTime? fromDate = null, toDate = null;
            //if (Datefilter != "")
            //{
            //    string[] strdate = Datefilter.Split('~');
            //    if (!string.IsNullOrEmpty(strdate[0].Trim()))
            //    {
            //        fromDate = General.ConvertStrToDate(strdate[0]);
            //    }
            //    if (!string.IsNullOrEmpty(strdate[1].Trim()))
            //    {
            //        toDate = General.ConvertStrToDate(strdate[1]).AddDays(1);
            //    }
            //}
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
            }
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            string userid = User.Identity.GetUserId();
            var reqChanges = (from s in db.ReqChanges.Where(rc => rc.User.USER_TYPE=="Retail" & rc.MAKER == userid && rc.STATUS != 1)
                              select s);
            //SEARCHING...

            if (!string.IsNullOrEmpty(rcUSERNAME))
            {
                reqChanges = reqChanges.Where(a => a.User.USERNAME.ToLower().Contains(rcUSERNAME.ToLower().Trim()));

            }
            if (!string.IsNullOrEmpty(rcTYPE))
            {
                reqChanges = reqChanges.Where(a => a.TYPE.ToString() == rcTYPE);
            }
            if (!string.IsNullOrEmpty(rcMAKER))
            {
                reqChanges = reqChanges.Where(a => a.MAKER.Contains(rcMAKER.ToLower().Trim()));

            }
            if (fromDate != null & toDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate);
            }
            else if (fromDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE >= fromDate);
            }
            else if (toDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE < toDate);
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                reqChanges = reqChanges.Where(a => a.User.USER_TYPE==usertype);

            }

            //if (!string.IsNullOrEmpty(Datefilter))
            //{
            //    DateTime rcdateTime = DateTime.Parse(Datefilter); 
            //    reqChanges = reqChanges.Where(a => a.REQUESTEDDATE.CompareTo(rcdateTime.Date)==0);
            //    //reqChanges = reqChanges.ToList().AsQueryable();
            //}
            if (!string.IsNullOrEmpty(rcSTATUS))
            {
                reqChanges = reqChanges.Where(a => a.STATUS.ToString() == rcSTATUS);
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                reqChanges = reqChanges.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = reqChanges.Count();
            var data = reqChanges.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        //Reset Password  request
        [HttpPost]
        public JsonResult Reset(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 2 && rc.STATUS == 0).Count();
                if(existingcount>0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 2);//type 2 => reset password
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting reset password successfully.", $"User id is {id}");
                    return Json("Requesting reset password successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
        }

        //Lock request
        [HttpPost]
        public ActionResult Lock(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 3 && rc.STATUS == 0).Count();
                if (existingcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 3);//type 3 => Lock
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting lock successfully.", $"User id is {id}");
                    return Json("Requesting lock successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //UnLock request
        [HttpPost]
        public ActionResult Unlock(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 4 && rc.STATUS == 0).Count();
                if (existingcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 4);//type 3 => Unlock
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting Unlock successfully.", $"User id is {id}");
                    return Json("Requesting Unlock successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Tran Unlock request
        [HttpPost]
        public ActionResult TranLock(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 5 && rc.STATUS == 0).Count();
                if (existingcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 5);//type 5 => Tran Lock
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting TranLock successfully.", $"User id is {id}");
                    return Json("Requesting TranLock successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

              //Tran Unlock request
        [HttpPost]
        public ActionResult TranUnlock(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get user by id
                User user = db._Users.Find(id);
                if (user == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                //check already requested
                int existingcount = db.ReqChanges.Where(rc => rc.USERID == id && rc.TYPE == 6 && rc.STATUS == 0).Count();
                if (existingcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(id, 6);//type 5 => Tran Lock
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting TranUnlock successfully.", $"User id is {id}");
                    return Json("Requesting TranUnlock successfully.", JsonRequestBehavior.AllowGet);
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        public ActionResult ChangesDetails(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ReqChange reqchanges = db.ReqChanges.Find(id);

                //get user name
                var user = db._Users.Where(u => u.ID == reqchanges.USERID).FirstOrDefault();
                ViewBag.Name = user.USERNAME;
                //get branch name
                var branch = db.Branches.Where(b => b.id == reqchanges.BranchID).FirstOrDefault();
                ViewBag.BranchName = branch.name;
                //get Type name
                ViewBag.Type = reqchanges.TYPE;
                if (ViewBag.Type == 2) { ViewBag.Type = "Reset Password"; }
                else if (ViewBag.Type == 3) { ViewBag.Type = "Lock"; }
                else if (ViewBag.Type == 4) { ViewBag.Type = "Unlock"; }
                else if (ViewBag.Type == 5) { ViewBag.Type = "Tran Lock"; }
                else if (ViewBag.Type == 6) { ViewBag.Type = "Tran Unlock"; }
                else if (ViewBag.Type == 7) { ViewBag.Type = "Delete"; }
                else { ViewBag.Type = "No"; }
                //get Status name
                ViewBag.Status = reqchanges.STATUS;
                //check status value
                if(ViewBag.Status==0) { ViewBag.StatusName = "Requested"; }
                else if (ViewBag.Status == 1) { ViewBag.StatusName = "Approved"; }
                else if (ViewBag.Status == 2) { ViewBag.StatusName = "Rejected"; }
                else { ViewBag.StatusName = "No"; }

                if (reqchanges == null)
                {
                    return HttpNotFound();
                }
                return View(reqchanges);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Get/ChangesRequest/Delete
        public ActionResult ChangesDelete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ReqChange reqchanges = db.ReqChanges.Find(id);
                //get user name
                var user = db._Users.Where(u => u.ID == reqchanges.USERID).FirstOrDefault();
                ViewBag.Name = user.USERNAME;
                //get branch name
                var branch = db.Branches.Where(b => b.id == reqchanges.BranchID).FirstOrDefault();
                ViewBag.BranchName = branch.name;
                //get Type name
                ViewBag.Type = reqchanges.TYPE;
                if (ViewBag.Type == 2) { ViewBag.Type = "Reset Password"; }
                else if (ViewBag.Type == 3) { ViewBag.Type = "Lock"; }
                else if (ViewBag.Type == 4) { ViewBag.Type = "Unlock"; }
                else if (ViewBag.Type == 5) { ViewBag.Type = "Tran Lock"; }
                else if (ViewBag.Type == 6) { ViewBag.Type = "Tran Unlock"; }
                else if (ViewBag.Type == 7) { ViewBag.Type = "Delete"; }
                else { ViewBag.Type = "No"; }
                //get Status name
                ViewBag.Status = reqchanges.STATUS;
                //check status value
                if (ViewBag.Status == 0) { ViewBag.StatusName = "Requested"; }
                else if (ViewBag.Status == 1) { ViewBag.StatusName = "Approved"; }
                else if (ViewBag.Status == 2) { ViewBag.StatusName = "Rejected"; }
                else { ViewBag.StatusName = "No"; }
                if (reqchanges == null)
                {
                    return HttpNotFound();
                }
                return View(reqchanges);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post/ChangesRequest/Delete
        [HttpPost,ActionName("ChangesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangesDeleteConfirm(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ReqChange reqchange = db.ReqChanges.Find(id);
                string username = reqchange.User.USERNAME;
                db.ReqChanges.Remove(reqchange);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3, $"User ID is {username}");
                return RedirectToAction("Changes");
            }catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Add Req Change by Type (2=> reset, 3=> lock, 4=> unlock, etc.)
        private Boolean AddReqChange(int userid, byte type)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string LoginUser = User.Identity.GetUserId();
            //Add req to ReqChanges table
            try
            {
                ReqChange reqChg = new ReqChange();
                reqChg.USERID = userid;
                reqChg.TYPE = type;
                //reqChg.REQUESTINFO = "";
                //Default fields
                reqChg.STATUS = 0;      //status = 0 for Request
                reqChg.MAKER = User.Identity.GetUserId();
                reqChg.REQUESTEDDATE = DateTime.Now;
                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);
                reqChg.BranchID = admin.BranchID;

                db.ReqChanges.Add(reqChg);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(LoginUser, controllerName, actionName, ex);
            }
            return false;
        }

        private Nullable<int> GetRule(string RuleCode)
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == RuleCode).FirstOrDefault();
                int IntRule = Convert.ToInt32(rule.Value);
                if (rule == null)
                    return null;
                else
                    return IntRule;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        private Nullable<decimal> GetDecimalRule(string RuleCode)
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == RuleCode).FirstOrDefault();
                if (rule == null)
                    return null;
                else
                    return rule.Value;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }
        #region Corporate
        //To fetch Corporate data from core by CIFID
        public JsonResult GetCorporate(string CIFID)
        {
            CorporateResponseInfo cri = new CorporateResponseInfo();
            try
            {
                if (CIFID != "")
                {
                    int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    if (count > 0)
                        return Json(new { message = "Duplicate CIFID" }, JsonRequestBehavior.AllowGet);
                    if (db.ReqUsers.Where(u => u.CIFID == CIFID & u.STATUS == 0).Count() > 0)
                    {
                        return Json(new { message = "Already Requested!" }, JsonRequestBehavior.AllowGet);
                    }
                    cri = GetCorporateResponse(CIFID);
                    if (cri.Name != null && cri.lstCorpAccountInfo.Count == 0)
                        return Json(new { message = "No account under this CIFID" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(cri, JsonRequestBehavior.AllowGet);
        }
        private CorporateResponseInfo GetCorporateResponse(string CIFID)
        {
            CorporateResponseInfo cri = new CorporateResponseInfo();
            try
            {
                CustomerInformationInquiry custInfoInquiry = new CustomerInformationInquiry();

                cri = custInfoInquiry.SelectCorporateInformation(CIFID, connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cri;
        }
        public ActionResult Corporate()
        {
            try
            {
                UserAndAccVM userandaccvm = new UserAndAccVM();
                userandaccvm.uservm = new UserVM();
                return View(userandaccvm);
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
        public ActionResult Corporate(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    UserVM uservm = userandaccvm.uservm;
                    //reget core data by CIFID
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    if (uservm.CIFID != "")
                        cri = GetCorporateResponse(uservm.CIFID);
                    if (cri.Name == null) //if wrong CIFID
                    {
                        ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                        return View(userandaccvm);
                    }
                    //checking Validation
                    #region Validation
                    //CIFID duplicate
                    if (db._Users.Where(u => u.CIFID == uservm.CIFID & u.DEL_STATUS == false).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.CIFID", "Duplicate CIFID");
                        return View(userandaccvm);
                    }
                    //duplicate requested CIFID (ReqUsers)
                    if (db.ReqUsers.Where(u => u.CIFID == uservm.CIFID & u.ISNEW & u.STATUS == 0).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.CIFID", "This CIFID is already requested.");
                        return View(userandaccvm);
                    }

                    #region username
                    //space and special char
                    Regex rgx = new Regex("^[a-zA-Z0-9]+?$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Username isn't allowed space and special character.");
                        return View(userandaccvm);
                    }
                    //length
                    rgx = new Regex("^([a-zA-Z0-9]){6,16}$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Minimum length is 6 and maximum length is 16.");
                        return View(userandaccvm);
                    }
                    //staring with number
                    rgx = new Regex("^[a-zA-Z]+([a-zA-Z0-9]){5,15}$");
                    if (!rgx.IsMatch(uservm.USERNAME))
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Invalid USERNAME, Username isn't allowed to start with number.");
                        return View(userandaccvm);
                    }
                    //username duplicate
                    if (db._Users.Where(u => u.USERNAME.ToLower() == uservm.USERNAME.ToLower()).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Duplicate USERNAME");
                        return View(userandaccvm);
                    }
                    //duplicate requested username (ReqUsers)
                    if (db.ReqUsers.Where(u => u.USERNAME.ToLower() == uservm.USERNAME.ToLower() & u.STATUS == 0).Count() > 0)
                    {
                        ModelState.AddModelError("uservm.USERNAME", "Duplicate Requested USERNAME");
                        return View(userandaccvm);
                    }
                    #endregion
                    #region VIP
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    //convert tran_amt and max_amt from string to int
                    try
                    {
                        allow_tran_count = General.ConvertStrToInt(uservm.ALLOW_TRAN_COUNT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "The field Allow Tran limit must be between 0 and 9999999999999999");
                        return View(userandaccvm);
                    }
                    try
                    {
                        daily_max_amount = General.ConvertStrToInt(uservm.DAILY_MAX_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9999999999999999");
                        return View(userandaccvm);
                    }
                    if (uservm.IsVIP)
                    {
                        //allow tran count
                        if (uservm.ALLOW_TRAN_COUNT == null || (allow_tran_count <= GetRule("R003")))
                        {
                            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count must be greater than Rule's count.");
                            return View(userandaccvm);
                        }
                        //allow tran count
                        if (uservm.DAILY_MAX_AMT == null || (daily_max_amount <= GetDecimalRule("R005")))
                        {
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's Amount.");
                            return View(userandaccvm);
                        }
                    }
                    #endregion
                    //checking to choose at least one account
                    if (userandaccvm.accountvms == null)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }
                    List<AccountVM> accvms = userandaccvm.accountvms;
                    List<AccountVM> allowedaccvms = accvms.Where(a => a.Active == true).ToList();
                    if (allowedaccvms.Count == 0)
                    {
                        ModelState.AddModelError("", "Please choose at least one account!");
                        return View(userandaccvm);
                    }

                    //check also at Request table


                    #endregion

                    string LoginUser = User.Identity.GetUserId();

                    //save into requested user table (temp)
                    ReqUser requser = new ReqUser();
                    //core data
                    requser.NRC = cri.AccountOpenDate;
                    requser.FULLNAME = cri.Name;
                    //formdata
                    requser.CIFID = uservm.CIFID;
                    requser.USERNAME = uservm.USERNAME;
                    requser.MOBILENO = uservm.MOBILENO;
                    requser.EMAIL = uservm.EMAIL;
                    requser.ADDRESS = uservm.ADDRESS;
                    //Check VIP
                    requser.IsVIP = uservm.IsVIP;
                    requser.USER_TYPE = CorporateType;
                    if (uservm.IsVIP)//save for only VIP users
                    {
                        //get form inputs
                        requser.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        requser.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    //request Info
                    requser.REQUESTINFO = "";

                    //Default fields
                    requser.ISNEW = true;   //true for register
                    requser.STATUS = 0;     //status => 0   request status, 1  approve, 2 reject
                    requser.MAKER = LoginUser;
                    requser.REQUESTEDDATE = DateTime.Now;
                    requser.ALLOW_TRAN_COUNT = allow_tran_count;
                    requser.DAILY_MAX_AMT = daily_max_amount;
                    //get BranchID from Logged In User
                    AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                    requser.BranchID = admin.BranchID;

                    db.ReqUsers.Add(requser);
                    db.SaveChanges();
                    //save log
                    log.Info(LoginUser, controllerName, actionName, 1, $"User ID is {requser.USERNAME}.");

                    //saving accounts
                    if (allowedaccvms.Count() > 0)//allowedaccvms is only selected accounts
                    {
                        List<AccountInfo> core_accs = cri.lstCorpAccountInfo.Select(ca => new AccountInfo
                        {
                            AccountNumber = ca.AccountNumber,
                            AccountType = ca.AccountType,
                            AccountTypeDesc = ca.AccountTypeDesc,
                            Schm_Code = ca.Schm_Code,
                            BranchID = ca.BranchID,
                            Currency = ca.Currency
                        }).ToList();
                        SaveAccounts(allowedaccvms, requser.CIFID, core_accs, requser.ID);
                    }
                    //success
                    return RedirectToAction("index");
                }
                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region bind user type ddl
        private void UserTypeDDL()
        {
            //create User Type List
            ViewBag.UserType = new List<SelectListItem>()
            {
                new SelectListItem{Text="Retail User",Value="Retail"},
                new SelectListItem{Text="Corporate User",Value="Corporate"}
            };
        }
        #endregion
    }
}