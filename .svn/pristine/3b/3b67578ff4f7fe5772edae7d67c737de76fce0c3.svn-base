using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Log")]
    public class Api_Log
    {
        public int ID { get; set; }
        [Display(Name ="Request Method")]
        public string RequestMethod { get; set; }
        [Display(Name = "Request Path")]
        public string RequestPath { get; set; }
        [Display(Name = "Request Data")]
        public string RequestData { get; set; }
        [Display(Name = "Request Card ID")]
        public string RequestCardID { get; set; }
        [Display(Name = "Request Date Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime RequestDateTime { get; set; }
        [Display(Name = "Request File Path")]
        public string RequestFilePath { get; set; }
        [Display(Name = "Log Message")]
        public string LogMessage { get; set; }
        [Display(Name = "Response Data")]
        public string ResponseData { get; set; }
        [Display(Name = "Response Date Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime ResponseDateTime { get; set; }
        [Display(Name = "Response Code")]
        public string ResponseCode { get; set; }
        [Display(Name = "Response Description")]
        public string ResponseDescription { get; set; }
        public string UserType { get; set; }
    }
}