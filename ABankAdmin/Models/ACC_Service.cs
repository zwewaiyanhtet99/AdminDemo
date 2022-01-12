using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_ACC_SERVICES")]
    public class ACC_Service
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name ="Account Type")]
        [StringLength(20)]
        public string Account_Type{ get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserID { get; set; }
        public Boolean Del_Flag { get; set; }
        [Display(Name ="Account Description")]
        [Required]
        public string Acc_Description { get; set; }
        [StringLength(10)]
        [Required]
        public string Language { get; set; }

        //public virtual ICollection<ACC_Service_Desc> accdescs { get; set; }

    }
}