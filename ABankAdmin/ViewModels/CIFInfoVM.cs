using AbankAdminAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    public class CIFInfoVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Customer's Phone Number :")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        [Required]
        [Display(Name = "Customer's Address :")]
        public string Address { get; set; }
        [Display(Name = "Remark :")]
        public string REMARK { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "Use Reserved Number ")]
        public Boolean Iconic { get; set; }
        [Display(Name = "Reserved Number: ")]
        public string IconicBooking { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "Branch Name:")]
        public string BranchName { get; set; }
        [Display(Name = "User Name:")]
        public string Createduserid { get; set; }
        
        [Display(Name = "RM ID:")]
        [Required(ErrorMessage = "This RM:field is required")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Approver's email:")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }
        
        [StringLength(100)]
        [Display(Name = "User's email:")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }

        public string Status { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date: ")]

        public string UpgradeStatus { get; set; }
        [Display(Name = "THE ICONIC Upgrade Status ")]
        public DateTime? DeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date:")]
        public string strDeactivateDate { get; set; }
        public bool DELFLAG { get; set; }
        public string DeactivateStatus { get; set; }
        public string Flag { get; set; }//create,edit,deactivate

        [Display(Name = "THE ICONIC New Effective Date: ")]
        public DateTime NewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public string strNewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public DateTime NewExpireDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public string strNewExpireDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Display(Name = "THE ICONIC Updated Date:")]
        public string strUpdatedDate { get; set; }
        public string DowngradeStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CIFInfo
    {
        public CIFInfoVM uservm { get; set; }
        
    }

    public class CIFInfoEdit
    {
        public CIFInfoVM uservm { get; set; }
        public List<AccountInfo> accountvms { get; set; }

       
    }

    public class RetailResponseInfo_VM
    {
        public string Address { get; set; }
        public string Currency { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string ErrorType { get; set; }
        public string Gender { get; set; }
        public List<AccountInfo> lAcctInfo { get; set; }
        public string LeadSource { get; set; }
        public string Manager { get; set; }
        public string Minor { get; set; }
        public string Name { get; set; }
        public string NRC { get; set; }
        public string PhoneNumber { get; set; }
        public string PrimarySolID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
        public string TotalAmt { get; set; }
        public string BranchName
        { get; set; }
    }

    #region Cron
    public class RunCRONResponseModel
    {
        public string RespCode { get; set; }
        public string RespDescription { get; set; }
    }
    public class CRONScheudleModel
    {
        public string ID { get; set; }
        public string Schedule { get; set; }
        public string TaskName { get; set; }
    }

    public class CRONLogModel
    {
        public string ScheduleTranId { get; set; }
        public string TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RespCode { get; set; }
        public string RespDescription { get; set; }
        public string LogMessage { get; set; }
        public string Status { get; set; }
        public int TranLogID { get; set; }

        public CRONLogModel(string TaskName, string resCode, string RespDesc)
        {
            this.TaskName = TaskName;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;
            this.RespCode = resCode;
            this.RespDescription = RespDesc;
        }
    }
    #endregion

    #region ForApprover
    public class CIFInfoVMForApprover
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Customer's Phone Number :")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        [Required]
        [Display(Name = "Customer's Address :")]
        public string Address { get; set; }
        [Display(Name = "Remark :")]
        public string REMARK { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "Use Reserved Number ")]
        public Boolean Iconic { get; set; }
        [Display(Name = "Reserved Number: ")]
        public string IconicBooking { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date :")]
        public string strDeactivateDate { get; set; }
        [Display(Name = "Branch Name:")]
        public string BranchName { get; set; }
        [Display(Name = "User Name:")]
        public string Createduserid { get; set; }
        [Required]
        [Display(Name = "RM ID:")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }

        [StringLength(100)]
        [Display(Name = "Approver's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }

        [StringLength(100)]
        [Display(Name = "User's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }
        public string Status { get; set; }
        public string UpgradeStatus { get; set; }
        public string DeactivateStatus { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "RM's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string RMEmail { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "RM TL's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string RM_TL_Email { get; set; }
        public DateTime? DeactivateDate { get; set; }

        public string Flag { get; set; }//create,edit,deactivate

    }




    #endregion

    #region ForDeactivator
    public class CIFInfoVMForDeactivator
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Customer's Phone Number :")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        [Required]
        [Display(Name = "Customer's Address :")]
        public string Address { get; set; }
        
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date :")]
        public DateTime DeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date :")]
        public string strDeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "Branch Name:")]
        public string BranchName { get; set; }
        [Required]
        [Display(Name = "RM ID:")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }

        [StringLength(100)]
        [Display(Name = "Approver's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }

        [StringLength(100)]
        [Display(Name = "User's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public DateTime NewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public string strNewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public DateTime NewExpireDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public string strNewExpireDate { get; set; }
        [Display(Name = "THE ICONIC New Number: ")]
        public string NewIconicNo { get; set; }
        [Display(Name = "Reserved Number: ")]
        public string IconicReservedlist { get; set; }
        [Display(Name = "Use Reserved Number ")]
        public bool IconicReservedNo { get; set; }
        public string Remark { get; set; }


    }

    #endregion

    #region ForReport
    public class CIFInfoVMForReport
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Customer's Phone Number :")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        [Required]
        [Display(Name = "Customer's Address :")]
        public string Address { get; set; }

        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "Branch Name:")]
        public string BranchName { get; set; }
        [Required]
        [Display(Name = "RM ID:")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }
        [Display(Name = "Remark :")]
        public string REMARK { get; set; }
        public bool IsRegister { get; set; }

    }
    public class CIFInfoVMForExcelReport
    {
        
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [StringLength(50)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "RM ID:")]
        public int RMID { get; set; }
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }

        [StringLength(100)]
        [Display(Name = "Approver's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }

        [StringLength(100)]
        [Display(Name = "User's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }

        
    }
    #endregion

    #region ForRenewal
    public class CIFInfoVMForRenewal
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date :")]
        public DateTime DeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date :")]
        public string strDeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
       
        [Required]
        [Display(Name = "RM ID:")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }

        [StringLength(100)]
        [Display(Name = "Approver's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }

        [StringLength(100)]
        [Display(Name = "User's email")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public DateTime NewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public string strNewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public DateTime NewExpireDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public string strNewExpireDate { get; set; }
        


    }

    #endregion

    #region ForDowngrade
    public class CIFInfoVMForDowngrade
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "CIFID :")]
        public string CIFID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's Name :")]
        public string NAME { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        [Display(Name = "Customer's Phone Number :")]
        public string PHONENO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "THE ICONIC Tier :")]
        public string USERTYPE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "THE ICONIC Number :")]
        public string USERTYPECODE { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer's NRC :")]
        public string NRC { get; set; }
        [Required]
        [Display(Name = "Customer's Address :")]
        public string Address { get; set; }
        [Display(Name = "Remark :")]
        public string REMARK { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public DateTime EXPIREDATE { get; set; }
        [Display(Name = "THE ICONIC Expire Date :")]
        public string strEXPIREDATE { get; set; }
        [Display(Name = "Use Reserved Number ")]
        public Boolean Iconic { get; set; }
        [Display(Name = "Reserved Number: ")]
        public string IconicBooking { get; set; }
        [Display(Name = "THE ICONIC Effective Date: ")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "THE ICONIC Effective Date:")]
        public string strEffectiveDate { get; set; }
        [Display(Name = "Branch Name:")]
        public string BranchName { get; set; }
        [Display(Name = "User Name:")]
        public string Createduserid { get; set; }

        [Display(Name = "RM ID:")]
        [Required(ErrorMessage = "This RM:field is required")]
        public int RMID { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "RM Name:")]
        public string RMName { get; set; }

        [StringLength(100)]
        [Display(Name = "Approver's email:")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string ApproverEmail { get; set; }

        [StringLength(100)]
        [Display(Name = "User's email:")]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string UserEmail { get; set; }

        public string Status { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date: ")]

        public string UpgradeStatus { get; set; }
        [Display(Name = "THE ICONIC Upgrade Status ")]
        public DateTime? DeactivateDate { get; set; }
        [Display(Name = "THE ICONIC Deactivate Date:")]
        public string strDeactivateDate { get; set; }
        public bool DELFLAG { get; set; }
        public string DeactivateStatus { get; set; }
        public string Flag { get; set; }//create,edit,deactivate

        [Display(Name = "THE ICONIC New Effective Date: ")]
        public DateTime NewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Effective Date: ")]
        public string strNewEffectiveDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public DateTime NewExpireDate { get; set; }
        [Display(Name = "THE ICONIC New Expire Date: ")]
        public string strNewExpireDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Display(Name = "THE ICONIC Updated Date:")]
        public string strUpdatedDate { get; set; }
        public string DowngradeStatus { get; set; }
        [Display(Name = "THE ICONIC New Number :")]
        public string NewIconicNo { get; set; }
    }

    #endregion
}