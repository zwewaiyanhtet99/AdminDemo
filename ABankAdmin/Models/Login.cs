using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_LOGIN")]
    public class Login
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string DeviceID { get; set; }
        public string SessionID { get; set; }
        public string Dynamickey { get; set; }
        public string LoginType { get; set; }
        public DateTime SessionExpirDate { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public DateTime? LAST_TXN_DATE { get; set; }
        public DateTime? LAST_UNSUCCESSFUL_LOGIN_TIME { get; set; }
    }
}