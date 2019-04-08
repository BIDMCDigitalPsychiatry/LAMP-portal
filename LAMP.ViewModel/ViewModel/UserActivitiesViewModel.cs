using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    ///  Class UserActivitiesViewModel
    /// </summary>
    public class UserActivitiesViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public string StudyId { get; set; }
        // Survey
        public UserSurveyResult SurveyList { get; set; }
        public StaticPagedList<UserSurvey> PagedSurveyList { get; set; }
        public SortPageOptions SurveyListSortPageOptions { get; set; } 
        // Cognition
        public UserCognitionResult CognitionList { get; set; }
        public StaticPagedList<UserCognitionResult> PagedCognitionList { get; set; }
        public SortPageOptions CognitionListSortPageOptions { get; set; }
        //Mindfulness Game
        public UserMindfulnessGameResult MindfulnessGameList { get; set; }
        public StaticPagedList<UserMindfulnessGameResult> PagedMindfulnessGameList { get; set; }
        public SortPageOptions MindfulnessGameListSortPageOptions { get; set; }
        // Location
        public List<UserEnvironment> _LocationList { get; set; }        
        public StaticPagedList<UserEnvironment> PagedLocationList { get; set; }
        public SortPageOptions LocationSortPageOptions { get; set; }
        // Enviroment
        public List<UserEnvironment> _EnvironmentList { get; set; }
        public StaticPagedList<UserEnvironment> PagedEnvironmentList { get; set; }
        public SortPageOptions EnvironmentSortPageOptions { get; set; }
        // Call History
        public List<UserCallHistory> CallHistoryList { get; set; }
        public StaticPagedList<UserCallHistory> PagedCallList { get; set; }
        public SortPageOptions CallHistorySortPageOptions { get; set; }        
        
        // Batch Details
        public BatchSchedule_UAResult BatchScheduleList { get; set; }
        public StaticPagedList<BatchSchedule_UAResult> PagedBatchCognitionList { get; set; }
        public SortPageOptions BatchCognitionListSortPageOptions { get; set; } 
        public UserActivitiesViewModel()
        {
            SurveyList = new UserSurveyResult();
            CognitionList = new UserCognitionResult();
            MindfulnessGameList = new UserMindfulnessGameResult();

            _LocationList = new List<UserEnvironment>();
            CallHistoryList = new List<UserCallHistory>();

            SurveyListSortPageOptions = new SortPageOptions();
            CognitionListSortPageOptions = new SortPageOptions();   
            LocationSortPageOptions = new SortPageOptions();
            EnvironmentSortPageOptions = new SortPageOptions(); 
            CallHistorySortPageOptions = new SortPageOptions();

            BatchScheduleList = new BatchSchedule_UAResult();
        }
    }
    /// <summary>
    /// Class UserSurveyResult
    /// </summary>
    public class UserSurveyResult
    {
        public string LastSurveyRating { get; set; }
        public string LastSurveyDate { get; set; }
        public string OverAllRating { get; set; }
        public Int16 TotalSurveys { get; set; }
        public decimal SurveyPoints { get; set; }
        public List<UserSurvey> UserSurveyList { get; set; }
    }

    /// <summary>
    /// Class UserSurvey
    /// </summary>
    public class UserSurvey
    {        
        public long SurveyResultID { get; set; }
        public string SurveyName { get; set; }
        public string Rating { get; set; }
        public string Date_Time { get; set; }
        public decimal? SurveyPoints { get; set; }
        public byte? Status { get; set; }
        public long? AdminBatchSchID { get; set; }
        public bool IsDistraction { get; set; }
        public bool IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long SurveyID { get; set; }
    }
    /// <summary>
    /// Class UserCognitionResult
    /// </summary>
    public class UserCognitionResult
    {
        public string LastResultRating { get; set; }
        public string LastResultDate { get; set; }
        public string OverAllRating { get; set; }
        public Int16 TotalGames { get; set; }
        public List<UserCognition> UserCognitionList { get; set; }
    }

    /// <summary>
    /// Class UserCognitionResult
    /// </summary>
    public class UserMindfulnessGameResult
    {
        public string LastResultRating { get; set; }
        public string LastResultDate { get; set; }
        public string OverAllRating { get; set; }
        public Int16 TotalGames { get; set; }
        public List<UserCognition> UserMindfulnessGameList { get; set; }
    }

    /// <summary>
    /// Class UserCognition
    /// </summary>
    public class UserCognition
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public Int16 CognitionType { get; set; }
        public string CognitionName { get; set; }
        public decimal Rating { get; set; }
        public string RatingName { get; set; }
        public string OverAllRating { get; set; }
        public Int16 TotalGames { get; set; }
        public string Date_Time { get; set; }
        public string GameName { get; set; }
        public decimal? EarnedPoints { get; set; }
        public long? AdminBatchSchID { get; set; }
        public bool IsDistraction { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public int Order { get; set; }
    }

    /// <summary>
    /// Class UserEnvironment
    /// </summary>
    public class UserEnvironment
    {
        public string Location { get; set; }
        public string Address { get; set; }
        public string Date_Time { get; set; }
        public byte Type { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    /// <summary>
    /// Class UserCallHistory
    /// </summary>
    public class UserCallHistory
    {
        public string CalledNumber { get; set; }
        public string Date_Time { get; set; }
        public string Duration { get; set; }
        public byte Type { get; set; }
    }

    //=============================
    /// <summary>
    /// Class BatchSchedule_UAResult
    /// </summary>
    public class BatchSchedule_UAResult
    {
        public string LastResultRating { get; set; }
        public string LastResultDate { get; set; }
        public string OverAllRating { get; set; }
        public Int16 TotalGames { get; set; }
        public List<Batch_UA> BatchList { get; set; }
    }

    /// <summary>
    /// Class Batch_UA
    /// </summary>
    public class Batch_UA
    {
        public BatchDetails_UA BatchDetails { get; set; }
        public List<BatchScheduleItem_UA> BatchScheduleItemList { get; set; }
    }

    /// <summary>
    /// Class BatchDetails_UA
    /// </summary>
    public class BatchDetails_UA
    {
        public string Name { get; set; }
        public string SlotTime { get; set; }
        public string RepeatInterval { get; set; }
        public string ScheduledDate { get; set; }
        public Int32 SurveyCount { get; set; }
        public Int32 GameCount { get; set; }
    }
    /// <summary>
    /// Class BatchScheduleItem_UA
    /// </summary>
    public class BatchScheduleItem_UA
    {
        public long UserID { get; set; }
        public Int16 Type { get; set; }
        public string Name { get; set; }
        public string Date_Games { get; set; }
        public string Status_Point { get; set; }
        public long AdminBatchSchID { get; set; }
        public Int16 CognitionType { get; set; }
        public long SurveyResultID { get; set; }
        public bool IsDistraction { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public int Order { get; set; }
    }    

}
