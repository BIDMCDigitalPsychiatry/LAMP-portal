using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionNBackViewModel
    /// </summary>
    public class CognitionNBackNewViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public List<CognitionNBackNewDetail> CognitionNBackNewDetailList { get; set; }
        public StaticPagedList<CognitionNBackNewDetail> PagedCTest_NBackNewDetailList { get; set; }
        public CognitionNBackNewSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionNBackNewDetail> CTest_NBackNewResultList { get; set; }
        public CognitionNBackNewViewModel()
        {
            SortPageOptions = new CognitionNBackNewSortPageOptions();
            CTest_NBackNewResultList = new List<CognitionNBackNewDetail>();
        }

    }

    /// <summary>
    /// Class CognitionNBackDetail
    /// </summary>
    public class CognitionNBackNewDetail
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
        public byte? Status { get; set; }
    }

    /// <summary>
    /// Class CognitionNBackSortPageOptions
    /// </summary>
    public class CognitionNBackNewSortPageOptions : PagingBase
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

