using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class PrivilegeUsageCreateVM
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
        [StringLength(200)]
        [Display(Name = "Privilege Name")]
        [Required(ErrorMessage = "Privilege Name is required")]
        public string PrivilegeName { get; set; }
        [StringLength(200)]
        [Display(Name = "Service Location")]
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Service Date")]
        [Required(ErrorMessage = "Date and Time is required")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string DateAndTime { get; set; }
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
        [Required]
        public int PrivilegeID { get; set; }
    }
}