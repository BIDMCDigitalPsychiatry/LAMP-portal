using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using LAMP.ViewModel;

namespace LAMP.API
{
    /// <summary>
    /// Interface IAuthentication for capable of class Authentication
    /// </summary>
    public partial interface IAuthentication
    {
        string SignIn(UserAuthenticateRequest user, OAuthAuthorizationServerOptions options, IAuthenticationManager authManager);
        void SignOut(IAuthenticationManager authManager);
        UserSessionToken CreateSessionTokenObject(long userId, string email, string studyId);
    }
}