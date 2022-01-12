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
    public partial class C_UtilityReport : System.Web.UI.Page
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

            rpUtility.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpC_Utility.rdlc");
            var br_code = Session["BranchCode"];
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("BranchCode", br_code.ToString());
            this.rpUtility.LocalReport.SetParameters(parameters);
            List<UtilityViewModel> item = Session["ReportData"] as List<UtilityViewModel>;
            rpUtility.LocalReport.DataSources.Add(this.GetUtility(item));
            rpUtility.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetUtility(List<UtilityViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "UtilityDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}