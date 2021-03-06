using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CifInfoDeactivateController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CIFInfoDeactivate";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        EventLogController log = new EventLogController();
        // GET: CifInfoDeactivate
        public ActionResult Index()
        {
            try
            {
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View(db.CIFInfoModels.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? arg1)
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            int currentYear = DateTime.Now.Year;

            var user = db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status == 1 || a.Status == 6) && a.DeactivateStatus!=1 && (a.UpgradeStatus ==0 || a.UpgradeStatus==2) &&  (a.DowngradeStatus == 0 || a.DowngradeStatus == 2)).ToList().ConvertAll(u => new CIFInfoVM
            {
                PHONENO = u.PHONENO,
                NAME = u.NAME,
                USERTYPE = u.USERTYPE,
                USERTYPECODE = u.USERTYPECODE,
                EXPIREDATE = u.EXPIREDATE,
                strEXPIREDATE = String.Format("{0:MM/yy}", u.EXPIREDATE),
                NRC = u.NRC,
                Address = u.ADDRESS,
                CIFID = u.CIFID,
                ID = u.ID,
                REMARK = u.REMARK,
                Createduserid = u.CREATEDUSERID,
                DeactivateDate = Convert.ToDateTime(u.DeactivateDate),
                Status = u.Status.ToString(),
                DeactivateStatus = u.DeactivateStatus.ToString()
            });

            
            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {
                var getTempData = (from s in db.CIFInfoForTemp
                                   where
                                       s.CIFID == item.CIFID && s.DELFLAG == false && (s.Status == 3 || s.Status == 5 || s.UpgradeStatus == 1 ||
                                           s.DeactivateStatus == 1 || s.DowngradeStatus == 1)
                                   select s)
                        .SingleOrDefault();
                if (getTempData == null)
                {
                    Info = item;
                    if (item.Createduserid != null)
                    {
                        Info.Createduserid =
                            db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.DeactivateStatus == "0")
                    {
                        Info.DeactivateStatus = "Can Deactivate";
                    }
                    else if (item.DeactivateStatus == "2")
                    {
                        Info.DeactivateStatus = "Already Approved";
                    }
                    else if (item.DeactivateStatus == "3")
                    {
                        Info.DeactivateStatus = "Already Rejected";

                    }
                    userData.Add(Info);
                }

            }
            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = userData.Count();
            var data = userData.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id)
        {
            CIFInfoVM infoVm = new CIFInfoVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            infoVm = JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            string userID = User.Identity.GetUserId();
            ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
            infoVm.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();


            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CIFInfoVM infoVm)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp = JsonConvert.DeserializeObject<CIFInfoModelForTemp>(JsonConvert.SerializeObject(DbInfo));
                    DbInfoForTemp.UPDATEDUSERID= User.Identity.GetUserId();
                    DbInfoForTemp.UpdatedDate = DateTime.Now;
                    //update approve status
                    DbInfoForTemp.DeactivateStatus = 1;//request
                    DbInfoForTemp.ApproverEmail = infoVm.ApproverEmail;
                    DbInfoForTemp.UserEmail = infoVm.UserEmail;
                    #endregion
                   
                    // save log
                    string controllerName = "CIFInfoDeactivate";
                    string getuserid = User.Identity.GetUserId();
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    try
                    {
                       
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            log.Info(getuserid, controllerName, "Edit", 2, eve.Entry.Entity.GetType().Name);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.Info(getuserid, controllerName, "Edit", 2, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    
                     ViewBag.CifDeactivateInfo = "\"" + infoVm.CIFID + "\"";
                    var getBranch = (from s in db.Users where s.Id == getuserid && s.DEL_FLAG == false select s).SingleOrDefault();
                    int branchid = getBranch.BranchID;
                    IEnumerable<SelectListItem> items2 = db.Users.Where(b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 3)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.Email == infoVm.ApproverEmail),
                                        Value = c.Email.ToString(),
                                        Text = c.Email
                                    }).ToList();

                    ViewBag.Emaillist = items2;
                    List<string> mailList = new List<string>();
                    mailList.Add(infoVm.UserEmail);
                    mailList.Add(infoVm.ApproverEmail);
                    #region sendEmail
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "Edit");
                    #endregion
                    return View(infoVm);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }

        #region Deactivate TBD

        //Fetch with CIFID
        public JsonResult getCIFIDInfo(string CIFID,string iconicno)
        {
            CIFInfoVMForDeactivator rri = new CIFInfoVMForDeactivator();
            CIFInfoModel info = new CIFInfoModel();
            try
            {
                if (CIFID != "" || iconicno !="")
                {
                    if (CIFID != "")
                    {
                        info =
                            (from s in db.CIFInfoModels where s.CIFID == CIFID && s.DELFLAG == false  && (s.Status == 1 || s.Status == 6) && (s.UpgradeStatus==0 || s.UpgradeStatus==2) && s.DeactivateStatus==0 && (s.DowngradeStatus==0 || s.DowngradeStatus==2) select s)
                                .SingleOrDefault();

                    }
                    else
                    {
                        info =
                           (from s in db.CIFInfoModels where s.USERTYPECODE == iconicno && s.DELFLAG == false && (s.Status == 1 || s.Status == 6) && (s.UpgradeStatus == 0 || s.UpgradeStatus == 2 )&& s.DeactivateStatus == 0 && (s.DowngradeStatus == 0 || s.DowngradeStatus == 2) select s)
                               .SingleOrDefault();
                    }
                    if (info == null)
                    {
                        return Json(new {message = "This CIFID is not registered"}, JsonRequestBehavior.AllowGet);

                    }
                    var getTempData = (from s in db.CIFInfoForTemp
                                       where
                                           s.CIFID == CIFID && s.DELFLAG == false &&
                                           (s.Status == 3 || s.Status == 5 || s.UpgradeStatus == 1 ||
                                            s.DeactivateStatus == 1 || s.DowngradeStatus == 1)
                                       select s)
                                   .SingleOrDefault();
                    if (getTempData == null)
                    {
                        rri.PHONENO = info.PHONENO;
                        rri.NAME = info.NAME;
                        rri.USERTYPE = info.USERTYPE;
                        rri.USERTYPECODE = info.USERTYPECODE;
                        rri.EXPIREDATE = info.EXPIREDATE;
                        rri.EffectiveDate = info.EFFECTIVEDATE;
                        rri.strEXPIREDATE = String.Format("{0:MM/yy}", info.EXPIREDATE);
                        rri.strEffectiveDate = String.Format("{0:MM/yy}", info.EFFECTIVEDATE);
                        rri.NRC = info.NRC;
                        rri.Address = info.ADDRESS;
                        rri.CIFID = info.CIFID;
                        rri.ID = info.ID;
                        rri.RMName = info.RMName;
                        rri.RMID = info.RMID;
                        rri.BranchName = info.BRANCHNAME;
                        rri.Remark = info.REMARK;
                    }
                    else
                    {
                        return Json(new { message = "This CIFID is in pending stage" }, JsonRequestBehavior.AllowGet);

                    }
                    return Json(rri, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return Json(rri, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateDeactivate()
        {
            try
            {
                CIFInfoVMForDeactivator infoVm = new CIFInfoVMForDeactivator();
                if (TempData["createReturn"] == "1")
                {
                    if (Session["CIFVMForDeactivateReturn"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVMForDeactivateReturn"].ToString()))
                        {
                            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForDeactivator>(Session["CIFVMForDeactivateReturn"].ToString());


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
        public ActionResult CreateDeactivate(CIFInfoVMForDeactivator infoVm)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(infoVm.USERTYPECODE))
                    {
                        string usertypecode = (from s in db.CIFInfoModels where s.USERTYPE == infoVm.USERTYPECODE && s.DELFLAG == false select s.USERTYPECODE).SingleOrDefault();
                        if(!usertypecode.Equals(infoVm.USERTYPECODE))
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
                   
                    //save log
                    Session["CIFVMForDeactivate"] = string.Empty;//To genereate PDF
                    Session["CIFVMForDeactivate"] = JsonConvert.SerializeObject(infoVm);
                    Session["CIFVMForDeactivateReturn"] = JsonConvert.SerializeObject(infoVm);
                   return RedirectToAction("CIF_ConfirmPageForDeactivate");
                }

                return View();


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        public ActionResult CIF_ConfirmPageForDeactivate()
        {
            TempData["createReturn"] = "1";
            CIFInfoVMForDeactivator reqmodel = new CIFInfoVMForDeactivator();
            try
            {
                if (TempData["createD"] == "1")
                {
                    if (Session["CIFVMForDeactivate"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["CIFVMForDeactivate"].ToString()))
                        {
                            reqmodel = JsonConvert.DeserializeObject<CIFInfoVMForDeactivator>(Session["CIFVMForDeactivate"].ToString());
                        }
                    }
                }


                string userID = User.Identity.GetUserId();
                ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
                reqmodel.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();



            }
            catch (Exception ex)
            { throw ex; }
            return View(reqmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CIF_ConfirmPageForDeactivate(CIFInfoVMForDeactivator reqVM)
        {
            try
            {
                Session["CIFVMForDeactivate"] = string.Empty;
                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == reqVM.ID && s.DELFLAG == false select s).SingleOrDefault();
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp = JsonConvert.DeserializeObject<CIFInfoModelForTemp>(JsonConvert.SerializeObject(DbInfo));
                    DbInfoForTemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoForTemp.UpdatedDate = DateTime.Now;
                    //update approve status
                    DbInfoForTemp.DeactivateStatus = 1;//request
                    DbInfoForTemp.ApproverEmail = reqVM.ApproverEmail;
                    DbInfoForTemp.UserEmail = reqVM.UserEmail;
                    var deactivate = reqVM.strDeactivateDate.Replace("/", "-");
                    DbInfoForTemp.DeactivateDate = General.ConvertStrToDate(deactivate);
                    
                    #endregion

                    // save log
                    string controllerName = "CIFInfoDeactivate";
                    string getuserid = User.Identity.GetUserId();
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    try
                    {

                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            log.Info(getuserid, controllerName, "Edit", 2, eve.Entry.Entity.GetType().Name);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.Info(getuserid, controllerName, "Edit", 2, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    

                    //save log
                    ViewBag.CifCreateInfo = "\"" + reqVM.CIFID + "\"";
                    var getBranch = (from s in db.Users where s.Id == getuserid && s.DEL_FLAG == false select s).SingleOrDefault();
                    int branchid = getBranch.BranchID;
                    IEnumerable<SelectListItem> items2 = db.Users.Where(b => b.DEL_FLAG == false && b.BranchID == branchid && b.Email != null && b.Role == 3)
                                    .Select(c => new SelectListItem
                                    {
                                        Selected = (c.Email == reqVM.ApproverEmail),
                                        Value = c.Email.ToString(),
                                        Text = c.Email
                                    }).ToList();

                    ViewBag.Emaillist = items2;
                    List<string> mailList = new List<string>();
                    mailList.Add(reqVM.UserEmail);
                    mailList.Add(reqVM.ApproverEmail);
                    #region sendEmail
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, reqVM.CIFID, getuserid, controllerName, "CIF_ConfirmPageForDeactivate");
                    #endregion
                    return View(reqVM);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }
        #endregion
    }
}