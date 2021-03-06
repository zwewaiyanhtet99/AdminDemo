using ABankAdmin.Models;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CurrencyController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CurrencyController";
        EventLogController log = new EventLogController();
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        // GET: Currency
        public ActionResult Index()
        {
            return View();           
        }
        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();

            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            //var city = db.Cities.ToList();
            var currency = (from s in db.Currencies
                        where s.DEL_FLAG == "0"
                        select s);

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                currency = currency.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = currency.Count();
            var data = currency.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public void bindChargesCode()
        {
            try
            {
                ChargeRateInquiry c = new ChargeRateInquiry();

                #region Get Currency ChargeCode For Retail
                ChargeRateInfo cinfo = new ChargeRateInfo();
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select ChargesCode--", Value = "" });
                items.Add(new SelectListItem { Text = "RC001", Value = "1" });
                items.Add(new SelectListItem { Text = "RC002", Value = "2" });
                items.Add(new SelectListItem { Text = "RC003", Value = "3" });
                items.Add(new SelectListItem { Text = "RC004", Value = "4" });
                items.Add(new SelectListItem { Text = "RC005", Value = "5" });
                try
                {
                    cinfo = c.FCYChargeRateInquiry();
                    if (cinfo.ChargeCodeInfo == null)
                    {
                        string cinfoObj = Newtonsoft.Json.JsonConvert.SerializeObject(cinfo);
                        log.Log(controllerName, "FCYChargeRateInquiry", "ChargeRateInfo", $"ChargeRateInfo is\n{cinfoObj}");
                    }
                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, "FCYChargeRateInquiry", ex1);
                }

                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        if (m.ChargeCode.StartsWith("EBCCY"))
                        {
                            items.Add(new SelectListItem { Text = $"{m.ChargeCode}", Value = m.ChargeCode }); /*{(m.FixedAmt == "0" ? string.Empty : '(' + m.FixedAmt + ')')} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}"*/
                        }
                    }
                }
                #endregion

                #region Get Currency ChargeCode For Corporate
                ChargeRateInfo cinfoForCorporate = new ChargeRateInfo();
                List<SelectListItem> citems = new List<SelectListItem>();//for corporate charges code
                citems.Add(new SelectListItem { Text = "--Select Corporate ChargesCode--", Value = "" });
                citems.Add(new SelectListItem { Text = "CR001", Value = "1" });
                citems.Add(new SelectListItem { Text = "CR002", Value = "2" });
                citems.Add(new SelectListItem { Text = "CR003", Value = "3" });
                citems.Add(new SelectListItem { Text = "CR004", Value = "4" });
                citems.Add(new SelectListItem { Text = "CR005", Value = "5" });
                try
                {
                    cinfoForCorporate = c.FCYChargeRateInquiryForCorporate();
                    if (cinfoForCorporate.ChargeCodeInfo == null)
                    {
                        string cinfoObj = Newtonsoft.Json.JsonConvert.SerializeObject(cinfoForCorporate);
                        log.Log(controllerName, "FCYChargeRateInquiryForCorporate", "ChargeRateInfo", $"ChargeRateInfo is\n{cinfoObj}");
                    }
                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, "FCYChargeRateInquiryForCorporate", ex1);
                }

                if (cinfoForCorporate.ChargeCodeInfo != null && cinfoForCorporate.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfoForCorporate.ChargeCodeInfo)
                    {
                        if (m.ChargeCode.StartsWith("COCCY"))
                        {
                            citems.Add(new SelectListItem { Text = $"{m.ChargeCode}", Value = m.ChargeCode }); /*{(m.FixedAmt == "0" ? string.Empty : '(' + m.FixedAmt + ')')} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}"*/
                        }
                    }
                }
                #endregion

                ViewBag.ChargesCode = items;
                ViewBag.C_ChargesCode = citems;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "bindChargesCode", ex);
            }
        }
        // GET: Currency/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                bindChargesCode();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Currency/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Description,ChargesCode,C_ChargesCode")] Currency currency)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //checking validation
                    //currency format eg. MMK, USD, 3 Captical letters
                    Regex rgx = new Regex("^[A-Z]{3}$");
                    if (!rgx.IsMatch(currency.Code))
                    {
                        ModelState.AddModelError("Code", "Invalid Currency Code!");
                        bindChargesCode();
                        return View(currency);
                    }
                    //code duplicate
                    if (db.Currencies.Where(c => c.Code == currency.Code && c.DEL_FLAG == "0").Count() > 0)
                    {
                        ModelState.AddModelError("Code", "Duplicate Currency Code!");
                        bindChargesCode();
                        return View(currency);
                    }
                    currency.CreatedDate = DateTime.Now;
                    currency.CreatedUserId = User.Identity.GetUserId();
                    currency.DEL_FLAG = "0";
                    db.Currencies.Add(currency);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, currency.ToString());
                    return RedirectToAction("Index");
                }
                bindChargesCode();
                return View(currency);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Currency/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Currencies
                             where s.DEL_FLAG == "0"
                             select s;
                Currency currency = result.SingleOrDefault(m => m.ID == id);
                if (currency == null)
                {
                    return HttpNotFound();
                }
                bindChargesCode();
                return View(currency);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Currency/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Description,ChargesCode,C_ChargesCode")] Currency currency)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //checking validation
                    //currency format eg. MMK, USD, 3 Captical letters
                    Regex rgx = new Regex("^[A-Z]{3}$");
                    if (!rgx.IsMatch(currency.Code))
                    {
                        ModelState.AddModelError("Code", "Invalid Currency Code!");
                        bindChargesCode();
                        return View(currency);
                    }
                    //code duplicate
                    if (db.Currencies.Where(c => c.Code == currency.Code && c.DEL_FLAG == "0" && c.ID != currency.ID).Count() > 0)
                    {
                        ModelState.AddModelError("Code", "Duplicate Currency Code!");
                        bindChargesCode();
                        return View(currency);
                    }
                    var result = from s in db.Currencies
                                 where s.DEL_FLAG == "0"
                                 select s;
                    var cur = result.SingleOrDefault(m => m.ID == currency.ID);
                    if (cur == null)
                    {
                        return HttpNotFound();
                    }
                    cur.Code = currency.Code;
                    cur.Description = currency.Description;
                    cur.ChargesCode = currency.ChargesCode;
                    cur.C_ChargesCode = currency.C_ChargesCode;
                    cur.UpdatedDate = DateTime.Now;
                    cur.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(cur).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                bindChargesCode();
                return View(currency);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Currency/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Currency currency = db.Currencies.Find(id);
                if (currency == null)
                {
                    return HttpNotFound();
                }
                return View(currency);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Currency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Currency currency = db.Currencies.Find(id);
                currency.DEL_FLAG = "1";
                currency.UpdatedDate = DateTime.Now;
                currency.UpdatedUserId = User.Identity.GetUserId();
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid,controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
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
