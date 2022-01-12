using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_UTILITIES")]
    public class Utilities
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [Display(Name="Image")]
        public string ImagePath { get; set; }
        [StringLength(1)]
        public string Active { get; set; }
        [StringLength(50)]
        [Required]
        [Display(Name="Biller Code")]
        public string Biller_Code { get; set; }
        [StringLength(200)]
        [Required]
        public string Remark { get; set; }
        //[StringLength(50)]
        //[Required]
        //[Display(Name="Discount Name")]
        //public string DiscountCode { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name="Utility Type")]
        public string Utility_Type { get; set; }
        [Display(Name = "Discount Percent")]
        public decimal DiscountPercent { get; set; }
    }
}
