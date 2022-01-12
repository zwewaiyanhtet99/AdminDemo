using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CIFInfoForViewAndEditController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "CIFInfoForViewAndEdit";
        EventLogController log = new EventLogController();

        // GET: CIFInfoForViewAndEdit
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
            var user = db.CIFInfoModels.Where(a => a.DELFLAG == false && (a.Status == 1|| a.Status == 4 || a.Status ==6) && (a.UpgradeStatus==0 || a.UpgradeStatus==2) && (a.DowngradeStatus == 0 || a.DowngradeStatus == 2) && a.DeactivateStatus ==0).ToList().ConvertAll(u => new CIFInfoVM
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
            });
            List<CIFInfoVM> userData = new List<CIFInfoVM>();
            var Info = new CIFInfoVM();
            foreach (var item in user)
            {
                var getType = (from s in db.CIFInfoForTemp
                               where s.CIFID == item.CIFID && s.DELFLAG == false && s.Status == 3 && (s.UpgradeStatus == 0 || s.UpgradeStatus == 2) && (s.DowngradeStatus == 0 || s.DowngradeStatus == 2) && s.DeactivateStatus == 0
                               select s)
                                   .SingleOrDefault();
                if (getType == null)
                {
                    Info = item;
                    if (item.Createduserid != null)
                    {
                        Info.Createduserid = db.Users.Where(x => x.Id == item.Createduserid).Select(x => x.UserName).FirstOrDefault();
                    }
                    if (item.Status == "4" || item.UpgradeStatus == "3" || item.DeactivateStatus == "3")
                    {
                        Info.Status = "Already Rejected";
                    }
                    else
                    {
                        if (item.Status == "0" || item.Status == "3")
                        {
                            item.Status = "Pending";
                        }
                        else if (item.Status == "1" || item.Status=="6")
                        {
                            item.Status = "Already Approved";
                        }
                        else if (item.Status == "2" || item.Status == "4")
                        {
                            item.Status = "Already Rejected";

                        }
                        
                    }
                    userData.Add(Info);
                }
                


            }
            //SEARCHING...

            if (!string.IsNullOrEmpty(cifid.Trim()))
            {
                userData = userData.Where(a => a.CIFID.ToLower().Contains(cifid.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(iconic.Trim()))
            {
                //int branchid = Convert.ToInt32(username);
                userData = userData.Where(a => a.USERTYPECODE==iconic).ToList();
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
        // GET: CIFInfoForViewAndEdit/Details/5
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

        

        // GET: CIFInfoForViewAndEdit/Edit/5
        public ActionResult Edit(int? id)
        {
            CIFInfoVM infoVm = new CIFInfoVM();
            if (TempData["editReturn"] == "1")
            {
                if (Session["CIFVMForEdit"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["CIFVMForEdit"].ToString()))
                    {
                        infoVm = JsonConvert.DeserializeObject<CIFInfoVM>(Session["CIFVMForEdit"].ToString());
                        if (infoVm.RMID != null)
                        {
                            IEnumerable<SelectListItem> items1 = db.RM_Info.Where(b => b.DEL_FLAG == false)
                                .Select(c => new SelectListItem
                                {
                                    Selected = (c.ID == infoVm.RMID),
                                    Value = c.ID.ToString(),
                                    Text = c.Name
                                }).ToList();

                            ViewBag.RMList = items1;

                        }


                    }
                }
            }
            else
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
                infoVm = JsonConvert.DeserializeObject<CIFInfoVM>(JsonConvert.SerializeObject(cIFInfoModel));
                infoVm.strEXPIREDATE = infoVm.EXPIREDATE.ToString("MM/yy");
                infoVm.strEffectiveDate = infoVm.EffectiveDate.ToString("MM/yy");
                IEnumerable<SelectListItem> items2 = db.RM_Info.Where(b => b.DEL_FLAG == false)
                                       .Select(c => new SelectListItem
                                       {
                                           Selected = (c.ID == infoVm.RMID),
                                           Value = c.ID.ToString(),
                                           Text = c.Name
                                       }).ToList();

                ViewBag.RMList = items2;
            }
            
            return View(infoVm);
        }
       
        // POST: CIFInfoForViewAndEdit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CIFInfoVM infoVm)
        {
            if (ModelState.IsValid)
            {
                Session["CIFVMForEdit"] = string.Empty;
                Session["CIFVMForEdit"] = JsonConvert.SerializeObject(infoVm);
                return RedirectToAction("CIF_ConfirmPageForEdit");
                
            }
            ViewBag.RMList = CIFInfoModelsController.GetRMList();
            return View(infoVm);
        }


        public ActionResult CIF_ConfirmPageForEdit()
        {
            TempData["editReturn"] = "1";
            CIFInfoVM reqmodel = new CIFInfoVM();
            try
            {
                if (Session["CIFVMForEdit"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["CIFVMForEdit"].ToString()))
                    {
                        reqmodel = JsonConvert.DeserializeObject<CIFInfoVM>(Session["CIFVMForEdit"].ToString());
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
        public ActionResult CIF_ConfirmPageForEdit(CIFInfoVM infoVm)
        {
            try
            {
                Session["CIFVMForEdit"] = string.Empty;
                TempData["editReturn"] = string.Empty;
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
                    DbInfoForTemp.RMID = infoVm.RMID;
                    DbInfoForTemp.RMName = infoVm.RMName;
                    DbInfoForTemp.REMARK = infoVm.REMARK;
                    DbInfoForTemp.Status = 3;//Request edit

                    #endregion
                    db.CIFInfoForTemp.Add(DbInfoForTemp);
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, "CIF_ConfirmPageForEdit", 2, $"{infoVm.CIFID} Creation is pending.");
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
                    bool sendEmail = CIFInfoModelsController.SendEmailForIconic(mailList, infoVm.CIFID,getuserid, controllerName, "CIF_ConfirmPageForEdit");
                    #endregion
                    return View(infoVm);
                }

            }
            catch (Exception ex)
            { throw ex; }
            return View();
        }
        // GET: CIFInfoForViewAndEdit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CIFInfoForViewAndEdit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CIFID,NAME,PHONENO,USERTYPE,USERTYPECODE,NRC,ADDRESS,REMARK,EXPIREDATE,CREATEDDATE,CREATEDUSERID,UpdatedDate,UPDATEDUSERID,DELFLAG,EFFECTIVEDATE,BRANCHNAME,RMName,RMID,Status,ApproverEmail,UserEmail,UpgradeStatus")] CIFInfoModel cIFInfoModel)
        {
            if (ModelState.IsValid)
            {
                db.CIFInfoModels.Add(cIFInfoModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cIFInfoModel);
        }

        // GET: CIFInfoForViewAndEdit/Delete/5
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

        // POST: CIFInfoForViewAndEdit/Delete/5
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
    }
}
