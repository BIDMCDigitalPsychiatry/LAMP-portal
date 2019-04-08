using System;
using System.Web.Http;
using System.Web.Http.Description;
using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.API.Controllers
{
    /// <summary>
    /// BaseController is responsible for handling common web api acitivities
    /// </summary>
    [Authorize]
    [RoutePrefix("api")]
    public class BaseController : ApiController
    {
        #region Fields
        private IAccountService _AccountService;
        private IAuthenticationContext _authContext;
        #endregion Fields

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="authContext"></param>
        public BaseController(IAccountService accountService, IAuthenticationContext authContext)
        {
            this._AccountService = accountService;
            this._authContext = authContext;
        }

        /// <summary>
        /// Validate User token
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("ValidateUserToken")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public APIResponseBase ValidateUserToken()
        {
            try
            {
                UserTokenRequest tokenRequest = new UserTokenRequest();
                tokenRequest.UserID = _authContext.UserID;
                tokenRequest.SessionToken = _authContext.Token;
                return _AccountService.AuthenticateUser(tokenRequest);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }
    }
}