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
    public class CIFInfoForApproverController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CIFInfoForApprover";
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        EventLogController log = new EventLogController();
        // GET: CIFInfoForApprover
        public ActionResult Index()
        {
            try
            {
                TempData["Stage"] = string.Empty;
                TempData["StoreFlagStage"] = string.Empty;
                TempData["FlagStage"] = string.Empty;
                TempData["FlagStage"] = "Create";
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
            List<CIFInfoVM> user = new List<CIFInfoVM>();
            if (!string.IsNullOrEmpty(TempData["FlagStage"].ToString()) || !string.IsNullOrEmpty(TempData["StoreFlagStage"].ToString()))
            {

                string flag = TempData["FlagStage"].ToString();
                if (string.IsNullOrEmpty(flag))
                {
                    flag = TempData["StoreFlagStage"].ToString();
                }
                TempData["StoreFlagStage"] = flag;
                TempData["FlagStage"] = string.Empty;
                if (flag == "Create")
                {
                    user =
                        db.CIFInfoModels.Where(
                            a => a.DELFLAG == false && (a.Status == 0 || a.Status == 1 || a.Status == 2 ) && a.DeactivateStatus == 0 && (a.UpgradeStatus == 0 || a.UpgradeStatus== 2) && (a.DowngradeStatus == 0 || a.DowngradeStatus == 2))
                            .ToList()
                            .ConvertAll(u => new CIFInfoVM
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
                                DeactivateStatus = u.DeactivateStatus.ToString(),
                                Flag = "Create"
                            });
                    var getType = (from s in db.CIFInfoForTemp
                                   where s.DELFLAG == false && s.Status == 0
                                   select s).ToList();
                    if (getType.Count>0)
                    {
                        foreach (var item in getType)
                        {
                            CIFInfoVM newUser = JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(item));
                            newUser.strEXPIREDATE = String.Format("{0:MM/yy}", newUser.EXPIREDATE);
                            user.Add(newUser);
                        }
                        

                    }
                    
                    foreach (var item in user)
                    {
                       
                        if (item.Createduserid != null)
                        {
                            item.Createduserid =
                                db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                        }
                        if (item.Status == "0")
                        {
                            item.Status = "Pending";
                        }
                        else if (item.Status == "1")
                        {
                            item.Status = "Already Approved";
                        }
                        else if (item.Status == "2")
                        {
                            item.Status = "Already Rejected";

                        }

                    }
                }

                else if (flag == "Edit")
                {

                    user =
                        db.CIFInfoModels.Where(
                            a => a.DELFLAG == false && (a.Status == 3 || a.Status == 1 || a.Status == 4) && a.DeactivateStatus == 0 && (a.UpgradeStatus == 0 || a.UpgradeStatus == 2) && (a.DowngradeStatus == 0 || a.DowngradeStatus == 2))
                            .ToList()
                            .ConvertAll(u => new CIFInfoVM
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
                                Flag = "Edit"
                            });
                    foreach (var item in user)
                    {
                        var getType = (from s in db.CIFInfoForTemp
                                       where s.CIFID == item.CIFID && s.DELFLAG == false && s.Status == 3 && s.UpgradeStatus !=1 && s.DowngradeStatus!=1 && s.DeactivateStatus==0
                                       select s)
                            .SingleOrDefault();
                        if (getType != null)
                        {
                            item.Status = getType.Status.ToString();
                            item.REMARK = getType.REMARK.ToString();
                            item.UpdatedDate = getType.UpdatedDate;
                            item.RMID = getType.RMID;
                            item.RMName = getType.RMName;
                            item.ApproverEmail = getType.ApproverEmail;
                            item.UserEmail = getType.UserEmail;
                        }
                        if (item.Createduserid != null)
                        {
                            item.Createduserid =
                                db.Users.Where(x => x.Id == item.Createduserid)
                                    .Select(x => x.UserName)
                                    .FirstOrDefault();
                        }
                        if (item.Status == "3")
                        {
                            item.Status = "Pending";
                        }
                        else if (item.Status == "1")
                        {
                            item.Status = "Already Approved";
                        }
                        else if (item.Status == "4")
                        {
                            item.Status = "Already Rejected";

                        }


                    }
                }
                else if (flag == "Deactivate")

                {
                    user =
                        db.CIFInfoModels.Where(
                            a => a.DELFLAG == false && (a.Status == 1 || a.Status == 6) && (a.UpgradeStatus == 0 || a.UpgradeStatus == 2) && (a.DowngradeStatus == 0 || a.DowngradeStatus == 2))
                            .ToList()
                            .ConvertAll(u => new CIFInfoVM
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
                                Status = u.Status.ToString()
                                ,
                                Flag = "Deactivate",
                                DeactivateStatus = u.DeactivateStatus.ToString()
                            });
                    
                    foreach (var item in user.ToList())
                    {
                        
                            var getType = (from s in db.CIFInfoForTemp
                                where
                                    s.CIFID == item.CIFID && s.DELFLAG == false &&
                                    s.DeactivateStatus == 1
                                select s)
                                .SingleOrDefault();
                        if (getType != null)
                        {
                            item.DeactivateStatus = getType.DeactivateStatus.ToString();
                            item.DeactivateDate = getType.DeactivateDate;
                            item.UpdatedDate = getType.UpdatedDate;
                            item.ApproverEmail = getType.ApproverEmail;
                            item.UserEmail = getType.UserEmail;
                        }
                        
                        if (item.DeactivateStatus != "0")
                        {

                            if (item.Createduserid != null)
                            {
                                item.Createduserid =
                                    db.Users.Where(x => x.Id == item.Createduserid)
                                        .Select(x => x.UserName)
                                        .FirstOrDefault();
                            }
                            if (item.DeactivateStatus == "1")
                            {
                                item.Status = "Pending";
                            }
                            else if (item.DeactivateStatus == "2")
                            {
                                item.Status = "Already Approved";
                            }
                            else if (item.DeactivateStatus == "3")
                            {
                                item.Status = "Already Rejected";

                            }
                        }
                        if(item.DeactivateStatus == "0")
                        {
                            user.Remove(item);
                        }
                    }
                }
            }
            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                user = user.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(username.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                user = user.Where(a => a.NAME.ToLower().Contains(username.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //user = user.Where(a => a.USER_TYPE.Contains(usertype)).ToList();
                user = user.Where(a => a.USERTYPE == usertype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }


            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: CIFInfoForApprover/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            return View(cIFInfoModel);
        }

        // GET: CIFInfoForApprover/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CIFInfoForApprover/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CIFID,NAME,PHONENO,USERTYPE,USERTYPECODE,NRC,ADDRESS,REMARK,EXPIREDATE,CREATEDDATE,CREATEDUSERID,UpdatedDate,UPDATEDUSERID,DELFLAG,EFFECTIVEDATE,BRANCHNAME,RMName,RMID,Status,ApproverEmail,UserEmail")] CIFInfoModel cIFInfoModel)
        {
            if (ModelState.IsValid)
            {
                db.CIFInfoModels.Add(cIFInfoModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cIFInfoModel);
        }

        // GET: CIFInfoForApprover/Edit/5
        public ActionResult Edit(int? id, string stage)
        {
            if (stage != null)
            {
                ViewBag.FromStage = stage;
                TempData["Stage"] = stage;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModelForTemp cIFInfoModel = new CIFInfoModelForTemp();
            string cifid = (from s in db.CIFInfoModels where s.ID == id && s.DELFLAG == false select s.CIFID).SingleOrDefault();
            string cifid_temp =
                (from s in db.CIFInfoForTemp where s.ID == id && s.DELFLAG == false select s.CIFID).SingleOrDefault();
            if (string.IsNullOrEmpty(cifid_temp))
            {
                 cIFInfoModel =
                    db.CIFInfoForTemp.Where(x => x.CIFID == cifid && x.DELFLAG == false).SingleOrDefault();

            }
            else
            {
                cIFInfoModel =
                   db.CIFInfoForTemp.Where(x => x.CIFID == cifid_temp && x.DELFLAG == false).SingleOrDefault();
            }
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            CIFInfoVMForApprover infoVm = new CIFInfoVMForApprover();
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForApprover>(JsonConvert.SerializeObject(cIFInfoModel));
            if (TempData["StoreFlagStage"] != null)
            {
                infoVm.Flag = TempData["StoreFlagStage"].ToString();
            }
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            if (infoVm.DeactivateStatus == "1")
            {
               
                infoVm.DeactivateStatus = cIFInfoModel.DeactivateStatus.ToString();
                if (infoVm.DeactivateDate == null)
                {
                    infoVm.DeactivateDate = DateTime.Now;
                    infoVm.strDeactivateDate = null;
                }
                else
                {
                    infoVm.strDeactivateDate = infoVm.DeactivateDate?.ToString("dd/MM/yyyy");
                }
            }
            TempData["StoreFlagStage"] = infoVm.Flag;
            infoVm.RMEmail = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            int RM_TL_ID = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.RM_TL_ID).SingleOrDefault();
            infoVm.RM_TL_Email = (from s in db.RM_TL_Info where s.ID == RM_TL_ID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            return View(infoVm);
        }

        // POST: CIFInfoForApprover/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CIFInfoVMForApprover infoVm)
        {
            bool isReject = false;
            try
            {
                bool isDelete = false;
                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    var DbInfoFortemp = (from s in db.CIFInfoForTemp where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s).SingleOrDefault();
                    if (DbInfo != null)
                    {
                        DbInfo.UPDATEDUSERID = User.Identity.GetUserId();
                        DbInfo.UpdatedDate = DateTime.Now;
                        DbInfo.ApproverEmail = infoVm.ApproverEmail;
                        DbInfo.UserEmail = infoVm.UserEmail;
                    }
                    
                    if (TempData["StoreFlagStage"] != null)
                    {
                        infoVm.Flag = TempData["StoreFlagStage"].ToString();
                    }
                    if (TempData["Stage"].ToString() == "Approve")
                    {
                        if (infoVm.Status == "1")
                        {
                            if (infoVm.DeactivateStatus == "1")
                            {
                                infoVm.Flag = "Deactivate";
                                DbInfo.DeactivateStatus = 2;
                                if (!string.IsNullOrEmpty(infoVm.strDeactivateDate))
                                {
                                    var deactivate = infoVm.strDeactivateDate.Replace("/", "-");
                                    DbInfo.DeactivateDate = General.ConvertStrToDate(deactivate);
                                }
                            }
                        }
                        if (infoVm.Status == "0" || infoVm.Status == "3")
                        {
                            if (infoVm.Status == "0")
                            {
                                CIFInfoModel DbInfo_Create = new CIFInfoModel();
                                DbInfo_Create.REMARK = DbInfoFortemp.REMARK;
                                DbInfo_Create.RMName = DbInfoFortemp.RMName;
                                DbInfo_Create.RMID = DbInfoFortemp.RMID;
                                DbInfo_Create.CIFID = DbInfoFortemp.CIFID;
                                DbInfo_Create.USERTYPE = DbInfoFortemp.USERTYPE;
                                DbInfo_Create.USERTYPECODE = DbInfoFortemp.USERTYPECODE;
                                DbInfo_Create.PHONENO = DbInfoFortemp.PHONENO;
                                DbInfo_Create.NRC = DbInfoFortemp.NRC;
                                DbInfo_Create.ADDRESS = DbInfoFortemp.ADDRESS;
                                DbInfo_Create.EXPIREDATE = DbInfoFortemp.EXPIREDATE;
                                DbInfo_Create.EFFECTIVEDATE = DbInfoFortemp.EFFECTIVEDATE;
                                DbInfo_Create.CREATEDDATE = DbInfoFortemp.CREATEDDATE;
                                DbInfo_Create.CREATEDUSERID = DbInfoFortemp.CREATEDUSERID;
                                DbInfo_Create.DELFLAG = DbInfoFortemp.DELFLAG;
                                DbInfo_Create.NAME = DbInfoFortemp.NAME;
                                DbInfo_Create.BRANCHNAME = DbInfoFortemp.BRANCHNAME;
                                DbInfo_Create.Status =1;
                                DbInfo_Create.ApproverEmail = DbInfoFortemp.ApproverEmail;
                                DbInfo_Create.UserEmail = DbInfoFortemp.UserEmail;
                                DbInfo_Create.DowngradeStatus = DbInfoFortemp.DowngradeStatus;
                                DbInfo_Create.DeactivateStatus = DbInfoFortemp.DeactivateStatus;
                                DbInfo_Create.UpgradeStatus = DbInfoFortemp.UpgradeStatus;
                                db.CIFInfoModels.Add(DbInfo_Create);
                                db.SaveChanges();
                                isDelete = true;
                            }
                            else
                            {
                                DbInfo.Status = 1;
                                if (DbInfoFortemp != null)
                                {
                                    DbInfo.REMARK = DbInfoFortemp.REMARK;
                                    DbInfo.RMName = DbInfoFortemp.RMName;
                                    DbInfo.RMID = DbInfoFortemp.RMID;

                                }
                            }
                           

                        }


                    }
                    else
                    {

                        isReject = true;
                        if (infoVm.Status == "1")
                        {
                            if (infoVm.DeactivateStatus == "1")
                            {
                                infoVm.Flag = "Deactivate";
                                DbInfo.DeactivateStatus = 3;//Reject

                            }
                        }

                        if (infoVm.Status == "0")
                        {
                             isDelete = true;

                        }
                        if (infoVm.Status == "3")
                        {
                            DbInfo.Status = 4;//Edit reject

                        }

                    }
                   
                    #endregion

                    if (!isDelete)
                    {
                        var localEntity = db.Set<CIFInfoModel>().Local.FirstOrDefault(f => f.ID == DbInfo.ID);
                        if (localEntity != null)
                        {
                            db.Entry(localEntity).State = EntityState.Detached;
                        }
                        db.Entry(DbInfo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                    #region DataPreparationToupdateCIFtemptable

                    DbInfoFortemp.UpdatedDate = DateTime.Now;
                    DbInfoFortemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoFortemp.DELFLAG = true;
                    db.Entry(DbInfoFortemp).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
                //save log
                if (isReject)
                {
                    ViewBag.CifRejectInfo = "\"" + infoVm.CIFID + "\"";

                }
                else
                {
                    ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
                }

                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, "Edit", 2, $"{infoVm.CIFID} Creation is approved.");
                List<string> mailList = new List<string>();
                if (infoVm.RMEmail != null)
                {
                    mailList.Add(infoVm.RMEmail);
                }

                mailList.Add(infoVm.UserEmail);
                mailList.Add(infoVm.RM_TL_Email);
                #region sendEmail
                bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "Edit");
                #endregion
                return View(infoVm);

            }
            catch (Exception ex)
            { throw ex; }
            return View();

        }



        // GET: CIFInfoForApprover/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            return View(cIFInfoModel);
        }

        // POST: CIFInfoForApprover/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CIFInfoModel cIFInfoModel = db.CIFInfoModels.Find(id);
            db.CIFInfoModels.Remove(cIFInfoModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region For Edit Approve
        public ActionResult EditIndex()
        {
            try
            {
                TempData["StoreFlagStage"] = string.Empty;
                TempData["FlagStage"] = string.Empty;
                TempData["FlagStage"] = "Edit";
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View("Index", db.CIFInfoModels.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Index");
        }


        #endregion

        #region For Deactivate Approve
        public ActionResult DeactivateIndex()
        {
            try
            {
                TempData["StoreFlagStage"] = string.Empty;
                TempData["FlagStage"] = string.Empty;
                TempData["FlagStage"] = "Deactivate";
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View("Index", db.CIFInfoModels.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Index");
        }

        #endregion

        #region For Renewal Approve
        public ActionResult RenewalIndex()
        {
            try
            {
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult RenewalIndex(int? arg1)
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
            var user = db.CIFInfoModels.Where(  
                            a => a.DELFLAG == false && (a.Status==1 || a.Status == 5 || a.Status == 6 || a.Status==7) && a.DeactivateStatus == 0 && a.UpgradeStatus != 1  && a.DowngradeStatus != 1)
                            .ToList()
                            .ConvertAll(u => new CIFInfoVM
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
                                DeactivateStatus = u.DeactivateStatus.ToString()

                            });
            foreach (var item in user.ToList())
            {
                var getType = (from s in db.CIFInfoForTemp
                               where
                                   s.CIFID == item.CIFID && s.DELFLAG == false  && (s.Status == 5)
                               select s)
                                  .SingleOrDefault();
                if (getType != null)
                {
                    item.Status = getType.Status.ToString();
                    item.EffectiveDate = getType.EFFECTIVEDATE;
                    item.EXPIREDATE = getType.EXPIREDATE;
                    item.strEXPIREDATE = String.Format("{0:MM/yy}", getType.EXPIREDATE);
                    item.strEffectiveDate = String.Format("{0:MM/yy}", getType.EFFECTIVEDATE);
                    item.ApproverEmail = getType.ApproverEmail;
                    item.UserEmail = getType.UserEmail;
                }
                if (item.Status == "6" || item.Status == "7" || item.Status == "5")
                {
                    if (item.Createduserid != null)
                    {
                        item.Createduserid =
                            db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.Status == "5")
                    {
                        item.Status = "Pending";
                    }
                    else if (item.Status == "6")
                    {
                        item.Status = "Already Approved";
                    }
                    else if (item.Status == "7")
                    {
                        item.Status = "Already Rejected";

                    }
                }
                else
                {
                    user.Remove(item);

                }
            }
               

            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                user = user.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(username.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                user = user.Where(a => a.NAME.ToLower().Contains(username.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(usertype))
            {
                //user = user.Where(a => a.USER_TYPE.Contains(usertype)).ToList();
                user = user.Where(a => a.USERTYPE == usertype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }


            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenewalEdit(int? id, string stage)
        {
            if (stage != null)
            {
                ViewBag.FromStage = stage;
                TempData["Stage"] = stage;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string cifid = (from s in db.CIFInfoModels where s.ID == id && s.DELFLAG == false select s.CIFID).SingleOrDefault();
            CIFInfoModelForTemp cIFInfoModel = db.CIFInfoForTemp.Where(x => x.CIFID == cifid && x.DELFLAG == false).SingleOrDefault();
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            CIFInfoVMForApprover infoVm = new CIFInfoVMForApprover();
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForApprover>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            infoVm.RMEmail = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            int RM_TL_ID = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.RM_TL_ID).SingleOrDefault();
            infoVm.RM_TL_Email = (from s in db.RM_TL_Info where s.ID == RM_TL_ID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenewalEdit(CIFInfoVMForApprover infoVm)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    var DbInfoFortemp = (from s in db.CIFInfoForTemp where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s).SingleOrDefault();

                    DbInfo.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfo.UpdatedDate = DateTime.Now;
                    var approverEmail = DbInfo.ApproverEmail;
                    DbInfo.UserEmail = infoVm.UserEmail;
                    if (TempData["Stage"].ToString() == "Approve")
                    {
                        if (DbInfoFortemp.Status == 5 && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.Status =6;//approve
                            if (DbInfoFortemp != null)
                            {
                                DbInfo.EFFECTIVEDATE = DbInfoFortemp.EFFECTIVEDATE;
                                DbInfo.EXPIREDATE = DbInfoFortemp.EXPIREDATE;
                                DbInfo.ApproverEmail = DbInfoFortemp.ApproverEmail;
                            }
                        }
                    }
                    else
                    {

                        if (DbInfoFortemp.Status == 5 && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.Status = 7;//reject
                            
                        }

                    }
                    
                    DbInfo.ApproverEmail = approverEmail;
                    #endregion
                    var localEntity = db.Set<CIFInfoModel>().Local.FirstOrDefault(f => f.ID == DbInfo.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    db.Entry(DbInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    #region DataPreparationToupdateCIFtemptable

                    DbInfoFortemp.UpdatedDate = DateTime.Now;
                    DbInfoFortemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoFortemp.DELFLAG = true;
                    db.Entry(DbInfoFortemp).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
                //save log
                ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, "RenewalEdit", 2, $"{infoVm.CIFID} Renewal  is approved.");
                List<string> mailList = new List<string>();
                if (infoVm.RMEmail != null)
                {
                    mailList.Add(infoVm.RMEmail);
                }

                mailList.Add(infoVm.UserEmail);
                mailList.Add(infoVm.RM_TL_Email);
                #region sendEmail
                bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "IconicApproverEdit");
                #endregion
                return View(infoVm);

            }
            catch (Exception ex)
            { throw ex; }

        }
        #endregion

        #region For Downgrade Approve
        public ActionResult DowngradeIndex()
        {
            try
            {
                var data = db.CIFInfoModels.ToList();
                int count = data.Count();

                if (count > 0)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult DowngradeIndex(int? arg1)
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
            var user = db.CIFInfoModels.Where(
                            a => a.DELFLAG == false && (a.Status == 1 || a.Status==6)  && a.DeactivateStatus == 0 && a.UpgradeStatus != 1)
                            .ToList()
                            .ConvertAll(u => new CIFInfoVM
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
                                DeactivateStatus = u.DeactivateStatus.ToString(),
                                DowngradeStatus = u.DowngradeStatus.ToString()

                            });
            foreach (var item in user.ToList())
            {
                var getType = (from s in db.CIFInfoForTemp
                               where
                                   s.CIFID == item.CIFID && s.DELFLAG == false && (s.Status == 1 || s.Status==6) &&
                                   s.DowngradeStatus == 1
                               select s)
                                  .SingleOrDefault();
                if (getType != null)
                {
                    item.DowngradeStatus = getType.DowngradeStatus.ToString();
                    item.EffectiveDate = getType.EFFECTIVEDATE;
                    item.EXPIREDATE = getType.EXPIREDATE;
                    item.strEXPIREDATE = String.Format("{0:MM/yy}", getType.EXPIREDATE);
                    item.strEffectiveDate = String.Format("{0:MM/yy}", getType.EFFECTIVEDATE);
                    item.ApproverEmail = getType.ApproverEmail;
                    item.UserEmail = getType.UserEmail;
                    item.USERTYPECODE = getType.USERTYPECODE;
                    item.USERTYPE = getType.USERTYPE;
                }
                if (item.DowngradeStatus != "0")
                {
                    if (item.Createduserid != null)
                    {
                        item.Createduserid =
                            db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.DowngradeStatus == "1")
                    {
                        item.Status = "Pending";
                    }
                    else if (item.DowngradeStatus == "2")
                    {
                        item.Status = "Already Approved";
                    }
                    else if (item.DowngradeStatus == "3")
                    {
                        item.Status = "Already Rejected";

                    }
                }
                else
                {
                    user.Remove(item);
                }

            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                user = user.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }


            recordsTotal = user.Count();
            var data = user.Skip(skip).Take(pageSize).ToList();
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult DowngradeEdit(int? id, string stage)
        {
            if (stage != null)
            {
                ViewBag.FromStage = stage;
                TempData["Stage"] = stage;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string cifid = (from s in db.CIFInfoModels where s.ID == id && s.DELFLAG == false select s.CIFID).SingleOrDefault();
            CIFInfoModelForTemp cIFInfoModel = db.CIFInfoForTemp.Where(x => x.CIFID == cifid && x.DELFLAG == false).SingleOrDefault();
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            if (cIFInfoModel == null)
            {
                return HttpNotFound();
            }
            CIFInfoVMForApprover infoVm = new CIFInfoVMForApprover();
            infoVm = JsonConvert.DeserializeObject<CIFInfoVMForApprover>(JsonConvert.SerializeObject(cIFInfoModel));
            infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
            infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
            infoVm.RMEmail = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            int RM_TL_ID = (from s in db.RM_Info where s.ID == infoVm.RMID && s.DEL_FLAG == false select s.RM_TL_ID).SingleOrDefault();
            infoVm.RM_TL_Email = (from s in db.RM_TL_Info where s.ID == RM_TL_ID && s.DEL_FLAG == false select s.Email).SingleOrDefault();
            return View(infoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DowngradeEdit(CIFInfoVMForApprover infoVm)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    #region Data preparation
                    var DbInfo = (from s in db.CIFInfoModels where s.ID == infoVm.ID && s.DELFLAG == false select s).SingleOrDefault();
                    var DbInfoFortemp = (from s in db.CIFInfoForTemp where s.CIFID == infoVm.CIFID && s.DELFLAG == false select s).SingleOrDefault();

                    DbInfo.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfo.UpdatedDate = DateTime.Now;
                    var approverEmail = DbInfo.ApproverEmail;
                    DbInfo.UserEmail = infoVm.UserEmail;
                    if (TempData["Stage"].ToString() == "Approve")
                    {
                        if (DbInfoFortemp.DowngradeStatus == 1 && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.DowngradeStatus = 2;
                            if (DbInfoFortemp != null)
                            {
                                DbInfo.EFFECTIVEDATE = DbInfoFortemp.EFFECTIVEDATE;
                                DbInfo.EXPIREDATE = DbInfoFortemp.EXPIREDATE;
                                DbInfo.ApproverEmail = DbInfoFortemp.ApproverEmail;
                                DbInfo.USERTYPE = DbInfoFortemp.USERTYPE;
                                DbInfo.USERTYPECODE = DbInfoFortemp.USERTYPECODE;
                            }
                        }
                    }
                    else
                    {

                        if (DbInfoFortemp.DowngradeStatus ==1 && DbInfo.DeactivateStatus == 0)
                        {
                            DbInfo.DowngradeStatus = 3;

                        }

                    }

                    DbInfo.ApproverEmail = approverEmail;
                    #endregion
                    var localEntity = db.Set<CIFInfoModel>().Local.FirstOrDefault(f => f.ID == DbInfo.ID);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                    }
                    db.Entry(DbInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    #region DataPreparationToupdateCIFtemptable

                    DbInfoFortemp.UpdatedDate = DateTime.Now;
                    DbInfoFortemp.UPDATEDUSERID = User.Identity.GetUserId();
                    DbInfoFortemp.DELFLAG = true;
                    db.Entry(DbInfoFortemp).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
                //save log
                ViewBag.CifCreateInfo = "\"" + infoVm.CIFID + "\"";
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid, controllerName, "DowngradeEdit", 2, $"{infoVm.CIFID} downgrade  is approved.");
                List<string> mailList = new List<string>();
                if (infoVm.RMEmail != null)
                {
                    mailList.Add(infoVm.RMEmail);
                }

                mailList.Add(infoVm.UserEmail);
                mailList.Add(infoVm.RM_TL_Email);
                #region sendEmail
                bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID, getuserid, controllerName, "DowngradeEdit");
                #endregion
                return View(infoVm);

            }
            catch (Exception ex)
            { throw ex; }

        }
        #endregion

    }
}
