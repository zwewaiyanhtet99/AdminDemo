using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.Utils;
using ABankAdmin.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ABankAdmin.Controllers
{
    public class CorporateUserReportController : Controller
    {
        private readonly AdminDBContext db = new AdminDBContext();

        private readonly string controllerName = "CorporateUserReportController";
        private readonly EventLogController log = new EventLogController();

        //get connection string
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        /// <summary>
        /// CorporateUserSummary
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CorporateUserSummary()
        {
            //get user roles
            var user_Role = await db.C_Role.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            }).OrderBy(o => o.Name).AsNoTracking().ToListAsync();
            ViewBag.UserRole = new SelectList(user_Role, "Id", "Name");

            return View();
        }

        /// <summary>
        /// CorporateUserSummary
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CorporateUserSummary(CorporateUserSearchVM data)
        {
            try
            {
                ViewBag.CompanyName = data.CompanyName;
                ViewBag.CorporateID = data.CorporateID;
                ViewBag.UserName = data.UserName;
                ViewBag.PhoneNo = data.PhoneNo;

                //get user roles
                var user_Role = await db.C_Role.Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name
                }).OrderBy(o => o.Name).AsNoTracking().ToListAsync();
                ViewBag.UserRole = new SelectList(user_Role, "Id", "Name", data.UserRole);

                var query = db.Database.SqlQuery<CorporateUserSummaryVM>("SP_Admin_RP_CorporateUserSummary @CompanyName, @CorporateID, @UserName, @PhoneNo",
                    new SqlParameter { ParameterName = "@CompanyName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CompanyName) },
                    new SqlParameter { ParameterName = "@CorporateID", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CorporateID) },
                    new SqlParameter { ParameterName = "@UserName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.UserName) },
                    new SqlParameter { ParameterName = "@PhoneNo", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.PhoneNo) });
                var reportData = await query.ToListAsync();

                ReportDataSource rdsDeliveryOrder = new ReportDataSource("CorporateUserDS", reportData);
                ReportViewer rpv = new ReportViewer()
                {
                    ZoomMode = ZoomMode.Percent,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(40),
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    ShowExportControls = true,
                    ShowFindControls = true,
                    ShowPrintButton = true,
                    ShowToolBar = true,
                    AsyncRendering = true
                };

                //ReportParameter[] parameters = new ReportParameter[1];
                //parameters[0] = new ReportParameter("UserID", GetUserName());

                rpv.LocalReport.ReportPath = @"RDLCReports\RDLC\rptCorporateUserSummary.rdlc";
                rpv.LocalReport.DataSources.Add(rdsDeliveryOrder);
                //rpv.LocalReport.SetParameters(parameters);
                rpv.LocalReport.Refresh();
                ViewBag.CorporateUserReport = rpv;
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// RequestCorporateUserSummary
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RequestCorporateUserSummary()
        {
            //role
            var user_Role = await db.C_Role.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).OrderBy(o => o.Text).AsNoTracking().ToListAsync();
            ViewBag.UserRole = new SelectList(user_Role, "Value", "Text");

            //type
            var typeList = CorporateUserRequest.GetTypeList();
            ViewBag.UserType = new SelectList(typeList, "Value", "Text");

            //status
            var statusList = CorporateUserRequest.GetStatusList();
            ViewBag.Status = new SelectList(statusList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// RequestCorporateUserSummary
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RequestCorporateUserSummary(RequestUserSearchVM data)
        {
            try
            {
                ViewBag.CompanyName = data.CompanyName;
                ViewBag.CorporateID = data.CorporateID;
                ViewBag.UserName = data.UserName;
                ViewBag.FromDate = data.FromDate;
                ViewBag.ToDate = data.ToDate;

                //role
                var user_Role = await db.C_Role.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(o => o.Text).AsNoTracking().ToListAsync();
                ViewBag.UserRole = new SelectList(user_Role, "Value", "Text", data.UserRole);

                //type
                var typeList = CorporateUserRequest.GetTypeList();
                ViewBag.UserType = new SelectList(typeList, "Value", "Text", data.UserType);

                //status
                var statusList = CorporateUserRequest.GetStatusList();
                ViewBag.Status = new SelectList(statusList, "Value", "Text", data.Status);

                var query = db.Database.SqlQuery<RequestCorporateUserSummaryVM>("SP_Admin_RP_RequestCorporateUserSummary @CompanyName, @CorporateID, @UserName, @UserType, @Status, @FromDate, @ToDate",
                    new SqlParameter { ParameterName = "@CompanyName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CompanyName) },
                    new SqlParameter { ParameterName = "@CorporateID", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CorporateID) },
                    new SqlParameter { ParameterName = "@UserName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.UserName) },
                    new SqlParameter { ParameterName = "@UserType", SqlDbType = SqlDbType.Bit, Value = CorporateUserRequest.ToDBNull(data.UserType) },
                    new SqlParameter { ParameterName = "@Status", SqlDbType = SqlDbType.TinyInt, Value = CorporateUserRequest.ToDBNull(data.Status) },
                    new SqlParameter { ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value = CorporateUserRequest.ToDBNull(data.FromDate) },
                    new SqlParameter { ParameterName = "@ToDate", SqlDbType = SqlDbType.Date, Value = CorporateUserRequest.ToDBNull(data.ToDate) });
                var reportData = await query.ToListAsync();

                ReportDataSource rdsDeliveryOrder = new ReportDataSource("CorporateUserDS", reportData);
                ReportViewer rpv = new ReportViewer()
                {
                    ZoomMode = ZoomMode.Percent,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(40),
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    ShowExportControls = true,
                    ShowFindControls = true,
                    ShowPrintButton = true,
                    ShowToolBar = true,
                    AsyncRendering = true
                };

                //ReportParameter[] parameters = new ReportParameter[1];
                //parameters[0] = new ReportParameter("UserID", GetUserName());

                rpv.LocalReport.ReportPath = @"RDLCReports\RDLC\rptRequestCorporateUserSummary.rdlc";
                rpv.LocalReport.DataSources.Add(rdsDeliveryOrder);
                //rpv.LocalReport.SetParameters(parameters);
                rpv.LocalReport.Refresh();
                ViewBag.RequestCorporateUserReport = rpv;
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// RequestCorporateChangesSummary
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestCorporateChangesSummary()
        {
            //type
            var typeList = CorporateUserRequest.GetChangesTypeList();
            ViewBag.UserType = new SelectList(typeList, "Value", "Text");

            //status
            var statusList = CorporateUserRequest.GetStatusList();
            ViewBag.Status = new SelectList(statusList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// RequestCorporateChangesSummary
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RequestCorporateChangesSummary(RequestChangesSearchVM data)
        {
            try
            {
                ViewBag.CompanyName = data.CompanyName;
                ViewBag.CorporateID = data.CorporateID;
                ViewBag.UserName = data.UserName;
                ViewBag.FromDate = data.FromDate;
                ViewBag.ToDate = data.ToDate;

                //type
                var typeList = CorporateUserRequest.GetChangesTypeList();
                ViewBag.UserType = new SelectList(typeList, "Value", "Text", data.UserType);

                //status
                var statusList = CorporateUserRequest.GetStatusList();
                ViewBag.Status = new SelectList(statusList, "Value", "Text", data.Status);

                var query = db.Database.SqlQuery<RequestCorporateChangesSummaryVM>("SP_Admin_RP_RequestCorporateChangesSummary @CompanyName, @CorporateID, @UserName, @UserType, @Status, @FromDate, @ToDate",
                    new SqlParameter { ParameterName = "@CompanyName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CompanyName) },
                    new SqlParameter { ParameterName = "@CorporateID", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.CorporateID) },
                    new SqlParameter { ParameterName = "@UserName", SqlDbType = SqlDbType.VarChar, Value = CorporateUserRequest.ToDBNull(data.UserName) },
                    new SqlParameter { ParameterName = "@UserType", SqlDbType = SqlDbType.TinyInt, Value = CorporateUserRequest.ToDBNull(data.UserType) },
                    new SqlParameter { ParameterName = "@Status", SqlDbType = SqlDbType.TinyInt, Value = CorporateUserRequest.ToDBNull(data.Status) },
                    new SqlParameter { ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value = CorporateUserRequest.ToDBNull(data.FromDate) },
                    new SqlParameter { ParameterName = "@ToDate", SqlDbType = SqlDbType.Date, Value = CorporateUserRequest.ToDBNull(data.ToDate) });
                var reportData = await query.ToListAsync();

                ReportDataSource rdsDeliveryOrder = new ReportDataSource("CorporateUserDS", reportData);
                ReportViewer rpv = new ReportViewer()
                {
                    ZoomMode = ZoomMode.Percent,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(40),
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    ShowExportControls = true,
                    ShowFindControls = true,
                    ShowPrintButton = true,
                    ShowToolBar = true,
                    AsyncRendering = true
                };

                //ReportParameter[] parameters = new ReportParameter[1];
                //parameters[0] = new ReportParameter("UserID", GetUserName());

                rpv.LocalReport.ReportPath = @"RDLCReports\RDLC\rptRequestCorporateChangesSummary.rdlc";
                rpv.LocalReport.DataSources.Add(rdsDeliveryOrder);
                //rpv.LocalReport.SetParameters(parameters);
                rpv.LocalReport.Refresh();
                ViewBag.RequestCorporateChangesReport = rpv;
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}