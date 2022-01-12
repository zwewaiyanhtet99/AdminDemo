using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_UserTranLimit")]
    public class C_Req_UserTranLimit
    {
        public int Id { get; set; }

        public int ReqUserID { get; set; }

        public int CorporateId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string RuleCode { get; set; }

        public decimal Value { get; set; }
    }
}