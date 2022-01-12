using ABankAdmin.ViewModels;
using System;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ABankAdmin.Models;
using System.Globalization;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class CFIDInfoReport : System.Web.UI.Page
    {
        public AdminDBContext db = new AdminDBContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }

        }

        private void BindData()
        {
            CFIDInfoReportViewer.Reset();
            CFIDInfoReportViewer.LocalReport.Dispose();
            CFIDInfoReportViewer.LocalReport.DataSources.Clear();

            CFIDInfoReportViewer.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpCIFIDInfo.rdlc");
            List<CIFIDInfoRPViewModel> item = Session["CIFIDInfoReportData"] as List<CIFIDInfoRPViewModel>;
            CFIDInfoReportViewer.LocalReport.DataSources.Add(this.GetCIFIDInfo(item));
            CFIDInfoReportViewer.LocalReport.Refresh();
            Session["CIFIDInfoReportData"] = null;
        }

        private ReportDataSource GetCIFIDInfo(List<CIFIDInfoRPViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CIFID_DataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}