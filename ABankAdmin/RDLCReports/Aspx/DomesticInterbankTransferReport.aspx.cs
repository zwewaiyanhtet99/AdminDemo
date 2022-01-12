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
    public partial class DomesticInterbankTransferReport : System.Web.UI.Page
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
            rpDomInterTransfer.Reset();
            rpDomInterTransfer.LocalReport.Dispose();
            rpDomInterTransfer.LocalReport.DataSources.Clear();

            rpDomInterTransfer.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpDomesticInterbankTransfer.rdlc");

            List<DomesticInterbankTransferViewModel> item = Session["ReportData"] as List<DomesticInterbankTransferViewModel>;
            rpDomInterTransfer.LocalReport.DataSources.Add(this.GetDomInterTransfer(item));
            rpDomInterTransfer.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetDomInterTransfer(List<DomesticInterbankTransferViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DomesticInterbankTransferDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}