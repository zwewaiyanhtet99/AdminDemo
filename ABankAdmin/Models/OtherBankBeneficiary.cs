using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_OTHERBANKBENEFICIARY")]
    public class OtherBankBeneficiary
    {
        [Required]
        [Key]
        public int OTHER_BANK_BENE_ID { get; set; }
        public int OTHER_BANK_ID { get; set; }
        public int OTHER_BRANCH_ID { get; set; }
        [StringLength(50)]
        public string ACCOUNTNO { get; set; }
        [Required]
        [StringLength(200)]
        public string ACCOUNTNAME { get; set; }
        [StringLength(50)]
        public string NRC { get; set; }
        [StringLength(200)]
        public string DESCRIPTION { get; set; }
        [Required]
        [StringLength(20)]
        public string PHONE { get; set; }
        [StringLength(200)]
        public string ADDRESS { get; set; }
        [Required]
        [StringLength(1)]
        public string BENEFICIARY_TYPE { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public Boolean Active { get; set; }
        public Boolean? FAV_FLAG { get; set; }
        [StringLength(250)]
        public string CreditEmail { get; set; }
    }
}