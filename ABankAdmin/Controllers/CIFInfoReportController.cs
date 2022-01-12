using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CIFInfoReportController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        EventLogController log = new EventLogController();
        string controllerName = "CIFInfoReport";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        string CSVFile_Path = ConstantValues.CSVFile_Path;
        // GET: CIFInfoReport
        public ActionResult Index()
        {
            return View();
        }

        #region New Registration Report
        public ActionResult NewRegisterationReport()
        {
            try
            {
                CIFInfoVMForReport infoVm = new CIFInfoVMForReport();
                if (TempData["createReturnReport"] == "1")
                {
                    if (Session["InfoForReport"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["InfoForReport"].ToString()))
                        {
                            infoVm= JsonConvert.DeserializeObject<CIFInfoVMForReport>(Session["InfoForReport"].ToString());

                        }
                    }
                }
                return View(infoVm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRegisterationReport(CIFInfoVMForReport infoVm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(infoVm.USERTYPECODE))
                    {
                        string usertypecode = (from s in db.CIFInfoModels where s.USERTYPE == infoVm.USERTYPECODE && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if (!usertypecode.Equals(infoVm.USERTYPECODE))
                        {
                            ModelState.AddModelError("USERTYPECODE", "THE ICONIC Number is Invalid");
                            return View(infoVm);
                        }

                    }
                    if (string.IsNullOrEmpty(infoVm.CIFID))
                    {
                        string cif = (from s in db.CIFInfoModels where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if (!cif.Equals(infoVm.CIFID))
                        {
                            ModelState.AddModelError("CIFID", "The CIFID is Invalid");
                            return View(infoVm);
                        }

                    }
                    Session["InfoForReport"] = string.Empty;//To genereate PDF
                    Session["InfoForReport"] = JsonConvert.SerializeObject(infoVm);
                    return RedirectToAction("InfoForReport");
                }

                return View();


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        public ActionResult InfoForReport()
        {
            TempData["createReturnReport"] = "1";
            CIFInfoVMForReport reqmodel = new CIFInfoVMForReport();
            try
            {
                if (Session["InfoForReport"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["InfoForReport"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<CIFInfoVMForReport>(Session["InfoForReport"].ToString());
                        }
                    }
                
                
            }
            catch (Exception ex)
            { throw ex; }
            return View(reqmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoForReport(CIFInfoVMForReport reqVM)
        {
            try
            {
                if (reqVM.IsRegister)
                {
                    return RedirectToAction("PDF", "CIFInfoModels",new { flag=true});
                }
                return RedirectToAction("PDF", "CIFInfoModels", new { flag = false });
                

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }
        #endregion
        
        
        
    }
}