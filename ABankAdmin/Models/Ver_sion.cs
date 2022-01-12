using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_VERSION")]
    public class Ver_sion
    {
        public int ID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name ="Version No.")]
        public Decimal? VersionNo { get; set; }
        [StringLength(200)]
        [Required]
        [Display(Name ="Platform")]
        public string PlatForm { get; set; }
        [StringLength(20)]
        [Required]
        [Display(Name ="User Type")]
        public string UserType { get; set; }
        [StringLength(50)]
        [Required]
        [Display(Name ="Version Name")]
        public string VersionName { get; set; }
        [StringLength(50)]
        [Required]
        [Display(Name ="Update Status")]
        public string UpdatedStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
    }
}