using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CIFInfoDowngradeController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CIFInfoDowngrade";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        EventLogController log = new EventLogController();
        // GET: CIFInfoDowngrade
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
            //find search columns info
            var cifid = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var iconic = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);//Only show not pending register status,deactivate normal,upgrade not pending in listing
            var user = db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status == 1 || a.Status == 6) && (a.UpgradeStatus == 0 || a.UpgradeStatus == 2) && (a.DowngradeStatus != 1) && (a.DeactivateStatus == 0)).ToList().ConvertAll(u => new CIFInfoVM
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
                Status = u.Status.ToString(),
                UpgradeStatus = u.UpgradeStatus.ToString(),
                DeactivateStatus = u.DeactivateStatus.ToString(),
                DowngradeStatus = u.DowngradeStatus.ToString(),
            });
            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {
                var getType = (from s in db.UserTypeModels
                               where
                                   s.USERTYPE == item.USERTYPE && s.DEL_FLAG == false &&
                                   (s.Type == "Tier 1" || s.Type == "Tier 1.1")
                               select s)
                    .SingleOrDefault();
                if (getType != null)
                {
                    var getTempInfo = (from s in db.CIFInfoForTemp
                                       where s.CIFID == item.CIFID && s.DELFLAG == false && s.DowngradeStatus == 1
                                       select s)
                        .SingleOrDefault();
                    if (getTempInfo == null)
                    {
                        Info = item;
                        if (item.Createduserid != null)
                        {
                            Info.Createduserid =
                                db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                        }

                        if (item.DowngradeStatus == "0")
                        {
                            Info.Status = "Normal";
                        }
                        else if (item.DowngradeStatus == "2")
                        {
                            if (item.UpgradeStatus == "2")
                            {
                                Info.Status = "Normal";
                            }
                            else
                            {
                                Info.Status = "Already Approved";

                            }


                        }
                        else
                        {
                            Info.Status = "Already Rejected";
                        }

                        userData.Add(Info);
                    }



                }
            }
            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = userData.Count();
            var data = userData.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        public ActionResult CifInfoForDowngrade(int? id)
        {
            CIFInfoVMForDowngrade infoVm = new CIFInfoVMForDowngrade();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForDowngrade>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            string userID = User.Identity.GetUserId();
            ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
            infoVm.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            Session["ApproverEmailList"] = ViewBag.Emaillist;
            DateTime today = DateTime.Now;
            DateTime addDate1 = today.AddMonths(1);
            infoVm.NewEffectiveDate = addDate1;
            infoVm.strNewEffectiveDate = addDate1.ToString("MM/yy");
            infoVm.strNewExpireDate = infoVm.NewEffectiveDate.AddDays(365).ToString("MM/yy");
            string new_usertype = String.Empty;
            var getInfo =
                (from s in db.UserTypeModels where s.USERTYPE == infoVm.USERTYPE && s.DEL_FLAG == false select s.Type)
                    .SingleOrDefault();
            if (getInfo == "Tier 1")
            {
                new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 2" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

            }
            if (getInfo == "Tier 1.1")
            {
                new_usertype = (from s in db.UserTypeModels where s.Type == "Tier 2.1" && s.DEL_FLAG == false select s.USERTYPE).SingleOrDefault();

            }
            infoVm.USERTYPE = new_usertype;
            if (!string.IsNullOrEmpty(new_usertype))
            {
                infoVm.NewIconicNo = CIFInfoModelsController.GenereateCodeForIconic(new_usertype);
                if (infoVm.NewIconicNo == "0")
                {
                    ViewBag.GetCode = "Generated Iconic Number is over exceeded current Usertype Code";
                    return View();
                }

            }

            return View(infoVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CifInfoForDowngrade(CIFInfoVMForDowngrade infoVm)
        {
            TempData["createD"] = "1";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    CIFInfoModelForTemp DbInfoForTemp = new CIFInfoModelForTemp();
                    DbInfoForTemp = JsonConvert.DeserializeObject<CIFInfoModelForTemp>(JsonConvert.SerializeObject(DbInfo));
                    DbInfoForTemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoForTemp.UpdatedDate = DateTime.Now;
                    DbInfoForTemp.ApproverEmail = infoVm.ApproverEmail;
                    DbInfoForTemp.UserEmail = infoVm.UserEmail;
                    DbInfoForTemp.DowngradeStatus = 1;//Request upgrade pending
                    DbInfoForTemp.USERTYPECODE = infoVm.NewIconicNo;
                    DbInfoForTemp.USERTYPE = infoVm.USERTYPE;
                    var dd = infoVm.strNewEffectiveDate.Replace("/", "-");
                    var ddexp = infoVm.strNewExpireDate.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    infoVm.EXPIREDATE = General.ConvertStrToDateForIconic(adddayexp);
                    infoVm.EffectiveDate = General.ConvertStrToDateForIconic(addday);
                    DbInfoForTemp.EFFECTIVEDATE = infoVm.EffectiveDate;
                    DbInfoForTemp.EXPIREDATE = infoVm.EXPIREDATE;
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();
                    #endregion

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, "CifInfoForDowngrade", 2, $"{infoVm.CIFID} Downgrade is pending.");
                    ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
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
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "CifInfoForDowngrade");
                    #endregion
                    ViewBag.Emaillist = Session["ApproverEmailList"];
                    return View(infoVm);
                }


            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            return View();
        }

        //Fetch with CIFID
        public JsonResult getCIFIDInfoForDowngrade(string CIFID, string iconicno)
        {
            CIFInfoVMForDowngrade rri = new CIFInfoVMForDowngrade();
            CIFInfoModel info = new CIFInfoModel();
            try
            {
                if (CIFID != "" || iconicno != "")
                {
                    if (CIFID != "")
                    {
                        info =
                            (from s in db.CIFInfoModels where s.CIFID == CIFID && s.DELFLAG == false && s.Status == 1 && (s.UpgradeStatus == 0 || s.UpgradeStatus == 2) && s.DeactivateStatus == 0 select s)
                                .SingleOrDefault();

                    }
                    else
                    {
                        info =
                           (from s in db.CIFInfoModels where s.USERTYPECODE == iconicno && s.DELFLAG == false && (s.Status == 1 || s.Status == 6) && (s.UpgradeStatus == 0 || s.UpgradeStatus == 2) && s.DeactivateStatus == 0 select s)
                               .SingleOrDefault();
                    }
                    if (info == null)
                    {
                        return Json(new { message = "This CIFID is not registered" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(info.USERTYPE))
                        {
                            var getType =
                                (from s in db.UserTypeModels
                                 where s.USERTYPE == info.USERTYPE && s.DEL_FLAG == false && (s.Type == "Tier 1" || s.Type == "Tier 1.1")
                                 select s)
                                    .SingleOrDefault();
                            if (getType == null)
                            {
                                return Json(new { message = "This CIFID can't degrade from Tier 1 to Tier 2" }, JsonRequestBehavior.AllowGet);

                            }
                            else
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

                            }
                        }
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

        public JsonResult GetBookingListByIconicTypeForDegrade(string iconic)
        {
            try
            {
                var new_usertypeInfo = (from s in db.UserTypeModels where s.USERTYPE != iconic && s.DEL_FLAG == false && s.Type == "Tier 2" select s).SingleOrDefault();

                List<SelectListItem> item = new List<SelectListItem>();
                List<IconicBookingModel> lstlist =
                    db.IconicBookingModels.Where(
                        b => b.DEL_FLAG == false && b.InUse == false && b.BOOKINGNUMBER.StartsWith(new_usertypeInfo.USERTYPE_CODE_LIMIT.ToString()) && b.UserTypeID == new_usertypeInfo.ID).ToList();

                item.Add(new SelectListItem { Text = "Select Reserved Number", Value = "" });
                if (lstlist.Count > 0)
                {
                    foreach (var x in lstlist)
                    {
                        item.Add(new SelectListItem { Text = x.BOOKINGNUMBER, Value = x.BOOKINGNUMBER.ToString() });
                    }
                }
                return Json(new SelectList(item, "Value", "Text", JsonRequestBehavior.AllowGet));



            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }




    }
}