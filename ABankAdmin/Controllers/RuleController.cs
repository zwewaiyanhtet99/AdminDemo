using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class RuleController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "RuleController";
        EventLogController log = new EventLogController();
        // GET: Rule
        public ActionResult Index()
        {
            return View();
            
        }
        //Post:Rule/Index
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
            var townshipCode = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Code = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var rule = (from v in db.Rules
                        where v.Del_Flag=="0"
                           select v);

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                rule = rule.OrderBy(sortColumn + " " + sortColumnDir);
            }
            
            recordsTotal = rule.Count();
            var data = rule.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);

        }
        
        // GET: Rule/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Rules
                             where s.Del_Flag == "0"
                             select s;
                Rule_ rule = result.SingleOrDefault(m => m.ID == id);
                if (rule == null)
                {
                    return HttpNotFound();
                }
                RuleVM rulevm = new RuleVM
                {
                    Code = rule.Code,
                    Description = rule.Description,
                    Value = General.ConvertCDecimalToStr(rule.Value)
                };
                return View(rulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
            //if (id != 0)
            //{
            //    result = result.Where(r => r.Del_Flag == "0" );
            //}
        }

        // POST: Rule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RuleVM rulevm/*[Bind(Include = "ID,Value")] Rule_ rule*/)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = from s in db.Rules
                                 where s.Del_Flag == "0"
                                 select s;
                    var db_rule = result.SingleOrDefault(m => m.ID == rulevm.ID);
                    //Rule_ rule_ = db.Rules.Find(rule.ID);
                    if (db_rule == null)
                    {
                        return HttpNotFound();
                    }
                    db_rule.Value = General.ConvertStrToDecimal(rulevm.Value);
                    db_rule.UpdatedDate = DateTime.Now;
                    db_rule.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(db_rule).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    decimal value = General.ConvertStrToDecimal(rulevm.Value);
                    //update normal user if Rule is R003 (ALLOW_TRAN_COUNT) and R005 (DAILY_MAX_AMT)
                    if (db_rule.Code == "R003")//for ALLOW_TRAN_COUNT
                    {
                        //Update allow tran count   //type = 1
                        UpdateNormalUsers(1, value);
                    } else if (db_rule.Code == "R005")
                    {//for DAILY_MAX_AMT
                        //Update DAILY_MAX_AMT   //type = 2
                        UpdateNormalUsers(2, value);
                    }

                    return RedirectToAction("Index");
                }
                return View(rulevm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        //Update amount of Normal Users (ALLOW_TRAN_COUNT and DAILY_MAX_AMT)
        private bool UpdateNormalUsers(byte type, decimal value)
        {
            string userid = User.Identity.GetUserId();
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //1=> Allow tran count,     2=> daily max amount
                if (!(type == 1 || type == 2))
                {
                    //invalid type
                    return false;
                }
                //failed user list
                string failedUsers = "";
                List<User> lstNormalUsers = db._Users.Where(u => u.IsVIP == false).ToList();
                foreach (var normaluser in lstNormalUsers)
                {
                    try
                    {
                        if (type == 1)
                        {
                            int Value = Convert.ToInt32(value);
                            normaluser.ALLOW_TRAN_COUNT = Value;
                        }
                        else if (type == 2)
                        {
                            normaluser.DAILY_MAX_AMT = value;
                        }
                        db.Entry(normaluser).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException)
                    {
                        failedUsers += $"{normaluser.USERNAME}, ";
                        db.Entry(normaluser).State = EntityState.Unchanged;
                        continue;//skip fail user and continue next ones
                    }
                }
                if (failedUsers != "") {
                    failedUsers = $"Failed Users = {failedUsers}";
                }
                //save log
                if (type == 1)
                {
                    log.Log(userid, controllerName, actionName, "Updating Normal Users", $"Allow Transaction Count of {lstNormalUsers.Count} Normal users is changed into {value}.\n{failedUsers}");
                }
                else if (type == 2)
                {
                    log.Log(userid, controllerName, actionName, "Updating Normal Users", $"Daily Maximum Amount of {lstNormalUsers.Count} Normal users is changed into {value}.\n{failedUsers}");
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
            return false;
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
