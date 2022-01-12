using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_TOWNSHIP")]
    public class Township
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        [Display(Name ="CITY")]
        [StringLength(20)]
        public string CITY_Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public string DEL_FLAG { get; set; }
    }
}