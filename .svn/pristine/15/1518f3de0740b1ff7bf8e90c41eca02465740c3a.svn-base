using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class ReqUserVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Invalid Mobile No!")]
        public string MOBILENO { get; set; }
        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }
        [StringLength(200)]
        public string FULLNAME { get; set; }
        [Required]
        [StringLength(50)]
        public string NRC { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required]
        public string ADDRESS { get; set; }
        [Required]
        [StringLength(20)]
        public string CIFID { get; set; }
        [StringLength(20)]
        public string MINOR { get; set; }
        [StringLength(20)]
        public string GENDER { get; set; }
        [Display(Name = "Allowed Transaction Count")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public int? ALLOW_TRAN_COUNT { get; set; }
        [Display(Name = "Maximum Amount")]
        [DisplayFormat(DataFormatString ="{0:N2}")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public decimal? DAILY_MAX_AMT { get; set; }
        [Display(Name ="Type")]
        public Boolean ISNEW { get; set; }
        public byte STATUS { get; set; }
        public string MAKER { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime REQUESTEDDATE { get; set; }
        public string REQUESTINFO { get; set; }
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string CHECKERREASON { get; set; }
        public string BRANCH { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name = "VIP User")]
        public Boolean IsVIP { get; set; }
        [StringLength(20)]
        [Display(Name = "COMPANY REGISTRATION DATE")]
        public string USER_TYPE { get; set; }
    }
}