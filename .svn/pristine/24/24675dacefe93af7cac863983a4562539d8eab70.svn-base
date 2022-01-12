using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class RuleVM
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(21, MinimumLength = 0, ErrorMessage = "The field From Amount must be between 0 and 9,999,999,999,999,999")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public string Value { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public string Del_Flag { get; set; }
    }
}