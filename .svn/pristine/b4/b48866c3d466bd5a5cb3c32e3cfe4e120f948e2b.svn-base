using ABankAdmin.Models;
using ABankAdmin.Utils;
using ABankAdmin.ViewModels;
using AbankAdminAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    /// <summary>
    /// CorporateUserRequest
    /// </summary>
    public class CorporateUserRequestController : Controller
    {
        private readonly AdminDBContext db = new AdminDBContext();

        private readonly string controllerName = "CorporateUserRequestController";
        private readonly EventLogController log = new EventLogController();

        //get connection string
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        #region Request User List

        /// <summary>
        /// RequestUserList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RequestUserList()
        {
            //role
            var user_Role = await db.C_Role.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).OrderBy(o => o.Text).AsNoTracking().ToListAsync();
            ViewBag.userRole = new SelectList(user_Role, "Value", "Text");

            //type
            var typeList = CorporateUserRequest.GetTypeList();
            //var typeList = General.GetUserType();
            ViewBag.userType = new SelectList(typeList, "Value", "Text");

            //status
            var statusList = CorporateUserRequest.GetStatusList();
            //var statusList = General.GetStatus();
            ViewBag.status = new SelectList(statusList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// GetRequestUserList
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRequestUserList(RequestUserSearchVM data, DataTableAjaxPostModel model)
        {
            try
            {
                int total = 0;
                int pageSize = model.length;
                int skip = model.start;
                int size = model.length;

                // sort Column Name + Sort Column Direction (asc, desc)
                var sortExpression = model.columns[model.order[0].column].name + " " + model.order[0].dir;

                if (pageSize < 0) pageSize = total;

                string userid = User.Identity.GetUserId();

                //role
                var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                //query data
                IQueryable<RequestUserListVM> recordData = (from u in db.C_Req_Users
                                                            join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                                            where u.MAKER == userid   // to show only own-requests // u.STATUS != 1
                                                            select new RequestUserListVM
                                                            {
                                                                Id = u.ID,
                                                                CompanyName = c.COMPANY_NAME,
                                                                CorporateId = c.CORPORATEID,
                                                                Username = u.USERNAME,
                                                                Fullname = u.FULLNAME,
                                                                Phoneno = u.MOBILENO,
                                                                Userroles = db.C_Req_UserInRoles.Where(w => w.ReqUserID == u.ID).Select(s => s.RoleId).ToList(),
                                                                RequestedDate = u.REQUESTEDDATE,
                                                                Type = u.ISNEW,
                                                                Status = u.STATUS,
                                                                IsDeleteable = u.STATUS != 1 && u.STATUS != 2
                                                            }).OrderBy(sortExpression);

                //check data
                if (!string.IsNullOrEmpty(data.CompanyName))
                {
                    recordData = recordData.Where(w => w.CompanyName.ToUpper().Contains(data.CompanyName.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.CorporateID))
                {
                    recordData = recordData.Where(w => w.CorporateId.ToUpper().Contains(data.CorporateID.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.UserName))
                {
                    recordData = recordData.Where(w => w.Username.ToUpper().Contains(data.UserName.ToUpper()));
                }

                if (data.UserRole.HasValue)
                {
                    recordData = recordData.Where(w => w.Userroles.Contains(data.UserRole.Value));
                }

                if (data.FromDate.HasValue && data.ToDate.HasValue)
                {
                    var ToDate = data.ToDate.Value.AddDays(1);
                    recordData = recordData.Where(a => a.RequestedDate >= data.FromDate.Value && a.RequestedDate < ToDate);
                }
                else if (data.FromDate.HasValue)
                {
                    recordData = recordData.Where(a => a.RequestedDate >= data.FromDate.Value);
                }
                else if (data.ToDate.HasValue)
                {
                    var ToDate = data.ToDate.Value.AddDays(1);
                    recordData = recordData.Where(a => a.RequestedDate < ToDate);
                }

                if (data.UserType.HasValue)
                {
                    recordData = recordData.Where(w => w.Type.Equals(data.UserType.Value));
                }

                if (data.Status.HasValue)
                {
                    recordData = recordData.Where(w => w.Status.Equals(data.Status.Value));
                }

                //get total record
                total = await recordData.AsNoTracking().CountAsync();

                //get data by pagination
                var recordDatas = await recordData.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();

                //get role names
                recordDatas.ForEach(x => x.Userrole = CorporateUserRequest.GetRoles(user_Role, x.Userroles));

                return Json(new { model.draw, recordsFiltered = total, recordsTotal = total, data = recordDatas });
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Corporate Request User List!" });
            }
        }

        /// <summary>
        /// EditReqCorporateUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditReqCorporateUser(int id)
        {
            //title
            ViewBag.PageTitle = "Edit";

            //user id
            ViewBag.CopRegId = id;

            //status id 2 for edit
            ViewBag.CopRegStatus = CorporateUserRequest.EDITREQCORPORATEUSER;

            //get transaction info
            ViewBag.EVMakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVMakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            //get departments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Where(d => d.DEL_FLAG == false).Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Where(p => p.DEL_FLAG == false).Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View(nameof(RequestCorporateUser));
        }

        /// <summary>
        /// DetailReqCorporateUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DetailReqCorporateUser(int id)
        {
            //title
            ViewBag.PageTitle = "Detail";

            //user id
            ViewBag.CopRegId = id;

            //status id 3 for detail
            ViewBag.CopRegStatus = CorporateUserRequest.DETAIREQLCORPORATEUSER;

            //get transaction info
            var requser = db.C_Req_Users.Where(u => u.ID == id).FirstOrDefault();
            ViewBag.EVMakerTranLimit = await db.C_Req_UserTranLimits.Where(w => w.CorporateId == requser.CORPORATEID && w.ReqUserID == requser.ID && w.RuleCode == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerTranLimit = await db.C_Req_UserTranLimits.Where(w => w.CorporateId == requser.CORPORATEID && w.ReqUserID == requser.ID && w.RuleCode == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVMakerBulkTranLimit = await db.C_Req_UserTranLimits.Where(w => w.CorporateId == requser.CORPORATEID && w.ReqUserID == requser.ID && w.RuleCode == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerBulkTranLimit = await db.C_Req_UserTranLimits.Where(w => w.CorporateId == requser.CORPORATEID && w.ReqUserID == requser.ID && w.RuleCode == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            if (ViewBag.EVMakerTranLimit == 0)
            {
                ViewBag.EVMakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVCheckerTranLimit == 0)
            {
                ViewBag.EVCheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVMakerBulkTranLimit == 0)
            {
                ViewBag.EVMakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVCheckerBulkTranLimit == 0)
            {
                ViewBag.EVCheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }

            //get departments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Where(d => d.DEL_FLAG == false).Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Where(p => p.DEL_FLAG == false).Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View(nameof(RequestCorporateUser));
        }

        /// <summary>
        /// DeleteRequestUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteRequestUser(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get data
                var reqUser = await db.C_Req_Users.FindAsync(id);
                if (reqUser == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }
                else if (reqUser.STATUS == 1)
                {
                    return new JsonErrorResult(new { error = "Approved User cannot be delete!" }, HttpStatusCode.BadRequest);
                }
                else if (reqUser.STATUS == 2)
                {
                    return new JsonErrorResult(new { error = "Rejected User cannot be delete!" }, HttpStatusCode.BadRequest);
                }
                string username = reqUser.USERNAME;

                //remove data
                db.C_Req_Users.Remove(reqUser);
                await db.SaveChangesAsync();

                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3, $"{username} is deleted!");
                return Json(new { title = "Success", message = "Deleted Successfully!" });
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while deleting Corporate Request User!" });
            }
        }

        #endregion Request User List

        #region Request Changes List

        /// <summary>
        /// RequestChangesList
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestChangesList()
        {
            //type
            var typeList = CorporateUserRequest.GetChangesTypeList();
            //var typeList = General.GetUserChangesType(); //get typelist from core
            ViewBag.userType = new SelectList(typeList, "Value", "Text");

            //status
            var statusList = CorporateUserRequest.GetStatusList();
            //var statusList = General.GetStatus();//get typelist from core
            ViewBag.status = new SelectList(statusList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// GetRequestChangesList
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRequestChangesList(RequestChangesSearchVM data, DataTableAjaxPostModel model)
        {
            try
            {
                int total = 0;
                int pageSize = model.length;
                int skip = model.start;
                int size = model.length;

                // sort Column Name + Sort Column Direction (asc, desc)
                var sortExpression = model.columns[model.order[0].column].name + " " + model.order[0].dir;

                if (pageSize < 0) pageSize = total;

                //get login user id
                string userid = User.Identity.GetUserId();

                //query data
                IQueryable<RequestChangesListVM> recordData = (from u in db.C_Req_Changes
                                                               join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                                               join user in db._Users on u.USERID equals user.ID
                                                               join user2 in db.Users on u.MAKER equals user2.Id
                                                               //where u.STATUS != 1
                                                               select new RequestChangesListVM
                                                               {
                                                                   Id = u.ID,
                                                                   CompanyName = c.COMPANY_NAME,
                                                                   CorporateId = c.CORPORATEID,
                                                                   Username = user.USERNAME,
                                                                   RequestedDate = u.REQUESTEDDATE,
                                                                   Maker = user2.UserName,
                                                                   Type = u.TYPE,
                                                                   Status = u.STATUS,
                                                                   IsDeleteable = u.STATUS != 1 && u.STATUS != 2
                                                               }).OrderBy(sortExpression);

                //check data
                if (!string.IsNullOrEmpty(data.CompanyName))
                {
                    recordData = recordData.Where(w => w.CompanyName.ToUpper().Contains(data.CompanyName.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.CorporateID))
                {
                    recordData = recordData.Where(w => w.CorporateId.ToUpper().Contains(data.CorporateID.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.UserName))
                {
                    recordData = recordData.Where(w => w.Username.ToUpper().Contains(data.UserName.ToUpper()));
                }

                if (data.UserType.HasValue)
                {
                    recordData = recordData.Where(w => w.Type.Equals(data.UserType.Value));
                }

                if (data.FromDate.HasValue && data.ToDate.HasValue)
                {
                    var ToDate = data.ToDate.Value.AddDays(1);
                    recordData = recordData.Where(a => a.RequestedDate >= data.FromDate.Value && a.RequestedDate < ToDate);
                }
                else if (data.FromDate.HasValue)
                {
                    recordData = recordData.Where(a => a.RequestedDate >= data.FromDate.Value);
                }
                else if (data.ToDate.HasValue)
                {
                    var ToDate = data.ToDate.Value.AddDays(1);
                    recordData = recordData.Where(a => a.RequestedDate < ToDate);
                }

                if (data.Status.HasValue)
                {
                    recordData = recordData.Where(w => w.Status.Equals(data.Status.Value));
                }

                //get total count
                total = await recordData.AsNoTracking().CountAsync();

                //get data by pagination
                var recordDatas = await recordData.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();

                return Json(new { model.draw, recordsFiltered = total, recordsTotal = total, data = recordDatas });
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Request Changes List!" });
            }
        }

        /// <summary>
        /// DeleteRequestChanges
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteRequestChanges(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get data
                var reqchange = await db.C_Req_Changes.FindAsync(id);
                if (reqchange == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }
                else if (reqchange.STATUS == 1)
                {
                    return new JsonErrorResult(new { error = "Approved Changes cannot be delete!" }, HttpStatusCode.BadRequest);
                }
                else if (reqchange.STATUS == 2)
                {
                    return new JsonErrorResult(new { error = "Rejected Changes cannot be delete!" }, HttpStatusCode.BadRequest);
                }
                int username = reqchange.USERID;

                //remove data
                db.C_Req_Changes.Remove(reqchange);
                await db.SaveChangesAsync();

                //save log
                log.Info(User.Identity.GetUserId(), controllerName, actionName, 3, $"{username} is deleted!");
                return Json(new { title = "Success", message = "Deleted Successfully!" });
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while deleting Request Changes!!" });
            }
        }

        /// <summary>
        /// GetRequestChanges
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRequestChanges(int id)
        {
            try
            {
                //get data
                var reqchanges = await (from u in db.C_Req_Changes
                                        join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                        join user in db._Users on u.USERID equals user.ID
                                        where u.ID == id
                                        select new RequestChangesDetailVM
                                        {
                                            Id = u.ID,
                                            CompanyName = c.COMPANY_NAME,
                                            Username = user.USERNAME,
                                            Checker = u.CHECKER,
                                            CheckedDate = u.CHECKEDDATE,
                                            CHeckedReason = u.CHECKERREASON,
                                            RequestedDate = u.REQUESTEDDATE,
                                            Maker = u.MAKER,
                                            TypeField = u.TYPE,
                                            StatusField = u.STATUS,
                                            BranchID = u.BranchID
                                        }).AsNoTracking().FirstOrDefaultAsync();

                if (reqchanges == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //get user name
                if (string.IsNullOrEmpty(reqchanges.Checker))
                {
                    reqchanges.Checker = "-";
                }
                else
                {
                    reqchanges.Checker = db.Users.Find(reqchanges.Checker).UserName;
                }
                reqchanges.Maker = db.Users.Find(reqchanges.Maker).UserName;
                reqchanges.BranchName = await db.Branches.Where(w => w.id == reqchanges.BranchID).Select(s => s.name).FirstOrDefaultAsync();

                //get reason, status and type name
                reqchanges.CHeckedReason = string.IsNullOrEmpty(reqchanges.CHeckedReason) ? "-" : reqchanges.CHeckedReason;
                reqchanges.Status = CorporateUserRequest.GetStatus(reqchanges.StatusField);
                reqchanges.Type = CorporateUserRequest.GetUserType(reqchanges.TypeField);

                return Json(reqchanges);
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Request Changes!" });
            }
        }

        #endregion Request Changes List

        #region Corporate User List

        /// <summary>
        /// CorporateUserList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CorporateUserList()
        {
            //get user roles
            var user_Role = await db.C_Role.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync();
            ViewBag.userRole = new SelectList(user_Role, "Id", "Name");

            return View();
        }

        /// <summary>
        /// GetCorporateUserList
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetCorporateUserList(CorporateUserSearchVM data, DataTableAjaxPostModel model)
        {
            try
            {
                int total = 0;
                int pageSize = model.length;
                int skip = model.start;
                int size = model.length;

                // sort Column Name + Sort Column Direction (asc, desc)
                var sortExpression = model.columns[model.order[0].column].name + " " + model.order[0].dir;

                if (pageSize < 0) pageSize = total;

                //get rule for Fail Transaction Count
                var rule = await db.Rules.Where(r => r.Code == CorporateUserRequest.FAILTRANPWDCOUNT_NAME).AsNoTracking().FirstOrDefaultAsync();
                int IntRule = Convert.ToInt32(rule.Value);
                int ruleValue = (rule == null ? 5 : IntRule);

                //get login user id
                string userid = User.Identity.GetUserId();

                //get roles
                var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                //query data
                IQueryable<CopUserRegListVM> recordData = (from u in db._Users
                                                           join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                                           where u.DEL_STATUS == false
                                                           select new CopUserRegListVM
                                                           {
                                                               Id = u.ID,
                                                               CompanyName = c.COMPANY_NAME,
                                                               Username = u.USERNAME,
                                                               Fullname = u.FULLNAME,
                                                               Phoneno = u.MOBILENO,
                                                               CorporateId = c.CORPORATEID,
                                                               Userroles = db.C_UserInRoles.Where(w => w.UserId == u.USERID).Select(s => s.RoleId).ToList(),
                                                               Signinlock = u.ISLOCK_FLAG,
                                                               Translock = (u.FAIL_TRAN_PWD_COUNT >= ruleValue)
                                                           }).OrderBy(sortExpression);

                //check data
                if (!string.IsNullOrEmpty(data.CompanyName))
                {
                    recordData = recordData.Where(w => w.CompanyName.ToUpper().Contains(data.CompanyName.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.CorporateID))
                {
                    recordData = recordData.Where(w => w.CorporateId.ToUpper().Contains(data.CorporateID.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.UserName))
                {
                    recordData = recordData.Where(w => w.Username.ToUpper().Contains(data.UserName.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.PhoneNo))
                {
                    recordData = recordData.Where(w => w.Phoneno.ToUpper().Contains(data.PhoneNo.ToUpper()));
                }

                if (data.UserRole.HasValue)
                {
                    recordData = recordData.Where(w => w.Userroles.Contains(data.UserRole.Value));
                }

                //get total count
                total = await recordData.AsNoTracking().CountAsync();

                //get data by pagination
                var recordDatas = await recordData.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();

                //get role names
                recordDatas.ForEach(x => x.Userrole = CorporateUserRequest.GetRoles(user_Role, x.Userroles));

                return Json(new { model.draw, recordsFiltered = total, recordsTotal = total, data = recordDatas });
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Corporate User List!" });
            }
        }

        /// <summary>
        /// EditCorporateUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditCorporateUser(int id)
        {           
            //title
            ViewBag.PageTitle = "Edit";

            //user id
            ViewBag.CopRegId = id;

            //status id 2 for edit
            ViewBag.CopRegStatus = CorporateUserRequest.EDITCORPORATEUSER;

            //get transaction info
            //ViewBag.EVMakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            //ViewBag.EVCheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            //ViewBag.EVMakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            //ViewBag.EVCheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            var user = db._Users.Where(u => u.ID == id).FirstOrDefault();
            ViewBag.EVMakerTranLimit = await db.C_UserTranLimits.Where(w => w.CorporateId == user.CORPORATEID && w.UserId == user.USERID && w.RuleCode == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerTranLimit = await db.C_UserTranLimits.Where(w => w.CorporateId == user.CORPORATEID && w.UserId == user.USERID && w.RuleCode == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVMakerBulkTranLimit = await db.C_UserTranLimits.Where(w => w.CorporateId == user.CORPORATEID && w.UserId == user.USERID && w.RuleCode == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.EVCheckerBulkTranLimit = await db.C_UserTranLimits.Where(w => w.CorporateId == user.CORPORATEID && w.UserId == user.USERID && w.RuleCode == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            if (ViewBag.EVMakerTranLimit == 0)
            {
                ViewBag.EVMakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVCheckerTranLimit == 0)
            {
                ViewBag.EVCheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVMakerBulkTranLimit == 0)
            {
                ViewBag.EVMakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            if (ViewBag.EVCheckerBulkTranLimit == 0)
            {
                ViewBag.EVCheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            }
            //get departments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Where(d => d.DEL_FLAG == false).Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Where(p => p.DEL_FLAG == false).Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View(nameof(RequestCorporateUser));
        }

        /// <summary>
        /// DetailCorporateUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DetailCorporateUser(int id)
        {
            //title
            ViewBag.PageTitle = "Detail";

            //user id
            ViewBag.CopRegId = id;

            //status id 3 for detail
            ViewBag.CopRegStatus = CorporateUserRequest.DETAILCORPORATEUSER;

            //get transaction info
            ViewBag.DVMakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.DVCheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.DVMakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.DVCheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            //get departments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Where(d => d.DEL_FLAG == false).Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Where(p => p.DEL_FLAG == false).Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View(nameof(RequestCorporateUser));
        }

        /// <summary>
        /// DeleteCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 7 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 7, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //update user with delete status
                    //user.DEL_STATUS = true;       //no need to delete immediately, When approve the request, will delete
                    //await db.SaveChangesAsync();

                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting delete successfully.", $"User id is {id}");
                    return Json(new { title = "Success", message = "Requesting delete successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting delete for Corporate User Registration!" });
            }
        }

        /// <summary>
        /// ResetPasswordCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPasswordCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 2 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 2, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting reset password successfully.", $"User id is {id}");
                    return Json(new { title = "Success", message = "Requesting reset password successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting reset password for Corporate User Registration!" });
            }
        }

        /// <summary>
        /// SigninLockCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SigninLockCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 3 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 3, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting lock successfully.", $"User id is { id }");
                    return Json(new { title = "Success", message = "Requesting lock successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting signin lock for Corporate User Registration!" });
            }
        }

        /// <summary>
        /// SigninUnLockCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SigninUnLockCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 4 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 4, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting Unlock successfully.", $"User id is { id }");
                    return Json(new { title = "Success", message = "Requesting Unlock successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting signin unlock for Corporate User Registration!" });
            }
        }

        /// <summary>
        /// TransLockCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> TransLockCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 5 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 5, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting TranLock successfully.", $"User id is { id }");
                    return Json(new { title = "Success", message = "Requesting Tranlock successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting transaction lock for Corporate User Registration!" });
            }
        }

        /// <summary>
        /// TransUnLockCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> TransUnLockCorporateUserReg(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get user by id
                User user = await db._Users.FindAsync(id);
                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                //check already requested
                bool existingchanges = await db.C_Req_Changes
                    .AnyAsync(rc => rc.USERID == id && rc.TYPE == 6 && rc.STATUS == 0);
                if (existingchanges)
                {
                    return new JsonErrorResult(new { error = "Already requested!" }, HttpStatusCode.BadRequest);
                }

                //get BranchID from Logged In User
                AdminUser admin = db.Users.Find(LoginUser);

                //save data
                db.C_Req_Changes.Add(CorporateUserRequest.GetReqChanges(id, 6, 0, LoginUser, admin.BranchID, user.CORPORATEID.GetValueOrDefault()));
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, controllerName, actionName, "Requesting TranUnlock successfully.", $"User id is { id }");
                    return Json(new { title = "Success", message = "Requesting TranUnlock successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while requesting transaction unlock for Corporate User Registration!" });
            }
        }

        #endregion Corporate User List

        #region Register Corporate User

        /// <summary>
        /// RequestCorporateUser
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RequestCorporateUser()
        {
            //title
            ViewBag.PageTitle = "Create";

            //get transaction info
            ViewBag.MakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.CheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.MakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
            ViewBag.CheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();

            //get dapartments
            ViewBag.DepartmentId = new SelectList(await db.C_Departments.Where(d => d.DEL_FLAG == false).Select(s => new
            {
                DepartmentId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "DepartmentId", "Name");

            //get positions
            ViewBag.PositionId = new SelectList(await db.C_Positions.Where(p => p.DEL_FLAG == false).Select(s => new
            {
                PositionId = s.ID,
                Name = s.NAME
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync(),
                "PositionId", "Name");

            return View();
        }

        /// <summary>
        /// SaveReqCorporateUserReg
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menuInfo"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveReqCorporateUserReg(CopUserRegVM model, List<MenuInfoVM> menuInfo, List<AccountInfoVM> accountInfo)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            //start trannsaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //check user name
                    var result = await UserNameIsNotExist(model.UserName, 0,model.Id, model.CorpID.GetValueOrDefault());
                    if (result is string)
                    {
                        return new JsonErrorResult(new { error = result }, HttpStatusCode.BadRequest);
                    }

                    //get login user id
                    var loginUser = User.Identity.GetUserId();
                    var branchId = await db.Users.Where(u => u.Id == loginUser).Select(s => s.BranchID).FirstOrDefaultAsync();

                    //get roles
                    var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                    //get menus
                    var allMenus = await db.C_Menus.Where(C => C.IsDelete == false).AsNoTracking().ToListAsync();

                    //corporate user
                    var corpUser = CorporateUserRequest.GetReqUser(model, true, branchId, loginUser);
                    db.C_Req_Users.Add(corpUser);
                    await db.SaveChangesAsync();

                    //transaction Info
                    if (model.IsTransaction)
                    {
                        //maker
                        if (model.IsMaker)
                        {
                            //transaction limit
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(3, model.MakerBulkTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(1, model.MakerTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            await db.SaveChangesAsync();
                        }
                        //viewer
                        if (model.IsApprover)
                        {
                            //transaction limit
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(4, model.CheckerBulkTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(2, model.CheckerTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }

                    //role info and menu info
                    if (model.IsAdmin)
                    {
                        //admin maker
                        if (model.IsAdminMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINMAKER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(1, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //admin approver
                        if (model.IsAdminApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINAPPROVER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(2, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        //maker
                        if (model.IsMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.MAKER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(3, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //viewer
                        if (model.IsViewer)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.VIEWER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(4, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //approver
                        if (model.IsApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.APPROVER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(5, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }

                    //account info
                    db.C_Req_UserInAccounts.AddRange(CorporateUserRequest.GetReqUserInAccounts(accountInfo, corpUser.ID, model.CorpID.GetValueOrDefault(), loginUser));
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbContextTransaction.Commit();

                    log.Info(loginUser, controllerName, actionName, 1, $"{corpUser.USERNAME} is added.");
                    return Json(new { title = "Success", message = "Saved Successfully!" });
                }
                catch (Exception ex)
                {
                    //rollback transaction
                    dbContextTransaction.Rollback();
                    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                    return new JsonErrorResult(new { error = "Error while saving Corporate User Registration!" });
                }
            }
        }

        #endregion Register Corporate User

        #region Corporate User

        /// <summary>
        /// GetCorporateData
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetCorporateData(string term)
        {
            try
            {
                var data = await db.C_Corporates
                    .Where(w => w.COMPANY_NAME.Contains(term) && w.DEL_FLAG == false )
                    .Select(s => new
                    {
                        CorpID = s.ID,
                        CorporateID = s.CORPORATEID,
                        CompanyName = s.COMPANY_NAME,
                        CIFID = s.CIFID
                    }).OrderBy(o => o.CompanyName).AsNoTracking().ToListAsync();
                //if (data == null || data.Count == 0)
                //{
                //    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                //}
                return Json(data);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while loading Corporate User!" });
            }
        }

        /// <summary>
        /// GetUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetUserReg(int id)
        {
            try
            {
                //get roles
                var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                //query data
                var user = await (from u in db._Users
                                  join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                  where u.ID == id
                                  select new CopUserRegVM
                                  {
                                      Id = u.ID,
                                      CompanyName = c.COMPANY_NAME,
                                      UserName = u.USERNAME,
                                      FullName = u.FULLNAME,
                                      Email = u.EMAIL,
                                      PhoneNo = u.MOBILENO,
                                      Address = u.ADDRESS,
                                      CorpID = u.CORPORATEID,
                                      CorporateID = c.CORPORATEID,
                                      CIFID = c.CIFID,
                                      DepartmentId = u.DepartmentId,
                                      PositionId = u.PositionId,
                                      USERID = u.USERID,
                                      Userroles = db.C_UserInRoles.Where(w => w.UserId == u.USERID).Select(s => s.RoleId).ToList()
                                  }).AsNoTracking().FirstOrDefaultAsync();

                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                user.MakerTranLimit = await db.C_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.MAKERTRANLIMIT_NAME && w.UserId == user.USERID && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.CheckerTranLimit = await db.C_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.CHECKERTRANLIMIT_NAME && w.UserId == user.USERID && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.MakerBulkTranLimit = await db.C_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME && w.UserId == user.USERID && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.CheckerBulkTranLimit = await db.C_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME && w.UserId == user.USERID && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();

                //check maker tran limit
                if (user.MakerTranLimit == 0)
                {
                    user.MakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check checker tran limit
                if (user.CheckerTranLimit == 0)
                {
                    user.CheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check maker bulk tran limit
                if (user.MakerBulkTranLimit == 0)
                {
                    user.MakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check checker bulk tran limit
                if (user.CheckerBulkTranLimit == 0)
                {
                    user.CheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }

                //check user role
                foreach (var item in user.Userroles)
                {
                    var role = user_Role.FirstOrDefault(s => s.Id == item);
                    if (role != null)
                    {
                        if (role.isAdmin.GetValueOrDefault())
                        {
                            user.IsAdmin = true;
                            if (role.Name.Equals(CorporateUserRequest.ADMINMAKER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsAdminMaker = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.ADMINAPPROVER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsAdminApprover = true;
                            }
                        }
                        else
                        {
                            user.IsTransaction = true;
                            if (role.Name.Equals(CorporateUserRequest.MAKER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsMaker = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.VIEWER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsViewer = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.APPROVER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsApprover = true;
                            }
                        }
                    }
                }
                return Json(user);
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading User Data" });
            }
        }

        /// <summary>
        /// GetMenuInfo
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetMenuInfo(RoleInfoVM data, string id)
        {
            try
            {
                List<MenuInfoVM> menuData = new List<MenuInfoVM>();

                //get existing menu
                var existingMenus = await db.C_MenuPermissions.Where(w => w.UserId == id).ToListAsync();

                //get menus
                var allMenus = await db.C_Menus.Where( c => c.IsDelete == false).AsNoTracking().ToListAsync();

                //check user type
                if (data.IsAdmin)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.ADMIN_NAME && w.IsDelete == false)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //admin maker
                        if (data.IsAdminMaker)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINMAKER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsAdminMakerEnable = true;
                                menu.IsAdminMaker = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //admin approver
                        if (data.IsAdminApprover)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINAPPROVER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsAdminApproverEnable = true;
                                menu.IsAdminApprover = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        menuData.Add(menu);
                    }
                }
                else if (data.IsTransaction)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.TRANSACTION_NAME)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //maker
                        if (data.IsMaker)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.MAKER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsMakerEnable = true;
                                menu.IsMaker = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //viewer
                        if (data.IsViewer)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.VIEWER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsViewerEnable = true;
                                menu.IsViewer = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //approver
                        if (data.IsApprover)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.APPROVER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsApproverEnable = true;
                                menu.IsApprover = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        menuData.Add(menu);
                    }
                }

                if (menuData == null || menuData.Count == 0)
                {
                    return new JsonErrorResult(new { error = "Menu Not Found!" }, HttpStatusCode.BadRequest);
                }
                return Json(menuData);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while loading Menu Data!" });
            }
        }

        /// <summary>
        /// GetAccountInfo
        /// </summary>
        /// <param name="CIFID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> GetAccountInfo(string CIFID, string id)
        {
            try
            {

                CorporateResponseInfo cri = null;

                //check cifid exist
                if (!string.IsNullOrEmpty(CIFID))
                {
                    //int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    //if (count > 0)
                    //    return Json(new { message = "Duplicate CIFID", error = true });

                    //get accounts by cifid
                    cri = CorporateUserRequest.GetCorporateResponse(CIFID, connectionString);

                    //check accounts
                    if (cri != null && !string.IsNullOrEmpty(cri.Name)
                        && cri.lstCorpAccountInfo != null && cri.lstCorpAccountInfo.Count != 0)
                    {
                        var accData = cri.lstCorpAccountInfo.Select(s => new AccountInfoVM
                        {
                            AccountNo = s.AccountNumber,
                            AccountType = s.AccountType,
                            AccountTypeDesc = s.AccountTypeDesc,
                            Currency = s.Currency,
                            IsActive = false
                        }).ToList();

                        //check existing for edit
                        if (!string.IsNullOrEmpty(id))
                        {
                            //get existing account
                            List<C_UserInAccount> existingAcc = await db.C_UserInAccounts.Where(w => w.UserId == id).AsNoTracking().ToListAsync();

                            if (existingAcc != null && existingAcc.Count() != 0)
                            {
                                //check api data with existing account
                                accData.ForEach(x =>
                                    x.IsActive = existingAcc.Any(e => e.AccountNo.Equals(x.AccountNo, StringComparison.CurrentCultureIgnoreCase))
                                );
                            }
                        }
                        return Json(accData);
                    }
                }
                return new JsonErrorResult(new { error = "No individual account under this CIFID!" }, HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while loading Account Data!" });
            }
        }

        /// <summary>
        /// GetAccountInfo
        /// </summary>
        /// <param name="CorporateID"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> GetCorporateLimit(int CorporateID)
        {
            try
            {
                if (CorporateID > 0)
                {
                    string RuleCode = "R043";
                    C_CorporateTranRule corporateLimit = await db.C_CorporateTranRules.Where(c => c.IsDelete == false && c.CorporateId == CorporateID && c.RuleCode == RuleCode).FirstOrDefaultAsync();

                    if (corporateLimit != null)
                        return Json( new { limit = corporateLimit.Value });

                    return new JsonErrorResult(new { error = "No Daily Corporate Limit for this Company!" }, HttpStatusCode.BadRequest);
                }

                return new JsonErrorResult(new { error = "Please choose one Company first!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while loading Account Data!" });
            }
        }

        /// <summary>
        /// UpdateCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="menuInfo"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateCorporateUserReg(int id, CopUserRegVM model, List<MenuInfoVM> menuInfo, List<AccountInfoVM> accountInfo)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            //start trannsaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get login user id
                    var loginUser = User.Identity.GetUserId();
                    var branchId = await db.Users.Where(u => u.Id == loginUser).Select(s => s.BranchID).FirstOrDefaultAsync();

                    //get user by id
                    var user = db._Users.Where(u => u.ID == id).FirstOrDefault();
                    bool existingUser = await db._Users.AnyAsync(w => w.ID == id);
                    bool existingReqUser = await db.C_Req_Users
                     .AnyAsync(rc => rc.USERNAME == user.USERNAME && rc.CORPORATEID == user.CORPORATEID && rc.STATUS == 0);
                    if (existingReqUser)
                    {
                        return Json(new { title = "Warning", message = "Already Requested!" });
                    }
                    if (!existingUser)
                    {
                        return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                    }

                    //check user name
                    var result = await UserNameIsNotExist(model.UserName, id, user.ID, model.CorpID.GetValueOrDefault());
                    if (result is string)
                    {
                        return new JsonErrorResult(new { error = result }, HttpStatusCode.BadRequest);
                    }

                    //no need to update the user as delete immediately, it will update after approve
                    //update existing user as delete
                    //existingUser.DEL_STATUS = true;
                    //await db.SaveChangesAsync();

                    //get roles
                    var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                    //get menus
                    var allMenus = await db.C_Menus.Where(c => c.IsDelete == false).AsNoTracking().ToListAsync();

                    //corporate user
                    var corpUser = CorporateUserRequest.GetReqUser(model, false, branchId, loginUser);
                    db.C_Req_Users.Add(corpUser);
                    await db.SaveChangesAsync();

                    //transaction Info
                    if (model.IsTransaction)
                    {
                        //maker
                        if (model.IsMaker)
                        {
                            //transaction limit
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(3, model.MakerBulkTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(1, model.MakerTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            await db.SaveChangesAsync();
                        }
                        //viewer
                        if (model.IsApprover)
                        {
                            //transaction limit
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(4, model.CheckerBulkTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(2, model.CheckerTranLimit, corpUser.CORPORATEID.GetValueOrDefault(), corpUser.ID, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }

                    //role info and menu info
                    if (model.IsAdmin)
                    {
                        //admin maker
                        if (model.IsAdminMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINMAKER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(1, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //admin approver
                        if (model.IsAdminApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINAPPROVER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(2, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        //maker
                        if (model.IsMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.MAKER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(3, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //viewer
                        if (model.IsViewer)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.VIEWER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(4, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //approver
                        if (model.IsApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.APPROVER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(5, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }

                    //account info
                    db.C_Req_UserInAccounts.AddRange(CorporateUserRequest.GetReqUserInAccounts(accountInfo, corpUser.ID, model.CorpID.GetValueOrDefault(), loginUser));
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbContextTransaction.Commit();

                    log.Info(loginUser, controllerName, actionName, 1, $"{corpUser.USERNAME} is updated.");
                    return Json(new { title = "Success", message = "Updated Successfully!" });
                }
                catch (Exception ex)
                {
                    //rollback transaction
                    dbContextTransaction.Rollback();
                    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                    return new JsonErrorResult(new { error = "Error while updating Corporate User Registration!" });
                }
            }
        }

#endregion Corporate User

#region Request Corporate User

        /// <summary>
        /// GetReqCorporateData
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        //[HttpPost]
        //public async Task<JsonResult> GetReqCorporateData(string term)
        //{
        //    var data = await db.C_Req_Corporates
        //        .Where(w => w.COMPANY_NAME.Contains(term))
        //        .Select(s => new
        //        {
        //            ID = s.ID,
        //            CorporateID = s.CORPORATEID,
        //            CompanyName = s.COMPANY_NAME,
        //            Cifid = s.CIFID
        //        }).OrderBy(o => o.CompanyName).AsNoTracking().ToListAsync();
        //    return Json(data);
        //}

        /// <summary>
        /// GetReqUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetReqUserReg(int id)
        {
            try
            {
                //get roles
                var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                //query data
                var user = await (from u in db.C_Req_Users
                                  join c in db.C_Corporates on u.CORPORATEID equals c.ID
                                  where u.ID == id
                                  select new CopUserRegVM
                                  {
                                      Id = u.ID,
                                      CompanyName = c.COMPANY_NAME,
                                      UserName = u.USERNAME,
                                      FullName = u.FULLNAME,
                                      Email = u.EMAIL,
                                      PhoneNo = u.MOBILENO,
                                      Address = u.ADDRESS,
                                      CorpID = u.CORPORATEID,
                                      CorporateID = c.CORPORATEID,
                                      CIFID = c.CIFID,
                                      DepartmentId = u.DepartmentID,
                                      PositionId = u.PositionID,
                                      Userroles = db.C_Req_UserInRoles.Where(w => w.ReqUserID == u.ID).Select(s => s.RoleId).ToList()
                                  }).AsNoTracking().FirstOrDefaultAsync();

                if (user == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                user.MakerTranLimit = await db.C_Req_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.MAKERTRANLIMIT_NAME && w.ReqUserID == user.Id && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.CheckerTranLimit = await db.C_Req_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.CHECKERTRANLIMIT_NAME && w.ReqUserID == user.Id && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.MakerBulkTranLimit = await db.C_Req_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME && w.ReqUserID == user.Id && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();
                user.CheckerBulkTranLimit = await db.C_Req_UserTranLimits.Where(w => w.RuleCode == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME && w.ReqUserID == user.Id && w.CorporateId == user.CorpID).Select(s => s.Value).FirstOrDefaultAsync();

                //check maker tran limit
                if (user.MakerTranLimit == 0)
                {
                    user.MakerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check checker tran limit
                if (user.CheckerTranLimit == 0)
                {
                    user.CheckerTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check maker bulk tran limit
                if (user.MakerBulkTranLimit == 0)
                {
                    user.MakerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.MAKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }
                //check checker bulk tran limit
                if (user.CheckerBulkTranLimit == 0)
                {
                    user.CheckerBulkTranLimit = await db.Rules.Where(w => w.Code == CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME).Select(s => s.Value).FirstOrDefaultAsync();
                }

                //check user role
                foreach (var item in user.Userroles)
                {
                    var role = user_Role.FirstOrDefault(s => s.Id == item);
                    if (role != null)
                    {
                        if (role.isAdmin.GetValueOrDefault())
                        {
                            user.IsAdmin = true;
                            if (role.Name.Equals(CorporateUserRequest.ADMINMAKER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsAdminMaker = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.ADMINAPPROVER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsAdminApprover = true;
                            }
                        }
                        else
                        {
                            user.IsTransaction = true;
                            if (role.Name.Equals(CorporateUserRequest.MAKER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsMaker = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.VIEWER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsViewer = true;
                            }
                            else if (role.Name.Equals(CorporateUserRequest.APPROVER_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                user.IsApprover = true;
                            }
                        }
                    }
                }
                return Json(user);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while loading User Data!" });
            }
        }

        /// <summary>
        /// GetReqMenuInfo
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetReqMenuInfo(RoleInfoVM data, int id)
        {
            try
            {
                List<MenuInfoVM> menuData = new List<MenuInfoVM>();

                //get existing menu
                var existingMenus = await db.C_Req_MenuPermissions.Where(w => w.ReqUserID == id).ToListAsync();

                //get menus
                var allMenus = await db.C_Menus.Where(c => c.IsDelete == false).AsNoTracking().ToListAsync();

                //check user type
                if (data.IsAdmin)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.ADMIN_NAME && w.IsDelete == false)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //admin maker
                        if (data.IsAdminMaker)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINMAKER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsAdminMakerEnable = true;
                                menu.IsAdminMaker = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //admin approver
                        if (data.IsAdminApprover)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINAPPROVER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsAdminApproverEnable = true;
                                menu.IsAdminApprover = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        menuData.Add(menu);
                    }
                }
                else if (data.IsTransaction)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.TRANSACTION_NAME && w.IsDelete == false)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //maker
                        if (data.IsMaker)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.MAKER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsMakerEnable = true;
                                menu.IsMaker = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //viewer
                        if (data.IsViewer)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.VIEWER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsViewerEnable = true;
                                menu.IsViewer = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        //approver
                        if (data.IsApprover)
                        {
                            var menuIds = allMenus.Where(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.APPROVER_NAME).Select(s => s.Id);
                            if (menuIds != null && menuIds.Count() > 0)
                            {
                                menu.IsApproverEnable = true;
                                menu.IsApprover = existingMenus.Any(w => menuIds.Contains(w.MenuId));
                            }
                        }
                        menuData.Add(menu);
                    }
                }

                if (menuData == null || menuData.Count == 0)
                {
                    return new JsonErrorResult(new { error = "Menu Not Found!" }, HttpStatusCode.BadRequest);
                }
                return Json(menuData);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while loading Menu Data!" });
            }
        }

        /// <summary>
        /// GetReqAccountInfo
        /// </summary>
        /// <param name="CIFID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> GetReqAccountInfo(string CIFID, int id)
        {
            try
            {


                CorporateResponseInfo cri = null;

                //check cifid exist
                if (!string.IsNullOrEmpty(CIFID))
                {
                    //int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    //if (count > 0)
                    //    return Json(new { message = "Duplicate CIFID", error = true });

                    //get accounts by cifid
                    cri = CorporateUserRequest.GetCorporateResponse(CIFID, connectionString);

                    //check accounts
                    if (cri != null && !string.IsNullOrEmpty(cri.Name)
                        && cri.lstCorpAccountInfo != null && cri.lstCorpAccountInfo.Count != 0)
                    {
                        var accData = cri.lstCorpAccountInfo.Select(s => new AccountInfoVM
                        {
                            AccountNo = s.AccountNumber,
                            AccountType = s.AccountType,
                            AccountTypeDesc = s.AccountTypeDesc,
                            Currency = s.Currency,
                            IsActive = false
                        }).ToList();

                        //check existing for edit
                        if (id != 0)
                        {
                            //get existing account
                            var existingAcc = await db.C_Req_UserInAccounts.Where(w => w.ReqUserID == id).AsNoTracking().ToListAsync();
                            if (existingAcc != null && existingAcc.Count() != 0)
                            {
                                //check api data with existing account
                                accData.ForEach(x =>
                                    x.IsActive = existingAcc.Any(e => e.AccountNo.Equals(x.AccountNo, StringComparison.CurrentCultureIgnoreCase))
                                );
                            }
                        }
                        return Json(accData);
                    }
                }
                return new JsonErrorResult(new { error = "No individual account under this CIFID!" }, HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while loading Account Data!" });
            }
        }

        /// <summary>
        /// UpdateReqCorporateUserReg
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="menuInfo"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateReqCorporateUserReg(int id, CopUserRegVM model, List<MenuInfoVM> menuInfo, List<AccountInfoVM> accountInfo)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            //start transaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get login user id
                    string loginUser = User.Identity.GetUserId();
                    int userId = 0;
                    var branchId = await db.Users.Where(u => u.Id == loginUser).Select(s => s.BranchID).FirstOrDefaultAsync();

                    //get user by id
                    var corpUser = await db.C_Req_Users.Where(w => w.ID == id).FirstOrDefaultAsync();
                    if (corpUser == null)
                    {
                        return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                    }

                    //not to allow editing approved requests
                    if (corpUser.STATUS == 1) //if approved
                    {
                        return new JsonErrorResult(new { error = "You can't change the approved request!" }, HttpStatusCode.BadRequest);
                    }

                    //not to allow editing rejected requests
                    if (corpUser.STATUS == 2) //if approved
                    {
                        return new JsonErrorResult(new { error = "You can't change the rejected request!" }, HttpStatusCode.BadRequest);
                    }
                    var user = db._Users.Where(u => u.CORPORATEID == corpUser.CORPORATEID && u.USERNAME == corpUser.USERNAME).FirstOrDefault();
                    if (user == null)
                    {
                        userId = 0;
                    }
                    else
                    {
                        userId = user.ID;
                    }
                    var result = await UserNameIsNotExist(model.UserName, id, userId, model.CorpID.GetValueOrDefault());
                    //check user name
                    if (result is string)
                    {
                        return new JsonErrorResult(new { error = result }, HttpStatusCode.BadRequest);
                    }

                    //get roles
                    var user_Role = await db.C_Role.AsNoTracking().ToListAsync();

                    //get menus
                    var allMenus = await db.C_Menus.Where(c => c.IsDelete == false).AsNoTracking().ToListAsync();

                    //update main data
                    corpUser.USERNAME = model.UserName;
                    corpUser.FULLNAME = model.FullName;
                    corpUser.EMAIL = model.Email;
                    corpUser.MOBILENO = model.PhoneNo;
                    corpUser.ADDRESS = model.Address;
                    corpUser.DepartmentID = model.DepartmentId;
                    corpUser.PositionID = model.PositionId;
                    //corpUser.ISNEW = false;
                    corpUser.REQUESTEDDATE = DateTime.Now;
                    corpUser.STATUS = 0;
                    corpUser.MAKER = loginUser;
                    corpUser.BranchID = branchId;
                    //requser.UPDATEDATE = DateTime.Now;
                    //requser.UPDATEUSERID = LoginUser;

                    //save
                    await db.SaveChangesAsync();

                    //trans Info
                    //maker
                    var makerBulkTranLimit = await db.C_Req_UserTranLimits.FirstOrDefaultAsync(w => w.ReqUserID == corpUser.ID && w.RuleCode.Equals(CorporateUserRequest.MAKERBULKTRANLIMIT_NAME));
                    var makerTranLimit = await db.C_Req_UserTranLimits.FirstOrDefaultAsync(w => w.ReqUserID == corpUser.ID && w.RuleCode.Equals(CorporateUserRequest.MAKERTRANLIMIT_NAME));
                    if (model.IsMaker)
                    {
                        if (makerBulkTranLimit != null && makerBulkTranLimit.Value != model.MakerBulkTranLimit)
                        {
                            makerBulkTranLimit.Value = model.MakerBulkTranLimit;
                        }
                        else
                        {
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(3, model.MakerBulkTranLimit, model.CorpID.GetValueOrDefault(), corpUser.ID, loginUser));
                        }

                        if (makerTranLimit != null && makerTranLimit.Value != model.MakerTranLimit)
                        {
                            makerTranLimit.Value = model.MakerTranLimit;
                        }
                        else
                        {
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(1, model.MakerTranLimit, model.CorpID.GetValueOrDefault(), corpUser.ID, loginUser));
                        }
                    }
                    else
                    {
                        if (makerBulkTranLimit != null)
                        {
                            db.C_Req_UserTranLimits.Remove(makerBulkTranLimit);
                        }
                        if (makerTranLimit != null)
                        {
                            db.C_Req_UserTranLimits.Remove(makerTranLimit);
                        }
                    }

                    //checker
                    var checkerBulkTranLimit = await db.C_Req_UserTranLimits.FirstOrDefaultAsync(w => w.ReqUserID == corpUser.ID && w.RuleCode.Equals(CorporateUserRequest.CHECKERBULKTRANLIMIT_NAME));
                    var checkerTranLimit = await db.C_Req_UserTranLimits.FirstOrDefaultAsync(w => w.ReqUserID == corpUser.ID && w.RuleCode.Equals(CorporateUserRequest.CHECKERTRANLIMIT_NAME));
                    if (model.IsApprover)
                    {
                        if (checkerBulkTranLimit != null && checkerBulkTranLimit.Value != model.CheckerBulkTranLimit)
                        {
                            checkerBulkTranLimit.Value = model.CheckerBulkTranLimit;
                        }
                        else
                        {
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(4, model.CheckerBulkTranLimit, model.CorpID.GetValueOrDefault(), corpUser.ID, loginUser));
                        }

                        if (checkerTranLimit != null && checkerTranLimit.Value != model.CheckerTranLimit)
                        {
                            checkerTranLimit.Value = model.CheckerTranLimit;
                        }
                        else
                        {
                            db.C_Req_UserTranLimits.Add(CorporateUserRequest.GetReqUserTranLimit(2, model.CheckerTranLimit, model.CorpID.GetValueOrDefault(), corpUser.ID, loginUser));
                        }
                    }
                    else
                    {
                        if (checkerBulkTranLimit != null)
                        {
                            db.C_Req_UserTranLimits.Remove(checkerBulkTranLimit);
                        }
                        if (checkerTranLimit != null)
                        {
                            db.C_Req_UserTranLimits.Remove(checkerTranLimit);
                        }
                    }

                    await db.SaveChangesAsync();

                    //role info and menu info
                    //delete menus if exist
                    var menus = await db.C_Req_MenuPermissions.Where(w => w.ReqUserID == corpUser.ID).ToListAsync();
                    if (menus != null && menus.Count != 0)
                    {
                        db.C_Req_MenuPermissions.RemoveRange(menus);
                        await db.SaveChangesAsync();
                    }

                    //delete roles if exist
                    var roles = await db.C_Req_UserInRoles.Where(w => w.ReqUserID == corpUser.ID).ToListAsync();
                    if (roles != null && roles.Count != 0)
                    {
                        db.C_Req_UserInRoles.RemoveRange(roles);
                        await db.SaveChangesAsync();
                    }

                    //add new role and menu
                    //admin
                    if (model.IsAdmin)
                    {
                        //maker
                        if (model.IsAdminMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINMAKER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(1, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //approver
                        if (model.IsAdminApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.ADMINAPPROVER_NAME, true);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(2, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        //maker
                        if (model.IsMaker)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.MAKER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(3, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //viewer
                        if (model.IsViewer)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.VIEWER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(4, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }

                        //approver
                        if (model.IsApprover)
                        {
                            int roleId = CorporateUserRequest.GetRoleId(user_Role, CorporateUserRequest.APPROVER_NAME, false);
                            db.C_Req_UserInRoles.Add(CorporateUserRequest.GetReqUserInRole(corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();

                            //save menu where checked
                            db.C_Req_MenuPermissions.AddRange(CorporateUserRequest.GetReqMenuPermissions(5, menuInfo, allMenus, corpUser.ID, roleId, loginUser));
                            await db.SaveChangesAsync();
                        }
                    }

                    //account info
                    //delete existing account
                    var accounts = await db.C_Req_UserInAccounts.Where(w => w.ReqUserID == corpUser.ID).ToListAsync();
                    if (accounts != null && accounts.Count != 0)
                    {
                        db.C_Req_UserInAccounts.RemoveRange(accounts);
                        await db.SaveChangesAsync();
                    }

                    //add new account
                    db.C_Req_UserInAccounts.AddRange(CorporateUserRequest.GetReqUserInAccounts(accountInfo, corpUser.ID, model.CorpID.GetValueOrDefault(), loginUser));
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbContextTransaction.Commit();

                    log.Info(loginUser, controllerName, actionName, 2, $"{corpUser.USERNAME} is updated.");
                    return Json(new { title = "Success", message = "Updated Successfully!" });
                }
                catch (Exception ex)
                {
                    //rollback transaction
                    dbContextTransaction.Rollback();
                    log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                    return new JsonErrorResult(new { error = "Error while updating Corporate User Registration!" });
                }
            }
        }

#endregion Request Corporate User

#region General

        /// <summary>
        /// GetMenu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetMenu(RoleInfoVM data)
        {
            try
            {
                List<MenuInfoVM> menuData = new List<MenuInfoVM>();
                //check user type
                if (data.IsAdmin)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.ADMIN_NAME && w.IsDelete == false)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //admin maker
                        if (data.IsAdminMaker)
                        {
                            bool isEnable = await db.C_Menus.AnyAsync(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINMAKER_NAME && w.IsDelete == false);
                            if (isEnable)
                            {
                                menu.IsAdminMakerEnable = true;
                            }
                        }
                        //admin approver
                        if (data.IsAdminApprover)
                        {
                            bool isEnable = await db.C_Menus.AnyAsync(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.ADMINAPPROVER_NAME && w.IsDelete == false);
                            if (isEnable)
                            {
                                menu.IsAdminApproverEnable = true;
                            }
                        }
                        menuData.Add(menu);
                    }
                }
                else if (data.IsTransaction)
                {
                    //get parent menu
                    var parentMenus = await db.C_Menus.Where(w => w.MenuType == CorporateUserRequest.TRANSACTION_NAME && w.IsDelete == false)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var parentMenu in parentMenus)
                    {
                        var menu = new MenuInfoVM
                        {
                            MenuInfoId = parentMenu.Id,
                            MenuName = parentMenu.Name
                        };

                        //maker
                        if (data.IsMaker)
                        {
                            bool isEnable = await db.C_Menus.AnyAsync(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.MAKER_NAME && w.IsDelete == false);
                            if (isEnable)
                            {
                                menu.IsMakerEnable = true;
                            }
                        }
                        //viewer
                        if (data.IsViewer)
                        {
                            bool isEnable = await db.C_Menus.AnyAsync(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.VIEWER_NAME);
                            if (isEnable)
                            {
                                menu.IsViewerEnable = true;
                            }
                        }
                        //approver
                        if (data.IsApprover)
                        {
                            bool isEnable = await db.C_Menus.AnyAsync(w => w.ParentId == parentMenu.Id && w.MenuType == CorporateUserRequest.APPROVER_NAME);
                            if (isEnable)
                            {
                                menu.IsApproverEnable = true;
                            }
                        }
                        menuData.Add(menu);
                    }
                }
                if (menuData == null || menuData.Count == 0)
                {
                    return new JsonErrorResult(new { error = "Menu Not Found!" }, HttpStatusCode.BadRequest);
                }
                return Json(menuData);
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Menu Data!" });
            }
        }

        /// <summary>
        /// GetAccount
        /// </summary>
        /// <param name="CIFID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> GetAccount(string CIFID)
        {
            try
            {

                CorporateResponseInfo cri = null;

                //check cifid exist
                if (!string.IsNullOrEmpty(CIFID))
                {
                    //int count = db._Users.Where(u => u.CIFID == CIFID & u.DEL_STATUS == false).Count();
                    //if (count > 0)
                    //    return Json(new { message = "Duplicate CIFID", error = true });

                    //get accounts by cifid
                    cri = CorporateUserRequest.GetCorporateResponse(CIFID, connectionString);

                    //check accounts
                    if (cri != null && !string.IsNullOrEmpty(cri.Name)
                        && cri.lstCorpAccountInfo != null && cri.lstCorpAccountInfo.Count != 0)
                    {
                        var accData = cri.lstCorpAccountInfo.Select(s => new AccountInfoVM
                        {
                            AccountNo = s.AccountNumber,
                            AccountType = s.AccountType,
                            AccountTypeDesc = s.AccountTypeDesc,
                            Currency = s.Currency,
                            IsActive = false
                        }).ToList();
                        return Json(accData);
                    }
                }
                return new JsonErrorResult(new { error = "No individual account under this CIFID!" }, HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return new JsonErrorResult(new { error = "Error while loading Account Data!" });
            }
        }

        /// <summary>
        /// CheckUserName
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckUserName(string checkData, string checkDataID = "")
        {
            try
            {
                bool isDataExists = false;
               // checkData = checkData.Trim();
               //{
               //     //check in User
               //     isDataExists = await db._Users.AsNoTracking().AnyAsync(x =>
               //         x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
               //         && x.ID != CheckDataID
               //     );
               // }
               // else
               // { if (int.TryParse(checkDataID, out int CheckDataID))
                
               //     //check in User
               //     isDataExists = await db._Users.AsNoTracking().AnyAsync(x =>
               //         x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
               //     );
               // }
                return Json(!isDataExists);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while checking Username in User List!" });
            }
        }

        /// <summary>
        /// CheckReqUserName
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckReqUserName(string checkData, string checkDataID = "")
        {
            try
            {
                bool isDataExists = false;
                checkData = checkData.Trim();
                //if (int.TryParse(checkDataID, out int CheckDataID)
                //{
                //    //check in req user
                //    isDataExists = await db.C_Req_Users.AsNoTracking().AnyAsync(x =>
                //            x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                //            && x.STATUS == 0
                //            && x.ID != CheckDataID
                //    );
                //}
                //else
                //{
                //    //check in req user
                //    isDataExists = await db.C_Req_Users.AsNoTracking().AnyAsync(x =>
                //            x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                //            && x.STATUS == 0
                //    );
                //}
                return Json(!isDataExists);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while checking Username in Request User List!" });
            }
        }

        /// <summary>
        /// CheckValidUserName
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckValidUserName(string checkData, int? checkDataID, int? checkuserID, int? corporateId)
        {
            try
            {
                checkData = checkData.Trim();
                var result = await UserNameIsNotExist(checkData, checkDataID.GetValueOrDefault(), checkuserID.GetValueOrDefault(), corporateId.GetValueOrDefault());
                return Json(result);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while loading Username List!" });
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

#endregion General

#region Private Method

        /// <summary>
        /// UserNameIsNotExist
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        private async Task<object> UserNameIsNotExist(string checkData, int checkDataID, int checkuserID, int corporateId)
        {
            bool isDataExists = false;
            if (corporateId == 0)
            {
                return "Please select company name first";
            }

            if (checkDataID != 0)
            {
                //check in User
                isDataExists = await db._Users.AsNoTracking().AnyAsync(x =>
                    x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                    && x.ID != checkuserID
                    && x.USER_TYPE == "Corporate"
                    && x.CORPORATEID == corporateId
                );

                //check in req user
                if (isDataExists)
                {
                    return "Duplicate USERNAME";
                }
                else
                {
                    isDataExists = await db.C_Req_Users.AsNoTracking().AnyAsync(x =>
                        x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                        && x.STATUS == 0
                        && x.ISNEW == true
                        && x.ID != checkDataID
                        && x.CORPORATEID == corporateId
                    );
                    if (isDataExists)
                    {
                        return "Duplicate Requested USERNAME";
                    }
                }
            }
            else
            {
                //check in User
                isDataExists = await db._Users.AsNoTracking().AnyAsync(x =>
                    x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                    && x.USER_TYPE == "Corporate"
                    && x.CORPORATEID == corporateId
                );

                //check in req user
                if (isDataExists)
                {
                    return "Duplicate USERNAME";
                }
                else
                {
                    isDataExists = await db.C_Req_Users.AsNoTracking().AnyAsync(x =>
                        x.USERNAME.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                        && x.STATUS == 0
                        && x.ISNEW == true
                        && x.CORPORATEID == corporateId
                    );
                    if (isDataExists)
                    {
                        return "Duplicate Requested USERNAME";
                    }
                }
            }
            return true;
        }

#endregion Private Method
    }
}