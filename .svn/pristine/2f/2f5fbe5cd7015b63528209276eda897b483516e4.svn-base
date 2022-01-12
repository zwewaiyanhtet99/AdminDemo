using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_FINACLE_LOGIN")]
    public class FinacleLogin
    {
        public int ID { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "User/Schema")]
        public string UserSchema { get; set; }
        [Required]
        [StringLength(20)]
        public string Host { get; set; }
        [Required]
        [StringLength(20)]
        public string SID { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        [Required]
        public int? Port { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

    }
}