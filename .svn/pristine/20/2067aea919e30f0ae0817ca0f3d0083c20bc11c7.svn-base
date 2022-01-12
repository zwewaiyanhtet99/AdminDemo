using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ABankAdmin.Models;
using System.Linq.Dynamic;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace ABankAdmin.Controllers
{
    public class Api_LogController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "Api_logController";
        EventLogController log = new EventLogController();
        // GET: Api_Log
        public ActionResult Index()
        {
            ViewBag.aReqMethodList = new List<SelectListItem>()
            {
                new SelectListItem{Text="FIAPI",Value="FIAPI"},
                new SelectListItem{Text="POST",Value="POST"},
                new SelectListItem{Text="SMS_GateWay",Value="SMS_GateWay"},
                new SelectListItem{Text="Test",Value="Test"}
            };
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
            var ReqMethod = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var ReqPath = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FromDate = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var ToDate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            DateTime fromDate = DateTime.Now.AddYears(-1);
            //DateTime fromDatetext = DateTime.Now;
            DateTime toDate = DateTime.Now;
            if (!String.IsNullOrEmpty(FromDate))
            {
                fromDate = DateTime.ParseExact(FromDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(ToDate))
            {
                toDate = DateTime.ParseExact(ToDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            var Api_logs = (from ApiLog in db.Api_Logs
                            where ApiLog.RequestMethod=="test"
                            select ApiLog);

            //SEARCHING...
            if (!string.IsNullOrEmpty(ReqMethod))
            {
                Api_logs = Api_logs.Where(a => a.RequestMethod.Contains(ReqMethod.Trim()));
            }
            if (!string.IsNullOrEmpty(ReqPath))
            {
                Api_logs = Api_logs.Where(a => a.RequestPath.ToLower().Contains(ReqPath.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(FromDate)|| !string.IsNullOrEmpty(ToDate))
            {
                Api_logs = Api_logs.Where(a => a.RequestDateTime >= fromDate && a.RequestDateTime <= toDate);
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                Api_logs = Api_logs.OrderBy(sortColumn + " " + sortColumnDir);
            }
            recordsTotal = Api_logs.Count();
            var data = Api_logs.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int? id) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from a in db.Api_Logs
                             select a;
                Api_Log api_Log = result.SingleOrDefault(a => a.ID == id);
                if (api_Log == null)
                {
                    return HttpNotFound();
                }
                return View(api_Log);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
            
    }
}