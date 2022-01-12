using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ABankAdmin.Models;
using Foolproof;

namespace ABankAdmin.ViewModels
{
    public class ReqCorporateVM
    {
        public int ID { get; set; }
        [Required]
        public string CIFID { get; set; }
        [Required]
        [Display(Name = "BRANCH")]
        public int Branch { get; set; }
        [Display(Name = "CORPORATEID")]
        public string Corporate_ID { get; set; }
        [Required]
        [Display(Name = "COMPANY NAME")]
        public string Company_Name { get; set; }
        private string _Email;
        [EmailAddress]
        [Display(Name = "COMPANY EMAIL")]
        public string Company_Email { get { return _Email; } set { _Email = string.IsNullOrWhiteSpace(value) ? null : value; } }
        [Display(Name = "COMPANY ADDRESS")]
        [StringLength(200, MinimumLength = 3)]
        public string Company_Address { get; set; }
        [Required]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = " Company Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        //[RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "COMPANY PHONE")]
        public string Company_Phone { get; set; }
        [Display(Name = "COUNTRY")]
        public int? Country { get; set; }
        [Display(Name = "STATE")]
        public int? State { get; set; }
        [Display(Name = "VIP")]
        public Boolean ISVIP { get; set; }
        [Display(Name = "TRANSFER CHARGES CODE")]
        [RequiredIfTrue("ISVIP")]
        [StringLength(20)]
        public string T_CHARGES_CODE { get; set; }
        [Display(Name = "REMITTANCE CHARGES CODE")]
        [RequiredIfTrue("ISVIP")]
        [StringLength(20)]
        public string R_CHARGES_CODE { get; set; }
        [Required]
        [Display(Name = "TRAN LIMIT")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field TRAN LIMIT must be between 0 and 9,999,999,999,999,999.99")]

        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public string Tran_Limit { get; set; }
        [Required]
        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name = "BULK CHARGES FIXED RATE")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field BULK CHARGES FIXED RATE must be between 0 and 9,999,999,999,999,999.99")]

        public string Bulk_Charges_Fix_Rate { get; set; }
        public int REQ_CORPORATEID { get; set; }
        [Display(Name = "STATUS")]
        public byte STATUS { get; set; }
        [Display(Name = "TYPE")]
        public byte ISNEW { get; set; }
        public string MAKER { get; set; }
        [Display(Name = "REQUESTED DATE")]
        public DateTime RequestedDate { get; set; }
        public string CHECKER { get; set; }
        [Display(Name = "CHECKED DATE")]
        public DateTime? CheckedDate { get; set; }
        [Display(Name = "CHECKED REASON")]
        public string CheckedReason { get; set; }
        [Display(Name = "CREATED DATE")]
        public DateTime? CreatedDate { get; set; }
        public Boolean IS_FIXRATE_BULK_CHARGES { get; set; }
        public bool CCT_IS_FIXRATE_BULK_CHARGES { get; set; }
        [Required]
        [Display(Name = "OTHER BANK")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field BULK CHARGES FIXED RATE must be between 0 and 9,999,999,999,999,999.99")]
        public string CCT_BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }

        [ForeignKey("Country")]
        public virtual C_Country VrCountry { get; set; }
        [ForeignKey("State")]
        public virtual C_State VrState { get; set; }
        [ForeignKey("Branch")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("MAKER")]
        public virtual AdminUser VrMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrChecker { get; set; }
    }
}