using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table ("TBL_RULE")]
    public class Rule_
    {
        [Key]
        public int ID { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Value { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public string Del_Flag { get; set; }
    }
}