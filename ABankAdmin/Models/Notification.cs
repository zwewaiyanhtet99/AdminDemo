using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_NOTIFICATION")]
    public class Notification
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(100)]
        [Required]
        public string TITLE { get; set; }
        [Required]
        public string CONTENT { get; set; }
        [StringLength(1)]
        public string SENDERTYPE { get; set; }
        [StringLength(20)]
        public string CONTENTTYPE { get; set; }
        [Required]
        public DateTime DATE { get; set; }
        //[Required]
        [StringLength(50)]
        public string TO_USERID { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public Boolean? ACTIVE { get; set; }

        [NotMapped]
        public Boolean IsAll { get; set; }

        [NotMapped]
        public string CIFIds { get; set; }

        [NotMapped]
        public RadioOptions Category { get; set; }

        public enum RadioOptions
        {
            All = 1,
            ByName = 2,
            Multiple = 3
        }
    }
}