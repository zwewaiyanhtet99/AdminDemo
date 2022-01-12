using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_APPROVE_RULE")]
    public class C_Approve_Rule
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Company Name")]
        public int CORPORATE_ID { get; set; }
        [Display(Name = "From Amount")]
        public decimal FROM_AMT { get; set; }
        [Display(Name = "To Amount")]
        public decimal TO_AMT { get; set; }
        [StringLength(10)]
        [Display(Name ="Currency")]
        public string CURRENCY { get; set; }
        [Display(Name = "No. of Approvers")]
        public int NO_OF_APPROVERS { get; set; }
        [Display(Name = "Approvers")]
        public string APPROVERS { get; set; }
        [Display(Name = "Position")]
        public string POSITION { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdateUserId { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public Boolean DEL_FLAG { get; set; }
        [ForeignKey("CORPORATE_ID")]
        public virtual C_Corporate Vrcorporate { get; set; }
    }
}