using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
//for pdfsharp
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Linq.Dynamic;

//for pdfsharp
using PdfSharp;
using PdfSharp.Pdf.IO;
using System.IO;
using PdfSharp.Drawing.Layout;

namespace ABankAdmin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private AdminSignInManager _signInManager;
        private AdminUserManager _userManager;
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "AdminController";
        EventLogController log = new EventLogController();
        private int AdminRoleID = ConstantValues.AdminRoleID;

        public AdminController()
        {
            
        }

        public AdminController(AdminUserManager userManager, AdminSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AdminSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AdminSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AdminUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AdminUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin
        public ActionResult Index()
        {         
            ViewBag.Branch = GetBranchSelectList();//get branch name
            ViewBag.Role = GetRoleSelectList();// get role   
            //carry pdf filename to download or null
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return View();
        }

        //Post :Admin
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
            var username = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var branch = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var role = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            var adminUser = db.Users.Where(a => a.DEL_FLAG == false).ToList().ConvertAll(u => new AdminViewModel
            {
                ID = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                BranchID = u.BranchID,
                Branch = u.VrBranch.name,//to show Branch Name at Index
                Role = u.Role,
                StaffID = u.StaffID,
                Phone = u.Phone,
                Email = u.Email,
                IsLock = u.LockoutEndDateUtc.HasValue,
                RoleName=u.VrRole.Name
            });

            //SEARCHING...

            if (!string.IsNullOrEmpty(username.Trim().ToLower()))
            {
                adminUser = adminUser.Where(a => a.UserName.ToLower().Contains(username.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(branch))
            {
                int branchid = Convert.ToInt32(branch);
                adminUser = adminUser.Where(a => a.BranchID == branchid).ToList();
            }
            if (!string.IsNullOrEmpty(role))
            {
                int roleid = Convert.ToInt32(role);
                adminUser = adminUser.Where(a => a.Role == roleid).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                adminUser = adminUser.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = adminUser.Count();
            var data = adminUser.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        //Edit
        public ActionResult Edit(string id) 
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null || id=="")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AdminUser admin = db.Users.Where(a=>a.Id==id & a.DEL_FLAG==false).FirstOrDefault();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                AdminViewModel adminvm = new AdminViewModel {
                    ID = admin.Id,
                    UserName = admin.UserName,
                    FullName = admin.FullName,
                    BranchID = admin.BranchID,
                    Role = admin.Role,
                    StaffID = admin.StaffID,
                    Phone = admin.Phone,
                    Email = admin.Email
                };
                var roles = new SelectList(db._Roles.ToList(), "ID", "Name");
                ViewBag.Role = roles;               
                ViewBag.Branch = GetBranchSelectList();
                return View(adminvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminViewModel adminvm)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    //validation
                    AdminUser dbadmin = db.Users.Where(a => a.Id == adminvm.ID & a.DEL_FLAG == false).FirstOrDefault();
                    if (dbadmin == null)
                    {
                        return HttpNotFound();
                    }

                    if (adminvm.StaffID != null)
                    {
                        adminvm.StaffID = adminvm.StaffID.Trim();
                    }

                    #region validation
                    //Duplicate StaffID
                    if (!String.IsNullOrEmpty(adminvm.StaffID) && (db.Users.Where(u => u.StaffID == adminvm.StaffID && u.Id != adminvm.ID && u.DEL_FLAG == false)).Count() > 0) //dbadmin.StaffID == adminvm.StaffID
                    {
                        ModelState.AddModelError("StaffID", "Duplicate StaffID");
                        ViewBag.Branch = GetBranchSelectList();
                        ViewBag.Role = GetRoleSelectList();
                        return View(adminvm);
                    }
                    #endregion
                    //update field what u want to be allowed to edit  (from formdata)
                    dbadmin.FullName = adminvm.FullName;
                    dbadmin.BranchID = adminvm.BranchID;
                    dbadmin.Role = adminvm.Role;
                    dbadmin.StaffID = adminvm.StaffID;
                    dbadmin.Phone = adminvm.Phone;
                    dbadmin.Email = adminvm.Email;
                    //default fields
                    dbadmin.UpdatedDateTime = DateTime.Now;
                    dbadmin.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(dbadmin).State = EntityState.Modified;
                    db.SaveChanges();
                    //save log
                    string getuserid = User.Identity.GetUserId();
                    log.Info(getuserid,controllerName, actionName, 2);
                    return RedirectToAction("Index");
                }
                return View(adminvm);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                AdminUser adminuser = db.Users.Where(u => u.DEL_FLAG == false && u.Id == id).FirstOrDefault();
                if (adminuser == null)
                {
                    return HttpNotFound();
                }
                AdminViewModel adminvm = new AdminViewModel
                {
                    ID = adminuser.Id,
                    UserName = adminuser.UserName,
                    FullName = adminuser.FullName,
                    //BranchID = adminuser.BranchID,
                    Branch = adminuser.VrBranch.name,
                    Role = adminuser.Role,
                    StaffID = adminuser.StaffID,
                    Phone = adminuser.Phone,
                    Email = adminuser.Email
                };

                return View(adminvm);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                AdminUser adminuser = db.Users.Find(id);
                //delete user
                adminuser.DEL_FLAG = true;
                adminuser.UpdatedDateTime = DateTime.Now;
                adminuser.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(adminuser).State = EntityState.Modified;
                db.SaveChanges();
                //save log
                string getuserid = User.Identity.GetUserId();
                log.Info(getuserid,controllerName, actionName, 3, id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult ResetPwd(string id)
        {
            try
            {
                AdminUser adminuser = db.Users.Where(a => a.Id == id & a.DEL_FLAG == false).FirstOrDefault();
                if (adminuser == null)
                {
                    Response.StatusCode = 500;
                    return Json("Reset password fail!", JsonRequestBehavior.AllowGet);
                }

                string LoginPWD = GeneratePassword.Generate(8, true, true, true);
                //hash password
                String hashedPassword = UserManager.PasswordHasher.HashPassword(LoginPWD);
                //update password
                adminuser.PasswordHash = hashedPassword;
                //generate branch name
                var branchName = db.Branches.Find(adminuser.BranchID);
                //generate row
                var rolename = db._Roles.Find(adminuser.Role);
                //force change
                adminuser.ForceChange_Flag = true;
                //update fail counts
                adminuser.AccessFailedCount = 0;

                adminuser.UpdatedDateTime = DateTime.Now;
                adminuser.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(adminuser).State = EntityState.Modified;
                db.SaveChanges();
                //generate pdf
                string filename = GeneratePdf(adminuser.StaffID, adminuser.UserName, adminuser.FullName, LoginPWD,branchName.name,rolename.Name,adminuser.Phone,adminuser.Email,1);

                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId(); // to get login user id
                log.Log(getuserid, controllerName, actionName, $"Resetting Password successfully.",$"User name is {adminuser.UserName}.");

                return Json(new { msg = $"New Password is {LoginPWD}", filename = filename }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                Response.StatusCode = 500;
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Lock(string id)
        {
            try
            {
                AdminUser adminuser = db.Users.Where(a => a.Id == id & a.DEL_FLAG == false).FirstOrDefault();
                if (adminuser == null)
                {
                    return Json("Lock fail!", JsonRequestBehavior.AllowGet);
                }
                //lock
                adminuser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(365 * 200);//update LockoutEndDate

                adminuser.UpdatedDateTime = DateTime.Now;
                adminuser.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(adminuser).State = EntityState.Modified;
                db.SaveChanges();
                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId(); // to get login user id
                log.Log(getuserid, controllerName, actionName, $"Lock successfully.",$"User name is {adminuser.UserName}.");

                return Json("Lock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Unlock(string id)
        {
            try
            {
                AdminUser adminuser = db.Users.Where(a => a.Id == id & a.DEL_FLAG == false).FirstOrDefault();
                if (adminuser == null)
                {
                    return Json("Unlock fail!", JsonRequestBehavior.AllowGet);
                }
                //unlock
                adminuser.LockoutEndDateUtc = null;
                //reset fail counts
                adminuser.AccessFailedCount = 0;

                adminuser.UpdatedDateTime = DateTime.Now;
                adminuser.UpdatedUserId = User.Identity.GetUserId();
                db.Entry(adminuser).State = EntityState.Modified;
                db.SaveChanges();
                //log
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string getuserid = User.Identity.GetUserId(); // to get login user id
                log.Log(getuserid, controllerName, actionName, $"Unlock successfully.",$"User name is {adminuser.UserName}.");

                return Json("Unlock successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string uid = User.Identity.GetUserId();
                log.Error(uid,controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            try
            {
                ViewBag.Branch = GetBranchSelectList();
                ViewBag.Role = GetRoleSelectList();
                return View();
            }
            catch (Exception ex)
            {
                string id= User.Identity.GetUserId();
                log.Error(id,controllerName, "Register", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //validation
                    #region Validation
                    //space and special char
                    Regex rgx = new Regex("^[a-zA-Z0-9]+?$");
                    if (!rgx.IsMatch(model.UserName))
                    {
                        ModelState.AddModelError("", "UserName isn't allowed space and special character.");
                        goto ValidationFail;
                    }
                    //length
                    rgx = new Regex("^([a-zA-Z0-9]){6,16}$");
                    if (!rgx.IsMatch(model.UserName))
                    {
                        ModelState.AddModelError("", "UserName must have at least 6 characters and at most 16 characters.");
                        goto ValidationFail;
                    }
                    //staring with number
                    rgx = new Regex("^[a-zA-Z]+([a-zA-Z0-9]){5,15}$");

                    if (!rgx.IsMatch(model.UserName))
                    {
                        ModelState.AddModelError("", "Invalid UserName, UserName isn't allowed to start with number.");
                        goto ValidationFail;
                    }

                    if (model.StaffID != null)
                    {
                        model.StaffID = model.StaffID.Trim();
                    }

                    //Duplicate StaffID
                    if (!String.IsNullOrEmpty(model.StaffID) && db.Users.Where(u => u.StaffID != null && u.StaffID == model.StaffID && u.DEL_FLAG == false).Count() > 0) //dbadmin.StaffID == adminvm.StaffID
                    {
                        ModelState.AddModelError("StaffID", "Duplicate StaffID");
                        ViewBag.Branch = GetBranchSelectList();
                        ViewBag.Role = GetRoleSelectList();
                        return View(model);
                    }
                    #endregion
                    var user = new AdminUser { UserName = model.UserName, Email = model.Email, FullName = model.FullName, BranchID = model.BranchID, Role = model.Role, StaffID = model.StaffID, Phone = model.Phone, CreatedDateTime= DateTime.Now, ForceChange_Flag=true };

                    //user.DEL_FLAG = false;
                    user.CreatedUserId = User.Identity.GetUserId();
                    //generate pwd
                    string pwd = GeneratePassword.Generate(8, true, true, true);
                    //generate branch name
                    var branchname = db.Branches.Find(model.BranchID);
                    //generate role name
                    var roleName = db._Roles.Find(model.Role);
                    var result = await UserManager.CreateAsync(user, pwd);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //return RedirectToAction("Index", "Home");
                        //no need to login directly

                        //generate pdf
                        string filename = GeneratePdf(model.StaffID,model.UserName,model.FullName,pwd,branchname.name,roleName.Name,model.Phone,model.Email,0);// for register,  status(0) parameter no need
                        //save log
                        string getuserid = User.Identity.GetUserId();
                        log.Info(getuserid,controllerName, "Register", 1, $"User ID is { user.UserName}.");

                        //carry filename to Index
                        TempData["filename"] = filename;
                        return RedirectToAction("Index");
                    }
                    AddErrors(result);
                }
    ValidationFail:
                ViewBag.Branch = GetBranchSelectList();
                ViewBag.Role = GetRoleSelectList();
                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                string id = User.Identity.GetUserId();
                log.Error(id,controllerName, "Register", ex);
                return View("Error");
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //to check username exist or deleted user or active user?    DEL_FLAG, Active
                var admin = db.Users.Where(a => a.UserName == model.UserName & a.DEL_FLAG == false).FirstOrDefault();
                if (admin == null)
                {
                    ModelState.AddModelError("", "UserName doesn't exist.");
                    return View(model);
                }

                #region check if username/password pair match.
                // check admin user is exist or not
                var loginAdmin = await UserManager.FindAsync(model.UserName, model.Password);
                if (loginAdmin != null)
                {
                    // change the security stamp only on correct username/password
                    await UserManager.UpdateSecurityStampAsync(loginAdmin.Id);
                }
                #endregion

                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
                switch (result)
                {
                    case SignInStatus.Success:
                        //force change flag
                        Boolean forceChangeFlag = admin.ForceChange_Flag;
                        Session["forcechange"] = forceChangeFlag;   //store force change state

                        //Role
                        Session["role"] = admin.Role;

                        //dynamic allowed permissions
                        if (admin.Role != AdminRoleID)    //if not administrator
                        {
                            IEnumerable<Menu> permissions = null;
                            permissions = GetPermissions(admin.Role);
                            Session["permissions"] = permissions;   //to store allowed menus for current role
                        }
                         //successful login
                        log.Log(admin.Id, controllerName, "Login", $"Login successfully.",$"User name is {admin.UserName}.");
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "Your account is locked.");
                        return View(model);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "UserName and password don't match.");
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("",controllerName, "Login", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult LogOff()
        {
            //clear session
            Session.Abandon();
            Session.Clear();
            //successful logout
            log.Log(User.Identity.GetUserId(), controllerName,"Logout", $"LogOut successfully.",$"User Name is {User.Identity.GetUserName()}.");
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            return RedirectToAction("Index", "Home");
        }

        // GET: /Admin/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Admin/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    //update ForceChange_Flag to false
                    AdminUser admin = db.Users.Find(User.Identity.GetUserId());
                    admin.ForceChange_Flag = false;
                    //default fields
                    admin.UpdatedDateTime = DateTime.Now;
                    admin.UpdatedUserId = User.Identity.GetUserId();
                    db.Entry(admin).State = EntityState.Modified;
                    db.SaveChanges();

                    //update forcechange session
                    Session["forcechange"] = false;

                    //log
                    //actionName is already call before try method
                    string getuserid = User.Identity.GetUserId(); // to get login user id
                    log.Log(getuserid, controllerName, actionName, $"Changing Password successfully.",$"User name is {admin.UserName}.");

                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                  
                    //carry success message
                    TempData["message"] = "Your password has been changed successfully.";
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }
        
        [AllowAnonymous]
        public virtual PartialViewResult Menu()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            IEnumerable<Menu> Menu = null;
            try
            {
                if (Request.IsAuthenticated && Session["menus"] != null)
                {
                    Menu = (IEnumerable<Menu>)Session["menus"];
                }
                else if (Request.IsAuthenticated)
                {
                    //get role of logged in User
                    string userid = User.Identity.GetUserId();
                    AdminUser admin = db.Users.Find(userid);
                    if (admin == null)
                        return PartialView("_Menu", Menu);
                    int role = admin.Role;//get role   1,2,3
                    Menu = GetMenus(role);
                    Session["menus"] = Menu;
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
            }
            return PartialView("_Menu", Menu);
        }

        //to get authorized menus
        private IEnumerable<Menu> GetMenus(int role)
        {
            IEnumerable<Menu> menus = null;
            try
            {
                if (role == AdminRoleID)//all visible menus for administrator
                {
                    menus = db.Menus.Where(m => m.Active == true && m.Visible == true).ToList();//get all visible menus from tbl_menu
                }
                else//if not administrator, get menus from rolemenu table
                {
                    //get menus from TBL_MENU join with TBL_ROLEMENU by roleid
                    menus = (from m in db.Menus
                            join rm in db.RoleMenus on m.ID equals rm.MenuID into tblmenus
                            from tblrm in tblmenus.ToList()
                            where m.Visible == true && m.Active == true && tblrm.RoleID == role
                            select m)
                            .Union(
                            from parents in db.Menus
                            join child in db.Menus on parents.ID equals child.Parent_ID
                            join rm in db.RoleMenus on child.ID equals rm.MenuID into tblmenus
                            from tblrm in tblmenus.ToList()
                            where parents.Visible == true && parents.Active == true && tblrm.RoleID == role
                            select parents
                            );
                    //to get top menus (#)
                    menus = menus
                        .Union(from m in db.Menus
                               join parent in menus on m.ID equals parent.Parent_ID into tblmenus
                               from tblrm in tblmenus.ToList()
                               where m.Visible == true && m.Active == true && m.Controller == "#"
                               select m
                        );
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "GetMenus", ex);
            }
            return menus;
        }

        //to get authorized permissions
        private IEnumerable<Menu> GetPermissions(int role)
        {
            IEnumerable<Menu> permissions = null;
            try
            {
                //get all active permissions from TBL_MENU join with TBL_ROLEMENU by roleid
                permissions = from m in db.Menus
                              join rm in db.RoleMenus on m.ID equals rm.MenuID into tblmenus
                              from tblrm in tblmenus.ToList()
                              where m.Active == true && tblrm.RoleID == role
                              select m;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, "GetPermissions", ex);
            }
            return permissions;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        //getting Role list
        private IEnumerable<SelectListItem> GetRoleSelectList()
        {
            try
            {
                //List<SelectListItem> items = new List<SelectListItem>();
                //items.Add(new SelectListItem { Text = "Maker", Value = "0"});
                //items.Add(new SelectListItem { Text = "Checker", Value = "1" });
                //items.Add(new SelectListItem { Text = "Administrator", Value = "2" });               
                ////change to IEnumerable for auto selected
                //IEnumerable<SelectListItem> IEitems = items.Select(i => new SelectListItem
                //{
                //    Value = i.Value.ToString() ,
                //    Text = i.Text
                //});
                //ViewBag.Role = new SelectList(items.AsEnumerable(), "Value", "Text", IEitems);
                ////return IEitems;
                //return new SelectList(items, "Value", "Text", IEitems);
                List<Role> lstrole = db._Roles.ToList();
                IEnumerable<SelectListItem> roles = lstrole.Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.Name

                });
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "GetRoleSelectList", ex);
                return null;
            }
        }

        //getting Branch list
        private IEnumerable<SelectListItem> GetBranchSelectList()
        {
            try
            {
                List<Branch> lstbranch = db.Branches.Where(b => b.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> branches = lstbranch.Select(b => new SelectListItem
                {
                    Value = b.id.ToString(),
                    Text = b.name

                });
                return branches;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "GetBranchSelectList", ex);
                return null;
            }
        }

        //generate pdf      //Status 0 => register, 1 => reset
        [Obsolete]
        private string GeneratePdf(string StaffID,string Username,string Fullname,string LoginPwd,string BranchName,string UserRole,string MobileNo,string Email,int Status=0)
        {
            try
            {
                //string AdminPdf_Path = System.Configuration.ConfigurationManager.AppSettings["AdminPdfSavePath"];
                string AdminPdf_Path = ConstantValues.AdminPdf_Path;
                string filename = "";
                if (Status == 1)
                {
                    AdminPdf_Path = $"{AdminPdf_Path}\\reset password";
                    //create if folder doesn't exist
                    if (!Directory.Exists(AdminPdf_Path))
                    {
                        Directory.CreateDirectory(AdminPdf_Path);
                    }
                    //create a new pdf document
                    PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                    // Create an empty page
                    PdfSharp.Pdf.PdfPage page = document.AddPage();
                    //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                    //draw background image
                    DrawImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    // Create a font
                    XFont font = new XFont("Arial", 20, XFontStyle.Bold, options);

                    //Draw page border
                    //XRect A4Rect = new XRect(50, 0, 500, 750);
                    //gfx.DrawRectangle(XPens.Black,A4Rect);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    // Draw the text
                    gfx.DrawString("eBanking Admin User Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo, 1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    //pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Created Date", font, XBrushes.Black, new XRect(335, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(420, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Staff ID name
                    gfx.DrawString($"  Staff_ID", font, XBrushes.Black, new XRect(335, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Staff ID value
                    gfx.DrawString($": {StaffID}", font, XBrushes.Black, new XRect(420, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    // Draw the text
                    gfx.DrawString("Resetting Password", font, XBrushes.Black,
                      new XRect(55, 110, page.Width, 20),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    //XPen pen = new XPen(XColors.Indigo, 1.5);
                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 143, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 168, 50, 280);
                    gfx.DrawLine(pens, 50, 280, 550, 280);
                    gfx.DrawLine(pens, 550, 168, 550, 280);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Admin User Information", font, XBrushes.Black, new XRect(60, 130, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    // For Customer name
                    gfx.DrawString($"●  Full Name", font, XBrushes.Black, new XRect(60, 183, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {Fullname}", font, XBrushes.Black, new XRect(210, 183, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 203, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MobileNo}", font, XBrushes.Black, new XRect(210, 203, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 223, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {Email}", font, XBrushes.Black, new XRect(210, 223, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  Branch Name ", font, XBrushes.Black, new XRect(60, 243, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {BranchName}", font, XBrushes.Black, new XRect(210, 243, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    gfx.DrawRectangle(pen, 50, 320, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 420, 50, 345);
                    gfx.DrawLine(pens, 50, 420, 550, 420);
                    gfx.DrawLine(pens, 550, 420, 550, 345);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 310, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //gfx.DrawLine();                    
                    // Draw the text name
                    gfx.DrawString($"●  User Name ", font, XBrushes.Black,
                      new XRect(55, 355, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // Draw the text value
                    gfx.DrawString($": {Username}", font, XBrushes.Black,
                      new XRect(210, 355, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Create password font
                    XFont pwfont = new XFont("Myanmar3", 12, XFontStyle.Regular, options);
                    //Draw the text name
                    gfx.DrawString($"●  Login Password ", font, XBrushes.Black,
                        new XRect(55, 375, page.Width, 10),
                        XStringFormats.CenterLeft);
                    // Draw the text value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black,
                      new XRect(210, 375, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // Draw UserRole name
                    gfx.DrawString($"●  User Role ", font, XBrushes.Black,
                        new XRect(55, 395, page.Width, 10),
                        XStringFormats.CenterLeft);
                    // Draw UserRole value
                    gfx.DrawString($": {UserRole}", font, XBrushes.Black,
                        new XRect(210, 395, page.Width, 10),
                        XStringFormats.CenterLeft);

                    // for font create
                    font = new XFont("Arial", 12, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle = XDashStyle.Dot;
                    //gfx.DrawLine(pens, 50, 460, 155, 460);
                    gfx.DrawLine(pens, 330, 460, 225, 460);
                    //for password policies
                    //gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(50, 430, page.Width, 45),
                    //  XStringFormats.CenterLeft);
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 430, page.Width, 45),
                      XStringFormats.CenterLeft);
                    // for font create
                    font = new XFont("Arial", 10, XFontStyle.Regular, options);
                    //for password policies
                    //gfx.DrawString($"●    Minimum of 8 characters", font, XBrushes.Black, new XRect(70, 470, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Maximum of 16 characters", font, XBrushes.Black, new XRect(70, 485, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Uppercase letter", font, XBrushes.Black, new XRect(70, 500, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Lowercase letter", font, XBrushes.Black, new XRect(70, 515, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(70, 530, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(70, 545, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Login password must be different from transaction password", font, XBrushes.Black, new XRect(70, 560, page.Width, 10),
                    //  XStringFormats.CenterLeft);

                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 470, page.Width, 10),
                     XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 485, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 500, page.Width, 10),
                    XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 530, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"      from transaction password", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                    XStringFormats.CenterLeft);

                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 485, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 500, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 515, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 530, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                   XStringFormats.CenterLeft);

                    //for  Ass / Manager
                    gfx.DrawString($"Assistant / Manager", font, XBrushes.Black, new XRect(440, 630, 50, 50),
                      XStringFormats.CenterLeft);

                    string timestamp = General.GetTimestamp(DateTime.Now);
                    // Save the document...
                    filename = $"{timestamp}_{Username}_resetpwd.pdf";
                    document.Save($"{AdminPdf_Path}\\{filename}");
                }
                else
                {
                    AdminPdf_Path = $"{AdminPdf_Path}\\register";
                    //create if folder doesn't exist
                    if (!Directory.Exists(AdminPdf_Path))
                    {
                        Directory.CreateDirectory(AdminPdf_Path);
                    }
                    //create a new pdf document
                    PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

                    // Create an empty page
                    PdfSharp.Pdf.PdfPage page = document.AddPage();
                    //page.Contents.CreateSingleContent().Stream.UnfilteredValue;

                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                    //draw background image
                    DrawImage(gfx);

                    //PDF Size
                    page.Size = PdfSharp.PageSize.A4;

                    // Put a logo in the header
                    DrawTitleImage(gfx);

                    // Create a font
                    XFont font = new XFont("Arial", 20, XFontStyle.Bold, options);

                    //Draw page border
                    //XRect A4Rect = new XRect(50, 0, 500, 750);
                    //gfx.DrawRectangle(XPens.Black,A4Rect);

                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    // Draw the text
                    gfx.DrawString("eBanking Admin User Information", font, XBrushes.Black,
                      new XRect(0, 0, page.Width, 190),
                      XStringFormats.Center);

                    //Draw Dotted Line
                    XPen pen = new XPen(XColors.Indigo, 1.5);
                    XPen pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawRectangle(pens, 330, 0, 219, 70);

                    //gfx.DrawLine();
                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //for Datetime name
                    gfx.DrawString($"  Created Date", font, XBrushes.Black, new XRect(335, 15, page.Width, 10),
                      XStringFormats.CenterLeft);
                    //for Datetime value
                    gfx.DrawString($": {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(420, 15, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Staff ID name
                    gfx.DrawString($"  Staff_ID", font, XBrushes.Black, new XRect(335, 45, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Staff ID value
                    gfx.DrawString($": {StaffID}", font, XBrushes.Black, new XRect(420, 45, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold | XFontStyle.Underline, options);

                    // Draw the text
                    gfx.DrawString("Register New Admin User", font, XBrushes.Black,
                      new XRect(55, 110, page.Width, 20),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    //XPen pen = new XPen(XColors.Indigo, 1.5);
                    pen.DashStyle = XDashStyle.Dot;
                    gfx.DrawRectangle(pen, 50, 143, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 168, 50, 280);
                    gfx.DrawLine(pens, 50, 280, 550, 280);
                    gfx.DrawLine(pens, 550, 168, 550, 280);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Admin User Information", font, XBrushes.Black, new XRect(60, 130, page.Width, 50),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    // For Customer name
                    gfx.DrawString($"●  Full Name", font, XBrushes.Black, new XRect(60, 183, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Customer name value
                    gfx.DrawString($": {Fullname}", font, XBrushes.Black, new XRect(210, 183, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For PhoneNo name
                    gfx.DrawString($"●  Phone Number ", font, XBrushes.Black, new XRect(60, 203, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For PhoneNo value
                    gfx.DrawString($": {MobileNo}", font, XBrushes.Black, new XRect(210, 203, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  E-mail Address ", font, XBrushes.Black, new XRect(60, 223, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {Email}", font, XBrushes.Black, new XRect(210, 223, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // For Email name
                    gfx.DrawString($"●  Branch Name ", font, XBrushes.Black, new XRect(60, 243, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // For Email value
                    gfx.DrawString($": {BranchName}", font, XBrushes.Black, new XRect(210, 243, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Draw Dotted Line
                    gfx.DrawRectangle(pen, 50, 320, 500, 25);
                    pens = new XPen(XColors.Indigo, 0.5);
                    gfx.DrawLine(pens, 50, 420, 50, 345);
                    gfx.DrawLine(pens, 50, 420, 550, 420);
                    gfx.DrawLine(pens, 550, 420, 550, 345);

                    // Create a font
                    font = new XFont("Arial", 14, XFontStyle.Bold, options);
                    // Draw the text
                    gfx.DrawString("Credential Information", font, XBrushes.Black, new XRect(60, 310, page.Width, 45),
                      XStringFormats.CenterLeft);

                    // Create a font
                    font = new XFont("Arial", 12, XFontStyle.Regular, options);

                    //gfx.DrawLine();                    
                    // Draw the text name
                    gfx.DrawString($"●  User Name ", font, XBrushes.Black,
                      new XRect(55, 355, page.Width, 10),
                      XStringFormats.CenterLeft);
                    // Draw the text value
                    gfx.DrawString($": {Username}", font, XBrushes.Black,
                      new XRect(210, 355, page.Width, 10),
                      XStringFormats.CenterLeft);

                    //Create password font
                    XFont pwfont = new XFont("Myanmar3", 12, XFontStyle.Regular, options);
                    //Draw the text name
                    gfx.DrawString($"●  Login Password ", font, XBrushes.Black,
                        new XRect(55, 375, page.Width, 10),
                        XStringFormats.CenterLeft);
                    // Draw the text value
                    gfx.DrawString($": {LoginPwd}", pwfont, XBrushes.Black,
                      new XRect(210, 375, page.Width, 10),
                      XStringFormats.CenterLeft);

                    // Draw UserRole name
                    gfx.DrawString($"●  User Role ", font, XBrushes.Black,
                        new XRect(55, 395, page.Width, 10),
                        XStringFormats.CenterLeft);
                    // Draw UserRole value
                    gfx.DrawString($": {UserRole}", font, XBrushes.Black,
                        new XRect(210, 395, page.Width, 10),
                        XStringFormats.CenterLeft);

                    // for font create
                    font = new XFont("Arial", 12, XFontStyle.Bold, options);
                    //Draw Dotted Line
                    pens = new XPen(XColors.Black, 1.5);
                    pens.DashStyle = XDashStyle.Dot;
                    //gfx.DrawLine(pens, 50, 460, 155, 460);
                    gfx.DrawLine(pens, 330, 460, 225, 460);
                    //for password policies
                    //gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(50, 430, page.Width, 45),
                    //  XStringFormats.CenterLeft);
                    gfx.DrawString("Password Policies", font, XBrushes.Black, new XRect(225, 430, page.Width, 45),
                      XStringFormats.CenterLeft);
                    // for font create
                    font = new XFont("Arial", 10, XFontStyle.Regular, options);
                    //for password policies
                    //gfx.DrawString($"●    Minimum of 8 characters", font, XBrushes.Black, new XRect(70, 470, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Maximum of 16 characters", font, XBrushes.Black, new XRect(70, 485, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Uppercase letter", font, XBrushes.Black, new XRect(70, 500, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Lowercase letter", font, XBrushes.Black, new XRect(70, 515, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(70, 530, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(70, 545, page.Width, 10),
                    //  XStringFormats.CenterLeft);
                    //gfx.DrawString($"●    Login password must be different from transaction password", font, XBrushes.Black, new XRect(70, 560, page.Width, 10),
                    //  XStringFormats.CenterLeft);

                    gfx.DrawString($"●   Minimum of 6 characters", font, XBrushes.Black, new XRect(60, 470, page.Width, 10),
                     XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Maximum of 16 characters", font, XBrushes.Black, new XRect(60, 485, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 letter", font, XBrushes.Black, new XRect(60, 500, page.Width, 10),
                    XStringFormats.CenterLeft);
                    gfx.DrawString($"●   At least 1 Special character (@#%&$^!*)", font, XBrushes.Black, new XRect(60, 515, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Cannot contain space and (< > /)", font, XBrushes.Black, new XRect(60, 530, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Login password must be different", font, XBrushes.Black, new XRect(60, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"      from transaction password", font, XBrushes.Black, new XRect(60, 560, page.Width, 10),
                    XStringFormats.CenterLeft);

                    // for font create
                    XFont fonts = new XFont("Zawgyi-One", 10, XFontStyle.Regular, options);
                    //for password policies
                    gfx.DrawString($"●   အနည္းဆုံး ၆ လုံးရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 470, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အမ်ားဆုံး ၁၆ လုံးအထိေပးႏိုင္ပါသည္။", fonts, XBrushes.Black, new XRect(270, 485, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   အကၡရာ စာလုံးအနည္းဆုံးတစ္လုံးပါဝင္ရမည္။", fonts, XBrushes.Black, new XRect(270, 500, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   Special Character(!@#%&^*)အနည္းဆုံး ၁ လုံးပါရွိရပါမည္။", fonts, XBrushes.Black, new XRect(270, 515, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"●   စကားဝွက္တြင္ တစ္လုံးႏွင့္တစ္လုံးမျခားရပါ။(< > /)မ်ားမပါဝင္ရပါ။", fonts, XBrushes.Black, new XRect(270, 530, 345, 10),
                      XStringFormats.CenterLeft);
                    // for font create
                    gfx.DrawString($"●   ၀င္ေရာက္ရန္ စကား၀ွက္နွင့္ လုပ္ေဆာင္မွုစကား၀ွက္တို့သည္", fonts, XBrushes.Black, new XRect(270, 545, page.Width, 10),
                      XStringFormats.CenterLeft);
                    gfx.DrawString($"     မတူညီရပါ။", fonts, XBrushes.Black, new XRect(270, 560, page.Width, 10),
                   XStringFormats.CenterLeft);

                    //for  Ass / Manager
                    gfx.DrawString($"Assistant / Manager", font, XBrushes.Black, new XRect(440, 620, 50, 50),
                      XStringFormats.CenterLeft);

                    string timestamp = General.GetTimestamp(DateTime.Now);
                    // Save the document...
                    filename = $"{timestamp}_{Username}.pdf";
                    document.Save($"{AdminPdf_Path}\\{filename}");
                }
                    //if (Status == 1)
                    //{
                    //    AdminPdf_Path = $"{AdminPdf_Path}\\reset password";
                    //    //create if folder doesn't exist
                    //    if (!System.IO.Directory.Exists(AdminPdf_Path))
                    //    {
                    //        System.IO.Directory.CreateDirectory(AdminPdf_Path);
                    //    }
                    //    filename = $"{name}_resetpwd.pdf"; //filename = $"{name}_loginpwd.pdf";
                    //    Renderer.RenderHtmlAsPdf($"<h1>ABank Admin User</h1><br>" +
                    //        $"<h3>Resetting Password</h3><br>" +
                    //        $"<h4>For {name}</h4><br>" +
                    //        $"<p>New Password is <b>{Pwd}</b><br><br>" +
                    //        $"Date:\t{DateTime.Now.ToString()}</p>"
                    //        ).SaveAs($"{AdminPdf_Path}\\{filename}");
                    //}
                    //else       //register
                    //{
                    //    AdminPdf_Path = $"{AdminPdf_Path}\\register";
                    //    //create if folder doesn't exist
                    //    if (!System.IO.Directory.Exists(AdminPdf_Path))
                    //    {
                    //        System.IO.Directory.CreateDirectory(AdminPdf_Path);
                    //    }
                    //    filename = $"{name}.pdf";
                    //    Renderer.RenderHtmlAsPdf($"<h1>ABank Admin User</h1><br>" +
                    //        $"<h3>Generating Password</h3><br>" +
                    //        $"<h4>For {name}</h4><br>" +
                    //        $"<p>New Password is <b>{Pwd}</b><br>" +
                    //        $"Date:\t{DateTime.Now.ToString()}</p>"
                    //        ).SaveAs($"{AdminPdf_Path}\\{filename}");
                    //}

                    return filename;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return null;
            }
        }

        private void DrawImage(XGraphics gfx)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/new_pdf_bg.jpg"));

            gfx.DrawImage(image, 50, 40, 500, 650);//fixed image size => width 500, height 650
        }

        void DrawTitleImage(XGraphics gfxTitle)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Images/pdftitle.jpg"));
            gfxTitle.DrawImage(image, 50, 0, 170, 60);
        }

        public ActionResult Download(string filename)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //string AdminPdf_Path = System.Configuration.ConfigurationManager.AppSettings["AdminPdfSavePath"];
                string AdminPdf_Path = ConstantValues.AdminPdf_Path;
                string fullpath = "";
                if (filename.Contains("pwd.pdf"))
                {
                    fullpath = $"{AdminPdf_Path}\\reset password\\{filename}";
                }
                else
                {
                    fullpath = $"{AdminPdf_Path}\\register\\{filename}"; //register
                }
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Downlod successfully.",$"File name is {filename}.");
                return File(fullpath, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, actionName, ex);
                return View("Error");
            }
        }

        #endregion
    }
}