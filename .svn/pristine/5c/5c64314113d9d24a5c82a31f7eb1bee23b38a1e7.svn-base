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
    public partial class CorporateReport : System.Web.UI.Page
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
            rpCorporate.Reset();
            rpCorporate.LocalReport.Dispose();
            rpCorporate.LocalReport.DataSources.Clear();

            rpCorporate.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpCorporate.rdlc");

            List<CorporateViewModel> item = Session["ReportData"] as List<CorporateViewModel>;
            rpCorporate.LocalReport.DataSources.Add(this.GetCorporate(item));
            rpCorporate.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetCorporate(List<CorporateViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CorporateDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}