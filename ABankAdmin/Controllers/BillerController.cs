using ABankAdmin.Core.Utils;
using ABankAdmin.Models;
using ABankAdmin.Utils;
using ABankAdmin.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Controllers
{
    /// <summary>
    /// BillerController
    /// </summary>
    [Obsolete]
    public class BillerController : Controller
    {
        #region Variable Decliration

        private readonly AdminDBContext db = new AdminDBContext();

        private readonly EventLogController log = new EventLogController();

        //get connection string
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _httpRequestPolicy;

        public BillerController()
        {
            _httpRequestPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));
        }

        #endregion Variable Decliration

        #region Create Biller

        /// <summary>
        /// BillerRegistration
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> BillerRegistration()
        {
            //title
            ViewBag.PageTitle = "Create";

            //getuserid
            string getuserid = User.Identity.GetUserId();

            //Currency
            ViewBag.Currency = new SelectList(await db.Currencies.Where(p => p.DEL_FLAG == "0").Select(s => new
            {
                Code = s.Code
                //Description = s.Description
            }).OrderBy(o => o.Code).AsNoTracking().ToListAsync(),
                "Code", "Code");

            //BillerType
            ViewBag.BillerType = new SelectList(await db.UltTypes.Select(s => new
            {
                TypeName = s.TypeName
                //Description = s.Description
            }).OrderBy(o => o.TypeName).AsNoTracking().ToListAsync(),
                "TypeName", "TypeName");

            //ChargesCode
            var chargescodeList = getChargesCode(getuserid);
            ViewBag.ChargesCode = new SelectList(chargescodeList, "Value", "Text");
            return View();
        }

        /// <summary>
        /// SaveBillerRegistration
        /// </summary>
        /// <param name="model"></param>
        /// <param name="biller_Fields"></param>
        /// <param name="Image"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveBillerRegistration(string model, string biller_Fields, HttpPostedFileBase Image)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();

            //start trannsaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get userid
                    string userId = User.Identity.GetUserId();

                    //biller data
                    var billerVM = JsonConvert.DeserializeObject<BillerVM>(model);

                    //check biller name
                    bool isBillerNameExists = await BillerNameExist(billerVM.Name, null);
                    if (isBillerNameExists)
                    {
                        return new JsonErrorResult(new { error = "Biller Name already exist. Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check biller code
                    bool isBillerCodeExists = await BillerCodeExist(billerVM.BillerCode, null);
                    if (isBillerCodeExists)
                    {
                        return new JsonErrorResult(new { error = "Biller Code already exist. Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check credit account No
                    AccountNoCheckResult resultCreditData = await AccountNoIsValid(billerVM.CreditAccountNo);
                    if (resultCreditData.result is string)
                    {
                        return new JsonErrorResult(new { error = $"Credit Account Number: {resultCreditData.result} Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check charges account no
                    if (!billerVM.ChargesAccountNo.Equals(billerVM.CreditAccountNo))
                    {
                        AccountNoCheckResult resultChargesData = await AccountNoIsValid(billerVM.ChargesAccountNo);
                        if (resultChargesData.result is string)
                        {
                            return new JsonErrorResult(new { error = $"Charges Account Number: {resultChargesData.result} Please check your form again!" }, HttpStatusCode.BadRequest);
                        }
                    }

                    var biller = BillerRequest.GetBiller(billerVM);

                    //check image
                    if (Image != null && Image.ContentLength > 0)
                    {
                        //get file extension
                        var postedFileExtension = Path.GetExtension(Image.FileName).TrimStart('.'); ;
                        //get file name
                        var filename = ConstantValues.GetGUID();// Path.GetFileName(Image.FileName);

                        byte[] fileInBytes = new byte[Image.ContentLength];
                        using (BinaryReader theReader = new BinaryReader(Image.InputStream))
                        {
                            fileInBytes = theReader.ReadBytes(Image.ContentLength);
                        }
                        string fileAsString = Convert.ToBase64String(fileInBytes);
                        //get session id
                        string sessionID = HttpContext.Session.SessionID;
                        //user type
                        string userType = "Admin";

                        string baseAddress = ConstantValues.APIEndPointURL;

                        string requestUri = "CBAPI/Image/SaveImage";

                        string hardCodeIV = ConstantValues.HardCodeIV;

                        string hardCodeKey = ConstantValues.HardCodeKey;

                        var reqModel = BillerRequest.SaveImageRequestModel(hardCodeIV, hardCodeKey, userId, sessionID, userType, fileAsString, filename, postedFileExtension);

                        var response = await SaveImage(baseAddress, requestUri, reqModel, userId, hardCodeIV, hardCodeKey);

                        biller.ImagePath = response?.ImagePath;
                    }

                    biller.Active = true;
                    biller.CREATED_DATETIME = DateTime.Now;
                    biller.CREATED_USER_ID = userId;

                    //biller field data
                    var billerFieldsVM = JsonConvert.DeserializeObject<List<Biller_FieldVM>>(biller_Fields);
                    var billerFields = BillerRequest.GetBillerField(billerFieldsVM, 0);
                    biller.TBL_Biller_Fields = billerFields == null ? null : new Collection<TBL_Biller_Field>(billerFields.ToList());

                    //save to db
                    db.TBL_Billers.Add(biller);
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbContextTransaction.Commit();

                    log.Info(userId, nameof(BillerController), actionName, 1, $"{biller.Name} is added.");
                    return Json(new { title = "Success", message = "Saved Successfully!" });
                }
                catch (Exception ex)
                {
                    //rollback transaction
                    dbContextTransaction.Rollback();
                    log.Error(User.Identity.GetUserId(), nameof(BillerController), actionName, ex);
                    return new JsonErrorResult(new { error = "Error while saving Biller Registration!" });
                }
            }
        }

        #endregion Create Biller

        #region Edit Biller

        /// <summary>
        /// EditBiller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditBiller(int id)
        {
            //title
            ViewBag.PageTitle = "Edit";

            //biller id
            ViewBag.BillerId = id;

            //status id 2 for edit
            ViewBag.BillerStatus = BillerRequest.EDITBILLER;

            //getuserid
            string getuserid = User.Identity.GetUserId();
            //Currency
            ViewBag.Currency = new SelectList(await db.Currencies.Where(p => p.DEL_FLAG == "0").Select(s => new
            {
                Code = s.Code
                //Description = s.Description
            }).OrderBy(o => o.Code).AsNoTracking().ToListAsync(),
                "Code", "Code");

            //BillerType
            ViewBag.BillerType = new SelectList(await db.UltTypes.Select(s => new
            {
                TypeName = s.TypeName
                //Description = s.Description
            }).OrderBy(o => o.TypeName).AsNoTracking().ToListAsync(),
                "TypeName", "TypeName");

            //ChargesCode
            var chargescodeList = getChargesCode(getuserid);
            ViewBag.ChargesCode = new SelectList(chargescodeList, "Value", "Text");

            return View(nameof(BillerRegistration));
        }

        /// <summary>
        /// UpdateBillerRegistration
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="biller_Fields"></param>
        /// <param name="Image"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateBillerRegistration(int id, string model, string biller_Fields, HttpPostedFileBase Image)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string prev_file = string.Empty;

            //start trannsaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get userid
                    string userId = User.Identity.GetUserId();

                    //get Image Path
                    string Image_Path = ConstantValues.Image_Path;

                    //biller data
                    var billerVM = JsonConvert.DeserializeObject<BillerVM>(model);
                    if (id != billerVM.ID)
                    {
                        return new JsonErrorResult(new { error = "Invalid Request!" }, HttpStatusCode.BadRequest);
                    }

                    //check biller name
                    bool isBillerNameExists = await BillerNameExist(billerVM.Name, billerVM.ID);
                    if (isBillerNameExists)
                    {
                        return new JsonErrorResult(new { error = "Biller Name already exist. Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check biller code
                    bool isBillerCodeExists = await BillerCodeExist(billerVM.BillerCode, billerVM.ID);
                    if (isBillerCodeExists)
                    {
                        return new JsonErrorResult(new { error = "Biller Code already exist. Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check credit account No
                    AccountNoCheckResult resultCreditData = await AccountNoIsValid(billerVM.CreditAccountNo);
                    if (resultCreditData.result is string)
                    {
                        return new JsonErrorResult(new { error = $"Credit Account Number: {resultCreditData.result} Please check your form again!" }, HttpStatusCode.BadRequest);
                    }

                    //check charges account no
                    if (!billerVM.ChargesAccountNo.Equals(billerVM.CreditAccountNo))
                    {
                        AccountNoCheckResult resultChargesData = await AccountNoIsValid(billerVM.ChargesAccountNo);
                        if (resultChargesData.result is string)
                        {
                            return new JsonErrorResult(new { error = $"Charges Account Number: {resultChargesData.result} Please check your form again!" }, HttpStatusCode.BadRequest);
                        }
                    }

                    //get biller from db
                    var biller = await db.TBL_Billers.FirstOrDefaultAsync(f => f.ID == id);
                    if (biller == null)
                    {
                        return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                    }

                    //biller field data
                    var billerFieldsVM = JsonConvert.DeserializeObject<List<Biller_FieldVM>>(biller_Fields);
                    var billerFields = BillerRequest.GetBillerField(billerFieldsVM, biller.ID);

                    //delete existing biller field data
                    var db_billerFields = await db.TBL_Biller_Fields.Where(w => w.Biller_Id == biller.ID).ToListAsync();
                    if (db_billerFields != null && db_billerFields.Count != 0)
                    {
                        db.TBL_Biller_Fields.RemoveRange(db_billerFields);
                        await db.SaveChangesAsync();
                    }

                    //check image
                    if (Image != null && Image.ContentLength > 0)
                    {
                        //get file extension
                        var postedFileExtension = Path.GetExtension(Image.FileName).TrimStart('.'); ;
                        //get file name
                        var filename = ConstantValues.GetGUID();// Path.GetFileName(Image.FileName);

                        byte[] fileInBytes = new byte[Image.ContentLength];
                        using (BinaryReader theReader = new BinaryReader(Image.InputStream))
                        {
                            fileInBytes = theReader.ReadBytes(Image.ContentLength);
                        }
                        string fileAsString = Convert.ToBase64String(fileInBytes);
                        //get session id
                        string sessionID = HttpContext.Session.SessionID;
                        //user type
                        string userType = "Admin";

                        string baseAddress = ConstantValues.APIEndPointURL;

                        string requestUri = "CBAPI/Image/SaveImage";

                        string hardCodeIV = ConstantValues.HardCodeIV;

                        string hardCodeKey = ConstantValues.HardCodeKey;

                        var reqModel = BillerRequest.SaveImageRequestModel(hardCodeIV, hardCodeKey, userId, sessionID, userType, fileAsString, filename, postedFileExtension);

                        var response = await SaveImage(baseAddress, requestUri, reqModel, userId, hardCodeIV, hardCodeKey);

                        biller.ImagePath = response?.ImagePath;
                    }

                    biller.Name = billerVM.Name;
                    biller.BillerCode = billerVM.BillerCode;
                    biller.IsApiIntegrate = billerVM.IsApiIntegrate;
                    biller.ChargesAmount = billerVM.ChargesAmount;
                    biller.ChargesAccountNo = billerVM.ChargesAccountNo;
                    biller.ChargesCode = billerVM.ChargesCode;
                    biller.CreditAccountNo = billerVM.CreditAccountNo;
                    biller.isFixRate = billerVM.isFixRate;
                    biller.DiscountAmount = billerVM.DiscountAmount;
                    biller.DiscountPercentage = billerVM.DiscountPercentage;
                    biller.BillerType = billerVM.BillerType;
                    biller.Currency = billerVM.Currency;
                    //biller.Active = true;
                    biller.UPDATED_DATETIME = DateTime.Now;
                    biller.UPDTED_USER_ID = userId;
                    biller.TBL_Biller_Fields = billerFields.ToList();

                    //save data
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbContextTransaction.Commit();

                    log.Info(userId, nameof(BillerController), actionName, 2, $"{biller.Name} is updated.");
                    return Json(new { title = "Success", message = "Updated Successfully!" });
                }
                catch (Exception ex)
                {
                    //rollback transaction
                    dbContextTransaction.Rollback();
                    log.Error(User.Identity.GetUserId(), nameof(BillerController), actionName, ex);
                    return new JsonErrorResult(new { error = "Error while updating Biller Registration!" });
                }
            }
        }

        #endregion Edit Biller

        #region Detail Biller

        /// <summary>
        /// DetailBiller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DetailBiller(int id)
        {
            //title
            ViewBag.PageTitle = "Detail";

            //biller id
            ViewBag.BillerId = id;

            //status id 3 for detail
            ViewBag.BillerStatus = BillerRequest.DETAILBILLER;

            //getuserid
            string getuserid = User.Identity.GetUserId();

            //Currency
            ViewBag.Currency = new SelectList(await db.Currencies.Where(p => p.DEL_FLAG == "0").Select(s => new
            {
                Code = s.Code
                //Description = s.Description
            }).OrderBy(o => o.Code).AsNoTracking().ToListAsync(),
                "Code", "Code");

            //BillerType
            ViewBag.BillerType = new SelectList(await db.UltTypes.Select(s => new
            {
                TypeName = s.TypeName
                //Description = s.Description
            }).OrderBy(o => o.TypeName).AsNoTracking().ToListAsync(),
                "TypeName", "TypeName");

            //ChargesCode
            var chargescodeList = getChargesCode(getuserid);
            ViewBag.ChargesCode = new SelectList(chargescodeList, "Value", "Text");

            return View(nameof(BillerRegistration));
        }

        #endregion Detail Biller

        #region Biller List

        /// <summary>
        /// BillerList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> BillerList()
        {
            //Currency
            ViewBag.Currency = new SelectList(await db.Currencies.Where(p => p.DEL_FLAG == "0").Select(s => new
            {
                Code = s.Code
                //Description = s.Description
            }).OrderBy(o => o.Code).AsNoTracking().ToListAsync(),
                "Code", "Code");

            //BillerType
            ViewBag.BillerType = new SelectList(await db.UltTypes.Select(s => new
            {
                TypeName = s.TypeName
                //Description = s.Description
            }).OrderBy(o => o.TypeName).AsNoTracking().ToListAsync(),
                "TypeName", "TypeName");

            //status
            var statusList = BillerRequest.GetStatusList();
            //var statusList = General.GetStatus();
            ViewBag.Status = new SelectList(statusList, "Value", "Text");

            return View();
        }

        /// <summary>
        /// GetBillerList
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetBillerList(BillerSearchVM data, DataTableAjaxPostModel model)
        {
            try
            {
                int total = 0;
                int pageSize = model.length;
                int skip = model.start;
                int size = model.length;

                // sort Column Name + Sort Column Direction (asc, desc)
                var sortExpression = model.columns[model.order[0].column].name + " " + model.order[0].dir;

                if (pageSize < 0) pageSize = total;

                //query data
                IQueryable<BillerListVM> recordData = (from b in db.TBL_Billers
                                                       select new BillerListVM
                                                       {
                                                           ID = b.ID,
                                                           Name = b.Name,
                                                           BillerCode = b.BillerCode,
                                                           CreditAccountNo = b.CreditAccountNo,
                                                           BillerType = b.BillerType,
                                                           Currency = b.Currency,
                                                           Active = b.Active
                                                       }).OrderBy(sortExpression);

                //check data
                if (!string.IsNullOrEmpty(data.Name))
                {
                    recordData = recordData.Where(w => w.Name.ToUpper().Contains(data.Name.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.BillerCode))
                {
                    recordData = recordData.Where(w => w.BillerCode.ToUpper().Contains(data.BillerCode.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.BillerType))
                {
                    recordData = recordData.Where(w => w.BillerType.ToUpper().Contains(data.BillerType.ToUpper()));
                }

                if (!string.IsNullOrEmpty(data.Currency))
                {
                    recordData = recordData.Where(w => w.Currency.ToUpper().Contains(data.Currency.ToUpper()));
                }

                if (data.Status.HasValue)
                {
                    recordData = recordData.Where(w => w.Active.Equals(data.Status.Value));
                }

                //get total record
                total = await recordData.AsNoTracking().CountAsync();

                //get data by pagination
                var recordDatas = await recordData.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();

                return Json(new { model.draw, recordsFiltered = total, recordsTotal = total, data = recordDatas });
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Biller List!" });
            }
        }

        #endregion Biller List

        #region Update Biller Status

        /// <summary>
        /// EnableBiller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EnableBiller(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get by id
                var biller = await db.TBL_Billers.FindAsync(id);
                if (biller == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                biller.Active = true;
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, nameof(BillerController), actionName, "Enable biller successfully.", $"Biller id is { id }");
                    return Json(new { title = "Success", message = "Enable biller successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), nameof(BillerController), actionName, ex);
                return new JsonErrorResult(new { error = "Error while enabling biller!" });
            }
        }

        /// <summary>
        /// DisableBiller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DisableBiller(int id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            try
            {
                //get login user id
                string LoginUser = User.Identity.GetUserId();

                //get by id
                var biller = await db.TBL_Billers.FindAsync(id);
                if (biller == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                biller.Active = false;
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    //save log
                    log.Log(LoginUser, nameof(BillerController), actionName, "Disable biller successfully.", $"Biller id is { id }");
                    return Json(new { title = "Success", message = "Disable biller successfully!" });
                }
                return new JsonErrorResult(new { error = "Request Fail!" }, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(), nameof(BillerController), actionName, ex);
                return new JsonErrorResult(new { error = "Error while disabling biller!" });
            }
        }

        #endregion Update Biller Status

        #region General

        /// <summary>
        /// CheckBillerName
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckBillerName(string checkData, int? checkDataID)
        {
            try
            {
                bool isDataExists = await BillerNameExist(checkData, checkDataID.GetValueOrDefault());
                return Json(!isDataExists);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while checking Biller Name in Biller List!" });
            }
        }

        /// <summary>
        /// CheckBillerCode
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckBillerCode(string checkData, int? checkDataID)
        {
            try
            {
                bool isDataExists = await BillerCodeExist(checkData, checkDataID.GetValueOrDefault());
                return Json(!isDataExists);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while checking Biller Code in Biller List!" });
            }
        }

        /// <summary>
        /// CheckValidAccNo
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckValidAccNo(string checkData)
        {
            try
            {
                AccountNoCheckResult resultData = await AccountNoIsValid(checkData);
                return Json(resultData);
            }
            catch (Exception)
            {
                return new JsonErrorResult(new { error = "Error while checking Account No.!" });
            }
        }

        /// <summary>
        /// GetBiller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetBiller(int id)
        {
            try
            {
                //get userid
                string userId = User.Identity.GetUserId();
                //get session id
                string sessionID = HttpContext.Session.SessionID;
                //user type
                string userType = "Admin";

                string baseAddress = ConstantValues.APIEndPointURL;

                string requestUri = "CBAPI/Image/GetImage";

                string hardCodeIV = ConstantValues.HardCodeIV;

                string hardCodeKey = ConstantValues.HardCodeKey;

                //get biller by id
                var billerVM = await db.TBL_Billers
                    .AsNoTracking()
                    .Where(w => w.ID == id)
                    .Select(s => new BillerVM
                    {
                        ID = s.ID,
                        Name = s.Name,
                        BillerCode = s.BillerCode,
                        IsApiIntegrate = s.IsApiIntegrate,
                        ChargesAmount = s.ChargesAmount,
                        ChargesAccountNo = s.ChargesAccountNo,
                        ChargesCode = s.ChargesCode,
                        CreditAccountNo = s.CreditAccountNo,
                        isFixRate = s.isFixRate,
                        DiscountAmount = s.DiscountAmount,
                        DiscountPercentage = s.DiscountPercentage,
                        BillerType = s.BillerType,
                        ImagePath = s.ImagePath,
                        Currency = s.Currency,
                        Active = s.Active
                    })
                    .FirstOrDefaultAsync();
                if (billerVM == null)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }

                var reqModel = BillerRequest.GetImageRequestModel(hardCodeIV, hardCodeKey, userId, sessionID, userType, billerVM.ImagePath);

                var response = await GetImage(baseAddress, requestUri, reqModel, userId, hardCodeIV, hardCodeKey);

                billerVM.ImagePath = response?.Base64Image;

                //get biller field
                var billerFields = await db.TBL_Biller_Fields
                    .AsNoTracking()
                    .Include(i => i.Children)
                    .Where(w => w.Biller_Id == billerVM.ID)
                    .ToListAsync();
                if (billerFields == null || billerFields.Count == 0)
                {
                    return new JsonErrorResult(new { error = "Data Not Found!" }, HttpStatusCode.BadRequest);
                }
                var billerFieldsVM = BillerRequest.GetBillerFieldVM(billerFields);

                return Json(new
                {
                    biller = billerVM,
                    billerFields = billerFieldsVM.Where(w => !w.ParentId.HasValue).OrderBy(o => o.SortOrder).ToList()
                });
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(new { error = "Error while loading Biller Data" });
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// BillerNameExist
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        private async Task<bool> BillerNameExist(string checkData, int? checkDataID)
        {
            bool isDataExists = false;
            checkData = checkData.Trim();
            if (checkDataID.HasValue)
            {
                //check in Biller
                isDataExists = await db.TBL_Billers.AsNoTracking().AnyAsync(x =>
                        x.Name.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                        //&& x.Active == true
                        && x.ID != checkDataID.Value
                );
            }
            else
            {
                //check in Biller
                isDataExists = await db.TBL_Billers.AsNoTracking().AnyAsync(x =>
                        x.Name.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                //&& x.Active == true
                );
            }
            //if (!isDataExists)
            //{
            //    //check in Biller
            //    isDataExists = await db.Utilities.AsNoTracking().AnyAsync(x =>
            //            x.Name.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
            //            && x.Active == "1"
            //    );
            //}
            return isDataExists;
        }

        /// <summary>
        /// BillerCodeExist
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="checkDataID"></param>
        /// <returns></returns>
        private async Task<bool> BillerCodeExist(string checkData, int? checkDataID)
        {
            bool isDataExists = false;
            checkData = checkData.Trim();
            if (checkDataID.HasValue)
            {
                //check in Biller
                isDataExists = await db.TBL_Billers.AsNoTracking().AnyAsync(x =>
                        x.BillerCode.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                        //&& x.Active == true
                        && x.ID != checkDataID.Value
                );
            }
            else
            {
                //check in Biller
                isDataExists = await db.TBL_Billers.AsNoTracking().AnyAsync(x =>
                        x.BillerCode.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
                //&& x.Active == true
                );
            }
            //if (!isDataExists)
            //{
            //    //check in Biller
            //    isDataExists = await db.Utilities.AsNoTracking().AnyAsync(x =>
            //            x.Biller_Code.Equals(checkData, StringComparison.CurrentCultureIgnoreCase)
            //            && x.Active == "1"
            //    );
            //}
            return isDataExists;
        }

        /// <summary>
        /// AccountNoIsValid
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        private async Task<AccountNoCheckResult> AccountNoIsValid(string checkData)
        {
            AccountNoCheckResult resultData;
            checkData = checkData.Trim();
#if DEBUG
            if (checkData == "123")
            {
                resultData = new AccountNoCheckResult { result = "Invalid Data" };
            }
            else
            {
                resultData = new AccountNoCheckResult
                {
                    result = true,
                    AccountName = "Test"
                };
            }
#else

            string getuserid = User.Identity.GetUserId(); ; //get login user id
                                                            //GetAccountInfo
            var accData = BillerRequest.GetAccountInfo(checkData, connectionString);
            if (accData.RespCode.Equals("000") && !string.IsNullOrEmpty(accData.FullName))
            {
                resultData = new AccountNoCheckResult
                {
                    result = true,
                    AccountName = accData.FullName
                };
            }
            else
            {
                resultData = new AccountNoCheckResult { result = accData.RespDescription };
                string logMsg = $"{accData.RespCode}, {accData.RespDescription}";
                log.Log(getuserid, nameof(BillerController), "CheckValidAccNo", logMsg);
            }
#endif
            return resultData;
        }

        /// <summary>
        /// getChargesCode
        /// </summary>
        /// <param name="getuserid"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> getChargesCode(string getuserid)
        {
            try
            {
#if DEBUG
                return BillerRequest.GetChargesCodeList_Debug();
#else
                //                //GetChargeRate
                var chargeCodes = BillerRequest.GetChargeRate();
                if (chargeCodes.ChargeCodeInfo == null) //cinfo.ChargeCodeInfo.Count == 0
                {
                    string logMsg = $"{chargeCodes.ResponseCode}, {chargeCodes.ResponseDesc}, ChargeRateInfo obj is null.";
                    log.Log(getuserid, nameof(BillerController), "Select_DynamicBillerChargeRate", logMsg);
                }
                else if (chargeCodes.ChargeCodeInfo != null && chargeCodes.ChargeCodeInfo.Count == 0)
                {
                    string logMsg = $"{chargeCodes.ResponseCode}, {chargeCodes.ResponseDesc}, ChargeRateInfo count is 0.";
                    log.Log(getuserid, nameof(BillerController), "Select_DynamicBillerChargeRate", logMsg);
                }
                else if (chargeCodes.ChargeCodeInfo != null && chargeCodes.ChargeCodeInfo.Count > 0)
                {
                    return chargeCodes.ChargeCodeInfo.Select(m => new SelectListItem
                    {
                        Text = $"{m.ChargeCode} {(m.FixedAmt == "0" ? string.Empty : '(' + m.FixedAmt + ')')} {(m.PercentageAmt == "0" ? string.Empty : '(' + m.PercentageAmt + '%' + ')')}",
                        Value = m.ChargeCode
                    }).OrderBy(o => o.Value).ToList();
                }

                throw new ArgumentException("No Charges Code found!");
#endif
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// SaveImage
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <param name="hardCodeIV"></param>
        /// <param name="hardCodeKey"></param>
        /// <returns></returns>
        private async Task<SaveImageJsonStringResponse> SaveImage(string baseAddress, string requestUri, ImageRequestModel model, string userId, string hardCodeIV, string hardCodeKey)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    HttpResponseMessage httpResponse = await _httpRequestPolicy.ExecuteAsync(() =>
                        client.PostAsJsonAsync(requestUri, model));
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<ImageResponseModel>(response);
                        if (responseModel.RespCode == "000" && !string.IsNullOrEmpty(responseModel.JsonStringResponse))
                        {
                            var responseStr = BillerRequest.DecryptAES(responseModel.JsonStringResponse, hardCodeKey, hardCodeIV);
                            var respModel = JsonConvert.DeserializeObject<SaveImageJsonStringResponse>(responseStr);
                            return respModel;
                        }
                        else
                        {
                            string logMsg = $"{responseModel.RespCode}, {responseModel.RespDescription}, Image is null.";
                            log.Log(userId, nameof(BillerController), "SaveImage", logMsg);
                        }
                    }
                    throw new ArgumentException("Error While Saving Image!");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// GetImage
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <param name="hardCodeIV"></param>
        /// <param name="hardCodeKey"></param>
        /// <returns></returns>
        private async Task<GetImageJsonStringResponse> GetImage(string baseAddress, string requestUri, ImageRequestModel model, string userId, string hardCodeIV, string hardCodeKey)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    HttpResponseMessage httpResponse = await _httpRequestPolicy.ExecuteAsync(() =>
                        client.PostAsJsonAsync(requestUri, model));
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<ImageResponseModel>(response);
                        if (responseModel.RespCode == "000" && !string.IsNullOrEmpty(responseModel.JsonStringResponse))
                        {
                            var responseStr = BillerRequest.DecryptAES(responseModel.JsonStringResponse, hardCodeKey, hardCodeIV);
                            var respModel = JsonConvert.DeserializeObject<GetImageJsonStringResponse>(responseStr);
                            return respModel;
                        }
                        else
                        {
                            string logMsg = $"{responseModel.RespCode}, {responseModel.RespDescription}, Image is null.";
                            log.Log(userId, nameof(BillerController), "GetImage", logMsg);
                        }
                    }
                    throw new ArgumentException("Error While Getting Image!");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion General
    }
}