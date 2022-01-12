using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_BULK_FILE_RECORD")]
    public class C_Bulk_File_Record
    {
        [Key]
        public int ID { get; set; }
        [Display(Name ="Company Name")]
        public int Corporate_ID { get; set; }
        [Display(Name ="Maximum Record")]
        public int Max_Record { get; set; }
        [StringLength(128)]
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdatedUserID { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool DEL_FLAG { get; set; }
        [ForeignKey("Corporate_ID")]
        public virtual C_Corporate Vrcorporate { get; set; }
    }
}