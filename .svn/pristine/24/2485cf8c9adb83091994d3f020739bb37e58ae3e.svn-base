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
    public partial class CorporateUserReport : System.Web.UI.Page
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
            rpCorporateUser.Reset();
            rpCorporateUser.LocalReport.Dispose();
            rpCorporateUser.LocalReport.DataSources.Clear();

            rpCorporateUser.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpCorporateUser.rdlc");

            List<CorporateUserViewModel> item = Session["ReportData"] as List<CorporateUserViewModel>;
            rpCorporateUser.LocalReport.DataSources.Add(this.GetCorporateUser(item));
            rpCorporateUser.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetCorporateUser(List<CorporateUserViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CorporateUserDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}