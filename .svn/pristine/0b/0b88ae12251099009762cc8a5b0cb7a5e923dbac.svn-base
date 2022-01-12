using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_REQ_USER")]
    public class C_Req_User
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }

        [Required]
        [StringLength(20)]
        public string MOBILENO { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required]
        public string ADDRESS { get; set; }

        [StringLength(200)]
        public string FULLNAME { get; set; }

        public bool ISNEW { get; set; }

        public byte STATUS { get; set; }

        [StringLength(128)]
        public string MAKER { get; set; }

        public DateTime? REQUESTEDDATE { get; set; }

        public int? BranchID { get; set; }

        [StringLength(128)]
        public string CHECKER { get; set; }

        public DateTime? CHECKEDDATE { get; set; }

        public string CHECKERREASON { get; set; }

        public int? CORPORATEID { get; set; }

        public int? PositionID { get; set; }

        public int? DepartmentID { get; set; }

        public string CORPORATEMAKER { get; set; }

        public string CORPORATECHECKER { get; set; }
        [ForeignKey("CORPORATEID")]
        public virtual C_Corporate VrCorporate { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch VrBranch { get; set; }

        [ForeignKey("PositionID")]
        public virtual C_Position VrPosition { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual C_Department VrDepartment { get; set; }
    }
}