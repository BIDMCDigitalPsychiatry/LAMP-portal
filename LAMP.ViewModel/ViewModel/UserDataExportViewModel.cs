using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMP.ViewModel
{
    /// <summary>
    /// User Data Export View Model
    /// </summary>
    public class UserDataExportViewModel
    {
        public List<UserDetails> UserDetails { get; set; }
        public List<SurveyHeader> SurveyHeader { get; set; }
        public List<SurveyExport> Surveys { get; set; }
        public List<CognitionTestHeader> CognitionTestHeader { get; set; }
        public List<NBackCTest> NBackCTestList { get; set; }
        public List<TrailsBCTest> TrailsBCTestList { get; set; }
        public List<TrailsBCTestDetail> TrailsBCTestDetailList { get; set; }
        public List<SpatialCTestForward> SpatialCTestForwardList { get; set; }
        public List<SpatialCTestBackward> SpatialCTestBackwardList { get; set; }
        public List<SimpleMemoryCTest> SimpleMemoryCTestList { get; set; }
        public List<Serial7CTest> Serial7CTestList { get; set; }
        public List<ThreeDFigureCTest> ThreeDFigureCTestList { get; set; }
        public List<VisualAssociationCTest> VisualAssociationCTestList { get; set; }
        public List<DigitSpanCTestForward> DigitSpanCTestForwardList { get; set; }
        public List<CatAndDogNewCTest> CatAndDogNewCTestList { get; set; }
        public List<TemporalOrderCTest> TemporalOrderCTestList { get; set; }
        public List<DigitSpanCTestBackward> DigitSpanCTestBackwardList { get; set; }
        public List<NBackNewCTest> NBackNewCTestList { get; set; }
        public List<TrailsBNewCTest> TrailsBNewCTest { get; set; }
        public List<TrailsBNewCTestDetail> TrailsBNewCTestDetailList { get; set; }
        public List<TrailsBDotTouchCTest> TrailsBDotTouchCTestList { get; set; }
        public List<TrailsBDotTouchCTestDetail> TrailsBDotTouchCTestDetailList { get; set; }
        public List<JewelsTrailsACTest> JewelsTrailsACTestList { get; set; }
        public List<JewelsTrailsACTestDetail> JewelsTrailsACTestDetailList { get; set; }
        public List<JewelsTrailsBCTest> JewelsTrailsBCTestList { get; set; }
        public List<JewelsTrailsBCTestDetail> JewelsTrailsBCTestDetailList { get; set; }
        public List<CognitionOverallPoints> CognitionOverallPoints { get; set; }
        public List<CallHistory> CallHistory { get; set; }
        public List<LocationExport> Locations { get; set; }
        public List<EnvironmentExport> Environment { get; set; }
        public List<HealthKitHeader> HealthKitHeader { get; set; }
        public List<HealthKitData> HealthKitData { get; set; }

        public List<ScratchImageCTest> ScratchImageCTestList { get; set; }
        public List<SpinWheelCTestDetail> SpinWheelCTestList { get; set; }
    }
    public class UserSurveyDetail
    {
        public long SurveyResultDtlID { get; set; }
        public long UserId { get; set; }
        public long SurveyResultID { get; set; }
        public string Question { get; set; }
        public string EnteredAnswer { get; set; }
        public int TimeTaken { get; set; }
        public string ClickRange { get; set; }

    }
    public class UserDetails
    {
        public long UserID { get; set; }
        public string StudyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> RegisteredOn { get; set; }
        public DateTime? LastSurveyDate { get; set; }
        public Nullable<decimal> Points { get; set; }
        public DateTime LastSurveyDateNew { get; set; }
        public string LastSurveyDateString { get; set; }
    }

    public class SurveyHeader
    {
        public long UserId { get; set; }
        public long SurveyResultID { get; set; }
        public string SurveyName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_string { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? TimeTaken { get; set; }
        public TimeSpan Duration { get; set; }
        public bool? IsDistraction { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }

    }
    public class SurveyExport
    {
        public long SurveyResultDtlID { get; set; }
        public long UserId { get; set; }
        public long SurveyResultID { get; set; }
        public string Question { get; set; }
        public string EnteredAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public int? TimeTaken { get; set; }
        public string Time_Taken { get; set; }
        public string ClickRange { get; set; }

    }

    public class CognitionTestHeader
    {
        public long UserId { get; set; }
        public int CTestID { get; set; }
        public string CTestName { get; set; }
        public int TotalGames { get; set; }
        public string Time_Taken { get; set; }
        public decimal? TotalPoints { get; set; }
        public int? TimeTaken { get; set; }
        public DateTime LastTestDate { get; set; }
        public string LastTestDateString { get; set; }

        public DateTime LastTestStartTime { get; set; }
        public DateTime? LastTestEndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Duration_string { get; set; }
    }
    public class NBackCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalQuestions { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public int? Version { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TrailsBCTest
    {
        public long TrailsBResultID { get; set; }
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public string DetailString { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TrailsBCTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long TrailsBResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    public class SpatialCTestForward
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class SpatialCTestBackward
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class SimpleMemoryCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalQuestions { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public int? Version { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class Serial7CTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalQuestions { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public int? Version { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class ThreeDFigureCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public string ActualImageFileName { get; set; }
        public string FileName { get; set; }
        public string DrawnFigFileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class VisualAssociationCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalQuestions { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public int? Version { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class DigitSpanCTestForward
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class CatAndDogNewCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TemporalOrderCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public int? Version { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class DigitSpanCTestBackward
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class NBackNewCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalQuestions { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TrailsBNewCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long TrailsBNewResultID { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public int? Version { get; set; }
        public string DetailString { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TrailsBNewCTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long TrailsBNewResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    public class TrailsBDotTouchCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long TrailsBDotTouchResultID { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string Duration_String { get; set; }
        public string DetailString { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class TrailsBDotTouchCTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long TrailsBDotTouchResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    public class JewelsTrailsACTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long JewelsTrailsAResultID { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public string TotalJewelsCollected { get; set; }
        public string TotalBonusCollected { get; set; }
        public string Duration_String { get; set; }
        public string DetailString { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class JewelsTrailsACTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long JewelsTrailsAResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    public class JewelsTrailsBCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public int? TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string StartTimeString { get; set; }
        public int Duration { get; set; }
        public long JewelsTrailsBResultID { get; set; }
        public string TotalJewelsCollected { get; set; }
        public string TotalBonusCollected { get; set; }
        public string Duration_String { get; set; }
        public string DetailString { get; set; }

        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
    public class JewelsTrailsBCTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long JewelsTrailsBResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    public class CognitionOverallPoints
    {
        public long UserId { get; set; }
        public DateTime LastResult { get; set; }
        public string LastResultString { get; set; }
        public int? OverallRating { get; set; }
    }

    public class CallHistory
    {
        public long UserId { get; set; }
        public long HelpCallID { get; set; }
        public string CallType { get; set; }
        public string CalledNumber { get; set; }
        public DateTime CallDateTime { get; set; }
        public string CallDateTimeString { get; set; }
        public long CallDuraion { get; set; }
        public string Duration_String { get; set; }
    }

    public class LocationExport
    {
        public long UserId { get; set; }
        public long LocationID { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }


    public class EnvironmentExport
    {
        public long UserId { get; set; }
        public long LocationID { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
    }
    //--HealthKit Data
    public class HealthKitHeader
    {
        public long HKBasicInfoID { get; set; }
        public long UserId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DOBString { get; set; }
        public string Sex { get; set; }
        public string BloodType { get; set; }
    }

    public class HealthKitData
    {
        public long HKDailyValueID { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public string RespiratoryRate { get; set; }
        public string Sleep { get; set; }
        public string Steps { get; set; }
        public string FlightClimbed { get; set; }
        public string Segment { get; set; }
        public string Distance { get; set; }
    }

    public class ScratchImageCTest
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long ScratchImageResultID { get; set; }
        public string DrawnFigFileName { get; set; }
        public string FileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? Duration { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public string Duration_String { get; set; }
        public string StartTimeString { get; set; }
        public long? AdminBatchSchID { get; set; }
    }

    public class SpinWheelCTestDetail
    {
        public int CTestID { get; set; }
        public long UserId { get; set; }
        public long SpinWheelResultID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CollectedStars { get; set; }
        public int? Duration { get; set; }
        public string Duration_String { get; set; }
    }

    public class HealthKitV2Data
    {
        public long HKParamValueID { get; set; }
        public long UserId { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public TimeSpan Time { get; set; }
        public string TimeString { get; set; }
        public long HKParamID { get; set; }
        public string HKParamName { get; set; }
        public string Value { get; set; }
    }
}
