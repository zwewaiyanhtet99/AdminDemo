using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ABankAdmin.ViewModels;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class BulkPaymentDetail : System.Web.UI.Page
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
            rpBulkPaymentDetail.Reset();
            rpBulkPaymentDetail.LocalReport.Dispose();
            rpBulkPaymentDetail.LocalReport.DataSources.Clear();

            rpBulkPaymentDetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpBulkPaymentDetail.rdlc");

            List<BulkPaymentDetailViewModel> item = Session["ReportData"] as List<BulkPaymentDetailViewModel>;
            rpBulkPaymentDetail.LocalReport.DataSources.Add(this.GetBulkPaymentDetail(item));
            rpBulkPaymentDetail.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetBulkPaymentDetail(List<BulkPaymentDetailViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "BulkPaymentDetailDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}