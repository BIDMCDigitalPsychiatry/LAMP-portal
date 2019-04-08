using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionSimpleMemoryViewModel
    /// </summary>
    public class CognitionSimpleMemoryViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionSimpleMemoryDetail> PagedCTest_SimpleMemoryDetailList { get; set; }
        public CognitionSimpleMemorySortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionSimpleMemoryDetail> CTest_SimpleMemoryResultList { get; set; }
        public CognitionSimpleMemoryViewModel()
        {
            SortPageOptions = new CognitionSimpleMemorySortPageOptions();
            CTest_SimpleMemoryResultList = new List<CognitionSimpleMemoryDetail>();
        }      
    }
    /// <summary>
    /// Class CognitionSimpleMemoryDetail
    /// </summary>
    public class CognitionSimpleMemoryDetail
    {
        public long SimpleMemoryResultID { get; set; }
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
    /// Class CognitionSimpleMemorySortPageOptions
    /// </summary>
    public class CognitionSimpleMemorySortPageOptions : PagingBase
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
