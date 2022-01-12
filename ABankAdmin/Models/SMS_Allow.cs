using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_SMS_ALLOW")]
    public class SMS_Allow
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        [Display(Name ="TRANSACTION TYPE")]
        public string TransactionType { get; set; }
        [Display(Name ="SENDER ALLOW")]
        public Boolean Sender_Allow { get; set; }
        [Display(Name ="RECEIVER ALLOW")]
        public Boolean Receiver_Allow { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean Active { get; set; }
        [Display(Name = "SENDER EMAIL ALLOW")]
        public Boolean Sender_Email_Allow { get; set; }
        [Display(Name = "RECEIVER EMAIL ALLOW")]
        public Boolean Receiver_Email_Allow { get; set; }
        
            
    }
}