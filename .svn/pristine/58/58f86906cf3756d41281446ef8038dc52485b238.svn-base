using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AbankAdminAPI.Models;
using AbankAdminAPI;
using AbankAdminAPI.BusinessLogic;
using System.Web.Mvc;
using ABankAdmin.Models;
using ABankAdmin.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

namespace ABankAdmin.Utils
{
    /// <summary>
    /// BillerRequest
    /// </summary>
    public static class BillerRequest
    {
        /// <summary>
        /// GetChargeRate
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static ChargeRateInfo GetChargeRate()
        {
            ChargeRateInfo codeInfo = new ChargeRateInfo();
            try
            {
                ChargeRateInquiryLogic rateInquiryLogic = new ChargeRateInquiryLogic();
                codeInfo = rateInquiryLogic.Select_DynamicBillerChargeRate();
            }
            catch (Exception ex)
            {
                throw;
            }
            return codeInfo;
        }

        /// <summary>
        /// GetAccountInfo
        /// </summary>
        /// <param name="AccId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Obsolete]
        public static AccountDetailInfo GetAccountInfo(string AccId, string connectionString)
        {
            AccountDetailInfo accountInfo = new AccountDetailInfo();
            try
            {
                CustomerInformationInquiry informationInquiry = new CustomerInformationInquiry();
                accountInfo = informationInquiry.AccountInquiry(AccId, connectionString);
            }
            catch (Exception ex)
            {
                throw;
            }
            return accountInfo;
        }

        /// <summary>
        /// GetChargesCodeList_Debug
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetChargesCodeList_Debug()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem {Text="Test 1",Value="1"},
                new SelectListItem {Text="Test 2",Value="2"}
            };
        }

        /// <summary>
        /// GetBillerFieldVM
        /// </summary>
        /// <param name="biller_Fields"></param>
        /// <returns></returns>
        public static IEnumerable<Biller_FieldVM> GetBillerFieldVM(IEnumerable<TBL_Biller_Field> biller_Fields)
        {
            return biller_Fields
                .Select(s => new Biller_FieldVM
                {
                    ID = s.ID,
                    Biller_Id = s.Biller_Id,
                    FieldName = s.FieldName,
                    FieldType = s.FieldType,
                    LableName = s.LableName,
                    LableNameMM = ToEmpty(s.LableNameMM),
                    DefaultValue = ToEmpty(s.DefaultValue),
                    ParentId = s.ParentId,
                    Placeholder = ToEmpty(s.Placeholder),
                    Attributes = GetAttributesVM(s.Attributes),
                    IsInput = s.IsInput,
                    IsOutput = s.IsOutput,
                    SortOrder = s.SortOrder,
                    IsHidden = s.IsHidden,
                    Children = s.Children == null ? null : GetBillerFieldVM(s.Children).OrderBy(or => or.SortOrder).ToList()
                })
                .OrderBy(o => o.SortOrder)
                .ToList();
        }

        /// <summary>
        /// GetBillerField
        /// </summary>
        /// <param name="biller_Fields"></param>
        /// <param name="billerId"></param>
        /// <returns></returns>
        public static IEnumerable<TBL_Biller_Field> GetBillerField(IEnumerable<Biller_FieldVM> biller_Fields, int billerId)
        {
            return biller_Fields.Select(s => new TBL_Biller_Field
            {
                Biller_Id = billerId,
                FieldName = s.FieldName,
                FieldType = s.FieldType,
                LableName = s.LableName,
                LableNameMM = ToNull(s.LableNameMM),
                DefaultValue = ToNull(s.DefaultValue),
                ParentId = s.ParentId,
                Placeholder = ToNull(s.Placeholder),
                Attributes = GetAttributes(s.Attributes),
                IsInput = s.IsInput,
                IsOutput = s.IsOutput,
                SortOrder = s.SortOrder,
                IsHidden = s.IsHidden,
                Children = s.Children == null ? null : new Collection<TBL_Biller_Field>(GetBillerField(s.Children, billerId).ToList())
            }).ToList();
        }

        /// <summary>
        /// ToNull
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// ToEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value;
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static string GetAttributes(Attributes attributes)
        {
            var attributesConvert = new AttributesConvert
            {
                maximum_length = attributes.MaxLength,
                minimum_length = attributes.MinLength,
                required = attributes.Required
            };
            if (attributesConvert != null)
            {
                return JsonConvert.SerializeObject(attributesConvert);
            }
            return null;
        }

        /// <summary>
        /// GetAttributesVM
        /// </summary>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static Attributes GetAttributesVM(string convert)
        {
            var data = JsonConvert.DeserializeObject<AttributesConvert>(convert);
            var attributes = new Attributes
            {
                MaxLength = data.maximum_length,
                MinLength = data.minimum_length,
                Required = data.required
            };
            return attributes;
        }

        /// <summary>
        /// GetBiller
        /// </summary>
        /// <param name="biller"></param>
        /// <returns></returns>
        public static TBL_Biller GetBiller(BillerVM biller)
        {
            return new TBL_Biller
            {
                ID = biller.ID.GetValueOrDefault(),
                Name = biller.Name,
                BillerCode = biller.BillerCode,
                IsApiIntegrate = biller.IsApiIntegrate,
                ChargesAmount = biller.ChargesAmount,
                ChargesAccountNo = biller.ChargesAccountNo,
                ChargesCode = biller.ChargesCode,
                CreditAccountNo = biller.CreditAccountNo,
                isFixRate = biller.isFixRate,
                DiscountAmount = biller.DiscountAmount,
                DiscountPercentage = biller.DiscountPercentage,
                BillerType = biller.BillerType,
                //ImagePath = biller.ImagePath,
                Currency = biller.Currency
            };
        }

        /// <summary>
        /// GetStatusList
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetStatusList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem{Text="All",Value=null},
                new SelectListItem{Text="Inactive",Value="False"},
                new SelectListItem{Text="Active",Value="True"}
            };
        }

        /// <summary>
        /// GetBillerTypeList
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetBillerTypeList()
        {
            return new List<SelectListItem>()
            {
                //new SelectListItem{Text="All",Value=null},
                new SelectListItem{Text="Fix Amount",Value="0"},
                new SelectListItem{Text="Charges Code",Value="1"},
                new SelectListItem{Text="Discount",Value="2"},
                new SelectListItem{Text="All",Value=null}
            };
        }

        /// <summary>
        /// EncryptAES
        /// </summary>
        /// <param name="text"></param>
        /// <param name="inputKey"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static string EncryptAES(string text, string inputKey, string IV)
        {
            var md5 = new MD5CryptoServiceProvider();
            var password = md5.ComputeHash(Encoding.ASCII.GetBytes(inputKey));
            var IVBytes = Encoding.UTF8.GetBytes(IV);

            //Initialize objects
            var cipher = new RijndaelManaged();
            var encryptor = cipher.CreateEncryptor(password, IVBytes);

            try
            {
                //var buffer = Encoding.ASCII.GetBytes(text);
                var buffer = Encoding.UTF8.GetBytes(text);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("inputCount uses an invalid value or inputBuffer has an invalid offset length. " + ae);
                return null;
            }
            catch (ObjectDisposedException oe)
            {
                Console.WriteLine("The object has already been disposed." + oe);
                return null;
            }
        }

        /// <summary>
        /// DecryptAES
        /// </summary>
        /// <param name="text"></param>
        /// <param name="inputKey"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static string DecryptAES(string text, string inputKey, string IV)
        {
            var md5 = new MD5CryptoServiceProvider();
            var password = md5.ComputeHash(Encoding.ASCII.GetBytes(inputKey));
            var IVBytes = Encoding.UTF8.GetBytes(IV);

            //Initialize objects
            var cipher = new RijndaelManaged();
            var decryptor = cipher.CreateDecryptor(password, IVBytes);

            try
            {
                byte[] input = Convert.FromBase64String(text);

                var newClearData = decryptor.TransformFinalBlock(input, 0, input.Length);
                //return Encoding.ASCII.GetString(newClearData);
                return Encoding.UTF8.GetString(newClearData);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("inputCount uses an invalid value or inputBuffer has an invalid offset length. " + ae);
                return null;
            }
            catch (ObjectDisposedException oe)
            {
                Console.WriteLine("The object has already been disposed." + oe);
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// GetImageRequestModel
        /// </summary>
        /// <param name="hardCodeIV"></param>
        /// <param name="hardCodeKey"></param>
        /// <param name="userId"></param>
        /// <param name="sessionID"></param>
        /// <param name="userType"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static ImageRequestModel GetImageRequestModel(string hardCodeIV, string hardCodeKey, string userId, string sessionID, string userType, string imagePath)
        {
            var jsonReq = new GetImageJsonStringRequest
            {
                UserID = userId,
                SessionID = sessionID,
                ImagePath = imagePath
            };
            string jsonReqStr = JsonConvert.SerializeObject(jsonReq);
            jsonReqStr = EncryptAES(jsonReqStr, hardCodeKey, hardCodeIV);

            userId = EncryptAES(userId, hardCodeKey, hardCodeIV);
            sessionID = EncryptAES(sessionID, hardCodeKey, hardCodeIV);
            userType = EncryptAES(userType, hardCodeKey, hardCodeIV);
            string encIV = EncryptAES(hardCodeIV, hardCodeKey, hardCodeIV);

            return new ImageRequestModel
            {
                UserId = userId,
                SessionID = sessionID,
                UserType = userType,
                IV = encIV,
                JsonStringRequest = jsonReqStr
            };
        }

        /// <summary>
        /// SaveImageRequestModel
        /// </summary>
        /// <param name="hardCodeIV"></param>
        /// <param name="hardCodeKey"></param>
        /// <param name="userId"></param>
        /// <param name="sessionID"></param>
        /// <param name="userType"></param>
        /// <param name="base64Image"></param>
        /// <param name="imageName"></param>
        /// <param name="imageFileType"></param>
        /// <returns></returns>
        public static ImageRequestModel SaveImageRequestModel(string hardCodeIV, string hardCodeKey, string userId, string sessionID, string userType, string base64Image, string imageName, string imageFileType)
        {
            var jsonReq = new SaveImageJsonStringRequest
            {
                UserID = userId,
                SessionID = sessionID,
                Base64Image = base64Image,
                ImageName = imageName,
                ImageFileType = imageFileType
            };
            string jsonReqStr = JsonConvert.SerializeObject(jsonReq);
            jsonReqStr = EncryptAES(jsonReqStr, hardCodeKey, hardCodeIV);

            userId = EncryptAES(userId, hardCodeKey, hardCodeIV);
            sessionID = EncryptAES(sessionID, hardCodeKey, hardCodeIV);
            userType = EncryptAES(userType, hardCodeKey, hardCodeIV);
            string encIV = EncryptAES(hardCodeIV, hardCodeKey, hardCodeIV);

            return new ImageRequestModel
            {
                UserId = userId,
                SessionID = sessionID,
                UserType = userType,
                IV = encIV,
                JsonStringRequest = jsonReqStr
            };
        }

        #region Constant

        public const string DATEFORMAT = "dd-MM-yyyy";
        public const int CREATEBILLER = 1;
        public const int EDITBILLER = 2;
        public const int DETAILBILLER = 3;
        public const string BILLERIMAGEPATH = @"\Images\Biller\";

        #endregion Constant

        /// <summary>
        /// BillerType
        /// </summary>
        public enum BillerType
        {
            FixAmount,
            ChargesCode,
            Discount
        }
    }
}