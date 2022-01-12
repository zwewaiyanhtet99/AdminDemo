using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ABankAdmin
{
    public class AdminUserManager : UserManager<AdminUser>
    {
        public AdminUserManager(IUserStore<AdminUser> store)
            : base(store)
        {
        }

        public AdminDBContext db = new AdminDBContext();
        public static AdminUserManager Create(IdentityFactoryOptions<AdminUserManager> options, IOwinContext context)
        {
            var manager = new AdminUserManager(new UserStore<AdminUser>(context.Get<AdminDBContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<AdminUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Enable Lock outs
            manager.UserLockoutEnabledByDefault = true;
            //get fail count from rule
            var rule = manager.db.Rules.Where(r => r.Code == "R002").FirstOrDefault();
            int IntRule = Convert.ToInt32(rule.Value);
            manager.MaxFailedAccessAttemptsBeforeLockout = (rule == null? 5 : IntRule);
            // for manurally unlock, indefinitely 200 years should be enough
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(365 * 200);

            return manager;
        }
    }

    public class AdminSignInManager : SignInManager<AdminUser, string>
    {
        public AdminSignInManager(AdminUserManager userManager, IAuthenticationManager authenticationManager)
          : base(userManager, authenticationManager)
        {
        }

        public static AdminSignInManager Create(IdentityFactoryOptions<AdminSignInManager> options, IOwinContext context)
        {
            return new AdminSignInManager(context.GetUserManager<AdminUserManager>(), context.Authentication);
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AdminUser user)
        {
            return user.GenerateUserIdentityAsync((AdminUserManager)UserManager);
        }
    }
}