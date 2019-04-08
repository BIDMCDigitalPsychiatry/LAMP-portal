using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using System.Globalization;
using LAMP.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace LAMP.Service
{
    /// <summary>
    /// RequestService is responsible for handling user webapi functions
    /// </summary>
    public class UserService : IUserService
    {
        #region Variables
        private IUnitOfWork _UnitOfWork;
        private IAccountService _AccountService;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="unitOfWork">unitOfWork</param>
        /// <param name="accountService">account Service</param>
        public UserService(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            _UnitOfWork = unitOfWork;
            _AccountService = accountService;
        }

        #endregion

        #region Api Methods

        /// <summary>
        /// User Sign in
        /// </summary>
        /// <param name="request">Sign in request</param>
        /// <returns>User setting details</returns>
        public UserLoginResponse UserSignIn(UserSignInRequest request)
        {
            string _passwordSalt = string.Empty;
            var response = new UserLoginResponse();
            try
            {
                if (request != null)
                {
                    User user = null;

                    string encriptedUsername = string.Empty;
                    if (Helper.IsValidEmail(request.Username) == true)
                    {
                        encriptedUsername = CryptoUtil.EncryptInfo(request.Username.Trim().ToLower());
                        user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.Email == encriptedUsername && u.IsDeleted != true);
                    }
                    else
                    {
                        encriptedUsername = CryptoUtil.EncryptInfo(request.Username);
                        user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.StudyId == encriptedUsername && u.IsDeleted != true);
                    }

                    if (user != null && (user.Status == null || user.Status == false))
                    {
                        response = new UserLoginResponse
                        {
                            ErrorCode = LAMPConstants.API_USER_INACTIVE,
                            ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_INACTIVE)
                        };
                        return response;
                    }

                    if (user != null)
                    {

                        if (CryptoUtil.DecryptStringWithKey(user.Password) == request.Password.Trim() && user.IsDeleted == false)
                        {
                            response.UserId = user.UserID;
                            response.StudyId = CryptoUtil.DecryptInfo(user.StudyId);
                            response.Email = CryptoUtil.DecryptInfo(user.Email);
                            response.Type = user.IsGuestUser == true ? (byte)UserType.Guest : (byte)UserType.User;
                            response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
                            updateUserLanguageSettings(user.UserID, request.Language);
                            UserSettingData userSettings = GetSettingForUser(user.UserID);
                            if (userSettings == null || userSettings.UserID == 0)
                            {
                                string language = "en";
                                if (request.Language != null && request.Language.Trim().Length > 0)
                                    language = request.Language.Trim();
                                userSettings = SaveDefaultUserSetting(user.UserID, language);
                            }
                            response.Data = userSettings;
                            // Update user App version
                            user.APPVersion = CryptoUtil.EncryptInfo(request.APPVersion.Trim());
                            user.EditedOn = DateTime.UtcNow;
                            _UnitOfWork.IUserRepository.Update(user);

                            // Update user device details
                            UserDevice userDevice = null;
                            userDevice = _UnitOfWork.IUserDeviceRepository.SingleOrDefault(u => u.UserID == user.UserID);
                            if (userDevice == null)
                                userDevice = new UserDevice();
                            userDevice.DeviceType = request.DeviceType;
                            userDevice.DeviceID = CryptoUtil.EncryptInfo(request.DeviceID.Trim());
                            userDevice.DeviceToken = CryptoUtil.EncryptInfo(request.DeviceToken.Trim());
                            userDevice.LastLoginOn = DateTime.UtcNow;
                            if (userDevice.UserID == 0)
                            {
                                userDevice.UserID = user.UserID;
                                _UnitOfWork.IUserDeviceRepository.Add(userDevice);
                            }
                            else
                                _UnitOfWork.IUserDeviceRepository.Update(userDevice);
                            _UnitOfWork.Commit();

                            // Bind Activity Points
                            response.ActivityPoints.SurveyPoint = Convert.ToDecimal(_UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints._3DFigurePoint = Convert.ToDecimal(_UnitOfWork.I3DFigureResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.CatAndDogPoint = Convert.ToDecimal(_UnitOfWork.ICatAndDogResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);                            
                            response.ActivityPoints.DigitSpanForwardPoint = Convert.ToDecimal(_UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID && w.Type == (byte)DigitalSpanType.Forward).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.DigitSpanBackwardPoint = Convert.ToDecimal(_UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID && w.Type == (byte)DigitalSpanType.Backward).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.NBackPoint = Convert.ToDecimal(_UnitOfWork.INBackResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.Serial7Point = Convert.ToDecimal(_UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.SimpleMemoryPoint = Convert.ToDecimal(_UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.SpatialForwardPoint = Convert.ToDecimal(_UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID && w.Type == (byte)SpatialType.Forward).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.SpatialBackwardPoint = Convert.ToDecimal(_UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID && w.Type == (byte)SpatialType.Backward).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.TrailsBPoint = Convert.ToDecimal(_UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.VisualAssociationPoint = Convert.ToDecimal(_UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.CatAndDogNewPoint = Convert.ToDecimal(_UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.TemporalOrderPoint = Convert.ToDecimal(_UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.NBackNewPoint = Convert.ToDecimal(_UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.TrailsBNewPoint = Convert.ToDecimal(_UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.TrailsBDotTouchPoint = Convert.ToDecimal(_UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.JewelsTrailsAPoint = Convert.ToDecimal(_UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);
                            response.ActivityPoints.JewelsTrailsBPoint = Convert.ToDecimal(_UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(w => w.UserID == user.UserID).Sum(s => s.Point) ?? 0);

                            IEnumerable<CognitionJewelsTrailsADetail> enumCTest_JewelsTrailsAResultDetailList = null;
                            enumCTest_JewelsTrailsAResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == user.UserID).ToList()
                                 .Select(c => new CognitionJewelsTrailsADetail()
                                 {
                                     TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                                     TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                                 });

                            IEnumerable<CognitionJewelsTrailsBDetail> enumCTest_JewelsTrailsBResultDetailList = null;
                            enumCTest_JewelsTrailsBResultDetailList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == user.UserID).ToList()
                                 .Select(c => new CognitionJewelsTrailsBDetail()
                                 {
                                     TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                                     TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                                 });

                            response.JewelsPoints.JewelsTrailsATotalBonus = enumCTest_JewelsTrailsAResultDetailList.Sum(s => s.TotalBonusCollected) ?? 0;
                            response.JewelsPoints.JewelsTrailsATotalJewels = enumCTest_JewelsTrailsAResultDetailList.Sum(s => s.TotalJewelsCollected) ?? 0;
                            response.JewelsPoints.JewelsTrailsBTotalBonus = enumCTest_JewelsTrailsBResultDetailList.Sum(s => s.TotalBonusCollected) ?? 0;
                            response.JewelsPoints.JewelsTrailsBTotalJewels = enumCTest_JewelsTrailsBResultDetailList.Sum(s => s.TotalJewelsCollected) ?? 0;

                        }
                        else if (CryptoUtil.DecryptStringWithKey(user.Password) == request.Password.Trim() && user.IsDeleted == true)
                        {
                            response.ErrorCode = LAMPConstants.API_USER_ACCOUNT_DELETED;
                            response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_ACCOUNT_DELETED);
                        }
                        else
                        {
                            response.ErrorCode = LAMPConstants.API_USER_LOGGED_IN_FAILED;
                            response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_LOGGED_IN_FAILED);
                        }
                    }
                    else
                    {

                        response.ErrorCode = LAMPConstants.API_USER_LOGGED_IN_FAILED;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_LOGGED_IN_FAILED);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserSignIn : " + ex);
                response = new UserLoginResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        ///  Update Lagugage settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Language"></param>
        private void updateUserLanguageSettings(long UserId, string Language)
        {
            if (!string.IsNullOrEmpty(Language))
            {
                UserSetting userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == UserId);
                if (userSetting != null)
                {
                    userSetting.Language = Language;
                    _UnitOfWork.IUserSettingRepository.Update(userSetting);

                }
            }
        }

        /// <summary>
        /// Send Password on mail
        /// </summary>
        /// <param name="request">Reset password request</param>
        /// <returns>Status</returns>
        public APIResponseBase SendPasswordOnEmail(ForgotPasswordRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    User user = null;

                    string encriptedEmail = CryptoUtil.EncryptInfo(request.Email.Trim().ToLower());
                    user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.Email == encriptedEmail);
                    if (user != null)
                    {
                        EmailData Edata = new EmailData();
                        Edata.Email = CryptoUtil.DecryptInfo(request.Email);
                        Edata.Subject = "LAMP - Your Password";
                        Edata.TemplateName = LAMPConstants.HTML_RESOURCE_FORGOTPASSWORD;
                        var lampHost = ConfigurationManager.AppSettings["lampHost"].ToString();
                        Edata.Data.Add(new replaceingData { Name = "LAMP_LOGO", Value = lampHost + LAMPConstants.LOGO_PATH });
                        Edata.Data.Add(new replaceingData { Name = "USER_NAME", Value = CryptoUtil.DecryptInfo(user.FirstName) + " " + CryptoUtil.DecryptInfo(user.LastName) });
                        Edata.Data.Add(new replaceingData { Name = "USER_PASSWORD", Value = CryptoUtil.DecryptStringWithKey(user.Password) });
                        Helper.SendEmail(Edata);
                    }
                    else
                    {
                        response.ErrorCode = LAMPConstants.INVALID_EMAIL;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.INVALID_EMAIL);
                    }
                }
                else
                {
                    LogUtil.Error("UserService/SendPasswordOnEmail: " + ResourceHelper.GetStringResource(LAMPConstants.API_INVALID_INPUT_DATA));
                    response = new APIResponseBase
                    {
                        ErrorCode = LAMPConstants.API_INVALID_INPUT_DATA,
                        ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_INVALID_INPUT_DATA)
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("SendPasswordOnEmail : " + ex);
                response = new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// User Signup
        /// </summary>
        /// <param name="request">Signup request</param>
        /// <returns>User details</returns>
        public UserResponse UserSignUp(UserSignUpRequest request)
        {
            var response = new UserResponse();
            try
            {
                if (request != null)
                {
                    User user = null;

                    string encriptedStudyId = CryptoUtil.EncryptInfo(request.StudyId);
                    string encriptedStudyCode = CryptoUtil.EncryptInfo(request.StudyCode);
                    user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.IsGuestUser == false && u.StudyId == encriptedStudyId && u.StudyCode == encriptedStudyCode);
                    if (user != null)
                    {
                        if (user.Password != null && user.Password.Trim().Length > 0)
                        {
                            response.ErrorCode = LAMPConstants.API_USER_ALREADY_REGISTRED;
                            response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_ALREADY_REGISTRED);
                        }
                        else
                        {
                            response.UserId = user.UserID;
                            response.Email = CryptoUtil.DecryptInfo(user.Email);
                            response.StudyId = CryptoUtil.DecryptInfo(user.StudyId);
                            response.Type = (byte)UserType.User;
                            user.Password = CryptoUtil.EncryptStringWithKey(request.Password.Trim());
                            user.RegisteredOn = DateTime.UtcNow;
                            user.Status = true;
                            user.APPVersion = CryptoUtil.EncryptInfo(request.APPVersion.ToString());
                            _UnitOfWork.IUserRepository.Update(user);
                            response.Data = SaveDefaultUserSetting(user.UserID, request.Language);

                            // Update user device details
                            UserDevice userDevice = new UserDevice();
                            userDevice.UserID = user.UserID;
                            userDevice.DeviceType = request.DeviceType;
                            userDevice.DeviceID = CryptoUtil.EncryptInfo(request.DeviceID.Trim());
                            userDevice.DeviceToken = CryptoUtil.EncryptInfo(request.DeviceToken.Trim());
                            userDevice.LastLoginOn = DateTime.UtcNow;
                            _UnitOfWork.IUserDeviceRepository.Add(userDevice);
                            _UnitOfWork.Commit();
                            response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
                        }
                    }
                    else
                    {
                        response.ErrorCode = LAMPConstants.API_INVALID_USER;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_INVALID_USER);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new UserResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Guest user Signup
        /// </summary>
        /// <param name="request">Signup request</param>
        /// <returns>User details</returns>
        public UserResponse GuestUserSignUp(GuestUserSignUpRequest request)
        {
            var response = new UserResponse();
            try
            {
                if (request != null)
                {
                    User existuser = null;

                    string encriptedEmail = CryptoUtil.EncryptInfo(request.Email.Trim().ToLower());
                    existuser = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.Email == encriptedEmail);

                    if (existuser == null)
                    {
                        User user = new User();
                        user.FirstName = CryptoUtil.EncryptInfo(request.FirstName.Trim());
                        user.LastName = CryptoUtil.EncryptInfo(request.LastName.Trim());
                        user.Email = CryptoUtil.EncryptInfo(request.Email.Trim().ToLower());
                        user.Password = CryptoUtil.EncryptStringWithKey(request.Password.Trim()); ;
                        user.CreatedOn = DateTime.UtcNow;
                        user.RegisteredOn = DateTime.UtcNow;
                        user.IsDeleted = false;
                        user.IsGuestUser = true;
                        user.Status = true;
                        user.StudyId = CryptoUtil.EncryptInfo("GU" + CryptoUtil.CreateNumericSalt(11).ToString());
                        user.APPVersion = CryptoUtil.EncryptInfo(request.APPVersion.ToString());
                        user.AdminID = (int)AdminRoles.SuperAdmin;
                        _UnitOfWork.IUserRepository.Add(user);
                        response.Data = SaveDefaultUserSetting(user.UserID, string.IsNullOrEmpty(request.Language) ? "en" : request.Language);

                        // Update user device details
                        UserDevice userDevice = new UserDevice();
                        userDevice.UserID = user.UserID;
                        userDevice.DeviceType = request.DeviceType;
                        userDevice.DeviceID = CryptoUtil.EncryptInfo(request.DeviceID.Trim());
                        userDevice.DeviceToken = CryptoUtil.EncryptInfo(request.DeviceToken.Trim());
                        userDevice.LastLoginOn = DateTime.UtcNow;
                        _UnitOfWork.IUserDeviceRepository.Add(userDevice);
                        _UnitOfWork.Commit();

                        User newuser = null;

                        newuser = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.Email == encriptedEmail);
                        response.UserId = newuser.UserID;
                        response.StudyId = CryptoUtil.DecryptInfo(newuser.StudyId);
                        response.Email = CryptoUtil.DecryptInfo(newuser.Email);
                        response.Data = GetSettingForUser(newuser.UserID);
                        response.Type = (byte)UserType.Guest;
                        response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
                    }
                    else
                    {
                        response.ErrorCode = LAMPConstants.API_USER_ALREADY_REGISTRED;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_ALREADY_REGISTRED);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new UserResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Save User settings
        /// </summary>
        /// <param name="settingId">Setting Id</param>
        /// <param name="request">Setting request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveUserSetting(long settingId, UserSettingRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    UserSetting userSetting = null;
                    if (settingId > 0)
                    {

                        userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserSettingID == settingId);
                        userSetting.EditedOn = DateTime.UtcNow;
                        //new code

                        userSetting.UserID = request.UserID;
                        userSetting.AppColor = CryptoUtil.EncryptInfo(request.AppColor.Trim());
                        userSetting.SympSurvey_SlotID = request.SympSurveySlotID;
                        userSetting.SympSurvey_Time = request.SympSurveySlotTime;
                        userSetting.SympSurvey_RepeatID = request.SympSurveyRepeatID;
                        userSetting.CognTest_SlotID = request.CognTestSlotID;
                        userSetting.CognTest_Time = request.CognTestSlotTime;
                        userSetting.CognTest_RepeatID = request.CognTestRepeatID;
                        userSetting.C24By7ContactNo = CryptoUtil.EncryptInfo(request.ContactNo.Trim());
                        userSetting.PersonalHelpline = CryptoUtil.EncryptInfo(request.PersonalHelpline.Trim());
                        userSetting.PrefferedSurveys = CryptoUtil.EncryptInfo(request.PrefferedSurveys.Trim());
                        userSetting.PrefferedCognitions = CryptoUtil.EncryptInfo(request.PrefferedCognitions.Trim());
                        userSetting.Protocol = request.Protocol;
                        userSetting.Language = request.Language;
                        _UnitOfWork.IUserSettingRepository.Update(userSetting);
                    }
                    else
                    {
                        userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == request.UserID);

                        if (userSetting == null)
                        {
                            userSetting = new UserSetting();
                            userSetting.CreatedOn = DateTime.UtcNow;
                            userSetting.UserID = request.UserID;
                            userSetting.AppColor = CryptoUtil.EncryptInfo(request.AppColor.Trim());
                            userSetting.SympSurvey_SlotID = request.SympSurveySlotID;
                            userSetting.SympSurvey_Time = request.SympSurveySlotTime;
                            userSetting.SympSurvey_RepeatID = request.SympSurveyRepeatID;
                            userSetting.CognTest_SlotID = request.CognTestSlotID;
                            userSetting.CognTest_Time = request.CognTestSlotTime;
                            userSetting.CognTest_RepeatID = request.CognTestRepeatID;
                            userSetting.C24By7ContactNo = CryptoUtil.EncryptInfo(request.ContactNo.Trim());
                            userSetting.PersonalHelpline = CryptoUtil.EncryptInfo(request.PersonalHelpline.Trim());
                            userSetting.PrefferedSurveys = CryptoUtil.EncryptInfo(request.PrefferedSurveys.Trim());
                            userSetting.PrefferedCognitions = CryptoUtil.EncryptInfo(request.PrefferedCognitions.Trim());
                            userSetting.Protocol = request.Protocol;
                            userSetting.Language = request.Language;
                            _UnitOfWork.IUserSettingRepository.Add(userSetting);
                        }
                    }
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Get User setting details
        /// </summary>
        /// <param name="request">User Id</param>
        /// <returns>User setting details</returns>
        public UserSettingResponse GetUserSetting(GetUserSettingRequest request)
        {
            var response = new UserSettingResponse();
            try
            {
                response.Data = GetSettingForUser(request.UserID);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new UserSettingResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="request">Delete request</param>
        /// <returns>Status</returns>
        public APIResponseBase DeleteUser(UserDeleteRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                User user = null;

                user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == request.UserID);
                if (user != null)
                {
                    user.IsDeleted = true;
                    user.EditedOn = DateTime.UtcNow;
                    _UnitOfWork.IUserRepository.Update(user);
                    _UnitOfWork.Commit();
                    response.ErrorCode = 0;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Save User HealthKit
        /// </summary>
        /// <param name="settingId">Setting Id</param>
        /// <param name="request">Setting request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveUserHealthKit(long userId, UserHealthKitRequest request)
        {
            var response = new APIResponseBase();
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                if (request != null)
                {
                    if (userId > 0)
                    {
                        HealthKit_BasicInfo healthKit_BasicInfo = null;
                        HealthKit_DailyValues healthKit_DailyValues = null;
                        healthKit_BasicInfo = _UnitOfWork.IHealthKitBasicInfoRepository.SingleOrDefault(s => s.UserID == userId);
                        if (healthKit_BasicInfo != null)
                        {
                            //update  query
                            healthKit_BasicInfo.UserID = request.UserID;
                            healthKit_BasicInfo.DateOfBirth = request.DateOfBirth;
                            healthKit_BasicInfo.Sex = CryptoUtil.EncryptInfo(request.Sex);
                            healthKit_BasicInfo.BloodType = CryptoUtil.EncryptInfo(request.BloodType);
                            healthKit_BasicInfo.EditedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitBasicInfoRepository.Update(healthKit_BasicInfo);

                            healthKit_DailyValues = new HealthKit_DailyValues();
                            healthKit_DailyValues.UserID = request.UserID;
                            healthKit_DailyValues.Height = CryptoUtil.EncryptInfo(request.Height);
                            healthKit_DailyValues.Weight = CryptoUtil.EncryptInfo(request.Weight);
                            healthKit_DailyValues.HeartRate = CryptoUtil.EncryptInfo(request.HeartRate);
                            healthKit_DailyValues.BloodPressure = CryptoUtil.EncryptInfo(request.BloodPressure);
                            healthKit_DailyValues.RespiratoryRate = CryptoUtil.EncryptInfo(request.RespiratoryRate);
                            healthKit_DailyValues.Sleep = CryptoUtil.EncryptInfo(request.Sleep);
                            healthKit_DailyValues.Steps = CryptoUtil.EncryptInfo(request.Steps);
                            healthKit_DailyValues.FlightClimbed = CryptoUtil.EncryptInfo(request.FlightClimbed);
                            healthKit_DailyValues.CreatedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitDailyValuesRepository.Add(healthKit_DailyValues);
                        }
                        else
                        {
                            //add
                            healthKit_BasicInfo = new HealthKit_BasicInfo();
                            healthKit_BasicInfo.UserID = (long)request.UserID;
                            healthKit_BasicInfo.DateOfBirth = request.DateOfBirth;
                            healthKit_BasicInfo.Sex = CryptoUtil.EncryptInfo(request.Sex);
                            healthKit_BasicInfo.BloodType = CryptoUtil.EncryptInfo(request.BloodType);
                            healthKit_BasicInfo.CreatedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitBasicInfoRepository.Add(healthKit_BasicInfo);

                            healthKit_DailyValues = new HealthKit_DailyValues();
                            healthKit_DailyValues.UserID = (long)request.UserID;
                            healthKit_DailyValues.Height = CryptoUtil.EncryptInfo(request.Height);
                            healthKit_DailyValues.Weight = CryptoUtil.EncryptInfo(request.Weight);
                            healthKit_DailyValues.HeartRate = CryptoUtil.EncryptInfo(request.HeartRate);
                            healthKit_DailyValues.BloodPressure = CryptoUtil.EncryptInfo(request.BloodPressure);
                            healthKit_DailyValues.RespiratoryRate = CryptoUtil.EncryptInfo(request.RespiratoryRate);
                            healthKit_DailyValues.Sleep = CryptoUtil.EncryptInfo(request.Sleep);
                            healthKit_DailyValues.Steps = CryptoUtil.EncryptInfo(request.Steps);
                            healthKit_DailyValues.FlightClimbed = CryptoUtil.EncryptInfo(request.FlightClimbed);
                            healthKit_DailyValues.CreatedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitDailyValuesRepository.Add(healthKit_DailyValues);
                        }
                        _UnitOfWork.Commit();

                        DateTime startDateTime = DateTime.Today; 
                        DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1);
                        List<HealthKit_DailyValues> list = _UnitOfWork.IHealthKitDailyValuesRepository.GetAll().Where(s => s.UserID == userId && s.CreatedOn <= endDateTime).OrderByDescending(x => x.CreatedOn).ToList();
                        if (list.Count() > 30)
                        {
                            int counter = 0;
                            foreach (HealthKit_DailyValues values in list)
                            {
                                counter++;
                                if (counter > 30)
                                {
                                }
                            }
                        }
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                // LogUtil.Error(ex);
                LogUtil.Error("UserService/SaveUserHealthKit: " + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Saves the user health kit hourly.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public APIResponseBase SaveUserHealthKitHourly(long userId, UserHealthKitRequest request)
        {
            var response = new APIResponseBase();
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                if (request != null)
                {
                    if (userId > 0)
                    {
                        HealthKit_BasicInfo healthKit_BasicInfo = null;
                        HealthKit_DailyValues healthKit_DailyValues = null;
                        healthKit_BasicInfo = _UnitOfWork.IHealthKitBasicInfoRepository.SingleOrDefault(s => s.UserID == userId);
                        if (healthKit_BasicInfo != null)
                        {
                            //add
                            healthKit_BasicInfo.UserID = request.UserID;
                            healthKit_BasicInfo.DateOfBirth = request.DateOfBirth;
                            healthKit_BasicInfo.Sex = CryptoUtil.EncryptInfo(request.Sex);
                            healthKit_BasicInfo.BloodType = CryptoUtil.EncryptInfo(request.BloodType);
                            healthKit_BasicInfo.EditedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitBasicInfoRepository.Update(healthKit_BasicInfo);
                        }
                        else
                        {
                            //add
                            healthKit_BasicInfo = new HealthKit_BasicInfo();
                            healthKit_BasicInfo.UserID = (long)request.UserID;
                            healthKit_BasicInfo.DateOfBirth = request.DateOfBirth;
                            healthKit_BasicInfo.Sex = CryptoUtil.EncryptInfo(request.Sex);
                            healthKit_BasicInfo.BloodType = CryptoUtil.EncryptInfo(request.BloodType);
                            healthKit_BasicInfo.CreatedOn = DateTime.UtcNow;
                            _UnitOfWork.IHealthKitBasicInfoRepository.Add(healthKit_BasicInfo);
                        }

                        healthKit_DailyValues = new HealthKit_DailyValues();
                        healthKit_DailyValues.UserID = (long)request.UserID;
                        healthKit_DailyValues.Height = CryptoUtil.EncryptInfo(request.Height);
                        healthKit_DailyValues.Weight = CryptoUtil.EncryptInfo(request.Weight);
                        healthKit_DailyValues.HeartRate = CryptoUtil.EncryptInfo(request.HeartRate);
                        healthKit_DailyValues.BloodPressure = CryptoUtil.EncryptInfo(request.BloodPressure);
                        healthKit_DailyValues.RespiratoryRate = CryptoUtil.EncryptInfo(request.RespiratoryRate);
                        healthKit_DailyValues.Sleep = CryptoUtil.EncryptInfo(request.Sleep);
                        healthKit_DailyValues.Steps = CryptoUtil.EncryptInfo(request.Steps);
                        healthKit_DailyValues.FlightClimbed = CryptoUtil.EncryptInfo(request.FlightClimbed);
                        healthKit_DailyValues.CreatedOn = DateTime.UtcNow;
                        _UnitOfWork.IHealthKitDailyValuesRepository.Add(healthKit_DailyValues);
                        _UnitOfWork.Commit();
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserService/SaveUserHealthKitHourly: " + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Saves the user health kit sp.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public APIResponseBase SaveUserHealthKit_sp(long userId, UserHealthKitRequest request)
        {
            var response = new APIResponseBase();
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                SaveHealthKitData_sp_Result result = new SaveHealthKitData_sp_Result();
                LAMPEntities context = new LAMPEntities();
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "SaveHealthKitData_sp";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@p_UserID", request.UserID));

                if (request.DateOfBirth != null)
                {
                    command.Parameters.Add(new SqlParameter("@p_DateOfBirth", request.DateOfBirth));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_DateOfBirth", null));
                }
                command.Parameters.Add(new SqlParameter("@p_Sex", CryptoUtil.EncryptInfo(request.Sex)));
                command.Parameters.Add(new SqlParameter("@p_BloodType", CryptoUtil.EncryptInfo(request.BloodType)));

                command.Parameters.Add(new SqlParameter("@p_Height", CryptoUtil.EncryptInfo(request.Height)));
                command.Parameters.Add(new SqlParameter("@p_Weight", CryptoUtil.EncryptInfo(request.Weight)));
                command.Parameters.Add(new SqlParameter("@p_HeartRate", CryptoUtil.EncryptInfo(request.HeartRate)));

                command.Parameters.Add(new SqlParameter("@p_BloodPressure", CryptoUtil.EncryptInfo(request.BloodPressure)));
                command.Parameters.Add(new SqlParameter("@p_RespiratoryRate", CryptoUtil.EncryptInfo(request.RespiratoryRate)));
                command.Parameters.Add(new SqlParameter("@p_Sleep", CryptoUtil.EncryptInfo(request.Sleep)));
                command.Parameters.Add(new SqlParameter("@p_Steps", CryptoUtil.EncryptInfo(request.Steps)));
                command.Parameters.Add(new SqlParameter("@p_FlightClimbed", CryptoUtil.EncryptInfo(request.FlightClimbed)));

                command.Parameters.Add(new SqlParameter("@p_Segment", CryptoUtil.EncryptInfo(request.Segment)));
                command.Parameters.Add(new SqlParameter("@p_Distance", CryptoUtil.EncryptInfo(request.Distance)));
                
                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);

                context.Database.Connection.Open();
                long ErrorCode = 0;
                var reader = command.ExecuteReader();
                if (outputErrorParameter.Value != null)
                {
                    ErrorCode = Convert.ToInt32(outputErrorParameter.Value);
                }
                return response;

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserService/SaveUserHealthKit_sp: " + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        public APIResponseBase SaveUserHealthKitV2(HealthKitUserRequest request)
        {
            LogUtil.Error("UserService/SaveUserHealthKitV2: xxxx1");
            var response = new APIResponseBase();
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                StringBuilder strHealthKitParams = new StringBuilder();
                if (request.HealthKitParams != null || request.HealthKitParams.Count > 0)
                {
                    LogUtil.Error("UserService/SaveUserHealthKitV2: xxxx2");
                    strHealthKitParams.Append("<ParamValues>");
                    foreach (HealthKitParam obj in request.HealthKitParams)
                    {
                        strHealthKitParams.Append("<ParamValue>");
                        strHealthKitParams.Append("<ParamID>" + obj.ParamID.ToString() + "</ParamID>");
                        strHealthKitParams.Append("<Value>" + CryptoUtil.EncryptInfo(obj.Value) + "</Value>");
                        strHealthKitParams.Append("<DateTime>" + obj.DateTime.ToString("yyyy/MM/dd hh:mm:ss tt") + "</DateTime>");
                        strHealthKitParams.Append("</ParamValue>");
                    }
                    strHealthKitParams.Append("</ParamValues>");
                }
                else
                {
                    return new APIResponseBase
                    {
                        ErrorCode = LAMPConstants.API_INVALID_INPUT_DATA,
                        ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_INVALID_INPUT_DATA)
                    };
                }
                LAMPEntities context = new LAMPEntities();
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "SaveHealthKitData_sp_v2";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@p_UserID", request.UserID));
                if (request.DateOfBirth != null)
                {
                    command.Parameters.Add(new SqlParameter("@p_DateOfBirth", request.DateOfBirth));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_DateOfBirth", null));
                }
                command.Parameters.Add(new SqlParameter("@p_Sex", CryptoUtil.EncryptInfo(request.Gender.ToUpper())));
                command.Parameters.Add(new SqlParameter("@p_BloodType", CryptoUtil.EncryptInfo(request.BloodType.ToUpper())));
                command.Parameters.Add(new SqlParameter("@p_ValuesXML", strHealthKitParams.ToString()));
                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);
                context.Database.Connection.Open();
                LogUtil.Error("UserService/SaveUserHealthKitV2: xxxx3");
                int val = command.ExecuteNonQuery();
                if (outputErrorParameter.Value != null)
                {
                    LogUtil.Error("UserService/SaveUserHealthKitV2: xxxx4");
                    response.ErrorCode = Convert.ToInt32(outputErrorParameter.Value);
                    if (response.ErrorCode > 0)
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR);
                }                
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserService/SaveUserHealthKitV2: xxxx5");
                LogUtil.Error("UserService/SaveUserHealthKitV2: " + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

            
        }

        /// <summary>
        /// Get Tips
        /// </summary>
        /// <returns></returns>
        public TipsResponse GetTips(GetTipsRequest request)
        {
            TipsResponse response = new TipsResponse();
            try
            {
                long adminId = GetAdminId(request.UserId);
                if (adminId != 0)
                {
                    Tip tip = _UnitOfWork.ITipRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();

                    if (tip != null)
                    {
                        response.TipText = tip.TipText;

                        // --Update Tips viewed time in UserSettings
                        UserSetting userSetting = null;
                        userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == request.UserId);

                        if (userSetting != null)
                        {
                            userSetting.TipsViewedOn = DateTime.UtcNow;
                            _UnitOfWork.IUserSettingRepository.Update(userSetting);
                            _UnitOfWork.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new TipsResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

            return response;
        }

        /// <summary>
        /// Get Blogs
        /// </summary>
        /// <returns></returns>
        public BlogResponse GetBlogs(GetBlogRequest request)
        {
            BlogResponse response = new BlogResponse();
            string imagePath = System.Configuration.ConfigurationManager.AppSettings["BlogImagePath"];

            try
            {
                long adminId = GetAdminId(request.UserId);
                List<BlogData> blogList = (from blog in _UnitOfWork.IBlogRepository.RetrieveAll().Where(x => x.AdminID == adminId && x.IsDeleted != true).OrderByDescending(x => x.CreatedOn)
                                           select new BlogData
                                           {
                                               BlogTitle = blog.BlogTitle,
                                               Content = blog.Content,
                                               ImageURL = blog.ImageURL,
                                               BlogText = blog.BlogText.Substring(0, 800)
                                           }).ToList();

                if (blogList != null && blogList.Count() > 0)
                {
                    blogList = blogList.Select(blog => new BlogData()
                    {
                        BlogTitle = blog.BlogTitle,
                        Content = blog.Content,
                        ImageURL = blog.ImageURL != null ? imagePath + CryptoUtil.DecryptInfo(blog.ImageURL) : "",
                        BlogText = blog.BlogText

                    }).ToList();

                    response.BlogList = blogList;
                }
                else
                    response.BlogList = null;

                response.BlogList = blogList;

                // --Update Blogs viewed time in UserSettings
                UserSetting userSetting = null;
                userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == request.UserId);

                if (userSetting != null)
                {
                    userSetting.BlogsViewedOn = DateTime.UtcNow;
                    _UnitOfWork.IUserSettingRepository.Update(userSetting);
                    _UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new BlogResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get the status of Tips and Blog updations to a given user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TipandBlogUpdateResponse GetTipsandBlogUpdates(GetTipsandBlogsUpdateRequest request)
        {
            var response = new TipandBlogUpdateResponse();

            try
            {
                UserSetting userSetting = null;
                long adminId = GetAdminId(request.UserId);
                userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == request.UserId);

                // --Get Blogs----------------------------------------------
                List<BlogUpdations> blogList = (from blog in _UnitOfWork.IBlogRepository.RetrieveAll().Where(x => x.AdminID == adminId && x.IsDeleted != true)
                                                select new BlogUpdations
                                               {
                                                   BlogId = blog.BlogID,
                                                   CreatedOn = blog.CreatedOn,
                                                   EditedOn = blog.EditedOn
                                               }).ToList();

                if (blogList != null && blogList.Count() > 0)
                {
                    foreach (BlogUpdations blog in blogList)
                    {
                        if (userSetting.BlogsViewedOn < blog.CreatedOn || userSetting.BlogsViewedOn < blog.EditedOn)
                        {
                            response.BlogsUpdate = true;
                            break;
                        }

                        else
                            response.BlogsUpdate = false;
                    }
                }

                // --Get Tips----------------------------------------------------
                Tip tip = _UnitOfWork.ITipRepository.GetAll().Where(x => x.AdminID == adminId).FirstOrDefault();

                if (tip != null)
                {
                    if (userSetting.TipsViewedOn < tip.CreatedOn || userSetting.TipsViewedOn < tip.EditedOn)
                        response.TipsUpdate = true;
                    else
                        response.TipsUpdate = false;
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new TipandBlogUpdateResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get App help.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AppHelpResponse GetAppHelp(GetAppHelpRequest request)
        {
            AppHelpResponse response = new AppHelpResponse();
            string imagePath = System.Configuration.ConfigurationManager.AppSettings["AppHelpImagePath"];

            try
            {
                long adminId = GetAdminId(request.UserId);
                if (adminId != 0)
                {
                    AppHelp appHelp = _UnitOfWork.IAppHelpRepository.GetAll().Where(s => s.AdminID == adminId && s.IsDeleted == false).SingleOrDefault();

                    if (appHelp != null)
                    {
                        response.HelpText = appHelp.HelpText;
                        response.Content = appHelp.Content;
                        response.ImageURL = appHelp.ImageURL != null ? imagePath + CryptoUtil.DecryptInfo(appHelp.ImageURL) : "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new AppHelpResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

            return response;
        }


        /// <summary>
        /// Get Survey and Game schedules by the given UserId.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ScheduleSurveyAndGame GetSurveyandGameScheduleandJewelsSettings(SurveyAndGameScheduleRequest request)
        {
            ScheduleSurveyAndGame response = new ScheduleSurveyAndGame();
            try
            {
                CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                LAMPEntities context = new LAMPEntities();
                /***************************************Survey*****Schedules******************************************************************/
                List<ScheduleSurvey> scheduleSurveyList = null;
                string date = request.LastUpdatedSurveyDate.HasValue ? request.LastUpdatedSurveyDate.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss.fff", culture) : string.Empty;                
                long errorcode = 0;
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "GetAdminSurveyScheduleByUserID_sp";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@p_UserID", request.UserId));
                if (request.LastUpdatedSurveyDate != null)
                {
                    command.Parameters.Add(new SqlParameter("@p_LastFetchedTS", Convert.ToDateTime(date)));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_LastFetchedTS", null));
                }

                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);

                var outputErrorParameterA = command.CreateParameter();
                outputErrorParameterA.ParameterName = "@p_LastUpdatedTS";
                outputErrorParameterA.DbType = DbType.DateTime;
                outputErrorParameterA.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameterA);

                context.Database.Connection.Open();
                var reader = command.ExecuteReader();

                var responseResult = ((IObjectContextAdapter)context).ObjectContext.Translate<ScheduleSurvey>(reader).ToList();
                scheduleSurveyList = responseResult.Select(s =>
                   {
                       s.SurveyScheduleID = s.SurveyScheduleID;
                       s.SurveyName = s.SurveyName;
                       s.Time = s.Time;
                       s.SlotTime = (s.Time != null) ? Convert.ToDateTime(s.Time).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty;
                       s.SurveyId = s.SurveyId;
                       s.ScheduleDate = s.ScheduleDate;
                       s.RepeatID = s.RepeatID;
                       s.IsDeleted = s.IsDeleted;
                       return s;
                   }).ToList();
                foreach (var time in scheduleSurveyList)
                {
                    if (time.RepeatID == 5 || time.RepeatID == 6 || time.RepeatID == 7 || time.RepeatID == 8 || time.RepeatID == 9 || time.RepeatID == 10 || time.RepeatID == 12)
                    {
                        time.ScheduleDate = time.Time;
                    }
                }
                if (reader.NextResult())
                {
                    var slotTimeOptions = ((IObjectContextAdapter)context).ObjectContext.Translate<SlotTimeOptions>(reader).ToList();
                    slotTimeOptions = slotTimeOptions.Select(s =>
                    {
                        s.Time = s.Time;
                        s.TimeString = (s.Time != null) ? Convert.ToDateTime(s.Time).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty;
                        s.ScheduleID = s.ScheduleID;
                        return s;
                    }).ToList();
                    foreach (var schedule in scheduleSurveyList)
                    {
                        var sequence = slotTimeOptions.Where(s => s.ScheduleID == schedule.SurveyScheduleID).Select(s => new { s.TimeString }).ToList();
                        schedule.SlotTimeOptions = sequence.Select(s => (string)s.TimeString).ToList();
                    }
                }

                reader.Close();

                if (outputErrorParameter.Value != null)
                {
                    errorcode = Convert.ToInt32(outputErrorParameter.Value);

                }
                if (outputErrorParameterA.Value != null)
                {
                    response.LastUpdatedSurveyDate = (DateTime)outputErrorParameterA.Value;
                }
                context.Database.Connection.Close();

                /***************************************Game*****Schedules******************************************************************/

                List<ScheduleGame> scheduleGameList = null;
                string gameDate = request.LastUpdatedGameDate.HasValue ? request.LastUpdatedGameDate.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss.fff", culture) : string.Empty;                
                var commandNew = context.Database.Connection.CreateCommand();
                commandNew.CommandText = "GetAdminCTestScheduleByUserID_sp";
                commandNew.CommandType = CommandType.StoredProcedure;
                commandNew.Parameters.Add(new SqlParameter("@p_UserID", request.UserId));
                if (request.LastUpdatedGameDate != null)
                {
                    commandNew.Parameters.Add(new SqlParameter("@p_LastFetchedTS", Convert.ToDateTime(gameDate)));
                }
                else
                {
                    commandNew.Parameters.Add(new SqlParameter("@p_LastFetchedTS", null));
                }

                var outputErrorParameterGame = commandNew.CreateParameter();
                outputErrorParameterGame.ParameterName = "@p_ErrID";
                outputErrorParameterGame.DbType = DbType.Int64;
                outputErrorParameterGame.Direction = ParameterDirection.Output;
                commandNew.Parameters.Add(outputErrorParameterGame);

                var outputParameterGameDate = commandNew.CreateParameter();
                outputParameterGameDate.ParameterName = "@p_LastUpdatedTS";
                outputParameterGameDate.DbType = DbType.DateTime;
                outputParameterGameDate.Direction = ParameterDirection.Output;
                commandNew.Parameters.Add(outputParameterGameDate);

                context.Database.Connection.Open();
                var readerGame = commandNew.ExecuteReader();

                var gameResult = ((IObjectContextAdapter)context).ObjectContext.Translate<ScheduleGame>(readerGame).ToList();
                scheduleGameList = gameResult.Select(s =>
                {
                    s.CTestId = s.CTestId;
                    s.CTestName = s.CTestName;
                    s.Version = s.Version;
                    s.Time = s.Time;
                    s.SlotTime = (s.Time != null) ? Convert.ToDateTime(s.Time).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty;
                    s.GameScheduleID = s.GameScheduleID;
                    s.ScheduleDate = s.ScheduleDate;
                    s.RepeatID = s.RepeatID;
                    s.IsDeleted = s.IsDeleted;
                    return s;
                }).ToList();
                foreach (var time in scheduleGameList)
                {
                    if (time.RepeatID == 5 || time.RepeatID == 6 || time.RepeatID == 7 || time.RepeatID == 8 || time.RepeatID == 9 || time.RepeatID == 10 || time.RepeatID == 12)
                    {
                        time.ScheduleDate = time.Time;
                    }
                }
                if (readerGame.NextResult())
                {
                    var slotTimeOptions = ((IObjectContextAdapter)context).ObjectContext.Translate<SlotTimeOptions>(readerGame).ToList();
                    slotTimeOptions = slotTimeOptions.Select(s =>
                    {
                        s.Time = s.Time;
                        s.TimeString = (s.Time != null) ? Convert.ToDateTime(s.Time).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty;
                        s.ScheduleID = s.ScheduleID;
                        return s;
                    }).ToList();
                    foreach (var schedule in scheduleGameList)
                    {
                        var sequence = slotTimeOptions.Where(s => s.ScheduleID == schedule.GameScheduleID).Select(s => new { s.TimeString }).ToList();
                        schedule.SlotTimeOptions = sequence.Select(s => (string)s.TimeString).ToList();
                    }
                }

                readerGame.Close();

                if (outputErrorParameterGame.Value != null)
                {
                    errorcode = Convert.ToInt32(outputErrorParameterGame.Value);

                }
                if (outputParameterGameDate.Value != null)
                {
                    response.LastUpdatedGameDate = (DateTime)outputParameterGameDate.Value;
                }
                context.Database.Connection.Close();
                response.ScheduleSurveyList = scheduleSurveyList;
                response.ScheduleGameList = scheduleGameList;

                /***************************************Jewels*****Settings******************************************************************/
                long adminId = GetAdminId(request.UserId);
                if (adminId != 0)
                {
                    Admin_JewelsTrailsASettings settingsA = _UnitOfWork.IAdminJewelsTrailsASettingsRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();
                    Admin_JewelsTrailsBSettings settingsB = _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();
                    if (settingsA != null)
                    {
                        response.JewelsTrailsASettings.NoOfBonusPoints = settingsA.NoOfBonusPoints;
                        response.JewelsTrailsASettings.NoOfDiamonds = settingsA.NoOfDiamonds;
                        response.JewelsTrailsASettings.NoOfSeconds_Adv = settingsA.NoOfSeconds_Adv;
                        response.JewelsTrailsASettings.NoOfSeconds_Beg = settingsA.NoOfSeconds_Beg;
                        response.JewelsTrailsASettings.NoOfSeconds_Exp = settingsA.NoOfSeconds_Exp;
                        response.JewelsTrailsASettings.NoOfSeconds_Int = settingsA.NoOfSeconds_Int;
                        response.JewelsTrailsASettings.NoOfShapes = settingsA.NoOfShapes;
                        response.JewelsTrailsASettings.X_NoOfChangesInLevel = settingsA.X_NoOfChangesInLevel;
                        response.JewelsTrailsASettings.X_NoOfDiamonds = settingsA.X_NoOfDiamonds;
                        response.JewelsTrailsASettings.Y_NoOfChangesInLevel = settingsA.Y_NoOfChangesInLevel;
                        response.JewelsTrailsASettings.Y_NoOfShapes = settingsA.Y_NoOfShapes;

                    }
                    if (settingsB != null)
                    {
                        response.JewelsTrailsBSettings.NoOfBonusPoints = settingsB.NoOfBonusPoints;
                        response.JewelsTrailsBSettings.NoOfDiamonds = settingsB.NoOfDiamonds;
                        response.JewelsTrailsBSettings.NoOfSeconds_Adv = settingsB.NoOfSeconds_Adv;
                        response.JewelsTrailsBSettings.NoOfSeconds_Beg = settingsB.NoOfSeconds_Beg;
                        response.JewelsTrailsBSettings.NoOfSeconds_Exp = settingsB.NoOfSeconds_Exp;
                        response.JewelsTrailsBSettings.NoOfSeconds_Int = settingsB.NoOfSeconds_Int;
                        response.JewelsTrailsBSettings.NoOfShapes = settingsB.NoOfShapes;
                        response.JewelsTrailsBSettings.X_NoOfChangesInLevel = settingsB.X_NoOfChangesInLevel;
                        response.JewelsTrailsBSettings.X_NoOfDiamonds = settingsB.X_NoOfDiamonds;
                        response.JewelsTrailsBSettings.Y_NoOfChangesInLevel = settingsB.Y_NoOfChangesInLevel;
                        response.JewelsTrailsBSettings.Y_NoOfShapes = settingsB.Y_NoOfShapes;
                    }
                }
                /***************************************Expiry*****Option******Setting*********************************************************/
                if (adminId != 0)
                {
                    Admin_Settings settingsA = _UnitOfWork.IAdminSettingsRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();
                    if (settingsA != null)
                    {
                        response.ReminderClearInterval = settingsA.ReminderClearInterval;
                    }
                    else
                    {
                        response.ReminderClearInterval = (long)ReminderClearInterval.None;
                    }
                }

                /********************************************* Start fetching Batch Survey and Ctest details ************************************************/                
                List<BatchScheduleSurvey_CTest> BatchScheduleSurvey_CTestList = new List<BatchScheduleSurvey_CTest>();
                List<BatchScheduleCustomTime> BatchScheduleCustomTimeList = new List<BatchScheduleCustomTime>();
                string batchScheduleDate = string.Empty;
                commandNew = context.Database.Connection.CreateCommand();
                commandNew.CommandText = "GetAdminBatchScheduleByUserID_sp";
                commandNew.CommandType = CommandType.StoredProcedure;
                commandNew.Parameters.Add(new SqlParameter("@p_UserID", request.UserId));
                if (request.LastUpdatedGameDate != null)
                {
                    batchScheduleDate = request.LastUpdatedGameDate.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss.fff", culture);
                    commandNew.Parameters.Add(new SqlParameter("@p_LastFetchedTS", Convert.ToDateTime(batchScheduleDate)));
                }
                else
                {
                    commandNew.Parameters.Add(new SqlParameter("@p_LastFetchedTS", null));
                }

                outputErrorParameterGame = commandNew.CreateParameter();
                outputErrorParameterGame.ParameterName = "@p_ErrID";
                outputErrorParameterGame.DbType = DbType.Int64;
                outputErrorParameterGame.Direction = ParameterDirection.Output;
                commandNew.Parameters.Add(outputErrorParameterGame);

                var outputParameterLastUpdatedTS = commandNew.CreateParameter();
                outputParameterLastUpdatedTS.ParameterName = "@p_LastUpdatedTS";
                outputParameterLastUpdatedTS.DbType = DbType.DateTime;
                outputParameterLastUpdatedTS.Direction = ParameterDirection.Output;
                commandNew.Parameters.Add(outputParameterLastUpdatedTS);

                context.Database.Connection.Open();
                using (System.Data.Common.DbDataReader readerBatchSchedule = commandNew.ExecuteReader())
                {
                    if (outputErrorParameterGame.Value != null)
                    {
                        return new ScheduleSurveyAndGame
                        {
                            ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR_FOR_BATCH_SCHEDULE,
                            ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                        };
                    }
                    else
                    {
                        List<BatchSchedule> BatchScheduleList = new List<BatchSchedule>();
                        // Batch details
                        BatchSchedule batchSchedule;
                        while (readerBatchSchedule.Read())
                        {
                            batchSchedule = new BatchSchedule();
                            batchSchedule.BatchScheduleId = Convert.ToInt64(readerBatchSchedule["ScheduleID"]);
                            batchSchedule.BatchName = readerBatchSchedule["BatchName"].ToString();
                            if (readerBatchSchedule["ScheduleDate"] != null && readerBatchSchedule["ScheduleDate"] != System.DBNull.Value)
                            {
                                batchSchedule.ScheduleDate = Convert.ToDateTime(readerBatchSchedule["ScheduleDate"]);
                            }
                            else
                                batchSchedule.ScheduleDate = null;

                            if (readerBatchSchedule["Time"] != null && readerBatchSchedule["Time"] != System.DBNull.Value)
                            {
                                batchSchedule.Time = Convert.ToDateTime(readerBatchSchedule["Time"]);
                            }
                            else
                                batchSchedule.Time = null;
                            batchSchedule.RepeatId = Convert.ToInt64(readerBatchSchedule["RepeatID"]);
                            batchSchedule.IsDeleted = Convert.ToBoolean(readerBatchSchedule["IsDeleted"]);
                            BatchScheduleList.Add(batchSchedule);
                        }

                        foreach (var time in BatchScheduleList)
                        {
                            time.SlotTime = (time.Time != null) ? Convert.ToDateTime(time.Time).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty;
                            if (time.RepeatId == 5 || time.RepeatId == 6 || time.RepeatId == 7 || time.RepeatId == 8 || time.RepeatId == 9 || time.RepeatId == 10 || time.RepeatId == 12)
                            {
                                time.ScheduleDate = time.Time;
                            }
                        }
                        // Game details                 
                        BatchScheduleSurvey_CTest batchScheduleSurvey_CTest;
                        if (readerBatchSchedule.NextResult())
                        {
                            while (readerBatchSchedule.Read())
                            {
                                batchScheduleSurvey_CTest = new BatchScheduleSurvey_CTest();
                                batchScheduleSurvey_CTest.BatchScheduleId = Convert.ToInt64(readerBatchSchedule["ScheduleID"]);
                                batchScheduleSurvey_CTest.Type = 2;
                                batchScheduleSurvey_CTest.ID = Convert.ToInt64(readerBatchSchedule["CTestID"]);
                                batchScheduleSurvey_CTest.Version = Convert.ToInt32(readerBatchSchedule["Version"]);
                                batchScheduleSurvey_CTest.Order = Convert.ToInt16(readerBatchSchedule["Order"]);
                                BatchScheduleSurvey_CTestList.Add(batchScheduleSurvey_CTest);
                            }
                        }

                        // Survey Details
                        if (readerBatchSchedule.NextResult())
                        {
                            while (readerBatchSchedule.Read())
                            {
                                batchScheduleSurvey_CTest = new BatchScheduleSurvey_CTest();
                                batchScheduleSurvey_CTest.BatchScheduleId = Convert.ToInt64(readerBatchSchedule["ScheduleID"]);
                                batchScheduleSurvey_CTest.Type = 1;
                                batchScheduleSurvey_CTest.ID = Convert.ToInt64(readerBatchSchedule["SurveyID"]);
                                batchScheduleSurvey_CTest.Version = 0;
                                batchScheduleSurvey_CTest.Order = Convert.ToInt16(readerBatchSchedule["Order"]);
                                BatchScheduleSurvey_CTestList.Add(batchScheduleSurvey_CTest);
                            }
                        }

                        // Custome Time Details              
                        BatchScheduleCustomTime batchScheduleCustomTime;
                        if (readerBatchSchedule.NextResult())
                        {
                            while (readerBatchSchedule.Read())
                            {
                                batchScheduleCustomTime = new BatchScheduleCustomTime();
                                batchScheduleCustomTime.BatchScheduleId = Convert.ToInt64(readerBatchSchedule["ScheduleID"]);                                
                                if (readerBatchSchedule["Time"] != null)
                                    batchScheduleCustomTime.Time = Convert.ToDateTime(readerBatchSchedule["Time"]).ToString("dd-MM-yyyy HH:mm:ss");
                                else
                                    batchScheduleCustomTime.Time = string.Empty;

                                BatchScheduleCustomTimeList.Add(batchScheduleCustomTime);
                            }
                        }

                        foreach (BatchSchedule batch in BatchScheduleList)
                        {
                            List<BatchScheduleSurvey_CTest> survey_CTestList = BatchScheduleSurvey_CTestList.Where(s => s.BatchScheduleId == batch.BatchScheduleId).ToList();
                            List<BatchScheduleCustomTime> customTimeList = BatchScheduleCustomTimeList.Where(t => t.BatchScheduleId == batch.BatchScheduleId).ToList();


                            if (survey_CTestList != null && survey_CTestList.Count > 0)
                                batch.BatchScheduleSurvey_CTest = survey_CTestList.OrderBy(o => o.Order).ToList();
                            else
                                batch.BatchScheduleSurvey_CTest = null;

                            if (customTimeList != null && customTimeList.Count > 0)
                                batch.BatchScheduleCustomTime = customTimeList;
                            else
                                batch.BatchScheduleCustomTime = null;
                        }
                        response.BatchScheduleList = BatchScheduleList;
                    }
                }
                if (outputParameterLastUpdatedTS.Value != null)
                {
                    response.LastUpdatedBatchDate = Convert.ToDateTime(outputParameterLastUpdatedTS.Value);
                }
                context.Database.Connection.Close();
                // End fetching Batch Survey and Ctest details
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new ScheduleSurveyAndGame
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }


        /// <summary>
        /// Get the list of distraction surveys for the given user by the CTest Id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DistractionSurveyResponse GetDistractionSurveys(DistractionSurveyRequest request)
        {
            DistractionSurveyResponse response = new DistractionSurveyResponse();
            try
            {
                long adminId = GetAdminId(request.UserId);
                response.Surveys = (from settings in _UnitOfWork.IAdminCTestSurveyRepository.GetAll()
                                    join survey in _UnitOfWork.ISurveyRepository.GetAll() on settings.SurveyID equals survey.SurveyID
                                    where (settings.AdminID == adminId && settings.CTestID == request.CTestId && survey.IsDeleted == false)
                                    select new DistractionSurvey
                                    {
                                        SurveyId = settings.SurveyID
                                    }).ToList();

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new DistractionSurveyResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get Protocol Date set by the admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProtocolDateResponse GetProtocolDate(ProtocolDateRequest request)
        {
            ProtocolDateResponse response = new ProtocolDateResponse();
            try
            {
                response.ProtocolDate = _UnitOfWork.IUserSettingRepository.SingleOrDefault(x => x.UserID == request.UserId).ProtocolDate;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new ProtocolDateResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }        
        #endregion

        #region Private Methods

        /// <summary>
        /// Save User default settings values
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User Settings details</returns>
        private UserSettingData SaveDefaultUserSetting(long userId, string Language)
        {
            UserSettingData reponse = new UserSettingData();
            try
            {
                UserSettingRequest request = new UserSettingRequest();
                request.UserID = userId;
                request.AppColor = "#359FFE";
                request.SympSurveySlotID = 1;
                request.SympSurveySlotTime = null;
                request.SympSurveyRepeatID = 1;
                request.CognTestSlotID = 1;
                request.CognTestSlotTime = null;
                request.CognTestRepeatID = 1;
                request.ContactNo = string.Empty;
                request.PersonalHelpline = string.Empty;
                request.PrefferedSurveys = string.Empty;
                request.PrefferedCognitions = string.Empty;
                request.Language = Language;
                APIResponseBase saveReponse = SaveUserSetting(0, request);
                if (saveReponse.ErrorCode == LAMPConstants.API_SUCCESS_CODE && userId > 0)
                {
                    reponse = GetSettingForUser(userId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return reponse;
        }

        /// <summary>
        /// Get User Settings details
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User Settings details</returns>
        private UserSettingData GetSettingForUser(long userId)
        {
            UserSettingData reponse = new UserSettingData();
            try
            {
                UserSetting userSetting = null;

                userSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(s => s.UserID == userId);
                if (userSetting != null)
                {
                    reponse.UserSettingID = userSetting.UserSettingID;
                    reponse.UserID = userSetting.UserID;
                    reponse.AppColor = CryptoUtil.DecryptInfo(userSetting.AppColor);
                    reponse.SympSurveySlotID = (long)userSetting.SympSurvey_SlotID;
                    if (userSetting.SympSurvey_Time == null)
                        reponse.SympSurveySlotTime = null;
                    else
                        reponse.SympSurveySlotTime = userSetting.SympSurvey_Time.Value;
                    reponse.SympSurveyRepeatID = (long)userSetting.SympSurvey_RepeatID;
                    reponse.CognTestSlotID = (long)userSetting.CognTest_SlotID;
                    if (userSetting.CognTest_Time == null)
                        reponse.CognTestSlotTime = null;
                    else
                        reponse.CognTestSlotTime = userSetting.CognTest_Time.Value;
                    reponse.CognTestRepeatID = (long)userSetting.CognTest_RepeatID;
                    reponse.ContactNo = CryptoUtil.DecryptInfo(userSetting.C24By7ContactNo);
                    reponse.PersonalHelpline = CryptoUtil.DecryptInfo(userSetting.PersonalHelpline);
                    reponse.PrefferedSurveys = CryptoUtil.DecryptInfo(userSetting.PrefferedSurveys);
                    reponse.PrefferedCognitions = CryptoUtil.DecryptInfo(userSetting.PrefferedCognitions);
                    reponse.Protocol = userSetting.Protocol;
                    reponse.Language = userSetting.Language;
                    reponse.ProtocolDate = userSetting.ProtocolDate;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return reponse;
        }

        /// <summary>
        /// To get the AdminId by the given UserId.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        private long GetAdminId(long UserId)
        {
            long adminId = 0;
            User user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == UserId && u.IsDeleted != true);
            if (user != null)
            {
                if (user.IsGuestUser == true)
                    adminId = (int)AdminRoles.SuperAdmin;
                else
                {
                    if (user.AdminID != null)
                        adminId = Convert.ToInt64(user.AdminID);
                }
            }

            return adminId;
        }
        /// <summary>
        /// User Profile Update
        /// </summary>
        /// <param name="request">User Profile request</param>
        /// <returns>Status</returns>
        public APIResponseBase UpdateUserProfile(UserProfileRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    User userProfile = null;

                    userProfile = _UnitOfWork.IUserRepository.SingleOrDefault(s => s.UserID == request.UserId);
                    userProfile.FirstName = CryptoUtil.EncryptInfo(request.FirstName.Trim());
                    userProfile.LastName = CryptoUtil.EncryptInfo(request.LastName.Trim());
                    userProfile.StudyId = CryptoUtil.EncryptInfo(request.StudyId.Trim());

                    userProfile.EditedOn = DateTime.UtcNow;
                    _UnitOfWork.IUserRepository.Update(userProfile);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

        }

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User Profile details</returns>
        public UserProfileResponse GetUserProfile(long userId)
        {
            UserProfileResponse response = new UserProfileResponse();
            UserProfileData data = new UserProfileData();
            try
            {
                User userProfile = null;

                userProfile = _UnitOfWork.IUserRepository.SingleOrDefault(s => s.UserID == userId);
                if (userProfile != null)
                {
                    data.UserId = userProfile.UserID;
                    data.FirstName = CryptoUtil.DecryptInfo(userProfile.FirstName);
                    data.LastName = CryptoUtil.DecryptInfo(userProfile.LastName);
                    data.StudyId = CryptoUtil.DecryptInfo(userProfile.StudyId);

                }
                response.Data = data;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new UserProfileResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Gets the user report.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserReportResponse GetUserReport(long userId)
        {
            UserReportResponse response = new UserReportResponse();
            try
            {
                LAMPEntities context = new LAMPEntities();

                List<JewelsTrialsAList> jewelsTrialsAList = new List<JewelsTrialsAList>();
                List<JewelsTrialsBList> jewelsTrialsBList = new List<JewelsTrialsBList>();
                List<JewelsTrialsAList> jewelsAList = null;
                var outputParameterA = new System.Data.Entity.Core.Objects.ObjectParameter("p_ErrID", typeof(int));
                jewelsAList = (from c in context.GetJewelsTrailsAResult_sp(userId, outputParameterA)
                               select new JewelsTrialsAList
                               {
                                   CreatedDate = c.Date,
                                   TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                                   TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                                   Score = Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)),
                               }).ToList();

                var result = jewelsAList.GroupBy(s => s.CreatedDate).Select(g => new JewelsTrialsAList { TotalBonusCollected = g.Sum(s => s.TotalBonusCollected), TotalJewelsCollected = g.Sum(s => s.TotalJewelsCollected), Score = g.Sum(s => s.Score), ScoreAvg = g.Average(s => s.Score) ?? 0, CreatedDate = g.Key });
                response.JewelsTrialsAList = result.ToList();

                //____________________________________________________

                List<JewelsTrialsBList> jewelsBList = null;
                var outputParameterB = new System.Data.Entity.Core.Objects.ObjectParameter("p_ErrID", typeof(int));
                jewelsBList = (from c in context.GetJewelsTrailsBResult_sp(userId, outputParameterB)
                               select new JewelsTrialsBList
                               {

                                   CreatedDate = c.Date,
                                   TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                                   TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                                   Score = Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)),
                               }).ToList();

                var resultB = jewelsBList.GroupBy(s => s.CreatedDate).Select(g => new JewelsTrialsBList { TotalBonusCollected = g.Sum(s => s.TotalBonusCollected), TotalJewelsCollected = g.Sum(s => s.TotalJewelsCollected), Score = g.Sum(s => s.Score), ScoreAvg = g.Average(s => s.Score) ?? 0, CreatedDate = g.Key });
                response.JewelsTrialsBList = resultB.ToList();

            }
            catch (Exception ex)
            {
                LogUtil.Error("GetuserReport : " + ex);
                return new UserReportResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        #endregion
    }
}




