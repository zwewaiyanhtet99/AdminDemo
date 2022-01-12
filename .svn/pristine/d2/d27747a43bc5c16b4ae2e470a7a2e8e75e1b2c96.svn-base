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
    public partial class TransactionSummaryReport : System.Web.UI.Page
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
            rpTransactionSummary.Reset();
            rpTransactionSummary.LocalReport.Dispose();
            rpTransactionSummary.LocalReport.DataSources.Clear();
            rpTransactionSummary.ProcessingMode = ProcessingMode.Local;
            rpTransactionSummary.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpTransactionSummary.rdlc");

            List<TransactionSummaryViewModel> item = Session["ReportData"] as List<TransactionSummaryViewModel>;
            //List<RpUser> u = item.ConvertAll(i => new RpUser { UserName = i.USERNAME }); 
            var branch_name = Session["BranchName"];
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("branch_name", branch_name.ToString());
            this.rpTransactionSummary.LocalReport.SetParameters(parameters);
            rpTransactionSummary.LocalReport.DataSources.Add(this.GetTransactionSummary(item));
            rpTransactionSummary.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetTransactionSummary(List<TransactionSummaryViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "TransactionSummaryDataset";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}