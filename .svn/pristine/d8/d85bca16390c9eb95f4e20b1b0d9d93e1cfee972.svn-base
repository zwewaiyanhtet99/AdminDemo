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
    public class CorporateVM
    {
        public int ID { get; set; }
        [Required]
        public string CIFID { get; set; }
        [Required]
        [Display(Name = "BRANCH")]
        public int Branch { get; set; }
        [Display(Name = "CORPORATE ID")]
        public string CorporateID { get; set; }
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
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "COMPANY PHONE")]
        public string Company_Phone { get; set; }
        [Display(Name = "COUNTRY")]
        public int? COUNTRY_ID { get; set; }
        [Display(Name = "STATE")]
        public int? STATE_ID { get; set; }
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
        [Display(Name = "BULK CHARGES FIXED RATE")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field BULK CHARGES FIXED RATE must be between 0 and 9,999,999,999,999,999.99")]
        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public string Bulk_Charges_Fix_Rate { get; set; }
        public int REQ_CORPORATEID { get; set; }
        public bool DEL_FLAG { get; set; }
        public Boolean IS_FIXRATE_BULK_CHARGES { get; set; }

        //For Other Bank Chearges
        public bool CCT_IS_FIXRATE_BULK_CHARGES { get; set; }
        [Required]
        [Display(Name = "OTHER BANK")]

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public string CCT_BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }

        public string CreatedUserID { get; set; }
        [Display(Name = "CREATED DATE")]
        public DateTime? CreatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        [ForeignKey("COUNTRY_ID")]
        public virtual C_Country VrCountry { get; set; }
        [ForeignKey("STATE_ID")]
        public virtual C_State VrState { get; set; }
        [ForeignKey("Branch")]
        public virtual Branch VrBranch { get; set; }
    }
}