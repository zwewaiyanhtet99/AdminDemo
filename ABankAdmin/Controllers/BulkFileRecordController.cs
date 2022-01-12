using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System.Linq.Dynamic;

namespace ABankAdmin.Controllers
{
    public class BulkFileRecordController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "BulkFileRecordController";
        EventLogController log = new EventLogController();

        // GET: BulkFileRecord
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //List<C_Corporate> corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();
                //IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                //{
                //    Value = a.ID.ToString(),
                //    Text = a.COMPANY_NAME

                //});
                //ViewBag.Corporate = items;
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //var c_Bulk_File_Records = db.C_Bulk_File_Records.Include(c => c.Vrcorporate);
            //return View(c_Bulk_File_Records.ToList());
        }

        //Post Bulk File Record
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
            var company = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            var bulkfile = (from b in db.C_Bulk_File_Records
                          where b.DEL_FLAG == false
                          & b.Vrcorporate.DEL_FLAG == false
                            select b);

            //Searchig
            if (!string.IsNullOrEmpty(company))
            {
                //int id = Convert.ToInt32(company);
                //bulkfile = bulkfile.Where(b=>b.Corporate_ID==id);
                bulkfile = bulkfile.Where(b => b.Vrcorporate.COMPANY_NAME.Trim().ToLower().Contains(company.ToLower()));
            }
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                bulkfile = bulkfile.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = bulkfile.Count();
            var data = bulkfile.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: BulkFileRecord/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                C_Bulk_File_Record c_Bulk_File_Record = db.C_Bulk_File_Records.Find(id);
                if (c_Bulk_File_Record == null)
                {
                    return HttpNotFound();
                }
                List<C_Corporate> corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text = a.COMPANY_NAME

                });
                ViewBag.Corporate = items;
                //ViewBag.Corporate_ID = new SelectList(db.C_Corporates, "ID", "CIFID", c_Bulk_File_Record.Corporate_ID);
                return View(c_Bulk_File_Record);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: BulkFileRecord/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Max_Record")] C_Bulk_File_Record c_Bulk_File_Record)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    C_Bulk_File_Record bulkfiledb = db.C_Bulk_File_Records.Find(c_Bulk_File_Record.ID);
                    //bulkfiledb.Corporate_ID = c_Bulk_File_Record.Corporate_ID;
                    bulkfiledb.Max_Record = c_Bulk_File_Record.Max_Record;
                    bulkfiledb.UpdatedDateTime = DateTime.Now;
                    bulkfiledb.UpdatedUserID = User.Identity.GetUserId();
                    db.Entry(bulkfiledb).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    //show success message
                    List<C_Corporate> corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();
                    IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                    {
                        Value = a.ID.ToString(),
                        Text = a.COMPANY_NAME

                    });
                    ViewBag.Corporate = items;
                    //Null value find
                    C_Bulk_File_Record c_Bulk_File_Record_Name = db.C_Bulk_File_Records.Find(c_Bulk_File_Record.ID);
                    ViewBag.bulkfilerecord ="\""+ c_Bulk_File_Record_Name.Vrcorporate.COMPANY_NAME+"\"";
                    return View(c_Bulk_File_Record);
                }
                //ViewBag.Corporate_ID = new SelectList(db.C_Corporates, "ID", "CIFID", c_Bulk_File_Record.Corporate_ID);
                return View(c_Bulk_File_Record);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
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
