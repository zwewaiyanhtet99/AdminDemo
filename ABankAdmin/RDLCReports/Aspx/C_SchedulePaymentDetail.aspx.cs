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
    public partial class C_SchedulePaymentDetail : System.Web.UI.Page
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
            rpCorScheduleTransferDetail.Reset();
            rpCorScheduleTransferDetail.LocalReport.Dispose();
            rpCorScheduleTransferDetail.LocalReport.DataSources.Clear();

            rpCorScheduleTransferDetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/C_rpSchedulePaymentDetail.rdlc");

            List<SchedulePaymentDetailVM> item = Session["ReportData"] as List<SchedulePaymentDetailVM>;
            rpCorScheduleTransferDetail.LocalReport.DataSources.Add(this.GetSchedulePayment(item));
            rpCorScheduleTransferDetail.LocalReport.Refresh();
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