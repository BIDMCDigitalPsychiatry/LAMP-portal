namespace LAMP.ViewModel
{
    /// <summary>
    ///  Class AccountViewModel
    /// </summary>
    public class AccountViewModel
    {
    }
    /// <summary>
    ///  Class LoginResponse
    /// </summary>
    public class LoginResponse : ResponseBase
    {
        public string ReturnUrl;
        public long AdminID { get; set; }
    }

    /// <summary>
    ///  Class ResetPasswordViewModel
    /// </summary>
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string AdminID { get; set; }
        public string Code { get; set; }
    }
}
