using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_RM_TL_Info")]
    public class RM_TL_Info
    {
        [Display(Name = "RM TL ID")]
        public int ID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM TL Name")]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "RM TL's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Phone No")]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        public string PhoneNo { get; set; }
        [StringLength(50)]
        public string Description { get; set; }

        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDUSERID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DEL_FLAG { get; set; }
    }

    [Table("TBL_RM")]
    public class RM_Info
    {
        [Display(Name = "RM ID")]
        public int ID { get; set; }
        public int RM_TL_ID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name")]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "RM's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string Email { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDUSERID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DEL_FLAG { get; set; }
    }
    public class RM_InfoVM
    {
        [Display(Name = "RM ID")]
        public int ID { get; set; }
        [Required]
        public int RM_TL_ID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name")]
        public string Name { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM TL Name")]
        public string RM_TL_Name { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "RM's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string Email { get; set; }
    }
}