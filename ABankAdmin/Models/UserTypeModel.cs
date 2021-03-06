using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_USERTYPE")]
    public class UserTypeModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "ICONIC Tier")]
        public string USERTYPE { get; set; }
        [Required]
        [Display(Name = "ICONIC Tier Code")]
        public int USERTYPE_CODE_LIMIT { get; set; }
        [Required]
        [Display(Name = "ICONIC Tier Limit")]
        public int GENERATED_LIMIT { get; set; }
        public bool DEL_FLAG { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        [Required]
        [Display(Name = "Tier Type")]
        public string Type { get; set; }
    }
    
}