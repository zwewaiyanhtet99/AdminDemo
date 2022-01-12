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
using ABankAdmin.Core.Utils;
using System.Data.SqlClient;
using System.Globalization;

namespace ABankAdmin.Controllers
{
    public class NotificationController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "NotificationController";
        EventLogController log = new EventLogController();
        private string NotiType { get { return ConstantValues.NotiType; } }     //"Information"
        private string All { get { return "All"; } }
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Index(string userid)
        {            
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
            var title  = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var content = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var username = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;


            var notification = (from s in db.Notifications
                                   where s.ACTIVE == true & s.CONTENTTYPE==NotiType
                                   select s);

            foreach (var c in notification.ToList())
            {
                var user = db._Users.Where(t => t.USERID == c.TO_USERID && t.DEL_STATUS == false).FirstOrDefault();
                if (c.TO_USERID != All)
                {
                    c.TO_USERID = user == null ? "" : user.USERNAME;
                }
            }

            //update notification for UserName changes for querying
            notification = notification.ToList().AsQueryable();

            //SEARCHING...
            if (!string.IsNullOrEmpty(title))
            {
                notification = notification.Where(a => a.TITLE.Contains(title.Trim()));
            }
            if (!string.IsNullOrEmpty(content))
            {
                notification = notification.Where(a => a.CONTENT.Contains(content.Trim()));
            }
            if (!string.IsNullOrEmpty(username))
            {
                notification = notification.Where(a => a.TO_USERID.Contains(username.Trim()));
            }

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                notification = notification.OrderBy(sortColumn + " " + sortColumnDir);
            }          
            recordsTotal = notification.Count();
            var data = notification.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetUserName(string username)
        {
            var user = db._Users.Where(r => r.USERNAME.Contains(username))
                      .Select(r => new {userid= r.USERID, fullname = r.FULLNAME, name = r.USERNAME, nrc= r.NRC, phone = r.MOBILENO });

            return Json(user, JsonRequestBehavior.AllowGet);
        }

       // GET: Notification/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Notification newNoti = new Notification();
                newNoti.IsAll = true;
                return View(newNoti);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
}

        // POST: Notification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TITLE,CONTENT,TO_USERID,CIFIds,Category")] Notification notification)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = User.Identity.GetUserId();
            try
            {

                var user = db._Users.Where(u => u.USERNAME == notification.TO_USERID).FirstOrDefault();
                //Checking Validation For UserID
                if ((int)notification.Category == 2 && (string.IsNullOrEmpty(notification.TO_USERID) || user == null))
                {
                    ModelState.AddModelError("TO_USERID", "Please choose one User!");
                    return View(notification);
                }
                if ((int)notification.Category == 3 && notification.CIFIds == null)
                {
                    return View(notification);
                }

                if (ModelState.IsValid)
                {
                    if ((int)notification.Category == 1)
                    {
                        notification.TO_USERID = All;
                    }
                    else if ((int)notification.Category == 2)
                    {
                        notification.TO_USERID = user.USERID.ToString();
                    }
                    else if ((int)notification.Category == 3)
                    {
                        string[] cifids = notification.CIFIds.Split(',');
                        foreach (var id in cifids)
                        {
                            var cifuser = db._Users.Where(u => u.CIFID == id).OrderByDescending(y => y.ID).FirstOrDefault();
                            notification.TO_USERID = cifuser.USERID.ToString();
                            notification.ACTIVE = true;
                            notification.DATE = DateTime.Now;
                            notification.CONTENTTYPE = NotiType;
                            notification.SENDERTYPE = "2";
                            notification.CreatedDate = DateTime.Now;
                            notification.CreatedUserId = User.Identity.GetUserId();
                            db.Notifications.Add(notification);
                            db.SaveChanges();
                            //save log;
                            log.Info(getuserid, controllerName, actionName, 1, notification.ToString());
                        }
                    }

                    if ((int)notification.Category == 1 || (int)notification.Category == 2)
                    {
                        notification.ACTIVE = true;
                        notification.DATE = DateTime.Now;
                        notification.CONTENTTYPE = NotiType;
                        notification.SENDERTYPE = "2";
                        notification.CreatedDate = DateTime.Now;
                        notification.CreatedUserId = User.Identity.GetUserId();
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                        //save log;
                        log.Info(getuserid, controllerName, actionName, 1, notification.ToString());
                    }

                    if ((int)notification.Category == 1 || (int)notification.Category == 2)
                    {
                        //send noti to all or by name
                        if (Send_FireBase_Notification(notification.TO_USERID, notification.TITLE, notification.CONTENT))
                        {
                            //noti send successfully
                            log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{notification.TO_USERID}\nTitle:{notification.TITLE}\nContent:{notification.CONTENT}");
                        }
                        else
                        {
                            //noti send fail
                            log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{notification.TO_USERID}");
                        }
                    }
                    else
                    {
                        string[] cifids = notification.CIFIds.Split(',');
                        foreach (var id in cifids)
                        {

                            var cifuser = db._Users.Where(u => u.CIFID == id).OrderByDescending(y => y.ID).FirstOrDefault();

                            if (Send_FireBase_Notification(cifuser.USERID.ToString(), notification.TITLE, notification.CONTENT))
                            {
                                log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{notification.TO_USERID}\nTitle:{notification.TITLE}\nContent:{notification.CONTENT}");
                            }
                            else
                            {
                                log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{notification.TO_USERID}");
                            }

                        }
                    }

                    return RedirectToAction("Index");
                }
                return View(notification);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        //send noti
        #region Noti 
        private Boolean Send_FireBase_Notification(string UserID, string NotiTitle, string NotiBody)
        {
            try
            {
                string FireBaseToken;
                //Check All or ByName
                if (UserID == All)    //If all, get api_topicname from TBL_CREDENTIAL
                {
                    //Get topic name
                    string fbtoken = db.Database.SqlQuery<string>("SELECT Value FROM TBL_CREDENTIAL " +
                        "where [Name] = 'FirebaseNoti' and [Key] = 'api_topicname' and [Status] = 1").FirstOrDefault();

                    FireBaseToken = fbtoken;
                }
                else        //if By Name (such as complain), get FireBaseToken from FirebaseToken by UserID
                {
                    string fbtoken = db.Database.SqlQuery<string>("select FirebaseToken from TBL_FIREBASE " +
                        $"where UserID = '{UserID}' " +
                        "order by convert(datetime,UpdatedDate,103) desc").FirstOrDefault();

                    FireBaseToken = fbtoken;
                }

                NotificationHelper.SendNotificationRequestModel notiModel = new NotificationHelper.SendNotificationRequestModel();
                notiModel.FireBaseToken = FireBaseToken;
                notiModel.NotiType = NotiType;
                notiModel.Title = NotiTitle;
                notiModel.MessageBody = NotiBody;
                NotificationHelper notiHelper = new NotificationHelper();
                Boolean IsSuccess = notiHelper.PushNotificationByFireBase(notiModel);

                //save log
                if (IsSuccess) 
                {
                    //noti send successfully
                    log.Log(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", $"Notification sent successfully.", $"To:{FireBaseToken}\nTitle:{NotiTitle}\nContent:{NotiBody}");
                }
                else
                {
                    //noti send fail
                    log.Log(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", $"Notification sent fail.", $"To:{FireBaseToken}");
                }

                return true;
            }
            catch (Exception ex)
            {
                //save error log
                log.Error(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", ex);
            }
            return false;
        }
        #endregion

        // GET: ADS/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Notification noti = db.Notifications.Where(n => n.ACTIVE == true && n.ID == id).FirstOrDefault();
                if(noti.TO_USERID != All)
                {
                    var user = db._Users.Where(t => t.USERID == noti.TO_USERID && t.DEL_STATUS == false).FirstOrDefault();
                    noti.TO_USERID = user == null ? "" : user.USERNAME;
                    ViewBag.FULLNAME = user.FULLNAME;
                    ViewBag.NRC = user.NRC;
                    ViewBag.PHONENO = user.MOBILENO;
                }
                ViewBag.USERID = noti.TO_USERID;
                if (noti == null)
                {
                    return HttpNotFound();
                }
                return View(noti);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ADS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Notification noti = db.Notifications.Where(n => n.ACTIVE == true && n.ID == id).FirstOrDefault();
                noti.ACTIVE = false;
                noti.UpdatedDate = DateTime.Now;
                noti.UpdatedUserId = User.Identity.GetUserId();
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
