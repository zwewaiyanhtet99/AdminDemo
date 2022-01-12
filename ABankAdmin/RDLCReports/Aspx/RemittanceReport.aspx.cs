﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ABankAdmin.ViewModels;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class RemittanceReport : System.Web.UI.Page
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
            rpRemittance.Reset();
            rpRemittance.LocalReport.Dispose();
            rpRemittance.LocalReport.DataSources.Clear();

            rpRemittance.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpRemittance.rdlc");

            List<RemittanceViewModel> item = Session["ReportData"] as List<RemittanceViewModel>;
            rpRemittance.LocalReport.DataSources.Add(this.GetTransaction(item));
            rpRemittance.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetTransaction(List<RemittanceViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "RemittanceDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}