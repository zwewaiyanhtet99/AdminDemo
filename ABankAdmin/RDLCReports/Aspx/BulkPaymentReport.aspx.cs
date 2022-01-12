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
    public partial class BulkPaymentReport : System.Web.UI.Page
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
            rpBulkPayment.Reset();
            rpBulkPayment.LocalReport.Dispose();
            rpBulkPayment.LocalReport.DataSources.Clear();

            rpBulkPayment.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpBulkPayment.rdlc");

            List<BulkPaymentViewModel> item = Session["ReportData"] as List<BulkPaymentViewModel>;
            rpBulkPayment.LocalReport.DataSources.Add(this.GetBulkPayment(item));
            rpBulkPayment.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetBulkPayment(List<BulkPaymentViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "BulkPaymentDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}