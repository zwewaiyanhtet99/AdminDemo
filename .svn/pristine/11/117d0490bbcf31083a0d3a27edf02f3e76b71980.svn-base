using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ICONIC_BLACKLIST")]
    public class IconicBlacklistModel
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string BlackListNumber { get; set; }
        [StringLength(50)]
        public string Description { get; set; }

        public DateTime? CREATEDDATE { get; set; }

        [Display(Name = "User Name")]
        public string CREATEDUSERID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DEL_FLAG { get; set; }
        public int UserTypeID { get; set; }
    }
}