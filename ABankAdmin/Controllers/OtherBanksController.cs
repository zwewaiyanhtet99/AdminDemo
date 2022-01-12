using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;


namespace ABankAdmin.Controllers
{
    public class OtherBanksController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "OtherBanksController";
        EventLogController log = new EventLogController();
        string connectionstring = ConstantValues.Connection_String;

        // GET: OtherBanks
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.OtherBanks.ToList());
        }

        //Post/OtherBanks/Index
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
            var bankname = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var shortname = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var bankcode = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var otherbanks = (from o in db.OtherBanks
                           where o.Active == true
                           select o);

            //foreach (var c in utilities.ToList())
            //{
            //    var rule = db.Rules.Where(t => t.Code == c.DiscountCode).FirstOrDefault();
            //    c.DiscountCode = rule == null ? "" : rule.Description;

            //}
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                otherbanks = otherbanks.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = otherbanks.Count();
            var data = otherbanks.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: OtherBanks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherBank otherBank = db.OtherBanks.Find(id);
            if (otherBank == null)
            {
                return HttpNotFound();
            }
            return View(otherBank);
        }

        // GET: OtherBanks/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                getOtherBank(); // bind Bank Name
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: OtherBanks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OTHER_BANK_ID,BANK_NAME,SHORT_NAME,BANK_CODE,IsACH")] OtherBank otherBank)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //get bank name by bank code
                    OtherBankInformationInquiry ob = new OtherBankInformationInquiry();
                    OtherBankInfoList oblist = ob.OtherBankInfoInquiry();
                    var otherbankinfo = oblist.OtherBankList.Where(x => x.bank_code == otherBank.BANK_CODE & x.del_flg == "N").FirstOrDefault();
                    //bank code duplicate
                    if (db.OtherBanks.Where(c => /*c.OTHER_BANK_ID != otherBank.OTHER_BANK_ID &&*/ c.BANK_CODE == otherBank.BANK_CODE && c.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("BANK_NAME", "Duplicate Bank Name!");
                        getOtherBank();
                        return View(otherBank);
                    }

                    otherBank.BANK_CODE = otherbankinfo.bank_code.ToString();
                    otherBank.BANK_NAME = otherbankinfo.bank_name.ToString();
                    otherBank.SHORT_NAME = otherbankinfo.bank_short_name.ToString();
                    //otherBank.IsACH = otherBank.IsACH;
                    otherBank.CreatedDate = DateTime.Now;
                    otherBank.CreatedUserId = User.Identity.GetUserId();
                    otherBank.Active = true;
                    db.OtherBanks.Add(otherBank);
                    db.SaveChanges();
                    // save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                    getOtherBank();
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

            return View(otherBank);
        }

        // GET: OtherBanks/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                OtherBank otherBank = db.OtherBanks.Find(id);
                //from core
                OtherBankInformationInquiry ob = new OtherBankInformationInquiry();
                OtherBankInfoList oblist = ob.OtherBankInfoInquiry();
                var otherbankinfo = oblist.OtherBankList.Where(x => x.del_flg == "N" & x.bank_code == otherBank.BANK_CODE).FirstOrDefault();
                List<OtherBank> otherbank = new List<OtherBank>();
                
                otherBank.BANK_CODE = otherbankinfo.bank_code;
                otherBank.BANK_NAME = otherbankinfo.bank_name;
                otherBank.SHORT_NAME = otherbankinfo.bank_short_name;

                getOtherBank();


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (otherBank == null)
                {
                    return HttpNotFound();
                }
                return View(otherBank);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: OtherBanks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OTHER_BANK_ID,BANK_NAME,SHORT_NAME,BANK_CODE,IsACH")] OtherBank otherBank)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //bank code duplicate
                    if (db.OtherBanks.Where(c => c.OTHER_BANK_ID != otherBank.OTHER_BANK_ID &&c.BANK_CODE==otherBank.BANK_CODE && c.Active ==true).Count() > 0)
                    {
                        ModelState.AddModelError("BANK_NAME", "Duplicate Bank Name!");
                        getOtherBank();
                        return View(otherBank);
                    }

                    OtherBank otherbankdb = db.OtherBanks.Find(otherBank.OTHER_BANK_ID);
                    otherbankdb.BANK_NAME = otherBank.BANK_NAME;
                    otherbankdb.SHORT_NAME = otherBank.SHORT_NAME;
                    otherbankdb.BANK_CODE = otherBank.BANK_CODE;
                    otherbankdb.IsACH = otherBank.IsACH;
                    otherbankdb.UpdatedDate = DateTime.Now;
                    otherbankdb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(otherbankdb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Update successfully.");
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            return View(otherBank);
        }

        // GET: OtherBanks/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OtherBank otherBank = db.OtherBanks.Find(id);
                if (otherBank == null)
                {
                    return HttpNotFound();
                }
                return View(otherBank);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: OtherBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                OtherBank otherBank = db.OtherBanks.Find(id);
                otherBank.Active = false;
                otherBank.UpdatedDate = DateTime.Now;
                otherBank.UpdatedUserId = User.Identity.GetUserId();
                //db.OtherBanks.Remove(otherBank);
                db.SaveChanges();
                //save log
                log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Update successfully.");
                return RedirectToAction("Index");
            }
            catch(Exception ex)
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

        [NonAction]
        [Obsolete]
        public void getOtherBank()
        {

            try
            {
                ChargeRateInquiry c = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();

                OtherBankChargeRateInfo o = new OtherBankChargeRateInfo();
                OtherBankInformationInquiry ob = new OtherBankInformationInquiry();
                OtherBankInfoList lstinfo = new OtherBankInfoList();

                try
                {
                    lstinfo = ob.OtherBankInfoInquiry();

                    if (lstinfo == null)
                    {
                        string lstinfoObj = Newtonsoft.Json.JsonConvert.SerializeObject(lstinfo);
                        log.Log(controllerName, "OtherBankChargeRateInquiry", "ChargeRateInfo", $"ChargeRateInfo is\n{lstinfoObj}");
                    }

                    //Other Bank Info
                    List<SelectListItem> obinfo = new List<SelectListItem>();
                    obinfo.Add(new SelectListItem { Text = "--Select Other Bank Name--", Value = "" });
                    obinfo.Add(new SelectListItem { Text = "KBZ", Value = "1" });
                    obinfo.Add(new SelectListItem { Text = "AYA", Value = "2" });
                    obinfo.Add(new SelectListItem { Text = "MAB", Value = "3" });
                    obinfo.Add(new SelectListItem { Text = "CB", Value = "4" });
                    if (lstinfo.OtherBankList != null && lstinfo.OtherBankList.Count > 0)
                    {
                        foreach (var m in lstinfo.OtherBankList)
                        {
                            obinfo.Add(new SelectListItem { Text = $"{m.bank_name}", Value = m.bank_name });
                        }
                    }
                    ViewBag.OtherBankInfo= obinfo;

                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, "RemittanceChargeRateInquiry", ex1);
                }
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "bindRemittanceRate", ex);
            }
        }

        [HttpPost]
        public JsonResult getOtherBankinfo(string name)
        {
            try
            {
                OtherBankInformationInquiry ob = new OtherBankInformationInquiry();
                OtherBankInfoList oblist = ob.OtherBankInfoInquiry();
                OtherBank otherbank = new OtherBank();
                if (!string.IsNullOrEmpty(name))
                {
                    if (oblist.OtherBankList != null && oblist.OtherBankList.Count > 0)
                    {
                        //var obank = db.OtherBanks.Where(x => x.BANK_CODE == code & x.Active == true).FirstOrDefault();
                        var obank = oblist.OtherBankList.Where(x => x.bank_name == name & x.del_flg=="N").FirstOrDefault();
                        if (obank == null) 
                        {
                            //save error log
                            log.Error(User.Identity.GetUserId(), controllerName, "getOtherBankinfo", null);
                            return null;
                        }
                        otherbank.BANK_NAME = obank.bank_name;
                        otherbank.BANK_CODE = obank.bank_code;
                        otherbank.SHORT_NAME = obank.bank_short_name;
                    }
                }
                return Json(otherbank, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //save log
                log.Error(User.Identity.GetUserId(), controllerName, "getOtherBankinfo", ex);
                return null;
            }
        }
    }
}
