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
    public partial class C_TopUpReport : System.Web.UI.Page
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
            rpC_TopUp.Reset();
            rpC_TopUp.LocalReport.Dispose();
            rpC_TopUp.LocalReport.DataSources.Clear();

            rpC_TopUp.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpC_TopUp.rdlc");
            var br_code = Session["BranchCode"];
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("BranchCode", br_code.ToString());
            this.rpC_TopUp.LocalReport.SetParameters(parameters);
            List<TopUpViewModel> item = Session["ReportData"] as List<TopUpViewModel>;
            rpC_TopUp.LocalReport.DataSources.Add(this.GetTopUp(item));
            rpC_TopUp.LocalReport.Refresh();
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