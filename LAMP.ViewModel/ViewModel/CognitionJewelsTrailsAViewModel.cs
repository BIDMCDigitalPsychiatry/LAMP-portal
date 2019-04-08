using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionJewelsTrailsAViewModel
    /// </summary>
    public class CognitionJewelsTrailsAViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public StaticPagedList<CognitionJewelsTrailsADetail> PagedCTest_CognitionJewelsTrailsADetailList { get; set; }
        public CognitionJewelsTrailsASortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionJewelsTrailsADetail> CTest_JewelsTrailsAResultList { get; set; }
        public CognitionJewelsTrailsAViewModel()
        {
            SortPageOptions = new CognitionJewelsTrailsASortPageOptions();
            CTest_JewelsTrailsAResultList = new List<CognitionJewelsTrailsADetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetail
    /// </summary>
    public class CognitionJewelsTrailsADetail
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
        public List<CognitionJewelsTrailsAResultDetail> CognitionJewelsTrailsAResultDetail { get; set; }
        public CognitionJewelsTrailsADetail()
        {
            CognitionJewelsTrailsAResultDetail = new List<CognitionJewelsTrailsAResultDetail>();
        }
    }
    /// <summary>
    /// Class CognitionTrailsBDetailListArray
    /// </summary>
    public class CognitionJewelsTrailsAResultDetail
    {
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Sequence { get; set; }
    }
    /// <summary>
    /// Class CognitionTrailsBSortPageOptions
    /// </summary>
    public class CognitionJewelsTrailsASortPageOptions : PagingBase
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


