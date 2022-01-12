using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_COMMENT")]
    public class COMMENT
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        [StringLength(20)]
        public string MOBILENO { get; set; }
        [StringLength(100)]
        public string REPORTTYPE { get; set; }
        [Display(Name = "COMMENT")]
        public string comment { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public string ACTIVE { get; set; }
        public bool COUNT { get; set; }
        //[ForeignKey("NAME")]
        //public virtual User VrUser { get; set; }
    }
}