using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_BulkPaymentFileUpload")]
    public class C_BulkPaymentFileUpload
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        public int CorporateID { get; set; }
        [StringLength(300)]
        [Display(Name = "Maximum Record")]
        public string FileName { get; set; }
        [StringLength(20)]
        public string FromAccount { get; set; }
        [StringLength(10)]
        public string FromAccountCurrency { get; set; }
        [StringLength(50)]
        public string TransType { get; set; }
        //[Display(Name = "Total Amount")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public decimal TotalAmount { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool DEV_FLAG { get; set; }
        [StringLength(50)]
        public string Status { get; set; }        
        public bool IsSetStatus { get; set; }
        public decimal? ChargesAmount { get; set; }
        public string TransferDate { get; set; }
        public string Description { get; set; }

        [ForeignKey("CorporateID")]
        public virtual C_Corporate VrCorporate { get; set; }
    }
}