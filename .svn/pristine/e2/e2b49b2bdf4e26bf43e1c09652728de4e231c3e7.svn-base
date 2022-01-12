using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.ViewModels;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using System.Text.RegularExpressions;
using ABankAdmin.Core.Utils;
using System.Data.Entity;
using System.IO;
using System.Configuration;
using System.Linq.Dynamic;
using System.Globalization;
using System.Net.Mail;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class ApproveController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ApproveController";
        EventLogController log = new EventLogController();
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        private const string RetailType = ConstantValues.Retail;
        private const string CorporateType = ConstantValues.Corporate;

        // GET: Approve
        public ActionResult Index()
        {
                //show only requests of his/her Branch
                //get BranchID from Logged In User
                string userid = User.Identity.GetUserId();
                AdminUser admin = db.Users.Find(userid);
                //bind user type
                UserTypeDDL();
                
                //Get Branch Code and check is branch code of login user "Head Office" or not
                var code = admin.VrBranch.code;
                ViewBag.IsHeadOffice = (code == "9999");
                //ViewBag.aCIFID = form["aCIFID"];
                //ViewBag.aUSERNAME = form["aUSERNAME"];
                //ViewBag.aFULLNAME = form["aFULLNAME"];
                //ViewBag.aNRC = form["aNRC"];
                //ViewBag.aType = form["aType"];
                //ViewBag.aStatus = form["aStatus"];
                //ViewBag.fromDate = form["FromDate"];
                //ViewBag.toDate = form["ToDate"];
                //ViewBag.aBranch = form["aBranch"];
                //DateTime? fromDate = null, toDate = null;
                //Create select list
                ViewBag.aStatusList = new List<SelectListItem>()
                {
                    new SelectListItem{Text="Requested",Value="0"},
                    new SelectListItem{Text="Approved",Value="1"},
                    new SelectListItem{Text="Rejected",Value="2"},
                    new SelectListItem{Text="All",Value=null}
                };

                //Create Types list
                ViewBag.aTypeList = new List<SelectListItem>()
                {
                    new SelectListItem{Text="Register",Value="True"},
                    new SelectListItem{Text="Update",Value="False"}
                };

                //assign initial value for "RequestedDate" and Status
                //if (/*form["Datefilter"] == null &&*/ form["aStatus"] == null)
                //{
                //    form["aStatus"] = "0";
                //    //form["fromDate"] = null;
                //    //form["toDate"] = null;
                //    ViewBag.aStatus = form["aStatus"];
                //    //ViewBag.ReqDate = form["Datefilter"];
                //}
                //branch data to bind branch dropdown list
                var lstBranch = db.Branches.Where(r => r.DEL_FLAG ==false).ToList();
                IEnumerable<SelectListItem> items = lstBranch.Select(a => new SelectListItem
                {
                    Value = a.name,
                    Text = a.name

                });
                ViewBag.aBranchlist = items;
                //if (!string.IsNullOrEmpty(ViewBag.fromDate))
                //{
                //    fromDate = DateTime.Parse(ViewBag.fromDate);
                //}
                //if (!string.IsNullOrEmpty(ViewBag.toDate))
                //{
                //    toDate = DateTime.Parse(ViewBag.toDate);
                //}
                //if (form["aCIFID"] != null && form["aCIFID"] != "")
                //    requservms = requservms.Where(r => r.CIFID.ToLower().Contains(form["aCIFID"].Trim().ToLower())).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["aUSERNAME"] != null && form["aUSERNAME"] != "")
                //    requservms = requservms.Where(u => u.USERNAME.ToLower().Contains(form["aUSERNAME"].Trim().ToLower())).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["aFULLNAME"] != null && form["aFULLNAME"] != "")
                //    requservms = requservms.Where(u => u.FULLNAME.ToLower().Contains(form["aFULLNAME"].Trim().ToLower())).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["aNRC"] != null && form["aNRC"] != "")
                //    requservms = requservms.Where(u => u.NRC.ToLower().Contains(form["aNRC"].Trim().ToLower())).ToList();
                //if (form["aType"] != null && form["aType"] != "")
                //    requservms = requservms.Where(u => u.ISNEW.ToString().Equals(form["aType"])).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["aStatus"] != null && form["aStatus"] != "")
                //    requservms = requservms.Where(u => u.STATUS.ToString().Equals(form["aStatus"])).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["FromDate"] != null && form["FromDate"] != "")
                //    requservms = requservms.Where(r => Convert.ToDateTime(r.REQUESTEDDATE).Date >= fromDate).OrderByDescending(u=>u.REQUESTEDDATE).ToList();
                //if (form["ToDate"] != null && form["ToDate"] != "")
                //    requservms = requservms.Where(r => Convert.ToDateTime(r.REQUESTEDDATE).Date <= toDate).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["FromDate"] != null && form["FromDate"] != "" && form["ToDate"] != null && form["ToDate"] != "")
                //    requservms = requservms.Where(r => (Convert.ToDateTime(r.REQUESTEDDATE).Date >= fromDate) && (Convert.ToDateTime(r.REQUESTEDDATE).Date <= toDate)).OrderByDescending(u => u.REQUESTEDDATE).ToList();
                //if (form["aBranch"] != null && form["aBranch"] != "")
                //    requservms = requservms.Where(u => u.BRANCH.Equals(form["aBranch"])).OrderByDescending(u => u.REQUESTEDDATE).ToList();


                //if (form["aStatus"] != null && form["aStatus"] != ""&& form["Datefilter"] != null && form["Datefilter"] != "") {
                //    requservms = requservms.Where(r => r.STATUS.ToString()=="0" && Convert.ToDateTime(r.REQUESTEDDATE).ToString("MM/dd/yyyy") == ViewBag.ReqDate.ToString()).ToList();
                //}


                //if (form["aStatus"] == "3" && form["Datefilter"] != "")
                //{
                //    requservms = requservms.Where(r => Convert.ToDateTime(r.REQUESTEDDATE).ToString("MM/dd/yyyy") == ViewBag.ReqDate.ToString()).ToList();
                //}  

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
            var aCIFID = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var aUSERNAME = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var aType = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var aStatus = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);
            var aBranch="";
            if (admin.VrBranch.code == "9999")
            {
                aBranch = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            }

            DateTime? fromDate = null, toDate = null;            
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
            }
            //var Datefilter = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            //DateTime? fromDate = null, toDate = null;
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
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();           
            IEnumerable<ReqUserVM> requservms;
            //only pending status(not approved/rejected), not own request
            //if admin is head office's admin, show requests of all branches
            //if branch admin, only same branch requests
            //requservms = db.ReqUsers.Where(u => ((admin.BranchID == u.BranchID || admin.VrBranch.code == "9999") && u.STATUS == 0 && u.MAKER != userid) || u.CHECKER == userid).ToList()
            requservms = db.ReqUsers.Where(u => (admin.BranchID == u.BranchID || admin.VrBranch.code == "9999") && u.MAKER != userid & u.USER_TYPE=="Retail").ToList()
                .ConvertAll(ru => new ReqUserVM
                {
                    ID = ru.ID,
                    MOBILENO = ru.MOBILENO,
                    USERNAME = ru.USERNAME,
                    FULLNAME = ru.FULLNAME,
                    NRC = ru.NRC,
                    EMAIL = ru.EMAIL,
                    ADDRESS = ru.ADDRESS,
                    BRANCH = ru.VrBranch.name,
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

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            ViewBag.IsHeadOffice = (code == "9999");

            if (!string.IsNullOrEmpty(aCIFID))
            {
                requservms = requservms.Where(a => a.CIFID.ToLower().Trim().Contains(aCIFID.ToLower().Trim()));

            }
            if (!string.IsNullOrEmpty(aUSERNAME))
            {
                requservms = requservms.Where(a => a.USERNAME.ToLower().Trim().Contains(aUSERNAME.ToLower().Trim()));

            }
            if (!string.IsNullOrEmpty(aType))
            {
                requservms = requservms.Where(a => a.ISNEW.ToString() == aType);

            }
            if (!string.IsNullOrEmpty(aStatus))
            {
                requservms = requservms.Where(a => a.STATUS.ToString() == aStatus);

            }
            if (admin.VrBranch.code == "9999")
            {
                if (!string.IsNullOrEmpty(aBranch))
                {
                    requservms = requservms.Where(a => a.BRANCH.ToString() == aBranch);

                }
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                requservms = requservms.Where(a => a.USER_TYPE == usertype);

            }

            if (fromDate != null & toDate != null)
            {
                requservms = requservms.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate);
            }
            else if (fromDate != null)
            {
                requservms = requservms.Where(a => a.REQUESTEDDATE >= fromDate);
            }
            else if (toDate != null)
            {
                requservms = requservms.Where(a => a.REQUESTEDDATE < toDate);
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                requservms = requservms.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = requservms.Count();
            var data = requservms.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        //Detail
        // GET: Approve/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                ReqUser requser = db.ReqUsers.Where(u => u.ID == id).FirstOrDefault();
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
                    BRANCH = requser.VrBranch.name,
                    ALLOW_TRAN_COUNT = requser.ALLOW_TRAN_COUNT,
                    DAILY_MAX_AMT = requser.DAILY_MAX_AMT,
                    ISNEW = requser.ISNEW,
                    STATUS = requser.STATUS,
                    MAKER = requser.VrMaker.UserName,
                    REQUESTEDDATE = requser.REQUESTEDDATE,
                    REQUESTINFO = requser.REQUESTINFO,
                    CHECKER = (requser.VrChecker==null? null: requser.VrChecker.UserName),
                    CHECKEDDATE = requser.CHECKEDDATE,
                    CHECKERREASON = requser.CHECKERREASON,
                    CREATEDDATE= (user == null ? null : user.CREATEDDATE),
                    IsVIP = requser.IsVIP,
                    USER_TYPE = requser.USER_TYPE,
                };

                //getting accounts by CIFID   
                List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == requser.CIFID & a.ISNEW==requser.ISNEW & a.REQUSER_ID==requser.ID).ToList().ConvertAll(x => new AccountVM
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
                ViewBag.CreatedDate = (user == null ? null : user.CREATEDDATE);
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

        //Approve Retail
        [HttpPost]
        public ActionResult Approve(int id) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string pdffilename = null;
            try
            {
                string LoginPwd = "";
                string TranPwd = "";
                ReqUser ruser = db.ReqUsers.Find(id);
                //get Accs
                List<ReqAcc> raccs = db.ReqAccs.Where(ra => ra.CIFID == ruser.CIFID && ra.ISNEW == ruser.ISNEW && ra.REQUSER_ID==ruser.ID).ToList();//filter by ReqUserID
                Boolean isSave = false;
                //validate
                string error = CheckValidate(ruser, raccs);
                if (error != null)
                {
                    // return Json(new { msg = "Approve successfully!", filename = pdffilename }, JsonRequestBehavior.AllowGet);
                    return Json(new {msg = error }, JsonRequestBehavior.AllowGet);
                }
                //Register(save) if ISNEW is true
                if (ruser.ISNEW)
                {
                    string daily_max_amt = (ruser.IsVIP? ruser.DAILY_MAX_AMT.ToString() : GetDecimalRule("R005").ToString());
                    ReqChange reqChange = db.ReqChanges.Find(id);
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // try to save at TBL_USER first
                            isSave = SaveUser(ruser, ref LoginPwd, ref TranPwd, daily_max_amt);

                            // try to save Accounts
                            SaveAccounts(raccs);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            ruser.STATUS = 1;
                            ruser.CHECKER = User.Identity.GetUserId();
                            ruser.CHECKEDDATE = DateTime.Now;
                            //Checker Reason

                            db.Entry(ruser).State = EntityState.Modified;
                            db.SaveChanges();

                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            ViewData["Message"] = "Approve successfully!";
                        }
                        catch (Exception ex)
                        {
                            // roll back all database operations, if anything goes wrong
                            transaction.Rollback();
                            //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        }
                    }
                    if (isSave)
                    {
                        //generate pdf
                        string getuserid = User.Identity.GetUserId();
                        var tran_amt = db.Rules.Where(r => r.Code == "R014").FirstOrDefault();
                        string tran_max_amt = tran_amt.Value.ToString();
                        //daily_max_amt must be same with SaveUser function
                        pdffilename = GeneratePdf(ruser.USERNAME, ruser.FULLNAME, ruser.CIFID, ruser.ADDRESS, ruser.MOBILENO, ruser.EMAIL, tran_max_amt, daily_max_amt, LoginPwd, TranPwd,0);
                        //Sent Email
                        if (!String.IsNullOrEmpty(ruser.EMAIL) && IsEmailValid(ruser.EMAIL) && IsMailSent())
                        {
                            //Validate Email Allow
                            var tranretail = "Retail User Management";
                            var mailallow = EmailAllow(tranretail);
                            if (mailallow.Receiver_Email_Allow == true)
                            {
                                try
                                {
                                    UserController user = new UserController();

                                    //Get Data For Email
                                    General.SMTPSetting setting = user.GetEmailSetting();
                                    var template = from t in db.EmailTemplates
                                                   where t.Type == "Registration"
                                                   select t;
                                    var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                    var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                    Body = Body.Replace("[USERNAME]", (ruser.USERNAME.ToString()));
                                    Body = Body.Replace("[FULLNAME]", (ruser.FULLNAME.ToString()));
                                    //string Subject = "A bank Mobile Banking Information";
                                    //string Body = "Your Account has been successfully created. Please check the credential at Attached pdf.";
                                    bool sentmail = General.SendEmail(ruser.EMAIL, Subject, Body, pdffilename, 0, 0, setting);
                                    //log for email
                                    log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {ruser.USERNAME}.Email Address is {ruser.EMAIL}.");
                                }
                                catch (Exception ex)
                                {
                                    log.Error(getuserid, controllerName, actionName, ex);
                                }
                            }
                        }
                    }
                }
                else //Update if ISNEW is false
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        User user = db._Users.Where(u => u.DEL_STATUS == false && u.CIFID == ruser.CIFID).FirstOrDefault();
                        try
                        {                          
                            //to update user
                            isSave = UpdateUser(ruser);

                            RetailResponseInfo rri = new RetailResponseInfo();
                            rri = GetRetailResponse(user.CIFID);

                            List<AccountInfo> core_accs = rri.lAcctInfo;

                            //UserAndAccVM userandaccvm = new UserAndAccVM();
                            //userandaccvm.closedaccs = new List<AccountVM>();
                            // try to save Accounts
                            UpdateAccounts(raccs, ruser.CIFID, core_accs);
                             
                                // try to update Req Status to Approved
                                // status to Approved (1)
                                ruser.STATUS = 1;
                                ruser.CHECKER = User.Identity.GetUserId();
                                ruser.CHECKEDDATE = DateTime.Now;
                                //Checker Reason

                                db.Entry(ruser).State = EntityState.Modified;
                                db.SaveChanges();

                                // everything is fine - save all the data permanently
                                transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // roll back all database operations, if anything goes wrong
                            transaction.Rollback();
                            //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        }
                    }                 
                }
            }          
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            return Json(new { msg = "Approve successfully!", filename = pdffilename }, JsonRequestBehavior.AllowGet);
        }
       
        //Reject Retail
        [HttpPost]
        public ActionResult Reject(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                
                ReqUser ruser = db.ReqUsers.Find(id);
                if (ruser.STATUS == 0)  //reject only from request status
                {
                    //Status => 2 for reject
                    ruser.STATUS = 2;
                    ruser.CHECKER = User.Identity.GetUserId();
                    ruser.CHECKEDDATE = DateTime.Now;
                    //checker reason
                    ruser.CHECKERREASON = remark;
                    db.Entry(ruser).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Reject successfully.", $"Request id = {id}, CIFID = {ruser.CIFID}");
                    
                    return Json("Reject Successfully", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json("Reject Fail!", JsonRequestBehavior.AllowGet);
        }

        //Delete requested accounts if rejected
        //private void DeleteAccount(string CIFID, Boolean IsNew)
        //{
        //    try
        //    {
        //        List<ReqAcc> deleteAccounts = db.ReqAccs.Where(ra => ra.CIFID == CIFID & ra.ISNEW == IsNew).ToList();
        //        string strAccNos = "";
        //        foreach (ReqAcc racc in deleteAccounts)
        //        {
        //            strAccNos += racc.ACCOUNTNO + ",";
        //        }
        //        db.ReqAccs.RemoveRange(db.ReqAccs.Where(ra => ra.CIFID == CIFID & ra.ISNEW == IsNew));
        //        db.SaveChanges();
        //        log.Log(User.Identity.GetUserId(), controllerName, "DeleteAccount", "Deleting requested accounts", $"{deleteAccounts.Count()} accounts were deleted.{strAccNos.Remove(strAccNos.Length-1)}");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
                int IntRule = Convert.ToInt32(rule);
                if (IntRule == 1)
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

        //changes approve
        public ActionResult ApproveChange(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loggedinID = User.Identity.GetUserId();
            try
            {
                string pdffilename = "";
                string loginPwd = "";
                string tranPwd = "";
                //get ReqChange obj by id
                ReqChange reqChange = db.ReqChanges.Find(id);
                if (reqChange == null)
                    return Json("Approve Fail!", JsonRequestBehavior.AllowGet);

                //validate status
                if (reqChange.STATUS == 1) //approved
                    return Json("Already Approved!", JsonRequestBehavior.AllowGet);
                if (reqChange.STATUS == 2) //rejected
                    return Json("Already Rejected!", JsonRequestBehavior.AllowGet);
                //get change type from ReqChange
                byte type = reqChange.TYPE;
                //get retail user to approver
                User user = db._Users.Find(reqChange.USERID);
                if(user==null)
                    return Json("Invalid User!", JsonRequestBehavior.AllowGet);
                //transaction
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //change values by Type
                        UpdateValues(type, ref user);
                        //if reset, hash plain text pwds
                        if (type == 2)
                        {
                            string getuserid = User.Identity.GetUserId();
                            loginPwd = user.LOGINPASSWORD;
                            tranPwd = user.TRANSACTIONPASSWORD;
                            user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(loginPwd,user.USERNAME);
                            user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(tranPwd, user.USERNAME);
                            user.CHK_old_pwd_flag = false;
                            
                            var tran_amt = db.Rules.Where(r => r.Code == "R014").FirstOrDefault();
                            string tran_max_amt = tran_amt.Value.ToString();

                            //generate pdf
                            pdffilename = GeneratePdf(user.USERNAME, user.FULLNAME, user.CIFID, user.ADDRESS, user.MOBILENO, user.EMAIL, tran_max_amt, user.DAILY_MAX_AMT.ToString(), loginPwd, tranPwd, 1);
                            //Sent Email
                            if (!String.IsNullOrEmpty(user.EMAIL) && IsEmailValid(user.EMAIL) && IsMailSent())
                            {
                                //Validate Email Allow
                                var tranretail = "Retail User Management";
                                var mailallow = EmailAllow(tranretail);
                                if (mailallow.Receiver_Email_Allow == true)
                                {
                                    try
                                    {
                                        UserController usercontroller = new UserController();

                                        //Get Data For Email
                                        General.SMTPSetting setting = usercontroller.GetEmailSetting();
                                        var template = from t in db.EmailTemplates
                                                       where t.Type == "Reset"
                                                       select t;
                                        var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                        var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                        Body = Body.Replace("[USERNAME]", (user.USERNAME.ToString()));
                                        Body = Body.Replace("[FULLNAME]", (user.FULLNAME.ToString()));
                                        //string Subject = "A bank Mobile Banking Information";
                                        //string Body = "Your Account has been successfully resetted. Please check the credential at Attached pdf.";
                                        bool sentmail = General.SendEmail(user.EMAIL, Subject, Body, pdffilename, 1, 0, setting);
                                        //log for email
                                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {user.USERNAME}.Email Address is {user.EMAIL}.");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(getuserid, controllerName, actionName, ex);
                                    }
                                }
                            }
                        }
                        if (type == 7)//delete accounts
                        {
                            db.Accounts.RemoveRange(db.Accounts.Where(a => a.CIFID == user.CIFID));
                            db.SaveChanges();
                        }
                        //default fields for all types
                        user.UPDATEDATE = DateTime.Now;
                        user.UPDATEUSERID = loggedinID;

                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        //update status => 1
                        reqChange.STATUS = 1;
                        reqChange.CHECKER = loggedinID;
                        reqChange.CHECKEDDATE = DateTime.Now;
                        //reqChange.CHECKERREASON = "";

                        db.Entry(reqChange).State = EntityState.Modified;
                        db.SaveChanges();
                        string typename = "";
                        if (type == 2)
                        {
                            typename = "Reset Password ";
                        }
                        else if (type == 3)
                        {
                            typename = "Lock";
                        }
                        else if (type == 4)
                        {
                            typename = "Unlock";
                        }
                        else if (type == 5)
                        {
                            typename = "Tran Lock";
                        }
                        else if (type == 6)
                        {
                            typename = "Tran Unlock";
                        }
                        else if (type == 7)
                        {
                            typename = "Delete";
                        }
                        //save log
                        log.Log(loggedinID, controllerName, actionName, $"{typename} Approved Successfully.", $"Request id is {id}");

                        //commit
                        transaction.Commit();
                    }
                    catch (Exception)
                        {
                            // roll back all database operations, if anything goes wrong
                            transaction.Rollback();
                            //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        }

                }
                if (type == 2)
                {
                    return Json(new { msg = $"New Login Password is {loginPwd}\r\nNew Transaction Password is {tranPwd} ", filename = pdffilename }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 3)
                {
                    return Json(new { msg = "Lock successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 4)
                {
                    return Json(new { msg = "Unlock successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 5)
                {
                    return Json(new { msg = "Transaction Lock successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 6)
                {
                    return Json(new { msg = "Transaction Unlock successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 7)
                {
                    return Json(new { msg = "Delete successfully!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json("Approve Fail!", JsonRequestBehavior.AllowGet);
        }

        //changes reject
        public ActionResult RejectChange(int id , string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loggedinID = User.Identity.GetUserId();

            try
            {
                ReqChange reqChange = db.ReqChanges.Find(id);
                if (reqChange == null)
                    return Json("Approve Fail!", JsonRequestBehavior.AllowGet);

                //validate status
                if (reqChange.STATUS == 1) //approved
                    return Json("Already Approved!", JsonRequestBehavior.AllowGet);
                if (reqChange.STATUS == 2) //rejected
                    return Json("Already Rejected!", JsonRequestBehavior.AllowGet);

                //update status => 2 (reject)
                reqChange.STATUS = 2;
                reqChange.CHECKER = loggedinID;
                reqChange.CHECKEDDATE = DateTime.Now;
                reqChange.CHECKERREASON = remark;

                db.Entry(reqChange).State = EntityState.Modified;
                db.SaveChanges();
                
                //save log
                log.Log(loggedinID, controllerName, actionName, "Reject successfully.", $"Request id is {id}");

                return Json("Reject Successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(loggedinID, controllerName, actionName, ex);
            }
            return Json("Reject Fail!", JsonRequestBehavior.AllowGet);
        }

        //do Request
        #region do request
        private void UpdateValues(byte type, ref User user)
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
                int IntRule = Convert.ToInt32(rule.Value);
                int ruleValue = (rule == null ? 5 : IntRule);
                switch (type)
                {
                    //2 => for reset
                    case 2://save old pwds
                        string old1 = user.LOGIN_PRE_PWD_1;
                        string old2 = user.LOGIN_PRE_PWD_2;
                        string old3 = user.LOGIN_PRE_PWD_3;
                        General.UpdateOldPwds(user.LOGINPASSWORD, ref old1, ref old2, ref old3);
                        //update new values
                        user.LOGIN_PRE_PWD_1 = old1;
                        user.LOGIN_PRE_PWD_2 = old2;
                        user.LOGIN_PRE_PWD_3 = old3;

                        string Told1 = user.TRAN_PRE_PWD_1;
                        string Told2 = user.TRAN_PRE_PWD_2;
                        string Told3 = user.TRAN_PRE_PWD_3;
                        General.UpdateOldPwds(user.TRANSACTIONPASSWORD, ref Told1, ref Told2, ref Told3);
                        //update new values
                        user.TRAN_PRE_PWD_1 = Told1;
                        user.TRAN_PRE_PWD_2 = Told2;
                        user.TRAN_PRE_PWD_3 = Told3;

                        //
                        user.LOGINPASSWORD = GeneratePassword.Generate(8, true, true, false);
                        System.Threading.Thread.Sleep(100);
                        user.TRANSACTIONPASSWORD = GeneratePassword.Generate(8, true, true, false);
                        //Flag changes
                        user.FORCE_PWD_CHANGE_FLG = true;
                        //update fail counts
                        user.FAIL_PASSWORD_COUNT = 0;
                        user.FAIL_TRAN_PWD_COUNT = 0;
                        user.PWD_EXP_DATE = DateTime.Now.AddDays(GetRule("R007") ?? 90);//default 90
                        user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(GetRule("R007") ?? 90);//default 90

                        //disable fp 
                        DisableFingerprint(user.USERID);

                        //Kill Session
                        KillSession(user.USERID);

                        break;
                    //3 => lock
                    case 3:
                        user.ISLOCK_FLAG = true;
                        user.ISLOGIN_FLAG = false;      //force logout

                        //Kill Session
                        KillSession(user.USERID);

                        //Old incomplete logic
                        //update logoutdate in TBL_LOGIN
                        //string userid = user.USERID;
                        //List<Login> lstLogin = db.Logins.Where(l => l.UserID == userid && l.LogoutDate == null).ToList();
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

                        break;
                    //4 => unlock
                    case 4:
                        user.ISLOCK_FLAG = false;
                        //will reset both fail counts?
                        //reset fail counts
                        user.FAIL_PASSWORD_COUNT = 0;
                        user.FAIL_TRAN_PWD_COUNT = 0;

                        break;
                    //5=> Tran Lock
                    case 5:                        
                            user.FAIL_TRAN_PWD_COUNT = ruleValue;
                        break;
                    //6=> Tran Unlock
                    case 6:
                        user.FAIL_TRAN_PWD_COUNT = 0;
                        break;
                    case 7:                       
                        user.DEL_STATUS = true;
                        user.UPDATEDATE = DateTime.Now;
                        user.UPDATEUSERID = User.Identity.GetUserId();
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    default: break;
                }
            }
            catch (Exception)
            {

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
        //to save old pwd into old pwd 1,2,3
        //private void UpdateOldPwds(string oldpwd, ref string old1, ref string old2, ref string old3)
        //{
        //    old3 = old2;
        //    old2 = old1;
        //    old1 = oldpwd;
        //}

        #endregion

        private string CheckValidate(ReqUser ruser, List<ReqAcc> listraccs) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //checking Validation
                #region Validation
                //check current status
                //if already approved
                if(ruser.STATUS == 1)
                    return "Already approved.";
                //if already rejected
                if (ruser.STATUS == 2)
                    return "Already rejected.";
                RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();
                if (ruser.CIFID != "") 
                {
                    if (ruser.USER_TYPE == RetailType)
                    {
                        rri = GetRetailResponse(ruser.CIFID);
                    }
                    else if (ruser.USER_TYPE == CorporateType) 
                    {
                        cri = GetCorporateResponse(ruser.CIFID);
                    }
                }
                if (ruser.USER_TYPE==RetailType && rri.NRC == null) //if wrong CIFID
                {
                    return "Invalid CIFID";
                }
                else if (ruser.USER_TYPE == CorporateType && cri.Name == null) //if wrong CIFID
                {
                    return "Invalid CIFID";
                }
                //CIFID duplicate
                if (ruser.ISNEW && (db._Users.Where(u => u.CIFID == ruser.CIFID & u.DEL_STATUS == false).Count() > 0))
                {
                    return "Duplicate CIFID";
                }

                #region username
                //no need to check format (it is already checked)
                //username duplicate
                if ( ruser.ISNEW && (db._Users.Where(u => u.USERNAME.ToLower() == ruser.USERNAME.ToLower()).Count() > 0))
                {
                    return "Duplicate USERNAME";
                }
                #endregion
                #region VIP
                if (ruser.IsVIP)
                {
                    //allow tran count
                    if (ruser.ALLOW_TRAN_COUNT == null || (ruser.ALLOW_TRAN_COUNT <= GetRule("R003")))
                    {
                        return "Allowed Transaction Count must be greater than Rule's count.";
                    }
                    //allow tran count
                    if (ruser.DAILY_MAX_AMT == null || (ruser.DAILY_MAX_AMT <= GetDecimalRule("R005")))
                    {
                        return "Maximum Amount must be greater than Rule's count.";
                    }
                }
                #endregion
                //checking to choose at least one account
                if (listraccs.Count == 0)
                {
                    return "This request doesn't have any choosed account!";
                }
                #endregion
                return null;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return "Validation error";
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

        //saving TBL_USER
        private Boolean SaveUser(ReqUser ruser, ref string Loginpwd, ref string Tranpwd, string daily_max_amt)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //generate pwd
                Loginpwd = GeneratePassword.Generate(8, true, true, false);
                System.Threading.Thread.Sleep(100);
                Tranpwd = GeneratePassword.Generate(8, true, true, false);

                //create new user
                User new_user = new User();
                //core data
                new_user.NRC = ruser.NRC;
                new_user.FULLNAME = ruser.FULLNAME;
                new_user.MINOR = ruser.MINOR;
                new_user.GENDER = ruser.GENDER;
                //formdata
                new_user.CIFID = ruser.CIFID;
                new_user.USERNAME = ruser.USERNAME;
                new_user.MOBILENO = ruser.MOBILENO;
                new_user.EMAIL = ruser.EMAIL;
                new_user.ADDRESS = ruser.ADDRESS;
                //Check VIP
                new_user.IsVIP = ruser.IsVIP;
                if (ruser.IsVIP)
                {
                    //get form inputs
                    new_user.ALLOW_TRAN_COUNT = ruser.ALLOW_TRAN_COUNT; //getting rule value by rule code   "Transaction Count Limit"
                    new_user.DAILY_MAX_AMT = ruser.DAILY_MAX_AMT;   //Transaction Amount Limit
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
                new_user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(Loginpwd, new_user.USERNAME);
                new_user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(Tranpwd, new_user.USERNAME);

                //default values
                new_user.CREATEDDATE = DateTime.Now;
                new_user.CREATEDUSERID = User.Identity.GetUserId();
                new_user.USER_TYPE = ruser.USER_TYPE;//"Retail";
                new_user.FIRSTTIME_FLAG = true;
                //pwd exp date by rule
                int expDays = GetRule("R007") ?? 90; //default 90
                new_user.PWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                new_user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                new_user.ALLOW_TRAN_COUNT = ruser.ALLOW_TRAN_COUNT ?? GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                new_user.DAILY_MAX_AMT = Convert.ToDecimal(daily_max_amt);   //Transaction Amount Limit

                db._Users.Add(new_user);
                db.SaveChanges();

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"User ID is { new_user.USERNAME}"); 
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //update user
        private Boolean UpdateUser(ReqUser ruser)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                User user = db._Users.Where(u => u.CIFID == ruser.CIFID).FirstOrDefault();
                //RetailResponseInfo rri = GetRetailResponse(ruser.CIFID);
                RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();
                if (ruser.USER_TYPE == RetailType)
                {
                    rri = GetRetailResponse(ruser.CIFID);
                }
                else if (ruser.USER_TYPE == CorporateType)
                {
                    cri = GetCorporateResponse(ruser.CIFID);
                }
                //validate 


                //user.USERNAME = uservm.USERNAME; //not editable username
                user.MOBILENO = ruser.MOBILENO;
                user.EMAIL = ruser.EMAIL;
                user.ADDRESS = ruser.ADDRESS;
                if (ruser.USER_TYPE == RetailType)
                {
                    user.FULLNAME = rri.Name;
                    user.NRC = rri.NRC;
                    user.MINOR = rri.Minor;
                    user.GENDER = rri.Gender;
                }
                else if (ruser.USER_TYPE == CorporateType)
                {
                    user.FULLNAME = cri.Name;
                    user.NRC = cri.AccountOpenDate;
                }
                //user.ALLOW_TRAN_COUNT = ruser.ALLOW_TRAN_COUNT;
                //user.DAILY_MAX_AMT = ruser.DAILY_MAX_AMT;
                //Check VIP
                user.IsVIP = ruser.IsVIP;//Update VIP flag
                if (ruser.IsVIP)
                {
                    //get form inputs
                    user.ALLOW_TRAN_COUNT = ruser.ALLOW_TRAN_COUNT; //getting rule value by rule code   "Transaction Count Limit"
                    user.DAILY_MAX_AMT = ruser.DAILY_MAX_AMT;   //Transaction Amount Limit
                }
                else
                {
                    //get rule values for Normal User
                    user.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                    user.DAILY_MAX_AMT = GetRule("R005");   //Transaction Amount Limit
                }
                //default fields
                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2, $"{ruser.USERNAME} is updated.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex; 
            }
            //return false;
        }


        //saving accounts
        private void SaveAccounts(List<ReqAcc> raccs)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                foreach (ReqAcc racc in raccs)
                {
                    Account existingAcc = db.Accounts.Where(a => a.CIFID == racc.CIFID && a.ACCOUNTNO == racc.ACCOUNTNO).FirstOrDefault();
                    if (existingAcc == null)
                    {
                        Account acc = new Account();
                        //core data
                        acc.ACCOUNTNO = racc.ACCOUNTNO;
                        acc.ACC_TYPE = racc.ACC_TYPE;
                        acc.SCHM_CODE = racc.SCHM_CODE;
                        acc.ACC_DESC = racc.ACC_DESC;
                        acc.BRANCHCODE = racc.BRANCHCODE;
                        acc.CURRENCY = racc.CURRENCY;
                        //db data
                        //QR allow false if Acc type is  ODA, LAA, TDA
                        if (acc.ACC_TYPE == "ODA" || acc.ACC_TYPE == "LAA" || acc.ACC_TYPE == "TDA" /*|| acc.CURRENCY != "MMK"*/)
                        {
                            acc.QR_ALLOW = false;
                        }
                        else
                            acc.QR_ALLOW = racc.QR_ALLOW;
                        //default values
                        acc.Active = true;
                        acc.CIFID = racc.CIFID;
                        acc.CreatedDateTime = DateTime.Now;
                        acc.CreatedUserID = User.Identity.GetUserId();
                        acc.ACCT_CLOSE_FLAG = false;

                        db.Accounts.Add(acc);
                        db.SaveChanges();
                    }
                    else
                    {
                        existingAcc.ACC_TYPE = racc.ACC_TYPE;
                        existingAcc.SCHM_CODE = racc.SCHM_CODE;
                        existingAcc.ACC_DESC = racc.ACC_DESC;
                        existingAcc.BRANCHCODE = racc.BRANCHCODE;
                        existingAcc.CURRENCY = racc.CURRENCY;
                        //db data
                        //QR allow false if Acc type is  ODA, LAA, TDA
                        if (existingAcc.ACC_TYPE == "ODA" || existingAcc.ACC_TYPE == "LAA" || existingAcc.ACC_TYPE == "TDA" /*|| existingAcc.CURRENCY != "MMK"*/)
                        {
                            existingAcc.QR_ALLOW = false;
                        }
                        else
                            existingAcc.QR_ALLOW = racc.QR_ALLOW;
                        //default values
                        existingAcc.Active = true;
                        existingAcc.CIFID = racc.CIFID;
                        existingAcc.UpdatedDateTime = DateTime.Now;
                        existingAcc.UpdatedUserId = User.Identity.GetUserId();
                        existingAcc.ACCT_CLOSE_FLAG = false;

                        db.Entry(existingAcc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{raccs.Count} acounts saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }
        //update accounts
        private void UpdateAccounts(List<ReqAcc> raccs, string CIFID, List<AccountInfo> core_accs)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                int saveCount = 0;
                List<Account> accounts = db.Accounts.Where(a => a.CIFID == CIFID).ToList();
                //old comment //to set active in acc if accno exist in reqacc
                foreach (Account activeAcc in accounts)
                {
                    var active = raccs.Where(r => r.ACCOUNTNO == activeAcc.ACCOUNTNO).Count();
                    if (active == 0)//uncheck accounts,  if exist in db.Accounts and not exist in req_accs, make unactive
                    {
                        activeAcc.Active = false;
                        activeAcc.QR_ALLOW = false;

                        db.Entry(activeAcc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //////delete all old records  (old logic)               
                //int oldRecords = db.Accounts.Where(a => a.CIFID == CIFID).Count();
                //if (oldRecords > 0)
                //{
                //    string deleteData = $"{oldRecords} accounts deleted.";
                //    db.Accounts.RemoveRange(db.Accounts.Where(a => a.CIFID == CIFID));
                //    db.SaveChanges();
                //    //save log
                //    log.Info(User.Identity.GetUserId(), controllerName, actionName, 3, deleteData);
                //}
               
                foreach (var accvm in raccs)
                {
                    var core_acc = core_accs.Where(c => c.AccountNumber == accvm.ACCOUNTNO).FirstOrDefault();
                    if (core_acc == null)//invalid acc no   or  closed accs at core
                        continue;
                    Account acc = db.Accounts.Where(a =>a.CIFID==accvm.CIFID & a.ACCOUNTNO == accvm.ACCOUNTNO).FirstOrDefault();
                    if (acc == null)    //new accs
                    {
                        //foreach (Account account in accounts) //Account not exist in ReqAcc , Active = false
                        //{                            
                        //    if (!accvm.ACCOUNTNO.Contains(account.ACCOUNTNO)/*accvm.Active*/)//uncheck accounts will not be saved.
                        //        continue;
                            saveCount++;
                            acc = new Account();
                            //core data
                            acc.ACCOUNTNO = core_acc.AccountNumber;
                            acc.ACC_TYPE = core_acc.AccountType;
                            acc.ACC_DESC = core_acc.AccountTypeDesc;
                            acc.SCHM_CODE = core_acc.Schm_Code;
                            acc.BRANCHCODE = core_acc.BranchID;
                            acc.CURRENCY = core_acc.Currency;
                            //QR allow false if Acc type is  ODA, LAA, TDA ,not MMK
                            if (accvm.ACC_TYPE == "ODA" || accvm.ACC_TYPE == "LAA" || accvm.ACC_TYPE == "TDA"/* || accvm.CURRENCY != "MMK"*/)
                            {
                                acc.QR_ALLOW = false;
                            }
                            else
                                acc.QR_ALLOW = accvm.QR_ALLOW;
                            //default values                       
                            //all req_accs are active. we save only active accs.
                            acc.Active = true;
                            acc.CIFID = CIFID;
                            acc.CreatedDateTime = DateTime.Now;
                            acc.CreatedUserID = User.Identity.GetUserId();
                            acc.ACCT_CLOSE_FLAG = false;

                            db.Accounts.Add(acc);
                            db.SaveChanges();
                    }
                    else    //update
                    {
                        //var active = accounts.Where(a => a.CIFID == CIFID).FirstOrDefault();
                        
                        acc.ACCT_CLOSE_FLAG = false;//to reopen closed acc\ 

                        //all req_accs are active. we save only active accs.
                        acc.Active = true;
                        //QR allow false if Acc type is  ODA, LAA, TDA
                        if (!acc.Active || acc.ACC_TYPE == "ODA" || acc.ACC_TYPE == "LAA" || acc.ACC_TYPE == "TDA" /*|| accvm.CURRENCY != "MMK"*/)
                        {
                            acc.QR_ALLOW = false;
                        }
                        else
                        acc.QR_ALLOW = accvm.QR_ALLOW;
                        //default values
                        acc.UpdatedDateTime = DateTime.Now;
                        acc.UpdatedUserId = User.Identity.GetUserId();
                        db.Entry(acc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                string getuserid = User.Identity.GetUserId();
                if (raccs.Count > 0)
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
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }
        //Changes list
        public ActionResult Changes(FormCollection form)
        {
            string userid = User.Identity.GetUserId();
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                AdminUser admin = db.Users.Find(userid);
                //only same branch requests, only pending status(not approved/rejected), not own request
                //List<ReqChange> lstChanges = db.ReqChanges.Where(rc => ((admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.STATUS==0 && rc.MAKER != userid) || rc.CHECKER== userid).ToList();

                //bind User Type
                UserTypeDDL();

                //Create Status List
                ViewBag.cTYPEList = new List<SelectListItem>
                {
                    new SelectListItem{Text="Reset Password",Value="2"},
                    new SelectListItem{Text="Lock",Value="3"},
                    new SelectListItem{Text="Unlock",Value="4"},
                    new SelectListItem{Text="Tran Lock",Value="5"},
                    new SelectListItem{Text="Tran Unlock",Value="6"},
                    new SelectListItem{Text="Delete",Value="7"}
                };

                //Create Status List
                ViewBag.cSTATUSList = new List<SelectListItem>
                {
                    new SelectListItem{Text="Requested",Value="0"},
                    new SelectListItem{Text="Approved",Value="1"},
                    new SelectListItem{Text="Rejected",Value="2"},
                    new SelectListItem{Text="All",Value=null}
                };

                ViewBag.cUSERNAME = form["cUSERNAME"];
                ViewBag.cTYPE = form["cTYPE"];
                ViewBag.cMAKER = form["cMAKER"];                
                ViewBag.cSTATUS = form["cSTATUS"];
                //ViewBag.DateRange = form["DateRangefilter"];
                //ViewBag.ReqDate = form["Datefilter"];
                ViewBag.aBranch = form["aBranch"];

                //Get Branch Code and check is branch code of login user "Head Office" or not
                var code = admin.VrBranch.code;
                ViewBag.IsHeadOffice = (code == "9999");

                //assign initial value for "RequestedDate" and Status
                if (/*form["Datefilter"] == null &&*/ form["aStatus"] == null)
                {
                    form["aStatus"] = "0";
                    //form["Datefilter"] = DateTime.Now.ToString("MM/dd/yyyy");
                    ViewBag.aStatus = form["aStatus"];
                    //ViewBag.ReqDate = form["Datefilter"];

                }
                //assign initial value for UserType
                //if (/*form["Datefilter"] == null &&*/ form["ddlUserType"] == null)
                //{
                //    form["ddlUserType"] = "Retail";
                //    ViewBag.UserType = form["ddlUserType"];
                //    //ViewBag.ReqDate = form["Datefilter"];

                //}

                //branch data to bind branch dropdown list
                var lstBranch = db.Branches.Where(r => r.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> items = lstBranch.Select(a => new SelectListItem
                {
                    Value = a.id.ToString(),
                    Text = a.name

                });
                ViewBag.aBranchlist = items;

                //Chooser serch box
               // if (ViewBag.cUSERNAME != null && ViewBag.cUSERNAMe != "")
               //     lstChanges = lstChanges.Where(lc => lc.User.USERNAME.ToLower().Contains(form["cUSERNAME"].Trim().ToLower())).OrderByDescending(lc=>lc.REQUESTEDDATE).ToList();
               //if (ViewBag.cTYPE != null && ViewBag.cTYPE != "")
               //     lstChanges = lstChanges.Where(lc=>lc.TYPE.ToString()==ViewBag.cTYPE).OrderByDescending(lc => lc.REQUESTEDDATE).ToList();
               //if (ViewBag.cMAKER != null && ViewBag.cMAKER != "")
               //     lstChanges = lstChanges.Where(lc=>lc.VrReqMaker.FullName.ToLower().Contains(form["cMAKER"].Trim().ToLower())).OrderByDescending(lc => lc.REQUESTEDDATE).ToList();
               //if (ViewBag.cSTATUS != null && ViewBag.cSTATUS != "")
               //     lstChanges = lstChanges.Where(lc=>lc.STATUS.ToString()==ViewBag.cSTATUS).OrderByDescending(lc => lc.REQUESTEDDATE).ToList();
               // if (form["Datefilter"] != null && form["Datefilter"] != "")
               //     lstChanges = lstChanges.Where(r => Convert.ToDateTime(r.REQUESTEDDATE).ToString("MM/dd/yyyy") == ViewBag.ReqDate.ToString()).OrderByDescending(lc => lc.REQUESTEDDATE).ToList();
               // if (form["aBranch"] != null && form["aBranch"] != "")
               //     lstChanges = lstChanges.Where(u => u.BranchID.ToString().Equals(form["aBranch"])).OrderByDescending(u => u.REQUESTEDDATE).ToList();


                return View();
            }
            catch (Exception ex)
            {
                log.Error(userid, controllerName, actionName, ex);
                throw ex;
            }
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
            var cUSERNAME = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var cTYPE = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            //var cMAKER = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var cSTATUS = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);
            var aBranch = "";
            if (admin.VrBranch.code == "9999")
            {
                aBranch = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            }
            var usertype = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();

            //var RequestedDate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            DateTime? fromDate = null, toDate = null;
            //if (!String.IsNullOrEmpty(RequestedDate))
            //{
            //    string[] strdate = RequestedDate.Split('~');
            //    if (!string.IsNullOrEmpty(strdate[0].Trim()))
            //    {
            //       fromDate = DateTime.ParseExact(strdate[0].Trim().ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            //    }
            //    if (!string.IsNullOrEmpty(strdate[1].Trim()))
            //    {
            //        toDate = DateTime.ParseExact(strdate[1].Trim().ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
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
            //var Datefilter = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            //DateTime? fromDate = null, toDate = null;
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
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var reqChanges = (from s in db.ReqChanges.Where(rc => ((admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.STATUS == 0 && rc.MAKER != userid) || rc.CHECKER == userid)
            //                  select s);//Old logic, branch filter and only own-checked requests (approved / rejected) )
            var reqChanges = (from s in db.ReqChanges.Where(rc => (admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.MAKER != userid & rc.User.USER_TYPE=="Retail")
                              select s);//only filter by Branch and not own requests

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            ViewBag.IsHeadOffice = (code == "9999");

            if (!string.IsNullOrEmpty(cUSERNAME))
            {
                reqChanges = reqChanges.Where(a => a.User.USERNAME.Contains(cUSERNAME.Trim()));

            }
            if (!string.IsNullOrEmpty(cTYPE))
            {
                reqChanges = reqChanges.Where(a => a.TYPE.ToString() == cTYPE);

            }
            //if (!string.IsNullOrEmpty(cMAKER))
            //{
            //    reqChanges = reqChanges.Where(a => a.VrReqMaker.FullName.Contains(cMAKER.Trim()));

            //}
            if (!string.IsNullOrEmpty(cSTATUS))
            {
                reqChanges = reqChanges.Where(a => a.STATUS.ToString() == cSTATUS);

            }
            if (admin.VrBranch.code == "9999")
            {
                if (!string.IsNullOrEmpty(aBranch))
                {
                    reqChanges = reqChanges.Where(a => a.VrBranch.id.ToString() == aBranch);

                }
            }

            if (!string.IsNullOrEmpty(usertype))
            {
                reqChanges = reqChanges.Where(a => a.User.USER_TYPE == usertype);

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

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                reqChanges = reqChanges.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = reqChanges.Count();
            var data = reqChanges.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        //Changes Detail
        public ActionResult ChangesDetails(int id)
        {
            try
            {
                ReqChange reqChange = db.ReqChanges.Where(u => u.ID == id).FirstOrDefault();
                if (reqChange == null)
                {
                    return HttpNotFound();
                }

                return View(reqChange);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
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

        //generate pdf      //if status 0 => register, 1 => reset
        private string GeneratePdf(string USERNAME, string FULLNAME, string CIFID, string ADDRESS, string MOBILENO, string EMAIL, string tran_max_amt, string daily_max_amt, string LoginPwd, string txnPwd, int Status)
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

                if (Status == 1)//if txnPwd = null, resetting login pwd
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
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //draw bg
        void DrawImage(XGraphics gfx)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/new_pdf_bg.jpg"));

            gfx.DrawImage(image, 50, 40, 500, 650);
        }

        //draw title image.
        void DrawTitleImage(XGraphics gfxTitle)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/pdftitle.jpg"));
            gfxTitle.DrawImage(image, 50, 0, 170, 60);
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

        #region Corporate
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