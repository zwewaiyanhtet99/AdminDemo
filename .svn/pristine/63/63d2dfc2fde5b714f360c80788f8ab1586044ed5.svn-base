using System;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class AccountVM
    {
        public int ID { get; set; }
        public string CIFID { get; set; }
        public Boolean Active { get; set; }
        [StringLength(40)]
        public string ACCOUNTNO { get; set; }
        [StringLength(50)]
        public string SCHM_TYPE { get; set; }
        [StringLength(50)]
        public string ACC_TYPE { get; set; }
        [StringLength(50)]
        [Display(Name = "Schema Code")]
        public string SCHM_CODE { get; set; }
        [StringLength(50)]
        public string ACC_DESC { get; set; }
        [StringLength(10)]
        public string CURRENCY { get; set; }
        [StringLength(20)]
        public string BRANCHCODE { get; set; }
        public Boolean QR_ALLOW { get; set; }
        public string AvailableAmt { get; set; }
    }
}