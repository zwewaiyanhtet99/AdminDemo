using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class RequestChangesListVM
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string CorporateId { get; set; }

        public string Username { get; set; }

        public byte Type { get; set; }

        public string Maker { get; set; }

        public DateTime? RequestedDate { get; set; }

        public byte Status { get; set; }
        public bool IsDeleteable { get; set; }
    }
}