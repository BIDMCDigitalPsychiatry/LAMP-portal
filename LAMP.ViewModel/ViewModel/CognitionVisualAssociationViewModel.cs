using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionVisualAssociationViewModel
    /// </summary>
    public class CognitionVisualAssociationViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionVisualAssociationDetail> PagedCTest_VisualAssociationDetailList { get; set; }
        public CognitionVisualAssociationSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionVisualAssociationDetail> CTest_VisualAssociationResultList { get; set; }
        public CognitionVisualAssociationViewModel()
        {
            SortPageOptions = new CognitionVisualAssociationSortPageOptions();
            CTest_VisualAssociationResultList = new List<CognitionVisualAssociationDetail>();
        }      
    }
    /// <summary>
    /// Class CognitionVisualAssociationDetail
    /// </summary>
    public class CognitionVisualAssociationDetail
    {
        public long VisualAssocResultID { get; set; }
        public int TotalQuestions { get; set; }
        public Int32 TotalAttempts { get; set; }
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
    /// Class CognitionVisualAssociationSortPageOptions
    /// </summary>
    public class CognitionVisualAssociationSortPageOptions : PagingBase
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
