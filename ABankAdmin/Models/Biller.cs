using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    public class Biller
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(200)]
        [Required]
        public string Name { get; set; }
        [StringLength(200)]
        [Required]
        public string BillerCode { get; set; }
        public bool IsApiIntegrate { get; set; }
        [StringLength(200)]
        public string Credentials_Info { get; set; }
        public decimal? ChargesAmount { get; set; }
        [StringLength(20)]
        public string ChargesAccountNo { get; set; }
        [StringLength(10)]
        public string ChargesCode { get; set; }
        [StringLength(20)]
        public string CreditAccountNo { get; set; }
        public int isFixRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        [StringLength(20)]
        public string BillerType { get; set; }
        public string ImagePath { get; set; }
        [StringLength(20)]
        public string Currency { get; set; }
        public bool Active { get; set; }
        [StringLength(128)]
        public string CREATED_USER_ID { get; set; }
        public DateTime? CREATED_DATETIME { get; set; }
        [StringLength(128)]
        public string UPDTED_USER_ID { get; set; }
        public DateTime? UPDATED_DATETIME { get; set; }
    }
}