using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_REQ_CHANGES")]
    public class C_Req_Changes
    {
        public int ID { get; set; }
        [Display(Name ="USERNAME")]
        public int USERID { get; set; }

        public byte TYPE { get; set; }

        public byte STATUS { get; set; }

        [StringLength(128)]
        public string MAKER { get; set; }

        public DateTime? REQUESTEDDATE { get; set; }

        [Display(Name ="BRANCH NAME")]
        public int? BranchID { get; set; }

        [StringLength(128)]
        public string CHECKER { get; set; }

        public DateTime? CHECKEDDATE { get; set; }

        public string CHECKERREASON { get; set; }

        public int CORPORATEID { get; set; }

        public string CMAKER { get; set; }

        public string C_CHECKER { get; set; }

        [ForeignKey("USERID")]
        public virtual User User { get; set; }

        [ForeignKey("MAKER")]
        public virtual AdminUser VrReqMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrReqChecker { get; set; }

        //[ForeignKey("CMAKER")]
        //public virtual User VrCReqMaker { get; set; }

        //[ForeignKey("C_CHECKER")]
        //public virtual User VrCReqChecker { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }
        [ForeignKey("CORPORATEID")]
        public virtual C_Corporate VrCorporate { get; set; }
    }
}