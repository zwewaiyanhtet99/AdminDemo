using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Privilege")]
    public class Privilege
    {
        public int ID { get; set; }
        [StringLength(255)]
        public string PrivilegeName { get; set; }
        [StringLength(255)]
        public string Location { get; set; }
        [StringLength(255)]
        public string Room { get; set; }
        public DateTime Time { get; set; }
        public DateTime Date { get; set; }
        [StringLength(50)]
        public string NoOfFollowers { get; set; }
        [StringLength(50)]
        public string Staff { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDelete { get; set; }
        public bool InUse { get; set; }
    }
}