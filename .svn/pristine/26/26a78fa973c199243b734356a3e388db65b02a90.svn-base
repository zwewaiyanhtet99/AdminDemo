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
    public partial class ScheduleTransferLogReport : System.Web.UI.Page
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
            rpSchTransferLog.Reset();
            rpSchTransferLog.LocalReport.Dispose();
            rpSchTransferLog.LocalReport.DataSources.Clear();

            rpSchTransferLog.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpScheduleTransferLog.rdlc");

            List<ScheduleTransferLogViewModel> item = Session["ReportData"] as List<ScheduleTransferLogViewModel>;
            rpSchTransferLog.LocalReport.DataSources.Add(this.GetSchTransfLog(item));
            rpSchTransferLog.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetSchTransfLog(List<ScheduleTransferLogViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ScheduleTransferLogDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}