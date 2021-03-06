using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_UserInAccount")]
    public class C_UserInAccount
    {
        public int Id { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedUserId { get; set; }

        public bool IsDelete { get; set; }
        public string SCHM_CODE { get; set; }
    }
}