using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using System.Net;
using PagedList;
using System.Web.UI.WebControls;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class EventLogController : Controller
    {
        private AdminLogDBContext db = new AdminLogDBContext();

        //for general, such as login, change password
        [NonAction]
        public void Log(string userID,string ControllerName, string ActionName, string desc, string message = null)
        {
            AdminLog log = new AdminLog();
            //default values
            if (userID.Length > 0)
            {
                log.UserID = userID;
            }
            else
            {
                log.UserID = "";
            }
            log.LogDateTime = DateTime.Now;
            log.LogType = "info";

            log.Desc = desc;
            log.Detail = message;
            log.Controller = ControllerName;
            log.Action = ActionName;
            db.AdminLogs.Add(log);
            db.SaveChanges();
        }
        //for create(type=1), update(2), delete(3)
        [NonAction]
        public void Info(string userid,string ControllerName, string ActionName, int type, string detail = null) 
        {
            AdminLog log = new AdminLog();
            //default values
            if (userid.Length > 0)
            {
                log.UserID = userid;
            }
            else
            {
                log.UserID = "";
            }
            log.LogDateTime = DateTime.Now;
            log.LogType = "info";

            //parameter values
            //Desc by type parameter
            string Desc = "";
            switch (type)
            {
                case 1: Desc = "Create Successfully"; break;
                case 2: Desc = "Update Successfully"; break;
                case 3: Desc = "Delete Successfully"; break;
            }
            log.Desc = Desc;
            log.Controller = ControllerName;
            log.Action = ActionName;
            log.Detail = detail;//changed information
            db.AdminLogs.Add(log);
            db.SaveChanges();
        }
        //for error
        [NonAction]
        public void Error(string userid, string ControllerName, string ActionName, Exception ex, string RequestData = null)
        {
            AdminLog log = new AdminLog();
            //default values
            if (userid.Length > 0)
            {
                log.UserID = userid;
            }
            else
            {
                log.UserID = "";
            }
            
            log.LogDateTime = DateTime.Now;
            log.LogType = "error";
            log.Desc = "System Error";

            //parameter values
            log.Detail = ex.Message;
            log.Controller = ControllerName;
            log.Action = ActionName;
            // Getting line no
            var st = new StackTrace(ex, true);
            var line = st.GetFrame(st.FrameCount - 1).GetFileLineNumber();
            log.LineNo = line;
            log.RequestData = RequestData;
            db.AdminLogs.Add(log);
            db.SaveChanges();
        }

        //Get error list 
        public ActionResult Index()
        {
            //log type dropdown list
            var listItems = new List<ListItem> {
        new ListItem { Text = "error", Value = "Error" },
        new ListItem { Text = "info", Value = "Info" }
            };
            ViewBag.LogList = listItems;

            
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
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var LogType = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var txtController = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var txtAction = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var txtDesc = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
           

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            int count = db.AdminLogs.OrderBy(e=>e.ID).Count();
            List<AdminLog> admlog = db.AdminLogs.ToList();
            IEnumerable<AdminLog> sortadmlog = admlog.OrderByDescending(e => e.LogDateTime).ToList();

            //// for search function
            if (!(string.IsNullOrEmpty(LogType)))
            {
                sortadmlog = sortadmlog.Where(a => a.LogType == LogType.ToString()).ToList();
            }           
            if(!(string.IsNullOrEmpty(txtController)))
            {
                sortadmlog = sortadmlog.Where(s => s.Controller.ToLower().Contains(txtController.ToLower()));
            }
            if(!(string.IsNullOrEmpty(txtAction)))
            {
                sortadmlog = sortadmlog.Where(s => s.Action.ToLower().Contains(txtAction.ToLower()));
            }
            if(!(string.IsNullOrEmpty(txtDesc)))
            {
                sortadmlog = sortadmlog.Where(s => s.Desc.ToLower().Contains(txtDesc.ToLower()));
            }
            
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                sortadmlog = sortadmlog.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = sortadmlog.Count();
            var data = sortadmlog.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
               return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminLog admlog = db.AdminLogs.Find(id);
            if (admlog == null)
            {
                return HttpNotFound();
            }
            return View(admlog);
        }

    }
}