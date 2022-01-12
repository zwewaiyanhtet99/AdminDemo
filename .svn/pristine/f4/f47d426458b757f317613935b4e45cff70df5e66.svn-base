using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Privilege_Usage")]
    public class Privilege_Usage
    {
        public int ID { get; set; }
        [StringLength(200)]
        public string CustomerName { get; set; }
        [StringLength(200)]
        public string CustomerNRC { get; set; }
        [StringLength(200)]
        public string IconicNumber { get; set; }
        [StringLength(200)]
        public string IconicTier { get; set; }
        public DateTime IconicExpireDate { get; set; }
        [StringLength(200)]
        public string PrivilegeUsage { get; set; }
        public DateTime ServiceDate { get; set; }
        [StringLength(200)]
        public string ServiceLocation { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDelete { get; set; }
    }
}