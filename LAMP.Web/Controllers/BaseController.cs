using LAMP.DataAccess;
using LAMP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
namespace LAMP.Web.Controllers
{
    /// <summary>
    /// Implements common functionalities required for all controllers.
    /// </summary>
    public abstract partial class BaseController : Controller
    {
        #region Properties
        public IUnitOfWork _UnitOfWork { get; set; }
        public long loggedInUserId { get; set; }
        public string loggedInUserName { get; set; }
        public string loggedInFirstName { get; set; }
        public string loggedInLastName { get; set; }
        #endregion

        #region Constructor
        public BaseController()
        {
            // TODO:- We will remove this code when dependancy injection is complete.
            _UnitOfWork = new UnitOfWork();
        }

        public BaseController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;

        }
        #endregion

        #region  Methods

        /// <summary>
        /// This action filter is called before an action execution.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IEnumerable<Claim> claims = ClaimsPrincipal.Current.Claims;

            if (claims != null)
            {
                if (claims.Count() > 0)
                {
                    var adminId = claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                    if (adminId != null)
                        loggedInUserId = Convert.ToInt64(adminId.Value);
                }
            }

            //To Get Logged in User Name
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            
            if (email != null)
            {
                var newEmail = CryptoUtil.EncryptInfo(email.Value);//email.Value
                if (_UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == newEmail && u.IsDeleted != true).FirstOrDefault() != null)
                {
                    loggedInFirstName = ((from admin in _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == newEmail && u.IsDeleted!=true) select admin.FirstName).FirstOrDefault());
                    loggedInLastName = ((from admin in _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.Email == newEmail && u.IsDeleted != true) select admin.LastName).FirstOrDefault());
                    ViewBag.Name = CryptoUtil.DecryptInfo(loggedInFirstName) + " " + CryptoUtil.DecryptInfo(loggedInLastName);
                   
                }
                else
                {
                    ViewBag.Name = "";
                }
                   
            }
        }

        /// <summary>
        /// This method is used to dispose the context object.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _UnitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}