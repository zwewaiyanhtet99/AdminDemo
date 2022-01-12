using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.Models
{
    [Table("TBL_IB_MENU")]
    public class IB_Menu
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string Menu_Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserID { get; set; }
        public bool Active { get; set; }
    }
}