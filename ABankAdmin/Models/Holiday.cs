using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Holiday")]
    public class Holiday
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Holiday Name")]
        public string HolidayName { get; set; }
        [Required]
        [Display(Name = "Holiday Date")]
        public DateTime HolidayDate { get; set; }
        [Required]
        public int FinancialYear { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public bool DEL_FLAG { get; set; }
    }
}