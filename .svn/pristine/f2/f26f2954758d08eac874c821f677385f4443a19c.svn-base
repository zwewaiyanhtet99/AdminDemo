using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_CREDENTIAL")]
    public class Credential
    {
        [Required]
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [StringLength(100)]
        public string Key { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public string Status { get; set; }

    }
}