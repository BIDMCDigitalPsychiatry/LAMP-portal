using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Security.Claims;
using System.Web;
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.API
{
    /// <summary>
    /// Authentication is responsible for handling authentication related acitivities
    /// </summary>
    public class Authentication : IAuthentication
    {
        #region Variables
        private IUnitOfWork _UnitOfWork;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="UnitOfWork">UnitOfWork</param>
        public Authentication(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        #endregion
        /// <summary>
        /// User Signin
        /// </summary>
        /// <param name="user">User Email</param>
        /// <param name="options">Authentication Type</param>
        /// <param name="authManager">Authentication Manager</param>
        /// <returns></returns>
        public string SignIn(UserAuthenticateRequest user, OAuthAuthorizationServerOptions options, IAuthenticationManager authManager)
        {
            ClaimsIdentity identity = new ClaimsIdentity(options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Username));
            identity.AddClaim(new Claim("UserID", user.UserID.ToString()));
            identity.AddClaim(new Claim("StudyID", user.StudyID));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(60));
            authManager.SignIn(identity);
            return options.AccessTokenFormat.Protect(ticket);
        }

        /// <summary>
        /// User Signout
        /// </summary>
        /// <param name="authManager">Authentication Manager</param>
        public void SignOut(IAuthenticationManager authManager)
        {
            authManager.SignOut();
        }

        /// <summary>
        /// Create Session token object
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="email">Email</param>
        /// <param name="studyId">StudyId</param>
        /// <returns>Session token</returns>
        public UserSessionToken CreateSessionTokenObject(long userId, string email, string studyId)
        {
            var response = new UserSessionToken();
            try
            {
                if (Helper.IsValidEmail(email) == false)
                    email = string.Empty;
                string token = string.Empty;
                IAuthenticationManager authentication = HttpContext.Current.GetOwinContext().Authentication;
                UserAuthenticateRequest userRequest = new UserAuthenticateRequest
                {
                    UserID = userId,
                    Username = email,
                    StudyID = studyId
                };

                token = SignIn(userRequest, Startup.OAuthOptions, authentication);
                User user = _UnitOfWork.IUserRepository.GetById(userId);
                if (user != null)
                {
                    user.SessionToken = token;
                    _UnitOfWork.IUserRepository.Update(user);
                    _UnitOfWork.Commit();
                    response = new UserSessionToken
                    {
                        SessionToken = token,
                        ErrorCode = LAMPConstants.API_SUCCESS_CODE
                    };
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new UserSessionToken
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

    }

}