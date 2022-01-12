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
    public partial class ApiLogReport : System.Web.UI.Page
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
            rpApiLog.Reset();
            rpApiLog.LocalReport.Dispose();
            rpApiLog.LocalReport.DataSources.Clear();

            rpApiLog.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpApiLog.rdlc");

            List<ApiLogViewModel> item = Session["ReportData"] as List<ApiLogViewModel>;
            rpApiLog.LocalReport.DataSources.Add(this.GetApiLog(item));
            rpApiLog.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetApiLog(List<ApiLogViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ApiLogDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}