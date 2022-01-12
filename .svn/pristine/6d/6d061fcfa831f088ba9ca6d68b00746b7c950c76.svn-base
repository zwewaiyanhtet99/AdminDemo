using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_ADMINLOG")]
    public class AdminLog
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public DateTime LogDateTime { get; set; }
        [Required]
        public string Controller { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        [Display(Name ="Log Type")]
        public string LogType { get; set; }
        [Display(Name ="Description")]
        public string Desc { get; set; }
        public string Detail { get; set; }
        [Display(Name ="Line No.")]
        public int? LineNo { get; set; }
        [Display(Name ="Request Data")]
        public string RequestData { get; set; }

    }
}