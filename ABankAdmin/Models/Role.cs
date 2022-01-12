using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ROLE")]
    public class Role
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        //public virtual ICollection<RoleMenu> rolemenus { get; set; }
    }
}