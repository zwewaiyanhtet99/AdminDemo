using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_SERVICEMANAGEMENT")]
    public class ServiceManagement
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Display(Name = "Service Name")]
        [StringLength(128)]
        public string SERVICENAME { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        [Display(Name = "Time Format")]
        [StringLength(50)]
        public string TimeFormat { get; set; }
    }
}