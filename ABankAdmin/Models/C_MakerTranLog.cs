using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_MakerTranLog")]
    public class C_MakerTranLog
    {
        [Key]
        public long Id { get; set; }
        [Display(Name = "Company Name")]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose the Company first.")]
        public int CorporateId { get; set; }
        [StringLength(50)]
        public string MakerId { get; set; }
        [StringLength(50)]
        public string TranId { get; set; }
        [StringLength(20)]
        public string FromAccount { get; set; }
        [StringLength(20)]
        public string ToAccount { get; set; }
        public decimal TranAmount { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(50)]
        public string TranType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? OtherBankBeneID { get; set; }
        [StringLength(500)]
        public string TransactionDescription { get; set; }
        [StringLength(15)]
        public string MobileNo { get; set; }
        [StringLength(15)]
        public string BillerCode { get; set; }
        public int? ScheduleTransferID { get; set; }
        [StringLength(250)]
        public string CreditEmail { get; set; }
        [StringLength(10)]
        public string Currency { get; set; }
        public decimal? ChargeAmount { get; set; }
        public int? BulkPaymentFileUploadID { get; set; }
        [StringLength(200)]
        public string BulkBeneficiaryName { get; set; }
        //public decimal MMKAmount { get; set; }
        
    }
}