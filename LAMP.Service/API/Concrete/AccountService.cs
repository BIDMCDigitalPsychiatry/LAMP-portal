using System;
using System.Linq;
using LAMP.DataAccess;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Class AccountService
    /// </summary>
    public class AccountService : IAccountService
    {
        #region Fields

        private IUnitOfWork _UnitOfWork;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="unitOfWork"></param>
        public AccountService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="request">User details</param>
        /// <returns>Status</returns>
        public APIResponseBase AuthenticateUser(UserTokenRequest request)
        {
            APIResponseBase response = new APIResponseBase();
            try
            {
                var mobileUser = _UnitOfWork.IUserRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.SessionToken == request.SessionToken).FirstOrDefault();
                if (mobileUser == null)
                {
                    response.ErrorCode = LAMPConstants.API_USER_SESSION_EXPIRED;
                    response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_USER_SESSION_EXPIRED);
                }
                else
                    response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response.ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR;
            }
            return response;
        }

        #endregion
    }
}
