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
    public partial class TopUpReport : System.Web.UI.Page
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
            rpUtility.Reset();
            rpUtility.LocalReport.Dispose();
            rpUtility.LocalReport.DataSources.Clear();

            rpUtility.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpTopUp.rdlc");
            var br_code = Session["BranchCode"];
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("BranchCode", br_code.ToString());
            this.rpUtility.LocalReport.SetParameters(parameters);
            List<TopUpViewModel> item = Session["ReportData"] as List<TopUpViewModel>;
            rpUtility.LocalReport.DataSources.Add(this.GetTopUp(item));
            rpUtility.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetTopUp(List<TopUpViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "TopUpDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}