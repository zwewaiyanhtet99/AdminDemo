using Foolproof;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_REQ_CORPORATE")]
    public class C_Req_Corporate
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string CIFID { get; set; }
        [Required]
        [Display(Name = "BRANCH")]
        public int BRANCH { get; set; }
        [StringLength(20)]
        [Display(Name = "CORPORATEID")]
        public string CORPORATEID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "COMPANY NAME")]
        public string COMPANY_NAME { get; set; }
        [StringLength(200)]
        private string _Email;
        [EmailAddress]
        [Display(Name = "COMPANY EMAIL")]
        public string COMPANY_EMAIL { get { return _Email; } set { _Email = string.IsNullOrWhiteSpace(value) ? null : value; } }
        [Display(Name = "COMPANY ADDRESS")]
        [StringLength(200, MinimumLength = 3)]
        public string COMPANY_ADDRESS { get; set; }
        [Required]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Minimum length is 9 and Maximum length is 11.")]
        //[RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "COMPANY PHONE")]
        [StringLength(100)]
        public string COMPANY_PHONE { get; set; }
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
        [Display(Name = "TRAN LIMIT")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public decimal TRAN_LIMIT { get; set; }
        [Display(Name = "BULK CHARGES FIXED RATE")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public Decimal BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }
        public int REQ_CORPORATEID { get; set; }
        public byte STATUS { get; set; }
        [Display(Name = "TYPE")]
        public byte ISNEW { get; set; }
        [Required]
        [StringLength(128)]
        public string MAKER { get; set; }
        [Display(Name = "REQUESTED DATE")]
        public DateTime RequestedDate { get; set; }
        [StringLength(128)]
        public string CHECKER { get; set; }
        [Display(Name = "CHECKED DATE")]
        public DateTime? CheckedDate { get; set; }
        [StringLength(500)]
        [Display(Name = "CHECKED REASON")]
        public string CheckedReason { get; set; }
        public Boolean IS_FIXRATE_BULK_CHARGES { get; set; }

        public bool CCT_IS_FIXRATE_BULK_CHARGES { get; set; }
        [Required]
        [Display(Name = "OTHER BANK BULK CHARGES FIXED RATE")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Decimal CCT_BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }
        [ForeignKey("COUNTRY_ID")]
        public virtual C_Country VrCountry { get; set; }
        [ForeignKey("STATE_ID")]
        public virtual C_State VrState { get; set; }
        [ForeignKey("BRANCH")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("MAKER")]
        public virtual AdminUser VrMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrChecker { get; set; }
        
    }
}