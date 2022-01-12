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
    public partial class SchedulePaymentDetailReport : System.Web.UI.Page
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
            rpScheduleTransferDetail.Reset();
            rpScheduleTransferDetail.LocalReport.Dispose();
            rpScheduleTransferDetail.LocalReport.DataSources.Clear();

            rpScheduleTransferDetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpScheduleTransferDetail.rdlc");

            List<SchedulePaymentDetailVM> item = Session["ReportData"] as List<SchedulePaymentDetailVM>;
            rpScheduleTransferDetail.LocalReport.DataSources.Add(this.GetSchedulePayment(item));
            rpScheduleTransferDetail.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetSchedulePayment(List<SchedulePaymentDetailVM> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ScheduleTransferDetailDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}