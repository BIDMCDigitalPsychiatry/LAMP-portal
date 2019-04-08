using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionSpatialSpanViewModel
    /// </summary>
    public class CognitionSpatialSpanViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionSpatialSpanDetail> PagedCTest_SpatialSpanDetailList { get; set; }
        public CognitionSpatialSpanSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionSpatialSpanDetail> CTest_SpatialSpanResultList { get; set; }
        public CognitionSpatialSpanViewModel()
        {
            SortPageOptions = new CognitionSpatialSpanSortPageOptions();
            CTest_SpatialSpanResultList = new List<CognitionSpatialSpanDetail>();
        }
      
    }
    /// <summary>
    /// Class CognitionSpatialSpanDetail
    /// </summary>
    public class CognitionSpatialSpanDetail
    {
        public long SpatialResultID { get; set; }
        public byte Type { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte? Status { get; set; }
          public List<CognitionSpatialSpanResultDetail> CognitionSpatialSpanResultDetail { get; set; }
          public CognitionSpatialSpanDetail()
        {
            CognitionSpatialSpanResultDetail = new List<CognitionSpatialSpanResultDetail>();
        }
    }

    public class CognitionSpatialSpanResultDetail
    {
        public byte? GameIndex { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Level { get; set; }
        public int? Sequence { get; set; }
    }
    /// <summary>
    /// Class CognitionSpatialSpanSortPageOptions
    /// </summary>
    public class CognitionSpatialSpanSortPageOptions : PagingBase
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
