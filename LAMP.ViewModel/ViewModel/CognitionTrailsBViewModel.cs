using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionTrailsBViewModel
    /// </summary>
    public class CognitionTrailsBViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionTrailsBDetail> PagedCTest_TrailsBDetailList { get; set; }
        public CognitionTrailsBSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionTrailsBDetail> CTest_TrailsBResultList { get; set; }
        public CognitionTrailsBViewModel()
        {
            SortPageOptions = new CognitionTrailsBSortPageOptions();
            CTest_TrailsBResultList = new List<CognitionTrailsBDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetail
    /// </summary>
    public class CognitionTrailsBDetail
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
        public List<CognitionTrailsBResultDetail> CognitionTrailsBResultDetail { get; set; }
        public CognitionTrailsBDetail()
        {
            CognitionTrailsBResultDetail = new List<CognitionTrailsBResultDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetailListArray
    /// </summary>
    public class CognitionTrailsBResultDetail
    {
        public string Alphabet { get; set; }
        //public int? TimeTaken { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    /// <summary>
    /// Class CognitionTrailsBSortPageOptions
    /// </summary>
    public class CognitionTrailsBSortPageOptions : PagingBase
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
