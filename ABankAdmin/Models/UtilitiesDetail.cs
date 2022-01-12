using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_UTILITIES_DETAIL")]
    public class UtilitiesDetail
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int UTILITIES_ID { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        //[RegularExpression("[+]*[0-9]*$", ErrorMessage = "DENO must be a positive number.")]
        public string DENO { get; set; }
        [StringLength(1)]
        public string ACTIVE { get; set; }
        [Required]
        [StringLength(100)]
        public string CURRENCY { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedUserId { get; set; }

        [ForeignKey("UTILITIES_ID")]
        public virtual Utilities VrUID { get; set; }
    }
}