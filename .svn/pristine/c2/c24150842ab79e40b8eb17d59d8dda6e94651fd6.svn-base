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
    public partial class ScheduleTransferReport : System.Web.UI.Page
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
            rpScheduleTransfer.Reset();
            rpScheduleTransfer.LocalReport.Dispose();
            rpScheduleTransfer.LocalReport.DataSources.Clear();

            rpScheduleTransfer.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpScheduleTransfer.rdlc");

            List<ScheduleTransferViewModel> item = Session["ReportData"] as List<ScheduleTransferViewModel>;
            rpScheduleTransfer.LocalReport.DataSources.Add(this.GetScheduleTransfer(item));
            rpScheduleTransfer.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetScheduleTransfer(List<ScheduleTransferViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ScheduleTransferDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}