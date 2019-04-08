using PagedList;
using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Cognition  Cat And Dog New View Model
    /// </summary>
    public class CognitionCatAndDogNewViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public StaticPagedList<CatAndDogNewDetail> CatAndDogNewGamePagedList { get; set; }
        public CatAndDogNewDetailSortPageOptions SortPageOptions { get; set; }
        public List<CatAndDogNewDetail> CatAndDogNewGameList { get; set; }
        public CognitionCatAndDogNewViewModel()
        {
            SortPageOptions = new CatAndDogNewDetailSortPageOptions();
            CatAndDogNewGameList = new List<CatAndDogNewDetail>();
        }
    }

    /// <summary>
    /// Class CatAndDogNewDetail
    /// </summary>
    public class CatAndDogNewDetail
    {
        public long CatAndDogNewResultID { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public string Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte? Status { get; set; }
        //public List<CatAndDogNewGameLevelDetail> GameLevelDetailList { get; set; }
        public string jsonGameLevelDetails { get; set; }
    }
    /// <summary>
    /// Class CatAndDogNewDetailSortPageOptions
    /// </summary>
    public class CatAndDogNewDetailSortPageOptions : PagingBase
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


    public class CatAndDogNewGameLevelDetail
    {
        public Int64 CatAndDogNewResultDtlID { get; set; }
        public Int64 CatAndDogNewResultID { get; set; }
        public int CorrectAnswer { get; set; }
        public int WrongAnswer { get; set; }
        public string TimeTaken { get; set; }
    }
}



