using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ADS_PHOTO")]
    public class ADS
    {
        public int ID { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        //format (https://www.)
        [Url(ErrorMessage = "Correct ImageRedirectLink format is such as 'https://www.google.com'.")]
        public string ImageRedirectLink { get; set; }        
        public string ImageType { get; set; }
        //[RegularExpression("[^0-9]", ErrorMessage = "OrderNo must be numeric.")]
        [Range(0, int.MaxValue, ErrorMessage = "OrderNo must be a positive number")]
        [Required]
        public int? OrderNo { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }

    }
}