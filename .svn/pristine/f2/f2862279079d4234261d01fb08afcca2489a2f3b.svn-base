using ABankAdmin.Models;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class ApvTranRuleAndDetailVM
    {
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose the Company first.")]
        public int CorporateId { get; set; }
        [Display(Name = "Company Name")]
        public string COMPANY_NAME { get; set; }
        //[Range(0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        [Required]
        [Display(Name ="From Amount")]
        //[DisplayFormat(DataFormatString ="{0:N2}")]
        [RegularExpression(@"[0-9]+(?:-[0-9]+)?(,[0-9]+(?:-[0-9]+)?)*", ErrorMessage = "Entered amount is not valid.")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field From Amount must be between 0 and 9,999,999,999,999,999.99")]
        public string FROM_AMT { get; set; }
        //[Range(0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        [Required]
        [Display(Name ="To Amount")]
        //[DisplayFormat(DataFormatString = "{0:N2}")]

        //[GreaterThan("FROM_AMT")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field To Amount must be between 0 and 9,999,999,999,999,999.99")]
        public string TO_AMT { get; set; }
        [Required]
        [Display(Name ="Currency")]
        public string CURRENCY { get; set; }
        [Range(0, 10, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name ="No. Of Approvers")]
        public int NO_OF_APPROVERS { get; set; }
        [Display(Name ="Approvers")]
        public string APPROVERS { get; set; }
        [Display(Name ="Positions")]
        public string POSITIONS { get; set; }
        [Display(Name = "Type")]
        public bool IsForBulkPayment { get; set; }
        public string APPROVERSID { get; set; }
        public string POSITIONSID { get; set; }
        [Required]
        [StringLength(128)]
        public string CreatedUserID { get; set; }
        [Display(Name = "CREATED DATE")]
        public DateTime? CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}