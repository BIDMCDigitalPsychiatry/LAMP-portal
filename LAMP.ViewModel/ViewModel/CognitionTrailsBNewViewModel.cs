using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionTrailsBNewViewModel
    /// </summary>
    public class CognitionTrailsBNewViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionTrailsBNewDetail> PagedCTest_TrailsBNewDetailList { get; set; }
        public CognitionTrailsBNewSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionTrailsBNewDetail> CTest_TrailsBNewResultList { get; set; }
        public CognitionTrailsBNewViewModel()
        {
            SortPageOptions = new CognitionTrailsBNewSortPageOptions();
            CTest_TrailsBNewResultList = new List<CognitionTrailsBNewDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetail
    /// </summary>
    public class CognitionTrailsBNewDetail
    {
        public long TrailsBResultID { get; set; }
        public int TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? Version { get; set; }
        public byte? Status { get; set; }
        public List<CognitionTrailsBNewResultDetail> CognitionTrailsBNewResultDetail { get; set; }
        public CognitionTrailsBNewDetail()
        {
            CognitionTrailsBNewResultDetail = new List<CognitionTrailsBNewResultDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetailListArray
    /// </summary>
    public class CognitionTrailsBNewResultDetail
    {
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    /// <summary>
    /// Class CognitionTrailsBSortPageOptions
    /// </summary>
    public class CognitionTrailsBNewSortPageOptions : PagingBase
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

