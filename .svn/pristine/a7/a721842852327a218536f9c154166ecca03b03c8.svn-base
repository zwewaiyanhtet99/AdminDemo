using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Req_UserInAccount")]
    public class C_Req_UserInAccount
    {
        public int Id { get; set; }

        public int ReqUserID { get; set; }

        public int CorporateId { get; set; }

        [Required]
        [StringLength(500)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountNo { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; }

        [StringLength(200)]
        public string AccountTypeDesc { get; set; }

        [StringLength(50)]
        public string Currency { get; set; }
    }
}