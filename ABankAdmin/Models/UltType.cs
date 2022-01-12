using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_UTILITIES_TYPE")]
    public class UltType
    {
        public int ID { get; set; }
        public string TypeName { get;set; }
    }
}