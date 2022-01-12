using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
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
using System.Linq.Dynamic;
using ABankAdmin.Core.Utils;

namespace ABankAdmin.Controllers
{
    public class ADSController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "ADSController";
        EventLogController log = new EventLogController();

        string Image_Path = ConstantValues.Image_Path;
        string Image_Domain = ConstantValues.Image_Domain;
        //string adsPhotoPath = ConfigurationManager.AppSettings["ADSPhotoPath"];
        //string adsPhotoDomain = ConfigurationManager.AppSettings["ADSPhotoDomain"];
        // GET: ADS
        public ActionResult Index()
        {
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
            var townshipCode = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Code = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Desc = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            //var city = db.Cities.ToList();
            var ads = (from v in db.ADSs
                        where v.Active==true
                        select v);

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                ads = ads.OrderBy(sortColumn + " " + sortColumnDir);
            }


            recordsTotal = ads.Count();
            var data = ads.Skip(skip).Take(pageSize).ToList();


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);

        }

        // GET: ADS/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var result = from s in db.ADSs
        //                 where s.Active == true
        //                 select s;
        //    ADS ads = result.SingleOrDefault(m => m.ID == id);
        //    if (ads == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ads);
        //}

        // GET: ADS/Create
        public ActionResult Create()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ADS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ImageName,ImageRedirectLink,OrderNo,ImageUrl")] ADS aDS, HttpPostedFileBase file)
        {
            string filename;
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string userid = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var postedFileExtension = Path.GetExtension(file.FileName);
                        filename = Path.GetFileName(file.FileName);
                        if (postedFileExtension.Equals(".jpg") || postedFileExtension.Equals(".JPG") || postedFileExtension.Equals(".png") || postedFileExtension.Equals(".PNG") || postedFileExtension.Equals(".jpeg") || postedFileExtension.Equals(".JPEG"))
                        {
                            #region validation
                            //duplicate branch code
                            if (db.ADSs.Where(b => b.OrderNo == aDS.OrderNo && b.Active == true).Count() > 0)
                            {
                                ModelState.AddModelError("OrderNo", "Duplicate ADS OrderNo");
                                return View(aDS);
                            }
                            #endregion
                            //var ads = Guid.NewGuid().ToString();
                            //filename = $"{ads}{postedFileExtension}";
                            string filepath = $"\\Images\\ADS\\{filename}";
                            string path = $"{Image_Path}{filepath}";
                            string createfolder = $"{Image_Path}\\Images\\ADS";
                            //create if folder doesn't exist
                            if (!System.IO.Directory.Exists(createfolder))
                            {
                                System.IO.Directory.CreateDirectory(createfolder);
                            }
                            //Saving file to Folder 
                            file.SaveAs(path);

                            aDS.Active = true;
                            aDS.ImageType = "ADS";
                            aDS.CreatedDate = DateTime.Now;
                            aDS.CreatedUserID = userid;
                            aDS.ImageUrl = filepath;
                            db.ADSs.Add(aDS);
                            db.SaveChanges();

                            //save log
                            log.Info(userid, controllerName, actionName, 1);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("ImageUrl", "Image must be ('.jpg', '.png') format.");
                            return View(aDS);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "Image is required.");
                        return View(aDS);
                    }
                }
                return View(aDS);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: ADS/Edit/5
        public ActionResult Edit(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }              

                ADS ads = db.ADSs.Where(a => a.Active == true && a.ID == id).FirstOrDefault();
                if (ads == null)
                {
                    return HttpNotFound();
                }
                ads.ImageUrl = Image_Domain + ads.ImageUrl;
                return View(ads);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ADS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ImageUrl,ImageName,ImageRedirectLink,OrderNo")] ADS aDS, HttpPostedFileBase file)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    ADS ads = db.ADSs.Where(a=> a.Active == true && a.ID== aDS.ID).FirstOrDefault();                    
                    if(ads == null)
                        return HttpNotFound();

                    #region validation
                    //duplicate OrderNo
                    if (db.ADSs.Where(b => b.OrderNo == aDS.OrderNo && b.ID != aDS.ID && b.Active == true).Count() > 0)
                    {
                        ModelState.AddModelError("OrderNo", "Duplicate ADS OrderNo");
                        return View(aDS);
                    }
                    #endregion

                    if (file != null)
                    {
                        var postedFileExtension = Path.GetExtension(file.FileName);
                        if (postedFileExtension.Equals(".jpg") || postedFileExtension.Equals(".JPG") || postedFileExtension.Equals(".png") || postedFileExtension.Equals(".PNG") || postedFileExtension.Equals(".jpeg") || postedFileExtension.Equals(".JPEG"))
                        {
                            try
                            {
                                string createfolder = $"{Image_Path}\\Images\\ADS";
                                //var Image = Guid.NewGuid().ToString();
                                //string name = $"{Image}{postedFileExtension}";
                                string name = $"{file.FileName}";
                                string filepath = $"\\Images\\ADS\\{name}";
                                string path = $"{Image_Path}{filepath}";
                                //create if folder doesn't exist
                                if (!System.IO.Directory.Exists(createfolder))
                                {
                                    System.IO.Directory.CreateDirectory(createfolder);
                                }

                                //Saving file to Folder
                                file.SaveAs(path);

                                //Delete Image

                                string ExistingImg = $"{Image_Path}{ads.ImageUrl}";
                                if (System.IO.File.Exists(ExistingImg))
                                {
                                    System.IO.File.Delete(ExistingImg);
                                }
                                ads.ImageUrl = filepath;
                            }
                            catch (Exception ex)
                            {
                                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                                return View("Error");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ImageUrl", "Image must be ('.jpg', '.png') format.");
                            return View(aDS);
                        }

                    }
                    //string filename = System.IO.Path.GetFileName(file.FileName);/*Path.GetFileName(file.FileName);*/ 
                    //if (name == filename || filename == name && postedFileExtension == ".jpg" || filename == name && postedFileExtension == ".JPG" || filename == name && postedFileExtension == ".png" || filename == name && postedFileExtension == ".PNG" || filename == name && postedFileExtension == ".jpeg" || filename == name && postedFileExtension == ".JPEG")
                    //{
                    //    ModelState.AddModelError("ImageUrl", "Duplicate ADS OrderNo");
                    //    return View(aDS);
                    //    //System.IO.File.Delete(path);
                    //}

                    ads.ImageName = aDS.ImageName;
                    ads.ImageRedirectLink = aDS.ImageRedirectLink;
                    ads.OrderNo = aDS.OrderNo;
                    db.Entry(ads).State = EntityState.Modified;
                    db.SaveChanges();

                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid, controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(aDS);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: ADS/Delete/5
        public ActionResult Delete(int? id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                ADS ads = db.ADSs.Where(a => a.Active == true && a.ID == id).FirstOrDefault();
                if (ads == null)
                {
                    return HttpNotFound();
                }
                ads.ImageUrl = Image_Domain + ads.ImageUrl;
                return View(ads);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: ADS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                ADS ads = db.ADSs.Where(a => a.Active == true && a.ID == id).FirstOrDefault();
                //string imagePath = System.Configuration.ConfigurationManager.AppSettings["ADSPhotoPath"];
                string Image_Path = ConstantValues.Image_Path;
                string filepath = ads.ImageUrl;
                string path = $"{Image_Path}{filepath}";
                ads.Active = false;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                   // System.IO.File.Delete(path);
                //db.ADSs.Remove(ads);
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
