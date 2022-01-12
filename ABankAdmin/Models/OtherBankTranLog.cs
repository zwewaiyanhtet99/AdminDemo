using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_OTHERBANKTRAN_LOG")]

    public class OtherBankTranLog
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name ="TRANSACTION ID")]
        public string TRAN_ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name ="FROM ACCOUNT")]
        public string FROM_ACCT { get; set; }
        //[Required]
        [StringLength(20)]
        [Display(Name = "TO ACCOUNT")]
        public string TO_ACCT { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name ="OFFICE ACCOUNT")]
        public string OFFICE_ACCT { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name ="FROM BRANCH")]
        public string FROM_BRANCH { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name ="TO BANK")]
        public string TO_BANK { get; set; }
        //[Required]
        [StringLength(200)]
        [Display(Name ="TO BRANCH")]
        public string TO_BRANCH { get; set; }
        [Required]
        [Display(Name ="TRANSACTION AMOUNT")]
        public decimal TRAN_AMT { get; set; }
        [StringLength(10)]
        [Display(Name ="CHARGES CODE")]
        public string CHARGE_CODE { get; set; }
        [Display(Name ="CHARGE AMOUNT")]
        public decimal? CHARGE_AMT { get; set; }
        [Display(Name ="DISCOUNT AMOUNT")]
        public decimal? DISCOUNT_AMT { get; set; }
        [Required]
        [Display(Name ="TOTAL AMOUNT")]
        public decimal TOTAL_AMT { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name ="VALUE DATE")]
        public string VALUE_DATE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name ="TRANSACTION DATE")]
        public string TRAN_DATE { get; set; }
        [StringLength(50)]
        public string NRC { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name ="USERNAME")]
        public string USERID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name ="TRANSACTION TYPE")]
        public string TRAN_TYPE { get; set; }
        //[Required]
        [StringLength(40)]
        [Display(Name ="PAYEE NAME")]
        public string PAYEE_NAME { get; set; }
        //[Required]
        [StringLength(20)]
        [Display(Name ="PAYEE PHONE")]
        public string PAYEE_PHONE { get; set; }
        //[Required]
        [Display(Name ="OTHER BANK BENEFICIARY ID")]
        public int? OTHER_BANK_BENE_ID { get; set; }
        [StringLength(250)]
        [Display(Name ="Status")]
        public string Core_status { get; set; }
        //[ForeignKey("USERID")]
        //public virtual User User { get; set; }
        public Boolean IsDownloaded { get; set; }
        public int? MakerTranLogId { get; set; }
        public int? BulkPaymentFileUploadId { get; set; }
        public string CBMTranId { get; set; }
        public string CBMStatus { get; set; }
        [StringLength(10)]
        public string CBMTran_Type { get; set; }
    }
}