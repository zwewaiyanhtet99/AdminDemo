using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class CIFIDAccountInfoReport : System.Web.UI.Page
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
            CIFIDAccountInfoReportViewer.Reset();
            CIFIDAccountInfoReportViewer.LocalReport.Dispose();
            CIFIDAccountInfoReportViewer.LocalReport.DataSources.Clear();

            CIFIDAccountInfoReportViewer.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpCIFIDAccountInfo.rdlc");
            List<CIFIDInfoRPViewModel> item = Session["CIFIDInfoReportData"] as List<CIFIDInfoRPViewModel>;
            List<IconicDetailAccVM> item2 = Session["CIFAccInfoReportData"] as List<IconicDetailAccVM>;
            CIFIDAccountInfoReportViewer.LocalReport.DataSources.Add(this.GetCIFIDInfo(item));
            CIFIDAccountInfoReportViewer.LocalReport.DataSources.Add(this.GetAccountInfo(item2));
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("TotalMMK", Session["DetailReportTotalAmtData"].ToString());
            CIFIDAccountInfoReportViewer.LocalReport.SetParameters(parameters);
            CIFIDAccountInfoReportViewer.LocalReport.Refresh();
            Session["CIFIDInfoReportData"] = null;
            Session["CIFAccInfoReportData"] = null;
            Session["DetailReportTotalAmtData"] = null;
        }

        private ReportDataSource GetCIFIDInfo(List<CIFIDInfoRPViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CIFID_DataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }

        private ReportDataSource GetAccountInfo(List<IconicDetailAccVM> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "Iconic_DetailAccDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}