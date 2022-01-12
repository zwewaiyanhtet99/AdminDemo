using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_DEPARTMENT")]
    public class C_Department
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string NAME { get; set; }

        [StringLength(128)]
        public string CreatedUserID { get; set; }

        public DateTime CreatedDateTime { get; set; }

        [StringLength(128)]
        public string UpdatedUserId { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

        public bool DEL_FLAG { get; set; }
    }
}