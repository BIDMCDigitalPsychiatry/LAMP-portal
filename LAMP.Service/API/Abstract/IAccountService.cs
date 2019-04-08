using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Interface IAccountService for capable of class AccountService
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        APIResponseBase AuthenticateUser(UserTokenRequest request);
    }
}
