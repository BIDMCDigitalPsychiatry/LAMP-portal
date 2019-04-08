using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionTrailsBDotTouchViewModel
    /// </summary>
    public class CognitionTrailsBDotTouchViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionTrailsBDotTouchDetail> PagedCTest_TrailsBDotTouchDetailList { get; set; }
        public CognitionTrailsBDotTouchSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionTrailsBDotTouchDetail> CTest_TrailsBDotTouchResultList { get; set; }
        public CognitionTrailsBDotTouchViewModel()
        {
            SortPageOptions = new CognitionTrailsBDotTouchSortPageOptions();
            CTest_TrailsBDotTouchResultList = new List<CognitionTrailsBDotTouchDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetail
    /// </summary>
    public class CognitionTrailsBDotTouchDetail
    {
        public long TrailsBResultID { get; set; }
        public int TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte? Status { get; set; }
        public List<CognitionTrailsBDotTouchResultDetail> CognitionTrailsBDotTouchResultDetail { get; set; }
        public CognitionTrailsBDotTouchDetail()
        {
            CognitionTrailsBDotTouchResultDetail = new List<CognitionTrailsBDotTouchResultDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetailListArray
    /// </summary>
    public class CognitionTrailsBDotTouchResultDetail
    {
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    /// <summary>
    /// Class CognitionTrailsBSortPageOptions
    /// </summary>
    public class CognitionTrailsBDotTouchSortPageOptions : PagingBase
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

