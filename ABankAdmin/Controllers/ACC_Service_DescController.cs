using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class ACC_Service_DescController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ACC_Service_DescController";
        EventLogController log = new EventLogController();

        // GET: ACC_Service_Desc
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

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            var acc_service_descs = (from b in db.ACC_Service_Descs
                                where b.Del_Flag == false
                                select b);

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                acc_service_descs = acc_service_descs.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = acc_service_descs.Count();
            var data = acc_service_descs.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: ACC_Service_Desc/Details/5
        public ActionResult Details(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ACC_Service_Desc aCC_Service_Desc = db.ACC_Service_Descs.Find(id);
                if (aCC_Service_Desc == null)
                {
                    return HttpNotFound();
                }
                return View(aCC_Service_Desc);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName,actionName, ex);
                return View("Error");
            }

        }

        // GET: ACC_Service_Desc/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                List<ACC_Service> acc_service = db.ACC_Services.Where(d => d.Del_Flag == false).ToList();
                IEnumerable<SelectListItem> items = acc_service.Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text=a.Acc_Description

                });
                ViewBag.ACC_Service= items;
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
           // return View();
        }

        // POST: ACC_Service_Desc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Account_Services_ID,Desc,OrderNo")] ACC_Service_Desc aCC_Service_Desc)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    bool reg = true;
                    //List<ACC_Service> acc_ser = db.ACC_Services.Where(a=>a.Del_Flag==false).ToList();
                    if (db.ACC_Service_Descs.Where(o=>o.OrderNo== aCC_Service_Desc.OrderNo && o.Account_Services_ID == aCC_Service_Desc.Account_Services_ID && o.Del_Flag ==false).Count() > 0)
                    {
                        ModelState.AddModelError("OrderNo", "Duplicate Order No!");
                        reg = false;
                    }
                    if (reg == false)
                    {
                        //return View(aCC_Service_Desc);
                        goto dropdown;
                    }

                    aCC_Service_Desc.CreatedDate = DateTime.Now;
                    aCC_Service_Desc.CreatedUserId = User.Identity.GetUserId();
                    aCC_Service_Desc.Del_Flag = false;
                    db.ACC_Service_Descs.Add(aCC_Service_Desc);
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 1);
                    return RedirectToAction("Index");
                }
            dropdown:
                //dropdown
                IEnumerable<SelectListItem> items = db.ACC_Services.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Acc_Description

                });
                ViewBag.ACC_Service =items;

                return View(aCC_Service_Desc);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
            
        }

        // GET: ACC_Service_Desc/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {

                List<ACC_Service> acc_service = db.ACC_Services.Where(d => d.Del_Flag == false).ToList();
                IEnumerable<SelectListItem> items = acc_service.Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text = a.Acc_Description

                });
                ViewBag.ACC_Service = items;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ACC_Service_Desc aCC_Service_Desc = db.ACC_Service_Descs.Find(id);
                if (aCC_Service_Desc == null)
                {
                    return HttpNotFound();
                }
                return View(aCC_Service_Desc);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
            
        }

        // POST: ACC_Service_Desc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Account_Services_ID,Desc,CreatedDate,CreatedUserId,UpdatedDate,UpdatedUserId,Del_Flag,OrderNo")] ACC_Service_Desc aCC_Service_Desc)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //check OrderNo is duplicated or not
                    bool reg = true;
                    if (db.ACC_Service_Descs.Where(d=>d.OrderNo==aCC_Service_Desc.OrderNo && d.Account_Services_ID== aCC_Service_Desc.Account_Services_ID && d.ID != aCC_Service_Desc.ID && d.Del_Flag==false).Count()>0)
                    {
                        ModelState.AddModelError("OrderNo","Duplicate Order No.!");
                        reg = false;
                    }

                    if(reg==false)
                    {
                        //return View(aCC_Service_Desc)
                        goto dropdown;
                    }


                    ACC_Service_Desc acc_desc = db.ACC_Service_Descs.Find(aCC_Service_Desc.ID);
                    acc_desc.Account_Services_ID = aCC_Service_Desc.Account_Services_ID;
                    acc_desc.Desc = aCC_Service_Desc.Desc;
                    acc_desc.OrderNo = aCC_Service_Desc.OrderNo;
                    acc_desc.UpdatedDate = DateTime.Now;
                    acc_desc.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(acc_desc).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            dropdown:
                //dropdown
                IEnumerable<SelectListItem> item = db.ACC_Services.Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text = a.Acc_Description
                });

                ViewBag.Acc_Service = item;
                return View(aCC_Service_Desc);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }

        }

        // GET: ACC_Service_Desc/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ACC_Service_Desc aCC_Service_Desc = db.ACC_Service_Descs.Find(id);
                if (aCC_Service_Desc == null)
                {
                    return HttpNotFound();
                }
                return View(aCC_Service_Desc);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }

        }

        // POST: ACC_Service_Desc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ACC_Service_Desc aCC_Service_Desc = db.ACC_Service_Descs.Find(id);
                aCC_Service_Desc.Del_Flag = true;
                aCC_Service_Desc.UpdatedDate = DateTime.Now;
                aCC_Service_Desc.UpdatedUserId = User.Identity.GetUserId();
                //db.ACC_Service_Descs.Remove(aCC_Service_Desc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }

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
