using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using Newtonsoft.Json;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using ABankAdmin.Core.Utils;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Diagnostics;
using PagedList;
using Microsoft.AspNet.Identity;
using System.IO;
//for pdfsharp
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Configuration;
using PdfSharp.Drawing.Layout;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.Globalization;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class UserController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        string controllerName = "UserController";
        EventLogController log = new EventLogController();
        int resetstatus;
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        // GET: User
        public ActionResult Index()
        {
            UserTypeDDL();
          //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
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

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                user = user.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(username.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                user = user.Where(a => a.USERNAME.ToLower().Contains(username.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(phoneNo.Trim()))
            {
                //int roleid = Convert.ToInt32(role);
                user = user.Where(a => a.MOBILENO.Contains(phoneNo.Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //user = user.Where(a => a.USER_TYPE.Contains(usertype)).ToList();
                user = user.Where(a => a.USER_TYPE==usertype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
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
                    CREATEDDATE = user.CREATEDDATE,
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
        // GET: User/Retail
        public ActionResult Retail()
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
        // POST: User/Retail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult retail(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string LoginPWD = "";
                string TranPwD = "";
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

                    #region username
                    ////username format
                    //Regex rgx = new Regex("^[a-zA-Z]+([a-zA-Z0-9]){5,15}$");
                    //if (!rgx.IsMatch(uservm.USERNAME))
                    //{
                    //    ModelState.AddModelError("uservm.USERNAME", "Invalid USERNAME, Username isn't allowed space and special character. Minimum length is 6 and maximum length is 16.");
                    //    return View(userandaccvm);
                    //}
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
                    #endregion
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    #region VIP
                    if (uservm.IsVIP)
                    {
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
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9999999999999999.99");
                            return View(userandaccvm);
                        }
                   
                        //allow tran count
                        if (uservm.ALLOW_TRAN_COUNT == null || (allow_tran_count <= GetRule("R003")))
                        {
                            ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "Allowed Transaction Count must be greater than Rule's count.");
                            return View(userandaccvm);
                        }
                        //allow tran count
                        if (uservm.DAILY_MAX_AMT == null || (daily_max_amount <= GetDecimalRule("R005")))
                        {
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's count.");
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
                    #endregion

                    //generate pwd
                    LoginPWD = GeneratePassword.Generate(8, true, true, false);
                    System.Threading.Thread.Sleep(100);
                    TranPwD = GeneratePassword.Generate(8, true, true, false);

                    //create new user
                    User new_user = new User();
                    //core data
                    new_user.NRC = rri.NRC;
                    new_user.FULLNAME = rri.Name;
                    new_user.MINOR = rri.Minor;
                    new_user.GENDER = rri.Gender;
                    //formdata
                    new_user.CIFID = uservm.CIFID;
                    new_user.USERNAME = uservm.USERNAME;
                    new_user.MOBILENO = uservm.MOBILENO;
                    new_user.EMAIL = uservm.EMAIL;
                    new_user.ADDRESS = uservm.ADDRESS;
                    //Check VIP
                    new_user.IsVIP = uservm.IsVIP;
                    if (uservm.IsVIP)
                    {
                        //get form inputs
                        new_user.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        new_user.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    else
                    {
                        //get rule values for Normal User
                        new_user.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        new_user.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }

                    //guid for userid
                    new_user.USERID = Guid.NewGuid().ToString();
                    //generate pwd
                    new_user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(LoginPWD, new_user.USERNAME);
                    new_user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(TranPwD, new_user.USERNAME);

                    //default values
                    new_user.CREATEDDATE = DateTime.Now;
                    new_user.CREATEDUSERID = User.Identity.GetUserId();
                    new_user.USER_TYPE = "Retail";
                    new_user.FIRSTTIME_FLAG = true;
                    //new_user.FORCE_PWD_CHANGE_FLG = true;//no need
                    //new_user.FORCE_TXN_PWD_CHANGE_FLG = true;
                    //pwd exp date by rule
                    int? expday = Convert.ToInt32(GetRule("R007"));
                    int expDays = expday ?? 90; //default 90
                    new_user.PWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                    new_user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(expDays);

                    db._Users.Add(new_user);
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, $"User ID is {new_user.USERNAME}.");

                    //saving accounts
                    if (allowedaccvms.Count() > 0)//allowedaccvms is only selected accounts
                    {
                        List<AccountInfo> core_accs = rri.lAcctInfo;
                        SaveAccounts(allowedaccvms, new_user.CIFID, core_accs);
                    }
                    //generate pdf
                    //string allow_tran_count = Convert.ToString(new_user.ALLOW_TRAN_COUNT);
                    var tran_amt = db.Rules.Where(r => r.Code == "R014").FirstOrDefault();
                    string tran_max_amt = General.ConvertNDecimalToStr(tran_amt.Value);
                    string daily_max_count = General.ConvertNDecimalToStr(new_user.DAILY_MAX_AMT);
                    string filename = GeneratePdf(new_user.USERNAME, new_user.FULLNAME, new_user.CIFID, new_user.ADDRESS, new_user.MOBILENO, new_user.EMAIL, tran_max_amt, daily_max_count, LoginPWD, TranPwD, resetstatus = 0);
                    //Sent Email                   
                    if (!String.IsNullOrEmpty(new_user.EMAIL) && IsEmailValid(new_user.EMAIL) && IsMailSent())
                    {
                        //Validate Email Allow
                        var tranretail = "Retail User Management";
                        var mailallow = EmailAllow(tranretail);
                        if (mailallow.Receiver_Email_Allow == true)
                        {
                            try
                            {
                                //Get Data For Email
                                General.SMTPSetting setting = GetEmailSetting();
                                var template = from t in db.EmailTemplates
                                               where t.Type == "Registration"
                                               select t;
                                var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                Body = Body.Replace("[USERNAME]", (new_user.USERNAME.ToString()));
                                Body = Body.Replace("[FULLNAME]", (new_user.FULLNAME.ToString()));

                                //mapAddress.Replace(" ", "_");
                                //string Subject = "A bank Mobile Banking Information";
                                //string Body = "Your Account has been successfully created. Please check the credential at Attached pdf.";
                                bool sentmail = General.SendEmail(new_user.EMAIL, Subject, Body, filename, 0, 0, setting);
                                //log for email
                                log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {new_user.USERNAME}.Email Address is {new_user.EMAIL}.");
                            }
                            catch (Exception ex)
                            {
                                log.Error(getuserid, controllerName, actionName, ex);
                            }
                        }
                    }
                    //carry filename to Index
                    TempData["filename"] = filename;
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
               
        //save accounts
        private void SaveAccounts(List<AccountVM> accvms, string CIFID, List<AccountInfo> core_accs)
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
                    Account acc = new Account();
                    //core data
                    acc.ACCOUNTNO = core_acc.AccountNumber;
                    acc.ACC_TYPE = core_acc.AccountType;
                    acc.SCHM_CODE = core_acc.Schm_Code;
                    acc.ACC_DESC = core_acc.AccountTypeDesc;
                    acc.BRANCHCODE = core_acc.BranchID;
                    acc.CURRENCY = core_acc.Currency;
                    //form data
                    acc.Active = accvm.Active;
                    //QR allow false if Acc type is  ODA, LAA, TDA
                    if (acc.ACC_TYPE == "ODA" || acc.ACC_TYPE == "LAA" || acc.ACC_TYPE == "TDA" /*|| acc.CURRENCY != "MMK"*/)
                    {
                        acc.QR_ALLOW = false;
                    }
                    else
                        acc.QR_ALLOW = accvm.QR_ALLOW;
                    //default values
                    acc.CIFID = CIFID;
                    acc.CreatedDateTime = DateTime.Now;
                    acc.CreatedUserID = User.Identity.GetUserId();
                    acc.ACCT_CLOSE_FLAG = false;

                    db.Accounts.Add(acc);
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

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                ViewBag.AllowTranCount = GetRule("R003");
                ViewBag.DailyMaxAmount = GetDecimalRule("R005");
                User user = db._Users.Where(u => u.DEL_STATUS == false && u.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                //old correct code for retail

                //RetailResponseInfo rri = GetRetailResponse(user.CIFID);
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
                //    ISLOCK_FLAG = user.ISLOCK_FLAG,
                //    IsVIP = user.IsVIP,
                //};
                ////getting accounts by CIFID
                //List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                //{
                //    ID = x.ID,
                //    ACCOUNTNO = x.ACCOUNTNO,
                //    ACC_TYPE = x.ACC_TYPE,
                //    SCHM_CODE=x.SCHM_CODE,
                //    ACC_DESC = x.ACC_DESC,
                //    BRANCHCODE = x.BRANCHCODE,
                //    CURRENCY = x.CURRENCY,
                //    Active = x.Active,
                //    QR_ALLOW = x.QR_ALLOW
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
                //                SCHM_CODE=acc.Schm_Code,
                //                ACC_DESC = acc.AccountTypeDesc,
                //                BRANCHCODE = acc.BranchID,
                //                CURRENCY = acc.Currency,
                //                Active = false,
                //                QR_ALLOW = false
                //            });
                //        }
                //    }
                //}

                //new for both retail and corporate
                UserAndAccVM userandaccvm = new UserAndAccVM();
                if (user.USER_TYPE == "Retail")
                {
                    RetailResponseInfo rri = GetRetailResponse(user.CIFID);
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
                        ISLOCK_FLAG = user.ISLOCK_FLAG,
                        IsVIP = user.IsVIP,
                        USER_TYPE=user.USER_TYPE,
                    };
                    //getting accounts by CIFID
                    List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                    {
                        ID = x.ID,
                        ACCOUNTNO = x.ACCOUNTNO,
                        ACC_TYPE = x.ACC_TYPE,
                        SCHM_CODE = x.SCHM_CODE,
                        ACC_DESC = x.ACC_DESC,
                        BRANCHCODE = x.BRANCHCODE,
                        CURRENCY = x.CURRENCY,
                        Active = x.Active,
                        QR_ALLOW = x.QR_ALLOW
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
                                    SCHM_CODE = acc.Schm_Code,
                                    ACC_DESC = acc.AccountTypeDesc,
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
                else if (user.USER_TYPE == "Corporate")
                {
                    CorporateResponseInfo cri = GetCorporateResponse(user.CIFID);
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
                        ISLOCK_FLAG = user.ISLOCK_FLAG,
                        IsVIP = user.IsVIP,
                        USER_TYPE=user.USER_TYPE,
                    };
                    //getting accounts by CIFID
                    List<AccountVM> accvms = db.Accounts.Where(a => a.ACCT_CLOSE_FLAG == false && a.CIFID == user.CIFID).ToList().ConvertAll(x => new AccountVM
                    {
                        ID = x.ID,
                        ACCOUNTNO = x.ACCOUNTNO,
                        ACC_TYPE = x.ACC_TYPE,
                        SCHM_CODE = x.SCHM_CODE,
                        ACC_DESC = x.ACC_DESC,
                        BRANCHCODE = x.BRANCHCODE,
                        CURRENCY = x.CURRENCY,
                        Active = x.Active,
                        QR_ALLOW = x.QR_ALLOW
                    });
                    List<AccountVM> closedaccvms = new List<AccountVM>();
                    //getting core accounts               
                    if (cri.lstCorpAccountInfo.Count > 0)
                    {
                        List<CorpAccountInfo> core_accs = cri.lstCorpAccountInfo;
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
                                    SCHM_CODE = acc.Schm_Code,
                                    ACC_DESC = acc.AccountTypeDesc,
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


                return View(userandaccvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                userandaccvm.closedaccs = new List<AccountVM>();
                if (ModelState.IsValid)
                {
                    UserVM uservm = userandaccvm.uservm;
                    List<AccountVM> accvms = userandaccvm.accountvms;
                    RetailResponseInfo rri = new RetailResponseInfo();
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    //rri = GetRetailResponse(uservm.CIFID);
                    User user = db._Users.Find(uservm.ID);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    //checking Validation
                    #region Validation
                    // commenting old validation before VIP logic
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
                        ModelState.AddModelError("uservm.DAILY_MAX_AMT", "The field Allow Tran limit must be between 0 and 9999999999999999.99.");
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
                    if (accvms == null)
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
                    #endregion

                    if (user.USER_TYPE == "Retail")
                    {
                        rri = GetRetailResponse(uservm.CIFID);

                        //user.USERNAME = uservm.USERNAME; //not editable username
                        user.FULLNAME = rri.Name;
                        user.NRC = rri.NRC;
                        user.MOBILENO = uservm.MOBILENO;
                        user.EMAIL = uservm.EMAIL;
                        user.ADDRESS = uservm.ADDRESS;
                        user.MINOR = rri.Minor;
                        user.GENDER = rri.Gender;
                    }
                    else if (user.USER_TYPE == "Corporate")
                    {
                        cri = GetCorporateResponse(uservm.CIFID);

                        user.FULLNAME = cri.Name;
                        user.NRC = cri.AccountOpenDate;
                        user.MOBILENO = uservm.MOBILENO;
                        user.EMAIL = uservm.EMAIL;
                        user.ADDRESS = uservm.ADDRESS;
                    }

                    //user.ALLOW_TRAN_COUNT = uservm.ALLOW_TRAN_COUNT;
                    //user.DAILY_MAX_AMT = uservm.DAILY_MAX_AMT;                    
                    //Check VIP
                    user.IsVIP = uservm.IsVIP;//Update VIP flag
                    if (uservm.IsVIP)
                    {
                        //get form inputs
                        user.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        user.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    else
                    {
                        //get rule values for Normal User
                        user.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        user.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }
                    //default fields
                    user.UPDATEDATE = DateTime.Now;
                    user.UPDATEUSERID = User.Identity.GetUserId();
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    // save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{uservm.USERNAME} is updated.");

                    //updating accounts                   
                    //if (rri.NRC == null) //if wrong CIFID
                    if (user.USER_TYPE == "Retail" && rri.NRC == null) //if wrong CIFID
                    {
                        ModelState.AddModelError("uservm.CIFID", "Invalid CIFID");
                        return View(userandaccvm);
                    }

                    List<AccountInfo> core_accs = new List<AccountInfo>();//rri.lAcctInfo;
                    if (user.USER_TYPE == "Retail")
                    {
                        core_accs = rri.lAcctInfo;
                    }
                    else
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
                    
                    //List<AccountInfo> core_accs = rri.lAcctInfo;

                    UpdateAccounts(accvms, user.CIFID, core_accs);

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
        private void UpdateAccounts(List<AccountVM> accvms, string CIFID, List<AccountInfo> core_accs)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ////delete all old records  (old logic)
                //int oldRecords = db.Accounts.Where(a => a.CIFID == CIFID).Count();
                //if (oldRecords > 0)
                //{
                //    string deleteData = $"{oldRecords} accounts deleted.";
                //    db.Accounts.RemoveRange(db.Accounts.Where(a => a.CIFID == CIFID));
                //    db.SaveChanges();
                //    //save log
                //    log.Info(controllerName, actionName, 3, deleteData);
                //}

                int saveCount = 0;
                foreach (var accvm in accvms)
                {
                    var core_acc = core_accs.Where(c => c.AccountNumber == accvm.ACCOUNTNO).FirstOrDefault();
                    if (core_acc == null)//invalid acc no   or  closed accs at core
                        continue;
                    Account acc = db.Accounts.Where(a => a.CIFID == CIFID & a.ACCOUNTNO == accvm.ACCOUNTNO).FirstOrDefault();
                    if (acc == null)    //new accs
                    {
                        if (!accvm.Active)//uncheck accounts will not be saved.
                            continue;
                        saveCount++;
                        acc = new Account();
                        //core data
                        acc.ACCOUNTNO = core_acc.AccountNumber;
                        acc.ACC_TYPE = core_acc.AccountType;
                        acc.SCHM_CODE = core_acc.Schm_Code;
                        acc.ACC_DESC = core_acc.AccountTypeDesc;
                        acc.BRANCHCODE = core_acc.BranchID;
                        acc.CURRENCY = core_acc.Currency;
                        //form data
                        acc.Active = accvm.Active;
                        //QR allow false if Acc type is  ODA, LAA, TDA ,not MMK
                        if (accvm.ACC_TYPE == "ODA" || accvm.ACC_TYPE == "LAA" || accvm.ACC_TYPE == "TDA" /*|| accvm.CURRENCY != "MMK"*/)
                        {
                            acc.QR_ALLOW = false;
                        }
                        else
                            acc.QR_ALLOW = accvm.QR_ALLOW;
                        //default values
                        acc.CIFID = CIFID;
                        acc.CreatedDateTime = DateTime.Now;
                        acc.CreatedUserID = User.Identity.GetUserId();
                        acc.ACCT_CLOSE_FLAG = false;

                        db.Accounts.Add(acc);
                        db.SaveChanges();
                    }
                    else    //update
                    {
                        saveCount++;//to count including update
                        acc.ACCT_CLOSE_FLAG = false;//to reopen closed acc
                        acc.Active = accvm.Active;
                        //QR allow false if Acc type is  ODA, LAA, TDA
                        if (!acc.Active || acc.ACC_TYPE == "ODA" || acc.ACC_TYPE == "LAA" || acc.ACC_TYPE == "TDA" /*|| accvm.CURRENCY != "MMK"*/)
                        {
                            acc.QR_ALLOW = false;
                        }
                        else
                            acc.QR_ALLOW = accvm.QR_ALLOW;
                        acc.UpdatedDateTime = DateTime.Now;
                        acc.UpdatedUserId = User.Identity.GetUserId();
                        db.Entry(acc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                string getuserid = User.Identity.GetUserId();
                //if (accvms.Count > 0)
                if (saveCount > 0)
                    log.Info(getuserid, controllerName, actionName, 2, $"{saveCount} accounts saved.");
                //close core_closed accs
                List<Account> accs = db.Accounts.Where(a => a.CIFID == CIFID).ToList();
                foreach (Account acc in accs)
                {
                    if (core_accs.Where(c => c.AccountNumber == acc.ACCOUNTNO).Count() == 0)
                    {
                        acc.ACCT_CLOSE_FLAG = true;//update flag if closed at core
                        db.Entry(acc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                User user = db._Users.Find(id);
                string deleteData = $"{db.Accounts.Where(a => a.CIFID == user.CIFID).Count()} accounts deleted.";
                //delete accounts if needs      //with CIFID
                db.Accounts.RemoveRange(db.Accounts.Where(a => a.CIFID == user.CIFID));
                db.SaveChanges();
                //save log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 3, deleteData);
                //delete user
                user.DEL_STATUS = true;
                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //save log
                deleteData = user.ToString();
                //string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 3, deleteData);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Validate EmailAddress
        public bool IsEmailValid(string emailaddress)
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

        //Validate Rule
        private bool IsMailSent()
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R035").Select(r => r.Value).FirstOrDefault();
                if (rule == 1)
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
            catch (Exception ex)
            {                
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return false;

            }
        }
        //For Retail Transfer Email
        private SMS_Allow EmailAllow(string type)
        {
            try
            {
                var emailallow = db.SMS_Allows.Where(a => a.TransactionType == type).FirstOrDefault();
                return emailallow;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }


    [HttpPost]
        public JsonResult SetResetPwd(int id)
        {
            try
            {
                User user = db._Users.Find(id);
                if (user == null)
                {
                    //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("Setting reset password fail!", JsonRequestBehavior.AllowGet);
                }

                string LoginPWD = "";
                string TranPWD = "";

                //transaction
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // try to save at TBL_USER first
                        //save old pwd
                        string old1 = user.LOGIN_PRE_PWD_1;
                        string old2 = user.LOGIN_PRE_PWD_2;
                        string old3 = user.LOGIN_PRE_PWD_3;
                        General.UpdateOldPwds(user.LOGINPASSWORD, ref old1, ref old2, ref old3);

                        string Told1 = user.TRAN_PRE_PWD_1;
                        string Told2 = user.TRAN_PRE_PWD_2;
                        string Told3 = user.TRAN_PRE_PWD_3;
                        General.UpdateOldPwds(user.TRANSACTIONPASSWORD, ref Told1, ref Told2, ref Told3);

                        LoginPWD = GeneratePassword.Generate(8, true, true, false);
                        System.Threading.Thread.Sleep(100);
                        TranPWD = GeneratePassword.Generate(8, true, true, false);
                        //old pwds
                        user.LOGIN_PRE_PWD_1 = old1;
                        user.LOGIN_PRE_PWD_2 = old2;
                        user.LOGIN_PRE_PWD_3 = old3;
                        user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(LoginPWD, user.USERNAME);
                        user.TRAN_PRE_PWD_1 = Told1;
                        user.TRAN_PRE_PWD_2 = Told2;
                        user.TRAN_PRE_PWD_3 = Told3;
                        user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(TranPWD, user.USERNAME);
                        user.FORCE_PWD_CHANGE_FLG = true;
                        //update fail counts
                        user.FAIL_PASSWORD_COUNT = 0;
                        user.FAIL_TRAN_PWD_COUNT = 0;
                        int? expday = Convert.ToInt32(GetRule("R007"));
                        user.PWD_EXP_DATE = DateTime.Now.AddDays(expday ?? 90);//default 90
                        user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(expday ?? 90);//default 90
                        user.UPDATEDATE = DateTime.Now;
                        user.UPDATEUSERID = User.Identity.GetUserId();
                        user.CHK_old_pwd_flag = false;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        // try to disable Fingerprint
                        DisableFingerprint(user.USERID);

                        //Kill Session
                        KillSession(user.USERID);

                        // everything is fine - save all the data permanently
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // roll back all database operations, if anything goes wrong
                        transaction.Rollback();
                        //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        throw ex;
                    }
                }


                //generate pdf
                //string filename = GeneratePdf(user.USERNAME, LoginPWD, TranPWD,resetstatus=1);
                //string allow_tran_count = Convert.ToString(user.ALLOW_TRAN_COUNT);
                var tran_amt = db.Rules.Where(r => r.Code == "R014").FirstOrDefault();
                string tran_max_amt = tran_amt.Value.ToString();
                string daily_max_count = Convert.ToString(user.DAILY_MAX_AMT);
                //var email = new System.Net.Mail.MailAddress(user.EMAIL);                
                string filename = GeneratePdf(user.USERNAME, user.FULLNAME, user.CIFID, user.ADDRESS, user.MOBILENO, user.EMAIL, tran_max_amt, daily_max_count, LoginPWD, TranPWD, resetstatus = 1);
                //send email to user
                General.SMTPSetting sms = new General.SMTPSetting();

                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Resetting Password successfully.", $"User ID is {user.USERNAME}.");
                //Sent Email
                if(!String.IsNullOrEmpty(user.EMAIL) && IsEmailValid(user.EMAIL) && IsMailSent())
                {
                    //Validate Email Allow
                    var tranretail = "Retail User Management";
                    var mailallow = EmailAllow(tranretail);
                    if (mailallow.Receiver_Email_Allow == true)
                    {
                        try
                        {
                            //SMTP setting setup
                            General.SMTPSetting setting = GetEmailSetting();
                            var template = from t in db.EmailTemplates
                                           where t.Type == "Reset"
                                           select t;

                            var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                            var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                            Body = Body.Replace("[USERNAME]", (user.USERNAME.ToString()));
                            Body = Body.Replace("[FULLNAME]", (user.FULLNAME.ToString()));
                            // var mail = GetEmail();
                            //setting.SenderMail = "bnkdmndev@gmail.com";
                            //setting.SenderName = "A bank";
                            //setting.Password = "$Abankadmin@123";
                            //setting.Host = "smtp.gmail.com";
                            //setting.Port = 587;
                            //string Subject = "A bank Mobile Banking Information";
                            //string Body = "Your Account has been successfully resetted. Please check the credential at the Attached pdf.";
                            bool sentmail = General.SendEmail(user.EMAIL, Subject, Body, filename, 1, 0, setting);
                            //log for email
                            log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {user.USERNAME}.Email Address is {user.EMAIL}.");
                        }
                        catch (Exception ex)
                        {
                            log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                        }
                    }
                }
                return Json(new { msg = $"New Login Password is {LoginPWD}\r\nNew Transaction Password is {TranPWD} ", filename = filename }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                Response.StatusCode = 500;
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        //private bool GetRuleValue()
        //{
        //    try
        //    {
        //        bool tranlock;

        //        foreach (var item in db._Users.ToList())
        //        {
        //            /*(db._Users.Where(u => u.DEL_STATUS == false).FirstOrDefault());*/
        //            int user = (from u in db._Users
        //                        where u.DEL_STATUS == false
        //                        select u.FAIL_TRAN_PWD_COUNT).FirstOrDefault();
        //            if (user <= rule)
        //            {
        //                tranlock = false;
        //            }
        //            else
        //            {
        //                tranlock = true;
        //            }
        //        }
        //        return tranlock;
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Error(controllerName, actionName, ex);
        //        return false;
        //    }
        //}
        //[HttpPost]
        //public JsonResult SetSigninPwd(int id)
        //{
        //    try
        //    {
        //        User user = db._Users.Find(id);
        //        if (user == null)
        //        {
        //            //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //            return Json("Setting login password fail!", JsonRequestBehavior.AllowGet);
        //        }

        //        //save old pwd
        //        string old1 = user.LOGIN_PRE_PWD_1;
        //        string old2 = user.LOGIN_PRE_PWD_2;
        //        string old3 = user.LOGIN_PRE_PWD_3;
        //        UpdateOldPwds(user.LOGINPASSWORD, ref old1, ref old2, ref old3);

        //        string LoginPWD = "";
        //        LoginPWD = GeneratePassword.Generate(8, true, true);
        //        //old pwds
        //        user.LOGIN_PRE_PWD_1 = old1;
        //        user.LOGIN_PRE_PWD_2 = old2;
        //        user.LOGIN_PRE_PWD_3 = old3;
        //        user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(LoginPWD);
        //        user.FORCE_PWD_CHANGE_FLG = true;
        //        user.PWD_EXP_DATE = DateTime.Now.AddDays(GetRule("R007") ?? 90);//default 90
        //        user.UPDATEDATE = DateTime.Now;
        //        user.UPDATEUSERID = "1"; //login user id
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();

        //        //generate pdf
        //        string filename = GeneratePdf(user.USERNAME, LoginPWD, null);

        //        //log
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Log(controllerName, actionName, $"Resetting LoginPassword successfully.\nUser ID is {user.USERNAME}.");

        //        return Json(new { msg = $"New Password is {LoginPWD}", filename = filename }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Error(controllerName, actionName, ex);
        //        Response.StatusCode = 500;
        //        return Json("Error", JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpPost]
        //public JsonResult SetTranPwd(int id)
        //{
        //    try
        //    {
        //        User user = db._Users.Find(id);
        //        if (user == null)
        //        {
        //            return Json("Setting Transaction password fail!", JsonRequestBehavior.AllowGet);
        //        }

        //        //save old pwd
        //        string old1 = user.TRAN_PRE_PWD_1;
        //        string old2 = user.TRAN_PRE_PWD_2;
        //        string old3 = user.TRAN_PRE_PWD_3;
        //        UpdateOldPwds(user.TRANSACTIONPASSWORD, ref old1, ref old2, ref old3);

        //        string TranPWD = "";
        //        TranPWD = GeneratePassword.Generate(8, true, true);
        //        user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(TranPWD);
        //        //old pwds
        //        user.TRAN_PRE_PWD_1 = old1;
        //        user.TRAN_PRE_PWD_2 = old2;
        //        user.TRAN_PRE_PWD_3 = old3;
        //        user.FORCE_TXN_PWD_CHANGE_FLG = true;
        //        user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(GetRule("R007") ?? 90);//default 90
        //        user.UPDATEDATE = DateTime.Now;
        //        user.UPDATEUSERID = "1"; //login user id
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();

        //        //generate pdf
        //        string filename = GeneratePdf(user.USERNAME, null, TranPWD);

        //        //log
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Log(controllerName, actionName, $"Resetting Transaction Password successfully.\nUser ID is {user.USERNAME}.");

        //        return Json(new { msg = $"New Transaction Password is {TranPWD}", filename = filename }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Error(controllerName, actionName, ex);
        //        return Json("Error", JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult Lock(int id)
        {
            try
            {
                User user = db._Users.Find(id);
                if (user == null)
                {
                    return Json("Lock fail!", JsonRequestBehavior.AllowGet);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //lock
                        user.ISLOCK_FLAG = true;
                        user.ISLOGIN_FLAG = false;      //force logout
                        user.UPDATEDATE = DateTime.Now;
                        user.UPDATEUSERID = User.Identity.GetUserId();
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        //Kill Session
                        KillSession(user.USERID);

                        //Old incomplete logic
                        //update logoutdate in TBL_LOGIN
                        //List<Login> lstLogin = db.Logins.Where(l => l.UserID == user.USERID && l.LogoutDate == null).ToList();
                        //if (lstLogin.Count > 0)
                        //{
                        //    foreach (Login login in lstLogin)
                        //    {
                        //        login.SessionExpirDate = DateTime.Now;//kill sessions
                        //        login.LogoutDate = DateTime.Now;
                        //        db.Entry(login).State = EntityState.Modified;
                        //        db.SaveChanges();
                        //    }
                        //}

                        // everything is fine - save all the data permanently
                        transaction.Commit();
                        ViewData["Message"] = "Approve successfully!";
                    }
                    catch (Exception)
                    {
                        // roll back all database operations, if anything goes wrong
                        transaction.Rollback();
                        //ViewBag.ResultMessage = "Error occured, records rolledback.";
                    }
                }

                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Lock successfully",$"User ID is {user.USERNAME}.");

                return Json("Lock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Unlock(int id)
        {
            try
            {
                User user = db._Users.Find(id);
                if (user == null)
                {
                    return Json("Unlock fail!", JsonRequestBehavior.AllowGet);
                }
                //unlock
                user.ISLOCK_FLAG = false;
                //reset fail counts
                user.FAIL_PASSWORD_COUNT = 0;
                user.FAIL_TRAN_PWD_COUNT = 0;

                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Unlock successfully",$"User ID is {user.USERNAME}.");

                return Json("Unlock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult SignLock_Unlock(int id)
        //{
        //    try
        //    {
        //        User user = db._Users.Find(id);
        //        if (user == null)
        //        {
        //            if (user.ISLOCK_FLAG == true)
        //            {
        //                return Json("Lock fail!", JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json("Unlock fail!", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        //lock
        //        if (user.ISLOCK_FLAG == true)
        //        {
        //            //unlock
        //            user.ISLOCK_FLAG = false;
        //            //reset fail counts
        //            user.FAIL_PASSWORD_COUNT = 0;
        //            user.FAIL_TRAN_PWD_COUNT = 0;                                      
        //        }
        //        else
        //        {
        //            //lock
        //            user.ISLOCK_FLAG = true;  
        //        }
        //        user.UPDATEDATE = DateTime.Now;
        //        user.UPDATEUSERID = User.Identity.GetUserId();
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();
        //        //log
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string getuserid = User.Identity.GetUserId();
        //        if (user.ISLOCK_FLAG == true)
        //        {
        //            log.Log(getuserid, controllerName, actionName, $"Lock successfully.\nUser ID is {user.USERNAME}.");
        //            return Json("Lock successfully!", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            log.Log(getuserid, controllerName, actionName, $"Unlock successfully.\nUser ID is {user.USERNAME}.");
        //            return Json("Unlock successfully!", JsonRequestBehavior.AllowGet);
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return Json("Error", JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpPost]
        //public JsonResult TranLock_Unlock(int id)
        //{
        //    try
        //    {
        //        var rule = db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
        //        int ruleValue = (rule == null ? 5 : rule.Value);
        //        User user = db._Users.Find(id);
        //        UserVM uservm = new UserVM();
        //        if (user == null)
        //        {
        //            if (user.ISLOCK_FLAG == true)
        //            {
        //                return Json("Transaction Lock fail!", JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json("Transaction Unlock fail!", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        //lock
        //        if (user.FAIL_TRAN_PWD_COUNT >= ruleValue)
        //        {
        //            //unlock
        //            uservm.TRAN_LOCK = false;
        //            //reset tran counts
        //            user.FAIL_TRAN_PWD_COUNT = 0;                   
        //        }
        //        else
        //        {
        //            //lock
        //            uservm.TRAN_LOCK = true;
        //            //reset tran counts
        //            user.FAIL_TRAN_PWD_COUNT = ruleValue;                   
        //        }
        //        user.UPDATEDATE = DateTime.Now;
        //        user.UPDATEUSERID = User.Identity.GetUserId();
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();
        //        //log
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string getuserid = User.Identity.GetUserId();
        //        if (user.FAIL_TRAN_PWD_COUNT >= 20)
        //        {
        //            log.Log(getuserid, controllerName, actionName, $"Transactoin lock successfully.\nUser ID is {user.USERNAME}.");
        //            return Json("Transaction lock successfully!", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            log.Log(getuserid, controllerName, actionName, $"Transactoin Unlock successfully.\nUser ID is {user.USERNAME}.");
        //            return Json("Transaction Unlock successfully!", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return Json("Error", JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult TransactionLock(int id)
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
                int IntRule = Convert.ToInt32(rule.Value);
                int ruleValue = (rule == null ? 5 : IntRule);
                User user = db._Users.Find(id);
                if (user == null)
                {
                    return Json("Transaction lock fail!", JsonRequestBehavior.AllowGet);
                }
                //lock
                UserVM uservm = new UserVM();
                uservm.TRAN_LOCK = true;
                //reset tran counts
                user.FAIL_TRAN_PWD_COUNT = ruleValue;

                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Transactoin lock successfully.",$"User ID is {user.USERNAME}.");

                return Json("Transaction lock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TransactionUnlock(int id)
        {
            try
            {
                User user = db._Users.Find(id);
                if (user == null)
                {
                    return Json("Transaction Unlock fail!", JsonRequestBehavior.AllowGet);
                }
                //unlock
                UserVM uservm = new UserVM();
                uservm.TRAN_LOCK = false;
                //reset tran counts
                user.FAIL_TRAN_PWD_COUNT = 0;

                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Transactoin Unlock successfully.",$"User ID is {user.USERNAME}.");

                return Json("Transaction Unlock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
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
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
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

        //generate pdf      //if txnPwd = null, resetting login pwd, if LoginPwd = null, resetting transaction pwd
        private string GeneratePdf(string USERNAME, string FULLNAME, string CIFID, string ADDRESS , string MOBILENO, string EMAIL, string tran_max_amt, string daily_max_amt, string LoginPwd, string txnPwd, int Status)
        {
            try
            {
                //tran_max_amt= (Convert.ToInt32(tran_max_amt).ToString("#,##0")).ToString()+" MMK"; //add thousand separator
                tran_max_amt = (decimal.Parse(tran_max_amt, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add currency code
                daily_max_amt = (decimal.Parse(daily_max_amt, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK";//add currency code
                //daily_max_amt = (Convert.ToInt32(daily_max_amt).ToString("#,##0")).ToString() + " MMK"; //add thousand separator
                //string Pdf_Path = System.Configuration.ConfigurationManager.AppSettings["PdfSavePath"];
                string Pdf_Path = ConstantValues.Pdf_Path;
                string filename = "";
                User new_user = new User();
                //IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
                  
                if(resetstatus == 1)//if txnPwd = null, resetting login pwd
                {
                    Pdf_Path = $"{Pdf_Path}\\reset password";
                    //create if folder doesn't exist
                    if (!Directory.Exists(Pdf_Path))
                    {
                        Directory.CreateDirectory(Pdf_Path);
                    }
                    // Create a new PDF document
                    PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                    // Create an empty page
                    PdfSharp.Pdf.PdfPage page = document.AddPage();
                    //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                    //draw bg
                    DrawImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    // Create a font
                    XFont font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo,1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    //pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);
                    //gfx.DrawRectangle(pens, 310, 0, 239, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Date", font, XBrushes.Black, new XRect(335, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(420, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For CIFID name
                    gfx.DrawString($"  Customer_ID", font, XBrushes.Black, new XRect(335, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For CIFID value
                    gfx.DrawString($": {CIFID}", font, XBrushes.Black, new XRect(420, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));
                    // Draw the text
                    gfx.DrawString("eBanking Password Reset Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    //Add Draw Dotted Line in full page
                    //XPen pens = XPens.DarkRed.Clone();
                    //pens.DashStyle = XDashStyle.Dot;
                    //for (double r = 0; r <= 90; r += 0.5)
                    //    gfx.DrawLine(pens, 0, 0, 1000 * Math.Cos(r / 90 * Math.PI), 1000 * Math.Sin(r / 90 * Math.PI));

                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));

                    //Draw Dotted Line
                    //XPen pen = new XPen(XColors.Indigo, 1.5);
                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 133, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    //gfx.DrawLine(pen, 50, 158, 550, 158);
                    gfx.DrawLine(pens, 50, 158, 50, 280);
                    gfx.DrawLine(pens, 50, 280, 550, 280);
                    gfx.DrawLine(pens, 550, 158, 550, 280);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);                    
                    // Draw the text
                    gfx.DrawString("Customer Information", font, XBrushes.Black,new XRect(60, 120, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    // For Customer name
                    gfx.DrawString($"●  Customer Name", font, XBrushes.Black, new XRect(60, 168, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {FULLNAME}", font, XBrushes.Black, new XRect(210, 168, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 188, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MOBILENO}", font, XBrushes.Black, new XRect(210, 188, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 208, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {EMAIL}", font, XBrushes.Black, new XRect(210, 208, page.Width, 10),
                      XStringFormats.CenterLeft);                    

                    //for address text autofix
                    var tf = new XTextFormatter(gfx);

                    // For Address name
                    gfx.DrawString($"●  Address", font, XBrushes.Black, new XRect(60, 228, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Address value
                    tf.DrawString($": {ADDRESS}", font, XBrushes.Black, new XRect(210, 228, 345, 50),
                      XStringFormats.TopLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 325, 500, 110);
                    //gfx.DrawLine(pen, 50, 350, 550, 350);
                    gfx.DrawRectangle(pen, 50,300, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 395, 50, 325);
                    gfx.DrawLine(pens, 50, 395, 550, 395);
                    gfx.DrawLine(pens, 550, 395, 550, 325);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 290, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);
                    //Create password font
                     XFont pwfont = new XFont("Myanmar Text", 12, XFontStyle.Regular, options);
                    // For Username name
                    gfx.DrawString($"●  Username", font, XBrushes.Black,new XRect(60, 335, page.Width, 10),
                      XStringFormats.CenterLeft);                                      
                    // For Username value
                    gfx.DrawString($": {USERNAME}", font, XBrushes.Black,new XRect(210, 335, page.Width, 10),
                      XStringFormats.CenterLeft);
                    
                    // For Loginpwd name
                    gfx.DrawString($"●  Login Password", font, XBrushes.Black,new XRect(60, 355, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Loginpwd value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black,new XRect(210, 355, page.Width, 10),
                      XStringFormats.CenterLeft);
                  
                    // For Tranpwd name
                    gfx.DrawString($"●  Transaction Password", font, XBrushes.Black,new XRect(60, 375, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {txnPwd}", pwfont, XBrushes.Black,new XRect(210, 375, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 455, 500, 90);
                    //gfx.DrawLine(pen, 50, 480, 550, 480);
                    gfx.DrawRectangle(pen, 50, 415, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 490, 50, 440);
                    gfx.DrawLine(pens, 50, 490, 550, 490);
                    gfx.DrawLine(pens, 550, 490, 550, 440);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 405, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);
                    // For Tranpwd name
                    gfx.DrawString($"●  One time Transaction Limit", font, XBrushes.Black, new XRect(60, 450, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {tran_max_amt}", font, XBrushes.Black, new XRect(300, 450, page.Width, 10),
                      XStringFormats.CenterLeft); 

                        // For Tranpwd name
                    gfx.DrawString($"●  Daily Limit", font, XBrushes.Black, new XRect(60, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {daily_max_amt}", font, XBrushes.Black, new XRect(300, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    font = new XFont("Arial", 12, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle =XDashStyle.Dot;
                    gfx.DrawLine(pens, 330, 520,225, 520);
                    //for password policies
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 490, page.Width, 45),
                      XStringFormats.CenterLeft);

                    //draw rectangle
                    //pens = new XPen(XColors.Indigo, 1);
                    //gfx.DrawRectangle(pens, 50, 540, 210, 110);

                    // for font create
                   
                    font = new XFont("Arial", 10, XFontStyle.Regular, options);

                    //for password policies
                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //gfx.DrawString($"●   At least 1 Uppercase letter", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●   At least 1 Lowercase letter", font, XBrushes.Black, new XRect(60, 575, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 575, page.Width, 10),
                    XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 590, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 605, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 620, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"      from transaction password", font, XBrushes.Black, new XRect(60, 635, page.Width, 10),
                    XStringFormats.CenterLeft);

                    //for myanmar language
                    //draw rectangle
                    //pens = new XPen(XColors.Indigo, 1);
                    //gfx.DrawRectangle(pens, 275, 540, 275, 110);

                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 575, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 590, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for address text autofix
                    tf = new XTextFormatter(gfx);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 605, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 620, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 635, page.Width, 10),
                   XStringFormats.CenterLeft);
                    // for font create
                   // XFont fonts = new XFont("Myanmar Text", 10, XFontStyle.Regular, options);
                   // //for password policies
                   // gfx.DrawString($"   အနည်းဆုံး ၆ လုံးရှိရပါမည်။", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                   //   XStringFormats.CenterLeft);
                   // gfx.DrawString($"   အများဆုံး ၁၆ လုံးအထိေပးနိုင်ပါသည်။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                   //   XStringFormats.CenterLeft);
                   // gfx.DrawString($"   အက္ခရာ စာလုံးအနည်းဆုံးတစ်လုံးပါဝင်ရပါမည်။", fonts, XBrushes.Black, new XRect(270, 575, page.Width, 10),
                   //   XStringFormats.CenterLeft);
                   // gfx.DrawString($"   Special Character(!@#%&^*)အနည်းဆုံး ၁ လုံးပါရှိရပါမည်။", fonts, XBrushes.Black, new XRect(270, 590, page.Width, 10),
                   //   XStringFormats.CenterLeft);

                   // //for address text autofix
                   // tf = new XTextFormatter(gfx);
                   // gfx.DrawString($"   စကားဝှက်တွင် စာလုံးတစ်လုံးနှင့်တစ်လုံးမြခားရပါ။", fonts, XBrushes.Black, new XRect(270, 605, 345, 10),
                   //   XStringFormats.CenterLeft);
                   // gfx.DrawString($"   (< > /)များမပါဝင်ရပါ။", fonts, XBrushes.Black, new XRect(270, 620, 345, 10),
                   // XStringFormats.CenterLeft);
                   // // for font create
                   // fonts = new XFont("Myanmar Text", 10, XFontStyle.Regular, options);
                   // gfx.DrawString($"   ၀င်ေရာက်ရန် စကားဝှက်နှင့် လုပ်ေဆာင်မှုစကားဝှက်တို့သည်", fonts, XBrushes.Black, new XRect(270, 635, page.Width, 10),
                   //   XStringFormats.CenterLeft);
                   // gfx.DrawString($"   မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 650, page.Width, 10),
                   //XStringFormats.CenterLeft);



                    //for  Ass / Manager
                    gfx.DrawString($"Assistant / Manager", font, XBrushes.Black, new XRect(450, 690, 50, 50),
                      XStringFormats.CenterLeft);
                   
                    string timestamp = General.GetTimestamp(DateTime.Now);                    
                    // Save the document...
                    filename = $"{timestamp}_{USERNAME}_resetpwd.pdf";
                    document.Save($"{Pdf_Path}\\{filename}");
                    // Draw background
                    //gfx.DrawImage(XImage.FromFile("D:/ABankAdmin/ABankAdmin/Images/mini-side-bar-logo.png"), 510, 700);


                    //XImage xImagePosterBackground = XImage.FromFile("D:/ABankAdmin/ABankAdmin/Images/"+"mini-side-bar-logo.png");

                    //XGraphics graphics = XGraphics.FromPdfPage(page);

                    //graphics.DrawImage(xImagePosterBackground, new XPoint(510, 700));

                    // Draw page border for better visual representation
                    //XRect A4Rect = new XRect(50, 0, 500, 750);
                    //gfx.DrawRectangle(XPens.Black, A4Rect);

                    // Draw the text
                    // gfx.DrawString($"New Login Password is {LoginPwd}", font, XBrushes.Black,
                    //    new XRect(60, 410, page.Width, 50),
                    //   XStringFormats.CenterLeft);

                    // font = new XFont("Arial", 10, XFontStyle.Bold, options);

                    //// Draw the text
                    // gfx.DrawString($"New Transaction Password is {txnPwd}", font, XBrushes.Black,
                    //   new XRect(60, 440, page.Width, 50),
                    //   XStringFormats.CenterLeft);

                    // font = new XFont("Arial", 8, XFontStyle.Regular, options);                    // Draw the text

                }
                else
                {
                    Pdf_Path = $"{Pdf_Path}\\register";
                    //create if folder doesn't exist
                    if (!Directory.Exists(Pdf_Path))
                    {
                        Directory.CreateDirectory(Pdf_Path);
                    }
                    // Create a new PDF document
                    PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                    // Create an empty page
                    PdfSharp.Pdf.PdfPage page = document.AddPage();
                    //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                    //draw bg
                    DrawImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    // Create a font
                    XFont font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo, 1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    //pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);
                    //gfx.DrawRectangle(pens, 310, 0, 239, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Date", font, XBrushes.Black, new XRect(335, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(420, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For CIFID name
                    gfx.DrawString($"  Customer_ID", font, XBrushes.Black, new XRect(335, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For CIFID value
                    gfx.DrawString($": {CIFID}", font, XBrushes.Black, new XRect(420, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));
                    // Draw the text
                    gfx.DrawString("eBanking User Register Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    //Add Draw Dotted Line in full page
                    //XPen pens = XPens.DarkRed.Clone();
                    //pens.DashStyle = XDashStyle.Dot;
                    //for (double r = 0; r <= 90; r += 0.5)
                    //    gfx.DrawLine(pens, 0, 0, 1000 * Math.Cos(r / 90 * Math.PI), 1000 * Math.Sin(r / 90 * Math.PI));

                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));

                    //Draw Dotted Line
                    //XPen pen = new XPen(XColors.Indigo, 1.5);
                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 133, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    //gfx.DrawLine(pen, 50, 158, 550, 158);
                    gfx.DrawLine(pens, 50, 158, 50, 280);
                    gfx.DrawLine(pens, 50, 280, 550, 280);
                    gfx.DrawLine(pens, 550, 158, 550, 280);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Customer Information", font, XBrushes.Black, new XRect(60, 120, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    // For Customer name
                    gfx.DrawString($"●  Customer Name", font, XBrushes.Black, new XRect(60, 168, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {FULLNAME}", font, XBrushes.Black, new XRect(210, 168, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 188, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MOBILENO}", font, XBrushes.Black, new XRect(210, 188, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 208, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {EMAIL}", font, XBrushes.Black, new XRect(210, 208, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //for address text autofix
                    var tf = new XTextFormatter(gfx);

                    // For Address name
                    gfx.DrawString($"●  Address", font, XBrushes.Black, new XRect(60, 228, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Address value
                    tf.DrawString($": {ADDRESS}", font, XBrushes.Black, new XRect(210, 228, 345, 50),
                      XStringFormats.TopLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 325, 500, 110);
                    //gfx.DrawLine(pen, 50, 350, 550, 350);
                    gfx.DrawRectangle(pen, 50, 300, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 395, 50, 325);
                    gfx.DrawLine(pens, 50, 395, 550, 395);
                    gfx.DrawLine(pens, 550, 395, 550, 325);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 290, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);
                    //Create password font
                    XFont pwfont = new XFont("Myanmar Text", 12, XFontStyle.Regular, options);
                    // For Username name
                    gfx.DrawString($"●  Username", font, XBrushes.Black, new XRect(60, 335, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Username value
                    gfx.DrawString($": {USERNAME}", font, XBrushes.Black, new XRect(210, 335, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Loginpwd name
                    gfx.DrawString($"●  Login Password", font, XBrushes.Black, new XRect(60, 355, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Loginpwd value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black, new XRect(210, 355, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Tranpwd name
                    gfx.DrawString($"●  Transaction Password", font, XBrushes.Black, new XRect(60, 375, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {txnPwd}", pwfont, XBrushes.Black, new XRect(210, 375, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 455, 500, 90);
                    //gfx.DrawLine(pen, 50, 480, 550, 480);
                    gfx.DrawRectangle(pen, 50, 415, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 490, 50, 440);
                    gfx.DrawLine(pens, 50, 490, 550, 490);
                    gfx.DrawLine(pens, 550, 490, 550, 440);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 405, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);
                    // For Tranpwd name
                    gfx.DrawString($"●  One time Transaction Limit", font, XBrushes.Black, new XRect(60, 450, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {tran_max_amt}", font, XBrushes.Black, new XRect(300, 450, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Tranpwd name
                    gfx.DrawString($"●  Daily Limit", font, XBrushes.Black, new XRect(60, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {daily_max_amt}", font, XBrushes.Black, new XRect(300, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    font = new XFont("Arial", 12, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle = XDashStyle.Dot;
                    gfx.DrawLine(pens, 330, 520, 225, 520);
                    //for password policies
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 490, page.Width, 45),
                      XStringFormats.CenterLeft);

                    //draw rectangle
                    //pens = new XPen(XColors.Indigo, 1);
                    //gfx.DrawRectangle(pens, 50, 540, 210, 110);

                    // for font create

                    font = new XFont("Arial", 10, XFontStyle.Regular, options);

                    //for password policies
                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //gfx.DrawString($"●   At least 1 Uppercase letter", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●   At least 1 Lowercase letter", font, XBrushes.Black, new XRect(60, 575, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 575, page.Width, 10),
                    XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 590, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 605, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 620, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"      from transaction password", font, XBrushes.Black, new XRect(60, 635, page.Width, 10),
                    XStringFormats.CenterLeft);

                    //for myanmar language
                    //draw rectangle
                    //pens = new XPen(XColors.Indigo, 1);
                    //gfx.DrawRectangle(pens, 275, 540, 275, 110);

                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 575, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 590, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for address text autofix
                    tf = new XTextFormatter(gfx);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 605, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 620, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 635, page.Width, 10),
                   XStringFormats.CenterLeft);
                    // for font create
                    // XFont fonts = new XFont("Myanmar Text", 10, XFontStyle.Regular, options);
                    // //for password policies
                    // gfx.DrawString($"   အနည်းဆုံး ၆ လုံးရှိရပါမည်။", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   အများဆုံး ၁၆ လုံးအထိေပးနိုင်ပါသည်။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   အက္ခရာ စာလုံးအနည်းဆုံးတစ်လုံးပါဝင်ရပါမည်။", fonts, XBrushes.Black, new XRect(270, 575, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   Special Character(!@#%&^*)အနည်းဆုံး ၁ လုံးပါရှိရပါမည်။", fonts, XBrushes.Black, new XRect(270, 590, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // //for address text autofix
                    // tf = new XTextFormatter(gfx);
                    // gfx.DrawString($"   စကားဝှက်တွင် စာလုံးတစ်လုံးနှင့်တစ်လုံးမြခားရပါ။", fonts, XBrushes.Black, new XRect(270, 605, 345, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   (< > /)များမပါဝင်ရပါ။", fonts, XBrushes.Black, new XRect(270, 620, 345, 10),
                    // XStringFormats.CenterLeft);
                    // // for font create
                    // fonts = new XFont("Myanmar Text", 10, XFontStyle.Regular, options);
                    // gfx.DrawString($"   ၀င်ေရာက်ရန် စကားဝှက်နှင့် လုပ်ေဆာင်မှုစကားဝှက်တို့သည်", fonts, XBrushes.Black, new XRect(270, 635, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 650, page.Width, 10),
                    //XStringFormats.CenterLeft);


                    //for  Ass / Manager
                    gfx.DrawString($"Assistant / Manager", font, XBrushes.Black, new XRect(450, 690, 50, 50),
                      XStringFormats.CenterLeft);

                    string timestamp = General.GetTimestamp(DateTime.Now);

                    // Save the document...
                    filename = $"{timestamp}_{USERNAME}.pdf";
                    document.Save($"{Pdf_Path}\\{filename}");
                }

                return filename;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return null;
            }
        }

        //draw bg image.
        void DrawImage(XGraphics gfx)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/new_pdf_bg.jpg"));

            gfx.DrawImage(image, 50, 40, 500, 650);//fixed image size => width 500, height 650

        }        

        //draw title image.
        void DrawTitleImage(XGraphics gfxTitle)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/pdftitle.jpg"));
            gfxTitle.DrawImage(image, 50, 0, 170, 60);
        }
        //public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        //{
        //    Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
        //    Graphics graphics = Graphics.FromImage(bmp);
        //    ColorMatrix colormatrix = new ColorMatrix();
        //    colormatrix.Matrix33 = opacityvalue;
        //    ImageAttributes imgAttribute = new ImageAttributes();
        //    imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        //    graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
        //    graphics.Dispose();   // Releasing all resource used by graphics 
        //    return bmp;
        //}
        //to save old pwd into old pwd 1,2,3
        //private void UpdateOldPwds(string oldpwd, ref string old1, ref string old2, ref string old3)
        //{  
        //    old3 = old2;
        //    old2 = old1;
        //    old1 = oldpwd;
        //}
        public ActionResult Download(string filename)//type 0=register, 1=reset login, 2=reset txn
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //string Pdf_Path = System.Configuration.ConfigurationManager.AppSettings["PdfSavePath"];
                string Pdf_Path = ConstantValues.Pdf_Path;
                string fullpath = "";
                if (filename.Contains("pwd.pdf"))
                {
                    fullpath = $"{Pdf_Path}\\reset password\\{filename}";
                }
                else
                {
                    fullpath = $"{Pdf_Path}\\register\\{filename}"; //register
                }
                //log
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Download successfully.", $"File name is {filename}.");
                return File(fullpath, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Disable Fingerprint Active => 0
        private void DisableFingerprint(string UserId) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loginUser = User.Identity.GetUserId();
            try
            {
                List<Fingerprint> fingerprints = db.Fingerprints.Where(fp => fp.UserId == UserId).ToList();
                foreach (Fingerprint fp in fingerprints)
                {
                    fp.Active = "0";
                    fp.UpdatedDate = DateTime.Now;
                    fp.UpdatedUserId = loginUser;
                    db.Entry(fp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                log.Log(loginUser, controllerName, "DisableFingerprint", $"Disabling Fingerprint", $"{fingerprints.Count} fingerprints were disabled.");
            }
            catch (Exception ex)
            {
                log.Error(loginUser, controllerName, "DisableFingerprint", ex);
            }
        }

        //Kill Session
        private void KillSession(string UserId)
        {
            string loginUser = User.Identity.GetUserId();
            try
            {
                //update SessionExpirDate in TBL_LOGIN
                List<Login> lstLogin = db.Logins.Where(l => l.UserID == UserId && l.LogoutDate == null).ToList();
                if (lstLogin.Count > 0)
                {
                    foreach (Login login in lstLogin)
                    {
                        login.SessionExpirDate = DateTime.Now;
                        login.LogoutDate = DateTime.Now;
                        db.Entry(login).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    log.Log(loginUser, controllerName, "KillSession", $"Killing sessions", $"{lstLogin.Count} sessions were killed.");
                }
            }
            catch (Exception ex)
            {
                log.Error(loginUser, controllerName, "KillSession", ex);
            }
        }

        //Get Email Template
        public General.SMTPSetting GetEmailSetting()
        {
            General.SMTPSetting setting = new General.SMTPSetting();
            var mail = (from a in db.Credentials
                        where a.Status == "1" & a.Name == "MailSetting"
                        select a).ToList();
            setting.SenderMail = mail.Where(a => a.Key == "sendermail").Select(n => n.Value).FirstOrDefault();
            setting.SenderName = mail.Where(a => a.Key == "sendername").Select(n => n.Value).FirstOrDefault();
            setting.Password = mail.Where(a => a.Key == "password").Select(n => n.Value).FirstOrDefault();
            setting.Host = mail.Where(a => a.Key == "host").Select(n => n.Value).FirstOrDefault();
            int port = Convert.ToInt32(mail.Where(m => m.Key == "port").Select(m => m.Value).FirstOrDefault());
            setting.Port = port;
            return setting;
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
        // GET: User/Retail
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
        // POST: User/Retail
        [HttpPost]
        public ActionResult Corporate(UserAndAccVM userandaccvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string LoginPWD = "";
                string TranPwD = "";
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

                    #region username
                    //username format
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
                    #endregion
                    #region VIP
                    int allow_tran_count = 0;
                    decimal daily_max_amount = 0;
                    //convert tran_amt and max_amt from string to int
                    try
                    {
                        allow_tran_count= General.ConvertStrToInt(uservm.ALLOW_TRAN_COUNT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("uservm.ALLOW_TRAN_COUNT", "The field Allow Tran limit must be between 0 and 9999999999999999");
                        return View(userandaccvm);
                    }
                    try
                    {
                        daily_max_amount = General.ConvertStrToDecimal(uservm.DAILY_MAX_AMT);
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
                            ModelState.AddModelError("uservm.DAILY_MAX_AMT", "Maximum Amount must be greater than Rule's count.");
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
                    #endregion

                    //generate pwd
                    LoginPWD = GeneratePassword.Generate(8, true, true, false);
                    System.Threading.Thread.Sleep(100);
                    TranPwD = GeneratePassword.Generate(8, true, true, false);

                    //create new user
                    User new_user = new User();
                    //core data
                    new_user.NRC = cri.AccountOpenDate;
                    new_user.FULLNAME = cri.Name;
                    //formdata
                    new_user.CIFID = uservm.CIFID;
                    new_user.USERNAME = uservm.USERNAME;
                    new_user.MOBILENO = uservm.MOBILENO;
                    new_user.EMAIL = uservm.EMAIL;
                    new_user.ADDRESS = uservm.ADDRESS;
                    //Check VIP
                    new_user.IsVIP = uservm.IsVIP;
                    if (uservm.IsVIP)
                    {
                        //get form inputs
                        new_user.ALLOW_TRAN_COUNT = allow_tran_count; //getting rule value by rule code   "Transaction Count Limit"
                        new_user.DAILY_MAX_AMT = daily_max_amount;   //Transaction Amount Limit
                    }
                    else
                    {
                        //get rule values for Normal User
                        new_user.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                        new_user.DAILY_MAX_AMT = GetDecimalRule("R005");   //Transaction Amount Limit
                    }

                    //guid for userid
                    new_user.USERID = Guid.NewGuid().ToString();
                    //generate pwd
                    new_user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(LoginPWD, new_user.USERNAME);
                    new_user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(TranPwD, new_user.USERNAME);

                    //default values
                    new_user.CREATEDDATE = DateTime.Now;
                    new_user.CREATEDUSERID = User.Identity.GetUserId();
                    new_user.USER_TYPE = "Corporate";//***********Corporate for corporate user
                    new_user.FIRSTTIME_FLAG = true;
                    //pwd exp date by rule
                    int? expday = Convert.ToInt32(GetRule("R007"));
                    int expDays = expday ?? 90; //default 90
                    new_user.PWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                    new_user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(expDays);

                    db._Users.Add(new_user);
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, $"User ID is {new_user.USERNAME}.");

                    //saving accounts
                    if (allowedaccvms.Count() > 0)//allowedaccvms is only selected accounts
                    {
                        List<AccountInfo> core_accs = cri.lstCorpAccountInfo.Select(ca=> new AccountInfo 
                        { 
                            AccountNumber = ca.AccountNumber,
                            AccountType = ca.AccountType,
                            AccountTypeDesc = ca.AccountTypeDesc,
                            Schm_Code = ca.Schm_Code,
                            BranchID = ca.BranchID,
                            Currency = ca.Currency
                        }).ToList();
                        SaveAccounts(allowedaccvms, new_user.CIFID, core_accs);
                    }
                    //generate pdf
                    //string allow_tran_count = Convert.ToString(new_user.ALLOW_TRAN_COUNT);
                    var tran_amt = db.Rules.Where(r => r.Code == "R014").FirstOrDefault();
                    string tran_max_amt = tran_amt.Value.ToString();
                    string daily_max_count = Convert.ToString(new_user.DAILY_MAX_AMT);
                    string filename = GeneratePdf(new_user.USERNAME, new_user.FULLNAME, new_user.CIFID, new_user.ADDRESS, new_user.MOBILENO, new_user.EMAIL, tran_max_amt, daily_max_count, LoginPWD, TranPwD, resetstatus = 0);
                    //Sent Email
                    if (!String.IsNullOrEmpty(new_user.EMAIL) && IsEmailValid(new_user.EMAIL) && IsMailSent())
                    {
                        //Validate Email Allow
                        var tranretail = "Retail User Management";
                        var mailallow = EmailAllow(tranretail);
                        if (mailallow.Receiver_Email_Allow == true)
                        {
                            try
                            {
                                //Get Data For Email
                                General.SMTPSetting setting = GetEmailSetting();
                                var template = from t in db.EmailTemplates
                                               where t.Type == "Registration"
                                               select t;
                                var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                Body = Body.Replace("[USERNAME]", (new_user.USERNAME.ToString()));
                                Body = Body.Replace("[FULLNAME]", (new_user.FULLNAME.ToString()));

                                bool sentmail = General.SendEmail(new_user.EMAIL, Subject, Body, filename, 0, 0, setting);
                                //log for email
                                log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {new_user.USERNAME}.Email Address is {new_user.EMAIL}.");
                            }
                            catch (Exception ex)
                            {
                                log.Error(getuserid, controllerName, actionName, ex);
                            }
                        }
                    }
                    //carry filename to Index
                    TempData["filename"] = filename;
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
