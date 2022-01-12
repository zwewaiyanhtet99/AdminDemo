using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_ApproverTranRule")]
    public class C_Req_ApproverTranRule
    {
        public C_Req_ApproverTranRule()
        {
            this.Details = new HashSet<C_Req_ApproverTranRuleDetail>();
        }

        [Key]
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose the Company first.")]
        public int CorporateId { get; set; }
        [Display(Name = "From Ammount")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public decimal FromAmount { get; set; }

        [Display(Name = "To Amount")]
        [Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]

        //[GreaterThan("FromAmount")]
        public decimal ToAmount { get; set; }
        [Range(0, 10, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name = "Number Of Approvers")]
        public int TotalApproverCount { get; set; }
        public bool IsHierarchy { get; set; }
        [Display(Name = "Bulk Transaction")]
        public bool IsForBulkPayment { get; set; }
        public bool IsDelete { get; set; }
        [StringLength(10)]
        public string Currency { get; set; }
        public int BranchID { get; set; }
        public int? Req_TranRuleId { get; set; }
        public byte TYPE { get; set; }
        public byte STATUS { get; set; }
        public string MAKER { get; set; }
        public DateTime? REQUESTEDDATE { get; set; }
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string CHECKERREASON { get; set; }
        [ForeignKey("CorporateId")]
        public virtual C_Corporate Vrcorporate { get; set; }

        public virtual ICollection<C_Req_ApproverTranRuleDetail> Details { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("MAKER")]
        public virtual AdminUser VrMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrChecker { get; set; }
    }
}