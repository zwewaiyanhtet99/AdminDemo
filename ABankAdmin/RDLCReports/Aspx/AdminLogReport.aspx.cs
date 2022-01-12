using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ABankAdmin.ViewModels;
using Microsoft.Reporting.WebForms;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class AdminLogReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            rpAdminLog.Reset();
            rpAdminLog.LocalReport.Dispose();
            rpAdminLog.LocalReport.DataSources.Clear();

            rpAdminLog.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpAdminLog.rdlc");

            List<AdminLogViewModel> item = Session["ReportData"] as List<AdminLogViewModel>;
            rpAdminLog.LocalReport.DataSources.Add(this.GetAdminLog(item));
            rpAdminLog.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetAdminLog(List<AdminLogViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "AdminLogDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}