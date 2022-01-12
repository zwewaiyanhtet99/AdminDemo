using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using AbankAdminAPI;
using AbankAdminAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Utils
{
    /// <summary>
    /// CorporateUserRequest
    /// </summary>
    public static class CorporateUserRequest
    {
        /// <summary>
        /// Converts to DBNull, if Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBNull(object value)
        {
            if (null != value)
                return value;
            return DBNull.Value;
        }

        /// <summary>
        /// GetRetailResponse
        /// </summary>
        /// <param name="CIFID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Obsolete]
        public static RetailResponseInfo GetRetailResponse(string CIFID, string connectionString)
        {
            RetailResponseInfo rri = new RetailResponseInfo();
            try
            {
                CustomerInformationInquiry custInfoInquiry = new CustomerInformationInquiry();

                rri = custInfoInquiry.SelectCustomerInformation(CIFID, connectionString);
            }
            catch (Exception ex)
            {
                throw;
            }
            return rri;
        }

        /// <summary>
        /// GetCorporateResponse
        /// </summary>
        /// <param name="CIFID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Obsolete]
        public static CorporateResponseInfo GetCorporateResponse(string CIFID, string connectionString)
        {
            CorporateResponseInfo cri = new CorporateResponseInfo();
            try
            {
                CustomerInformationInquiry custInfoInquiry = new CustomerInformationInquiry();

                cri = custInfoInquiry.SelectCorporateInformation(CIFID, connectionString);
            }
            catch (Exception ex)
            {
                throw;
            }
            return cri;
        }

        /// <summary>
        /// GetRoles
        /// </summary>
        /// <param name="data"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static string GetRoles(List<C_Role> data, List<int> roles)
        {
            List<string> roleNames = new List<string>();
            foreach (var item in roles)
            {
                var roleName = data.Where(w => w.Id == item).Select(s => s.Name).FirstOrDefault();
                if (!string.IsNullOrEmpty(roleName))
                {
                    roleNames.Add(roleName);
                }
            }
            return string.Join(", ", roleNames);
        }

        /// <summary>
        /// GetRoleId
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static int GetRoleId(List<C_Role> data, string name, bool isAdmin = false)
        {
            int id = data.Where(w => w.isAdmin.GetValueOrDefault() == isAdmin && w.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Id)
                .FirstOrDefault();
            return id;
        }

        /// <summary>
        /// GetStatus
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetStatus(int id)
        {
            string status;
            switch (id)
            {
                case 0:
                    status = "Requested";
                    break;

                case 1:
                    status = "Approved";
                    break;

                case 2:
                    status = "Rejected";
                    break;

                default:
                    status = "Unknown";
                    break;
            }
            return status;
        }

        /// <summary>
        /// GetType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetUserType(int id)
        {
            string type;
            switch (id)
            {
                case 2:
                    type = "Reset Password";
                    break;

                case 3:
                    type = "Lock";
                    break;

                case 4:
                    type = "Unlock";
                    break;

                case 5:
                    type = "Tran Lock";
                    break;

                case 6:
                    type = "Tran Unlock";
                    break;

                case 7:
                    type = "Delete";
                    break;

                default:
                    type = "Unknown";
                    break;
            }
            return type;
        }

        /// <summary>
        /// GetUserRole
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetUserRole(int id)
        {
            string role;
            switch (id)
            {
                case 1:
                    role = "Maker";
                    break;

                case 2:
                    role = "Viewer";
                    break;

                case 3:
                    role = "Approver";
                    break;

                case 4:
                    role = "AdminMaker";
                    break;

                case 5:
                    role = "AdminApprover";
                    break;

                default:
                    role = "Unknown";
                    break;
            }
            return role;
        }

        /// <summary>
        /// GetType
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetType(bool status)
        {
            string type;
            switch (status)
            {
                case true:
                    type = "Register";
                    break;

                default:
                    type = "Update";
                    break;
            }
            return type;
        }

        /// <summary>
        /// GetTypeList
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetTypeList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem {Text="Register",Value="true"},
                new SelectListItem {Text="Update",Value="false"}
            };
        }

        /// <summary>
        /// GetStatusList
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetStatusList()
        {
            return new List<SelectListItem>()
            {
                //new SelectListItem{Text="All",Value=null},
                new SelectListItem{Text="Requested",Value="0"},
                new SelectListItem{Text="Approved",Value="1"},
                new SelectListItem{Text="Rejected",Value="2"},
                new SelectListItem{Text="All",Value=null}
            };
        }

        /// <summary>
        /// GetChangesTypeList
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetChangesTypeList()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Reset Password",Value="2"},
                    new SelectListItem{Text="Lock",Value="3"},
                    new SelectListItem{Text="Unlock",Value="4"},
                    new SelectListItem{Text="Tran Lock",Value="5"},
                    new SelectListItem{Text="Tran Unlock",Value="6"},
                    new SelectListItem{Text="Delete",Value="7"}
                };
        }

        /// <summary>
        /// GetReqUserTranLimit
        /// </summary>
        /// <param name="limitType"></param>
        /// <param name="tranLimit"></param>
        /// <param name="corporateId"></param>
        /// <param name="reqUserId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static C_Req_UserTranLimit GetReqUserTranLimit(int limitType, decimal tranLimit, int corporateId, int reqUserId, string userId)
        {
            C_Req_UserTranLimit req_UserTranLimit;
            switch (limitType)
            {
                case 1:
                    req_UserTranLimit = new C_Req_UserTranLimit
                    {
                        RuleCode = MAKERTRANLIMIT_NAME,
                        Value = tranLimit,
                        CorporateId = corporateId,
                        ReqUserID = reqUserId,
                        UserId = userId
                    };
                    break;

                case 2:
                    req_UserTranLimit = new C_Req_UserTranLimit
                    {
                        RuleCode = CHECKERTRANLIMIT_NAME,
                        Value = tranLimit,
                        CorporateId = corporateId,
                        ReqUserID = reqUserId,
                        UserId = userId
                    };
                    break;

                case 3:
                    req_UserTranLimit = new C_Req_UserTranLimit
                    {
                        RuleCode = MAKERBULKTRANLIMIT_NAME,
                        Value = tranLimit,
                        CorporateId = corporateId,
                        ReqUserID = reqUserId,
                        UserId = userId
                    };
                    break;

                case 4:
                    req_UserTranLimit = new C_Req_UserTranLimit
                    {
                        RuleCode = CHECKERBULKTRANLIMIT_NAME,
                        Value = tranLimit,
                        CorporateId = corporateId,
                        ReqUserID = reqUserId,
                        UserId = userId
                    };
                    break;

                default:
                    throw new ArgumentNullException();
            }
            return req_UserTranLimit;
        }

        /// <summary>
        /// GetReqUserInRole
        /// </summary>
        /// <param name="reqUserId"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static C_Req_UserInRole GetReqUserInRole(int reqUserId, int roleId, string userId)
        {
            return new C_Req_UserInRole
            {
                ReqUserID = reqUserId,
                RoleId = roleId,
                UserId = userId
            };
        }

        /// <summary>
        /// GetReqMenuPermissions
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="menuInfo"></param>
        /// <param name="menus"></param>
        /// <param name="reqUserId"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<C_Req_MenuPermission> GetReqMenuPermissions(int menuType, IEnumerable<MenuInfoVM> menuInfo, IEnumerable<C_Menu> menus, int reqUserId, int roleId, string userId)
        {
            List<C_Req_MenuPermission> menuPermissions = new List<C_Req_MenuPermission>();
            switch (menuType)
            {
                case 1:
                    var adminMakerMenuInfos = menuInfo.Where(w => w.IsAdminMaker);
                    foreach (var parentMenu in adminMakerMenuInfos)
                    {
                        var cMenus = menus.Where(w => w.ParentId == parentMenu.MenuInfoId && w.MenuType == ADMINMAKER_NAME && w.IsDelete == false)
                            .Select(s => new C_Req_MenuPermission
                            {
                                MenuId = s.Id,
                                RoleId = roleId,
                                ReqUserID = reqUserId,
                                UserId = userId
                            });
                        menuPermissions.AddRange(cMenus);
                    }
                    break;

                case 2:
                    var adminApproverMenuInfos = menuInfo.Where(w => w.IsAdminApprover);
                    foreach (var parentMenu in adminApproverMenuInfos)
                    {
                        var cMenus = menus.Where(w => w.ParentId == parentMenu.MenuInfoId && w.MenuType == ADMINAPPROVER_NAME && w.IsDelete == false)
                            .Select(s => new C_Req_MenuPermission
                            {
                                MenuId = s.Id,
                                RoleId = roleId,
                                ReqUserID = reqUserId,
                                UserId = userId
                            });
                        menuPermissions.AddRange(cMenus);
                    }
                    break;

                case 3:
                    var makerMenuInfos = menuInfo.Where(w => w.IsMaker);
                    foreach (var parentMenu in makerMenuInfos)
                    {
                        var cMenus = menus.Where(w => w.ParentId == parentMenu.MenuInfoId && w.MenuType == MAKER_NAME && w.IsDelete == false)
                            .Select(s => new C_Req_MenuPermission
                            {
                                MenuId = s.Id,
                                RoleId = roleId,
                                ReqUserID = reqUserId,
                                UserId = userId
                            });
                        menuPermissions.AddRange(cMenus);
                    }
                    break;

                case 4:
                    var viewerMenuInfos = menuInfo.Where(w => w.IsViewer);
                    foreach (var parentMenu in viewerMenuInfos)
                    {
                        var cMenus = menus.Where(w => w.ParentId == parentMenu.MenuInfoId && w.MenuType == VIEWER_NAME && w.IsDelete == false)
                            .Select(s => new C_Req_MenuPermission
                            {
                                MenuId = s.Id,
                                RoleId = roleId,
                                ReqUserID = reqUserId,
                                UserId = userId
                            });
                        menuPermissions.AddRange(cMenus);
                    }
                    break;

                case 5:
                    var approverMenuInfos = menuInfo.Where(w => w.IsApprover);
                    foreach (var parentMenu in approverMenuInfos)
                    {
                        var cMenus = menus.Where(w => w.ParentId == parentMenu.MenuInfoId && w.MenuType == APPROVER_NAME && w.IsDelete == false)
                            .Select(s => new C_Req_MenuPermission
                            {
                                MenuId = s.Id,
                                RoleId = roleId,
                                ReqUserID = reqUserId,
                                UserId = userId
                            });
                        menuPermissions.AddRange(cMenus);
                    }
                    break;

                default:
                    throw new ArgumentNullException();
            }
            return menuPermissions;
        }

        /// <summary>
        /// GetReqUserInAccounts
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="reqUserId"></param>
        /// <param name="corpID"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<C_Req_UserInAccount> GetReqUserInAccounts(IEnumerable<AccountInfoVM> accountInfo, int reqUserId, int corpID, string userId)
        {
            return accountInfo.Where(w => w.IsActive).Select(s => new C_Req_UserInAccount
            {
                AccountNo = s.AccountNo,
                AccountType = s.AccountType,
                ReqUserID = reqUserId,
                AccountTypeDesc = s.AccountTypeDesc,
                Currency = s.Currency,
                CorporateId = corpID,
                UserId = userId
            });
        }

        /// <summary>
        /// GetReqUser
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isNew"></param>
        /// <param name="branchId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static C_Req_User GetReqUser(CopUserRegVM model, bool isNew, int branchId, string userId)
        {
            return new C_Req_User
            {
                CORPORATEID = model.CorpID,
                USERNAME = model.UserName,
                FULLNAME = model.FullName,
                EMAIL = model.Email,
                MOBILENO = model.PhoneNo,
                ADDRESS = model.Address,
                DepartmentID = model.DepartmentId,
                PositionID = model.PositionId,
                ISNEW = isNew,
                REQUESTEDDATE = DateTime.Now,
                STATUS = 0,
                MAKER = userId,
                BranchID = branchId
            };
        }

        /// <summary>
        /// GetReqChanges
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        /// <param name="branchId"></param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public static C_Req_Changes GetReqChanges(int id, byte type, byte status, string userId, int? branchId, int corporateId)
        {
            return new C_Req_Changes
            {
                USERID = id,
                TYPE = type,
                STATUS = status,
                MAKER = userId,
                REQUESTEDDATE = DateTime.Now,
                BranchID = branchId,
                CORPORATEID = corporateId
            };
        }

        #region Constant

        public const string FAILTRANPWDCOUNT_NAME = "R002";
        public const string MAKERTRANLIMIT_NAME = "R041";
        public const string CHECKERTRANLIMIT_NAME = "R042";
        public const string MAKERBULKTRANLIMIT_NAME = "R045";
        public const string CHECKERBULKTRANLIMIT_NAME = "R046";
        public const string ADMIN_NAME = "AdminRoot";
        public const string TRANSACTION_NAME = "TranRoot";
        public const string ADMINMAKER_NAME = "AdminMaker";
        public const string ADMINAPPROVER_NAME = "AdminApprover";
        public const string MAKER_NAME = "Maker";
        public const string VIEWER_NAME = "Viewer";
        public const string APPROVER_NAME = "Approver";
        public const string DATEFORMAT = "dd-MM-yyyy";
        public const int EDITCORPORATEUSER = 2;
        public const int DETAILCORPORATEUSER = 3;
        public const int EDITREQCORPORATEUSER = 4;
        public const int DETAIREQLCORPORATEUSER = 5;

        #endregion Constant
    }
}