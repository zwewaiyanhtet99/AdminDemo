using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class RenewalController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "Renewal";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        EventLogController log = new EventLogController();

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
            var username = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var usertype = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var rule = db.UserTypeModels.Where(r => r.DEL_FLAG == false).FirstOrDefault();
            int ruleValue = (rule == null ? 5 : rule.ID);
            var user = db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status==1 ||a.Status == 6) && (a.DeactivateStatus == 0) && (a.UpgradeStatus == 0 || a.UpgradeStatus==2) && (a.DowngradeStatus == 0 || a.DowngradeStatus == 2)).ToList().ConvertAll(u => new CIFInfoVM
            {
                PHONENO = u.PHONENO,
                NAME = u.NAME,
                USERTYPE = u.USERTYPE,
                USERTYPECODE = u.USERTYPECODE,
                EXPIREDATE = u.EXPIREDATE,
                strEXPIREDATE = String.Format("{0:MM-yy}", u.EXPIREDATE),
                NRC = u.NRC,
                Address = u.ADDRESS,
                CIFID = u.CIFID,
                ID = u.ID,
                REMARK = u.REMARK,
                Createduserid = u.CREATEDUSERID,
                UpgradeStatus = u.UpgradeStatus.ToString(),
                DeactivateStatus = u.DeactivateStatus.ToString(),
                Status = u.Status.ToString()
            });
            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {
                var getType = (from s in db.CIFInfoForTemp
                               where s.CIFID == item.CIFID && s.DELFLAG == false && (s.Status == 5)
                               select s)
                                  .SingleOrDefault();
                if (getType == null)
                {
                    Info = item;
                    if (item.Createduserid != null)
                    {
                        item.Createduserid =
                            db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.Status == "7")
                    {
                        item.Status = "Already Rejected";
                    }
                    else if (item.Status == "6")
                    {
                        item.Status = "Already Approved";
                    }
                    else
                    {
                        item.Status = "Normal";
                    }
                    userData.Add(Info);
                }
            }

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                userData = userData.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(username.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                userData = userData.Where(a => a.NAME.ToLower().Contains(username.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //user = user.Where(a => a.USER_TYPE.Contains(usertype)).ToList();
                userData = userData.Where(a => a.USERTYPE == usertype).ToList();
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
       
        public ActionResult Renewal(int? id)
        {
            CIFInfoVMForRenewal infoVm = new CIFInfoVMForRenewal();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            cIFInfoModel.DeactivateDate = Convert.ToDateTime(cIFInfoModel.DeactivateDate);

            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForRenewal>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            string userID = User.Identity.GetUserId();
            ViewBag.Emaillist = CIFInfoModelsController.GetApproverEmailList(userID);
            infoVm.UserEmail = (from s in db.Users where s.Id == userID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            DateTime today = DateTime.Now;
            DateTime addDate1 = today.AddMonths(1);
            infoVm.NewEffectiveDate = addDate1;
            infoVm.strNewEffectiveDate = addDate1.ToString("MM/yy");
            infoVm.strNewExpireDate = infoVm.NewEffectiveDate.AddDays(365).ToString("MM/yy");
            
            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renewal(CIFInfoVMForRenewal infoVm)
        {
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
                    //DbInfo.UserEmail = infoVm.UserEmail;
                    //DbInfo.RMID = infoVm.RMID;
                    //DbInfo.RMName = infoVm.RMName;
                    DbInfoForTemp.Status = 5;//Renewal pending
                    DbInfoForTemp.USERTYPECODE = infoVm.USERTYPECODE;
                    var dd = infoVm.strNewEffectiveDate.Replace("/", "-");
                    var ddexp = infoVm.strNewExpireDate.Replace("/", "-");
                    string addday = "01-" + dd;
                    string adddayexp = "01-" + ddexp;
                    infoVm.EXPIREDATE = General.ConvertStrToDateForIconic(adddayexp);
                    infoVm.EffectiveDate = General.ConvertStrToDateForIconic(addday);
                    DbInfoForTemp.EFFECTIVEDATE = infoVm.EffectiveDate;
                    DbInfoForTemp.EXPIREDATE = infoVm.EXPIREDATE;
                    #endregion
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, "Renewal", 2, $"{infoVm.CIFID} renewal is pending.");
                    ViewBag.RenewalInfo = "\"" + infoVm.CIFID + "\"";
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
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "CIF_Upgrade");
                    #endregion
                    //ViewBag.Emaillist = Session["ApproverEmailList"];
                    return View(infoVm);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }

    }
}