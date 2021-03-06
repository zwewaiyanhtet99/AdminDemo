using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class ReportController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        private AdminLogDBContext logDb = new AdminLogDBContext();
        private EventLogController log = new EventLogController();
        //private HomeController home = new HomeController();
        string controllerName = "ReportController";
        private string ConString { get { return ConstantValues.Connection_String; } }
        private string Date_Format { get { return ConstantValues.Date_Format; } }
        bool br_code = false;
        string branch_name = "";//for transaction report parameter

        //get connection string
        string connectionString = ConstantValues.Connection_String;

        //OtherBankChargeRateInfo chargerate = new OtherBankChargeRateInfo();
        [Obsolete]
        OtherBankInformationInquiry bankinfo = new OtherBankInformationInquiry();

        #region User
        // GET: Reports
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                UserSearchViewModel usersvm = new UserSearchViewModel();
                usersvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                usersvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                BindBranchDDL();
                return View(usersvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //User Report
        [HttpPost]
        public ActionResult Index(UserSearchViewModel usersvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string name = usersvm.Name ?? "";
                    string CIFID = usersvm.CIFID ?? "";
                    string phoneno = usersvm.PhoneNo ?? "";
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    name = name.ToLower().Trim();
                    CIFID = CIFID.ToLower().Trim();
                    phoneno = phoneno.ToLower().Trim();
                    string choosedbranch = "";

                    if (!String.IsNullOrEmpty(usersvm.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(usersvm.FromDate);
                        //fromDate = DateTime.ParseExact(usersvm.FromDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(usersvm.ToDate))
                    {
                        toDate = General.ConvertStrToDate(usersvm.ToDate);
                        //toDate = DateTime.ParseExact(usersvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindBranchDDL();
                    //    return View(usersvm);
                    //}
                    #endregion

                    //Add One Day for toDate                    
                    toDate = toDate.AddDays(1);

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(usersvm);
                    }
                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        choosedbranch = usersvm.Branch ?? "";
                        ViewBag.BranchList = GetBranchSelectList();
                        br_code = true;
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                        br_code = false;
                    }

                    var users = (from u in db._Users
                                 join d in db.Users on u.CREATEDUSERID equals d.Id into tbluser
                                 from tblApprover in tbluser.DefaultIfEmpty()
                                 where u.DEL_STATUS == false & (u.USER_TYPE == "Retail" || u.USER_TYPE == null) & u.CIFID.Contains(CIFID) & u.USERNAME.Contains(name) & u.MOBILENO.Contains(phoneno) & (tblApprover.VrBranch.code == choosedbranch || choosedbranch == "")
                                 & u.CREATEDDATE >= fromDate & u.CREATEDDATE < toDate
                                 //(
                                 //((fromDate != null & toDate != null) & (u.CREATEDDATE >= fromDate & u.CREATEDDATE < toDate))
                                 //|| (fromDate != null & toDate == null & (u.CREATEDDATE >= fromDate))
                                 //|| (fromDate == null & toDate != null & (u.CREATEDDATE < toDate))
                                 //|| (fromDate == null & toDate == null & 1 == 1)
                                 //)
                                 orderby u.CREATEDDATE ascending
                                 select new UserRPViewModel
                                 {
                                     FULLNAME = u.FULLNAME,
                                     MOBILENO = u.MOBILENO,
                                     USERNAME = u.USERNAME,
                                     NRC = u.NRC,
                                     EMAIL = u.EMAIL,
                                     CIFID = u.CIFID,
                                     APPROVER = tblApprover.UserName,
                                     APPROVEDDATE = u.CREATEDDATE
                                 });

                    //get creator from requser (IsNew = true)
                    List<UserRPViewModel> userrp = users.ToList();
                    foreach (var u in userrp)
                    {
                        var requser = db.ReqUsers.Where(ru => ru.USERNAME == u.USERNAME & ru.ISNEW == true).FirstOrDefault();
                        if (requser != null)
                        {
                            u.CREATOR = requser.VrMaker.UserName;
                            u.CREATEDDATE = requser.REQUESTEDDATE;
                        }
                    }

                    List<UserRPViewModel> result = userrp.ToList<UserRPViewModel>();
                    usersvm.userrpvms = result;
                    Session["ReportData"] = usersvm.userrpvms;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"User report generates successfully.");
                }
                BindBranchDDL();
                TempData.Keep();
                return View(usersvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Topup

        public ActionResult Topup()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                TopUpSearchViewModel searchvm = new TopUpSearchViewModel();
                searchvm.RetailSearch = new TopUpSearch();
                searchvm.CorporateSearch = new TopUpSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                searchvm.RetailSearch.FromDate = strToday;
                searchvm.RetailSearch.ToDate = strToday;

                //for corporate
                searchvm.CorporateSearch.FromDate = strToday;
                searchvm.CorporateSearch.ToDate = strToday;

                BindBranchDDL();
                BindOperatorDDL();
                return View(searchvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Topup(TopUpSearchViewModel searchvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    string branchcode, trantype, choosedbranch, biller, fromdate, todate, company = "";
                    Boolean IsCorporate = searchvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = searchvm.CorporateSearch.BranchCode ?? "";
                        trantype = "TopUp";
                        choosedbranch = "";
                        biller = searchvm.CorporateSearch.Utility_Type;
                        fromdate = searchvm.CorporateSearch.FromDate ?? "";
                        todate = searchvm.CorporateSearch.ToDate ?? "";
                        company = searchvm.CorporateSearch.CompanyName ?? "";
                        IsCorporate = true;
                    }
                    else
                    {
                        //retail search
                        branchcode = searchvm.RetailSearch.BranchCode ?? "";
                        trantype = "TopUp";
                        choosedbranch = "";
                        biller = searchvm.RetailSearch.Utility_Type;
                        fromdate = searchvm.RetailSearch.FromDate ?? "";
                        todate = searchvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }

                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        from = fromDate.ToString(ConstantValues.Date_TAndUReports);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_TAndUReports);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindBranchDDL();
                    //    BindOperatorDDL();
                    //    return View(searchvm);
                    //}
                    #endregion

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(searchvm);
                    }
                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = searchvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = searchvm.RetailSearch.BranchCode ?? "";
                        }
                        br_code = true;
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                        br_code = false;
                    }
                    Session["BranchCode"] = br_code;
                    DataTable dtResult;
                    if (IsCorporate == true)
                    {
                        dtResult = GetCorTopUp(choosedbranch, trantype, biller, from, to, IsCorporate, company);
                    }
                    else
                    {
                        dtResult = GetTopUp(choosedbranch, trantype, biller, from, to, IsCorporate);
                    }
                    //DataTable dtResult = GetUtility(choosedbranch, trantype, biller, from, to,IsCorporate);
                    List<TopUpViewModel> result = (from DataRow dr in dtResult.Rows
                                                   select new TopUpViewModel()
                                                   {
                                                       TRAN_ID = dr["TRAN_ID"].ToString(),
                                                       FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                       FROM_BRANCH = dr["FROM_BRANCH"].ToString(),
                                                       TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                                       VALUE_DATE_DT = (DateTime)dr["VALUE_DATE_DT"],
                                                       TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString(), "yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture),
                                                       FULLNAME = dr["USERNAME"].ToString(),
                                                       NetAmount = dr["NetAmount"].ToString(),
                                                       DiscountAmount = dr["DiscountAmount"].ToString(),
                                                       TotalAmount = dr["TotalAmount"].ToString(),
                                                       PartnerAmount = dr["PartnerAmount"].ToString(),
                                                       RespDesc = dr["RespDesc"].ToString(),
                                                       Status = dr["Status"].ToString(),
                                                       BillerCode = dr["BillerCode"].ToString(),
                                                       BillPhone = dr["BillPhone"].ToString(),
                                                       Deno = dr["Deno"].ToString(),
                                                       ThirdPartyID = dr["ThirdPartyID"].ToString(),
                                                       CompanyName = dr["CompanyName"].ToString()
                                                   }).ToList();
                    searchvm.TopUpsVM = result;
                    Session["ReportData"] = searchvm.TopUpsVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Topup report generates successfully.");
                }
                //bind ddls
                BindBranchDDL();
                BindOperatorDDL();
                TempData.Keep();
                return View(searchvm);

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Transaction
        //Transaction
        public ActionResult Transaction()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                TransactionSearchViewModel tranSearch = new TransactionSearchViewModel();
                tranSearch.RetailSearch = new TransactionSearch();
                tranSearch.CorporateSearch = new TransactionSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");
                //for retail
                tranSearch.RetailSearch.FromDate = strToday;
                tranSearch.RetailSearch.ToDate = strToday;
                //for corporate
                tranSearch.CorporateSearch.FromDate = strToday;
                tranSearch.CorporateSearch.ToDate = strToday;

                BindTranDDL();
                BindBranchDDL();
                BindCurrencyDDL();
                return View(tranSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Transaction(TransactionSearchViewModel transvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    string from = null, to = null;
                    string choosedbranch = "";
                    string branchcode, trantype, debitorcredit, currency, fromdate, todate, company = "";
                    Boolean IsCorporate = transvm.IsCorporate;
                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = transvm.CorporateSearch.BranchCode ?? "";
                        trantype = transvm.CorporateSearch.TranType ?? "TranOwn";
                        debitorcredit = transvm.CorporateSearch.DebitOrCredit ?? "Both";
                        currency = transvm.CorporateSearch.Currency ?? "";
                        fromdate = transvm.CorporateSearch.FromDate ?? "";
                        todate = transvm.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                        company = transvm.CorporateSearch.CompanyName ?? "";
                    }
                    else
                    {
                        //retail search
                        branchcode = transvm.RetailSearch.BranchCode ?? "";
                        trantype = transvm.RetailSearch.TranType ?? "TranOwn";
                        debitorcredit = transvm.RetailSearch.DebitOrCredit ?? "Both";
                        currency = transvm.RetailSearch.Currency ?? "";
                        fromdate = transvm.RetailSearch.FromDate ?? "";
                        todate = transvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        //from = Convert.ToDateTime(transvm.FromDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        fromDate = General.ConvertStrToDate(fromdate);
                        //from = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                        from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        //toDate = DateTime.ParseExact(transvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                        //to = Convert.ToDateTime(transvm.ToDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (IsCorporate)
                    //{
                    //    if (toDate < fromDate)
                    //    {
                    //        ModelState.AddModelError("CorporateSearch.FromDate", "From Date must be less than To Date.");
                    //        BindTranDDL();
                    //        BindBranchDDL();
                    //        BindCurrencyDDL();
                    //        return View(transvm);
                    //    }
                    //}
                    //else
                    //{
                    //    if (toDate < fromDate)
                    //    {
                    //        ModelState.AddModelError("RetailSearch.FromDate", "From Date must be less than To Date.");
                    //        BindTranDDL();
                    //        BindBranchDDL();
                    //        BindCurrencyDDL();
                    //        return View(transvm);
                    //    }
                    //}
                    #endregion

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(transvm);
                    }
                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = transvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = transvm.RetailSearch.BranchCode ?? "";
                        }
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                    }
                    DataTable dtResult;

                    if (IsCorporate == true)
                    {
                        dtResult = GetCorTransaction(choosedbranch, trantype, debitorcredit, currency, from, to, IsCorporate, company);
                    }
                    else
                    {
                        dtResult = GetTransaction(choosedbranch, trantype, debitorcredit, currency, from, to, IsCorporate);
                    }

                    //DataTable dtResult = GetTransaction(choosedbranch, trantype, debitorcredit,currency, from, to,IsCorporate);
                    //List<TransactionViewModel> result = GetTransaction(branchcode,trantype, debitorcredit, from, to);
                    List<TransactionViewModel> result = (from DataRow dr in dtResult.Rows
                                                         select new TransactionViewModel()
                                                         {
                                                             TRAN_ID = dr["TRAN_ID"].ToString(),
                                                             FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                             TO_ACCT = dr["TO_ACCT"].ToString(),
                                                             FROM_BRANCH = dr["FROM_BRANCH"].ToString(),
                                                             TO_BRANCH = dr["TO_BRANCH"].ToString(),
                                                             TRAN_AMT = dr["TRAN_AMT"].ToString(),
                                                             CHARGE_CODE = dr["CHARGE_CODE"].ToString(),
                                                             CHARGE_AMT = dr["CHARGE_AMT"].ToString(),
                                                             TOTAL_AMT = dr["TOTAL_AMT"].ToString(),
                                                             //VALUE_DATE = DateTime.ParseExact(dr["VALUE_DATE"].ToString().Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                             VALUE_DATE_DT = (DateTime)dr["VALUE_DATE_DT"],
                                                             TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString().Trim(), "yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture),
                                                             TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                                             FULLNAME = dr["FULLNAME"].ToString(),
                                                             USERNAME = dr["USERNAME"].ToString(),
                                                             MOBILENO = dr["MOBILENO"].ToString(),
                                                             ADDRESS = dr["ADDRESS"].ToString(),
                                                             PayerNRC = dr["PayerNRC"].ToString(),
                                                             CURRENCY = dr["Currency"].ToString(),
                                                             PayeeNRC = dr["PayeeNRC"].ToString(),
                                                             PayeeName = dr["PAYEE_NAME"].ToString(),
                                                             CompanyName = dr["CompanyName"].ToString(),
                                                         }).ToList();
                    transvm.TransactionsVM = result;
                    Session["ReportData"] = transvm.TransactionsVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Transaction report generates successfully.");
                }
                //bind ddls
                BindTranDDL();
                BindBranchDDL();
                BindCurrencyDDL();
                TempData.Keep();
                return View(transvm);

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Transaction Summary
        //Transaction Summary
        public ActionResult TransactionSummary()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                TransactionSummarySearchViewModel transumSearch = new TransactionSummarySearchViewModel();
                transumSearch.RetailSearch = new TransactionSummarySearch();
                transumSearch.CorporateSearch = new TransactionSummarySearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                transumSearch.RetailSearch.FromDate = strToday;
                transumSearch.RetailSearch.ToDate = strToday;

                //for corporate
                transumSearch.CorporateSearch.FromDate = strToday;
                transumSearch.CorporateSearch.ToDate = strToday;

                BindBranchDDL();
                return View(transumSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult TransactionSummary(TransactionSummarySearchViewModel transumsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    string branchcode, choosedbranch, fromdate, todate, company = ""; ;
                    Boolean IsCorporate = transumsvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = transumsvm.CorporateSearch.BranchCode ?? "";
                        choosedbranch = "";
                        fromdate = transumsvm.CorporateSearch.FromDate ?? "";
                        company = transumsvm.CorporateSearch.Company_Name ?? "";
                        todate = transumsvm.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                    }
                    else
                    {
                        //retail search
                        branchcode = transumsvm.RetailSearch.BranchCode ?? "";
                        choosedbranch = "";
                        fromdate = transumsvm.RetailSearch.FromDate ?? "";
                        todate = transumsvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }
                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        from = fromDate.ToString(ConstantValues.Date_TAndUReports);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_TAndUReports);
                    }
                    //if (!String.IsNullOrEmpty(fromdate))
                    //{
                    //    fromDate = General.ConvertStrToDate(fromdate);
                    //    //fromDate = DateTime.ParseExact(transumsvm.FromDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    //    from = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                    //}
                    //if (!String.IsNullOrEmpty(todate))
                    //{
                    //    toDate = General.ConvertStrToDate(todate);
                    //    //toDate = DateTime.ParseExact(transumsvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    //    to = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                    //}

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(transumsvm);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindBranchDDL();
                    //    return View(transumsvm);
                    //}
                    #endregion

                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        var lstbranch = db.Branches.Where(b => b.code == branchcode && b.DEL_FLAG == false).FirstOrDefault();
                        if (IsCorporate)
                        {
                            choosedbranch = transumsvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = transumsvm.RetailSearch.BranchCode ?? "";
                        }
                        //branch_name = "ALL";
                        if (choosedbranch == "")
                        {
                            branch_name = "ALL";
                        }
                        else
                        {
                            branch_name = lstbranch.name.ToString();
                        }
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                        var BCode = db.Branches.Where(b => b.code == choosedbranch && b.DEL_FLAG == false).FirstOrDefault();
                        branch_name = BCode.name.ToString();
                    }
                    Session["BranchName"] = branch_name;
                    DataTable dtResult;
                    if (IsCorporate == true)
                    {
                        dtResult = GetCorTransactionSummary(choosedbranch, from, to, IsCorporate, company);
                    }
                    else
                    {
                        dtResult = GetTransactionSummary(choosedbranch, from, to, IsCorporate);
                    }

                    //DataTable dtResult = GetTransactionSummary(choosedbranch, from, to,IsCorporate);
                    List<TransactionSummaryViewModel> result = new List<TransactionSummaryViewModel>();

                    if (IsCorporate)
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new TransactionSummaryViewModel()
                                  {
                                      TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                      FROM_DATE = transumsvm.CorporateSearch.FromDate.ToString(),
                                      TO_DATE = transumsvm.CorporateSearch.ToDate.ToString(),
                                      TRAN_COUNT = dr["Transaction Count"].ToString(),
                                      TRAN_AMT = dr["Transaction Amount"].ToString(),
                                      CHARGE_AMT = dr["Charges Amount"].ToString(),
                                      TOTAL_AMT = dr["Total Amount"].ToString()
                                  }).ToList();
                    }
                    else
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new TransactionSummaryViewModel()
                                  {
                                      TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                      FROM_DATE = transumsvm.RetailSearch.FromDate.ToString(),
                                      TO_DATE = transumsvm.RetailSearch.ToDate.ToString(),
                                      TRAN_COUNT = dr["Transaction Count"].ToString(),
                                      TRAN_AMT = dr["Transaction Amount"].ToString(),
                                      CHARGE_AMT = dr["Charges Amount"].ToString(),
                                      TOTAL_AMT = dr["Total Amount"].ToString()
                                  }).ToList();
                    }


                    //create new Transaction record for Transfer
                    TransactionSummaryViewModel newtran = new TransactionSummaryViewModel();
                    newtran.TRAN_TYPE = "Transfer";
                    if (IsCorporate)
                    {
                        newtran.FROM_DATE = transumsvm.CorporateSearch.FromDate.ToString();
                        newtran.TO_DATE = transumsvm.CorporateSearch.ToDate.ToString();
                    }
                    else
                    {
                        newtran.FROM_DATE = transumsvm.RetailSearch.FromDate.ToString();
                        newtran.TO_DATE = transumsvm.RetailSearch.ToDate.ToString();
                    }
                    newtran.TRAN_AMT = "";
                    newtran.TOTAL_AMT = "";
                    newtran.CHARGE_AMT = "";
                    newtran.TRAN_COUNT = "";
                    //
                    Decimal tran_amt = 0;
                    Decimal total_amt = 0;
                    Decimal charge_amt = 0;
                    int tran_count = 0;

                    foreach (var tran in result)
                    {
                        if (tran.TRAN_TYPE == "TranOwn" || tran.TRAN_TYPE == "TranOther")
                        {
                            tran_amt += Convert.ToDecimal(tran.TRAN_AMT);
                            total_amt += Convert.ToDecimal(tran.TOTAL_AMT);
                            charge_amt += Convert.ToDecimal(tran.CHARGE_AMT);
                            tran_count += Convert.ToInt32(tran.TRAN_COUNT);

                            newtran.TRAN_AMT = tran_amt.ToString();
                            newtran.TOTAL_AMT = total_amt.ToString();
                            newtran.CHARGE_AMT = charge_amt.ToString();
                            newtran.TRAN_COUNT = tran_count.ToString();
                        }
                    }
                    //result.RemoveAll(result.Where(rs => rs.TRAN_TYPE == "TranOwn" || rs.TRAN_TYPE == "TranOther").FirstOrDefault());
                    //remove Transaction type "" TranOwn" and "TranOther" from list
                    result = result.Except(result.Where(rs => rs.TRAN_TYPE == "TranOwn" || rs.TRAN_TYPE == "TranOther").ToList()).ToList();
                    if (result.Count() != 0)
                    {
                        result.Add(newtran);
                    }
                    transumsvm.TransactionSummaryVM = result;
                    Session["ReportData"] = transumsvm.TransactionSummaryVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Transaction summary report generates successfully.");
                }
                //bind ddls
                BindBranchDDL();
                TempData.Keep();
                return View(transumsvm);

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }


        //get transaction summary data for retail
        private DataTable GetTransactionSummary(string branchcode, string from, string to, bool IsCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_TransactionSummary",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get transaction summary data for corporate
        private DataTable GetCorTransactionSummary(string branchcode, string from, string to, bool IsCorporate, string Company)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_TransactionSummary",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        #endregion

        #region Utility
        //Utility
        public ActionResult Utility()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try

            {
                UtilitySearchViewModel ultSearch = new UtilitySearchViewModel();
                ultSearch.RetailSearch = new UtilitySearch();
                ultSearch.CorporateSearch = new UtilitySearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                ultSearch.RetailSearch.FromDate = strToday;
                ultSearch.RetailSearch.ToDate = strToday;

                //for corporate
                ultSearch.CorporateSearch.FromDate = strToday;
                ultSearch.CorporateSearch.ToDate = strToday;

                BindBranchDDL();
                BindTranDDL();
                bindUserType();
                bindCorUserType();
                //BindBillerDDL();
                return View(ultSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Utility(UtilitySearchViewModel ultvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    string branchcode, trantype, biller, choosedbranch, fromdate, todate, company = "", usertype;
                    Boolean IsCorporate = ultvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = ultvm.CorporateSearch.BranchCode ?? "";
                        trantype = "BillPayment";
                        choosedbranch = "";
                        biller = ultvm.CorporateSearch.UtilityType ?? "";
                        fromdate = ultvm.CorporateSearch.FromDate ?? "";
                        todate = ultvm.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                        company = ultvm.CorporateSearch.Company_Name ?? "";
                        usertype = ultvm.CorporateSearch.UserType ?? "";
                    }
                    else
                    {
                        //retail search
                        branchcode = ultvm.RetailSearch.BranchCode ?? "";
                        trantype = "BillPayment";
                        choosedbranch = "";
                        biller = ultvm.RetailSearch.UtilityType ?? "";
                        fromdate = ultvm.RetailSearch.FromDate ?? "";
                        todate = ultvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                        usertype = ultvm.RetailSearch.UserType ?? "";
                    }

                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        //fromDate= DateTime.ParseExact(ultvm.FromDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                        from = fromDate.ToString(ConstantValues.Date_TAndUReports);
                        //from = Convert.ToDateTime(ultvm.FromDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        //toDate= DateTime.ParseExact(ultvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_TAndUReports);
                        //to = Convert.ToDateTime(ultvm.ToDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(ultvm);
                    }
                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = ultvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = ultvm.RetailSearch.BranchCode ?? "";
                        }
                        br_code = true;
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                        br_code = false;
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindBranchDDL();
                    //    BindTranDDL();
                    //    BindBillerDDL();
                    //    return View(ultvm);
                    //}
                    #endregion

                    Session["BranchCode"] = br_code;
                    DataTable dtResult = new DataTable();
                    DataTable dtBiller = new DataTable();
                    DataTable dtUtility = new DataTable();
                    DataTable dtOtherUtility = new DataTable();
                    List<Utilities> utilities = db.Utilities.Where(u => u.Active == "1" & u.Utility_Type == "GiftCard").ToList();
                    List<TBL_Biller> billers = db.TBL_Billers.Where(x => x.Active == true).ToList();

                    // get report data by biller filter
                    if (usertype == "MB")
                    {
                        //foreach(Utilities u in utilities)
                        //{
                        //    if(biller == u.Biller_Code)
                        //    {
                        if (IsCorporate == true)
                        {
                            dtUtility = GetCorUtility(choosedbranch, trantype, biller, from, to, IsCorporate, company, usertype);
                            dtOtherUtility = GetCorOtherUtility(choosedbranch, trantype, biller, from, to, IsCorporate, company, usertype);
                            dtResult = dtUtility.Copy();
                            dtResult.Merge(dtOtherUtility, true);
                        }
                        else
                        {
                            dtUtility = GetUtility(choosedbranch, trantype, biller, from, to, IsCorporate, usertype);
                            dtOtherUtility = GetOtherUtility(choosedbranch, trantype, biller, from, to, IsCorporate, usertype);
                            dtResult = dtUtility.Copy();
                            dtResult.Merge(dtOtherUtility, true);
                        }
                        //    }
                        //}
                    }
                    if (usertype == "CB" || usertype == "IB")
                    {

                        //foreach (TBL_Biller b in billers)
                        //{
                        //    if (biller == b.BillerCode)
                        //    {
                        if (IsCorporate == true)
                        {
                            dtResult = GetCorBiller(choosedbranch, trantype, biller, from, to, IsCorporate, company, usertype);
                        }
                        else
                        {
                            dtResult = GetBiller(choosedbranch, trantype, biller, from, to, IsCorporate, usertype);
                        }
                        //    }
                        //}
                    }
                    //else
                    //{
                    //    if (IsCorporate == true)
                    //    {
                    //        dtUtility = GetCorUtility(choosedbranch, trantype, biller, from, to, IsCorporate, company,usertype);
                    //        dtBiller = GetCorBiller(choosedbranch, trantype, biller, from, to, IsCorporate, company);
                    //    }
                    //    else
                    //    {
                    //        dtUtility = GetUtility(choosedbranch, trantype, biller, from, to, IsCorporate,usertype);
                    //        dtBiller = GetBiller(choosedbranch, trantype, biller, from, to, IsCorporate);
                    //    }
                    //}

                    //Utility dt and Biller dt Union
                    //dtResult = dtUtility.Copy();
                    //dtResult.Merge(dtBiller, true);

                    //DataTable dtResult = GetUtility(choosedbranch, trantype, biller, from, to,IsCorporate);
                    List<UtilityViewModel> result = (from DataRow dr in dtResult.Rows
                                                     select new UtilityViewModel()
                                                     {
                                                         TRAN_ID = dr["TRAN_ID"].ToString(),
                                                         FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                         FROM_BRANCH = dr["FROM_BRANCH"].ToString(),
                                                         VALUE_DATE_DT = (DateTime)dr["VALUE_DATE_DT"],
                                                         TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString(), "yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture),
                                                         TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                                         FULLNAME = dr["USERNAME"].ToString(),
                                                         MOBILENO = dr["MOBILENO"].ToString(),
                                                         NetAmount = dr["NetAmount"].ToString(),
                                                         ChargesAmount = dr["CHARGE_AMT"].ToString(),
                                                         DiscountAmount = dr["DiscountAmount"].ToString(),
                                                         TotalAmount = dr["TotalAmount"].ToString(),
                                                         PartnerAmount = dr["PartnerAmount"].ToString(),
                                                         RespDesc = dr["RespDesc"].ToString(),
                                                         requestDatetime = DateTime.Parse(dr["RequestDatetime"].ToString()),
                                                         Status = dr["Status"].ToString(),
                                                         BillerCode = dr["BillerCode"].ToString(),
                                                         BillPhone = dr["BillPhone"].ToString(),
                                                         Deno = dr["Deno"].ToString(),
                                                         Currency = dr["Currency"].ToString(),
                                                         ThirdPartyID = dr["ThirdPartyID"].ToString(),
                                                         CompanyName = dr["CompanyName"].ToString()
                                                     }).ToList();
                    ultvm.UtilityVM = result;
                    Session["ReportData"] = ultvm.UtilityVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Utility report generates successfully.");
                }
                //bind ddl
                BindBranchDDL();
                BindTranDDL();
                bindUserType();
                bindCorUserType();
                //BindBillerDDL();
                TempData.Keep();
                return View(ultvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //get topup for retail 
        private DataTable GetTopUp(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Transactions",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get topup for retail 
        private DataTable GetCorTopUp(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Company)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Transactions",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get utility for retail 
        private DataTable GetUtility(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Usertype)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Utility",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@UserType", Usertype));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        private DataTable GetOtherUtility(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Usertype)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherUtility",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@UserType", Usertype));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }


        //get utility for corporate 
        private DataTable GetCorUtility(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Company, string Usertype)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Utility",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    command.Parameters.Add(new SqlParameter("@UserType", Usertype));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        private DataTable GetCorOtherUtility(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Company, string Usertype)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherUtility",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    command.Parameters.Add(new SqlParameter("@UserType", Usertype));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }


        //get utility for retail 
        private DataTable GetBiller(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string UserType)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_DynamicBiller",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@UserType", UserType));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get utility for retail 
        private DataTable GetCorBiller(string branchcode, string trantype, string biller, string from, string to, bool IsCorporate, string Company, string UserType)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_DynamicBiller",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    if (!String.IsNullOrEmpty(biller))
                    {
                        command.Parameters.Add(new SqlParameter("@Biller", biller));
                    }
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    command.Parameters.Add(new SqlParameter("@UserType", UserType));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }
        #endregion

        #region Utility Summary
        //Utility Summary
        public ActionResult UtilitySummary()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                UtilitySummarySearchViewModel ultsumSearch = new UtilitySummarySearchViewModel();
                ultsumSearch.RetailSearch = new UtilitySummarySearch();
                ultsumSearch.CorporateSearch = new UtilitySummarySearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                ultsumSearch.RetailSearch.FromDate = strToday;
                ultsumSearch.RetailSearch.ToDate = strToday;

                //for corporate
                ultsumSearch.CorporateSearch.FromDate = strToday;
                ultsumSearch.CorporateSearch.ToDate = strToday;

                string BranchCode = GetCurrentAdminBranch();
                TempData["Code"] = BranchCode;
                return View(ultsumSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UtilitySummary(UtilitySummarySearchViewModel ultsumsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    string from = null, to = null;
                    string branchcode, choosedbranch, fromdate, todate;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    Boolean IsCorporate = ultsumsvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = ultsumsvm.CorporateSearch.BranchCode ?? "";
                        choosedbranch = "";
                        fromdate = ultsumsvm.CorporateSearch.FromDate ?? "";
                        todate = ultsumsvm.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                    }
                    else
                    {
                        //retail search
                        branchcode = ultsumsvm.RetailSearch.BranchCode ?? "";
                        choosedbranch = "";
                        fromdate = ultsumsvm.RetailSearch.FromDate ?? "";
                        todate = ultsumsvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }
                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        from = fromDate.ToString(ConstantValues.Date_TAndUReports);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_TAndUReports);
                    }
                    //if (!String.IsNullOrEmpty(fromdate))
                    //{
                    //    fromDate = General.ConvertStrToDate(fromdate);
                    //    //fromDate = DateTime.ParseExact(ultsumsvm.FromDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    //    from = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                    //    //from = fromDate.ToString("dd/MM/yyyy HH:mm:ss");
                    //}
                    //if (!String.IsNullOrEmpty(todate))
                    //{
                    //    toDate = General.ConvertStrToDate(todate);
                    //    //toDate = DateTime.ParseExact(ultsumsvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    //    to = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                    //    //to = toDate.ToString("dd/MM/yyyy HH:mm:ss");
                    //}

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    return View(ultsumsvm);
                    //}
                    #endregion

                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = ultsumsvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = ultsumsvm.RetailSearch.BranchCode ?? "";
                        }
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                    }

                    //DataTable dtResult = GetTransaction(choosedbranch, trantype, debitorcredit, from, to);
                    DataTable dtResult = GetUtilitySummary(choosedbranch, from, to, IsCorporate);
                    List<UtilitySummaryViewModel> result = new List<UtilitySummaryViewModel>();
                    if (IsCorporate)
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new UtilitySummaryViewModel()
                                  {
                                      TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                      FROM_DATE = ultsumsvm.CorporateSearch.FromDate.ToString(),
                                      TO_DATE = ultsumsvm.CorporateSearch.ToDate.ToString(),
                                      BILLER_CODE = dr["BillerCode"].ToString(),
                                      TRAN_COUNT = dr["Tran Count"].ToString(),
                                      TRAN_AMT = Convert.ToDecimal(dr["Customer Tran Amount"].ToString()),
                                      COMMISION_AMT = Convert.ToDecimal(dr["Customer Tran Amount"].ToString()) - Convert.ToDecimal(dr["Partner Amount"].ToString()),
                                      PARTNER_AMT = Convert.ToDecimal(dr["Partner Amount"].ToString())
                                  }).ToList();
                    }
                    else
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new UtilitySummaryViewModel()
                                  {
                                      TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                      FROM_DATE = ultsumsvm.RetailSearch.FromDate.ToString(),
                                      TO_DATE = ultsumsvm.RetailSearch.ToDate.ToString(),
                                      BILLER_CODE = dr["BillerCode"].ToString(),
                                      TRAN_COUNT = dr["Tran Count"].ToString(),
                                      TRAN_AMT = Convert.ToDecimal(dr["Customer Tran Amount"].ToString()),
                                      COMMISION_AMT = Convert.ToDecimal(dr["Customer Tran Amount"].ToString()) - Convert.ToDecimal(dr["Partner Amount"].ToString()),
                                      PARTNER_AMT = Convert.ToDecimal(dr["Partner Amount"].ToString())
                                  }).ToList();
                    }

                    foreach (var item in result)
                    {
                        if (item.TRAN_TYPE == "BillPayment")
                        {
                            item.TRAN_TYPE = "Utility";
                        }
                        else if (item.TRAN_TYPE == "TopUp")
                        {
                            item.TRAN_TYPE = "Mobile TopUp";
                        }
                        if (item.PARTNER_AMT == 0)
                        {
                            item.COMMISION_AMT = 0;
                        }
                    }

                    ultsumsvm.UtilitySummaryVM = result;
                    Session["ReportData"] = ultsumsvm.UtilitySummaryVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Utility summary report generates successfully.");
                }
                TempData.Keep();
                return View(ultsumsvm);

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //get utility summary data
        private DataTable GetUtilitySummary(string choosedbranch, string from, string to, bool IsCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_UtilitySummary",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", choosedbranch));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }
        #endregion

        #region Remittance
        public ActionResult Remittance()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                RemittanceSearchViewModel remitSearch = new RemittanceSearchViewModel();
                remitSearch.RetailSearch = new RemittanceSearch();
                remitSearch.CorporateSearch = new RemittanceSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                remitSearch.RetailSearch.FromDate = strToday;
                remitSearch.RetailSearch.ToDate = strToday;

                //for corporate
                remitSearch.CorporateSearch.FromDate = strToday;
                remitSearch.CorporateSearch.ToDate = strToday;

                BindBranchDDL();
                return View(remitSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Remittance(RemittanceSearchViewModel remitsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    string branchcode, trantype, debitorcredit, currency, choosedbranch, fromdate, todate, company = "";
                    Boolean IsCorporate = remitsvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = remitsvm.CorporateSearch.BranchCode ?? "";
                        trantype = "Remit";
                        debitorcredit = "Both";
                        currency = "";
                        choosedbranch = "";
                        fromdate = remitsvm.CorporateSearch.FromDate ?? "";
                        todate = remitsvm.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                        company = remitsvm.CorporateSearch.CompanyName ?? "";
                    }
                    else
                    {
                        //retail search
                        branchcode = remitsvm.RetailSearch.BranchCode ?? "";
                        trantype = "Remit";
                        debitorcredit = "Both";
                        currency = "";
                        choosedbranch = "";
                        fromdate = remitsvm.RetailSearch.FromDate ?? "";
                        todate = remitsvm.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }

                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                    }

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(remitsvm);
                    }
                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = remitsvm.CorporateSearch.BranchCode ?? "";
                        }
                        else
                        {
                            choosedbranch = remitsvm.RetailSearch.BranchCode ?? "";
                        }
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindBranchDDL();
                    //    return View(remitsvm);
                    //}
                    #endregion

                    DataTable dtResult;
                    if (IsCorporate == true)
                    {
                        dtResult = GetCorTransaction(choosedbranch, trantype, debitorcredit, currency, from, to, IsCorporate, company);
                    }
                    else
                    {
                        dtResult = GetTransaction(choosedbranch, trantype, debitorcredit, currency, from, to, IsCorporate);
                    }

                    //DataTable dtResult = GetTransaction(choosedbranch, trantype, debitorcredit,currency, from, to,IsCorporate);
                    List<RemittanceViewModel> result = (from DataRow dr in dtResult.Rows
                                                        select new RemittanceViewModel()
                                                        {
                                                            TRAN_ID = dr["TRAN_ID"].ToString(),
                                                            FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                            TO_ACCT = dr["TO_ACCT"].ToString(),
                                                            FROM_BRANCH = dr["FROM_BRANCH"].ToString(),
                                                            TO_BRANCH = dr["TO_BRANCH"].ToString(),
                                                            TRAN_AMT = dr["TRAN_AMT"].ToString(),
                                                            CHARGE_CODE = dr["CHARGE_CODE"].ToString(),
                                                            CHARGE_AMT = dr["CHARGE_AMT"].ToString(),
                                                            TOTAL_AMT = dr["TOTAL_AMT"].ToString(),
                                                            //VALUE_DATE = DateTime.ParseExact(dr["VALUE_DATE"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                                                            VALUE_DATE_DT = (DateTime)dr["VALUE_DATE_DT"],
                                                            TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString(), "yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture),
                                                            PayeeName = dr["PAYEE_NAME"].ToString(),
                                                            PayeeNRC = dr["PayeeNRC"].ToString(),
                                                            PayeePhone = dr["PAYEE_PHONE"].ToString(),
                                                            TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
                                                            FULLNAME = dr["FULLNAME"].ToString(),
                                                            ADDRESS = dr["ADDRESS"].ToString(),
                                                            PayerNRC = dr["PayerNRC"].ToString(),
                                                            USERNAME = dr["USERNAME"].ToString(),
                                                            CompanyName = dr["CompanyName"].ToString()

                                                        }).ToList();
                    remitsvm.RemittancesVM = result;
                    Session["ReportData"] = remitsvm.RemittancesVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Remittance report generates successfully.");
                }
                //bind ddls
                BindBranchDDL();
                TempData.Keep();
                return View(remitsvm);

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Transfer Own
        //public ActionResult TransferOwn()
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        TransferOwnSearchViewModel tranownSearch = new TransferOwnSearchViewModel();
        //        BindBranchDDL();
        //        return View(tranownSearch);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName,actionName, ex);
        //        return View("Error");
        //    }
        //}

        //[HttpPost]
        //public ActionResult TransferOwn(TransferOwnSearchViewModel tranownsvm)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string from = null, to = null;
        //            //search
        //            string branchcode = tranownsvm.BranchCode ?? "";
        //            string trantype = "TranOwn";
        //            string debitorcredit = "Both";
        //            string choosedbranch = "";
        //            if (!String.IsNullOrEmpty(tranownsvm.FromDate))
        //            {
        //                from = Convert.ToDateTime(tranownsvm.FromDate).ToString("yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture);
        //            }
        //            if (!String.IsNullOrEmpty(tranownsvm.ToDate))
        //            {
        //                to = Convert.ToDateTime(tranownsvm.ToDate).ToString("yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture);
        //            }

        //            //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
        //            if (TempData["Code"] == null || TempData["Code"].ToString() == "")
        //            {
        //                ModelState.AddModelError("", "Unknown Branch User");
        //                return View(tranownsvm);
        //            }
        //            string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
        //            if (branchCode == "9999")
        //            {
        //                choosedbranch = tranownsvm.BranchCode ?? "";
        //            }
        //            else
        //            {
        //                //***must be sent Branch code for filtering if branch users 
        //                choosedbranch = branchCode;     //if branch admin, filter by only that branch
        //            }

        //            DataTable dtResult = GetTransaction(choosedbranch, trantype, debitorcredit, from, to);
        //            List<TransferOwnViewModel> result = (from DataRow dr in dtResult.Rows
        //                                                 select new TransferOwnViewModel()
        //                                                 {
        //                                                     TRAN_ID = dr["TRAN_ID"].ToString(),
        //                                                     FROM_ACCT = dr["FROM_ACCT"].ToString(),
        //                                                     TO_ACCT = dr["TO_ACCT"].ToString(),
        //                                                     FROM_BRANCH = dr["FROM_BRANCH"].ToString(),
        //                                                     TO_BRANCH = dr["TO_BRANCH"].ToString(),
        //                                                     TRAN_AMT = dr["TRAN_AMT"].ToString(),
        //                                                     CHARGE_CODE = dr["CHARGE_CODE"].ToString(),
        //                                                     CHARGE_AMT = dr["CHARGE_AMT"].ToString(),
        //                                                     TOTAL_AMT = dr["TOTAL_AMT"].ToString(),
        //                                                     VALUE_DATE = DateTime.ParseExact(dr["VALUE_DATE"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
        //                                                     TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString(), "yyyy-MM-ddTHH:mm:ss.", CultureInfo.InvariantCulture),
        //                                                     PayeeNRC = dr["PayeeNRC"].ToString(),
        //                                                     TRAN_TYPE = dr["TRAN_TYPE"].ToString(),
        //                                                     FULLNAME = dr["FULLNAME"].ToString(),
        //                                                     MOBILENO = dr["MOBILENO"].ToString(),
        //                                                     ADDRESS = dr["ADDRESS"].ToString(),
        //                                                     PayerNRC = dr["PayerNRC"].ToString()
        //                                                 }).ToList();
        //            tranownsvm.TransferOwnVM = result;
        //            Session["ReportData"] = tranownsvm.TransferOwnVM;
        //            //save log
        //            log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Transfer Own report generates successfully.");
        //        }
        //        //bind ddls
        //        BindBranchDDL();
        //        return View(tranownsvm);

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}
        #endregion

        #region Feedback
        // Comment
        public ActionResult Feedback()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CommentSearchViewModel cmtSearch = new CommentSearchViewModel();
                cmtSearch.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                cmtSearch.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                BindReportTypeDDL();
                return View(cmtSearch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Feedback(CommentSearchViewModel cmtvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string name = cmtvm.NAME ?? "";
                    string reporttype = cmtvm.REPORTTYPE ?? "";
                    string comment = cmtvm.comment ?? "";
                    name = name.ToLower().Trim();
                    reporttype = reporttype.ToLower().Trim();
                    comment = comment.ToLower().Trim();
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    //string[] strdate = cmtvm.daterange.Split('~');
                    //if (!string.IsNullOrEmpty(strdate[0].Trim()))
                    //{
                    //    fromDate = General.ConvertStrToDate(strdate[0]);
                    //}
                    //if (!string.IsNullOrEmpty(strdate[1].Trim()))
                    //{
                    //    toDate = General.ConvertStrToDate(strdate[1]).AddDays(1); 
                    if (!String.IsNullOrEmpty(cmtvm.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(cmtvm.FromDate);
                        //fromDate = DateTime.ParseExact(cmtvm.FromDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(cmtvm.ToDate))
                    {
                        toDate = General.ConvertStrToDate(cmtvm.ToDate);
                        //toDate = DateTime.ParseExact(cmtvm.ToDate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindReportTypeDDL();
                    //    return View(cmtvm);
                    //}
                    #endregion
                    //Add One Day for toDate
                    toDate = toDate.AddDays(1);

                    var feedback = (from c in db.COMMENTs
                                    join u in db._Users on c.CreatedUserID equals u.USERID into tbluser
                                    from tblrm in tbluser.DefaultIfEmpty()
                                        //where c.ACTIVE == "1" & c.COUNT == true & c.NAME.Contains(name) & c.REPORTTYPE.Contains(reporttype) & c.comment.Contains(comment)
                                    where c.ACTIVE == "1" & c.COUNT == true & tblrm.USERNAME.Contains(name) & c.REPORTTYPE.Contains(reporttype) & c.comment.Contains(comment)
                                    & c.CreatedDateTime >= fromDate & c.CreatedDateTime < toDate
                                    //orderby c.CreatedDateTime descending
                                    orderby c.CreatedDateTime ascending
                                    select new CommentViewModel
                                    {
                                        REPORTTYPE = c.REPORTTYPE,
                                        comment = c.comment,
                                        NAME = tblrm.USERNAME,
                                        FULLNAME = tblrm.FULLNAME,
                                        EMAIL = tblrm.EMAIL,
                                        MOBILENO = tblrm.MOBILENO,
                                        ADDRESS = tblrm.ADDRESS,
                                        CreatedDateTime = c.CreatedDateTime
                                    });
                    List<CommentViewModel> result = feedback.ToList<CommentViewModel>();
                    cmtvm.CommentVM = result;
                    Session["ReportData"] = cmtvm.CommentVM;
                }
                //bind ddl
                BindReportTypeDDL();
                TempData.Keep();
                return View(cmtvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Admin Log
        //Get Admin Log
        public ActionResult AdminLog()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                AdminLogSearchViewModel admlogsvm = new AdminLogSearchViewModel();
                admlogsvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                admlogsvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                BindLogType();
                return View(admlogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AdminLog(AdminLogSearchViewModel admlogsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string username = admlogsvm.Username ?? "";
                    string logtype = admlogsvm.Log_Type ?? "";
                    string desc = admlogsvm.Description ?? "";
                    string detail = admlogsvm.Detail ?? "";
                    username = username.ToLower().Trim();
                    logtype = logtype.ToLower().Trim();
                    desc = desc.ToLower().Trim();
                    detail = detail.ToLower().Trim();
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    //check start and end date
                    if (!String.IsNullOrEmpty(admlogsvm.FromDate))
                    {
                        //fromDate = General.ConvertStrToDateTime(admlogsvm.FromDate);
                        fromDate = DateTime.ParseExact(admlogsvm.FromDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(admlogsvm.ToDate))
                    {
                        //toDate = General.ConvertStrToDateTime(admlogsvm.ToDate);
                        toDate = DateTime.ParseExact(admlogsvm.ToDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check To Date must be greater than From Date
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "To Date must be greater than FromDate.");
                    //    BindLogType();
                    //    return View(admlogsvm);
                    //}
                    #endregion
                    //Add One Minute for toDate
                    toDate = toDate.AddMinutes(1);

                    //log data from adminlog table
                    var adminlog = (from a in logDb.AdminLogs
                                    where (a.LogType.ToLower().Trim().Contains(logtype) || logtype == "")
                                    & (a.Desc.ToLower().Trim().Contains(desc) || desc == "")
                                    & (a.Detail.ToLower().Trim().Contains(detail) || detail == "")
                                    & a.LogDateTime >= fromDate & a.LogDateTime < toDate
                                    select a).Take(1000);

                    //join user table
                    var adminlog_data = (from a in adminlog
                                         join d in db.Users on a.UserID equals d.Id into tbluser
                                         from tblrp in tbluser.DefaultIfEmpty()
                                         where (tblrp.UserName.ToLower().Trim().Contains(username) || username == "")
                                         & a.LogDateTime >= fromDate & a.LogDateTime < toDate
                                         orderby a.ID descending
                                         select new AdminLogViewModel
                                         {
                                             USERNAME = tblrp.UserName,
                                             LOG_DATETIME = a.LogDateTime,
                                             CONTROLLER = a.Controller,
                                             ACTION = a.Action,
                                             LOG_TYPE = a.LogType,
                                             DESCRIPTION = a.Desc,
                                             DETAIL = a.Detail,
                                             LINE_NO = a.LineNo.ToString(),
                                             REQUEST_DATA = a.RequestData
                                         });


                    //var adminlog = (from a in db.AdminLogs
                    //                join d in db.Users on a.UserID equals d.Id into tbluser
                    //                from tblrp in tbluser.DefaultIfEmpty()
                    //                where (a.LogType.ToLower().Trim().Contains(logtype) || logtype == "") & (a.Desc.ToLower().Trim().Contains(desc) || desc == "") & (tblrp.UserName.ToLower().Trim().Contains(username) || username == "") & (a.Detail.ToLower().Trim().Contains(detail) || detail == "")
                    //                & a.LogDateTime >= fromDate & a.LogDateTime < toDate
                    //                orderby a.ID descending
                    //                select new AdminLogViewModel
                    //                {
                    //                    USERNAME = tblrp.UserName,
                    //                    LOG_DATETIME = a.LogDateTime,
                    //                    CONTROLLER = a.Controller,
                    //                    ACTION = a.Action,
                    //                    LOG_TYPE = a.LogType,
                    //                    DESCRIPTION = a.Desc,
                    //                    DETAIL = a.Detail,
                    //                    LINE_NO = a.LineNo.ToString(),
                    //                    REQUEST_DATA = a.RequestData
                    //                });
                    List<AdminLogViewModel> result = adminlog_data.ToList<AdminLogViewModel>();
                    admlogsvm.adminlogVM = result;
                    Session["ReportData"] = admlogsvm.adminlogVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Admin Log report generates successfully.");
                }
                //bind ddl
                BindLogType();
                TempData.Keep();
                return View(admlogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Api Log/User Activity Log
        //Get User Activity Log
        public ActionResult ApiLog()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ApiLogSearchViewModel apilogsvm = new ApiLogSearchViewModel();
                apilogsvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                apilogsvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                UserTypeDDL();
                ReqMethodDDL();
                return View(apilogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Api Log
        [HttpPost]
        public ActionResult ApiLog(ApiLogSearchViewModel apilogsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string reqmethod = apilogsvm.RequestMethod ?? "";
                    string reqpath = apilogsvm.RequestPath ?? "";
                    string usertype = apilogsvm.UserType ?? "";
                    string username = apilogsvm.Username ?? "";
                    reqmethod = reqmethod.ToLower().Trim();
                    reqpath = reqpath.ToLower().Trim();
                    usertype = usertype.ToLower().Trim();
                    username = username.ToLower().Trim();
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(apilogsvm.FromDate))
                    {
                        //fromDate = General.ConvertStrToDateTime(apilogsvm.FromDate);
                        fromDate = DateTime.ParseExact(apilogsvm.FromDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(apilogsvm.ToDate))
                    {
                        //toDate = General.ConvertStrToDateTime(apilogsvm.ToDate);
                        toDate = DateTime.ParseExact(apilogsvm.ToDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check To Date must be greater than From Date
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "End Date must be greater than Start Date.");
                    //    ReqMethodDDL();
                    //    UserTypeDDL();
                    //    return View(apilogsvm);
                    //}
                    #endregion
                    //Add One Minute for toDate
                    toDate = toDate.AddMinutes(1);

                    var data = (from a in logDb.Api_Logs
                                where (a.RequestMethod == reqmethod || reqmethod == "")
                                & (a.RequestPath.ToLower().Trim().Contains(reqpath) || reqpath == "")
                                & (a.UserType == usertype || usertype == "")
                                & a.RequestDateTime >= fromDate & a.RequestDateTime < toDate
                                select a).Take(1000);


                    var apilog = (from a in data
                                  join u in db._Users on a.RequestCardID equals u.USERID into tbluser
                                  from tblrp in tbluser.DefaultIfEmpty()
                                  where (tblrp.USERNAME.ToLower().Trim().Contains(username) || username == "")
                                  orderby a.ID descending
                                  select new ApiLogViewModel
                                  {
                                      REQUESTMETHOD = a.RequestMethod,
                                      REQUESTPATH = a.RequestPath,
                                      //REQUESTDATA=a.RequestData,
                                      REQUESTCARDID = tblrp.USERNAME,
                                      REQUESTDATETIME = a.RequestDateTime,
                                      REQUESTFILEPATH = a.RequestFilePath,
                                      LOGMESSAGE = a.LogMessage,
                                      //RESPONSEDATA = a.ResponseData,
                                      RESPONSEDATETIME = a.ResponseDateTime,
                                      RESPONSECODE = a.ResponseCode,
                                      RESPONSEDESCRIPTION = a.ResponseDescription,
                                      USERTYPE = a.UserType
                                  });

                    //var apilog = (from a in db.Api_Logs
                    //              join u in db._Users on a.RequestCardID equals u.USERID into tbluser
                    //              from tblrp in tbluser.DefaultIfEmpty()
                    //              where (a.RequestMethod.ToLower().Trim().Contains(reqmethod) || reqmethod == "")
                    //              & (a.RequestPath.ToLower().Trim().Contains(reqpath) || reqpath == "")
                    //              & (a.UserType.ToLower().Trim().Contains(usertype) || usertype == "")
                    //              & (tblrp.USERNAME.ToLower().Trim().Contains(username) || username == "")
                    //              & a.RequestDateTime >= fromDate & a.RequestDateTime < toDate
                    //              select new ApiLogViewModel
                    //              {
                    //                  REQUESTMETHOD = a.RequestMethod,
                    //                  REQUESTPATH = a.RequestPath,
                    //                  //REQUESTDATA=a.RequestData,
                    //                  REQUESTCARDID = tblrp.USERNAME,
                    //                  REQUESTDATETIME = a.RequestDateTime,
                    //                  REQUESTFILEPATH = a.RequestFilePath,
                    //                  LOGMESSAGE = a.LogMessage,
                    //                  //RESPONSEDATA = a.ResponseData,
                    //                  RESPONSEDATETIME = a.ResponseDateTime,
                    //                  RESPONSECODE = a.ResponseCode,
                    //                  RESPONSEDESCRIPTION = a.ResponseDescription,
                    //                  USERTYPE = a.UserType
                    //              });

                    List<ApiLogViewModel> result = apilog.ToList<ApiLogViewModel>();
                    apilogsvm.apilogVM = result;
                    Session["ReportData"] = apilogsvm.apilogVM;
                }
                //bind ddl
                UserTypeDDL();
                ReqMethodDDL();
                TempData.Keep();
                return View(apilogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Cron Log
        //Get Cron Log
        public ActionResult CronLog()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CronLogSearchViewModel cronlogsvm = new CronLogSearchViewModel();
                cronlogsvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                cronlogsvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                return View(cronlogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult CronLog(CronLogSearchViewModel crnlogsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string taskname = crnlogsvm.TaskName ?? "";
                    taskname = taskname.ToLower().Trim();
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(crnlogsvm.FromDate))
                    {
                        fromDate = DateTime.ParseExact(crnlogsvm.FromDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(crnlogsvm.ToDate))
                    {
                        toDate = DateTime.ParseExact(crnlogsvm.ToDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }

                    #region validation
                    //check To Date must be greater than From Date
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "End Date must be greater than Start Date.");
                    //    return View(crnlogsvm);
                    //}
                    #endregion
                    //Add One Minute for toDate
                    toDate = toDate.AddMinutes(1);

                    var cronlog = (from cl in db.CronLogs
                                   where (cl.TASKNAME.ToLower().Trim().Contains(taskname) || taskname == "")
                                   & cl.STARTDATE >= fromDate & cl.ENDDATE < toDate
                                   select new CronLogViewModel
                                   {
                                       TASKNAME = cl.TASKNAME,
                                       STARTDATE = cl.STARTDATE,
                                       ENDDATE = cl.ENDDATE,
                                       RESPCODE = cl.RESPCODE,
                                       RESPDESC = cl.RESPDESC,
                                       LOGMESSAGE = cl.LOGMESSAGE
                                   });
                    List<CronLogViewModel> result = cronlog.ToList<CronLogViewModel>();
                    crnlogsvm.cronlogVM = result;
                    Session["ReportData"] = crnlogsvm.cronlogVM;
                }
                TempData.Keep();
                return View(crnlogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Schedule Transfer
        //Get Schedule Transfer
        public ActionResult ScheduleTransfer()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ScheduleTransferSearchViewModel schtransfsvm = new ScheduleTransferSearchViewModel();
                //schtransfsvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                //schtransfsvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy");

                schtransfsvm.RetailSearch = new ScheduleTransferSearch();
                schtransfsvm.CorporateSearch = new ScheduleTransferSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                schtransfsvm.RetailSearch.FromDate = strToday;
                schtransfsvm.RetailSearch.ToDate = strToday;

                //for corporate
                schtransfsvm.CorporateSearch.FromDate = strToday;
                schtransfsvm.CorporateSearch.ToDate = strToday;

                return View(schtransfsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Schedule Transfer
        [HttpPost]
        public ActionResult ScheduleTransfer(ScheduleTransferSearchViewModel schtransfsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    //string username = schtransfsvm.Username ?? "";
                    //username = username.ToLower().Trim();
                    string fromdate, todate, username, company = "";
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    Boolean IsCorporate = schtransfsvm.IsCorporate;

                    if (IsCorporate)
                    {
                        //corporate search
                        username = schtransfsvm.CorporateSearch.Username ?? "";
                        username = username.ToLower().Trim();
                        //convert datetime format 
                        if (!String.IsNullOrEmpty(schtransfsvm.CorporateSearch.FromDate))
                        {
                            fromDate = DateTime.ParseExact(schtransfsvm.CorporateSearch.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        }
                        if (!String.IsNullOrEmpty(schtransfsvm.CorporateSearch.ToDate))
                        {
                            toDate = DateTime.ParseExact(schtransfsvm.CorporateSearch.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        }
                        company = schtransfsvm.CorporateSearch.CompanyName ?? "";
                        IsCorporate = true;
                    }
                    else
                    {
                        //retail search
                        username = schtransfsvm.RetailSearch.Username ?? "";
                        username = username.ToLower().Trim();
                        //convert datetime format 
                        if (!String.IsNullOrEmpty(schtransfsvm.RetailSearch.FromDate))
                        {
                            fromDate = DateTime.ParseExact(schtransfsvm.RetailSearch.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        }
                        if (!String.IsNullOrEmpty(schtransfsvm.RetailSearch.ToDate))
                        {
                            toDate = DateTime.ParseExact(schtransfsvm.RetailSearch.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        }
                        IsCorporate = false;
                    }

                    ////convert datetime format 
                    //if (!String.IsNullOrEmpty(schtransfsvm.FromDate))
                    //{
                    //    fromDate = DateTime.ParseExact(schtransfsvm.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //}
                    //if (!String.IsNullOrEmpty(schtransfsvm.ToDate))
                    //{
                    //    toDate = DateTime.ParseExact(schtransfsvm.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //}

                    #region validation
                    //check To Date must be greater than From Date
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "End Date must be greater than Start Date.");
                    //    return View(schtransfsvm);
                    //}
                    #endregion
                    //Add One Minute for toDate
                    toDate = toDate.AddDays(1);
                    List<ScheduleTransferViewModel> result = new List<ScheduleTransferViewModel>();


                    if (IsCorporate == true)
                    {
                        var c_schtransfer = (from st in db.ScheduleTransfers
                                             join u in db._Users on st.UserID equals u.USERID into tbluser
                                             from tblrp in tbluser.DefaultIfEmpty()
                                             join c in db.C_Corporates on tblrp.CORPORATEID equals c.ID into tbtblcoluser
                                             from tblCor in tbtblcoluser.DefaultIfEmpty()
                                             where (tblrp.USERNAME.ToLower().Trim().Contains(username) || username == "")
                                           & st.TransferDate >= fromDate & st.TransferDate < toDate
                                           & (tblCor.COMPANY_NAME.ToLower().Trim().Contains(company) || company == "")
                                           & (tblrp.USER_TYPE == "Corporate")
                                             orderby st.TransferDate ascending
                                             select new ScheduleTransferViewModel
                                             {
                                                 USERID = tblrp.USERNAME,
                                                 SESSIONID = st.SessionID,
                                                 FROM_ACC_NAME = st.FromAccName,
                                                 FROM_ACC_NO = st.FromAccNo,
                                                 TO_ACC_NAME = st.ToAccName,
                                                 TO_ACC_NO = st.ToAccNo,
                                                 AMOUNT = st.Amount,
                                                 TRANID = st.TranID,
                                                 TRANSFER_DATE = st.TransferDate,
                                                 TRANSFER_STATUS = st.TransferStatus,
                                                 DESCRIPTION = st.Description,
                                                 CREATEDDATE = st.CreatedDate,
                                                 CREATEDUSERID = tblrp.USERNAME,
                                                 UPDATEDDATE = st.UpdatedDate,
                                                 UPDATEDUSERID = tblrp.USERNAME,
                                                 CHARGES = st.Charges,
                                                 CompanyName = tblCor.COMPANY_NAME
                                             });
                        result = c_schtransfer.ToList<ScheduleTransferViewModel>();

                    }
                    else
                    {
                        var schtransfer = (from st in db.ScheduleTransfers
                                           join u in db._Users on st.UserID equals u.USERID into tbluser
                                           from tblrp in tbluser.DefaultIfEmpty()
                                           where (tblrp.USERNAME.ToLower().Trim().Contains(username) || username == "")
                                           & st.TransferDate >= fromDate & st.TransferDate < toDate
                                           orderby st.TransferDate ascending
                                           select new ScheduleTransferViewModel
                                           {
                                               USERID = tblrp.USERNAME,
                                               SESSIONID = st.SessionID,
                                               FROM_ACC_NAME = st.FromAccName,
                                               FROM_ACC_NO = st.FromAccNo,
                                               TO_ACC_NAME = st.ToAccName,
                                               TO_ACC_NO = st.ToAccNo,
                                               AMOUNT = st.Amount,
                                               TRANID = st.TranID,
                                               TRANSFER_DATE = st.TransferDate,
                                               TRANSFER_STATUS = st.TransferStatus,
                                               DESCRIPTION = st.Description,
                                               CREATEDDATE = st.CreatedDate,
                                               CREATEDUSERID = tblrp.USERNAME,
                                               UPDATEDDATE = st.UpdatedDate,
                                               UPDATEDUSERID = tblrp.USERNAME,
                                               CHARGES = st.Charges
                                           });

                        result = schtransfer.ToList<ScheduleTransferViewModel>();

                    }

                    //var schtransfer = (from st in db.ScheduleTransfers
                    //                   join u in db._Users on st.UserID equals u.USERID into tbluser
                    //                   from tblrp in tbluser.DefaultIfEmpty()
                    //                   where (tblrp.USERNAME.ToLower().Trim().Contains(username) || username == "")
                    //                   & st.TransferDate >= fromDate & st.TransferDate < toDate
                    //                   orderby st.TransferDate ascending
                    //                   select new ScheduleTransferViewModel
                    //                   {
                    //                       USERID = tblrp.USERNAME,
                    //                       SESSIONID = st.SessionID,
                    //                       FROM_ACC_NAME = st.FromAccName,
                    //                       FROM_ACC_NO = st.FromAccNo,
                    //                       TO_ACC_NAME = st.ToAccName,
                    //                       TO_ACC_NO = st.ToAccNo,
                    //                       AMOUNT = st.Amount,
                    //                       TRANID = st.TranID,
                    //                       TRANSFER_DATE = st.TransferDate,
                    //                       TRANSFER_STATUS = st.TransferStatus,
                    //                       DESCRIPTION = st.Description,
                    //                       CREATEDDATE = st.CreatedDate,
                    //                       CREATEDUSERID = tblrp.USERNAME,
                    //                       UPDATEDDATE = st.UpdatedDate,
                    //                       UPDATEDUSERID = tblrp.USERNAME,
                    //                       CHARGES = st.Charges
                    //                   });
                    //List<ScheduleTransferViewModel> result = schtransfer.ToList<ScheduleTransferViewModel>();
                    schtransfsvm.schtranfVM = result;
                    Session["ReportData"] = schtransfsvm.schtranfVM;
                }
                TempData.Keep();
                return View(schtransfsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Schedule Transfer Log
        //Get Schedule Transfer Log
        public ActionResult ScheduleTransferLog()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ScheduleTransferLogSearchViewModel schtranlogsvm = new ScheduleTransferLogSearchViewModel();
                schtranlogsvm.StartDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                schtranlogsvm.EndDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                return View(schtranlogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Schedule Transfer Log 
        [HttpPost]
        public ActionResult ScheduleTransferLog(ScheduleTransferLogSearchViewModel schtransfLogsvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string taskname = schtransfLogsvm.Taskname ?? "";
                    taskname = taskname.ToLower().Trim();
                    DateTime startDate = DateTime.Now, endDate = DateTime.Now;

                    //convert datetime format 
                    if (!String.IsNullOrEmpty(schtransfLogsvm.StartDate))
                    {
                        startDate = DateTime.ParseExact(schtransfLogsvm.StartDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(schtransfLogsvm.EndDate))
                    {
                        endDate = DateTime.ParseExact(schtransfLogsvm.EndDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    }

                    #region Validation
                    //if (endDate < startDate)
                    //{
                    //    ModelState.AddModelError("startDate", "End Date must be greater than Start Date.");
                    //    return View(schtransfLogsvm);
                    //}
                    #endregion
                    //Add One Minute for toDate
                    endDate = endDate.AddMinutes(1);

                    var schtransferlog = (from stl in db.ScheduleTransferLogs
                                          join st in db.ScheduleTransfers on stl.ScheduleTransferID equals st.ID into tblSchTransf
                                          from tblrp in tblSchTransf.DefaultIfEmpty()
                                          where (stl.TASKNAME.ToLower().Trim().Contains(taskname) || taskname == "")
                                          & stl.STARTDATE >= startDate & stl.ENDDATE < endDate
                                          select new ScheduleTransferLogViewModel
                                          {
                                              SCHEDULETRANSFERID = stl.ScheduleTransferID,
                                              TASKNAME = stl.TASKNAME,
                                              STARTDATE = stl.STARTDATE,
                                              ENDDATE = stl.ENDDATE,
                                              RESPCODE = stl.RESPCODE,
                                              RESPDESC = stl.RESPDESC,
                                              LOGMESSAGE = stl.LOGMESSAGE
                                          });
                    List<ScheduleTransferLogViewModel> result = schtransferlog.ToList<ScheduleTransferLogViewModel>();
                    schtransfLogsvm.schtransflogVM = result;
                    Session["ReportData"] = schtransfLogsvm.schtransflogVM;
                }
                TempData.Keep();
                return View(schtransfLogsvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region Domestic Interbank Transfer
        //Get Domestic Interbank Transfer
        [HttpGet]
        public ActionResult DomesticInterbankTransfer()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DomesticInterbankTransferSearchViewModel domintertransvm = new DomesticInterbankTransferSearchViewModel();
                //domintertransvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                //domintertransvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy");

                domintertransvm.RetailSearch = new DomesticInterbankTransferSearch();
                domintertransvm.CorporateSearch = new DomesticInterbankTransferSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");

                //for retail
                domintertransvm.RetailSearch.FromDate = strToday;
                domintertransvm.RetailSearch.ToDate = strToday;

                //for corporate
                domintertransvm.CorporateSearch.FromDate = strToday;
                domintertransvm.CorporateSearch.ToDate = strToday;

                bindOtherBranch();
                TransactionTypeDDL();
                TransactionStatusDDL();
                CBMStatusDDL();
                BindCBMTran_Type();
                return View(domintertransvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Domestic InterBank Transfer
        [HttpPost]
        public ActionResult DomesticInterbankTransfer(DomesticInterbankTransferSearchViewModel domintertransvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    //string from = null, to = null;
                    //string bankID = domintertransvm.BankName ?? "" ;
                    //string branch = domintertransvm.BranchName ?? "";
                    //string trantype = domintertransvm.Tran_Type ?? "";
                    //string transtatus = domintertransvm.Tran_Status ?? "";
                    //branch = branch.ToLower().Trim();
                    //trantype = trantype.ToLower().Trim();
                    //transtatus = transtatus.ToLower().Trim();
                    Boolean IsCorporate = domintertransvm.IsCorporate;
                    string bankID, branch, trantype, transtatus, fromdate = "", todate = "", company = "", cbmstatus, cbmtrantype;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (IsCorporate)
                    {
                        //corporate search
                        bankID = domintertransvm.CorporateSearch.BankName ?? "";
                        branch = domintertransvm.CorporateSearch.BranchName ?? "";
                        trantype = domintertransvm.CorporateSearch.Tran_Type ?? "";
                        transtatus = domintertransvm.CorporateSearch.Tran_Status ?? "";
                        cbmstatus = domintertransvm.CorporateSearch.CBMStatus ?? "";
                        cbmtrantype = domintertransvm.CorporateSearch.CBMTranType ?? "";
                        branch = branch.ToLower().Trim();
                        trantype = trantype.ToLower().Trim();
                        transtatus = transtatus.ToLower().Trim();
                        cbmstatus = cbmstatus.ToLower().Trim();
                        cbmtrantype = cbmtrantype.ToLower().Trim();
                        if (!String.IsNullOrEmpty(domintertransvm.CorporateSearch.FromDate))
                        {
                            fromDate = General.ConvertStrToDate(domintertransvm.CorporateSearch.FromDate);
                            fromdate = fromDate.ToString(ConstantValues.Date_Format);
                        }
                        if (!String.IsNullOrEmpty(domintertransvm.CorporateSearch.ToDate))
                        {
                            toDate = General.ConvertStrToDate(domintertransvm.CorporateSearch.ToDate);
                            todate = toDate.ToString(ConstantValues.Date_Format);
                        }

                        company = domintertransvm.CorporateSearch.CompanyName ?? "";
                        IsCorporate = true;
                    }
                    else
                    {
                        //retail search
                        bankID = domintertransvm.RetailSearch.BankName ?? "";
                        branch = domintertransvm.RetailSearch.BranchName ?? "";
                        trantype = domintertransvm.RetailSearch.Tran_Type ?? "";
                        transtatus = domintertransvm.RetailSearch.Tran_Status ?? "";
                        cbmstatus = domintertransvm.RetailSearch.CBMStatus ?? "";
                        cbmtrantype = domintertransvm.RetailSearch.CBMTranType ?? "";
                        branch = branch.ToLower().Trim();
                        trantype = trantype.ToLower().Trim();
                        transtatus = transtatus.ToLower().Trim();
                        cbmstatus = cbmstatus.ToLower().Trim();
                        cbmtrantype = cbmtrantype.ToLower().Trim();
                        if (!String.IsNullOrEmpty(domintertransvm.RetailSearch.FromDate))
                        {
                            fromDate = General.ConvertStrToDate(domintertransvm.RetailSearch.FromDate);
                            fromdate = fromDate.ToString(ConstantValues.Date_Format);
                        }
                        if (!String.IsNullOrEmpty(domintertransvm.RetailSearch.ToDate))
                        {
                            toDate = General.ConvertStrToDate(domintertransvm.RetailSearch.ToDate);
                            todate = toDate.ToString(ConstantValues.Date_Format);
                        }
                        IsCorporate = false;
                    }

                    if (fromdate == todate)
                    {
                        todate = toDate.AddHours(23).AddMinutes(59).AddSeconds(59).ToString(ConstantValues.Date_Format);
                    }

                    #region validation
                    //check To Date must be greater than From Date
                    //if (IsCorporate)
                    //{
                    //    if (toDate < fromDate)
                    //    {
                    //        ModelState.AddModelError("CorporateSearch.FromDate", "End Date must be greater than Start Date.");
                    //        bindOtherBranch();
                    //        TransactionTypeDDL();
                    //        TransactionStatusDDL();
                    //        return View(domintertransvm);
                    //    }
                    //}
                    //else
                    //{
                    //    if (toDate < fromDate)
                    //    {
                    //        ModelState.AddModelError("RetailSearch.FromDate", "End Date must be greater than Start Date.");
                    //        bindOtherBranch();
                    //        TransactionTypeDDL();
                    //        TransactionStatusDDL();
                    //        return View(domintertransvm);
                    //    }
                    //}
                    #endregion
                    //Add One Minute for toDate
                    // toDate = toDate.AddMinutes(1);

                    string bankname = "";
                    //get Other Bank Name 
                    if (bankID != "")
                    {
                        int ID = Convert.ToInt32(bankID);
                        var otherBank = db.OtherBanks.Where(x => x.OTHER_BANK_ID == ID).FirstOrDefault();
                        bankname = otherBank.BANK_NAME.ToString();
                        bankname = bankname.ToLower().Trim();
                    }
                    DataTable dtResult;
                    if (IsCorporate == true)
                    {
                        dtResult = GetCorOtherBankTranLog(bankname, branch, trantype, transtatus, fromdate, todate, company, IsCorporate, cbmstatus, cbmtrantype);
                    }
                    else
                    {
                        dtResult = GetOtherBankTranLog(bankname, branch, trantype, transtatus, fromdate, todate, IsCorporate, cbmstatus, cbmtrantype);
                    }

                    //DataTable dtResult = GetOtherBankTranLog(bankname, branch, trantype, transtatus, from, to);
                    List<DomesticInterbankTransferViewModel> result = (from DataRow dr in dtResult.Rows
                                                                       select new DomesticInterbankTransferViewModel()
                                                                       {
                                                                           VALUE_DATE = DateTime.ParseExact(dr["VALUE_DATE"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                                           TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString().Trim(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                                                                           //VALUE_DATE= dr["VALUE_DATE"].ToString(),
                                                                           //= dr["TRAN_DATE"].ToString(),
                                                                           TRAN_ID = dr["TRAN_ID"].ToString(),
                                                                           USERNAME = dr["USERNAME"].ToString(),
                                                                           FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                                           PayerName = dr["FULLNAME"].ToString(),
                                                                           PayeeName = dr["ACCOUNTNAME"].ToString(),
                                                                           //PayeeName = dr["PAYEE_NAME"].ToString(),
                                                                           PAYEE_PHONE = dr["PAYEE_PHONE"].ToString(),
                                                                           TO_ACCT = dr["TO_ACCT"].ToString(),
                                                                           TO_BANK = dr["TO_BANK"].ToString(),
                                                                           BRANCH = dr["TO_BRANCH"].ToString(),
                                                                           AMOUNT = dr["TRAN_AMT"].ToString(),
                                                                           CHARGES = dr["CHARGE_AMT"].ToString(),
                                                                           TOTAL = dr["TOTAL_AMT"].ToString(),
                                                                           CompanyName = dr["CompanyName"].ToString(),
                                                                           Core_status = dr["Core_status"].ToString(),
                                                                           CBMTranId = dr["CBMTranId"].ToString(),
                                                                           CBMStatus = dr["CBMStatus"].ToString(),
                                                                           CBMTran_Type = dr["CBMTran_Type"].ToString()
                                                                       }).ToList();
                    #region comment close
                    //var domintertran = (from otl in db.OtherBankTranLogs
                    //                    join u in db._Users on otl.USERID equals u.USERID into tbluser
                    //                    from tblrp in tbluser.DefaultIfEmpty()
                    //                    where (otl.TO_BANK.Contains(bankname) || bankname == "")
                    //                    & (otl.TO_BRANCH.Contains(branch) || branch == "")
                    //                    & (otl.TRAN_TYPE.Contains(trantype) || trantype == "")
                    //                    //& otl.TRAN_DATE >= fromDate & otl.TRAN_DATE < toDate
                    //                    select new DomesticInterbankTransferViewModel
                    //                    {
                    //                        //VALUE_DATE = DateTime.ParseExact(otl.VALUE_DATE.ToString().Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    //                        //TRAN_DATE = DateTime.ParseExact(otl.TRAN_DATE.ToString().Trim(), "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
                    //                        TRAN_ID = otl.TRAN_ID,
                    //                        USERNAME = tblrp.USERNAME,
                    //                        FROM_ACC = otl.FROM_ACCT,
                    //                        PayerName = tblrp.FULLNAME,
                    //                        PayeeName = otl.PAYEE_NAME,
                    //                        Payee_ACC = otl.TO_ACCT,
                    //                        TO_BANK = otl.TO_BANK,
                    //                        BRANCH = otl.TO_BRANCH,
                    //                        AMOUNT = otl.TRAN_AMT,
                    //                        CHARGES = otl.CHARGE_AMT,
                    //                        TOTAL = otl.TOTAL_AMT
                    //                    });
                    //List<DomesticInterbankTransferViewModel> result = domintertran.ToList<DomesticInterbankTransferViewModel>();
                    #endregion

                    domintertransvm.domintertranVM = result;
                    Session["ReportData"] = domintertransvm.domintertranVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Domestic Interbank Transfer report generates successfully.");
                }
                //bind
                bindOtherBranch();
                TransactionTypeDDL();
                TransactionStatusDDL();
                CBMStatusDDL();
                BindCBMTran_Type();
                TempData.Keep();
                return View(domintertransvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region Schedule Payment Detail  
        public ActionResult SchedulePaymentDetail()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                SchedulePaymentDetailSearchVM scheDetailSearchVM = new SchedulePaymentDetailSearchVM();
                scheDetailSearchVM.RetailSearch = new SchedulePaymentDetailSearch();
                scheDetailSearchVM.CorporateSearch = new SchedulePaymentDetailSearch();
                string strToday = DateTime.Now.ToString("dd-MM-yyyy");
                //for retail
                scheDetailSearchVM.RetailSearch.FromDate = strToday;
                scheDetailSearchVM.RetailSearch.ToDate = strToday;
                //for corporate
                scheDetailSearchVM.CorporateSearch.FromDate = strToday;
                scheDetailSearchVM.CorporateSearch.ToDate = strToday;

                BindBranchDDL();
                BindScheduleTranStatus();
                return View(scheDetailSearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //private List<TopUpViewModel> GetTopUp(TopUpSearchViewModel svm)
        //{
        //    return new List<TopUpViewModel>();
        //}

        [HttpPost]
        public ActionResult SchedulePaymentDetail(SchedulePaymentDetailSearchVM scheDetailVM)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {

                    //search
                    string from = null, to = null;
                    string transtatus, branchcode = "", choosedbranch = "", fromdate, todate, company = "";
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                    Boolean IsCorporate = scheDetailVM.IsCorporate;
                    if (IsCorporate)
                    {
                        //corporate search
                        branchcode = scheDetailVM.CorporateSearch.Branch ?? "";
                        transtatus = scheDetailVM.CorporateSearch.Transaction_Status ?? "ALL";
                        fromdate = scheDetailVM.CorporateSearch.FromDate ?? "";
                        todate = scheDetailVM.CorporateSearch.ToDate ?? "";
                        IsCorporate = true;
                        company = scheDetailVM.CorporateSearch.CompanyName ?? "";
                    }
                    else
                    {
                        //retail search
                        branchcode = scheDetailVM.RetailSearch.Branch ?? "";
                        transtatus = scheDetailVM.RetailSearch.Transaction_Status ?? "ALL";
                        fromdate = scheDetailVM.RetailSearch.FromDate ?? "";
                        todate = scheDetailVM.RetailSearch.ToDate ?? "";
                        IsCorporate = false;
                    }

                    if (!String.IsNullOrEmpty(fromdate))
                    {
                        fromDate = General.ConvertStrToDate(fromdate);
                        from = from = fromDate.ToString("MM-dd-yyyy");
                    }
                    if (!String.IsNullOrEmpty(todate))
                    {
                        toDate = General.ConvertStrToDate(todate);
                        to = toDate.ToString("MM-dd-yyyy");
                    }

                    //if (!String.IsNullOrEmpty(scheDetailVM.FromDate))
                    //{
                    //    fromDate = General.ConvertStrToDate(scheDetailVM.FromDate);
                    //    /*from = fromDate.ToString("dd-MM-yyyy");*///Stored Procedure accepts only 'dd-MM-yyyy'
                    //    from = fromDate.ToString("MM-dd-yyyy");
                    //}
                    //if (!String.IsNullOrEmpty(scheDetailVM.ToDate))
                    //{
                    //    toDate = General.ConvertStrToDate(scheDetailVM.ToDate);
                    //    /*to = toDate.ToString("dd-MM-yyyy");*///Stored Procedure accepts only 'dd-MM-yyyy'
                    //    to = toDate.ToString("MM-dd-yyyy");
                    //}

                    #region validation
                    //check toDate is greater than fromDate or not
                    //if (toDate < fromDate)
                    //{
                    //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                    //    BindScheduleTranStatus();
                    //    return View(scheDetailVM);
                    //}
                    #endregion

                    //branch if HeadOffice user => bind ddl and search, if branch user => show only that branch's data
                    if (TempData["Code"] == null || TempData["Code"].ToString() == "")
                    {
                        ModelState.AddModelError("", "Unknown Branch User");
                        return View(scheDetailVM);
                    }

                    string branchCode = TempData["Code"].ToString();//logged in Admin's Branch code
                    if (branchCode == "9999")
                    {
                        if (IsCorporate)
                        {
                            choosedbranch = scheDetailVM.CorporateSearch.Branch ?? "";
                        }
                        else
                        {
                            choosedbranch = scheDetailVM.RetailSearch.Branch ?? "";
                        }
                    }
                    else
                    {
                        //***must be sent Branch code for filtering if branch users 
                        choosedbranch = branchCode;     //if branch admin, filter by only that branch
                    }
                    DataTable dtResult;

                    if (IsCorporate == true)
                    {
                        dtResult = GetCorSchedulePaymentDetail(choosedbranch, transtatus, from, to, IsCorporate, company);
                    }
                    else
                    {
                        dtResult = GetSchedulePaymentDetail(choosedbranch, transtatus, from, to);
                    }

                    List<SchedulePaymentDetailVM> result = (from DataRow dr in dtResult.Rows
                                                            select new SchedulePaymentDetailVM
                                                            {
                                                                VALUE_DATE = dr["VALUE_DATE"].ToString(),
                                                                //VALUE_DATE_DT=(DateTime)dr["VALUE_DATE_DT"],
                                                                //ScheduleDate = dr["ScheduleDate"].ToString(),
                                                                ScheduleDate = dr["ScheduleDate"].ToString(),//(String.IsNullOrEmpty(dr["ScheduleDate"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dr["ScheduleDate"].ToString())),
                                                                TRAN_DATE = (String.IsNullOrEmpty(dr["TRAN_DATE"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dr["TRAN_DATE"].ToString())),
                                                                ScheduleID = dr["ScheduleID"].ToString(),
                                                                TranID = dr["TranID"].ToString(),
                                                                FromAccNo = dr["FromAccNo"].ToString(),
                                                                ToAccNo = dr["ToAccNo"].ToString(),
                                                                USERNAME = dr["USERNAME"].ToString(),
                                                                PayerName = dr["PayerName"].ToString(),
                                                                PayerNRC = dr["PayerNRC"].ToString(),
                                                                PayeeName = dr["PayeeName"].ToString(),
                                                                TRAN_AMT = dr["TRAN_AMT"].ToString(),
                                                                CHARGE_AMT = dr["CHARGE_AMT"].ToString(),
                                                                TOTAL_AMT = dr["TOTAL_AMT"].ToString(),
                                                                Status = dr["Status"].ToString(),
                                                                CompanyName = dr["CompanyName"].ToString()
                                                            }).ToList();
                    scheDetailVM.schschedulePDVM = result;
                    Session["ReportData"] = scheDetailVM.schschedulePDVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Transaction report generates successfully.");
                }
                //bind ddls
                BindScheduleTranStatus();
                BindBranchDDL();
                return View(scheDetailVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Corporate
        public ActionResult Corporate()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CorporateSearchViewModel corsearchvm = new CorporateSearchViewModel();
                corsearchvm.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                corsearchvm.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                BindCorporateType();
                return View(corsearchvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Corporate(CorporateSearchViewModel corsearchvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string company = corsearchvm.Company_Name ?? "";
                    string corporateid = corsearchvm.Corporate_Id ?? "";
                    string cifid = corsearchvm.CIFID ?? "";
                    string type = corsearchvm.Corporate_Type ?? "";
                    bool? isVIP = null;
                    company = company.ToLower().Trim();
                    corporateid = corporateid.ToLower().Trim();
                    cifid = cifid.ToLower().Trim();
                    //get corporate type
                    if (!String.IsNullOrEmpty(type))
                    {
                        isVIP = true ? "1" == type : "0" == type;
                    }
                    //else
                    //{
                    //    isVIP = false;
                    //}
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(corsearchvm.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(corsearchvm.FromDate);
                        //from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(corsearchvm.ToDate))
                    {
                        toDate = General.ConvertStrToDate(corsearchvm.ToDate);
                        //to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    if (toDate < fromDate)
                    {
                        ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                        BindCorporateType();
                        return View(corsearchvm);
                    }
                    #endregion

                    //Add One Minute for toDate
                    toDate = toDate.AddDays(1);

                    List<CorporateViewModel> result = (from c in db.C_Corporates
                                                       join r in db.C_Req_Corporates on c.ID equals r.REQ_CORPORATEID into tblreq
                                                       from tblrc in tblreq.DefaultIfEmpty()
                                                       join t in db.C_CorporateTranRules on c.ID equals t.CorporateId into tblrule
                                                       from tbltran in tblrule.DefaultIfEmpty()
                                                       where c.DEL_FLAG == false & (c.COMPANY_NAME.ToLower().Contains(company) || c.COMPANY_NAME == "")
                                                       & (c.CORPORATEID.ToLower().Contains(corporateid) || c.CORPORATEID == "")
                                                       & (c.CIFID.ToLower().Contains(cifid) || c.CIFID == "")
                                                       & (isVIP == null || c.ISVIP == isVIP)
                                                       & (tblrc.RequestedDate >= fromDate & tblrc.RequestedDate < toDate)
                                                       orderby tblrc.RequestedDate ascending
                                                       select new CorporateViewModel
                                                       {
                                                           //CREATEDDATE = DateTime.ParseExact(tblrc.RequestedDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                           //APPROVEDDATE = DateTime.ParseExact(tblrc.CheckedDate.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                                                           CREATEDDATE = tblrc.RequestedDate,
                                                           APPROVEDDATE = tblrc.CheckedDate,
                                                           CIFID = c.CIFID,
                                                           COMPANY_NAME = c.COMPANY_NAME,
                                                           CORPORATEID = c.CORPORATEID,
                                                           PHONE_NO = c.COMPANY_PHONE,
                                                           EMAIL = c.COMPANY_EMAIL,
                                                           CORPORATE_TYPE = c.ISVIP,
                                                           TRAN_LIMIT = tbltran.Value.ToString(),
                                                           BULK_CHARGES_TYPE = c.IS_FIXRATE_BULK_CHARGES,
                                                           BULK_CHARGES_FIX_RATE = c.BULK_CHARGES_FIX_RATE_N_PERCENT.ToString(),
                                                           MAKER = tblrc.VrMaker.UserName,
                                                           CHECKER = tblrc.VrChecker.UserName
                                                       }).ToList();
                    corsearchvm.corporateVM = result;
                    Session["ReportData"] = corsearchvm.corporateVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Corporate report generates successfully.");
                }
                BindCorporateType();
                return View(corsearchvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Corporate User
        public ActionResult CorporateUser()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CorporateUserSearchViewModel corusersearchVM = new CorporateUserSearchViewModel();
                corusersearchVM.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                corusersearchVM.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                return View(corusersearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult CorporateUser(CorporateUserSearchViewModel corusersearchvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string company = corusersearchvm.Company_Name ?? "";
                    string username = corusersearchvm.Username ?? "";
                    string corporateid = corusersearchvm.Corporate_Id ?? "";
                    company = company.ToLower().Trim();
                    username = username.ToLower().Trim();
                    corporateid = corporateid.ToLower().Trim();
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(corusersearchvm.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(corusersearchvm.FromDate);
                        from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(corusersearchvm.ToDate))
                    {
                        toDate = General.ConvertStrToDate(corusersearchvm.ToDate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    if (toDate < fromDate)
                    {
                        ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                        return View(corusersearchvm);
                    }
                    #endregion

                    //Add One Minute for toDate
                    toDate = toDate.AddDays(1);
                    var admin = db.Users.Where(u => u.DEL_FLAG == false).ToList();
                    var user = db._Users.Where(u => u.DEL_STATUS == false).ToList();

                    List<C_Req_UserInRole> roles = db.C_Req_UserInRoles.Where(u => u.ReqUserID > 0).ToList();
                    var role = roles.GroupBy(cc => cc.ReqUserID).Select(dd => new { ReqUserID = dd.Key, Role = string.Join(",", dd.Select(ee => ee.VrRole.Name).ToList()) });
                    List<CorporateUserViewModel> result = (from u in db.C_Req_Users
                                                               //join r in role on u.ID equals r.ReqUserID into tbluser
                                                               //from tblrole in tbluser.DefaultIfEmpty()
                                                           where u.STATUS == 1
                                                           & u.ISNEW == true
                                                           & (u.VrCorporate.COMPANY_NAME.ToLower().Contains(company) || u.VrCorporate.COMPANY_NAME == "")
                                                           & (u.VrCorporate.CORPORATEID.ToLower().Contains(corporateid) || u.VrCorporate.CORPORATEID == "")
                                                           & (u.USERNAME.ToLower().Contains(username) || u.USERNAME == "")
                                                           & (u.REQUESTEDDATE >= fromDate & u.REQUESTEDDATE < toDate)
                                                           orderby u.REQUESTEDDATE ascending
                                                           select new CorporateUserViewModel
                                                           {
                                                               ID = u.ID,
                                                               CREATEDDATE = u.REQUESTEDDATE,
                                                               APPROVEDDATE = u.CHECKEDDATE,
                                                               COMPANY_NAME = u.VrCorporate.COMPANY_NAME,
                                                               CORPORATEID = u.VrCorporate.CORPORATEID,
                                                               USERNAME = u.USERNAME,
                                                               PHONE_NO = u.MOBILENO,
                                                               EMAIL = u.EMAIL,
                                                               //ROLE= "",
                                                               DEPARTMENT = u.VrDepartment.NAME,
                                                               POSITION = u.VrPosition.NAME

                                                           }).ToList();


                    foreach (CorporateUserViewModel cuser in result)
                    {
                        cuser.ROLE = role.Where(r => r.ReqUserID == cuser.ID).Select(s => s.Role).FirstOrDefault();
                    }
                    //to get roles (,)
                    //foreach (CorporateUserViewModel c_uservm in result)
                    //{
                    //    c_uservm.Vr
                    //    string roles = c_uservm.
                    //}

                    corusersearchvm.coruserVM = result;
                    Session["ReportData"] = corusersearchvm.coruserVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Corporate user report generates successfully.");
                }
                return View(corusersearchvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Bulk Payment
        //Get Bulk Payment
        public ActionResult BulkPayment()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                BulkPaymentSearchViewModel bulkpaymentsearchVM = new BulkPaymentSearchViewModel();
                bulkpaymentsearchVM.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                bulkpaymentsearchVM.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                BulkStatus();
                BulkTranType();
                return View(bulkpaymentsearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult BulkPayment(BulkPaymentSearchViewModel bulkpaymentsearchVM)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string company = bulkpaymentsearchVM.Company_Name ?? "";
                    string corporateid = bulkpaymentsearchVM.Corporate_ID ?? "";
                    string debit_acct = bulkpaymentsearchVM.Debit_Acct ?? "";
                    string debit_amt = bulkpaymentsearchVM.Debit_Amt ?? "";
                    string status = bulkpaymentsearchVM.Status ?? "";
                    string trantype = bulkpaymentsearchVM.TranType ?? "";
                    string bfilename = bulkpaymentsearchVM.BulkFileName ?? "";
                    company = company.ToLower().Trim();
                    corporateid = corporateid.ToLower().Trim();
                    debit_acct = debit_acct.ToLower().Trim();
                    debit_amt = debit_amt.ToLower().Trim();
                    status = status.ToLower().Trim();
                    trantype = trantype.ToLower().Trim();
                    bfilename = bfilename.ToLower().Trim();
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(bulkpaymentsearchVM.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(bulkpaymentsearchVM.FromDate);
                        from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(bulkpaymentsearchVM.ToDate))
                    {
                        toDate = General.ConvertStrToDate(bulkpaymentsearchVM.ToDate);
                        to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    if (toDate < fromDate)
                    {
                        ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                        BindTransactionStatus();
                        return View(bulkpaymentsearchVM);
                    }
                    #endregion

                    //Add One Minute for toDate
                    toDate = toDate.AddDays(1);

                    //var GroupJoinQS = from files in db.C_BulkPaymentFileUploads
                    //                  join trans in db.C_MakerTranLogs
                    //                  on files.Id equals trans.BulkPaymentFileUploadID
                    //                  into TranGroups
                    //                  select new { files, TranGroups };

                    List<C_MakerTranLog> tranlogs = db.C_MakerTranLogs.Where(u => u.BulkPaymentFileUploadID > 0).ToList();
                    var tranlog = tranlogs.GroupBy(cc => cc.BulkPaymentFileUploadID)
                                  .Select(dd => new
                                  {
                                      BulkPaymentFileUploadID = dd.Key,
                                      Tran_Date = dd.Select(ee => ee.TransactionDate).First(),
                                      Value_Date = dd.Select(ee => ee.CreatedDate).First(),
                                      Fail_Count = dd.Count(r => r.Status == "Fail"),
                                      Amount = dd.Where(x => x.Status == "Success" & x.TranType != "ChargesAmount").Sum(x => x.TranAmount),
                                      //MMKAmount = dd.Sum(ee => ee.MMKAmount)
                                      //Charges_Amt = dd.Where(x => x.Status == "Success").Sum(x => x.ChargeAmount)
                                  });

                    List<BulkPaymentViewModel> result = (from files in db.C_BulkPaymentFileUploads
                                                         where files.DEV_FLAG == false
                                                         & (files.VrCorporate.COMPANY_NAME.ToLower().Contains(company) || files.VrCorporate.COMPANY_NAME == "")
                                                         & (files.VrCorporate.CORPORATEID.ToLower().Contains(corporateid) || files.VrCorporate.CORPORATEID == "")
                                                         & (files.FromAccount.ToLower().Contains(debit_acct) || files.FromAccount == "")
                                                         & (files.TotalAmount.ToString().ToLower().Contains(debit_amt) || files.TotalAmount.ToString() == "")
                                                         & (files.Status.ToLower().Contains(status) || files.Status == "")
                                                         & (files.TransType.ToLower().Contains(trantype) || files.TransType == "")
                                                         & (files.FileName.ToLower().Contains(bfilename) || files.FileName == "")
                                                         orderby files.Id ascending
                                                         select new BulkPaymentViewModel
                                                         {
                                                             ID = files.Id,
                                                             //TRAN_DATE = DateTime.Now,
                                                             //VALUE_DATE = DateTime.Now,
                                                             COMPANY_NAME = files.VrCorporate.COMPANY_NAME,
                                                             CORPORATEID = files.VrCorporate.CORPORATEID,
                                                             FROM_ACCT = files.FromAccount,
                                                             DEBIT_AMT = (files.TotalAmount + (files.ChargesAmount == null ? 0 : files.ChargesAmount)),
                                                             AMOUNT = 0,
                                                             MMKAMOUNT = (Decimal?)0.00,
                                                             STATUS = files.Status,
                                                             FAIL_COUNT = 0,
                                                             TRAN_TYPE = files.TransType,
                                                             BulkFileName = files.FileName,
                                                             CHARGES_AMT = files.ChargesAmount
                                                         }).ToList();

                    foreach (BulkPaymentViewModel bulk in result)
                    {
                        bulk.TRAN_DATE = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.Tran_Date).FirstOrDefault();
                        bulk.VALUE_DATE = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.Value_Date).FirstOrDefault();
                        bulk.AMOUNT = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.Amount).FirstOrDefault();
                        bulk.FAIL_COUNT = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.Fail_Count).FirstOrDefault();
                        //bulk.MMKAMOUNT = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.MMKAmount).FirstOrDefault();
                        //bulk.CHARGES_AMT = tranlog.Where(r => r.BulkPaymentFileUploadID == bulk.ID).Select(s => s.Charges_Amt).FirstOrDefault();
                    }


                    //filter by date
                    if (fromDate != null & toDate != null)
                    {
                        result = result.Where(a => a.VALUE_DATE >= fromDate && a.VALUE_DATE < toDate).ToList();
                    }
                    else if (fromDate != null)
                    {
                        result = result.Where(a => a.VALUE_DATE >= fromDate).ToList();
                    }
                    else if (toDate != null)
                    {
                        result = result.Where(a => a.VALUE_DATE < toDate).ToList();
                    }

                    //filter by date
                    //if (fromDate != null & toDate != null)
                    //{
                    //    result = result.Where(a => a.VALUE_DATE >= fromDate && a.VALUE_DATE < toDate).ToList();
                    //}
                    //else if (fromDate != null)
                    //{
                    //    result = result.Where(a => a.VALUE_DATE >= fromDate).ToList();
                    //}
                    //else if (toDate != null)
                    //{
                    //    result = result.Where(a => a.VALUE_DATE < toDate).ToList();
                    //}

                    bulkpaymentsearchVM.bulkpaymentVM = result;
                    Session["ReportData"] = bulkpaymentsearchVM.bulkpaymentVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Bulk Payment report generates successfully.");
                }
                BulkStatus();
                BulkTranType();
                return View(bulkpaymentsearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Bulk Payment Detail
        //Get Bulk Payment Detail
        public ActionResult BulkPaymentDetail()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                BulkPaymentDetailSearchViewModel bulkpaymentdetailksearchVM = new BulkPaymentDetailSearchViewModel();
                bulkpaymentdetailksearchVM.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                bulkpaymentdetailksearchVM.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                BulkDetailStatus();
                BulkDetailTranType();
                CBMStatusDDL();
                return View(bulkpaymentdetailksearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult BulkPaymentDetail(BulkPaymentDetailSearchViewModel bulkpaymentdetailsearchVM)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    //string company = bulkpaymentdetailsearchVM.Company_Name ?? "";
                    string corporateid = bulkpaymentdetailsearchVM.Corporate_ID ?? "";
                    string from_acct = bulkpaymentdetailsearchVM.Debit_Acct ?? "";
                    string status = bulkpaymentdetailsearchVM.Status ?? "";
                    string trantype = bulkpaymentdetailsearchVM.TranType ?? "OtherBankTransfer";
                    string bfilename = bulkpaymentdetailsearchVM.BulkFileName ?? "a";
                    string cbmstatus = bulkpaymentdetailsearchVM.CBMStatus ?? "";
                    //company = company.ToLower().Trim();
                    corporateid = corporateid.ToLower().Trim();
                    from_acct = from_acct.Trim();
                    status = status.Trim();
                    trantype = trantype.ToLower().Trim();
                    bfilename = bfilename.ToLower().Trim();
                    cbmstatus = cbmstatus.ToLower().Trim();
                    string from = null, to = null;
                    DateTime fromDate = DateTime.Now, toDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(bulkpaymentdetailsearchVM.FromDate))
                    {
                        fromDate = General.ConvertStrToDate(bulkpaymentdetailsearchVM.FromDate);
                        from = fromDate.ToString(ConstantValues.Date_Format);
                    }
                    if (!String.IsNullOrEmpty(bulkpaymentdetailsearchVM.ToDate))
                    {
                        toDate = General.ConvertStrToDate(bulkpaymentdetailsearchVM.ToDate);
                        to = toDate.ToString(ConstantValues.Date_Format);
                    }

                    #region validation
                    //check toDate is greater than fromDate or not
                    if (toDate < fromDate)
                    {
                        ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
                        BindTransactionStatus();
                        return View(bulkpaymentdetailsearchVM);
                    }
                    #endregion

                    //Add One day for toDate
                    //if(from == to){ to = to.AddDays(1); }

                    var dtResult = GetBulkPaymentDetail(corporateid, from_acct, bfilename, cbmstatus, status, trantype, from, to);

                    List<BulkPaymentDetailViewModel> result = new List<BulkPaymentDetailViewModel>();
                    if (trantype != "otherbanktransfer")
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new BulkPaymentDetailViewModel
                                  {
                                      CORPORATEID = (String.IsNullOrEmpty(dr["CORPORATEID"].ToString()) ? "-" : dr["CORPORATEID"].ToString()),
                                      TRAN_DATE = (String.IsNullOrEmpty(dr["TRAN_DATE"].ToString()) ? null : dr["TRAN_DATE"].ToString()),
                                      VALUE_DATE = (String.IsNullOrEmpty(dr["VALUE_DATE"].ToString()) ? null : dr["VALUE_DATE"].ToString()),
                                      TRAN_ID = (String.IsNullOrEmpty(dr["TRAN_ID"].ToString()) ? "-" : dr["TRAN_ID"].ToString()),
                                      TRAN_TYPE = (String.IsNullOrEmpty(dr["TRAN_TYPE"].ToString()) ? "-" : dr["TRAN_TYPE"].ToString()),
                                      FROM_ACC = (String.IsNullOrEmpty(dr["FROM_ACC"].ToString()) ? "-" : dr["FROM_ACC"].ToString()),
                                      TO_ACC = (String.IsNullOrEmpty(dr["TO_ACC"].ToString()) ? "-" : dr["TO_ACC"].ToString()),
                                      TO_BANK = "-",
                                      BRANCH = "-",
                                      TRAN_AMT = (String.IsNullOrEmpty(dr["TRAN_AMT"].ToString()) ? "-" : dr["TRAN_AMT"].ToString()),
                                      CHARGES_AMT = (String.IsNullOrEmpty(dr["CHARGES_AMT"].ToString()) ? "-" : dr["CHARGES_AMT"].ToString()),
                                      CBMTRAN_ID = "-",
                                      CBM_STATUS = "-",
                                      STATUS = (String.IsNullOrEmpty(dr["Status"].ToString()) ? "-" : dr["Status"].ToString()),
                                      BULK_FILE_NAME = (String.IsNullOrEmpty(dr["BULK_FILE_NAME"].ToString()) ? "-" : dr["BULK_FILE_NAME"].ToString()),
                                  }).ToList();
                    }
                    else
                    {
                        result = (from DataRow dr in dtResult.Rows
                                  select new BulkPaymentDetailViewModel
                                  {
                                      CORPORATEID = (String.IsNullOrEmpty(dr["CORPORATEID"].ToString()) ? "-" : dr["CORPORATEID"].ToString()),
                                      TRAN_DATE = (String.IsNullOrEmpty(dr["TRAN_DATE"].ToString()) ? null : dr["TRAN_DATE"].ToString()),
                                      VALUE_DATE = (String.IsNullOrEmpty(dr["VALUE_DATE"].ToString()) ? null : dr["VALUE_DATE"].ToString()),
                                      TRAN_ID = (String.IsNullOrEmpty(dr["TRAN_ID"].ToString()) ? "-" : dr["TRAN_ID"].ToString()),
                                      TRAN_TYPE = (String.IsNullOrEmpty(dr["TRAN_TYPE"].ToString()) ? "-" : dr["TRAN_TYPE"].ToString()),
                                      FROM_ACC = (String.IsNullOrEmpty(dr["FROM_ACC"].ToString()) ? "-" : dr["FROM_ACC"].ToString()),
                                      TO_ACC = (String.IsNullOrEmpty(dr["TO_ACC"].ToString()) ? "-" : dr["TO_ACC"].ToString()),
                                      TO_BANK = (String.IsNullOrEmpty(dr["TO_BANK"].ToString()) ? "-" : dr["TO_BANK"].ToString()),
                                      BRANCH = (String.IsNullOrEmpty(dr["BRANCH"].ToString()) ? "-" : dr["BRANCH"].ToString()),
                                      TRAN_AMT = (String.IsNullOrEmpty(dr["TRAN_AMT"].ToString()) ? "-" : dr["TRAN_AMT"].ToString()),
                                      CHARGES_AMT = (String.IsNullOrEmpty(dr["CHARGES_AMT"].ToString()) ? "-" : dr["CHARGES_AMT"].ToString()),
                                      CBMTRAN_ID = (String.IsNullOrEmpty(dr["CBMTRAN_ID"].ToString()) ? "-" : dr["CBMTRAN_ID"].ToString()),
                                      CBM_STATUS = (String.IsNullOrEmpty(dr["CBM_STATUS"].ToString()) ? "-" : dr["CBM_STATUS"].ToString()),
                                      STATUS = (String.IsNullOrEmpty(dr["Status"].ToString()) ? "-" : dr["Status"].ToString()),
                                      BULK_FILE_NAME = (String.IsNullOrEmpty(dr["BULK_FILE_NAME"].ToString()) ? "-" : dr["BULK_FILE_NAME"].ToString()),
                                  }).ToList();
                    }

                    //foreach (BulkPaymentDetailViewModel bulk in result)
                    //{
                    //    if (bulk.CHARGES_AMT == "0.00" || bulk.CHARGES_AMT == "")
                    //    {
                    //        bulk.CHARGES_AMT = "-";
                    //    }
                    //}

                    bulkpaymentdetailsearchVM.bulkpaymentdetailVM = result;
                    Session["ReportData"] = bulkpaymentdetailsearchVM.bulkpaymentdetailVM;
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Bulk Payment Detail report generates successfully.");
                }
                BulkDetailStatus();
                BulkDetailTranType();
                CBMStatusDDL();
                return View(bulkpaymentdetailsearchVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region CIF Information
        // GET: Reports
        public ActionResult CIFInformation()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CIFIDInfoSearchViewModel cifinfovm = new CIFIDInfoSearchViewModel();
                ViewBag.UserType = GetUserTypeList();
                //cifinfovm.CREATEDDATE = DateTime.Now.ToString("dd-MM-yyyy");
                return View(cifinfovm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
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
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }
        //User Report
        [HttpPost]
        public ActionResult CIFInformation(CIFIDInfoSearchViewModel cifinfovm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //search
                    string name = cifinfovm.NAME ?? "";
                    string CIFID = cifinfovm.CIFID ?? "";
                    string usertype = cifinfovm.USERTYPE ?? "";
                    string iconicno = cifinfovm.ICONICCODE ?? "";
                    string nrc = cifinfovm.NRC ?? "";
                    DateTime effectivedate = DateTime.Now;
                    DateTime expdate = DateTime.Now;
                    DateTime createdDate = DateTime.Now;
                    DateTime fromDate = DateTime.Now;
                    DateTime toDate = DateTime.Now;
                    name = name.ToLower().Trim();
                    CIFID = CIFID.ToLower().Trim();
                    usertype = usertype.ToLower().Trim();
                    iconicno = iconicno.ToLower().Trim();
                    nrc = nrc.ToLower().Trim();

                    List<CIFIDInfoRPViewModel> cifidrp = new List<CIFIDInfoRPViewModel>();
                    List<CIFIDInfoRPViewModel> result = new List<CIFIDInfoRPViewModel>();
                    if ((!String.IsNullOrEmpty(cifinfovm.FROMDATE)) && (!String.IsNullOrEmpty(cifinfovm.TODATE)))
                    {
                        fromDate = General.ConvertStrToDate(cifinfovm.FROMDATE);
                        toDate = General.ConvertStrToDate(cifinfovm.TODATE);
                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where c.CIFID.Contains(CIFID) & c.NAME.Contains(name) & c.USERTYPE.Contains(usertype) & DbFunctions.TruncateTime(c.CREATEDDATE) >= fromDate && DbFunctions.TruncateTime(c.CREATEDDATE) <= toDate && c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;
                    }
                    else if (!String.IsNullOrEmpty(cifinfovm.ICONICCODE))
                    {
                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where
                                             c.USERTYPECODE.Contains(iconicno) &&
                                             c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;

                    }
                    else if (!String.IsNullOrEmpty(cifinfovm.NRC))
                    {
                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where
                                             c.NRC.Contains(nrc) &&
                                             c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;

                    }
                    else if (!String.IsNullOrEmpty(cifinfovm.EffectiveDate))
                    {
                        var dd = cifinfovm.EffectiveDate.Replace("/", "-");
                        string addday = "01-" + dd;
                        effectivedate = General.ConvertStrToDateForIconic(addday);

                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where c.CIFID.Contains(CIFID) & c.NAME.Contains(name) & c.USERTYPE.Contains(usertype) & DbFunctions.TruncateTime(c.EFFECTIVEDATE) == effectivedate && c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;
                    }
                    else if (!String.IsNullOrEmpty(cifinfovm.ExpireDate))
                    {
                        var ddexp = cifinfovm.ExpireDate.Replace("/", "-");
                        string adddayexp = "01-" + ddexp;
                        expdate = General.ConvertStrToDateForIconic(adddayexp);
                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where c.CIFID.Contains(CIFID) & c.NAME.Contains(name) & c.USERTYPE.Contains(usertype) & DbFunctions.TruncateTime(c.EXPIREDATE) == expdate && c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;
                    }
                    else
                    {
                        var CFIDInfos = (from c in db.CIFInfoModels
                                         where
                                             c.CIFID.Contains(CIFID) & c.NAME.Contains(name) & c.USERTYPE.Contains(usertype) &&
                                             c.DELFLAG == false
                                         orderby c.CREATEDDATE ascending
                                         select new CIFIDInfoRPViewModel
                                         {

                                             NAME = c.NAME,
                                             CIFID = c.CIFID,
                                             ADDRESS = c.ADDRESS,
                                             USERTYPE = c.USERTYPE,
                                             USERTYPECODE = c.USERTYPECODE,
                                             PHONENO = c.PHONENO,
                                             REMARK = c.REMARK,
                                             CREATEDDATE = c.CREATEDDATE,
                                             EXPIREDATE = c.EXPIREDATE,
                                             NRC = c.NRC,
                                             EFFECTIVEDATE = c.EFFECTIVEDATE,
                                             RMID = c.RMID.ToString(),
                                             RMName = c.RMName
                                         });

                        cifidrp = CFIDInfos.ToList();
                        result = cifidrp;

                        cifinfovm.CIFIDInfoRPVM = result;

                        Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;
                    }
                    ViewBag.UserType = GetUserTypeList();
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"CIF Information report generates successfully.");
                }
                TempData.Keep();
                return View(cifinfovm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Account Information
        // GET: Reports
        public ActionResult CIFAccountInformation()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                CIFAccountInfoSearchViewModel cifinfovm = new CIFAccountInfoSearchViewModel();
                return View(cifinfovm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //User Report
        [HttpPost]
        public ActionResult CIFAccountInformation(CIFAccountInfoSearchViewModel cifinfovm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (String.IsNullOrEmpty(cifinfovm.CIFID) && String.IsNullOrEmpty(cifinfovm.ICONICNUMBER))
                {
                    ModelState.AddModelError("CIFID", "Required CIFID.");
                    ModelState.AddModelError("ICONICNUMBER", "Required ICONIC NUMBER.");
                    return View(cifinfovm);

                }

                string CIFID = string.Empty;
                List<CIFIDInfoRPViewModel> cifidrp = new List<CIFIDInfoRPViewModel>();
                //search
                if ((!String.IsNullOrEmpty(cifinfovm.CIFID)) && (!String.IsNullOrEmpty(cifinfovm.ICONICNUMBER)))
                {
                    CIFID = cifinfovm.CIFID ?? "";
                    CIFID = CIFID.ToLower().Trim();

                    var CFIDInfos = (from c in db.CIFInfoModels
                                     where
                                         c.CIFID.Contains(CIFID) && c.USERTYPECODE == cifinfovm.ICONICNUMBER &&
                                         c.DELFLAG == false
                                     orderby c.CREATEDDATE ascending
                                     select new CIFIDInfoRPViewModel
                                     {

                                         NAME = c.NAME,
                                         CIFID = c.CIFID,
                                         ADDRESS = c.ADDRESS,
                                         USERTYPE = c.USERTYPE,
                                         USERTYPECODE = c.USERTYPECODE,
                                         PHONENO = c.PHONENO,
                                         REMARK = c.REMARK,
                                         EXPIREDATE = c.EXPIREDATE,
                                         NRC = c.NRC,
                                         EFFECTIVEDATE = c.EFFECTIVEDATE,
                                         RMID = c.RMID.ToString(),
                                         RMName = c.RMName
                                     });
                    cifidrp = CFIDInfos.ToList();
                    var cifidcount = CFIDInfos.ToList().Count();
                    if (cifidcount > 0)
                    {
                        CIFID = cifidrp[0].CIFID;
                    }
                    else { CIFID = null;
                        ModelState.AddModelError("CIFID", "This CIFID is not Iconic User.");

                        return View(cifinfovm);
                    }
                }
                else if (!String.IsNullOrEmpty(cifinfovm.CIFID))
                {
                    CIFID = cifinfovm.CIFID ?? "";
                    CIFID = CIFID.ToLower().Trim();

                    var CFIDInfos = (from c in db.CIFInfoModels
                                     where
                                         c.CIFID.Contains(CIFID) && c.DELFLAG == false
                                     orderby c.CREATEDDATE ascending
                                     select new CIFIDInfoRPViewModel
                                     {

                                         NAME = c.NAME,
                                         CIFID = c.CIFID,
                                         ADDRESS = c.ADDRESS,
                                         USERTYPE = c.USERTYPE,
                                         USERTYPECODE = c.USERTYPECODE,
                                         PHONENO = c.PHONENO,
                                         REMARK = c.REMARK,
                                         EXPIREDATE = c.EXPIREDATE,
                                         NRC = c.NRC,
                                         EFFECTIVEDATE = c.EFFECTIVEDATE,
                                         RMID = c.RMID.ToString(),
                                         RMName = c.RMName
                                     });
                   
                    cifidrp = CFIDInfos.ToList();
                    var cifidcount = CFIDInfos.ToList().Count();
                    if (cifidcount > 0)
                    {
                        CIFID = cifidrp[0].CIFID;
                    }
                    else
                    {
                        CIFID = null;
                        ModelState.AddModelError("CIFID", "This CIFID is not Iconic User.");
                        
                        return View(cifinfovm);
                    }
                }
                else
                {
                    var CFIDInfos = (from c in db.CIFInfoModels
                                     where c.USERTYPECODE == cifinfovm.ICONICNUMBER &&
                                          c.DELFLAG == false
                                     orderby c.CREATEDDATE ascending
                                     select new CIFIDInfoRPViewModel
                                     {

                                         NAME = c.NAME,
                                         CIFID = c.CIFID,
                                         ADDRESS = c.ADDRESS,
                                         USERTYPE = c.USERTYPE,
                                         USERTYPECODE = c.USERTYPECODE,
                                         PHONENO = c.PHONENO,
                                         REMARK = c.REMARK,
                                         EXPIREDATE = c.EXPIREDATE,
                                         NRC = c.NRC,
                                         EFFECTIVEDATE = c.EFFECTIVEDATE,
                                         RMID = c.RMID.ToString(),
                                         RMName = c.RMName
                                     });
                    cifidrp = CFIDInfos.ToList();
                    var cifidcount = CFIDInfos.ToList().Count();
                    if (cifidcount > 0)
                    {
                        CIFID = cifidrp[0].CIFID;
                    }
                    else
                    {
                        CIFID = null;
                        ModelState.AddModelError("CIFID", "This CIFID is not Iconic User.");

                        return View(cifinfovm);
                    }
                }
                RetailResponseInfo rri = new RetailResponseInfo();
                if (cifinfovm.CIFID != "")
                    rri = GetRetailResponse(cifinfovm.CIFID);
                 List<AccountInfo> core_accs = rri.lAcctInfo;
                List<IconicDetailAccVM> cifacc= new List<IconicDetailAccVM>();
                Decimal totalamt = 0;
                foreach (AccountInfo accvm in core_accs)
                {
                    IconicDetailAccVM acc_detail = new IconicDetailAccVM
                    {
                        AccountNumber = accvm.AccountNumber,
                        AccountType = accvm.AccountType,
                        Currency = accvm.Currency,
                        AvailableAmt = Convert.ToDecimal(accvm.AvailableAmt)
                    };
                    if (acc_detail.Currency == "MMK")
                    {
                        totalamt += acc_detail.AvailableAmt;
                    }
                    cifacc.Add(acc_detail);
                }
                
                List<IconicDetailAccVM> accinfo = cifacc;
                List<CIFIDInfoRPViewModel> result = cifidrp;
                List<IconicDetailAccVM> result2 = accinfo;
                cifinfovm.CIFIDInfoRPVM = result;
                cifinfovm.CIFIDAccInfoRPVM = result2;
                Session["CIFIDInfoReportData"] = cifinfovm.CIFIDInfoRPVM;
                Session["CIFAccInfoReportData"] = cifinfovm.CIFIDAccInfoRPVM;
                Session["DetailReportTotalAmtData"] = totalamt;
                //save log
                log.Log(User.Identity.GetUserId(), controllerName, actionName,
                    $"User Detail report generates successfully.");

                BindBranchDDL();
                TempData.Keep();
                return View(cifinfovm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
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

        //branch ddl
        private void BindBranchDDL()
        {
            string BranchCode = GetCurrentAdminBranch();
            if (string.IsNullOrEmpty(BranchCode))
            {
                ModelState.AddModelError("", "Unknown Branch User");
            }
            else if (BranchCode == "9999")
            {
                ViewBag.BranchList = GetBranchSelectList();
            }
            TempData["Code"] = BranchCode;
        }

        //bind other bank list
        //private void BindBankListDDL()
        //{
        //    ViewBag.OtherBank = GetBankSelectList();
        //}

        //reporttype
        private void BindReportTypeDDL()
        {
            ViewBag.ReportType = GetReportType();
        }
        //bind TranType and DebitOrCredit
        private void BindTranDDL()
        {
            ViewBag.TranType = GetTransferDDL();
            ViewBag.DebitOrCredit = General.GetDebitOrCredit();
        }

        //bind Currency Code
        private void BindCurrencyDDL()
        {
            ViewBag.CurrencyList = GetCurrencyList();
        }

        //bind utility name by utility type
        private void BindOperatorDDL()
        {
            ViewBag.MobileTopupUltList = GetMobileTopupUltList();
        }

        //bind utility name by utility type
        private void BindBillerDDL()
        {
            ViewBag.GiftUltList = GetGiftCardUltList();
        }

        //bind log type for admin log
        private void BindLogType()
        {
            //create Log Type List
            ViewBag.LogType = new List<SelectListItem>()
            {
                new SelectListItem{Text="Info",Value="info"},
                new SelectListItem{Text="Error",Value="error"}
            };
        }

        //bind request method for api log
        private void ReqMethodDDL()
        {
            var lstcurrency = db.Api_Logs.Select(c => c.RequestMethod).ToList();
            IEnumerable<SelectListItem> currencylist = lstcurrency.Distinct().Select(b => new SelectListItem
            {
                Value = b,
                Text = b
            });
            ViewBag.ReqMethod = currencylist;

            //create Log Type List
            //ViewBag.ReqMethod = new List<SelectListItem>()
            //{
            //    new SelectListItem{Text="FIAPI",Value="FIAPI"},
            //    new SelectListItem{Text="POST",Value="POST"},
            //    new SelectListItem{Text="SMS Gateway",Value="SMS_Gateway"},
            //    new SelectListItem{Text="Test",Value="Test"}
            //};
        }

        //bind user type for api log
        private void UserTypeDDL()
        {
            //create Log Type List
            ViewBag.UserType = new List<SelectListItem>()
            {
                new SelectListItem{Text="Internet Banking",Value="IB"},
                new SelectListItem{Text="Mobile Banking",Value="MB"}
            };
        }
        //bind Transaction_Status
        private void BindTransactionStatus()
        {
            ViewBag.TranStatus = General.BulkTransaction_Status();
        }

        //bind Transaction_Status for Schedule Payment Detail report
        private void BindScheduleTranStatus()
        {
            ViewBag.TranStatus = General.GetTransaction_Status();
        }

        //get Transaction Type
        private void TransactionTypeDDL()
        {
            //create Log Type List
            ViewBag.TransactionType = new List<SelectListItem>()
            {
                new SelectListItem{Text="Other Bank Transfer",Value="OtherBankTransfer"},
                new SelectListItem{Text="Other Bank Remmittance",Value="OtherBankRemittance"}
            };
        }

        //get CBMTran_Type
        private void BindCBMTran_Type()
        {
            var lstCBMTranType = db.OtherBankTranLogs.Select(o => o.CBMTran_Type).ToList();
            IEnumerable<SelectListItem> CBMTran_Type = lstCBMTranType.Distinct().Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            });
            ViewBag.CBMTran_Type = CBMTran_Type;
        }

        //get Transaction Status
        private void TransactionStatusDDL()
        {
            //create Log Type List
            ViewBag.TransactionStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Success",Value="Success"},
                new SelectListItem{Text="Fail",Value="Fail"}
            };
        }

        //get CBM Status
        private void CBMStatusDDL()
        {
            //create Log Type List
            ViewBag.CBMStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Pending",Value="Pending"},
                new SelectListItem{Text="CBM Success",Value="CBM Success"},
                new SelectListItem{Text="CBM Reject",Value="CBM Reject"},
                new SelectListItem{Text="OtherBank Success",Value="OtherBank Success"},
                new SelectListItem{Text="OtherBank Fail",Value="OtherBank Fail"},
                new SelectListItem{Text="Reverse Success",Value="Reverse Success"}
            };
        }

        #region get bulk payment report Status
        private void BulkStatus()
        {
            //create Log Type List
            ViewBag.BulkStatus = new List<SelectListItem>()
            {
                new SelectListItem{Text="Approved",Value="Approved"},
                new SelectListItem{Text="Rejected",Value="Rejected"},
                new SelectListItem{Text="Pending",Value="Pending"}
            };
        }
        #endregion

        #region get bulk payment detail report Status
        private void BulkDetailStatus()
        {
            ViewBag.BulkDetailStatus = new List<SelectListItem>()
            {
                //new SelectListItem{Text="Pending",Value="Pending"},
                new SelectListItem{Text="Success",Value="Success"},
                //new SelectListItem{Text="Reject",Value="Rejecte"},
                new SelectListItem{Text="Fail",Value="Fail"}
            };
        }
        #endregion

        #region get bulk payment report Tran Type
        private void BulkTranType()
        {
            //create Log Type List
            ViewBag.BulkTranType = new List<SelectListItem>()
            {
                new SelectListItem{Text="ScheduleOther",Value="ScheduleOther"},
                new SelectListItem{Text="ScheduleOwn",Value="ScheduleOwn"},
                new SelectListItem{Text="TransferOther",Value="TransferOther"},
                new SelectListItem{Text="TransferOtherBank",Value="TransferOtherBank"},
                new SelectListItem{Text="Reversal",Value="Reversal"},
            };
        }
        #endregion

        #region get bulk payment detail report Tran Type
        private void BulkDetailTranType()
        {
            //create Log Type List
            ViewBag.BulkDetailTranType = new List<SelectListItem>()
            {
                //new SelectListItem{Text="Select Tran Type",Value="ALL"},
                new SelectListItem{Text="Schedule Own",Value="ScheduleOwn"},
                new SelectListItem{Text="Schedule Other",Value="ScheduleOther"},
                new SelectListItem{Text="Transfer Own",Value="TransferOwn"},
                new SelectListItem{Text="Transfer Other",Value="TransferOther"},
                new SelectListItem{Text="Other Bank Transfer",Value="OtherBankTransfer"},
            };
        }
        #endregion

        //bind corporate type
        private void BindCorporateType()
        {
            ViewBag.COrporateType = General.GetCorporateType();
        }

        //get transaction data for retail 
        private DataTable GetTransaction(string branchcode, string trantype, string debitorcredit, string currency, string from, string to, bool IsCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Transactions",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    command.Parameters.Add(new SqlParameter("@DebitORCredit", debitorcredit));
                    command.Parameters.Add(new SqlParameter("@Currency", currency));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    connection.Open();

                    //SqlDataReader reader = command.ExecuteReader();

                    //if (reader.HasRows)
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine("{0}\t{1}", reader.GetInt32(0),
                    //            reader.GetString(1));
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("No rows found.");
                    //}
                    //reader.Close();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get transaction data for corporate
        private DataTable GetCorTransaction(string branchcode, string trantype, string debitorcredit, string currency, string from, string to, bool IsCorporate, string Company)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_Transactions",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    command.Parameters.Add(new SqlParameter("@DebitORCredit", debitorcredit));
                    command.Parameters.Add(new SqlParameter("@Currency", currency));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get other bank transaction data for retail
        private DataTable GetOtherBankTranLog(string otherbank, string otherbranch, string trantype, string transtatus, string from, string to, bool IsCorporate, string cbmstatus, string CBMTran_Type)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherBankTranLog",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@OtherBank", otherbank));
                    command.Parameters.Add(new SqlParameter("@Branch", otherbranch));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    command.Parameters.Add(new SqlParameter("@Status", transtatus));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@CBMStatus", cbmstatus));
                    command.Parameters.Add(new SqlParameter("@CBMTranType", CBMTran_Type));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get other bank transaction data for retail
        private DataTable GetCorOtherBankTranLog(string otherbank, string otherbranch, string trantype, string transtatus, string from, string to, string company, bool IsCorporate, string cbmstatus, string CBMTran_Type)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherBankTranLog",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@OtherBank", otherbank));
                    command.Parameters.Add(new SqlParameter("@Branch", otherbranch));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    command.Parameters.Add(new SqlParameter("@Status", transtatus));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@Company", company));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@CBMStatus", cbmstatus));
                    command.Parameters.Add(new SqlParameter("@CBMTranType", CBMTran_Type));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get current login admin branch
        private string GetCurrentAdminBranch()
        {
            string loginUserid = User.Identity.GetUserId();
            try
            {
                AdminUser admin = db.Users.Find(loginUserid);
                return admin.VrBranch.code;
            }
            catch (Exception ex)
            {
                log.Error(loginUserid, controllerName, "GetCurrentAdminBranch", ex);
            }
            return "";
        }

        //generate branch list
        private IEnumerable<SelectListItem> GetBranchSelectList()
        {
            try
            {
                List<Branch> lstbranch = db.Branches.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> branches = lstbranch.Select(b => new SelectListItem
                {
                    Value = b.code,//Code eg."0001","9999" is used for Value.
                    Text = b.name

                });
                return branches;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "HomeController", "GetBranchSelectList", ex);
                return null;
            }
        }

        //get bank list
        //private IEnumerable<SelectListItem> GetBankSelectList()
        //{
        //    try
        //    {
        //        List<OtherBank> lstbank = db.OtherBanks.Where(b => b.Active == true).ToList();
        //        IEnumerable<SelectListItem> banks = lstbank.Select(b => new SelectListItem
        //        {
        //            Value = b.OTHER_BANK_ID.ToString(),
        //            Text = b.BANK_NAME

        //        });
        //        return banks;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), "HomeController", "GetBankSelectList", ex);
        //        return null;
        //    }
        //}

        //get other bank's branch list
        //private IEnumerable<SelectListItem> GetOtherBranchSelectList(int bankid)
        //{
        //    try
        //    {
        //        List<OtherBranch> lstbank = db.OtherBranches.Where(b => b.Active == true & b.OTHER_BANK_ID == bankid).ToList();
        //        IEnumerable<SelectListItem> banks = lstbank.Select(b => new SelectListItem
        //        {
        //            Value = b.BR_CODE,//Code eg."110","111" is used for Value.
        //            Text = b.BR_NAME

        //        });
        //        return banks;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), "HomeController", "GetBankSelectList", ex);
        //        return null;
        //    }
        //}

        //get report type for feedback report
        private IEnumerable<SelectListItem> GetReportType()
        {
            try
            {
                List<FeedBack> lstfeedback = db.Feedbacks.Where(f => f.ACTIVE == "1").ToList();
                IEnumerable<SelectListItem> reporttype = lstfeedback.Select(b => new SelectListItem
                {
                    Value = b.TITLE,
                    Text = b.TITLE
                });
                return reporttype;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "HomeController", "GetReportType", ex);
                return null;
            }
        }

        //get Currency Code List
        private IEnumerable<SelectListItem> GetCurrencyList()
        {
            try
            {
                List<Currency> lstcurrency = db.Currencies.Where(c => c.DEL_FLAG == "0").ToList();
                IEnumerable<SelectListItem> currencylist = lstcurrency.Select(b => new SelectListItem
                {
                    Value = b.Code,
                    Text = b.Code
                });
                return currencylist;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "HomeController", "GetReportType", ex);
                return null;
            }
        }

        //get Transfer Type
        private IEnumerable<SelectListItem> GetTransferDDL()
        {
            try
            {
                var lsttrantype = db.Database.SqlQuery<string>("SELECT TranType FROM TBL_TRAN_TYPE " + "where [Active] = 1 and [TranType]!='Remit' and [TranType]!='TopUp' and [TranType]!='BillPayment'").ToList();
                List<SelectListItem> tranlist = new List<SelectListItem>();
                tranlist.Add(new SelectListItem { Text = "All", Value = "TranAll" });
                for (int i = 0; i < lsttrantype.Count; i++)
                {
                    tranlist.Add(new SelectListItem { Text = lsttrantype[i].ToString(), Value = lsttrantype[i].ToString() });
                }
                //foreach(var item in lsttrantype)
                //{
                //    tranlist.Add(new SelectListItem { Text = item[0].ToString(), Value = item[0].ToString() });
                //}
                //IEnumerable<SelectListItem> lsttrantype = db.Database.SqlQuery<SelectListItem>("SELECT * FROM TBL_TRAN_TYPE " +
                //        "where [Active] = 1");

                ////List<SelectList> trantypelist = lsttrantype.ToList();
                IEnumerable<SelectListItem> trantypelst = tranlist.Select(b => new SelectListItem
                {
                    Value = b.Value,
                    Text = b.Text
                });
                return trantypelst;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "HomeController", "GetReportType", ex);
                return null;
            }
        }

        //get Mobile Topup Utility type from Utility 
        private IEnumerable<SelectListItem> GetMobileTopupUltList()
        {
            try
            {
                List<Utilities> lstmobiletopup = db.Utilities.Where(u => u.Active == "1" && u.Utility_Type == "MobileTopup").ToList();
                IEnumerable<SelectListItem> mobiletopup = lstmobiletopup.Select(u => new SelectListItem
                {
                    Value = u.Name,
                    Text = u.Name

                });
                return mobiletopup;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetMobilTopupUltList", ex);
                return null;
            }
        }

        //get Gift Card Utility type from Utility 
        //private IEnumerable<SelectListItem> GetGiftCardUltList()
        //{
        //    try
        //    {
        //        List<Utilities> lstgiftcard = db.Utilities.Where(u => u.Active == "1" && u.Utility_Type == "GiftCard").ToList();
        //        IEnumerable<SelectListItem> giftcard = lstgiftcard.Select(u => new SelectListItem
        //        {
        //            Value = u.Name,
        //            Text = u.Name

        //        });
        //        return giftcard;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, "GetGiftCardUltList", ex);
        //        return null;
        //    }
        //}

        private IEnumerable<SelectListItem> GetGiftCardUltList()
        {
            try
            {
                //var utilities = (from utility in db.Utilities
                //         where utility.Active == "1" & utility.Utility_Type == "GiftCard"
                //         select utility.Name).ToList();


                //var billers = (from biller in db.TBL_Billers
                //              where biller.Active == true & biller.BillerType == "GiftCard"
                //              select biller.Name).ToList();

                //utilities.AddRange(billers);

                //var lstGiftCard = utilities.Union(billers).ToList();

                var utility = db.Utilities.Where(u => u.Active == "1" && u.Utility_Type == "GiftCard").Select(x => new
                {
                    Name = x.Name,
                    Biller_Code = x.Biller_Code
                }).ToList();

                var biller = db.TBL_Billers.Where(u => u.Active == true && u.BillerType == "GiftCard").Select(x => new
                {
                    Name = x.Name,
                    Biller_Code = x.BillerCode
                }).ToList();

                var lstGiftCard = utility.Concat(biller);

                //List<Utilities> lstgiftcard = db.Utilities.Where(u => u.Active == "1" && u.Utility_Type == "GiftCard").ToList();
                IEnumerable<SelectListItem> giftcard = lstGiftCard.Select(u => new SelectListItem
                {
                    Value = u.Biller_Code,
                    Text = u.Name

                });
                return giftcard;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetGiftCardUltList", ex);
                return null;
            }
        }

        //get retail schedule payment detail
        private DataTable GetSchedulePaymentDetail(string branchcode, string transtatus, string from, string to)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_SchedulePaymentDetail",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@Transtatus", transtatus));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get corporate schedule payment detail
        private DataTable GetCorSchedulePaymentDetail(string branchcode, string transtatus, string from, string to, bool IsCorporate, string Company)
        {//choosedbranch, transtatus, from, to, IsCorporate, company
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_SchedulePaymentDetail",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Branch", branchcode));
                    command.Parameters.Add(new SqlParameter("@Transtatus", transtatus));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    command.Parameters.Add(new SqlParameter("@Company", Company));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get bulk payment 
        private DataTable GetBulkPayment(string from, string to)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherBankTranLog",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    //command.Parameters.Add(new SqlParameter("@OtherBank", otherbank));
                    //command.Parameters.Add(new SqlParameter("@Branch", otherbranch));
                    //command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    //command.Parameters.Add(new SqlParameter("@Status", transtatus));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get corporate schedule payment detail
        private DataTable GetBulkPaymentDetail(string corporateid, string from_acc, string bfilename, string cbmstatus, string status, string trantype, string fromdate, string todate)
        {//choosedbranch, transtatus, from, to, IsCorporate, company
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_BulkPaymentDetail",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@CorporateID", corporateid));
                    command.Parameters.Add(new SqlParameter("@FromAcc", from_acc));
                    command.Parameters.Add(new SqlParameter("@BulkFileName", bfilename));
                    command.Parameters.Add(new SqlParameter("@CBMStatus", cbmstatus));
                    command.Parameters.Add(new SqlParameter("@Status", status));
                    command.Parameters.Add(new SqlParameter("@TranType", trantype));
                    command.Parameters.Add(new SqlParameter("@FromDate", fromdate));
                    command.Parameters.Add(new SqlParameter("@ToDate", todate));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }

        //get biller type when selecting user type
        [NonAction]
        private void bindUserType(string billertype = "")
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try

            {
                var lstUtility = db.Utilities.Where(u => u.Active == "1" & u.Utility_Type == "GiftCard").ToList();
                var lstBiller = db.TBL_Billers.Where(b => b.Active == true).ToList();
                var usertype = new List<SelectListItem>()
                            {
                                new SelectListItem{Text="Retail Mobile Banking",Value="MB"},
                                new SelectListItem{Text="Retail Internet Banking",Value="IB"},
                                //new SelectListItem{Text="Corporate Banking",Value="CB"}
                            };
                //Get user type
                //var otherbanks = db.OtherBanks.Where(ob => ob.Active == true).ToList();
                List<SelectListItem> li = new List<SelectListItem>();
                //li.Add(new SelectListItem { Text = "--Select UserType--", Value = "" });
                if (usertype.Count > 0)
                {
                    foreach (var m in usertype)
                    {
                        li.Add(new SelectListItem { Text = m.Text, Value = m.Value });
                    }
                }
                ViewBag.UserTypeList = li;

                //get biller type if usertype is selected
                List<SelectListItem> biller = new List<SelectListItem>();
                biller.Add(new SelectListItem { Text = "--Select Biller Type--", Value = "" });
                if (billertype != "" & billertype == "MB")
                {
                    if (lstUtility.Count > 0)
                    {
                        foreach (var t in lstUtility)
                        {
                            biller.Add(new SelectListItem { Text = t.Name, Value = t.Biller_Code });
                        }
                    }
                }
                if (billertype != "" & billertype == "IB" || billertype == "CB")
                {
                    if (lstBiller.Count > 0)
                    {
                        foreach (var t in lstBiller)
                        {
                            biller.Add(new SelectListItem { Text = t.Name, Value = t.BillerCode });
                        }
                    }
                }
                ViewBag.BillerList = biller;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }

        [NonAction]
        private void bindCorUserType(string billertype = "")
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try

            {
                var lstUtility = db.Utilities.Where(u => u.Active == "1" & u.Utility_Type == "GiftCard").ToList();
                var lstBiller = db.TBL_Billers.Where(b => b.Active == true).ToList();
                var usertype = new List<SelectListItem>()
                            {
                                new SelectListItem{Text="Corporate Banking",Value="CB"},
                            };
                //Get user type
                //var otherbanks = db.OtherBanks.Where(ob => ob.Active == true).ToList();
                List<SelectListItem> li = new List<SelectListItem>();
                li.Add(new SelectListItem { Text = "--Select UserType--", Value = "CB" });
                if (usertype.Count > 0)
                {
                    foreach (var m in usertype)
                    {
                        li.Add(new SelectListItem { Text = m.Text, Value = m.Value });
                    }
                }
                ViewBag.CorUserTypeList = li;

                //get biller type if usertype is selected
                List<SelectListItem> biller = new List<SelectListItem>();
                biller.Add(new SelectListItem { Text = "--Select Biller Type--", Value = "" });
                if (billertype != "" & billertype == "MB")
                {
                    if (lstUtility.Count > 0)
                    {
                        foreach (var t in lstUtility)
                        {
                            biller.Add(new SelectListItem { Text = t.Name, Value = t.Biller_Code });
                        }
                    }
                }
                if (billertype != "" & billertype == "IB" || billertype == "CB")
                {
                    if (lstBiller.Count > 0)
                    {
                        foreach (var t in lstBiller)
                        {
                            biller.Add(new SelectListItem { Text = t.Name, Value = t.BillerCode });
                        }
                    }
                }
                ViewBag.BillerList = biller;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }

        public JsonResult getBillerType(string usertype)
        {
            try
            {
                if (usertype == null)
                {
                    return null;
                }
                var lstUtility = db.Utilities.Where(u => u.Active == "1" & u.Utility_Type == "GiftCard").ToList();
                var lstBiller = db.TBL_Billers.Where(b => b.Active == true).ToList();

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select Biller Type--", Value = "" });
                if (usertype == "MB")
                {
                    if (lstUtility != null && lstUtility.Count > 0)
                    {
                        foreach (var m in lstUtility)
                        {
                            items.Add(new SelectListItem { Text = m.Name, Value = m.Biller_Code });
                        }
                    }
                }
                if (usertype == "IB" || usertype == "CB")
                {
                    if (lstBiller != null && lstBiller.Count > 0)
                    {
                        foreach (var m in lstBiller)
                        {
                            items.Add(new SelectListItem { Text = m.Name, Value = m.BillerCode });
                        }
                    }
                }

                return Json(new SelectList(items, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [NonAction]
        private void bindOtherBranch(string branchname = "")
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try

            {
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(branchname);
                //Get Bank
                var otherbanks = db.OtherBanks.Where(ob => ob.Active == true).ToList();
                List<SelectListItem> li = new List<SelectListItem>();
                li.Add(new SelectListItem { Text = "--Select Bank--", Value = "" });
                if (otherbanks.Count > 0)
                {
                    foreach (var m in otherbanks)
                    {
                        li.Add(new SelectListItem { Text = m.BANK_NAME, Value = m.OTHER_BANK_ID.ToString() });
                    }
                }
                ViewBag.BankList = li;

                //get branch if bank is selected
                List<SelectListItem> branch = new List<SelectListItem>();
                branch.Add(new SelectListItem { Text = "--Select Branch--", Value = "" });
                if (branchname != "")
                {
                    if (branchlst.OtherBranchList.Count > 0)
                    {
                        foreach (var t in branchlst.OtherBranchList)
                        {
                            branch.Add(new SelectListItem { Text = t.BR_Name, Value = t.BR_Name });
                        }
                    }
                }
                ViewBag.BranchList = branch;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }

        public JsonResult getOtherBranch(int bankid)
        {
            try
            {
                OtherBank otherbank = db.OtherBanks.Find(bankid);
                if (otherbank == null)
                {
                    return null;
                }
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(otherbank.BANK_CODE);
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select Branch--", Value = "" });
                if (branchlst.OtherBranchList != null && branchlst.OtherBranchList.Count > 0)
                {
                    foreach (var m in branchlst.OtherBranchList)
                    {
                        items.Add(new SelectListItem { Text = m.BR_Name, Value = m.BR_Name });
                    }
                }
                return Json(new SelectList(items, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}