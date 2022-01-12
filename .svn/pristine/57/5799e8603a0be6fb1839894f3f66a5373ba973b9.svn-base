using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_SCHEDULETRANSFER")]
    public class Schedule_Tranfer
    {
        [Key]
        public int ID { get; set; }
        [StringLength(128)]
        public string UserID { get; set; }
        [StringLength(50)]
        public string SessionID { get; set; }
        [StringLength(200)]
        public string FromAccName { get; set; }
        [StringLength(50)]
        public string FromAccNo { get; set; }
        [StringLength(200)]
        public string ToAccName { get; set; }
        [StringLength(50)]
        public string ToAccNo { get; set; }
        public decimal Amount { get; set; }
        [StringLength(128)]
        public string TranID { get; set; }
        public DateTime? TransferDate { get; set; }
        [StringLength(50)]
        public string TransferStatus { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public bool Active { get; set; }
        public decimal Charges { get; set; }

    }
}