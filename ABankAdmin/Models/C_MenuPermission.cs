using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    public class C_MenuPermission
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }
}