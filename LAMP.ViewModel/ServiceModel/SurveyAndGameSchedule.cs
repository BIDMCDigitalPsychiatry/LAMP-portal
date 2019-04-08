using System;
using System.Collections.Generic;


namespace LAMP.ViewModel
{
    /// <summary>
    /// Survey And Game Schedule Request
    /// </summary>
    public class SurveyAndGameScheduleRequest
    {
        public long UserId { get; set; }
        public DateTime? LastUpdatedGameDate { get; set; }
        public DateTime? LastUpdatedSurveyDate { get; set; }
        public DateTime? LastFetchedBatchDate { get; set; }
    }
    public class ScheduleSurveyAndGame : APIResponseBase
    {
        public List<ScheduleSurvey> ScheduleSurveyList { get; set; }
        public List<ScheduleGame> ScheduleGameList { get; set; }
        public DateTime LastUpdatedSurveyDate { get; set; }
        public DateTime LastUpdatedGameDate { get; set; }
        public JewelsTrailsASettings JewelsTrailsASettings { get; set; }
        public JewelsTrailsBSettings JewelsTrailsBSettings { get; set; }
        public long? ReminderClearInterval { get; set; }

        public List<BatchSchedule> BatchScheduleList { get; set; }
        public DateTime LastUpdatedBatchDate { get; set; }
        public ScheduleSurveyAndGame()
        {
            ScheduleSurveyList = new List<ScheduleSurvey>();
            ScheduleGameList = new List<ScheduleGame>();
            JewelsTrailsASettings = new JewelsTrailsASettings();
            JewelsTrailsBSettings = new JewelsTrailsBSettings();

            BatchScheduleList = new List<BatchSchedule>();
        }
    }
    public class ScheduleSurvey
    {
        public long? SurveyScheduleID { get; set; }
        public long SurveyId { get; set; }
        public string SurveyName { get; set; }
        public DateTime? Time { get; set; }
        public string SlotTime { get; set; }
        public long? RepeatID { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public bool? IsDeleted { get; set; }
        public List<string> SlotTimeOptions { get; set; }
    }
    public class ScheduleGame
    {
        public long? GameScheduleID { get; set; }
        public long CTestId { get; set; }
        public string CTestName { get; set; }
        public int? Version { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? Time { get; set; }
        public string SlotTime { get; set; }
        public long? RepeatID { get; set; }
        public bool? IsDeleted { get; set; }
        public List<string> SlotTimeOptions { get; set; }
    }
    public class SlotTimeOptions
    {
        public long? ScheduleID { get; set; }
        public string TimeString { get; set; }
        public DateTime? Time { get; set; }
    }
    public class JewelsTrailsASettings
    {
        public int? NoOfSeconds_Beg { get; set; }
        public int? NoOfSeconds_Int { get; set; }
        public int? NoOfSeconds_Adv { get; set; }
        public int? NoOfSeconds_Exp { get; set; }
        public int? NoOfDiamonds { get; set; }
        public int? NoOfShapes { get; set; }
        public int? NoOfBonusPoints { get; set; }
        public int? X_NoOfChangesInLevel { get; set; }
        public int? X_NoOfDiamonds { get; set; }
        public int? Y_NoOfChangesInLevel { get; set; }
        public int? Y_NoOfShapes { get; set; }
    }
    public class JewelsTrailsBSettings
    {
        public int? NoOfSeconds_Beg { get; set; }
        public int? NoOfSeconds_Int { get; set; }
        public int? NoOfSeconds_Adv { get; set; }
        public int? NoOfSeconds_Exp { get; set; }
        public int? NoOfDiamonds { get; set; }
        public int? NoOfShapes { get; set; }
        public int? NoOfBonusPoints { get; set; }
        public int? X_NoOfChangesInLevel { get; set; }
        public int? X_NoOfDiamonds { get; set; }
        public int? Y_NoOfChangesInLevel { get; set; }
        public int? Y_NoOfShapes { get; set; }
    }

    public class BatchScheduleRequest
    {
        public long UserId { get; set; }
        public DateTime? LastFetchedBatchDate { get; set; }
    }

    public class BatchScheduleResponse : APIResponseBase
    {
        public List<BatchSchedule> BatchScheduleList { get; set; }
        public DateTime LastUpdatedBatchDate { get; set; }
        public BatchScheduleResponse()
        {
            BatchScheduleList = new List<BatchSchedule>();
        }
    }

    public class BatchSchedule
    {
        public long? BatchScheduleId { get; set; }
        public string BatchName { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? Time { get; set; }
        public long RepeatId { get; set; }
        public bool? IsDeleted { get; set; }
        public List<BatchScheduleSurvey_CTest> BatchScheduleSurvey_CTest { get; set; }
        public List<BatchScheduleCustomTime> BatchScheduleCustomTime { get; set; }
        public string SlotTime { get; set; }
    }

    public class BatchScheduleSurvey_CTest
    {
        public long? BatchScheduleId { get; set; }
        /// <summary>
        /// 1: Survey, 2:CTest ( Game )
        /// </summary>
        public Int16 Type { get; set; }
        public long? ID { get; set; }
        public Int32 Version { get; set; }
        public Int16 Order { get; set; }
    }


    public class BatchScheduleCustomTime
    {
        public long? BatchScheduleId { get; set; }
        public string Time { get; set; }
    }
}
