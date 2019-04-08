using LAMP.ViewModel;
using System;
using System.Web;
using System.Web.Security;

namespace LAMP.Utility
{
    /// <summary>
    /// CustomAuthentication is responsible for handling authentication functions
    /// </summary>
    public class CustomAuthentication : ICustomAuthentication 
    {       
        /// <summary>
        /// User Authenticate
        /// </summary>
        /// <param name="model">User request</param>
        public void Authenticate(LoginViewModel model)
        {
            SetCookie(model);

            HttpContext.Current.Session.Clear();
            string roles = string.Empty;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.Email, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false,
                roles, FormsAuthentication.FormsCookiePath);

            string hash = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Session[LAMPConstants.LoggedInUser] = model;
        }

        /// <summary>
        /// User Signout
        /// </summary>
        public void SignOut()
        {
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// The function checks if the Remember Me checkbox has been checked. If so, the cookie is saved, else the cookie is removed.
        /// </summary>
        ///  <param name="model">LoginViewModel</param>
        private void SetCookie(LoginViewModel model)
        {
            //Check if the Remember Me checkbox has been checked. If so, set the value in the cookie.            
            if (model.RememberMe)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(LAMPConstants.Cookie_Name);
                if (cookie == null)
                    cookie = new HttpCookie(LAMPConstants.Cookie_Name);
                cookie.Values.Clear();
                cookie.Values.Add(LAMPConstants.UserName_Cookie, model.Email);
                cookie.Values.Add(LAMPConstants.Password_Cookie, CryptoUtil.EncryptStringWithKey(model.Password));

                cookie.Expires = DateTime.Now.AddYears(30);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
            else
            {
                //if not remove the cookie    
                HttpCookie cookie = new HttpCookie(LAMPConstants.Cookie_Name)
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }
    }
}
