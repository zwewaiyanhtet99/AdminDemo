using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_CORPORATE")]
    public class C_Corporate
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public C_Corporate()
        //{
        //    C_Bulk_File_Records = new HashSet<C_Bulk_File_Record>();
        //}

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string CIFID { get; set; }

        [Required]
        public int BRANCH { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
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
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Company Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
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
        [Display(Name = "REMMITANCE CHARGES CODE")]
        [RequiredIfTrue("ISVIP")]
        [StringLength(20)]
        public string R_CHARGES_CODE { get; set; }
        [Display(Name = "BULK CHARGES FIXED RATE")]
        //[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public Decimal BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }
        [Required]
        [StringLength(128)]
        public string CreatedUserID { get; set; }
        [Display(Name = "CREATED DATE")]
        public DateTime? CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool DEL_FLAG { get; set; }
        public Boolean IS_FIXRATE_BULK_CHARGES { get; set; }
        [ForeignKey("COUNTRY_ID")]
        public virtual C_Country VrCountry { get; set; }
        [ForeignKey("STATE_ID")]
        public virtual C_State VrState { get; set; }
        [ForeignKey("BRANCH")]
        public virtual Branch VrBranch { get; set; }

        public bool CCT_IS_FIXRATE_BULK_CHARGES { get; set; }
        [Required]
        [Display(Name = "OTHER BANK BULK CHARGES FIXED RATE")]
        public Decimal CCT_BULK_CHARGES_FIX_RATE_N_PERCENT { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<C_Bulk_File_Record> C_Bulk_File_Records { get; set; }
    }
}