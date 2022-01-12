using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_OTHER_BRANCH")]
    public class OtherBranch
    {
        [Key]
        public int OTHER_BRANCH_ID { get; set; }
        [Display(Name = "BANK NAME")]
        public int OTHER_BANK_ID { get; set; }
        [StringLength(6)]
        [Display(Name = "BRANCH CODE")]
        [Required]
        public string BR_CODE { get; set; }
        [StringLength(10)]
        [Display(Name = "BRANCH SHORT NAME")]
        [Required]
        public string BR_SHORT_NAME { get; set; }
        [StringLength(30)]
        [Display(Name = "BRANCH NAME")]
        [Required]
        public string BR_NAME { get; set; }
        [StringLength(30)]
        [Display(Name = "FI CODE")]
        [Required]
        public string FI_CODE { get; set; }
        [StringLength(10)]
        [Display(Name = "CR BR CODE")]
        [Required]
        public string CR_BR_CODE { get; set; }
        [StringLength(25)]
        [Display(Name = "CHARGES CODE")]
        [Required]
        public string CHARGES_CODE { get; set; }
        [StringLength(5)]
        [Display(Name = "BR CITY CODE")]
        [Required]
        public string BR_CITY_CODE { get; set; }
        [StringLength(10)]
        [Display(Name = "BR STATE CODE")]
        [Required]
        public string BR_STATE_CODE { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public Boolean Active { get; set; }
        [Display(Name ="ADDRESS")]
        public string Address { get; set; }
        [ForeignKey("OTHER_BANK_ID")]
        public virtual OtherBank OtherBank { get; set; }
    }
}