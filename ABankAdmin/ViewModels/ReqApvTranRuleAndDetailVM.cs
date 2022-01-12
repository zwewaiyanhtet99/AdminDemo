using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ABankAdmin.Models;

namespace ABankAdmin.ViewModels
{
    public class ReqApvTranRuleAndDetailVM
    {       
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose the Company first.")]
        public int CorporateId { get; set; }
        [Display(Name = "Company Name")]
        public string COMPANY_NAME { get; set; }
        [Required]
        [Display(Name = "From Amount")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field From Amount must be between 0 and 9,999,999,999,999,999.99")]
        //[RegularExpression(@"[0-9]+(?:-[0-9]+)?(,[0-9]+(?:-[0-9]+)?)*", ErrorMessage = "Entered amount is not valid.")]
        public string FROM_AMT { get; set; }
        [Required]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field To Amount must be between 0 and 9,999,999,999,999,999.99")]
        [Display(Name = "To Amount")]
        public string TO_AMT { get; set; }
        [Required]
        [Display(Name = "Currency")]
        public string CURRENCY { get; set; }
        [Range(0, 10, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name = "No. Of Approvers")]
        public int NO_OF_APPROVERS { get; set; }
        [Display(Name = "Approvers")]
        public string APPROVERS { get; set; }
        [Display(Name = "Positions")]
        public string POSITIONS { get; set; }
        [Display(Name = "Transaction Type")]
        public bool IsForBulkPayment { get; set; }
        public string APPROVERSID { get; set; }
        public string POSITIONSID { get; set; }
        public int BranchID { get; set; }
        public int? Req_TranRuleId { get; set; }
        public byte TYPE { get; set; }
        public byte STATUS { get; set; }
        public string MAKER { get; set; }
        public DateTime? REQUESTEDDATE { get; set; }
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string CHECKERREASON { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("CorporateId")]
        public virtual C_Corporate Vrcorporate { get; set; }
        [ForeignKey("Req_TranRuleId")]
        public virtual C_Req_ApproverTranRule VrReqApvtranrules { get; set; }

    }
}