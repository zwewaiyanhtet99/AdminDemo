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
    public partial class CronLogReport : System.Web.UI.Page
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
            rpCronLog.Reset();
            rpCronLog.LocalReport.Dispose();
            rpCronLog.LocalReport.DataSources.Clear();

            rpCronLog.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpCronLog.rdlc");

            List<CronLogViewModel> item = Session["ReportData"] as List<CronLogViewModel>;
            rpCronLog.LocalReport.DataSources.Add(this.GetCronLog(item));
            rpCronLog.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetCronLog(List<CronLogViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CronLogDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}