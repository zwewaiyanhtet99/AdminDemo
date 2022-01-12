using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_IB_HOWTOUSE")]

    public class IB_HowToUse
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public string FormName { get; set; }
        [Required]
        [Display(Name = "IB Menu")]
        public int MenuID { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public Boolean Del_Flag { get; set; }        

        [ForeignKey("MenuID")]
        public virtual IB_Menu VrIB_Menu { get; set; }
    }
}