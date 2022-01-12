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
    public partial class Feedback : System.Web.UI.Page
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
            rpFeedbackReport.Reset();
            rpFeedbackReport.LocalReport.Dispose();
            rpFeedbackReport.LocalReport.DataSources.Clear();

            rpFeedbackReport.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpFeedback.rdlc");

            List<CommentViewModel> item = Session["ReportData"] as List<CommentViewModel>;
            //List<RpUser> u = item.ConvertAll(i => new RpUser { UserName = i.USERNAME });
            rpFeedbackReport.LocalReport.DataSources.Add(this.GetFeedback(item));
            rpFeedbackReport.LocalReport.Refresh();
            Session["ReportData"] = null;
        }

        private ReportDataSource GetFeedback(List<CommentViewModel> item)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "FeedbackDataSet";
            reportDataSource.Value = item;

            return reportDataSource;
        }
    }
}