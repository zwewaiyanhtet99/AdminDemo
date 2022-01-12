using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AbankAdminAPI;
using Newtonsoft.Json;
using ABankAdmin.Models;
using ABankAdmin.Core.Utils;
using Microsoft.AspNet.Identity;

namespace ABankAdmin.Controllers
{

    public class HomeController : Controller
    {

        private AdminDBContext db = new AdminDBContext();
        private EventLogController log = new EventLogController();
        public ActionResult Index()
        {
            if (TempData["message"] != null && TempData["message"].ToString() != "")
                ViewBag.message = TempData["message"];
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //getting Branch list
        public IEnumerable<SelectListItem> GetBranchSelectList(string callerMethod)
        {
            try
            {
                List<Branch> lstbranch = db.Branches.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> branches = lstbranch.Select(b => new SelectListItem
                {
                    Value = b.id.ToString(),
                    Text = b.name

                });
                return branches;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "HomeController", "GetBranchSelectList", ex, $"called from {callerMethod}");
                return null;
            }
        }
    }
}