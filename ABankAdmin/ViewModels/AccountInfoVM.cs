using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class AccountInfoVM
    {
        public int AccountInfoId { get; set; }

        public bool IsActive { get; set; }

        public string AccountNo { get; set; }

        public string AccountType { get; set; }

        public string Currency { get; set; }

        public string AccountTypeDesc { get; set; }
        public string Schm_Code { get; set; }
    }
}