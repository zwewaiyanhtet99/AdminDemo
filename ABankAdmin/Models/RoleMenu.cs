using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ROLEMENU")]
    public class RoleMenu
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int MenuID { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role role { get; set; }
        [ForeignKey("MenuID")]
        public virtual Menu menu { get; set; }
    }
}