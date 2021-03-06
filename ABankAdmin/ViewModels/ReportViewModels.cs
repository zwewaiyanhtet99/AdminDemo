using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    //public class ReportViewModels
    //{

    //}
    #region User
    public class UserRPViewModel
    {
        public int ID { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILENO { get; set; }
        public string USERNAME { get; set; }
        public string NRC { get; set; }
        public string EMAIL { get; set; }
        //public string ADDRESS { get; set; }
        public string CIFID { get; set; }
        public string CREATOR { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string APPROVER { get; set; }
        public DateTime? APPROVEDDATE { get; set; }
    }

    public class UserSearchViewModel
    {
        public string Name { get; set; }
        public string CIFID { get; set; }
        public string PhoneNo { get; set; }
        public string Branch { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
        public string FromDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
        public string ToDate { get; set; }
        public List<UserRPViewModel> userrpvms { get; set; }
    }
    #endregion

    #region topup
    public class TopUpViewModel
    {
        public string TRAN_ID { get; set; }
        public string FROM_ACCT { get; set; }
        public string TO_ACCT { get; set; }
        public string FROM_BRANCH { get; set; }
        public string TO_BRANCH { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_AMT { get; set; }
        public string DISCOUNT_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string PayeeNRC { get; set; }
        public string TRAN_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILENO { get; set; }
        public string ADDRESS { get; set; }
        public string PayerNRC { get; set; }
        public string USER_ID { get; set; }
        public string TransactionType { get; set; }
        public string FromChannel { get; set; }
        public string ToChannel { get; set; }
        public string TransactionIDFrom2C2P { get; set; }
        public string TransactionID { get; set; }
        public string NetAmount { get; set; }
        public string TotalAmount { get; set; }
        public string PartnerAmount { get; set; }
        public string DiscountAmount { get; set; }
        public string ChargeAmount { get; set; }
        public string ResponseCode { get; set; }
        public string RespDesc { get; set; }
        public DateTime? requestDatetime { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public string PaymentType { get; set; }
        public string Acctno { get; set; }
        public string Status { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string BillerCode { get; set; }
        public string BillPhone { get; set; }
        public string Deno { get; set; }
        public string PinCode { get; set; }

        //public string ReferenceID { get; set; }
        public string ThirdPartyID { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }
        public string CompanyName { get; set; }
    }

    public class TopUpSearch
    {
        public string BranchCode { get; set; }
        public string Utility_Type { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class TopUpSearchViewModel
    {
        public TopUpSearch RetailSearch { get; set; }
        public TopUpSearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<TopUpViewModel> TopUpsVM { get; set; }
    }
    #endregion

    #region Transaction
    public class TransactionViewModel
    {
        public string TRAN_ID { get; set; }
        public string FROM_ACCT { get; set; }
        public string TO_ACCT { get; set; }
        public string FROM_BRANCH { get; set; }
        public string TO_BRANCH { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_AMT { get; set; }
        public string DISCOUNT_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string TRAN_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string USERNAME { get; set; }
        public string MOBILENO { get; set; }
        public string ADDRESS { get; set; }
        public string PayeeName { get; set; }
        public string PayeeNRC { get; set; }

        public string PayerNRC { get; set; }
        public string CURRENCY { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }
        public string CompanyName { get; set; }

    }

    public class TransactionSearch
    {
        public string BranchCode { get; set; }
        public string TranType { get; set; }
        public string DebitOrCredit { get; set; }
        public string Currency { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class TransactionSearchViewModel
    {
        public TransactionSearch RetailSearch { get; set; }
        public TransactionSearch CorporateSearch { get; set; }
        public Boolean IsCorporate { get; set; }
        public List<TransactionViewModel> TransactionsVM { get; set; }
    }
    #endregion

    #region Utility
    public class UtilityViewModel
    {
        public string TRAN_ID { get; set; }
        public string FROM_ACCT { get; set; }
        //public string TO_ACCT { get; set; }
        public string FROM_BRANCH { get; set; }
        // public string TO_BRANCH { get; set; }
        // public string TRAN_AMT { get; set; }
        // public string CHARGE_CODE { get; set; }
        //public string CHARGE_AMT { get; set; }
        // string DISCOUNT_AMT { get; set; }
        // public string TOTAL_AMT { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string PayeeNRC { get; set; }
        public string TRAN_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILENO { get; set; }
        public string ADDRESS { get; set; }
        public string PayerNRC { get; set; }
        public string USER_ID { get; set; }
        public string TransactionType { get; set; }
        public string FromChannel { get; set; }
        public string ToChannel { get; set; }
        public string TransactionIDFrom2C2P { get; set; }
        public string TransactionID { get; set; }
        public string NetAmount { get; set; }
        public string TotalAmount { get; set; }
        public string PartnerAmount { get; set; }
        public string DiscountAmount { get; set; }
        public string ChargesAmount { get; set; }
        public string ResponseCode { get; set; }
        public string RespDesc { get; set; }
        public DateTime? requestDatetime { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public string PaymentType { get; set; }
        public string Acctno { get; set; }
        public string Status { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string BillerCode { get; set; }
        public string BillPhone { get; set; }
        public string Deno { get; set; }
        public string PinCode { get; set; }
        // public string ReferenceID { get; set; }
        public string Currency { get; set; }
        public string ThirdPartyID { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }
        public string CompanyName { get; set; }
    }

    public class UtilitySearch
    {
        public string BranchCode { get; set; }
        public string TranType { get; set; }
        public string DebitOrCredit { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string UtilityType { get; set; }
        public string Company_Name { get; set; }
        public string UserType { get; set; }
    }

    public class UtilitySearchViewModel
    {
        public UtilitySearch RetailSearch { get; set; }
        public UtilitySearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<UtilityViewModel> UtilityVM { get; set; }
    }
    #endregion

    #region Remittance
    public class RemittanceViewModel
    {
        public string TRAN_ID { get; set; }
        public string FROM_ACCT { get; set; }
        public string TO_ACCT { get; set; }
        public string FROM_BRANCH { get; set; }
        public string TO_BRANCH { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_AMT { get; set; }
        public string DISCOUNT_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string PayeeName { get; set; }
        public string PayeeNRC { get; set; }
        public string PayeePhone { get; set; }
        public string TRAN_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string ADDRESS { get; set; }
        public string PayerName { get; set; }
        public string PayerNRC { get; set; }
        public string USERNAME { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }
        public string CompanyName { get; set; }

    }

    public class RemittanceSearch
    {
        public string BranchCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class RemittanceSearchViewModel
    {
        public RemittanceSearch RetailSearch { get; set; }
        public RemittanceSearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<RemittanceViewModel> RemittancesVM { get; set; }
    }
    #endregion

    #region Transfer Own
    public class TransferOwnViewModel
    {
        public string TRAN_ID { get; set; }
        public string FROM_ACCT { get; set; }
        public string TO_ACCT { get; set; }
        public string FROM_BRANCH { get; set; }
        public string TO_BRANCH { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_AMT { get; set; }
        public string DISCOUNT_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string PayeeNRC { get; set; }
        public string TRAN_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILENO { get; set; }
        public string ADDRESS { get; set; }
        public string PayerNRC { get; set; }
    }

    public class TransferOwnSearchViewModel
    {
        public string BranchCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public List<TransferOwnViewModel> TransferOwnVM { get; set; }
    }
    #endregion

    #region Feedback
    public class CommentViewModel
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        [StringLength(20)]
        public string MOBILENO { get; set; }
        [StringLength(100)]
        public string REPORTTYPE { get; set; }
        public string comment { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string FULLNAME { get; set; }
        public string ADDRESS { get; set; }
    }

    public class CommentSearchViewModel
    {
        public string NAME { get; set; }
        public string REPORTTYPE { get; set; }
        public string comment { get; set; }
        //public string daterange { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public List<CommentViewModel> CommentVM { get; set; }
    }
    #endregion

    #region Transaction Summary
    public class TransactionSummaryViewModel
    {
        public string TRAN_TYPE { get; set; }
        //public DateTime? TRAN_DATE { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string TRAN_COUNT { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public string CompanyName { get; set; }
    }

    public class TransactionSummarySearch
    {
        public string Company_Name { get; set; }
        public string BranchCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }

    public class TransactionSummarySearchViewModel
    {
        public TransactionSummarySearch RetailSearch { get; set; }
        public TransactionSummarySearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<TransactionSummaryViewModel> TransactionSummaryVM { get; set; }
    }
    #endregion

    #region Utility Summary 
    public class UtilitySummaryViewModel
    {
        public string TRAN_TYPE { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string BILLER_CODE { get; set; }
        public string TRAN_COUNT { get; set; }
        public Decimal TRAN_AMT { get; set; }
        public Decimal COMMISION_AMT { get; set; }
        public Decimal PARTNER_AMT { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }
    }

    public class UtilitySummarySearch
    {
        public string BranchCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string COMPANY_NAME { get; set; }
    }

    public class UtilitySummarySearchViewModel
    {
        public UtilitySummarySearch RetailSearch { get; set; }
        public UtilitySummarySearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<UtilitySummaryViewModel> UtilitySummaryVM { get; set; }
    }
    #endregion

    #region Admin Log
    public class AdminLogViewModel
    {
        public int ID { get; set; }
        public string USERNAME { get; set; }
        public DateTime? LOG_DATETIME { get; set; }
        public string CONTROLLER { get; set; }
        public string ACTION { get; set; }
        public string LOG_TYPE { get; set; }
        public string DESCRIPTION { get; set; }
        public string DETAIL { get; set; }
        public string LINE_NO { get; set; }
        public string REQUEST_DATA { get; set; }
    }

    public class AdminLogSearchViewModel
    {
        public string Username { get; set; }
        public string Log_Type { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<AdminLogViewModel> adminlogVM { get; set; }
    }

    #endregion

    #region API Log/User Activity Log
    public class ApiLogViewModel
    {
        public int ID { get; set; }
        public string REQUESTMETHOD { get; set; }
        public string REQUESTPATH { get; set; }
        //public string REQUESTDATA { get; set; }
        public string REQUESTCARDID { get; set; }
        public DateTime REQUESTDATETIME { get; set; }
        public string REQUESTFILEPATH { get; set; }
        public string LOGMESSAGE { get; set; }
        //public string RESPONSEDATA { get; set; }
        public DateTime RESPONSEDATETIME { get; set; }
        public string RESPONSECODE { get; set; }
        public string RESPONSEDESCRIPTION { get; set; }
        public string USERTYPE { get; set; }
    }

    public class ApiLogSearchViewModel
    {
        public string RequestMethod { get; set; }
        public string Username { get; set; }
        public string RequestPath { get; set; }
        public string UserType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<ApiLogViewModel> apilogVM { get; set; }
    }
    #endregion

    #region Cron Log
    public class CronLogViewModel
    {
        public int ID { get; set; }
        public string TASKNAME { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public string RESPCODE { get; set; }
        public string RESPDESC { get; set; }
        public string LOGMESSAGE { get; set; }
    }

    public class CronLogSearchViewModel
    {
        public string TaskName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<CronLogViewModel> cronlogVM { get; set; }
    }
    #endregion

    #region Schedule Transfer
    public class ScheduleTransferViewModel
    {
        public int ID { get; set; }
        [StringLength(128)]
        public string USERID { get; set; }
        [StringLength(50)]
        public string SESSIONID { get; set; }
        [StringLength(200)]
        public string FROM_ACC_NAME { get; set; }
        [StringLength(50)]
        public string FROM_ACC_NO { get; set; }
        [StringLength(200)]
        public string TO_ACC_NAME { get; set; }
        [StringLength(50)]
        public string TO_ACC_NO { get; set; }
        public decimal AMOUNT { get; set; }
        [StringLength(128)]
        public string TRANID { get; set; }
        public DateTime? TRANSFER_DATE { get; set; }
        [StringLength(50)]
        public string TRANSFER_STATUS { get; set; }
        [StringLength(500)]
        public string DESCRIPTION { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        [StringLength(128)]
        public string CREATEDUSERID { get; set; }
        [StringLength(128)]
        public DateTime? UPDATEDDATE { get; set; }
        public string UPDATEDUSERID { get; set; }
        public bool ACTIVE { get; set; }
        public decimal CHARGES { get; set; }
        public string CompanyName { get; set; }
    }

    public class ScheduleTransferSearch
    {
        public string Username { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class ScheduleTransferSearchViewModel
    {
        public ScheduleTransferSearch RetailSearch { get; set; }
        public ScheduleTransferSearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<ScheduleTransferViewModel> schtranfVM { get; set; }
    }

    //public class ScheduleTransferSearchViewModel
    //{
    //    public string Username { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //}
    #endregion

    #region Schedule Transfer Log
    public class ScheduleTransferLogViewModel
    {
        public int ID { get; set; }
        public int SCHEDULETRANSFERID { get; set; }
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

    public class ScheduleTransferLogSearchViewModel
    {
        public string Taskname { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<ScheduleTransferLogViewModel> schtransflogVM { get; set; }
    }
    #endregion

    #region SchedulePaymentDetail
    public class SchedulePaymentDetailVM
    {
        public int ID { get; set; }
        public string VALUE_DATE { get; set; }
        public string ScheduleDate { get; set; }
        public DateTime? TRAN_DATE { get; set; }
        public string ScheduleID { get; set; }
        public string TranID { get; set; }
        public string FromAccNo { get; set; }
        public string ToAccNo { get; set; }
        public string USERNAME { get; set; }
        public string PayerName { get; set; }
        public string PayerNRC { get; set; }
        public string PayeeName { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGE_AMT { get; set; }
        public string TOTAL_AMT { get; set; }
        public string Status { get; set; }
        public DateTime VALUE_DATE_DT { get; set; }  
        public string CompanyName { get; set; }

    }
    public class SchedulePaymentDetailSearch
    {
        public string Branch { get; set; }
        public string Transaction_Status { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }
    public class SchedulePaymentDetailSearchVM
    {
        public SchedulePaymentDetailSearch RetailSearch { get; set; }
        public SchedulePaymentDetailSearch CorporateSearch { get; set; }     
        public Boolean IsCorporate { get; set; }
        public List<SchedulePaymentDetailVM> schschedulePDVM { get; set; }
    }
    #endregion

    #region Domestic Interbank Transfer
    public class DomesticInterbankTransferViewModel
    {
        public string ID { get; set; }
        [Display(Name = "VALUE DATE")]
        public DateTime? VALUE_DATE { get; set; }
        [Display(Name = "TRANSACTION DATE")]
        public DateTime? TRAN_DATE { get; set; }
        [Display(Name = "TRANSACTION ID")]
        public string TRAN_ID { get; set; }
        [Display(Name = "TRANSACTION TYPE")]
        public string TRAN_TYPE { get; set; }
        public string USERNAME { get; set; }
        [Display(Name = "FROM ACCOUNT")]
        public string FROM_ACCT { get; set; }
        public string PayerName { get; set; }
        [Display(Name = "PAYEE NAME")]
        public string PayeeName { get; set; }
        public string Payee_ACC { get; set; }
        [Display(Name = "TO ACCOUNT")]
        public string TO_ACCT { get; set; }
        [Display(Name = "OFFICE ACCOUNT")]
        public string OFFICE_ACCT { get; set; }
        [Display(Name = "PAYEE PHONE")]
        public string PAYEE_PHONE { get; set; }
        public string PayeeNRC { get; set; }
        [Display(Name = "TO BANK")]
        public string TO_BANK { get; set; }
        [Display(Name = "FROM BRANCH")]
        public string FROM_BRANCH { get; set; }
        [Display(Name = "TO BRANCH")]
        public string BRANCH { get; set; }
        [Display(Name = "TRANSACTION AMOUNT")]
        public string AMOUNT { get; set; }
        [Display(Name = "CHARGE CODE")]
        public string CHARGE_CODE { get; set; }
        [Display(Name = "CHARGE AMOUNT")]
        public string CHARGES { get; set; }
        [Display(Name = "TOTAL AMOUNT")]
        public string TOTAL { get; set; }
        public int? OTHER_BANK_BENE_ID { get; set; }
        [Display(Name = "STATUS")]
        public string Core_status { get; set; }
        public string CompanyName { get; set; }
        public string CBMTranId { get; set; }
        public string CBMStatus { get; set; }
        [Display(Name ="CBM TRANSACTION TYPE")]
        public string CBMTran_Type { get; set; }
    }

    public class DomesticInterbankTransferSearch
    {
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string Tran_Type { get; set; }
        public string Tran_Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
        public string CBMStatus { get; set; }
        public string CBMTranType { get; set; }
    }

    public class DomesticInterbankTransferSearchViewModel
    {
        public DomesticInterbankTransferSearch RetailSearch { get; set; }
        public DomesticInterbankTransferSearch CorporateSearch { get; set; }
        public bool IsCorporate { get; set; }
        public List<DomesticInterbankTransferViewModel> domintertranVM { get; set; }
    }

    //public class DomesticInterbankTransferSearchViewModel
    //{
    //    public string BankName { get; set; }
    //    public string BranchName { get; set; }
    //    public string Tran_Type { get; set; }
    //    public string Tran_Status { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //    public List<DomesticInterbankTransferViewModel> domintertranVM { get; set; }
    //}
    #endregion

    #region Corporate 
    public class CorporateViewModel
    {
        public string ID { get; set; }
        [Display(Name ="CREATED DATE")]
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name ="APPROVED DATE")]
        public DateTime? APPROVEDDATE { get; set; }
        public string CIFID { get; set; }
        [Display(Name ="CORPORATE ID")]
        public string CORPORATEID { get; set; }
        [Display(Name ="COMPANY NAME")]
        public string COMPANY_NAME { get; set; }
        [Display(Name ="PHONE NO.")]
        public string PHONE_NO { get; set; }
        public string EMAIL { get; set; }
        [Display(Name ="CORPORATE TYPE")]
        public bool CORPORATE_TYPE { get; set; }
        [Display(Name ="TRANSACTION LIMIT")]
        public string TRAN_LIMIT { get; set; }
        [Display(Name ="BULK CHARGES TYPE")]
        public bool BULK_CHARGES_TYPE { get; set; }
        [Display(Name ="BULK CHARGES RATE")]
        public string BULK_CHARGES_FIX_RATE { get; set; }
        public string MAKER { get; set; }
        public string CHECKER { get; set; }
    }

    public class CorporateSearchViewModel
    {
        public string Corporate_Id { get; set; }
        public string Company_Name { get; set; }
        public string CIFID { get; set; }
        public string Corporate_Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<CorporateViewModel> corporateVM { get; set; }
    }
    #endregion

    #region Corporate User
    public class CorporateUserViewModel
    {
        public int ID { get; set; }
        [Display(Name ="CREATED DATE")]
        public DateTime? CREATEDDATE { get; set; }
        [Display(Name ="APPROVED DATE")]
        public DateTime? APPROVEDDATE { get; set; }
        [Display(Name ="COMPANY NAME")]
        public string COMPANY_NAME { get; set; }
        [Display(Name ="CORPORATE ID")]
        public string CORPORATEID { get; set; }
        public string USERNAME { get; set; }
        [Display(Name ="PHONE NO.")]
        public string PHONE_NO { get; set; }
        public string EMAIL { get; set; }
        public string ROLE { get; set; }
        [Display(Name ="DEPARTMENT")]
        public string DEPARTMENT { get; set; }
        public string POSITION { get; set; }
    }

    public class CorporateUserSearchViewModel
    {
        public string Company_Name { get; set; }
        public string Corporate_Id { get; set; }
        public string Username { get; set; }
        //public string Position { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<CorporateUserViewModel> coruserVM { get; set; }
    }
    #endregion

    #region Bulk Payment
    public class BulkPaymentViewModel
    {
        public int ID { get; set; }
        [Display(Name = "TRANSACTION DATE")]
        public DateTime? TRAN_DATE { get; set; }
        [Display(Name = "VALUE DATE")]
        public DateTime? VALUE_DATE { get; set; }
        [Display(Name ="COMPANY NAME")]
        public string COMPANY_NAME { get; set; }
        [Display(Name ="CORPORATE ID")]
        public string CORPORATEID { get; set; }
        [Display(Name = "DEBIT ACCOUNT")]
        public string FROM_ACCT { get; set; }
        [Display(Name ="DEBIT AMOUNT")]
        public decimal? DEBIT_AMT { get; set; }
        [Display(Name = "TRANSACTION AMOUNT")]
        public decimal? AMOUNT { get; set; }
        public string STATUS { get; set; }
        [Display(Name ="FAIL COUNT")]
        public int FAIL_COUNT { get; set; }
        public string TRAN_TYPE { get; set; }
        public decimal? CHARGES_AMT { get; set; }
        public string BulkFileName { get; set; }
        public decimal? MMKAMOUNT { get; set; }

    }

    public class BulkPaymentSearchViewModel
    {
        public string Company_Name { get; set; }
        public string Corporate_ID { get; set; }
        public string Debit_Acct { get; set; }
        public string Debit_Amt { get; set; }
        public string Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TranType { get; set; }
        public string BulkFileName { get; set; }
        public List<BulkPaymentViewModel> bulkpaymentVM { get; set; }
    }
    #endregion

    #region Bulk Payment Detail
    public class BulkPaymentDetailViewModel
    {
        public int ID { get; set; }
        public string CORPORATEID { get; set; }
        public string TRAN_DATE { get; set; }
        public string VALUE_DATE { get; set; }
        public string TRAN_ID { get; set; }
        public string FROM_ACC { get; set; }
        public string TO_ACC { get; set; }
        public string TO_BANK { get; set; }
        public string BRANCH { get; set; }
        public string COMPANY_NAME { get; set; }
        public string TRAN_AMT { get; set; }
        public string CHARGES_AMT { get; set; }
        public string CBMTRAN_ID { get; set; }
        public string CBM_STATUS { get; set; }
        public string STATUS { get; set; }
        public string BULK_FILE_NAME { get; set; }
        public string TRAN_TYPE { get; set; }
    }

    public class BulkPaymentDetailSearchViewModel
    {
        public string Company_Name { get; set; }
        public string Corporate_ID { get; set; }
        public string Debit_Acct { get; set; }
        public string Debit_Amt { get; set; }
        public string Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TranType { get; set; }
        public string BulkFileName { get; set; }
        public string CBMStatus { get; set; }
        public List<BulkPaymentDetailViewModel> bulkpaymentdetailVM { get; set; }
    }
    #endregion

    #region CIFID Information
    public class CIFIDInfoRPViewModel
    {
        public int ID { get; set; }
        public string CIFID { get; set; }
        public string NAME { get; set; }
        public string USERTYPE { get; set; }
        public string USERTYPECODE { get; set; }
        public string PHONENO { get; set; }
        public string NRC { get; set; }
        public string ADDRESS { get; set; }
        public string REMARK { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public DateTime? EXPIREDATE { get; set; }
        public DateTime? EFFECTIVEDATE { get; set; }
        [Display(Name = "RM ID")]
        public string RMID { get; set; }
        [Display(Name = "RM Name")]
        public string RMName { get; set; }
    }
    public class CIFIDInfoSearchViewModel
    {
        public string CIFID { get; set; }
        public string NAME { get; set; }
        public string USERTYPE { get; set; }
        public string USERTYPECODE { get; set; }
        [Display(Name = "THE ICONIC Number")]
        public string ICONICCODE { get; set; }
        public string NRC { get; set; }
        public string CREATEDDATE { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpireDate { get; set; }
        public List<CIFIDInfoRPViewModel> CIFIDInfoRPVM { get; set; }
    }

    //public class CIFAccountInfoViewModel
    //{
    //    public string CIFID { get; set; }
    //    public string ACCOUNTNO { get; set; }
    //    public string SCHM_TYPE { get; set; }
    //    public string SCHM_CODE { get; set; }
    //    public string ACC_DESC { get; set; }
    //    public decimal AVAI_BALANCE { get; set; }
    //    public string CURRENCY { get; set; }
    //    public string BRANCHCODE { get; set; }
    //    public string CreatedUserID { get; set; }
    //    public DateTime? CreatedDateTime { get; set; }
    //    public string UpdatedUserId { get; set; }
    //    public DateTime? UpdatedDateTime { get; set; }
    //    public Boolean ACCT_CLOSE_FLAG { get; set; }
    //}
    public class IconicDetailAccVM
    {
        
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal AvailableAmt { get; set; }
        public string Currency { get; set; }
        
        
    }
    public class CIFAccountInfoSearchViewModel
    {
        public string CIFID { get; set; }
        public string ICONICNUMBER { get; set; }
        public List<CIFIDInfoRPViewModel> CIFIDInfoRPVM { get; set; }
        public List<IconicDetailAccVM> CIFIDAccInfoRPVM { get; set; }
    }

    #endregion
}