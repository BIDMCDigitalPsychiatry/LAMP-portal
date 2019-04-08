namespace LAMP.ViewModel
{
    /// <summary>
    /// Class GuestUserSignUpRequest
    /// </summary>
    public class GuestUserSignUpRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string APPVersion { get; set; }
        public byte DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceToken { get; set; }
        public string Language { get; set; }
    }
}
