using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{ 
    [HandleError]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult Unauthorize()
        {
            Response.StatusCode = 403;
            return View();
        }

        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult General()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}