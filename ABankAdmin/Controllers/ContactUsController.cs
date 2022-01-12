using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;

namespace ABankAdmin.Controllers
{
    public class ContactUsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ContactUsController";
        EventLogController log = new EventLogController();
        // GET: ContactUs
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ContactUs contactus = db.Contactus.FirstOrDefault();
                return View(contactus);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.ContactUs.ToList());
        }

        //Post Contactus
        [HttpPost]
        public ActionResult Index(ContactUs contactUs)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    var contactusdb = db.Contactus.SingleOrDefault(a => a.ID == contactUs.ID);
                    contactusdb.AppName = contactUs.AppName;
                    contactusdb.Version = contactUs.Version;
                    contactusdb.About_Us_Desc = contactUs.About_Us_Desc;
                    contactusdb.Contact_Us_Desc = contactUs.Contact_Us_Desc;
                    contactusdb.Customer_Care_Phone_1 = contactUs.Customer_Care_Phone_1;
                    contactusdb.Customer_Care_Phone_2 = contactUs.Customer_Care_Phone_2;
                    contactusdb.Customer_Care_Phone_3 = contactUs.Customer_Care_Phone_3;
                    contactusdb.Contact_Bank_Mail = contactUs.Contact_Bank_Mail;
                    contactusdb.Contact_Bank_Website = contactUs.Contact_Bank_Website;
                    contactusdb.Contact_Bank_Address = contactUs.Contact_Bank_Address;
                    //default fields
                    contactusdb.UpdatedDate = DateTime.Now;
                    contactusdb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(contactusdb).State = EntityState.Modified;
                    db.SaveChanges();

                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    TempData["AlertMessage"] = "Successfully Save!";
                    return RedirectToAction("Index");
                }
                return View(contactUs);
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
    }
}