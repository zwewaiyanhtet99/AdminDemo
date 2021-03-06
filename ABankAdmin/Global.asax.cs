using ABankAdmin.Controllers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ABankAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CIFInfoModelsController.RunCronService();//for report 1st monday on every week
            CIFInfoModelsController.RunCronServiceForWeekday();//for report Monday to friday on every week
            CIFInfoModelsController.RunCronServiceForMonthly();//for report Monday on every month

        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            if (context.Request.IsAjaxRequest() && context.Response.StatusCode == 302)
            {
                Context.Response.Clear();
                Context.Response.Write("Please login again!");
                Context.Response.StatusCode = 401;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            //Exception ex = app.Server.GetLastError();
            Exception ex = Server.GetLastError();
            //if(ex as SqlException == null)
            //{
            //    app.Response.Redirect("~/Views/Error/Index.cshtml",true);
            //}
            Response.Clear();
            HttpException httpex = ex as HttpException;
            //RouteData route = new RouteData();
            //route.Values.Add("controller", "Error");
            if (httpex != null)
            {
                string action = "";
                switch (httpex.GetHttpCode())
                {
                    case 400:
                        action = "Index";
                        //route.Values.Add("action", "Index");
                        break;
                    case 403:
                        action = "Unauthorization";
                        //route.Values.Add("action", "Unauthorization");
                        break;
                    case 404:
                        action = "PageNotFound";
                        //route.Values.Add("action", "PageNotFound");
                        break;
                    default:
                        action = "General";
                        //route.Values.Add("action", "General");
                        break;
                }
                Server.ClearError();
                Response.TrySkipIisCustomErrors = true;
                Response.Redirect(String.Format("~/Error/{0}", action));
            }
            //IController errorController = new ErrorController();
            //errorController.Execute(new HttpRequest(new HttpContextWrapper(Context), route));
        }
    }
}
