using System;
//using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
//using System.Net;
//using System.Net.Mime;
//using System.Text.RegularExpressions;
//using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;

namespace ABankAdmin.Controllers
{
    public class FinacleLoginController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "FinacleLoginController";
        EventLogController log = new EventLogController();

        // GET: FinacleLogin
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                FinacleLogin finacle = db.FinacleLogins.FirstOrDefault();
                return View(finacle);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }        

        [HttpPost]
        public ActionResult Index([Bind(Include = "ID,UserSchema,Host,SID,Password,Port")]FinacleLogin finacleLogin)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //Regex rgx = new Regex(@"\[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
                    //if (!rgx.IsMatch(finacleLogin.Host))
                    //{
                    //    ModelState.AddModelError("Host", "Invalid Host!");
                    //    return View("Index");
                    //}
                    var finacledb = db.FinacleLogins.SingleOrDefault(m => m.ID == finacleLogin.ID);
                    string olddata = finacledb.ID + "," + finacledb.UserSchema + "," + finacledb.Host + "," + finacledb.SID + "," + finacledb.Password + "," + finacledb.Port + "," + finacledb.UpdatedUserId + "," + finacledb.UpdatedDateTime;
                    finacledb.UserSchema = finacleLogin.UserSchema;
                    finacledb.Host = finacleLogin.Host;
                    finacledb.SID = finacleLogin.SID;
                    finacledb.Password = finacleLogin.Password;
                    finacledb.Port = finacleLogin.Port;
                    //default value
                    finacledb.UpdatedDateTime = DateTime.Now;
                    finacledb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(finacledb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2,olddata);
                    TempData["AlertMessage"] = "Successfully Save!";
                    return RedirectToAction("Index");
                }
                return View(finacleLogin);
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
