using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionNBackViewModel
    /// </summary>
    public class CognitionNBackViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public List<CognitionNBackDetail> CognitionNBackDetailList { get; set; }
        public StaticPagedList<CognitionNBackDetail> PagedCTest_NBackDetailList { get; set; }
        public CognitionNBackSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionNBackDetail> CTest_NBackResultList { get; set; }
        public CognitionNBackViewModel()
        {
            SortPageOptions = new CognitionNBackSortPageOptions();
            CTest_NBackResultList = new List<CognitionNBackDetail>();
        }
      
    }

    /// <summary>
    /// Class CognitionNBackDetail
    /// </summary>
    public class CognitionNBackDetail
    {
        public long NBackResultID { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? Version { get; set; }
        public byte? Status { get; set; }
    }

    /// <summary>
    /// Class CognitionNBackSortPageOptions
    /// </summary>
    public class CognitionNBackSortPageOptions : PagingBase
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
