using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_OTHER_BANK_RECON")]

    public class Other_Bank_Recon
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public int OTH_BK_TRAN_LOG_ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Reconciliation Number")]
        public string RECON_NUMBER { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Fund Debtor FI Branch Code")]
        public string DR_BR_CODE { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Fund Creditor FI Branch Code")]
        public string CR_BR_CODE { get; set; } 
        [Required]
        [StringLength(50)]
        [Display(Name = "Total Fund Settlement Amount(MMK)")]
        public string TOTAL_SETTLEMENT_AMT {get; set;}
        [Required]
        [StringLength(50)]
        [Display(Name = "Transfer Amount(MMK)")]      

        public string TRAN_AMT { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Interbank Charge Amount (MMK) to Debtor FI")]
        public string DR_INTERBANK_CHARGE_AMT { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Interbank Charge Amount (MMK) to Creditor FI")]
        public string CR_INTERBANK_CHARGE_AMT { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Event (Fund) Code")]
        public string EVENT_CODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Event (Fund) Name")]
        public string EVENT_NAME { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Debtor Agent FI Branch Code")]
        public string DR_FI_BRANCH_CODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Debtor Agent FI Branch Name")]
        public string DR_FI_BRANCH_NAME { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "Debtor Identification Category Code")]
        public string DR_ID_CATEGORY { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Debtor identification category name")]
        public string DR_ACCOUNT { get; set; }
        [Required]
        [StringLength(40)]
        [Display(Name = "Debtor Name")] 
        public string DR_NAME { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Debtor Postal Address")]      
        public string DR_POSTAL_ADDRESS { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Debtor Phone Number")]
        public string DR_PHONE { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Other Information")]
        public string OTHER_INFORMATION { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Creditor Agent FI Branch Number")]
        public string CR_FI_BRANCH_CODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Creditor Agent FI Branch Name")]
        public string CR_FI_BRANCH_NAME { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "Creditor Identification Category Code")]
        public string CR_ID_CATEGORY { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Creditor Identification")]
        public string CR_ACCT_OR_NRC { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Creditor Name")]     
        public string CR_NAME { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Creditor Postal Address")]
        public string CR_ADDRESS { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Creditor Phone Number")]
        public string CR_PHONE { get; set; }
        public string Cust_CR_Transfer { get; set; }

    }
}