using System.Web.Optimization;

namespace ABankAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.bootstrap.min.js"));

            //"~/Scripts/daterangepicker.min.js",
            //            "~/Scripts/moment.min.js"

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/site.css",
                      // custom style
                      "~/Content/newstyle.css",
                      //Theme Style
                      "~/Content/dist/css/AdminLTE.min.css",
                      //skin
                      "~/Content/dist/css/skins/_all-skins.min.css",
                      //datatable
                      //"~/Content/datatables.min.css",
                      "~/Content/dataTables.bootstrap.min.css"
                      //daterangepicker
                      // "~/Content/daterangepicker.css"
                      ));

            //cop user reg
            bundles.Add(new StyleBundle("~/Content/userreg").Include(
                      "~/Content/userregistration.css"));

            //biller reg
            bundles.Add(new StyleBundle("~/Content/billerreg").Include(
                      "~/Content/billerregistration.css"));

            //admin lte
            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                      "~/Scripts/adminlte.min.js"));

            //retail
            bundles.Add(new ScriptBundle("~/bundles/retail").Include(
                      "~/Scripts/mine/retail.js"));
            //CIFInfo
            bundles.Add(new ScriptBundle("~/bundles/CIFInfo").Include(
                      "~/Scripts/mine/CIFInfo.js"));

            //cop user reg
            bundles.Add(new ScriptBundle("~/bundles/userreg").Include(
                      "~/Scripts/mine/userregistration.js"));

            //biller reg
            bundles.Add(new ScriptBundle("~/bundles/billerreg").Include(
                      "~/Scripts/mine/billerregistration.js"));

            //userregistrationlist
            bundles.Add(new ScriptBundle("~/bundles/userreglst").Include(
                      "~/Scripts/mine/userregistrationlist.js"));

            //billerlist
            bundles.Add(new ScriptBundle("~/bundles/billerlst").Include(
                      "~/Scripts/mine/billerregistrationlist.js"));

            //requestuserlist
            bundles.Add(new ScriptBundle("~/bundles/requserlst").Include(
                      "~/Scripts/mine/requestuserlist.js"));

            //requestchangelist
            bundles.Add(new ScriptBundle("~/bundles/reqchangelst").Include(
                      "~/Scripts/mine/requestchangelist.js"));

            //admin
            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
                      "~/Scripts/mine/admin.js"));

            //request
            bundles.Add(new ScriptBundle("~/bundles/request").Include(
                      "~/Scripts/mine/request.js"));

            //approve
            bundles.Add(new ScriptBundle("~/bundles/approve").Include(
                      "~/Scripts/mine/approve.js"));

            //corporate
            bundles.Add(new ScriptBundle("~/bundles/corporate").Include(
                      "~/Scripts/mine/corporate.js"));

            //Corporate Request
            bundles.Add(new ScriptBundle("~/bundles/reqCorporate").Include(
                      "~/Scripts/mine/reqCorporate.js"));
            //approve
            bundles.Add(new ScriptBundle("~/bundles/approveCorporate").Include(
                      "~/Scripts/mine/approveCorporate.js"));

            //approve
            bundles.Add(new ScriptBundle("~/bundles/cuserapprove").Include(
                      "~/Scripts/mine/CUserApprove.js"));

            //date range for report
            bundles.Add(new ScriptBundle("~/bundles/daterange").Include(
                      "~/Scripts/mine/reportdaterangevalidation.js"));

            //other bank tran log
            bundles.Add(new ScriptBundle("~/bundles/otherbanktranlog").Include(
                      "~/Scripts/mine/otherbanktranlog.js"));

            //corporate request approve rule
            bundles.Add(new ScriptBundle("~/bundles/reqApproveRule").Include(
                      "~/Scripts/mine/c_req_approverule.js"));

            //Corporate approve ApproveRule
            bundles.Add(new ScriptBundle("~/bundles/capproverule").Include(
                      "~/Scripts/mine/CApproveRule.js"));
            //IconicBooking
            bundles.Add(new ScriptBundle("~/bundles/iconicbooking").Include(
                      "~/Scripts/mine/iconicbooking.js"));
            //IconicBlackList
            bundles.Add(new ScriptBundle("~/bundles/iconicblacklist").Include(
                      "~/Scripts/mine/iconicblacklist.js"));
            //RM_Info
            bundles.Add(new ScriptBundle("~/bundles/RM_Info").Include(
                      "~/Scripts/mine/RM_Info.js"));
            //CIFID_InfoForDeactivate
            bundles.Add(new ScriptBundle("~/bundles/CIFInfoForDeactivate").Include(
                      "~/Scripts/mine/CIFInfoForDeactivate.js"));
            //CIFID_InfoForUpgrade
            bundles.Add(new ScriptBundle("~/bundles/CIFInfoForUpgrade").Include(
                      "~/Scripts/mine/CIFInfoForUpgrade.js"));
            //Privilege
            bundles.Add(new ScriptBundle("~/bundles/Privilege").Include(
                      "~/Scripts/mine/Privilege.js"));

            //CIFInfoForDowngrade
            bundles.Add(new ScriptBundle("~/bundles/CIFInfoForDowngrade").Include(
                      "~/Scripts/mine/CIFInfoForDowngrade.js"));
            BundleTable.EnableOptimizations = true;
        }
    }
}
