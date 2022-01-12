using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_CURRENCY")]
    public class Currency
    {
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public string DEL_FLAG { get; set; }
        [StringLength(10)]
        [Display(Name = " Retail ChargesCode")]
        public string ChargesCode { get; set; }
        [StringLength(10)]
        [Display(Name = "Corporate ChargesCode")]
        public string C_ChargesCode { get; set; }
       

    }
}