using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_CRONLOG")]
    public class CronLog
    {
        [Key]
        public int ID { get; set; }
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

    [Table("TBL_CRONLOG_ICONIC")]
    public class CronLog_Iconic
    {
        [Key]
        public int ID { get; set; }
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