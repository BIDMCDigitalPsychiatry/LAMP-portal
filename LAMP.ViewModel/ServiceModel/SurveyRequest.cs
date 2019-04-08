using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    ///  Class SurveyRequest
    /// </summary>
    public class SurveyRequest
    {
        public long UserID { get; set; }
        public byte SurveyType { get; set; }
        public string SurveyName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
        public decimal Point { get; set; }
        public byte StatusType { get; set; }
        public bool? IsDistraction { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
        public long? SurveyID { get; set; }
        public List<SurveyQueAndAns> QuestAndAnsList { get; set; }
        public SurveyRequest()
        {
            QuestAndAnsList = new List<SurveyQueAndAns>();
        }
    }
    
}
