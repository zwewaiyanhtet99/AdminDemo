using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Configuration;
using System.Globalization;

namespace ABankAdmin.Controllers
{
    public class C_ApproveRuleController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "C_ApproveRuleController";
        EventLogController log = new EventLogController();
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        #region C_ApproveRule List
        // GET: ApproveRule
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Getddl();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Approve Rule
        [HttpPost]
        public ActionResult Index(int? args)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
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
                var companyname = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var approver = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var trantype = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                var fromdate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                var todate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
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

                //filter IsDelete flag,     for Corporate   => ID > 0 and vrCorporate not null,     for either approver or position     => Details > 0
                var aprrules = db.C_ApproverTranRules.Where(ar => ar.IsDelete == false & ar.CorporateId > 0 && ar.Vrcorporate != null && ar.Vrcorporate.DEL_FLAG == false /*&& ar.Details.Count > 0*/).ToList();

                var apvrule = aprrules.ConvertAll(apr => new ApvTranRuleAndDetailVM
                {
                    ID = apr.Id,
                    COMPANY_NAME = apr.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = apr.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(apr.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(apr.ToAmount),
                    CURRENCY = apr.Currency,
                    NO_OF_APPROVERS = apr.TotalApproverCount,
                    APPROVERS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == true && p.UserId != null)
                                .Join(db._Users,
                                aprd => aprd.UserId,
                                _user => _user.USERID,
                                (aprd, _user) => new
                                {
                                    UserName = _user.USERNAME
                                })
                                .Select(d => d.UserName).ToList()
                                .Where(s => !string.IsNullOrEmpty(s))),

                    POSITIONS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == false && p.Vrposition != null)
                                .Select(d => d.Vrposition.NAME).ToList()
                                .Where(s => !string.IsNullOrEmpty(s)))
                }).AsEnumerable();

                if (!string.IsNullOrEmpty(companyname))
                {
                    apvrule = apvrule.Where(a => a.COMPANY_NAME.ToLower().Trim().Contains(companyname.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(approver))
                {
                    apvrule = apvrule.Where(a => a.APPROVERS.ToLower().Trim().Contains(approver.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(trantype))
                {
                    apvrule = apvrule.Where(a => a.IsForBulkPayment.ToString() == trantype).ToList();
                }
                //if (fromDate != null & toDate != null)
                //{
                //    apvrule = apvrule.Where(a => a.CreatedDateTime >= fromDate && a.CreatedDateTime < toDate).ToList();
                //}
                //else if (fromDate != null)
                //{
                //    apvrule = apvrule.Where(a => a.CreatedDateTime >= fromDate).ToList();
                //}
                //else if (toDate != null)
                //{
                //    apvrule = apvrule.Where(a => a.CreatedDateTime < toDate).ToList();
                //}

                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    apvrule = apvrule.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //var search = Request.Form.GetValues("search[value]").FirstOrDefault();
                //search = search.ToLower().Trim();
                //if (!string.IsNullOrEmpty(search))
                //{
                //    apvrule = apvrule.Where(a => a.COMPANY_NAME.ToLower().Contains(search)
                //                || a.FROM_AMT.ToString().Contains(search)
                //                || a.TO_AMT.ToString().Contains(search)
                //                || a.CURRENCY.ToLower().Contains(search)
                //                || a.NO_OF_APPROVERS.ToString().ToLower().Contains(search)
                //                || a.APPROVERS.ToLower().Contains(search)
                //                || a.POSITIONS.ToLower().Contains(search)
                //                //Replace bool true to string "bulk" fale to string "normal"
                //                || a.IsForBulkPayment.ToString().Replace(a.IsForBulkPayment.ToString(), a.IsForBulkPayment == true ? "bulk" : "normal").Contains(search)
                //    ).AsEnumerable();
                //}

                recordsTotal = apvrule.Count();
                var data = apvrule.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region Detail
        //GET: ApproveRule/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Find(id);
                List<C_ApproverTranRuleDetail> c_appruledetail = db.C_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_Approve_Rule.Id).ToList();
                if (c_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                ApvTranRuleAndDetailVM tranrulevm = new ApvTranRuleAndDetailVM();
                tranrulevm.ID = c_Approve_Rule.Id;
                tranrulevm.CorporateId = c_Approve_Rule.CorporateId;
                tranrulevm.COMPANY_NAME = c_Approve_Rule.Vrcorporate == null ? "" : c_Approve_Rule.Vrcorporate.COMPANY_NAME;
                tranrulevm.IsForBulkPayment = c_Approve_Rule.IsForBulkPayment;
                tranrulevm.FROM_AMT = General.ConvertDecimalToStr(c_Approve_Rule.FromAmount);
                tranrulevm.TO_AMT = General.ConvertDecimalToStr(c_Approve_Rule.ToAmount);
                tranrulevm.CURRENCY = c_Approve_Rule.Currency;
                tranrulevm.NO_OF_APPROVERS = c_Approve_Rule.TotalApproverCount;
                tranrulevm.APPROVERS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                tranrulevm.POSITIONS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));
                return View(tranrulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Create,Edit,Delete

        #region create
        // GET: ApproveRule/Create
        //public ActionResult Create()
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        //ViewBag.CORPORATE_ID = new SelectList(db.C_Corporates, "ID", "CIFID");
        //        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code");
        //        ApvTranRuleAndDetailVM apvtrananddetailvm = new ApvTranRuleAndDetailVM();
        //        //apvtrananddetailvm.apvtranrules = new List<C_ApproverTranRule>();
        //        return View(apvtrananddetailvm);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        ////POST: ApproveRule/Create
        ////To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        ////more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ApvTranRuleAndDetailVM aprruleanddetailvm)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            #region validation
        //            //validation
        //            //get approver count
        //            //int aprCount = 0;
        //            //if (!string.IsNullOrWhiteSpace(aprruleanddetailvm.APPROVERSID))
        //            //{
        //            //    aprCount = aprruleanddetailvm.APPROVERSID.Split(',').Length;
        //            //}
        //            //int positionAprCount = 0;
        //            //if (!String.IsNullOrWhiteSpace(aprruleanddetailvm.POSITIONSID))
        //            //{
        //            //    int posID = 0;
        //            //    foreach (var positionID in aprruleanddetailvm.POSITIONSID.Split(','))
        //            //    {
        //            //        if (Int32.TryParse(positionID, out posID))
        //            //        {
        //            //            positionAprCount += db._Users
        //            //           .Join(db.C_UserInRoles, u => u.USERID, ur => ur.UserId,
        //            //           ((user, userInRole) => new { user = user, userInRole = userInRole }))
        //            //           .Where(w => w.user.PositionId == (int?)posID && w.user.CORPORATEID == (int?)aprruleanddetailvm.CorporateId &&  w.userInRole.VrCRole.Name == "Approver" )
        //            //           .Count();
        //            //        }
        //            //    }
        //            //}
        //            //if (aprruleanddetailvm.NO_OF_APPROVERS > (aprCount + positionAprCount))
        //            if (!CheckApproverCount(aprruleanddetailvm.APPROVERSID, aprruleanddetailvm.POSITIONSID, aprruleanddetailvm.CorporateId, aprruleanddetailvm.NO_OF_APPROVERS))
        //            {
        //                ModelState.AddModelError("APPROVERS", "Please choose more approvers or postions!");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }

        //            #region amount overlap validation
        //            //check from_amt,to_amt overlap
        //            decimal from_amt = 0;
        //            decimal to_amt = 0;
        //            //convert from amount limit
        //            try
        //            {
        //                from_amt = General.ConvertStrToDecimal(aprruleanddetailvm.FROM_AMT);
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("FROM_AMT", "The field Allow From Amount must be between 0 and 79,228,162,514,264,337,593,543,950,335.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            //check to amount limit
        //            try
        //            {
        //                to_amt = General.ConvertStrToDecimal(aprruleanddetailvm.TO_AMT);
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("TO_AMT", "The field Allow To Amount must be between 0 and 79,228,162,514,264,337,593,543,950,335.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            if (from_amt >= to_amt)
        //            {
        //                ModelState.AddModelError("TO_AMT", "To Amount must be greater than From Amount.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            //to check inner gap
        //            if (db.C_ApproverTranRules.Where(a => a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment).Count() > 0)
        //            {
        //                ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            //to check From
        //            if (db.C_ApproverTranRules.Where(a => a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment).Count() > 0)
        //            {
        //                ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            //to check To
        //            if (db.C_ApproverTranRules.Where(a => a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment).Count() > 0)
        //            {
        //                ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            //to validate To_Amt is not greater than company Limit
        //            C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.CorporateId == aprruleanddetailvm.CorporateId).FirstOrDefault();
        //            decimal cortranvalue = Convert.ToDecimal(cortranrule.Value);
        //            if (to_amt > cortranvalue)
        //            {
        //                ModelState.AddModelError("TO_AMT", "TO_AMOUNT Can\'t Exceed The Company Amount Limit.");
        //                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                return View(aprruleanddetailvm);
        //            }
        //            #endregion

        //            #endregion

        //            using (var transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    string LoginUser = User.Identity.GetUserId();

        //                    //save rule
        //                    C_ApproverTranRule c_Approve_Rule = new C_ApproverTranRule();
        //                    c_Approve_Rule.CorporateId = aprruleanddetailvm.CorporateId;
        //                    c_Approve_Rule.IsForBulkPayment = aprruleanddetailvm.IsForBulkPayment;
        //                    c_Approve_Rule.FromAmount = from_amt;
        //                    c_Approve_Rule.ToAmount = to_amt;
        //                    c_Approve_Rule.TotalApproverCount = aprruleanddetailvm.NO_OF_APPROVERS;
        //                    c_Approve_Rule.Currency = aprruleanddetailvm.CURRENCY;
        //                    c_Approve_Rule.CreatedDateTime = DateTime.Now;
        //                    c_Approve_Rule.CreatedUserID = User.Identity.GetUserId();
        //                    if (TryValidateModel(c_Approve_Rule))
        //                    {
        //                        //default fields
        //                        //c_Approve_Rule.createduserid = 1;
        //                        db.C_ApproverTranRules.Add(c_Approve_Rule);
        //                        db.SaveChanges();
        //                        //save log
        //                        log.Info(LoginUser, controllerName, actionName, 1, $"Corporate ID {c_Approve_Rule.CorporateId} is Saved.");

        //                        //save detail
        //                        if (aprruleanddetailvm.NO_OF_APPROVERS > 0)
        //                        {
        //                            if (!String.IsNullOrWhiteSpace(aprruleanddetailvm.APPROVERSID))
        //                            //if (aprruleanddetailvm.APPROVERSID != 0)
        //                            {
        //                                //save approvers
        //                                string[] userids = aprruleanddetailvm.APPROVERSID.Split(',');
        //                                foreach (var userid in userids)
        //                                {
        //                                    C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
        //                                    newaprdetail.ApproverTranRuleId = c_Approve_Rule.Id;
        //                                    newaprdetail.IsUser = true;
        //                                    newaprdetail.UserId = userid;
        //                                    db.C_ApproverTranRuleDetails.Add(newaprdetail);
        //                                    db.SaveChanges();
        //                                }
        //                                //save log
        //                                log.Info(LoginUser, controllerName, actionName, 1, $"Approver IDs = {aprruleanddetailvm.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
        //                            }

        //                            if (!String.IsNullOrWhiteSpace(aprruleanddetailvm.POSITIONSID))
        //                            {
        //                                //save position
        //                                string[] positionIDs = aprruleanddetailvm.POSITIONSID.Split(',');
        //                                foreach (var positionID in positionIDs)
        //                                {
        //                                    C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
        //                                    newaprdetail.ApproverTranRuleId = c_Approve_Rule.Id;
        //                                    newaprdetail.IsUser = false;
        //                                    newaprdetail.PositionId = Convert.ToInt32(positionID);
        //                                    db.C_ApproverTranRuleDetails.Add(newaprdetail);
        //                                    db.SaveChanges();
        //                                }
        //                                //save log
        //                                log.Info(LoginUser, controllerName, actionName, 1, $"Position IDs= {aprruleanddetailvm.POSITIONSID}. Position Saved Count = {positionIDs.Count()}.");
        //                            }
        //                        }

        //                        // everything is fine - save all the data permanently
        //                        transaction.Commit();
        //                        ViewBag.Message = "\"" + aprruleanddetailvm.COMPANY_NAME + "\"";
        //                    }
        //                    else
        //                    {
        //                        transaction.Rollback();
        //                        ModelState.AddModelError("TO_AMT", "To Amount must be greater than FromAmount.");
        //                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //                        return View(aprruleanddetailvm);
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    // roll back all database operations, if anything goes wrong
        //                    transaction.Rollback();
        //                    //ViewBag.ResultMessage = "Error occured, records rolledback.";
        //                }
        //            }

        //            //  return RedirectToAction("Index");
        //        }

        //        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
        //        return View(aprruleanddetailvm);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        #endregion
        //GET: ApproveRule/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Find(id);
                C_ApproverTranRuleDetail c_appruledetail = db.C_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_Approve_Rule.Id & a.IsDelete == false).FirstOrDefault();
                ReqApvTranRuleAndDetailVM apvRule = new ReqApvTranRuleAndDetailVM
                {
                    Req_TranRuleId = c_Approve_Rule.Id,
                    CorporateId = c_Approve_Rule.CorporateId,
                    COMPANY_NAME = c_Approve_Rule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = c_Approve_Rule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(c_Approve_Rule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(c_Approve_Rule.ToAmount),
                    CURRENCY = c_Approve_Rule.Currency,
                    NO_OF_APPROVERS = c_Approve_Rule.TotalApproverCount,
                    APPROVERS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    POSITIONS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Select(d => d.UserId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.PositionId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString())))
                };

                if (c_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.CORPORATE_ID = new SelectList(db.C_Corporates, "ID", "CIFID", c_Approve_Rule.CorporateId);
                ViewBag.NoOfApprover = General.GetNoOfApprover();
                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", apvRule.CURRENCY);
                return View(apvRule);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //POST: ApproveRule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReqApvTranRuleAndDetailVM aprruleanddetailvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string getuserid = User.Identity.GetUserId();
                    if (ModelState.IsValid)
                    {
                        #region validation

                        //get corporate by id
                        C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Where(c => c.Id == aprruleanddetailvm.Req_TranRuleId && c.IsDelete == false).FirstOrDefault();
                        if (c_Approve_Rule == null)
                        {
                            ModelState.AddModelError("", "Request Fail!");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                        }

                        //check already requested and check delete request
                        int existcount = db.C_Req_ApproverTranRules.Where(u => u.Req_TranRuleId == aprruleanddetailvm.Req_TranRuleId && (u.TYPE == 2 || u.TYPE == 1) && u.STATUS == 0 && u.IsDelete == false && u.Vrcorporate.DEL_FLAG == false).Count();
                        if (existcount > 0)
                        {
                            ModelState.AddModelError("", "Already requested!");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                        }

                        //if (aprruleanddetailvm.NO_OF_APPROVERS > (aprCount + positionAprCount))
                        if (!CheckApproverCount(aprruleanddetailvm.APPROVERSID, aprruleanddetailvm.POSITIONSID, aprruleanddetailvm.CorporateId, aprruleanddetailvm.NO_OF_APPROVERS))
                        {
                            ModelState.AddModelError("APPROVERS", "Please choose more approvers or postions!");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }


                        #region amount overlap validation
                        //check from_amt,to_amt overlap
                        decimal from_amt = 0;
                        decimal to_amt = 0;
                        //convert from amount limit
                        try
                        {
                            from_amt = General.ConvertStrToDecimal(aprruleanddetailvm.FROM_AMT);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("FROM_AMT", "The field Allow From Amount must be between 0 and 9999999999999999.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //check to amount limit
                        try
                        {
                            to_amt = General.ConvertStrToDecimal(aprruleanddetailvm.TO_AMT);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("TO_AMT", "The field Allow To Amount must be between 0 and 9999999999999999.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        if (from_amt >= to_amt)
                        {
                            ModelState.AddModelError("TO_AMT", "To Amount must be greater than From Amount.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to check inner gap
                        if (db.C_ApproverTranRules.Where(a => a.Id != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG ==false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to check From
                        if (db.C_ApproverTranRules.Where(a => a.Id != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to check To
                        if (db.C_ApproverTranRules.Where(a => a.Id != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }

                        if (db.C_Req_ApproverTranRules.Where(a => a.Req_TranRuleId != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to check From
                        if (db.C_Req_ApproverTranRules.Where(a => a.Req_TranRuleId != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to check To
                        if (db.C_Req_ApproverTranRules.Where(a => a.Req_TranRuleId != aprruleanddetailvm.ID && a.CorporateId == aprruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == aprruleanddetailvm.CURRENCY & a.IsForBulkPayment == aprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        //to validate To_Amt is not greater than company Limit
                        C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.CorporateId == aprruleanddetailvm.CorporateId && c.IsDelete == false).FirstOrDefault();
                        decimal cortranvalue = Convert.ToDecimal(cortranrule.Value);
                        if (to_amt > cortranvalue)
                        {
                            ModelState.AddModelError("TO_AMT", "TO_AMOUNT Can\'t Exceed The Company Amount Limit.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                            return View(aprruleanddetailvm);
                        }
                        #endregion

                        #endregion
                        //Delete Old TranRuleDetails
                        List<C_Req_ApproverTranRuleDetail> c_req_appruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_Approve_Rule.Id && a.IsDelete == false).ToList();
                        db.C_Req_ApproverTranRuleDetails.RemoveRange(c_req_appruledetail);
                        db.SaveChanges();
                        //C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Where(a => a.Id == aprruleanddetailvm.ID).FirstOrDefault();
                        if (c_Approve_Rule == null)
                        {
                            return HttpNotFound();
                        }

                        C_Req_ApproverTranRule c_req_Approve_Rule = new C_Req_ApproverTranRule();
                        c_req_Approve_Rule.Req_TranRuleId = aprruleanddetailvm.Req_TranRuleId;
                        c_req_Approve_Rule.CorporateId = aprruleanddetailvm.CorporateId;
                        c_req_Approve_Rule.IsForBulkPayment = aprruleanddetailvm.IsForBulkPayment;
                        c_req_Approve_Rule.FromAmount = from_amt;
                        c_req_Approve_Rule.ToAmount = to_amt;
                        c_req_Approve_Rule.TotalApproverCount = aprruleanddetailvm.NO_OF_APPROVERS;
                        c_req_Approve_Rule.Currency = aprruleanddetailvm.CURRENCY;
                        c_req_Approve_Rule.TYPE = 1;//0 for register, 1 for update, 2 for delete
                        c_req_Approve_Rule.STATUS = 0;//status => 0   request status, 1  approve, 2 reject
                        c_req_Approve_Rule.IsDelete = false;
                        c_req_Approve_Rule.MAKER = User.Identity.GetUserId();
                        c_req_Approve_Rule.REQUESTEDDATE = DateTime.Now;
                        //get BranchID from Logged In User
                        AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                        c_req_Approve_Rule.BranchID = admin.BranchID;

                        if (TryValidateModel(c_req_Approve_Rule))
                        {
                            //default fields                           
                            db.C_Req_ApproverTranRules.Add(c_req_Approve_Rule);
                            db.SaveChanges();
                            log.Info(getuserid, controllerName, actionName, 2, $"ApproveRule ID is {c_Approve_Rule.Id}.Company Name {c_Approve_Rule.Vrcorporate.COMPANY_NAME} is Saved.");

                            //save detail
                            if (aprruleanddetailvm.NO_OF_APPROVERS > 0)
                            {
                                if (!String.IsNullOrWhiteSpace(aprruleanddetailvm.APPROVERSID))
                                {
                                    //save approvers
                                    string[] userids = aprruleanddetailvm.APPROVERSID.Split(',');
                                    foreach (var userid in userids)
                                    {
                                        C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                                        newreqaprdetail.ApproverTranRuleId = c_req_Approve_Rule.Id;
                                        newreqaprdetail.IsUser = true;
                                        newreqaprdetail.UserId = userid;
                                        db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                                        db.SaveChanges();
                                    }
                                    log.Info(getuserid, controllerName, actionName, 1, $"ApproveRule ID is {c_req_Approve_Rule.Id}. Approver IDs = {aprruleanddetailvm.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                                }

                                if (!String.IsNullOrWhiteSpace(aprruleanddetailvm.POSITIONSID))
                                {
                                    //save position
                                    string[] positionIDs = aprruleanddetailvm.POSITIONSID.Split(',');
                                    foreach (var positionID in positionIDs)
                                    {
                                        C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                                        newreqaprdetail.ApproverTranRuleId = c_req_Approve_Rule.Id;
                                        newreqaprdetail.IsUser = false;
                                        newreqaprdetail.PositionId = Convert.ToInt32(positionID);
                                        db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                                        db.SaveChanges();
                                    }
                                    log.Info(getuserid, controllerName, actionName, 1, $"ApproveRule ID is {c_req_Approve_Rule.Id}. POSITIONS IDs = {aprruleanddetailvm.POSITIONSID}.Position Saved Count = {positionIDs.Count()}.");
                                }
                            }

                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            ViewBag.Message = "\"" + aprruleanddetailvm.COMPANY_NAME + "\"";
                        }
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                        return View(aprruleanddetailvm);
                    }
                    else
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("TO_AMT", "To Amount must be greater than FromAmount.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", aprruleanddetailvm.CURRENCY);
                        return View(aprruleanddetailvm);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                    return View("Error");
                }
            }
        }

        // GET: ApproveRule/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Find(id);
                List<C_ApproverTranRuleDetail> c_appruledetail = db.C_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_Approve_Rule.Id).ToList();

                if (c_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                ReqApvTranRuleAndDetailVM apvRule = new ReqApvTranRuleAndDetailVM
                {
                    ID = c_Approve_Rule.Id,
                    CorporateId = c_Approve_Rule.CorporateId,
                    COMPANY_NAME = c_Approve_Rule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = c_Approve_Rule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(c_Approve_Rule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(c_Approve_Rule.ToAmount),
                    CURRENCY = c_Approve_Rule.Currency,
                    NO_OF_APPROVERS = c_Approve_Rule.TotalApproverCount,
                    APPROVERS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    POSITIONS = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Select(d => d.UserId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", c_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.PositionId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString())))
                };
                return View(apvRule);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ApproveRule/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(ReqApvTranRuleAndDetailVM tranrulevm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = User.Identity.GetUserId();

            try
            {
                //get corporate by id
                C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Where(c => c.Id == tranrulevm.ID).FirstOrDefault();
                if (c_Approve_Rule == null)
                {
                    return Json("Request Fail!", JsonRequestBehavior.AllowGet);
                }

                //check already requested and check delete request
                int existcount = db.C_Req_ApproverTranRules.Where(u => u.Req_TranRuleId == tranrulevm.ID && (u.TYPE == 2 || u.TYPE == 1) && u.STATUS == 0).Count();
                if (existcount > 0)
                {
                    return Json("Already requested!", JsonRequestBehavior.AllowGet);
                }

                tranrulevm = new ReqApvTranRuleAndDetailVM
                {
                    Req_TranRuleId = c_Approve_Rule.Id,
                    CorporateId = c_Approve_Rule.CorporateId,
                    COMPANY_NAME = c_Approve_Rule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = c_Approve_Rule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(c_Approve_Rule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(c_Approve_Rule.ToAmount),
                    CURRENCY = c_Approve_Rule.Currency,
                    NO_OF_APPROVERS = c_Approve_Rule.TotalApproverCount,
                    APPROVERS = string.Join(",", c_Approve_Rule.Details
                       .Where(p => p.IsUser == true && p.UserId != null)
                       .Join(db._Users,
                       aprd => aprd.UserId,
                       _user => _user.USERID,
                       (aprd, _user) => new
                       {
                           UserName = _user.USERNAME
                       })
                       .Select(d => d.UserName).ToList()
                       .Where(s => !string.IsNullOrEmpty(s))),
                    POSITIONS = string.Join(",", c_Approve_Rule.Details
                       .Where(p => p.IsUser == false && p.Vrposition != null)
                       .Select(d => d.Vrposition.NAME).ToList()
                       .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", c_Approve_Rule.Details
                       .Where(p => p.IsUser == true && p.UserId != null)
                       .Select(d => d.UserId).ToList()
                       .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", c_Approve_Rule.Details
                       .Where(p => p.IsUser == false && p.Vrposition != null)
                       .Select(d => d.PositionId).ToList()
                       .Where(s => !string.IsNullOrEmpty(s.ToString())))
                };

                Boolean isSuccess = AddReqDelete(tranrulevm);//For Delete Request
                if (isSuccess)
                {
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, "Requesting delete successfully.");
                    return Json("Requesting delete successfully.", JsonRequestBehavior.AllowGet);
                    // return RedirectToAction("Index");
                }
                return Json("Request Fail!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

        }

        private Boolean AddReqDelete(ReqApvTranRuleAndDetailVM tranrules)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string LoginUser = User.Identity.GetUserId();
            //Add req to ReqChanges table
            try
            {
                C_ApproverTranRule c_Approve_Rule = db.C_ApproverTranRules.Where(c => c.Id == tranrules.Req_TranRuleId).FirstOrDefault();
                C_Req_ApproverTranRule c_Req_Approve_Rule = new C_Req_ApproverTranRule();
                C_Req_ApproverTranRuleDetail reqappTranRuleDetail = new C_Req_ApproverTranRuleDetail();


                ReqApvTranRuleAndDetailVM apvRule = new ReqApvTranRuleAndDetailVM
                {
                    Req_TranRuleId = c_Approve_Rule.Id,
                    CorporateId = c_Approve_Rule.CorporateId,
                    COMPANY_NAME = c_Approve_Rule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = c_Approve_Rule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(c_Approve_Rule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(c_Approve_Rule.ToAmount),
                    CURRENCY = c_Approve_Rule.Currency,
                    NO_OF_APPROVERS = c_Approve_Rule.TotalApproverCount,
                    APPROVERS = string.Join(",", c_Approve_Rule.Details
                          .Where(p => p.IsUser == true && p.UserId != null)
                          .Join(db._Users,
                          aprd => aprd.UserId,
                          _user => _user.USERID,
                          (aprd, _user) => new
                          {
                              UserName = _user.USERNAME
                          })
                          .Select(d => d.UserName).ToList()
                          .Where(s => !string.IsNullOrEmpty(s))),
                    POSITIONS = string.Join(",", c_Approve_Rule.Details
                          .Where(p => p.IsUser == false && p.Vrposition != null)
                          .Select(d => d.Vrposition.NAME).ToList()
                          .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", c_Approve_Rule.Details
                          .Where(p => p.IsUser == true && p.UserId != null)
                          .Select(d => d.UserId).ToList()
                          .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", c_Approve_Rule.Details
                          .Where(p => p.IsUser == false && p.Vrposition != null)
                          .Select(d => d.PositionId).ToList()
                          .Where(s => !string.IsNullOrEmpty(s.ToString())))
                };

                //save into requested approve tran rule table (temp)
                //save rule
                c_Req_Approve_Rule.Req_TranRuleId = c_Approve_Rule.Id;
                c_Req_Approve_Rule.CorporateId = c_Approve_Rule.CorporateId;
                c_Req_Approve_Rule.IsForBulkPayment = c_Approve_Rule.IsForBulkPayment;
                c_Req_Approve_Rule.FromAmount = c_Approve_Rule.FromAmount;
                c_Req_Approve_Rule.ToAmount = c_Approve_Rule.ToAmount;
                c_Req_Approve_Rule.TotalApproverCount = c_Approve_Rule.TotalApproverCount;
                c_Req_Approve_Rule.Currency = c_Approve_Rule.Currency;
                c_Req_Approve_Rule.TYPE = 2;//0 for register, 1 for update, 2 for delete
                c_Req_Approve_Rule.STATUS = 0;//status => 0   request status, 1  approve, 2 reject
                c_Req_Approve_Rule.IsDelete = false;
                c_Req_Approve_Rule.MAKER = LoginUser;
                c_Req_Approve_Rule.REQUESTEDDATE = DateTime.Now;
                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                c_Req_Approve_Rule.BranchID = admin.BranchID;
                db.C_Req_ApproverTranRules.Add(c_Req_Approve_Rule);
                db.SaveChanges();
                //save log
                log.Info(LoginUser, controllerName, actionName, 1, $"Corporate ID {c_Req_Approve_Rule.CorporateId} is Saved.");

                //save detail
                if (c_Approve_Rule.TotalApproverCount > 0)
                {
                    if (!String.IsNullOrWhiteSpace(apvRule.APPROVERSID))
                    //if (aprruleanddetailvm.APPROVERSID != 0)
                    {
                        //save approvers
                        string[] userids = apvRule.APPROVERSID.Split(',');
                        foreach (var userid in userids)
                        {
                            C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                            newreqaprdetail.ApproverTranRuleId = c_Req_Approve_Rule.Id;
                            newreqaprdetail.IsUser = true;
                            newreqaprdetail.UserId = userid;
                            db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 1, $"Approver IDs = {apvRule.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                    }

                    if (!String.IsNullOrWhiteSpace(apvRule.POSITIONSID))
                    {
                        //save position
                        string[] positionIDs = apvRule.POSITIONSID.Split(',');
                        foreach (var positionID in positionIDs)
                        {
                            C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                            newreqaprdetail.ApproverTranRuleId = c_Req_Approve_Rule.Id;
                            newreqaprdetail.IsUser = false;
                            newreqaprdetail.PositionId = Convert.ToInt32(positionID);
                            db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 1, $"Position IDs= {apvRule.POSITIONSID}. Position Saved Count = {positionIDs.Count()}.");
                    }
                }
                ViewBag.Message = "\"" + apvRule.COMPANY_NAME + "\"";
                return true;
            }
            catch (Exception ex)
            {
                log.Error(LoginUser, controllerName, actionName, ex);
            }
            return false;
        }

        #endregion

        #region Approve Rule Request

        #region approve rule request
        //Get:C_ApproveRule/Register
        public ActionResult Register()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ReqApvTranRuleAndDetailVM reqapvtrananddetailvm = new ReqApvTranRuleAndDetailVM();
                ViewBag.NoOfApprover = General.GetNoOfApprover();
                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code");
                return View(reqapvtrananddetailvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: C_ApproveRule/Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ReqApvTranRuleAndDetailVM reqapvtranruleanddetailvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region validation

                    if (!CheckApproverCount(reqapvtranruleanddetailvm.APPROVERSID, reqapvtranruleanddetailvm.POSITIONSID, reqapvtranruleanddetailvm.CorporateId, reqapvtranruleanddetailvm.NO_OF_APPROVERS))
                    {
                        ModelState.AddModelError("APPROVERS", "Please choose more approvers or postions!");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }

                    #region amount overlap validation
                    //check from_amt,to_amt overlap
                    decimal from_amt = 0;
                    decimal to_amt = 0;
                    //convert from amount limit
                    try
                    {
                        from_amt = General.ConvertStrToDecimal(reqapvtranruleanddetailvm.FROM_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("FROM_AMT", "The field Allow From Amount must be between 0 and 9,999,999,999,999,999.99");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    //check to amount limit
                    try
                    {
                        to_amt = General.ConvertStrToDecimal(reqapvtranruleanddetailvm.TO_AMT);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("TO_AMT", "The field Allow To Amount must be between 0 and 9,999,999,999,999,999.99");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    if (from_amt >= to_amt)
                    {
                        ModelState.AddModelError("TO_AMT", "To Amount must be greater than From Amount.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    //to check inner gap
                    if (db.C_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG ==false).Count() > 0)
                    {
                        ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    //to check From
                    //if (db.C_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                    //{
                    //    ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                    //    ViewBag.NoOfApprover = General.GetNoOfApprover();
                    //    ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                    //    return View(reqapvtranruleanddetailvm);
                    //}
                    //to check To
                    if (db.C_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }

                    //to check inner gap
                    if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    //to check From
                    if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    //to check To
                    if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqapvtranruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == reqapvtranruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqapvtranruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }

                    //to validate To_Amt is not greater than company Limit
                    C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.CorporateId == reqapvtranruleanddetailvm.CorporateId & c.IsDelete == false).FirstOrDefault();
                    decimal cortranvalue = Convert.ToDecimal(cortranrule.Value);
                    if (to_amt > cortranvalue)
                    {
                        ModelState.AddModelError("TO_AMT", "TO_AMOUNT Can\'t Exceed The Company Amount Limit.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                        return View(reqapvtranruleanddetailvm);
                    }
                    #endregion

                    #endregion

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            string LoginUser = User.Identity.GetUserId();

                            //save rule
                            C_Req_ApproverTranRule c_Req_Approve_Rule = new C_Req_ApproverTranRule();
                            c_Req_Approve_Rule.CorporateId = reqapvtranruleanddetailvm.CorporateId;
                            c_Req_Approve_Rule.IsForBulkPayment = reqapvtranruleanddetailvm.IsForBulkPayment;
                            c_Req_Approve_Rule.FromAmount = from_amt;
                            c_Req_Approve_Rule.ToAmount = to_amt;
                            c_Req_Approve_Rule.TotalApproverCount = reqapvtranruleanddetailvm.NO_OF_APPROVERS;
                            c_Req_Approve_Rule.Currency = reqapvtranruleanddetailvm.CURRENCY;
                            c_Req_Approve_Rule.TYPE = 0;
                            c_Req_Approve_Rule.STATUS = 0;
                            c_Req_Approve_Rule.IsDelete = false;
                            c_Req_Approve_Rule.MAKER = LoginUser;
                            c_Req_Approve_Rule.REQUESTEDDATE = DateTime.Now;
                            //get BranchID from Logged In User
                            AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                            c_Req_Approve_Rule.BranchID = admin.BranchID;

                            if (TryValidateModel(c_Req_Approve_Rule))
                            {
                                //default fields
                                //c_Req_Approve_Rule.createduserid = 1;
                                db.C_Req_ApproverTranRules.Add(c_Req_Approve_Rule);
                                db.SaveChanges();
                                //save log
                                log.Info(LoginUser, controllerName, actionName, 1, $"Corporate ID {c_Req_Approve_Rule.CorporateId} is Saved.");

                                //save detail
                                if (reqapvtranruleanddetailvm.NO_OF_APPROVERS > 0)
                                {
                                    if (!String.IsNullOrWhiteSpace(reqapvtranruleanddetailvm.APPROVERSID))
                                    //if (aprruleanddetailvm.APPROVERSID != 0)
                                    {
                                        //save approvers
                                        string[] userids = reqapvtranruleanddetailvm.APPROVERSID.Split(',');
                                        foreach (var userid in userids)
                                        {
                                            C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                                            newreqaprdetail.ApproverTranRuleId = c_Req_Approve_Rule.Id;
                                            newreqaprdetail.IsUser = true;
                                            newreqaprdetail.UserId = userid;
                                            db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                                            db.SaveChanges();
                                        }
                                        //save log
                                        log.Info(LoginUser, controllerName, actionName, 1, $"Approver IDs = {reqapvtranruleanddetailvm.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                                    }

                                    if (!String.IsNullOrWhiteSpace(reqapvtranruleanddetailvm.POSITIONSID))
                                    {
                                        //save position
                                        string[] positionIDs = reqapvtranruleanddetailvm.POSITIONSID.Split(',');
                                        foreach (var positionID in positionIDs)
                                        {
                                            C_Req_ApproverTranRuleDetail newreqaprdetail = new C_Req_ApproverTranRuleDetail();
                                            newreqaprdetail.ApproverTranRuleId = c_Req_Approve_Rule.Id;
                                            newreqaprdetail.IsUser = false;
                                            newreqaprdetail.PositionId = Convert.ToInt32(positionID);
                                            db.C_Req_ApproverTranRuleDetails.Add(newreqaprdetail);
                                            db.SaveChanges();
                                        }
                                        //save log
                                        log.Info(LoginUser, controllerName, actionName, 1, $"Position IDs= {reqapvtranruleanddetailvm.POSITIONSID}. Position Saved Count = {positionIDs.Count()}.");
                                    }
                                }

                                // everything is fine - save all the data permanently
                                transaction.Commit();
                                ViewBag.Message = "\"" + reqapvtranruleanddetailvm.COMPANY_NAME + "\"";
                            }
                            else
                            {
                                transaction.Rollback();
                                ModelState.AddModelError("TO_AMT", "To Amount must be greater than FromAmount.");
                                ViewBag.NoOfApprover = General.GetNoOfApprover();
                                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                                return View(reqapvtranruleanddetailvm);
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                ViewBag.NoOfApprover = General.GetNoOfApprover();
                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvtranruleanddetailvm.CURRENCY);
                return View(reqapvtranruleanddetailvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region requested approve rule list
        //Get:C_ApproveRule/ReqApproveRuleList
        public ActionResult ReqApproveRuleList()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Getddl();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Request Approve Rule
        [HttpPost]
        public ActionResult ReqApproveRuleList(int? args)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
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
                var companyname = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var approver = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var type = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                var trantype = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                var fromdate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
                var todate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
                var status = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
                DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
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

                //get requested list of logged in admin
                string userid = User.Identity.GetUserId();

                //filter IsDelete flag,     for Corporate   => ID > 0 and vrCorporate not null,     for either approver or position     => Details > 0
                var reqaprrules = db.C_Req_ApproverTranRules.Where(ar => ar.MAKER == userid && ar.IsDelete == false & ar.CorporateId > 0 && ar.Vrcorporate != null && ar.Vrcorporate.DEL_FLAG == false /*&& ar.Details.Count > 0*/).ToList();

                var reqapvrule = reqaprrules.ConvertAll(apr => new ReqApvTranRuleAndDetailVM
                {
                    ID = apr.Id,
                    COMPANY_NAME = apr.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = apr.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(apr.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(apr.ToAmount),
                    CURRENCY = apr.Currency,
                    NO_OF_APPROVERS = apr.TotalApproverCount,
                    APPROVERS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == true && p.UserId != null)
                                .Join(db._Users,
                                aprd => aprd.UserId,
                                _user => _user.USERID,
                                (aprd, _user) => new
                                {
                                    UserName = _user.USERNAME
                                })
                                .Select(d => d.UserName).ToList()
                                .Where(s => !string.IsNullOrEmpty(s))),

                    POSITIONS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == false && p.Vrposition != null)
                                .Select(d => d.Vrposition.NAME).ToList()
                                .Where(s => !string.IsNullOrEmpty(s))),
                    REQUESTEDDATE = apr.REQUESTEDDATE,
                    TYPE = apr.TYPE,
                    STATUS = apr.STATUS
                }).AsEnumerable();

                //SEARCHING...

                if (!string.IsNullOrEmpty(companyname))
                {
                    reqapvrule = reqapvrule.Where(a => a.COMPANY_NAME.ToLower().Trim().Contains(companyname.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(approver))
                {
                    reqapvrule = reqapvrule.Where(a => a.APPROVERS.ToLower().Trim().Contains(approver.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(trantype))
                {
                    reqapvrule = reqapvrule.Where(a => a.IsForBulkPayment.ToString() == trantype).ToList();
                }
                if (fromDate != null & toDate != null)
                {
                    reqapvrule = reqapvrule.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate).ToList();
                }
                else if (fromDate != null)
                {
                    reqapvrule = reqapvrule.Where(a => a.REQUESTEDDATE >= fromDate).ToList();
                }
                else if (toDate != null)
                {
                    reqapvrule = reqapvrule.Where(a => a.REQUESTEDDATE < toDate).ToList();
                }
                if (!string.IsNullOrEmpty(type))
                {
                    int tran_type = Convert.ToInt32(type);
                    reqapvrule = reqapvrule.Where(a => a.TYPE == tran_type).ToList();
                }
                if (!string.IsNullOrEmpty(status))
                {
                    reqapvrule = reqapvrule.Where(a => a.STATUS.ToString() == status).ToList();
                }

                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    reqapvrule = reqapvrule.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //var search = Request.Form.GetValues("search[value]").FirstOrDefault();
                //search = search.ToLower().Trim();
                //if (!string.IsNullOrEmpty(search))
                //{
                //    reqapvrule = reqapvrule.Where(a => a.COMPANY_NAME.ToLower().Contains(search)
                //                || a.FROM_AMT.ToString().Contains(search)
                //                || a.TO_AMT.ToString().Contains(search)
                //                || a.CURRENCY.ToLower().Contains(search)
                //                || a.NO_OF_APPROVERS.ToString().ToLower().Contains(search)
                //                || a.APPROVERS.ToLower().Contains(search)
                //                || a.POSITIONS.ToLower().Contains(search)
                //                //Replace bool true to string "bulk" fale to string "normal"
                //                || a.IsForBulkPayment.ToString().Replace(a.IsForBulkPayment.ToString(), a.IsForBulkPayment == true ? "bulk" : "normal").Contains(search)
                //    ).AsEnumerable();
                //}

                recordsTotal = reqapvrule.Count();
                var data = reqapvrule.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region details request approve rule

        //Get:Approve Rule/DetailsReqApproveRule
        public ActionResult DetailsReqApproveRule(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Req_ApproverTranRule c_req_Approve_Rule = db.C_Req_ApproverTranRules.Find(id);
                List<C_Req_ApproverTranRuleDetail> c_req_appruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_req_Approve_Rule.Id).ToList();
                if (c_req_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                ReqApvTranRuleAndDetailVM reqtranrulevm = new ReqApvTranRuleAndDetailVM();
                reqtranrulevm.ID = c_req_Approve_Rule.Id;
                reqtranrulevm.CorporateId = c_req_Approve_Rule.CorporateId;
                reqtranrulevm.COMPANY_NAME = c_req_Approve_Rule.Vrcorporate == null ? "" : c_req_Approve_Rule.Vrcorporate.COMPANY_NAME;
                reqtranrulevm.IsForBulkPayment = c_req_Approve_Rule.IsForBulkPayment;
                reqtranrulevm.FROM_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.FromAmount);
                reqtranrulevm.TO_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.ToAmount);
                reqtranrulevm.CURRENCY = c_req_Approve_Rule.Currency;
                reqtranrulevm.STATUS = c_req_Approve_Rule.STATUS;
                reqtranrulevm.NO_OF_APPROVERS = c_req_Approve_Rule.TotalApproverCount;
                reqtranrulevm.APPROVERS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                reqtranrulevm.POSITIONS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));
                return View(reqtranrulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #region edit requested approve rule

        //Get:C_ApproveRule/EditReqApproveRule
        public ActionResult EditReqApproveRule(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Req_ApproverTranRule c_ReqApprove_Rule = db.C_Req_ApproverTranRules.Find(id);
                C_Req_ApproverTranRuleDetail c_reqappruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_ReqApprove_Rule.Id).FirstOrDefault();
                ReqApvTranRuleAndDetailVM reqapvRule = new ReqApvTranRuleAndDetailVM
                {
                    ID = c_ReqApprove_Rule.Id,
                    Req_TranRuleId = c_ReqApprove_Rule.Req_TranRuleId,
                    CorporateId = c_ReqApprove_Rule.CorporateId,
                    COMPANY_NAME = c_ReqApprove_Rule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = c_ReqApprove_Rule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(c_ReqApprove_Rule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(c_ReqApprove_Rule.ToAmount),
                    CURRENCY = c_ReqApprove_Rule.Currency,
                    NO_OF_APPROVERS = c_ReqApprove_Rule.TotalApproverCount,
                    APPROVERS = string.Join(",", c_ReqApprove_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    POSITIONS = string.Join(",", c_ReqApprove_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", c_ReqApprove_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Select(d => d.UserId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", c_ReqApprove_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.PositionId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString())))
                };

                if (c_ReqApprove_Rule == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.CORPORATE_ID = new SelectList(db.C_Corporates, "ID", "CIFID", c_Approve_Rule.CorporateId);
                ViewBag.NoOfApprover = General.GetNoOfApprover();
                ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqapvRule.CURRENCY);
                return View(reqapvRule);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post:C_ApproveRule/EditReqApproveRule
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReqApproveRule(ReqApvTranRuleAndDetailVM reqaprruleanddetailvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string getuserid = User.Identity.GetUserId();
                    if (ModelState.IsValid)
                    {
                        #region validation

                        if (!CheckApproverCount(reqaprruleanddetailvm.APPROVERSID, reqaprruleanddetailvm.POSITIONSID, reqaprruleanddetailvm.CorporateId, reqaprruleanddetailvm.NO_OF_APPROVERS))
                        {
                            ModelState.AddModelError("APPROVERS", "Please choose more approvers or postions!");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }


                        #region amount overlap validation
                        //check from_amt,to_amt overlap
                        decimal from_amt = 0;
                        decimal to_amt = 0;
                        //convert from amount limit
                        try
                        {
                            from_amt = General.ConvertStrToDecimal(reqaprruleanddetailvm.FROM_AMT);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("FROM_AMT", "The field Allow From Amount must be between 0 and 9,999,999,999,999,999.99");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //check to amount limit
                        try
                        {
                            to_amt = General.ConvertStrToDecimal(reqaprruleanddetailvm.TO_AMT);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("TO_AMT", "The field Allow To Amount must be between 0 and 9,999,999,999,999,999.99");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        if (from_amt >= to_amt)
                        {
                            ModelState.AddModelError("TO_AMT", "To Amount must be greater than From Amount.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to check inner gap
                        if (db.C_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.Req_TranRuleId && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to check From
                        if (db.C_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.Req_TranRuleId && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to check To
                        if (db.C_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.Req_TranRuleId && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }

                        if (db.C_Req_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.ID && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount >= from_amt & a.ToAmount <= to_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to check From
                        if (db.C_Req_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.ID && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount <= from_amt & a.ToAmount >= from_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("FROM_AMT", "Duplicate From Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to check To
                        if (db.C_Req_ApproverTranRules.Where(a => a.Id != reqaprruleanddetailvm.ID && a.CorporateId == reqaprruleanddetailvm.CorporateId & a.FromAmount <= to_amt & a.ToAmount >= to_amt & a.IsDelete == false & a.Currency == reqaprruleanddetailvm.CURRENCY & a.IsForBulkPayment == reqaprruleanddetailvm.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                        {
                            ModelState.AddModelError("TO_AMT", "Duplicate To Amount! Please choose another.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        //to validate To_Amt is not greater than company Limit
                        C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.CorporateId == reqaprruleanddetailvm.CorporateId && c.IsDelete == false).FirstOrDefault();
                        decimal cortranvalue = Convert.ToDecimal(cortranrule.Value);
                        if (to_amt > cortranvalue)
                        {
                            ModelState.AddModelError("TO_AMT", "TO_AMOUNT Can\'t Exceed The Company Amount Limit.");
                            ViewBag.NoOfApprover = General.GetNoOfApprover();
                            ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                            return View(reqaprruleanddetailvm);
                        }
                        #endregion

                        #endregion

                        C_Req_ApproverTranRule c_req_Approve_Rule = db.C_Req_ApproverTranRules.Where(a => a.Id == reqaprruleanddetailvm.ID && a.Vrcorporate.DEL_FLAG == false).FirstOrDefault();
                        List<C_Req_ApproverTranRuleDetail> c_req_appruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_req_Approve_Rule.Id).ToList();

                        //Delete Old TranRuleDetails
                        db.C_Req_ApproverTranRuleDetails.RemoveRange(c_req_appruledetail);
                        db.SaveChanges();
                        log.Info(getuserid, controllerName, actionName, 3, $"ApproveRule ID is {c_req_Approve_Rule.Id}.Company Name {c_req_Approve_Rule.Vrcorporate.COMPANY_NAME} is Deleted.");

                        //save rule
                        c_req_Approve_Rule.CorporateId = reqaprruleanddetailvm.CorporateId;
                        c_req_Approve_Rule.IsForBulkPayment = reqaprruleanddetailvm.IsForBulkPayment;
                        c_req_Approve_Rule.FromAmount = from_amt;
                        c_req_Approve_Rule.ToAmount = to_amt;
                        c_req_Approve_Rule.TotalApproverCount = reqaprruleanddetailvm.NO_OF_APPROVERS;
                        c_req_Approve_Rule.Currency = reqaprruleanddetailvm.CURRENCY;
                        //default fields
                        if (c_req_Approve_Rule.STATUS == 2)
                        {
                            c_req_Approve_Rule.STATUS = 0;     //change status back to requested status
                        }
                        c_req_Approve_Rule.REQUESTEDDATE = DateTime.Now;

                        if (TryValidateModel(c_req_Approve_Rule))
                        {
                            //default fields
                            //c_Approve_Rule.createduserid = 1;
                            db.Entry(c_req_Approve_Rule).State = EntityState.Modified;
                            db.SaveChanges();
                            log.Info(getuserid, controllerName, actionName, 2, $"ApproveRule ID is {c_req_Approve_Rule.Id}.Company Name {c_req_Approve_Rule.Vrcorporate.COMPANY_NAME} is Saved.");

                            //save detail
                            if (reqaprruleanddetailvm.NO_OF_APPROVERS > 0)
                            {
                                if (!String.IsNullOrWhiteSpace(reqaprruleanddetailvm.APPROVERSID))
                                {
                                    //save approvers
                                    string[] userids = reqaprruleanddetailvm.APPROVERSID.Split(',');
                                    foreach (var userid in userids)
                                    {
                                        C_Req_ApproverTranRuleDetail newaprdetail = new C_Req_ApproverTranRuleDetail();
                                        newaprdetail.ApproverTranRuleId = c_req_Approve_Rule.Id;
                                        newaprdetail.IsUser = true;
                                        newaprdetail.UserId = userid;
                                        db.C_Req_ApproverTranRuleDetails.Add(newaprdetail);
                                        db.SaveChanges();
                                    }
                                    log.Info(getuserid, controllerName, actionName, 1, $"ApproveRule ID is {c_req_Approve_Rule.Id}. Approver IDs = {reqaprruleanddetailvm.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                                }

                                if (!String.IsNullOrWhiteSpace(reqaprruleanddetailvm.POSITIONSID))
                                {
                                    //save position
                                    string[] positionIDs = reqaprruleanddetailvm.POSITIONSID.Split(',');
                                    foreach (var positionID in positionIDs)
                                    {
                                        C_Req_ApproverTranRuleDetail newaprdetail = new C_Req_ApproverTranRuleDetail();
                                        newaprdetail.ApproverTranRuleId = c_req_Approve_Rule.Id;
                                        newaprdetail.IsUser = false;
                                        newaprdetail.PositionId = Convert.ToInt32(positionID);
                                        db.C_Req_ApproverTranRuleDetails.Add(newaprdetail);
                                        db.SaveChanges();
                                    }
                                    log.Info(getuserid, controllerName, actionName, 1, $"ApproveRule ID is {c_req_Approve_Rule.Id}. POSITIONS IDs = {reqaprruleanddetailvm.POSITIONSID}.Position Saved Count = {positionIDs.Count()}.");
                                }
                            }

                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            ViewBag.Message = "\"" + reqaprruleanddetailvm.COMPANY_NAME + "\"";
                            //return RedirectToAction("Index");
                        }
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                        return View(reqaprruleanddetailvm);
                    }
                    else
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("TO_AMT", "To Amount must be greater than FromAmount.");
                        ViewBag.NoOfApprover = General.GetNoOfApprover();
                        ViewBag.Currencies = new SelectList(db.Currencies.Where(c => c.DEL_FLAG == "0"), "Code", "Code", reqaprruleanddetailvm.CURRENCY);
                        return View(reqaprruleanddetailvm);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                    return View("Error");
                }
            }
        }

        #endregion

        #region delete requested approve list

        //Get:C_ApproveRule/DeleteApproveRule
        public ActionResult DeleteReqApproveRule(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Req_ApproverTranRule c_req_Approve_Rule = db.C_Req_ApproverTranRules.Find(id);
                List<C_Req_ApproverTranRuleDetail> c_req_appruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_req_Approve_Rule.Id).ToList();

                if (c_req_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                ReqApvTranRuleAndDetailVM reqtranrulevm = new ReqApvTranRuleAndDetailVM();
                reqtranrulevm.ID = c_req_Approve_Rule.Id;
                reqtranrulevm.CorporateId = c_req_Approve_Rule.CorporateId;
                reqtranrulevm.COMPANY_NAME = c_req_Approve_Rule.Vrcorporate == null ? "" : c_req_Approve_Rule.Vrcorporate.COMPANY_NAME;
                reqtranrulevm.IsForBulkPayment = c_req_Approve_Rule.IsForBulkPayment;
                reqtranrulevm.FROM_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.FromAmount);
                reqtranrulevm.TO_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.ToAmount);
                reqtranrulevm.CURRENCY = c_req_Approve_Rule.Currency;
                reqtranrulevm.NO_OF_APPROVERS = c_req_Approve_Rule.TotalApproverCount;
                reqtranrulevm.APPROVERS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                reqtranrulevm.POSITIONS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                return View(reqtranrulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ApproveRule/Delete/5
        [HttpPost, ActionName("DeleteReqApproveRule")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReqApproveRuleConfirmed(ReqApvTranRuleAndDetailVM reqtranrulevm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = User.Identity.GetUserId();
            try
            {
                //Delete ApproverTranRule
                C_Req_ApproverTranRule c_req_Approve_Rule = db.C_Req_ApproverTranRules.Where(c => c.Id == reqtranrulevm.ID).FirstOrDefault();
                c_req_Approve_Rule.IsDelete = true;
                db.Entry(c_req_Approve_Rule).State = EntityState.Modified;
                db.SaveChanges();

                //Delete ApproverTranRuleDetail
                List<C_Req_ApproverTranRuleDetail> reqapvTranRuleDetail = db.C_Req_ApproverTranRuleDetails.Where(c => c.ApproverTranRuleId == reqtranrulevm.ID).ToList();
                foreach (var apvTranDetail in reqapvTranRuleDetail)
                {
                    apvTranDetail.IsDelete = true;
                    db.Entry(apvTranDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }
                log.Info(getuserid, controllerName, actionName, 3, $"ApproveRule ID is {c_req_Approve_Rule.Id}.Company Name {c_req_Approve_Rule.Vrcorporate.COMPANY_NAME} is Deleted.{reqapvTranRuleDetail.Count} row Deleted.");

                //to Bind Data if popUp Show in Delete
                reqtranrulevm.ID = c_req_Approve_Rule.Id;
                reqtranrulevm.CorporateId = c_req_Approve_Rule.CorporateId;
                reqtranrulevm.COMPANY_NAME = c_req_Approve_Rule.Vrcorporate == null ? "" : c_req_Approve_Rule.Vrcorporate.COMPANY_NAME;
                reqtranrulevm.IsForBulkPayment = c_req_Approve_Rule.IsForBulkPayment;
                reqtranrulevm.FROM_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.FromAmount);
                reqtranrulevm.TO_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.ToAmount);
                reqtranrulevm.CURRENCY = c_req_Approve_Rule.Currency;
                reqtranrulevm.NO_OF_APPROVERS = c_req_Approve_Rule.TotalApproverCount;
                reqtranrulevm.APPROVERS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                reqtranrulevm.POSITIONS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));
                ViewBag.Message = "\"" + reqtranrulevm.COMPANY_NAME + "\"";
                return View(reqtranrulevm);
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion

        #endregion

        #region C_ApproveRuleApprovedList

        private void Getddl()
        {
            ViewBag.ruTYPEList = General.GetTypes();
            //Create list
            ViewBag.ruSTATUSList = General.GetStatus();

            ViewBag.ruRequestStatus = General.GetRequestStatus();

            ViewBag.ruTransactionType = General.GetTransactionTypes();

            ViewBag.ruAppTranType = General.GetPaymentTypes();
        }

        // GET: ApproveRule
        public ActionResult ApprovedIndex()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Getddl();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Post Approve Rule
        [HttpPost]
        public ActionResult ApprovedIndex(int? args)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
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
                var aCompanyName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var aType = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var aTransactionType = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                var aStatus = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                var fromdate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
                var todate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
                var approverName = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();

                //show only requests of his/her Branch
                //get BranchID from Logged In User
                string userid = User.Identity.GetUserId();
                AdminUser admin = db.Users.Find(userid);
                DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
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
                //filter IsDelete flag,     for Corporate   => ID > 0 and vrCorporate not null,     for either approver or position     => Details > 0
                //IEnumerable<ReqCorporateVM> reqcorvm = db.C_Req_Corporates.Where(u => ((admin.BranchID == u.VrMaker.BranchID || admin.VrBranch.code == "9999") && u.MAKER != userid) || u.CHECKER == userid ).ToList()
                var apvrule = db.C_Req_ApproverTranRules.Where(u => u.IsDelete == false && ((admin.BranchID == u.VrMaker.BranchID || admin.VrBranch.code == "9999") && u.MAKER != userid) || u.CHECKER == userid && u.CorporateId > 0 && u.Vrcorporate != null && u.Vrcorporate.DEL_FLAG == false).ToList()
                .ConvertAll(apr => new ReqApvTranRuleAndDetailVM
                {
                    ID = apr.Id,
                    COMPANY_NAME = apr.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = apr.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(apr.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(apr.ToAmount),
                    CURRENCY = apr.Currency,
                    NO_OF_APPROVERS = apr.TotalApproverCount,
                    APPROVERS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == true && p.UserId != null)
                                .Join(db._Users,
                                aprd => aprd.UserId,
                                _user => _user.USERID,
                                (aprd, _user) => new
                                {
                                    UserName = _user.USERNAME
                                })
                                .Select(d => d.UserName).ToList()
                                .Where(s => !string.IsNullOrEmpty(s))),                 
                    POSITIONS = string.Join(",", apr.Details
                                .Where(p => p.IsUser == false && p.Vrposition != null)
                                .Select(d => d.Vrposition.NAME).ToList()
                                .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", apr.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Select(d => d.UserId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", apr.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.PositionId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    REQUESTEDDATE = apr.REQUESTEDDATE,
                    TYPE = apr.TYPE,
                    STATUS = apr.STATUS
                });

                if (!string.IsNullOrEmpty(aCompanyName))
                {
                    apvrule = apvrule.Where(a => a.COMPANY_NAME.ToLower().Trim().Contains(aCompanyName.ToLower().Trim())).ToList();
                }

                if (!string.IsNullOrEmpty(aType))
                {
                    int tran_type = Convert.ToInt32(aType);
                    apvrule = apvrule.Where(a => a.TYPE == tran_type).ToList();
                }

                if (!string.IsNullOrEmpty(aTransactionType))
                {
                    apvrule = apvrule.Where(a => a.IsForBulkPayment.ToString() == aTransactionType).ToList();
                }

                if (!string.IsNullOrEmpty(approverName))
                {
                    apvrule = apvrule.Where(a => a.APPROVERS.ToLower().Trim().Contains(approverName.ToLower().Trim())).ToList();
                }

                if (!string.IsNullOrEmpty(aStatus))
                {
                    apvrule = apvrule.Where(a => a.STATUS.ToString() == aStatus.ToString()).ToList();
                }

                if (fromDate != null & toDate != null)
                {
                    apvrule = apvrule.Where(a => a.REQUESTEDDATE >= fromDate && a.REQUESTEDDATE < toDate).ToList();
                }

                else if (fromDate != null)
                {
                    apvrule = apvrule.Where(a => a.REQUESTEDDATE >= fromDate).ToList();
                }

                else if (toDate != null)
                {
                    apvrule = apvrule.Where(a => a.REQUESTEDDATE < toDate).ToList();
                }


                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    apvrule = apvrule.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                #region Global Search
                //var search = Request.Form.GetValues("search[value]").FirstOrDefault();
                //search = search.ToLower().Trim();
                //if (!string.IsNullOrEmpty(search))
                //{
                //    apvrule = apvrule.Where(a => a.COMPANY_NAME.ToLower().Contains(search)
                //                || a.FROM_AMT.ToString().Contains(search)
                //                || a.TO_AMT.ToString().Contains(search)
                //                || a.CURRENCY.ToLower().Contains(search)
                //                || a.NO_OF_APPROVERS.ToString().ToLower().Contains(search)
                //                || a.APPROVERS.ToLower().Contains(search)
                //                || a.POSITIONS.ToLower().Contains(search)
                //                //Replace bool true to string "bulk" fale to string "normal"
                //                || a.IsForBulkPayment.ToString().Replace(a.IsForBulkPayment.ToString(), a.IsForBulkPayment == true ? "bulk" : "normal").Contains(search)
                //    ).AsEnumerable();
                //}
                #endregion

                recordsTotal = apvrule.Count();
                var data = apvrule.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
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
                C_Req_ApproverTranRule reqTranRule = db.C_Req_ApproverTranRules.Where(r => r.Id == id && r.IsDelete == false).FirstOrDefault();
                //List<C_Req_ApproverTranRuleDetail> reqTranRuleDetail = db.C_Req_ApproverTranRuleDetails.Where(d => d.ApproverTranRuleId == reqTranRule.Id && d.IsDelete == reqTranRule.IsDelete).ToList();

                //validate
                string error = CheckValidate(reqTranRule);
                if (error != null)
                {
                    return Json(new { msg = error }, JsonRequestBehavior.AllowGet);
                }               

                var apvrule = new ReqApvTranRuleAndDetailVM
                {
                    Req_TranRuleId = reqTranRule.Req_TranRuleId,
                    CorporateId = reqTranRule.CorporateId,
                    COMPANY_NAME = reqTranRule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = reqTranRule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(reqTranRule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(reqTranRule.ToAmount),
                    CURRENCY = reqTranRule.Currency,
                    NO_OF_APPROVERS = reqTranRule.TotalApproverCount,
                    APPROVERS = string.Join(",", reqTranRule.Details
                              .Where(p => p.IsUser == true && p.UserId != null)
                              .Join(db._Users,
                              aprd => aprd.UserId,
                              _user => _user.USERID,
                              (aprd, _user) => new
                              {
                                  UserName = _user.USERNAME
                              })
                              .Select(d => d.UserName).ToList()
                              .Where(s => !string.IsNullOrEmpty(s))),

                    POSITIONS = string.Join(",", reqTranRule.Details
                              .Where(p => p.IsUser == false && p.Vrposition != null)
                              .Select(d => d.Vrposition.NAME).ToList()
                              .Where(s => !string.IsNullOrEmpty(s))),
                    APPROVERSID = string.Join(",", reqTranRule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Select(d => d.UserId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    POSITIONSID = string.Join(",", reqTranRule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.PositionId).ToList()
                            .Where(s => !string.IsNullOrEmpty(s.ToString()))),
                    REQUESTEDDATE = reqTranRule.REQUESTEDDATE,
                    TYPE = reqTranRule.TYPE,
                    STATUS = reqTranRule.STATUS
                };



                Boolean isSave = false;
                //Boolean isSaveBulkFile = false;
                //Boolean isSaveTranRule = false;

                //if Type is 0(register = 0, update = 1 , delete = 2)
                if (apvrule.TYPE == 0)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            C_ApproverTranRule c_approverule = new C_ApproverTranRule();
                            // try to save at Corporate table first
                            isSave = SaveCApproveRule(apvrule, ref c_approverule);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            reqTranRule.STATUS = 1;
                            reqTranRule.Req_TranRuleId = c_approverule.Id;
                            reqTranRule.CorporateId = c_approverule.CorporateId;
                            reqTranRule.CHECKER = User.Identity.GetUserId();
                            reqTranRule.CHECKEDDATE = DateTime.Now;
                            //Checker Reason

                            db.Entry(reqTranRule).State = EntityState.Modified;
                            db.SaveChanges();

                            // everything is fine - save all the data permanently
                            transaction.Commit();
                            // ViewData["Message"] = "Approve successfully!";
                        }
                        catch (Exception ex)
                        {
                            // roll back all database operations, if anything goes wrong
                            transaction.Rollback();
                            //ViewBag.ResultMessage = "Error occured, records rolledback.";
                        }
                    }
                }
                else if (apvrule.TYPE == 1) //Update if Type is 1 
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        C_ApproverTranRule approverule = db.C_ApproverTranRules.Where(a => a.IsDelete == false && a.Id == apvrule.Req_TranRuleId).FirstOrDefault();
                        try
                        {
                            //to update user
                            isSave = UpdateCApproveRule(apvrule);

                            // try to update Req Status to Approved
                            // status to Approved (1)
                            reqTranRule.STATUS = 1;
                            reqTranRule.CHECKER = User.Identity.GetUserId();
                            reqTranRule.CHECKEDDATE = DateTime.Now;
                            //Checker Reason

                            db.Entry(reqTranRule).State = EntityState.Modified;
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
                            isSave = DeleteCApproveRule(apvrule);
                            reqTranRule.STATUS = 1;
                            reqTranRule.CHECKER = User.Identity.GetUserId();
                            reqTranRule.CHECKEDDATE = DateTime.Now;
                            //Checker Reason
                            db.Entry(reqTranRule).State = EntityState.Modified;
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

        //saving ApproveRule
        private Boolean SaveCApproveRule(ReqApvTranRuleAndDetailVM apvrule, ref C_ApproverTranRule approverule)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            decimal from_amt = 0;
            decimal to_amt = 0;
            try
            {
                string LoginUser = User.Identity.GetUserId();
                from_amt = General.ConvertStrToDecimal(apvrule.FROM_AMT);
                to_amt = General.ConvertStrToDecimal(apvrule.TO_AMT);

                //save rule
                approverule.CorporateId = apvrule.CorporateId;
                approverule.IsForBulkPayment = apvrule.IsForBulkPayment;
                approverule.FromAmount = from_amt;
                approverule.ToAmount = to_amt;
                approverule.TotalApproverCount = apvrule.NO_OF_APPROVERS;
                approverule.Currency = apvrule.CURRENCY;
                approverule.CreatedDateTime = DateTime.Now;
                approverule.CreatedUserID = User.Identity.GetUserId();
                //default fields
                //c_Approve_Rule.createduserid = 1;
                db.C_ApproverTranRules.Add(approverule);
                db.SaveChanges();
                //save log
                log.Info(LoginUser, controllerName, actionName, 1, $"Corporate ID {approverule.CorporateId} is Saved.");

                //save detail
                if (apvrule.NO_OF_APPROVERS > 0)
                {
                    if (!String.IsNullOrWhiteSpace(apvrule.APPROVERSID))
                    //if (aprruleanddetailvm.APPROVERSID != 0)
                    {
                        //save approvers
                        string[] userids = apvrule.APPROVERSID.Split(',');
                        foreach (var userid in userids)
                        {
                            C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
                            newaprdetail.ApproverTranRuleId = approverule.Id;
                            newaprdetail.IsUser = true;
                            newaprdetail.UserId = userid;
                            db.C_ApproverTranRuleDetails.Add(newaprdetail);
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 1, $"Approver IDs = {apvrule.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                    }

                    if (!String.IsNullOrWhiteSpace(apvrule.POSITIONSID))
                    {
                        //save position
                        string[] positionIDs = apvrule.POSITIONSID.Split(',');
                        foreach (var positionID in positionIDs)
                        {
                            C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
                            newaprdetail.ApproverTranRuleId = approverule.Id;
                            newaprdetail.IsUser = false;
                            newaprdetail.PositionId = Convert.ToInt32(positionID);
                            db.C_ApproverTranRuleDetails.Add(newaprdetail);
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 1, $"Position IDs= {apvrule.POSITIONSID}. Position Saved Count = {positionIDs.Count()}.");

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
        }


        //update ApproveRule
        private Boolean UpdateCApproveRule(ReqApvTranRuleAndDetailVM apvrule)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {

                C_ApproverTranRule approverule = db.C_ApproverTranRules.Where(a => a.IsDelete == false && a.Id == apvrule.Req_TranRuleId).FirstOrDefault();
                string LoginUser = User.Identity.GetUserId();
                decimal from_amt = 0;
                decimal to_amt = 0;
                from_amt = General.ConvertStrToDecimal(apvrule.FROM_AMT);
                to_amt = General.ConvertStrToDecimal(apvrule.TO_AMT);

                //save rule
                //C_ApproverTranRule c_Approve_Rule = new C_ApproverTranRule();
                approverule.CorporateId = apvrule.CorporateId;
                approverule.IsForBulkPayment = apvrule.IsForBulkPayment;
                approverule.FromAmount = from_amt;
                approverule.ToAmount = to_amt;
                approverule.TotalApproverCount = apvrule.NO_OF_APPROVERS;
                approverule.Currency = apvrule.CURRENCY;
                approverule.UpdatedDateTime = DateTime.Now;
                approverule.UpdatedUserId = User.Identity.GetUserId();
                //default fields
                //c_Approve_Rule.createduserid = 1;
                db.Entry(approverule).State = EntityState.Modified;
                //db.C_ApproverTranRules.Add(approverule);
                db.SaveChanges();
                //save log
                log.Info(LoginUser, controllerName, actionName, 1, $"Corporate ID {approverule.CorporateId} is Saved.");

                //Delete Old TranRuleDetails
                List<C_ApproverTranRuleDetail> c_appruledetail = db.C_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == approverule.Id).ToList();
                db.C_ApproverTranRuleDetails.RemoveRange(c_appruledetail);
                db.SaveChanges();
                log.Info(LoginUser, controllerName, actionName, 3, $"ApproveRule ID is {approverule.Id}.Company Name {approverule.Vrcorporate.COMPANY_NAME} is Deleted.");
                //save detail
                if (apvrule.NO_OF_APPROVERS > 0)
                {
                    if (!String.IsNullOrWhiteSpace(apvrule.APPROVERSID))
                    //if (aprruleanddetailvm.APPROVERSID != 0)
                    {
                        //save approvers
                        string[] userids = apvrule.APPROVERSID.Split(',');
                        foreach (var userid in userids)
                        {
                            C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
                            newaprdetail.ApproverTranRuleId = approverule.Id;
                            newaprdetail.IsUser = true;
                            newaprdetail.UserId = userid;
                            db.C_ApproverTranRuleDetails.Add(newaprdetail);
                            //db.Entry(newaprdetail).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 2, $"Approver IDs = {apvrule.APPROVERSID}. Approver Saved Count =  {userids.Count()}.");
                    }

                    if (!String.IsNullOrWhiteSpace(apvrule.POSITIONSID))
                    {
                        //save position
                        string[] positionIDs = apvrule.POSITIONSID.Split(',');
                        foreach (var positionID in positionIDs)
                        {
                            C_ApproverTranRuleDetail newaprdetail = new C_ApproverTranRuleDetail();
                            newaprdetail.ApproverTranRuleId = approverule.Id;
                            newaprdetail.IsUser = false;
                            newaprdetail.PositionId = Convert.ToInt32(positionID);
                            db.C_ApproverTranRuleDetails.Add(newaprdetail);
                            //db.Entry(newaprdetail).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //save log
                        log.Info(LoginUser, controllerName, actionName, 2, $"Position IDs= {apvrule.POSITIONSID}. Position Saved Count = {positionIDs.Count()}.");

                    }
                }

                // save log
               
                log.Info(LoginUser, controllerName, actionName, 2, $"Updated Approve Rule for {approverule.CorporateId} is successful.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }

        //Delete ApproveRule
        private Boolean DeleteCApproveRule(ReqApvTranRuleAndDetailVM apvrule)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string LoginUser = User.Identity.GetUserId();
                C_ApproverTranRule approverule = db.C_ApproverTranRules.Where(a => a.IsDelete == false && a.Id == apvrule.Req_TranRuleId).FirstOrDefault();
                approverule.IsDelete = true;
                approverule.UpdatedDateTime = DateTime.Now;
                approverule.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(approverule).State = EntityState.Modified;
                db.SaveChanges();
                // save log
                string getuserid = User.Identity.GetUserId();
                log.Info(LoginUser, controllerName, actionName, 3, $"Approver IDs = {apvrule.APPROVERSID} is Deleted.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                throw ex;
            }
            //return false;
        }

        //Reject C_ApproveRule
        [HttpPost]
        public ActionResult Reject(int id, string remark)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string LoginUser = User.Identity.GetUserId();
                C_Req_ApproverTranRule reqTranRule = db.C_Req_ApproverTranRules.Where(r => r.Id == id).FirstOrDefault();

                var apvrule = new ReqApvTranRuleAndDetailVM
                {
                    ID = reqTranRule.Id,
                    CorporateId = reqTranRule.CorporateId,
                    COMPANY_NAME = reqTranRule.Vrcorporate.COMPANY_NAME,
                    IsForBulkPayment = reqTranRule.IsForBulkPayment,
                    FROM_AMT = General.ConvertDecimalToStr(reqTranRule.FromAmount),
                    TO_AMT = General.ConvertDecimalToStr(reqTranRule.ToAmount),
                    CURRENCY = reqTranRule.Currency,
                    NO_OF_APPROVERS = reqTranRule.TotalApproverCount,
                    APPROVERS = string.Join(",", reqTranRule.Details
                              .Where(p => p.IsUser == true && p.UserId != null)
                              .Join(db._Users,
                              aprd => aprd.UserId,
                              _user => _user.USERID,
                              (aprd, _user) => new
                              {
                                  UserName = _user.USERNAME
                              })
                              .Select(d => d.UserName).ToList()
                              .Where(s => !string.IsNullOrEmpty(s))),

                    POSITIONS = string.Join(",", reqTranRule.Details
                              .Where(p => p.IsUser == false && p.Vrposition != null)
                              .Select(d => d.Vrposition.NAME).ToList()
                              .Where(s => !string.IsNullOrEmpty(s))),
                    REQUESTEDDATE = reqTranRule.REQUESTEDDATE,
                    TYPE = reqTranRule.TYPE,
                    STATUS = reqTranRule.STATUS
                };

                //string[] userids = apvrule.APPROVERSID.Split(',');
                C_Req_ApproverTranRule reqApproveRule = db.C_Req_ApproverTranRules.Find(id);
                if (reqApproveRule.STATUS == 0)  //reject only from request status
                {
                    //Status => 2 for reject
                    reqApproveRule.STATUS = 2;
                    reqApproveRule.CHECKER = User.Identity.GetUserId();
                    reqApproveRule.CHECKEDDATE = DateTime.Now;
                    //checker reason
                    reqApproveRule.CHECKERREASON = remark;
                    db.Entry(reqApproveRule).State = EntityState.Modified;
                    db.SaveChanges();

                    log.Info(LoginUser, controllerName, actionName, 3, $"Approver IDs = {apvrule.APPROVERSID} is rejected.");

                    return Json("Reject Successfully", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json("Reject Fail!", JsonRequestBehavior.AllowGet);
        }

        // GET: ApproveRule/ApprovedDetails/5
        public ActionResult ApprovedDetails(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Req_ApproverTranRule c_req_Approve_Rule = db.C_Req_ApproverTranRules.Find(id);
                List<C_Req_ApproverTranRuleDetail> c_appruledetail = db.C_Req_ApproverTranRuleDetails.Where(a => a.ApproverTranRuleId == c_req_Approve_Rule.Id).ToList();
                if (c_req_Approve_Rule == null)
                {
                    return HttpNotFound();
                }
                ReqApvTranRuleAndDetailVM reqAppRulevm = new ReqApvTranRuleAndDetailVM();
                reqAppRulevm.ID = c_req_Approve_Rule.Id;
                reqAppRulevm.CorporateId = c_req_Approve_Rule.CorporateId;
                reqAppRulevm.COMPANY_NAME = c_req_Approve_Rule.Vrcorporate == null ? "" : c_req_Approve_Rule.Vrcorporate.COMPANY_NAME;
                reqAppRulevm.IsForBulkPayment = c_req_Approve_Rule.IsForBulkPayment;
                reqAppRulevm.FROM_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.FromAmount);
                reqAppRulevm.TO_AMT = General.ConvertDecimalToStr(c_req_Approve_Rule.ToAmount);
                reqAppRulevm.CURRENCY = c_req_Approve_Rule.Currency;
                reqAppRulevm.NO_OF_APPROVERS = c_req_Approve_Rule.TotalApproverCount;
                reqAppRulevm.APPROVERS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == true && p.UserId != null)
                            .Join(db._Users,
                            aprd => aprd.UserId,
                            _user => _user.USERID,
                            (aprd, _user) => new
                            {
                                UserName = _user.USERNAME
                            })
                            .Select(d => d.UserName).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));

                reqAppRulevm.POSITIONS = string.Join(",", c_req_Approve_Rule.Details
                            .Where(p => p.IsUser == false && p.Vrposition != null)
                            .Select(d => d.Vrposition.NAME).ToList()
                            .Where(s => !string.IsNullOrEmpty(s)));
                reqAppRulevm.APPROVERSID = string.Join(",", c_req_Approve_Rule.Details
                           .Where(p => p.IsUser == true && p.UserId != null)
                           .Select(d => d.UserId).ToList()
                           .Where(s => !string.IsNullOrEmpty(s.ToString())));
                reqAppRulevm.POSITIONSID = string.Join(",", c_req_Approve_Rule.Details
                         .Where(p => p.IsUser == false && p.Vrposition != null)
                         .Select(d => d.PositionId).ToList()
                         .Where(s => !string.IsNullOrEmpty(s.ToString())));
                reqAppRulevm.REQUESTEDDATE = c_req_Approve_Rule.REQUESTEDDATE;
                reqAppRulevm.TYPE = c_req_Approve_Rule.TYPE;
                reqAppRulevm.STATUS = c_req_Approve_Rule.STATUS;
                reqAppRulevm.CHECKEDDATE = c_req_Approve_Rule.CHECKEDDATE;
                reqAppRulevm.CHECKERREASON = c_req_Approve_Rule.CHECKERREASON;
                reqAppRulevm.MAKER = c_req_Approve_Rule.VrMaker.UserName;
                reqAppRulevm.CHECKER = (c_req_Approve_Rule.VrChecker == null ? null : c_req_Approve_Rule.VrChecker.UserName);

                return View(reqAppRulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        /// <summary>
        /// GetApprovers
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetApprovers(string term)
        {
            var data = await db.C_UserInRoles
           .Join(db._Users, ur => ur.UserId, u => u.USERID,
           ((userInRole, user) => new { user = user, userInRole = userInRole }))
           .Where(w => w.user.CORPORATEID != null && w.user.USERNAME.Contains(term) && w.userInRole.RoleId == 3)
                   .Select(s => new
                   {
                       UserID = s.user.USERID,
                       UserName = s.user.USERNAME,

                   }).OrderBy(o => o.UserName).AsNoTracking().ToListAsync();
            //var data = await db._Users
            //    .Where(w => w.CORPORATEID!=null && w.USERNAME.Contains(term))
            //    .Select(s => new
            //    {
            //        UserID = s.USERID,
            //        UserName = s.USERNAME,

            //    }).OrderBy(o => o.UserName).AsNoTracking().ToListAsync();
            return Json(data);
        }
        [HttpPost]
        public async Task<JsonResult> GetCorporateData(string term)
        {
            var data = await db.C_Corporates
                .Where(w => w.COMPANY_NAME.Contains(term) && w.DEL_FLAG == false)
                .Select(s => new
                {
                    value = s.ID,
                    text = s.COMPANY_NAME,
                }).OrderBy(o => o.text).AsNoTracking().ToListAsync();
            return Json(data);
        }
        [HttpPost]
        public async Task<JsonResult> GetCorporateApprover(string term, string corporateID)
        {
            //var data = await db._Users
            //    .Where(w => w.CORPORATEID.ToString() == corporateID && w.USERNAME.Contains(term))
            //    .Select(s => new
            //    {
            //        value = s.USERID,
            //        text = s.USERNAME,

            //    }).OrderBy(o => o.text).AsNoTracking().ToListAsync();
            var data = await db._Users
           .Join(db.C_UserInRoles, u => u.USERID, ur => ur.UserId,
           ((user, userInRole) => new { user = user, userInRole = userInRole }))
           .Where(w => w.user.CORPORATEID != null && w.user.USERNAME.Contains(term) && w.userInRole.VrCRole.Name == "Approver" && w.user.CORPORATEID.ToString() == corporateID && w.user.DEL_STATUS == false)
           .Select(s => new
           {
               value = s.user.USERID,
               text = s.user.USERNAME,

           }).OrderBy(o => o.text).AsNoTracking().ToListAsync();
            return Json(data);
        }

        /// <summary>
        /// GetPositions
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetPositions(string term)
        {
            var data = await db.C_Positions
                .Where(w => w.NAME.Contains(term) && w.DEL_FLAG == false)
                .Select(s => new
                {
                    value = s.ID,
                    text = s.NAME
                }).OrderBy(o => o.text).AsNoTracking().ToListAsync();
            return Json(data);
        }

        //to check approver count
        private Boolean CheckApproverCount(string AprIDs, string PositionIDs, int CorporateID, int NoOfApprovers)
        {
            //get approver count
            int aprCount = 0;
            if (!string.IsNullOrWhiteSpace(AprIDs))
            {
                aprCount = AprIDs.Split(',').Length;
                //need to check with selected positions (not to overlap)

            }
            int positionAprCount = 0;
            if (!String.IsNullOrWhiteSpace(PositionIDs))
            {
                int posID = 0;
                foreach (var positionID in PositionIDs.Split(','))
                {
                    if (Int32.TryParse(positionID, out posID))
                    {
                        positionAprCount += db._Users
                      .Join(db.C_UserInRoles, u => u.USERID, ur => ur.UserId,
                      ((user, userInRole) => new { user = user, userInRole = userInRole }))
                      .Where(w => w.user.PositionId == (int?)posID && w.user.CORPORATEID == (int?)CorporateID
                      && w.userInRole.VrCRole.Name == "Approver" && w.user.DEL_STATUS == false)
                      .Count();
                    }
                }
            }
            return ((aprCount + positionAprCount) >= NoOfApprovers);
        }

        private string CheckValidate(C_Req_ApproverTranRule reqTranRule)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //checking Validation
                #region Validation
                //check current status
                //if already approved
                if (reqTranRule.STATUS == 1)
                    return "Already approved.";
                //if already rejected
                if (reqTranRule.STATUS == 2)
                    return "Already rejected.";

                #endregion

                #region amount overlap validation
                //convert from amount limit
                var amt_limit = Convert.ToDecimal(9999999999999999.99);  
                if(reqTranRule.FromAmount > amt_limit)
                { 
                    return "The field Allow From Amount must be between 0 and 9,999,999,999,999,999.99";
                }
                //check to amount limit
                if (reqTranRule.ToAmount > amt_limit)
                {
                    return "The field Allow To Amount must be between 0 and 9,999,999,999,999,999.99";
                }
                if (reqTranRule.FromAmount >= reqTranRule.ToAmount)
                {
                    return "To Amount must be greater than From Amount.";
                }
                //to check inner gap
                if (db.C_ApproverTranRules.Where(a =>a.Id != reqTranRule.Req_TranRuleId & a.CorporateId == reqTranRule.CorporateId & a.FromAmount >= reqTranRule.FromAmount & a.ToAmount <= reqTranRule.ToAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                {
                    return "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.";
                }
                //to check From
                if (db.C_ApproverTranRules.Where(a => a.Id != reqTranRule.Req_TranRuleId & a.CorporateId == reqTranRule.CorporateId & a.FromAmount <= reqTranRule.FromAmount & a.ToAmount >= reqTranRule.FromAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                {
                    return "Duplicate From Amount! Please choose another.";
                }
                //to check To
                if (db.C_ApproverTranRules.Where(a => a.Id != reqTranRule.Req_TranRuleId & a.CorporateId == reqTranRule.CorporateId & a.FromAmount <= reqTranRule.ToAmount & a.ToAmount >= reqTranRule.ToAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                {
                    return "Duplicate To Amount! Please choose another.";
                }

                ////to check inner gap
                //if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqTranRule.CorporateId & a.FromAmount >= reqTranRule.FromAmount & a.ToAmount <= reqTranRule.ToAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                //{
                //    return "Duplicate Rule! One or more rules with current range is existing now. Please choose other range.";
                //}
                ////to check From
                //if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqTranRule.CorporateId & a.FromAmount <= reqTranRule.FromAmount & a.ToAmount >= reqTranRule.FromAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                //{
                //    return "Duplicate From Amount! Please choose another.";
                //}
                ////to check To
                //if (db.C_Req_ApproverTranRules.Where(a => a.CorporateId == reqTranRule.CorporateId & a.FromAmount <= reqTranRule.ToAmount & a.ToAmount >= reqTranRule.ToAmount & a.IsDelete == false & a.Currency == reqTranRule.Currency & a.IsForBulkPayment == reqTranRule.IsForBulkPayment & a.STATUS == 0 & a.Vrcorporate.DEL_FLAG == false).Count() > 0)
                //{
                //    return "Duplicate To Amount! Please choose another.";
                //}

                //to validate To_Amt is not greater than company Limit
                C_CorporateTranRule cortranrule = db.C_CorporateTranRules.Where(c => c.CorporateId == reqTranRule.CorporateId & c.IsDelete == false).FirstOrDefault();
                decimal cortranvalue = Convert.ToDecimal(cortranrule.Value);
                if (reqTranRule.ToAmount > cortranvalue)
                {
                    return "TO_AMOUNT Can\'t Exceed The Company Amount Limit.";
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