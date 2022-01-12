using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class PrivilegeVM
    {
        public int ID { get; set; }
        [StringLength(200)]
        [Display(Name = "Privilege Name")]
        [Required(ErrorMessage = "Privilege Name is required")]
        public string PrivilegeName { get; set; }
        [StringLength(200)]
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }
        [StringLength(200)]
        [Display(Name = "Room")]
        [Required(ErrorMessage = "Room is required")]
        public string Room { get; set; }
        [StringLength(50)]
        [Display(Name = "DateTime")]
        [Required(ErrorMessage = "Date and Time is required")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string DateAndTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH mm ss}")]
        public string Time { get; set; }
        [StringLength(50)]
        [Display(Name = "No. of followers")]
        [Required(ErrorMessage = "No. of followers is required")]
        public string NoOfFollowers { get; set; }
        [StringLength(50)]
        [Display(Name = "Staff")]
        [Required(ErrorMessage = "Staff is required")]
        public string Staff { get; set; }
        public bool InUse { get; set; }
        
    }
}