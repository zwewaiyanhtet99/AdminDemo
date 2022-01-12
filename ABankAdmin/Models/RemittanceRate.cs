using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_REMITTANCE_RATE")]
    public class RemittanceRate
    {
        public int ID { get; set; }
        [Required]
        [StringLength(10)]
        public string CODE { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression("[0-9]*$", ErrorMessage = "FROM_AMT must be numeric.")] 
        public string FROM_AMT { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression("[0-9]*$", ErrorMessage = "TO_AMT must be numeric.")]
        public string TO_AMT { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool Active{get; set;}
    }
}