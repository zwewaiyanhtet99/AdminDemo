using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("tbl_cifinformation")]
    public class CIFInfoModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID")]
        public string CIFID { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Name")]
        public string NAME { get; set; }
        
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Phone Number")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "ICONIC Number")]
        public string USERTYPECODE { get; set; }
        
        [StringLength(50)]
        [Display(Name = "NRC")]
        public string NRC { get; set; }
        [Display(Name = "Address")]
        public string ADDRESS { get; set; }
        [Display(Name = "Remark")]
        public string REMARK { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "Expire Date ")]
        public DateTime EXPIREDATE { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name = "User Name")]
        public string CREATEDUSERID { get; set; }
        [Display(Name = "Updated Date ")]
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DELFLAG { get; set; }
        [Display(Name = "Effective Date")]
        public DateTime EFFECTIVEDATE { get; set; }
        [Display(Name = "Branch Name")]
        public string BRANCHNAME { get; set; }
        public string RMName { get; set; }
        public int RMID { get; set; }
        public byte Status { get; set; }
        public string ApproverEmail { get; set; }
        public string UserEmail { get; set; }
        public byte UpgradeStatus { get; set; }
        public byte DeactivateStatus { get; set; }
        public byte DowngradeStatus { get; set; }

    }

    [Table("TBL_CIFINFORMATION_TEMP")]
    public class CIFInfoModelForTemp
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID")]
        public string CIFID { get; set; }

        [StringLength(50)]
        [Display(Name = "Name")]
        public string NAME { get; set; }

        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Phone Number")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "ICONIC Number")]
        public string USERTYPECODE { get; set; }

        [StringLength(50)]
        [Display(Name = "NRC")]
        public string NRC { get; set; }
        [Display(Name = "Address")]
        public string ADDRESS { get; set; }
        [Display(Name = "Remark")]
        public string REMARK { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "Expire Date ")]
        public DateTime EXPIREDATE { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name = "User Name")]
        public string CREATEDUSERID { get; set; }
        [Display(Name = "Updated Date ")]
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DELFLAG { get; set; }
        [Display(Name = "Effective Date")]
        public DateTime EFFECTIVEDATE { get; set; }
        [Display(Name = "Branch Name")]
        public string BRANCHNAME { get; set; }
        public string RMName { get; set; }
        public int RMID { get; set; }
        public byte Status { get; set; }
        public string ApproverEmail { get; set; }
        public string UserEmail { get; set; }
        public byte UpgradeStatus { get; set; }
        public byte DeactivateStatus { get; set; }
        public byte DowngradeStatus { get; set; }
    }
}