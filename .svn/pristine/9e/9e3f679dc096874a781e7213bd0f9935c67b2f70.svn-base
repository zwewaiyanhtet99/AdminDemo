using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_OFFICE_ACCOUNT")]
    public class Office_Account
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(20)]
        [Required]
        public string Code { get; set; }
        [Display(Name = "Account No")]
        [StringLength(20)]
        [Required]
        public string AcctNo { get; set; }
        [StringLength(200)]
        [Required]
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public Boolean? DEL_FLAG { get; set; }

    }
}