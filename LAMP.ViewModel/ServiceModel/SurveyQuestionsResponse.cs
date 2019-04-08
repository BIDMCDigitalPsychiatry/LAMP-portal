using System;
using System.Collections.Generic;


namespace LAMP.ViewModel
{
    /// <summary>
    /// Survey Questions Response
    /// </summary>
    public class SurveyQuestionsResponse : APIResponseBase
    {
        public List<SurveyWithQuestions> Survey { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class SurveyWithQuestions
    {
        public long SurveyID { get; set; }
        public string SurveyName { get; set; }
        public string LanguageCode { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public List<SurveyQuestions> Questions { get; set; }
    }

    public class SurveyQuestions
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerType { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public List<Options> QuestionOptions { get; set; }
    }

    public class Options
    {
        public string OptionText {get; set;}
    }

   
}
