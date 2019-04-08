using LAMP.ViewModel;
namespace LAMP.Utility
{
    /// <summary>
    /// Interface ICustomAuthentication for capable of class CustomAuthentication
    /// </summary>
    public interface ICustomAuthentication
    {
        void Authenticate(LoginViewModel model);
        void SignOut();
    }

}




