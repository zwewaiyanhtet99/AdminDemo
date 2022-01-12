using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_BRANCH")]
    public class Branch
    {
        public int id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Code")]
        public string code { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Address")]
        public string address { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Transfer Rate")]
        public string RateCode { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Remittance Rate")]
        public string RemitRateCode { get; set; }
        [StringLength(10)]
        [Display(Name = "Rate Description")]
        public string RateDesc { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:0.00000000}")]
        [RegularExpression(@"[-]?\d{1,2}\.\d{1,8}", ErrorMessage = "Invalid Latitude Format!")]
        public Decimal? LATITUDE { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00000000}")]
        [RegularExpression(@"[-]?\d{1,3}\.\d{1,8}", ErrorMessage = "Invalid Longitude Format!")]
        public Decimal? LONGITUDE { get; set; }
        [Required]
        [StringLength(20)]
        //[ForeignKey("Code")]
        public string CITY { get; set; }
        [Required]
        [StringLength(20)]
        public string TOWNSHIP { get; set; }
        [Required]
        [StringLength(40)]
        [Display(Name = "Phone No")]
        public string PHONE_NO { get; set; }
        public bool DEL_FLAG { get; set; }        
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        //public City _city { get; set; }
    }
}