using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_BANKINFORMATION")]
    public class ContactUs
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(100)]
        [Display(Name ="Application Name")]
        [Required]
        public string AppName { get; set; }
        [StringLength(50)]
        [Required]
        public string Version { get; set; }
        public DateTime? Last_Update_Date { get; set; }
        [Display(Name ="About Us Description")]
        [Required]
        public string About_Us_Desc { get; set; }
        [Display(Name ="Contact Us Description")]
        [Required]
        public string Contact_Us_Desc { get; set; }
        [StringLength(100)]
        [Display(Name ="Customer Care Phone 1")]
        [Required]
        public string Customer_Care_Phone_1 { get; set; }
        [StringLength(100)]
        [Display(Name = "Customer Care Phone 2")]
        [Required]
        public string Customer_Care_Phone_2{ get; set; }
        [Display(Name = "Customer Care Phone 3")]
        [StringLength(100)]
        [Required]
        public string Customer_Care_Phone_3 { get; set; }
        [StringLength(200)]
        [Display(Name = "Contact Bank Mail")]
        [EmailAddress]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        [Required]
        public string Contact_Bank_Mail { get; set; }
        [StringLength(200)]
        [Display(Name = "Contact Bank Website")]
        [Required]
        public string Contact_Bank_Website { get; set; }
        [Display(Name ="Contact Bank Address")]
        [Required]
        public string Contact_Bank_Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        [StringLength(1)]
        public string DEL_FLAG { get; set; }

    }
}