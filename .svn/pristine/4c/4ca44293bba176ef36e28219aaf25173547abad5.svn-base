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
    public partial class C_TransactionReport : System.Web.UI.Page
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
            rpUserRetail.Reset();
            rpUserRetail.LocalReport.Dispose();
            rpUserRetail.LocalReport.DataSources.Clear();

            rpUserRetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpC_Transaction.rdlc");

            List<TransactionViewModel> item = Session["ReportData"] as List<TransactionViewModel>;
            rpUserRetail.LocalReport.DataSources.Add(this.GetTransaction(item));
            rpUserRetail.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetTransaction(List<TransactionViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "TransactionDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}