using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_MenuPermission")]
    public class C_Req_MenuPermission
    {
        public int Id { get; set; }

        public int ReqUserID { get; set; }

        [StringLength(36)]
        public string UserId { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }
    }
}