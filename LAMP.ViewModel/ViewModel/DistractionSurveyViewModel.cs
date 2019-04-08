using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Distraction Survey View Model
    /// </summary>
    public class DistractionSurveyViewModel : ViewModelBase
    {
        public long LoggedInUserId { get; set; }
        public long SurveyId { get; set; }
        public List<CTestViewModel> CTestViewModelList { get; set; }
        public List<SelectListItem> SurveyList { get; set; }
        public List<Admin_CTestSurveySettingsViewModel> Admin_CTestSurveySettingsViewModelList { get; set; }
        public bool IsSaved { get; set; }
        public JewelsTrailsSettings JewelsTrailsSettings { get; set; }
        public AdminSettings AdminSettings { get; set; }
        public long ExpiryOptionId { get; set; }
        public List<SelectListItem> ExpiryOptions { get; set; }
        public DistractionSurveyViewModel()
        {
            Admin_CTestSurveySettingsViewModelList = new List<Admin_CTestSurveySettingsViewModel>();
            CTestViewModelList = new List<CTestViewModel>();
            SurveyList = new List<SelectListItem>();
            JewelsTrailsSettings = new JewelsTrailsSettings();
            AdminSettings = new AdminSettings();
            ExpiryOptions = new List<SelectListItem>(){
                 new SelectListItem { Text = "None", Value = "1" },
                 new SelectListItem { Text = "1 Hour", Value = "2" },         
                 new SelectListItem { Text = "6 Hours", Value = "3" }, 
            };

        }
    }
    public class JewelsTrailsSettings : ViewModelBase
    {
        public long AdminJTASettingID { get; set; }
        public long AdminJTBSettingID { get; set; }
        public long JewelsTrailsSettingsType { get; set; }
        public long? AdminID { get; set; }
        public int? NoOfSeconds_Beg { get; set; }
        public int? NoOfSeconds_Int { get; set; }
        public int? NoOfSeconds_Adv { get; set; }
        public int? NoOfSeconds_Exp { get; set; }
        [Range(0, 30, ErrorMessage = "0 to 30")]
        public int? NoOfDiamonds { get; set; }
        //[Range(2, 4)]
        public int? NoOfShapes { get; set; }
        public int? NoOfBonusPoints { get; set; }
        public int? X_NoOfChangesInLevel { get; set; }

        [Range(0, 10, ErrorMessage = "Enter number between 0 to 10")]
        public int? X_NoOfDiamonds { get; set; }
        public int? Y_NoOfChangesInLevel { get; set; }
        [Range(0, 2)]
        public int? Y_NoOfShapes { get; set; }
        public List<SelectListItem> JewelsTypeList { get; set; }
        public bool IsSaved { get; set; }
        public JewelsTrailsSettings()
        {
            JewelsTypeList = new List<SelectListItem>(){
                 new SelectListItem { Text = "JewelsTrialsA", Value = "1" },
                 new SelectListItem { Text = "JewelsTrialsB", Value = "2" },         
            };
        }


    }
    public class Admin_CTestSurveySettingsViewModel
    {
        public long AdminCTestSurveySettingID { get; set; }
        public long? AdminID { get; set; }
        public long? CTestID { get; set; }
        public long? SurveyID { get; set; }
    }
    public class AdminSettings : ViewModelBase
    {
        public long AdminSettingID { get; set; }
        public long? AdminID { get; set; }
        public long? ReminderClearInterval { get; set; }
        public bool IsSaved { get; set; }
    }
    public class CTestViewModel
    {
        public long CTestID { get; set; }
        public string CTestName { get; set; }
        public bool? IsDistractionSurveyRequired { get; set; }
        public string[] SurveyArray { get; set; }
    }
}
