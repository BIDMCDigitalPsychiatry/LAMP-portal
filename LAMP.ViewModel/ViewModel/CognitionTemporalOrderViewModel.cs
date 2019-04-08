using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Cognition Temporal Order View Model
    /// </summary>
    public class CognitionTemporalOrderViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public StaticPagedList<TemporalOrderDetail> TemporalOrderGamePagedList { get; set; }
        public TemporalOrderDetailSortPageOptions SortPageOptions { get; set; }
        public List<TemporalOrderDetail> TemporalOrderGameList { get; set; }
        public CognitionTemporalOrderViewModel()
        {
            SortPageOptions = new TemporalOrderDetailSortPageOptions();
            TemporalOrderGameList = new List<TemporalOrderDetail>();
        }
    }

    /// <summary>
    /// Class TemporalOrderDetail
    /// </summary>
    public class TemporalOrderDetail
    {
        public long TemporalOrderResultID { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
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
    /// Class TemporalOrderDetailSortPageOptions
    /// </summary>
    public class TemporalOrderDetailSortPageOptions : PagingBase
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
