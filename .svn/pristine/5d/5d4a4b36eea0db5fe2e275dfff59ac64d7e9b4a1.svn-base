using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using Microsoft.AspNet.Identity;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class ExchangeRateController : Controller
    {
       
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ExchangeRateController";
        EventLogController log = new EventLogController();
        // GET: ExchangeRate
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //get last datetime from db           
            try
            {
                var exchange = (from x in db.ExchangeRates
                                group x by x.fxdCurrencyCode into g
                                select g.OrderByDescending(t => t.RateListDate).FirstOrDefault()).OrderBy(t => t.id);

                return View(exchange.ToList());
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
     
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //Get the value from database and then set it to ViewBag to pass it View
                IEnumerable<SelectListItem> items = db.Currencies.Where(c => c.Code != "MMK").Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Code

                });
                ViewBag.Currency = items;
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ExchangeRate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fxdCurrencyCode,BUY_RATE,SELL_RATE")] ExchangeRate exchangeRate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    exchangeRate.varCurrencyCode = "MMK";
                    exchangeRate.RateListDate = DateTime.Now;
                    db.ExchangeRates.Add(exchangeRate);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, exchangeRate.ToString());
                    return RedirectToAction("Index");
                }
                return View(exchangeRate);
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
