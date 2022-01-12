using ABankAdmin.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ABankAdmin.ViewModels;

namespace ABankAdmin.RDLCReports.Aspx
{
    public partial class UserReport : System.Web.UI.Page
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
            rpUserRetail.Reset();
            rpUserRetail.LocalReport.Dispose();
            rpUserRetail.LocalReport.DataSources.Clear();
           // rpUserRetail.ProcessingMode = ProcessingMode.Local;
            rpUserRetail.LocalReport.ReportPath = Server.MapPath("~/RDLCReports/RDLC/rpUser.rdlc");

            List<UserRPViewModel> item = Session["ReportData"] as List<UserRPViewModel>;
            //List<RpUser> u = item.ConvertAll(i => new RpUser { UserName = i.USERNAME }); 
            //var BranchCode = Session["BranchCode"];
            //ReportParameter[] parameters = new ReportParameter[1];
            //parameters[0] = new ReportParameter("BranchCode", BranchCode.ToString());
            //this.rpUserRetail.LocalReport.SetParameters(parameters);
            rpUserRetail.LocalReport.DataSources.Add(this.GetUser(item));
            rpUserRetail.LocalReport.Refresh();
            Session["ReportData"] = null;
        }
        private ReportDataSource GetUser(List<UserRPViewModel> ds)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "UserDataSet";
            reportDataSource.Value = ds;

            return reportDataSource;
        }
    }
}
       