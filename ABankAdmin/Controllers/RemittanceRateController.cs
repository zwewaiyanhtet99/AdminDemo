using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using Microsoft.AspNet.Identity;
using System.Configuration;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class RemittanceRateController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "RemittanceRateController";
        EventLogController log = new EventLogController();

        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        // GET: RemittanceRate
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                List<RemittanceRate> lstRemittanceRate = db.RemittanceRates.Where(rr => rr.Active == true).OrderBy(rr => rr.CODE).ToList();
                return View(lstRemittanceRate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
        public void bindRemittanceRate()
        {
            try
            {
                ChargeRateInquiry c = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                try
                {
                    cinfo = c.RemittanceChargeRateInquiry(connectionString);
                    if (cinfo.ChargeCodeInfo == null)
                    {
                        string cinfoObj = Newtonsoft.Json.JsonConvert.SerializeObject(cinfo);
                        log.Log(controllerName, "RemittanceChargeRateInquiry", "ChargeRateInfo", $"ChargeRateInfo is\n{cinfoObj}");
                    }
                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(),controllerName, "RemittanceChargeRateInquiry", ex1);
                }    
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "--Select RemittanceRateCode--", Value = "" });
                    if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                    {
                        foreach (var m in cinfo.ChargeCodeInfo)
                        {
                        items.Add(new SelectListItem { Text = $"{m.ChargeCode} {(m.FixedAmt == "0" ? string.Empty : '('+ m.FixedAmt +')' )} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}", Value = m.ChargeCode });
                        }
                    }
                    ViewBag.RemittanceRateCode = items;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "bindRemittanceRate", ex);
            }
        }

        // GET: RemittanceRate/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                bindRemittanceRate();
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: RemittanceRate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CODE,FROM_AMT,TO_AMT")] RemittanceRate remittanceRate)
        {
            long fromAmt = 0;
            long toAmt =0;
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    bool message = true;
                    bindRemittanceRate();
                    var from = remittanceRate.FROM_AMT.Replace(",","");
                    var to = remittanceRate.TO_AMT.Replace(",", "");
                    if (db.RemittanceRates.Where(m => m.CODE == remittanceRate.CODE && m.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("CODE", "Duplicate RemittanceRate Code!");
                        message = false;
                    }
                    fromAmt = Convert.ToInt64(from);
                    toAmt = Convert.ToInt64(to);
                    if (fromAmt > toAmt)
                    {
                        ModelState.AddModelError("TO_AMT", "To_Amount must be greater than From_Amount!");
                        message = false;
                    }
                    if (message == false)
                    {
                        return View(remittanceRate);
                    }                   
                    remittanceRate.Active = true;
                    remittanceRate.CreatedDateTime = DateTime.Now;
                    remittanceRate.CreatedUserID = User.Identity.GetUserId();
                    db.RemittanceRates.Add(remittanceRate);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, remittanceRate.ToString());  
                    return RedirectToAction("Index");
                }
                return View(remittanceRate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: RemittanceRate/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                bindRemittanceRate();
                var result = from s in db.RemittanceRates
                             where s.Active == true
                             select s;
                RemittanceRate remittanceRate = result.SingleOrDefault(m => m.ID == id);    
                if (remittanceRate == null)
                {
                    return HttpNotFound();
                }
                return View(remittanceRate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: RemittanceRate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CODE,FROM_AMT,TO_AMT,CreatedUserID,CreatedDateTime,UpdatedUserId,UpdatedDateTime,Active")] RemittanceRate remittanceRate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    bool message = true;
                    var result = from s in db.RemittanceRates
                                 where s.Active == true
                                 select s;
                    RemittanceRate remittance = result.SingleOrDefault(m => m.ID == remittanceRate.ID);
                    var from = remittanceRate.FROM_AMT.Replace(",", "");
                    var to = remittanceRate.TO_AMT.Replace(",", "");
                    long fromAmt = Convert.ToInt64(from);
                    long toAmt = Convert.ToInt64(to);
                    if (db.RemittanceRates.Where(e => e.CODE == remittanceRate.CODE && e.ID != remittance.ID && e.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("CODE", "Duplicate RemittanceRate Code!");
                        bindRemittanceRate();
                        message = false;
                    }
                    if (fromAmt > toAmt)
                    {
                        ModelState.AddModelError("TO_AMT", "To_Amount must be greater than From_Amount!");
                        message = false;
                    }
                    if (message == false)
                    {
                        return View(remittanceRate);
                    }
                    remittance.CODE = remittanceRate.CODE;
                    remittance.FROM_AMT = remittanceRate.FROM_AMT;
                    remittance.TO_AMT = remittanceRate.TO_AMT;
                    remittance.UpdatedDateTime = DateTime.Now;
                    remittance.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(remittance).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(remittanceRate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: RemittanceRate/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.RemittanceRates
                             where s.Active == true
                             select s;
                RemittanceRate remittanceRate = result.SingleOrDefault(m => m.ID == id);
                //RemittanceRate remittanceRate = db.RemittanceRates.Find(id);
                if (remittanceRate == null)
                {
                    return HttpNotFound();
                }
                return View(remittanceRate);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: RemittanceRate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                RemittanceRate remittanceRate = db.RemittanceRates.Find(id);
                remittanceRate.Active = false;
                //db.RemittanceRates.Remove(remittanceRate);
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
