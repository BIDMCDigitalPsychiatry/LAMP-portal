using Microsoft.Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace LAMP.API
{
    /// <summary>
    /// This class inherits from IAuthenticationContext, handles the owinconext elements
    /// </summary>
    public class AuthenticationContext : IAuthenticationContext
    {

        #region Fields
        private HttpContextBase httpContext { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="httpContext"></param>
        public AuthenticationContext(Func<HttpContextBase> httpContext)
        {
            this.httpContext = httpContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Holds owin context
        /// </summary>
        public IOwinContext OwinContext
        {
            get { return httpContext.Request.GetOwinContext(); }
        }

        /// <summary>
        /// Property to get the UserID
        /// </summary>
        public long UserID
        {
            get
            {                
                var userIdClaim = OwinContext.Authentication.User.Claims.SingleOrDefault(a => a.Type == "UserID");
                if (userIdClaim == null)
                {
                    return default(int);
                }
                else
                {
                    return long.Parse(userIdClaim.Value);
                }

            }

        }

        /// <summary>
        /// Property to get the user name
        /// </summary>
        public string Username
        {
            get
            {
                var usernameClaim =
                    OwinContext.Authentication.User.Claims.SingleOrDefault(a => a.Type == ClaimTypes.Name);
                if (usernameClaim == null)
                {
                    return null;
                }
                else
                {
                    return usernameClaim.Value;
                }
            }
        }

        /// <summary>
        /// Property to get the Token
        /// </summary>
        public string Token
        {
            get
            {
                if (OwinContext.Request.Headers["Authorization"] != null)
                {
                    var _token = OwinContext.Request.Headers["Authorization"].ToString();
                    return _token.Replace("Bearer ", "");
                }
                else
                    return null;
            }
        }

        #endregion

    }
}
