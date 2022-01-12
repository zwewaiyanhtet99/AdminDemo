using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ABankAdmin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private int AdminRoleID = ABankAdmin.Core.Utils.ConstantValues.AdminRoleID;

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var response = httpContext.Response;
            var user = httpContext.User;

            if (request.IsAjaxRequest())
            {
                if (user.Identity.IsAuthenticated == false)
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                else
                    response.StatusCode = (int)HttpStatusCode.Forbidden;

                response.SuppressFormsAuthenticationRedirect = true;
                response.End();
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                string request = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + ":" +
                    filterContext.ActionDescriptor.ActionName;
                if (request != "Admin:Login" && request!="Admin:Menu" )
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Admin", action = "Login" }));
                }
            }
            else
            {
                //get request controller and action
                string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string action = filterContext.ActionDescriptor.ActionName.ToLower();

                //skip for Error controller
                if (controller == "Error")
                    return;

                //for general actions (eg. logoff, change password, menu, accessed denied)
                if (controller == "Admin" && (action == "logoff" || action == "changepassword" || action == "menu"))
                    return;

                //to redirect from login page to home
                if (controller == "Admin" && action == "login")
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Home", action = "Index" }));
                    return;
                }

                //if role doesn't exist at session
                if (filterContext.HttpContext.Session.Contents["forcechange"] == null)
                {
                    //to logout if session is null
                    filterContext.HttpContext.Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //to redirect to login
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Admin", action = "Login" }));
                    return;
                }


                //check Force change
                if ((Boolean)filterContext.HttpContext.Session.Contents["forcechange"])
                {
                    filterContext.Result = new RedirectToRouteResult(new
                          RouteValueDictionary(new { controller = "Admin", action = "ChangePassword" }));
                    return;
                }

                //skip for Home controller
                if (controller == "Home")
                    return;

                //if role doesn't exist at session
                if (filterContext.HttpContext.Session.Contents["role"] == null)
                {
                    //to logout if session is null
                    filterContext.HttpContext.Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //to redirect to login
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Admin", action = "Login" }));
                    return;
                }

                //get role from session
                int role = Convert.ToInt32(filterContext.HttpContext.Session.Contents["role"]);

                //get allowed menus from rolemenus
                if (role == AdminRoleID)//allow if administrator
                {
                    return;
                }
                else if(filterContext.HttpContext.Session.Contents["permissions"] == null)//checking permission session
                {
                    //to logout if session is null
                    filterContext.HttpContext.Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //to redirect to login
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Admin", action = "Login" }));
                    return;
                }
                else//if not administrator, get permissions from session
                {
                    IEnumerable<Menu> permissions = null;
                    permissions = (IEnumerable<Menu>)filterContext.HttpContext.Session.Contents["permissions"];
                    //search request controller and action at 
                    int menucount = permissions.Where(p => p.Controller.ToLower() == controller.ToLower() && p.Action.ToLower() == action).Count();
                    if (menucount > 0)
                        return;
                }

                filterContext.Result = new RedirectToRouteResult(new
                          RouteValueDictionary(new { controller = "Error", action = "Unauthorize" }));
            }
        }
    }
}