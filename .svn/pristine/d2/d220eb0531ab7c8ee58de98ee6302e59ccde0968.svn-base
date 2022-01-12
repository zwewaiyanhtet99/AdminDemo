using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System.Linq.Dynamic;
using ABankAdmin.Core.Utils;

namespace ABankAdmin.Controllers
{
    public class UtilitiesController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "UtilitiesController";
        EventLogController log = new EventLogController();

        string Image_Path = ConstantValues.Image_Path;
        string Image_Domain = ConstantValues.Image_Domain;
        string Utility_Path { get { return "\\Images\\Bill_Payment\\"; } }

        // GET: Utilities/Index
        public ActionResult Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //IEnumerable<SelectListItem> items = db.Rules.Select(r => new SelectListItem
                //{
                //    Value = r.Code,
                //    Text = r.Description

                //});
                IEnumerable<SelectListItem> utype = db.UltTypes.Select(u => new SelectListItem
                {
                    Value = u.TypeName,
                    Text = u.TypeName

                });
                //ViewBag.Rule = items;
                ViewBag.UltType = utype;
                return View();
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(db.Utilities.ToList());
        }

        //Post/Utilities/Index
        [HttpPost]
        public ActionResult Index(int? arg1) //adding parameter is in order to use Action name 'Index' same with View action name
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
            var name = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var billercode = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var remark = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            //var discode = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var dispercent = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var utype = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var utility = (from u in db.Utilities
                            where u.Active == "1"
                            select u);

            //foreach (var c in utility.ToList())
            //{
            //    var rule = db.Rules.Where(t => t.Code == c.DiscountCode).FirstOrDefault();
            //    c.DiscountCode = rule == null ? "" : rule.Description;

            //}
            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                utility = utility.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = utility.Count();
            var data = utility.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Utilities/Details/5
        //public ActionResult Details(int? id)
        //{
        //    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Utilities utilities = db.Utilities.Find(id);
        //        if (utilities == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(utilities);
        //    }
        //    catch(Exception ex)
        //    {
        //        log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
        //        return View("Error");
        //    }
        //}

        // GET: Utilities/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //rule data to bind discount code dropdown list
                //var rule =db.Rules.Where(r => r.Description.Contains("DiscountPercentage") && r.Del_Flag == "0").ToList();
                var ulttype = db.UltTypes.ToList();
                //IEnumerable<SelectListItem> items =rule.Select(r => new SelectListItem
                //{
                //    Value =r.Code,
                //    Text = r.Description

                //});
                IEnumerable<SelectListItem> utype = db.UltTypes.Select(u => new SelectListItem
                {
                    Value = u.TypeName,
                    Text = u.TypeName

                });
                //ViewBag.Rule = items;
                ViewBag.UltType = utype;
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: Utilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ImagePath,Active,Biller_Code,Remark,Utility_Type,DiscountPercent")] Utilities utilities, HttpPostedFileBase file)
        {
            string filename;
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //check Name is duplicated or not
                    if (db.Utilities.Where(d => d.Name == utilities.Name && d.ID != utilities.ID && d.Active == "1").Count() > 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate Name!");
                        //return View(utilities);
                        goto dropdown;
                    }
                    //check Biller Code is duplicated or not
                    if (db.Utilities.Where(d => d.Biller_Code == utilities.Biller_Code && d.ID != utilities.ID && d.Active == "1").Count() > 0)
                    {
                        ModelState.AddModelError("Biller_Code", "Duplicate Biller Code!");
                        //return View(utilities);
                        goto dropdown;
                    }

                    if (file != null && file.ContentLength > 0)
                    {
                        var postedFileExtension = Path.GetExtension(file.FileName);
                        filename = Path.GetFileName(file.FileName);
                        if (postedFileExtension.Equals(".jpg") || postedFileExtension.Equals(".JPG") || postedFileExtension.Equals(".png") || postedFileExtension.Equals(".PNG") || postedFileExtension.Equals(".jpeg") || postedFileExtension.Equals(".JPEG"))
                        {
                            var ult = ConstantValues.GetGUID();
                            filename = $"{ult}{postedFileExtension}";
                            string filepath = $"\\Images\\Bill_Payment\\{filename}";
                            string path = $"{Image_Path}{filepath}";
                            string createfolder = $"{Image_Path}\\Images\\Bill_Payment";
                            //create if folder doesn't exit
                            if (!System.IO.Directory.Exists(createfolder))
                            {
                                System.IO.Directory.CreateDirectory(createfolder);
                            }
                            //Saving file to Folder
                            file.SaveAs(path);

                            utilities.Active = "1";
                            utilities.ImagePath = filepath;
                            utilities.CreatedDateTime = DateTime.Now;
                            utilities.CreatedUserId = User.Identity.GetUserId();
                            db.Utilities.Add(utilities);
                            db.SaveChanges();
                            //save log
                            log.Info(User.Identity.GetUserId(), controllerName, actionName, 1);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("ImagePath", "Image must be ('.jpg', '.png') format.");
                            return View(utilities);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImagePath", "Image is required.");
                        goto dropdown;
                        //return View(utilities);
                    }
                }
                dropdown:
                //rule data to bind discount code dropdown list
                //var rule = db.Rules.Where(r => r.Description.Contains("DiscountPercentage") && r.Del_Flag == "0").ToList();
                //IEnumerable<SelectListItem> items = rule.Select(r => new SelectListItem
                //{
                //    Value = r.Code,
                //    Text = r.Description

                //});
                //utility type data to bind utility type dropdown list
                var ulttype = db.UltTypes.ToList();
                IEnumerable<SelectListItem> utype = db.UltTypes.Select(u => new SelectListItem
                {
                    Value = u.TypeName,
                    Text = u.TypeName

                });
                //ViewBag.Rule = items;
                ViewBag.UltType = utype;

                return View(utilities);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }

            //return View(utilities);
        }

        // GET: Utilities/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Utilities ult= db.Utilities.Where(a => a.Active == "1" && a.ID == id).FirstOrDefault();
                //rule data to bind discount code dropdown list
                //var lstrule = db.Rules.Where(r=>r.Description.Contains("DiscountPercentage")&& r.Del_Flag=="0").ToList();
                //IEnumerable<SelectListItem> items = lstrule.Select(a => new SelectListItem
                //{
                //    Value = a.Code,
                //    Text = a.Description

                //});
                //utility type data to bind utility type dropdown list
                var ulttype = db.UltTypes.ToList();
                IEnumerable<SelectListItem> utype = db.UltTypes.Select(u => new SelectListItem
                {
                    Value = u.TypeName,
                    Text = u.TypeName

                });
                //ViewBag.Rule = items;
                ViewBag.UltType = utype;
                //Utilities utilities = db.Utilities.Find(id);
                if (ult == null)
                {
                    return HttpNotFound();
                }
                ult.ImagePath = Image_Domain + ult.ImagePath;
                return View(ult);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
            //return View(utilities);
        }

        // POST: Utilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ImagePath,Active,Biller_Code,Remark,Utility_Type,DiscountPercent")] Utilities utilities, HttpPostedFileBase file)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    Utilities lstult = db.Utilities.Where(a => a.Active == "1" && a.ID == utilities.ID).FirstOrDefault();
                    if (lstult == null)
                        return HttpNotFound();

                    //check Name is duplicated or not
                    if (db.Utilities.Where(d => d.Name == utilities.Name && d.ID != utilities.ID && d.Active == "1").Count() > 0)
                    {
                        ModelState.AddModelError("Name", "Duplicate Name!");
                        //return View(utilities);
                        goto dropdown;
                    }
                    //check Biller Code is duplicated or not
                    if (db.Utilities.Where(d =>d.Biller_Code == utilities.Biller_Code && d.ID != utilities.ID && d.Active == "1").Count() > 0)
                    {
                        ModelState.AddModelError("Biller_Code", "Duplicate Biller Code.!");
                        //return View(utilities);
                        goto dropdown;
                    }

                    if (file != null)
                    {
                        var postedFileExtension = Path.GetExtension(file.FileName);
                        if (postedFileExtension.Equals(".jpg") || postedFileExtension.Equals(".JPG") || postedFileExtension.Equals(".png") || postedFileExtension.Equals(".PNG") || postedFileExtension.Equals(".jpeg") || postedFileExtension.Equals(".JPEG"))
                        {
                            string createfolder = $"{Image_Path}\\Images\\Bill_Payment";
                            var ultname = ConstantValues.GetGUID();
                            string name = $"{ultname}{postedFileExtension}";
                            //create if folder doesn't exist
                            if (!System.IO.Directory.Exists(createfolder))
                            {
                                System.IO.Directory.CreateDirectory(createfolder);
                            }

                            //Saving file to Folder
                            string filepath = $"\\Images\\Bill_Payment\\{name}";
                            string path = $"{Image_Path}{filepath}";
                            file.SaveAs(path);

                            lstult.ImagePath = filepath;
                        }
                        else
                        {
                            ModelState.AddModelError("ImagePath", "Image must be ('.jpg', '.png') format.");
                            goto dropdown;
                            //return View(utilities);
                        }
                    }


                    Utilities ult = db.Utilities.Find(utilities.ID);
                    ult.Name = utilities.Name;
                    ult.Biller_Code = utilities.Biller_Code;
                    ult.Remark = utilities.Remark;
                    //ult.DiscountCode = utilities.DiscountCode;
                    ult.Utility_Type = utilities.Utility_Type;
                    ult.UpdatedDateTime = DateTime.Now;
                    ult.UpdatedUserId = User.Identity.GetUserId();
                    ult.DiscountPercent = utilities.DiscountPercent;
                    db.Entry(ult).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    log.Info(User.Identity.GetUserId(), controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        dropdown:
            //rule data to bind discount code dropdown list
            //var rule = db.Rules.Where(r => r.Description.Contains("DiscountPercentage") && r.Del_Flag == "0").ToList();
            //IEnumerable<SelectListItem> items =rule.Select(r => new SelectListItem
            //{
            //    Value = r.Code,
            //    Text = r.Description

            //});
            //utility type data to bind utility type dropdown list
            var ulttype = db.UltTypes.ToList();
            IEnumerable<SelectListItem> utype = db.UltTypes.Select(u => new SelectListItem
            {
                Value = u.TypeName,
                Text = u.TypeName

            });
            //ViewBag.Rule = items;
            ViewBag.UltType = utype;
            return View(utilities);
        }

        // GET: Utilities/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Utilities utilities = db.Utilities.Find(id);
                Utilities ult = db.Utilities.Where(a => a.Active == "1" && a.ID == id).FirstOrDefault();
                //var rule = db.Rules.Where(r => r.Code == ult.DiscountCode).FirstOrDefault();
                //ult.DiscountCode = rule == null ? "" : rule.Description;
                if (ult == null)
                {
                    return HttpNotFound();
                }
                ult.ImagePath = Image_Domain + ult.ImagePath;
                return View(ult);
            }
            catch(Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            } 
        }

        // POST: Utilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                Utilities utilities = db.Utilities.Find(id);
                utilities.Active = "0";
                utilities.UpdatedDateTime = DateTime.Now;
                utilities.UpdatedUserId = User.Identity.GetUserId();
                string Image_Path = ConstantValues.Image_Path;
                //string imagePath = System.Configuration.ConfigurationManager.AppSettings["Image_Path"];
                string filepath = utilities.ImagePath;
                string path = $"{Image_Path}{filepath}";
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                // System.IO.File.Delete(path);
                //db.Utilities.Remove(utilities);
                db.SaveChanges();
                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
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
