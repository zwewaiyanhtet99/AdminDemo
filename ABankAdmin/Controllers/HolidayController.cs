using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class HolidayController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "HolidayController";
        EventLogController log = new EventLogController();
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        // GET: Holiday

        #region Index
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            var FinacialYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            HolidayCommonRequestModel commonrequestModel = new HolidayCommonRequestModel();
            commonrequestModel.FinancialYear = FinacialYear;

            var holiday = (from h in db.Holidays
                           where h.FinancialYear == FinacialYear && h.DEL_FLAG == false
                           select new
                           {
                               ID = h.ID,
                               HolidayName = h.HolidayName,
                               HolidayDate = h.HolidayDate,
                               FinancialYear = h.FinancialYear
                           })
                           .Select(holidayModel => new Holidays
                           {
                               ID = holidayModel.ID,
                               HolidayName = holidayModel.HolidayName,
                               HolidayDate = holidayModel.HolidayDate,
                               FinancialYear = holidayModel.FinancialYear
                           }).ToList();
            HolidayList holidayList = new HolidayList
            {
                Holidays = holiday
            };
            Session["holidaystring"] = holidayList;
            Session["comonrequeststring"] = commonrequestModel;
            ViewBag.MessageType = "MI";
            return View();
           // return Json(new { messageType = "MI", message = "Success", data = holiday }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        // GET: Holiday/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
       

        // POST: Holiday/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HolidayVM holidayvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            try
            {
                if (ModelState.IsValid)
                {
                    Holiday holiday = new Holiday();
                    holiday.HolidayName = holidayvm.HolidayName;
                    holiday.HolidayDate = DateTime.ParseExact(holidayvm.HolidayDate, "dd-MM-yyyy", null);
                    //HolidayName duplicate 
                    /*if (db.Holidays.Where(c => c.HolidayName == holiday.HolidayName && c.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("HolidayName", "Duplicate Holiday Name!");
                        return View(holidayvm);
                    }*/
                    //HolidayName duplicate 
                    if (db.Holidays.Where(c => DbFunctions.TruncateTime(c.HolidayDate) == holiday.HolidayDate.Date && c.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("HolidayDate", "Duplicate Holiday Date!");
                        return View(holidayvm);
                    }
                    string substring = holidayvm.HolidayDate.Substring(6, 4);
                    holiday.FinancialYear = Convert.ToInt32(substring);
                    holiday.CreatedDate = DateTime.Now;
                    holiday.CreatedUserId = User.Identity.GetUserId();
                    holiday.DEL_FLAG = false;
                    db.Holidays.Add(holiday);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 1, holiday.ToString());
                    return RedirectToAction("Index");
                }
                return View(holidayvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Search
        [HttpPost]
        public ActionResult Search(string reqModel)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            var FinacialYear = Convert.ToInt32(reqModel);
            HolidayCommonRequestModel commonrequestModel = new HolidayCommonRequestModel();
            commonrequestModel.FinancialYear = FinacialYear;

            var holiday = (from h in db.Holidays
                           where h.FinancialYear == FinacialYear && h.DEL_FLAG == false
                           select new
                           {
                               ID = h.ID,
                               HolidayName = h.HolidayName,
                               HolidayDate = h.HolidayDate,
                               FinancialYear = h.FinancialYear
                           })
                           .Select(holidayModel => new Holidays
                                   {
                                       ID = holidayModel.ID,
                                       HolidayName = holidayModel.HolidayName,
                                       HolidayDate = holidayModel.HolidayDate,
                                       FinancialYear = holidayModel.FinancialYear
                                   }).ToList();
            HolidayList holidayList = new HolidayList
            {
                Holidays = holiday
            };
            Session["holidaystring"] = holidayList;
            Session["comonrequeststring"] = commonrequestModel;

            return Json(new { messageType = "MI", message = "Success", data = holiday }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region HolidayDataTableList
        public ActionResult HolidayDataTableList()
        {
            HolidayCommonRequestModel commonrequestmodel = new HolidayCommonRequestModel();
            HolidayList holidayList = ((HolidayList)Session["holidaystring"]);
            commonrequestmodel = ((HolidayCommonRequestModel)Session["comonrequeststring"]);
            List<EventList> events = new List<EventList>();
            for (int i = 0; i < holidayList.Holidays.Count; i++)
            {
                var item = holidayList.Holidays[i];
                EventList itemCalendar = new EventList();
                itemCalendar.title = item.HolidayName;
                itemCalendar.start = item.HolidayDate == null ? "" : Convert.ToDateTime(item.HolidayDate).ToString("yyyy-MM-dd");
                itemCalendar.extendedProps.ID = item.ID;
                itemCalendar.extendedProps.FinancialYear = item.FinancialYear;
                events.Add(itemCalendar);
            }
            holidayList.FullCalendar.EventLists = events;
            holidayList.FullCalendar.validRange.start = new DateTime(commonrequestmodel.FinancialYear, 1, 1).Date.ToString("yyyy-MM-dd");
            holidayList.FullCalendar.validRange.end = new DateTime(commonrequestmodel.FinancialYear, 12, 31).AddDays(1).Date.ToString("yyyy-MM-dd");
            holidayList.FullCalendar.validRange.initialdate = new DateTime(commonrequestmodel.FinancialYear, DateTime.Now.Month, DateTime.Now.Day).Date.ToString("yyyy-MM-dd");
            return View(holidayList);
        }
        #endregion

        #region Detail
        [HttpPost]
        public ActionResult Detail(int? ID)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var holidaydetail = (from h in db.Holidays
                                     where h.ID == ID && h.DEL_FLAG == false
                                     select new
                                     {
                                         ID = h.ID,
                                         HolidayName = h.HolidayName,
                                         HolidayDate = h.HolidayDate,
                                         FinancialYear = h.FinancialYear
                                     })
                           .Select(holidayModel => new Holidays
                           {
                               ID = holidayModel.ID,
                               HolidayName = holidayModel.HolidayName,
                               HolidayDate = holidayModel.HolidayDate,
                               FinancialYear = holidayModel.FinancialYear
                           }).FirstOrDefault();

                if (holidaydetail == null)
                {
                    return HttpNotFound();
                }
                Holiday holiday = new Holiday
                {
                    ID = holidaydetail.ID,
                    HolidayName = holidaydetail.HolidayName,
                    HolidayDate = holidaydetail.HolidayDate,
                    FinancialYear = holidaydetail.FinancialYear
                };

                return View(holiday);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Edit
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
                
                var holiday = (from h in db.Holidays
                                     where h.ID == id && h.DEL_FLAG == false
                               select new
                                     {
                                         ID = h.ID,
                                         HolidayName = h.HolidayName,
                                         HolidayDate = h.HolidayDate,
                                         FinancialYear = h.FinancialYear
                                     })
                           .Select(holidayModel => new Holidays
                           {
                               ID = holidayModel.ID,
                               HolidayName = holidayModel.HolidayName,
                               HolidayDate = holidayModel.HolidayDate,
                               FinancialYear = holidayModel.FinancialYear
                           }).FirstOrDefault();

                if (holiday == null)
                {
                    return HttpNotFound();
                }
                HolidayVM holidayvm = new HolidayVM
                {
                    ID = holiday.ID,
                    HolidayName = holiday.HolidayName,
                    HolidayDate = holiday.HolidayDate.ToString("dd-MM-yyyy")
                };
                return View(holidayvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Currency/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HolidayVM holidayVM)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Holiday holiday = new Holiday();
                    holiday.ID = holidayVM.ID;
                    holiday.HolidayName = holidayVM.HolidayName;
                    holiday.HolidayDate = DateTime.ParseExact(holidayVM.HolidayDate, "dd-MM-yyyy", null);
                    //code duplicate
                    if (db.Holidays.Where(h => h.HolidayName == holiday.HolidayName && h.DEL_FLAG == false && h.ID != holiday.ID).Count() > 0)
                    {
                        ModelState.AddModelError("HolidayName", "Duplicate Holiday Name!");
                        return View(holidayVM);
                    }
                    if (db.Holidays.Where(h => DbFunctions.TruncateTime(h.HolidayDate) == holiday.HolidayDate.Date && h.DEL_FLAG == false && h.ID != holiday.ID).Count() > 0)
                    {
                        ModelState.AddModelError("HolidayDate", "Duplicate Holiday Date!");
                        return View(holidayVM);
                    }

                    var result = from s in db.Holidays
                                 where s.DEL_FLAG == false
                                 select s;
                    var holidaydetail = result.SingleOrDefault(m => m.ID == holiday.ID);

                    if (holidaydetail == null)
                    {
                        return HttpNotFound();
                    }
                    holidaydetail.HolidayName = holiday.HolidayName;
                    holidaydetail.HolidayDate = holiday.HolidayDate;
                    string substring = holidayVM.HolidayDate.Substring(6, 4);
                    holidaydetail.FinancialYear = Convert.ToInt32(substring);
                    holidaydetail.UpdatedDate = DateTime.Now;
                    holidaydetail.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(holidaydetail).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(holidayVM);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion

        #region Delete
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
                Holiday holiday = db.Holidays.Find(id);
                if (holiday == null)
                {
                    return HttpNotFound();
                }
                return View(holiday);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
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
                Holiday holiday = db.Holidays.Find(id);
                holiday.DEL_FLAG = true;
                holiday.UpdatedDate = DateTime.Now;
                holiday.UpdatedUserId = User.Identity.GetUserId();
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        #endregion
    }
}