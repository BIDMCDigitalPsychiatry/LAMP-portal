using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Class AdminService
    /// </summary>
    public class AdminService : IAdminService
    {
        #region Variables
        private IUnitOfWork _UnitOfWork;
        private UrlHelper _urlHelp;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="UnitOfWork">UnitOfWork</param>
        public AdminService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
            _urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        #endregion

        #region Methods

        /// <summary>
        /// checks if the user is valid
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public LoginResponse AuthenticateUser(LoginViewModel loginViewModel, string returnUrl)
        {
            LogUtil.Info("AuthenticateUser...");
            LoginResponse response = new LoginResponse();
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _urlHelp.Action("Index", "Account");
            }
            try
            {
                if (string.IsNullOrEmpty(loginViewModel.Email))
                {
                    response.Errors.Add(new LAMPError("Email", ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_EMAIL_ADDRESS)));
                }
                else
                {
                    //Validation for email format
                    string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                    Regex re = new Regex(emailRegex);
                    if (!re.IsMatch(loginViewModel.Email))
                    {
                        response.Errors.Add(new LAMPError("Email", ResourceHelper.GetStringResource(LAMPConstants.MSG_INVALID_EMAIL)));
                    }
                }
                if (string.IsNullOrEmpty(loginViewModel.Password))
                {
                    response.Errors.Add(new LAMPError("Password", ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_PASSWORD)));
                }
                if (response.Errors.Count == 0)
                {
                    string encriptedEmail = CryptoUtil.EncryptInfo(loginViewModel.Email.Trim());
                    Admin user = _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == encriptedEmail && u.IsDeleted == false).FirstOrDefault();
                    if (user != null && user.AdminID > 0)
                    {
                        if (CryptoUtil.DecryptStringWithKey(user.Password).Equals(loginViewModel.Password))
                        {
                            response.AdminID = user.AdminID;
                            returnUrl = _urlHelp.Action("Users", "UserAdmin");
                        }
                        else
                        {
                            response.Errors.Add(new LAMPError("CustomError", ResourceHelper.GetStringResource(LAMPConstants.MSG_INVALID_CREDENTIALS)));
                        }
                    }
                    else
                    {
                        response.Errors.Add(new LAMPError("CustomError", ResourceHelper.GetStringResource(LAMPConstants.MSG_INVALID_CREDENTIALS)));
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
            }
            response.ReturnUrl = returnUrl;
            return response;
        }

        /// <summary>
        /// Send mail to admin on forget password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resetPasswordUrl"></param>
        /// <returns></returns>
        public LoginResponse ForgotPasswordEmailSendAction(Admin user, string resetPasswordUrl)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                if (user != null && user.AdminID > 0)
                {
                    EmailData Edata = new EmailData();
                    Edata.Email = CryptoUtil.DecryptInfo(user.Email);
                    Edata.Subject = "LAMP - Your Password";
                    Edata.TemplateName = LAMPConstants.HTML_RESOURCE_ADMINFORGOTPASSWORD;
                    var lampHost = ConfigurationManager.AppSettings["lampHost"].ToString();
                    LogUtil.Error("forgot path: " + lampHost + LAMPConstants.LOGO_PATH);
                    Edata.Data.Add(new replaceingData { Name = "LAMP_LOGO", Value = lampHost + LAMPConstants.LOGO_PATH });
                    Edata.Data.Add(new replaceingData { Name = "USER_NAME", Value = CryptoUtil.DecryptInfo(user.FirstName) });
                    Edata.Data.Add(new replaceingData { Name = "RESETURL", Value = resetPasswordUrl });
                    Helper.SendEmail(Edata);
                    response.SuccessMessage = ResourceHelper.GetStringResource(LAMPConstants.MSG_RESET_PASSWORD_LINK_EMAIL_SEND);
                }
            }
            catch (Exception ex)
            {
                LAMPError error = new LAMPError("0", "An error occurred during the process. Please try again later.");
                System.Collections.Generic.List<LAMPError> listObj = new System.Collections.Generic.List<LAMPError>();
                listObj.Add(error);
                response.Errors = listObj;
                response.ErrorMessage = "An error occurred during the process. Please try again later.";
                LogUtil.Error(ex);
            }
            return response;
        }

        /// <summary>
        /// Resets the password
        /// </summary>
        /// <param name="resetPasswordViewModel"></param>
        /// <returns></returns>
        public LoginResponse ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {

            LoginResponse response = new LoginResponse();
            if (string.IsNullOrEmpty(resetPasswordViewModel.Password))
            {
                //Required validation for password
                response.Errors.Add(new LAMPError("Password", ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_PASSWORD)));
            }
            if (string.IsNullOrEmpty(resetPasswordViewModel.ConfirmPassword) || resetPasswordViewModel.ConfirmPassword != resetPasswordViewModel.Password)
            {
                //Check if confirm password and new password match
                response.Errors.Add(new LAMPError("Password", ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_SAME_PASSWORDS)));
            }
            if (response.Errors.Count == 0)
            {
                var userid = Convert.ToInt32(CryptoUtil.DecryptStringWithKey(resetPasswordViewModel.AdminID));
                //Reset Password
                Admin admin = new Admin();
                admin = _UnitOfWork.IAdminRepository.GetById(userid);
                admin.Password = CryptoUtil.EncryptStringWithKey(resetPasswordViewModel.Password.Trim());
                _UnitOfWork.IAdminRepository.Update(admin);
                _UnitOfWork.Commit();
                response.ReturnUrl = _urlHelp.Action("Index", "Account");
                response.SuccessMessage = ResourceHelper.GetStringResource(LAMPConstants.MSG_PASSWORD_CHANGED_SUCCESSFULLY);
            }
            return response;
        }

        #endregion

    }
}
