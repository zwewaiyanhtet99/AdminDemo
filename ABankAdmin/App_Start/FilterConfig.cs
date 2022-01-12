using ABankAdmin.Filters;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new CustomAuthorizeAttribute());
        }
    }
}
