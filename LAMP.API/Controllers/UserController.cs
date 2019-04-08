using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.API.Controllers
{
    /// <summary>
    /// UserController is responsible for handling user web api acitivities
    /// </summary>
    [Authorize]
    [RoutePrefix("api")]
    public class UserController : BaseController
    {
        #region Fields
        private IAccountService _accountService;
        private IAuthentication _authService;
        private IAuthenticationContext _authContext;
        private IUserService _userService;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="requestService">Request Service</param>
        /// <param name="authServices">Authentication</param>
        public UserController(IAccountService accountService, IAuthentication authServices, IAuthenticationContext authContext, IUserService userService)
            : base(accountService, authContext)
        {
            _authService = authServices;
            _userService = userService;
            _accountService = accountService;
            _authContext = authContext;
        }

        #endregion

        #region Users
        /// <summary>
        /// Tests the encryption.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [Route("TestEncryption")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase TestEncryption(APIResponseBase request)
        {
            APIResponseBase response = new APIResponseBase();
            response.ErrorMessage = CryptoUtil.EncryptInfo(request.ErrorMessage);
            return response;
        }

        /// <summary>
        /// Tests the decryption.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [Route("TestDecryption")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase TestDecryption(APIResponseBase request)
        {
            APIResponseBase response = new APIResponseBase();
            response.ErrorMessage = CryptoUtil.DecryptInfo(request.ErrorMessage);
            return response;
        }

        /// <summary>
        /// Test Encryption for password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("TestEncryptStringWithKey")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase TestEncryptStringWithKey(APIResponseBase request)
        {
            APIResponseBase response = new APIResponseBase();
            response.ErrorMessage = CryptoUtil.EncryptStringWithKey(request.ErrorMessage);
            return response;
        }

        /// <summary>
        /// Test Decryption for password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("TestDecryptStringWithKey")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase TestDecryptStringWithKey(APIResponseBase request)
        {
            APIResponseBase response = new APIResponseBase();
            if (request.ErrorCode == 11)
                response.ErrorMessage = CryptoUtil.DecryptStringWithKey(request.ErrorMessage);
            else
                response.ErrorMessage = "";
            return response;
        }

        /// <summary>
        /// User Sign in    
        /// </summary>
        /// <param name="request">Sign in request</param>
        /// <returns>User Details</returns>
        [Route("SignIn")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserLoginResponse))]
        public async Task<UserLoginResponse> SignIn(UserSignInRequest request)
        {
            LogUtil.Debug("SignIn : " + request.Username + " In time: " + DateTime.UtcNow.ToString());
            IAuthenticationManager authentication = HttpContext.Current.GetOwinContext().Authentication;
            UserLoginResponse response = await Task.Run(() => _userService.UserSignIn(request));
            string token = string.Empty;

            if (response != null && response.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
            {
                UserSessionToken tokenObj = _authService.CreateSessionTokenObject(response.UserId, response.Email, response.StudyId);
                if (tokenObj.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
                    response.SessionToken = tokenObj.SessionToken;
                else
                {
                    response = new UserLoginResponse
                    {
                        ErrorCode = LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED,
                        ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED)
                    };
                }
            }
            LogUtil.Debug("SignIn : " + request.Username + " Out time: " + DateTime.UtcNow.ToString());
            return response;
        }

        /// <summary>
        /// Send Password on User email
        /// </summary>
        /// <param name="request">Email</param>
        /// <returns>Status</returns>
        [Route("ForgotPassword")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> ForgotPassword(ForgotPasswordRequest request)
        {
            APIResponseBase response = await Task.Run(() => _userService.SendPasswordOnEmail(request));
            return response;
        }

        /// <summary>
        /// Registered user signup
        /// </summary>
        /// <param name="request">Signup request</param>
        /// <returns>User details</returns>
        [Route("UserSignUp")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserResponse))]
        public async Task<UserResponse> UserSignUp(UserSignUpRequest request)
        {
            LogUtil.Error("UserSignUp in UserController : " + request.StudyId + ", " + request.Password);
            UserResponse response = await Task.Run(() => _userService.UserSignUp(request));
            if (response != null && response.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
            {
                UserSessionToken tokenObj = _authService.CreateSessionTokenObject(response.UserId, response.Email, response.StudyId);
                if (tokenObj.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
                    response.SessionToken = tokenObj.SessionToken;
                else
                {
                    response = new UserResponse
                    {
                        ErrorCode = LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED,
                        ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED)
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Guest user signup
        /// </summary>
        /// <param name="request">Guest user details</param>
        /// <returns>User dtails</returns>
        [Route("GuestUserSignUp")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserResponse))]
        public async Task<UserResponse> GuestUserSignUp(GuestUserSignUpRequest request)
        {
            UserResponse response = await Task.Run(() => _userService.GuestUserSignUp(request));
            if (response != null && response.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
            {
                UserSessionToken tokenObj = _authService.CreateSessionTokenObject(response.UserId, response.Email, response.StudyId);
                if (tokenObj.ErrorCode == LAMPConstants.API_SUCCESS_CODE)
                    response.SessionToken = tokenObj.SessionToken;
                else
                {
                    response = new UserResponse
                    {
                        ErrorCode = LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED,
                        ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_SESSION_TOKEN_UPDATION_FAILED)
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Save User settings
        /// </summary>
        /// <param name="request">Settings request</param>
        /// <returns>Status</returns>
        [Route("SaveUserSetting")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveUserSetting(UserSettingRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.SaveUserSetting(request.UserSettingID, request));
            return response;
        }

        /// <summary>
        /// Get user settings for a user
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>User settings details</returns>
        [Route("GetUserSetting")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserSettingResponse))]
        public async Task<UserSettingResponse> GetUserSetting(GetUserSettingRequest request)
        {
            var response = new UserSettingResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetUserSetting(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Status</returns>
        [Route("DeleteUser")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> DeleteUser(UserDeleteRequest request)
        {
            var response = new APIResponseBase();
            response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.DeleteUser(request));
            return response;
        }

        /// <summary>
        ///  User Profile Update
        /// </summary>
        /// <param name="request">User Profile Request</param>
        /// <returns>Status</returns>
        [Route("UpdateUserProfile")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> UpdateUserProfile(UserProfileRequest request)
        {
            APIResponseBase response = new APIResponseBase();
            response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.UpdateUserProfile(request));
            return response;
        }

        /// <summary>
        /// Get user profile for a user
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>User profile details</returns>
        [Route("GetUserProfile")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserProfileResponse))]
        public async Task<UserProfileResponse> GetUserProfile(GetUserProfileRequest request)
        {
            var response = new UserProfileResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetUserProfile(request.UserID));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Gets the user report.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Game details</returns>
        [Route("GetUserReport")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(UserReportResponse))]
        public async Task<UserReportResponse> GetUserReport(GetUserReportRequest request)
        {
            var response = new UserReportResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetUserReport(request.UserId));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }


        /// <summary>
        /// Save User Health Kit
        /// </summary>
        /// <param name="request">Settings request</param>
        /// <returns>Status</returns>
        [Route("SaveUserHealthKit")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveUserHealthKit(UserHealthKitRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.SaveUserHealthKit_sp((long)request.UserID, request));
            return response;
        }

        /// <summary>
        /// Save User Health Kit
        /// </summary>
        /// <param name="request">Settings request</param>
        /// <returns>Status</returns>
        [Route("SaveUserHealthKitV2")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveUserHealthKitV2(HealthKitUserRequest request)
        {
            LogUtil.Error("UserController/SaveUserHealthKitV2: xxxx1");
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
            {
                response = await Task.Run(() => _userService.SaveUserHealthKitV2(request));
            }
            return response;
        }

        /// <summary>
        /// Log any details
        /// </summary>
        /// <param name="request">Log data</param>
        /// <returns>Status</returns>
        [Route("LogData")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase LogData(LogRequest request)
        {
            APIResponseBase response = new APIResponseBase();
            try
            {
                LogUtil.Error("LogData =============================================");
                LogUtil.Error("Id: " + request.Id.ToString() + " Text: " + request.Text);
                response.ErrorCode = (int)LAMPConstants.API_SUCCESS;
            }
            catch (Exception ex)
            {
                response.ErrorCode = (int)LAMPConstants.API_FAIL;
            }
            return response;
        }

        #endregion

        #region Tips and Blogs

        /// <summary>
        /// Get Tips and update the Last tip viewed time in User Settings.
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>Tip text</returns>
        [Route("GetTips")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(TipsResponse))]
        public async Task<TipsResponse> GetTips(GetTipsRequest request)
        {
            var response = new TipsResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetTips(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get Blogs and update the Last tip viewed time in User Settings.
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>Blog list</returns>
        [Route("GetBlogs")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(BlogResponse))]
        public async Task<BlogResponse> GetBlogs(GetBlogRequest request)
        {
            var response = new BlogResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetBlogs(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get the status of Tips and Blog updations to a given user.
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>Status</returns>
        [Route("GetTipsandBlogUpdates")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(TipandBlogUpdateResponse))]
        public async Task<TipandBlogUpdateResponse> GetTipsandBlogUpdates(GetTipsandBlogsUpdateRequest request)
        {
            var response = new TipandBlogUpdateResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetTipsandBlogUpdates(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get App help.
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>Help details</returns>
        [Route("GetAppHelp")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(AppHelpResponse))]
        public async Task<AppHelpResponse> GetAppHelp(GetAppHelpRequest request)
        {
            var response = new AppHelpResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetAppHelp(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        #endregion

        #region Schedule and Settings
        /// <summary>
        /// Get the survey and game schedule by the given User Id.
        /// </summary>
        /// <param name="request">User request</param>
        /// <returns>Survey and Game details</returns>
        [Route("GetSurveyAndGameSchedule")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(ScheduleSurveyAndGame))]
        public async Task<ScheduleSurveyAndGame> GetSurveyandGameScheduleandJewelsSettings(SurveyAndGameScheduleRequest request)
        {
            var response = new ScheduleSurveyAndGame();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
            {
                response = await Task.Run(() => _userService.GetSurveyandGameScheduleandJewelsSettings(request));
            }
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get the list of distraction surveys for the given user by the CTest Id.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Distraction surveys</returns>
        [Route("GetDistractionSurveys")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(DistractionSurveyResponse))]
        public async Task<DistractionSurveyResponse> GetDistractionSurveys(DistractionSurveyRequest request)
        {
            var response = new DistractionSurveyResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetDistractionSurveys(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get Protocol Date set by the admin
        /// </summary>
        /// <param name="request">User id</param>
        /// <returns>Protocol Date</returns>
        [Route("GetProtocolDate")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(ProtocolDateResponse))]
        public async Task<ProtocolDateResponse> GetProtocolDate(ProtocolDateRequest request)
        {
            var response = new ProtocolDateResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _userService.GetProtocolDate(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }
        #endregion

    }
}
