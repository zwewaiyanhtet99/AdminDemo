using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Biller")]
    public class TBL_Biller
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string BillerCode { get; set; }
        public bool IsApiIntegrate { get; set; }
        public string Credentials_Info { get; set; }
        public decimal? ChargesAmount { get; set; }
        public string ChargesAccountNo { get; set; }
        public string ChargesCode { get; set; }
        public string CreditAccountNo { get; set; }
        public byte isFixRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string BillerType { get; set; }
        public string ImagePath { get; set; }
        public string Currency { get; set; }
        public bool Active { get; set; }
        public string CREATED_USER_ID { get; set; }
        public DateTime CREATED_DATETIME { get; set; }
        public string UPDTED_USER_ID { get; set; }
        public DateTime? UPDATED_DATETIME { get; set; }
        public virtual ICollection<TBL_Biller_Field> TBL_Biller_Fields { get; set; }
    }
}