using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    public class CognitionSpinWheelViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public List<CognitionSpinWheelDetail> CognitionSpinWheelDetailList { get; set; }
        public StaticPagedList<CognitionSpinWheelDetail> PagedCTest_SpinWheelDetailList { get; set; }
        public CognitionSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionSpinWheelDetail> CTest_SpinWheelResultList { get; set; }
        public CognitionSpinWheelViewModel()
        {
            SortPageOptions = new CognitionSortPageOptions();
            CTest_SpinWheelResultList = new List<CognitionSpinWheelDetail>();
        }
    }

    /// <summary>
    /// Class CognitionNBackDetail
    /// </summary>
    public class CognitionSpinWheelDetail
    {
        //SpinWheelResultID
        public DateTime StartTime { get; set; }
        public String CollectedStars { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    /// <summary>
    /// Class CognitionNBackSortPageOptions
    /// </summary>
    public class CognitionSortPageOptions : PagingBase
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
