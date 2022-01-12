using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_SMS_BODY")]
    public class SMS_Body
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        [Display(Name ="Transaction Type")]
        public string TransactionType { get; set; }
        [Display(Name ="Debit Message")]
        public string DebitMessage { get; set; }
        [Display(Name ="Credit Message")]
        public string CreditMessage { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean Active { get; set; }
    }
}