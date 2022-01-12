using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class UserVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        //[RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Invalid Mobile No!")]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        public string MOBILENO { get; set; }
        [Required]
        //[StringLength(50)]
        [MaxLength(16, ErrorMessage = "Maximum length is 16."), MinLength(6, ErrorMessage = "Minimum length is 6.")]
        public string USERNAME { get; set; }
        [StringLength(200)]
        public string FULLNAME { get; set; }
        [Required]
        [StringLength(50)]
        public string NRC { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required]
        public string ADDRESS { get; set; }
        [Required]
        [StringLength(20)]
        public string CIFID { get; set; }
        [StringLength(20)]
        public string MINOR { get; set; }
        [StringLength(20)]
        public string GENDER { get; set; }
        [Display(Name = "Allowed Transaction Count")]
        [StringLength(13, MinimumLength = 0, ErrorMessage = "The field From Amount must be between 0 and 2,147,483,647.")]
        ///[Range(typeof(Decimal), "0", "999999999999999999.99", ErrorMessage = "{0} must be a number between {1} and {2}.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Must be a Positive Number")]
        public string ALLOW_TRAN_COUNT { get; set; }
        [Display(Name = "Maximum Amount")]
        [StringLength(24, MinimumLength = 0, ErrorMessage = "The field From Amount must be between 0 and 9,999,999,999,999,999.99")]
        public string DAILY_MAX_AMT { get; set; }
        [Display(Name ="Lock Status")]
        public Boolean ISLOCK_FLAG { get; set; }
        [Display(Name = "Tran_Lock")]
        public Boolean TRAN_LOCK { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name = "VIP User")]
        public Boolean IsVIP { get; set; }
        [StringLength(20)]
        [Display(Name = "COMPANY REGISTRATION DATE")]
        public string USER_TYPE { get; set; }

    }
}