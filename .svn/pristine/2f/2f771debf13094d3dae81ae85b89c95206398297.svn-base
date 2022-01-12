using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_STATE")]
    public class C_State
    {
        public int ID { get; set; }

        [Required]
        //[StringLength(50)]
        [StringLength(20, MinimumLength = 3)]
        [DisplayName("State Name")]
        public string NAME { get; set; }
        public int COUNTRY_ID { get; set; }
        [StringLength(128)]
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        [StringLength(128)]
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        [ForeignKey("COUNTRY_ID")]
        public virtual C_Country Vrcountry { get; set; }
    }
}