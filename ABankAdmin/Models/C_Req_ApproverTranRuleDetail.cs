using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_ApproverTranRuleDetail")]
    public class C_Req_ApproverTranRuleDetail
    {
        [Key]
        public int Id { get; set; }
        public int ApproverTranRuleId { get; set; }
        public int PositionId { get; set; }
        [StringLength(50)]
        public string UserId { get; set; }
        public bool IsDelete { get; set; }
        public bool IsUser { get; set; }
        [ForeignKey("ApproverTranRuleId")]
        public virtual C_Req_ApproverTranRule VrReqApvtranrules { get; set; }
        [ForeignKey("PositionId")]
        public virtual C_Position Vrposition { get; set; }
    }
}