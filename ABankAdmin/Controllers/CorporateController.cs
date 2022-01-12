using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class CorporateController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CorporateController";
        EventLogController log = new EventLogController();
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        // GET: Corporate
        public ActionResult Index()
        {
            Getddl();
            return View();
        }
        private void Getddl()
        {
            ViewBag.ruTYPEList = General.GetTypes();
            //Create list
            ViewBag.ruSTATUSList = General.GetStatus();

            ViewBag.ruRequestStatus = General.GetRequestStatus();
        }
        // get company list
        private void GetCompanySelectList()
        {
            try
            {
                List<C_Corporate> corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                {
                    Value = a.COMPANY_NAME,
                    Text = a.COMPANY_NAME

                });
                ViewBag.Corporate = items;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "CorporateController", "GetCompanySelectList", ex);
            }
        }
        //generate branch list
        private void GetBranchSelectList()
        {
            try
            {
                List<Branch> lstbranch = db.Branches.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> branches = lstbranch.Select(b => new SelectListItem
                {
                    Value = b.id.ToString(),//Code eg."0001","9999" is used for Value.
                    Text = b.name

                });
                ViewBag.BranchList = branches;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "CorporateController", "GetBranchSelectList", ex);
            }
        }
        [HttpPost]
        // GET: Corporate
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
            var companyname = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var companyphone = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var type = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var status = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
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
            var rule = db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
            decimal ruleValue = (rule == null ? 5 : rule.Value);
            string userid = User.Identity.GetUserId();

            var reqcorporate = db.C_Req_Corporates.Where(u => u.MAKER == userid /*&& u.STATUS != 1*/).ToList().ConvertAll(u => new ReqCorporateVM
            {
                ID = u.ID,
                CIFID = u.CIFID,
                Branch = u.BRANCH,
                Corporate_ID = u.CORPORATEID,
                Company_Name = u.COMPANY_NAME,
                Company_Email = u.COMPANY_EMAIL,
                Company_Address = u.COMPANY_ADDRESS,
                Company_Phone = u.COMPANY_PHONE,
                ISVIP = u.ISVIP,
                T_CHARGES_CODE = u.T_CHARGES_CODE,
                R_CHARGES_CODE = u.R_CHARGES_CODE,
                Tran_Limit = General.ConvertDecimalToStr(u.TRAN_LIMIT),
                Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(u.BULK_CHARGES_FIX_RATE_N_PERCENT),
                STATUS = u.STATUS,
                ISNEW = u.ISNEW,
                MAKER = u.MAKER,
                RequestedDate = u.RequestedDate,
                CHECKER = u.CHECKER,
                CheckedReason = u.CheckedReason
            });

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                reqcorporate = reqcorporate.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(companyname))
            {
                reqcorporate = reqcorporate.Where(a => a.Company_Name.ToLower().Trim().Contains(companyname.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(companyphone.Trim()))
            {
                reqcorporate = reqcorporate.Where(a => a.Company_Phone.Contains(companyphone.Trim())).ToList();
            }
            if (fromDate != null & toDate != null)
            {
                reqcorporate = reqcorporate.Where(a => a.RequestedDate >= fromDate && a.RequestedDate < toDate).ToList();
            }
            else if (fromDate != null)
            {
                reqcorporate = reqcorporate.Where(a => a.RequestedDate >= fromDate).ToList();
            }
            else if (toDate != null)
            {
                reqcorporate = reqcorporate.Where(a => a.RequestedDate < toDate).ToList();
            }
            if (!string.IsNullOrEmpty(type))
            {
                reqcorporate = reqcorporate.Where(a => a.ISNEW.ToString() == type).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                reqcorporate = reqcorporate.Where(a => a.STATUS.ToString() == status).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                reqcorporate = reqcorporate.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = reqcorporate.Count();
            var data = reqcorporate.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            //if (TempData["filename"] != null && TempData["filename"].ToString() != "")
            //    ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Corporate/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                C_Req_Corporate req_Corporate = db.C_Req_Corporates.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                if (req_Corporate == null)
                {
                    return HttpNotFound();
                }
                var isVIP = req_Corporate.ISVIP;
                ViewBag.IsVIP = (isVIP == true);
                return View(req_Corporate);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [NonAction]
        private void bindCountry(int? CountryId)
        {
            var country = db.C_Countries.ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Select Country--", Value = "0" });
            if (country.Count > 0)
            {
                foreach (var m in country)
                {
                    li.Add(new SelectListItem
                    {
                        Text = m.NAME,
                        Value = m.ID.ToString()
                    });
                }
            }
            ViewBag.Country = li;

            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem { Text = "--Select State--", Value = "" });
            item.Add(new SelectListItem { Text = "State01", Value = "1" });
            item.Add(new SelectListItem { Text = "State02", Value = "1" });
            item.Add(new SelectListItem { Text = "State03", Value = "1" });
            item.Add(new SelectListItem { Text = "State04", Value = "1" });
            item.Add(new SelectListItem { Text = "State05", Value = "1" });
            var state = db.C_States.Where(t => t.COUNTRY_ID == CountryId).ToList();
            //get state if country is selected
            if (state.Count > 0)
            {
                foreach (var t in state)
                {
                    item.Add(new SelectListItem { Text = t.NAME, Value = t.ID.ToString() });
                }
            }
            ViewBag.State = item;
        }
        public JsonResult getState(int id)
        {
            try
            {
                var ddlstate = db.C_States.Where(x => x.COUNTRY_ID == id).ToList();
                List<SelectListItem> item = new List<SelectListItem>();
                item.Add(new SelectListItem { Text = "--Select State--", Value = "" });
                if (ddlstate.Count > 0)
                {
                    foreach (var x in ddlstate)
                    {
                        item.Add(new SelectListItem { Text = x.NAME, Value = x.ID.ToString() });
                    }
                }
                return Json(new SelectList(item, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ////Get Charges Code
        //private void GetCharges()
        //{
        //    ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
        //    CoporateChargeRateInfo cinfo = new CoporateChargeRateInfo();
        //    cinfo = CRInquiry.CoporateChargeRateInquiry(connectionString);

        //    List<SelectListItem> items = new List<SelectListItem>();
        //    items.Add(new SelectListItem { Text = "--Select Charges Code--", Value = "" });
        //    if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
        //    {
        //        foreach (var m in cinfo.ChargeCodeInfo)
        //        {
        //            items.Add(new SelectListItem
        //            {
        //                Text = $"{m.ChargeCode}",
        //                Value = m.ChargeCode
        //            });
        //        }
        //    }
        //    ViewBag.ChargesCode = items;
        //}
        //Get Charges Code

        private void GetTransferCharges()
        {
            ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
            CoporateChargeRateInfo cinfo = new CoporateChargeRateInfo();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "--Select Transfer Charges Code--", Value = "" });
            try
            {
                cinfo = CRInquiry.CoporateTransferChargeRateInquiry();

                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = $"{m.ChargeCode}",
                            Value = m.ChargeCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            ViewBag.TransferChargesCode = items;
        }

        //Get Charges Code
        private void GetRemittanceCharges()
        {
            ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
            CoporateChargeRateInfo cinfo = new CoporateChargeRateInfo();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "--Select Remittance Charges Code--", Value = "" });
            try
            {
                cinfo = CRInquiry.CoporateRemittanceChargeRateInquiry();

                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = $"{m.ChargeCode}",
                            Value = m.ChargeCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            ViewBag.RemittanceChargesCode = items;
        }

        //Get Corporate Transaction Value
        private void GetCorTranValue()
        {
            decimal tranvalue = 0;
            var rule = db.Rules.Where(r => r.Code == "R043" & r.Del_Flag == "0").FirstOrDefault();
            if (rule != null)
            {
                tranvalue = rule.Value;
            }
            ViewBag.CorTranValue =General.ConvertCDecimalToStr(tranvalue);
        }

        //To fetch Corporate data from core by CIFID
        public JsonResult GetCorporate(string CIFID)
        {
            CorporateResponseInfo cri = new CorporateResponseInfo();
            try
            {
                if (CIFID != "")
                {
                    int count = db.C_Corporates.Where(u => u.CIFID == CIFID & u.DEL_FLAG == false).Count();
                    if (count > 0)
                    {
                        return Json(new { message = "Duplicate CIFID" }, JsonRequestBehavior.AllowGet);
                    }
                    int ReqCount = db.C_Req_Corporates.Where(u => u.CIFID == CIFID & u.STATUS == 0).Count();
                    if (ReqCount > 0)
                    {

                        return Json(new { message = "Already Requested!" }, JsonRequestBehavior.AllowGet);
                    }
                    cri = GetCorporateResponse(CIFID);
                    if (cri.Name != null && cri.lstCorpAccountInfo.Count == 0)
                        return Json(new { message = "No individual account under this CIFID" }, JsonRequestBehavior.AllowGet);
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

        // GET: Corporate/Register
        public ActionResult Register()
        {
            try
            {
                ReqCorporateVM reqCorporate = new ReqCorporateVM();
                bindCountry(reqCorporate.Country);
                GetTransferCharges();
                GetRemittanceCharges();
                GetBranchSelectList();
                GetCorTranValue(); // to bind transaction value into tran_limit textbox
                return View(reqCorporate);
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
        public ActionResult Register(ReqCorporateVM reqCorporate/*,FormCollection frm*/)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //reget core data by CIFID
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    if (reqCorporate.CIFID != "")
                        cri = GetCorporateResponse(reqCorporate.CIFID);
                    if (cri.Name == null) //if wrong CIFID
                    {
                        ModelState.AddModelError("CIFID", "Invalid CIFID");
                        goto ValidationFail;
                    }
                    //checking Validation
                    #region Validation
                    //CIFID duplicate
                    if (db.C_Corporates.Where(u => u.CIFID == reqCorporate.CIFID & u.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("CIFID", "Duplicate CIFID");
                        goto ValidationFail;
                    }
                    //duplicate requested CIFID(ReqCorporates)
                    if (db.C_Req_Corporates.Where(u => u.CIFID == reqCorporate.CIFID & u.ISNEW == 0 & u.STATUS == 0).Count() > 0)
                    {
                        ModelState.AddModelError("CIFID", "This CIFID is already requested.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region check tran limit
                    decimal tran_limit = 0;
                    try
                    {
                        tran_limit = General.ConvertStrToDecimal(reqCorporate.Tran_Limit);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Tran_Limit", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    if (db.Rules.Where(r => r.Code == "R047" & r.Value < tran_limit & r.Del_Flag == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Tran_Limit", "Corporate Transaction Amount has exceeded the Bank Transaction Limit.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region check Bulkfix Charges    
                    decimal bulkfixcharge = 0;
                    try
                    {
                        bulkfixcharge = General.ConvertStrToDecimal(reqCorporate.Bulk_Charges_Fix_Rate);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    //int bulkfixCharge = General.ConvertDecimalToInteger(reqCorporate.Bulk_Charges_Fix_Rate);
                    if (reqCorporate.IS_FIXRATE_BULK_CHARGES == false && bulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region Other bank check Bulkfix Charges  
                    decimal ohterbankbulkfixcharge = 0;
                    try
                    {
                        ohterbankbulkfixcharge = General.ConvertStrToDecimal(reqCorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    // int ohterbankbulkfixcharge = General.ConvertDecimalToInteger(corvm.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    if (reqCorporate.CCT_IS_FIXRATE_BULK_CHARGES == false && ohterbankbulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "Other Bank Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion


                    string LoginUser = User.Identity.GetUserId();
                    //save into requested corporate table (temp)
                    C_Req_Corporate req_Corporate = new C_Req_Corporate();
                    //core data
                    req_Corporate.CIFID = reqCorporate.CIFID;
                    req_Corporate.BRANCH = reqCorporate.Branch;
                    req_Corporate.COMPANY_NAME = cri.Name;
                    req_Corporate.COMPANY_EMAIL = reqCorporate.Company_Email;//(cri.Email == "" ? reqCorporate.Company_Email : cri.Email);
                    req_Corporate.COMPANY_PHONE = reqCorporate.Company_Phone;//cri.PhoneNumber;
                    req_Corporate.COMPANY_ADDRESS = reqCorporate.Company_Address;
                    req_Corporate.COUNTRY_ID = reqCorporate.Country;
                    req_Corporate.STATE_ID = reqCorporate.State;
                    //req_Corporate.CHARGES_CODE = reqCorporate.Charges_Code;  
                    if (reqCorporate.ISVIP)
                    {
                        req_Corporate.ISVIP = reqCorporate.ISVIP;
                        req_Corporate.T_CHARGES_CODE = reqCorporate.T_CHARGES_CODE;
                        req_Corporate.R_CHARGES_CODE = reqCorporate.R_CHARGES_CODE;
                    }
                    else
                    {
                        req_Corporate.ISVIP = reqCorporate.ISVIP;
                        req_Corporate.T_CHARGES_CODE = null;
                        req_Corporate.R_CHARGES_CODE = null;
                    }
                    //Default fields
                    req_Corporate.ISNEW = 0;   //IsNew = 0 for register,IsNew = 1 For Update, IsNew =2 For Delete
                    req_Corporate.STATUS = 0;     //status => 0   request status, 1  approve, 2 reject
                    req_Corporate.MAKER = LoginUser;
                    req_Corporate.RequestedDate = DateTime.Now;
                    req_Corporate.TRAN_LIMIT = General.ConvertStrToDecimal(reqCorporate.Tran_Limit);
                    req_Corporate.IS_FIXRATE_BULK_CHARGES = reqCorporate.IS_FIXRATE_BULK_CHARGES;//Percentage = 0, Fixed Amt = 1
                    req_Corporate.BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(reqCorporate.Bulk_Charges_Fix_Rate);
                    //other bank charges
                    req_Corporate.CCT_IS_FIXRATE_BULK_CHARGES = reqCorporate.CCT_IS_FIXRATE_BULK_CHARGES;
                    req_Corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(reqCorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    db.C_Req_Corporates.Add(req_Corporate);
                    db.SaveChanges();

                    // tranlimit = db.C_CorporateTranRules.Where(t => t.)
                    //save log
                    log.Info(LoginUser, controllerName, actionName, 1, $"Company Name is {req_Corporate.COMPANY_NAME}.");

                    //success
                    return RedirectToAction("index");
                }
                ValidationFail:
                bindCountry(reqCorporate.Country);
                GetBranchSelectList();
                GetTransferCharges();
                GetRemittanceCharges();
                GetCorTranValue();
                return View(reqCorporate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Corporate/Edit/
        public ActionResult Edit(int? id)
        {
            try
            {
                C_Req_Corporate reqcorporate = db.C_Req_Corporates.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                if (reqcorporate == null)
                {
                    return HttpNotFound();
                }
                //RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();
                cri = GetCorporateResponse(reqcorporate.CIFID);
                ReqCorporateVM reqcorVM = new ReqCorporateVM
                {
                    ID = reqcorporate.ID,
                    CIFID = reqcorporate.CIFID,
                    Branch = reqcorporate.BRANCH,
                    //Corporate_ID = reqcorporate.CORPORATEID,
                    Company_Name = cri.Name,
                    //Company_Email = cri.Email,
                    //Company_Name = reqcorporate.COMPANY_NAME,
                    Company_Email = reqcorporate.COMPANY_EMAIL,
                    Company_Address = reqcorporate.COMPANY_ADDRESS,
                    //Company_Phone = cri.PhoneNumber,
                    Company_Phone = reqcorporate.COMPANY_PHONE,
                    Country = reqcorporate.COUNTRY_ID,
                    State = reqcorporate.STATE_ID,
                    //Charges_Code = reqcorporate.CHARGES_CODE,
                    ISVIP = reqcorporate.ISVIP,
                    T_CHARGES_CODE = reqcorporate.T_CHARGES_CODE,
                    R_CHARGES_CODE = reqcorporate.R_CHARGES_CODE,
                    Tran_Limit = General.ConvertCDecimalToStr(reqcorporate.TRAN_LIMIT),
                    IS_FIXRATE_BULK_CHARGES = reqcorporate.IS_FIXRATE_BULK_CHARGES,
                    Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(reqcorporate.BULK_CHARGES_FIX_RATE_N_PERCENT),
                    CCT_IS_FIXRATE_BULK_CHARGES = reqcorporate.CCT_IS_FIXRATE_BULK_CHARGES,
                    CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(reqcorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT)
                };
                bindCountry(reqcorporate.COUNTRY_ID);
                GetTransferCharges();
                GetRemittanceCharges();
                GetBranchSelectList();
                IEnumerable<SelectListItem> items = db.C_Countries
                   .Select(c => new SelectListItem
                   {
                       Value = c.ID.ToString(),
                       Text = c.NAME

                   });
                ViewBag.Country = items;

                IEnumerable<SelectListItem> state = db.C_States
                            .Where(g => g.COUNTRY_ID == reqcorporate.COUNTRY_ID)
                            .Select(f => new SelectListItem()
                            {
                                Value = f.ID.ToString(),
                                Text = f.NAME
                            });
                ViewBag.State = state;

                return View(reqcorVM);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Corporate/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReqCorporateVM ReqCorVM)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    C_Req_Corporate reqCor = db.C_Req_Corporates.Find(ReqCorVM.ID);
                    if (reqCor == null)
                    {
                        return HttpNotFound();
                    }
                    //CorporateResponseInfo cri = new CorporateResponseInfo();
                    //cri = GetCorporateResponse(ReqCorVM.CIFID);
                    #region Validation
                    //not to allow editing approved requests
                    if (reqCor.STATUS == 1) //if approved
                    {
                        ModelState.AddModelError("", "You can't change the approved request!");
                        goto ValidationFail;
                    }
                    //checking CIFID
                    //RetailResponseInfo rri = new RetailResponseInfo();                 

                    //if (reqCor.Name == null) //check Name if corporate CIFID
                    //{
                    //    ModelState.AddModelError("CIFID", "Invalid CIFID");
                    //    bindCountry(reqCor.COUNTRY_ID);
                    //    GetBranchSelectList();
                    //    return View(ReqCorVM);
                    //}
                    #endregion

                    #region check tran limit
                    decimal tran_limit = 0;
                    try
                    {
                        tran_limit = General.ConvertStrToDecimal(ReqCorVM.Tran_Limit);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Tran_Limit", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    if (db.Rules.Where(r => r.Code == "R047" & r.Value < tran_limit & r.Del_Flag == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Tran_Limit", "Corporate Transaction Limit Can't Exceed The Bank Transaction Limit.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region check Bulkfix Charges 
                    decimal bulkfixcharge = 0;
                    try
                    {
                        bulkfixcharge = General.ConvertStrToDecimal(ReqCorVM.Bulk_Charges_Fix_Rate);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    //int bulkfixCharge = General.ConvertDecimalToInteger(ReqCorVM.Bulk_Charges_Fix_Rate);
                    if (ReqCorVM.IS_FIXRATE_BULK_CHARGES == false && bulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region Other bank check Bulkfix Charges  
                    decimal ohterbankbulkfixcharge = 0;
                    try
                    {
                        ohterbankbulkfixcharge = General.ConvertStrToDecimal(ReqCorVM.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    // int ohterbankbulkfixcharge = General.ConvertDecimalToInteger(corvm.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    if (ReqCorVM.CCT_IS_FIXRATE_BULK_CHARGES == false && ohterbankbulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "Other Bank Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion
                    //core data
                    reqCor.COMPANY_NAME = ReqCorVM.Company_Name;//cri.Name;
                    //reqCor.COMPANY_EMAIL = (cri.Email == "" ? ReqCorVM.Company_Email : cri.Email);
                    //reqCor.COMPANY_PHONE = cri.PhoneNumber;

                    //formdata
                    //get form data if request is only new (register
                    reqCor.COMPANY_EMAIL = ReqCorVM.Company_Email;
                    reqCor.COMPANY_PHONE = ReqCorVM.Company_Phone;
                    reqCor.COMPANY_ADDRESS = ReqCorVM.Company_Address;
                    reqCor.BRANCH = ReqCorVM.Branch;
                    reqCor.TRAN_LIMIT = General.ConvertStrToInt(ReqCorVM.Tran_Limit);
                    reqCor.IS_FIXRATE_BULK_CHARGES = ReqCorVM.IS_FIXRATE_BULK_CHARGES;
                    reqCor.BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(ReqCorVM.Bulk_Charges_Fix_Rate);
                    reqCor.COUNTRY_ID = ReqCorVM.Country;
                    reqCor.STATE_ID = ReqCorVM.State;
                    reqCor.CCT_IS_FIXRATE_BULK_CHARGES = ReqCorVM.CCT_IS_FIXRATE_BULK_CHARGES;
                    reqCor.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(ReqCorVM.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    //reqCor.CHARGES_CODE = ReqCorVM.Charges_Code;
                    if (ReqCorVM.ISVIP)
                    {
                        reqCor.ISVIP = ReqCorVM.ISVIP;
                        reqCor.T_CHARGES_CODE = ReqCorVM.T_CHARGES_CODE;
                        reqCor.R_CHARGES_CODE = ReqCorVM.R_CHARGES_CODE;
                    }
                    else
                    {
                        reqCor.ISVIP = ReqCorVM.ISVIP;
                        reqCor.T_CHARGES_CODE = null;
                        reqCor.R_CHARGES_CODE = null;
                    }
                    //default fields
                    if (reqCor.STATUS == 2)
                    {
                        reqCor.STATUS = 0;     //change status back to requested status
                    }
                    reqCor.RequestedDate = DateTime.Now;
                    db.Entry(reqCor).State = EntityState.Modified;
                    db.SaveChanges();
                    // save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{ReqCorVM.Company_Name} is updated.");
                    return RedirectToAction("Index");
                }
                ValidationFail:
                bindCountry(ReqCorVM.Country);
                GetTransferCharges();
                GetRemittanceCharges();
                GetBranchSelectList();
                return View(ReqCorVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Corporate/Delete/
        public ActionResult Delete(int? id)
        {
            try
            {
                C_Req_Corporate req_Corporate = db.C_Req_Corporates.Where(u => u.STATUS != 1 && u.ID == id).FirstOrDefault();
                if (req_Corporate == null)
                {
                    return HttpNotFound();
                }
                var isVIP = req_Corporate.ISVIP;
                ViewBag.IsVIP = (isVIP == true);
                return View(req_Corporate);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Corporate/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Req_Corporate req_Corporate = db.C_Req_Corporates.Find(id);
                //don't allow to delete approved request
                if (req_Corporate.STATUS == 1)
                {
                    ModelState.AddModelError("", "You can't delete approved request.");
                    return RedirectToAction("Delete", new { id = id });
                }
                //save log
                string getuserid = User.Identity.GetUserId();
                //delete user
                db.C_Req_Corporates.Remove(req_Corporate);
                db.SaveChanges();
                //save log
                log.Info(getuserid, controllerName, actionName, 3);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        //Request in already table data
        public ActionResult CorporateLists()
        {
            return View();
        }
        [HttpPost]
        // GET: Corporate
        public ActionResult CorporateLists(int? arg)
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
            var companyname = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var companyphone = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            string userid = User.Identity.GetUserId();
            C_Corporate corporates = db.C_Corporates.Where(u => u.DEL_FLAG == false).FirstOrDefault();
            C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == corporates.ID).FirstOrDefault();
            var corporate = db.C_Corporates.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new CorporateVM
            {
                ID = u.ID,
                CIFID = u.CIFID,
                Branch = u.BRANCH,
                CorporateID = u.CORPORATEID,
                Company_Name = u.COMPANY_NAME,
                Company_Email = u.COMPANY_EMAIL,
                Company_Address = u.COMPANY_ADDRESS,
                Company_Phone = u.COMPANY_PHONE,
                ISVIP = u.ISVIP,
                T_CHARGES_CODE = u.T_CHARGES_CODE,
                R_CHARGES_CODE = u.R_CHARGES_CODE,
                Tran_Limit = General.ConvertDecimalToStr(cortranrule.Value),//Tran_Limit
                Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(u.BULK_CHARGES_FIX_RATE_N_PERCENT)
            });
            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                corporate = corporate.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(companyname.Trim()))
            {
                corporate = corporate.Where(a => a.Company_Name.ToLower().Contains(companyname.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(companyphone.Trim()))
            {
                corporate = corporate.Where(a => a.Company_Phone.Contains(companyphone.Trim())).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                corporate = corporate.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = corporate.Count();
            var data = corporate.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Corporate/Details/5
        public ActionResult CorporateDetail(int? id)
        {
            {
                C_Corporate corporate = db.C_Corporates.Where(u => u.DEL_FLAG == false && u.ID == id).FirstOrDefault();
                if (corporate == null)
                {
                    return HttpNotFound();
                }
                var rule = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
                C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == corporate.ID).FirstOrDefault();
                CorporateVM corvm = new CorporateVM
                {
                    ID = corporate.ID,
                    CIFID = corporate.CIFID,
                    Branch = corporate.BRANCH,
                    CorporateID = corporate.CORPORATEID,
                    Company_Name = corporate.COMPANY_NAME,
                    Company_Email = corporate.COMPANY_EMAIL,
                    Company_Address = corporate.COMPANY_ADDRESS,
                    Company_Phone = corporate.COMPANY_PHONE,
                    ISVIP = corporate.ISVIP,
                    T_CHARGES_CODE = corporate.T_CHARGES_CODE,
                    R_CHARGES_CODE = corporate.R_CHARGES_CODE,
                    Tran_Limit = General.ConvertDecimalToStr(cortranrule.Value),//corporate.Tran_Limit,
                    IS_FIXRATE_BULK_CHARGES = corporate.IS_FIXRATE_BULK_CHARGES,
                    Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(corporate.BULK_CHARGES_FIX_RATE_N_PERCENT),
                    CCT_IS_FIXRATE_BULK_CHARGES = corporate.CCT_IS_FIXRATE_BULK_CHARGES,
                    CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT),
                    CreatedDateTime = corporate.CreatedDateTime
                };
                ViewBag.Country = (corporate.VrCountry == null ? null : corporate.VrCountry.NAME);
                ViewBag.State = (corporate.VrState == null ? null : corporate.VrState.NAME);
                ViewBag.Branch = (corporate.VrBranch == null ? null : corporate.VrBranch.name);
                ViewBag.IsVIP = corporate.ISVIP;//for check vip or not
                //ViewBag.Country = corporate.VrCountry.NAME;
                //ViewBag.State = corporate.VrState.NAME;
                return View(corvm);
            }
        }
        // GET: Corporate/Edit/5
        public ActionResult CorporateEdit(int? id)
        {
            C_Corporate corporate = db.C_Corporates.Where(u => u.DEL_FLAG == false && u.ID == id).FirstOrDefault();
            if (corporate == null)
            {
                return HttpNotFound();
            }
            var rule = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
            C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == corporate.ID).FirstOrDefault();
            CorporateResponseInfo cri = new CorporateResponseInfo();
            cri = GetCorporateResponse(corporate.CIFID);
            CorporateVM corvm = new CorporateVM
            {
                REQ_CORPORATEID = corporate.ID,
                CorporateID = corporate.CORPORATEID,
                CIFID = corporate.CIFID,
                Branch = corporate.BRANCH,
                Company_Name = cri.Name,//corporate.COMPANY_NAME,//cri.Name,
                Company_Email = corporate.COMPANY_EMAIL,//cri.Email,
                Company_Address = corporate.COMPANY_ADDRESS,
                Company_Phone = corporate.COMPANY_PHONE,//cri.PhoneNumber,
                COUNTRY_ID = corporate.COUNTRY_ID,
                STATE_ID = corporate.STATE_ID,
                ISVIP = corporate.ISVIP,
                T_CHARGES_CODE = corporate.T_CHARGES_CODE,
                R_CHARGES_CODE = corporate.R_CHARGES_CODE,
                Tran_Limit = General.ConvertCDecimalToStr(cortranrule.Value),
                IS_FIXRATE_BULK_CHARGES = corporate.IS_FIXRATE_BULK_CHARGES,
                Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(corporate.BULK_CHARGES_FIX_RATE_N_PERCENT),
                CCT_IS_FIXRATE_BULK_CHARGES = corporate.CCT_IS_FIXRATE_BULK_CHARGES,
                CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT)
            };

            bindCountry(corporate.COUNTRY_ID);
            GetTransferCharges();
            GetRemittanceCharges();
            GetBranchSelectList();
            IEnumerable<SelectListItem> items = db.C_Countries
               .Select(c => new SelectListItem
               {
                   Value = c.ID.ToString(),
                   Text = c.NAME

               });
            ViewBag.Country = items;

            IEnumerable<SelectListItem> state = db.C_States
                        .Where(g => g.COUNTRY_ID == corporate.COUNTRY_ID)
                        .Select(f => new SelectListItem()
                        {
                            Value = f.ID.ToString(),
                            Text = f.NAME
                        });
            ViewBag.State = state;
            return View(corvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CorporateEdit(CorporateVM corvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    C_Corporate corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false && c.ID == corvm.ID).FirstOrDefault();
                    if (corporate == null)
                    {
                        return HttpNotFound();
                    }
                    C_Req_Corporate reqcorporate = new C_Req_Corporate();
                    //checking CIFID
                    //RetailResponseInfo rri = new RetailResponseInfo();
                    CorporateResponseInfo cri = new CorporateResponseInfo();
                    cri = GetCorporateResponse(corvm.CIFID);

                    #region Validation                    
                    var reqCor = db.C_Req_Corporates.Where(r => r.CIFID == corvm.CIFID && r.STATUS == 0 && (r.ISNEW == 1 || r.ISNEW == 2)).Count();
                    if (reqCor > 0)
                    {
                        ModelState.AddModelError("", "Already Requested!");
                        goto ValidationFail;
                    }

                    if (cri.Name == null) //check Name if corporate CIFID
                    {
                        ModelState.AddModelError("CIFID", "Invalid CIFID");
                        goto ValidationFail;
                    }

                    #endregion

                    #region check tran limit
                    decimal tran_limit = 0;
                    try
                    {
                        tran_limit = General.ConvertStrToDecimal(corvm.Tran_Limit);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Tran_Limit", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    if (db.Rules.Where(r => r.Code == "R047" & r.Value < tran_limit & r.Del_Flag == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Tran_Limit", "Corporate Transaction Limit Can't Exceed The Bank Transaction Limit.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region check Bulkfix Charges  
                    decimal bulkfixcharge = 0;
                    try
                    {
                        bulkfixcharge = General.ConvertStrToDecimal(corvm.Bulk_Charges_Fix_Rate);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    //int bulkfixCharge = General.ConvertDecimalToInteger(corvm.Bulk_Charges_Fix_Rate);
                    //int bulkfixCharge = General.ConvertStrToInt(corvm.Bulk_Charges_Fix_Rate);
                    if (corvm.IS_FIXRATE_BULK_CHARGES == false && bulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("Bulk_Charges_Fix_Rate", "Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion

                    #region Other bank check Bulkfix Charges  
                    decimal ohterbankbulkfixcharge = 0;
                    try
                    {
                        ohterbankbulkfixcharge = General.ConvertStrToDecimal(corvm.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "The field Allow Transaction limit must be between 0 and 9999999999999999.99");
                        goto ValidationFail;
                    }
                    //int ohterbankbulkfixcharge = General.ConvertDecimalToInteger(corvm.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    if (corvm.CCT_IS_FIXRATE_BULK_CHARGES == false && ohterbankbulkfixcharge >= 100)
                    {
                        ModelState.AddModelError("CCT_BULK_CHARGES_FIX_RATE_N_PERCENT", "Other Bank Bulk Charges Fix Rate Percentage value can't exceed 100.");
                        goto ValidationFail;
                    }
                    #endregion

                    string LoginUser = User.Identity.GetUserId();
                    //core data
                    reqcorporate.REQ_CORPORATEID = corporate.ID;
                    //reqcorporate.COMPANY_EMAIL = cri.Email;
                    //reqcorporate.COMPANY_PHONE = cri.PhoneNumber;
                    reqcorporate.COMPANY_NAME = cri.Name;
                    //formdata                    
                    //get form data if request is only new (register)  
                    reqcorporate.COMPANY_EMAIL = corvm.Company_Email;
                    reqcorporate.COMPANY_PHONE = corvm.Company_Phone;
                    reqcorporate.CIFID = corvm.CIFID;
                    reqcorporate.CORPORATEID = corvm.CorporateID;
                    reqcorporate.BRANCH = corvm.Branch;
                    reqcorporate.COMPANY_ADDRESS = corvm.Company_Address;
                    reqcorporate.TRAN_LIMIT = General.ConvertStrToInt(corvm.Tran_Limit);
                    reqcorporate.IS_FIXRATE_BULK_CHARGES = corvm.IS_FIXRATE_BULK_CHARGES;
                    reqcorporate.BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(corvm.Bulk_Charges_Fix_Rate);
                    reqcorporate.CCT_IS_FIXRATE_BULK_CHARGES = corvm.CCT_IS_FIXRATE_BULK_CHARGES;
                    reqcorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertStrToDecimal(corvm.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT);
                    reqcorporate.COUNTRY_ID = corvm.COUNTRY_ID;
                    reqcorporate.STATE_ID = corvm.STATE_ID;
                    if (corvm.ISVIP)
                    {
                        reqcorporate.ISVIP = corvm.ISVIP;
                        reqcorporate.T_CHARGES_CODE = corvm.T_CHARGES_CODE;
                        reqcorporate.R_CHARGES_CODE = corvm.R_CHARGES_CODE;
                    }
                    else
                    {
                        reqcorporate.ISVIP = corvm.ISVIP;
                        reqcorporate.T_CHARGES_CODE = null;
                        reqcorporate.R_CHARGES_CODE = null;
                    }

                    //Default fields
                    reqcorporate.CheckedReason = "";
                    reqcorporate.ISNEW = 1;   //0 for register, 1 for update, 2 for delete
                    reqcorporate.STATUS = 0;
                    reqcorporate.MAKER = LoginUser;
                    reqcorporate.RequestedDate = DateTime.Now;
                    db.C_Req_Corporates.Add(reqcorporate);
                    db.SaveChanges();
                    // save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2, $"{reqcorporate.COMPANY_NAME} is updated.");
                    return RedirectToAction("Index");
                }
                ValidationFail:
                bindCountry(corvm.COUNTRY_ID);
                GetTransferCharges();
                GetRemittanceCharges();
                GetBranchSelectList();
                return View(corvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        public ActionResult CorporateDelete(int? id)
        {
            try
            {
                C_Corporate corporate = db.C_Corporates.Where(u => u.DEL_FLAG == false && u.ID == id).FirstOrDefault();
                C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == corporate.ID).FirstOrDefault();
                CorporateVM corvm = new CorporateVM
                {
                    ID = corporate.ID,
                    CIFID = corporate.CIFID,
                    Branch = corporate.BRANCH,
                    CorporateID = corporate.CORPORATEID,
                    Company_Name = corporate.COMPANY_NAME,
                    Company_Email = corporate.COMPANY_EMAIL,
                    Company_Address = corporate.COMPANY_ADDRESS,
                    Company_Phone = corporate.COMPANY_PHONE,
                    ISVIP = corporate.ISVIP,
                    T_CHARGES_CODE = corporate.T_CHARGES_CODE,
                    R_CHARGES_CODE = corporate.R_CHARGES_CODE,
                    Tran_Limit = General.ConvertDecimalToStr(cortranrule.Value),//corporate.Tran_Limit,
                    IS_FIXRATE_BULK_CHARGES = corporate.IS_FIXRATE_BULK_CHARGES,
                    Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(corporate.BULK_CHARGES_FIX_RATE_N_PERCENT),
                    CCT_IS_FIXRATE_BULK_CHARGES = corporate.CCT_IS_FIXRATE_BULK_CHARGES,
                    CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT),

                    CreatedDateTime = corporate.CreatedDateTime
                };
                ViewBag.Country = (corporate.VrCountry == null ? null : corporate.VrCountry.NAME);
                ViewBag.State = (corporate.VrState == null ? null : corporate.VrState.NAME);
                ViewBag.Branch = (corporate.VrBranch == null ? null : corporate.VrBranch.name);
                var isVIP = corporate.ISVIP;
                ViewBag.IsVIP = (isVIP == true);
                if (corporate == null)
                {
                    return HttpNotFound();
                }
                return View(corvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        // POST: Corporate/Delete/5
        [HttpPost, ActionName("CorporateDelete")]
        public ActionResult CorporateDeleteConfirmed(CorporateVM corvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get corporate by id
                C_Corporate corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false && c.ID == corvm.ID).FirstOrDefault();
                if (corporate == null)
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);

                //check already requested and check delete request
                int existcount = db.C_Req_Corporates.Where(u => u.REQ_CORPORATEID == corvm.ID && (u.ISNEW == 2 || u.ISNEW == 1) && u.STATUS == 0).Count();
                if (existcount > 0)
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);

                Boolean isSuccess = AddReqChange(corporate);//For Delete Request
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting delete successfully.", $"Company Name is {corporate.COMPANY_NAME}");
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

        private Boolean AddReqChange(C_Corporate corporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string LoginUser = User.Identity.GetUserId();
            //Add req to ReqChanges table
            try
            {
                //save into requested corporate table (temp)
                CorporateResponseInfo cri = new CorporateResponseInfo();
                cri = GetCorporateResponse(corporate.CIFID);
                var tranlimit = db.C_CorporateTranRules.Where(c => c.CorporateId == corporate.ID && c.IsDelete == false).FirstOrDefault();
                C_Req_Corporate req_Corporate = new C_Req_Corporate();
                //core data
                req_Corporate.CORPORATEID = corporate.CORPORATEID;
                req_Corporate.REQ_CORPORATEID = corporate.ID;
                req_Corporate.CIFID = corporate.CIFID;
                req_Corporate.BRANCH = corporate.BRANCH;
                req_Corporate.COMPANY_NAME = cri.Name;
                req_Corporate.COMPANY_EMAIL = corporate.COMPANY_EMAIL;//cri.Email;
                req_Corporate.COMPANY_PHONE = corporate.COMPANY_PHONE;//cri.PhoneNumber;
                req_Corporate.COMPANY_ADDRESS = corporate.COMPANY_ADDRESS;
                req_Corporate.COUNTRY_ID = corporate.COUNTRY_ID;
                req_Corporate.STATE_ID = corporate.STATE_ID;
                req_Corporate.ISVIP = corporate.ISVIP;
                req_Corporate.T_CHARGES_CODE = corporate.T_CHARGES_CODE;
                req_Corporate.R_CHARGES_CODE = corporate.R_CHARGES_CODE;
                req_Corporate.TRAN_LIMIT = tranlimit.Value;//corporate.Tran_Limit;
                req_Corporate.IS_FIXRATE_BULK_CHARGES = corporate.IS_FIXRATE_BULK_CHARGES;
                req_Corporate.BULK_CHARGES_FIX_RATE_N_PERCENT = corporate.BULK_CHARGES_FIX_RATE_N_PERCENT;
                req_Corporate.CCT_IS_FIXRATE_BULK_CHARGES = corporate.CCT_IS_FIXRATE_BULK_CHARGES;
                req_Corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT;
                //Default fields
                req_Corporate.CheckedReason = "";
                req_Corporate.ISNEW = 2;   //0 for register, 1 for update , 2 for delete
                req_Corporate.STATUS = 0;     //status => 0   request status, 1  approve, 2 reject
                req_Corporate.MAKER = LoginUser;
                req_Corporate.RequestedDate = DateTime.Now;
                db.C_Req_Corporates.Add(req_Corporate);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(LoginUser, controllerName, actionName, ex);
            }
            return false;
        }
        // GET: Approve
        public ActionResult ApproveIndex()
        {
            //show only requests of his/her Branch
            //get BranchID from Logged In User
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);

            //Get Branch Code and check is branch code of login user "Head Office" or not
            //var code = admin.VrBranch.code;          

            Getddl();
            return View();

        }
        [HttpPost]
        public ActionResult ApproveIndex(int? arg1)
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
            var aCompanyName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var aType = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var aStatus = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            string userid = User.Identity.GetUserId();
            AdminUser admin = db.Users.Find(userid);
            //var aBranch = "";
            //if (admin.VrBranch.code == "9999")
            //{
            //    aBranch = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            //}

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
            //only pending status(not approved/rejected), not own request
            //if admin is head office's admin, show requests of all branches
            //if branch admin, only same branch requests
            //requservms = db.ReqUsers.Where(u => ((admin.BranchID == u.BranchID || admin.VrBranch.code == "9999") && u.STATUS == 0 && u.MAKER != userid) || u.CHECKER == userid).ToList()
            IEnumerable<ReqCorporateVM> reqcorvm = db.C_Req_Corporates.Where(u => ((admin.BranchID == u.VrMaker.BranchID || admin.VrBranch.code == "9999") && u.MAKER != userid) || u.CHECKER == userid).ToList()
                .ConvertAll(rc => new ReqCorporateVM
                {
                    ID = rc.ID,
                    CIFID = rc.CIFID,
                    Branch = rc.BRANCH,
                    Corporate_ID = rc.CORPORATEID,
                    Company_Name = rc.COMPANY_NAME,
                    Company_Email = rc.COMPANY_EMAIL,
                    Company_Address = rc.COMPANY_ADDRESS,
                    Company_Phone = rc.COMPANY_PHONE,
                    //Charges_Code = rc.CHARGES_CODE,
                    Tran_Limit = General.ConvertDecimalToStr(rc.TRAN_LIMIT),
                    Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(rc.BULK_CHARGES_FIX_RATE_N_PERCENT),
                    //other bank cherges rate
                    CCT_IS_FIXRATE_BULK_CHARGES = rc.CCT_IS_FIXRATE_BULK_CHARGES,
                    CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(rc.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT),
                    STATUS = rc.STATUS,
                    ISNEW = rc.ISNEW,
                    MAKER = rc.MAKER,
                    RequestedDate = rc.RequestedDate,
                    CHECKER = rc.CHECKER,
                    CheckedReason = rc.CheckedReason
                });

            //Get Branch Code and check is branch code of login user "Head Office" or not
            var code = admin.VrBranch.code;
            // ViewBag.IsHeadOffice = (code == "9999");

            if (!string.IsNullOrEmpty(aCIFID))
            {
                reqcorvm = reqcorvm.Where(a => a.CIFID.ToLower().Trim().Contains(aCIFID.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(aCompanyName))
            {
                reqcorvm = reqcorvm.Where(a => a.Company_Name.ToLower().Trim().Contains(aCompanyName.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(aType))
            {
                reqcorvm = reqcorvm.Where(a => a.ISNEW.ToString() == aType);

            }
            if (!string.IsNullOrEmpty(aStatus))
            {
                reqcorvm = reqcorvm.Where(a => a.STATUS.ToString() == aStatus);

            }
            if (fromDate != null & toDate != null)
            {
                reqcorvm = reqcorvm.Where(a => a.RequestedDate >= fromDate && a.RequestedDate < toDate);
            }
            else if (fromDate != null)
            {
                reqcorvm = reqcorvm.Where(a => a.RequestedDate >= fromDate);
            }
            else if (toDate != null)
            {
                reqcorvm = reqcorvm.Where(a => a.RequestedDate < toDate);
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                reqcorvm = reqcorvm.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = reqcorvm.Count();
            var data = reqcorvm.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
               JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            try
            {
                C_Req_Corporate req_Corporate = db.C_Req_Corporates.Where(u => u.ID == id).FirstOrDefault();
                C_Corporate corporate = db.C_Corporates.Where(c => c.CIFID == req_Corporate.CIFID).FirstOrDefault();
                if (req_Corporate == null)
                {
                    return HttpNotFound();
                }
                ReqCorporateVM reqcorvm = new ReqCorporateVM
                {
                    ID = req_Corporate.ID,
                    CIFID = req_Corporate.CIFID,
                    Branch = req_Corporate.BRANCH,
                    Corporate_ID = req_Corporate.CORPORATEID,
                    Company_Name = req_Corporate.COMPANY_NAME,
                    Company_Email = req_Corporate.COMPANY_EMAIL,
                    Company_Address = req_Corporate.COMPANY_ADDRESS,
                    Company_Phone = req_Corporate.COMPANY_PHONE,
                    //Charges_Code = req_Corporate.CHARGES_CODE,
                    ISVIP = req_Corporate.ISVIP,
                    T_CHARGES_CODE = req_Corporate.T_CHARGES_CODE,
                    R_CHARGES_CODE = req_Corporate.R_CHARGES_CODE,
                    Tran_Limit = General.ConvertDecimalToStr(req_Corporate.TRAN_LIMIT),
                    IS_FIXRATE_BULK_CHARGES = req_Corporate.IS_FIXRATE_BULK_CHARGES,
                    Bulk_Charges_Fix_Rate = General.ConvertDecimalToStr(req_Corporate.BULK_CHARGES_FIX_RATE_N_PERCENT),
                    CCT_IS_FIXRATE_BULK_CHARGES = req_Corporate.CCT_IS_FIXRATE_BULK_CHARGES,
                    CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = General.ConvertDecimalToStr(req_Corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT),
                    STATUS = req_Corporate.STATUS,
                    ISNEW = req_Corporate.ISNEW,
                    RequestedDate = req_Corporate.RequestedDate,
                    CheckedDate = req_Corporate.CheckedDate,
                    CheckedReason = req_Corporate.CheckedReason,
                    CreatedDate = (corporate == null ? null : corporate.CreatedDateTime)
                };
                ViewBag.Country = (req_Corporate.VrCountry == null ? null : req_Corporate.VrCountry.NAME);
                ViewBag.State = (req_Corporate.VrState == null ? null : req_Corporate.VrState.NAME);
                ViewBag.Maker = req_Corporate.VrMaker.UserName;
                ViewBag.Checker = (req_Corporate.VrChecker == null ? null : req_Corporate.VrChecker.UserName);
                ViewBag.Branch = (req_Corporate.VrBranch == null ? null : req_Corporate.VrBranch.name);
                var isVIP = req_Corporate.ISVIP;
                ViewBag.IsVIP = (isVIP == true);
                return View(reqcorvm);
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
            try
            {
                C_Req_Corporate reqCorporate = db.C_Req_Corporates.Find(id);
                Boolean isSave = false;
                Boolean isSaveBulkFile = false;
                Boolean isSaveTranRule = false;
                //validate
                //**Close For Corporate Core**
                string error = CheckValidate(reqCorporate);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }
                //Register(save) if ISNEW is 0(register = 0, update = 1 , delete = 2)
                if (reqCorporate.ISNEW == 0)
                {
                    //string daily_max_amt = (ruser.IsVIP ? ruser.DAILY_MAX_AMT.ToString() : GetRule("R005").ToString());
                    //ReqChange reqChange = db.ReqChanges.Find(id);
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            C_Corporate corporate = new C_Corporate();
                            // try to save at Corporate table first
                            isSave = SaveCorporate(reqCorporate, ref corporate);
                            isSaveBulkFile = SaveBulkFileRecord(reqCorporate, ref corporate);
                            isSaveTranRule = SaveTranRule(reqCorporate, ref corporate);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            reqCorporate.STATUS = 1;
                            reqCorporate.REQ_CORPORATEID = corporate.ID;
                            reqCorporate.CHECKER = User.Identity.GetUserId();
                            reqCorporate.CheckedDate = DateTime.Now;
                            //Checker Reason

                            db.Entry(reqCorporate).State = EntityState.Modified;
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
                }
                else if (reqCorporate.ISNEW == 1) //Update if ISNEW is 1 
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        C_Corporate corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false && c.ID == reqCorporate.REQ_CORPORATEID).FirstOrDefault();
                        try
                        {
                            //to update user
                            isSave = UpdateCorporate(reqCorporate);
                            isSaveTranRule = UpdateCorporateTranRule(reqCorporate, corporate.ID);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            reqCorporate.STATUS = 1;
                            reqCorporate.CHECKER = User.Identity.GetUserId();
                            reqCorporate.CheckedDate = DateTime.Now;
                            //Checker Reason

                            db.Entry(reqCorporate).State = EntityState.Modified;
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
                else
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            isSave = DeleteCorporate(reqCorporate);
                            isSaveTranRule = DeleteCorporateTranRule(reqCorporate);
                            reqCorporate.STATUS = 1;
                            reqCorporate.CHECKER = User.Identity.GetUserId();
                            reqCorporate.CheckedDate = DateTime.Now;
                            //Checker Reason
                            db.Entry(reqCorporate).State = EntityState.Modified;
                            db.SaveChanges();
                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            ViewData["Message"] = "Delete successfully!";
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
            return Json(new { msg = "Approve successfully!" }, JsonRequestBehavior.AllowGet);
        }
        //saving BulkFileRecord
        private Boolean SaveBulkFileRecord(C_Req_Corporate reqCorporate, ref C_Corporate corporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {

                C_Bulk_File_Record bulkFile = new C_Bulk_File_Record();
                bulkFile.Corporate_ID = corporate.ID;
                var bulkfilerecord = db.Rules.Where(r => r.Code == "R044").FirstOrDefault();
                int IntRule = Convert.ToInt32(bulkfilerecord.Value);
                bulkFile.Max_Record = IntRule;
                bulkFile.CreatedDateTime = DateTime.Now;
                bulkFile.CreatedUserID = User.Identity.GetUserId();
                bulkFile.DEL_FLAG = false;

                db.C_Bulk_File_Records.Add(bulkFile);
                db.SaveChanges();

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"BulkFileRecord is Saved.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //saving TBL_USER
        private Boolean SaveCorporate(C_Req_Corporate reqCorporate, ref C_Corporate corporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {

                //formdata
                corporate.CIFID = reqCorporate.CIFID;
                //corporate.CorporateID = reqCorporate.CORPORATEID; //no need, CorporateID is db auto-generate.
                corporate.BRANCH = reqCorporate.BRANCH;
                corporate.COMPANY_NAME = reqCorporate.COMPANY_NAME;
                corporate.COMPANY_EMAIL = reqCorporate.COMPANY_EMAIL;
                corporate.COMPANY_PHONE = reqCorporate.COMPANY_PHONE;
                corporate.COMPANY_ADDRESS = reqCorporate.COMPANY_ADDRESS;
                corporate.COUNTRY_ID = reqCorporate.COUNTRY_ID;
                corporate.STATE_ID = reqCorporate.STATE_ID;
                if (reqCorporate.ISVIP)
                {
                    corporate.ISVIP = reqCorporate.ISVIP;
                    corporate.T_CHARGES_CODE = reqCorporate.T_CHARGES_CODE;
                    corporate.R_CHARGES_CODE = reqCorporate.R_CHARGES_CODE;
                }
                else
                {
                    corporate.ISVIP = reqCorporate.ISVIP;
                    corporate.T_CHARGES_CODE = null;
                    corporate.R_CHARGES_CODE = null;
                }
                corporate.ISVIP = reqCorporate.ISVIP;
                corporate.T_CHARGES_CODE = reqCorporate.T_CHARGES_CODE;
                corporate.R_CHARGES_CODE = reqCorporate.R_CHARGES_CODE;
                //corporate.Tran_Limit = reqCorporate.Tran_Limit;
                corporate.IS_FIXRATE_BULK_CHARGES = reqCorporate.IS_FIXRATE_BULK_CHARGES;
                corporate.BULK_CHARGES_FIX_RATE_N_PERCENT = reqCorporate.BULK_CHARGES_FIX_RATE_N_PERCENT;
                corporate.CCT_IS_FIXRATE_BULK_CHARGES = reqCorporate.CCT_IS_FIXRATE_BULK_CHARGES;
                corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = reqCorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT;
                corporate.CreatedDateTime = DateTime.Now;
                corporate.CreatedUserID = User.Identity.GetUserId();

                db.C_Corporates.Add(corporate);
                db.SaveChanges();

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"{ corporate.COMPANY_NAME} is Created.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //SaveTranRule
        private Boolean SaveTranRule(C_Req_Corporate reqcorporate, ref C_Corporate corporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
                C_CorporateTranRule cortranrule = new C_CorporateTranRule();
                cortranrule.CorporateId = corporate.ID;
                cortranrule.Value = reqcorporate.TRAN_LIMIT;
                cortranrule.IsDelete = false;
                cortranrule.RuleCode = rule.Code;

                db.C_CorporateTranRules.Add(cortranrule);
                db.SaveChanges();

                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 1, $"Rule Code {cortranrule.RuleCode}, Corporate ID {corporate.ID}, Rule Value {cortranrule.Value} is Saved.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }

        //Delete Corporate
        private Boolean DeleteCorporate(C_Req_Corporate reqCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Corporate corporate = db.C_Corporates.Where(r => r.ID == reqCorporate.REQ_CORPORATEID).FirstOrDefault();
                corporate.DEL_FLAG = true;
                corporate.UpdatedDateTime = DateTime.Now;
                corporate.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(corporate).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2, $"{corporate.COMPANY_NAME} is updated.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }

        //Delete corporate TranRule
        private Boolean DeleteCorporateTranRule(C_Req_Corporate reqCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_CorporateTranRule corporateTranRule = db.C_CorporateTranRules.Where(t => t.CorporateId == reqCorporate.REQ_CORPORATEID).FirstOrDefault();
                corporateTranRule.IsDelete = true;
                db.Entry(corporateTranRule).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2, $"Corporate TranRule is updated.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }
        //update user
        private Boolean UpdateCorporate(C_Req_Corporate reqCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Corporate corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false && c.ID == reqCorporate.REQ_CORPORATEID).FirstOrDefault();
                //RetailResponseInfo rri = GetRetailResponse(ruser.CIFID);
                //RetailResponseInfo rri = new RetailResponseInfo();
                CorporateResponseInfo cri = new CorporateResponseInfo();
                cri = GetCorporateResponse(corporate.CIFID);
                //validate 
                corporate.COMPANY_NAME = cri.Name;
                //corporate.COMPANY_EMAIL = cri.Email;//********need to consider from core or user input
                //corporate.COMPANY_PHONE = cri.PhoneNumber;//********need to consider from core or user input
                corporate.COMPANY_EMAIL = reqCorporate.COMPANY_EMAIL;
                corporate.COMPANY_PHONE = reqCorporate.COMPANY_PHONE;
                corporate.COMPANY_ADDRESS = reqCorporate.COMPANY_ADDRESS;
                corporate.COUNTRY_ID = reqCorporate.COUNTRY_ID;
                corporate.STATE_ID = reqCorporate.STATE_ID;
                corporate.T_CHARGES_CODE = reqCorporate.T_CHARGES_CODE;
                //corporate.Tran_Limit = reqCorporate.Tran_Limit;
                corporate.BULK_CHARGES_FIX_RATE_N_PERCENT = reqCorporate.BULK_CHARGES_FIX_RATE_N_PERCENT;
                corporate.IS_FIXRATE_BULK_CHARGES = reqCorporate.IS_FIXRATE_BULK_CHARGES;
                //other bank tan limit
                corporate.CCT_IS_FIXRATE_BULK_CHARGES = reqCorporate.CCT_IS_FIXRATE_BULK_CHARGES;
                corporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT = reqCorporate.CCT_BULK_CHARGES_FIX_RATE_N_PERCENT;
                //Check VIP        
                if (reqCorporate.ISVIP)
                {
                    corporate.ISVIP = reqCorporate.ISVIP;
                    corporate.T_CHARGES_CODE = reqCorporate.T_CHARGES_CODE;
                    corporate.R_CHARGES_CODE = reqCorporate.R_CHARGES_CODE;
                }
                else
                {
                    corporate.ISVIP = reqCorporate.ISVIP;
                    corporate.T_CHARGES_CODE = null;
                    corporate.R_CHARGES_CODE = null;
                }
                //default fields
                corporate.UpdatedDateTime = DateTime.Now;
                corporate.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(corporate).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2, $"{corporate.COMPANY_NAME} is updated.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }

        //Update TranRule
        private Boolean UpdateCorporateTranRule(C_Req_Corporate reqCorporate, int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == reqCorporate.REQ_CORPORATEID).FirstOrDefault();
                var rule = db.Rules.Where(r => r.Code == "R043").FirstOrDefault();
                cortranrule.CorporateId = id;
                cortranrule.Value = reqCorporate.TRAN_LIMIT;
                cortranrule.IsDelete = false;
                cortranrule.RuleCode = rule.Code;

                db.Entry(cortranrule).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 2, $"CorporateTranRule is updated.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }
        private string CheckValidate(C_Req_Corporate reqCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //checking Validation
                #region Validation
                //check current status
                CorporateResponseInfo cri = new CorporateResponseInfo();
                cri = GetCorporateResponse(reqCorporate.CIFID);
                //if already approved
                if (reqCorporate.STATUS == 1)
                    return "Already approved.";
                //if already rejected
                if (reqCorporate.STATUS == 2)
                    return "Already rejected.";
                //RetailResponseInfo rri = new RetailResponseInfo();
                if (cri.Name == null) //if wrong CIFID
                {
                    return "Invalid CIFID";
                }
                //CIFID duplicate
                if (reqCorporate.ISNEW == 0 && (db.C_Corporates.Where(u => u.CIFID == reqCorporate.CIFID & u.DEL_FLAG == false).Count() > 0))
                {
                    return "Duplicate CIFID";
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
        //Reject Retail
        [HttpPost]
        public ActionResult Reject(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                C_Req_Corporate reqCorporate = db.C_Req_Corporates.Find(id);
                if (reqCorporate.STATUS == 0)  //reject only from request status
                {
                    //Status => 2 for reject
                    reqCorporate.STATUS = 2;
                    reqCorporate.CHECKER = User.Identity.GetUserId();
                    reqCorporate.CheckedDate = DateTime.Now;
                    //checker reason
                    reqCorporate.CheckedReason = remark;
                    db.Entry(reqCorporate).State = EntityState.Modified;
                    db.SaveChanges();

                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Reject successfully.", $"Request id = {id}, CIFID = {reqCorporate.CIFID}");

                    return Json("Reject Successfully", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json("Reject Fail!", JsonRequestBehavior.AllowGet);
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
