using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Models
{
    [Table("TBL_EXCHANGERATE")]
    public class ExchangeRate
    {
        public int id { get; set; }
        public DateTime RateListDate { get; set; }

        [Required]
        [Column("FXD_CurrencyCode")]
        [Display(Name = "Foreign Currency")]
        public string fxdCurrencyCode { get; set; }

        [Column("VAR_CurrencyCode")]
        [Display(Name = "Home Currency")]
        public string varCurrencyCode { get; set; }
        [Required]
        public decimal BUY_RATE { get; set; }
        [Required]
        public decimal SELL_RATE { get; set; }

      

    } 
}