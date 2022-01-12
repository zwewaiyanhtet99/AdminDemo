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
    public partial class TransactionReport : System.Web.UI.Page
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

            rpUserRetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpTransaction.rdlc");

            List<TransactionViewModel> item = Session["ReportData"] as List<TransactionViewModel>;
            //List<RpUser> u = item.ConvertAll(i => new RpUser { UserName = i.USERNAME });
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