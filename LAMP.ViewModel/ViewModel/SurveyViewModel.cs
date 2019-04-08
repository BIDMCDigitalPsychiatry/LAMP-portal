
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace LAMP.ViewModel
{
    /// <summary>
    /// Survey View Model
    /// </summary>
    public class SurveyViewModel : ViewModelBase
    {
        public long SurveyID { get; set; }
        public string SurveyName { get; set; }
        public Nullable<long> AdminID { get; set; }
        public long QuestionID { get; set; }
        public string QuestionText { get; set; }
        public Nullable<byte> AnswerType { get; set; }
        public string Options { get; set; }
        public List<SurveyQuestionViewModel> Questions { get; set; }
        public SurveyQuestionTypes QuestionTypes { get; set; }
        public bool IsSaved { get; set; }
        public bool IsEdit { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        public string LanguageCode { get; set; }
        public SurveyViewModel()
        {
            LanguageList = new List<SelectListItem>(){
                 new SelectListItem { Text = "English", Value = "en" },
                 new SelectListItem { Text = "Spanish", Value = "es" },
                new SelectListItem { Text = "Potuguese", Value = "pt-br" },
                 new SelectListItem { Text = "Chinese", Value = "cmn" }
            };
        }
    }

    public class SurveyListViewModel : ViewModelBase
    {
        public long UserId { get; set; }
        public short NoOfPages { get; set; }
        public StaticPagedList<SurveyForList> PagedSurveyList { get; set; }
        public string SearchText { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public string ErrorMessage { get; set; }
        public List<SurveyForList> SurveyList { get; set; }
        public SurveyListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            SurveyList = new List<SurveyForList>();

        }
    }

    public class SurveyQuestionViewModel
    {
        public long QuestionID { get; set; }
        public long SurveyID { get; set; }
        public string QuestionText { get; set; }
        public Nullable<byte> AnswerType { get; set; }
        public List<SurveyQuestionOptionsViewModel> Options { get; set; }
    }

    public class SurveyQuestionOptionsViewModel
    {
        public long OptionID { get; set; }
        public long QuestionID { get; set; }
        public string OptionText { get; set; }
    }

    public class SurveyForList
    {
        public long SurveyID { get; set; }
        public string SurveyName { get; set; }
        public string Language { get; set; }
        public long AdminId { get; set; }
        public string CreatedAdmin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminlastName { get; set; }
    }
    public enum SurveyQuestionTypes
    {
        [Display(Name = "Likert Response")]
        LikertResponse = 1,
        [Display(Name = "Scroll Wheels")]
        ScrollWheels = 2,
        [Display(Name = "Yes or No")]
        YesNO = 3,
        [Display(Name = "Clock")]
        Clock = 4,
        [Display(Name = "Years")]
        Years = 5,
        [Display(Name = "Months")]
        Months = 6,
        [Display(Name = "Days")]
        Days = 7
    }
}
