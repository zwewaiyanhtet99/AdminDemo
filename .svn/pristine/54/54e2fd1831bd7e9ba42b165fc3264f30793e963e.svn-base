using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_SCHEDULETRANSFERLOG")]
    public class ScheduleTransferLog
    {
        [Key]
        public int ID { get; set; }
        public int ScheduleTransferID { get; set; }
        [StringLength(50)]
        public string TASKNAME { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        [StringLength(50)]
        public string RESPCODE { get; set; }
        [StringLength(128)]
        public string RESPDESC { get; set; }
        [StringLength(128)]
        public string LOGMESSAGE { get; set; }
    }
}