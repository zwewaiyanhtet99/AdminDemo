using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_REQ_USER")]
    public class ReqUser
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }
        [Required]
        [StringLength(20)]
        public string MOBILENO { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required]
        public string ADDRESS { get; set; }
        [StringLength(200)]
        public string FULLNAME { get; set; }
        [Required]
        [StringLength(50)]
        public string NRC { get; set; }
        [StringLength(20)]
        public string MINOR { get; set; }
        [StringLength(20)]
        public string GENDER { get; set; }
        [Display(Name = "Transaction Count")]
        public int? ALLOW_TRAN_COUNT { get; set; }
        [Display(Name = "Maximum Amount")]
        public decimal? DAILY_MAX_AMT { get; set; }
        public Boolean ISNEW { get; set; }
        public byte STATUS { get; set; }
        public string MAKER { get; set; }
        public DateTime REQUESTEDDATE { get; set; }
        [Display(Name = "Branch")]
        public int BranchID { get; set; }
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string REQUESTINFO { get; set; }
        public string CHECKERREASON { get; set; }
        [StringLength(20)]
        public string USER_TYPE { get; set; }
        public Boolean IsVIP { get; set; }

        [ForeignKey("MAKER")]
        public virtual AdminUser VrMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrChecker { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }

    }
}