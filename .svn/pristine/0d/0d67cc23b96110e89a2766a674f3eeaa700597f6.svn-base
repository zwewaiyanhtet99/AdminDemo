using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    //read only
    [Table("TBL_FIREBASE")]
    public class FireBase
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string UserID { get; set; }
        [Required]
        [StringLength(300)]
        public string FirebaseToken { get; set; }
        //public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}