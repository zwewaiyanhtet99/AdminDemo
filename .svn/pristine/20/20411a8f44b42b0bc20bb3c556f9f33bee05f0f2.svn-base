using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_ACC_SERVICES_DESC")]

    public class ACC_Service_Desc
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        [Display(Name =" Account Service Name")]
        public int Account_Services_ID { get; set; }
        [Required]
        [Display(Name ="Description")]
        public string Desc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public Boolean Del_Flag { get; set; }
        [Required]
        [Display(Name ="Order No.")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public int? OrderNo { get; set; }

        [ForeignKey("Account_Services_ID")]
        public virtual ACC_Service acc_service { get; set; }

    }
}