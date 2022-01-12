using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    public class CCTController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        string controllerName = "CCTController";
        EventLogController log = new EventLogController();
        private string ConString { get { return ConstantValues.Connection_String; } }
        string CSVFile_Path = ConstantValues.CSVFile_Path;
        string CSVBulkFile_Path = ConstantValues.CSVBulkFile_Path;

        #region PendingList
        // GET: CCT
        public ActionResult PendingList()
        {
            bindBankList();
            BindCBMTran_Type();
            ViewBag.IsBulkFile = TempData["IsBulkFile"];
            return View();
        }
        [ActionName("PendingList")]
        [HttpPost]
        public ActionResult PendingLists()
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var username = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var fromacct = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var toacct = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var bank = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var cbmtrantype= Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            var Iscorporate = 2;
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            string status = "Pending";

            //if (!String.IsNullOrEmpty(fromdate))
            //{
            //    fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            //}
            //else
            //{
            //    fromdate = DateTime.Now.ToString("dd-MM-yyyy");
            //}
            //if (!String.IsNullOrEmpty(todate))
            //{
            //    toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
            //}
            //else
            //{
            //    todate = DateTime.Now.ToString("dd-MM-yyyy");
            //}

            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                fromdate = fromDate.ToString("dd-MM-yyyy");
            }
            else
            {
                fromdate = DateTime.Now.ToString("dd-MM-yyyy");
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
                todate = toDate.ToString("dd-MM-yyyy");
            }
            else
            {
                todate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            }


            //#region validation
            ////check toDate is greater than fromDate or not
            //if (toDate < fromDate)
            //{
            //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
            //    bindBankList();
            //    return View();
            //}
            //#endregion

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            //var otherbank_tranlog = (from otl in db.OtherBankTranLogs
            //                         join u in db._Users on otl.USERID equals u.USERID into tbluser
            //                         from tblrp in tbluser.DefaultIfEmpty()
            //                         where otl.Core_status == "Pending"
            //                         select otl).ToList();

            DataTable dtResult = GetOtherBankTranLog(username, fromacct, toacct, bank, status, fromdate, todate,cbmtrantype,Iscorporate);
            List<DomesticInterbankTransferViewModel> otherbank_tranlog = (from DataRow dr in dtResult.Rows
                                                                          select new DomesticInterbankTransferViewModel()
                                                                          {
                                                                              ID = dr["ID"].ToString(),
                                                                              VALUE_DATE = DateTime.ParseExact(dr["VALUE_DATE"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                                              //TRAN_DATE = DateTime.ParseExact(dr["TRAN_DATE"].ToString().Trim(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                                                                              TRAN_ID = dr["TRAN_ID"].ToString(),
                                                                              USERNAME = dr["USERNAME"].ToString(),
                                                                              FROM_ACCT = dr["FROM_ACCT"].ToString(),
                                                                              //PayerName = dr["FULLNAME"].ToString(),
                                                                              PayeeName = dr["PAYEE_NAME"].ToString(),
                                                                              //PAYEE_PHONE = dr["PAYEE_PHONE"].ToString(),
                                                                              TO_ACCT = dr["TO_ACCT"].ToString(),
                                                                              TO_BANK = dr["TO_BANK"].ToString(),
                                                                              BRANCH = dr["TO_BRANCH"].ToString(),
                                                                              AMOUNT = dr["TRAN_AMT"].ToString(),
                                                                              //CHARGES = dr["CHARGE_AMT"].ToString(),
                                                                              //TOTAL = dr["TOTAL_AMT"].ToString()
                                                                              CBMTran_Type= dr["CBMTran_Type"].ToString()
                                                                          }).ToList();

            // SEARCHING...

            if (!string.IsNullOrEmpty(username))
            {
                otherbank_tranlog = otherbank_tranlog.Where(a => a.USERNAME.ToLower().Trim().Contains(username.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(fromacct))
            {
                otherbank_tranlog = otherbank_tranlog.Where(a => a.FROM_ACCT.ToLower().Trim().Contains(fromacct.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(toacct))
            {
                otherbank_tranlog = otherbank_tranlog.Where(a => a.TO_ACCT.ToLower().Trim().Contains(toacct.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(bank))
            {
                otherbank_tranlog = otherbank_tranlog.Where(a => a.TO_BANK.ToLower().Trim().Contains(bank.ToLower().Trim())).ToList();
            }
            //if (fromDate != null & toDate != null)
            //{
            //    otherbank_tranlog = otherbank_tranlog.Where(a => a.TRAN_DATE >= fromDate && a.TRAN_DATE < toDate).ToList();
            //}
            //else if (fromDate != null)
            //{
            //    otherbank_tranlog = otherbank_tranlog.Where(a => a.TRAN_DATE >= fromDate).ToList();
            //}
            //else if (toDate != null)
            //{
            //    otherbank_tranlog = otherbank_tranlog.Where(a => a.TRAN_DATE < toDate).ToList();
            //}

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                otherbank_tranlog = otherbank_tranlog.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = otherbank_tranlog.Count();
            List<DomesticInterbankTransferViewModel> data;
            if (pageSize == -1)
            {
                data = otherbank_tranlog.ToList();
            }
            else
            {
                data = otherbank_tranlog.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        //Get Details CCT Pending
        public ActionResult Details(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OtherBankTranLog obtranlog = db.OtherBankTranLogs.Find(id);
                User user = db._Users.Where(u => u.USERID == obtranlog.USERID).FirstOrDefault();
                Branch branch = db.Branches.Where(b => b.code == obtranlog.FROM_BRANCH).FirstOrDefault();
                //var result = from o in db.OtherBankTranLogs
                //             join d in db._Users on o.USERID equals d.USERID into tbluser
                //             from tblname in tbluser.DefaultIfEmpty()
                //             where o.ID == id
                //             select o;
                DomesticInterbankTransferViewModel dibtranfer = new DomesticInterbankTransferViewModel
                {
                    VALUE_DATE = DateTime.ParseExact(obtranlog.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TRAN_ID = obtranlog.TRAN_ID,
                    FROM_ACCT = obtranlog.FROM_ACCT,
                    TO_ACCT = obtranlog.TO_ACCT,
                    OFFICE_ACCT = obtranlog.OFFICE_ACCT,
                    FROM_BRANCH = branch.name,
                    TO_BANK = obtranlog.TO_BANK,
                    BRANCH = obtranlog.TO_BRANCH,
                    AMOUNT = obtranlog.TRAN_AMT.ToString(),
                    CHARGE_CODE = obtranlog.CHARGE_CODE,
                    CHARGES = obtranlog.CHARGE_AMT.ToString(),
                    TOTAL = obtranlog.TOTAL_AMT.ToString(),
                    TRAN_DATE = DateTime.ParseExact(obtranlog.TRAN_DATE.ToString().Trim(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                    USERNAME = user.USERNAME,
                    TRAN_TYPE = obtranlog.TRAN_TYPE,
                    PayeeName = obtranlog.PAYEE_NAME,
                    PAYEE_PHONE = obtranlog.PAYEE_PHONE,
                    OTHER_BANK_BENE_ID = obtranlog.OTHER_BANK_BENE_ID,
                    Core_status = obtranlog.Core_status,
                    CBMTran_Type=obtranlog.CBMTran_Type
                };
                if (obtranlog == null)
                {
                    return HttpNotFound();
                }
                return View(dibtranfer);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }        

        #region bind bank list ddl
        private void bindBankList()
        {
            //create Bank List
            IEnumerable<SelectListItem> items = db.OtherBanks.Where(b => b.Active == true).Select(b => new SelectListItem
            {
                Value = b.BANK_NAME,
                Text = b.BANK_NAME

            });
            ViewBag.BankList = items;
        }
        #endregion
        private EmailTemplate Template(string Type)
        {
            var template = db.EmailTemplates.Where(t => t.Type == Type).FirstOrDefault();
            return template;
        }
        //For Retail Transfer Email
        private SMS_Allow EmailAllow(string type)
        {
            var emailallow = db.SMS_Allows.Where(a => a.TransactionType == type).FirstOrDefault();
            return emailallow;
        }


        //For Retail Transfer
        private Boolean RetailTransfer(string fullname, string email, OtherBankTranLog obTranlog, string RecievedEmail, string ReceivedName, string Desc, bool Sender_Allow, bool Receiver_Allow, bool Sender_Email_Allow, bool Receiver_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                DateTime TranDate = DateTime.ParseExact(obTranlog.TRAN_DATE.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                string trandate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                getuserid = User.Identity.GetUserId();
                if (!String.IsNullOrEmpty(RecievedEmail) && IsEmailValid(RecievedEmail) && Receiver_Email_Allow == true)
                {
                    string CreditType = "SUCCESS CCT_Retail_TranCR";
                    string FromBankName = "A Bank";
                    string fromAcc = obTranlog.FROM_ACCT;
                    //Sent Mail For Transfer Credit
                    EmailTemplate tempCredit = Template(CreditType);
                    var CreditSubject = tempCredit.Subject;
                    var CreditBody = tempCredit.Body;
                    CreditBody = CreditBody.Replace("[ReceiverName]", (ReceivedName));
                    CreditBody = CreditBody.Replace("[TranDate]", (trandate));
                    CreditBody = CreditBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    CreditBody = CreditBody.Replace("[FromBankName]", FromBankName);
                    CreditBody = CreditBody.Replace("[SenderName]", (fullname));
                    CreditBody = CreditBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    CreditBody = CreditBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    CreditBody = CreditBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    CreditBody = CreditBody.Replace("[ToAccount]", (obTranlog.TO_ACCT));
                    CreditBody = CreditBody.Replace("[Amount]", (obTranlog.TRAN_AMT.ToString()));
                    try
                    {
                        bool sentmailForCredit = General.SendEmailForCCT(RecievedEmail, CreditSubject, CreditBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {ReceivedName}.Email Address is {RecievedEmail}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                //Sent Mail For Transfer Debit
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "SUCCESS CCT_Retail_TranDB";
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[SenderName]", fullname);
                    DebitBody = DebitBody.Replace("[TranDate]", trandate);
                    DebitBody = DebitBody.Replace("[TranNumber]", obTranlog.TRAN_ID);
                    DebitBody = DebitBody.Replace("[FromAccount]", obTranlog.FROM_ACCT);
                    DebitBody = DebitBody.Replace("[ToBankName]", obTranlog.TO_BANK);
                    DebitBody = DebitBody.Replace("[ToBranchName]", obTranlog.TO_BRANCH);
                    DebitBody = DebitBody.Replace("[ToAccount]", obTranlog.TO_ACCT);
                    DebitBody = DebitBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    DebitBody = DebitBody.Replace("[Amount]", obTranlog.TRAN_AMT.ToString());
                    DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {fullname}.Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }

        //For Retail Remittance
        private Boolean RetailRemittance(string fullname, string email, OtherBankTranLog obTranlog, string RecievedEmail, string ReceivedName, string Desc, bool Sender_Allow, bool Receiver_Allow, bool Sender_Email_Allow, bool Receiver_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                DateTime TranDate = DateTime.ParseExact(obTranlog.TRAN_DATE.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                string trandate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                getuserid = User.Identity.GetUserId();
                if (!String.IsNullOrEmpty(RecievedEmail) && IsEmailValid(RecievedEmail) && Receiver_Email_Allow == true)
                {
                    string CreditType = "SUCCESS CCT_Retail_RemitCR";
                    string FromBankName = "A Bank";
                    //Sent Mail For Transfer Credit
                    EmailTemplate tempCredit = Template(CreditType);
                    var CreditSubject = tempCredit.Subject;
                    var CreditBody = tempCredit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    CreditBody = CreditBody.Replace("[ReceiverName]", (ReceivedName));
                    CreditBody = CreditBody.Replace("[TranDate]", (trandate));
                    CreditBody = CreditBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    CreditBody = CreditBody.Replace("[FromBankName]", FromBankName);
                    CreditBody = CreditBody.Replace("[SenderName]", (fullname));
                    CreditBody = CreditBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    CreditBody = CreditBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    CreditBody = CreditBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    CreditBody = CreditBody.Replace("[NRC]", (obTranlog.NRC));
                    CreditBody = CreditBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    CreditBody = CreditBody.Replace("[ToAccount]", (obTranlog.TO_ACCT));
                    CreditBody = CreditBody.Replace("[Amount]", (obTranlog.TRAN_AMT.ToString()));
                    CreditBody = CreditBody.Replace("[Currency]", "MMK");
                    try
                    {
                        bool sentmailForCredit = General.SendEmailForCCT(RecievedEmail, CreditSubject, CreditBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {ReceivedName}.Email Address is {RecievedEmail}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "SUCCESS CCT_Retail_RemitDB";
                    //Sent Mail For Transfer Debit
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[SenderName]", (fullname));
                    DebitBody = DebitBody.Replace("[TranDate]", (trandate));
                    DebitBody = DebitBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    DebitBody = DebitBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    DebitBody = DebitBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    DebitBody = DebitBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    DebitBody = DebitBody.Replace("[NRC]", (obTranlog.NRC));
                    DebitBody = DebitBody.Replace("[PhoneNo]", (obTranlog.PAYEE_PHONE));
                    DebitBody = DebitBody.Replace("[BodyReceivedName]", (BodyReceivedName));
                    DebitBody = DebitBody.Replace("[Amount]", (obTranlog.TRAN_AMT.ToString()));
                    DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {fullname}.Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }

        //For Coroperate Transfer
        private Boolean CorporateTransfer(string fullname, string email, OtherBankTranLog obTranlog, string RecievedEmail, string ReceivedName, string Desc, bool Sender_Allow, bool Receiver_Allow, bool Sender_Email_Allow, bool Receiver_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                DateTime TranDate = DateTime.ParseExact(obTranlog.TRAN_DATE.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                string trandate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                getuserid = User.Identity.GetUserId();
                if (!String.IsNullOrEmpty(RecievedEmail) && IsEmailValid(RecievedEmail) && Receiver_Email_Allow == true)
                {
                    string CreditType = "SUCCESS CCT_Co_TranCR";
                    string FromBankName = "A Bank";
                    //Sent Mail For Transfer Credit
                    EmailTemplate tempCredit = Template(CreditType);
                    var CreditSubject = tempCredit.Subject;
                    var CreditBody = tempCredit.Body;

                    CreditBody = CreditBody.Replace("[ReceiverName]", (ReceivedName));
                    CreditBody = CreditBody.Replace("[TranDate]", (trandate));
                    CreditBody = CreditBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    CreditBody = CreditBody.Replace("[FromBankName]", FromBankName);
                    CreditBody = CreditBody.Replace("[SenderName]", (fullname));
                    CreditBody = CreditBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    CreditBody = CreditBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    CreditBody = CreditBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    CreditBody = CreditBody.Replace("[ToAccount]", (obTranlog.TO_ACCT));
                    CreditBody = CreditBody.Replace("[Amount]", (obTranlog.TRAN_AMT.ToString()));
                    CreditBody = CreditBody.Replace("[Currency]", "MMK");
                    try
                    {
                        bool sentmailForCredit = General.SendEmailForCCT(RecievedEmail, CreditSubject, CreditBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {ReceivedName}.Email Address is {RecievedEmail}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                //Sent Mail For Transfer Debit
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "SUCCESS CCT_Co_TranDB";
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[SenderName]", fullname);
                    DebitBody = DebitBody.Replace("[TranDate]", trandate);
                    DebitBody = DebitBody.Replace("[TranNumber]", obTranlog.TRAN_ID);
                    DebitBody = DebitBody.Replace("[FromAccount]", obTranlog.FROM_ACCT);
                    DebitBody = DebitBody.Replace("[ToBankName]", obTranlog.TO_BANK);
                    DebitBody = DebitBody.Replace("[ToBranchName]", obTranlog.TO_BRANCH);
                    DebitBody = DebitBody.Replace("[ToAccount]", obTranlog.TO_ACCT);
                    DebitBody = DebitBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    DebitBody = DebitBody.Replace("[Amount]", obTranlog.TRAN_AMT.ToString());
                    DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {fullname}.Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }
        //For Corporate Remittance
        private Boolean CorporateRemittance(string fullname, string email, OtherBankTranLog obTranlog, string RecievedEmail, string ReceivedName, string Desc, bool Sender_Allow, bool Receiver_Allow, bool Sender_Email_Allow, bool Receiver_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                DateTime TranDate = DateTime.ParseExact(obTranlog.TRAN_DATE.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                string trandate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                getuserid = User.Identity.GetUserId();
                if (!String.IsNullOrEmpty(RecievedEmail) && IsEmailValid(RecievedEmail) && Receiver_Email_Allow == true)
                {
                    string CreditType = "SUCCESS CCT_Co_RemitCR";
                    string FromBankName = "A Bank";
                    //Sent Mail For Transfer Credit
                    EmailTemplate tempCredit = Template(CreditType);
                    var CreditSubject = tempCredit.Subject;
                    var CreditBody = tempCredit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    CreditBody = CreditBody.Replace("[ReceiverName]", (ReceivedName));
                    CreditBody = CreditBody.Replace("[TranDate]", (trandate));
                    CreditBody = CreditBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    CreditBody = CreditBody.Replace("[FromBankName]", FromBankName);
                    CreditBody = CreditBody.Replace("[SenderName]", (fullname));
                    CreditBody = CreditBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    CreditBody = CreditBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    CreditBody = CreditBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    CreditBody = CreditBody.Replace("[NRC]", (obTranlog.NRC));
                    CreditBody = CreditBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    CreditBody = CreditBody.Replace("[Amount]", obTranlog.TRAN_AMT.ToString());
                    CreditBody = CreditBody.Replace("[Currency]", "MMK");
                    try
                    {
                        bool sentmailForCredit = General.SendEmailForCCT(RecievedEmail, CreditSubject, CreditBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {ReceivedName}.Email Address is {RecievedEmail}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "SUCCESS CCT_Co_RemitDB";
                    //Sent Mail For Transfer Debit
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[SenderName]", (fullname));
                    DebitBody = DebitBody.Replace("[TranDate]", (trandate));
                    DebitBody = DebitBody.Replace("[TranNumber]", (obTranlog.TRAN_ID));
                    DebitBody = DebitBody.Replace("[FromAccount]", (obTranlog.FROM_ACCT));
                    DebitBody = DebitBody.Replace("[ToBankName]", (obTranlog.TO_BANK));
                    DebitBody = DebitBody.Replace("[ToBranchName]", (obTranlog.TO_BRANCH));
                    DebitBody = DebitBody.Replace("[NRC]", (obTranlog.NRC));
                    DebitBody = DebitBody.Replace("[PhoneNo]", (obTranlog.PAYEE_PHONE));
                    DebitBody = DebitBody.Replace("[BodyReceivedName]", (BodyReceivedName));
                    DebitBody = DebitBody.Replace("[Amount]", (obTranlog.TRAN_AMT.ToString()));
                    DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {fullname}.Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }
      
        //For Coroperate Transfer For BulkFile Fail
        private Boolean CorporateBulkTransferSuccess(string fullname, string email, string trandate, string username /*, string valuedate*/, /*string Desc,*/ bool Sender_Allow, bool Sender_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                string strTranDate = "";
                if (!string.IsNullOrWhiteSpace(trandate))
                {
                    //DateTime TranDate = DateTime.ParseExact(trandate.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                    DateTime TranDate = DateTime.ParseExact(trandate.Substring(0,10).Replace('/','-'), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    strTranDate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                }

                getuserid = User.Identity.GetUserId();
                //Sent Mail For Transfer Debit
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "BulkPaymentCorTransaction_Success";
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    //string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[FULLNAME]", fullname);
                    DebitBody = DebitBody.Replace("[TranDate]", trandate);
                    DebitBody = DebitBody.Replace("[USERNAME]", username);
                    //DebitBody = DebitBody.Replace("[TranNumber]", obTranlog.TRAN_ID);
                    //DebitBody = DebitBody.Replace("[FromAccount]", obTranlog.FROM_ACCT);
                    //DebitBody = DebitBody.Replace("[ToBankName]", obTranlog.TO_BANK);
                    //DebitBody = DebitBody.Replace("[ToBranchName]", obTranlog.TO_BRANCH);
                    //DebitBody = DebitBody.Replace("[ToAccount]", obTranlog.TO_ACCT);
                    ////DebitBody = DebitBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    //DebitBody = DebitBody.Replace("[Amount]", obTranlog.TRAN_AMT.ToString());
                    //DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    //DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    //DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    //DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"FullName is {fullname}.Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }
        private Boolean CorporateBulkTransferFail(string email, string fullname,  string username , /*, string trandate,, string valuedate, /*string Desc,*/ bool Sender_Allow, bool Sender_Email_Allow)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string getuserid = "";
            try
            {
                General.SMTPSetting setting = GetEmailSetting();
                //string strTranDate = "";
                //if (!string.IsNullOrWhiteSpace(trandate))
                //{
                //    //DateTime TranDate = DateTime.ParseExact(trandate.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                //    DateTime TranDate = DateTime.ParseExact(trandate.Substring(0, 10).Replace('/', '-'), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    strTranDate = TranDate.ToString("dd/MM/yyyy  HH:mm:ss");
                //}

                getuserid = User.Identity.GetUserId();
                //Sent Mail For Transfer Debit
                if (!String.IsNullOrEmpty(email) && IsEmailValid(email) && Sender_Email_Allow == true)
                {
                    string DebitType = "BulkPaymentCorTransaction_Fail";
                    EmailTemplate tempDebit = Template(DebitType);
                    var DebitSubject = tempDebit.Subject;
                    var DebitBody = tempDebit.Body;
                    //string BodyReceivedName = "MR./Ms. " + ReceivedName;
                    DebitBody = DebitBody.Replace("[USERNAME]", username);
                    DebitBody = DebitBody.Replace("[FULLNAME]", fullname);
                    //DebitBody = DebitBody.Replace("[TranDate]", trandate);
                    //DebitBody = DebitBody.Replace("[TranNumber]", obTranlog.TRAN_ID);
                    //DebitBody = DebitBody.Replace("[FromAccount]", obTranlog.FROM_ACCT);
                    //DebitBody = DebitBody.Replace("[ToBankName]", obTranlog.TO_BANK);
                    //DebitBody = DebitBody.Replace("[ToBranchName]", obTranlog.TO_BRANCH);
                    //DebitBody = DebitBody.Replace("[ToAccount]", obTranlog.TO_ACCT);
                    ////DebitBody = DebitBody.Replace("[BodyReceivedName]", BodyReceivedName);
                    //DebitBody = DebitBody.Replace("[Amount]", obTranlog.TRAN_AMT.ToString());
                    //DebitBody = DebitBody.Replace("[Currency]", "MMK");
                    //DebitBody = DebitBody.Replace("[TranFee]", obTranlog.CHARGE_AMT.ToString());
                    //DebitBody = DebitBody.Replace("[FeeCurrency]", "MMK");
                    //DebitBody = DebitBody.Replace("[Desc]", Desc);
                    try
                    {
                        bool sentmailForDebit = General.SendEmailForCCT(email, DebitSubject, DebitBody, setting);
                        //log for email
                        log.Log(getuserid, controllerName, actionName, $"Email Sent successfully.", $"Email Address is {email}.");
                    }
                    catch (Exception ex)
                    {
                        log.Error(getuserid, controllerName, actionName, ex);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(getuserid, controllerName, actionName, ex);
                return false;
            }
        }

        //Set Success at Core_status
        [HttpPost]
        public JsonResult SetSuccess(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string contenttype = "Transaction";
                id = id.TrimEnd(',');
                string[] IDs = id.Split(',');
                foreach (var item in IDs)
                {
                    int Id = Convert.ToInt32(item.ToString());
                    OtherBankTranLog obTranlog = db.OtherBankTranLogs.Find(Id);
                    if (obTranlog == null)
                    {
                        return Json("Setting status fail!", JsonRequestBehavior.AllowGet);
                    }

                    obTranlog.Core_status = "Success";
                    db.Entry(obTranlog).State = EntityState.Modified;
                    db.SaveChanges();
                    var user = db._Users.Where(u => u.USERID == obTranlog.USERID).FirstOrDefault();
                    var beneficial = db.OtherBankBeneficiarys.Where(b => b.OTHER_BANK_BENE_ID == obTranlog.OTHER_BANK_BENE_ID).FirstOrDefault();
                    string getuserid = User.Identity.GetUserId();
                    string timestamp = General.GetTimestamp(DateTime.Now);
                    if (user.USER_TYPE == "Retail")
                    {
                        // Email Sent
                        if (IsMailSent())
                        {
                            try
                            {
                                General.SMTPSetting setting = GetEmailSetting();
                                //Get Data For Email
                                if (obTranlog.TRAN_TYPE == "OtherBankTransfer")
                                {
                                    var tranretail = "SUCCESS CCT_Retail_Tran";
                                    var mailallow = EmailAllow(tranretail);
                                    bool SentMailForRetailTran = RetailTransfer(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, mailallow.Sender_Allow, mailallow.Receiver_Allow, mailallow.Sender_Email_Allow, mailallow.Receiver_Email_Allow);
                                }
                                else if (obTranlog.TRAN_TYPE == "OtherBankRemittance")
                                {
                                    var tranretail = "SUCCESS CCT_Retail_Remit";
                                    var retailallow = EmailAllow(tranretail);
                                    bool SentMailForRetailRemit = RetailRemittance(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, retailallow.Sender_Allow, retailallow.Receiver_Allow, retailallow.Sender_Email_Allow, retailallow.Receiver_Email_Allow);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(getuserid, controllerName, actionName, ex);
                            }
                        }
                    }
                    //if (user.USER_TYPE == "Corporate")
                    //{
                    //    if (obTranlog.TRAN_TYPE == "OtherBankTransfer")
                    //    {
                    //        var trantype = "SUCCESS CCT_Co_Tran";
                    //        var mailallow = EmailAllow(trantype);
                    //        bool SentMailForCorporateTran = CorporateTransfer(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, mailallow.Sender_Allow, mailallow.Receiver_Allow, mailallow.Sender_Email_Allow, mailallow.Receiver_Email_Allow);
                    //    }
                    //    else if (obTranlog.TRAN_TYPE == "OtherBankRemittance")
                    //    {
                    //        var trantype = "SUCCESS CCT_Co_Remit";
                    //        var mailallow = EmailAllow(trantype);
                    //        bool SentMailForCorporateRemit = CorporateRemittance(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, mailallow.Sender_Allow, mailallow.Receiver_Allow, mailallow.Sender_Email_Allow, mailallow.Receiver_Email_Allow);
                    //    }
                    //}
                    else if (user.USER_TYPE == "Corporate")
                    {
                        // Corporate Email Sent
                        if (IsCorMailSent())
                        {
                            try
                            {
                                General.SMTPSetting setting = GetEmailSetting();
                                //var makertranlog = db.C_MakerTranLogs.Where(m => m.Id == obTranlog.MakerTranLogId).FirstOrDefault();
                                // if(makertranlog != null)
                                //{
                                //if (!string.IsNullOrEmpty(makertranlog.CreditEmail))                                
                                    if (obTranlog.TRAN_TYPE == "OtherBankTransfer")
                                    {
                                        var trantype = "SUCCESS CCT_Co_Tran";
                                        var mailallow = EmailAllow(trantype);
                                        bool SentMailForCorporateTran = CorporateTransfer(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, mailallow.Sender_Allow, mailallow.Receiver_Allow, mailallow.Sender_Email_Allow, mailallow.Receiver_Email_Allow);
                                    }
                                    else if (obTranlog.TRAN_TYPE == "OtherBankRemittance")
                                    {
                                        var trantype = "SUCCESS CCT_Co_Remit";
                                        var mailallow = EmailAllow(trantype);
                                        bool SentMailForCorporateRemit = CorporateRemittance(user.FULLNAME, user.EMAIL, obTranlog, beneficial.CreditEmail, beneficial.ACCOUNTNAME, beneficial.DESCRIPTION, mailallow.Sender_Allow, mailallow.Receiver_Allow, mailallow.Sender_Email_Allow, mailallow.Receiver_Email_Allow);
                                    }
                            }
                            catch (Exception ex)
                            {
                                log.Error(getuserid, controllerName, actionName, ex);
                            }
                        }
                    }                    

                    Notification notification = new Notification();
                    notification.TO_USERID = user.USERID.ToString();
                    //get userid from autocomplete textbox
                    notification.ACTIVE = true;
                    notification.DATE = DateTime.Now;
                    notification.CONTENTTYPE = contenttype;
                    notification.TITLE = "Testing Title Transfer";
                    notification.CONTENT = "Testing Content Transfer";
                    notification.SENDERTYPE = "1";
                    notification.CreatedDate = DateTime.Now;
                    notification.CreatedUserId = User.Identity.GetUserId();
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                    //save log;
                    log.Info(getuserid, controllerName, actionName, 1, notification.ToString());

                    //Sent Notification
                    if (user.USER_TYPE == "Retail")
                    {
                        if (obTranlog.TRAN_TYPE == "OtherBankTransfer")
                        {
                            //Sent Noti For Transfer Debit
                            if (!String.IsNullOrEmpty(user.EMAIL) && IsEmailValid(user.EMAIL))
                            {
                                if (Send_FireBase_Notification(user.USERID, notification.TITLE, notification.CONTENT, notification.CONTENTTYPE))
                                {
                                    //noti send successfully
                                    log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{user.USERID}\nTitle:{"Testing Title"}\nContent:{"Testing Content"}");
                                }
                                else
                                {
                                    //noti send fail
                                    log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{user.USERID}");
                                }
                            }
                        }
                        else if (obTranlog.TRAN_TYPE == "OtherBankRemittance")
                        {
                            if (!String.IsNullOrEmpty(user.USERID) && IsEmailValid(user.USERID))
                            {
                                if (Send_FireBase_Notification(user.USERID, notification.TITLE, notification.CONTENT, notification.CONTENTTYPE))
                                {
                                    //noti send successfully
                                    log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{user.USERID}\nTitle:{"Testing Title"}\nContent:{"Testing Content"}");
                                }
                                else
                                {
                                    //noti send fail
                                    log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{user.USERID}");
                                }
                            }
                        }
                    }
                    //log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Set status successfully", $"User ID is {obTranlog.USERID}.");
                }
                return Json("Setting status successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
       
        #region Noti 
        private Boolean Send_FireBase_Notification(string UserID, string NotiTitle, string NotiBody, string TranNotiType)
        {
            try
            {
                string FireBaseToken;
                //string TransactionNotiType = "Transaction";
                var user = db._Users.Where(u => u.USERID == UserID).FirstOrDefault();

                //if By Name (such as complain), get FireBaseToken from FirebaseToken by UserID               

                string fbtoken = db.Database.SqlQuery<string>("select FirebaseToken from TBL_FIREBASE " +
                       $"where UserID = '{UserID}' " +
                       "order by convert(datetime,UpdatedDate,103) desc").FirstOrDefault();

                FireBaseToken = fbtoken;

                NotificationHelper.SendNotificationRequestModel notiModel = new NotificationHelper.SendNotificationRequestModel();
                notiModel.FireBaseToken = FireBaseToken;
                notiModel.NotiType = TranNotiType;
                notiModel.Title = NotiTitle;
                notiModel.MessageBody = NotiBody;
                NotificationHelper notiHelper = new NotificationHelper();
                Boolean IsSuccess = notiHelper.PushNotificationByFireBase(notiModel);

                //save log
                if (IsSuccess)
                {
                    //noti send successfully
                    log.Log(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", $"Notification sent successfully.", $"To:{FireBaseToken}\nTitle:{NotiTitle}\nContent:{NotiBody}");
                }
                else
                {
                    //noti send fail
                    log.Log(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", $"Notification sent fail.", $"To:{FireBaseToken}");
                }

                return true;
            }
            catch (Exception ex)
            {
                //save error log
                log.Error(User.Identity.GetUserId(), controllerName, "Send_FireBase_Notification", ex);
            }
            return false;
        }
        #endregion
        //Validate EmailAddress
        public bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //Validate Rule For Retail
        private bool IsMailSent()
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == "R035").Select(r => r.Value).FirstOrDefault();
                int IntRule = Convert.ToInt32(rule);
                if (IntRule == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return false;

            }
        }

        //Validate Rule For Corporate
        private bool IsCorMailSent()
        {
            try
            {
                var rule = db.Rules.Where(r => r.Code == "C006").Select(r => r.Value).FirstOrDefault();
                int IntRule = Convert.ToInt32(rule);
                if (IntRule == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return false;

            }
        }

        //Get Email Template
        public General.SMTPSetting GetEmailSetting()
        {
            General.SMTPSetting setting = new General.SMTPSetting();
            var mail = (from a in db.Credentials
                        where a.Status == "1" & a.Name == "MailSetting"
                        select a).ToList();
            setting.SenderMail = mail.Where(a => a.Key == "sendermail").Select(n => n.Value).FirstOrDefault();
            setting.SenderName = mail.Where(a => a.Key == "sendername").Select(n => n.Value).FirstOrDefault();
            setting.Password = mail.Where(a => a.Key == "password").Select(n => n.Value).FirstOrDefault();
            setting.Host = mail.Where(a => a.Key == "host").Select(n => n.Value).FirstOrDefault();
            int port = Convert.ToInt32(mail.Where(m => m.Key == "port").Select(m => m.Value).FirstOrDefault());
            setting.Port = port;
            return setting;
        }

        //Set Fail at Core_status
        [HttpPost]
        public JsonResult SetFail(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                id = id.TrimEnd(',');
                string[] IDs = id.Split(',');
                foreach (var item in IDs)
                {
                    int Id = Convert.ToInt32(item.ToString());
                    OtherBankTranLog obTranlog = db.OtherBankTranLogs.Find(Id);
                    if (obTranlog == null)
                    {
                        return Json("Setting status fail!", JsonRequestBehavior.AllowGet);
                    }
                    obTranlog.Core_status = "Fail";
                    db.Entry(obTranlog).State = EntityState.Modified;
                    db.SaveChanges();
                    //log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Set status successfully", $"User ID is {obTranlog.USERID}.");
                }                

                return Json("Setting status successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        //get other bank transaction data
        private DataTable GetOtherBankTranLog(string username, string fromacct, string toacct, string bank, string status, string from, string to,string cbmtrantype,int IsCorporate)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(ConString);
                using (connection)
                {
                    SqlCommand command = new SqlCommand(
                      "SP_Admin_RP_OtherBankTranLog",
                      connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", username));
                    command.Parameters.Add(new SqlParameter("@FromAcct", fromacct));
                    command.Parameters.Add(new SqlParameter("@ToAcct", toacct));
                    command.Parameters.Add(new SqlParameter("@OtherBank", bank));
                    command.Parameters.Add(new SqlParameter("@Status", status));
                    command.Parameters.Add(new SqlParameter("@FromDate", from));
                    command.Parameters.Add(new SqlParameter("@ToDate", to));
                    command.Parameters.Add(new SqlParameter("@CBMTranType", cbmtrantype));
                    command.Parameters.Add(new SqlParameter("@IsCorporate", IsCorporate));
                    connection.Open();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    //conn.Close();

                    var das = ds.Tables[0].AsEnumerable();
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return null;
            }
        }
        #endregion
        
        #region OtherBankRecon
        public ActionResult OtherBankRecon()
        {
            OtherBankReconSearchVM otherbankvm = new OtherBankReconSearchVM();
            otherbankvm.FromDate = DateTime.Now.ToString(ConstantValues.Date_Format);
            otherbankvm.ToDate = DateTime.Now.ToString(ConstantValues.Date_Format);
            bindDownloadType();
            BindCBMTran_Type();
            ViewBag.IsBulkFile = TempData["IsBulkFile"];
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return View(otherbankvm);
        }

        //Index Post
        [HttpPost, ActionName("OtherBankRecon")]
        public ActionResult OtherBankRecons()
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var isDownload = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var cbmtrantype = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            //#region validation
            ////check toDate is greater than fromDate or not
            //if (toDate < fromDate)
            //{
            //    ModelState.AddModelError("FromDate", "From Date must be less than To Date.");
            //    return View();
            //}
            //#endregion  
            //Add One Day for toDate                    
            toDate = toDate.AddDays(1);

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            var otherbankrecon = (from t in db.OtherBankTranLogs
                                  join o in db.Other_Bank_Recons on t.ID equals o.OTH_BK_TRAN_LOG_ID into tblrecon
                                  from tblotrecon in tblrecon
                                  where t.Core_status == "Pending" && (t.BulkPaymentFileUploadId == null || t.BulkPaymentFileUploadId == 0)
                                  select new
                                  {
                                      tblotrecon.ID,
                                      tblotrecon.RECON_NUMBER,
                                      tblotrecon.DR_BR_CODE,
                                      tblotrecon.CR_BR_CODE,
                                      tblotrecon.TOTAL_SETTLEMENT_AMT,
                                      tblotrecon.TRAN_AMT,
                                      tblotrecon.DR_INTERBANK_CHARGE_AMT,
                                      tblotrecon.CR_INTERBANK_CHARGE_AMT,
                                      tblotrecon.EVENT_CODE,
                                      tblotrecon.EVENT_NAME,
                                      tblotrecon.DR_FI_BRANCH_CODE,
                                      tblotrecon.DR_FI_BRANCH_NAME,
                                      tblotrecon.DR_ID_CATEGORY,
                                      tblotrecon.DR_ACCOUNT,
                                      tblotrecon.DR_NAME,
                                      IsDownloaded = t.IsDownloaded,
                                      VALUE_DATE = t.VALUE_DATE.ToString(),
                                      CBMTranType=t.CBMTran_Type

                                  }).ToList();

                if (fromDate != null & toDate != null)
            {
                otherbankrecon = otherbankrecon.Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate.Date && DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate.Date).ToList();
            }
            if (isDownload == "0")
            {
                otherbankrecon = otherbankrecon.Where(p => p.IsDownloaded == false).ToList();
            }
            if (isDownload == "1")
            {
                otherbankrecon = otherbankrecon.Where(p => p.IsDownloaded == true).ToList();
            }
            if (cbmtrantype != "")
            {
                otherbankrecon = otherbankrecon.Where(p => p.CBMTranType == cbmtrantype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                otherbankrecon = otherbankrecon.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = otherbankrecon.Count();
            var data = otherbankrecon.Skip(skip).Take(pageSize).ToList();
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }
        #region bind download Type ddl
        private void bindDownloadType()
        {
            ViewBag.DownloadType = new List<SelectListItem>
                {
                    new SelectListItem{Text="New",Value= "0" },
                    new SelectListItem{Text="Downloaded",Value="1"},
                    new SelectListItem{Text="Both",Value="2"}
                };
        }
        #endregion
        //GetTable For Normal Transaction
        public DataTable GetTable(string FromDate, string ToDate, string isDownloaded)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;            
            if (!String.IsNullOrEmpty(FromDate))
            {
                fromDate = General.ConvertStrToDate(FromDate);
            }
            if (!String.IsNullOrEmpty(ToDate))
            {
                toDate = General.ConvertStrToDate(ToDate);
            }

            //Add One Day for toDate                    
            toDate = toDate.AddDays(1);
            var otherbankrecon = (from t in db.OtherBankTranLogs
                                  join o in db.Other_Bank_Recons on t.ID equals o.OTH_BK_TRAN_LOG_ID into tblrecon
                                  from tblotrecon in tblrecon
                                  where t.Core_status == "Pending" && (t.BulkPaymentFileUploadId == null || t.BulkPaymentFileUploadId == 0)
                                  select new Other_Bank_ReconVM
                                  {
                                      RECON_NUMBER = tblotrecon.RECON_NUMBER,
                                      DR_BR_CODE = tblotrecon.DR_BR_CODE,
                                      CR_BR_CODE = tblotrecon.CR_BR_CODE,
                                      TOTAL_SETTLEMENT_AMT = tblotrecon.TOTAL_SETTLEMENT_AMT,
                                      TRAN_AMT = tblotrecon.TRAN_AMT,
                                      DR_INTERBANK_CHARGE_AMT = tblotrecon.DR_INTERBANK_CHARGE_AMT,
                                      CR_INTERBANK_CHARGE_AMT = tblotrecon.CR_INTERBANK_CHARGE_AMT,
                                      EVENT_CODE = tblotrecon.EVENT_CODE,
                                      EVENT_NAME = tblotrecon.EVENT_NAME,
                                      DR_FI_BRANCH_CODE = tblotrecon.DR_FI_BRANCH_CODE,
                                      DR_FI_BRANCH_NAME = tblotrecon.DR_FI_BRANCH_NAME,
                                      DR_ID_CATEGORY = tblotrecon.DR_ID_CATEGORY,
                                      DR_ACCOUNT = tblotrecon.DR_ACCOUNT,
                                      DR_NAME = tblotrecon.DR_NAME,
                                      DR_POSTAL_ADDRESS = tblotrecon.DR_POSTAL_ADDRESS,
                                      DR_PHONE = tblotrecon.DR_PHONE,
                                      OTHER_INFORMATION = tblotrecon.OTHER_INFORMATION,
                                      CR_FI_BRANCH_CODE = tblotrecon.CR_FI_BRANCH_CODE,
                                      CR_FI_BRANCH_NAME = tblotrecon.CR_FI_BRANCH_NAME,
                                      CR_ID_CATEGORY = tblotrecon.CR_ID_CATEGORY,
                                      CR_ACCT_OR_NRC = tblotrecon.CR_ACCT_OR_NRC,
                                      CR_NAME = tblotrecon.CR_NAME,
                                      CR_ADDRESS = tblotrecon.CR_ADDRESS,
                                      CR_PHONE = tblotrecon.CR_PHONE,
                                      Cust_CR_Transfer = tblotrecon.Cust_CR_Transfer,
                                      TranID = t.ID,
                                      IsDownloaded=t.IsDownloaded,
                                      VALUE_DATE = t.VALUE_DATE
                                  }).ToList();
            if (fromDate != null & toDate != null)
            {
                otherbankrecon = otherbankrecon.AsEnumerable()
                    .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate & DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate)
                    .ToList();
            }
            else if (fromDate != null)
            {
                otherbankrecon = otherbankrecon.AsEnumerable()
                    .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate)
                    .ToList();
            }
            else if (toDate != null)
            {
                otherbankrecon = otherbankrecon.AsEnumerable()
                   .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate)
                   .ToList();
            }
            if (isDownloaded == "0")
            {
                otherbankrecon = otherbankrecon.AsEnumerable()
                  .Where(p => p.IsDownloaded == false)
                  .ToList();
            }
            else if (isDownloaded == "1")
            {
                otherbankrecon = otherbankrecon.AsEnumerable()
                  .Where(p => p.IsDownloaded == true)
                  .ToList();
            }
            List<Other_Bank_ReconVM> otherbankVM = otherbankrecon.ToList();            
            Session["TableData"] = otherbankVM;            
            DataTable table = ConvertToDataTable(otherbankrecon);
            if (table.Rows.Count > 0)
            {
                table.Columns.Remove("TranID");
                table.Columns.Remove("VALUE_DATE");
                table.Columns.Remove("IsDownloaded");
                table.Columns["RECON_NUMBER"].ColumnName = "Reconciliation Number";
                table.Columns["DR_BR_CODE"].ColumnName = "Fund Debtor FI Branch Code";
                table.Columns["CR_BR_CODE"].ColumnName = "Fund Creditor FI Branch Code";
                table.Columns["TOTAL_SETTLEMENT_AMT"].ColumnName = "Total Fund Settlement Amount(MMK)";
                table.Columns["TRAN_AMT"].ColumnName = "Transfer Amount(MMK)";
                table.Columns["DR_INTERBANK_CHARGE_AMT"].ColumnName = "Interbank Charge Amount (MMK) to Debtor FI";
                table.Columns["CR_INTERBANK_CHARGE_AMT"].ColumnName = "Interbank Charge Amount (MMK) to Creditor FI";
                table.Columns["EVENT_CODE"].ColumnName = "Event (Fund) Code";
                table.Columns["EVENT_NAME"].ColumnName = "Event (Fund) Name";
                table.Columns["DR_FI_BRANCH_CODE"].ColumnName = "Debtor Agent FI Branch Code";
                table.Columns["DR_FI_BRANCH_NAME"].ColumnName = "Debtor Agent FI Branch Name";
                table.Columns["DR_ID_CATEGORY"].ColumnName = "Debtor Identification Category Code";
                table.Columns["DR_ACCOUNT"].ColumnName = "Debtor identification category name";
                table.Columns["DR_NAME"].ColumnName = "Debtor Name";
                table.Columns["DR_POSTAL_ADDRESS"].ColumnName = "Debtor Postal Address";
                table.Columns["DR_PHONE"].ColumnName = "Debtor Phone Number";
                table.Columns["OTHER_INFORMATION"].ColumnName = "Other Information";
                table.Columns["CR_FI_BRANCH_CODE"].ColumnName = "Creditor Agent FI Branch Number";
                table.Columns["CR_FI_BRANCH_NAME"].ColumnName = "Creditor Agent FI Branch Name";
                table.Columns["CR_ID_CATEGORY"].ColumnName = "Creditor Identification Category Code";
                table.Columns["CR_ACCT_OR_NRC"].ColumnName = "Creditor Identification";
                table.Columns["CR_NAME"].ColumnName = "Creditor Name";
                table.Columns["CR_ADDRESS"].ColumnName = "Creditor Postal Address";
                table.Columns["CR_PHONE"].ColumnName = "Creditor Phone Number";
            }
            return table;
        }
        //GetTable For Bulk Transaction
        //Get Table For Bulk
        public DataTable GetBulkTable(string FromDate, string ToDate, string isDownloaded, string CompanyName, string FileName)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!String.IsNullOrEmpty(FromDate))
            {
                fromDate = General.ConvertStrToDate(FromDate);
            }
            if (!String.IsNullOrEmpty(ToDate))
            {
                toDate = General.ConvertStrToDate(ToDate);
            }

            //Add One Day for toDate                    
            toDate = toDate.AddDays(1);

            var bulkpaymentrecon = (from t in db.OtherBankTranLogs
                                    join o in db.Other_Bank_Recons on t.ID equals o.OTH_BK_TRAN_LOG_ID into tblrecon
                                    join b in db.C_BulkPaymentFileUploads on t.BulkPaymentFileUploadId equals b.Id into tblbulkfile
                                    from tblotrecon in tblrecon
                                    from tblbulk in tblbulkfile
                                    where t.Core_status == "Pending" && t.BulkPaymentFileUploadId != null
                                    select new Other_Bank_ReconVM
                                    {
                                        RECON_NUMBER = tblotrecon.RECON_NUMBER,
                                        DR_BR_CODE = tblotrecon.DR_BR_CODE,
                                        CR_BR_CODE = tblotrecon.CR_BR_CODE,
                                        TOTAL_SETTLEMENT_AMT = tblotrecon.TOTAL_SETTLEMENT_AMT,
                                        TRAN_AMT = tblotrecon.TRAN_AMT,
                                        DR_INTERBANK_CHARGE_AMT = tblotrecon.DR_INTERBANK_CHARGE_AMT,
                                        CR_INTERBANK_CHARGE_AMT = tblotrecon.CR_INTERBANK_CHARGE_AMT,
                                        EVENT_CODE = tblotrecon.EVENT_CODE,
                                        EVENT_NAME = tblotrecon.EVENT_NAME,
                                        DR_FI_BRANCH_CODE = tblotrecon.DR_FI_BRANCH_CODE,
                                        DR_FI_BRANCH_NAME = tblotrecon.DR_FI_BRANCH_NAME,
                                        DR_ID_CATEGORY = tblotrecon.DR_ID_CATEGORY,
                                        DR_ACCOUNT = tblotrecon.DR_ACCOUNT,
                                        DR_NAME = tblotrecon.DR_NAME,
                                        DR_POSTAL_ADDRESS = tblotrecon.DR_POSTAL_ADDRESS,
                                        DR_PHONE = tblotrecon.DR_PHONE,
                                        OTHER_INFORMATION = tblotrecon.OTHER_INFORMATION,
                                        CR_FI_BRANCH_CODE = tblotrecon.CR_FI_BRANCH_CODE,
                                        CR_FI_BRANCH_NAME = tblotrecon.CR_FI_BRANCH_NAME,
                                        CR_ID_CATEGORY = tblotrecon.CR_ID_CATEGORY,
                                        CR_ACCT_OR_NRC = tblotrecon.CR_ACCT_OR_NRC,
                                        CR_NAME = tblotrecon.CR_NAME,
                                        CR_ADDRESS = tblotrecon.CR_ADDRESS,
                                        CR_PHONE = tblotrecon.CR_PHONE,
                                        Cust_CR_Transfer = tblotrecon.Cust_CR_Transfer,
                                        TranID = t.ID,
                                        IsDownloaded = t.IsDownloaded,
                                        VALUE_DATE = t.VALUE_DATE,
                                        CompanyName = tblbulk.VrCorporate.COMPANY_NAME,
                                        FileName = tblbulk.FileName
                                    }).ToList();
            if (!string.IsNullOrEmpty(CompanyName))
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(a => a.CompanyName.ToLower().Trim().Contains(CompanyName.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(FileName))
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(a => a.FileName.ToLower().Trim().Contains(FileName.ToLower().Trim())).ToList();
            }
            if (fromDate != null & toDate != null)
            {
                bulkpaymentrecon = bulkpaymentrecon.AsEnumerable()
                    .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate & DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate)
                    .ToList();
            }
            else if (fromDate != null)
            {
                bulkpaymentrecon = bulkpaymentrecon.AsEnumerable()
                    .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate)
                    .ToList();
            }
            else if (toDate != null)
            {
                bulkpaymentrecon = bulkpaymentrecon.AsEnumerable()
                   .Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate)
                   .ToList();
            }
            if (isDownloaded == "0")
            {
                bulkpaymentrecon = bulkpaymentrecon.AsEnumerable()
                  .Where(p => p.IsDownloaded == false)
                  .ToList();
            }
            else if (isDownloaded == "1")
            {
                bulkpaymentrecon = bulkpaymentrecon.AsEnumerable()
                  .Where(p => p.IsDownloaded == true)
                  .ToList();
            }
            List<Other_Bank_ReconVM> otherbankVM = bulkpaymentrecon.ToList();
            Session["TableData"] = otherbankVM;
            DataTable table = ConvertToDataTable(bulkpaymentrecon);
            if (table.Rows.Count > 0)
            {
                table.Columns.Remove("TranID");
                table.Columns.Remove("VALUE_DATE");
                table.Columns.Remove("IsDownloaded");
                table.Columns.Remove("CompanyName");
                table.Columns.Remove("FileName");
                table.Columns["RECON_NUMBER"].ColumnName = "Reconciliation Number";
                table.Columns["DR_BR_CODE"].ColumnName = "Fund Debtor FI Branch Code";
                table.Columns["CR_BR_CODE"].ColumnName = "Fund Creditor FI Branch Code";
                table.Columns["TOTAL_SETTLEMENT_AMT"].ColumnName = "Total Fund Settlement Amount(MMK)";
                table.Columns["TRAN_AMT"].ColumnName = "Transfer Amount(MMK)";
                table.Columns["DR_INTERBANK_CHARGE_AMT"].ColumnName = "Interbank Charge Amount (MMK) to Debtor FI";
                table.Columns["CR_INTERBANK_CHARGE_AMT"].ColumnName = "Interbank Charge Amount (MMK) to Creditor FI";
                table.Columns["EVENT_CODE"].ColumnName = "Event (Fund) Code";
                table.Columns["EVENT_NAME"].ColumnName = "Event (Fund) Name";
                table.Columns["DR_FI_BRANCH_CODE"].ColumnName = "Debtor Agent FI Branch Code";
                table.Columns["DR_FI_BRANCH_NAME"].ColumnName = "Debtor Agent FI Branch Name";
                table.Columns["DR_ID_CATEGORY"].ColumnName = "Debtor Identification Category Code";
                table.Columns["DR_ACCOUNT"].ColumnName = "Debtor identification category name";
                table.Columns["DR_NAME"].ColumnName = "Debtor Name";
                table.Columns["DR_POSTAL_ADDRESS"].ColumnName = "Debtor Postal Address";
                table.Columns["DR_PHONE"].ColumnName = "Debtor Phone Number";
                table.Columns["OTHER_INFORMATION"].ColumnName = "Other Information";
                table.Columns["CR_FI_BRANCH_CODE"].ColumnName = "Creditor Agent FI Branch Number";
                table.Columns["CR_FI_BRANCH_NAME"].ColumnName = "Creditor Agent FI Branch Name";
                table.Columns["CR_ID_CATEGORY"].ColumnName = "Creditor Identification Category Code";
                table.Columns["CR_ACCT_OR_NRC"].ColumnName = "Creditor Identification";
                table.Columns["CR_NAME"].ColumnName = "Creditor Name";
                table.Columns["CR_ADDRESS"].ColumnName = "Creditor Postal Address";
                table.Columns["CR_PHONE"].ColumnName = "Creditor Phone Number";
            }
            return table;
        }
        //Update Download Status 
        public void updateDownload(DataTable dt) {
            List<Other_Bank_ReconVM> item = Session["TableData"] as List<Other_Bank_ReconVM>;
            var tranLog = from o in item
                            where o.IsDownloaded == false
                            select o;
            foreach (var a in tranLog)
            {
                int id = Convert.ToInt32(a.TranID);
                OtherBankTranLog obTranLog = db.OtherBankTranLogs.SingleOrDefault(ob => ob.ID == id);
                obTranLog.IsDownloaded = true;
                db.Entry(obTranLog).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        public void ExportToExcel(DataTable dt, FormCollection form)//ActionResult
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string datestamp = General.GetDatestamp(DateTime.Now);
                //create if folder doesn't exist
                if (!Directory.Exists(CSVFile_Path))
                {
                    Directory.CreateDirectory(CSVFile_Path);
                }
                string path = $"{CSVFile_Path}";
                var files = new DirectoryInfo(path).GetFiles("*.*");
                string latestfile = string.Empty;
                string filename = string.Empty;
                int FileNo = 100;
                DateTime lastModified = DateTime.MinValue;
                DateTime todayDate = DateTime.Today;
                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime > lastModified & file.LastWriteTime.Date == todayDate.Date)
                    {
                        lastModified = file.LastWriteTime;
                        latestfile = file.Name;
                    }
                }
                if (!String.IsNullOrEmpty(latestfile))
                {
                    latestfile = latestfile.Replace("_IF_A040_01.csv", "");
                    FileNo = Convert.ToInt32(latestfile.Substring(latestfile.Length-3)) + 1;
                    filename = $"{datestamp}_AYFDMMMYYGN_"+ FileNo.ToString("D3") + "_IF_A040_01" + ".csv";
                }
                else
                {
                    filename = $"{datestamp}_AYFDMMMYYGN_" + FileNo.ToString("D3") + "_IF_A040_01" + ".csv";
                }
                string fromDate = form["FromDate"];
                string toDate = form["ToDate"];
                string isDownloaded = form["ddlDownloadType"];              
                //OtherBankReconSearchVM obsvm = new OtherBankReconSearchVM();
                //string from = null, to = null;
                //DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                //obsvm.FromDate = DateTime.Now.ToString(ConstantValues.Date_Format);
                //obsvm.ToDate = DateTime.Now.ToString(ConstantValues.Date_Format);
                //if (!String.IsNullOrEmpty(obsvm.FromDate))
                //{
                //    fromDate = General.ConvertStrToDate(obsvm.FromDate);
                //    from = fromDate.ToString(ConstantValues.Date_Format);
                //}
                // if (!String.IsNullOrEmpty(obsvm.FromDate))
                //{
                //    toDate = General.ConvertStrToDate(obsvm.ToDate);
                //    to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                //}
                string filepath = $"{path}\\{filename}";
                dt = GetTable(fromDate, toDate, isDownloaded);
                dt.Columns.Remove("CompanyName");
                dt.Columns.Remove("FileName");
                //fromDate, toDate

                //Build the CSV file data as a Comma separated string.
                string csv = string.Empty;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                //AddDoubleQuote("2") + ","
                csv += AddDoubleQuote("1") + "," + AddDoubleQuote(date) + "," + AddDoubleQuote("AYFDMMMYYGN") + "," + AddDoubleQuote("Customer credit transfer (MMK)") + "," + AddDoubleQuote("csv");
                //csv += "\r\n";
                //csv += ",";
                //foreach (DataColumn column in dt.Columns)
                //{
                //    //Add the Header row for CSV file.
                //    csv += column.ColumnName + ',';
                //}
                //Add new line.
                csv += "\r\n";
                decimal totalfund = 0;
                foreach (DataRow row in dt.Rows)
                {
                    csv += AddDoubleQuote("2") + ",";
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName.ToString().Equals("Total Fund Settlement Amount(MMK)") || column.ColumnName.ToString().Equals("Transfer Amount(MMK)") || column.ColumnName.ToString().Equals("Interbank Charge Amount (MMK) to Debtor FI") || column.ColumnName.ToString().Equals("Interbank Charge Amount (MMK) to Creditor FI"))
                        {
                            csv += row[column.ColumnName].ToString() + ",";
                        }
                        else
                        {
                            csv += AddDoubleQuote(row[column.ColumnName].ToString()) + ",";
                        }
                    }
                    csv = csv.TrimEnd(',');

                    //csv += "\"\"\"" + "1" + "\"\"\"";
                    totalfund += Convert.ToDecimal(row["Total Fund Settlement Amount(MMK)"]);//Convert.ToDecimal(row.ItemArray[4]);
                                                                                             //Add new line.
                    csv += "\r\n";
                }
                var rows = dt.Rows.Count;
                csv += AddDoubleQuote("9") + "," + rows + "," + totalfund;
                updateDownload(dt);
                TempData["filename"] = filename;
                //Download the CSV file.
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(csv);
                System.IO.File.WriteAllText(filepath, csv.ToString());
                Response.Redirect("~/CCT/OtherBankRecon");
                //Response.Flush();
                //Response.End();
                
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }
        public ActionResult Download(string filename)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //string Pdf_Path = System.Configuration.ConfigurationManager.AppSettings["PdfSavePath"];
                string csvfile_path = ConstantValues.CSVFile_Path; ;
                string fullpath = "";
                fullpath = $"{csvfile_path}\\{filename}";                
                //log
                string getuserid = User.Identity.GetUserId();
                log.Log(getuserid, controllerName, actionName, $"Download successfully.", $"File name is {filename}.");
                return File(fullpath, "application/text", filename);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return View("Error");
            }
        }
        private string AddDoubleQuote(string ValueToEscape)
        {
            return string.Format("{0}{1}{2}", "\"", ValueToEscape, "\"");
        }
        #endregion

        // get company list
        private void GetCompanySelectList()
        {
            try
            {
                List<C_Corporate> corporate = db.C_Corporates.Where(c => c.DEL_FLAG == false).ToList();
                IEnumerable<SelectListItem> items = corporate.Select(a => new SelectListItem
                {
                    Value = a.COMPANY_NAME,
                    Text = a.COMPANY_NAME

                });
                ViewBag.Corporate = items;
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), "CorporateController", "GetCompanySelectList", ex);
            }
        }
        #region Export CSV for Bulk
        public ActionResult BulkPaymentRecon()
        {
            OtherBankReconSearchVM otherbankvm = new OtherBankReconSearchVM();
            otherbankvm.FromDate = DateTime.Now.ToString(ConstantValues.Date_Format);
            otherbankvm.ToDate = DateTime.Now.ToString(ConstantValues.Date_Format);
            bindDownloadType();
            BindCBMTran_Type();
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return View(otherbankvm);
        }

        //Index Post
        [HttpPost, ActionName("BulkPaymentRecon")]
        public ActionResult BulkPaymentRecons()
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var companyname = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var bulkfilename = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var isDownload = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var cbmtrantype = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();

            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
            }
            //Add One Day for toDate                    
            toDate = toDate.AddDays(1);

            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;

            var bulkpaymentrecon = (from t in db.OtherBankTranLogs
                                  join o in db.Other_Bank_Recons on t.ID equals o.OTH_BK_TRAN_LOG_ID into tblrecon
                                  join b in db.C_BulkPaymentFileUploads on t.BulkPaymentFileUploadId equals b.Id into tblbulkfile
                                  from tblotrecon in tblrecon
                                  from tblbulk in tblbulkfile
                                  where t.Core_status == "Pending" && (t.BulkPaymentFileUploadId != null || t.BulkPaymentFileUploadId != 0) /*&& t.MakerTranLogId != null*/
                                    select new
                                  {
                                      tblotrecon.ID,
                                      tblotrecon.RECON_NUMBER,
                                      tblotrecon.DR_BR_CODE,
                                      tblotrecon.CR_BR_CODE,
                                      tblotrecon.TOTAL_SETTLEMENT_AMT,
                                      tblotrecon.TRAN_AMT,
                                      tblotrecon.DR_INTERBANK_CHARGE_AMT,
                                      tblotrecon.CR_INTERBANK_CHARGE_AMT,
                                      tblotrecon.EVENT_CODE,
                                      tblotrecon.EVENT_NAME,
                                      tblotrecon.DR_FI_BRANCH_CODE,
                                      tblotrecon.DR_FI_BRANCH_NAME,
                                      tblotrecon.DR_ID_CATEGORY,
                                      tblotrecon.DR_ACCOUNT,
                                      tblotrecon.DR_NAME,
                                      IsDownloaded = t.IsDownloaded,
                                      CompanyName = tblbulk.VrCorporate.COMPANY_NAME,
                                      FileName = tblbulk.FileName,
                                      VALUE_DATE = t.VALUE_DATE.ToString(),
                                      CBMTranType = t.CBMTran_Type
                                    }).ToList();
            if (!string.IsNullOrEmpty(companyname))
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(a => a.CompanyName.ToLower().Trim().Contains(companyname.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(bulkfilename))
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(a => a.FileName.ToLower().Trim().Contains(bulkfilename.ToLower().Trim())).ToList();
            }
            if (fromDate != null & toDate != null)
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(p => DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fromDate.Date && DateTime.ParseExact(p.VALUE_DATE.ToString().Trim().Replace("-", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < toDate.Date).ToList();
            }
            if (isDownload == "0")
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(p => p.IsDownloaded == false).ToList();
            }
            if (isDownload == "1")
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(p => p.IsDownloaded == true).ToList();
            }
            if (cbmtrantype != "")
            {
                bulkpaymentrecon = bulkpaymentrecon.Where(p => p.CBMTranType == cbmtrantype).ToList();
            }

            ////SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                bulkpaymentrecon = bulkpaymentrecon.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = bulkpaymentrecon.Count();
            var data = bulkpaymentrecon.Skip(skip).Take(pageSize).ToList();
            if (TempData["filename"] != null && TempData["filename"].ToString() != "")
                ViewBag.filename = TempData["filename"];
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public void BulkPaymentExportToExcel(DataTable dt, FormCollection form)//ActionResult
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                string datestamp = General.GetDatestamp(DateTime.Now);
                //create if folder doesn't exist
                if (!Directory.Exists(CSVFile_Path))
                {
                    Directory.CreateDirectory(CSVFile_Path);
                }
                string path = $"{CSVFile_Path}";
                var files = new DirectoryInfo(path).GetFiles("*.*");
                string latestfile = string.Empty;
                string filename = string.Empty;
                int FileNo = 100;
                DateTime lastModified = DateTime.MinValue;
                DateTime todayDate = DateTime.Today;
                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime > lastModified & file.LastWriteTime.Date == todayDate.Date)
                    {
                        lastModified = file.LastWriteTime;
                        latestfile = file.Name;
                    }
                }
                if (!String.IsNullOrEmpty(latestfile))
                {
                    latestfile = latestfile.Replace("_IF_A040_01.csv", "");
                    FileNo = Convert.ToInt32(latestfile.Substring(latestfile.Length - 3)) + 1;
                    filename = $"{datestamp}_AYFDMMMYYGN_" + FileNo.ToString("D3") + "_IF_A040_01" + ".csv";
                }
                else
                {
                    filename = $"{datestamp}_AYFDMMMYYGN_" + FileNo.ToString("D3") + "_IF_A040_01" + ".csv";
                }

                TempData["IsBulkFile"] = "true";
                string fromDate = form["BulkFromDate"];
                string toDate = form["BulkToDate"];
                string isDownloaded = form["ddlBulkDownloadType"];
                string companyName = form["txtCompanyName"];
                string fileName = form["txtBulkFileName"];
                //OtherBankReconSearchVM obsvm = new OtherBankReconSearchVM();
                //string from = null, to = null;
                //DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
                //obsvm.FromDate = DateTime.Now.ToString(ConstantValues.Date_Format);
                //obsvm.ToDate = DateTime.Now.ToString(ConstantValues.Date_Format);
                //if (!String.IsNullOrEmpty(obsvm.FromDate))
                //{
                //    fromDate = General.ConvertStrToDate(obsvm.FromDate);
                //    from = fromDate.ToString(ConstantValues.Date_Format);
                //}
                // if (!String.IsNullOrEmpty(obsvm.FromDate))
                //{
                //    toDate = General.ConvertStrToDate(obsvm.ToDate);
                //    to = toDate.AddDays(1).ToString(ConstantValues.Date_Format);
                //}
                string filepath = $"{path}\\{filename}";
                dt = GetBulkTable(fromDate, toDate, isDownloaded, companyName, fileName);
                //fromDate, toDate

                //Build the CSV file data as a Comma separated string.
                string csv = string.Empty;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                //AddDoubleQuote("2") + ","
                csv += AddDoubleQuote("1") + "," + AddDoubleQuote(date) + "," + AddDoubleQuote("AYFDMMMYYGN") + "," + AddDoubleQuote("Customer credit transfer (MMK)") + "," + AddDoubleQuote("csv");
                //csv += "\r\n";
                //csv += ",";
                //foreach (DataColumn column in dt.Columns)
                //{
                //    //Add the Header row for CSV file.
                //    csv += column.ColumnName + ',';
                //}
                //Add new line.
                csv += "\r\n";
                decimal totalfund = 0;
                foreach (DataRow row in dt.Rows)
                {
                    csv += AddDoubleQuote("2") + ",";
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName.ToString().Equals("Total Fund Settlement Amount(MMK)") || column.ColumnName.ToString().Equals("Transfer Amount(MMK)") || column.ColumnName.ToString().Equals("Interbank Charge Amount (MMK) to Debtor FI") || column.ColumnName.ToString().Equals("Interbank Charge Amount (MMK) to Creditor FI"))
                        {
                            csv += row[column.ColumnName].ToString() + ",";
                        }
                        else
                        {
                            csv += AddDoubleQuote(row[column.ColumnName].ToString()) + ",";
                        }
                    }
                    csv = csv.TrimEnd(',');

                    //csv += "\"\"\"" + "1" + "\"\"\"";
                    totalfund += Convert.ToDecimal(row["Total Fund Settlement Amount(MMK)"]);//Convert.ToDecimal(row.ItemArray[4]);
                                                                                             //Add new line.
                    csv += "\r\n";
                }
                var rows = dt.Rows.Count;
                csv += AddDoubleQuote("9") + "," + rows + "," + totalfund;
                updateDownload(dt);
                TempData["filename"] = filename;
                //Download the CSV file.
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(csv);
                System.IO.File.WriteAllText(filepath, csv.ToString());
               Response.Redirect("~/CCT/OtherBankRecon");
                //Response.Flush();
                //Response.End();

            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
            }
        }
        #endregion

        #region Pending List with Bulk Charges
        private void GetStatusddl()
        {           
            //Create status
            ViewBag.Status = new List<SelectListItem>()
            {
                new SelectListItem{Text="Approve",Value="Approve"},
                new SelectListItem{Text="Rejected",Value="Rejected"},
                new SelectListItem{Text="Pending",Value="Pending"}
            };
        }
        // GET: CCT
        public ActionResult BulkPaymentPendingList()
        {
                GetStatusddl();
                return View();
        }

            [ActionName("BulkPaymentPendingList")]
            [HttpPost]
            public ActionResult BulkPaymentPendingLists()
            {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var FromAccountNo = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var fromdate = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var todate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            DateTime fromDate = DateTime.Now.Date, toDate = DateTime.Now.Date;
            if (!String.IsNullOrEmpty(fromdate))
            {
                fromDate = DateTime.ParseExact(fromdate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
                //fromdate = fromDate.ToString("dd-MM-yyyy");
            }
            if (!String.IsNullOrEmpty(todate))
            {
                toDate = DateTime.ParseExact(todate.ToString(), ConstantValues.Date_Format, CultureInfo.InvariantCulture).AddDays(1);
                //todate = toDate.ToString("dd-MM-yyyy");
            }
            if(fromDate == toDate) { toDate = toDate.AddDays(1); }
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            List<C_BulkFileUploadVM> bulkfileupload = (from c in db.C_BulkPaymentFileUploads
                                  join u in db._Users on c.CreatedUserId equals u.USERID into tbluser
                                from tblrm in tbluser//.DefaultIfEmpty()
                                where c.DEV_FLAG == false && c.TransType == "TransferOtherBank" && c.Status == "Approved" && c.IsSetStatus == false
                                orderby c.CreatedDate descending
                                select new C_BulkFileUploadVM
                                {
                                    Id = c.Id,
                                    MakerName = tblrm.USERNAME,
                                    FileName = c.FileName,
                                    FromAccount = c.FromAccount,
                                    CreatedDate = c.CreatedDate,
                                    TotalAmount = c.TotalAmount,
                                }).ToList();
            // SEARCHING...
            if (!string.IsNullOrEmpty(FromAccountNo))
            {
                bulkfileupload = bulkfileupload.Where(a => a.FromAccount.ToLower().Contains(FromAccountNo.ToLower().Trim())).ToList();

            }
            if (fromDate != null & toDate != null)
            {

                bulkfileupload = bulkfileupload.Where(a => a.CreatedDate >= fromDate && a.CreatedDate < toDate).ToList();
            }
            else if (fromDate != null)
            {
                bulkfileupload = bulkfileupload.Where(a => a.CreatedDate >= fromDate).ToList();
            }
            else if (toDate != null)
            {
                bulkfileupload = bulkfileupload.Where(a => a.CreatedDate < toDate).ToList();
            }

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                bulkfileupload = bulkfileupload.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = bulkfileupload.Count();
            List<C_BulkFileUploadVM> data;
            if (pageSize == -1)
            {
                data = bulkfileupload;
            }
            else
            {
                data = bulkfileupload.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }        
        // GET: Details For Bulk Transaction
        public ActionResult BulkDetail(int? id)
        {
            GetStatusddl();
            if (id != null)
            {
                ViewBag.BulkFileID = id.ToString();
            }
            return View();
        }
        [ActionName("BulkDetail")]
        [HttpPost]
        public ActionResult BulkDetails()
        {
            int recordsTotal = 0;
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var strbulkFileID = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            ViewBag.BulkFileID = strbulkFileID;
            int bulkFileID = 0;

            if (!Int32.TryParse(strbulkFileID, out bulkFileID))
            {
                return Json(new { message = "An error occours.", HttpStatusCode.BadRequest });
            }

            var accNo = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var accName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            List<C_BulkFileUploadVM> bulkfileupload = (from c in db.OtherBankTranLogs
                                                       where c.BulkPaymentFileUploadId == bulkFileID
                                                       orderby c.VALUE_DATE descending
                                                       select new C_BulkFileUploadVM
                                                       {
                                                           Id = c.ID,
                                                           AccountNo = c.TO_ACCT,
                                                           AccountName = c.PAYEE_NAME,
                                                           Currency = "MMK",
                                                           TranAmount = c.TRAN_AMT,
                                                           Status = c.Core_status
                                                       }).ToList();
            // SEARCHING...

            if (!string.IsNullOrEmpty(accNo))
            {
                bulkfileupload = bulkfileupload.Where(a => a.AccountNo.ToLower().Trim().Contains(accNo.ToLower().Trim())).ToList();

            }
            if (!string.IsNullOrEmpty(accName))
            {
                bulkfileupload = bulkfileupload.Where(a => a.AccountName.ToLower().Trim().Contains(accName.ToLower().Trim())).ToList();
            }          

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                bulkfileupload = bulkfileupload.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }

            recordsTotal = bulkfileupload.Count();
            List<C_BulkFileUploadVM> data;
            if (pageSize == -1)
            {
                data = bulkfileupload.ToList();
            }
            else
            {
                data = bulkfileupload.Skip(skip).Take(pageSize).ToList();
            }
            TempData["IsBulkFile"] = "true";
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        //SetBulkSuccess
        //Set Success at Core_status
        [HttpPost]
        public JsonResult SetBulkSuccess(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //string contenttype = "Bulk Transaction";
                if (id == null)
                {
                    return Json("Setting status fail!", JsonRequestBehavior.AllowGet);
                }
                id = id.TrimEnd(',');
                string[] IDs = id.Split(',');

                foreach (var item in IDs)
                {                   
                    int Id = int.Parse(item.ToString());
                    var bulkfile = db.C_BulkPaymentFileUploads.Where(b => b.Id == Id && b.TransType == "TransferOtherBank" && b.Status == "Approved" && b.DEV_FLAG == false).FirstOrDefault();
                    if (bulkfile == null)
                    {
                        continue;
                    }
                    //Set if BulkFileUpload Success , change SetStatus = true
                    {
                        bulkfile.IsSetStatus = true;
                        db.Entry(bulkfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Update Success Core_Status in all transaction according to bultfileID
                    var bulktranlog = db.OtherBankTranLogs.Where(o => o.BulkPaymentFileUploadId == Id).ToList();
                    string trandate = "";
                    string valuedate = "";
                    if (bulktranlog.Count > 0)
                    {
                        trandate = bulktranlog.FirstOrDefault().TRAN_DATE;
                        valuedate = bulktranlog.FirstOrDefault().VALUE_DATE;
                    }
                    foreach (var btranlog in bulktranlog)
                    {
                        btranlog.Core_status = "Success";
                        db.Entry(btranlog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Update Success Status in all maker transaction according to bultfileID
                    var makertranlog = db.C_MakerTranLogs.Where(m => m.BulkPaymentFileUploadID != null && m.BulkPaymentFileUploadID == Id).ToList();
                    foreach (var mtranlog in makertranlog)
                    {
                        mtranlog.Status = "Success";
                        db.Entry(mtranlog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    #region sent Mail For Fail
                    //var user = db._Users.Where(u => u.USERID == bulkfile.CreatedUserId).FirstOrDefault();
                    //string getuserid = User.Identity.GetUserId();
                    //string timestamp = General.GetTimestamp(DateTime.Now);

                    //// Email Sent 
                    //if (IsMailSent())
                    //{
                    //    try
                    //    {
                    //        General.SMTPSetting setting = GetEmailSetting();
                    //        //Get Data For Email                           
                    //        if (user.USER_TYPE == "Corporate")
                    //        {
                    //            var trantype = "BulkPaymentCorTran_Success";
                    //            var mailallow = EmailAllow(trantype);
                    //            bool SentMailForCorporateTran = CorporateBulkTransferSuccess(user.FULLNAME, user.EMAIL, trandate, user.USERNAME, /*valuedate,*/ mailallow.Sender_Allow, mailallow.Sender_Email_Allow);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        log.Error(getuserid, controllerName, actionName, ex);
                    //    }
                    //}       
                    #endregion
                    #region noti
                    //Notification notification = new Notification();
                    //notification.TO_USERID = user.USERID.ToString();

                    ////get userid from autocomplete textbox
                    //notification.ACTIVE = true;
                    //notification.DATE = DateTime.Now;
                    //notification.CONTENTTYPE = contenttype;
                    //notification.TITLE = "Testing Title Transfer";
                    //notification.CONTENT = "Testing Content Transfer";
                    //notification.SENDERTYPE = "1";
                    //notification.CreatedDate = DateTime.Now;
                    //notification.CreatedUserId = User.Identity.GetUserId();
                    //db.Notifications.Add(notification);
                    //db.SaveChanges();
                    ////save log;
                    //log.Info(getuserid, controllerName, actionName, 1, notification.ToString());

                    ////Sent Notification
                    //if (user.USER_TYPE == "Retail")
                    //{
                    //    if (obTranlog.TRAN_TYPE == "OtherBankTransfer")
                    //    {
                    //        //Sent Noti For Transfer Debit
                    //        if (!String.IsNullOrEmpty(user.EMAIL) && IsEmailValid(user.EMAIL))
                    //        {
                    //            if (Send_FireBase_Notification(user.USERID, notification.TITLE, notification.CONTENT, notification.CONTENTTYPE))
                    //            {
                    //                //noti send successfully
                    //                log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{user.USERID}\nTitle:{"Testing Title"}\nContent:{"Testing Content"}");
                    //            }
                    //            else
                    //            {
                    //                //noti send fail
                    //                log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{user.USERID}");
                    //            }
                    //        }
                    //    }
                    //    else if (obTranlog.TRAN_TYPE == "OtherBankRemittance")
                    //    {
                    //        if (!String.IsNullOrEmpty(user.USERID) && IsEmailValid(user.USERID))
                    //        {
                    //            if (Send_FireBase_Notification(user.USERID, notification.TITLE, notification.CONTENT, notification.CONTENTTYPE))
                    //            {
                    //                //noti send successfully
                    //                log.Log(getuserid, controllerName, actionName, $"Notification sent successfully.", $"To:{user.USERID}\nTitle:{"Testing Title"}\nContent:{"Testing Content"}");
                    //            }
                    //            else
                    //            {
                    //                //noti send fail
                    //                log.Log(getuserid, controllerName, actionName, $"Notification sent fail.", $"To:{user.USERID}");
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    //log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Set status successfully", $"Bulk ID is {item}.");
                }
                TempData["IsBulkFile"] = "true";
                return Json("Setting status successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        //Set Fail at Core_status
        [HttpPost]
        public JsonResult SetBulkFail(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                id = id.TrimEnd(',');
                string[] IDs = id.Split(',');
                var FailCount = 0;
                string getuserid = User.Identity.GetUserId();
                string timestamp = General.GetTimestamp(DateTime.Now);

                foreach (var item in IDs)
                {
                    var Id = Convert.ToInt32(item.ToString());
                    var obTranlog = db.OtherBankTranLogs.Where(o => o.ID == Id).FirstOrDefault();
                    if (obTranlog == null)
                    {
                        continue;
                    }
                    //Update Success Core_Status in all transaction according to bultfileID
                    if (obTranlog.Core_status == "Fail")
                    {
                        obTranlog.Core_status = "Fail";
                        db.Entry(obTranlog).State = EntityState.Modified;
                        db.SaveChanges();
                        FailCount += 1;
                    }
                    else if(obTranlog.Core_status == "Success")
                    {
                        obTranlog.Core_status = "Success";
                        db.Entry(obTranlog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                  
                   //Update Success Status in all maker transaction according to bultfileID
                   var makertranlog = db.C_MakerTranLogs.Where(m => m.BulkPaymentFileUploadID != null && m.BulkPaymentFileUploadID == obTranlog.BulkPaymentFileUploadId).ToList();
                   //For Email Sent
                    var user = db._Users.Where(u => u.USERID == obTranlog.USERID).FirstOrDefault();
                    TempData["Email"] = user.EMAIL;
                    TempData["Username"] = user.USERNAME;
                    TempData["Fullname"] = user.FULLNAME;
                    if (obTranlog != null)
                    {
                        var trandate = obTranlog.TRAN_DATE;
                        var valuedate = obTranlog.VALUE_DATE;
                    }
                    foreach (var mtranlog in makertranlog)
                    {
                        if (mtranlog.Status == "Fail")
                        {
                            mtranlog.Status = "Fail";
                            db.Entry(mtranlog).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (mtranlog.Status == "Success")
                        {
                            mtranlog.Status = "Success";
                            db.Entry(mtranlog).State = EntityState.Modified;
                            db.SaveChanges();
                        }                       
                    }                    
                    //log
                    log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Set status successfully", $"Bulk ID is {item}.");
                }
        #region sent Mail For Fail
                //if (FailCount > 0)
                //{
                //    // Email Sent                    
                //    if (IsMailSent())
                //    {
                //        try
                //        {
                //            General.SMTPSetting setting = GetEmailSetting();
                //            var trantype = "BulkPaymentCorTran_Fail";
                //            string email = TempData["Email"].ToString();
                //            string username = TempData["Username"].ToString();
                //            string fullname = TempData["Fullname"].ToString();
                //            var mailallow = EmailAllow(trantype);
                //            bool SentMailForCorporateTran = CorporateBulkTransferFail(email,fullname, username/*, user.EMAIL, trandate, valuedate,*/, mailallow.Sender_Allow, mailallow.Sender_Email_Allow);
                //        }
                //        catch (Exception ex)
                //        {
                //            log.Error(getuserid, controllerName, actionName, ex);
                //        }
                //    }
                //}
         #endregion
                //log
                TempData["IsBulkFile"] = "true";
                log.Log(User.Identity.GetUserId(), controllerName, actionName, $"Set status successfully"/*, $"Bulk ID is {item}."*/);
                return Json("Setting status successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), controllerName, actionName, ex);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region get CBMTran_Type
        private void BindCBMTran_Type()
        {
            var lstCBMTranType = db.OtherBankTranLogs.Select(o => o.CBMTran_Type).ToList();
            IEnumerable<SelectListItem> CBMTran_Type = lstCBMTranType.Distinct().Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            });
            ViewBag.CBMTran_Type = CBMTran_Type;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}