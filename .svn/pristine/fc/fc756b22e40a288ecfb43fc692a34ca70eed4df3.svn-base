using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_MENU")]
    public class Menu
    {
        [Key]
        public int ID { get; set; }
        public int Parent_ID { get; set; }
        public string Menu_Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public Boolean Visible { get; set; }
        public Boolean Active { get; set; }
        public int Order { get; set; }
        public virtual ICollection<RoleMenu> rolemenus { get; set; }
    }
}