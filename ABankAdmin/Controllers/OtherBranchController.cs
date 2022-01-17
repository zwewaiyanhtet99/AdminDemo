using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System.Linq.Dynamic;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using ABankAdmin.Core.Utils;

namespace ABankAdmin.Controllers
{
    public class OtherBranchController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "OtherBranchController";
        EventLogController log = new EventLogController();

        //get connection string
        string connectionString = ConstantValues.Connection_String;
        OtherBankChargeRateInfo chargerate = new OtherBankChargeRateInfo(); 
        OtherBankInformationInquiry bankinfo = new OtherBankInformationInquiry();

        // GET: OtherBranch
        public ActionResult Index()
        {
            bindBranch();
            return View();
        }

        //Index Post
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
            var bank = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            var branch = (from b in db.OtherBranches
                          where b.Active == true
                          select b);

            //Searching            
            if (!string.IsNullOrEmpty(bank))
            {
                int bank_id = Convert.ToInt32(bank);
                branch = branch.Where(a => a.OTHER_BANK_ID == bank_id);
            }

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                branch = branch.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = branch.Count();
            var data = branch.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        // GET: OtherBranch/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //OtherBranch otherBranch = db.OtherBranches.Find(id);
                var result = from b in db.OtherBranches
                             where b.Active == true
                             select b;
                OtherBranch otherBranch = result.SingleOrDefault(m => m.OTHER_BRANCH_ID == id);
                if (otherBranch == null)
                {
                    return HttpNotFound();
                }
                return View(otherBranch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
      
        // GET: OtherBranch/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                bindBranch();
                return View();

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        public JsonResult getBranch(int bankId)
        {
            try
            {
                OtherBank otherbank = db.OtherBanks.Find(bankId);
                if (otherbank == null)
                {
                    return null;
                }
                //OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(otherbank.BANK_CODE);
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select Branch--", Value = "" });
                items.Add(new SelectListItem { Text = "Branch 01", Value = "1" });
                items.Add(new SelectListItem { Text = "Branch 02", Value = "1" });
                items.Add(new SelectListItem { Text = "Branch 03", Value = "1" });
                //if (branchlst.OtherBranchList != null && branchlst.OtherBranchList.Count > 0)
                //{
                //    foreach (var m in branchlst.OtherBranchList)
                //    {
                //        items.Add(new SelectListItem {Text = m.BR_Name, Value = m.BR_Name });
                //    }
                //}
                return Json(new SelectList(items, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
         public JsonResult getBranchData(int bankId, string branchname)
        {
            try
            {
                OtherBank otherbank = db.OtherBanks.Find(bankId);
                if (otherbank == null)
                {
                    return null;
                }
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(otherbank.BANK_CODE);
                OtherBranch otherbranch = new OtherBranch();
                if (!string.IsNullOrEmpty(branchname))
                {
                    if (branchlst.OtherBranchList != null && branchlst.OtherBranchList.Count > 0)
                    {
                        var branch = branchlst.OtherBranchList.Where(c => c.BR_Name == branchname).FirstOrDefault();
                        otherbranch.BR_NAME = branch.BR_Name;
                        otherbranch.BR_CODE = branch.BR_CODE;
                        otherbranch.BR_SHORT_NAME = branch.br_short_name;
                        otherbranch.FI_CODE = branch.FI_CODE;
                        otherbranch.CR_BR_CODE = branch.CR_BR_CODE;
                        otherbranch.BR_CITY_CODE = branch.br_city_code;
                        otherbranch.BR_STATE_CODE = branch.br_state_code;
                    }
                }
                return Json(otherbranch, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [NonAction]
        private void bindBranch(string branchname = "")
        {
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
                ViewBag.OtherBank = li;

                //get branch if bank is selected
                List<SelectListItem> branch = new List<SelectListItem>();
                branch.Add(new SelectListItem { Text = "--Select Branch--", Value = "" });
                branch.Add(new SelectListItem { Text = "Branch01", Value = "1" });
                branch.Add(new SelectListItem { Text = "Branch02", Value = "2" });
                branch.Add(new SelectListItem { Text = "Branch03", Value = "3" });
                branch.Add(new SelectListItem { Text = "Branch04", Value = "4" });
                branch.Add(new SelectListItem { Text = "Branch05", Value = "5" });
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
                ViewBag.Branch = branch;

                //Get Charges Code
                ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                cinfo = CRInquiry.OtherBankChargeRateInquiry();

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select Charges Code--", Value = "" });
                items.Add(new SelectListItem { Text = "C001", Value = "1" });
                items.Add(new SelectListItem { Text = "C002", Value = "2" });
                items.Add(new SelectListItem { Text = "C003", Value = "3" });
                items.Add(new SelectListItem { Text = "C004", Value = "4" });
                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        items.Add(new SelectListItem { Text = $"{m.ChargeCode} ", Value = m.ChargeCode });   //showing charge code dropdown inclding fix amt and %
                    }
                }
                ViewBag.ChargeCode = items;

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "bindBranch", ex);
            }

        }
   
        // POST: OtherBranch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OTHER_BRANCH_ID,OTHER_BANK_ID,BR_CODE,BR_SHORT_NAME,BR_NAME,FI_CODE,CR_BR_CODE,CHARGES_CODE,BR_CITY_CODE,BR_STATE_CODE,Address")] OtherBranch otherBranch)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //Duplicate OtherBranch Code
                    if (db.OtherBranches.Where(b => b.BR_CODE == otherBranch.BR_CODE && b.OTHER_BANK_ID == otherBranch.OTHER_BANK_ID && b.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("BR_NAME", "Duplicate Branch Code");
                        bindBranch();
                        goto dropdown;
                    }

                    OtherBank otherBank = db.OtherBanks.Find(otherBranch.OTHER_BANK_ID);
                    if (otherBank == null)
                    {
                        return null;
                    }
                    OtherBranchInfoList branchlist = bankinfo.OtherBranchInfoInquiry(otherBank.BANK_CODE);

                    // try to save at core data
                    var BranchDB = branchlist.OtherBranchList.Where(c => c.BR_CODE == otherBranch.BR_CODE).FirstOrDefault();
                    otherBranch.BR_NAME = BranchDB.BR_Name;
                    otherBranch.BR_CODE = BranchDB.BR_CODE;
                    otherBranch.BR_SHORT_NAME = BranchDB.br_short_name;
                    otherBranch.FI_CODE = BranchDB.FI_CODE;
                    otherBranch.CR_BR_CODE = BranchDB.CR_BR_CODE;
                    otherBranch.BR_CITY_CODE = BranchDB.br_city_code;
                    otherBranch.BR_STATE_CODE = BranchDB.br_state_code;

                    otherBranch.Active = true;
                    otherBranch.CreatedDate = DateTime.Now;
                    otherBranch.CreatedUserId = User.Identity.GetUserId();
                    db.OtherBranches.Add(otherBranch);
                    db.SaveChanges();
                    //save log 
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, otherBranch.ToString());
                    return RedirectToAction("Index");
                }
                dropdown:

                IEnumerable<SelectListItem> otherbank = db.OtherBanks.Where(b => b.Active == true)
                          .Select(f => new SelectListItem()
                          {
                              Value = f.OTHER_BANK_ID.ToString(),
                              Text = f.BANK_NAME
                          });
                ViewBag.OtherBank = otherbank;

                //Get Select Branch               
                var bankcode = db.OtherBanks.Where(a => a.OTHER_BANK_ID == otherBranch.OTHER_BANK_ID).FirstOrDefault();
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(bankcode.BANK_CODE);
                IEnumerable<SelectListItem> branch = branchlst.OtherBranchList
                           .Select(f => new SelectListItem()
                           {
                               Value = f.BR_Name,
                               Text = f.BR_Name
                           });
                ViewBag.Branch = branch;

                //For ChargesCode
                ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                cinfo = CRInquiry.OtherBankChargeRateInquiry();

                IEnumerable<SelectListItem> chargecode = cinfo.ChargeCodeInfo
                           .Select(f => new SelectListItem()
                           {
                               Value = f.ChargeCode,
                               Text = f.ChargeCode
                           });
                ViewBag.ChargeCode = chargecode;
                return View(otherBranch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: OtherBranch/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();            
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                 var result = from b in db.OtherBranches
                             where b.Active == true
                             select b;
                OtherBranch otherBranch = result.SingleOrDefault(m => m.OTHER_BRANCH_ID == id);
                if (otherBranch == null)
                {
                    return HttpNotFound();
                }
                bindBranch();


                //Get Select Bank
                IEnumerable<SelectListItem> otherbank = db.OtherBanks.Where(b => b.Active == true)
                          .Select(f => new SelectListItem()
                          {
                              Value = f.OTHER_BANK_ID.ToString(),
                              Text = f.BANK_NAME
                          });
                ViewBag.OtherBank = otherbank;

                //Get Select Branch               
                var bankcode = db.OtherBanks.Where(a => a.OTHER_BANK_ID == otherBranch.OTHER_BANK_ID).FirstOrDefault();
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(bankcode.BANK_CODE);
                IEnumerable<SelectListItem> branch = branchlst.OtherBranchList
                           .Select(f => new SelectListItem()
                           {
                               Value = f.BR_Name,
                               Text = f.BR_Name
                           });
                ViewBag.Branch = branch;

                //For ChargesCode
                ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                cinfo = CRInquiry.OtherBankChargeRateInquiry();

                IEnumerable<SelectListItem> chargecode = cinfo.ChargeCodeInfo
                           .Select(f => new SelectListItem()
                           {
                               Value = f.ChargeCode,
                               Text = f.ChargeCode
                           });
                ViewBag.ChargeCode = chargecode;



                if (otherBranch == null)
                {
                    return HttpNotFound();
                }
                return View(otherBranch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: OtherBranch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OTHER_BRANCH_ID,OTHER_BANK_ID,BR_CODE,BR_SHORT_NAME,BR_NAME,FI_CODE,CR_BR_CODE,CHARGES_CODE,BR_CITY_CODE,BR_STATE_CODE,Address")] OtherBranch otherBranch)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //Duplicate OtherBranch Code
                    if (db.OtherBranches.Where(b => b.BR_CODE == otherBranch.BR_CODE && b.OTHER_BANK_ID == otherBranch.OTHER_BANK_ID && b.OTHER_BRANCH_ID != otherBranch.OTHER_BRANCH_ID && b.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("BR_NAME", "Duplicate Branch Code");
                        bindBranch();
                        goto dropdown;
                    }
                    OtherBank otherBank = db.OtherBanks.Find(otherBranch.OTHER_BANK_ID);
                    if (otherBank == null)
                    {
                        return null;
                    }
                    OtherBranchInfoList branchlist = bankinfo.OtherBranchInfoInquiry(otherBank.BANK_CODE);                   

                    var BranchDB = branchlist.OtherBranchList.Where(c => c.BR_CODE == otherBranch.BR_CODE).FirstOrDefault();

                    var result = from b in db.OtherBranches
                                 where b.Active == true
                                 select b;
                    var branchdb = result.SingleOrDefault(m => m.OTHER_BRANCH_ID == otherBranch.OTHER_BRANCH_ID);
                    //update field what u want to be allowed to edit  (from formdata)
                    branchdb.OTHER_BANK_ID = otherBranch.OTHER_BANK_ID;
                    branchdb.Address = otherBranch.Address;
                    // try to save at core data
                    branchdb.BR_CODE = BranchDB.BR_CODE;
                    branchdb.BR_SHORT_NAME = BranchDB.br_short_name; 
                    branchdb.BR_NAME = BranchDB.BR_Name;
                    branchdb.FI_CODE = BranchDB.FI_CODE;
                    branchdb.CR_BR_CODE = BranchDB.CR_BR_CODE; 
                    branchdb.CHARGES_CODE = otherBranch.CHARGES_CODE; 
                    branchdb.BR_CITY_CODE = BranchDB.br_city_code;
                    branchdb.BR_STATE_CODE = BranchDB.br_state_code;

                    if (branchdb == null)
                    {
                        return HttpNotFound();
                    }
                    db.Entry(branchdb).State = EntityState.Modified;
                    db.SaveChanges();
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }

                dropdown:
                IEnumerable<SelectListItem> otherbank = db.OtherBanks.Where(b => b.Active == true)
                          .Select(f => new SelectListItem()
                          {
                              Value = f.OTHER_BANK_ID.ToString(),
                              Text = f.BANK_NAME
                          });
                ViewBag.OtherBank = otherbank;

                //Get Select Branch               
                var bankcode = db.OtherBanks.Where(a => a.OTHER_BANK_ID == otherBranch.OTHER_BANK_ID).FirstOrDefault();
                OtherBranchInfoList branchlst = bankinfo.OtherBranchInfoInquiry(bankcode.BANK_CODE);
                IEnumerable<SelectListItem> branch = branchlst.OtherBranchList
                           .Select(f => new SelectListItem()
                           {
                               Value = f.BR_Name,
                               Text = f.BR_Name
                           });
                ViewBag.Branch = branch;

                //For ChargesCode
                ChargeRateInquiry CRInquiry = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                cinfo = CRInquiry.OtherBankChargeRateInquiry();

                IEnumerable<SelectListItem> chargecode = cinfo.ChargeCodeInfo
                           .Select(f => new SelectListItem()
                           {
                               Value = f.ChargeCode,
                               Text = f.ChargeCode
                           });
                ViewBag.ChargeCode = chargecode;
                return View(otherBranch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
      
        // GET: OtherBranch/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OtherBranch otherBranch = db.OtherBranches.Find(id);
                if (otherBranch == null)
                {
                    return HttpNotFound();
                }
                return View(otherBranch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: OtherBranch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {               
                OtherBranch otherBranch = db.OtherBranches.Find(id);
                otherBranch.Active = false;
                db.OtherBranches.Remove(otherBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
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
