using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ABankAdmin.Models;

namespace ABankAdmin.ViewModels
{
    public class CReqChangesVM
    {
        public int ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CORPORATEID { get; set; }
        public string USERNAME { get; set; } 
        public byte TYPE { get; set; }
        public string MAKER { get; set; }
        public string CHECKER { get; set; }
        public string CORPORATE_MAKER { get; set; }
        public string CORPORATE_CHECKER { get; set; }
        public DateTime? REQUESTEDDATE { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string CHECKERREASON { get; set; }
        public byte STATUS { get; set; }
        public int? BranchId { get; set; }
        public string BRANCH { get; set; }

        [ForeignKey("USERID")]
        public virtual User User { get; set; }

        [ForeignKey("MAKER")]
        public virtual AdminUser VrReqMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrReqChecker { get; set; }
       
        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("CORPORATEID")]
        public virtual C_Corporate VrCorporate { get; set; }
    }
}