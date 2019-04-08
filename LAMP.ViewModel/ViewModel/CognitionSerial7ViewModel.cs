using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionSerial7ViewModel
    /// </summary>
    public class CognitionSerial7ViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionSerial7Detail> PagedCTest_Serial7DetailList { get; set; }
        public CognitionSerial7SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public List<CognitionSerial7Detail> CTest_Serial7ResultList { get; set; }
        public CognitionSerial7ViewModel()
        {
            SortPageOptions = new CognitionSerial7SortPageOptions();
            CTest_Serial7ResultList = new List<CognitionSerial7Detail>();
        }      
    }
    /// <summary>
    /// Class CognitionSerial7Detail
    /// </summary>
    public class CognitionSerial7Detail
    {
        public long Serial7ResultID { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttempts { get; set; }
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
    /// Class CognitionSerial7SortPageOptions
    /// </summary>
    public class CognitionSerial7SortPageOptions : PagingBase
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
