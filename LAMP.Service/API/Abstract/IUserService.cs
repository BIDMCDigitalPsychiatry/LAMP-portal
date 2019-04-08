using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Interface IUserService for capable of class UserService
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// User Sign in
        /// </summary>
        /// <param name="request">Sign in request</param>
        /// <returns>User setting details</returns>
        UserLoginResponse UserSignIn(UserSignInRequest request);

        /// <summary>
        /// Send Password on mail
        /// </summary>
        /// <param name="request">Reset password request</param>
        /// <returns>Status</returns>
        APIResponseBase SendPasswordOnEmail(ForgotPasswordRequest request);

        /// <summary>
        /// User Signup
        /// </summary>
        /// <param name="request">Signup request</param>
        /// <returns>User details</returns>
        UserResponse UserSignUp(UserSignUpRequest request);

        /// <summary>
        /// Guest user Signup
        /// </summary>
        /// <param name="request">Signup request</param>
        /// <returns>User details</returns>
        UserResponse GuestUserSignUp(GuestUserSignUpRequest request);

        /// <summary>
        /// Save User settings
        /// </summary>
        /// <param name="settingId">Setting Id</param>
        /// <param name="request">Setting request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveUserSetting(long settingId, UserSettingRequest request);

        /// <summary>
        /// Get User setting details
        /// </summary>
        /// <param name="request">User Id</param>
        /// <returns>User setting details</returns>
        UserSettingResponse GetUserSetting(GetUserSettingRequest request);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="request">Delete request</param>
        /// <returns>Status</returns>
        APIResponseBase DeleteUser(UserDeleteRequest request);

        /// <summary>
        /// User Profile Update
        /// </summary>
        /// <param name="request">User Profile request</param>
        /// <returns>Status</returns>
        APIResponseBase UpdateUserProfile(UserProfileRequest request);

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User Profile details</returns>
        UserProfileResponse GetUserProfile(long userId);

        /// <summary>
        /// Saves the user health kit.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveUserHealthKit(long userId, UserHealthKitRequest request);

        /// <summary>
        /// Saves the user health kit hourly.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveUserHealthKitHourly(long userId, UserHealthKitRequest request);

        /// <summary>
        /// Saves the user health kit sp.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveUserHealthKit_sp(long userId, UserHealthKitRequest request);
        APIResponseBase SaveUserHealthKitV2(HealthKitUserRequest request);

        /// <summary>
        /// Gets the user report.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        UserReportResponse GetUserReport(long userId);

        /// <summary>
        /// Get Survey and Game schedules by the given UserId.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ScheduleSurveyAndGame GetSurveyandGameScheduleandJewelsSettings(SurveyAndGameScheduleRequest request);

        /// <summary>
        /// Get the list of distraction surveys for the given user by the CTest Id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DistractionSurveyResponse GetDistractionSurveys(DistractionSurveyRequest request);

        /// <summary>
        /// Get tips
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TipsResponse GetTips(GetTipsRequest request);

        /// <summary>
        /// GetBlogs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BlogResponse GetBlogs(GetBlogRequest request);

        /// <summary>
        ///  Get the status of Tips and Blog updations to a given user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TipandBlogUpdateResponse GetTipsandBlogUpdates(GetTipsandBlogsUpdateRequest request);

        /// <summary>
        /// Get app help.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AppHelpResponse GetAppHelp(GetAppHelpRequest request);

        /// <summary>
        /// Get Protocol Date set by the admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ProtocolDateResponse GetProtocolDate(ProtocolDateRequest request);
    }    
}
