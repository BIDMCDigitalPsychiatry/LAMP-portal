using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognitionCatDogViewModel
    /// </summary>
    public class CognitionCatDogViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public StaticPagedList<CognitionCatDogDetail> PagedCTest_CatDogDetailList { get; set; }
        public CognitionCatDogSortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }

        public List<CognitionCatDogDetail> CTest_CatDogResultList { get; set; }
        public CognitionCatDogViewModel()
        {
            SortPageOptions = new CognitionCatDogSortPageOptions();
            CTest_CatDogResultList = new List<CognitionCatDogDetail>();
        }      
    }
    /// <summary>
    /// Class CognitionCatDogDetail
    /// </summary>
    public class CognitionCatDogDetail
    {
        public long CatAndDogResultID { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte? Status { get; set; }
    }
    /// <summary>
    /// Class CognitionCatDogSortPageOptions
    /// </summary>
    public class CognitionCatDogSortPageOptions : PagingBase
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
