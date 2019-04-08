using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionDigitSpanViewModel
    /// </summary>
    public class CognitionDigitSpanViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionDigitSpanDetail> PagedCTest_DigitSpanDetailList { get; set; }
        public CognitionDigitSpanSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionDigitSpanDetail> CTest_DigitSpanResultList { get; set; }
        public CognitionDigitSpanViewModel()
        {
            SortPageOptions = new CognitionDigitSpanSortPageOptions();
            CTest_DigitSpanResultList = new List<CognitionDigitSpanDetail>();
        }
      
    }
    /// <summary>
    /// Class CognitionDigitSpanDetail
    /// </summary>
    public class CognitionDigitSpanDetail
    {
        public long DigitSpanResultID { get; set; }
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

    }
    /// <summary>
    /// Class CognitionDigitSpanSortPageOptions
    /// </summary>
    public class CognitionDigitSpanSortPageOptions : PagingBase
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
