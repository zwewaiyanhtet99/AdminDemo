using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_REQ_CHANGES")]
    public class ReqChange
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public int USERID { get; set; }
        public byte TYPE { get; set; }
        public byte STATUS { get; set; }
        [Display(Name = "Maker")]
        public string MAKER { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime REQUESTEDDATE { get; set; }
        [Display(Name = "Branch Name")]
        public int BranchID { get; set; }
        [Display(Name = "Checker")]
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string REQUESTINFO { get; set; }
        public string CHECKERREASON { get; set; }

        [ForeignKey("USERID")]
        public virtual User User { get; set; }

        [ForeignKey("MAKER")]
        public virtual AdminUser VrReqMaker { get; set; }

        [ForeignKey("CHECKER")]
        public virtual AdminUser VrReqChecker { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }
    }
}