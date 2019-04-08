using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{    
    /// <summary>
    /// Class SurveyResponse
    /// </summary>
    public class SurveyResponse : APIResponseBase
    {
        public List<SurveyQueAndAns> SurveyQueAndAnsList { get; set; }
        public SurveyResponse()
        {
            SurveyQueAndAnsList = new List<SurveyQueAndAns>();
        }
    }

    /// <summary>
    /// Class CompletedSurveyResponse
    /// </summary>
    public class CompletedSurveyResponse : APIResponseBase
    {
        public List<CompletedSurvey> CompletedSurveyList { get; set; }
        public CompletedSurveyResponse()
        {
            CompletedSurveyList = new List<CompletedSurvey>();
        }
    }
    /// <summary>
    /// Class CompletedSurvey
    /// </summary>
    public class CompletedSurvey
    {
        public long SurveyResultID { get; set; }
        public string SurveyName { get; set; }
        public DateTime EndTime { get; set; }
    }
    /// <summary>
    /// Class SurveyQueAndAns
    /// </summary>
    public class SurveyQueAndAns
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public Nullable<int> TimeTaken { get; set; }
        public string ClickRange { get; set; }

    }
}
