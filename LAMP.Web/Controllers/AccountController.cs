using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace LAMP.Web.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        #region Fields
        private IAdminService _adminService;
        private ICustomAuthentication _customAuthentication;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        public AccountController()
        {
            // TODO:- We will remove this code when dependancy injection is complete.
            _UnitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="adminService">admin Service</param>
        /// <param name="unitOfWork">unitOfWork</param>
        /// <param name="customAuthentication">customAuthentication</param>
        public AccountController(IAdminService adminService, IUnitOfWork unitOfWork, ICustomAuthentication customAuthentication)
        {
            _adminService = adminService;
            _UnitOfWork = unitOfWork;
            _customAuthentication = customAuthentication;
        }
        #endregion

        #region  Methods
        /// <summary>
        /// start login page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null && Request.Cookies["Login"].Values["RememberMe"] == "true")
            {
                model.Email = Request.Cookies["Login"].Values[LAMPConstants.Cookie_Name];
                model.Password = CryptoUtil.DecryptStringWithKey(Request.Cookies["Login"].Values[LAMPConstants.Password_Cookie]);
                model.RememberMe = true;
            }
            return View("Index", model);
        }

        /// <summary>
        /// Helps this instance.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }


        /// <summary>
        /// shows login page with data if saved previously
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();

            if (Request.Cookies[LAMPConstants.Cookie_Name] != null)
            {
                if (Request.Cookies[LAMPConstants.Cookie_Name][LAMPConstants.UserName_Cookie] != null && Request.Cookies[LAMPConstants.Cookie_Name][LAMPConstants.Password_Cookie] != null)
                {
                    model.Email = Request.Cookies[LAMPConstants.Cookie_Name][LAMPConstants.UserName_Cookie];
                    string cookiePwd = Request.Cookies[LAMPConstants.Cookie_Name][LAMPConstants.Password_Cookie];
                    Admin adminUser = _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == model.Email && u.IsDeleted == false).FirstOrDefault();
                    if (null != adminUser && adminUser.AdminID > 0)
                    {
                        cookiePwd = CryptoUtil.DecryptStringWithKey(cookiePwd);
                        string passwordHash = cookiePwd;
                        if (adminUser.Password.Equals(passwordHash))
                        {
                            model.Password = cookiePwd;
                            model.RememberMe = true;
                            ModelState.Clear();
                            return View("Index", model);
                        }
                        else
                        {
                            ModelState.Clear();
                            model.Email = string.Empty;
                            model.Password = string.Empty;
                            model.RememberMe = false;
                            return View("Index", model);
                        }
                    }
                    else
                    {
                        ModelState.Clear();
                        model.RememberMe = false;
                        return View("Index", model);
                    }
                }
                else
                {
                    ModelState.Clear();
                    return View("Index", model);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("Index", model);
        }

        /// <summary>
        /// Process login data on the click of login button
        /// </summary>
        /// <param name="loginViewModel">loginViewModel</param>
        /// <param name="returnUrl">returnUrl</param>
        /// <returns>json data</returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel, string returnUrl)
        {
            LogUtil.Info("Login: " + loginViewModel.Email + " : " + loginViewModel.Password);
            Int16 offset = Session["OffsetValue"] == null ? (Int16)0 : Convert.ToInt16(Session["OffsetValue"].ToString());
            LoginResponse response = new LoginResponse();
            response = _adminService.AuthenticateUser(loginViewModel, returnUrl);
            LogUtil.Info("Login Error: " + response.ErrorCode + " : " + response.ErrorMessage + " : " + response.ReturnUrl);
            if (response.Errors.Count == 0)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, loginViewModel.Email));
                claims.Add(new Claim(ClaimTypes.Sid, response.AdminID.ToString()));
                SignIn(claims);
                _customAuthentication.Authenticate(loginViewModel);
                Session["OffsetValue"] = offset;
            }
            if (loginViewModel.RememberMe)
            {
                HttpCookie cookie = new HttpCookie("Login");
                cookie.Values.Add(LAMPConstants.Cookie_Name, loginViewModel.Email);
                cookie.Values.Add(LAMPConstants.Password_Cookie, CryptoUtil.EncryptStringWithKey(loginViewModel.Password));
                cookie.Values.Add("RememberMe", "true");
                Response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie cookie = new HttpCookie("Login");
                cookie.Values.Add("RememberMe", "false");
                Response.Cookies.Add(cookie);
            }
            ViewBag.ReturnUrl = response.ReturnUrl;
            ViewBag.response = response;
            return Json(response);
        }

        /// <summary>
        /// shows Reset Password page
        /// </summary>
        /// <param name="adminID">adminID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string adminID)
        {
            return View(new ResetPasswordViewModel() { AdminID = CryptoUtil.DecryptStringWithKey(adminID) });
        }

        /// <summary>
        /// Save the changed password on click of reset password button
        /// </summary>
        /// <param name="resetPasswordViewModel">resetPasswordViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            LoginResponse response = new LoginResponse();
            response = _adminService.ResetPassword(resetPasswordViewModel);
            return Json(response);
        }

        /// <summary>
        /// goes to change password page
        /// </summary>
        /// <param name="passwordUpdated">passwordUpdated</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePassword(int? passwordUpdated)
        {
            return View(new changePasswordViewModel() { UserID = loggedInUserId });
        }

        /// <summary>
        /// Save the changed password
        /// </summary>
        /// <param name="changePasswordViewModel">changePasswordViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult ChangePassword(changePasswordViewModel changePasswordViewModel)
        {
            LoginResponse response = new LoginResponse();
            // Getting user data from DB.
            Admin user = _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.AdminID == changePasswordViewModel.UserID).FirstOrDefault();

            // Validation of passwords.
            if (string.IsNullOrEmpty(changePasswordViewModel.OldPassword))
            {
                response.Errors.Add(new LAMPError(LAMPConstants.MSG_CURRENT_PASSWORD.ToString(), ResourceHelper.GetStringResource(LAMPConstants.MSG_CURRENT_PASSWORD)));
            }
            else if (user != null)
            {
                string currentPassword = CryptoUtil.DecryptStringWithKey(user.Password);
                if (!currentPassword.Equals(changePasswordViewModel.OldPassword.Trim()))
                {
                    response.Errors.Add(new LAMPError(LAMPConstants.MSG_WRONG_OLD_PASSWORD.ToString(), ResourceHelper.GetStringResource(LAMPConstants.MSG_WRONG_OLD_PASSWORD)));
                }
            }
            else if (user == null)
            {
                response.Errors.Add(new LAMPError(LAMPConstants.MSG_INVALID_USER.ToString(), ResourceHelper.GetStringResource(LAMPConstants.MSG_INVALID_USER)));
            }

            if (string.IsNullOrEmpty(changePasswordViewModel.NewPassword))
            {
                response.Errors.Add(new LAMPError(ResourceHelper.GetStringResource(LAMPConstants.MSG_NEW_PASSWORD), ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_NEW_PASSWORD)));
            }
            else if (changePasswordViewModel.OldPassword != null && changePasswordViewModel.OldPassword.CompareTo(changePasswordViewModel.NewPassword) == 0)
            {
                response.Errors.Add(new LAMPError(ResourceHelper.GetStringResource(LAMPConstants.MSG_NEW_PASSWORD), ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_DIFFERENT_PASSWORDS)));
            }

            if (string.IsNullOrEmpty(changePasswordViewModel.ConfirmPassword))
            {
                response.Errors.Add(new LAMPError(ResourceHelper.GetStringResource(LAMPConstants.MSG_CONFIRM_PASSWORD), ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_CONFIRM_PASSWORD)));
            }
            if (changePasswordViewModel.NewPassword != null && changePasswordViewModel.ConfirmPassword != null)
            {
                if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
                {
                    response.Errors.Add(new LAMPError(ResourceHelper.GetStringResource(LAMPConstants.MSG_CONFIRM_PASSWORD), ResourceHelper.GetStringResource(LAMPConstants.MSG_SPECIFY_SAME_PASSWORDS)));
                }
            }

            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
            resetPasswordViewModel.AdminID = user.AdminID.ToString();
            resetPasswordViewModel.Email = user.Email;
            resetPasswordViewModel.Password = changePasswordViewModel.NewPassword;
            resetPasswordViewModel.ConfirmPassword = changePasswordViewModel.ConfirmPassword;
            // Updating newly changed login password.
            if (response.Errors.Count == 0)
            {
                response = _adminService.ResetPassword(resetPasswordViewModel);
            }
            return Json(response);
        }

       
        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="loginViewModel">loginViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(LoginViewModel loginViewModel)
        {
            LoginResponse response = new LoginResponse();
            var email = CryptoUtil.EncryptInfo(loginViewModel.Email);
            var email1 = CryptoUtil.EncryptInfo("Super");
            var email2 = CryptoUtil.EncryptInfo("Admin");
            Admin user = _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == email).FirstOrDefault();
            if(user!=null)
            {
                var code = ResourceHelper.GenerateUniqueAlphaNumericCode(10);
                //Save user token
                var resetPasswordUrl = Url.Action("ResetPassword", "Account", new { AdminID = CryptoUtil.EncryptStringWithKey(user.AdminID.ToString()) }, protocol: Request.Url.Scheme);
                response = _adminService.ForgotPasswordEmailSendAction(user, resetPasswordUrl);
                if (response.Errors.Count == 0)
                {
                    loginViewModel.Status = 0;
                    loginViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_RESET_PASSWORD_LINK_EMAIL_SEND);
                }
                else
                {
                    loginViewModel.Status = 1;
                    loginViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.MAIL_CANNOT_SEND);
                }
            }
            else
            {
                loginViewModel.Status = 1;
                loginViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.ADMIN_NOT_REGISTERED);
            }
            return PartialView("_ForgotPassword", loginViewModel);
        }

        /// <summary>
        /// User sign in using OWIN middle-ware.
        /// </summary>
        /// <param name="claims">claims</param>
        private void SignIn(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            //This uses OWIN authentication
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);
            HttpContext.User = new ClaimsPrincipal(AuthenticationManager.AuthenticationResponseGrant.Principal);
        }

        /// <summary>
        /// This action is used to get AuthenticationManager
        /// </summary>
        /// <returns></returns>  
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /// <summary>
        /// The user is loged out 
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.Session.RemoveAll();

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Account");
        }

        #endregion
    }
}