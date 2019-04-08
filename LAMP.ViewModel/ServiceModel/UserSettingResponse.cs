using System;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserSettingResponse
    /// </summary>
    public class UserSettingResponse : APIResponseBase
    {
        public UserSettingData Data { get; set; }
        public UserSettingResponse()
        {
            Data = new UserSettingData();
        }
    }

    /// <summary>
    /// Class UserSettingData
    /// </summary>
    public class UserSettingData
    {
        public long UserSettingID { get; set; }
        public long UserID { get; set; }
        public string AppColor { get; set; }
        public long SympSurveySlotID { get; set; }
        public DateTime? SympSurveySlotTime { get; set; }
        public long SympSurveyRepeatID { get; set; }
        public long CognTestSlotID { get; set; }
        public DateTime? CognTestSlotTime { get; set; }
        public long CognTestRepeatID { get; set; }
        public string ContactNo { get; set; }
        public string PersonalHelpline { get; set; }
        public string PrefferedSurveys { get; set; }
        public string PrefferedCognitions { get; set; }
        public bool? Protocol { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public string Language { get; set; }
    }
}
