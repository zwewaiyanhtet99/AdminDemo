using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_REQ_ACC")]
    public class ReqAcc
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string CIFID { get; set; }
        [StringLength(40)]
        public string ACCOUNTNO { get; set; }
        [StringLength(50)]
        public string ACC_TYPE { get; set; }
        [StringLength(50)]
        [Display(Name = "Schema Code")]
        public string SCHM_CODE { get; set; }
        [StringLength(200)]
        public string ACC_DESC { get; set; }
        [StringLength(10)]
        public string CURRENCY { get; set; }
        [StringLength(20)]
        public string BRANCHCODE { get; set; }
        public Boolean QR_ALLOW { get; set; }
        public Boolean ISNEW { get; set; }
        public int? REQUSER_ID { get; set; }
    }
}