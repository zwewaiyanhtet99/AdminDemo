using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class IconicBookingVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "User Type :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Tier:")]
        public string USERTYPECODE { get; set; }
        [Required]
        [Display(Name = "THE ICONIC Reserved Number:")]
        public string BookingNo { get; set; }
        [StringLength(50)]
        [Display(Name = "Description :")]
        public string Description { get; set; }

        public string PrefixBooking { get; set; }
        public bool InUse { get; set; }
        [Display(Name = "User Name:")]
        public string Createduserid { get; set; }
    }

    public class IconicBlacklistVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "User Type :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Tier:")]
        public string USERTYPECODE { get; set; }
        [Required]
        [Display(Name = "Black list No :")]
        public string BlacklistNo { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Description :")]
        public string Description { get; set; }
        public string PrefixBooking { get; set; }
        [Display(Name = "User Name:")]
        public string Createduserid { get; set; }
    }

    public class IconicBookingBulkViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Type")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "User Type :")]
        public string USERTYPE { get; set; }
        public HttpPostedFileBase File { get; set; }

        
        
    }
    
    public class IconicBookingBulkItemResponse
    {
        public string BookingNo { get; set; }
        public string Description { get; set; }
        public string USERTYPECODE { get; set; }
        public string USERTYPE { get; set; }
        public string Message { get; set; }
    }
    public class IconicBookingBulkValidationViewModel
    {
        public List<IconicBookingBulkItemResponse> BulkList { get; set; }
        public int MessageCount { get; set; }
        


    }

}