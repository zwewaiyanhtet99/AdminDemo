using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_OTHER_BANK")]
    public class OtherBank
    {
        [Key]
        [Display(Name = "Bank Name")]
        public int OTHER_BANK_ID { get; set; }
        [StringLength(200)]
        [Display(Name = "Bank Name")]
        [Required]
        public string BANK_NAME { get; set; }
        [StringLength(10)]
        [Display(Name = "Short Name")]
        [Required]
        public string SHORT_NAME { get; set; }
        [StringLength(20)]
        [Display(Name = "Bank Code")]
        [Required]
        public string BANK_CODE { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public Boolean Active { get; set; }
        public Boolean IsACH { get; set; }
    }
}