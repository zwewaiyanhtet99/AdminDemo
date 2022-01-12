using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_EMAIL_TEMPLATE")]

    public class EmailTemplate
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(500)]
        public string Type { get; set; }
        [Required]
        [StringLength(500)]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        
    }
}