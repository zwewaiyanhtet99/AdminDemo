using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_UserInRole")]
    public class C_Req_UserInRole
    {
        public int Id { get; set; }

        public int ReqUserID { get; set; }

        public int RoleId { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("RoleId")]
        public virtual C_Role VrRole { get; set; }
    }
}