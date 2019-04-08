using Microsoft.Owin;

namespace LAMP.API
{
    /// <summary>
    /// Interface IAuthenticationContext for capable of class AuthenticationContext
    /// </summary>
    public interface IAuthenticationContext
    {
        IOwinContext OwinContext { get; }        
        long UserID { get; }
        string Username { get; }
        string Token { get; }
    }
}
