using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LAMP.Web.Controllers
{
    /// <summary>
    /// Class for UserAdminController
    /// </summary>
    [Authorize]
    public class UserAdminController : BaseController
    {
        #region Fields
        private IAccountService _accountService;
        private IUserAdminService _userService;
        private IScheduleService _sheduleService;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="requestService">Request Service</param>
        /// <param name="authServices">Authentication</param>
        public UserAdminController(IAccountService accountService, IUserAdminService userService, IScheduleService sheduleService)
        {
            _userService = userService;
            _accountService = accountService;
            _sheduleService = sheduleService;
        }
        #endregion

        #region  Public Methods
        [HttpPost]
        public void SetCurrentPage(string page)
        {
            Session["CurrentPage"] = page;
        }

        [HttpGet]
        public string GetCurrentPage()
        {
            if (Session["CurrentPage"] != null)
                return Session["CurrentPage"].ToString();
            else
                return string.Empty;
        }

        #region  Admin

        /// <summary>
        /// Creates the admin.
        /// </summary>
        /// <param name="adminrModel">The admin model.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateAdmin(AdminViewModel adminModel)
        {
            ModelState.Clear();
            return View(adminModel);
        }

        /// <summary>
        /// Deletes the admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAdmin(long adminId)
        {
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.AdminListViewModel = _userService.DeleteAdmin(adminId);
            response.UserTypeDropdownId = "2";
            response.LoggedInAdminId = loggedInUserId;
            return RedirectToAction("BackToAdmins");
        }

        /// <summary>
        /// Creates the admin.
        /// </summary>
        /// <param name="adminModel">The admin model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateAdmin(AdminViewModel adminModel, string url)
        {
            var model = new AdminViewModel();
            CultureInfo culture = new CultureInfo("en-US");
            if (ModelState.IsValid)
            {
                model = _userService.SaveAdmin(adminModel);
                model.IsSaved = true;
                if (model.Status != LAMPConstants.SUCCESS_CODE)
                {
                    adminModel.Status = model.Status;
                    adminModel.Message = model.Message;
                    return View(adminModel);
                }
                model.AdminID = adminModel.AdminID;
                ModelState.Clear();
            }
            return View(model);
        }

        /// <summary>
        /// Edit User Data
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Status</returns>
        [HttpGet]
        public ActionResult EditAdmin(long adminId)
        {
            AdminViewModel model = new AdminViewModel();
            model.AdminID = adminId;
            model = _userService.GetAdminDetails(adminId);
            return View("CreateAdmin", model);
        }

        /// <summary>
        /// Set Protocol dates
        /// </summary>
        /// <param name="protocolModel">protocolModel</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetProtocolDate(ProtocolViewModel protocolModel)
        {
            ViewModelBase model = _userService.SaveProtocolDate(protocolModel);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Protocol dates
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getProtocolDate(long UserId)
        {
            ProtocolViewModel model = _userService.getProtocolDate(UserId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="response">user list</param>
        private void GetAdminsResponse(AdminListViewModel model, ref AdminListViewModel response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "CreatedOn";
                _SortPageOptions.SortOrder = "desc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "CreatedOn" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "desc" : model.SortPageOptions.SortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.AdminList.ToPagedList<Admins>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedAdminList = new StaticPagedList<Admins>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.AdminList.Count);

            response.AdminList = response.AdminList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.AdminList = response.AdminList.Take(_SortPageOptions.PageSize).ToList();
            response.AdminList.Select(admin =>
            {
                admin.AdminID = admin.AdminID;
                admin.Email = CryptoUtil.DecryptInfo(admin.Email);
                admin.FirstName = CryptoUtil.DecryptInfo(admin.FirstName);
                admin.LastName = CryptoUtil.DecryptInfo(admin.LastName);
                admin.CreatedOnString = Helper.GetDateString(admin.CreatedOn, "MM/dd/yyyy");
                return admin;
            }).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["AdminsPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Show User Page
        /// </summary>
        /// <param name="model">User model object</param>
        /// <param name="userId">User Id</param>
        /// <returns>All Users data</returns>
        public ActionResult LoadAdmin(AdminandUserListViewModel model, long? userId)
        {
            LogUtil.Info("Admins: " + userId.ToString());
            if (model == null)
                model = new AdminandUserListViewModel();
            if (HttpContext.Session["AdminsPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["AdminsPagingCriteria"].ToString().Split('|');
                model.AdminListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.AdminListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.AdminListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.AdminListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.AdminListViewModel.SortPageOptions.CurrentPage = 1;
                model.AdminListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.AdminListViewModel.SortPageOptions.SortField = "CreatedOn";
                model.AdminListViewModel.SortPageOptions.SortOrder = "desc";
            }
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserTypeDropdownId = "2";
            model.AdminListViewModel.LoggedInAdminId = loggedInUserId;
            response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);
            var userNewResponse = response.AdminListViewModel;
            GetAdminsResponse(model.AdminListViewModel, ref userNewResponse);
            response.AdminListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            model.UserTypeDropdownId = "2";
            model.LoggedInAdminId = loggedInUserId;
            model.AdminListViewModel = userNewResponse;
            return View(model);
        }

        /// <summary>
        /// Search the admin.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchAdmin(AdminListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new AdminListViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "CreatedOn";
                model.SortPageOptions.SortOrder = "desc";
            }
            model.LoggedInAdminId = loggedInUserId;
            if (searchString == null || searchString.Trim().Length == 0)
                searchString = model.SearchId;
            if (searchString != null && searchString.Trim().Length > 0)
            {
                model.SearchId = searchString;
                response = _userService.GetAllAdmins(model);
            }
            else
            { response = _userService.GetAllAdmins(model); }

            GetAdminsResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_ADMIN_MESSAGE);
            response.SearchId = searchString;
            ModelState.Clear();
            AdminandUserListViewModel newResponse = new AdminandUserListViewModel();
            newResponse.UserTypeDropdownId = "2";
            newResponse.AdminListViewModel = response;
            newResponse.LoggedInAdminId = loggedInUserId;
            return PartialView("_AdminListPartial", newResponse);
        }

        /// <summary>
        /// Backs to admins.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToAdmins()
        {
            AdminandUserListViewModel model = new AdminandUserListViewModel();
            if (HttpContext.Session["AdminsPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["AdminsPagingCriteria"].ToString().Split('|');
                model.AdminListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.AdminListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.AdminListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.AdminListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.AdminListViewModel.SortPageOptions.CurrentPage = 1;
                model.AdminListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.AdminListViewModel.SortPageOptions.SortField = "CreatedOn";
                model.AdminListViewModel.SortPageOptions.SortOrder = "desc";
            }
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserTypeDropdownId = "2";
            response.LoggedInAdminId = loggedInUserId;
            model.AdminListViewModel.LoggedInAdminId = loggedInUserId;
            response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);
            var newresponse = response.AdminListViewModel;
            GetAdminsResponse(model.AdminListViewModel, ref newresponse);
            response.AdminListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View("Users", response);
        }

        #endregion

        #region App Help
        /// <summary>
        /// Hows to use this application.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult HowToUseThisApp(AppHelpViewModel model)
        {
            model = new AppHelpViewModel();
            model.AdminID = loggedInUserId;
            model = _userService.GetAppHelpOfLoggedInAdmin(model);
            return View(model);
        }

        /// <summary>
        /// Gets the application help details.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        public ActionResult GetAppHelpDetails(long helpId)
        {
            AppHelpViewModel model = new AppHelpViewModel();
            model.AdminID = loggedInUserId;
            model = _userService.GetAppHelpDetailsByHelpId(helpId);
            return View("AppHelpView", model);
        }

        /// <summary>
        /// Adds the application help.
        /// </summary>
        /// <param name="appHelpViewModel">The application help view model.</param>
        /// <param name="appImage">The application image.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAppHelp(AppHelpViewModel appHelpViewModel, HttpPostedFileBase appImage)
        {
            appHelpViewModel.AdminID = loggedInUserId;
            if (appImage != null)
            {
                appHelpViewModel.AppHelpExtension = System.IO.Path.GetExtension(appImage.FileName);
                appHelpViewModel = _userService.SaveAppHelp(appHelpViewModel, appImage.InputStream);
            }
            else
            {
                appHelpViewModel = _userService.SaveAppHelp(appHelpViewModel, null);
            }
            TempData["AppHelpSaveMessage"] = appHelpViewModel.Message;
            TempData["AppHelpStatus"] = appHelpViewModel.Status;
            TempData["IsSaved"] = appHelpViewModel.IsSaved;
            return RedirectToAction("HowToUseThisApp", "UserAdmin");
        }

        /// <summary>
        /// Deletes the application help.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAppHelp(long helpId)
        {
            AppHelpViewModelList response = new AppHelpViewModelList();
            response = _userService.DeleteAppHelp(helpId);
            ModelState.Clear();
            return View("ViewAllAppHelp", response);
        }

        /// <summary>
        /// Gets the application help response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetAppHelpResponse(AppHelpViewModelList model, ref AppHelpViewModelList response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "CreatedOn";
                _SortPageOptions.SortOrder = "desc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.APPHELP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "CreatedOn" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "desc" : model.SortPageOptions.SortOrder;    // sortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   // page;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.AppHelpList.ToPagedList<AppHelpViewModel>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedAppHelpList = new StaticPagedList<AppHelpViewModel>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.AppHelpList.Count);

            response.AppHelpList = response.AppHelpList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.AppHelpList = response.AppHelpList.Take(_SortPageOptions.PageSize).ToList();
            response.AppHelpList.Select(appHelp =>
            {
                appHelp.HelpText = CryptoUtil.DecryptInfo(appHelp.HelpText);
                appHelp.ImageURL = CryptoUtil.DecryptInfo(appHelp.ImageURL);
                appHelp.Content = CryptoUtil.DecryptInfo(appHelp.Content);
                appHelp.CreatedOnString = Helper.GetDateString(appHelp.CreatedOn, "MM/dd/yyyy");
                appHelp.AdminFullName = CryptoUtil.DecryptInfo(appHelp.CreatedAdminFName) + " " + CryptoUtil.DecryptInfo(appHelp.CreatedAdminLName);
                return appHelp;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["AppHelpPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();

        }

        /// <summary>
        /// Views all application help.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public ActionResult ViewAllAppHelp(AppHelpViewModelList model, long? userId)
        {
            Session["CurrentPage"] = "ViewAllAppHelp";
            LogUtil.Info("ViewAllAppHelp: " + userId.ToString());
            if (model == null)
                model = new AppHelpViewModelList();
            if (userId != null && userId > 0)
            {
                string[] pageConditions = HttpContext.Session["AppHelpPagingCriteria"].ToString().Split('|');
                model.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.APPHELP_PAGE_SIZE;
                model.SortPageOptions.SortField = "CreatedOn";
                model.SortPageOptions.SortOrder = "desc";
            }
            AppHelpViewModelList response = new AppHelpViewModelList();
            model.AdminLoggedInId = loggedInUserId;
            response = _userService.GetAllAppHelp(model);
            response.AdminLoggedInId = loggedInUserId;
            GetAppHelpResponse(model, ref response);
            response.UnregisteredAppHelpMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View(response);
        }

        /// <summary>
        /// Search all application help.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">Current page</param>
        /// <returns></returns>
        public ActionResult SearchAllAppHelp(AppHelpViewModelList model, string command, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new AppHelpViewModelList();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }
            response = _userService.GetAllAppHelp(model);
            GetAppHelpResponse(model, ref response);
            response.AdminLoggedInId = loggedInUserId;
            ModelState.Clear();
            return PartialView("_AppHelpListPartial", response);
        }

        /// <summary>
        /// Back to application help.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToAppHelp()
        {
            AppHelpViewModelList model = new AppHelpViewModelList();
            if (HttpContext.Session["AppHelpPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["AppHelpPagingCriteria"].ToString().Split('|');
                model.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.APPHELP_PAGE_SIZE;
                model.SortPageOptions.SortField = "CreatedOn";
                model.SortPageOptions.SortOrder = "desc";
            }
            AppHelpViewModelList response = new AppHelpViewModelList();
            model.AdminLoggedInId = loggedInUserId;
            response = _userService.GetAllAppHelp(model);
            GetAppHelpResponse(model, ref response);
            response.UnregisteredAppHelpMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            response.AdminLoggedInId = loggedInUserId;
            return View("ViewAllAppHelp", response);
        }

        #endregion

        #region Users

        /// <summary>
        /// Exports the specified user ids.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public FileContentResult Export(string userIds, string fromDate, string toDate)
        {
            try
            {
                var UserDataExportListViewModel = _userService.GetUserExportDetailList(userIds, fromDate, toDate);
                byte[] filecontent = ExcelExportHelper.ExportExcelViewModel(UserDataExportListViewModel, "", true);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "UserReport.xlsx");
            }
            catch (Exception ex)
            {
                LogUtil.Error("controller Export: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Call Edit page
        /// </summary>
        /// <param name="userModel">User Object</param>
        /// <returns>user details</returns>
        [HttpGet]
        public ActionResult Edit(UserViewModel userModel)
        {
            ViewBag.Status = null;
            userModel.Gender = "";
            userModel.StudyId = Helper.GetNemericCode();
            userModel.FormattedStudyId = userModel.StudyId;
            userModel.BirthDateString = "";
            ModelState.Clear();
            return View(userModel);
        }

        /// <summary>
        /// Edits the user
        /// </summary>
        /// <param name="userModel">User details</param>
        /// <param name="profile">Clinical Profile data</param>
        /// <returns>the updated user data</returns>
        [HttpPost]
        public ActionResult Edit(UserViewModel userModel, HttpPostedFileBase profile)
        {
            var model = new UserViewModel();
            model.FormattedStudyId = userModel.StudyId;
            CultureInfo culture = new CultureInfo("en-US");
            DateTime BirthDate;
            if (userModel.BirthDateString != null && userModel.BirthDateString.Trim().Length > 0)
            {
                if (DateTime.TryParseExact(userModel.BirthDateString.Trim(), "MM/dd/yyyy",
                                 new CultureInfo("en-US"),
                                 DateTimeStyles.None,
                                 out BirthDate))
                {
                    if (BirthDate > DateTime.Today)
                    {
                        ModelState.AddModelError("BirthDateString", "Date of birth should be less than today's date.");
                    }
                }
                else
                {
                    ModelState.AddModelError("BirthDateString", "Invalid date of birth.");
                }
            }

            if (ModelState.IsValid)
            {
                userModel.AdminID = loggedInUserId;

                if (profile != null)
                {
                    userModel.ClinicalProfileExtension = System.IO.Path.GetExtension(profile.FileName);
                    model = _userService.SaveUser(userModel, profile.InputStream);
                }
                else
                {
                    userModel.ClinicalProfileExtension = string.Empty;
                    model = _userService.SaveUser(userModel, null);
                }
                if (model.Status == LAMPConstants.SUCCESS_CODE)
                    model.StudyId = Helper.GetNemericCode();
                else
                    model.StudyId = userModel.StudyId;

                model.FormattedStudyId = model.StudyId;
                model.IsSaved = true;

                if (model.Status != LAMPConstants.SUCCESS_CODE)
                {
                    userModel.Status = model.Status;
                    userModel.Message = model.Message;
                    userModel.FormattedStudyId = model.FormattedStudyId;
                    return View(userModel);
                }
                model.UserID = userModel.UserID;
                ModelState.Clear();
            }
            return View(model);
        }

        /// <summary>
        /// For  Listing of User
        /// </summary>
        /// <returns>All Users data</returns>
        public ActionResult UsersList()
        {
            UserListViewModel model = new UserListViewModel();
            string[] pageConditions = HttpContext.Session["UsersPagingCriteria"].ToString().Split('|');
            model.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
            model.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
            model.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
            model.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);

            UserListViewModel response = new UserListViewModel();
            response = _userService.GetUsers(model);
            GetUsersResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View("Users", response);
        }

        /// <summary>
        /// Back to users.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToUsers()
        {
            AdminandUserListViewModel model = new AdminandUserListViewModel();
            if (HttpContext.Session["UsersPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["UsersPagingCriteria"].ToString().Split('|');
                model.UserListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.UserListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.UserListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.UserListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.UserListViewModel.SortPageOptions.CurrentPage = 1;
                model.UserListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.UserListViewModel.SortPageOptions.SortField = "StudyId";
                model.UserListViewModel.SortPageOptions.SortOrder = "asc";
            }
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserTypeDropdownId = "1";
            response.LoggedInAdminId = loggedInUserId;
            model.UserListViewModel.LoggedInAdminId = loggedInUserId;
            response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);
            var newresponse = response.UserListViewModel;
            GetUsersResponse(model.UserListViewModel, ref newresponse);
            response.UserListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View("Users", response);

        }

        /// <summary>
        /// Search for users with id like the search string
        /// </summary>
        /// <param name="model">User list model</param>
        /// <param name="command">command</param>
        /// <param name="searchString">Search String</param>
        /// <param name="sortColumn">Sort Column</param>
        /// <param name="sortOrder">Sort Order</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <returns>selected user list</returns>
        [HttpGet]
        public ActionResult SearchUsers(UserListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new UserListViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "StudyId";
                model.SortPageOptions.SortOrder = "asc";
            }
            model.LoggedInAdminId = loggedInUserId;
            if (searchString == null || searchString.Trim().Length == 0)
                searchString = model.SearchId;
            if (searchString != null && searchString.Trim().Length > 0)
            {
                model.SearchId = searchString;
                response = _userService.GetUsers(model);
            }
            else
            {
                response = _userService.GetUsers(model);
            }

            GetUsersResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            response.SearchId = searchString;
            ModelState.Clear();
            AdminandUserListViewModel newResponse = new AdminandUserListViewModel();
            newResponse.UserTypeDropdownId = "1";
            newResponse.UserListViewModel = response;
            newResponse.LoggedInAdminId = loggedInUserId;
            return PartialView("_UserListPartial", newResponse);
        }


        /// <summary>
        /// Searches the admin or users.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchAdminOrUsers(AdminandUserListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            if (model.UserTypeDropdownId == "1")
            {
                if (model.UserListViewModel.SortPageOptions.SortField == null)
                {
                    model.UserListViewModel.SortPageOptions.CurrentPage = 1;
                    model.UserListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                    model.UserListViewModel.SortPageOptions.SortField = "StudyId";
                    model.UserListViewModel.SortPageOptions.SortOrder = "asc";
                }
                model.LoggedInAdminId = loggedInUserId;
                model.UserListViewModel.LoggedInAdminId = loggedInUserId;
                if (searchString == null || searchString.Trim().Length == 0)
                    searchString = model.UserListViewModel.SearchId;
                if (searchString != null && searchString.Trim().Length > 0)
                {

                    response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);
                }
                else
                    response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);

                var newUserList = response.UserListViewModel;
                GetUsersResponse(model.UserListViewModel, ref newUserList);
                response.UserListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            }
            else
            {
                if (model.AdminListViewModel.SortPageOptions.SortField == null)
                {
                    model.AdminListViewModel.SortPageOptions.CurrentPage = 1;
                    model.AdminListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                    model.AdminListViewModel.SortPageOptions.SortField = "Createdon";
                    model.AdminListViewModel.SortPageOptions.SortOrder = "desc";
                }
                if (searchString == null || searchString.Trim().Length == 0)
                    searchString = model.AdminListViewModel.SearchId;
                if (searchString != null && searchString.Trim().Length > 0)
                    response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);
                else
                    response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);

                var newUserList = response.AdminListViewModel;
                GetAdminsResponse(model.AdminListViewModel, ref newUserList);
                response.AdminListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            }

            response.UserTypeDropdownId = model.UserTypeDropdownId;
            response.LoggedInAdminId = loggedInUserId;
            return PartialView("_UserListPartial", response);
        }


        /// <summary>
        /// Loads the admin or users.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoadAdminOrUsers(int mode)
        {
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            AdminandUserListViewModel model = new AdminandUserListViewModel();
            string searchString = string.Empty;

            if (mode == 1)
            {
                if (model.UserListViewModel.SortPageOptions.SortField == null)
                {
                    model.UserListViewModel.SortPageOptions.CurrentPage = 1;
                    model.UserListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                    model.UserListViewModel.SortPageOptions.SortField = "StudyId";
                    model.UserListViewModel.SortPageOptions.SortOrder = "asc";
                }
                model.LoggedInAdminId = loggedInUserId;
                model.UserListViewModel.LoggedInAdminId = loggedInUserId;
                if (searchString == null || searchString.Trim().Length == 0)
                    searchString = model.UserListViewModel.SearchId;
                if (searchString != null && searchString.Trim().Length > 0)
                {
                    response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);
                }
                else
                    response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);

                var newUserList = response.UserListViewModel;
                GetUsersResponse(model.UserListViewModel, ref newUserList);
                response.UserListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
                response.UserTypeDropdownId = model.UserTypeDropdownId;
                response.LoggedInAdminId = loggedInUserId;
                response.UserTypeDropdownId = "1";
                return PartialView("_UserListPartial", response);
            }
            else
            {
                if (model.AdminListViewModel.SortPageOptions.SortField == null)
                {
                    model.AdminListViewModel.SortPageOptions.CurrentPage = 1;
                    model.AdminListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                    model.AdminListViewModel.SortPageOptions.SortField = "Createdon";
                    model.AdminListViewModel.SortPageOptions.SortOrder = "desc";
                }
                if (searchString == null || searchString.Trim().Length == 0)
                    searchString = model.AdminListViewModel.SearchId;
                if (searchString != null && searchString.Trim().Length > 0)
                    response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);
                else
                    response.AdminListViewModel = _userService.GetAllAdmins(model.AdminListViewModel);

                var newUserList = response.AdminListViewModel;
                GetAdminsResponse(model.AdminListViewModel, ref newUserList);
                response.AdminListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
                response.UserTypeDropdownId = model.UserTypeDropdownId;
                response.LoggedInAdminId = loggedInUserId;
                response.UserTypeDropdownId = "2";
                return PartialView("_AdminListPartial", response);
            }
        }

        /// <summary>
        /// Userses the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public ActionResult Users(AdminandUserListViewModel model, long? userId)
        {
            Session["CurrentPage"] = "Users";
            LogUtil.Info("Users: " + userId.ToString());
            if (model == null)
                model = new AdminandUserListViewModel();
            if (HttpContext.Session["UsersPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["UsersPagingCriteria"].ToString().Split('|');
                model.UserListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.UserListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.UserListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.UserListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.UserListViewModel.SortPageOptions.CurrentPage = 1;
                model.UserListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.UserListViewModel.SortPageOptions.SortField = "StudyId";
                model.UserListViewModel.SortPageOptions.SortOrder = "asc";
            }

            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserTypeDropdownId = "1";
            model.UserListViewModel.LoggedInAdminId = loggedInUserId;
            response.UserListViewModel = _userService.GetUsers(model.UserListViewModel);
            var userNewResponse = response.UserListViewModel;
            GetUsersResponse(model.UserListViewModel, ref userNewResponse);
            response.UserListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            model.UserTypeDropdownId = "1";
            model.LoggedInAdminId = loggedInUserId;
            model.UserListViewModel = userNewResponse;
            return View(model);
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="response">user list</param>
        private void GetUsersResponse(UserListViewModel model, ref UserListViewModel response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "StudyId";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "StudyId" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.UserList.ToPagedList<AdminUser>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedUserList = new StaticPagedList<AdminUser>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.UserList.Count);

            response.UserList = response.UserList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.UserList = response.UserList.Take(_SortPageOptions.PageSize).ToList();
            response.UserList.Select(user =>
            {
                user.ClinicalProfileURL = CryptoUtil.DecryptInfo(user.ClinicalProfileURL);
                user.Email = CryptoUtil.DecryptInfo(user.Email);
                user.FirstName = CryptoUtil.DecryptInfo(user.FirstName);
                user.LastName = CryptoUtil.DecryptInfo(user.LastName);
                user.Phone = CryptoUtil.DecryptInfo(user.Phone);
                user.StudyId = CryptoUtil.DecryptInfo(user.StudyId);

                user.RegisteredOnString = Helper.GetDateString(user.RegisteredOn, "MM/dd/yyyy");
                user.LastSurveyDateString =
                    Helper.GetDateString(
                    Convert.ToDateTime((from survey in _UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(u => u.UserID == user.UserID).OrderByDescending(o => o.StartTime)
                                        select survey.StartTime).FirstOrDefault()), "MM/dd/yyyy");
                return user;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["UsersPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();

        }

        /// <summary>
        /// Shows user details
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User data</returns>
        [HttpGet]
        public ActionResult ViewUser(long userId)
        {
            UserViewModel model = new UserViewModel();
            model = _userService.GetUserDetails(userId);
            model.FormattedStudyId = model.StudyId;
            model.IsDisabled = true;
            return View("Edit", model);
        }

        /// <summary>
        /// Edit User Data
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Status</returns>
        [HttpGet]
        public ActionResult EditUser(long userId)
        {
            UserViewModel model = new UserViewModel();
            model = _userService.GetUserDetails(userId);
            model.FormattedStudyId = model.StudyId;
            return View("Edit", model);
        }

        /// <summary>
        /// Delete User details
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User list</returns>
        [HttpPost]
        public ActionResult DeleteUser(long userId)
        {
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserListViewModel = _userService.DeleteUser(userId);
            ModelState.Clear();
            return View("Users", response);
        }

        #endregion

        #region Tips and Blogs

        /// <summary>
        /// Adds the tips and blogs.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult AddTipsAndBlogs(TipsBlogsViewModel model)
        {
            Session["CurrentPage"] = "AddTipsAndBlogs";
            model = new TipsBlogsViewModel();
            model.LoggedInAdminId = loggedInUserId;
            model.TipsViewModel.AdminId = loggedInUserId;
            model.TipsViewModel = _userService.GetTips(model.TipsViewModel);
            return View(model);
        }

        /// <summary>
        /// Adds the tips.
        /// </summary>
        /// <param name="TipsBlogsViewModel">The tips blogs view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTips(TipsBlogsViewModel TipsBlogsViewModel)
        {
            TipsBlogsViewModel.LoggedInAdminId = loggedInUserId;
            TipsBlogsViewModel.TipsViewModel.AdminId = loggedInUserId;
            _userService.SaveTips(TipsBlogsViewModel.TipsViewModel);
            return View("AddTipsAndBlogs", TipsBlogsViewModel);
        }

        /// <summary>
        /// Adds the blogs.
        /// </summary>
        /// <param name="tipsBlogsViewModel">The tips blogs view model.</param>
        /// <param name="profile">The profile.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBlogs(TipsBlogsViewModel tipsBlogsViewModel, HttpPostedFileBase blogImage)
        {
            tipsBlogsViewModel.LoggedInAdminId = loggedInUserId;
            tipsBlogsViewModel.BlogsViewModel.AdminId = loggedInUserId;
            if (blogImage != null)
            {
                tipsBlogsViewModel.BlogsViewModel.BlogExtension = System.IO.Path.GetExtension(blogImage.FileName);
                tipsBlogsViewModel.BlogsViewModel = _userService.SaveBlog(tipsBlogsViewModel.BlogsViewModel, blogImage.InputStream);
            }
            else
            {
                tipsBlogsViewModel.BlogsViewModel = _userService.SaveBlog(tipsBlogsViewModel.BlogsViewModel, null);
            }

            TempData["BlogSaveMessage"] = tipsBlogsViewModel.BlogsViewModel.Message;
            TempData["BlogStatus"] = tipsBlogsViewModel.BlogsViewModel.Status;
            TempData["IsSaved"] = tipsBlogsViewModel.BlogsViewModel.IsSaved;
            return RedirectToAction("AddTipsAndBlogs", "UserAdmin");
        }

        /// <summary>
        /// Gets the blogs response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetBlogsResponse(TipsBlogsViewModel model, ref TipsBlogsViewModel response)
        {
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "CreatedOn";
                _SortPageOptions.SortOrder = "desc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.BLOG_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "CreatedOn" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "desc" : model.SortPageOptions.SortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.BlogList.ToPagedList<BlogsViewModel>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedBlogList = new StaticPagedList<BlogsViewModel>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.BlogList.Count);

            response.BlogList = response.BlogList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.BlogList = response.BlogList.Take(_SortPageOptions.PageSize).ToList();
            response.BlogList.Select(user =>
            {
                user.ImageURL = CryptoUtil.DecryptInfo(user.ImageURL);
                user.BlogTitle = user.BlogTitle;
                user.Content = user.Content;
                user.CreatedOn = user.CreatedOn;
                user.BlogID = user.BlogID;
                user.CreatedOnString = Helper.GetDateString(user.CreatedOn, "MM/dd/yyyy");
                user.CreatedAdminName = CryptoUtil.DecryptInfo(user.CreatedAdminFName) + " " + CryptoUtil.DecryptInfo(user.CreatedAdminLName);
                return user;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["BlogsPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Views all blogs.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult ViewAllBlogs(TipsBlogsViewModel model)
        {
            model.SortPageOptions.CurrentPage = 1;
            model.SortPageOptions.PageSize = LAMPConstants.BLOG_PAGE_SIZE;
            model.SortPageOptions.SortField = "CreatedOn";
            model.SortPageOptions.SortOrder = "desc";
            TipsBlogsViewModel response = new TipsBlogsViewModel();
            response = _userService.GetAllBlogsList(model);
            GetBlogsResponse(model, ref response);
            response.LoggedInAdminId = loggedInUserId;
            ModelState.Clear();
            return View("ViewAllBlogs", response);
        }

        /// <summary>
        /// Back to blogs.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToBlogs()
        {
            TipsBlogsViewModel model = new TipsBlogsViewModel();
            if (HttpContext.Session["BlogsPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["BlogsPagingCriteria"].ToString().Split('|');
                model.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.BLOG_PAGE_SIZE;
                model.SortPageOptions.SortField = "CreatedOn";
                model.SortPageOptions.SortOrder = "desc";
            }
            TipsBlogsViewModel response = new TipsBlogsViewModel();
            response = _userService.GetAllBlogsList(model);
            GetBlogsResponse(model, ref response);
            response.LoggedInAdminId = loggedInUserId;
            ModelState.Clear();
            return View("ViewAllBlogs", response);
        }

        /// <summary>
        /// Searches all blogs.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult SearchAllBlogs(TipsBlogsViewModel model, string command, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new TipsBlogsViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }
            response = _userService.GetAllBlogsList(model);
            GetBlogsResponse(model, ref response);
            response.LoggedInAdminId = loggedInUserId;
            ModelState.Clear();
            return PartialView("_BlogListPartial", response);
        }

        [HttpGet]
        /// <summary>
        /// Edits the blog.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        public ActionResult EditBlog(long blogId)
        {
            TipsBlogsViewModel model = new TipsBlogsViewModel();
            model = _userService.GetBlogDetails(blogId);
            model.TipsViewModel = _userService.GetTips(model.TipsViewModel);
            return View("AddTipsAndBlogs", model);
        }

        /// <summary>
        /// Deletes the blog.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteBlog(long blogId)
        {
            TipsBlogsViewModel response = new TipsBlogsViewModel();
            response = _userService.DeleteBlog(blogId);
            ModelState.Clear();
            return View("ViewAllBlogs", response);
        }

        #endregion

        #region Surveys & Schedule & settings

        /// <summary>
        /// Sorting and paging survey details
        /// </summary>
        /// <param name="model">Survey model</param>
        /// <param name="response">Survey model response</param>
        private void GetSurveyDetailsResponse(SurveyResultsViewModel model, ref SurveyResultsViewModel response)
        {
            var _SortPageOptions = new QAndASortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "Question";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "Question" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerQAndAs = response.QuestAndAnsList.ToPagedList<SurveyResultsDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedSurveyResultsDetailList = new StaticPagedList<SurveyResultsDetail>(providerQAndAs, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.QuestAndAnsList.Count);
            response.QuestAndAnsList = response.QuestAndAnsList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.QuestAndAnsList = response.QuestAndAnsList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows survey details
        /// </summary>
        /// <param name="model">Survey result model</param>
        /// <param name="sortColumn">Column name</param>
        /// <param name="sortOrder">Sort order</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="page">Current page</param>
        /// <param name="surveyResultId">Survey result id</param>
        /// <param name="studyId">Study id</param>
        /// <param name="AdminBatchSchID">Admin Batch SchID</param>
        /// <returns>Survey details</returns>
        [HttpGet]
        public ActionResult SurveyDetails(SurveyResultsViewModel model, string sortColumn, string sortOrder, int? pageSize, int? page, long surveyResultId, string studyId, long AdminBatchSchID)
        {
            var response = new SurveyResultsViewModel();
            if (sortColumn != null && pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else
            {
                model.SortPageOptions.CurrentPage = (page != null) ? Convert.ToInt16(page) : Convert.ToInt16(1);
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "Question";
                model.SortPageOptions.SortOrder = "asc";
                HttpContext.Session["UserActivitySurveyPagingCriteria"] = model.SortPageOptions.SortField + "|" + model.SortPageOptions.SortOrder + "|" + model.SortPageOptions.PageSize.ToString() + "|" + model.SortPageOptions.CurrentPage.ToString();
                model.SortPageOptions = null;
            }

            long SurveyResultID = surveyResultId;
            response = _userService.GetSurveyResults(SurveyResultID, AdminBatchSchID);
            response.StudyId = studyId;
            GetSurveyDetailsResponse(model, ref response);
            ModelState.Clear();
            if (sortColumn != null && pageSize != 0 && pageSize > 0)
            {
                return PartialView("_SurveyDetailsListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /// <summary>
        /// Shedules the survey and game.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        public ActionResult SheduleSurveyAndGame(long userId)
        {
            ScheduleViewModel scheduleViewModel = new ScheduleViewModel();
            scheduleViewModel.LoggedInUserId = loggedInUserId;
            scheduleViewModel.UserId = userId;
            scheduleViewModel = _userService.GetScheduleViewModelDetails(scheduleViewModel);
            return View(scheduleViewModel);
        }

        /// <summary>
        /// Saves the shedule survey and game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveSheduleSurveyAndGame(ScheduleViewModel scheduleViewModel)
        {
            scheduleViewModel.LoggedInUserId = loggedInUserId;
            scheduleViewModel = _userService.SaveSheduleSurveyAndGame(scheduleViewModel);
            TempData["DistractionSurveyMessage"] = scheduleViewModel.Message;
            TempData["DistractionSurveyStatus"] = scheduleViewModel.Status;
            TempData["IsSaved"] = scheduleViewModel.IsSaved;
            return RedirectToAction("SheduleSurveyAndGame", new { userId = scheduleViewModel.UserId });
        }

        /// <summary>
        /// Settingses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Settings()
        {
            Session["CurrentPage"] = "Settings";
            DistractionSurveyViewModel distractionSurveyViewModel = new DistractionSurveyViewModel();
            distractionSurveyViewModel.LoggedInUserId = loggedInUserId;
            distractionSurveyViewModel.JewelsTrailsSettings.JewelsTrailsSettingsType = 1;
            distractionSurveyViewModel = _userService.GetDistractionSurveyDetails(distractionSurveyViewModel);
            return View(distractionSurveyViewModel);
        }

        /// <summary>
        /// Gets the type of the jewels settings by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetJewelsSettingsByType(string type)
        {
            var result = _userService.GetJewelsTrailsSettingsByType(type, loggedInUserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the distraction survey list.
        /// </summary>
        /// <param name="cTestViewModelList">The c test view model list.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveDistractionSurveyList(List<CTestViewModel> cTestViewModelList)
        {
            DistractionSurveyViewModel response = new DistractionSurveyViewModel();
            response.LoggedInUserId = loggedInUserId;
            response.CTestViewModelList = cTestViewModelList;
            response = _userService.SaveDistractionSurveyDetails(response);
            TempData["DistractionSurveyMessage"] = response.Message;
            TempData["DistractionSurveyStatus"] = response.Status;
            TempData["IsSaved"] = response.IsSaved;

            return Json(response);
        }

        /// <summary>
        /// Adds the jewels settings.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddJewelsSettings(DistractionSurveyViewModel DistractionSurveyViewModel)
        {

            DistractionSurveyViewModel.LoggedInUserId = loggedInUserId;
            DistractionSurveyViewModel = _userService.SaveJewelsTrialsSettings(DistractionSurveyViewModel);
            TempData["JewelsMessage"] = DistractionSurveyViewModel.JewelsTrailsSettings.Message;
            TempData["JewelsStatus"] = DistractionSurveyViewModel.JewelsTrailsSettings.Status;
            TempData["IsSaved"] = DistractionSurveyViewModel.JewelsTrailsSettings.IsSaved;
            TempData["IsJewel"] = true;
            return RedirectToAction("Settings", DistractionSurveyViewModel);
        }

        /// <summary>
        /// Adds the expiry option.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddExpiryOption(DistractionSurveyViewModel DistractionSurveyViewModel)
        {

            DistractionSurveyViewModel.LoggedInUserId = loggedInUserId;
            DistractionSurveyViewModel = _userService.SaveExpiryOption(DistractionSurveyViewModel);
            TempData["ExpiryMessage"] = DistractionSurveyViewModel.AdminSettings.Message;
            TempData["ExpiryStatus"] = DistractionSurveyViewModel.AdminSettings.Status;
            TempData["IsSaved"] = DistractionSurveyViewModel.AdminSettings.IsSaved;
            TempData["IsExpiry"] = true;
            return RedirectToAction("Settings", DistractionSurveyViewModel);
        }

        //*****************************New*****************Schedule*************************************//
        /// <summary>
        /// sorting and paging user list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="response">user list</param>
        private void GetSurveyScheduleResponse(ScheduleSurveyListViewModel model, ref ScheduleSurveyListViewModel response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "SurveyName";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "SurveyName" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.AdminSurveyScheduleViewModelList.ToPagedList<AdminSurveyScheduleViewModel>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedAdminSurveyScheduleViewModel = new StaticPagedList<AdminSurveyScheduleViewModel>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.AdminSurveyScheduleViewModelList.Count);
            response.AdminSurveyScheduleViewModelList = response.AdminSurveyScheduleViewModelList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.AdminSurveyScheduleViewModelList = response.AdminSurveyScheduleViewModelList.Take(_SortPageOptions.PageSize).ToList();
            response.AdminSurveyScheduleViewModelList.Select(admin =>
            {
                admin.AdminID = admin.AdminID;
                admin.AdminSurveySchID = admin.AdminSurveySchID;
                admin.CreatedOn = admin.CreatedOn;
                admin.RepeatID = admin.RepeatID;
                admin.CreatedOnString = Helper.GetDateString(admin.CreatedOn, "MM/dd/yyyy");
                admin.RepeatInterval = admin.RepeatInterval;
                admin.ScheduleDate = admin.ScheduleDate;
                admin.SlotID = admin.SlotID;
                admin.SlotName = admin.SlotName;
                admin.SurveyID = admin.SurveyID;
                admin.SurveyName = admin.SurveyName;
                admin.Time = admin.Time;
                return admin;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["SurveyPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Backs to survey schedule.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToSurveySchedule()
        {
            ScheduleGameSurveyViewModel model = new ScheduleGameSurveyViewModel();
            if (HttpContext.Session["SurveyPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["SurveyPagingCriteria"].ToString().Split('|');
                model.ScheduleSurveyListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.ScheduleSurveyListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.ScheduleSurveyListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.ScheduleSurveyListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.ScheduleSurveyListViewModel.SortPageOptions.CurrentPage = 1;
                model.ScheduleSurveyListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.ScheduleSurveyListViewModel.SortPageOptions.SortField = "SurveyName";
                model.ScheduleSurveyListViewModel.SortPageOptions.SortOrder = "asc";
            }
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            response.AdminId = loggedInUserId;
            model.ScheduleSurveyListViewModel.LoggedInAdminId = loggedInUserId;
            response.ScheduleSurveyListViewModel = _userService.GetSurveyScheduledList(model.ScheduleSurveyListViewModel);
            var newresponse = response.ScheduleSurveyListViewModel;
            GetSurveyScheduleResponse(model.ScheduleSurveyListViewModel, ref newresponse);
            response.ScheduleSurveyListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View("ManageSchedule", response);
        }

        /// <summary>
        /// Backs to game schedule.
        /// </summary>
        /// <returns></returns>
        public ActionResult BackToGameSchedule()
        {
            ScheduleGameSurveyViewModel model = new ScheduleGameSurveyViewModel();
            if (HttpContext.Session["GamePagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["GamePagingCriteria"].ToString().Split('|');
                model.ScheduleGameListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.ScheduleGameListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.ScheduleGameListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.ScheduleGameListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.ScheduleGameListViewModel.SortPageOptions.CurrentPage = 1;
                model.ScheduleGameListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.ScheduleGameListViewModel.SortPageOptions.SortField = "GameName";
                model.ScheduleGameListViewModel.SortPageOptions.SortOrder = "asc";
            }
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            response.AdminId = loggedInUserId;
            model.ScheduleGameListViewModel.LoggedInAdminId = loggedInUserId;
            response.ScheduleGameListViewModel = _userService.GetGameScheduledList(model.ScheduleGameListViewModel);
            var newresponse = response.ScheduleGameListViewModel;
            GetGameScheduleResponse(model.ScheduleGameListViewModel, ref newresponse);
            response.ScheduleGameListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            return View("ManageSchedule", response);
        }

        /// <summary>
        /// Search the admin.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchScheduleSurvey(ScheduleSurveyListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new ScheduleSurveyListViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "SurveyName";
                model.SortPageOptions.SortOrder = "asc";
            }
            model.LoggedInAdminId = loggedInUserId;
            if (searchString == null || searchString.Trim().Length == 0)
                searchString = model.SearchId;
            if (searchString != null && searchString.Trim().Length > 0)
            {
                model.SearchId = searchString;
                response = _userService.GetSurveyScheduledList(model);
            }
            else
            { response = _userService.GetSurveyScheduledList(model); }

            GetSurveyScheduleResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_ADMIN_MESSAGE);
            response.SearchId = searchString;
            ModelState.Clear();
            ScheduleGameSurveyViewModel newResponse = new ScheduleGameSurveyViewModel();
            newResponse.ScheduleSurveyListViewModel = response;
            newResponse.AdminId = loggedInUserId;
            return PartialView("_ScheduleSurveyListPartial", newResponse);
        }

        /// <summary>
        /// Search the admin.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="command">The command.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchScheduleGame(ScheduleGameListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new ScheduleGameListViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "GameName";
                model.SortPageOptions.SortOrder = "asc";
            }
            model.LoggedInAdminId = loggedInUserId;
            if (searchString == null || searchString.Trim().Length == 0)
                searchString = model.SearchId;
            if (searchString != null && searchString.Trim().Length > 0)
            {
                model.SearchId = searchString;
                response = _userService.GetGameScheduledList(model);
            }
            else
            {
                response = _userService.GetGameScheduledList(model);
            }
            GetGameScheduleResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_ADMIN_MESSAGE);
            response.SearchId = searchString;
            ModelState.Clear();
            ScheduleGameSurveyViewModel newResponse = new ScheduleGameSurveyViewModel();
            newResponse.ScheduleGameListViewModel = response;
            newResponse.AdminId = loggedInUserId;
            return PartialView("_ScheduleGameListPartial", newResponse);
        }

        /// <summary>
        /// Search in the batch list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="command">command</param>
        /// <param name="searchString">searchString</param>
        /// <param name="sortColumn">sortColumn</param>
        /// <param name="sortOrder">sortOrder</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="page">page</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchScheduleBatch(ScheduleListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new ScheduleListViewModel();
            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "BatchName";
                model.SortPageOptions.SortOrder = "asc";
            }
            model.LoggedInAdminId = loggedInUserId;
            if (searchString == null || searchString.Trim().Length == 0)
                searchString = model.SearchId;
            if (searchString != null && searchString.Trim().Length > 0)
            {
                model.SearchId = searchString;
                response = _sheduleService.GetBatchScheduledList(model);        
            }
            else
            {
                response = _sheduleService.GetBatchScheduledList(model);
            }
            GetBatchScheduleResponse(model, ref response);
            response.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_ADMIN_MESSAGE);
            response.SearchId = searchString;
            ModelState.Clear();
            ScheduleGameSurveyViewModel newResponse = new ScheduleGameSurveyViewModel();
            newResponse.ScheduleBatchListViewModel = response;
            newResponse.AdminId = loggedInUserId;
            return PartialView("_ScheduleBatchListPartial", newResponse);
        }


        /// <summary>
        /// sorting and paging user list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="response">user list</param>
        private void GetGameScheduleResponse(ScheduleGameListViewModel model, ref ScheduleGameListViewModel response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "GameName";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "GameName" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;   
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.AdminCTestScheduleViewModelList.ToPagedList<AdminCTestScheduleViewModel>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedAdminCTestScheduleViewModelList = new StaticPagedList<AdminCTestScheduleViewModel>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.AdminCTestScheduleViewModelList.Count);
            response.AdminCTestScheduleViewModelList = response.AdminCTestScheduleViewModelList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.AdminCTestScheduleViewModelList = response.AdminCTestScheduleViewModelList.Take(_SortPageOptions.PageSize).ToList();
            response.AdminCTestScheduleViewModelList.Select(admin =>
            {
                admin.AdminID = admin.AdminID;
                admin.AdminCTestSchID = admin.AdminCTestSchID;
                admin.CreatedOn = admin.CreatedOn;
                admin.RepeatID = admin.RepeatID;
                admin.CreatedOnString = Helper.GetDateString(admin.CreatedOn, "MM/dd/yyyy");
                admin.RepeatInterval = admin.RepeatInterval;
                admin.ScheduleDate = admin.ScheduleDate;
                admin.SlotID = admin.SlotID;
                admin.SlotName = admin.SlotName;
                admin.CTestID = admin.CTestID;
                admin.CTestName = admin.CTestName;
                admin.Version = admin.Version;
                admin.Time = admin.Time;
                return admin;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["GamePagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Get Batch Schedule Response
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="response">response</param>
        private void GetBatchScheduleResponse(ScheduleListViewModel model, ref ScheduleListViewModel response)
        {
            response.SearchId = ((model.SearchId == null || model.SearchId.Length == 0) ? "" : model.SearchId);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "BatchName";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "GameName" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;    
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.AdminScheduleViewModelList.ToPagedList<AdminScheduleViewModel>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedAdminScheduleViewModelList = new StaticPagedList<AdminScheduleViewModel>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.AdminScheduleViewModelList.Count);
            response.AdminScheduleViewModelList = response.AdminScheduleViewModelList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.AdminScheduleViewModelList = response.AdminScheduleViewModelList.Take(_SortPageOptions.PageSize).ToList();
            response.AdminScheduleViewModelList.Select(admin =>
            {
                admin.BatchName = admin.BatchName;
                admin.AdminID = admin.AdminID;
                admin.AdminSchID = admin.AdminSchID;
                admin.CreatedOn = admin.CreatedOn;
                admin.RepeatID = admin.RepeatID;
                admin.CreatedOnString = Helper.GetDateString(admin.CreatedOn, "MM/dd/yyyy");
                admin.RepeatInterval = admin.RepeatInterval;
                admin.ScheduleDate = admin.ScheduleDate;
                admin.SlotID = admin.SlotID;
                admin.SlotName = admin.SlotName;
                admin.Version = admin.Version;
                admin.Time = admin.Time;
                return admin;
            }).ToList();
            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["BatchPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

       /// <summary>
        /// Delete Survey Schedule
       /// </summary>
        /// <param name="surveyId">surveyId</param>
       /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSurveySchedule(long surveyId)
        {
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            response.ScheduleSurveyListViewModel = _userService.DeleteSurveySchedule(surveyId);
            response.AdminId = loggedInUserId;
            return RedirectToAction("BackToSurveySchedule");
        }

        /// <summary>
        /// Delete Game Schedule
        /// </summary>
        /// <param name="cTestId">cTestId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteGameSchedule(long cTestId)
        {
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            response.ScheduleGameListViewModel = _userService.DeleteGameSchedule(cTestId);
            response.AdminId = loggedInUserId;
            return RedirectToAction("BackToGameSchedule");
        }

        /// <summary>
        /// Delete Batch Schedule
        /// </summary>
        /// <param name="batchId">batchId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBatchSchedule(long batchId)
        {
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            response.ScheduleBatchListViewModel = _userService.DeleteBatchSchedule(batchId);
            response.AdminId = loggedInUserId;
            return RedirectToAction("BackToGameSchedule");
        }

        /// <summary>
        /// EditGameSchedule
        /// </summary>
        /// <param name="adminCTestSchID">adminCTestSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditGameSchedule(long adminCTestSchID)
        {
            ScheduleGameSurveyViewModel model = new ScheduleGameSurveyViewModel();
            model.AdminCTestSchID = adminCTestSchID;
            model.LoggedInUserId = loggedInUserId;
            model = _userService.GetGameScheduleDetailsByAdminCTestSchID(model);
            return View("AdminScheduleGame", model);
        }

        /// <summary>
        /// EditSurveySchedule
        /// </summary>
        /// <param name="adminSurveySchID">adminSurveySchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditSurveySchedule(long adminSurveySchID)
        {
            ScheduleGameSurveyViewModel model = new ScheduleGameSurveyViewModel();
            model.AdminSurveySchID = adminSurveySchID;
            model.LoggedInUserId = loggedInUserId;
            model = _userService.GetSurveyScheduleDetailsByAdminSurveySchID(model);
            return View("AdminScheduleSurvey", model);
        }

        /// <summary>
        /// EditBatchSchedule
        /// </summary>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditBatchSchedule(long adminBatchSchID)
        {
            ScheduleGameSurveyViewModel model = new ScheduleGameSurveyViewModel();
            model.AdminBatchSchID = adminBatchSchID;
            model.LoggedInUserId = loggedInUserId;
            model = _userService.GetBatchScheduleDetailsByAdminBatchSchID(model);
            return View("AdminScheduleBatch", model);
        }

        /// <summary>
        /// Manage Schedule
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public ActionResult ManageSchedule(ScheduleGameSurveyViewModel model)
        {
            if (model == null)
                model = new ScheduleGameSurveyViewModel();
            if (HttpContext.Session["SurveyPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["SurveyPagingCriteria"].ToString().Split('|');
                model.ScheduleSurveyListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.ScheduleSurveyListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.ScheduleSurveyListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.ScheduleSurveyListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.ScheduleSurveyListViewModel.SortPageOptions.CurrentPage = 1;
                model.ScheduleSurveyListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.ScheduleSurveyListViewModel.SortPageOptions.SortField = "SurveyName";
                model.ScheduleSurveyListViewModel.SortPageOptions.SortOrder = "asc";
            }
            if (HttpContext.Session["GamePagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["GamePagingCriteria"].ToString().Split('|');
                model.ScheduleGameListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.ScheduleGameListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.ScheduleGameListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.ScheduleGameListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.ScheduleGameListViewModel.SortPageOptions.CurrentPage = 1;
                model.ScheduleGameListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.ScheduleGameListViewModel.SortPageOptions.SortField = "GameName";
                model.ScheduleGameListViewModel.SortPageOptions.SortOrder = "asc";
            }
            if (HttpContext.Session["BatchPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["BatchPagingCriteria"].ToString().Split('|');
                model.ScheduleBatchListViewModel.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                model.ScheduleBatchListViewModel.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                model.ScheduleBatchListViewModel.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                model.ScheduleBatchListViewModel.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }
            else
            {
                model.ScheduleBatchListViewModel.SortPageOptions.CurrentPage = 1;
                model.ScheduleBatchListViewModel.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.ScheduleBatchListViewModel.SortPageOptions.SortField = "BatchName";
                model.ScheduleBatchListViewModel.SortPageOptions.SortOrder = "asc";
            }
            ScheduleGameSurveyViewModel response = new ScheduleGameSurveyViewModel();
            model.AdminId = loggedInUserId;
            model.ScheduleSurveyListViewModel.LoggedInAdminId = loggedInUserId;
            response.ScheduleSurveyListViewModel = _userService.GetSurveyScheduledList(model.ScheduleSurveyListViewModel);
            var surveyNewResponse = response.ScheduleSurveyListViewModel;
            GetSurveyScheduleResponse(model.ScheduleSurveyListViewModel, ref surveyNewResponse);
            response.ScheduleSurveyListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            model.ScheduleSurveyListViewModel = surveyNewResponse;

            model.ScheduleGameListViewModel.LoggedInAdminId = loggedInUserId;
            response.ScheduleGameListViewModel = _userService.GetGameScheduledList(model.ScheduleGameListViewModel);
            var gameNewResponse = response.ScheduleGameListViewModel;
            GetGameScheduleResponse(model.ScheduleGameListViewModel, ref gameNewResponse);
            response.ScheduleGameListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            model.ScheduleGameListViewModel = gameNewResponse;


            model.ScheduleBatchListViewModel.LoggedInAdminId = loggedInUserId;
            response.ScheduleBatchListViewModel = _sheduleService.GetBatchScheduledList(model.ScheduleBatchListViewModel);
            var batchNewResponse = response.ScheduleBatchListViewModel;
            GetBatchScheduleResponse(model.ScheduleBatchListViewModel, ref batchNewResponse);
            response.ScheduleBatchListViewModel.UnregisteredUserMessage = ResourceHelper.GetStringResource(LAMPConstants.UN_REGISTERED_USER_MESSAGE);
            ModelState.Clear();
            model.ScheduleBatchListViewModel = batchNewResponse;
            return View(model);
        }

        /// <summary>
        /// Admins the schedule game.
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminScheduleGame()
        {
            ScheduleGameSurveyViewModel scheduleViewModel = new ScheduleGameSurveyViewModel();
            scheduleViewModel.LoggedInUserId = loggedInUserId;
            scheduleViewModel.AdminId = loggedInUserId;
            scheduleViewModel = _userService.GetScheduleViewModelDetailsForAdmin(scheduleViewModel);
            return View(scheduleViewModel);
        }

        /// <summary>
        /// Saves the schedule game.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdminScheduleGame(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)
        {
            if (ScheduleGameSurveyViewModel.OptionsArray != null)
            {
                var options = JArray.Parse(ScheduleGameSurveyViewModel.OptionsArray).ToObject<List<String>>();
                ScheduleGameSurveyViewModel.OptionsStringList = options;
            }
            ScheduleGameSurveyViewModel.LoggedInUserId = loggedInUserId;
            ScheduleGameSurveyViewModel.AdminId = loggedInUserId;
            ScheduleGameSurveyViewModel = _userService.SaveSheduleGame(ScheduleGameSurveyViewModel);
            TempData["GameMessage"] = ScheduleGameSurveyViewModel.Message;
            TempData["GameStatus"] = ScheduleGameSurveyViewModel.Status;
            TempData["IsSaved"] = ScheduleGameSurveyViewModel.IsSaved;
            return View("AdminScheduleGame", ScheduleGameSurveyViewModel);
        }

        /// <summary>
        /// Admins the schedule game.
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminScheduleSurvey()
        {
            ScheduleGameSurveyViewModel scheduleViewModel = new ScheduleGameSurveyViewModel();
            scheduleViewModel.LoggedInUserId = loggedInUserId;
            scheduleViewModel.AdminId = loggedInUserId;
            scheduleViewModel = _userService.GetScheduleViewModelDetailsForAdmin(scheduleViewModel);
            return View(scheduleViewModel);
        }

        /// <summary>
        /// Saves the shedule survey and game.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdminScheduleSurvey(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)
        {
            if (ScheduleGameSurveyViewModel.OptionsArray != null)
            {
                var options = JArray.Parse(ScheduleGameSurveyViewModel.OptionsArray).ToObject<List<String>>();
                ScheduleGameSurveyViewModel.OptionsStringList = options;
            }
            ScheduleGameSurveyViewModel.LoggedInUserId = loggedInUserId;
            ScheduleGameSurveyViewModel.AdminId = loggedInUserId;
            ScheduleGameSurveyViewModel = _userService.SaveSheduleSurvey(ScheduleGameSurveyViewModel);
            TempData["SurveyMessage"] = ScheduleGameSurveyViewModel.Message;
            TempData["SurveyStatus"] = ScheduleGameSurveyViewModel.Status;
            TempData["IsSaved"] = ScheduleGameSurveyViewModel.IsSaved;
            return View(ScheduleGameSurveyViewModel);
        }

        /// <summary>
        /// Admin Schedule Batch load
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminScheduleBatch()
        {
            ScheduleGameSurveyViewModel scheduleViewModel = new ScheduleGameSurveyViewModel();
            scheduleViewModel.LoggedInUserId = loggedInUserId;
            scheduleViewModel.AdminId = loggedInUserId;
            ScheduleGameSurveyViewModel request = new ScheduleGameSurveyViewModel();
            request.LoggedInUserId = loggedInUserId;
            request.AdminId = loggedInUserId;
            scheduleViewModel = _userService.GetBatchScheduleDetailsByAdminBatchSchID(request);
            return View(scheduleViewModel);
        }

        /// <summary>
        /// Admin schedule Batch Post
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">ScheduleGameSurveyViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdminScheduleBatch(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)  
        {
            if (ScheduleGameSurveyViewModel.OptionsArray != null)
            {
                var options = JArray.Parse(ScheduleGameSurveyViewModel.OptionsArray).ToObject<List<String>>();
                ScheduleGameSurveyViewModel.OptionsStringList = options;
            }
            ScheduleGameSurveyViewModel.LoggedInUserId = loggedInUserId;
            ScheduleGameSurveyViewModel.AdminId = loggedInUserId;
            ScheduleBatchViewModel request = new ScheduleBatchViewModel();
            request.LoggedInUserId = loggedInUserId;
            request.AdminId = loggedInUserId;
            request.AdminCTestSchID = ScheduleGameSurveyViewModel.AdminCTestSchID;
            request.GameScheduleDateString = ScheduleGameSurveyViewModel.GameScheduleDateString;
            request.CognitionTestSlotId = ScheduleGameSurveyViewModel.CognitionTestSlotId;
            request.AdminBatchSchID = ScheduleGameSurveyViewModel.AdminBatchSchID;
            request.BatchName = ScheduleGameSurveyViewModel.BatchName;
            request.GameScheduleDateString = ScheduleGameSurveyViewModel.GameScheduleDateString;
            request.CognitionTestRepeatId = ScheduleGameSurveyViewModel.CognitionTestRepeatId;
            request.BatchSlotTimeString = ScheduleGameSurveyViewModel.BatchSlotTimeString;

            //BatchSurvey CognitionTest
            List<BatchSurvey> surveyList = new List<BatchSurvey>();
            List<BatchCognitionTest> gameList = new List<BatchCognitionTest>();

            BatchSurvey bObj;
            BatchCognitionTest cObj;
            if (ScheduleGameSurveyViewModel.BatchSurveyGames != null && ScheduleGameSurveyViewModel.BatchSurveyGames.Length > 0)
            {
                string[] batchSurveyGames = ScheduleGameSurveyViewModel.BatchSurveyGames.Split('|');
                //type:survey/ctest Id:version
                Int16 orderVal = 1;
                foreach (string batchSurveyGame in batchSurveyGames)
                {
                    if (batchSurveyGame != "undefined" && batchSurveyGame.Trim().Length > 0)
                    {
                        string[] _batchSurveyGame = batchSurveyGame.Split(':');
                        if (_batchSurveyGame[0] == "1")
                        {
                            //1: Survey
                            bObj = new BatchSurvey();
                            bObj.SurveyId = Convert.ToInt64(_batchSurveyGame[1]);
                            bObj.Order = Convert.ToInt16(orderVal);
                            surveyList.Add(bObj);
                        }
                        else
                        {
                            //2: Ctest
                            cObj = new BatchCognitionTest();
                            cObj.CognitionTestId = Convert.ToInt64(_batchSurveyGame[1]);
                            cObj.CognitionVersionId = Convert.ToInt64(_batchSurveyGame[2]);
                            cObj.Order = Convert.ToInt16(orderVal);
                            gameList.Add(cObj);
                        }
                        orderVal++;
                    }
                }

            }
            request.BatchSurvey = surveyList;
            request.CognitionTest = gameList;
            request.SurveyRepeatId = ScheduleGameSurveyViewModel.BatchRepeatId;
            if (ScheduleGameSurveyViewModel.BatchRepeatId == 11)
            {
                if (ScheduleGameSurveyViewModel.OptionsArray != null)
                {
                    var options = JArray.Parse(ScheduleGameSurveyViewModel.OptionsArray).ToObject<List<String>>();
                    request.OptionsStringList = options;
                }
                request.OptionsStringList = ScheduleGameSurveyViewModel.OptionsStringList;
            }
            else if ((ScheduleGameSurveyViewModel.BatchRepeatId > 4 && ScheduleGameSurveyViewModel.BatchRepeatId < 11) || ScheduleGameSurveyViewModel.BatchRepeatId == 12)
            {
                List<string> options = new List<string>();
                options.Add(ScheduleGameSurveyViewModel.CognitionTestSlotTimeString);
                request.OptionsStringList = options;
            }

            ScheduleGameSurveyViewModel = _sheduleService.SaveBatchShedule(request);
            TempData["BatchMessage"] = ScheduleGameSurveyViewModel.Message;
            TempData["BatchStatus"] = ScheduleGameSurveyViewModel.Status;
            TempData["IsSaved"] = ScheduleGameSurveyViewModel.IsSaved;
            return View(ScheduleGameSurveyViewModel);
        }

        /// <summary>
        /// Gets the cognition version.
        /// </summary>
        /// <param name="cognitionId">The cognition identifier.</param>
        /// <returns></returns>
        public JsonResult GetCognitionVersion(int cognitionId)
        {
            var versions = _userService.GetCognitionVersion(cognitionId);
            return Json(versions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region User Activities

        /// <summary>
        /// Get the clinicalPofile of the user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>File path</returns>
        [HttpGet]
        public ActionResult Download(long userId)
        {
            UserViewModel model = new UserViewModel();
            model = _userService.GetUserDetails(userId);
            string fileName = model.ClinicalProfileURL;
            var filepath = System.Web.Hosting.HostingEnvironment.MapPath(LAMPConstants.CLINICAL_PROFILE_PATH) + "/" + fileName;
            if (System.IO.File.Exists(filepath))
            {
                return File(filepath, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return RedirectToAction("BackToUsers", "UserAdmin");
            }
        }

        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="Status">Status</param>
        /// <returns>User list</returns>
        [HttpPost]
        public ActionResult ChangeUserStatus(long userId, string Status)
        {
            AdminandUserListViewModel response = new AdminandUserListViewModel();
            response.UserListViewModel = _userService.ChangeUserStatus(userId, Convert.ToBoolean(Status));
            ModelState.Clear();
            return View("Users", response);
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User details</returns>
        [HttpGet]
        public ActionResult UserProfile(long userId)
        {
            UserViewModel model = new UserViewModel();
            model = _userService.GetUserDetails(userId);
            return View("UserProfile", model);
        }

        /// <summary>
        /// Backs to user activities.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="studyId">The study identifier.</param>
        /// <returns></returns>
        public ActionResult BackToUserActivities(long userId, string studyId)
        {
            Int16 surveyCurrentPage = 1;
            if (HttpContext.Session["UserActivitySurveyPagingCriteria"] != null)
            {
                string[] pageConditions = HttpContext.Session["UserActivitySurveyPagingCriteria"].ToString().Split('|');
                surveyCurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
            }

            UserActivitiesViewModel response = new UserActivitiesViewModel();
            response = GetUserActivities(userId, studyId, surveyCurrentPage);
            if (response.Status == LAMPConstants.ERROR_CODE)
                return RedirectToAction("Index", "Account");
            else
                return View("UserActivities", response);
        }

        /// <summary>
        /// Get User Activities
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="studyId">Study Id</param>
        /// <returns>User Activities</returns>
        [HttpGet]
        public ActionResult UserActivities(long userId, string studyId)
        {
            UserActivitiesViewModel response = new UserActivitiesViewModel();
            response = GetUserActivities(userId, studyId, 1);
            if (response.Status == LAMPConstants.ERROR_CODE)
                return RedirectToAction("Index", "Account");
            else
                return View(response);
        }

        /// <summary>
        /// Gets the user activities.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="studyId">The study identifier.</param>
        /// <param name="surveyCurrentPage">The survey current page.</param>
        /// <returns></returns>
        private UserActivitiesViewModel GetUserActivities(long userId, string studyId, Int16 surveyCurrentPage)
        {
            LogUtil.Info("UserActivities: " + userId);
            // Cognition Paging
            UserActivitiesViewModel response = new UserActivitiesViewModel();
            response = _userService.GetUserActivities(userId);
            if (response.Status != LAMPConstants.SUCCESS_CODE)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.Abandon();
                HttpContext.Session.RemoveAll();
                HttpContext.GetOwinContext().Authentication.SignOut();
                response.Status = LAMPConstants.ERROR_CODE;
                return response;
            }
            response.StudyId = studyId;
            // Survey Paging
            response.SurveyListSortPageOptions.CurrentPage = surveyCurrentPage;
            response.SurveyListSortPageOptions.PageSize = 2;
            if (response.SurveyList.UserSurveyList != null)
            {
                var providerSurveys = response.SurveyList.UserSurveyList.ToPagedList<UserSurvey>((int)response.SurveyListSortPageOptions.CurrentPage, (int)response.SurveyListSortPageOptions.PageSize);
                response.PagedSurveyList = new StaticPagedList<UserSurvey>(providerSurveys, (int)response.SurveyListSortPageOptions.CurrentPage, (int)response.SurveyListSortPageOptions.PageSize, response.SurveyList.UserSurveyList.Count);
                response.SurveyList.UserSurveyList = response.SurveyList.UserSurveyList.Skip(((int)surveyCurrentPage - 1) * (int)response.SurveyListSortPageOptions.PageSize).ToList();
                response.SurveyList.UserSurveyList = response.SurveyList.UserSurveyList.Take((int)response.SurveyListSortPageOptions.PageSize).ToList();
            }
            // Call history Paging
            response.CallHistorySortPageOptions.CurrentPage = 1;
            response.CallHistorySortPageOptions.PageSize = 2;
            if (response.CallHistoryList != null)
            {
                var providerCalls = response.CallHistoryList.ToPagedList<UserCallHistory>((int)response.CallHistorySortPageOptions.CurrentPage, (int)response.CallHistorySortPageOptions.PageSize);
                response.PagedCallList = new StaticPagedList<UserCallHistory>(providerCalls, (int)response.CallHistorySortPageOptions.CurrentPage, (int)response.CallHistorySortPageOptions.PageSize, response.CallHistoryList.Count);
                response.CallHistoryList = response.CallHistoryList.Skip(0).ToList();
                response.CallHistoryList = response.CallHistoryList.Take(2).ToList();
            }
            // Location Paging
            response.LocationSortPageOptions.CurrentPage = 1;
            response.LocationSortPageOptions.PageSize = 2;
            if (response._LocationList != null)
            {
                var providerLocations = response._LocationList.ToPagedList<UserEnvironment>((int)response.LocationSortPageOptions.CurrentPage, (int)response.LocationSortPageOptions.PageSize);
                response.PagedLocationList = new StaticPagedList<UserEnvironment>(providerLocations, (int)response.LocationSortPageOptions.CurrentPage, (int)response.LocationSortPageOptions.PageSize, response._LocationList.Count);
                response._LocationList = response._LocationList.Skip(0).ToList();
                response._LocationList = response._LocationList.Take(2).ToList();
            }
            // Enviroment Paging
            response.EnvironmentSortPageOptions.CurrentPage = 1;
            response.EnvironmentSortPageOptions.PageSize = 2;
            if (response._EnvironmentList != null)
            {
                var providerEnvironments = response._EnvironmentList.ToPagedList<UserEnvironment>((int)response.EnvironmentSortPageOptions.CurrentPage, (int)response.EnvironmentSortPageOptions.PageSize);
                response.PagedEnvironmentList = new StaticPagedList<UserEnvironment>(providerEnvironments, (int)response.EnvironmentSortPageOptions.CurrentPage, (int)response.EnvironmentSortPageOptions.PageSize, response._EnvironmentList.Count);
                response._EnvironmentList = response._EnvironmentList.Skip(0).ToList();
                response._EnvironmentList = response._EnvironmentList.Take(2).ToList();
            }
            return response;
        }
                
        /// <summary>
        /// Search Survey List
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="page">page</param>
        /// <param name="studyId">studyId</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchSurveyList(long userId, int? pageSize, int? page, string studyId)
        {
            var response = new UserActivitiesViewModel();
            response = _userService.GetUserActivities(userId);
            response.StudyId = studyId;
            var providerSurveys = response.SurveyList.UserSurveyList.ToPagedList<UserSurvey>((int)page, (int)pageSize);
            response.PagedSurveyList = new StaticPagedList<UserSurvey>(providerSurveys, (int)page, (int)pageSize, response.SurveyList.UserSurveyList.Count);
            response.SurveyList.UserSurveyList = response.SurveyList.UserSurveyList.Skip(((int)page - 1) * (int)pageSize).ToList();
            response.SurveyList.UserSurveyList = response.SurveyList.UserSurveyList.Take((int)pageSize).ToList();
            response.SurveyListSortPageOptions.PageSize = (Int16)pageSize;
            response.SurveyListSortPageOptions.CurrentPage = (Int16)page;
            ModelState.Clear();
            return PartialView("_SurveyListPartial", response);
        }

        /// <summary>
        /// Get location list
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <returns>Location list</returns>
        [HttpGet]
        public ActionResult SearchLocationList(long userId, int? pageSize, int? page)
        {
            var response = new UserActivitiesViewModel();
            response = _userService.GetUserActivities(userId);
            var providerLocations = response._LocationList.ToPagedList<UserEnvironment>((int)page, (int)pageSize);
            response.PagedLocationList = new StaticPagedList<UserEnvironment>(providerLocations, (int)page, (int)pageSize, response._LocationList.Count);
            response._LocationList = response._LocationList.Skip(((int)page - 1) * (int)pageSize).ToList();
            response._LocationList = response._LocationList.Take((int)pageSize).ToList();
            response.LocationSortPageOptions.PageSize = (Int16)pageSize;
            ModelState.Clear();
            return PartialView("_LocationListPartial", response);
        }

        /// <summary>
        /// Get environment list
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <returns>Location list</returns>
        [HttpGet]
        public ActionResult SearchEnvironmentList(long userId, int? pageSize, int? page)
        {
            var response = new UserActivitiesViewModel();
            response = _userService.GetUserActivities(userId);
            var providerEnvironments = response._EnvironmentList.ToPagedList<UserEnvironment>((int)page, (int)pageSize);
            response.PagedEnvironmentList = new StaticPagedList<UserEnvironment>(providerEnvironments, (int)page, (int)pageSize, response._EnvironmentList.Count);
            response._EnvironmentList = response._EnvironmentList.Skip(((int)page - 1) * (int)pageSize).ToList();
            response._EnvironmentList = response._EnvironmentList.Take((int)pageSize).ToList();
            response.EnvironmentSortPageOptions.PageSize = (Int16)pageSize;
            ModelState.Clear();
            return PartialView("_EnvironmentListPartial", response);
        }
        /// <summary>
        /// Get call list
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <returns>Call list</returns>
        [HttpGet]
        public ActionResult SearchCallList(long userId, int? pageSize, int? page)
        {
            var response = new UserActivitiesViewModel();
            response = _userService.GetUserActivities(userId);
            var providerCalls = response.CallHistoryList.ToPagedList<UserCallHistory>((int)page, (int)pageSize);
            response.PagedCallList = new StaticPagedList<UserCallHistory>(providerCalls, (int)page, (int)pageSize, response.CallHistoryList.Count);
            response.CallHistoryList = response.CallHistoryList.Skip(((int)page - 1) * (int)pageSize).ToList();
            response.CallHistoryList = response.CallHistoryList.Take((int)pageSize).ToList();
            response.CallHistorySortPageOptions.PageSize = (Int16)pageSize;
            ModelState.Clear();
            return PartialView("_CallListPartial", response);
        }

        #endregion

        #region Games

        //******************N_Back************************//       
        /// <summary>
        /// Induce pageing for the NBack details
        /// </summary>
        /// <param name="model">Model object</param>
        /// <param name="response">Paged NBack  details</param>
        private void GetCtest_N_BackResponse(CognitionNBackViewModel model, ref CognitionNBackViewModel response)
        {
            var _SortPageOptions = new CognitionNBackSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestNBack = response.CTest_NBackResultList.ToPagedList<CognitionNBackDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_NBackDetailList = new StaticPagedList<CognitionNBackDetail>(providerCTestNBack, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_NBackResultList.Count);
            response.CTest_NBackResultList = response.CTest_NBackResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_NBackResultList = response.CTest_NBackResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }
        
        /// <summary>
        /// Shows the details of the game NBack for a user
        /// </summary>
        /// <param name="model">NBack object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>NBack details</returns>
        [HttpGet]
        public ActionResult Cognition_N_Back(CognitionNBackViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionNBackViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionNBack(userId, adminBatchSchID);
            GetCtest_N_BackResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_N_BackListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /// <summary>
        /// Gets the ctest n back new response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_N_BackNewResponse(CognitionNBackNewViewModel model, ref CognitionNBackNewViewModel response)
        {
            var _SortPageOptions = new CognitionNBackNewSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestNBack = response.CTest_NBackNewResultList.ToPagedList<CognitionNBackNewDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_NBackNewDetailList = new StaticPagedList<CognitionNBackNewDetail>(providerCTestNBack, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_NBackNewResultList.Count);

            response.CTest_NBackNewResultList = response.CTest_NBackNewResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_NBackNewResultList = response.CTest_NBackNewResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Cognitions the n back new.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_N_BackNew(CognitionNBackNewViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionNBackNewViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionNBackNew(userId, adminBatchSchID);
            GetCtest_N_BackNewResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_N_BackNewListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        //******************************************Cat_Dog        
        /// <summary>
        /// Induce pageing for the CatDog details
        /// </summary>
        /// <param name="model">CatDog object</param>
        /// <param name="response">CatDog details</param>
        private void GetCtest_Cat_DogResponse(CognitionCatDogViewModel model, ref CognitionCatDogViewModel response)
        {
            var _SortPageOptions = new CognitionCatDogSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   // page;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestCatDog = response.CTest_CatDogResultList.ToPagedList<CognitionCatDogDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_CatDogDetailList = new StaticPagedList<CognitionCatDogDetail>(providerCTestCatDog, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_CatDogResultList.Count);
            response.CTest_CatDogResultList = response.CTest_CatDogResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_CatDogResultList = response.CTest_CatDogResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game Cat & Dog for a user
        /// </summary>
        /// <param name="model">Cat & Dog model</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current PAge</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>CatDog game details</returns>
        [HttpGet]
        public ActionResult Cognition_Cat_Dog(CognitionCatDogViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionCatDogViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionCatDog(userId, adminBatchSchID);
            GetCtest_Cat_DogResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_Cat_DogListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        //******************************************Serial7        
        /// <summary>
        /// Induce pageing for the Serial7 details
        /// </summary>
        /// <param name="model">Serial7 object</param>
        /// <param name="response">Paged Serial7 details</param>
        private void GetCtest_Serial7Response(CognitionSerial7ViewModel model, ref CognitionSerial7ViewModel response)
        {
            var _SortPageOptions = new CognitionSerial7SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   // page;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestSerial7 = response.CTest_Serial7ResultList.ToPagedList<CognitionSerial7Detail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_Serial7DetailList = new StaticPagedList<CognitionSerial7Detail>(providerCTestSerial7, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_Serial7ResultList.Count);
            response.CTest_Serial7ResultList = response.CTest_Serial7ResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_Serial7ResultList = response.CTest_Serial7ResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();

        }

        /// <summary>
        /// Shows the details of the game Serial7 for a user
        /// </summary>
        /// <param name="model">Serial7 object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>Serial7 game details</returns>
        [HttpGet]
        public ActionResult Cognition_Serial7(CognitionSerial7ViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionSerial7ViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionSerial7(userId, adminBatchSchID);
            GetCtest_Serial7Response(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_Serial7ListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Simple Memory*********/
        /// <summary>
        /// Induce pageing for the Simple Memory details
        /// </summary>
        /// <param name="model">Simple Memory object</param>
        /// <param name="response">Simple Memory details</param>
        private void GetCtest_Simple_MemoryResponse(CognitionSimpleMemoryViewModel model, ref CognitionSimpleMemoryViewModel response)
        {
            var _SortPageOptions = new CognitionSimpleMemorySortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestSimpleMemory = response.CTest_SimpleMemoryResultList.ToPagedList<CognitionSimpleMemoryDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_SimpleMemoryDetailList = new StaticPagedList<CognitionSimpleMemoryDetail>(providerCTestSimpleMemory, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_SimpleMemoryResultList.Count);
            response.CTest_SimpleMemoryResultList = response.CTest_SimpleMemoryResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_SimpleMemoryResultList = response.CTest_SimpleMemoryResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game Simple Memory for a user
        /// </summary>
        /// <param name="model">Simple Memory object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>Simple Memory details</returns>
        [HttpGet]
        public ActionResult Cognition_Simple_Memory(CognitionSimpleMemoryViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionSimpleMemoryViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionSimpleMemory(userId, adminBatchSchID);
            GetCtest_Simple_MemoryResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_Simple_MemoryListPartial", response);
            }
            else
            {
                return View(response);
            }
        }


        /***************************************************Visual Association*********/
        /// <summary>
        /// Induce pageing for the details
        /// </summary>
        /// <param name="model">VisualAssociation object</param>
        /// <param name="response">Paged Visual Association details</param>
        private void GetCtest_Visual_AssociationResponse(CognitionVisualAssociationViewModel model, ref CognitionVisualAssociationViewModel response)
        {
            var _SortPageOptions = new CognitionVisualAssociationSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestVisualAssociation = response.CTest_VisualAssociationResultList.ToPagedList<CognitionVisualAssociationDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_VisualAssociationDetailList = new StaticPagedList<CognitionVisualAssociationDetail>(providerCTestVisualAssociation, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_VisualAssociationResultList.Count);
            response.CTest_VisualAssociationResultList = response.CTest_VisualAssociationResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_VisualAssociationResultList = response.CTest_VisualAssociationResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game Visual Association for a user
        /// </summary>
        /// <param name="model">Visual Association object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>>Visual Association details</returns>
        [HttpGet]
        public ActionResult Cognition_Visual_Association(CognitionVisualAssociationViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionVisualAssociationViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionVisualAssociation(userId, adminBatchSchID);
            GetCtest_Visual_AssociationResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_Visual_AssociationListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Trails B*********/
        /// <summary>
        /// Induce pageing for the TrailsB details
        /// </summary>
        /// <param name="model">TrailsB object</param>
        /// <param name="response">Paged TrailsB details</param>
        private void GetCtest_TrailsBResponse(CognitionTrailsBViewModel model, ref CognitionTrailsBViewModel response)
        {
            var _SortPageOptions = new CognitionTrailsBSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestTrailsB = response.CTest_TrailsBResultList.ToPagedList<CognitionTrailsBDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_TrailsBDetailList = new StaticPagedList<CognitionTrailsBDetail>(providerCTestTrailsB, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_TrailsBResultList.Count);
            response.CTest_TrailsBResultList = response.CTest_TrailsBResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_TrailsBResultList = response.CTest_TrailsBResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game TrailsB for a user
        /// </summary>
        /// <param name="model">TrailsB object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>TrailsB details</returns>
        [HttpGet]
        public ActionResult Cognition_TrailsB(CognitionTrailsBViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionTrailsBViewModel();

            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionTrailsB(userId, adminBatchSchID);
            GetCtest_TrailsBResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_TrailsBListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Trails B New*********/
        /// <summary>
        /// Induce pageing for the TrailsB New details
        /// </summary>
        /// <param name="model">TrailsB object</param>
        /// <param name="response">Paged TrailsB details</param>
        private void GetCtest_TrailsBNewResponse(CognitionTrailsBNewViewModel model, ref CognitionTrailsBNewViewModel response)
        {
            var _SortPageOptions = new CognitionTrailsBNewSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestTrailsB = response.CTest_TrailsBNewResultList.ToPagedList<CognitionTrailsBNewDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_TrailsBNewDetailList = new StaticPagedList<CognitionTrailsBNewDetail>(providerCTestTrailsB, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_TrailsBNewResultList.Count);
            response.CTest_TrailsBNewResultList = response.CTest_TrailsBNewResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_TrailsBNewResultList = response.CTest_TrailsBNewResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game TrailsB New for a user
        /// </summary>
        /// <param name="model">TrailsB object</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>TrailsB details</returns>
        [HttpGet]
        public ActionResult Cognition_TrailsBNew(CognitionTrailsBNewViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionTrailsBNewViewModel();

            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionTrailsBNew(userId, adminBatchSchID);
            GetCtest_TrailsBNewResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_TrailsBNewListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Trails B Dot Touch*********/

        /// <summary>
        /// Gets the ctest trails b dot touch response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_TrailsBDotTouchResponse(CognitionTrailsBDotTouchViewModel model, ref CognitionTrailsBDotTouchViewModel response)
        {
            var _SortPageOptions = new CognitionTrailsBDotTouchSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestTrailsBDotTouch = response.CTest_TrailsBDotTouchResultList.ToPagedList<CognitionTrailsBDotTouchDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_TrailsBDotTouchDetailList = new StaticPagedList<CognitionTrailsBDotTouchDetail>(providerCTestTrailsBDotTouch, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_TrailsBDotTouchResultList.Count);
            response.CTest_TrailsBDotTouchResultList = response.CTest_TrailsBDotTouchResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_TrailsBDotTouchResultList = response.CTest_TrailsBDotTouchResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Cognitions the trails b dot touch.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_TrailsBDotTouch(CognitionTrailsBDotTouchViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionTrailsBDotTouchViewModel();

            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionTrailsBDotTouch(userId, adminBatchSchID);
            GetCtest_TrailsBDotTouchResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_TrailsBDotTouchListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Jewels Trails A*************/

        /// <summary>
        /// Gets the ctest jewels trails a response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_JewelsTrailsAResponse(CognitionJewelsTrailsAViewModel model, ref CognitionJewelsTrailsAViewModel response)
        {
            var _SortPageOptions = new CognitionJewelsTrailsASortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestJewelsTrailsA = response.CTest_JewelsTrailsAResultList.ToPagedList<CognitionJewelsTrailsADetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_CognitionJewelsTrailsADetailList = new StaticPagedList<CognitionJewelsTrailsADetail>(providerCTestJewelsTrailsA, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_JewelsTrailsAResultList.Count);
            response.CTest_JewelsTrailsAResultList = response.CTest_JewelsTrailsAResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_JewelsTrailsAResultList = response.CTest_JewelsTrailsAResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Cognitions the jewels trails a.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_JewelsTrailsA(CognitionJewelsTrailsAViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionJewelsTrailsAViewModel();

            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionJewelsTrailsA(userId, adminBatchSchID);
            GetCtest_JewelsTrailsAResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_JewelsTrailsAListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Jewels Trails B*************/

        /// <summary>
        /// Gets the ctest jewels trails b response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_JewelsTrailsBResponse(CognitionJewelsTrailsBViewModel model, ref CognitionJewelsTrailsBViewModel response)
        {
            var _SortPageOptions = new CognitionJewelsTrailsBSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestJewelsTrailsB = response.CTest_JewelsTrailsBResultList.ToPagedList<CognitionJewelsTrailsBDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_CognitionJewelsTrailsBDetailList = new StaticPagedList<CognitionJewelsTrailsBDetail>(providerCTestJewelsTrailsB, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_JewelsTrailsBResultList.Count);
            response.CTest_JewelsTrailsBResultList = response.CTest_JewelsTrailsBResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_JewelsTrailsBResultList = response.CTest_JewelsTrailsBResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Cognitions the jewels trails b.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        public ActionResult Cognition_JewelsTrailsB(CognitionJewelsTrailsBViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionJewelsTrailsBViewModel();

            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionJewelsTrailsB(userId, adminBatchSchID);
            GetCtest_JewelsTrailsBResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_JewelsTrailsBListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Digit Span*********/
        /// <summary>
        /// Induce pageing for DigitSpan details
        /// </summary>
        /// <param name="model">Digit Span object</param>
        /// <param name="response">Paged Digit Span details</param>
        private void GetCtest_DigitSpanResponse(CognitionDigitSpanViewModel model, ref CognitionDigitSpanViewModel response)
        {
            var _SortPageOptions = new CognitionDigitSpanSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestDigitSpan = response.CTest_DigitSpanResultList.ToPagedList<CognitionDigitSpanDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_DigitSpanDetailList = new StaticPagedList<CognitionDigitSpanDetail>(providerCTestDigitSpan, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_DigitSpanResultList.Count);
            response.CTest_DigitSpanResultList = response.CTest_DigitSpanResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_DigitSpanResultList = response.CTest_DigitSpanResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game Digit Span for a user
        /// </summary>
        /// <param name="model">DigitSpan model</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>Digit Span details</returns>
        [HttpGet]
        public ActionResult Cognition_DigitSpan(CognitionDigitSpanViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionDigitSpanViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionDigitSpan(userId, adminBatchSchID);
            GetCtest_DigitSpanResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_DigitSpanListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /***************************************************Spatial Span*********/
        /// <summary>
        /// Induce pageing for SpatialSpan details
        /// </summary>
        /// <param name="model">Spatial Span object</param>
        /// <param name="response">Paged Spatial Span details</param>
        private void GetCtest_SpatialSpanResponse(CognitionSpatialSpanViewModel model, ref CognitionSpatialSpanViewModel response)
        {
            var _SortPageOptions = new CognitionSpatialSpanSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestSpatialSpan = response.CTest_SpatialSpanResultList.ToPagedList<CognitionSpatialSpanDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_SpatialSpanDetailList = new StaticPagedList<CognitionSpatialSpanDetail>(providerCTestSpatialSpan, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_SpatialSpanResultList.Count);
            response.CTest_SpatialSpanResultList = response.CTest_SpatialSpanResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_SpatialSpanResultList = response.CTest_SpatialSpanResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Shows the details of the game Spatial Span for a user
        /// </summary>
        /// <param name="model">SpatialSpan model</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="page">Current Page</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>Spatial Span details</returns>
        [HttpGet]
        public ActionResult Cognition_SpatialSpan(CognitionSpatialSpanViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionSpatialSpanViewModel();
            try
            {
                if (pageSize != 0 && pageSize > 0)
                {
                    model.SortPageOptions.CurrentPage = (short)page;
                    model.SortPageOptions.PageSize = (short)pageSize;
                }
                else
                {
                    model.SortPageOptions.CurrentPage = 1;
                    model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                }
                response = _userService.GetCognitionSpatialSpan(userId, adminBatchSchID);
                GetCtest_SpatialSpanResponse(model, ref response);
                LogUtil.Error("SpatialSpan 1");
                ModelState.Clear();
                if (pageSize != null && pageSize != 0 && pageSize > 0)
                {
                    return PartialView("_Cognition_SpatialSpanListPartial", response);
                }
                else
                {
                    return View(response);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("SpatialSpan: " + ex);
                return View(response);
            }
        }


        /// <summary>
        /// Cognitions the new cat and dog.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_NewCatAndDog(CognitionCatAndDogNewViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionCatAndDogNewViewModel();
            try
            {
                if (pageSize != 0 && pageSize > 0)
                {
                    model.SortPageOptions.CurrentPage = (short)page;
                    model.SortPageOptions.PageSize = (short)pageSize;
                }
                else
                {
                    model.SortPageOptions.CurrentPage = 1;
                    model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                }
                response = _userService.GetCognitionCatAndDogNew(userId, adminBatchSchID);
                GetCtest_CatAndDogNewResponse(model, ref response);
                ModelState.Clear();
                if (pageSize != null && pageSize != 0 && pageSize > 0)
                {
                    return PartialView("_Cognition_CatAndDogNewListPartial", response);
                }
                else
                {
                    return View(response);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("SpatialSpan: " + ex);
                return View(response);
            }
        }

        /// <summary>
        /// Gets the ctest cat and dog new response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_CatAndDogNewResponse(CognitionCatAndDogNewViewModel model, ref CognitionCatAndDogNewViewModel response)
        {
            var _SortPageOptions = new CatAndDogNewDetailSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestTemporalOrder = response.CatAndDogNewGameList.ToPagedList<CatAndDogNewDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.CatAndDogNewGamePagedList = new StaticPagedList<CatAndDogNewDetail>(providerCTestTemporalOrder, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CatAndDogNewGameList.Count);
            response.CatAndDogNewGameList = response.CatAndDogNewGameList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CatAndDogNewGameList = response.CatAndDogNewGameList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Cognitions the temporal order.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_TemporalOrder(CognitionTemporalOrderViewModel model, int? pageSize, int? page, long userId, long adminBatchSchID)
        {
            var response = new CognitionTemporalOrderViewModel();
            try
            {
                if (pageSize != 0 && pageSize > 0)
                {
                    model.SortPageOptions.CurrentPage = (short)page;
                    model.SortPageOptions.PageSize = (short)pageSize;
                }
                else
                {
                    model.SortPageOptions.CurrentPage = 1;
                    model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                }
                response = _userService.GetCognitionTemporalOrder(userId, adminBatchSchID);
                GetCtest_TemporalOrderResponse(model, ref response);
                ModelState.Clear();
                if (pageSize != null && pageSize != 0 && pageSize > 0)
                {
                    return PartialView("_Cognition_TemporalOrderListPartial", response);
                }
                else
                {
                    return View(response);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("SpatialSpan: " + ex);
                return View(response);
            }
        }

        /// <summary>
        /// Gets the ctest temporal order response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_TemporalOrderResponse(CognitionTemporalOrderViewModel model, ref CognitionTemporalOrderViewModel response)
        {
            var _SortPageOptions = new TemporalOrderDetailSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestTemporalOrder = response.TemporalOrderGameList.ToPagedList<TemporalOrderDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.TemporalOrderGamePagedList = new StaticPagedList<TemporalOrderDetail>(providerCTestTemporalOrder, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.TemporalOrderGameList.Count);
            response.TemporalOrderGameList = response.TemporalOrderGameList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.TemporalOrderGameList = response.TemporalOrderGameList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        /// <summary>
        /// Get Cognition Spin Wheel details
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="page">page</param>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_SpinWheel(CognitionSpinWheelViewModel model, int? pageSize, int? page, long userId)
        {
            var response = new CognitionSpinWheelViewModel();
            if (pageSize != 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
            }
            else
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            response = _userService.GetCognitionSpinWheel(userId);
            GetCtest_SpinWheelResponse(model, ref response);
            ModelState.Clear();
            if (pageSize != 0 && pageSize > 0)
            {
                return PartialView("_Cognition_SpinWheelListPartial", response);
            }
            else
            {
                return View(response);
            }
        }

        /// <summary>
        /// Gets the ctest n back new response.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        private void GetCtest_SpinWheelResponse(CognitionSpinWheelViewModel model, ref CognitionSpinWheelViewModel response)
        {
            var _SortPageOptions = new CognitionSortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;
            var providerCTestNBack = response.CTest_SpinWheelResultList.ToPagedList<CognitionSpinWheelDetail>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedCTest_SpinWheelDetailList = new StaticPagedList<CognitionSpinWheelDetail>(providerCTestNBack, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.CTest_SpinWheelResultList.Count);
            response.CTest_SpinWheelResultList = response.CTest_SpinWheelResultList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.CTest_SpinWheelResultList = response.CTest_SpinWheelResultList.Take(_SortPageOptions.PageSize).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["CognitionPagingCriteria"] = response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }


        /// <summary>
        /// Shows the details of the game 3D figure for a user
        /// </summary>
        /// <param name="model">3DFigure object</param>
        /// <param name="userId">User Id</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns>3D figure details</returns>
        [HttpGet]
        public ActionResult Cognition_3DFigure(Cognition3DFigureViewModel model, long userId, long adminBatchSchID)
        {
            var response = new Cognition3DFigureViewModel();
            response = _userService.GetCognition3DFigure(userId, adminBatchSchID);
            ModelState.Clear();
            return View(response);
        }

        /// <summary>
        /// Get Cognition Scratch Images
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="userId">userId</param>
        /// <param name="adminBatchSchID">adminBatchSchID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cognition_ScratchImage(CognitionScratchImageViewModel model, long userId, long adminBatchSchID)
        {
            var response = new CognitionScratchImageViewModel();
            response = _userService.GetCognitionScratchImage(userId, adminBatchSchID);
            ModelState.Clear();
            return View(response);
        }

        /// <summary>
        /// Update client offset value
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>Offset</returns>
        [AllowAnonymous]
        [HttpGet]
        public JsonResult UpdateOffsetValue(string offset)
        {
            if (Session["OffsetValue"] == null)
                Session["OffsetValue"] = offset;
            return Json(offset, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }
}