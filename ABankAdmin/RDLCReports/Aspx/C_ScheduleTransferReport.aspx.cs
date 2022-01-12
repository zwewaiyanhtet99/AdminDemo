using ABankAdmin.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class C_ScheduleTransferReport : System.Web.UI.Page
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
            rpC_ScheduleTransfer.Reset();
            rpC_ScheduleTransfer.LocalReport.Dispose();
            rpC_ScheduleTransfer.LocalReport.DataSources.Clear();

            rpC_ScheduleTransfer.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpC_ScheduleTransfer.rdlc");

            List<ScheduleTransferViewModel> item = Session["ReportData"] as List<ScheduleTransferViewModel>;
            rpC_ScheduleTransfer.LocalReport.DataSources.Add(this.GetScheduleTransfer(item));
            rpC_ScheduleTransfer.LocalReport.Refresh();
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