using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionJewelsTrailsBViewModel
    /// </summary>
    public class CognitionJewelsTrailsBViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public StaticPagedList<CognitionJewelsTrailsBDetail> PagedCTest_CognitionJewelsTrailsBDetailList { get; set; }
        public CognitionJewelsTrailsBSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionJewelsTrailsBDetail> CTest_JewelsTrailsBResultList { get; set; }
        public CognitionJewelsTrailsBViewModel()
        {
            SortPageOptions = new CognitionJewelsTrailsBSortPageOptions();
            CTest_JewelsTrailsBResultList = new List<CognitionJewelsTrailsBDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetail
    /// </summary>
    public class CognitionJewelsTrailsBDetail
    {
        public long TrailsBResultID { get; set; }
        public int TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public byte? Status { get; set; }
        public List<CognitionJewelsTrailsBResultDetail> CognitionJewelsTrailsBResultDetail { get; set; }
        public CognitionJewelsTrailsBDetail()
        {
            CognitionJewelsTrailsBResultDetail = new List<CognitionJewelsTrailsBResultDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetailListArray
    /// </summary>
    public class CognitionJewelsTrailsBResultDetail
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
    public class CognitionJewelsTrailsBSortPageOptions : PagingBase
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



