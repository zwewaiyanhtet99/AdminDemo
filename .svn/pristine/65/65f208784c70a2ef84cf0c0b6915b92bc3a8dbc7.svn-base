using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("TBL_USER")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(36)]
        public string USERID { get; set; }
        [Required]
        [StringLength(20)]
        public string MOBILENO { get; set; }
        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }
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
        [Required]
        [StringLength(250)]
        public string LOGINPASSWORD { get; set; }
        [Required]
        [StringLength(250)]
        public string TRANSACTIONPASSWORD { get; set; }
        public Boolean FIRSTTIME_FLAG { get; set; }
        public Boolean ISLOGIN_FLAG { get; set; }
        public Boolean ISLOCK_FLAG { get; set; }
        public Boolean FORCE_PWD_CHANGE_FLG { get; set; }
        public Boolean FORCE_TXN_PWD_CHANGE_FLG { get; set; }
        public Boolean DEL_STATUS { get; set; }
        public int FAIL_PASSWORD_COUNT { get; set; }
        public int FAIL_TRAN_PWD_COUNT { get; set; }
        public DateTime? PWD_EXP_DATE { get; set; }
        [StringLength(20)]
        public string USER_TYPE { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDUSERID { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public string UPDATEUSERID { get; set; }
        [StringLength(200)]
        public string FULLNAME { get; set; }
        [StringLength(250)]
        public string LOGIN_PRE_PWD_1 { get; set; }
        [StringLength(250)]
        public string LOGIN_PRE_PWD_2 { get; set; }
        [StringLength(250)]
        public string LOGIN_PRE_PWD_3 { get; set; }
        [StringLength(250)]
        public string TRAN_PRE_PWD_1 { get; set; }
        [StringLength(250)]
        public string TRAN_PRE_PWD_2 { get; set; }
        [StringLength(250)]
        public string TRAN_PRE_PWD_3 { get; set; }
        [StringLength(20)]
        public string MINOR { get; set; }
        [StringLength(20)]
        public string GENDER { get; set; }
        [Display(Name = "Transaction Count")]
        //[DisplayFormat(DataFormatString = "{0:N2}")]        
        public int? ALLOW_TRAN_COUNT { get; set; }
        [Display(Name = "Maximum Amount")]
        //[DisplayFormat(DataFormatString ="{0:N2}")]
        public decimal? DAILY_MAX_AMT { get; set; }
        public DateTime? TRANPWD_EXP_DATE { get; set; }
        public Boolean IsVIP { get; set; }
        public int? CORPORATEID { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("PositionId")]
        public virtual C_Position VrPosition { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual C_Department VrDepartment { get; set; }
        [ForeignKey("CORPORATEID")]
        public virtual C_Corporate VrCorporate { get; set; }
        public Boolean CHK_old_pwd_flag { get; set; }
    }
}