using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using ABankAdmin.Core.Utils;
using System.Globalization;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;
using System.IO;
using System.Threading.Tasks;
using ABankAdmin.Utils;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class CorporateUserController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CorporateUserController";
        string UserType = "Corporate";
        EventLogController log = new EventLogController();
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        // GET: Approve
        public ActionResult Index()
        {
            //show only requests of his/her Branch
            //get BranchID from Logged In User
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            ViewBag.IsHeadOffice = (code == "9999");

            //Create select list
            ViewBag.aStatusList = General.GetStatus();

            //Create Types list
            ViewBag.aTypeList = General.GetUserType();

            //branch data to bind branch dropdown list
            var lstBranch = db.Branches.Where(r => r.DEL_FLAG == false).ToList();
            IEnumerable<SelectListItem> items = lstBranch.Select(a => new SelectListItem
            {
                Value = a.name,
                Text = a.name

            });
            ViewBag.aBranchlist = items;

            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];

            //carry pdf filename to download or null
            //if (TempData["corporateid"] != null && TempData["corporateid"].ToString() != "")
            //    ViewBag.corporateid = TempData["corporateid"];

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
            var aCompany = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var aUSERNAME = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var aType = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var aStatus = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var aCorporate = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);
            var aBranch = "";
            if (admin.VrBranch.code == "9999")
            {
                aBranch = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
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

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            IEnumerable<C_ReqUserVM> c_requservms;
            //only pending status(not approved/rejected), not own request
            //if admin is head office's admin, show requests of all branches
            //if branch admin, only same branch requests
            //int c = db.C_Req_Users.Count();
            //C_Req_User cru = db.C_Req_Users.FirstOrDefault();
            c_requservms = db.C_Req_Users.Where(u => (admin.BranchID == u.BranchID || admin.VrBranch.code == "9999") && u.MAKER != userid).ToList().ConvertAll(ru => new C_ReqUserVM
            {
                ID = ru.ID,
                MOBILENO = ru.MOBILENO,
                USERNAME = ru.USERNAME,
                FULLNAME = ru.FULLNAME,
                EMAIL = ru.EMAIL,
                ADDRESS = ru.ADDRESS,
                BRANCH = (ru.VrBranch==null? "" :ru.VrBranch.name),
                ISNEW = ru.ISNEW,
                STATUS = ru.STATUS,
                REQUESTEDDATE = ru.REQUESTEDDATE ?? DateTime.Now,
                CHECKERREASON = ru.CHECKERREASON,
                CompanyName = (ru.VrCorporate == null ? "" : ru.VrCorporate.COMPANY_NAME),
                Position = (ru.VrPosition == null ? "" : ru.VrPosition.NAME),
                Department = (ru.VrDepartment == null ? "" : ru.VrDepartment.NAME),
                CorporateId = (ru.VrCorporate == null ? "" : ru.VrCorporate.CORPORATEID)
            });

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            ViewBag.IsHeadOffice = (code == "9999");


            if (!string.IsNullOrEmpty(aCompany))
            {
                c_requservms = c_requservms.Where(a => a.CompanyName.ToLower().Trim().Contains(aCompany.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(aCorporate))
            {
                c_requservms = c_requservms.Where(a => a.CorporateId.ToLower().Trim().Contains(aCorporate.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(aUSERNAME))
            {
                c_requservms = c_requservms.Where(a => a.USERNAME.ToLower().Trim().Contains(aUSERNAME.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(aType))
            {
                c_requservms = c_requservms.Where(a => a.ISNEW.ToString() == aType);
            }
            if (!string.IsNullOrEmpty(aStatus))
            {
                c_requservms = c_requservms.Where(a => a.STATUS.ToString() == aStatus);

            }
            if (admin.VrBranch.code == "9999")
            {
                if (!string.IsNullOrEmpty(aBranch))
                {
                    c_requservms = c_requservms.Where(a => a.BRANCH.ToString() == aBranch);

                }
            }

            if (fromDate != null & toDate != null)
            {
                c_requservms = c_requservms.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate);
            }
            else if (fromDate != null)
            {
                c_requservms = c_requservms.Where(a => a.REQUESTEDDATE >= fromDate);
            }
            else if (toDate != null)
            {
                c_requservms = c_requservms.Where(a => a.REQUESTEDDATE < toDate);
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                c_requservms = c_requservms.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = c_requservms.Count();
            var data = c_requservms.Skip(skip).Take(pageSize).ToList();

            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// DetailCorporateUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DetailsCorporateUser(int id)
        {
            //title
            ViewBag.PageTitle = "Details";

            //user id
            ViewBag.CopRegId = id;

            //status id 3 for detail
            ViewBag.CopRegStatus = CorporateUserRequest.DETAIREQLCORPORATEUSER;

            //get departments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View();
            //return View(nameof(RequestCorporateUser));
        }

        /// <summary>
        /// RequestCorporateUser
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RequestCorporateUser()
        {
            //get transaction info
            ViewBag.MakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.CheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.MakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.CheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            //get dapartments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View();
        }

        //Detail may remove       // GET: Approve/Details/5
        //public ActionResult Details(int id)
        //{
        //    try
        //    {
        //        C_Req_User c_requser = db.C_Req_Users.Where(u => u.ID == id).FirstOrDefault();
        //        User user = db._Users.Where(u => u.CIFID == c_requser.CIFID).FirstOrDefault();
        //        if (requser == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        ReqUserVM requservm = new ReqUserVM
        //        {
        //            ID = requser.ID,
        //            MOBILENO = requser.MOBILENO,
        //            USERNAME = requser.USERNAME,
        //            FULLNAME = requser.FULLNAME,
        //            NRC = requser.NRC,
        //            EMAIL = requser.EMAIL,
        //            ADDRESS = requser.ADDRESS,
        //            CIFID = requser.CIFID,
        //            MINOR = requser.MINOR,
        //            GENDER = requser.GENDER,
        //            BRANCH = requser.VrBranch.name,
        //            ALLOW_TRAN_COUNT = requser.ALLOW_TRAN_COUNT,
        //            DAILY_MAX_AMT = requser.DAILY_MAX_AMT,
        //            ISNEW = requser.ISNEW,
        //            STATUS = requser.STATUS,
        //            MAKER = requser.VrMaker.UserName,
        //            REQUESTEDDATE = requser.REQUESTEDDATE,
        //            REQUESTINFO = requser.REQUESTINFO,
        //            CHECKER = (requser.VrChecker == null ? null : requser.VrChecker.UserName),
        //            CHECKEDDATE = requser.CHECKEDDATE,
        //            CHECKERREASON = requser.CHECKERREASON,
        //            CREATEDDATE = (user == null ? null : user.CREATEDDATE),
        //            IsVIP = requser.IsVIP,
        //            USER_TYPE = requser.USER_TYPE,
        //        };

        //        //getting accounts by CIFID   
        //        List<AccountVM> accvms = db.ReqAccs.Where(a => a.CIFID == requser.CIFID & a.ISNEW == requser.ISNEW & a.REQUSER_ID == requser.ID).ToList().ConvertAll(x => new AccountVM
        //        {
        //            ID = x.ID,
        //            ACCOUNTNO = x.ACCOUNTNO,
        //            ACC_TYPE = x.ACC_TYPE,
        //            SCHM_CODE = x.SCHM_CODE,
        //            BRANCHCODE = x.BRANCHCODE,
        //            CURRENCY = x.CURRENCY,
        //            QR_ALLOW = x.QR_ALLOW,
        //            Active = true

        //        });
        //        ViewBag.CreatedDate = (user == null ? null : user.CREATEDDATE);
        //        ReqUserAndAccVM userandaccvm = new ReqUserAndAccVM();
        //        userandaccvm.reqUservm = requservm;
        //        userandaccvm.accountvms = accvms;
        //        return View(userandaccvm);
        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        //Approve Retail

        //Validate Rule
        private bool IsAdminRole(List<C_Req_UserInRole> r_role)
        {
            try
            {
                int roleId = r_role.Select(r => r.RoleId).FirstOrDefault();
                var rrole = db.C_Role.Where(c => c.Id == roleId).FirstOrDefault();
                if (rrole.isAdmin == true)
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

        [HttpPost]
        public ActionResult Approve(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string pdffilename = null;
            string corid=null;
            string corporateid = null;
            int type = 0;
            try
            {
                string LoginPwd = "";
                string TranPwd = "";
                C_Req_User c_ruser = db.C_Req_Users.Find(id);
                var Corporate = db.C_Corporates.Find(c_ruser.CORPORATEID);
                //get Role
                List<C_Req_UserInRole> rroles = db.C_Req_UserInRoles.Where(r => r.ReqUserID == id).ToList();                 

                //get Menu
                List<C_Req_MenuPermission> rmenus = db.C_Req_MenuPermissions.Where(r => r.ReqUserID == id).ToList();

                //get Tran Limit
                List<C_Req_UserTranLimit> rlimits = db.C_Req_UserTranLimits.Where(r => r.ReqUserID == id).ToList();

                //get Accounts
                List<C_Req_UserInAccount> raccs = db.C_Req_UserInAccounts.Where(r => r.ReqUserID == id).ToList();

                Boolean isSave = false;

                #region Validation
                //User Info
                string error = CheckUserInfo(c_ruser);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }

                //Role Info
                error = CheckRoleInfo(rroles);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }

                //Menu Info
                error = CheckMenuInfo(rmenus);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }

                //Account Info                
                error = CheckAccountInfo(raccs, rroles);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                //Register(save) if ISNEW is true
                if (c_ruser.ISNEW)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // try to save at TBL_USER first
                            string new_userid = SaveUser(c_ruser, ref LoginPwd, ref TranPwd);
                            isSave = true;

                            // save Role
                            SaveRoles(new_userid, rroles,true);

                            // save Menus
                            SaveMenus(new_userid, rmenus, true);

                            // save tran limit
                            SaveTranLimits(new_userid, rlimits, true);

                            // try to save Accounts
                            SaveAccounts(new_userid, raccs, true);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            c_ruser.STATUS = 1;
                            c_ruser.CHECKER = User.Identity.GetUserId();
                            c_ruser.CHECKEDDATE = DateTime.Now;
                            //Checker Reason

                            db.Entry(c_ruser).State = EntityState.Modified;
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

                        //daily corporate tran amount
                        var tran_amt = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
                        string daily_cortran_amt = tran_amt.Value.ToString();

                        //corporate tran limit amt
                        var cor_limit = db.C_CorporateTranRules.Where(c => c.CorporateId == c_ruser.CORPORATEID).FirstOrDefault();
                        string cor_tran_limit = cor_limit.Value.ToString();

                        //one time corporate transaction max amount
                        var cor_amt = db.Rules.Where(r => r.Code == "C002").FirstOrDefault();
                        string cor_tran_max_amt = cor_amt.Value.ToString();

                        //get role list
                        var role = db.C_Req_UserInRoles.Where(r => r.Id > 0).ToList();
                        //maker tran limit
                        string maker_tran_limit = "";
                        var maker = role.Where(r => r.ReqUserID == c_ruser.ID && r.VrRole.Name == "Maker").FirstOrDefault();
                        if (maker != null)
                        {
                            var maker_tran_amt = db.C_Req_UserTranLimits.Where(r => r.ReqUserID == id && r.CorporateId == c_ruser.CORPORATEID && r.RuleCode == "R041").FirstOrDefault();
                            maker_tran_limit = maker_tran_amt.Value.ToString();
                        }
                        else { maker_tran_limit = "0"; }

                        //approver tran limit
                        string approver_tran_limit = "";
                        var approver = role.Where(r => r.ReqUserID == c_ruser.ID && r.VrRole.Name == "Approver").FirstOrDefault();
                        if (approver != null)
                        {
                            var approver_tran_amt = db.C_Req_UserTranLimits.Where(r => r.ReqUserID == c_ruser.ID && r.CorporateId == c_ruser.CORPORATEID && r.RuleCode == "R042").FirstOrDefault();
                            approver_tran_limit = approver_tran_amt.Value.ToString();
                        }
                        else { approver_tran_limit = "0"; }

                        //get user role
                        //var role = db.C_Req_UserInRoles.Where(r => r.ReqUserID > 0).ToList();
                        var rolename = role.GroupBy(cc => cc.ReqUserID).Select(dd => new { ReqUserID = dd.Key, Role = string.Join(",", dd.Select(ee => ee.VrRole.Name).ToList()) });
                        string rolenames = rolename.Where(rn => rn.ReqUserID.ToString() == c_ruser.ID.ToString()).Select(s => s.Role).FirstOrDefault();

                        int rolestatus = 0;
                        if (rolenames == "AdminMaker" || rolenames == "AdminApprover" || rolenames == "AdminMaker,AdminApprover") { rolestatus = 0; }
                        if (rolenames == "Viewer") { rolestatus = 1; }
                        if (rolenames == "Maker,Viewer,Approver" || rolenames == "Maker,Approver") { rolestatus = 2; }
                        if (rolenames == "Maker,Viewer" || rolenames == "Maker") { rolestatus = 3; }
                        if (rolenames == "Approver,Viewer" || rolenames == "Approver") { rolestatus = 4; }

                        //get corporate id
                        corporateid = c_ruser.VrCorporate.CORPORATEID + "/";
                        //generate pdf
                        pdffilename = corporateid + GeneratePdf(c_ruser.VrCorporate.CIFID, Corporate.COMPANY_NAME, c_ruser.USERNAME, rolenames, c_ruser.FULLNAME, Corporate.CORPORATEID, c_ruser.ADDRESS, c_ruser.MOBILENO, c_ruser.EMAIL, daily_cortran_amt, cor_tran_limit, cor_tran_max_amt, maker_tran_limit, approver_tran_limit, LoginPwd, TranPwd, 0, rolestatus);
                        //Sent Email
                        if (!String.IsNullOrEmpty(c_ruser.EMAIL) && IsEmailValid(c_ruser.EMAIL) && IsMailSent())
                        {
                            //Validate Email Allow
                            var tranCor = "Corporate User Management";
                            var mailallow = EmailAllow(tranCor);
                            if (mailallow.Receiver_Email_Allow == true)
                            {
                                try
                                {
                                    CorporateUserController cuser = new CorporateUserController();

                                    //Get Data For Email
                                    General.SMTPSetting setting = cuser.GetEmailSetting();
                                    var template = from t in db.EmailTemplates
                                                   where t.Type == "Registration"
                                                   select t;
                                    var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                    var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                    Body = Body.Replace("[USERNAME]", (c_ruser.USERNAME.ToString()));
                                    Body = Body.Replace("[FULLNAME]", (c_ruser.FULLNAME.ToString()));
                                    //string Subject = "A bank Mobile Banking Information";
                                    //string Body = "Your Account has been successfully created. Please check the credential at Attached pdf.";
                                    bool sentmail = General.SendEmail(c_ruser.EMAIL, Subject, Body, pdffilename, 0, 1, setting);
                                    //log for email
                                    log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"User ID is {c_ruser.USERNAME}.Email Address is {c_ruser.EMAIL}.");
                                }
                                catch (Exception ex)
                                {
                                    log.Error(getuserid, controllerName, actionName, ex);
                                }
                            }
                        }
                        TempData["corporateid"] = corid;
                        type = 1; // 1 for corporate user register
                    }
                }
                //Update
                else //Update if ISNEW is false
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // try to update the existing user at TBL_USER
                            string new_userid = UpdateUser(c_ruser);

                            // save Role
                            SaveRoles(new_userid, rroles,false);

                            // save Menus
                            SaveMenus(new_userid, rmenus, false);

                            // save tran limit
                            SaveTranLimits(new_userid, rlimits, false);

                            // try to save Accounts
                            SaveAccounts(new_userid, raccs, false);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            c_ruser.STATUS = 1;
                            c_ruser.CHECKER = User.Identity.GetUserId();
                            c_ruser.CHECKEDDATE = DateTime.Now;
                            //Checker Reason

                            db.Entry(c_ruser).State = EntityState.Modified;
                            db.SaveChanges();

                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            ViewData["Message"] = "Approve successfully!";
                            type = 2; // 2 for corporate user update data
                        }
                        catch (Exception ex)
                        {
                            // roll back all database operations, if anything goes wrong
                            transaction.Rollback();
                            //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        }
                    }
                }
                if (type == 1)
                {
                    return Json(new { msg = "Approve user register successfully!", filename = pdffilename }, JsonRequestBehavior.AllowGet);
                }
                else if (type == 2)
                {
                    return Json(new { msg = "Approve user data update successfully!", filename = pdffilename }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            return Json(new { msg = "Approve Fail!" }, JsonRequestBehavior.AllowGet);
        }

        //Reject Corporate User
        [HttpPost]
        public ActionResult Reject(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {

                C_Req_User c_ruser = db.C_Req_Users.Find(id);
                if (c_ruser.STATUS == 0)  //reject only from request status
                {
                    //Status => 2 for reject
                    c_ruser.STATUS = 2;
                    c_ruser.CHECKER = User.Identity.GetUserId();
                    c_ruser.CHECKEDDATE = DateTime.Now;
                    //checker reason
                    c_ruser.CHECKERREASON = remark;
                    db.Entry(c_ruser).State = EntityState.Modified;
                    db.SaveChanges();

                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Reject successfully.", $"Request id = {id}, UserName = {c_ruser.USERNAME}");

                    return Json("Reject Successfully", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json("Reject Fail!", JsonRequestBehavior.AllowGet);
        }

        #region Validation
        private string CheckUserInfo(C_Req_User c_ruser)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //check current status
                //if already approved
                if (c_ruser.STATUS == 1)
                    return "Already approved.";
                //if already rejected
                if (c_ruser.STATUS == 2)
                    return "Already rejected.";

                CorporateResponseInfo cri = new CorporateResponseInfo();
                C_Corporate cor = db.C_Corporates.Find(c_ruser.CORPORATEID);
                if (cor != null)
                {
                    cri = GetCorporateResponse(cor.CIFID);
                }

                #region username
                //no need to check format (it is already checked)
                //username duplicate
                if (c_ruser.ISNEW && (db._Users.Where(u => u.USERNAME.ToLower() == c_ruser.USERNAME.ToLower() && u.USER_TYPE == "Corporate" && u.CORPORATEID == c_ruser.CORPORATEID).Count() > 0))
                {
                    return "Duplicate USERNAME";
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

        private string CheckRoleInfo(List<C_Req_UserInRole> c_roles)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (c_roles.Count == 0)
                    return "Please choose at least one role!";
                return null;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return "Validation error";
            }
        }

        private string CheckMenuInfo(List<C_Req_MenuPermission> c_menus)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (c_menus.Count == 0)
                    return "Please choose at least one menu!";
                return null;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return "Validation error";
            }
        }

        private string CheckTranInfo(List<C_Req_UserTranLimit> c_tranlimits)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (c_tranlimits.Count == 0)
                    return "Please set the transaction limit!";
                return null;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return "Validation error";
            }
        }

        private string CheckAccountInfo(List<C_Req_UserInAccount> c_accs,List<C_Req_UserInRole> r_role)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get Role For Admin
                if (IsAdminRole(r_role) == false)
                {
                    if (c_accs.Count == 0)
                        return "Please choose at least one account!";
                }     
                return null;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return "Validation error";
            }
        }

        #endregion

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
                var rule = db.Rules.Where(r => r.Code == "C006").Select(r => r.Value).FirstOrDefault();
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
                string corporateid = "";
                //get ReqChange obj by id
                C_Req_Changes reqChange = db.C_Req_Changes.Find(id);
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
                var Corporate = db.C_Corporates.Find(user.CORPORATEID);
                if (user == null)
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

                            //daily corporate tran amount
                            var tran_amt = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
                            string daily_cortran_amt = tran_amt.Value.ToString();

                            //corporate tran limit amt
                            var cor_limit = db.C_CorporateTranRules.Where(c => c.CorporateId == user.CORPORATEID).FirstOrDefault();
                            string cor_tran_limit = cor_limit.Value.ToString();

                            //one time corporate transaction max amount
                            var cor_amt = db.Rules.Where(r => r.Code == "C002").FirstOrDefault();
                            string cor_tran_max_amt = cor_amt.Value.ToString();

                            //get role list
                            var role = db.C_UserInRoles.Where(r => r.Id > 0).ToList();
                            //maker tran limit
                            string maker_tran_limit = "";
                            var maker = role.Where(r => r.UserId == user.USERID && r.VrCRole.Name == "Maker").FirstOrDefault();
                            if (maker != null)
                            {
                                var maker_tran_amt = db.C_UserTranLimits.Where(r => r.UserId == user.USERID && r.CorporateId == user.CORPORATEID && r.RuleCode == "R041").FirstOrDefault();
                                maker_tran_limit = maker_tran_amt.Value.ToString();
                            }
                            else { maker_tran_limit = "0"; }

                            //approver tran limit
                            string approver_tran_limit = "";
                            var approver = role.Where(r => r.UserId == user.USERID && r.VrCRole.Name == "Approver").FirstOrDefault();
                            if (approver != null)
                            {
                                var approver_tran_amt = db.C_UserTranLimits.Where(r => r.UserId == user.USERID && r.CorporateId == user.CORPORATEID && r.RuleCode == "R042").FirstOrDefault();
                                approver_tran_limit = approver_tran_amt.Value.ToString();
                            }
                            else { approver_tran_limit = "0"; }

                            //get corporate id
                            corporateid = reqChange.VrCorporate.CORPORATEID + "/";

                            //get user role
                            var rolename = role.GroupBy(cc => cc.UserId).Select(dd => new { UserId = dd.Key, Role = string.Join(",", dd.Select(xx => xx.VrCRole.Name).ToList()) });
                            string rolenames = rolename.Where(rn => rn.UserId == user.USERID).Select(uu => uu.Role).FirstOrDefault();

                            int rolestatus = 0;
                            if (rolenames == "AdminMaker" || rolenames == "AdminApprover" || rolenames == "AdminMaker,AdminApprover") { rolestatus = 0; }
                            if (rolenames == "Viewer") { rolestatus = 1; }
                            if (rolenames == "Maker,Viewer,Approver" || rolenames == "Maker,Approver") { rolestatus = 2; }
                            if (rolenames == "Maker,Viewer" || rolenames == "Maker") { rolestatus = 3; }
                            if (rolenames == "Viewer,Approver" || rolenames == "Approver") { rolestatus = 4; }

                            //generate pdf
                            pdffilename = corporateid + GeneratePdf(Corporate.CIFID, Corporate.COMPANY_NAME, user.USERNAME, rolenames, user.FULLNAME, Corporate.CORPORATEID, user.ADDRESS, user.MOBILENO, user.EMAIL, daily_cortran_amt, cor_tran_limit, cor_tran_max_amt, maker_tran_limit, approver_tran_limit, loginPwd, tranPwd, 1, rolestatus);
                            //Sent Email
                            if (!String.IsNullOrEmpty(user.EMAIL) && IsEmailValid(user.EMAIL) && IsMailSent())
                            {  
                                //Validate Email Allow
                                var tranCor = "Corporate User Management";
                                var mailallow = EmailAllow(tranCor);
                                if (mailallow.Receiver_Email_Allow == true)
                                {
                                    try
                                    {
                                        CorporateUserController cusercontroller = new CorporateUserController();

                                        //Get Data For Email
                                        General.SMTPSetting setting = cusercontroller.GetEmailSetting();
                                        var template = from t in db.EmailTemplates
                                                       where t.Type == "Reset"
                                                       select t;
                                        var Subject = template.Select(s => s.Subject).FirstOrDefault().ToString();
                                        var Body = template.Select(b => b.Body).FirstOrDefault().ToString();
                                        Body = Body.Replace("[USERNAME]", (user.USERNAME.ToString()));
                                        Body = Body.Replace("[FULLNAME]", (user.FULLNAME.ToString()));
                                        //string Subject = "A bank Mobile Banking Information";
                                        //string Body = "Your Account has been successfully resetted. Please check the credential at Attached pdf.";
                                        bool sentmail = General.SendEmail(user.EMAIL, Subject, Body, pdffilename, 1, 1, setting);
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
                    return Json(new { msg = "Reset password successfully!", filename = pdffilename }, JsonRequestBehavior.AllowGet);
                    //return Json(new { msg = $"New Login Password is {loginPwd}\r\nNew Transaction Password is {tranPwd} ", filename = pdffilename }, JsonRequestBehavior.AllowGet);
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
        public ActionResult RejectChange(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loggedinID = User.Identity.GetUserId();

            try
            {
                C_Req_Changes reqChange = db.C_Req_Changes.Find(id);
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

        //To kill Session
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

        #endregion

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

        #region User Info
        //saving TBL_USER
        private string SaveUser(C_Req_User c_ruser, ref string Loginpwd, ref string Tranpwd)
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
                new_user.NRC = "-"/*c_ruser.NRC*/;
                new_user.FULLNAME = c_ruser.FULLNAME;
                //formdata
                //new_user.CIFID = "";/*c_ruser.CIFID*/;
                new_user.CIFID = c_ruser.VrCorporate.CIFID;
                new_user.USERNAME = c_ruser.USERNAME;
                new_user.MOBILENO = c_ruser.MOBILENO;
                new_user.EMAIL = c_ruser.EMAIL;
                new_user.ADDRESS = c_ruser.ADDRESS;

                new_user.CORPORATEID = c_ruser.CORPORATEID;
                new_user.DepartmentId = c_ruser.DepartmentID;
                new_user.PositionId = c_ruser.PositionID;
                
                //guid for userid
                new_user.USERID = Guid.NewGuid().ToString();
                //generate pwd
                new_user.LOGINPASSWORD = PasswordHash.SHA256HexHashString(Loginpwd,new_user.USERNAME);
                new_user.TRANSACTIONPASSWORD = PasswordHash.SHA256HexHashString(Tranpwd, new_user.USERNAME);

                //default values
                new_user.CREATEDDATE = DateTime.Now;
                new_user.CREATEDUSERID = User.Identity.GetUserId();
                new_user.USER_TYPE = UserType;
                new_user.FIRSTTIME_FLAG = true;
                //pwd exp date by rule
                int expDays = GetRule("R007") ?? 90; //default 90
                new_user.PWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                new_user.TRANPWD_EXP_DATE = DateTime.Now.AddDays(expDays);
                new_user.ALLOW_TRAN_COUNT = GetRule("R003"); //getting rule value by rule code   "Transaction Count Limit"
                //new_user.DAILY_MAX_AMT = Convert.ToInt32(daily_max_amt);   //Transaction Amount Limit

                db._Users.Add(new_user);
                db.SaveChanges();

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"User ID is { new_user.USERNAME}");
                return new_user.USERID;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //update TBL_USER
        private string UpdateUser(C_Req_User ruser)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                User user = db._Users.Where(u => u.USERNAME == ruser.USERNAME && u.CORPORATEID == ruser.CORPORATEID).FirstOrDefault();    //need to change with PrimaryKey
                CorporateResponseInfo cri = new CorporateResponseInfo();

                //validate 
                //

                user.MOBILENO = ruser.MOBILENO;
                user.EMAIL = ruser.EMAIL;
                user.ADDRESS = ruser.ADDRESS;

                //branchid ?
                user.PositionId = ruser.PositionID;
                user.DepartmentId = ruser.DepartmentID;

                //default fields
                user.UPDATEDATE = DateTime.Now;
                user.UPDATEUSERID = User.Identity.GetUserId();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string loginuserid = User.Identity.GetUserId();
                log.Info(loginuserid, controllerName, actionName, 2, $"{ruser.USERNAME} is updated.");
                return user.USERID;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }
        #endregion

        #region Role Info
        //saving roles
        private void SaveRoles(string userid, List<C_Req_UserInRole> c_ruserroles, bool IsNew)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loginuserid = User.Identity.GetUserId();
            try
            {
                if (!IsNew)     //if not user registration      //all delete and insert
                {
                    //for delete
                    var oldUserRoles = db.C_UserInRoles.Where(a => a.UserId == userid).ToList();
                    if (oldUserRoles != null && oldUserRoles.Count != 0)
                    {
                        db.C_UserInRoles.RemoveRange(oldUserRoles);
                        db.SaveChanges();
                        //delete log
                        log.Info(loginuserid, controllerName, actionName, 3, $"{oldUserRoles.Count} old roles had been deleted.");
                    }
                }

                foreach (C_Req_UserInRole role in c_ruserroles)
                {
                    C_UserInRole new_role = new C_UserInRole();
                    new_role.RoleId = role.RoleId;
                    new_role.UserId = userid;// role.UserId;
                    //acc.CreatedDateTime = DateTime.Now;
                    //acc.CreatedUserID = User.Identity.GetUserId();

                    db.C_UserInRoles.Add(new_role);
                    db.SaveChanges();
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{c_ruserroles.Count} roles saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        #endregion

        #region Menu Info
        //saving menus
        private void SaveMenus(string userid, List<C_Req_MenuPermission> c_rmenus, bool IsNew)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loginuserid = User.Identity.GetUserId();
            try
            {
                if (!IsNew)     //if not user registration      //all delete and insert
                {
                    //for delete
                    var oldUserMenus = db.C_MenuPermissions.Where(a => a.UserId == userid).ToList();
                    if (oldUserMenus != null && oldUserMenus.Count != 0)
                    {
                        db.C_MenuPermissions.RemoveRange(oldUserMenus);
                        db.SaveChanges();
                        //delete log
                        log.Info(loginuserid, controllerName, actionName, 3, $"{oldUserMenus.Count} old menus had been deleted.");
                    }
                }

                foreach (C_Req_MenuPermission rmenu in c_rmenus)
                {
                    C_MenuPermission new_menu = new C_MenuPermission();
                    new_menu.UserId = userid;//rmenu.UserId;
                    new_menu.RoleId = rmenu.RoleId;
                    new_menu.MenuId = rmenu.MenuId;

                    db.C_MenuPermissions.Add(new_menu);
                    db.SaveChanges();
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{c_rmenus.Count} menus saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        #endregion

        #region Tran Info
        private void SaveTranLimits(string userid, List<C_Req_UserTranLimit> c_limits, bool IsNew)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loginuserid = User.Identity.GetUserId();
            try
            {
                if (!IsNew)     //if not user registration      //all delete and insert
                {
                    //for delete
                    var oldUserTranLimits = db.C_UserTranLimits.Where(a => a.UserId == userid).ToList();
                    if (oldUserTranLimits != null && oldUserTranLimits.Count != 0)
                    {
                        db.C_UserTranLimits.RemoveRange(oldUserTranLimits);
                        db.SaveChanges();
                        //delete log
                        log.Info(loginuserid, controllerName, actionName, 3, $"{oldUserTranLimits.Count} old transaction limits had been deleted.");
                    }
                }

                foreach (C_Req_UserTranLimit limit in c_limits)
                {
                    C_UserTranLimit new_limit = new C_UserTranLimit();
                    new_limit.CorporateId = limit.CorporateId;
                    new_limit.UserId = userid;//limit.UserId;
                    new_limit.RuleCode = limit.RuleCode;
                    new_limit.Value = limit.Value;
                    new_limit.IsDelete = false;
                    db.C_UserTranLimits.Add(new_limit);
                    db.SaveChanges();
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{c_limits.Count} transaction limits saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        #endregion

        #region Account Info
        //saving accounts
        private void SaveAccounts(string userid, List<C_Req_UserInAccount> c_raccs, bool IsNew)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string loginuserid = User.Identity.GetUserId();
            try
            {
                if (!IsNew)     //if not user registration      //all delete and insert
                {
                    //for delete
                    var oldUserAccounts = db.C_UserInAccounts.Where(a => a.UserId == userid).ToList();
                    if (oldUserAccounts != null && oldUserAccounts.Count != 0)
                    {
                        db.C_UserInAccounts.RemoveRange(oldUserAccounts);
                        db.SaveChanges();
                        //delete log
                        log.Info(loginuserid, controllerName, actionName, 3, $"{c_raccs.Count} old accounts had been deleted.");
                    }
                }
                var corporateID = c_raccs[0].CorporateId;
                C_Corporate corporate = db.C_Corporates.Where(u => u.DEL_FLAG == false && u.ID == corporateID).FirstOrDefault();
                var accountinfos = GetAccountForUserSaving(corporate.CIFID);

                foreach (C_Req_UserInAccount racc in c_raccs)
                {
                    C_UserInAccount new_acc = new C_UserInAccount();

                    new_acc.CorporateId = racc.CorporateId;
                    new_acc.UserId = userid;//racc.UserId;
                    new_acc.AccountNo = racc.AccountNo;
                    new_acc.AccountType = racc.AccountType;
                    new_acc.AccountTypeDesc = racc.AccountTypeDesc;
                    new_acc.Currency = racc.Currency;
                    var accountinfo = accountinfos.FirstOrDefault(x => x.AccountNo == racc.AccountNo);
                    new_acc.SCHM_CODE = accountinfo.Schm_Code ;
                    new_acc.CreatedDate = DateTime.Now;
                    new_acc.CreatedUserId = 1;
                    new_acc.UpdatedDate = DateTime.Now;
                    new_acc.UpdatedUserId = 1;
                    new_acc.IsDelete = false;

                    db.C_UserInAccounts.Add(new_acc);
                    db.SaveChanges();
                }

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{c_raccs.Count} acounts saved.");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //update accounts
        //private void UpdateAccounts(List<ReqAcc> raccs, string CIFID, List<AccountInfo> core_accs)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        int saveCount = 0;
        //        List<Account> accounts = db.Accounts.Where(a => a.CIFID == CIFID).ToList();
        //        //old comment //to set active in acc if accno exist in reqacc
        //        foreach (Account activeAcc in accounts)
        //        {
        //            var active = raccs.Where(r => r.ACCOUNTNO == activeAcc.ACCOUNTNO).Count();
        //            if (active == 0)//uncheck accounts,  if exist in db.Accounts and not exist in req_accs, make unactive
        //            {
        //                activeAcc.Active = false;
        //                activeAcc.QR_ALLOW = false;

        //                db.Entry(activeAcc).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }

        //        //////delete all old records  (old logic)               
        //        //int oldRecords = db.Accounts.Where(a => a.CIFID == CIFID).Count();
        //        //if (oldRecords > 0)
        //        //{
        //        //    string deleteData = $"{oldRecords} accounts deleted.";
        //        //    db.Accounts.RemoveRange(db.Accounts.Where(a => a.CIFID == CIFID));
        //        //    db.SaveChanges();
        //        //    //save log
        //        //    log.Info(User.Identity.GetUserId(), controllerName, actionName, 3, deleteData);
        //        //}

        //        foreach (var accvm in raccs)
        //        {
        //            var core_acc = core_accs.Where(c => c.AccountNumber == accvm.ACCOUNTNO).FirstOrDefault();
        //            if (core_acc == null)//invalid acc no   or  closed accs at core
        //                continue;
        //            Account acc = db.Accounts.Where(a => a.CIFID == accvm.CIFID & a.ACCOUNTNO == accvm.ACCOUNTNO).FirstOrDefault();
        //            if (acc == null)    //new accs
        //            {
        //                //foreach (Account account in accounts) //Account not exist in ReqAcc , Active = false
        //                //{                            
        //                //    if (!accvm.ACCOUNTNO.Contains(account.ACCOUNTNO)/*accvm.Active*/)//uncheck accounts will not be saved.
        //                //        continue;
        //                saveCount++;
        //                acc = new Account();
        //                //core data
        //                acc.ACCOUNTNO = core_acc.AccountNumber;
        //                acc.ACC_TYPE = core_acc.AccountType;
        //                acc.ACC_DESC = core_acc.AccountTypeDesc;
        //                acc.SCHM_CODE = core_acc.Schm_Code;
        //                acc.BRANCHCODE = core_acc.BranchID;
        //                acc.CURRENCY = core_acc.Currency;
        //                //QR allow false if Acc type is  ODA, LAA, TDA ,not MMK
        //                if (accvm.ACC_TYPE == "ODA" || accvm.ACC_TYPE == "LAA" || accvm.ACC_TYPE == "TDA"/* || accvm.CURRENCY != "MMK"*/)
        //                {
        //                    acc.QR_ALLOW = false;
        //                }
        //                else
        //                    acc.QR_ALLOW = accvm.QR_ALLOW;
        //                //default values                       
        //                //all req_accs are active. we save only active accs.
        //                acc.Active = true;
        //                acc.CIFID = CIFID;
        //                acc.CreatedDateTime = DateTime.Now;
        //                acc.CreatedUserID = User.Identity.GetUserId();
        //                acc.ACCT_CLOSE_FLAG = false;

        //                db.Accounts.Add(acc);
        //                db.SaveChanges();
        //            }
        //            else    //update
        //            {
        //                //var active = accounts.Where(a => a.CIFID == CIFID).FirstOrDefault();

        //                acc.ACCT_CLOSE_FLAG = false;//to reopen closed acc\ 

        //                //all req_accs are active. we save only active accs.
        //                acc.Active = true;
        //                //QR allow false if Acc type is  ODA, LAA, TDA
        //                if (!acc.Active || acc.ACC_TYPE == "ODA" || acc.ACC_TYPE == "LAA" || acc.ACC_TYPE == "TDA" /*|| accvm.CURRENCY != "MMK"*/)
        //                {
        //                    acc.QR_ALLOW = false;
        //                }
        //                else
        //                    acc.QR_ALLOW = accvm.QR_ALLOW;
        //                //default values
        //                acc.UpdatedDateTime = DateTime.Now;
        //                acc.UpdatedUserId = User.Identity.GetUserId();
        //                db.Entry(acc).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //        string getuserid = User.Identity.GetUserId();
        //        if (raccs.Count > 0)
        //            log.Info(getuserid, controllerName, actionName, 2, $"{saveCount} accounts saved.");
        //        //close core_closed accs
        //        List<Account> accs = db.Accounts.Where(a => a.CIFID == CIFID).ToList();
        //        foreach (Account acc in accs)
        //        {
        //            if (core_accs.Where(c => c.AccountNumber == acc.ACCOUNTNO).Count() == 0)
        //            {
        //                acc.ACCT_CLOSE_FLAG = true;//update flag if closed at core
        //                db.Entry(acc).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        throw ex;
        //    }
        //}
        #endregion

        public List<AccountInfoVM> GetAccountForUserSaving(string CIFID)
        {
            try
            {

                CorporateResponseInfo cri = null;

                //check cifid exist
                if (!string.IsNullOrEmpty(CIFID))
                {
                    //int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    //if (count > 0)
                    //    return Json(new { message = "Duplicate CIFID", error = true });

                    //get accounts by cifid
                    cri = CorporateUserRequest.GetCorporateResponse(CIFID, connectionString);

                    //check accounts
                    if (cri != null && !string.IsNullOrEmpty(cri.Name)
                        && cri.lstCorpAccountInfo != null && cri.lstCorpAccountInfo.Count != 0)
                    {
                        var accData = cri.lstCorpAccountInfo.Select(s => new AccountInfoVM
                        {
                            AccountNo = s.AccountNumber,
                            AccountType = s.AccountType,
                            AccountTypeDesc = s.AccountTypeDesc,
                            Currency = s.Currency,
                            Schm_Code = s.Schm_Code,
                            IsActive = false
                        }).ToList();
                        return accData;
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
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
                List<C_Req_Changes> lstChanges = db.C_Req_Changes.Where(rc => ((admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.STATUS==0 && rc.MAKER != userid) || rc.CHECKER== userid).ToList();

                //Create Status List
                ViewBag.cTYPEList = General.GetUserChangesType();

                //Create Status List
                ViewBag.cSTATUSList = General.GetStatus();

                //ViewBag.aCompany = form["aCompany"];
                //ViewBag.aCorporate = form["aCorporate"];
                ViewBag.cUSERNAME = form["cUSERNAME"];
                ViewBag.cTYPE = form["cTYPE"];
                ViewBag.cMAKER = form["cMAKER"];
                ViewBag.cSTATUS = form["cSTATUS"];
                ViewBag.aBranch = form["aBranch"];

                //Get Branch Code and check is branch code of login user "Head Office" or not
                var code = admin.VrBranch.code;
                ViewBag.IsHeadOffice = (code == "9999");

                //assign initial value for "RequestedDate" and Status
                if (form["aStatus"] == null)
                {
                    form["aStatus"] = "0";
                    //form["Datefilter"] = DateTime.Now.ToString("MM/dd/yyyy");
                    ViewBag.aStatus = form["aStatus"];
                }

                //branch data to bind branch dropdown list
                var lstBranch = db.Branches.Where(r => r.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> items = lstBranch.Select(a => new SelectListItem
                {
                    Value = a.id.ToString(),
                    Text = a.name

                });
                ViewBag.aBranchlist = items;

                //carry pdf filename to download or null
                if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                    ViewBag.filename = TempData["filename"];

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
            var aCompany = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var aCorporate = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();

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
            //var reqChanges = (from s in db.C_Req_Changes.Where(rc => (admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.MAKER != userid)
            //                  select s);//only filter by Branch and not own requests
            
            var reqChanges = db.C_Req_Changes.Where(rc => ((admin.BranchID == rc.BranchID || admin.VrBranch.code == "9999") && rc.STATUS == 0 && rc.MAKER != userid) || rc.CHECKER == userid).ToList().ConvertAll(u => new
            {
                ID = u.ID,
                COMPANY_NAME = u.VrCorporate.COMPANY_NAME,
                CORPORATEID = u.VrCorporate.CORPORATEID,
                USERNAME = u.User.USERNAME,
                TYPE = u.TYPE,                
                Maker = (u.VrReqMaker == null ? "" : u.VrReqMaker.FullName),
                CorporateMaker = (u.CMAKER == null ? "" : (db._Users.Where(us => us.USERID == u.CMAKER).FirstOrDefault()).FULLNAME),
                REQUESTEDDATE = u.REQUESTEDDATE,
                STATUS = u.STATUS,
                BranchId = u.VrBranch.id
            });

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            ViewBag.IsHeadOffice = (code == "9999");

            if (!string.IsNullOrEmpty(aCompany))
            {
                reqChanges = reqChanges.Where(a => a.COMPANY_NAME.ToLower().Trim().Contains(aCompany.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(aCorporate))
            {
                reqChanges = reqChanges.Where(a => a.CORPORATEID.ToLower().Trim().Contains(aCorporate.ToLower().Trim())).ToList();
            }

            if (!string.IsNullOrEmpty(cUSERNAME))
            {
                reqChanges = reqChanges.Where(a => a.USERNAME.Contains(cUSERNAME.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(cTYPE))
            {
                reqChanges = reqChanges.Where(a => a.TYPE.ToString() == cTYPE).ToList();

            }

            if (!string.IsNullOrEmpty(cSTATUS))
            {
                reqChanges = reqChanges.Where(a => a.STATUS.ToString() == cSTATUS).ToList();

            }
            if (admin.VrBranch.code == "9999")
            {
                if (!string.IsNullOrEmpty(aBranch))
                {
                    reqChanges = reqChanges.Where(a => a.BranchId.ToString() == aBranch).ToList();

                }
            }

            if (fromDate != null & toDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate).ToList();
            }
            else if (fromDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE >= fromDate).ToList();
            }
            else if (toDate != null)
            {
                reqChanges = reqChanges.Where(a => a.REQUESTEDDATE < toDate).ToList();
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                reqChanges = reqChanges.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }


            recordsTotal = reqChanges.Count();
            var data = reqChanges.Skip(skip).Take(pageSize).ToList();

            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        //Changes Detail
        public ActionResult ChangesDetails(int id)
        {
            try
            {
                //C_Req_Changes reqChange = db.C_Req_Changes.Where(u => u.ID == id).FirstOrDefault();
                var reqChange = db.C_Req_Changes.Where(u => u.ID == id).FirstOrDefault();
                CReqChangesVM reqChangevm = new CReqChangesVM
                {
                    ID = reqChange.ID,
                    COMPANY_NAME = reqChange.VrCorporate.COMPANY_NAME,
                    CORPORATEID = reqChange.VrCorporate.CORPORATEID,
                    USERNAME = reqChange.User.USERNAME,
                    TYPE = reqChange.TYPE,
                    MAKER = (reqChange.VrReqMaker == null ? "" : reqChange.VrReqMaker.FullName),
                    CHECKER = (reqChange.VrReqChecker == null ? "" : reqChange.VrReqChecker.FullName),
                    CORPORATE_MAKER = (reqChange.CMAKER == null ? "" : (db._Users.Where(us => us.USERID == reqChange.CMAKER).FirstOrDefault()).FULLNAME),
                    CORPORATE_CHECKER = (reqChange.C_CHECKER == null ? "" : (db._Users.Where(us => us.USERID == reqChange.C_CHECKER).FirstOrDefault()).FULLNAME),
                    CHECKEDDATE = reqChange.CHECKEDDATE,
                    CHECKERREASON = reqChange.CHECKERREASON,
                    REQUESTEDDATE = reqChange.REQUESTEDDATE,
                    STATUS = reqChange.STATUS,
                    BRANCH = reqChange.VrBranch.name
                };
                if (reqChange == null)
                {
                    return HttpNotFound();
                }

                return View(reqChangevm);
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

        //generate pdf      //if status 0 => register, 1 => reset
        private string GeneratePdf(string CIFID, string COMPANY_NAME, string USERNAME, string USERROLE, string FULLNAME, string CorporateID, string ADDRESS, string MOBILENO, string EMAIL, string daily_cortran_amt, string cor_tran_limit, string cor_tran_max_amt, string maker_tran_limit, string approver_tran_limit, string LoginPwd, string txnPwd, int Status, int RoleStatus)//string name, string LoginPwd, string txnPwd,int Status)
        {
            try
            {
                daily_cortran_amt = (decimal.Parse(daily_cortran_amt, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add thousand separator
                cor_tran_limit = (decimal.Parse(cor_tran_limit, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add thousand separator
                cor_tran_max_amt = (decimal.Parse(cor_tran_max_amt, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add thousand separator
                maker_tran_limit = (decimal.Parse(maker_tran_limit, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add thousand separator
                approver_tran_limit = (decimal.Parse(approver_tran_limit, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)).ToString("#,##0") + " MMK"; //add thousand separator
                //string Pdf_Path = System.Configuration.ConfigurationManager.AppSettings["PdfSavePath"];
                string CorporatePdf_Path = ConstantValues.CorporatePdf_Path;
                string filename = "";
                User new_user = new User();
                // IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();

                if (Status == 1)//resetting  pwd
                {
                    CorporatePdf_Path = $"{CorporatePdf_Path}\\reset password\\" + CorporateID;
                    //create if folder doesn't exist
                    if (!Directory.Exists(CorporatePdf_Path))
                    {
                        Directory.CreateDirectory(CorporatePdf_Path);
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

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Create a font
                    XFont font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo, 1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    //pen.DashStyle = XDashStyle.Dot;
                    //gfx.DrawRectangle(pens, 330, 0, 219, 70);
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Date", font, XBrushes.Black, new XRect(340, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(425, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Corporate ID name
                    gfx.DrawString($"  CIFID", font, XBrushes.Black, new XRect(340, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //For Corporate ID value
                    gfx.DrawString($": {CIFID}", font, XBrushes.Black, new XRect(425, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));
                    // Draw the text
                    gfx.DrawString("eBanking Password Reset Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 123, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    //gfx.DrawLine(pen, 50, 158, 550, 158);
                    gfx.DrawLine(pens, 50, 150, 50, 295);
                    gfx.DrawLine(pens, 50, 295, 550, 295);
                    gfx.DrawLine(pens, 550, 150, 550, 295);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Corporate User Information", font, XBrushes.Black, new XRect(60, 110, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    //for company name and address text autofix
                    var tf = new XTextFormatter(gfx);
                    //tf.Alignment = XParagraphAlignment.Justify;

                    // For Corporate name
                    gfx.DrawString($"●  Company Name", font, XBrushes.Black, new XRect(60, 158, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Corporate name value
                    //gfx.DrawString($"- {COMPANY_NAME}", font, XBrushes.Black, new XRect(210, 168, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    tf.DrawString($": {COMPANY_NAME}", font, XBrushes.Black, new XRect(210, 158, page.Width, 10),
                     XStringFormats.TopLeft);

                    // For Corporate user name
                    gfx.DrawString($"●  Fullname", font, XBrushes.Black, new XRect(60, 198, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {FULLNAME}", font, XBrushes.Black, new XRect(210, 198, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 218, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MOBILENO}", font, XBrushes.Black, new XRect(210, 218, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 238, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {EMAIL}", font, XBrushes.Black, new XRect(210, 238, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Address name
                    gfx.DrawString($"●  Address", font, XBrushes.Black, new XRect(60, 258, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Address value
                    tf.DrawString($": {ADDRESS}", font, XBrushes.Black, new XRect(210, 258, 345, 50),
                      XStringFormats.TopLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 325, 500, 110);
                    //gfx.DrawLine(pen, 50, 350, 550, 350);
                    gfx.DrawRectangle(pen, 50, 315, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 447, 50, 340);
                    gfx.DrawLine(pens, 50, 447, 550, 447);
                    gfx.DrawLine(pens, 550, 447, 550, 340);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 305, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);
                    //Create password font
                    XFont pwfont = new XFont("Myanmar3", 12, XFontStyle.Regular, options);

                    // For Corporate ID name
                    gfx.DrawString($"●  Corporate ID", font, XBrushes.Black, new XRect(60, 350, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //For Corporate ID value
                    gfx.DrawString($": {CorporateID}", font, XBrushes.Black, new XRect(210, 350, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Username name
                    gfx.DrawString($"●  Username", font, XBrushes.Black, new XRect(60, 370, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Username value
                    gfx.DrawString($": {USERNAME}", font, XBrushes.Black, new XRect(210, 370, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For user roll name
                    gfx.DrawString($"●  User Role", font, XBrushes.Black, new XRect(60, 390, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For user role value
                    gfx.DrawString($": {USERROLE}", pwfont, XBrushes.Black, new XRect(210, 390, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Loginpwd name
                    gfx.DrawString($"●  Login Password", font, XBrushes.Black, new XRect(60, 410, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Loginpwd value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black, new XRect(210, 410, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Tranpwd name
                    gfx.DrawString($"●  Transaction Password", font, XBrushes.Black, new XRect(60, 430, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {txnPwd}", pwfont, XBrushes.Black, new XRect(210, 430, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //status =0 ,user role for AdminMaker,AdminApprover
                    if (RoleStatus == 0)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 535, 50, 490);
                        gfx.DrawLine(pens, 50, 535, 550, 535);
                        gfx.DrawLine(pens, 550, 535, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        // For Tranpwd name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For Tranpwd value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For Tranpwd name
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For Tranpwd value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status=1 , user role for Viewer
                    //if (RoleStatus == 1){}

                    //status =1 ,user role for Maker,Viewer,Approver
                    if (RoleStatus == 2)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 570, 50, 490);
                        gfx.DrawLine(pens, 50, 570, 550, 570);
                        gfx.DrawLine(pens, 550, 570, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        //// For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For maker tran limit name
                        gfx.DrawString($"●  Maker Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {maker_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For approver tran limit name
                        gfx.DrawString($"●  Approver Transaction Limit", font, XBrushes.Black, new XRect(60, 555, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For approver tran limit value
                        gfx.DrawString($": {approver_tran_limit}", font, XBrushes.Black, new XRect(350, 555, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status =2 ,user role for Maker
                    if (RoleStatus == 3)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 560, 50, 490);
                        gfx.DrawLine(pens, 50, 560, 550, 560);
                        gfx.DrawLine(pens, 550, 560, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        // For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For maker tran limit name
                        gfx.DrawString($"●  Maker Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {maker_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status =3 ,user role for Approver
                    if (RoleStatus == 4)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 560, 50, 490);
                        gfx.DrawLine(pens, 50, 560, 550, 560);
                        gfx.DrawLine(pens, 550, 560, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        //// For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For approver tran limit name
                        gfx.DrawString($"●  Approver Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For approver tran limit value
                        gfx.DrawString($": {approver_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    // for font create
                    font = new XFont("Arial", 11, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle = XDashStyle.Dot;
                    gfx.DrawLine(pens, 320, 590, 225, 590);
                    //for password policies
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 560, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // for font create
                    font = new XFont("Arial", 10, XFontStyle.Regular, options);

                    //for password policies
                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 600, page.Width, 10),
                     XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 615, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 630, page.Width, 10),
                   XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 645, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 660, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 675, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     from transaction password", font, XBrushes.Black, new XRect(60, 690, page.Width, 10),
                    XStringFormats.CenterLeft);

                    //for myanmar language
                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 600, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 615, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 630, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 645, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for address text autofix
                    tf = new XTextFormatter(gfx);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 660, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 675, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"    မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 690, page.Width, 10),
                   XStringFormats.CenterLeft);

                    //for  Ass / Manager
                    gfx.DrawString($"Assistant Manager / Manager", font, XBrushes.Black, new XRect(450, 720, 50, 50),
                      XStringFormats.CenterLeft);

                    string timestamp = General.GetTimestamp(DateTime.Now);

                    // Save the document...
                    filename = $"{timestamp}_{USERNAME}_resetpwd.pdf";
                    document.Save($"{CorporatePdf_Path}\\{filename}");
                }
                else
                {
                    CorporatePdf_Path = $"{CorporatePdf_Path}\\register\\" + CorporateID;

                    //create if folder doesn't exist
                    if (!Directory.Exists(CorporatePdf_Path))
                    {
                        Directory.CreateDirectory(CorporatePdf_Path);
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

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Create a font
                    XFont font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo, 1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    //pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Date", font, XBrushes.Black, new XRect(340, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(425, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Corporate ID name
                    gfx.DrawString($"  CIFID", font, XBrushes.Black, new XRect(340, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //For Corporate ID value
                    gfx.DrawString($": {CIFID}", font, XBrushes.Black, new XRect(425, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                    //Draw BgColor
                    //gfx.DrawRectangle(XBrushes.LightGray, new XRect(50, 80, 500, 30));
                    // Draw the text
                    gfx.DrawString("eBanking Corporate User Register Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 123, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    //gfx.DrawLine(pen, 50, 158, 550, 158);
                    gfx.DrawLine(pens, 50, 150, 50, 295);
                    gfx.DrawLine(pens, 50, 295, 550, 295);
                    gfx.DrawLine(pens, 550, 150, 550, 295);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Corporate User Information", font, XBrushes.Black, new XRect(60, 110, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    //for company name and address text autofix
                    var tf = new XTextFormatter(gfx);
                    //tf.Alignment = XParagraphAlignment.Justify;

                    // For Corporate name
                    gfx.DrawString($"●  Company Name", font, XBrushes.Black, new XRect(60, 158, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Corporate name value
                    //gfx.DrawString($"- {COMPANY_NAME}", font, XBrushes.Black, new XRect(210, 168, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    tf.DrawString($": {COMPANY_NAME}", font, XBrushes.Black, new XRect(210, 158, page.Width, 10),
                     XStringFormats.TopLeft);

                    // For Corporate user name
                    gfx.DrawString($"●  Fullname", font, XBrushes.Black, new XRect(60, 198, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {FULLNAME}", font, XBrushes.Black, new XRect(210, 198, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 218, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MOBILENO}", font, XBrushes.Black, new XRect(210, 218, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 238, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {EMAIL}", font, XBrushes.Black, new XRect(210, 238, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Address name
                    gfx.DrawString($"●  Address", font, XBrushes.Black, new XRect(60, 258, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Address value
                    tf.DrawString($": {ADDRESS}", font, XBrushes.Black, new XRect(210, 258, 345, 50),
                      XStringFormats.TopLeft);

                    //Draw Dotted Line
                    //gfx.DrawRectangle(pen, 50, 325, 500, 110);
                    //gfx.DrawLine(pen, 50, 350, 550, 350);
                    gfx.DrawRectangle(pen, 50, 315, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 447, 50, 340);
                    gfx.DrawLine(pens, 50, 447, 550, 447);
                    gfx.DrawLine(pens, 550, 447, 550, 340);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 305, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 11, XFontStyle.Regular, options);
                    //Create password font
                    XFont pwfont = new XFont("Myanmar3", 12, XFontStyle.Regular, options);

                    // For Corporate ID name
                    gfx.DrawString($"●  Corporate ID", font, XBrushes.Black, new XRect(60, 350, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //For Corporate ID value
                    gfx.DrawString($": {CorporateID}", font, XBrushes.Black, new XRect(210, 350, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Username name
                    gfx.DrawString($"●  Username", font, XBrushes.Black, new XRect(60, 370, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Username value
                    gfx.DrawString($": {USERNAME}", font, XBrushes.Black, new XRect(210, 370, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For user roll name
                    gfx.DrawString($"●  User Role", font, XBrushes.Black, new XRect(60, 390, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For user role value
                    gfx.DrawString($": {USERROLE}", pwfont, XBrushes.Black, new XRect(210, 390, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Loginpwd name
                    gfx.DrawString($"●  Login Password", font, XBrushes.Black, new XRect(60, 410, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Loginpwd value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black, new XRect(210, 410, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Tranpwd name
                    gfx.DrawString($"●  Transaction Password", font, XBrushes.Black, new XRect(60, 430, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Tranpwd value
                    gfx.DrawString($": {txnPwd}", pwfont, XBrushes.Black, new XRect(210, 430, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //status =0 ,user role for AdminMaker,AdminApprover
                    if (RoleStatus == 0)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 535, 50, 490);
                        gfx.DrawLine(pens, 50, 535, 550, 535);
                        gfx.DrawLine(pens, 550, 535, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        // For Tranpwd name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For Tranpwd value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For Tranpwd name
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For Tranpwd value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status=1 , user role for Viewer
                    //if (RoleStatus == 1){}

                    //status =1 ,user role for Maker,Viewer,Approver
                    if (RoleStatus == 2)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 570, 50, 490);
                        gfx.DrawLine(pens, 50, 570, 550, 570);
                        gfx.DrawLine(pens, 550, 570, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        //// For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For maker tran limit name
                        gfx.DrawString($"●  Maker Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {maker_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For approver tran limit name
                        gfx.DrawString($"●  Approver Transaction Limit", font, XBrushes.Black, new XRect(60, 555, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For approver tran limit value
                        gfx.DrawString($": {approver_tran_limit}", font, XBrushes.Black, new XRect(350, 555, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status =2 ,user role for Maker
                    if (RoleStatus == 3)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 560, 50, 490);
                        gfx.DrawLine(pens, 50, 560, 550, 560);
                        gfx.DrawLine(pens, 550, 560, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        // For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For maker tran limit name
                        gfx.DrawString($"●  Maker Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {maker_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    //status =3 ,user role for Approver
                    if (RoleStatus == 4)
                    {
                        ////Draw Dotted Line
                        gfx.DrawRectangle(pen, 50, 465, 500, 25);
                        ////draw rounded border
                        pens = new XPen(XColors.Indigo, 0.5);
                        gfx.DrawLine(pens, 50, 560, 50, 490);
                        gfx.DrawLine(pens, 50, 560, 550, 560);
                        gfx.DrawLine(pens, 550, 560, 550, 490);

                        // Create a font
                        font = new XFont("Arial", 14, XFontStyle.Bold, options);
                        // Draw the text
                        gfx.DrawString("Transaction Information", font, XBrushes.Black, new XRect(60, 455, page.Width, 45),
                        XStringFormats.CenterLeft);

                        // Create a font
                        font = new XFont("Arial", 11, XFontStyle.Regular, options);
                        //// For daily corporate tran amount name
                        //gfx.DrawString($"●  Daily Corporate Transaction Amount", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);
                        //// For corporate tran amount value
                        //gfx.DrawString($": {daily_cortran_amt}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        //XStringFormats.CenterLeft);

                        // For corporate tran limit name
                        gfx.DrawString($"●  Corporate Transaction Limit", font, XBrushes.Black, new XRect(60, 495, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For corporate limit value
                        gfx.DrawString($": {cor_tran_limit}", font, XBrushes.Black, new XRect(350, 495, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For one time cor tran maximum amt
                        gfx.DrawString($"●  One Time Corporate Transaction Maximum Amount", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For maker tran limit value
                        gfx.DrawString($": {cor_tran_max_amt}", font, XBrushes.Black, new XRect(350, 515, page.Width, 10),
                        XStringFormats.CenterLeft);

                        // For approver tran limit name
                        gfx.DrawString($"●  Approver Transaction Limit", font, XBrushes.Black, new XRect(60, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                        // For approver tran limit value
                        gfx.DrawString($": {approver_tran_limit}", font, XBrushes.Black, new XRect(350, 535, page.Width, 10),
                        XStringFormats.CenterLeft);
                    }

                    // for font create
                    font = new XFont("Arial", 11, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle = XDashStyle.Dot;
                    gfx.DrawLine(pens, 320, 590, 225, 590);
                    //for password policies
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 560, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // for font create
                    font = new XFont("Arial", 10, XFontStyle.Regular, options);

                    //for password policies
                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 600, page.Width, 10),
                     XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 615, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 630, page.Width, 10),
                   XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 645, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 660, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 675, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     from transaction password", font, XBrushes.Black, new XRect(60, 690, page.Width, 10),
                    XStringFormats.CenterLeft);

                    //for myanmar language
                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 600, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 615, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 630, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 645, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for address text autofix
                    tf = new XTextFormatter(gfx);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 660, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 675, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"    မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 690, page.Width, 10),
                   XStringFormats.CenterLeft);

                    //for  Ass / Manager
                    gfx.DrawString($"Assistant / Manager", font, XBrushes.Black, new XRect(450, 720, 50, 50),
                      XStringFormats.CenterLeft);

                    #region comment close region
                    // //create if folder doesn't exist
                    // if (!Directory.Exists(CorporatePdf_Path))
                    // {
                    //     Directory.CreateDirectory(CorporatePdf_Path);
                    // }
                    // // Create a new PDF document
                    // PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                    // // Create an empty page
                    // PdfSharp.Pdf.PdfPage page = document.AddPage();
                    // //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                    // // Get an XGraphics object for drawing
                    // XGraphics gfx = XGraphics.FromPdfPage(page);

                    // XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                    // //draw bg
                    // DrawImage(gfx);

                    // // Put a logo in the header
                    // DrawTitleImage(gfx);

                    // //PDF Size
                    // page.Size = PdfSharp.PageSize.A4;

                    // // Create a font
                    // XFont font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    // //Draw Dotted Line
                    // XPen pen = new XPen(XColors.Indigo, 1.5);
                    // XPen pens = new XPen(XColors.Indigo, 0.5);
                    // //pen.DashStyle = XDashStyle.Dot;
                    // gfx.DrawRectangle(pens, 330, 0, 219, 70);

                    // //gfx.DrawLine();
                    // // Create a font
                    // font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    // //for Datetime name
                    // gfx.DrawString($"  Date", font, XBrushes.Black, new XRect(340, 15, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // //for Datetime value
                    // gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy")}", font, XBrushes.Black, new XRect(445, 15, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For CIFID name
                    // gfx.DrawString($"  Corporate ID", font, XBrushes.Black, new XRect(340, 45, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // //For CIFID value
                    // gfx.DrawString($": {CorporateID}", font, XBrushes.Black, new XRect(445, 45, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);
                    // //Draw BgColor
                    // // Draw the text
                    // gfx.DrawString("eBanking Password Reset Information", font, XBrushes.Black,
                    //   new XRect(0, 0, page.Width, 190),
                    //   XStringFormats.Center);

                    // pen.DashStyle = XDashStyle.Dot;
                    // gfx.DrawRectangle(pen, 50, 133, 500, 25);
                    // pens = new XPen(XColors.Indigo, 0.5);
                    // //gfx.DrawLine(pen, 50, 158, 550, 158);
                    // gfx.DrawLine(pens, 50, 153, 50, 310);
                    // gfx.DrawLine(pens, 50, 310, 550, 310);
                    // gfx.DrawLine(pens, 550, 153, 550, 310);

                    // // Create a font
                    // font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // // Draw the text
                    // gfx.DrawString("Corporate User Information", font, XBrushes.Black, new XRect(60, 120, page.Width, 50),
                    //   XStringFormats.CenterLeft);

                    // // Create a font
                    // font = new XFont("Arial", 11, XFontStyle.Regular, options);

                    // //for company name and address text autofix
                    // var tf = new XTextFormatter(gfx);
                    // tf.Alignment = XParagraphAlignment.Justify;

                    // // For Corporate name
                    // gfx.DrawString($"●  Company Name", font, XBrushes.Black, new XRect(60, 168, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Customer name value
                    // tf.DrawString($"- {COMPANY_NAME}", font, XBrushes.Black, new XRect(210, 168, page.Width, 10),
                    //  XStringFormats.TopLeft);

                    // // For Corporate user name
                    // gfx.DrawString($"●  Fullname", font, XBrushes.Black, new XRect(60, 208, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Customer name value
                    // gfx.DrawString($"- {FULLNAME}", font, XBrushes.Black, new XRect(210, 208, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For PhoneNo name
                    // gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 228, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For PhoneNo value
                    // gfx.DrawString($"- {MOBILENO}", font, XBrushes.Black, new XRect(210, 228, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For Email name
                    // gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 248, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Email value
                    // gfx.DrawString($"- {EMAIL}", font, XBrushes.Black, new XRect(210, 248, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For Address name
                    // gfx.DrawString($"●  Address", font, XBrushes.Black, new XRect(60, 268, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Address value
                    // tf.DrawString($"- {ADDRESS}", font, XBrushes.Black, new XRect(210, 268, 345, 50),
                    //   XStringFormats.TopLeft);

                    // //Draw Dotted Line
                    // //gfx.DrawRectangle(pen, 50, 325, 500, 110);
                    // //gfx.DrawLine(pen, 50, 350, 550, 350);
                    // gfx.DrawRectangle(pen, 50, 340, 500, 25);
                    // pens = new XPen(XColors.Indigo, 0.5);
                    // gfx.DrawLine(pens, 50, 435, 50, 365);
                    // gfx.DrawLine(pens, 50, 435, 550, 435);
                    // gfx.DrawLine(pens, 550, 435, 550, 365);

                    // // Create a font
                    // font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // // Draw the text
                    // gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 330, page.Width, 45),
                    //   XStringFormats.CenterLeft);

                    // // Create a font
                    // font = new XFont("Arial", 11, XFontStyle.Regular, options);
                    // //Create password font
                    // XFont pwfont = new XFont("Myanmar3", 12, XFontStyle.Regular, options);
                    // // For Username name
                    // gfx.DrawString($"●  Username", font, XBrushes.Black, new XRect(60, 375, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Username value
                    // gfx.DrawString($"- {USERNAME}", font, XBrushes.Black, new XRect(210, 375, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For Loginpwd name
                    // gfx.DrawString($"●  Login Password", font, XBrushes.Black, new XRect(60, 395, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Loginpwd value
                    // gfx.DrawString($"- {LoginPwd}", pwfont, XBrushes.Black, new XRect(210, 395, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // For Tranpwd name
                    // gfx.DrawString($"●  Transaction Password", font, XBrushes.Black, new XRect(60, 415, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // // For Tranpwd value
                    // gfx.DrawString($"- {txnPwd}", pwfont, XBrushes.Black, new XRect(210, 415, page.Width, 10),
                    //   XStringFormats.CenterLeft);

                    // // for font create
                    // font = new XFont("Arial", 11, XFontStyle.Bold, options);
                    // //Draw Dotted Line
                    // pens = new XPen(XColors.Black, 1.5);
                    // pens.DashStyle = XDashStyle.Dot;
                    // gfx.DrawLine(pens, 330, 520, 225, 520);
                    // //for password policies
                    // gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 490, page.Width, 45),
                    //   XStringFormats.CenterLeft);

                    // // for font create
                    // font = new XFont("Arial", 10, XFontStyle.Regular, options);

                    // //for password policies
                    // gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 545, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 575, page.Width, 10),
                    //XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 590, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 605, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 635, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"     from transaction password", font, XBrushes.Black, new XRect(60, 650, page.Width, 10),
                    // XStringFormats.CenterLeft);

                    // //for myanmar language

                    // // for font create
                    // XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    // //for password policies
                    // gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 575, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 590, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // //for address text autofix
                    // tf = new XTextFormatter(gfx);
                    // gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။ ", fonts, XBrushes.Black, new XRect(270, 605, 345, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   (< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 620, 345, 10),
                    // XStringFormats.CenterLeft);
                    // // for font create
                    // gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 635, page.Width, 10),
                    //   XStringFormats.CenterLeft);
                    // gfx.DrawString($"   မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 650, page.Width, 10),
                    //XStringFormats.CenterLeft);

                    // //for  Ass / Manager
                    // gfx.DrawString($"Ass / Manager", font, XBrushes.Black, new XRect(450, 690, 50, 50),
                    //   XStringFormats.CenterLeft);
                    #endregion

                    string timestamp = General.GetTimestamp(DateTime.Now);

                    // Save the document...
                    filename = $"{timestamp}_{USERNAME}.pdf";
                    document.Save($"{CorporatePdf_Path}\\{filename}");
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

        #region download pdf file
        public ActionResult Download(string filename)//type 0=register, 1=reset login, 2=reset txn
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //string Pdf_Path = System.Configuration.ConfigurationManager.AppSettings["PdfSavePath"];
                string Pdf_Path = ConstantValues.CorporatePdf_Path;
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
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        //Kill Session

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
        #endregion
    }
}
