using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class RequestUserSearchVM
    {
        public string CompanyName { get; set; }
        public string CorporateID { get; set; }
        public string UserName { get; set; }
        public int? UserRole { get; set; }
        public bool? UserType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public byte? Status { get; set; }
    }
}