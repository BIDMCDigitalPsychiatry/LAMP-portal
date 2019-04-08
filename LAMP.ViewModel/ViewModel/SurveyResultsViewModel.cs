using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class SurveyResultsViewModel
    /// </summary>
    public class SurveyResultsViewModel : ViewModelBase
    {
        public long SurveyResultID { get; set; }
        public long UserID { get; set; }
        public string SurveyName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
        public DateTime SurveyDate { get; set; }
        public string StudyId { get; set; }

        public List<SurveyResultsDetail> SurveyResultsDetailList { get; set; }
        public StaticPagedList<SurveyResultsDetail> PagedSurveyResultsDetailList { get; set; }
        public QAndASortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<SurveyResultsDetail> QuestAndAnsList { get; set; }
        public SurveyResultsViewModel()
        {
            SortPageOptions = new QAndASortPageOptions();
            QuestAndAnsList = new List<SurveyResultsDetail>();
        }
    }
    /// <summary>
    /// Class SurveyResultsDetail
    /// </summary>
    public class SurveyResultsDetail
    {
        public long SurveyResultDtlID { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string EnteredAnswer { get; set; }
        //public int? TimeTaken { get; set; }
        public double? TimeTaken { get; set; }
        public string ClickRange { get; set; }
    }
    /// <summary>
    /// Class QAndASortPageOptions
    /// </summary>
    public class QAndASortPageOptions : PagingBase
    {
        /// <summary>
        /// Current sort field
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// Current sort direction.
        /// </summary>
        public string SortOrder { get; set; }
    }
}
