using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Schedule View Model
    /// </summary>
    public class ScheduleViewModel : ViewModelBase
    {
        public long ScheduleID { get; set; }
        public long UserSettingID { get; set; }
        public long LoggedInUserId { get; set; }
        public long UserId { get; set; }
        public long? SurveySlotId { get; set; }
        public long? SurveyRepeatId { get; set; }
        public TimeSpan? SurveySlotTime { get; set; }
        public DateTime? SurveySlotTimeDatetime { get; set; }
        public string SurveySlotTimeString { get; set; }
        public long SurveyId { get; set; }
        public List<SelectListItem> SlotList { get; set; }
        public List<SelectListItem> RepeatList { get; set; }
        public List<SelectListItem> SurveyList { get; set; }

        public long? CognitionTestSlotId { get; set; }
        public long? CognitionTestRepeatId { get; set; }
        public TimeSpan? CognitionTestSlotTime { get; set; }
        public DateTime? CognitionTestTimeDatetime { get; set; }
        public string CognitionTestSlotTimeString { get; set; }
        public long CognitionTesttId { get; set; }
        public List<SelectListItem> CognitionTestList { get; set; }

        public long[] CTesTArray { get; set; }
        public string CTesTArrayString { get; set; }
        public long[] SurveyArray { get; set; }
        public string SurveyArrayString { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsSaved { get; set; }
        public UserSettingsViewModel UserSettingsViewModel { get; set; }

        public ScheduleViewModel()
        {
            SlotList = new List<SelectListItem>();
            RepeatList = new List<SelectListItem>();
            CognitionTestList = new List<SelectListItem>();
            SurveyList = new List<SelectListItem>();
            UserSettingsViewModel = new UserSettingsViewModel();
        }
    }
    public class UserSettingsViewModel
    {
        public long UserSettingID { get; set; }
        public long UserID { get; set; }
        public string AppColor { get; set; }
        public long? SympSurvey_SlotID { get; set; }
        public DateTime? SympSurvey_Time { get; set; }
        public long? SympSurvey_RepeatID { get; set; }
        public long? CognTest_SlotID { get; set; }
        public DateTime? CognTest_Time { get; set; }
        public long? CognTest_RepeatID { get; set; }
        public string C24By7ContactNo { get; set; }
        public string PersonalHelpline { get; set; }
        public string PrefferedSurveys { get; set; }
        public string PrefferedCognitions { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? Protocol { get; set; }
        public DateTime? BlogsViewedOn { get; set; }
        public DateTime? TipsViewedOn { get; set; }

    }
    //new schedule viewmodel classes.
    public class ScheduleGameSurveyViewModel : ViewModelBase
    {
        public long ScheduleID { get; set; }
        public long LoggedInUserId { get; set; }
        public long AdminId { get; set; }
        public long AdminCTestSchID { get; set; }
        public long AdminSurveySchID { get; set; }
        public long AdminBatchSchID { get; set; }
        public string Options { get; set; }
        public string OptionsArray { get; set; }
        public List<string> OptionsStringList { get; set; }

        public long? SurveySlotId { get; set; }
        public long? SurveyRepeatId { get; set; }
        public TimeSpan? SurveySlotTime { get; set; }
        public DateTime? SurveySlotTimeDatetime { get; set; }
        public string SurveySlotTimeString { get; set; }
        public long SurveyId { get; set; }
        public string SurveyScheduleDateValue { get; set; }
        public string SurveyScheduleDateString { get; set; }

        public long? CognitionTestSlotId { get; set; }
        public long? CognitionTestRepeatId { get; set; }
        public TimeSpan? CognitionTestSlotTime { get; set; }
        public DateTime? CognitionTestTimeDatetime { get; set; }
        public string CognitionTestSlotTimeString { get; set; }
        public long CognitionTestId { get; set; }
        public long CognitionVersionId { get; set; }
        public string GameScheduleDateString { get; set; }
        public string GameScheduleDateValue { get; set; }
        public List<SelectListItem> SlotList { get; set; }
        public List<SelectListItem> RepeatList { get; set; }
        public List<SelectListItem> SurveyList { get; set; }
        public List<SelectListItem> CognitionTestList { get; set; }
        public List<SelectListItem> CognitionVersionList { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsSaved { get; set; }

        public ScheduleGameListViewModel ScheduleGameListViewModel { get; set; }
        public ScheduleSurveyListViewModel ScheduleSurveyListViewModel { get; set; }
        public ScheduleListViewModel ScheduleBatchListViewModel { get; set; }
        public List<BatchScheduleCustomTimeViewModel> BatchScheduleCustomTimeViewModel { get; set; }
        public string CognitionTestId_Vers { get; set; }
        public ScheduleGameSurveyViewModel()
        {
            ScheduleGameListViewModel = new ScheduleGameListViewModel();
            ScheduleSurveyListViewModel = new ScheduleSurveyListViewModel();
            ScheduleBatchListViewModel = new ScheduleListViewModel();
            SlotList = new List<SelectListItem>();
            RepeatList = new List<SelectListItem>();
            CognitionTestList = new List<SelectListItem>();
            SurveyList = new List<SelectListItem>();
            CognitionVersionList = new List<SelectListItem>();
            BatchScheduleCustomTimeViewModel = new List<BatchScheduleCustomTimeViewModel>();
        }

        [Required(ErrorMessage = "Specify batch name.")]
        [RegularExpression(@"[^<>]*", ErrorMessage = "Invalid batch name.")]
        public string BatchName { get; set; }
        public string BatchScheduleDateString { get; set; }
        public string BatchScheduleDateValue { get; set; }
        public long? BatchSlotId { get; set; }
        public long? BatchRepeatId { get; set; }
        public TimeSpan? BatchSlotTime { get; set; }
        public DateTime? BatchSlotTimeDatetime { get; set; }
        public string BatchSlotTimeString { get; set; }
        public List<BatchSurvey> BatchSurvey { get; set; }
        public List<BatchCognitionTest> CognitionTest { get; set; }
        public string BatchSurveyGames { get; set; }
    }

    public class BatchScheduleCTestViewModel
    {
        public long AdminBatchSchID { get; set; }
        public long CognitionTestId { get; set; }
        public long CognitionVersionId { get; set; }
        public string  strCognitionTestId { get; set; }
    }

    public class BatchScheduleSurveyViewModel
    {
        public long AdminBatchSchID { get; set; }
        public long SurveyId { get; set; }
    }

    public class BatchScheduleCustomTimeViewModel
    {
        public long AdminBatchSchID { get; set; }
        public TimeSpan? CustomTime { get; set; }
        public DateTime? CustomDatetime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScheduleBatchViewModel : ViewModelBase
    {
        public long ScheduleID { get; set; }
        public long LoggedInUserId { get; set; }
        public long AdminId { get; set; }
        public long AdminCTestSchID { get; set; }
        public long AdminSurveySchID { get; set; }
        public long AdminBatchSchID { get; set; }
        public string Options { get; set; }
        public string OptionsArray { get; set; }
        public List<string> OptionsStringList { get; set; }

        public long? SurveySlotId { get; set; }
        public long? SurveyRepeatId { get; set; }
        public TimeSpan? SurveySlotTime { get; set; }
        public DateTime? SurveySlotTimeDatetime { get; set; }
        public string SurveySlotTimeString { get; set; }
        public List<BatchSurvey> BatchSurvey { get; set; }
        public List<BatchCognitionTest> CognitionTest { get; set; }

        public string SurveyScheduleDateValue { get; set; }
        public string SurveyScheduleDateString { get; set; }

        public long? CognitionTestSlotId { get; set; }
        public long? CognitionTestRepeatId { get; set; }
        public TimeSpan? CognitionTestSlotTime { get; set; }
        public DateTime? CognitionTestTimeDatetime { get; set; }
        public string CognitionTestSlotTimeString { get; set; }


        public string GameScheduleDateString { get; set; }
        public string GameScheduleDateValue { get; set; }
        public List<SelectListItem> SlotList { get; set; }
        public List<SelectListItem> RepeatList { get; set; }
        public List<SelectListItem> SurveyList { get; set; }
        public List<SelectListItem> CognitionTestList { get; set; }
        public List<SelectListItem> CognitionVersionList { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsSaved { get; set; }

        public ScheduleGameListViewModel ScheduleGameListViewModel { get; set; }
        public ScheduleSurveyListViewModel ScheduleSurveyListViewModel { get; set; }
        public ScheduleListViewModel ScheduleBatchListViewModel { get; set; }
        public ScheduleBatchViewModel()
        {
            ScheduleGameListViewModel = new ScheduleGameListViewModel();
            ScheduleSurveyListViewModel = new ScheduleSurveyListViewModel();
            ScheduleBatchListViewModel = new ScheduleListViewModel();
            SlotList = new List<SelectListItem>();
            RepeatList = new List<SelectListItem>();
            CognitionTestList = new List<SelectListItem>();
            SurveyList = new List<SelectListItem>();
            CognitionVersionList = new List<SelectListItem>();
        }
        public string BatchName { get; set; }
        public string BatchSlotTimeString { get; set; }
    }
    /// <summary>
    /// Class GameListViewModel
    /// </summary>
    public class ScheduleGameListViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public string SearchId { get; set; }
        public List<AdminCTestScheduleViewModel> AdminCTestScheduleViewModelList { get; set; }
        public StaticPagedList<AdminCTestScheduleViewModel> PagedAdminCTestScheduleViewModelList { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public string UnregisteredUserMessage { get; set; }

        public ScheduleGameListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            AdminCTestScheduleViewModelList = new List<AdminCTestScheduleViewModel>();
        }
    }

    /// <summary>
    /// Class GameListViewModel
    /// </summary>
    public class ScheduleSurveyListViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public string SearchId { get; set; }
        public List<AdminSurveyScheduleViewModel> AdminSurveyScheduleViewModelList { get; set; }
        public StaticPagedList<AdminSurveyScheduleViewModel> PagedAdminSurveyScheduleViewModel { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public string UnregisteredUserMessage { get; set; }

        public ScheduleSurveyListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            AdminSurveyScheduleViewModelList = new List<AdminSurveyScheduleViewModel>();
        }
    }


    /// <summary>
    /// Model class for admin batch creation
    /// </summary>
    public class ScheduleListViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public string SearchId { get; set; }
        public List<AdminScheduleViewModel> AdminScheduleViewModelList { get; set; }
        public StaticPagedList<AdminScheduleViewModel> PagedAdminScheduleViewModelList { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public string UnregisteredUserMessage { get; set; }

        public ScheduleListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            AdminScheduleViewModelList = new List<AdminScheduleViewModel>();
        }
    }

    public class AdminCTestScheduleViewModel
    {
        public long AdminCTestSchID { get; set; }
        public long? AdminID { get; set; }
        public long? CTestID { get; set; }
        public string CTestName { get; set; }
        public int? Version { get; set; }
        public long? SlotID { get; set; }
        public string SlotName { get; set; }
        public DateTime? Time { get; set; }
        public long? RepeatID { get; set; }
        public string RepeatInterval { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime? EditedOn { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string SlotTimeStamp { get; set; }
        public List<string> SlotTimeOptions { get; set; }
    }
    public partial class AdminSurveyScheduleViewModel
    {
        public long AdminSurveySchID { get; set; }
        public long? AdminID { get; set; }
        public long? SurveyID { get; set; }
        public string SurveyName { get; set; }
        public long? SlotID { get; set; }
        public string SlotName { get; set; }
        public DateTime? Time { get; set; }
        public long? RepeatID { get; set; }
        public string RepeatInterval { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime? EditedOn { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string SlotTimeStamp { get; set; }
        public List<string> SlotTimeOptions { get; set; }

    }
    public class AdminScheduleViewModel
    {
        public long AdminSchID { get; set; }
        public long? AdminID { get; set; }
        public long? ID { get; set; }
        public string Name { get; set; }
        public int? Version { get; set; }
        public long? SlotID { get; set; }
        public string SlotName { get; set; }
        public DateTime? Time { get; set; }
        public long? RepeatID { get; set; }
        public string RepeatInterval { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime? EditedOn { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string SlotTimeStamp { get; set; }
        public List<string> SlotTimeOptions { get; set; }
        public string BatchName { get; set; }
    }
    public class BatchCognitionTest
    {
        public long CognitionTestId { get; set; }
        public long CognitionVersionId { get; set; }
        public Int16 Order { get; set; }
    }

    public class BatchSurvey
    {
        public long SurveyId { get; set; }
        public Int16 Order { get; set; }
    }

}
