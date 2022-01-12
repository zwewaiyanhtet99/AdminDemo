using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class C_ReqUserVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Invalid Mobile No!")]
        public string MOBILENO { get; set; }
        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }
        [StringLength(200)]
        public string FULLNAME { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required]
        public string ADDRESS { get; set; }
        [Display(Name = "Type")]
        public Boolean ISNEW { get; set; }
        public byte STATUS { get; set; }
        public string MAKER { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime REQUESTEDDATE { get; set; }
        public string CHECKER { get; set; }
        public DateTime? CHECKEDDATE { get; set; }
        public string CHECKERREASON { get; set; }
        public string BRANCH { get; set; }
        public string Corporate { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string CompanyName { get; set; }
        public string CorporateId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? CREATEDDATE { get; set; }
    }
}