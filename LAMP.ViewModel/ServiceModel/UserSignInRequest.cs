namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserSignInRequest
    /// </summary>
    public class UserSignInRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }        
        public string APPVersion { get; set; }
        public byte DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceToken { get; set; }
        public string Language { get; set; }
    }
}
