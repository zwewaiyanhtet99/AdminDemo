using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin
{
    [HandleError]
    public partial class Startup
    {
        private AdminDBContext db = new AdminDBContext();
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(AdminDBContext.Create);
            app.CreatePerOwinContext<AdminUserManager>(AdminUserManager.Create);
            app.CreatePerOwinContext<AdminSignInManager>(AdminSignInManager.Create);


            //get session timeout minutes from rule (R024) description => Admin Sessiontimeout
            var expiretimespan_min = 60;
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R024").FirstOrDefault();
                int IntRule = Convert.ToInt32(rule.Value);
                expiretimespan_min = rule == null ? 60 : IntRule;
            }
           catch (Exception ex)
            {
                throw ex;
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Admin/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AdminUserManager, AdminUser>(
                validateInterval: TimeSpan.FromMinutes(0),
                regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(expiretimespan_min)
            });


        }
    }
}