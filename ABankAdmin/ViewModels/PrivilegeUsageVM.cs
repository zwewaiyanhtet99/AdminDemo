using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class PrivilegeUsageVM
    {
        [StringLength(200)]
        [Display(Name = "Customer's Name")]
        [Required(ErrorMessage = "Customer's Name is required")]
        public string CustomerName { get; set; }
        [StringLength(200)]
        [Display(Name = "Customer's NRC")]
        [Required(ErrorMessage = "Customer's NRC is required")]
        public string CustomerNRC { get; set; }
        [StringLength(200)]
        [Display(Name = "Iconic Number")]
        [Required(ErrorMessage = "Iconic Number is required")]
        public string IconicNumber { get; set; }
        [StringLength(200)]
        [Display(Name = "Iconic Tier")]
        [Required(ErrorMessage = "Iconic Tier is required")]
        public string IconicTier { get; set; }
        [StringLength(50)]
        [Display(Name = "Iconic Expire Date")]
        [Required(ErrorMessage = "Iconic Expire Date is required")]
        public string IconicExpireDate { get; set; }
        [StringLength(50)]
        [Display(Name = "Service Date")]
        [Required(ErrorMessage = "Service Date is required")]
        public string ServiceDate { get; set; }
        [StringLength(200)]
        [Display(Name = "Service Location")]
        [Required(ErrorMessage = "Service Location is required")]
        public string ServiceLocation { get; set; }
        [Required]
        [Display(Name = "Privilege")]
        public int PrivilegeId { get; set; }
        [Required(ErrorMessage = "Privilege Usage is required")]
        [StringLength(200)]
        [Display(Name = "Privilege Usage")]
        public string PrivilegeName { get; set; }
        public string strExpireDate { get; set; }
        public string strServiceDate { get; set; }
        public string PrivilegeUsage { get; set; }
        [Display(Name = "Service Time")]
        [DisplayFormat(DataFormatString = "{0:HH mm ss}")]
        public string Time { get; set; }
        [StringLength(50)]
        [Display(Name = "No. of followers")]
        [Required(ErrorMessage = "No. of followers is required")]
        public string NoOfFollowers { get; set; }
        [StringLength(50)]
        [Display(Name = "Staff Name")]
        [Required(ErrorMessage = "Staff is required")]
        public string Staff { get; set; }
       
    }
}