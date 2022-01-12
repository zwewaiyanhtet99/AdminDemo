using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class OtherBankReconSearchVM
    {
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public List<Other_Bank_ReconVM> otherbankvms { get; set; }
    }
}