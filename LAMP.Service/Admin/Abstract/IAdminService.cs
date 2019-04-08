using LAMP.DataAccess.Entities;
using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Interface IAdminService for capable of class AdminService
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// checks if the user is valid
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        LoginResponse AuthenticateUser(LoginViewModel loginViewModel, string returnUrl);

        /// <summary>
        /// Send mail to admin on forget password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resetPasswordUrl"></param>
        /// <returns></returns>
        LoginResponse ForgotPasswordEmailSendAction(Admin user, string resetPasswordUrl);        

        /// <summary>
        /// Resets the password
        /// </summary>
        /// <param name="resetPasswordViewModel"></param>
        /// <returns></returns>
        LoginResponse ResetPassword(ResetPasswordViewModel resetPasswordViewModel);
    }
}
