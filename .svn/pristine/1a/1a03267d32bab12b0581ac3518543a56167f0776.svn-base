using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_FEEDBACK")]
    public class FeedBack
    {
        public int ID { get; set; }
        [Required]
        public string TITLE { get; set; }
        [Required]
        public string DESCRIPTION { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public string ACTIVE { get; set; }
    }
}