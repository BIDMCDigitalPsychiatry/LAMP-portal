namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserSignUpRequest
    /// </summary>
    public class UserSignUpRequest
    {
        public string StudyCode { get; set; }
        public string StudyId { get; set; }
        public string Password { get; set; }
        public string APPVersion { get; set; }
        public byte DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceToken { get; set; }
        public string Language { get; set; }
    }
}
