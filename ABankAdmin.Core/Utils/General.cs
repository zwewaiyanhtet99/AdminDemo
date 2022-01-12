using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web.Mvc;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace ABankAdmin.Core.Utils
{
    public class General
    {


        //get time stamp with "yyMMddHHmmss" format
        public static string GetTimestamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss"); 
        }
        //get time stamp with "yyMMddHHmmss" format
        public static string GetDatestamp(DateTime time)
        {
            return time.ToString("ddMMyy");
        }
        //get datetime from string format        
        public static DateTime ConvertStrToDate(String StrDate)
        {
            //DateTime date;
            return DateTime.ParseExact(StrDate.Trim(), ConstantValues.Date_Format, CultureInfo.InvariantCulture);
        }
        //get datetime from string format for iconic     
        public static DateTime ConvertStrToDateForIconic(String StrDate)
        {
            //DateTime date;
            return DateTime.ParseExact(StrDate.Trim(), ConstantValues.Date_FormatForIconic, CultureInfo.InvariantCulture);
        }
        //get datetime from string format (date + time)       
        public static DateTime ConvertStrToDateTime(String StrDate)
        {
            //DateTime date;
            return DateTime.ParseExact(StrDate.Trim(), ConstantValues.Date_Time_Format, CultureInfo.InvariantCulture);
        }

        //convert from string format to (int)       
        public static int ConvertStrToInt(String StrValue)
        {
            try
            {
                return int.Parse(StrValue, NumberStyles.AllowThousands);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //convert from int format to (string)       
        public static string ConvertIntToStr(int? IntValue)
        {
            return string.Format("{0:N0}", IntValue);
        }

        //convert from string format to (decimal)       
        public static decimal ConvertStrToDecimal(String StrValue)
        {
            return decimal.Parse(StrValue, NumberStyles.AllowThousands|NumberStyles.AllowDecimalPoint);
        }
        //convert from decimal format to (Integer)       
        public static int ConvertDecimalToInteger(String StrValue)
        {
            return int.Parse(StrValue, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
        }
        //convert from decimal format to (string)       
        public static string ConvertDecimalToStr(Decimal? DecValue)
        {
            return string.Format("{0:N2}", DecValue);
        }
        public static string ConvertNDecimalToStr(Decimal? DecValue)
        {
            return string.Format("{0:N2}", DecValue);
        }

        //convert from c approve rule decimal format to (string)       
        public static string ConvertCDecimalToStr(Decimal DecValue)
        {
            return string.Format("{0:N0}", DecValue);
        }
        //convert from c approve rule decimal format to (string)       
        public static string ConvertCNDecimalToStr(Decimal? DecValue)
        {
            return string.Format("{0:N0}", DecValue);
        }
        //to save old pwd into old pwd 1,2,3
        public static void UpdateOldPwds(string oldpwd, ref string old1, ref string old2, ref string old3)
        {
            old3 = old2;//first, 2 is moved into 3.
            old2 = old1;//second, 1 is moved into 2.
            old1 = oldpwd;//finally, old pwd is set at 1.
        }

        //For report ddl
        //Trasaction Type
        public static List<SelectListItem> GetTranType()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Transfer Own",Value="TranOwn"},
                    new SelectListItem{Text="Transfer Other",Value="TranOther"},
                    new SelectListItem{Text="Remittance",Value="Remit"},
                    new SelectListItem{Text="Bill Payment",Value="BillPayment"},
                    new SelectListItem{Text="TopUp",Value="TopUp"}
                };
        }

        //Trasaction DDL
        public static List<SelectListItem> GetTransferDDL()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Transfer Own",Value="TranOwn"},
                    new SelectListItem{Text="Transfer Other",Value="TranOther"},
                    new SelectListItem{Text="All",Value="TranAll"}
                };
        }

        //Trasaction DDL
        public static List<SelectListItem> GetUtilityDDL()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Bill Payment",Value="BillPayment"},
                    new SelectListItem{Text="TopUp",Value="TopUp"}
                };
        }
        //Debit or Credit
        public static List<SelectListItem> GetDebitOrCredit()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Both",Value="Both"},
                    new SelectListItem{Text="Debit",Value="Debit"},
                    new SelectListItem{Text="Credit",Value="Credit"}
                };
        }
        //TransactionStatus
        public static List<SelectListItem> GetTransaction_Status()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="All",Value="ALL"},
                    new SelectListItem{Text="Pending",Value="Pending"},
                    new SelectListItem{Text="Success",Value="Success"},
                    new SelectListItem{Text="Fail",Value="Fail"}
                };
        }

        // Bulk Payment TransactionStatus
        public static List<SelectListItem> BulkTransaction_Status()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Approved",Value="Approved"},
                    new SelectListItem{Text="Rejected",Value="Rejected"},
                };
        }

        #region Corporate and Corporate User Status
        public static List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Requested",Value="0"},
                    new SelectListItem{Text="Approved",Value="1"},
                    new SelectListItem{Text="Rejected",Value="2"},
                    new SelectListItem{Text="All",Value=null}
                };
        }
        #endregion
        public static List<SelectListItem> GetRequestStatus()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="Requested",Value="0"},
                    new SelectListItem{Text="Approved",Value="1"},
                    new SelectListItem{Text="Rejected",Value="2"},
                    new SelectListItem{Text="All",Value=null}
                };
        }

        //C_Approve Rule/PaymentType       

        #region Corporate Type
        public static List<SelectListItem> GetTypes()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem {Text="Register",Value="0"},
                    new SelectListItem {Text="Update",Value="1"},
                    new SelectListItem {Text="Delete",Value="2"}
                };
        }
        #endregion

        #region Corporate Payment Type
        public static List<SelectListItem> GetTransactionTypes()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{ Text="Select TranType", Value= null},
                    new SelectListItem {Text="Normal", Value= "0"},
                    new SelectListItem {Text="Bulk", Value= "1"}
                };
        }

        public static List<SelectListItem> GetPaymentTypes()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{ Text="Select TranType", Value= null},
                    new SelectListItem {Text="Normal",Value="False"},
                    new SelectListItem {Text="Bulk",Value="True"}
                };
        }
        #endregion

        #region Corporate account Type
        public static List<SelectListItem> GetCorporateType()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem {Text="Normal",Value="0"},
                    new SelectListItem {Text="VIP",Value="1"},
                };
        }
        #endregion


        #region Corporate User Type
        public static List<SelectListItem> GetUserType()
        {
            return new List<SelectListItem>
                {
                new SelectListItem {Text="Register",Value="True"},
                new SelectListItem {Text="Update",Value="False"}
                };
        }
        #endregion

        #region Corporate Changes Type
        public static List<SelectListItem> GetChangesType()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem {Text="Reset Password",Value="0"},
                    new SelectListItem {Text="Lock",Value="1"},
                    new SelectListItem {Text="Unlock",Value="2"},
                    new SelectListItem {Text="Tran Lock",Value="3"},
                    new SelectListItem {Text="Tran Unlock",Value="4"}
                };
        }
        #endregion

        #region Corporate User Changes Type
        public static List<SelectListItem> GetUserChangesType()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem {Text="Reset Password",Value="2"},
                    new SelectListItem {Text="Lock",Value="3"},
                    new SelectListItem {Text="Unlock",Value="4"},
                    new SelectListItem {Text="Tran Lock",Value="5"},
                    new SelectListItem {Text="Tran Unlock",Value="6"},
                    new SelectListItem {Text="Delete",Value="7"}
                };
        }
        #endregion

        #region No of Approver DDL
        public static List<SelectListItem> GetNoOfApprover()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{Text="0",Value="0"},
                    new SelectListItem{Text="1",Value="1"},
                    new SelectListItem{Text="2",Value="2"},
                    new SelectListItem{Text="3",Value="3"},
                    new SelectListItem{Text="4",Value="4"},
                    new SelectListItem{Text="5",Value="5"},
                    new SelectListItem{Text="6",Value="6"},
                    new SelectListItem{Text="7",Value="7"},
                    new SelectListItem{Text="8",Value="8"},
                    new SelectListItem{Text="9",Value="9"},
                    new SelectListItem{Text="10",Value="10"}
                };
        }
        #endregion

        //Sent Email
        public static Boolean SendEmail(string receiver, string subject, string message, string filename, int status,int userstatus, SMTPSetting setting)
        {
            try
            {
                string type = "";
                if (status == 1)
                {
                    type = "reset password";
                }
                else
                {
                    type = "register";
                }
                string Pdf_Path = ConstantValues.Pdf_Path;
                string CorporatePdf_Path = ConstantValues.CorporatePdf_Path;
                if(userstatus == 0)
                {
                    Pdf_Path = $"{Pdf_Path}\\{type}";
                }
                else
                {
                    CorporatePdf_Path = $"{CorporatePdf_Path}\\{type}";
                }
                var senderEmail = new MailAddress(setting.SenderMail, setting.SenderName);
                var receiverEmail = new MailAddress(receiver, receiver);
                var password = setting.Password;
                var sub = subject;
                var body = message;

                var path = "";
                //for retail user
                if (userstatus == 0) { path = $"{Pdf_Path}\\{filename}"; }
                //for corporate user
                else { path = $"{CorporatePdf_Path}\\{filename}"; }

                var att = new Attachment(path);

                var smtp = new SmtpClient
                {
                    Host = setting.Host,
                    Port = setting.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password),
                    Timeout = 10000
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    mess.Attachments.Add(att);
                    smtp.Send(mess);
                }
                return true;
            }
            catch (SmtpFailedRecipientsException smtpfrex)
            {
                throw smtpfrex;
            }
            catch (SmtpException smtpex)
            {

                throw smtpex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static Boolean SendEmailForCCT(string receiver, string subject, string message, SMTPSetting setting)
        {
            try
            {                
                var senderEmail = new MailAddress(setting.SenderMail, setting.SenderName);
                var receiverEmail = new MailAddress(receiver, receiver);
                var password = setting.Password;
                var sub = subject;
                var body = message;

                var smtp = new SmtpClient
                {
                    Host = setting.Host,
                    Port = setting.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password),
                    Timeout = 10000
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
                return true;
            }
            catch (SmtpFailedRecipientsException smtpfrex)
            {
                throw smtpfrex;
            }
            catch (SmtpException smtpex)
            {

                throw smtpex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public class SMTPSetting
        {
            [Required]
            [EmailAddress]
            public string SenderMail { get; set; }
            [Required]
            public string SenderName { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        //pro.PropertyType.GetDefaultMembers.
                        var type = pro.PropertyType.Name;
                        //pro.SetValue(obj, (ConvertFromDBVal<type>(dr[column.ColumnName])), null);
                        pro.SetValue(obj, (dr[column.ColumnName] == DBNull.Value ? null : dr[column.ColumnName]), null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
        private static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
        private DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
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

        public static Boolean SendEmailForICONIC(string receiver, string subject, string message,SMTPSetting setting)
        {
            try
            {
                var senderEmail = new MailAddress(setting.SenderMail, setting.SenderName);
                var receiverEmail = new MailAddress(receiver, receiver);
                var password = setting.Password;
                var sub = subject;
                var body = message;

                var smtp = new SmtpClient
                {
                    Host = setting.Host,
                    Port = setting.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password),
                    Timeout = 10000
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                   smtp.Send(mess);
                }
                return true;
            }
            catch (SmtpFailedRecipientsException smtpfrex)
            {
                throw smtpfrex;
            }
            catch (SmtpException smtpex)
            {

                throw smtpex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static Boolean SendEmailForICONICReport(string receiver, string subject, string message, string filename, SMTPSetting setting)
        {
            try
            {
                var senderEmail = new MailAddress(setting.SenderMail, setting.SenderName);
                var receiverEmail = new MailAddress(receiver, receiver);
                var password = setting.Password;
                var sub = subject;
                var body = message;
                string excelPath = ConstantValues.CSVIconicFile_Path;
                var path = $"{excelPath}\\{filename}"; 
                var att = new Attachment(path);
                var smtp = new SmtpClient
                {
                    Host = setting.Host,
                    Port = setting.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password),
                    Timeout = 10000
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    mess.Attachments.Add(att);
                    smtp.Send(mess);
                }
                return true;
            }
            catch (SmtpFailedRecipientsException smtpfrex)
            {
                throw smtpfrex;
            }
            catch (SmtpException smtpex)
            {

                throw smtpex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
