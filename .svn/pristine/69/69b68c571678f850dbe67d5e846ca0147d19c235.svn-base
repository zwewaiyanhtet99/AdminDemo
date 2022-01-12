using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ICONIC_BOOKING")]
    public class IconicBookingModel
    {
        public int ID { get; set; }
        
        [StringLength(50)]
        [Display(Name = "THE ICONIC Reserved Number")]
        public string BOOKINGNUMBER { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name = "User Name")]
        public string CREATEDUSERID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool DEL_FLAG { get; set; }
        [Display(Name = "Used")]
        public bool InUse { get; set; }
        public int UserTypeID { get; set; }
    }

   
}