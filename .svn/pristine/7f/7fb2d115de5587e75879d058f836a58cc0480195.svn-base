using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ABankAdmin.Models
{
    public class AdminUser : IdentityUser

    {
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        public int BranchID { get; set; }
        public int Role { get; set; }
        [StringLength(50)]
        public string StaffID { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        public Boolean DEL_FLAG { get; set; }
        public Boolean ForceChange_Flag { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }

        [ForeignKey("Role")]
        public virtual Role VrRole { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AdminUser> manager)

        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType

            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here

            return userIdentity;

        }

    }
}