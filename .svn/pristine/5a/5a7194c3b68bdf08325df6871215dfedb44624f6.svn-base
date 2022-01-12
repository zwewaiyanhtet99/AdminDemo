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
    public partial class UtilitySummaryReport : System.Web.UI.Page
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
            rpUtilitySummary.Reset();
            rpUtilitySummary.LocalReport.Dispose();
            rpUtilitySummary.LocalReport.DataSources.Clear();

            rpUtilitySummary.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpUtilitySummary.rdlc");

            List<UtilitySummaryViewModel> item = Session["ReportData"] as List<UtilitySummaryViewModel>;
            rpUtilitySummary.LocalReport.DataSources.Add(this.GetUtilitySummary(item));
            rpUtilitySummary.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetUtilitySummary(List<UtilitySummaryViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "UtilitySummaryDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}