using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Newtonsoft.Json;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using PagedList;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    [ObsoleteAttribute("OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    public class BranchController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "BranchController";
        EventLogController log = new EventLogController();
        //get connection string
        string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;
        
        // GET: Branch
        public ActionResult Index()
        {
          return View();
        }

        //Index Post
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
            var Code = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
           
            var branch = (from b in db.Branches
                            where b.DEL_FLAG==false
                            select b);
            //Searchig
            if (!string.IsNullOrEmpty(Code.Trim()))
            {
                branch = branch.Where(a => a.code.Contains(Code.Trim()));
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                branch = branch.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = branch.Count();
            var data = branch.Skip(skip).Take(pageSize).ToList();
            return Json( new {draw=draw,recordsFiltered=recordsTotal, recordsTotal=recordsTotal,data=data},
                JsonRequestBehavior.AllowGet);            
        }
        

        // GET: Branch/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Branches
                             where s.DEL_FLAG == false
                             select s;
                Branch branch = result.SingleOrDefault(m => m.id == id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                //city and tsp
                var city1 = db.Cities.Where(c => c.Code == branch.CITY & c.DEL_FLAG == "0").FirstOrDefault();
                branch.CITY = city1 == null ? "" : city1.Description;
                var tsp = db.Townships.Where(t => t.Code == branch.TOWNSHIP & t.DEL_FLAG == "0").FirstOrDefault();
                branch.TOWNSHIP = tsp == null ? "" : tsp.Description;
                return View(branch);
            }
            catch (Exception ex)
            {  
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Branch/Create
        [HttpGet]
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            { 
                 bindTownship();
                bindRemittanceRate();
                RateDesc();
                 return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
       
        [NonAction]
        private void bindTownship(string CityCode = "")
        {
            try
            {
                //Get the value from database and then set it to ViewBag to pass it View
                //city
                var city = db.Cities.Where(ct => ct.DEL_FLAG == "0").ToList();
                List<SelectListItem> li = new List<SelectListItem>();
                li.Add(new SelectListItem { Text = "--Select City--", Value = "" });
                if (city.Count > 0)
                {
                    foreach (var m in city)
                    {
                        li.Add(new SelectListItem { Text = m.Description, Value = m.Code/*, Selected = (m.Code == CityCode)*/ });
                    }
                }
                ViewBag.City = li;

                //township only initializing viewbag variable for create 
                List<SelectListItem> tsp = new List<SelectListItem>();
                tsp.Add(new SelectListItem { Text = "--Select Township--", Value = "" });
                
                //get township if City is selected
                if (CityCode != "")
                {
                    var townships = db.Townships.Where(t => t.CITY_Code == CityCode && t.DEL_FLAG == "0").ToList();
                    if (townships.Count > 0)
                    {
                        foreach (var t in townships)
                        {
                            tsp.Add(new SelectListItem { Text = t.Description, Value = t.Code });
                        }
                    }
                }
                ViewBag.Township = tsp;

                //rate code
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select RateCode--", Value = "" });
                items.Add(new SelectListItem { Text = "1.0", Value = "1" });
                items.Add(new SelectListItem { Text = "2.0", Value = "2" });
                items.Add(new SelectListItem { Text = "3.0", Value = "3" });
                items.Add(new SelectListItem { Text = "4.0", Value = "4" });
                items.Add(new SelectListItem { Text = "5.0", Value = "5" });

                //get core rate code and add if not error
                ChargeRateInquiry c = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                
                try
                {
                    string getuserid = User.Identity.GetUserId(); ; //get login user id
                    cinfo = c.TransferChargeRateInquiry();
                    if (cinfo.ChargeCodeInfo == null) //cinfo.ChargeCodeInfo.Count == 0
                    {
                        string logMsg = $"{cinfo.ResponseCode}, {cinfo.ResponseDesc}, ChargeCodeInfo obj is null.";
                        log.Log(getuserid, controllerName, "TransferChargeRateInquiry", logMsg);
                    }
                    if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count == 0)
                    {
                        string logMsg = $"{cinfo.ResponseCode}, {cinfo.ResponseDesc}, ChargeCodeInfo count is 0.";
                        log.Log(getuserid, controllerName, "TransferChargeRateInquiry", logMsg);
                    }

                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, "TransferChargeRateInquiry", ex1);
                }

                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        items.Add(new SelectListItem { Text = $"{m.ChargeCode} {(m.FixedAmt == "0" ? string.Empty : '(' + m.FixedAmt + ')')} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}", Value = m.ChargeCode });   //showing charge code dropdown inclding fix amt and %
                    }
                }
                ViewBag.RateCode = items;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "bindTownship", ex);
            }
            
        }

        [NonAction]
        public void bindRemittanceRate()
        {
            try
            {
                ChargeRateInquiry c = new ChargeRateInquiry();
                ChargeRateInfo cinfo = new ChargeRateInfo();
                try
                {
                    cinfo = c.RemittanceChargeRateInquiry();
                    if (cinfo.ChargeCodeInfo == null)
                    {
                        string cinfoObj = Newtonsoft.Json.JsonConvert.SerializeObject(cinfo);
                        log.Log(controllerName, "RemittanceChargeRateInquiry", "ChargeRateInfo", $"ChargeRateInfo is\n{cinfoObj}");
                    }
                }
                catch (Exception ex1)
                {
                    log.Error(User.Identity.GetUserId(), controllerName, "RemittanceChargeRateInquiry", ex1);
                }
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "--Select RemittanceRateCode--", Value = "" });
                items.Add(new SelectListItem { Text = "1.0", Value = "1" });
                items.Add(new SelectListItem { Text = "2.0", Value = "2" });
                items.Add(new SelectListItem { Text = "3.0", Value = "3" });
                items.Add(new SelectListItem { Text = "4.0", Value = "4" });
                items.Add(new SelectListItem { Text = "5.0", Value = "5" });

                if (cinfo.ChargeCodeInfo != null && cinfo.ChargeCodeInfo.Count > 0)
                {
                    foreach (var m in cinfo.ChargeCodeInfo)
                    {
                        items.Add(new SelectListItem { Text = $"{m.ChargeCode} {(m.FixedAmt == "0" ? string.Empty : '(' + m.FixedAmt + ')')} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}", Value = m.ChargeCode });
                    }
                }
                ViewBag.RemittanceRateCode = items;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "bindRemittanceRate", ex);
            }
        }

        public JsonResult getTownship(string code)
        {
            try
            {
                var ddlTownship = db.Townships.Where(x => x.CITY_Code == code & x.DEL_FLAG == "0").ToList();
                List<SelectListItem> township = new List<SelectListItem>();
                township.Add(new SelectListItem { Text = "--Select Township--", Value = "" });
                if (ddlTownship.Count>0)
                {
                    foreach (var x in ddlTownship)
                    {
                        township.Add(new SelectListItem { Text = x.Description, Value = x.Code });
                    }
                }
                return Json(new SelectList(township, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // POST: Branch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,name,address,RateCode,RateDesc,RemitRateCode,LATITUDE,LONGITUDE,CITY,TOWNSHIP,PHONE_NO")] Branch branch)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //validation
                    #region validation
                    //duplicate branch code
                    if (db.Branches.Where(b => b.code == branch.code && b.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("code", "Duplicate Branch Code");
                        bindTownship(branch.CITY);
                        bindRemittanceRate();
                        RateDesc();
                        return View(branch);
                    }
                    #endregion
                    //if (branch.RateCode == "EBT05")
                    //{
                    //    branch.RateDesc = "Normal";
                    //}
                    //if (branch.RateCode == "EBT10")
                    //{
                    //    branch.RateDesc = "Special";
                    //}
                    branch.DEL_FLAG = false;
                    branch.CreatedDateTime = DateTime.Now;
                    branch.CreatedUserID = User.Identity.GetUserId();
                    db.Branches.Add(branch);
                    db.SaveChanges();
                    //save log 
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1, branch.ToString());
                    return RedirectToAction("Index");
                }
                bindTownship(branch.CITY);
                bindRemittanceRate();
                RateDesc();
                return View(branch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
        

        // GET: Branch/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Branches
                             where s.DEL_FLAG == false
                             select s;
                Branch branch = result.SingleOrDefault(m => m.id == id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                bindTownship(branch.CITY);
                bindRemittanceRate();
                RateDesc();
                IEnumerable<SelectListItem> items = db.Cities.Where(c=>c.DEL_FLAG=="0")
                    .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Description

                });
                ViewBag.City = items;

                IEnumerable<SelectListItem> townships = db.Townships
                            .Where(g => g.CITY_Code == branch.CITY & g.DEL_FLAG=="0")
                            .Select(f => new SelectListItem()
                            {
                                Value = f.Code,
                                Text = f.Description
                            });
                
                ViewBag.Township = townships;
                return View(branch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
        
        // POST: Branch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,name,address,RateCode,RemitRateCode,RateDesc,LATITUDE,LONGITUDE,CITY,TOWNSHIP,PHONE_NO")] Branch branch)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //validation
                    #region validation
                    //duplicate branch code
                    if (db.Branches.Where(b => b.code == branch.code & b.id != branch.id & b.DEL_FLAG == false).Count() > 0)
                    {
                        ModelState.AddModelError("code", "Duplicate Branch Code");
                        goto dropdown;
                    }
                    #endregion
                    //get data from db
                    var result = from s in db.Branches
                                 where s.DEL_FLAG == false
                                 select s;
                    var branchdb = result.SingleOrDefault(m => m.id == branch.id);
                    if (branchdb == null)
                    {
                        return HttpNotFound();
                    }    
                    //if (branch.RateCode == "EBT05")
                    //{
                    //    branchdb.RateDesc = "Normal";
                    //}
                    //if (branch.RateCode == "EBT10")
                    //{
                    //    branchdb.RateDesc = "Special";
                    //}
                    //update field what u want to be allowed to edit  (from formdata)
                    branchdb.name = branch.name;
                    branchdb.address = branch.address;
                    branchdb.PHONE_NO = branch.PHONE_NO;
                    branchdb.code = branch.code;
                    branchdb.RateCode = branch.RateCode;
                    //remittance rate changes
                    branchdb.RemitRateCode = branch.RemitRateCode;
                    branchdb.RateDesc = branch.RateDesc;
                    branchdb.LATITUDE = branch.LATITUDE;
                    branchdb.LONGITUDE = branch.LONGITUDE;
                    branchdb.CITY = branch.CITY;
                    branchdb.TOWNSHIP = branch.TOWNSHIP;
                    branchdb.UpdatedDateTime = DateTime.Now;
                    branchdb.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(branchdb).State = EntityState.Modified;
                    db.SaveChanges();
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }

                dropdown:
                //IEnumerable<SelectListItem> items = db.Cities.Where(c=>c.DEL_FLAG=="0")
                //    .Select(c => new SelectListItem
                //{
                //    Value = c.Code,
                //    Text = c.Description

                //});
                //ViewBag.City = items;
                //IEnumerable<SelectListItem> townships = db.Townships
                //        .Where(g => g.CITY_Code == branch.CITY & g.DEL_FLAG=="0")
                //        .Select(f => new SelectListItem()
                //        {
                //            Value = f.Code,
                //            Text = f.Description
                //        });
                //ViewBag.Township = townships;
                bindTownship(branch.CITY);
                bindRemittanceRate();
                RateDesc();
                return View(branch);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Branch/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var result = from s in db.Branches
                             where s.DEL_FLAG == false
                             select s;
                Branch branchdb = result.SingleOrDefault(m => m.id == id);
                if (branchdb == null)
                {
                    return HttpNotFound();
                }
                //city and tsp
                var city1 = db.Cities.Where(c => c.Code == branchdb.CITY & c.DEL_FLAG == "0").FirstOrDefault();
                branchdb.CITY = city1 == null ? "" : city1.Description;
                var tsp = db.Townships.Where(t => t.Code == branchdb.TOWNSHIP & t.DEL_FLAG == "0").FirstOrDefault();
                branchdb.TOWNSHIP = tsp == null ? "" : tsp.Description;
                return View(branchdb);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Branch branch = db.Branches.Find(id);             
                branch.DEL_FLAG = true;
                branch.UpdatedDateTime = DateTime.Now;
                branch.UpdatedUserId = User.Identity.GetUserId();
                //db.Branches.Remove(branch);
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid,controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {               
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        //bind user type for api log
        private void RateDesc()
        {
            //create Log Type List
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "Normal", Value = "Normal" });
            items.Add(new SelectListItem() { Text = "Special", Value = "Special" });
            ViewBag.RateDesc = new SelectList(items, "Text", "Value");
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
