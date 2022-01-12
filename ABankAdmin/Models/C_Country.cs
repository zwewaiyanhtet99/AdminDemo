using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_COUNTRY")]
    public class C_Country
    {
        public int ID { get; set; }

        [Required]
        //[StringLength(50)]
        [StringLength(20, MinimumLength = 3)]
        [DisplayName("Country Name")]
        public string NAME { get; set; }
        [StringLength(128)]
        public string CreatedUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }

        [StringLength(128)]
        public string UpdatedUserId { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}