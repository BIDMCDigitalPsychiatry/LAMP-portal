using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Interface IUserAdminService for capable of class UserAdminService
    /// </summary>
    public interface IUserAdminService
    {
        #region Users

        /// <summary>
        /// Gets the user export detail list.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        UserDataExportViewModel GetUserExportDetailList(string userIds, string fromDate, string toDate);

        /// <summary>
        /// Saves the user details
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        UserViewModel SaveUser(UserViewModel userModel, Stream stream);

        /// <summary>
        /// Get the User  list
        /// </summary>
        /// <param name="model"></param>
        /// <returns>User detail list</returns>
        UserListViewModel GetUsers(UserListViewModel model);

        /// <summary>
        /// gets survey results
        /// </summary>
        /// <param name="SurveyResultID"></param>
        /// <returns>survey details</returns>
        SurveyResultsViewModel GetSurveyResults(long SurveyResultID, long AdminBatchSchID);

        /// <summary>
        /// gets user details
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>User details</returns>
        UserViewModel GetUserDetails(long UserID);

        /// <summary>
        /// delets a user is delete status is changed to true
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserListViewModel DeleteUser(long UserID);

        /// <summary>
        /// get selected user list
        /// </summary>
        /// <param name="strSearch"></param>
        /// <returns>selected user list</returns>
        UserListViewModel GetSelectedUsers(string strSearch);

        /// <summary>
        /// user status is changed Active ,inactive
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        UserListViewModel ChangeUserStatus(long UserID, bool Status);

        /// <summary>
        /// Get user Activities details
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>user activities details</returns>
        UserActivitiesViewModel GetUserActivities(long UserID);

        #endregion

        #region Games

        /// <summary>
        /// To get the details of  the game N back for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionNBackViewModel</returns>
        CognitionNBackViewModel GetCognitionNBack(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Cat and Dog for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionCatDogViewModel</returns>
        CognitionCatDogViewModel GetCognitionCatDog(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Serial 7 for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionSerial7ViewModel</returns>
        CognitionSerial7ViewModel GetCognitionSerial7(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Simple Memory for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionSimpleMemoryViewModel</returns>
        CognitionSimpleMemoryViewModel GetCognitionSimpleMemory(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Visual Association for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionVisualAssociationViewModel</returns>
        CognitionVisualAssociationViewModel GetCognitionVisualAssociation(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game TrailsB for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionTrailsBViewModel</returns>
        CognitionTrailsBViewModel GetCognitionTrailsB(long UserID, long adminBatchSchID);

        /// <summary>
        /// Gets the cognition trails b new.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        CognitionTrailsBNewViewModel GetCognitionTrailsBNew(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Digit Span for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionDigitSpanViewModel</returns>
        CognitionDigitSpanViewModel GetCognitionDigitSpan(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the Details Of the game 3D figures for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>Cognition3DFigureViewModel</returns>
        Cognition3DFigureViewModel GetCognition3DFigure(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the details of  the game Spatial Span for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionSpatialSpanViewModel</returns>
        CognitionSpatialSpanViewModel GetCognitionSpatialSpan(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get CatAndDogNew game details
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        CognitionCatAndDogNewViewModel GetCognitionCatAndDogNew(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get Temporal Order game details
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        CognitionTemporalOrderViewModel GetCognitionTemporalOrder(long UserID, long adminBatchSchID);

        /// <summary>
        /// Gets the cognition n back new.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        CognitionNBackNewViewModel GetCognitionNBackNew(long UserID, long adminBatchSchID);

        /// <summary>
        /// Gets the cognition trails b dot touch.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        CognitionTrailsBDotTouchViewModel GetCognitionTrailsBDotTouch(long UserID, long adminBatchSchID);

        /// <summary>
        /// Gets the cognition jewels trails a.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        CognitionJewelsTrailsAViewModel GetCognitionJewelsTrailsA(long UserID, long adminBatchSchID);

        /// <summary>
        /// Gets the cognition jewels trails b.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        CognitionJewelsTrailsBViewModel GetCognitionJewelsTrailsB(long UserID, long adminBatchSchID);

        /// <summary>
        /// To get the Details Of the game Scratch Image for a user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>CognitionScratchImageViewModel</returns>
        CognitionScratchImageViewModel GetCognitionScratchImage(long UserID, long adminBatchSchID);

        CognitionSpinWheelViewModel GetCognitionSpinWheel(long UserID);
        #endregion

        #region Tips and Blogs

        /// <summary>
        /// Saves the tips.
        /// </summary>
        /// <param name="tipsViewModel">The tips view model.</param>
        /// <returns></returns>
        TipsViewModel SaveTips(TipsViewModel tipsViewModel);

        /// <summary>
        /// Saves the blog.
        /// </summary>
        /// <param name="blogsViewModel">The blogs view model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        BlogsViewModel SaveBlog(BlogsViewModel blogsViewModel, Stream stream);

        /// <summary>
        /// Gets all blogs list.
        /// </summary>
        /// <returns></returns>
        TipsBlogsViewModel GetAllBlogsList(TipsBlogsViewModel model);

        /// <summary>
        /// Deletes the blog.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        TipsBlogsViewModel DeleteBlog(long blogId);

        /// <summary>
        /// Gets the blog details.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        TipsBlogsViewModel GetBlogDetails(long blogId);

        /// <summary>
        /// Gets the tips.
        /// </summary>
        /// <param name="tipsViewModel">The tips view model.</param>
        /// <returns></returns>
        TipsViewModel GetTips(TipsViewModel tipsViewModel);

        #endregion

        #region Admin

        /// <summary>
        /// Saves the admin.
        /// </summary>
        /// <param name="adminModel">The admin model.</param>
        /// <returns></returns>
        AdminViewModel SaveAdmin(AdminViewModel adminModel);

        /// <summary>
        /// Gets the admin details.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        AdminViewModel GetAdminDetails(long adminId);

        /// <summary>
        /// Gets all admins.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        AdminListViewModel GetAllAdmins(AdminListViewModel model);

        /// <summary>
        /// Deletes the admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        AdminListViewModel DeleteAdmin(long adminId);

        #endregion

        #region AppHelp

        /// <summary>
        /// Gets the application help of logged in admin.
        /// </summary>
        /// <param name="appHelpViewModel">The application help view model.</param>
        /// <returns></returns>
        AppHelpViewModel GetAppHelpOfLoggedInAdmin(AppHelpViewModel appHelpViewModel);

        /// <summary>
        /// Saves the application help.
        /// </summary>
        /// <param name="appHelpViewModel">The application help view model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        AppHelpViewModel SaveAppHelp(AppHelpViewModel appHelpViewModel, Stream stream);

        /// <summary>
        /// Gets all application help.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        AppHelpViewModelList GetAllAppHelp(AppHelpViewModelList model);

        /// <summary>
        /// Deletes the application help.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        AppHelpViewModelList DeleteAppHelp(long helpId);

        /// <summary>
        /// Gets the application help details by help identifier.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        AppHelpViewModel GetAppHelpDetailsByHelpId(long helpId);

        #endregion

        #region Schedule and Settings

        /// <summary>
        /// Gets the schedule view model details.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        ScheduleViewModel GetScheduleViewModelDetails(ScheduleViewModel scheduleViewModel);

        /// <summary>
        /// Saves the shedule survey and game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        ScheduleViewModel SaveSheduleSurveyAndGame(ScheduleViewModel scheduleViewModel);

        /// <summary>
        /// Gets the distraction survey details.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        DistractionSurveyViewModel GetDistractionSurveyDetails(DistractionSurveyViewModel DistractionSurveyViewModel);

        /// <summary>
        /// Saves the distraction survey details.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        DistractionSurveyViewModel SaveDistractionSurveyDetails(DistractionSurveyViewModel DistractionSurveyViewModel);

        /// <summary>
        /// Gets the type of the jewels trails settings by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="LoggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        JewelsTrailsSettings GetJewelsTrailsSettingsByType(string type, long LoggedInUserId);

        /// <summary>
        /// Saves the jewels trials settings.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        DistractionSurveyViewModel SaveJewelsTrialsSettings(DistractionSurveyViewModel DistractionSurveyViewModel);

        /// <summary>
        /// Saves the expiry option.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
       DistractionSurveyViewModel SaveExpiryOption(DistractionSurveyViewModel DistractionSurveyViewModel);

        /// <summary>
        /// Gets the schedule view model details for admin.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel GetScheduleViewModelDetailsForAdmin(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel);

        /// <summary>
        /// Saves the shedule survey.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel SaveSheduleSurvey(ScheduleGameSurveyViewModel scheduleViewModel);

        /// <summary>
        /// Gets the survey schedule details.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel GetSurveyScheduleDetailsByAdminSurveySchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel);

        /// <summary>
        /// Saves the shedule game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel SaveSheduleGame(ScheduleGameSurveyViewModel scheduleViewModel);

        /// <summary>
        /// Gets the survey scheduled list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        ScheduleSurveyListViewModel GetSurveyScheduledList(ScheduleSurveyListViewModel model);

        /// <summary>
        /// Gets the cognition version.
        /// </summary>
        /// <param name="cognitionId">The cognition identifier.</param>
        /// <returns></returns>
        List<SelectListItem> GetCognitionVersion(long cognitionId);

        /// <summary>
        /// Gets the game scheduled list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        ScheduleGameListViewModel GetGameScheduledList(ScheduleGameListViewModel model);

        /// <summary>
        /// Deletes the survey schedule.
        /// </summary>
        /// <param name="surveyId">The survey identifier.</param>
        /// <returns></returns>
        ScheduleSurveyListViewModel DeleteSurveySchedule(long surveyId);

        /// <summary>
        /// Deletes the game schedule.
        /// </summary>
        /// <param name="cTestId">The c test identifier.</param>
        /// <returns></returns>
        ScheduleGameListViewModel DeleteGameSchedule(long cTestId);

        /// <summary>
        /// Gets the game schedule details by admin c test SCH identifier.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel GetGameScheduleDetailsByAdminCTestSchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel"></param>
        /// <returns></returns>
        ScheduleGameSurveyViewModel GetBatchScheduleDetailsByAdminBatchSchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel);
        //ScheduleBatchViewModel GetBatchScheduleDetailsByAdminBatchSchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel);
        /// <summary>
        /// Save protocol date
        /// </summary>
        /// <param name="protocolModel">The protocol viewmodel</param>
        /// <returns></returns>
        ViewModelBase SaveProtocolDate(ProtocolViewModel protocolModel);

        /// <summary>
        /// Get protocol date
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        ProtocolViewModel getProtocolDate(long UserId);
        #endregion

        #region SurveyManagement

        /// <summary>
        /// Gets the surveys.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        SurveyListViewModel GetSurveys(SurveyListViewModel model);

        /// <summary>
        /// Saves the survey.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        SurveyViewModel SaveSurvey(SurveyViewModel model);

        /// <summary>
        /// Gets the survey by survey identifier.
        /// </summary>
        /// <param name="SurveyId">The survey identifier.</param>
        /// <returns></returns>
        SurveyViewModel GetSurveyBySurveyId(long SurveyId);

        /// <summary>
        /// Deletes the survey.
        /// </summary>
        /// <param name="SurveyId">The survey identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <returns></returns>
        SurveyListViewModel DeleteSurvey(long SurveyId, long UserId);

        /// <summary>
        /// Deletes the survey question.
        /// </summary>
        /// <param name="QuestionId">The question identifier.</param>
        /// <returns></returns>
        SurveyViewModel DeleteSurveyQuestion(long QuestionId);

        /// <summary>
        /// Edits the survey question.
        /// </summary>
        /// <param name="QuestionId">The question identifier.</param>
        /// <returns></returns>
        SurveyViewModel EditSurveyQuestion(long QuestionId);

        #endregion

        /// <summary>
        /// DeleteBatchSchedule
        /// </summary>
        /// <param name="batchId">batchId</param>
        /// <returns></returns>       
        ScheduleListViewModel DeleteBatchSchedule(long batchId);
        
    }
}
