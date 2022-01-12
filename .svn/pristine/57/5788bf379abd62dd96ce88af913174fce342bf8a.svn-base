using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ACCOUNT_INFO")]
    public class Account_Info
	{
        public int ID { get; set; }
        [StringLength(50)]
        public string CIFID { get; set; }
        [StringLength(40)]
        public string ACCOUNTNO { get; set; }
        [StringLength(50)]
        public string SCHM_TYPE { get; set; }
        [StringLength(50)]
        [Display(Name = "Schema Code")]
        public string SCHM_CODE { get; set; }
        [StringLength(200)]
        public string ACC_DESC { get; set; }
        [StringLength(10)]
        public string CURRENCY { get; set; }
        [StringLength(20)]
        public string BRANCHCODE { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public Boolean ACCT_CLOSE_FLAG { get; set; }
        public decimal AVAI_BALANCE { get; set; }

    }
}