using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using LAMP.ViewModel;
using System.Configuration;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Xml.Linq;



namespace LAMP.Service
{
    /// <summary>
    /// Class UserAdminService
    /// </summary>
    public class UserAdminService : IUserAdminService
    {
        #region Variables

        private IUnitOfWork _UnitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="UnitOfWork"></param>
        public UserAdminService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        #endregion

        #region PublicMethods

        #region AppHelp

        /// <summary>
        /// Gets the application help of logged in admin.
        /// </summary>
        /// <param name="appHelpViewModel">The application help view model.</param>
        /// <returns></returns>
        public AppHelpViewModel GetAppHelpOfLoggedInAdmin(AppHelpViewModel appHelpViewModel)
        {
            AppHelp appHelp = _UnitOfWork.IAppHelpRepository.RetrieveAll().Where(u => u.AdminID == appHelpViewModel.AdminID && u.IsDeleted != true).FirstOrDefault();
            if (appHelp != null)
            {
                appHelpViewModel.HelpText = appHelp.HelpText;
                appHelpViewModel.Content = appHelp.Content;
                appHelpViewModel.HelpID = appHelp.HelpID;
                appHelpViewModel.ImageURL = CryptoUtil.DecryptInfo(appHelp.ImageURL);
                appHelpViewModel.CreatedOn = appHelp.CreatedOn;
                appHelpViewModel.EditedOn = appHelp.EditedOn;
                appHelpViewModel.IsDeleted = appHelp.IsDeleted;
            }
            return appHelpViewModel;
        }

        /// <summary>
        /// Gets the application help details by help identifier.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        public AppHelpViewModel GetAppHelpDetailsByHelpId(long helpId)
        {
            AppHelpViewModel appHelpViewModel = new AppHelpViewModel();
            AppHelp appHelp = _UnitOfWork.IAppHelpRepository.RetrieveAll().Where(u => u.HelpID == helpId).FirstOrDefault();
            if (appHelp != null)
            {
                appHelpViewModel.HelpText = appHelp.HelpText;
                appHelpViewModel.Content = appHelp.Content;
                appHelpViewModel.HelpID = appHelp.HelpID;
                appHelpViewModel.ImageURL = CryptoUtil.DecryptInfo(appHelp.ImageURL);
                appHelpViewModel.CreatedOn = appHelp.CreatedOn;
                appHelpViewModel.EditedOn = appHelp.EditedOn;
                appHelpViewModel.IsDeleted = appHelp.IsDeleted;
                appHelpViewModel.AdminFullName = CryptoUtil.DecryptInfo(appHelp.Admin.FirstName) + " " + CryptoUtil.DecryptInfo(appHelp.Admin.LastName);
                appHelpViewModel.CreatedOnString = Helper.GetDateString(appHelp.CreatedOn, "MM/dd/yyyy");
            }
            return appHelpViewModel;
        }

        /// <summary>
        /// Deletes the application help.
        /// </summary>
        /// <param name="helpId">The help identifier.</param>
        /// <returns></returns>
        public AppHelpViewModelList DeleteAppHelp(long helpId)
        {
            AppHelpViewModelList model = new AppHelpViewModelList();
            var response = new AppHelpViewModelList();
            AppHelp appHelp = _UnitOfWork.IAppHelpRepository.GetById(helpId);
            if (appHelp != null)
            {
                appHelp.IsDeleted = true;
                _UnitOfWork.IAppHelpRepository.Update(appHelp);
                _UnitOfWork.Commit();
                response = GetAllAppHelp(model);
            }
            return response;
        }

        /// <summary>
        /// Saves the application help.
        /// </summary>
        /// <param name="appHelpViewModel">The application help view model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public AppHelpViewModel SaveAppHelp(AppHelpViewModel appHelpViewModel, Stream stream)
        {
            AppHelp appHelp = null;
            appHelp = _UnitOfWork.IAppHelpRepository.SingleOrDefault(u => u.HelpID == appHelpViewModel.HelpID);
            if (appHelp == null)
            {
                appHelp = new AppHelp();
                appHelp.Content = appHelpViewModel.Content;
                appHelp.CreatedOn = DateTime.UtcNow;
                appHelp.IsDeleted = false;
                appHelp.HelpText = appHelpViewModel.HelpText;
                appHelp.AdminID = appHelpViewModel.AdminID;

                // Upload file
                string path = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["AppHelpImagePath"]);
                bool isUploadSuccess = true;
                _UnitOfWork.IAppHelpRepository.Add(appHelp);
                _UnitOfWork.Commit();
                if (stream != null && stream.Length > 0)
                {
                    string newImageFileName = appHelp.HelpID + "_" + Guid.NewGuid().ToString() + appHelpViewModel.AppHelpExtension;
                    isUploadSuccess = Helper.SaveStreamToFile(path + "\\" + newImageFileName, stream);
                    if (isUploadSuccess == true)
                    {
                        appHelpViewModel.ImageURL = CryptoUtil.EncryptInfo(newImageFileName);
                        appHelp.ImageURL = appHelpViewModel.ImageURL;
                        _UnitOfWork.IAppHelpRepository.Update(appHelp);
                        _UnitOfWork.Commit();
                    }
                }
                appHelpViewModel.Status = LAMPConstants.SUCCESS_CODE;
                appHelpViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.APP_HELP_SAVED_SUCCESSFULLY);
                appHelpViewModel.IsSaved = true;
            }
            else
            {
                if (appHelp.HelpID > 0)
                {
                    appHelp.Content = appHelpViewModel.Content;
                    appHelp.EditedOn = DateTime.UtcNow;
                    appHelp.IsDeleted = false;
                    appHelp.HelpText = appHelpViewModel.HelpText;
                    appHelp.AdminID = appHelpViewModel.AdminID;

                    string path = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["AppHelpImagePath"]);
                    bool isUploadSuccess = true;
                    if (stream != null && stream.Length > 0)
                    {
                        string newImageFileName = appHelp.HelpID + "_" + Guid.NewGuid().ToString() + appHelpViewModel.AppHelpExtension;
                        isUploadSuccess = Helper.SaveStreamToFile(path + "\\" + newImageFileName, stream);
                        if (isUploadSuccess == true)
                        {
                            appHelpViewModel.ImageURL = CryptoUtil.EncryptInfo(newImageFileName);
                            appHelp.ImageURL = appHelpViewModel.ImageURL;
                        }
                    }
                    _UnitOfWork.IAppHelpRepository.Update(appHelp);
                    _UnitOfWork.Commit();
                    appHelpViewModel.Status = LAMPConstants.SUCCESS_CODE;
                    appHelpViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.APP_HELP_UPDATED_SUCCESSFULLY);
                    appHelpViewModel.IsSaved = true;
                }
            }
            return appHelpViewModel;
        }

        /// <summary>
        /// Gets all application help.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public AppHelpViewModelList GetAllAppHelp(AppHelpViewModelList model)
        {
            var response = new AppHelpViewModelList();
            try
            {
                IQueryable<AppHelpViewModel> enumAppHelpList = null;
                enumAppHelpList = (from appHelp in _UnitOfWork.IAppHelpRepository.RetrieveAll().Where(s => s.IsDeleted != true)
                                   select new AppHelpViewModel
                                   {
                                       HelpID = appHelp.HelpID,
                                       HelpText = appHelp.HelpText,
                                       Content = appHelp.Content,
                                       ImageURL = appHelp.ImageURL,
                                       CreatedOn = appHelp.CreatedOn,
                                       EditedOn = appHelp.EditedOn,
                                       IsDeleted = appHelp.IsDeleted,
                                       AdminID = appHelp.AdminID,
                                       CreatedAdminFName = appHelp.Admin.FirstName,
                                       CreatedAdminLName = appHelp.Admin.LastName,
                                   });
                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "CreatedOn" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "desc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            enumAppHelpList = enumAppHelpList.OrderBy(c => c.CreatedOn);
                        else
                            enumAppHelpList = enumAppHelpList.OrderByDescending(c => c.CreatedOn);
                        break;
                    default:
                        enumAppHelpList = enumAppHelpList.OrderByDescending(c => c.CreatedOn);
                        break;
                }
                //for checking ADD/EDIT Button
                AppHelp apphelp = _UnitOfWork.IAppHelpRepository.RetrieveAll().Where(u => u.AdminID == model.AdminLoggedInId && u.IsDeleted != true).FirstOrDefault();
                if (apphelp != null)
                    response.IsSuperAdminSaved = true;
                else
                    response.IsSuperAdminSaved = false;

                response.AppHelpList = enumAppHelpList.ToList();
                response.TotalRows = enumAppHelpList.Count();
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetAllAppHelp: " + ex);
                response = new AppHelpViewModelList
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        #endregion

        #region Admin

        /// <summary>
        /// Gets the admin details.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        public AdminViewModel GetAdminDetails(long adminId)
        {
            var model = new AdminViewModel();
            Admin adminDetails = null;
            adminDetails = _UnitOfWork.IAdminRepository.SingleOrDefault(u => u.AdminID == adminId);
            if (adminDetails != null)
            {
                model.AdminID = adminDetails.AdminID;
                model.FirstName = CryptoUtil.DecryptInfo(adminDetails.FirstName);
                model.LastName = CryptoUtil.DecryptInfo(adminDetails.LastName);
                model.Email = CryptoUtil.DecryptInfo(adminDetails.Email);
                model.Password = CryptoUtil.DecryptStringWithKey(adminDetails.Password);
            }
            return model;
        }

        /// <summary>
        /// Saves the admin.
        /// </summary>
        /// <param name="adminModel">The admin model.</param>
        /// <returns></returns>
        public AdminViewModel SaveAdmin(AdminViewModel adminModel)
        {
            var model = new AdminViewModel();
            try
            {
                Admin admin = null;
                string adminPassword = string.Empty;
                var newEmail = CryptoUtil.EncryptInfo(adminModel.Email.Trim().ToLower());
                if (adminModel.AdminID == 0)
                {
                    admin = _UnitOfWork.IAdminRepository.SingleOrDefault(u => u.Email == newEmail && u.IsDeleted != true);
                    if (admin == null)
                    {
                        admin = new Admin();
                        admin.Email = CryptoUtil.EncryptInfo(adminModel.Email.Trim().ToLower());
                        admin.FirstName = CryptoUtil.EncryptInfo(adminModel.FirstName.Trim());
                        admin.LastName = CryptoUtil.EncryptInfo(adminModel.LastName.Trim());

                        adminPassword = adminModel.Password.Trim();
                        admin.Password = CryptoUtil.EncryptStringWithKey(adminModel.Password.Trim());
                        admin.CreatedOn = DateTime.UtcNow;
                        admin.IsDeleted = false;
                        admin.AdminType = (byte)AdminRoles.Admin;
                        _UnitOfWork.IAdminRepository.Add(admin);
                        _UnitOfWork.Commit();
                        model.Status = LAMPConstants.SUCCESS_CODE;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.ADMIN_SAVED_SUCCESSFULLY);
                        model.IsSaved = true;
                        //saving jewels settings

                        Admin_JewelsTrailsASettings settings = new Admin_JewelsTrailsASettings();
                        settings.AdminJTASettingID = 0;
                        settings.NoOfSeconds_Adv = 25;
                        settings.NoOfSeconds_Beg = 90;
                        settings.NoOfSeconds_Exp = 15;
                        settings.NoOfSeconds_Int = 30;
                        settings.NoOfDiamonds = 25;
                        settings.NoOfShapes = 1;
                        settings.NoOfBonusPoints = 50;
                        settings.X_NoOfChangesInLevel = 1;
                        settings.X_NoOfDiamonds = 1;
                        settings.Y_NoOfChangesInLevel = 1;
                        settings.Y_NoOfShapes = 1;
                        settings.AdminID = (long)(admin.AdminID);
                        _UnitOfWork.IAdminJewelsTrailsASettingsRepository.Add(settings);

                        Admin_JewelsTrailsBSettings settingsB = new Admin_JewelsTrailsBSettings();
                        settingsB.AdminJTBSettingID = 0;
                        settingsB.NoOfSeconds_Adv = 60;
                        settingsB.NoOfSeconds_Beg = 180;
                        settingsB.NoOfSeconds_Exp = 45;
                        settingsB.NoOfSeconds_Int = 90;
                        settingsB.NoOfDiamonds = 25;
                        settingsB.NoOfShapes = 2;
                        settingsB.NoOfBonusPoints = 50;
                        settingsB.X_NoOfChangesInLevel = 1;
                        settingsB.X_NoOfDiamonds = 1;
                        settingsB.Y_NoOfChangesInLevel = 1;
                        settingsB.Y_NoOfShapes = 2;
                        settingsB.AdminID = (long)(admin.AdminID);
                        _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.Add(settingsB);

                        _UnitOfWork.Commit();

                        bool val = EmailNormalAdminCredentials(adminModel.Email, adminModel.FirstName + " " + adminModel.LastName, adminPassword);
                    }
                    else
                    {
                        model.Status = LAMPConstants.ERROR_CODE;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.ADMIN_NOT_SAVED);
                        model.IsSaved = false;
                    }
                }
                else if (adminModel.AdminID > 0)
                {
                    //update case
                    admin = _UnitOfWork.IAdminRepository.SingleOrDefault(u => u.AdminID == adminModel.AdminID && u.IsDeleted != true);
                    if (admin != null)
                    {
                        admin.Email = CryptoUtil.EncryptInfo(adminModel.Email.Trim().ToLower());
                        admin.FirstName = CryptoUtil.EncryptInfo(adminModel.FirstName.Trim());
                        admin.LastName = CryptoUtil.EncryptInfo(adminModel.LastName.Trim());
                        admin.Password = CryptoUtil.EncryptStringWithKey(adminModel.Password.Trim());
                        admin.EditedOn = DateTime.UtcNow;

                        _UnitOfWork.IAdminRepository.Update(admin);
                        _UnitOfWork.Commit();
                        model.Status = LAMPConstants.SUCCESS_CODE;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.ADMIN_UPDATED_SUCCESSFULLY);
                        model.IsSaved = true;
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveAdmin: " + ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// Get the User list
        /// </summary>
        /// <param name="model">Paging/Sorting details</param>
        /// <returns>User detail list</returns>
        public AdminListViewModel GetAllAdmins(AdminListViewModel model)
        {
            var response = new AdminListViewModel();
            try
            {
                IQueryable<Admins> enumAdminList = null;
                enumAdminList = (from admin in _UnitOfWork.IAdminRepository.RetrieveAll().Where(u => u.IsDeleted == false)
                                 select new Admins
                                 {
                                     AdminID = admin.AdminID,
                                     CreatedOn = admin.CreatedOn,
                                     Email = admin.Email,
                                     FirstName = admin.FirstName,
                                     LastName = admin.LastName,
                                     FullName = admin.FirstName + " " + admin.LastName
                                 });
                List<Admins> adminList = new List<Admins>();
                if (model.SearchId != null)
                {
                    adminList = enumAdminList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.FirstName).ToUpper().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToUpper())
                               || CryptoUtil.DecryptInfo(w.FirstName).ToUpper().Contains(model.SearchId)
                               || (CryptoUtil.DecryptInfo(w.FirstName) ?? string.Empty).Replace(" ", "").Trim().ToUpper().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToUpper())
                               || CryptoUtil.DecryptInfo(w.FirstName).ToLower().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToLower())
                               || CryptoUtil.DecryptInfo(w.FirstName).ToLower().Contains(model.SearchId)
                               || (CryptoUtil.DecryptInfo(w.FirstName) ?? string.Empty).Replace(" ", "").Trim().ToLower().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToLower())
                        //FisrtName
                               || CryptoUtil.DecryptInfo(w.LastName).ToUpper().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToUpper())
                               || CryptoUtil.DecryptInfo(w.LastName).ToUpper().Contains(model.SearchId)
                               || (CryptoUtil.DecryptInfo(w.LastName) ?? string.Empty).Replace(" ", "").Trim().ToUpper().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToUpper())
                               || CryptoUtil.DecryptInfo(w.LastName).ToLower().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToLower())
                               || CryptoUtil.DecryptInfo(w.LastName).ToLower().Contains(model.SearchId)
                               || (CryptoUtil.DecryptInfo(w.LastName) ?? string.Empty).Replace(" ", "").Trim().ToLower().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToLower())
                        //LastName
                               || (CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)).ToUpper().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToUpper())
                               || (CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)).ToUpper().Contains(model.SearchId)
                               || ((CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)) ?? string.Empty).Replace(" ", "").Trim().ToUpper().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToUpper())
                               || (CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)).ToLower().Trim().Contains((model.SearchId ?? string.Empty).Trim().ToLower())
                               || (CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)).ToLower().Contains(model.SearchId)
                               || ((CryptoUtil.DecryptInfo(w.FirstName) + " " + CryptoUtil.DecryptInfo(w.LastName)) ?? string.Empty).Replace(" ", "").Trim().ToLower().Contains((model.SearchId ?? string.Empty).Replace(" ", "").Trim().ToLower())
                        //FullName
                               ).ToList();
                }
                else
                    adminList = enumAdminList.ToList();
                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "CreatedOn" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "desc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "Email":
                        if (sortDirection == "asc")
                            adminList = adminList.OrderBy(c => c.Email).ToList();
                        else
                            adminList = adminList.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            adminList = adminList.OrderBy(c => c.CreatedOn).ToList();
                        else
                            adminList = adminList.OrderByDescending(c => c.CreatedOn).ToList();
                        break;
                    case "FirstName":
                        if (sortDirection == "asc")
                            adminList = adminList.OrderBy(c => c.FirstName).ToList();
                        else
                            adminList = adminList.OrderByDescending(c => c.FirstName).ToList();
                        break;
                    case "LastName":
                        if (sortDirection == "asc")
                            adminList = adminList.OrderBy(c => c.LastName).ToList();
                        else
                            adminList = adminList.OrderByDescending(c => c.LastName).ToList();
                        break;
                    default:
                        adminList = adminList.OrderByDescending(c => c.CreatedOn).ToList();
                        break;
                }
                response.AdminList = adminList;
                response.TotalRows = adminList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetAllAdmins: " + ex);
                response = new AdminListViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Deletes the admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        public AdminListViewModel DeleteAdmin(long adminId)
        {
            AdminListViewModel model = new AdminListViewModel();
            var response = new AdminListViewModel();
            Admin admin = _UnitOfWork.IAdminRepository.GetById(adminId);
            if (admin != null)
            {
                admin.IsDeleted = true;
                _UnitOfWork.IAdminRepository.Update(admin);
                _UnitOfWork.Commit();
                response = GetAllAdmins(model);
            }
            return response;
        }

        #endregion

        #region Schedule Game and Survey and Setting Distraction Survey

        /// <summary>
        /// Gets the schedule view model details.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        public ScheduleViewModel GetScheduleViewModelDetails(ScheduleViewModel scheduleViewModel)
        {
            var scheduleViewModelResponse = new ScheduleViewModel();
            try
            {
                scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                scheduleViewModelResponse.UserId = scheduleViewModel.UserId;

                UserSetting UserSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(u => u.UserID == scheduleViewModel.UserId);
                if (UserSetting != null)
                {
                    scheduleViewModelResponse.UserSettingID = UserSetting.UserSettingID;
                    scheduleViewModelResponse.UserId = (long)UserSetting.UserID;
                    scheduleViewModelResponse.SurveySlotId = UserSetting.SympSurvey_SlotID;
                    scheduleViewModelResponse.SurveyRepeatId = UserSetting.SympSurvey_RepeatID;
                    scheduleViewModelResponse.CognitionTestSlotId = UserSetting.CognTest_SlotID;
                    scheduleViewModelResponse.CognitionTestRepeatId = UserSetting.CognTest_RepeatID;
                    scheduleViewModelResponse.SurveySlotTimeDatetime = UserSetting.SympSurvey_Time;
                    scheduleViewModelResponse.CognitionTestTimeDatetime = UserSetting.CognTest_Time;
                    if (scheduleViewModelResponse.SurveySlotTimeDatetime != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(scheduleViewModelResponse.SurveySlotTimeDatetime);
                        string surveySlotDisplayTime = surveyDisplayTime.ToString("G", CultureInfo.CreateSpecificCulture("en-us"));
                        scheduleViewModelResponse.SurveySlotTimeString = surveySlotDisplayTime;
                    }
                    if (scheduleViewModelResponse.CognitionTestTimeDatetime != null)
                    {
                        DateTime cognitionTestDisplayTime = (DateTime)(scheduleViewModelResponse.CognitionTestTimeDatetime);
                        string cognitionDisplayTime = cognitionTestDisplayTime.ToString("G", CultureInfo.CreateSpecificCulture("en-us"));
                        scheduleViewModelResponse.CognitionTestSlotTimeString = cognitionDisplayTime;
                    }

                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == scheduleViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();


                    if (!string.IsNullOrEmpty(CryptoUtil.DecryptInfo(UserSetting.PrefferedSurveys)))
                    {
                        string[] SurveyArrayStringList = (CryptoUtil.DecryptInfo(UserSetting.PrefferedSurveys) ?? string.Empty).Split(',');
                        long[] SurveyArray = SurveyArrayStringList.Select(long.Parse).ToArray();
                        scheduleViewModelResponse.SurveyArray = SurveyArray.ToArray();
                    }
                    if (!string.IsNullOrEmpty(CryptoUtil.DecryptInfo(UserSetting.PrefferedCognitions)))
                    {
                        string[] CTesTArrayStringList = (CryptoUtil.DecryptInfo(UserSetting.PrefferedCognitions) ?? string.Empty).Split(',');
                        long[] CTesTArray = CTesTArrayStringList.Select(long.Parse).ToArray();
                        scheduleViewModelResponse.CTesTArray = CTesTArray.ToArray();
                    }
                }
                else
                {
                    //for  user which is not saved
                    scheduleViewModelResponse.UserId = scheduleViewModel.UserId;
                    scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                    scheduleViewModelResponse.SurveySlotTimeString = "12:00 AM";
                    scheduleViewModelResponse.CognitionTestSlotTimeString = "12:00 AM";

                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == scheduleViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetScheduleViewModelDetails: " + ex);
                scheduleViewModelResponse = new ScheduleViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleViewModelResponse;
        }

        /// <summary>
        /// Gets the schedule view model details.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel GetScheduleViewModelDetailsForAdmin(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)
        {
            var scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                //filling dropdownlist
                var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true && u.CTestID != 19).OrderBy(u => u.SortOrder).ToList();
                scheduleGameSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                var fisrtCtestselected = scheduleGameSurveyViewModelResponse.CognitionTestList.ElementAt(0).Value;
                var cognitionTestVerisonList = GetCognitionVersion(Convert.ToInt64(fisrtCtestselected)).ToList();

                scheduleGameSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text.ToString(), Value = x.Value.ToString(), Selected = true }).ToList();
                scheduleGameSurveyViewModelResponse.GameScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
                scheduleGameSurveyViewModelResponse.SurveyScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetScheduleViewModelDetailsForAdmin: " + ex);
                scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleGameSurveyViewModelResponse;
        }

        /// <summary>
        /// Gets the survey schedule details.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel GetSurveyScheduleDetailsByAdminSurveySchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)
        {
            var scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                scheduleGameSurveyViewModelResponse.AdminSurveySchID = ScheduleGameSurveyViewModel.AdminSurveySchID;
                Admin_SurveySchedule schedule = _UnitOfWork.IAdminSurveyScheduleRepository.SingleOrDefault(u => u.AdminSurveySchID == ScheduleGameSurveyViewModel.AdminSurveySchID);
                if (schedule != null)
                {
                    scheduleGameSurveyViewModelResponse.AdminSurveySchID = schedule.AdminSurveySchID;
                    scheduleGameSurveyViewModelResponse.AdminId = (long)schedule.AdminID;
                    scheduleGameSurveyViewModelResponse.SurveySlotId = schedule.SlotID;
                    scheduleGameSurveyViewModelResponse.SurveyRepeatId = schedule.RepeatID;
                    scheduleGameSurveyViewModelResponse.SurveyId = (long)schedule.SurveyID;
                    scheduleGameSurveyViewModelResponse.SurveySlotTimeDatetime = schedule.Time;
                    scheduleGameSurveyViewModelResponse.SurveyScheduleDateValue = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("MM/dd/yyyy") : "";
                    scheduleGameSurveyViewModelResponse.SurveyScheduleDateString = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("dd/MM/yyyy") : "";

                    if (scheduleGameSurveyViewModelResponse.SurveySlotTimeDatetime != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(scheduleGameSurveyViewModelResponse.SurveySlotTimeDatetime);
                        string surveySlotDisplayTime = surveyDisplayTime.ToString("G", CultureInfo.CreateSpecificCulture("en-us"));
                        scheduleGameSurveyViewModelResponse.SurveySlotTimeString = surveySlotDisplayTime;
                    }
                    long repeatId = (long)scheduleGameSurveyViewModelResponse.SurveyRepeatId;
                    if (repeatId == 11)
                    {
                        List<Admin_SurveyScheduleCustomTime> scheduleList = _UnitOfWork.IAdminSurveyScheduleCustomTimeRepository.RetrieveAll().Where(u => u.AdminSurveySchID == schedule.AdminSurveySchID).ToList();
                        List<string> options = new List<string>();
                        foreach (var item in scheduleList)
                        {
                            DateTime surveyDisplayTime = (DateTime)(item.Time);
                            var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                            options.Add(Timestamp.ToString());
                        }
                        if (scheduleList != null)
                        {
                            scheduleGameSurveyViewModelResponse.OptionsStringList = options;
                        }
                    }
                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();

                }
                else
                {
                    //for  user which is not saved
                    scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                    scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;

                    scheduleGameSurveyViewModelResponse.SurveySlotTimeString = "12:00 AM";
                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                }
                if (string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.SurveyScheduleDateValue) && string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.SurveyScheduleDateString))
                {
                    scheduleGameSurveyViewModelResponse.SurveyScheduleDateValue = DateTime.Now.ToString("MM/dd/yyyy");
                    scheduleGameSurveyViewModelResponse.SurveyScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetSurveyScheduleDetailsByAdminSurveySchID: " + ex);
                scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleGameSurveyViewModelResponse;
        }

        /// <summary>
        /// Gets the game schedule details by admin c test SCH identifier.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel GetGameScheduleDetailsByAdminCTestSchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)
        {
            var scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                scheduleGameSurveyViewModelResponse.AdminCTestSchID = ScheduleGameSurveyViewModel.AdminCTestSchID;
                Admin_CTestSchedule schedule = _UnitOfWork.IAdminCTestScheduleRepository.SingleOrDefault(u => u.AdminCTestSchID == ScheduleGameSurveyViewModel.AdminCTestSchID);
                if (schedule != null)
                {
                    scheduleGameSurveyViewModelResponse.AdminCTestSchID = schedule.AdminCTestSchID;
                    scheduleGameSurveyViewModelResponse.AdminId = (long)schedule.AdminID;
                    scheduleGameSurveyViewModelResponse.CognitionTestSlotId = schedule.SlotID;
                    scheduleGameSurveyViewModelResponse.CognitionTestRepeatId = schedule.RepeatID;
                    scheduleGameSurveyViewModelResponse.CognitionTestId = (long)schedule.CTestID;
                    scheduleGameSurveyViewModelResponse.CognitionTestTimeDatetime = schedule.Time;
                    scheduleGameSurveyViewModelResponse.CognitionVersionId = (long)schedule.Version;
                    scheduleGameSurveyViewModelResponse.GameScheduleDateValue = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("MM/dd/yyyy") : "";
                    scheduleGameSurveyViewModelResponse.GameScheduleDateString = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("dd/MM/yyyy") : "";

                    if (scheduleGameSurveyViewModelResponse.CognitionTestTimeDatetime != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(scheduleGameSurveyViewModelResponse.CognitionTestTimeDatetime);

                        string surveySlotDisplayTime = surveyDisplayTime.ToString("G", CultureInfo.CreateSpecificCulture("en-us"));
                        scheduleGameSurveyViewModelResponse.CognitionTestSlotTimeString = surveySlotDisplayTime;
                    }
                    long repeatId = (long)scheduleGameSurveyViewModelResponse.CognitionTestRepeatId;
                    if (repeatId == 11)
                    {
                        List<Admin_CTestScheduleCustomTime> scheduleList = _UnitOfWork.IAdminCTestScheduleCustomTimeRepository.RetrieveAll().Where(u => u.AdminCTestSchID == schedule.AdminCTestSchID).ToList();
                        List<string> options = new List<string>();
                        foreach (var item in scheduleList)
                        {
                            DateTime surveyDisplayTime = (DateTime)(item.Time);
                            var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                            options.Add(Timestamp.ToString());
                        }
                        if (scheduleList != null)
                        {
                            scheduleGameSurveyViewModelResponse.OptionsStringList = options;
                        }
                    }
                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true && u.CTestID != 19).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                    var cognitionTestVerisonList = GetCognitionVersion(scheduleGameSurveyViewModelResponse.CognitionTestId).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = true }).ToList();
                }
                else
                {
                    //for  user which is not saved
                    scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                    scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                    scheduleGameSurveyViewModelResponse.SurveySlotTimeString = "12:00 AM";
                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                    var fisrtCtestselected = scheduleGameSurveyViewModelResponse.CognitionTestList.ElementAt(0).Value;
                    var cognitionTestVerisonList = GetCognitionVersion(Convert.ToInt64(fisrtCtestselected)).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = true }).ToList();
                }
                if (string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.GameScheduleDateValue) && string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.GameScheduleDateString))
                {
                    scheduleGameSurveyViewModelResponse.GameScheduleDateValue = DateTime.Now.ToString("MM/dd/yyyy");
                    scheduleGameSurveyViewModelResponse.GameScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetSurveyScheduleDetailsByAdminSurveySchID: " + ex);
                scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleGameSurveyViewModelResponse;
        }

        /// <summary>
        /// Gets the game schedule details by admin c test SCH identifier.
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel">The schedule game survey view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel GetBatchScheduleDetailsByAdminBatchSchID(ScheduleGameSurveyViewModel ScheduleGameSurveyViewModel)        
        {
            var scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                scheduleGameSurveyViewModelResponse.AdminBatchSchID = ScheduleGameSurveyViewModel.AdminBatchSchID;
                Admin_BatchSchedule schedule = _UnitOfWork.IAdminBatchScheduleRepository.SingleOrDefault(u => u.AdminBatchSchID == ScheduleGameSurveyViewModel.AdminBatchSchID);
                if (schedule != null)
                {
                    scheduleGameSurveyViewModelResponse.AdminBatchSchID = schedule.AdminBatchSchID;
                    scheduleGameSurveyViewModelResponse.AdminId = (long)schedule.AdminID;
                    scheduleGameSurveyViewModelResponse.BatchName = schedule.BatchName;
                    scheduleGameSurveyViewModelResponse.BatchScheduleDateValue = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("MM/dd/yyyy") : "";
                    scheduleGameSurveyViewModelResponse.BatchScheduleDateString = schedule.ScheduleDate.HasValue ? schedule.ScheduleDate.Value.ToString("dd/MM/yyyy") : "";
                    scheduleGameSurveyViewModelResponse.BatchSlotId = schedule.SlotID;
                    scheduleGameSurveyViewModelResponse.BatchSlotTimeDatetime = schedule.Time;
                    scheduleGameSurveyViewModelResponse.BatchRepeatId = schedule.RepeatID;

                    if (scheduleGameSurveyViewModelResponse.BatchSlotTimeDatetime != null)
                    {
                        DateTime batchDisplayTime = (DateTime)(scheduleGameSurveyViewModelResponse.BatchSlotTimeDatetime);
                        string batchSlotDisplayTime = batchDisplayTime.ToString("G", CultureInfo.CreateSpecificCulture("en-us"));
                        scheduleGameSurveyViewModelResponse.BatchSlotTimeString = batchSlotDisplayTime;
                    }

                    // BatchScheduleCTest List
                    List<Admin_BatchScheduleCTest> repBatchScheduleCTestList = _UnitOfWork.IAdminBatchScheduleCTestRepository.RetrieveAll().Where(u => u.AdminBatchSchID == ScheduleGameSurveyViewModel.AdminBatchSchID).ToList();
                    List<BatchScheduleSurvey_CTest> batchScheduleSurvey_CTestList = new List<BatchScheduleSurvey_CTest>();
                    BatchScheduleSurvey_CTest obj;
                    foreach (var item in repBatchScheduleCTestList)
                    {
                        obj = new BatchScheduleSurvey_CTest();
                        obj.BatchScheduleId = Convert.ToInt64(item.AdminBatchSchID);
                        obj.Type = Convert.ToInt16(2);
                        obj.ID = Convert.ToInt64(item.CTestID);
                        obj.Version = Convert.ToInt32(item.Version);
                        obj.Order = Convert.ToInt16(item.Order);
                        batchScheduleSurvey_CTestList.Add(obj);
                    }                   

                    // BatchScheduleSurvey List
                    List<Admin_BatchScheduleSurvey> repBatchScheduleSurveyList = _UnitOfWork.IAdminBatchScheduleSurveyRepository.RetrieveAll().Where(u => u.AdminBatchSchID == ScheduleGameSurveyViewModel.AdminBatchSchID).ToList();
                    foreach (var item in repBatchScheduleSurveyList)
                    {
                        obj = new BatchScheduleSurvey_CTest();
                        obj.BatchScheduleId = Convert.ToInt64(item.AdminBatchSchID);
                        obj.Type = Convert.ToInt16(1);
                        obj.ID = Convert.ToInt64(item.SurveyID);
                        obj.Version = Convert.ToInt32(0);
                        obj.Order = Convert.ToInt16(item.Order);
                        batchScheduleSurvey_CTestList.Add(obj);
                    }
                    batchScheduleSurvey_CTestList = batchScheduleSurvey_CTestList.OrderBy(o => o.Order).ToList();
                    string strBatchSurveyGames = string.Empty;
                    int loop = 1;
                    string batchName = string.Empty;
                    foreach (BatchScheduleSurvey_CTest objSurveyGame in batchScheduleSurvey_CTestList)
                    {
                        //format type:id:name:version => 1:4:Pressure:0|2:6:Serial 7s:2|1:2:mood:0
                        if (objSurveyGame.Type == 1)
                        {
                            Survey survey = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(s => s.SurveyID == objSurveyGame.ID).FirstOrDefault();
                            batchName = survey.SurveyName;
                        }
                        else
                        {
                            CTest game = _UnitOfWork.ICTestRepository.RetrieveAll().Where(s => s.CTestID == objSurveyGame.ID).FirstOrDefault();
                            batchName = game.CTestName;
                        }
                        if (loop == 1)
                        {
                            strBatchSurveyGames = objSurveyGame.Type.ToString() + ":" + objSurveyGame.ID.ToString() + ":" + batchName + ":" + objSurveyGame.Version.ToString();
                        }
                        else
                        {
                            strBatchSurveyGames = strBatchSurveyGames + "|" + objSurveyGame.Type.ToString() + ":" + objSurveyGame.ID.ToString() + ":" + batchName + ":" + objSurveyGame.Version.ToString();
                        }
                        loop++;
                    }
                    scheduleGameSurveyViewModelResponse.BatchSurveyGames = strBatchSurveyGames;
                    // BatchScheduleCustomTime List
                    List<Admin_BatchScheduleCustomTime> repBatchScheduleCustomTimeList = _UnitOfWork.IAdminBatchScheduleCustomTimeRepository.RetrieveAll().Where(u => u.AdminBatchSchID == ScheduleGameSurveyViewModel.AdminBatchSchID).ToList();
                    List<BatchScheduleCustomTimeViewModel> batchScheduleCustomTimeList = new List<BatchScheduleCustomTimeViewModel>();
                    BatchScheduleCustomTimeViewModel batchScheduleCustomTime;
                    foreach (var item in repBatchScheduleCustomTimeList)
                    {
                        batchScheduleCustomTime = new BatchScheduleCustomTimeViewModel();
                        batchScheduleCustomTime.AdminBatchSchID = Convert.ToInt64(item.AdminBatchSchID);
                        batchScheduleCustomTime.CustomDatetime = Convert.ToDateTime(item.Time);
                        batchScheduleCustomTimeList.Add(batchScheduleCustomTime);
                    }
                    scheduleGameSurveyViewModelResponse.BatchScheduleCustomTimeViewModel = batchScheduleCustomTimeList;
                    
                    long repeatId = (long)scheduleGameSurveyViewModelResponse.BatchRepeatId;
                    if (repeatId == 11)
                    {
                        List<Admin_BatchScheduleCustomTime> scheduleList = _UnitOfWork.IAdminBatchScheduleCustomTimeRepository.RetrieveAll().Where(u => u.AdminBatchSchID == ScheduleGameSurveyViewModel.AdminBatchSchID).ToList();
                        List<string> options = new List<string>();
                        foreach (var item in scheduleList)
                        {
                            DateTime surveyDisplayTime = (DateTime)(item.Time);
                            var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                            options.Add(Timestamp.ToString());
                        }
                        if (scheduleList != null)
                        {
                            scheduleGameSurveyViewModelResponse.OptionsStringList = options;
                        }
                    }

                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true && u.CTestID != 19).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                    var cognitionTestVerisonList = GetCognitionVersion(scheduleGameSurveyViewModelResponse.CognitionTestId).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = true }).ToList();                    
                }
                else
                {
                    //for  user which is not saved
                    scheduleGameSurveyViewModelResponse.AdminId = ScheduleGameSurveyViewModel.AdminId;
                    scheduleGameSurveyViewModelResponse.LoggedInUserId = ScheduleGameSurveyViewModel.LoggedInUserId;
                    scheduleGameSurveyViewModelResponse.SurveySlotTimeString = "12:00 AM";
                    //filling dropdownlist
                    var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                    scheduleGameSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                    var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                    var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleGameSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                    scheduleGameSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                    var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true && u.CTestID != 19).OrderBy(u => u.SortOrder).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                    var fisrtCtestselected = scheduleGameSurveyViewModelResponse.CognitionTestList.ElementAt(0).Value;
                    var cognitionTestVerisonList = GetCognitionVersion(Convert.ToInt64(fisrtCtestselected)).ToList();
                    scheduleGameSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = true }).ToList();
                    scheduleGameSurveyViewModelResponse.BatchSurveyGames = "";
                }

                scheduleGameSurveyViewModelResponse.SurveyList.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                scheduleGameSurveyViewModelResponse.CognitionTestList.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                scheduleGameSurveyViewModelResponse.CognitionVersionList.Insert(0, new SelectListItem { Value = "0", Text = "Select" });

                if (string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.GameScheduleDateValue) && string.IsNullOrEmpty(scheduleGameSurveyViewModelResponse.GameScheduleDateString))
                {
                    scheduleGameSurveyViewModelResponse.GameScheduleDateValue = DateTime.Now.ToString("MM/dd/yyyy");
                    scheduleGameSurveyViewModelResponse.GameScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetSurveyScheduleDetailsByAdminSurveySchID: " + ex);
                scheduleGameSurveyViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleGameSurveyViewModelResponse;
        }

        /// <summary>
        /// Saves the shedule survey and game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        public ScheduleViewModel SaveSheduleSurveyAndGame(ScheduleViewModel scheduleViewModel)
        {
            var scheduleViewModelResponse = new ScheduleViewModel();
            try
            {
                scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                scheduleViewModelResponse.UserId = scheduleViewModel.UserId;
                scheduleViewModelResponse.UserSettingID = scheduleViewModel.UserSettingID;
                UserSetting UserSetting = null;
                UserSetting = _UnitOfWork.IUserSettingRepository.SingleOrDefault(u => u.UserID == scheduleViewModel.UserId);
                if (UserSetting == null)
                {
                    UserSetting = new UserSetting();
                    UserSetting.UserID = scheduleViewModel.UserId;
                    UserSetting.SympSurvey_SlotID = scheduleViewModel.SurveySlotId;
                    UserSetting.SympSurvey_RepeatID = scheduleViewModel.SurveyRepeatId;
                    UserSetting.CognTest_SlotID = scheduleViewModel.CognitionTestSlotId;
                    string appcolor = "#359FFE";
                    UserSetting.AppColor = CryptoUtil.EncryptInfo(appcolor.Trim());
                    UserSetting.CognTest_Time = Convert.ToDateTime(scheduleViewModel.CognitionTestSlotTimeString).ToUniversalTime();
                    UserSetting.SympSurvey_Time = Convert.ToDateTime(scheduleViewModel.SurveySlotTimeString).ToUniversalTime();
                    UserSetting.CognTest_RepeatID = scheduleViewModel.CognitionTestRepeatId;
                    UserSetting.CreatedOn = DateTime.UtcNow;
                    UserSetting.PrefferedSurveys = CryptoUtil.EncryptInfo(scheduleViewModel.SurveyArrayString);
                    UserSetting.PrefferedCognitions = CryptoUtil.EncryptInfo(scheduleViewModel.CTesTArrayString);
                    _UnitOfWork.IUserSettingRepository.Add(UserSetting);
                    _UnitOfWork.Commit();
                    scheduleViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                    scheduleViewModelResponse.Message = ResourceHelper.GetStringResource(LAMPConstants.SCHEDULE_SAVED_SUCCESSFULLY);
                    scheduleViewModelResponse.IsSaved = true;
                }
                else
                {
                    scheduleViewModel.UserSettingID = UserSetting.UserSettingID;
                    UserSetting.UserID = scheduleViewModel.UserId;
                    UserSetting.SympSurvey_SlotID = scheduleViewModel.SurveySlotId;
                    UserSetting.SympSurvey_RepeatID = scheduleViewModel.SurveyRepeatId;
                    UserSetting.CognTest_SlotID = scheduleViewModel.CognitionTestSlotId;
                    UserSetting.CognTest_RepeatID = scheduleViewModel.CognitionTestRepeatId;
                    UserSetting.CognTest_Time = Convert.ToDateTime(scheduleViewModel.CognitionTestSlotTimeString).ToUniversalTime();
                    UserSetting.SympSurvey_Time = Convert.ToDateTime(scheduleViewModel.SurveySlotTimeString).ToUniversalTime();
                    UserSetting.EditedOn = DateTime.UtcNow;
                    UserSetting.PrefferedSurveys = CryptoUtil.EncryptInfo(scheduleViewModel.SurveyArrayString);
                    UserSetting.PrefferedCognitions = CryptoUtil.EncryptInfo(scheduleViewModel.CTesTArrayString);
                    _UnitOfWork.IUserSettingRepository.Update(UserSetting);
                    _UnitOfWork.Commit();
                    scheduleViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                    scheduleViewModelResponse.Message = ResourceHelper.GetStringResource(LAMPConstants.SCHEDULE_UPDATED_SUCCESSFULLY);
                    scheduleViewModelResponse.IsSaved = true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveSheduleSurveyAndGame: " + ex);
                scheduleViewModelResponse = new ScheduleViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleViewModelResponse;
        }

        /// <summary>
        /// Saves the shedule survey and game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel SaveSheduleSurvey(ScheduleGameSurveyViewModel scheduleViewModel)
        {
            var scheduleViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                scheduleViewModelResponse.AdminId = scheduleViewModel.AdminId;
                scheduleViewModelResponse.AdminSurveySchID = scheduleViewModel.AdminSurveySchID;
                scheduleViewModelResponse.SurveyScheduleDateString = scheduleViewModel.SurveyScheduleDateString;
                scheduleViewModelResponse.SurveySlotTimeString = scheduleViewModel.SurveySlotTimeString;
                scheduleViewModelResponse.SurveySlotId = scheduleViewModel.SurveySlotId;

                long repeatId = (long)scheduleViewModel.SurveyRepeatId;
                XElement xmlOptionElements = null;
                if (repeatId == 11)
                {
                    List<string> optionList = new List<string>();
                    foreach (string item in scheduleViewModel.OptionsStringList.ToList())
                    {
                        DateTime dto = Convert.ToDateTime(item).ToUniversalTime();
                        string item2 = dto.ToString("yyyy-MM-dd HH:mm");
                        optionList.Add(item2);
                    }
                    xmlOptionElements = new XElement("CustomTimes", optionList.Select(i => new XElement("CustomTime", i)));
                }

                var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                scheduleViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == scheduleViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                scheduleViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();

                Admin_SaveSurveySchedule_sp_Result result = new Admin_SaveSurveySchedule_sp_Result();
                LAMPEntities context = new LAMPEntities();
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "Admin_SaveSurveySchedule_sp";
                command.CommandType = CommandType.StoredProcedure;
                DateTime dt1;
                DateTime dt2;

                command.Parameters.Add(new SqlParameter("@p_AdminSurveySchID", scheduleViewModel.AdminSurveySchID));
                command.Parameters.Add(new SqlParameter("@p_AdminID", scheduleViewModel.AdminId));
                command.Parameters.Add(new SqlParameter("@p_SurveyID", scheduleViewModel.SurveyId));
                command.Parameters.Add(new SqlParameter("@p_RepeatID", scheduleViewModel.SurveyRepeatId));
                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", null));
                }
                else
                {
                    dt1 = DateTime.ParseExact(scheduleViewModel.SurveyScheduleDateString, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", dt1));
                }

                command.Parameters.Add(new SqlParameter("@p_SlotID", scheduleViewModel.SurveySlotId));

                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_Time", null));
                }
                else
                {
                    dt2 = Convert.ToDateTime(scheduleViewModel.SurveySlotTimeString).ToUniversalTime();
                    command.Parameters.Add(new SqlParameter("@p_Time", dt2));
                    LogUtil.Error("SaveSheduleSurvey for @p_Time: SurveySlotTimeString" + scheduleViewModel.SurveySlotTimeString + " dt2: " + dt2.ToString() + " for survey id: " + scheduleViewModel.SurveyId.ToString());
                }

                if (repeatId == 11)
                {
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", xmlOptionElements.ToString()));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", null));
                }

                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);
                context.Database.Connection.Open();
                long ErrorCode = 0;
                var reader = command.ExecuteReader();
                if (outputErrorParameter.Value != null)
                {
                    ErrorCode = Convert.ToInt32(outputErrorParameter.Value);
                }
                if (ErrorCode == 0)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                    scheduleViewModelResponse.Message = "The survey schedule have been saved successfully.";
                    scheduleViewModelResponse.IsSaved = true;
                }
                else if (ErrorCode == 1002)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.ERROR_CODE;
                    scheduleViewModelResponse.Message = "Survey Schedule Already Exists.";
                    scheduleViewModelResponse.IsSaved = false;
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveSheduleSurvey: " + ex);
                scheduleViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleViewModelResponse;
        }

        /// <summary>
        /// Saves the shedule game.
        /// </summary>
        /// <param name="scheduleViewModel">The schedule view model.</param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel SaveSheduleGame(ScheduleGameSurveyViewModel scheduleViewModel)
        {
            var scheduleViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                scheduleViewModelResponse.AdminId = scheduleViewModel.AdminId;
                scheduleViewModelResponse.AdminCTestSchID = scheduleViewModel.AdminCTestSchID;
                LAMPEntities context = new LAMPEntities();

                scheduleViewModelResponse.GameScheduleDateString = scheduleViewModel.GameScheduleDateString;
                scheduleViewModelResponse.CognitionTestSlotTimeString = scheduleViewModel.CognitionTestSlotTimeString;
                scheduleViewModelResponse.CognitionTestSlotId = scheduleViewModel.CognitionTestSlotId;
                scheduleViewModelResponse.CognitionTestId = scheduleViewModel.CognitionTestId;
                scheduleViewModelResponse.CognitionVersionId = scheduleViewModel.CognitionVersionId;

                var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                scheduleViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                var fisrtCtestselected = scheduleViewModelResponse.CognitionTestList.ElementAt(0).Value;
                var cognitionTestVerisonList = GetCognitionVersion(Convert.ToInt64(scheduleViewModelResponse.CognitionTestId)).ToList();
                scheduleViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = true }).ToList();


                long repeatId = (long)scheduleViewModel.CognitionTestRepeatId;
                XElement xmlOptionElements = null;
                if (repeatId == 11)
                {
                    List<string> optionList = new List<string>();
                    foreach (string item in scheduleViewModel.OptionsStringList.ToList())
                    {
                        DateTime dto = Convert.ToDateTime(item).ToUniversalTime();
                        string item2 = dto.ToString("yyyy-MM-dd HH:mm");
                        optionList.Add(item2);
                    }
                    xmlOptionElements = new XElement("CustomTimes", optionList.Select(i => new XElement("CustomTime", i)));
                }


                Admin_SaveCTestSchedule_sp_Result result = new Admin_SaveCTestSchedule_sp_Result();

                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "Admin_SaveCTestSchedule_sp";
                command.CommandType = CommandType.StoredProcedure;


                DateTime dt1;
                DateTime dt2;

                command.Parameters.Add(new SqlParameter("@p_AdminCTestSchID", scheduleViewModel.AdminCTestSchID));
                command.Parameters.Add(new SqlParameter("@p_AdminID", scheduleViewModel.AdminId));
                command.Parameters.Add(new SqlParameter("@p_CTestID", scheduleViewModel.CognitionTestId));
                command.Parameters.Add(new SqlParameter("@p_Version", (int)scheduleViewModel.CognitionVersionId));
                command.Parameters.Add(new SqlParameter("@p_RepeatID", scheduleViewModel.CognitionTestRepeatId));

                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", null));
                }
                else
                {
                    dt1 = DateTime.ParseExact(scheduleViewModel.GameScheduleDateString, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", dt1));
                }
                command.Parameters.Add(new SqlParameter("@p_SlotID", scheduleViewModel.CognitionTestSlotId));

                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_Time", null));
                }
                else
                {
                    dt2 = Convert.ToDateTime(scheduleViewModel.CognitionTestSlotTimeString).ToUniversalTime();
                    command.Parameters.Add(new SqlParameter("@p_Time", dt2));
                }

                if (repeatId == 11)
                {
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", xmlOptionElements.ToString()));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", null));
                }

                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);
                context.Database.Connection.Open();
                long ErrorCode = 0;
                var reader = command.ExecuteReader();
                if (outputErrorParameter.Value != null)
                {
                    ErrorCode = Convert.ToInt32(outputErrorParameter.Value);
                }
                if (ErrorCode == 0)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                    scheduleViewModelResponse.Message = "The Game schedule have been saved successfully.";
                    scheduleViewModelResponse.IsSaved = true;
                }
                else if (ErrorCode == 1001)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.ERROR_CODE;
                    scheduleViewModelResponse.Message = "Game Schedule Already Exists.";
                    scheduleViewModelResponse.IsSaved = false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveSheduleGame: " + ex);
                scheduleViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleViewModelResponse;
        }

        /// <summary>
        /// Gets the cognition version.
        /// </summary>
        /// <param name="cognitionId">The cognition identifier.</param>
        /// <returns></returns>
        public List<SelectListItem> GetCognitionVersion(long cognitionId)
        {
            var cTest = _UnitOfWork.ICTestRepository.SingleOrDefault(u => u.CTestID == cognitionId);
            if (cTest == null)
            {
                cTest = new CTest();
            }
            List<int> versions = new List<int>();
            if (cTest.MaxVersions != null)
            {
                versions = Enumerable.Range(1, cTest.MaxVersions.Value).ToList();
            }
            List<SelectListItem> item = new List<SelectListItem>();
            if (versions.Count == 0)
            {

                SelectListItem select = new SelectListItem(); select.Text = "No Version"; select.Value = "-1";
                item.Insert(0, select);
            }
            else
            {
                item = versions.ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.ToString(),
                        Value = a.ToString(),
                        Selected = false
                    };
                });
            }
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ScheduleSurveyListViewModel DeleteSurveySchedule(long surveyId)
        {
            ScheduleSurveyListViewModel model = new ScheduleSurveyListViewModel();
            var response = new ScheduleSurveyListViewModel();
            Admin_SurveySchedule survey = _UnitOfWork.IAdminSurveyScheduleRepository.GetById(surveyId);
            if (survey != null)
            {
                survey.IsDeleted = true;
                survey.EditedOn = DateTime.UtcNow;
                _UnitOfWork.IAdminSurveyScheduleRepository.Update(survey);
                _UnitOfWork.Commit();
                response = GetSurveyScheduledList(model);
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cTestId"></param>
        /// <returns></returns>
        public ScheduleGameListViewModel DeleteGameSchedule(long cTestId)
        {
            ScheduleGameListViewModel model = new ScheduleGameListViewModel();
            var response = new ScheduleGameListViewModel();
            Admin_CTestSchedule cTest = _UnitOfWork.IAdminCTestScheduleRepository.GetById(cTestId);
            if (cTest != null)
            {
                cTest.IsDeleted = true;
                cTest.EditedOn = DateTime.UtcNow;
                _UnitOfWork.IAdminCTestScheduleRepository.Update(cTest);
                _UnitOfWork.Commit();
                response = GetGameScheduledList(model);
            }
            return response;
        }

        public ScheduleListViewModel DeleteBatchSchedule(long batchId)
        {
            ScheduleGameListViewModel model = new ScheduleGameListViewModel();
            var response = new ScheduleListViewModel();
            Admin_BatchSchedule batch = _UnitOfWork.IAdminBatchScheduleRepository.GetById(batchId);
            if (batch != null)
            {
                batch.IsDeleted = true;
                batch.EditedOn = DateTime.UtcNow;
                _UnitOfWork.IAdminBatchScheduleRepository.Update(batch);
                _UnitOfWork.Commit();
            }
            return response;
        }

        /// <summary>
        /// Gets the survey scheduled list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ScheduleSurveyListViewModel GetSurveyScheduledList(ScheduleSurveyListViewModel model)
        {
            var response = new ScheduleSurveyListViewModel();
            try
            {
                IEnumerable<AdminSurveyScheduleViewModel> enumSurveyScheduleList = (from scheduledsurveys in _UnitOfWork.IAdminSurveyScheduleRepository.RetrieveAll()
                                                                                    join slots in _UnitOfWork.ISlotRepository.RetrieveAll() on scheduledsurveys.SlotID equals slots.SlotID into ps
                                                                                    from p in ps.DefaultIfEmpty()
                                                                                    join surveys in _UnitOfWork.ISurveyRepository.RetrieveAll() on scheduledsurveys.SurveyID equals surveys.SurveyID into qs
                                                                                    from q in qs.DefaultIfEmpty()
                                                                                    join repeats in _UnitOfWork.IRepeatRepository.RetrieveAll() on scheduledsurveys.RepeatID equals repeats.RepeatID into rs
                                                                                    from r in rs.DefaultIfEmpty()
                                                                                    where scheduledsurveys.IsDeleted != true && scheduledsurveys.AdminID == model.LoggedInAdminId
                                                                                    select new AdminSurveyScheduleViewModel
                                                                                    {
                                                                                        AdminSurveySchID = scheduledsurveys.AdminSurveySchID,
                                                                                        AdminID = scheduledsurveys.AdminID,
                                                                                        CreatedOn = scheduledsurveys.CreatedOn,
                                                                                        RepeatID = scheduledsurveys.RepeatID,
                                                                                        RepeatInterval = r.RepeatInterval,
                                                                                        ScheduleDate = scheduledsurveys.ScheduleDate,
                                                                                        SlotID = scheduledsurveys.SlotID,
                                                                                        SlotName = p.SlotName,
                                                                                        SurveyID = scheduledsurveys.SurveyID,
                                                                                        SurveyName = q.SurveyName,
                                                                                        Time = scheduledsurveys.Time,
                                                                                    }).ToList();
                foreach (var item in enumSurveyScheduleList)
                {
                    if (item.Time != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(item.Time);
                        var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        item.SlotTimeStamp = Timestamp.ToString();
                    }
                    item.SlotTimeOptions = GetSlotTimeOptionsForSurvey(item.AdminSurveySchID);
                }

                List<AdminSurveyScheduleViewModel> surveyList = new List<AdminSurveyScheduleViewModel>();
                if (model.SearchId != null)
                    surveyList = enumSurveyScheduleList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.SurveyName).StartsWith(model.SearchId));

                else
                    surveyList = enumSurveyScheduleList.ToList();

                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "SurveyName" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "SurveyName":
                        if (sortDirection == "asc")
                            surveyList = surveyList.OrderBy(c => CryptoUtil.DecryptInfo(c.SurveyName)).ToList();
                        else
                            surveyList = surveyList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.SurveyName)).ToList();
                        break;
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            surveyList = surveyList.OrderBy(c => c.CreatedOn).ToList();
                        else
                            surveyList = surveyList.OrderByDescending(c => c.CreatedOn).ToList();
                        break;

                    default:
                        surveyList = surveyList.OrderBy(c => CryptoUtil.DecryptInfo(c.SurveyName)).ToList();
                        break;
                }

                response.AdminSurveyScheduleViewModelList = surveyList;
                response.TotalRows = surveyList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetSurveyScheduledList: " + ex);
                response = new ScheduleSurveyListViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// GetGameScheduledList
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScheduleGameListViewModel GetGameScheduledList(ScheduleGameListViewModel model)
        {
            var response = new ScheduleGameListViewModel();
            try
            {
                IEnumerable<AdminCTestScheduleViewModel> enumGameScheduleList = (from scheduledgames in _UnitOfWork.IAdminCTestScheduleRepository.RetrieveAll()
                                                                                 join slots in _UnitOfWork.ISlotRepository.RetrieveAll() on scheduledgames.SlotID equals slots.SlotID into ps
                                                                                 from p in ps.DefaultIfEmpty()
                                                                                 join games in _UnitOfWork.ICTestRepository.RetrieveAll() on scheduledgames.CTestID equals games.CTestID into qs
                                                                                 from q in qs.DefaultIfEmpty()
                                                                                 join repeats in _UnitOfWork.IRepeatRepository.RetrieveAll() on scheduledgames.RepeatID equals repeats.RepeatID into rs
                                                                                 from r in rs.DefaultIfEmpty()
                                                                                 where scheduledgames.IsDeleted != true && scheduledgames.AdminID == model.LoggedInAdminId
                                                                                 select new AdminCTestScheduleViewModel
                                                                                 {
                                                                                     AdminCTestSchID = scheduledgames.AdminCTestSchID,
                                                                                     AdminID = scheduledgames.AdminID,
                                                                                     CreatedOn = scheduledgames.CreatedOn,
                                                                                     RepeatID = scheduledgames.RepeatID,
                                                                                     RepeatInterval = r.RepeatInterval,
                                                                                     ScheduleDate = scheduledgames.ScheduleDate,
                                                                                     SlotID = scheduledgames.SlotID,
                                                                                     SlotName = p.SlotName,
                                                                                     Version = scheduledgames.Version,
                                                                                     CTestID = scheduledgames.CTestID,
                                                                                     CTestName = q.CTestName,
                                                                                     Time = scheduledgames.Time
                                                                                 }).ToList();
                foreach (var item in enumGameScheduleList)
                {
                    if (item.Time != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(item.Time);
                        var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        item.SlotTimeStamp = Timestamp.ToString();
                    }
                    item.SlotTimeOptions = GetSlotTimeOptionsForGame(item.AdminCTestSchID);
                }

                List<AdminCTestScheduleViewModel> gameList = new List<AdminCTestScheduleViewModel>();
                if (model.SearchId != null)
                    gameList = enumGameScheduleList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.CTestName).StartsWith(model.SearchId));

                else
                    gameList = enumGameScheduleList.ToList();


                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "GameName" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "GameName":
                        if (sortDirection == "asc")
                            gameList = gameList.OrderBy(c => CryptoUtil.DecryptInfo(c.CTestName)).ToList();
                        else
                            gameList = gameList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.CTestName)).ToList();
                        break;
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            gameList = gameList.OrderBy(c => c.CreatedOn).ToList();
                        else
                            gameList = gameList.OrderByDescending(c => c.CreatedOn).ToList();
                        break;

                    default:
                        gameList = gameList.OrderBy(c => CryptoUtil.DecryptInfo(c.CTestName)).ToList();
                        break;
                }

                response.AdminCTestScheduleViewModelList = gameList;
                response.TotalRows = gameList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetGameScheduledList: " + ex);
                response = new ScheduleGameListViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Gets the type of the jewels trails settings by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="LoggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public JewelsTrailsSettings GetJewelsTrailsSettingsByType(string type, long LoggedInUserId)
        {
            JewelsTrailsSettings JewelsTrailsSettings = new JewelsTrailsSettings();
            if (type == "1")
            {
                Admin_JewelsTrailsASettings settings = _UnitOfWork.IAdminJewelsTrailsASettingsRepository.SingleOrDefault(u => u.AdminID == LoggedInUserId);
                if (settings != null)
                {
                    JewelsTrailsSettings.AdminJTASettingID = settings.AdminJTASettingID;
                    JewelsTrailsSettings.NoOfSeconds_Adv = settings.NoOfSeconds_Adv;
                    JewelsTrailsSettings.NoOfSeconds_Beg = settings.NoOfSeconds_Beg;
                    JewelsTrailsSettings.NoOfSeconds_Exp = settings.NoOfSeconds_Exp;
                    JewelsTrailsSettings.NoOfSeconds_Int = settings.NoOfSeconds_Int;
                    JewelsTrailsSettings.NoOfDiamonds = settings.NoOfDiamonds;
                    JewelsTrailsSettings.NoOfShapes = settings.NoOfShapes;
                    JewelsTrailsSettings.NoOfBonusPoints = settings.NoOfBonusPoints;
                    JewelsTrailsSettings.X_NoOfChangesInLevel = settings.X_NoOfChangesInLevel;
                    JewelsTrailsSettings.X_NoOfDiamonds = settings.X_NoOfDiamonds;
                    JewelsTrailsSettings.Y_NoOfChangesInLevel = settings.Y_NoOfChangesInLevel;
                    JewelsTrailsSettings.Y_NoOfShapes = settings.Y_NoOfShapes;
                }
            }
            else if (type == "2")
            {
                Admin_JewelsTrailsBSettings settings = _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.SingleOrDefault(u => u.AdminID == LoggedInUserId);
                if (settings != null)
                {
                    JewelsTrailsSettings.AdminJTBSettingID = settings.AdminJTBSettingID;
                    JewelsTrailsSettings.NoOfSeconds_Adv = settings.NoOfSeconds_Adv;
                    JewelsTrailsSettings.NoOfSeconds_Beg = settings.NoOfSeconds_Beg;
                    JewelsTrailsSettings.NoOfSeconds_Exp = settings.NoOfSeconds_Exp;
                    JewelsTrailsSettings.NoOfSeconds_Int = settings.NoOfSeconds_Int;
                    JewelsTrailsSettings.NoOfDiamonds = settings.NoOfDiamonds;
                    JewelsTrailsSettings.NoOfShapes = settings.NoOfShapes;
                    JewelsTrailsSettings.NoOfBonusPoints = settings.NoOfBonusPoints;
                    JewelsTrailsSettings.X_NoOfChangesInLevel = settings.X_NoOfChangesInLevel;
                    JewelsTrailsSettings.X_NoOfDiamonds = settings.X_NoOfDiamonds;
                    JewelsTrailsSettings.Y_NoOfChangesInLevel = settings.Y_NoOfChangesInLevel;
                    JewelsTrailsSettings.Y_NoOfShapes = settings.Y_NoOfShapes;
                }

            }
            return JewelsTrailsSettings;
        }

        /// <summary>
        /// Gets the distraction survey details.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        public DistractionSurveyViewModel GetDistractionSurveyDetails(DistractionSurveyViewModel DistractionSurveyViewModel)
        {
            var distractionSurveyViewModelResponse = new DistractionSurveyViewModel();
            try
            {
                long adminId = DistractionSurveyViewModel.LoggedInUserId;
                distractionSurveyViewModelResponse.LoggedInUserId = DistractionSurveyViewModel.LoggedInUserId;
                long Type = distractionSurveyViewModelResponse.JewelsTrailsSettings.JewelsTrailsSettingsType = DistractionSurveyViewModel.JewelsTrailsSettings.JewelsTrailsSettingsType;
                if (Type == 1)
                {
                    Admin_JewelsTrailsASettings settings = null;
                    settings = _UnitOfWork.IAdminJewelsTrailsASettingsRepository.SingleOrDefault(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId);
                    if (settings != null)
                    {
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.AdminJTASettingID = settings.AdminJTASettingID;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Adv = settings.NoOfSeconds_Adv;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Beg = settings.NoOfSeconds_Beg;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Exp = settings.NoOfSeconds_Exp;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Int = settings.NoOfSeconds_Int;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfDiamonds = settings.NoOfDiamonds;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfShapes = settings.NoOfShapes;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfBonusPoints = settings.NoOfBonusPoints;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.X_NoOfChangesInLevel = settings.X_NoOfChangesInLevel;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.X_NoOfDiamonds = settings.X_NoOfDiamonds;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Y_NoOfChangesInLevel = settings.Y_NoOfChangesInLevel;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Y_NoOfShapes = settings.Y_NoOfShapes;
                        distractionSurveyViewModelResponse.LoggedInUserId = (long)settings.AdminID;

                    }

                }
                else if (Type == 2)
                {
                    Admin_JewelsTrailsBSettings settings = null;
                    settings = _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.SingleOrDefault(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId);
                    if (settings != null)
                    {
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.AdminJTASettingID = settings.AdminJTBSettingID;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Adv = settings.NoOfSeconds_Adv;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Beg = settings.NoOfSeconds_Beg;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Exp = settings.NoOfSeconds_Exp;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfSeconds_Int = settings.NoOfSeconds_Int;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfDiamonds = settings.NoOfDiamonds;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfShapes = settings.NoOfShapes;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.NoOfBonusPoints = settings.NoOfBonusPoints;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.X_NoOfChangesInLevel = settings.X_NoOfChangesInLevel;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.X_NoOfDiamonds = settings.X_NoOfDiamonds;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Y_NoOfChangesInLevel = settings.Y_NoOfChangesInLevel;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Y_NoOfShapes = settings.Y_NoOfShapes;
                        distractionSurveyViewModelResponse.LoggedInUserId = (long)settings.AdminID;

                    }
                }
                IQueryable<CTestViewModel> enumCTestViewModelList = null;
                enumCTestViewModelList = (from cTest in _UnitOfWork.ICTestRepository.RetrieveAll()
                                          select new CTestViewModel
                                          {
                                              CTestID = cTest.CTestID,
                                              CTestName = cTest.CTestName,
                                              IsDistractionSurveyRequired = cTest.IsDistractionSurveyRequired
                                          });
                distractionSurveyViewModelResponse.CTestViewModelList = enumCTestViewModelList.ToList();
                var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                distractionSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString(), Selected = true }).ToList();

                IQueryable<Admin_CTestSurveySettingsViewModel> enumAdminCTestSurveySettingsList = null;
                enumAdminCTestSurveySettingsList = (from cTest in _UnitOfWork.IAdminCTestSurveyRepository.RetrieveAll().Where(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId)
                                                    select new Admin_CTestSurveySettingsViewModel
                                                    {
                                                        AdminCTestSurveySettingID = cTest.AdminCTestSurveySettingID,
                                                        AdminID = cTest.AdminID,
                                                        CTestID = cTest.CTestID,
                                                        SurveyID = cTest.SurveyID
                                                    });
                var adminCtestSurveySettings = enumAdminCTestSurveySettingsList.ToList();

                var surveySettingsQuery = from userSettings in adminCtestSurveySettings
                                          join surveys in surveyList on userSettings.SurveyID equals surveys.SurveyID
                                          where surveys.IsDeleted != true
                                          select new Admin_CTestSurveySettingsViewModel
                                          {
                                              AdminCTestSurveySettingID = userSettings.AdminCTestSurveySettingID,
                                              AdminID = userSettings.AdminID,
                                              CTestID = userSettings.CTestID,
                                              SurveyID = userSettings.SurveyID
                                          };
                var surveySettings = surveySettingsQuery;
                var list = enumCTestViewModelList.ToList();
                foreach (var item in list.Where(u => u.IsDistractionSurveyRequired == true))
                {
                    item.SurveyArray = surveySettings.Where(s => s.CTestID == item.CTestID).Select(i => i.SurveyID.ToString()).ToArray();

                }
                distractionSurveyViewModelResponse.CTestViewModelList = list;
                if (adminId != 0)
                {
                    Admin_Settings settingsA = _UnitOfWork.IAdminSettingsRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();
                    if (settingsA != null)
                    {
                        distractionSurveyViewModelResponse.ExpiryOptionId = (long)settingsA.ReminderClearInterval;
                    }
                    else
                    {
                        distractionSurveyViewModelResponse.ExpiryOptionId = (long)ReminderClearInterval.None;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetDistractionSurveyDetails: " + ex);
                distractionSurveyViewModelResponse = new DistractionSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return distractionSurveyViewModelResponse;
        }

        /// <summary>
        /// Saves the distraction survey details.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        public DistractionSurveyViewModel SaveDistractionSurveyDetails(DistractionSurveyViewModel DistractionSurveyViewModel)
        {
            var distractionSurveyViewModelResponse = new DistractionSurveyViewModel();
            try
            {
                distractionSurveyViewModelResponse.LoggedInUserId = DistractionSurveyViewModel.LoggedInUserId;
                distractionSurveyViewModelResponse.CTestViewModelList = DistractionSurveyViewModel.CTestViewModelList;

                //Save Code
                var cTestList = DistractionSurveyViewModel.CTestViewModelList.ToList();
                foreach (var ctestItem in cTestList)
                {

                    var adminCTestSurveyList = _UnitOfWork.IAdminCTestSurveyRepository.RetrieveAll().Where(u => u.CTestID == ctestItem.CTestID && u.AdminID == DistractionSurveyViewModel.LoggedInUserId).ToList();
                    if (adminCTestSurveyList.Count() > 0)
                    {//if exixts  delete it 
                        foreach (var adminCTestSurveyListItem in adminCTestSurveyList)
                        {
                            Admin_CTestSurveySettings adminCTestSurvey = new Admin_CTestSurveySettings();
                            adminCTestSurvey = _UnitOfWork.IAdminCTestSurveyRepository.RetrieveAll().Where(u => u.AdminCTestSurveySettingID == adminCTestSurveyListItem.AdminCTestSurveySettingID).SingleOrDefault();
                            _UnitOfWork.IAdminCTestSurveyRepository.Delete(adminCTestSurvey);
                            _UnitOfWork.Commit();
                        }
                    }
                    //create new
                    if (ctestItem.SurveyArray != null)
                    {
                        long[] SurveyArrayList = ctestItem.SurveyArray.Select(long.Parse).ToArray();
                        for (int i = 0; i < SurveyArrayList.Length; i++)
                        {
                            Admin_CTestSurveySettings adminCTestSurveySettings = new Admin_CTestSurveySettings();
                            adminCTestSurveySettings.AdminID = distractionSurveyViewModelResponse.LoggedInUserId;
                            adminCTestSurveySettings.CTestID = ctestItem.CTestID;
                            adminCTestSurveySettings.SurveyID = SurveyArrayList[i];
                            _UnitOfWork.IAdminCTestSurveyRepository.Add(adminCTestSurveySettings);
                            _UnitOfWork.Commit();
                        }
                    }
                }
                distractionSurveyViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                distractionSurveyViewModelResponse.Message = ResourceHelper.GetStringResource(LAMPConstants.DISTRACTION_SURVEY_SAVED_SUCCESSFULLY);
                distractionSurveyViewModelResponse.IsSaved = true;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveDistractionSurveyDetails: " + ex);
                distractionSurveyViewModelResponse = new DistractionSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return distractionSurveyViewModelResponse;
        }

        /// <summary>
        /// Saves the expiry option.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        public DistractionSurveyViewModel SaveExpiryOption(DistractionSurveyViewModel DistractionSurveyViewModel)
        {
            var distractionSurveyViewModelResponse = new DistractionSurveyViewModel();
            try
            {
                long adminId = DistractionSurveyViewModel.LoggedInUserId;
                distractionSurveyViewModelResponse.LoggedInUserId = DistractionSurveyViewModel.LoggedInUserId;
                distractionSurveyViewModelResponse = GetDistractionSurveyDetails(DistractionSurveyViewModel);
                if (adminId != 0)
                {
                    Admin_Settings settingsA = _UnitOfWork.IAdminSettingsRepository.GetAll().Where(s => s.AdminID == adminId).SingleOrDefault();
                    if (settingsA != null)
                    {
                        settingsA.AdminID = adminId;
                        settingsA.ReminderClearInterval = DistractionSurveyViewModel.ExpiryOptionId;
                        _UnitOfWork.IAdminSettingsRepository.Update(settingsA);
                        distractionSurveyViewModelResponse.AdminSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.AdminSettings.Message = "Expiry option updated sucessfully.";
                        distractionSurveyViewModelResponse.AdminSettings.IsSaved = true;
                    }
                    else
                    {
                        settingsA = new Admin_Settings();
                        settingsA.AdminID = adminId;
                        settingsA.ReminderClearInterval = DistractionSurveyViewModel.ExpiryOptionId;
                        _UnitOfWork.IAdminSettingsRepository.Add(settingsA);
                        distractionSurveyViewModelResponse.AdminSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.AdminSettings.Message = "Expiry option saved sucessfully.";
                        distractionSurveyViewModelResponse.AdminSettings.IsSaved = true;
                    }
                    _UnitOfWork.Commit();
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveExpiryOption: " + ex);
                distractionSurveyViewModelResponse = new DistractionSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return distractionSurveyViewModelResponse;
        }

        /// <summary>
        /// Saves the jewels trials settings.
        /// </summary>
        /// <param name="DistractionSurveyViewModel">The distraction survey view model.</param>
        /// <returns></returns>
        public DistractionSurveyViewModel SaveJewelsTrialsSettings(DistractionSurveyViewModel DistractionSurveyViewModel)
        {
            var distractionSurveyViewModelResponse = new DistractionSurveyViewModel();
            try
            {
                distractionSurveyViewModelResponse.LoggedInUserId = DistractionSurveyViewModel.LoggedInUserId;
                distractionSurveyViewModelResponse = GetDistractionSurveyDetails(DistractionSurveyViewModel);
                long Type = distractionSurveyViewModelResponse.JewelsTrailsSettings.JewelsTrailsSettingsType = DistractionSurveyViewModel.JewelsTrailsSettings.JewelsTrailsSettingsType;
                if (Type == 1)
                {
                    Admin_JewelsTrailsASettings settings = _UnitOfWork.IAdminJewelsTrailsASettingsRepository.SingleOrDefault(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId);// u.AdminJTASettingID == DistractionSurveyViewModel.JewelsTrailsSettings.AdminJTASettingID &&
                    if (settings != null)
                    {
                        settings.AdminJTASettingID = DistractionSurveyViewModel.JewelsTrailsSettings.AdminJTASettingID;
                        settings.NoOfSeconds_Adv = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Adv;
                        settings.NoOfSeconds_Beg = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Beg;
                        settings.NoOfSeconds_Exp = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Exp;
                        settings.NoOfSeconds_Int = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Int;
                        settings.NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfDiamonds;
                        settings.NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfShapes;
                        settings.NoOfBonusPoints = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfBonusPoints;
                        settings.X_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfChangesInLevel;
                        settings.X_NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfDiamonds;
                        settings.Y_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfChangesInLevel;
                        settings.Y_NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfShapes;
                        settings.AdminID = DistractionSurveyViewModel.LoggedInUserId;
                        _UnitOfWork.IAdminJewelsTrailsASettingsRepository.Update(settings);

                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Message = "Jewels A Settings saved sucessfully.";
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.IsSaved = true;
                    }
                    else
                    {
                        settings = new Admin_JewelsTrailsASettings();
                        settings.NoOfSeconds_Adv = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Adv;
                        settings.NoOfSeconds_Beg = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Beg;
                        settings.NoOfSeconds_Exp = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Exp;
                        settings.NoOfSeconds_Int = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Int;
                        settings.NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfDiamonds;
                        settings.NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfShapes;
                        settings.NoOfBonusPoints = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfBonusPoints;
                        settings.X_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfChangesInLevel;
                        settings.X_NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfDiamonds;
                        settings.Y_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfChangesInLevel;
                        settings.Y_NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfShapes;
                        settings.AdminID = DistractionSurveyViewModel.LoggedInUserId;
                        _UnitOfWork.IAdminJewelsTrailsASettingsRepository.Add(settings);
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Message = "Jewels A Settings saved sucessfully.";
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.IsSaved = true;
                    }
                }
                else
                {
                    Admin_JewelsTrailsBSettings settings = _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.SingleOrDefault(u => u.AdminID == DistractionSurveyViewModel.LoggedInUserId);//u.AdminJTBSettingID == DistractionSurveyViewModel.JewelsTrailsSettings.AdminJTBSettingID &&
                    if (settings != null)
                    {
                        settings.AdminJTBSettingID = DistractionSurveyViewModel.JewelsTrailsSettings.AdminJTBSettingID;
                        settings.NoOfSeconds_Adv = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Adv;
                        settings.NoOfSeconds_Beg = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Beg;
                        settings.NoOfSeconds_Exp = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Exp;
                        settings.NoOfSeconds_Int = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Int;
                        settings.NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfDiamonds;
                        settings.NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfShapes;
                        settings.NoOfBonusPoints = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfBonusPoints;
                        settings.X_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfChangesInLevel;
                        settings.X_NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfDiamonds;
                        settings.Y_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfChangesInLevel;
                        settings.Y_NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfShapes;
                        settings.AdminID = DistractionSurveyViewModel.LoggedInUserId;
                        _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.Update(settings);
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Message = "Jewels B Settings saved sucessfully.";
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.IsSaved = true;
                    }
                    else
                    {
                        settings = new Admin_JewelsTrailsBSettings();
                        settings.NoOfSeconds_Adv = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Adv;
                        settings.NoOfSeconds_Beg = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Beg;
                        settings.NoOfSeconds_Exp = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Exp;
                        settings.NoOfSeconds_Int = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfSeconds_Int;
                        settings.NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfDiamonds;
                        settings.NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfShapes;
                        settings.NoOfBonusPoints = DistractionSurveyViewModel.JewelsTrailsSettings.NoOfBonusPoints;
                        settings.X_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfChangesInLevel;
                        settings.X_NoOfDiamonds = DistractionSurveyViewModel.JewelsTrailsSettings.X_NoOfDiamonds;
                        settings.Y_NoOfChangesInLevel = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfChangesInLevel;
                        settings.Y_NoOfShapes = DistractionSurveyViewModel.JewelsTrailsSettings.Y_NoOfShapes;
                        settings.AdminID = DistractionSurveyViewModel.LoggedInUserId;
                        _UnitOfWork.IAdminJewelsTrailsBSettingsRepository.Add(settings);
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Status = LAMPConstants.SUCCESS_CODE;
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.Message = "Jewels B Settings saved sucessfully.";
                        distractionSurveyViewModelResponse.JewelsTrailsSettings.IsSaved = true;
                    }
                }
                _UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveJewelsTrialsSettings: " + ex);
                distractionSurveyViewModelResponse = new DistractionSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return distractionSurveyViewModelResponse;
        }


        /// <summary>
        /// Save Protocol Date and time
        /// </summary>
        /// <param name="protocolModel"></param>
        /// <returns></returns>
        public ViewModelBase SaveProtocolDate(ProtocolViewModel protocolModel)
        {
            var BaseViewModel = new ViewModelBase();
            try
            {
                long UserId = protocolModel.UserId;
                DateTime dt1;
                DateTime dt2;
                dt1 = DateTime.ParseExact(protocolModel.DatePart, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                dt2 = Convert.ToDateTime(protocolModel.TimePart).ToUniversalTime();
                if (UserId != 0)
                {
                    UserSetting settingsU = _UnitOfWork.IUserSettingRepository.GetAll().Where(s => s.UserID == UserId).SingleOrDefault();
                    if (settingsU != null)
                    {
                        settingsU.ProtocolDate = dt1.Add(dt2.TimeOfDay);
                        _UnitOfWork.IUserSettingRepository.Update(settingsU);
                        BaseViewModel.Status = LAMPConstants.SUCCESS_CODE;
                        BaseViewModel.Message = "Expiry option updated sucessfully.";
                    }
                    else
                    {
                        settingsU = new UserSetting();
                        settingsU.UserID = UserId;
                        settingsU.ProtocolDate = dt1.Add(dt2.TimeOfDay);
                        _UnitOfWork.IUserSettingRepository.Add(settingsU);
                        BaseViewModel.Status = LAMPConstants.SUCCESS_CODE;
                        BaseViewModel.Message = "Expiry option updated sucessfully.";
                    }
                    _UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/SaveProtocolDate: " + ex);
            }
            return BaseViewModel;
        }

        /// <summary>
        /// Get Protocol Date
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ProtocolViewModel getProtocolDate(long UserId)
        {
            var ProtocolViewModel = new ProtocolViewModel();
            try
            {
                if (UserId != 0)
                {
                    UserSetting settingsU = _UnitOfWork.IUserSettingRepository.GetAll().Where(s => s.UserID == UserId).SingleOrDefault();
                    if (settingsU != null)
                    {
                        if (settingsU.ProtocolDate != null && settingsU.ProtocolDate != DateTime.MinValue)
                        {
                            DateTime DisplayTime = (DateTime)(settingsU.ProtocolDate);
                            var Timestamp = DisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                            ProtocolViewModel.TimePart = Timestamp.ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetProtocolDate: " + ex);
            }
            return ProtocolViewModel;
        }
        #endregion

        #region Users

        /// <summary>
        /// Gets the user export detail list.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        public UserDataExportViewModel GetUserExportDetailList(string userIds, string fromDate, string toDate)
        {
            List<string> newUserIds = userIds.Split(',').ToList<string>();
            UserDataExportViewModel response = new UserDataExportViewModel();
            XElement xmlElements = new XElement("UserIDs", newUserIds.Select(i => new XElement("UserID", i)));
            DateTime dt1 = DateTime.ParseExact(fromDate, @"dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(toDate, @"dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
            dt2 = dt2.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            dt1 = dt1.AddMinutes(offsetValue * -1);
            dt2 = dt2.AddMinutes(offsetValue * -1);
            int errorcode = 0;
            try
            {
                var context = new LAMPEntities();
                {
                    var command = context.Database.Connection.CreateCommand();

                    command.CommandText = "Admin_GetUserDataToExport_sp";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@p_UserIDsXML", xmlElements.ToString()));
                    command.Parameters.Add(new SqlParameter("@p_DateFrom", dt1));
                    command.Parameters.Add(new SqlParameter("@p_DateTo", dt2));
                    var outputErrorParameter = new SqlParameter("@p_ErrID", 0) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(outputErrorParameter);
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    if (outputErrorParameter.Value != null)
                    {
                        errorcode = Convert.ToInt32(outputErrorParameter.Value);
                    }
                    else
                    {
                        ////________________________User Details___________________///
                        var _userDetails = ((IObjectContextAdapter)context).ObjectContext.Translate<UserDetails>(reader).ToList();
                        response.UserDetails = _userDetails.Select(s =>
                        {
                            s.UserID = s.UserID;
                            s.StudyId = CryptoUtil.DecryptInfo(s.StudyId);
                            s.Email = CryptoUtil.DecryptInfo(s.Email);
                            s.FirstName = CryptoUtil.DecryptInfo(s.FirstName);
                            s.LastName = CryptoUtil.DecryptInfo(s.LastName);
                            s.LastSurveyDate = s.LastSurveyDate;
                            s.LastSurveyDateString = (s.LastSurveyDate != null) ? s.LastSurveyDate.Value.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture) : "";

                            s.Phone = CryptoUtil.DecryptInfo(s.Phone);
                            s.RegisteredOn = s.RegisteredOn;
                            return s;
                        }).ToList();
                        ////________________________User Survey Details___________________///
                        if (reader.NextResult())
                        {
                            var _surveyHeader = ((IObjectContextAdapter)context).ObjectContext.Translate<SurveyHeader>(reader).ToList();
                            response.SurveyHeader = _surveyHeader.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.TimeTaken = s.TimeTaken;
                                s.SurveyResultID = s.SurveyResultID;
                                s.SurveyName = CryptoUtil.DecryptInfo(s.SurveyName);
                                s.CreatedOn = s.CreatedOn;

                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.CreatedOn_string = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.Duration = s.EndTime.Subtract(s.StartTime);

                                s.IsDistraction = s.IsDistraction;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore != null ? CryptoUtil.DecryptInfo(s.SpinWheelScore) : "-";
                                return s;
                            }).ToList();

                        }
                        if (reader.NextResult())
                        {
                            var _surveys = ((IObjectContextAdapter)context).ObjectContext.Translate<SurveyExport>(reader).ToList();
                            response.Surveys = _surveys.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.SurveyResultID = s.SurveyResultID;
                                s.SurveyResultDtlID = s.SurveyResultDtlID;
                                s.Question = CryptoUtil.DecryptInfo(s.Question);
                                s.TimeTaken = s.TimeTaken;
                                if (s.TimeTaken.ToString().Length > 0)
                                {
                                    s.Time_Taken = TimeSpan.FromSeconds((double)s.TimeTaken).TotalMilliseconds.ToString();
                                }
                                s.EnteredAnswer = CryptoUtil.DecryptInfo(s.EnteredAnswer);
                                s.CorrectAnswer = CryptoUtil.DecryptInfo(s.CorrectAnswer);
                                s.ClickRange = CryptoUtil.DecryptInfo(s.ClickRange);
                                return s;
                            }).ToList();
                        }
                        ////_______________________User_Games__Results______________///
                        if (reader.NextResult())
                        {
                            DateTime currentDate = DateTime.UtcNow;
                            TimeSpan spVal = currentDate.Subtract(currentDate);
                            var _cognitionTestHeader = ((IObjectContextAdapter)context).ObjectContext.Translate<CognitionTestHeader>(reader).ToList();
                            response.CognitionTestHeader = _cognitionTestHeader.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.TotalPoints = s.TotalPoints;
                                s.TotalGames = s.TotalGames;
                                s.TimeTaken = s.TimeTaken;
                                if (s.TimeTaken != null && s.TimeTaken.ToString().Length > 0)
                                {
                                    s.Time_Taken = TimeSpan.FromSeconds((double)s.TimeTaken).TotalMilliseconds.ToString();
                                }

                                s.LastTestDateString = s.LastTestStartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.CTestID = s.CTestID;
                                s.CTestName = CryptoUtil.DecryptInfo(s.CTestName);
                                s.LastTestStartTime = (DateTime)s.LastTestStartTime;
                                s.LastTestEndTime = s.LastTestEndTime;  // (DateTime)s.LastTestEndTime;
                                s.Duration = s.LastTestEndTime != null ? Convert.ToDateTime(s.LastTestEndTime).Subtract(s.LastTestStartTime) : spVal;
                                s.Duration_string = s.Duration.ToString();
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var nbackTestResult = ((IObjectContextAdapter)context).ObjectContext.Translate<NBackCTest>(reader).ToList();
                            response.NBackCTestList = nbackTestResult.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalQuestions = s.TotalQuestions;
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.Version = s.Version;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var trialsBTestResult = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBCTest>(reader).ToList();
                            response.TrailsBCTestList = trialsBTestResult.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TrailsBResultID = s.TrailsBResultID;
                                s.UserId = s.UserId;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            response.TrailsBCTestDetailList = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBCTestDetail>(reader).ToList();
                        }

                        if (response.TrailsBCTestList != null)
                        {
                            foreach (var trail in response.TrailsBCTestList)
                            {
                                var sequence = response.TrailsBCTestDetailList.Where(s => s.TrailsBResultID == trail.TrailsBResultID).Select(s => new { s.Alphabet, s.Sequence }).ToList();

                                var sequenceNumber = sequence.Select(s => s.Sequence).Distinct().ToList();
                                string details = string.Empty;

                                foreach (var sq in sequenceNumber)
                                {
                                    var alphabets = sequence.Where(s => s.Sequence == sq).Select(s => s.Alphabet).ToList();

                                    foreach (var alphabet in alphabets)
                                    {
                                        details = details + alphabet + "->";
                                    }
                                    var lastindex = details.LastIndexOf("-");
                                    details = details.Remove(lastindex);
                                    details = details + ",";
                                }
                                details.TrimEnd(',');
                                trail.DetailString = details;
                            }
                        }

                        if (reader.NextResult())
                        {
                            var spatialForwardList = ((IObjectContextAdapter)context).ObjectContext.Translate<SpatialCTestForward>(reader).ToList();
                            response.SpatialCTestForwardList = spatialForwardList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;

                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;

                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var spatialBackwardList = ((IObjectContextAdapter)context).ObjectContext.Translate<SpatialCTestBackward>(reader).ToList();
                            response.SpatialCTestBackwardList = spatialBackwardList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var simpleMemoryList = ((IObjectContextAdapter)context).ObjectContext.Translate<SimpleMemoryCTest>(reader).ToList();
                            response.SimpleMemoryCTestList = simpleMemoryList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.Version = s.Version;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalQuestions = s.TotalQuestions;
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var serial7List = ((IObjectContextAdapter)context).ObjectContext.Translate<Serial7CTest>(reader).ToList();
                            response.Serial7CTestList = serial7List.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.Version = s.Version;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TotalQuestions = s.TotalQuestions;
                                s.UserId = s.UserId;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var _threeDFigureCTestList = ((IObjectContextAdapter)context).ObjectContext.Translate<ThreeDFigureCTest>(reader).ToList();
                            response.ThreeDFigureCTestList = _threeDFigureCTestList.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.ActualImageFileName = s.FileName;
                                s.DrawnFigFileName = CryptoUtil.DecryptInfo(s.DrawnFigFileName);
                                s.FileName = s.FileName;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.CTestID = s.CTestID;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var visualAssociationList = ((IObjectContextAdapter)context).ObjectContext.Translate<VisualAssociationCTest>(reader).ToList();
                            response.VisualAssociationCTestList = visualAssociationList.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.Version = s.Version;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TotalQuestions = s.TotalQuestions;
                                s.UserId = s.UserId;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var digitSpanForwardList = ((IObjectContextAdapter)context).ObjectContext.Translate<DigitSpanCTestForward>(reader).ToList();
                            response.DigitSpanCTestForwardList = digitSpanForwardList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var catAndDogNewList = ((IObjectContextAdapter)context).ObjectContext.Translate<CatAndDogNewCTest>(reader).ToList();
                            response.CatAndDogNewCTestList = catAndDogNewList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var temporalOrderList = ((IObjectContextAdapter)context).ObjectContext.Translate<TemporalOrderCTest>(reader).ToList();
                            response.TemporalOrderCTestList = temporalOrderList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.Version = s.Version;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var digitSpanBackwardList = ((IObjectContextAdapter)context).ObjectContext.Translate<DigitSpanCTestBackward>(reader).ToList();
                            response.DigitSpanCTestBackwardList = digitSpanBackwardList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }

                        if (reader.NextResult())
                        {
                            var nBackNewList = ((IObjectContextAdapter)context).ObjectContext.Translate<NBackNewCTest>(reader).ToList();
                            response.NBackNewCTestList = nBackNewList.Select(s =>
                            {
                                s.CorrectAnswers = s.CorrectAnswers;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.UserId = s.UserId;
                                s.WrongAnswers = s.WrongAnswers;
                                s.TotalQuestions = s.TotalQuestions;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            var trailsBNewList = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBNewCTest>(reader).ToList();
                            response.TrailsBNewCTest = trailsBNewList.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.Version = s.Version;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TrailsBNewResultID = s.TrailsBNewResultID;
                                s.UserId = s.UserId;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            response.TrailsBNewCTestDetailList = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBNewCTestDetail>(reader).ToList();
                        }
                        if (response.TrailsBNewCTest != null)
                        {
                            foreach (var trail in response.TrailsBNewCTest)
                            {
                                var sequence = response.TrailsBNewCTestDetailList.Where(s => s.TrailsBNewResultID == trail.TrailsBNewResultID).Select(s => new { s.Alphabet, s.Sequence }).ToList();

                                var sequenceNumber = sequence.Select(s => s.Sequence).Distinct().ToList();
                                string details = string.Empty;

                                foreach (var sq in sequenceNumber)
                                {
                                    var alphabets = sequence.Where(s => s.Sequence == sq).Select(s => s.Alphabet).ToList();

                                    foreach (var alphabet in alphabets)
                                    {
                                        details = details + alphabet + "->";
                                    }
                                    var lastindex = details.LastIndexOf("-");
                                    details = details.Remove(lastindex);
                                    details = details + ",";
                                }
                                details.TrimEnd(',');
                                trail.DetailString = details;
                            }
                        }

                        if (reader.NextResult())
                        {
                            var trailsBDotTouchList = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBDotTouchCTest>(reader).ToList();
                            response.TrailsBDotTouchCTestList = trailsBDotTouchList.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TrailsBDotTouchResultID = s.TrailsBDotTouchResultID;
                                s.UserId = s.UserId;
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }

                        if (reader.NextResult())
                        {
                            response.TrailsBDotTouchCTestDetailList = ((IObjectContextAdapter)context).ObjectContext.Translate<TrailsBDotTouchCTestDetail>(reader).ToList();
                        }

                        if (response.TrailsBDotTouchCTestList != null)
                        {
                            foreach (var trail in response.TrailsBDotTouchCTestList)
                            {
                                var sequence = response.TrailsBDotTouchCTestDetailList.Where(s => s.TrailsBDotTouchResultID == trail.TrailsBDotTouchResultID).Select(s => new { s.Alphabet, s.Sequence }).ToList();

                                var sequenceNumber = sequence.Select(s => s.Sequence).Distinct().ToList();
                                string details = string.Empty;

                                foreach (var sq in sequenceNumber)
                                {
                                    var alphabets = sequence.Where(s => s.Sequence == sq).Select(s => s.Alphabet).ToList();

                                    foreach (var alphabet in alphabets)
                                    {
                                        details = details + alphabet + "->";
                                    }
                                    var lastindex = details.LastIndexOf("-");
                                    details = details.Remove(lastindex);
                                    details = details + ",";
                                }
                                details.TrimEnd(',');
                                trail.DetailString = details;
                            }
                        }

                        if (reader.NextResult())
                        {
                            var _jewelsTrailsACTestList = ((IObjectContextAdapter)context).ObjectContext.Translate<JewelsTrailsACTest>(reader).ToList();
                            response.JewelsTrailsACTestList = _jewelsTrailsACTestList.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.JewelsTrailsAResultID = s.JewelsTrailsAResultID;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TotalBonusCollected = CryptoUtil.DecryptInfo(s.TotalBonusCollected);
                                s.TotalJewelsCollected = CryptoUtil.DecryptInfo(s.TotalJewelsCollected);
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            response.JewelsTrailsACTestDetailList = ((IObjectContextAdapter)context).ObjectContext.Translate<JewelsTrailsACTestDetail>(reader).ToList();
                        }

                        if (response.JewelsTrailsACTestList != null)
                        {
                            foreach (var trail in response.JewelsTrailsACTestList)
                            {
                                var sequence = response.JewelsTrailsACTestDetailList.Where(s => s.JewelsTrailsAResultID == trail.JewelsTrailsAResultID).Select(s => new { s.Alphabet, s.Sequence }).ToList();

                                var sequenceNumber = sequence.Select(s => s.Sequence).Distinct().ToList();
                                string details = string.Empty;

                                foreach (var sq in sequenceNumber)
                                {
                                    var alphabets = sequence.Where(s => s.Sequence == sq).Select(s => s.Alphabet).ToList();

                                    foreach (var alphabet in alphabets)
                                    {
                                        details = details + alphabet + "->";
                                    }
                                    var lastindex = details.LastIndexOf("-");
                                    details = details.Remove(lastindex);
                                    details = details + ",";
                                }
                                details = details.TrimEnd(',');
                                trail.DetailString = details;
                            }
                        }

                        if (reader.NextResult())
                        {
                            var _jewelsTrailsBCTestList = ((IObjectContextAdapter)context).ObjectContext.Translate<JewelsTrailsBCTest>(reader).ToList();
                            response.JewelsTrailsBCTestList = _jewelsTrailsBCTestList.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.CTestID = s.CTestID;
                                s.Duration = s.Duration;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.DurationTime = s.EndTime.Subtract(s.StartTime);
                                s.Duration_String = s.DurationTime.ToString();
                                s.JewelsTrailsBResultID = s.JewelsTrailsBResultID;
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.TotalAttempts = s.TotalAttempts;
                                s.TotalBonusCollected = CryptoUtil.DecryptInfo(s.TotalBonusCollected);
                                s.TotalJewelsCollected = CryptoUtil.DecryptInfo(s.TotalJewelsCollected);
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = s.SpinWheelScore;
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }
                        if (reader.NextResult())
                        {
                            response.JewelsTrailsBCTestDetailList = ((IObjectContextAdapter)context).ObjectContext.Translate<JewelsTrailsBCTestDetail>(reader).ToList();
                        }

                        if (response.JewelsTrailsBCTestList != null)
                        {
                            foreach (var trail in response.JewelsTrailsBCTestList)
                            {
                                var sequence = response.JewelsTrailsBCTestDetailList.Where(s => s.JewelsTrailsBResultID == trail.JewelsTrailsBResultID).Select(s => new { s.Alphabet, s.Sequence }).ToList();

                                var sequenceNumber = sequence.Select(s => s.Sequence).Distinct().ToList();
                                string details = string.Empty;

                                foreach (var sq in sequenceNumber)
                                {
                                    var alphabets = sequence.Where(s => s.Sequence == sq).Select(s => s.Alphabet).ToList();

                                    foreach (var alphabet in alphabets)
                                    {
                                        details = details + alphabet + "->";
                                    }
                                    var lastindex = details.LastIndexOf("-");
                                    details = details.Remove(lastindex);
                                    details = details + ",";
                                }
                                details.TrimEnd(',');
                                trail.DetailString = details;
                            }
                        }

                        // Scratch Image Game
                        if (reader.NextResult())
                        {
                            var _scratchImageCTestList = ((IObjectContextAdapter)context).ObjectContext.Translate<ScratchImageCTest>(reader).ToList();
                            response.ScratchImageCTestList = _scratchImageCTestList.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.UserId = s.UserId;
                                s.ScratchImageResultID = s.ScratchImageResultID;
                                s.DrawnFigFileName = CryptoUtil.DecryptInfo(s.DrawnFigFileName);
                                s.FileName = s.FileName;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = (DateTime)s.EndTime;
                                s.Duration = s.Duration;    
                                s.IsNotificationGame = s.IsNotificationGame;
                                s.SpinWheelScore = CryptoUtil.DecryptInfo(s.SpinWheelScore);
                                s.Duration_String = s.Duration.ToString();
                                s.StartTimeString = s.StartTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.AdminBatchSchID = s.AdminBatchSchID;
                                return s;
                            }).ToList();
                        }

                        // SpinWheel Game
                        if (reader.NextResult())
                        {
                            var _spinWheelCTestList = ((IObjectContextAdapter)context).ObjectContext.Translate<SpinWheelCTestDetail>(reader).ToList();
                            response.SpinWheelCTestList = _spinWheelCTestList.Select(s =>
                            {
                                s.CTestID = s.CTestID;
                                s.UserId = s.UserId;
                                s.SpinWheelResultID = s.SpinWheelResultID;
                                s.StartTime = (DateTime)s.StartTime;
                                s.EndTime = s.EndTime;
                                s.CollectedStars = CryptoUtil.DecryptInfo(s.CollectedStars);
                                s.Duration = s.Duration;   
                                s.Duration_String = s.Duration.ToString();
                                return s;
                            }).ToList();
                        }

                        //==================

                        if (reader.NextResult())
                        {
                            var _cognitionOverallPoints = ((IObjectContextAdapter)context).ObjectContext.Translate<CognitionOverallPoints>(reader).ToList();

                            response.CognitionOverallPoints = _cognitionOverallPoints.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.OverallRating = s.OverallRating;
                                s.LastResult = Convert.ToDateTime(s.LastResult);
                                s.LastResultString = s.LastResult.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                return s;
                            }).ToList();
                        }

                        ////________________________Help Call History___________________///
                        if (reader.NextResult())
                        {
                            var _callHistory = ((IObjectContextAdapter)context).ObjectContext.Translate<CallHistory>(reader).ToList();
                            response.CallHistory = _callHistory.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.CallDateTime = Convert.ToDateTime(s.CallDateTime);
                                s.CallDateTimeString = s.CallDateTime.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.CallDuraion = s.CallDuraion;
                                s.CalledNumber = CryptoUtil.DecryptInfo(s.CalledNumber);
                                s.CallType = s.CallType;
                                s.HelpCallID = s.HelpCallID;
                                return s;
                            }).ToList();
                        }

                        if (reader.NextResult())
                        {
                            var _locations = ((IObjectContextAdapter)context).ObjectContext.Translate<LocationExport>(reader).ToList();
                            response.Locations = _locations.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.Address = CryptoUtil.DecryptInfo(s.Address);
                                s.CreatedOn = Convert.ToDateTime(s.CreatedOn);
                                s.CreatedOnString = s.CreatedOn.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.LocationID = s.LocationID;
                                s.LocationName = CryptoUtil.DecryptInfo(s.LocationName);
                                s.Latitude = CryptoUtil.DecryptInfo(s.Latitude);
                                s.Longitude = CryptoUtil.DecryptInfo(s.Longitude);
                                return s;
                            }).ToList();
                        }


                        if (reader.NextResult())
                        {
                            var _environment = ((IObjectContextAdapter)context).ObjectContext.Translate<EnvironmentExport>(reader).ToList();
                            response.Environment = _environment.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.Address = CryptoUtil.DecryptInfo(s.Address);
                                s.CreatedOn = Convert.ToDateTime(s.CreatedOn);
                                s.CreatedOnString = s.CreatedOn.AddMinutes(offsetValue).ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                s.LocationID = s.LocationID;
                                s.LocationName = CryptoUtil.DecryptInfo(s.LocationName);

                                return s;
                            }).ToList();
                        }

                        if (reader.NextResult())
                        {
                            var _healthKitHeader = ((IObjectContextAdapter)context).ObjectContext.Translate<HealthKitHeader>(reader).ToList();
                            response.HealthKitHeader = _healthKitHeader.Select(s =>
                            {
                                s.UserId = s.UserId;
                                s.BloodType = CryptoUtil.DecryptInfo(s.BloodType);
                                s.DateOfBirth = s.DateOfBirth;

                                s.DOBString = (s.DateOfBirth != null) ? s.DateOfBirth.Value.AddMinutes(offsetValue).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : "";
                                s.HKBasicInfoID = s.HKBasicInfoID;
                                s.Sex = CryptoUtil.DecryptInfo(s.Sex);
                                return s;
                            }).ToList();

                        }
                        List<HealthKitData> lstHealthKitData = new List<HealthKitData>();
                        if (reader.NextResult())
                        {                            
                            var _healthKitData = ((IObjectContextAdapter)context).ObjectContext.Translate<HealthKitData>(reader).ToList();
                            lstHealthKitData = _healthKitData;                            
                        }
                        if (reader.NextResult())
                        {
                            var _healthKitData = ((IObjectContextAdapter)context).ObjectContext.Translate<HealthKitV2Data>(reader).ToList();
                            string _uniqueVal = "";
                            HealthKitData healthKitDataObj;
                            foreach (HealthKitV2Data obj in _healthKitData)
                            {

                                if (_uniqueVal == "" || _uniqueVal != (obj.UserId.ToString() + "|" + obj.DateTime.ToString()))
                                {
                                    _uniqueVal = obj.UserId.ToString() + "|" + obj.DateTime.ToString();
                                    healthKitDataObj = new HealthKitData();
                                    healthKitDataObj.UserId = obj.UserId;
                                    healthKitDataObj.CreatedOn = obj.DateTime;
                                    lstHealthKitData.Add(healthKitDataObj);
                                }

                                switch (obj.HKParamID)
                                {
                                    case (int)HealthKitParameter.Height:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Height = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.Weight:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Weight = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.HeartRate:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().HeartRate = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.BloodPressure:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().BloodPressure = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.RespiratoryRate:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().RespiratoryRate = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.Sleep:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Sleep = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.Steps:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Steps = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.FlightClimbed:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().FlightClimbed = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.Segment:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Segment = obj.Value;
                                        break;
                                    case (int)HealthKitParameter.Distance:
                                        lstHealthKitData.Where(h => h.UserId == obj.UserId && h.CreatedOn == obj.DateTime).FirstOrDefault().Distance = obj.Value;
                                        break;
                                }
                            }
                        }

                        DateTime _dt;
                        string _date = "";
                        string _time = "";
                        response.HealthKitData = lstHealthKitData.Select(s =>
                        {
                            _dt = s.CreatedOn.AddMinutes(offsetValue);
                            _date = _dt.ToString("MM/dd/yyyy");
                            _time = _dt.ToString("hh:mm tt");

                            s.UserId = s.UserId;                            
                            s.CreatedOn = Convert.ToDateTime(s.CreatedOn);
                            s.HKDailyValueID = s.HKDailyValueID;
                            s.CreatedOnString = _date;
                            s.BloodPressure = (s.BloodPressure != null && s.BloodPressure.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.BloodPressure)) : "";
                            s.FlightClimbed = (s.FlightClimbed != null && s.FlightClimbed.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.FlightClimbed)) : "";
                            s.HeartRate = (s.HeartRate != null && s.HeartRate.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.HeartRate)) : "";
                            s.Height = (s.Height != null && s.Height.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Height)) : "";
                            s.RespiratoryRate = (s.RespiratoryRate != null && s.RespiratoryRate.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.RespiratoryRate)) : "";
                            s.Sleep = (s.Sleep != null && s.Sleep.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Sleep)) : "";
                            s.Steps = (s.Steps != null && s.Steps.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Steps)) : "";
                            s.Weight = (s.Weight != null && s.Weight.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Weight)) : "";
                            s.Segment = (s.Segment != null && s.Segment.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Segment)) : "";
                            s.Distance = (s.Distance != null && s.Distance.Length > 0) ? (_time + " | " + CryptoUtil.DecryptInfo(s.Distance)) : "";
                            return s;
                        }).ToList();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserService - GetUserExportDetailList: " + ex);

            }
            return response;
        }

        /// <summary>
        /// Saves the user details
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public UserViewModel SaveUser(UserViewModel userModel, Stream stream)
        {
            var model = new UserViewModel();
            string userPassword = string.Empty;
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                UserDevice userDevice = null;
                User user = null;
                User validationUser = null;
                bool isCredentialchanged = true;  
                string encriptedStudyId = CryptoUtil.EncryptInfo(userModel.StudyId.Trim());
                user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == userModel.UserID);
                if (user == null)
                {
                    user = new User();
                    validationUser = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.StudyId == encriptedStudyId);
                    if (validationUser != null && validationUser.UserID > 0)
                    {
                        model.UserID = userModel.UserID;
                        model.Status = LAMPConstants.INVALID_STUDY_ID;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.INVALID_STUDY_ID);
                        return model;
                    }
                }
                else
                {
                    userPassword = CryptoUtil.DecryptStringWithKey(user.Password);
                    userDevice = _UnitOfWork.IUserDeviceRepository.SingleOrDefault(u => u.UserID == user.UserID);
                    if (userDevice != null && (userModel.Password == null || userModel.Password.Trim() == ""))
                    {
                        model.UserID = userModel.UserID;
                        model.Status = LAMPConstants.EMPTY_PASSWORD;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.EMPTY_PASSWORD);
                        return model;
                    }

                    validationUser = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.StudyId == encriptedStudyId && u.UserID != user.UserID);
                    if (validationUser != null && validationUser.UserID > 0)
                    {
                        model.UserID = userModel.UserID;
                        model.Status = LAMPConstants.INVALID_STUDY_ID;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.INVALID_STUDY_ID);
                        return model;
                    }
                    string oldCredential = user.Email + "|" + user.Password + "|" + user.StudyId;
                    string newCredential = CryptoUtil.EncryptInfo(userModel.Email.Trim().ToLower()) + "|" + CryptoUtil.EncryptStringWithKey(userModel.Password.Trim()) + "|" + CryptoUtil.EncryptInfo(userModel.StudyId.Trim());
                    if (oldCredential == newCredential)
                        isCredentialchanged = false;
                }
                User useremail1 = null;
                if (!string.IsNullOrEmpty(userModel.Email))
                {

                    string encriptedEmail1 = CryptoUtil.EncryptInfo(userModel.Email.Trim().ToLower());
                    useremail1 = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.UserID != userModel.UserID && u.Email == encriptedEmail1);
                }
                if (useremail1 != null && useremail1.UserID > 0)
                {
                    model.UserID = user.UserID;
                    model.Status = LAMPConstants.USER_EMAIL_ALREADY_EXIST;
                    model.Message = ResourceHelper.GetStringResource(LAMPConstants.USER_EMAIL_ALREADY_EXIST);
                    return model;
                }
                user.FirstName = CryptoUtil.EncryptInfo(userModel.FirstName.Trim());
                user.LastName = CryptoUtil.EncryptInfo(userModel.LastName.Trim());
                if (!string.IsNullOrEmpty(userModel.Email))
                {
                    user.Email = CryptoUtil.EncryptInfo(userModel.Email.Trim().ToLower());
                }
                if (isCredentialchanged == true)
                    user.SessionToken = string.Empty;
                if (userModel.Password == null || userModel.Password.Trim() == "")
                {
                    user.Password = string.Empty;
                    user.Status = false;
                }
                else
                {
                    User useremail = null;
                    if (!string.IsNullOrEmpty(userModel.Email))
                    {
                        string encriptedEmail = CryptoUtil.EncryptInfo(userModel.Email.Trim().ToLower());
                        useremail = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.IsDeleted == false && u.UserID != userModel.UserID && u.Email == encriptedEmail);
                    }
                    if (useremail != null && useremail.UserID > 0)
                    {
                        model.UserID = user.UserID;
                        model.Status = LAMPConstants.USER_EMAIL_ALREADY_EXIST;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.USER_EMAIL_ALREADY_EXIST);
                        return model;
                    }
                    if (user.Password == null || user.Password.Trim().Length == 0)
                        user.RegisteredOn = DateTime.UtcNow;

                    if (userModel.Password.Trim() != "xxx")
                    {
                        user.Password = CryptoUtil.EncryptStringWithKey(userModel.Password.Trim());
                        user.Status = true;
                        userPassword = userModel.Password.Trim();
                    }
                }
                user.Phone = userModel.Phone != null ? CryptoUtil.EncryptInfo(userModel.Phone.Trim()) : string.Empty;
                user.ZipCode = userModel.ZipCode != null ? CryptoUtil.EncryptInfo(userModel.ZipCode.Trim()) : string.Empty;
                user.City = userModel.City != null ? CryptoUtil.EncryptInfo(userModel.City.Trim()) : string.Empty;
                user.State = userModel.State != null ? CryptoUtil.EncryptInfo(userModel.State.Trim()) : string.Empty;
                user.Gender = userModel.Gender == "Male" ? Convert.ToByte(1) : Convert.ToByte(2);
                if (userModel.Age == null)
                    user.Age = null;
                else
                    user.Age = (byte)userModel.Age;
                user.BirthDate = (userModel.BirthDateString == null || userModel.BirthDateString.Trim().Length == 0) ? Convert.ToDateTime("1900/01/01").Date : Convert.ToDateTime(userModel.BirthDateString.Trim(), culture).Date;
                if (userModel.PhysicianFirstName != null && userModel.PhysicianFirstName.Trim().Length > 0)
                    user.PhysicianFirstName = CryptoUtil.EncryptInfo(userModel.PhysicianFirstName.Trim());
                else
                    user.PhysicianFirstName = string.Empty;

                if (userModel.PhysicianLastName != null && userModel.PhysicianLastName.Trim().Length > 0)
                    user.PhysicianLastName = CryptoUtil.EncryptInfo(userModel.PhysicianLastName.Trim());
                else
                    user.PhysicianLastName = string.Empty;

                if (userModel.StudyCode != null && userModel.StudyCode.Trim().Length > 0)
                    user.StudyCode = CryptoUtil.EncryptInfo(userModel.StudyCode.Trim());
                else
                    user.StudyCode = string.Empty;

                if (user.UserID == 0)
                    user.IsGuestUser = false;
                else
                    user.IsGuestUser = userModel.IsGuestUser;

                user.StudyId = CryptoUtil.EncryptInfo(userModel.StudyId.Trim());
                user.AdminID = userModel.AdminID;
                if (user.UserID > 0)
                {
                    user.EditedOn = DateTime.UtcNow;
                    _UnitOfWork.IUserRepository.Update(user);
                }
                else
                {
                    user.IsDeleted = false;
                    user.CreatedOn = DateTime.UtcNow;

                    _UnitOfWork.IUserRepository.Add(user);
                }
                _UnitOfWork.Commit();
                model.UserID = user.UserID;
                // Upload file
                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(LAMPConstants.CLINICAL_PROFILE_PATH);
                bool isUploadSuccess = true;
                if (stream != null && stream.Length > 0)
                {
                    isUploadSuccess = Helper.SaveStreamToFile(filePath + "\\" + user.UserID + "_" + userModel.ClinicalProfileExtension, stream);
                    if (isUploadSuccess == true)
                    {
                        user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == user.UserID);
                        user.ClinicalProfileURL = CryptoUtil.EncryptInfo(user.UserID + "_" + userModel.ClinicalProfileExtension);
                        _UnitOfWork.IUserRepository.Update(user);
                        _UnitOfWork.Commit();
                    }
                }
                model.Status = LAMPConstants.SUCCESS_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_USER_DETAILS_SAVED_SUCCESSFULLY);
                bool val=false;
                if (isCredentialchanged == true)
                    val = EmailUserStudyId_Pwd(userModel.Email, userModel.FirstName + " " + userModel.LastName, userModel.StudyId, userModel.StudyCode, userPassword);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                if (model.UserID > 0 && stream != null && stream.Length > 0)
                    model.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_USER_DETAILS_SAVED_SUCCESSFULLY_IMAGE_NOT_SAVED);
                else
                    model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// gets user details
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>User details</returns>
        public UserViewModel GetUserDetails(long userID)
        {
            var model = new UserViewModel();
            User userDetails = null;
            userDetails = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == userID);
            if (userDetails != null)
            {
                model.UserID = userDetails.UserID;
                model.FirstName = CryptoUtil.DecryptInfo(userDetails.FirstName);
                model.LastName = CryptoUtil.DecryptInfo(userDetails.LastName);
                model.Email = CryptoUtil.DecryptInfo(userDetails.Email);
                model.Password = userDetails.Password.Length > 0 ? "xxx" : null;
                model.Password = CryptoUtil.DecryptStringWithKey(userDetails.Password);
                model.Phone = CryptoUtil.DecryptInfo(userDetails.Phone);
                model.ZipCode = CryptoUtil.DecryptInfo(userDetails.ZipCode);
                model.City = CryptoUtil.DecryptInfo(userDetails.City);
                model.State = CryptoUtil.DecryptInfo(userDetails.State);
                if (userDetails.Gender == 1)
                    model.Gender = "Male";
                else if (userDetails.Gender == 2)
                    model.Gender = "Female";
                else
                    model.Gender = "Not given";
                model.Age = userDetails.Age == null ? (byte)0 : (byte)userDetails.Age;
                model.BirthDateString = (userDetails.BirthDate == null || Helper.GetDateString(userDetails.BirthDate, "MM/dd/yyyy") == "01/01/1900") ? string.Empty : Helper.GetDateString(userDetails.BirthDate, "MM/dd/yyyy");
                model.ClinicalProfileURL = CryptoUtil.DecryptInfo(userDetails.ClinicalProfileURL);
                model.PhysicianFirstName = CryptoUtil.DecryptInfo(userDetails.PhysicianFirstName);
                model.PhysicianLastName = CryptoUtil.DecryptInfo(userDetails.PhysicianLastName);
                model.StudyCode = CryptoUtil.DecryptInfo(userDetails.StudyCode);
                model.StudyId = CryptoUtil.DecryptInfo(userDetails.StudyId);
                model.IsGuestUser = (bool)userDetails.IsGuestUser;
            }
            return model;
        }

        /// <summary>
        /// Get the User list
        /// </summary>
        /// <param name="model">Paging/Sorting details</param>
        /// <returns>User detail list</returns>
        public UserListViewModel GetUsers(UserListViewModel model)
        {
            var response = new UserListViewModel();
            try
            {
                IQueryable<AdminUser> enumUserList = null;
                enumUserList = (from user in _UnitOfWork.IUserRepository.RetrieveAll()
                                join survey in _UnitOfWork.ISurveyResultRepository.RetrieveAll()
                                on user.UserID equals survey.UserID
                                into userSurvey
                                where user.IsDeleted == false
                                from survey in userSurvey.DefaultIfEmpty()
                                select new AdminUser
                                {
                                    UserID = user.UserID,
                                    StudyId = user.StudyId,
                                    RegisteredOn = user.RegisteredOn,
                                    Email = user.Email,
                                    Phone = user.Phone,
                                    Device = "device",
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Surveys = userSurvey.Count().ToString(),
                                    IsActive = (user.Status == null || user.Status == false) ? false : true,
                                    ClinicalProfileURL = user.ClinicalProfileURL,
                                    AdminId = (long)user.AdminID,

                                }).Distinct();
                List<AdminUser> userList = new List<AdminUser>();
                if (model.SearchId != null)
                    userList = enumUserList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.StudyId).StartsWith(model.SearchId));

                else
                    userList = enumUserList.ToList();

                if (model.LoggedInAdminId != 1)//if no super admin,filter the user list
                {
                    userList = userList.Where(u => u.AdminId == model.LoggedInAdminId).ToList();
                }
                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "StudyId" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "StudyId":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                        else
                            userList = userList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                        break;
                    case "RegisteredOn":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => c.RegisteredOn).ToList();
                        else
                            userList = userList.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "Email":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.Email)).ToList();
                        else
                            userList = userList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.Email)).ToList();
                        break;
                    default:
                        userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                        break;
                }

                if (userList != null)
                {
                    foreach (var user in userList)
                    {
                        string device = "";
                        UserDevice userDetails = null;
                        userDetails = _UnitOfWork.IUserDeviceRepository.SingleOrDefault(u => u.UserID == user.UserID);
                        if (userDetails != null)
                        {
                            if (userDetails.DeviceType == 1)
                            {
                                device = "IOS";
                            }
                            else if (userDetails.DeviceType == 2)
                            {
                                device = "Android";
                            }
                        }
                        else
                        {
                            device = "Web";
                        }
                        user.Device = device;
                    }
                }

                response.UserList = userList;
                response.TotalRows = userList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetUsers: " + ex);
                response = new UserListViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// get selected user list
        /// </summary>
        /// <param name="strSearch">Study Id</param>
        /// <returns>selected user list</returns>
        public UserListViewModel GetSelectedUsers(string strSearch)
        {
            IQueryable<AdminUser> enumUserList = null;
            var listUserResponse = new UserListViewModel();
            try
            {
                enumUserList = (from user in _UnitOfWork.IUserRepository.RetrieveAll().Where(u => u.IsDeleted == false)
                                select new AdminUser
                                {
                                    UserID = user.UserID,
                                    StudyId = user.StudyId,
                                    RegisteredOn = user.RegisteredOn,
                                    Email = user.Email,
                                    Phone = user.Phone,
                                    Device = "device",
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Surveys = "0",
                                    IsActive = (user.Status == null || user.Status == false) ? false : true,
                                    ClinicalProfileURL = user.ClinicalProfileURL
                                });
                List<AdminUser> userList = new List<AdminUser>();
                userList = enumUserList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.StudyId).StartsWith(strSearch));
                string sortField = "StudyId";
                string sortDirection = "asc";
                switch (sortField)
                {
                    case "StudyId":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => c.StudyId).ToList();
                        else
                            userList = userList.OrderByDescending(c => c.StudyId).ToList();
                        break;
                    case "RegisteredOn":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => c.RegisteredOn).ToList();
                        else
                            userList = userList.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "Email":
                        if (sortDirection == "asc")
                            userList = userList.OrderBy(c => c.Email).ToList();
                        else
                            userList = userList.OrderByDescending(c => c.Email).ToList();
                        break;
                    default:
                        userList = userList.OrderBy(c => c.StudyId).ToList();
                        break;
                }
                listUserResponse.UserList = userList;
                listUserResponse.TotalRows = userList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                listUserResponse.Status = LAMPConstants.ERROR_CODE;
            }
            return listUserResponse;
        }

        /// <summary>
        /// delets a user is delete status is changed to true
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User List</returns>
        public UserListViewModel DeleteUser(long userId)
        {
            UserListViewModel model = new UserListViewModel();
            var response = new UserListViewModel();
            User user = _UnitOfWork.IUserRepository.GetById(userId);
            if (user != null)
            {
                user.IsDeleted = true;
                _UnitOfWork.IUserRepository.Update(user);
                _UnitOfWork.Commit();
                response = GetUsers(model);
            }
            return response;
        }

        /// <summary>
        /// user status is changed Active ,inactive
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="Status">Active/Inactive status</param>
        /// <returns>User List</returns>
        public UserListViewModel ChangeUserStatus(long userId, bool Status)
        {
            UserListViewModel model = new UserListViewModel();
            var response = new UserListViewModel();
            User user = _UnitOfWork.IUserRepository.GetById(userId);
            if (user != null)
            {
                user.Status = Status;
                user.SessionToken = string.Empty;
                _UnitOfWork.IUserRepository.Update(user);
                _UnitOfWork.Commit();
                response = GetUsers(model);
            }
            return response;
        }

        #endregion

        #region Blogs & Tips

        /// <summary>
        /// Saves the tips.
        /// </summary>
        /// <param name="tipsViewModel">The tips view model.</param>
        /// <returns></returns>
        public TipsViewModel SaveTips(TipsViewModel tipsViewModel)
        {
            var model = new TipsViewModel();
            try
            {
                Tip tip = _UnitOfWork.ITipRepository.SingleOrDefault(u => u.AdminID == tipsViewModel.AdminId);
                tipsViewModel.TipText = tipsViewModel.TipText.Trim();
                if (null != tip && tip.TipID > 0)
                {
                    tip.TipText = tipsViewModel.TipText;
                    tip.EditedOn = DateTime.UtcNow;
                    tip.IsDeleted = false;
                    tip.AdminID = tipsViewModel.AdminId;
                    _UnitOfWork.ITipRepository.Update(tip);
                }
                else
                {
                    Tip tipNew = new Tip();
                    tipNew.TipText = tipsViewModel.TipText;
                    tipNew.CreatedOn = DateTime.UtcNow;
                    tipNew.IsDeleted = false;
                    tipNew.AdminID = tipsViewModel.AdminId;
                    _UnitOfWork.ITipRepository.Add(tipNew);
                }
                _UnitOfWork.Commit();
                tipsViewModel.Status = LAMPConstants.SUCCESS_CODE;
                tipsViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.TIP_SAVED_SUCCESSFULLY);
                tipsViewModel.IsSaved = true;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }

            return model;
        }

        /// <summary>
        /// Saves the blog.
        /// </summary>
        /// <param name="blogsViewModel">The blogs view model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public BlogsViewModel SaveBlog(BlogsViewModel blogsViewModel, Stream stream)
        {
            blogsViewModel.BlogTitle = blogsViewModel.BlogTitle.Trim();
            Blog blog = null;
            blog = _UnitOfWork.IBlogRepository.SingleOrDefault(u => u.BlogID == blogsViewModel.BlogID);
            if (blog == null)
            {
                blog = new Blog();
                blog.BlogTitle = blogsViewModel.BlogTitle;
                blog.Content = blogsViewModel.Content;
                blog.CreatedOn = DateTime.UtcNow;
                blog.IsDeleted = false;
                blog.BlogText = blogsViewModel.BlogText;
                blog.AdminID = blogsViewModel.AdminId;

                // Upload file
                string path = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BlogImagePath"]);
                bool isUploadSuccess = true;
                _UnitOfWork.IBlogRepository.Add(blog);
                _UnitOfWork.Commit();
                if (stream != null && stream.Length > 0)
                {
                    string newImageFileName = blog.BlogID + "_" + Guid.NewGuid().ToString() + blogsViewModel.BlogExtension;
                    isUploadSuccess = Helper.SaveStreamToFile(path + "\\" + newImageFileName, stream);
                    if (isUploadSuccess == true)
                    {
                        blogsViewModel.ImageURL = CryptoUtil.EncryptInfo(newImageFileName);
                        blog.ImageURL = blogsViewModel.ImageURL;
                        _UnitOfWork.IBlogRepository.Update(blog);
                        _UnitOfWork.Commit();
                    }
                }
                blogsViewModel.Status = LAMPConstants.SUCCESS_CODE;
                blogsViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.BLOG_SAVED_SUCCESSFULLY);
                blogsViewModel.IsSaved = true;
            }
            else
            {
                if (blog.BlogID > 0)
                {
                    blog.BlogTitle = blogsViewModel.BlogTitle;
                    blog.Content = blogsViewModel.Content;
                    blog.IsDeleted = false;
                    blog.EditedOn = DateTime.UtcNow;
                    blog.AdminID = blogsViewModel.AdminId;
                    string path = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BlogImagePath"]);
                    bool isUploadSuccess = true;
                    if (stream != null && stream.Length > 0)
                    {
                        string newImageFileName = blog.BlogID + "_" + Guid.NewGuid().ToString() + blogsViewModel.BlogExtension;
                        isUploadSuccess = Helper.SaveStreamToFile(path + "\\" + newImageFileName, stream);
                        if (isUploadSuccess == true)
                        {
                            blogsViewModel.ImageURL = CryptoUtil.EncryptInfo(newImageFileName);
                            blog.ImageURL = blogsViewModel.ImageURL;
                        }
                    }
                    _UnitOfWork.IBlogRepository.Update(blog);
                    _UnitOfWork.Commit();
                    blogsViewModel.Status = LAMPConstants.SUCCESS_CODE;
                    blogsViewModel.Message = ResourceHelper.GetStringResource(LAMPConstants.BLOG_UPDATED_SUCCESSFULLY);
                    blogsViewModel.IsSaved = true;
                }
            }
            return blogsViewModel;
        }

        /// <summary>
        /// Gets all blogs list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TipsBlogsViewModel GetAllBlogsList(TipsBlogsViewModel model)
        {
            IQueryable<BlogsViewModel> enumBlogsList = null;
            var listUserResponse = new TipsBlogsViewModel();
            enumBlogsList = (from blog in _UnitOfWork.IBlogRepository.RetrieveAll().Where(s => s.IsDeleted != true)
                             select new BlogsViewModel
                             {
                                 BlogID = blog.BlogID,
                                 BlogTitle = blog.BlogTitle,
                                 Content = blog.Content,
                                 ImageURL = blog.ImageURL,
                                 CreatedOn = blog.CreatedOn,
                                 EditedOn = blog.EditedOn,
                                 IsDeleted = blog.IsDeleted,
                                 AdminId = blog.AdminID,
                                 CreatedAdminFName = blog.Admin.FirstName,
                                 CreatedAdminLName = blog.Admin.LastName
                             });
            List<BlogsViewModel> blogList = new List<BlogsViewModel>();
            foreach (var item in enumBlogsList)
            {
                item.ImageURL = CryptoUtil.DecryptInfo(item.ImageURL);
                blogList.Add(item);
            }
            string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "CreatedOn" : model.SortPageOptions.SortField;
            string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "desc" : model.SortPageOptions.SortOrder;
            switch (sortField)
            {
                case "CreatedOn":
                    if (sortDirection == "asc")
                        blogList = blogList.OrderBy(c => c.CreatedOn).ToList();
                    else
                        blogList = blogList.OrderByDescending(c => c.CreatedOn).ToList();
                    break;
                default:
                    blogList = blogList.OrderByDescending(c => c.CreatedOn).ToList();
                    break;
            }

            listUserResponse.BlogList = blogList;
            listUserResponse.TotalRows = blogList.Count;
            return listUserResponse;
        }

        /// <summary>
        /// Gets the tips.
        /// </summary>
        /// <param name="tipsViewModel">The tips view model.</param>
        /// <returns></returns>
        public TipsViewModel GetTips(TipsViewModel tipsViewModel)
        {
            Tip tip = _UnitOfWork.ITipRepository.RetrieveAll().Where(u => u.AdminID == tipsViewModel.AdminId && u.IsDeleted != true).FirstOrDefault();
            if (tip != null)
            {
                tipsViewModel.TipText = tip.TipText;
                tipsViewModel.AdminId = tip.AdminID;
            }
            return tipsViewModel;
        }

        /// <summary>
        /// Gets the blog details.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        public TipsBlogsViewModel GetBlogDetails(long blogId)
        {
            var model = new TipsBlogsViewModel();
            Blog blogDetails = null;
            blogDetails = _UnitOfWork.IBlogRepository.SingleOrDefault(u => u.BlogID == blogId);
            if (blogDetails != null)
            {
                model.BlogsViewModel.BlogID = blogDetails.BlogID;
                model.BlogsViewModel.BlogTitle = blogDetails.BlogTitle;
                model.BlogsViewModel.Content = blogDetails.Content;
                model.BlogsViewModel.CreatedOn = blogDetails.CreatedOn;
                model.BlogsViewModel.ImageURL = CryptoUtil.DecryptInfo(blogDetails.ImageURL);
                model.BlogsViewModel.IsDeleted = blogDetails.IsDeleted;
            }
            return model;
        }

        /// <summary>
        /// Gets the blogs.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public TipsBlogsViewModel GetBlogs(TipsBlogsViewModel model)
        {
            var response = new TipsBlogsViewModel();
            try
            {
                response = GetAllBlogsList(model);
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetBlogs: " + ex);
                response = new TipsBlogsViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Deletes the blog.
        /// </summary>
        /// <param name="blogId">The blog identifier.</param>
        /// <returns></returns>
        public TipsBlogsViewModel DeleteBlog(long blogId)
        {
            TipsBlogsViewModel model = new TipsBlogsViewModel();
            var response = new TipsBlogsViewModel();
            Blog blog = _UnitOfWork.IBlogRepository.GetById(blogId);
            if (blog != null)
            {
                blog.IsDeleted = true;
                _UnitOfWork.IBlogRepository.Update(blog);
                _UnitOfWork.Commit();
                response = GetAllBlogsList(model);
            }
            return response;
        }

        #endregion

        #region User Activities

        /// <summary>
        /// gets survey results
        /// </summary>
        /// <param name="SurveyResultID">Survey Result ID</param>
        /// <returns>Survey Details</returns>
        public SurveyResultsViewModel GetSurveyResults(long SurveyResultID, long AdminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new SurveyResultsViewModel();
            try
            {
                SurveyResult surveyResultDetails = null;
                if (AdminBatchSchID > 0)
                    surveyResultDetails = _UnitOfWork.ISurveyResultRepository.SingleOrDefault(u => u.SurveyResultID == SurveyResultID && u.AdminBatchSchID == AdminBatchSchID);
                else
                    surveyResultDetails = _UnitOfWork.ISurveyResultRepository.SingleOrDefault(u => u.SurveyResultID == SurveyResultID && u.AdminBatchSchID == null);
                if (surveyResultDetails != null)
                {
                    model.SurveyResultID = surveyResultDetails.SurveyResultID;
                    model.UserID = surveyResultDetails.UserID;
                    model.SurveyName = CryptoUtil.DecryptInfo(surveyResultDetails.SurveyName);
                    model.StartTime = (DateTime)surveyResultDetails.StartTime;
                    model.EndTime = (DateTime)surveyResultDetails.EndTime;
                    model.Rating = CryptoUtil.DecryptInfo(surveyResultDetails.Rating);
                    model.Comment = CryptoUtil.DecryptInfo(surveyResultDetails.Comment);
                    model.SurveyDate = (DateTime)surveyResultDetails.CreatedOn;
                    model.SurveyDate = model.SurveyDate.AddMinutes(offsetValue);
                    model.Duration = model.EndTime.Subtract(model.StartTime);
                    IQueryable<SurveyResultsDetail> enumSurveyResultsDetailList = null;
                    enumSurveyResultsDetailList = (from surveyResultDtl in _UnitOfWork.ISurveyResultDtlRepository.RetrieveAll().Where(u => u.SurveyResultID == surveyResultDetails.SurveyResultID)
                                                   select new SurveyResultsDetail
                                                   {
                                                       SurveyResultDtlID = surveyResultDtl.SurveyResultDtlID,
                                                       Question = surveyResultDtl.Question,
                                                       CorrectAnswer = surveyResultDtl.CorrectAnswer,
                                                       EnteredAnswer = surveyResultDtl.EnteredAnswer,
                                                       TimeTaken = surveyResultDtl.TimeTaken,
                                                       ClickRange = surveyResultDtl.ClickRange
                                                   });
                    string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "SurveyResultDtlID" : model.SortPageOptions.SortField;
                    string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;

                    switch (sortField)
                    {
                        case "SurveyResultDtlID":
                            if (sortDirection == "asc")
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderBy(c => c.SurveyResultDtlID);
                            else
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderByDescending(c => c.SurveyResultDtlID);
                            break;
                        case "Question":
                            if (sortDirection == "asc")
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderBy(c => c.Question);
                            else
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderByDescending(c => c.Question);
                            break;
                        case "CorrectAnswer":
                            if (sortDirection == "asc")
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderBy(c => c.CorrectAnswer);
                            else
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderByDescending(c => c.CorrectAnswer);
                            break;
                        case "EnteredAnswer":
                            if (sortDirection == "asc")
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderBy(c => c.EnteredAnswer);
                            else
                                enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderByDescending(c => c.EnteredAnswer);
                            break;
                        default:
                            enumSurveyResultsDetailList = enumSurveyResultsDetailList.OrderBy(c => c.Question);
                            break;
                    }
                    List<SurveyResultsDetail> srlist = enumSurveyResultsDetailList.ToList();
                    model.TotalRows = srlist.Count;
                    foreach (SurveyResultsDetail survey in srlist)
                    {
                        survey.Question = CryptoUtil.DecryptInfo(survey.Question);
                        survey.CorrectAnswer = CryptoUtil.DecryptInfo(survey.CorrectAnswer);
                        survey.EnteredAnswer = CryptoUtil.DecryptInfo(survey.EnteredAnswer);
                        survey.TimeTaken = TimeSpan.FromSeconds((double)survey.TimeTaken).TotalMilliseconds;
                        survey.ClickRange = survey.ClickRange;
                    }
                    model.QuestAndAnsList = srlist;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// Get user Activities details
        /// </summary>
        /// <param name="userID">User Id</param>
        /// <returns>User activities details</returns>
        public UserActivitiesViewModel GetUserActivities(long userID)
        {
            var userActivities = new UserActivitiesViewModel();
            try
            {
                if (HttpContext.Current.Session["OffsetValue"] == null)
                {
                    userActivities.Status = Convert.ToInt16(LAMPConstants.API_USER_SESSION_EXPIRED);
                }
                else
                    userActivities = GetUserActivitiesDetails(userID);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
            }
            return userActivities;
        }

        /// <summary>
        /// Get user Activities details
        /// </summary>
        /// <param name="userID">User Id</param>
        /// <returns>User activities details</returns>
        private UserActivitiesViewModel GetUserActivitiesDetails(long userID)
        {
            LogUtil.Error("GetUserActivities()====================================");
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var userActivities = new UserActivitiesViewModel();
            string lastSurveyDate = string.Empty;
            try
            {
                IQueryable<UserSurvey> querySurveyList = null;
                querySurveyList = (from survey in _UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(u => u.UserID == userID)
                                   select new UserSurvey
                                   {
                                       SurveyResultID = survey.SurveyResultID,
                                       SurveyName = survey.SurveyName,
                                       Rating = survey.Rating,
                                       Date_Time = survey.StartTime.ToString(),
                                       SurveyPoints = (decimal)survey.Point,
                                       Status = (byte)survey.Status,
                                       AdminBatchSchID = survey.AdminBatchSchID, 
                                       IsDistraction = (bool)survey.IsDistraction,
                                       IsNotificationGame = (bool)survey.IsNotificationGame,
                                       SpinWheelScore = survey.SpinWheelScore
                                   });

                // ######################################### START COGNITION #########################################
                DateTime lastResultDate = Convert.ToDateTime("1900/01/01"); 
                string lastResultStatus = string.Empty;
                List<UserCognition> CognitionList = new List<UserCognition>();
                UserCognitionResult objCognitionList = new UserCognitionResult();
                objCognitionList.UserCognitionList = null;


                IQueryable<UserCognition> queryCognitionList = null;
                List<UserCognition> gameList = new List<UserCognition>();
                DateTime _lastResultDate = Convert.ToDateTime("1900/01/01");
                Int16 totalGames = 0;
                Int16 overAllRating = 0;
                decimal earnedPoints = 0;
                string spinWheelScore = string.Empty;
                string lastResultStatus_batchSchedule = string.Empty;
                List<UserCognition> gameList_batchSchedule = new List<UserCognition>();
                List<UserCognition> CognitionList_batchSchedule = new List<UserCognition>();
                DateTime lastResultDate_batchSchedule = Convert.ToDateTime("1900/01/01");

                List<UserCognition> batchScheduleList = new List<UserCognition>();
                // 1. Cat and Dog Game                
                queryCognitionList = (from catAndDog in _UnitOfWork.ICatAndDogResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Cats and Dogs",
                                          Rating = (byte)catAndDog.Rating,
                                          Date_Time = ((DateTime)catAndDog.StartTime).ToString(),
                                          EarnedPoints = (decimal)catAndDog.Point,
                                          AdminBatchSchID = catAndDog.AdminBatchSchID,
                                          IsNotificationGame = (bool)catAndDog.IsNotificationGame,
                                          SpinWheelScore = catAndDog.SpinWheelScore
                                      });
                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {

                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.CatAndDog, "Cats and Dogs", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.CatAndDog, "Cats and Dogs", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 2. N-Back Game                
                queryCognitionList = (from nBack in _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "n-Back",
                                          Rating = (byte)nBack.Rating,
                                          Date_Time = ((DateTime)nBack.StartTime).ToString(),
                                          EarnedPoints = (decimal)nBack.Point,
                                          AdminBatchSchID = nBack.AdminBatchSchID,
                                          IsNotificationGame = (bool)nBack.IsNotificationGame,
                                          SpinWheelScore = nBack.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.NBack, "n-Back", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.NBack, "n-Back", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 12. N-Back New Game                
                queryCognitionList = (from nBack in _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "n-Back New",
                                          Rating = (byte)nBack.Rating,
                                          Date_Time = ((DateTime)nBack.StartTime).ToString(),
                                          EarnedPoints = (decimal)nBack.Point,
                                          AdminBatchSchID = nBack.AdminBatchSchID,
                                          IsNotificationGame = (bool)nBack.IsNotificationGame,
                                          SpinWheelScore = nBack.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.NBackNew, "n-Back New", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.NBackNew, "n-Back New", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 3. Serial 7 Game                
                queryCognitionList = (from serial7 in _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Serial 7s",
                                          Rating = (byte)serial7.Rating,
                                          Date_Time = ((DateTime)serial7.StartTime).ToString(),
                                          EarnedPoints = (decimal)serial7.Point,
                                          AdminBatchSchID = serial7.AdminBatchSchID,
                                          IsNotificationGame = (bool)serial7.IsNotificationGame,
                                          SpinWheelScore = serial7.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.Serial7, "Serial 7s", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.Serial7, "Serial 7s", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 4. Simple Memory                
                queryCognitionList = (from simpleMemory in _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Simple memory",
                                          Rating = (byte)simpleMemory.Rating,
                                          Date_Time = ((DateTime)simpleMemory.StartTime).ToString(),
                                          EarnedPoints = (decimal)simpleMemory.Point,
                                          AdminBatchSchID = simpleMemory.AdminBatchSchID,
                                          IsNotificationGame = (bool)simpleMemory.IsNotificationGame,
                                          SpinWheelScore = simpleMemory.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);  
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.SimpleMemory, "Simple memory", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.SimpleMemory, "Simple memory", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 5. Digit Span 
                queryCognitionList = (from digitSpan in _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == userID && u.Type == 1).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Digit Span",
                                          Rating = (byte)digitSpan.Rating,
                                          Date_Time = ((DateTime)digitSpan.StartTime).ToString(),
                                          EarnedPoints = (decimal)digitSpan.Point,
                                          AdminBatchSchID = digitSpan.AdminBatchSchID,
                                          IsNotificationGame = (bool)digitSpan.IsNotificationGame,
                                          SpinWheelScore = digitSpan.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.DigitSpan, "Digit Span", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.DigitSpan, "Digit Span", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }
                // 13. Digit Span Backward             
                queryCognitionList = (from digitSpan in _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == userID && u.Type == 2).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Digit Span Backward",
                                          Rating = (byte)digitSpan.Rating,
                                          Date_Time = ((DateTime)digitSpan.StartTime).ToString(),
                                          EarnedPoints = (decimal)digitSpan.Point,
                                          AdminBatchSchID = digitSpan.AdminBatchSchID,
                                          IsNotificationGame = (bool)digitSpan.IsNotificationGame,
                                          SpinWheelScore = digitSpan.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.DigitSpanBackward, "Digit Span Backward", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.DigitSpan, "Digit Span", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 6. Trails B                
                queryCognitionList = (from trailsB in _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Trails-b",
                                          Rating = (byte)trailsB.Rating,
                                          Date_Time = ((DateTime)trailsB.StartTime).ToString(),
                                          EarnedPoints = (decimal)trailsB.Point,
                                          AdminBatchSchID = trailsB.AdminBatchSchID,
                                          IsNotificationGame = (bool)trailsB.IsNotificationGame,
                                          SpinWheelScore = trailsB.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.TrailsB, "Trails-b", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }

                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.TrailsB, "Trails-b", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 13. Trails B New               
                queryCognitionList = (from trailsB in _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Trails-b New",
                                          Rating = (byte)trailsB.Rating,
                                          Date_Time = ((DateTime)trailsB.StartTime).ToString(),
                                          EarnedPoints = (decimal)trailsB.Point,
                                          AdminBatchSchID = trailsB.AdminBatchSchID,
                                          IsNotificationGame = (bool)trailsB.IsNotificationGame,
                                          SpinWheelScore = trailsB.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);  
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.TrailsBNew, "Trails-b New", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.TrailsBNew, "Trails-b New", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 16.TrailsB Dot Touch
                queryCognitionList = (from trailsB in _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Trails-b DotTouch",
                                          Rating = (byte)trailsB.Rating,
                                          Date_Time = ((DateTime)trailsB.StartTime).ToString(),
                                          EarnedPoints = (decimal)trailsB.Point,
                                          AdminBatchSchID = trailsB.AdminBatchSchID,
                                          IsNotificationGame = (bool)trailsB.IsNotificationGame,
                                          SpinWheelScore = trailsB.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.TrailsBDotTouch, "Trails-b DotTouch", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.TrailsBDotTouch, "Trails-b DotTouch", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                //17.JEWELS Trails A 
                queryCognitionList = (from trailsB in _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Jewels Trails A",
                                          Rating = (byte)trailsB.Rating,
                                          Date_Time = ((DateTime)trailsB.StartTime).ToString(),
                                          EarnedPoints = (decimal)trailsB.Point,
                                          AdminBatchSchID = trailsB.AdminBatchSchID,
                                          IsNotificationGame = (bool)trailsB.IsNotificationGame,
                                          SpinWheelScore = trailsB.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID); 
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.JewelsTrailsA, "Jewels Trails A", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.JewelsTrailsA, "Jewels Trails A", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                //18.JEWELS Trails B 
                queryCognitionList = (from trailsB in _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Jewels Trails B",
                                          Rating = (byte)trailsB.Rating,
                                          Date_Time = ((DateTime)trailsB.StartTime).ToString(),
                                          EarnedPoints = (decimal)trailsB.Point,
                                          AdminBatchSchID = trailsB.AdminBatchSchID,
                                          IsNotificationGame = (bool)trailsB.IsNotificationGame,
                                          SpinWheelScore = trailsB.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.JewelsTrailsB, "Jewels Trails B", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.JewelsTrailsB, "Jewels Trails B", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }
                
                // 7. Visual Association                
                queryCognitionList = (from visualAssociation in _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Visual Association Task",
                                          Rating = (byte)visualAssociation.Rating,
                                          Date_Time = ((DateTime)visualAssociation.StartTime).ToString(),
                                          EarnedPoints = (decimal)visualAssociation.Point,
                                          AdminBatchSchID = visualAssociation.AdminBatchSchID,
                                          IsNotificationGame = (bool)visualAssociation.IsNotificationGame,
                                          SpinWheelScore = visualAssociation.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);  
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.VisualAssociation, "Visual Association Task", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.VisualAssociation, "Visual Association Task", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 8. 3DFigure                
                queryCognitionList = (from threeDFigure in _UnitOfWork.I3DFigureResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = threeDFigure.DrawnFigFileName,
                                          Date_Time = ((DateTime)threeDFigure.StartTime).ToString(),
                                          EarnedPoints = (decimal)threeDFigure.Point,
                                          AdminBatchSchID = threeDFigure.AdminBatchSchID,
                                          IsNotificationGame = (bool)threeDFigure.IsNotificationGame,
                                          SpinWheelScore = threeDFigure.SpinWheelScore
                                      }).Distinct();

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                var qry = from figure in _UnitOfWork.I3DFigureResultRepository.RetrieveAll()
                          where figure.UserID == userID
                          group figure by figure.GameName
                              into grp
                              select new
                              {
                                  GameName = grp.Key,
                                  Count = grp.Distinct().Count()
                              };
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.ThreeDFigure, "3D Figure", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(qry.Count());
                        overAllRating = LAMPConstants.RATING_NIL;
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.ThreeDFigure, "3D Figure", LAMPConstants.RATING_NIL, overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                    }
                }
                // 9. Spatial Span               
                queryCognitionList = (from spatialSpan in _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Spatial Span",
                                          Rating = (byte)spatialSpan.Rating,
                                          Date_Time = ((DateTime)spatialSpan.StartTime).ToString(),
                                          EarnedPoints = (decimal)spatialSpan.Point,
                                          AdminBatchSchID = spatialSpan.AdminBatchSchID,
                                          IsNotificationGame = (bool)spatialSpan.IsNotificationGame,
                                          SpinWheelScore = spatialSpan.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);  
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.SpatialSpan, "Spatial Span", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.SpatialSpan, "Spatial Span", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }
                // 10. CatAndDogNew               
                queryCognitionList = (from catAndDog in _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Cats and Dogs New",
                                          Rating = (byte)catAndDog.Rating,
                                          Date_Time = ((DateTime)catAndDog.StartTime).ToString(),
                                          EarnedPoints = (decimal)catAndDog.Point,
                                          AdminBatchSchID = catAndDog.AdminBatchSchID,
                                          IsNotificationGame = (bool)catAndDog.IsNotificationGame,
                                          SpinWheelScore = catAndDog.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.CatAndDogNew, "Cats and Dogs New", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.CatAndDogNew, "Cats and Dogs New", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }

                // 11. TemporalOrder               
                queryCognitionList = (from temporalOrder in _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          GameName = "Temporal Order",
                                          Rating = (byte)temporalOrder.Rating,
                                          Date_Time = ((DateTime)temporalOrder.StartTime).ToString(),
                                          EarnedPoints = (decimal)temporalOrder.Point,
                                          AdminBatchSchID = temporalOrder.AdminBatchSchID,
                                          IsNotificationGame = (bool)temporalOrder.IsNotificationGame,
                                          SpinWheelScore = temporalOrder.SpinWheelScore
                                      });

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList() : null;
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);  
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.TemporalOrder, "Temporal Order", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count);
                        overAllRating = Convert.ToInt16(gameList.Select(r => r.Rating).Sum() / totalGames);
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        CognitionList.Add(CreateGame(userID, (Int16)CognitionType.TemporalOrder, "Temporal Order", Convert.ToInt16(gameList.First().Rating), overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                        lastResultStatus = Helper.GetRating(Convert.ToInt16(gameList.First().Rating));
                    }
                }
                if (CognitionList.Count > 0)
                {
                    objCognitionList.LastResultDate = Helper.GetDateString(lastResultDate.AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    objCognitionList.LastResultRating = lastResultStatus;
                    objCognitionList.OverAllRating = Helper.GetRating(Convert.ToInt16(CognitionList.Select(r => r.Rating).Sum() / CognitionList.Count));

                    objCognitionList.UserCognitionList = CognitionList;
                    objCognitionList.TotalGames = 0;
                }
                // ######################################### END COGNITION #########################################

                List<UserCognition> MindfulnessGameList = new List<UserCognition>();
                UserMindfulnessGameResult objMindfulnessGameList = new UserMindfulnessGameResult();
                objMindfulnessGameList.UserMindfulnessGameList = null;
                // 19. Scratch Image                
                queryCognitionList = (from scratchImage in _UnitOfWork.IScratchImageResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          ID = scratchImage.ScratchImageResultID,
                                          GameName = scratchImage.DrawnFigFileName,
                                          Date_Time = ((DateTime)scratchImage.StartTime).ToString(),
                                          EarnedPoints = (decimal)scratchImage.Point,
                                          AdminBatchSchID = scratchImage.AdminBatchSchID,
                                          IsNotificationGame = (bool)scratchImage.IsNotificationGame,
                                          SpinWheelScore = scratchImage.SpinWheelScore
                                      }).Distinct();

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList().OrderBy(o => o.ID).ToList() : null;
                var qryScratchImage = from figure in _UnitOfWork.IScratchImageResultRepository.RetrieveAll()
                                      where figure.UserID == userID
                                      group figure by figure.GameName
                                          into grp
                                          select new
                                          {
                                              GameName = grp.Key,
                                              Count = grp.Distinct().Count()
                                          };
                if (gameList != null)
                {
                    if (gameList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                    {
                        batchScheduleList = gameList.Where(g => g.AdminBatchSchID != null).ToList();
                        var GroupedData = batchScheduleList.GroupBy(g => g.AdminBatchSchID);   
                        foreach (var _batch in GroupedData)
                        {
                            totalGames = Convert.ToInt16(_batch.Count());
                            overAllRating = Convert.ToInt16(_batch.Select(r => r.Rating).Sum() / totalGames);
                            if (_batch != null)
                            {
                                earnedPoints = _batch.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                                spinWheelScore = _batch.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                            }
                            gameList_batchSchedule.Add(CreateBatchScheduleGame(Convert.ToInt64(_batch.First().AdminBatchSchID), userID, (Int16)CognitionType.ScratchImage, "Scratch Image", Convert.ToInt16(_batch.First().Rating), overAllRating, totalGames, Convert.ToDateTime(_batch.First().Date_Time), ref lastResultDate_batchSchedule, ref lastResultStatus_batchSchedule, earnedPoints, _batch.First().IsNotificationGame, spinWheelScore));
                            lastResultStatus_batchSchedule = Helper.GetRating(Convert.ToInt16(_batch.First().Rating));
                        }
                    }
                    gameList = gameList.Where(g => g.AdminBatchSchID == null).ToList();
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count());  
                        overAllRating = LAMPConstants.RATING_NIL;
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        MindfulnessGameList.Add(CreateGame(userID, (Int16)CognitionType.ScratchImage, "Scratch Image", LAMPConstants.RATING_NIL, overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                    }
                }

                // 20. SpinWheel game
                queryCognitionList = (from spinWheel in _UnitOfWork.ISpinWheelResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime)
                                      select new UserCognition
                                      {
                                          ID = spinWheel.SpinWheelResultID,
                                          GameName = "",
                                          Date_Time = ((DateTime)spinWheel.StartTime).ToString(),
                                          EarnedPoints = 0,
                                          AdminBatchSchID = null,
                                          IsNotificationGame = false,
                                          SpinWheelScore = spinWheel.CollectedStars
                                      }).Distinct();

                gameList = queryCognitionList.Count() > 0 ? queryCognitionList.ToList().OrderBy(o => o.ID).ToList() : null;
                if (gameList != null)
                {
                    if (gameList != null && gameList.Count > 0)
                    {
                        totalGames = Convert.ToInt16(gameList.Count());  
                        overAllRating = LAMPConstants.RATING_NIL;
                        if (gameList != null)
                        {
                            earnedPoints = gameList.Sum(s => s.EarnedPoints == null ? 0 : (decimal)s.EarnedPoints);
                            spinWheelScore = gameList.Sum(s => (s.SpinWheelScore != null && s.SpinWheelScore.Length > 0) ? Convert.ToDecimal(CryptoUtil.DecryptInfo(s.SpinWheelScore)) : 0).ToString();
                        }
                        MindfulnessGameList.Add(CreateGame(userID, (Int16)CognitionType.SpinWheel, "Spin Wheel", LAMPConstants.RATING_NIL, overAllRating, totalGames, Convert.ToDateTime(gameList.First().Date_Time), ref lastResultDate, ref lastResultStatus, earnedPoints, gameList.First().IsNotificationGame, spinWheelScore));
                    }
                }
                //---------------
                if (MindfulnessGameList.Count > 0)
                {
                    objMindfulnessGameList.LastResultDate = Helper.GetDateString(lastResultDate.AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    objMindfulnessGameList.LastResultRating = lastResultStatus;
                    objMindfulnessGameList.OverAllRating = Helper.GetRating(Convert.ToInt16(MindfulnessGameList.Select(r => r.Rating).Sum() / MindfulnessGameList.Count));

                    objMindfulnessGameList.UserMindfulnessGameList = MindfulnessGameList;
                    objMindfulnessGameList.TotalGames = 0;
                }
                // ######################################## END MIND FULNESS GAME #############################################

                IQueryable<UserEnvironment> queryLocationList = null;
                queryLocationList = (from location in _UnitOfWork.ILocationRepository.RetrieveAll().Where(u => u.UserID == userID)
                                     select new UserEnvironment
                                     {
                                         Location = location.LocationName,
                                         Address = location.Address,
                                         Date_Time = location.CreatedOn.ToString(),
                                         Type = location.Type == null ? (byte)2 : (byte)location.Type,
                                         Latitude = location.Latitude == null ? "-" : location.Latitude,
                                         Longitude = location.Longitude == null ? "-" : location.Longitude
                                     });

                IQueryable<UserCallHistory> queryCallList = null;
                queryCallList = (from call in _UnitOfWork.IHelpCallRepository.RetrieveAll().Where(u => u.UserID == userID)
                                 select new UserCallHistory
                                 {
                                     CalledNumber = call.CalledNumber,
                                     Duration = call.CallDuraion.ToString(),
                                     Date_Time = call.CallDateTime.ToString(),
                                     Type = (byte)call.Type
                                 });

                List<UserSurvey> surveyList = new List<UserSurvey>();
                surveyList = querySurveyList.ToList().OrderByDescending(o => Convert.ToDateTime(o.Date_Time)).ToList();
                surveyList.Select(survey =>
                {
                    survey.Date_Time = Helper.GetDateString(Convert.ToDateTime(survey.Date_Time).AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    survey.SpinWheelScore = CryptoUtil.DecryptInfo(survey.SpinWheelScore) + " SW Score";
                    return survey;
                }).ToList();

                foreach (UserSurvey obj in surveyList)
                {
                    obj.SurveyName = CryptoUtil.DecryptInfo(obj.SurveyName);
                    obj.Rating = CryptoUtil.DecryptInfo(obj.Rating);
                }

                BatchSchedule_UAResult objBatchSchedule = new BatchSchedule_UAResult();
                List<BatchScheduleItem_UA> BatchScheduleItemList = new List<BatchScheduleItem_UA>();
                if (gameList_batchSchedule != null && gameList_batchSchedule.Count > 0)
                {
                    objBatchSchedule.LastResultDate = Helper.GetDateString(lastResultDate_batchSchedule.AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    objBatchSchedule.LastResultRating = lastResultStatus_batchSchedule;
                    objBatchSchedule.OverAllRating = Helper.GetRating(Convert.ToInt16(gameList_batchSchedule.Select(r => r.Rating).Sum() / gameList_batchSchedule.Count));
                    objCognitionList.TotalGames = 0;
                    BatchScheduleItem_UA item;
                    foreach (UserCognition game in gameList_batchSchedule)
                    {
                        item = new BatchScheduleItem_UA();
                        item.UserID = userID;
                        item.Type = 2;
                        item.Name = game.CognitionName; 
                        item.Date_Games = game.TotalGames.ToString() + " Game(s)";
                        item.Status_Point = string.Format("{0:0} Point(s)", (decimal)game.EarnedPoints); 
                        item.AdminBatchSchID = (Int64)game.AdminBatchSchID;
                        item.CognitionType = game.CognitionType;
                        item.IsNotificationGame = game.IsNotificationGame;
                        item.SpinWheelScore = game.SpinWheelScore;
                        item.Order = game.Order;
                        BatchScheduleItemList.Add(item);
                    }

                }
                if (surveyList.Where(g => g.AdminBatchSchID != null).ToList().Count > 0)
                {
                    List<UserSurvey> batchScheduleSurveyList = new List<UserSurvey>();
                    batchScheduleSurveyList = surveyList.Where(g => g.AdminBatchSchID != null).ToList();
                    BatchScheduleItem_UA item;
                    foreach (UserSurvey survey in batchScheduleSurveyList)
                    {
                        item = new BatchScheduleItem_UA();
                        item.UserID = userID;
                        item.Type = 1;
                        item.Name = survey.SurveyName;
                        item.Date_Games = survey.Date_Time.ToString();
                        item.Status_Point = survey.Status.ToString() == "1" ? "Exited" : "Completed";
                        item.AdminBatchSchID = (Int64)survey.AdminBatchSchID;
                        item.SurveyResultID = survey.SurveyResultID;
                        item.IsDistraction = survey.IsDistraction;
                        item.IsNotificationGame = survey.IsNotificationGame;
                        item.SpinWheelScore = survey.SpinWheelScore;
                        item.Order = 0;
                        BatchScheduleItemList.Add(item);
                    }
                }

                // Survey List
                UserSurveyResult objSurveyList = new UserSurveyResult();
                surveyList = surveyList.Where(g => g.AdminBatchSchID == null).ToList();
                if (surveyList != null && surveyList.Count > 0)
                {
                    SurveyResult _surveyResult = null;
                    _surveyResult = _UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(u => u.UserID == userID).OrderByDescending(o => o.StartTime).FirstOrDefault();
                    if (_surveyResult != null && _surveyResult.StartTime != null)
                        lastSurveyDate = Helper.GetDateString(Convert.ToDateTime(_surveyResult.StartTime).AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    objSurveyList.UserSurveyList = surveyList;
                    objSurveyList.LastSurveyDate = lastSurveyDate;
                    objSurveyList.LastSurveyRating = "Good";
                    objSurveyList.OverAllRating = "Good";
                    objSurveyList.TotalSurveys = Convert.ToInt16(objSurveyList.UserSurveyList.Count);
                    if (surveyList != null)
                        objSurveyList.SurveyPoints = surveyList.Sum(s => s.SurveyPoints == null ? 0 : (decimal)s.SurveyPoints);
                }


                List<Batch_UA> batchSheduleList = new List<Batch_UA>();
                if (BatchScheduleItemList != null && BatchScheduleItemList.Count > 0)
                {
                    BatchScheduleItemList = BatchScheduleItemList.OrderBy(o => o.AdminBatchSchID).ToList();
                    Batch_UA batchShedule;
                    List<BatchScheduleItem_UA> newItemList;
                    BatchDetails_UA batchDetails;
                    BatchScheduleItem_UA newItem;
                    int maxCount = BatchScheduleItemList.Count;

                    var GroupedData = BatchScheduleItemList.GroupBy(g => g.AdminBatchSchID); 
                    foreach (var batchSchedule in GroupedData)
                    {
                        batchShedule = new Batch_UA();
                        Admin_BatchSchedule _BatchScheduleResult = _UnitOfWork.IAdminBatchScheduleRepository.RetrieveAll().Where(u => u.AdminBatchSchID == batchSchedule.Key).FirstOrDefault();
                        batchDetails = new BatchDetails_UA();
                        batchDetails.Name = _BatchScheduleResult.BatchName;
                        batchDetails.SlotTime = _BatchScheduleResult.Time.ToString();
                        batchDetails.RepeatInterval = _BatchScheduleResult.RepeatID.ToString();
                        batchDetails.ScheduledDate = _BatchScheduleResult.ScheduleDate.ToString();

                        newItemList = new List<BatchScheduleItem_UA>();
                        foreach (var item in batchSchedule)
                        {
                            newItem = new BatchScheduleItem_UA();
                            newItem.UserID = item.UserID;
                            newItem.Type = item.Type;
                            newItem.Name = item.Name;
                            newItem.Date_Games = item.Date_Games;
                            newItem.Status_Point = item.Status_Point;
                            newItem.AdminBatchSchID = item.AdminBatchSchID;
                            newItem.CognitionType = item.CognitionType;
                            newItem.SurveyResultID = item.SurveyResultID;
                            newItem.IsDistraction = item.IsDistraction;
                            newItem.IsNotificationGame = item.IsNotificationGame;
                            newItem.SpinWheelScore = item.SpinWheelScore;
                            newItem.Order = item.Order;
                            newItemList.Add(newItem);
                        }
                        batchDetails.SurveyCount = newItemList.Where(w => w.Type == 1).ToList().Count();
                        batchDetails.GameCount = newItemList.Where(w => w.Type == 2).ToList().Count();
                        batchShedule.BatchDetails = batchDetails;

                        batchShedule.BatchScheduleItemList = newItemList.OrderBy(o => o.Order).ToList();
                        batchSheduleList.Add(batchShedule);
                    }

                    objBatchSchedule.TotalGames = 0;
                    objBatchSchedule.BatchList = batchSheduleList;                    
                }

                /*Location List*/
                List<UserEnvironment> objLocationList = new List<UserEnvironment>();
                objLocationList = queryLocationList.ToList();
                foreach (UserEnvironment obj in objLocationList)
                {
                    obj.Location = CryptoUtil.DecryptInfo(obj.Location);
                    obj.Address = CryptoUtil.DecryptInfo(obj.Address);
                    obj.Latitude = CryptoUtil.DecryptInfo(obj.Latitude);
                    obj.Longitude = CryptoUtil.DecryptInfo(obj.Longitude);
                }
                /*Call List*/
                List<UserCallHistory> objCallList = new List<UserCallHistory>();
                objCallList = queryCallList.ToList();
                foreach (UserCallHistory obj in objCallList)
                {
                    obj.CalledNumber = CryptoUtil.DecryptInfo(obj.CalledNumber);
                }

                userActivities.UserID = userID;
                userActivities.StudyId = "0000";
                userActivities.SurveyList = objSurveyList;
                userActivities.CognitionList = objCognitionList;
                /*Converting Location List datetime to new date time format*/
                objLocationList = objLocationList.OrderByDescending(o => Convert.ToDateTime(o.Date_Time)).ToList();
                objLocationList.Select(location =>
                {
                    location.Date_Time = Helper.GetDateString(Convert.ToDateTime(location.Date_Time).AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    return location;
                }).ToList();

                userActivities._LocationList = objLocationList.Where(w => w.Type == 1).ToList();
                userActivities._EnvironmentList = objLocationList.Where(w => w.Type == 2).ToList();
                /*Converting call List datetime to new date time format*/
                objCallList = objCallList.OrderByDescending(o => Convert.ToDateTime(o.Date_Time)).ToList();
                objCallList.Select(call =>
                {
                    call.Date_Time = Helper.GetDateString(Convert.ToDateTime(call.Date_Time).AddMinutes(offsetValue), "MM/dd/yyyy hh:mm tt");
                    return call;
                }).ToList();

                userActivities.CallHistoryList = objCallList;
                userActivities.BatchScheduleList = objBatchSchedule;
                userActivities.MindfulnessGameList = objMindfulnessGameList;
            }
            catch (Exception ex)
            {
                LogUtil.Error("UserAdminService/GetUserActivities: " + ex);
                throw;
            }
            return userActivities;
        }

        #endregion

        #region Games

        /// <summary>
        /// To get the details of  the game N back for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>NBack game details</returns>
        public CognitionNBackViewModel GetCognitionNBack(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionNBackViewModel();
            try
            {
                IQueryable<CognitionNBackDetail> enumCTest_NBackResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_NBackResultDetailList = (from CTest_NBackResult in _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                       select new CognitionNBackDetail
                                                       {
                                                           NBackResultID = CTest_NBackResult.NBackResultID,
                                                           TotalQuestions = CTest_NBackResult.TotalQuestions == null ? 0 : (int)CTest_NBackResult.TotalQuestions,
                                                           CorrectAnswers = (int)CTest_NBackResult.CorrectAnswers,
                                                           WrongAnswers = (int)CTest_NBackResult.WrongAnswers,
                                                           StartTime = (DateTime)CTest_NBackResult.StartTime,
                                                           EndTime = (DateTime)CTest_NBackResult.EndTime,
                                                           Rating = (String)CTest_NBackResult.Rating.ToString(),
                                                           CreatedOn = (DateTime)CTest_NBackResult.CreatedOn,
                                                           Version = (int)CTest_NBackResult.Version,
                                                           Status = (byte)CTest_NBackResult.Status
                                                       });
                }
                else
                {
                    enumCTest_NBackResultDetailList = (from CTest_NBackResult in _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                       select new CognitionNBackDetail
                                                       {
                                                           NBackResultID = CTest_NBackResult.NBackResultID,
                                                           TotalQuestions = CTest_NBackResult.TotalQuestions == null ? 0 : (int)CTest_NBackResult.TotalQuestions,
                                                           CorrectAnswers = (int)CTest_NBackResult.CorrectAnswers,
                                                           WrongAnswers = (int)CTest_NBackResult.WrongAnswers,
                                                           StartTime = (DateTime)CTest_NBackResult.StartTime,
                                                           EndTime = (DateTime)CTest_NBackResult.EndTime,
                                                           Rating = (String)CTest_NBackResult.Rating.ToString(),
                                                           CreatedOn = (DateTime)CTest_NBackResult.CreatedOn,
                                                           Version = (int)CTest_NBackResult.Version,
                                                           Status = (byte)CTest_NBackResult.Status
                                                       });
                }
                List<CognitionNBackDetail> CTlist = enumCTest_NBackResultDetailList.ToList();
                if (CTlist != null)
                {
                    model.UserID = UserID;
                    int totRating = 0;
                    foreach (CognitionNBackDetail CTest in CTlist)
                    {
                        if (enumCTest_NBackResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totRating = totRating + Convert.ToInt32(CTest.Rating);
                        CognitionNBackDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }

                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_NBackResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// Gets the cognition n back new.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public CognitionNBackNewViewModel GetCognitionNBackNew(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionNBackNewViewModel();
            try
            {
                IQueryable<CognitionNBackNewDetail> enumCTest_NBackResultDetailList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_NBackResultDetailList = (from CTest_NBackResult in _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                       select new CognitionNBackNewDetail
                                                       {
                                                           NBackResultID = CTest_NBackResult.NBackNewResultID,
                                                           TotalQuestions = CTest_NBackResult.TotalQuestions == null ? 0 : (int)CTest_NBackResult.TotalQuestions,
                                                           CorrectAnswers = (int)CTest_NBackResult.CorrectAnswers,
                                                           WrongAnswers = (int)CTest_NBackResult.WrongAnswers,
                                                           StartTime = (DateTime)CTest_NBackResult.StartTime,
                                                           EndTime = (DateTime)CTest_NBackResult.EndTime,
                                                           Rating = (String)CTest_NBackResult.Rating.ToString(),
                                                           CreatedOn = (DateTime)CTest_NBackResult.CreatedOn,
                                                           Status = (byte)CTest_NBackResult.Status
                                                       });
                }
                else
                {
                    enumCTest_NBackResultDetailList = (from CTest_NBackResult in _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                       select new CognitionNBackNewDetail
                                                       {
                                                           NBackResultID = CTest_NBackResult.NBackNewResultID,
                                                           TotalQuestions = CTest_NBackResult.TotalQuestions == null ? 0 : (int)CTest_NBackResult.TotalQuestions,
                                                           CorrectAnswers = (int)CTest_NBackResult.CorrectAnswers,
                                                           WrongAnswers = (int)CTest_NBackResult.WrongAnswers,
                                                           StartTime = (DateTime)CTest_NBackResult.StartTime,
                                                           EndTime = (DateTime)CTest_NBackResult.EndTime,
                                                           Rating = (String)CTest_NBackResult.Rating.ToString(),
                                                           CreatedOn = (DateTime)CTest_NBackResult.CreatedOn,
                                                           Status = (byte)CTest_NBackResult.Status
                                                       });
                }

                List<CognitionNBackNewDetail> CTlist = enumCTest_NBackResultDetailList.ToList();
                if (CTlist != null)
                {
                    model.UserID = UserID;
                    int totRating = 0;
                    foreach (CognitionNBackNewDetail CTest in CTlist)
                    {
                        if (enumCTest_NBackResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totRating = totRating + Convert.ToInt32(CTest.Rating);
                        CognitionNBackNewDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }

                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_NBackNewResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Cat and Dog for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>CatDog game details</returns>
        public CognitionCatDogViewModel GetCognitionCatDog(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionCatDogViewModel();
            try
            {
                IQueryable<CognitionCatDogDetail> enumCTest_CatDogResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_CatDogResultDetailList = (from CTest_CatAndDogResult in _UnitOfWork.ICatAndDogResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                        select new CognitionCatDogDetail
                                                        {
                                                            CatAndDogResultID = CTest_CatAndDogResult.CatAndDogResultID,
                                                            TotalQuestions = CTest_CatAndDogResult == null ? 0 : (int)CTest_CatAndDogResult.TotalQuestions,
                                                            CorrectAnswers = (int)CTest_CatAndDogResult.CorrectAnswers,
                                                            WrongAnswers = (int)CTest_CatAndDogResult.WrongAnswers,
                                                            StartTime = (DateTime)CTest_CatAndDogResult.StartTime,
                                                            EndTime = (DateTime)CTest_CatAndDogResult.EndTime,
                                                            Rating = (String)CTest_CatAndDogResult.Rating.ToString(),
                                                            CreatedOn = (DateTime)CTest_CatAndDogResult.CreatedOn,
                                                            Status = (byte)CTest_CatAndDogResult.Status
                                                        });
                }
                else
                {
                    enumCTest_CatDogResultDetailList = (from CTest_CatAndDogResult in _UnitOfWork.ICatAndDogResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                        select new CognitionCatDogDetail
                                                        {
                                                            CatAndDogResultID = CTest_CatAndDogResult.CatAndDogResultID,
                                                            TotalQuestions = CTest_CatAndDogResult == null ? 0 : (int)CTest_CatAndDogResult.TotalQuestions,
                                                            CorrectAnswers = (int)CTest_CatAndDogResult.CorrectAnswers,
                                                            WrongAnswers = (int)CTest_CatAndDogResult.WrongAnswers,
                                                            StartTime = (DateTime)CTest_CatAndDogResult.StartTime,
                                                            EndTime = (DateTime)CTest_CatAndDogResult.EndTime,
                                                            Rating = (String)CTest_CatAndDogResult.Rating.ToString(),
                                                            CreatedOn = (DateTime)CTest_CatAndDogResult.CreatedOn,
                                                            Status = (byte)CTest_CatAndDogResult.Status
                                                        });
                }
                List<CognitionCatDogDetail> CTlist = enumCTest_CatDogResultDetailList.ToList();
                if (CTlist != null)
                {
                    model.UserID = UserID;
                    int totCatRating = 0;
                    foreach (CognitionCatDogDetail CTest in CTlist)
                    {
                        if (enumCTest_CatDogResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totCatRating = totCatRating + Convert.ToInt32(CTest.Rating);
                        CognitionCatDogDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }

                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_CatDogResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totCatRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Serial 7 for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>Serial7 game details</returns>
        public CognitionSerial7ViewModel GetCognitionSerial7(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionSerial7ViewModel();
            try
            {
                IQueryable<CognitionSerial7Detail> enumCTest_Serial7ResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_Serial7ResultDetailList = (from CTest_Serial7Result in _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                         select new CognitionSerial7Detail
                                                         {
                                                             Serial7ResultID = CTest_Serial7Result.Serial7ResultID,
                                                             TotalQuestions = CTest_Serial7Result.TotalQuestions == null ? 0 : (int)CTest_Serial7Result.TotalQuestions,
                                                             TotalAttempts = (int)CTest_Serial7Result.TotalAttempts,
                                                             StartTime = (DateTime)CTest_Serial7Result.StartTime,
                                                             EndTime = (DateTime)CTest_Serial7Result.EndTime,
                                                             Rating = (String)CTest_Serial7Result.Rating.ToString(),
                                                             CreatedOn = (DateTime)CTest_Serial7Result.CreatedOn,
                                                             Version = (int)CTest_Serial7Result.Version,
                                                             Status = (byte)CTest_Serial7Result.Status
                                                         });
                }
                else
                {
                    enumCTest_Serial7ResultDetailList = (from CTest_Serial7Result in _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                         select new CognitionSerial7Detail
                                                         {
                                                             Serial7ResultID = CTest_Serial7Result.Serial7ResultID,
                                                             TotalQuestions = CTest_Serial7Result.TotalQuestions == null ? 0 : (int)CTest_Serial7Result.TotalQuestions,
                                                             TotalAttempts = (int)CTest_Serial7Result.TotalAttempts,
                                                             StartTime = (DateTime)CTest_Serial7Result.StartTime,
                                                             EndTime = (DateTime)CTest_Serial7Result.EndTime,
                                                             Rating = (String)CTest_Serial7Result.Rating.ToString(),
                                                             CreatedOn = (DateTime)CTest_Serial7Result.CreatedOn,
                                                             Version = (int)CTest_Serial7Result.Version,
                                                             Status = (byte)CTest_Serial7Result.Status
                                                         });
                }
                List<CognitionSerial7Detail> CTlist = enumCTest_Serial7ResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totSerial7Rating = 0;
                    foreach (CognitionSerial7Detail CTest in CTlist)
                    {
                        if (enumCTest_Serial7ResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totSerial7Rating = totSerial7Rating + Convert.ToInt32(CTest.Rating);
                        CognitionSerial7Detail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }

                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_Serial7ResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totSerial7Rating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Simple Memory for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>Simple Memory game details</returns>
        public CognitionSimpleMemoryViewModel GetCognitionSimpleMemory(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionSimpleMemoryViewModel();
            try
            {
                IQueryable<CognitionSimpleMemoryDetail> enumCTest_SimpleMemoryResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_SimpleMemoryResultDetailList = (from CTest_SimpleMemoryResult in _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                              select new CognitionSimpleMemoryDetail
                                                              {
                                                                  SimpleMemoryResultID = CTest_SimpleMemoryResult.SimpleMemoryResultID,
                                                                  TotalQuestions = CTest_SimpleMemoryResult.TotalQuestions == null ? 0 : (int)CTest_SimpleMemoryResult.TotalQuestions,
                                                                  CorrectAnswers = (int)CTest_SimpleMemoryResult.CorrectAnswers,
                                                                  WrongAnswers = (int)CTest_SimpleMemoryResult.WrongAnswers,
                                                                  StartTime = (DateTime)CTest_SimpleMemoryResult.StartTime,
                                                                  EndTime = (DateTime)CTest_SimpleMemoryResult.EndTime,
                                                                  Rating = (String)CTest_SimpleMemoryResult.Rating.ToString(),
                                                                  CreatedOn = (DateTime)CTest_SimpleMemoryResult.CreatedOn,
                                                                  Version = (int)CTest_SimpleMemoryResult.Version,
                                                                  Status = (byte)CTest_SimpleMemoryResult.Status
                                                              });
                }
                else
                {
                    enumCTest_SimpleMemoryResultDetailList = (from CTest_SimpleMemoryResult in _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                              select new CognitionSimpleMemoryDetail
                                                              {
                                                                  SimpleMemoryResultID = CTest_SimpleMemoryResult.SimpleMemoryResultID,
                                                                  TotalQuestions = CTest_SimpleMemoryResult.TotalQuestions == null ? 0 : (int)CTest_SimpleMemoryResult.TotalQuestions,
                                                                  CorrectAnswers = (int)CTest_SimpleMemoryResult.CorrectAnswers,
                                                                  WrongAnswers = (int)CTest_SimpleMemoryResult.WrongAnswers,
                                                                  StartTime = (DateTime)CTest_SimpleMemoryResult.StartTime,
                                                                  EndTime = (DateTime)CTest_SimpleMemoryResult.EndTime,
                                                                  Rating = (String)CTest_SimpleMemoryResult.Rating.ToString(),
                                                                  CreatedOn = (DateTime)CTest_SimpleMemoryResult.CreatedOn,
                                                                  Version = (int)CTest_SimpleMemoryResult.Version,
                                                                  Status = (byte)CTest_SimpleMemoryResult.Status
                                                              });
                }
                List<CognitionSimpleMemoryDetail> CTlist = enumCTest_SimpleMemoryResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totSimpleMemoryRating = 0;
                    foreach (CognitionSimpleMemoryDetail CTest in CTlist)
                    {
                        if (enumCTest_SimpleMemoryResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totSimpleMemoryRating = totSimpleMemoryRating + Convert.ToInt32(CTest.Rating);
                        CognitionSimpleMemoryDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }

                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_SimpleMemoryResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totSimpleMemoryRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Visual Association for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>Visual Association game details</returns>
        public CognitionVisualAssociationViewModel GetCognitionVisualAssociation(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionVisualAssociationViewModel();
            try
            {
                IQueryable<CognitionVisualAssociationDetail> enumCTest_VisualAssociationResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_VisualAssociationResultDetailList = (from CTest_VisualAssociationResult in _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                                   select new CognitionVisualAssociationDetail
                                                                   {
                                                                       VisualAssocResultID = CTest_VisualAssociationResult.VisualAssocResultID,
                                                                       TotalQuestions = CTest_VisualAssociationResult.TotalQuestions == null ? 0 : (int)CTest_VisualAssociationResult.TotalQuestions,
                                                                       TotalAttempts = (int)CTest_VisualAssociationResult.TotalAttempts,
                                                                       StartTime = (DateTime)CTest_VisualAssociationResult.StartTime,
                                                                       EndTime = (DateTime)CTest_VisualAssociationResult.EndTime,
                                                                       Rating = (String)CTest_VisualAssociationResult.Rating.ToString(),
                                                                       CreatedOn = (DateTime)CTest_VisualAssociationResult.CreatedOn,
                                                                       Version = (int)CTest_VisualAssociationResult.Version,
                                                                       Status = (byte)CTest_VisualAssociationResult.Status
                                                                   });
                }
                else
                {
                    enumCTest_VisualAssociationResultDetailList = (from CTest_VisualAssociationResult in _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                                   select new CognitionVisualAssociationDetail
                                                                   {
                                                                       VisualAssocResultID = CTest_VisualAssociationResult.VisualAssocResultID,
                                                                       TotalQuestions = CTest_VisualAssociationResult.TotalQuestions == null ? 0 : (int)CTest_VisualAssociationResult.TotalQuestions,
                                                                       TotalAttempts = (int)CTest_VisualAssociationResult.TotalAttempts,
                                                                       StartTime = (DateTime)CTest_VisualAssociationResult.StartTime,
                                                                       EndTime = (DateTime)CTest_VisualAssociationResult.EndTime,
                                                                       Rating = (String)CTest_VisualAssociationResult.Rating.ToString(),
                                                                       CreatedOn = (DateTime)CTest_VisualAssociationResult.CreatedOn,
                                                                       Version = (int)CTest_VisualAssociationResult.Version,
                                                                       Status = (byte)CTest_VisualAssociationResult.Status
                                                                   });
                }

                List<CognitionVisualAssociationDetail> CTlist = enumCTest_VisualAssociationResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totVisualAssociationRating = 0;
                    foreach (CognitionVisualAssociationDetail CTest in CTlist)
                    {
                        if (enumCTest_VisualAssociationResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totVisualAssociationRating = totVisualAssociationRating + Convert.ToInt32(CTest.Rating);
                        CognitionVisualAssociationDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_VisualAssociationResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totVisualAssociationRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game TrailsB for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>TrailsB game details</returns>
        public CognitionTrailsBViewModel GetCognitionTrailsB(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionTrailsBViewModel();
            try
            {
                IQueryable<CognitionTrailsBDetail> enumCTest_TrailsBResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_TrailsBResultDetailList = (from CTest_TrailsBResult in _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                         select new CognitionTrailsBDetail
                                                         {
                                                             TrailsBResultID = CTest_TrailsBResult.TrailsBResultID,
                                                             TotalAttempts = CTest_TrailsBResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBResult.TotalAttempts,
                                                             StartTime = (DateTime)CTest_TrailsBResult.StartTime,
                                                             EndTime = (DateTime)CTest_TrailsBResult.EndTime,
                                                             Rating = (String)CTest_TrailsBResult.Rating.ToString(),
                                                             CreatedOn = (DateTime)CTest_TrailsBResult.CreatedOn,
                                                             Status = (byte)CTest_TrailsBResult.Status
                                                         });
                }
                else
                {
                    enumCTest_TrailsBResultDetailList = (from CTest_TrailsBResult in _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                         select new CognitionTrailsBDetail
                                                         {
                                                             TrailsBResultID = CTest_TrailsBResult.TrailsBResultID,
                                                             TotalAttempts = CTest_TrailsBResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBResult.TotalAttempts,
                                                             StartTime = (DateTime)CTest_TrailsBResult.StartTime,
                                                             EndTime = (DateTime)CTest_TrailsBResult.EndTime,
                                                             Rating = (String)CTest_TrailsBResult.Rating.ToString(),
                                                             CreatedOn = (DateTime)CTest_TrailsBResult.CreatedOn,
                                                             Status = (byte)CTest_TrailsBResult.Status
                                                         });
                }

                List<CognitionTrailsBDetail> CTlist = enumCTest_TrailsBResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totTrailsBRating = 0;
                    foreach (CognitionTrailsBDetail CTest in CTlist)
                    {
                        IQueryable<CognitionTrailsBResultDetail> enumCognitionTrailsBResultDetailList = null;
                        enumCognitionTrailsBResultDetailList = (from cognitionTrailsBResultDtl in _UnitOfWork.ITrailsBResultDtlRepository.RetrieveAll().Where(u => u.TrailsBResultID == CTest.TrailsBResultID)
                                                                select new CognitionTrailsBResultDetail
                                                                {
                                                                    Alphabet = cognitionTrailsBResultDtl.Alphabet,
                                                                    Status = cognitionTrailsBResultDtl.Status,
                                                                    TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                    
                                                                    Sequence = cognitionTrailsBResultDtl.Sequence
                                                                });
                        enumCognitionTrailsBResultDetailList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionTrailsBResultDetail = enumCognitionTrailsBResultDetailList.ToList();

                        if (enumCTest_TrailsBResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totTrailsBRating = totTrailsBRating + Convert.ToInt32(CTest.Rating);
                        CognitionTrailsBDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_TrailsBResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totTrailsBRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// Gets the cognition trails b new.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public CognitionTrailsBNewViewModel GetCognitionTrailsBNew(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionTrailsBNewViewModel();
            try
            {
                IQueryable<CognitionTrailsBNewDetail> enumCTest_TrailsBNewResultDetailList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_TrailsBNewResultDetailList = (from CTest_TrailsBNewResult in _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                            select new CognitionTrailsBNewDetail
                                                            {
                                                                TrailsBResultID = CTest_TrailsBNewResult.TrailsBNewResultID,
                                                                TotalAttempts = CTest_TrailsBNewResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBNewResult.TotalAttempts,
                                                                StartTime = (DateTime)CTest_TrailsBNewResult.StartTime,
                                                                EndTime = (DateTime)CTest_TrailsBNewResult.EndTime,
                                                                Rating = (String)CTest_TrailsBNewResult.Rating.ToString(),
                                                                CreatedOn = (DateTime)CTest_TrailsBNewResult.CreatedOn,
                                                                Version = (int)CTest_TrailsBNewResult.Version,
                                                                Status = (byte)CTest_TrailsBNewResult.Status
                                                            });
                }
                else
                {
                    enumCTest_TrailsBNewResultDetailList = (from CTest_TrailsBNewResult in _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                            select new CognitionTrailsBNewDetail
                                                            {
                                                                TrailsBResultID = CTest_TrailsBNewResult.TrailsBNewResultID,
                                                                TotalAttempts = CTest_TrailsBNewResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBNewResult.TotalAttempts,
                                                                StartTime = (DateTime)CTest_TrailsBNewResult.StartTime,
                                                                EndTime = (DateTime)CTest_TrailsBNewResult.EndTime,
                                                                Rating = (String)CTest_TrailsBNewResult.Rating.ToString(),
                                                                CreatedOn = (DateTime)CTest_TrailsBNewResult.CreatedOn,
                                                                Version = (int)CTest_TrailsBNewResult.Version,
                                                                Status = (byte)CTest_TrailsBNewResult.Status
                                                            });
                }
                List<CognitionTrailsBNewDetail> CTlist = enumCTest_TrailsBNewResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totTrailsBRating = 0;
                    foreach (CognitionTrailsBNewDetail CTest in CTlist)
                    {
                        IQueryable<CognitionTrailsBNewResultDetail> enumCognitionTrailsBResultDetailList = null;
                        enumCognitionTrailsBResultDetailList = (from cognitionTrailsBResultDtl in _UnitOfWork.ITrailsBNewResultDtlRepository.RetrieveAll().Where(u => u.TrailsBNewResultID == CTest.TrailsBResultID)
                                                                select new CognitionTrailsBNewResultDetail
                                                                {
                                                                    Alphabet = cognitionTrailsBResultDtl.Alphabet,
                                                                    Status = cognitionTrailsBResultDtl.Status,
                                                                    TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                    
                                                                    Sequence = cognitionTrailsBResultDtl.Sequence
                                                                });
                        enumCognitionTrailsBResultDetailList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionTrailsBNewResultDetail = enumCognitionTrailsBResultDetailList.ToList();

                        if (enumCTest_TrailsBNewResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totTrailsBRating = totTrailsBRating + Convert.ToInt32(CTest.Rating);
                        CognitionTrailsBNewDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_TrailsBNewResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totTrailsBRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// Gets the cognition trails b dot touch.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public CognitionTrailsBDotTouchViewModel GetCognitionTrailsBDotTouch(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionTrailsBDotTouchViewModel();
            try
            {
                IQueryable<CognitionTrailsBDotTouchDetail> enumCTest_TrailsBDotTouchResultDetailList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_TrailsBDotTouchResultDetailList = (from CTest_TrailsBDotTouchResult in _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                                 select new CognitionTrailsBDotTouchDetail
                                                                 {
                                                                     TrailsBResultID = CTest_TrailsBDotTouchResult.TrailsBDotTouchResultID,
                                                                     TotalAttempts = CTest_TrailsBDotTouchResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBDotTouchResult.TotalAttempts,
                                                                     StartTime = (DateTime)CTest_TrailsBDotTouchResult.StartTime,
                                                                     EndTime = (DateTime)CTest_TrailsBDotTouchResult.EndTime,
                                                                     Rating = (String)CTest_TrailsBDotTouchResult.Rating.ToString(),
                                                                     CreatedOn = (DateTime)CTest_TrailsBDotTouchResult.CreatedOn,
                                                                     Status = (byte)CTest_TrailsBDotTouchResult.Status
                                                                 });
                }
                else
                {
                    enumCTest_TrailsBDotTouchResultDetailList = (from CTest_TrailsBDotTouchResult in _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                                 select new CognitionTrailsBDotTouchDetail
                                                                 {
                                                                     TrailsBResultID = CTest_TrailsBDotTouchResult.TrailsBDotTouchResultID,
                                                                     TotalAttempts = CTest_TrailsBDotTouchResult.TotalAttempts == null ? 0 : (int)CTest_TrailsBDotTouchResult.TotalAttempts,
                                                                     StartTime = (DateTime)CTest_TrailsBDotTouchResult.StartTime,
                                                                     EndTime = (DateTime)CTest_TrailsBDotTouchResult.EndTime,
                                                                     Rating = (String)CTest_TrailsBDotTouchResult.Rating.ToString(),
                                                                     CreatedOn = (DateTime)CTest_TrailsBDotTouchResult.CreatedOn,
                                                                     Status = (byte)CTest_TrailsBDotTouchResult.Status
                                                                 });
                }
                List<CognitionTrailsBDotTouchDetail> CTlist = enumCTest_TrailsBDotTouchResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totTrailsBRating = 0;
                    foreach (CognitionTrailsBDotTouchDetail CTest in CTlist)
                    {
                        IQueryable<CognitionTrailsBDotTouchResultDetail> enumCognitionTrailsBDotTouchResultDetailList = null;
                        enumCognitionTrailsBDotTouchResultDetailList = (from cognitionTrailsBResultDtl in _UnitOfWork.ITrailsBDotTouchResultDtlRepository.RetrieveAll().Where(u => u.TrailsBDotTouchResultID == CTest.TrailsBResultID)
                                                                        select new CognitionTrailsBDotTouchResultDetail
                                                                        {
                                                                            Alphabet = cognitionTrailsBResultDtl.Alphabet,
                                                                            Status = cognitionTrailsBResultDtl.Status,
                                                                            TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                            
                                                                            Sequence = cognitionTrailsBResultDtl.Sequence
                                                                        });
                        enumCognitionTrailsBDotTouchResultDetailList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionTrailsBDotTouchResultDetail = enumCognitionTrailsBDotTouchResultDetailList.ToList();

                        if (enumCTest_TrailsBDotTouchResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totTrailsBRating = totTrailsBRating + Convert.ToInt32(CTest.Rating);
                        CognitionTrailsBDotTouchDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_TrailsBDotTouchResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totTrailsBRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// Gets the cognition jewels trails a.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public CognitionJewelsTrailsAViewModel GetCognitionJewelsTrailsA(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionJewelsTrailsAViewModel();
            try
            {
                IEnumerable<CognitionJewelsTrailsADetail> enumCTest_JewelsTrailsAResultDetailList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_JewelsTrailsAResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID).ToList()
                     .Select(c => new CognitionJewelsTrailsADetail()
                     {
                         TrailsBResultID = c.JewelsTrailsAResultID,
                         TotalAttempts = c.TotalAttempts == null ? 0 : (int)c.TotalAttempts,
                         StartTime = (DateTime)c.StartTime,
                         EndTime = (DateTime)c.EndTime,
                         Rating = (String)c.Rating.ToString(),
                         CreatedOn = (DateTime)c.CreatedOn,
                         TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                         TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                         Status = (byte)c.Status
                     });
                }
                else
                {
                    enumCTest_JewelsTrailsAResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null).ToList()
                     .Select(c => new CognitionJewelsTrailsADetail()
                     {
                         TrailsBResultID = c.JewelsTrailsAResultID,
                         TotalAttempts = c.TotalAttempts == null ? 0 : (int)c.TotalAttempts,
                         StartTime = (DateTime)c.StartTime,
                         EndTime = (DateTime)c.EndTime,
                         Rating = (String)c.Rating.ToString(),
                         CreatedOn = (DateTime)c.CreatedOn,
                         TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                         TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                         Status = (byte)c.Status
                     });
                }
                List<CognitionJewelsTrailsADetail> CTlist = enumCTest_JewelsTrailsAResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totTrailsBRating = 0;
                    foreach (CognitionJewelsTrailsADetail CTest in CTlist)
                    {
                        IQueryable<CognitionJewelsTrailsAResultDetail> enumCognitionJewelsTrailsAResultDetailList = null;
                        enumCognitionJewelsTrailsAResultDetailList = (from cognitionTrailsBResultDtl in _UnitOfWork.IJewelsTrailsAResultDtlRepository.RetrieveAll().Where(u => u.JewelsTrailsAResultID == CTest.TrailsBResultID)
                                                                      select new CognitionJewelsTrailsAResultDetail
                                                                      {
                                                                          Alphabet = cognitionTrailsBResultDtl.Alphabet,
                                                                          Status = cognitionTrailsBResultDtl.Status,
                                                                          TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                          
                                                                          Sequence = cognitionTrailsBResultDtl.Sequence
                                                                      });
                        enumCognitionJewelsTrailsAResultDetailList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionJewelsTrailsAResultDetail = enumCognitionJewelsTrailsAResultDetailList.ToList();

                        if (enumCTest_JewelsTrailsAResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totTrailsBRating = totTrailsBRating + Convert.ToInt32(CTest.Rating);
                        CognitionJewelsTrailsADetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_JewelsTrailsAResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totTrailsBRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
                if (enumCTest_JewelsTrailsAResultDetailList.Count() > 0)
                {
                    model.TotalBonusCollected = enumCTest_JewelsTrailsAResultDetailList.Sum(s => s.TotalBonusCollected) ?? 0;
                    model.TotalJewelsCollected = enumCTest_JewelsTrailsAResultDetailList.Sum(s => s.TotalJewelsCollected) ?? 0;
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// Gets the cognition jewels trails b.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public CognitionJewelsTrailsBViewModel GetCognitionJewelsTrailsB(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionJewelsTrailsBViewModel();
            try
            {
                IEnumerable<CognitionJewelsTrailsBDetail> enumCTest_JewelsTrailsBResultDetailList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_JewelsTrailsBResultDetailList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID).ToList()
                     .Select(c => new CognitionJewelsTrailsBDetail()
                     {
                         TrailsBResultID = c.JewelsTrailsBResultID,
                         TotalAttempts = c.TotalAttempts == null ? 0 : (int)c.TotalAttempts,
                         StartTime = (DateTime)c.StartTime,
                         EndTime = (DateTime)c.EndTime,
                         Rating = (String)c.Rating.ToString(),
                         CreatedOn = (DateTime)c.CreatedOn,
                         TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                         TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                         Status = (byte)c.Status
                     });
                }
                else
                {
                    enumCTest_JewelsTrailsBResultDetailList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null).ToList()
                     .Select(c => new CognitionJewelsTrailsBDetail()
                     {
                         TrailsBResultID = c.JewelsTrailsBResultID,
                         TotalAttempts = c.TotalAttempts == null ? 0 : (int)c.TotalAttempts,
                         StartTime = (DateTime)c.StartTime,
                         EndTime = (DateTime)c.EndTime,
                         Rating = (String)c.Rating.ToString(),
                         CreatedOn = (DateTime)c.CreatedOn,
                         TotalBonusCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalBonusCollected)),
                         TotalJewelsCollected = Convert.ToInt32(CryptoUtil.DecryptInfo(c.TotalJewelsCollected)),
                         Status = (byte)c.Status
                     });
                }
                List<CognitionJewelsTrailsBDetail> CTlist = enumCTest_JewelsTrailsBResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totTrailsBRating = 0;
                    foreach (CognitionJewelsTrailsBDetail CTest in CTlist)
                    {
                        IQueryable<CognitionJewelsTrailsBResultDetail> enumCognitionJewelsTrailsBResultDetailList = null;
                        enumCognitionJewelsTrailsBResultDetailList = (from cognitionTrailsBResultDtl in _UnitOfWork.IJewelsTrailsBResultDtlRepository.RetrieveAll().Where(u => u.JewelsTrailsBResultID == CTest.TrailsBResultID)
                                                                      select new CognitionJewelsTrailsBResultDetail
                                                                      {
                                                                          Alphabet = cognitionTrailsBResultDtl.Alphabet,
                                                                          Status = cognitionTrailsBResultDtl.Status,
                                                                          TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                          
                                                                          Sequence = cognitionTrailsBResultDtl.Sequence
                                                                      });
                        enumCognitionJewelsTrailsBResultDetailList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionJewelsTrailsBResultDetail = enumCognitionJewelsTrailsBResultDetailList.ToList();

                        if (enumCTest_JewelsTrailsBResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totTrailsBRating = totTrailsBRating + Convert.ToInt32(CTest.Rating);
                        CognitionJewelsTrailsBDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_JewelsTrailsBResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totTrailsBRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
                if (enumCTest_JewelsTrailsBResultDetailList.Count() > 0)
                {
                    model.TotalBonusCollected = enumCTest_JewelsTrailsBResultDetailList.Sum(s => s.TotalBonusCollected) ?? 0;
                    model.TotalJewelsCollected = enumCTest_JewelsTrailsBResultDetailList.Sum(s => s.TotalJewelsCollected) ?? 0;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Digit Span for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>DigitSpan game details</returns>
        public CognitionDigitSpanViewModel GetCognitionDigitSpan(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionDigitSpanViewModel();
            try
            {
                IQueryable<CognitionDigitSpanDetail> enumCTest_DigitSpanResultDetailList = null;
                if (adminBatchSchID > 0)
                {
                    enumCTest_DigitSpanResultDetailList = (from CTest_DigitSpanResult in _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                           select new CognitionDigitSpanDetail
                                                           {
                                                               DigitSpanResultID = CTest_DigitSpanResult.DigitSpanResultID,
                                                               Type = (byte)CTest_DigitSpanResult.Type,
                                                               CorrectAnswers = (int)CTest_DigitSpanResult.CorrectAnswers,
                                                               WrongAnswers = (int)CTest_DigitSpanResult.WrongAnswers,
                                                               StartTime = (DateTime)CTest_DigitSpanResult.StartTime,
                                                               EndTime = (DateTime)CTest_DigitSpanResult.EndTime,
                                                               Rating = (String)CTest_DigitSpanResult.Rating.ToString(),
                                                               CreatedOn = (DateTime)CTest_DigitSpanResult.CreatedOn,
                                                               Status = (byte)CTest_DigitSpanResult.Status
                                                           });
                }
                else
                {
                    enumCTest_DigitSpanResultDetailList = (from CTest_DigitSpanResult in _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                           select new CognitionDigitSpanDetail
                                                           {
                                                               DigitSpanResultID = CTest_DigitSpanResult.DigitSpanResultID,
                                                               Type = (byte)CTest_DigitSpanResult.Type,
                                                               CorrectAnswers = (int)CTest_DigitSpanResult.CorrectAnswers,
                                                               WrongAnswers = (int)CTest_DigitSpanResult.WrongAnswers,
                                                               StartTime = (DateTime)CTest_DigitSpanResult.StartTime,
                                                               EndTime = (DateTime)CTest_DigitSpanResult.EndTime,
                                                               Rating = (String)CTest_DigitSpanResult.Rating.ToString(),
                                                               CreatedOn = (DateTime)CTest_DigitSpanResult.CreatedOn,
                                                               Status = (byte)CTest_DigitSpanResult.Status
                                                           });
                }

                List<CognitionDigitSpanDetail> CTlist = enumCTest_DigitSpanResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totDigitSpanRating = 0;
                    foreach (CognitionDigitSpanDetail CTest in CTlist)
                    {
                        if (enumCTest_DigitSpanResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totDigitSpanRating = totDigitSpanRating + Convert.ToInt32(CTest.Rating);
                        CognitionDigitSpanDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_DigitSpanResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totDigitSpanRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the Details Of the game 3D figures for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>3DFigure game details</returns>
        public Cognition3DFigureViewModel GetCognition3DFigure(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new Cognition3DFigureViewModel();
            try
            {
                IQueryable<Cognition3DFigureDetails> enumCTest_3DFigureResultList = null;

                if (adminBatchSchID > 0)
                {
                    enumCTest_3DFigureResultList = (from CTest_3DFigureResult in _UnitOfWork.I3DFigureResultRepository.RetrieveAll()
                                                    where CTest_3DFigureResult.UserID == UserID && CTest_3DFigureResult.AdminBatchSchID == adminBatchSchID
                                                    select new Cognition3DFigureDetails
                                                    {
                                                        FigureResultID = CTest_3DFigureResult.C3DFigureResultID,
                                                        FigureID = CTest_3DFigureResult.C3DFigureID,
                                                        GameName = CTest_3DFigureResult.GameName,
                                                        DrawnFigFileName = (String)CTest_3DFigureResult.DrawnFigFileName,
                                                        StartTime = (DateTime)CTest_3DFigureResult.StartTime,
                                                        EndTime = (DateTime)CTest_3DFigureResult.EndTime,
                                                        CreatedOn = (DateTime)CTest_3DFigureResult.CreatedOn,
                                                        Points = (decimal)CTest_3DFigureResult.Point,
                                                    });
                }
                else
                {
                    enumCTest_3DFigureResultList = (from CTest_3DFigureResult in _UnitOfWork.I3DFigureResultRepository.RetrieveAll()
                                                    where CTest_3DFigureResult.UserID == UserID && CTest_3DFigureResult.AdminBatchSchID == null
                                                    select new Cognition3DFigureDetails
                                                    {
                                                        FigureResultID = CTest_3DFigureResult.C3DFigureResultID,
                                                        FigureID = CTest_3DFigureResult.C3DFigureID,
                                                        GameName = CTest_3DFigureResult.GameName,
                                                        DrawnFigFileName = (String)CTest_3DFigureResult.DrawnFigFileName,
                                                        StartTime = (DateTime)CTest_3DFigureResult.StartTime,
                                                        EndTime = (DateTime)CTest_3DFigureResult.EndTime,
                                                        CreatedOn = (DateTime)CTest_3DFigureResult.CreatedOn,
                                                        Points = (decimal)CTest_3DFigureResult.Point,
                                                    });
                }
                List<Cognition3DFigureDetails> CTlist = enumCTest_3DFigureResultList.ToList().OrderBy(o => CryptoUtil.DecryptInfo(o.GameName)).ToList();
                List<Cognition3DFigureDetail> FigureGrouplist = new List<Cognition3DFigureDetail>();
                model.UserID = UserID;
                if (CTlist != null && CTlist.Count != 0)
                {
                    int gameCount = 1;
                    string currGName = "";
                    List<Cognition3DFigureGroupDetail> Figurelist = new List<Cognition3DFigureGroupDetail>();
                    Cognition3DFigureDetail FDetail = new Cognition3DFigureDetail();
                    TimeSpan? gameDuration = null;
                    bool isLastGame = true;
                    foreach (Cognition3DFigureDetails CTest in CTlist)
                    {
                        if (CTlist.First() == CTest)
                            gameDuration = CTest.EndTime.Subtract(CTest.StartTime);

                        if (currGName != CTest.GameName)
                        {
                            if (CTlist.First() != CTest)
                                isLastGame = false;
                            if (currGName != "")
                            {
                                FDetail.CTest_Cognition3DFigureGroupDetailList = Figurelist;
                                model.CTest_3DFigureResultList.Add(FDetail);
                                gameCount++;
                            }
                            FDetail = new Cognition3DFigureDetail();
                            currGName = CTest.GameName;
                            FDetail.GameNumber = gameCount;

                            FDetail.GameName = string.Empty;
                            Figurelist = new List<Cognition3DFigureGroupDetail>();
                        }
                        if (CTlist.First() != CTest && isLastGame == true)
                            gameDuration = gameDuration + CTest.EndTime.Subtract(CTest.StartTime);

                        FDetail.Points = Convert.ToInt16(CTest.Points);
                        model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                        if (currGName != CTest.GameName)
                        {

                        }

                        Cognition3DFigureGroupDetail FGDetail = new Cognition3DFigureGroupDetail();
                        FGDetail.FigureResultID = CTest.FigureResultID;
                        CTest_3DFigure Figure = _UnitOfWork.I3DFigureRepository.GetById(CTest.FigureID);
                        FGDetail.FileName = CryptoUtil.DecryptInfo(Figure.FileName);
                        FGDetail.DrawnFigFileName = CryptoUtil.DecryptInfo(CTest.DrawnFigFileName);
                        FGDetail.StartTime = CTest.StartTime.AddMinutes(offsetValue);
                        FGDetail.EndTime = CTest.EndTime.AddMinutes(offsetValue);
                        FGDetail.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (FGDetail.Duration.TotalHours > 24)
                        {
                            FGDetail.DurationString = "More Than a Day";
                        }
                        else
                        {
                            FGDetail.DurationString = FGDetail.Duration.ToString();
                        }
                        Figurelist.Add(FGDetail);
                        if (CTlist.Last() == CTest)
                        {
                            FDetail.CTest_Cognition3DFigureGroupDetailList = Figurelist;
                            model.CTest_3DFigureResultList.Add(FDetail);
                        }
                    }
                    model.Duration = (TimeSpan)gameDuration;
                    if (model.Duration.TotalHours > 24)
                    {
                        model.DurationString = "More Than a Day";
                    }
                    else
                    {
                        model.DurationString = model.Duration.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// To get the details of  the game Spatial Span for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>Spatial Span game details</returns>
        public CognitionSpatialSpanViewModel GetCognitionSpatialSpan(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionSpatialSpanViewModel();
            try
            {
                IQueryable<CognitionSpatialSpanDetail> enumCTest_SpatialSpanResultDetailList = null;

                if (adminBatchSchID > 0)
                {
                    enumCTest_SpatialSpanResultDetailList = (from CTest_SpatialSpanResult in _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                                             select new CognitionSpatialSpanDetail
                                                             {
                                                                 SpatialResultID = CTest_SpatialSpanResult.SpatialResultID,
                                                                 Type = (byte)CTest_SpatialSpanResult.Type,
                                                                 CorrectAnswers = (int)CTest_SpatialSpanResult.CorrectAnswers,
                                                                 WrongAnswers = (int)CTest_SpatialSpanResult.WrongAnswers,
                                                                 StartTime = (DateTime)CTest_SpatialSpanResult.StartTime,
                                                                 EndTime = (DateTime)CTest_SpatialSpanResult.EndTime,
                                                                 Rating = (String)CTest_SpatialSpanResult.Rating.ToString(),
                                                                 CreatedOn = (DateTime)CTest_SpatialSpanResult.CreatedOn,
                                                                 Status = (byte)CTest_SpatialSpanResult.Status
                                                             });
                }
                else
                {
                    enumCTest_SpatialSpanResultDetailList = (from CTest_SpatialSpanResult in _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                                             select new CognitionSpatialSpanDetail
                                                             {
                                                                 SpatialResultID = CTest_SpatialSpanResult.SpatialResultID,
                                                                 Type = (byte)CTest_SpatialSpanResult.Type,
                                                                 CorrectAnswers = (int)CTest_SpatialSpanResult.CorrectAnswers,
                                                                 WrongAnswers = (int)CTest_SpatialSpanResult.WrongAnswers,
                                                                 StartTime = (DateTime)CTest_SpatialSpanResult.StartTime,
                                                                 EndTime = (DateTime)CTest_SpatialSpanResult.EndTime,
                                                                 Rating = (String)CTest_SpatialSpanResult.Rating.ToString(),
                                                                 CreatedOn = (DateTime)CTest_SpatialSpanResult.CreatedOn,
                                                                 Status = (byte)CTest_SpatialSpanResult.Status
                                                             });
                }

                List<CognitionSpatialSpanDetail> CTlist = enumCTest_SpatialSpanResultDetailList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int totSpatialSpanRating = 0;
                    foreach (CognitionSpatialSpanDetail CTest in CTlist)
                    {
                        IQueryable<CognitionSpatialSpanResultDetail> enumCognitionSpatialSpanResultList = null;
                        enumCognitionSpatialSpanResultList = (from cognitionTrailsBResultDtl in _UnitOfWork.ISpatialResultDtlRepository.RetrieveAll().Where(u => u.SpatialResultID == CTest.SpatialResultID)
                                                              select new CognitionSpatialSpanResultDetail
                                                              {
                                                                  GameIndex = cognitionTrailsBResultDtl.GameIndex,
                                                                  Status = cognitionTrailsBResultDtl.Status,
                                                                  TimeTaken = cognitionTrailsBResultDtl.TimeTaken,                                                                  
                                                                  Sequence = cognitionTrailsBResultDtl.Sequence,
                                                                  Level = cognitionTrailsBResultDtl.Level
                                                              }).OrderBy(x => x.GameIndex);
                        enumCognitionSpatialSpanResultList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        CTest.CognitionSpatialSpanResultDetail = enumCognitionSpatialSpanResultList.ToList();

                        if (enumCTest_SpatialSpanResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        totSpatialSpanRating = totSpatialSpanRating + Convert.ToInt32(CTest.Rating);
                        CognitionSpatialSpanDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.CTest_SpatialSpanResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = totSpatialSpanRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get CatAndDogNew game details
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public CognitionCatAndDogNewViewModel GetCognitionCatAndDogNew(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionCatAndDogNewViewModel();
            try
            {
                IQueryable<CatAndDogNewDetail> _CatAndDogList = null;
                if (adminBatchSchID > 0)
                {

                    _CatAndDogList = (from catAndDog in _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                      select new CatAndDogNewDetail
                                      {
                                          CatAndDogNewResultID = catAndDog.CatAndDogNewResultID,
                                          CorrectAnswers = (int)catAndDog.CorrectAnswers,
                                          WrongAnswers = (int)catAndDog.WrongAnswers,
                                          StartTime = (DateTime)catAndDog.StartTime,
                                          EndTime = (DateTime)catAndDog.EndTime,
                                          Rating = (String)catAndDog.Rating.ToString(),
                                          CreatedOn = (DateTime)catAndDog.CreatedOn,
                                          Status = (byte)catAndDog.Status
                                      });
                }
                else
                {
                    _CatAndDogList = (from catAndDog in _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                      select new CatAndDogNewDetail
                                      {
                                          CatAndDogNewResultID = catAndDog.CatAndDogNewResultID,
                                          CorrectAnswers = (int)catAndDog.CorrectAnswers,
                                          WrongAnswers = (int)catAndDog.WrongAnswers,
                                          StartTime = (DateTime)catAndDog.StartTime,
                                          EndTime = (DateTime)catAndDog.EndTime,
                                          Rating = (String)catAndDog.Rating.ToString(),
                                          CreatedOn = (DateTime)catAndDog.CreatedOn,
                                          Status = (byte)catAndDog.Status
                                      });
                }

                List<CatAndDogNewDetail> CTlist = _CatAndDogList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int catAndDogRating = 0;
                    foreach (CatAndDogNewDetail CTest in CTlist)
                    {
                        if (_CatAndDogList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        catAndDogRating = catAndDogRating + Convert.ToInt32(CTest.Rating);
                        CatAndDogNewDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);

                        IQueryable<CatAndDogNewGameLevelDetail> _GameLevelList = null;
                        _GameLevelList = (from catAndDog in _UnitOfWork.ICatAndDogNewResultDtlRepository.RetrieveAll().Where(c => c.CatAndDogNewResultID == CTest.CatAndDogNewResultID).OrderBy(o => o.CatAndDogNewResultDtlID)
                                          select new CatAndDogNewGameLevelDetail
                                          {
                                              CatAndDogNewResultID = catAndDog.CatAndDogNewResultID,
                                              CorrectAnswer = (int)catAndDog.CorrectAnswers,
                                              WrongAnswer = (int)catAndDog.WrongAnswers,
                                              TimeTaken = catAndDog.TimeTaken
                                          });
                        _GameLevelList.ToList().Select(game =>
                        {
                            game.TimeTaken = TimeSpan.FromSeconds(Convert.ToDouble(game.TimeTaken)).TotalMilliseconds.ToString();
                            return game;
                        }).ToList();

                        if (_GameLevelList != null && _GameLevelList.Count() > 0)
                        {
                            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(_GameLevelList.ToList().ToList());
                            data.jsonGameLevelDetails = json;
                        }
                        model.CatAndDogNewGameList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = catAndDogRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get Temporal Order game details
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public CognitionTemporalOrderViewModel GetCognitionTemporalOrder(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionTemporalOrderViewModel();
            try
            {
                IQueryable<TemporalOrderDetail> _TemporalOrderList = null;
                if (adminBatchSchID > 0)
                {

                    _TemporalOrderList = (from catAndDog in _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == adminBatchSchID)
                                          select new TemporalOrderDetail
                                          {
                                              TemporalOrderResultID = catAndDog.TemporalOrderResultID,
                                              CorrectAnswers = (int)catAndDog.CorrectAnswers,
                                              WrongAnswers = (int)catAndDog.WrongAnswers,
                                              StartTime = (DateTime)catAndDog.StartTime,
                                              EndTime = (DateTime)catAndDog.EndTime,
                                              Rating = (String)catAndDog.Rating.ToString(),
                                              CreatedOn = (DateTime)catAndDog.CreatedOn,
                                              Version = (int)catAndDog.Version,
                                              Status = (byte)catAndDog.Status
                                          });
                }
                else
                {
                    _TemporalOrderList = (from catAndDog in _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == UserID && u.AdminBatchSchID == null)
                                          select new TemporalOrderDetail
                                          {
                                              TemporalOrderResultID = catAndDog.TemporalOrderResultID,
                                              CorrectAnswers = (int)catAndDog.CorrectAnswers,
                                              WrongAnswers = (int)catAndDog.WrongAnswers,
                                              StartTime = (DateTime)catAndDog.StartTime,
                                              EndTime = (DateTime)catAndDog.EndTime,
                                              Rating = (String)catAndDog.Rating.ToString(),
                                              CreatedOn = (DateTime)catAndDog.CreatedOn,
                                              Version = (int)catAndDog.Version,
                                              Status = (byte)catAndDog.Status
                                          });
                }

                List<TemporalOrderDetail> CTlist = _TemporalOrderList.ToList();
                if (CTlist != null && CTlist.Count != 0)
                {
                    model.UserID = UserID;
                    int catAndDogRating = 0;
                    foreach (TemporalOrderDetail CTest in CTlist)
                    {
                        if (_TemporalOrderList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                                model.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                            }
                        }
                        if (model.Duration.TotalHours > 24)
                        {
                            model.DurationString = "More Than a Day";
                        }
                        else
                        {
                            model.DurationString = model.Duration.ToString();
                        }
                        catAndDogRating = catAndDogRating + Convert.ToInt32(CTest.Rating);
                        TemporalOrderDetail data = CTest;
                        data.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (data.Duration.TotalHours > 24)
                        {
                            data.DurationString = "More Than a Day";
                        }
                        else
                        {
                            data.DurationString = data.Duration.ToString();
                        }
                        switch (CTest.Rating)
                        {
                            case "0": data.Rating = "Bad"; break;
                            case "1": data.Rating = "Average"; break;
                            case "2": data.Rating = "Good"; break;
                            case "3": data.Rating = "Very Good"; break;
                        }
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        data.EndTime = data.EndTime.AddMinutes(offsetValue);
                        model.TemporalOrderGameList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                    double AvarageRate = catAndDogRating / CTlist.Count;
                    switch ((int)AvarageRate)
                    {
                        case 0: model.Rating = "Bad"; break;
                        case 1: model.Rating = "Average"; break;
                        case 2: model.Rating = "Good"; break;
                        case 3: model.Rating = "Very Good"; break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);

            }
            return model;
        }

        /// <summary>
        /// To get the Details Of the game 3D figures for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>3DFigure game details</returns>
        public CognitionScratchImageViewModel GetCognitionScratchImage(long UserID, long adminBatchSchID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionScratchImageViewModel();
            try
            {
                IQueryable<CognitionScratchImageDetails> enumCTest_ScratchImageResultList = null;
                if (adminBatchSchID > 0)
                {

                    enumCTest_ScratchImageResultList = (from CTest_ScratchImageResult in _UnitOfWork.IScratchImageResultRepository.RetrieveAll()
                                                        where CTest_ScratchImageResult.UserID == UserID && CTest_ScratchImageResult.AdminBatchSchID == adminBatchSchID
                                                        select new CognitionScratchImageDetails
                                                        {
                                                            FigureResultID = CTest_ScratchImageResult.ScratchImageResultID,
                                                            FigureID = CTest_ScratchImageResult.ScratchImageID,
                                                            GameName = CTest_ScratchImageResult.GameName,
                                                            DrawnFigFileName = (String)CTest_ScratchImageResult.DrawnFigFileName,
                                                            StartTime = (DateTime)CTest_ScratchImageResult.StartTime,
                                                            EndTime = (DateTime)CTest_ScratchImageResult.EndTime,
                                                            CreatedOn = (DateTime)CTest_ScratchImageResult.CreatedOn,
                                                            Points = (decimal)CTest_ScratchImageResult.Point,
                                                        });
                }
                else
                {
                    enumCTest_ScratchImageResultList = (from CTest_ScratchImageResult in _UnitOfWork.IScratchImageResultRepository.RetrieveAll()
                                                        where CTest_ScratchImageResult.UserID == UserID && CTest_ScratchImageResult.AdminBatchSchID == null
                                                        select new CognitionScratchImageDetails
                                                        {
                                                            FigureResultID = CTest_ScratchImageResult.ScratchImageResultID,
                                                            FigureID = CTest_ScratchImageResult.ScratchImageID,
                                                            GameName = CTest_ScratchImageResult.GameName,
                                                            DrawnFigFileName = (String)CTest_ScratchImageResult.DrawnFigFileName,
                                                            StartTime = (DateTime)CTest_ScratchImageResult.StartTime,
                                                            EndTime = (DateTime)CTest_ScratchImageResult.EndTime,
                                                            CreatedOn = (DateTime)CTest_ScratchImageResult.CreatedOn,
                                                            Points = (decimal)CTest_ScratchImageResult.Point,
                                                        });
                }
                List<CognitionScratchImageDetails> CTlist = enumCTest_ScratchImageResultList.ToList().OrderByDescending(o => o.FigureResultID).ToList();
                List<CognitionScratchImageDetail> FigureGrouplist = new List<CognitionScratchImageDetail>();
                model.UserID = UserID;
                if (CTlist != null && CTlist.Count != 0)
                {
                    int gameCount = 1;
                    string currGName = "";
                    List<CognitionScratchImageGroupDetail> Figurelist = new List<CognitionScratchImageGroupDetail>();
                    CognitionScratchImageDetail FDetail = new CognitionScratchImageDetail();
                    TimeSpan? gameDuration = null;
                    bool isLastGame = true;
                    foreach (CognitionScratchImageDetails CTest in CTlist)
                    {
                        if (CTlist.First() == CTest)
                            gameDuration = CTest.EndTime.Subtract(CTest.StartTime);

                        if (currGName != CTest.GameName)
                        {
                            if (CTlist.First() != CTest)
                                isLastGame = false;
                            if (currGName != "")
                            {
                                FDetail.CTest_CognitionScratchImageGroupDetailList = Figurelist;
                                model.CTest_ScratchImageResultList.Add(FDetail);
                                gameCount++;
                            }
                            FDetail = new CognitionScratchImageDetail();
                            currGName = CTest.GameName;
                            FDetail.GameNumber = gameCount;

                            FDetail.GameName = string.Empty;
                            Figurelist = new List<CognitionScratchImageGroupDetail>();
                        }
                        if (CTlist.First() != CTest && isLastGame == true)
                            gameDuration = gameDuration + CTest.EndTime.Subtract(CTest.StartTime);

                        FDetail.Points = Convert.ToInt16(CTest.Points);
                        model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                        if (currGName != CTest.GameName)
                        {

                        }

                        CognitionScratchImageGroupDetail FGDetail = new CognitionScratchImageGroupDetail();
                        FGDetail.FigureResultID = CTest.FigureResultID;
                        CTest_ScratchImage Figure = _UnitOfWork.IScratchImageRepository.GetById(CTest.FigureID);
                        FGDetail.FileName = CryptoUtil.DecryptInfo(Figure.FileName);
                        FGDetail.DrawnFigFileName = CryptoUtil.DecryptInfo(CTest.DrawnFigFileName);
                        FGDetail.StartTime = CTest.StartTime.AddMinutes(offsetValue);
                        FGDetail.EndTime = CTest.EndTime.AddMinutes(offsetValue);
                        FGDetail.Duration = CTest.EndTime.Subtract(CTest.StartTime);
                        if (FGDetail.Duration.TotalHours > 24)
                        {
                            FGDetail.DurationString = "More Than a Day";
                        }
                        else
                        {
                            FGDetail.DurationString = FGDetail.Duration.ToString();
                        }
                        Figurelist.Add(FGDetail);
                        if (CTlist.Last() == CTest)
                        {
                            FDetail.CTest_CognitionScratchImageGroupDetailList = Figurelist;
                            model.CTest_ScratchImageResultList.Add(FDetail);
                        }
                    }
                    model.Duration = (TimeSpan)gameDuration;
                    if (model.Duration.TotalHours > 24)
                    {
                        model.DurationString = "More Than a Day";
                    }
                    else
                    {
                        model.DurationString = model.Duration.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }


        /// <summary>
        /// To get the details of  the game N back for a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <returns>NBack game details</returns>
        public CognitionSpinWheelViewModel GetCognitionSpinWheel(long UserID)
        {
            double offsetValue = HttpContext.Current.Session["OffsetValue"] == null ? (double)0 : Convert.ToDouble(HttpContext.Current.Session["OffsetValue"].ToString());
            var model = new CognitionSpinWheelViewModel();
            try
            {
                IQueryable<CognitionSpinWheelDetail> enumCTest_SpinWheelResultDetailList = null;
                enumCTest_SpinWheelResultDetailList = (from spinWheel in _UnitOfWork.ISpinWheelResultRepository.RetrieveAll().Where(u => u.UserID == UserID)
                                                       select new CognitionSpinWheelDetail
                                                       {
                                                           StartTime = (DateTime)spinWheel.StartTime,                                                           
                                                           CollectedStars = spinWheel.CollectedStars,
                                                           CreatedOn = (DateTime)spinWheel.CreatedOn
                                                       });
                List<CognitionSpinWheelDetail> CTlist = enumCTest_SpinWheelResultDetailList.ToList();

                CTlist.Select(game =>
                {
                    game.CollectedStars = CryptoUtil.DecryptInfo(game.CollectedStars);
                    return game;
                }).ToList();

                if (CTlist != null)
                {
                    model.UserID = UserID;
                    foreach (CognitionSpinWheelDetail CTest in CTlist)
                    {
                        if (enumCTest_SpinWheelResultDetailList.First() == CTest)
                        {
                            model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                        }
                        else
                        {
                            if (CTest.CreatedOn.AddMinutes(offsetValue) > model.LastCognitionDate)
                            {
                                model.LastCognitionDate = CTest.CreatedOn.AddMinutes(offsetValue);
                            }
                        }                        
                        CognitionSpinWheelDetail data = CTest;                        
                        data.StartTime = data.StartTime.AddMinutes(offsetValue);
                        model.CTest_SpinWheelResultList.Add(data);
                    }
                    model.TotalRows = CTlist.Count;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }
        #endregion

        #region SurveyManagement

        /// <summary>
        /// To get the survey list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SurveyListViewModel GetSurveys(SurveyListViewModel model)
        {
            SurveyListViewModel response = new SurveyListViewModel();
            AdminViewModel admin = GetAdminDetails(model.UserId);

            if (admin != null)
            {
                List<SurveyForList> survey = (from surveys in _UnitOfWork.ISurveyRepository.RetrieveAll().Where(s => s.AdminID == model.UserId && s.IsDeleted == false).OrderBy(s => s.SurveyName)
                                              select new SurveyForList
                                              {
                                                  SurveyID = surveys.SurveyID,
                                                  SurveyName = surveys.SurveyName,
                                                  Language = surveys.Language,
                                                  AdminFirstName = admin.FirstName,
                                                  AdminlastName = admin.LastName,
                                                  CreatedOn = (DateTime)surveys.CreatedOn
                                              }).ToList();
                survey.Select(s =>
                {
                    s.SurveyID = s.SurveyID;
                    s.SurveyName = CryptoUtil.DecryptInfo(s.SurveyName);
                    s.Language = s.Language == null ? LAMPConstants.DEFAULT_LANGUAGE : s.Language;
                    s.CreatedAdmin = CryptoUtil.DecryptInfo(s.AdminFirstName) + " " + CryptoUtil.DecryptInfo(s.AdminlastName);
                    s.CreatedOn = s.CreatedOn;
                    return s;
                }).ToList();

                if (model.SearchText != null)
                    survey = survey.FindAll(s => s.SurveyName.StartsWith(model.SearchText, StringComparison.InvariantCultureIgnoreCase));

                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "SurveyName" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "SurveyName":
                        if (sortDirection == "asc")
                            survey = survey.OrderBy(s => CryptoUtil.DecryptInfo(s.SurveyName)).ToList();
                        else
                            survey = survey.OrderByDescending(c => CryptoUtil.DecryptInfo(c.SurveyName)).ToList();
                        break;
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            survey = survey.OrderBy(c => c.CreatedOn).ToList();
                        else
                            survey = survey.OrderByDescending(c => c.CreatedOn).ToList();
                        break;
                    case "CreatedAdmin":
                        if (sortDirection == "asc")
                            survey = survey.OrderBy(c => c.CreatedAdmin).ToList();
                        else
                            survey = survey.OrderByDescending(c => c.CreatedAdmin).ToList();
                        break;
                    default:
                        survey = survey.OrderBy(c => c.SurveyName).ToList();
                        break;
                }

                response.SurveyList = survey;

            }
            return response;
        }

        /// <summary>
        /// To save survey details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SurveyViewModel SaveSurvey(SurveyViewModel model)
        {
            try
            {
                if (model.SurveyID > 0)
                {
                    Survey survey = _UnitOfWork.ISurveyRepository.GetById(model.SurveyID);
                    survey.SurveyName = model.SurveyName;
                    survey.Language = model.LanguageCode;
                    survey.EditedOn = DateTime.UtcNow;
                    _UnitOfWork.ISurveyRepository.Update(survey);

                    if (model.QuestionID != null && model.QuestionID > 0)
                    {
                        SurveyQuestion question = _UnitOfWork.ISurveyQuestionRepository.GetById(model.QuestionID);
                        question.QuestionText = model.QuestionText;
                        question.AnswerType = model.AnswerType;
                        _UnitOfWork.ISurveyQuestionRepository.Update(question);

                        if (model.Options != null && model.Options != "")
                        {
                            string[] options = new string[1];
                            if (model.Options.Contains(","))
                            {
                                options = model.Options.Split(",".ToCharArray());
                            }
                            else
                            {
                                options.SetValue(model.Options, 0);
                            }



                            var exceptionList = new List<Int64>();

                            foreach (string item in options)
                            {

                                string optionId = item.Split("_".ToCharArray())[0];
                                string optionText = item.Split("_".ToCharArray())[1];
                                if (optionText != "")
                                {
                                    if (optionId != "0")
                                    {
                                        SurveyQuestionOption option = _UnitOfWork.ISurveyQuestionOptionRepository.GetById(Convert.ToInt64(optionId));
                                        option.OptionText = optionText;
                                        _UnitOfWork.ISurveyQuestionOptionRepository.Update(option);
                                        _UnitOfWork.Commit();

                                        exceptionList.Add(Convert.ToInt64(optionId));
                                    }
                                    else
                                    {
                                        SurveyQuestionOption option = new SurveyQuestionOption();
                                        option.QuestionID = model.QuestionID;
                                        option.OptionText = optionText;
                                        _UnitOfWork.ISurveyQuestionOptionRepository.Add(option);
                                        _UnitOfWork.Commit();

                                        long newOptionId = _UnitOfWork.ISurveyQuestionOptionRepository.GetAll().Where(q => q.QuestionID == model.QuestionID).OrderByDescending(q => q.OptionID).First().OptionID;
                                        exceptionList.Add(newOptionId);
                                    }
                                }


                            }
                            // Delete the removed options from the table.
                            if (exceptionList.Count() > 0)
                            {
                                List<SurveyQuestionOption> deletedOptions = _UnitOfWork.ISurveyQuestionOptionRepository.GetAll().Where(x => x.QuestionID == model.QuestionID && !exceptionList.Contains(x.OptionID)).ToList();
                                if (deletedOptions != null && deletedOptions.Count() > 0)
                                {
                                    foreach (SurveyQuestionOption option in deletedOptions)
                                    {
                                        _UnitOfWork.ISurveyQuestionOptionRepository.Delete(option);
                                    }
                                    _UnitOfWork.Commit();
                                }
                            }
                        }
                    }
                    else // adding new questions to existing survey.
                    {
                        if (model.QuestionText != null && model.QuestionText != "")
                        {
                            SurveyQuestion question = new SurveyQuestion();
                            question.QuestionText = model.QuestionText;
                            question.AnswerType = model.AnswerType;
                            question.SurveyID = model.SurveyID;
                            question.IsDeleted = false;
                            _UnitOfWork.ISurveyQuestionRepository.Add(question);
                            _UnitOfWork.Commit();

                            if (model.Options != null && model.Options != "")
                            {
                                long questionId = _UnitOfWork.ISurveyQuestionRepository.GetAll().Where(q => q.SurveyID == model.SurveyID).OrderByDescending(q => q.QuestionID).First().QuestionID;
                                if (model.Options.Contains(","))
                                {
                                    string[] options = model.Options.Split(",".ToCharArray());
                                    foreach (string item in options)
                                    {

                                        string optionId = item.Split("_".ToCharArray())[0];
                                        string optionText = item.Split("_".ToCharArray())[1];

                                        if (optionText != "")
                                        {
                                            SurveyQuestionOption option = new SurveyQuestionOption();
                                            option.QuestionID = questionId;
                                            option.OptionText = optionText;
                                            _UnitOfWork.ISurveyQuestionOptionRepository.Add(option);
                                        }
                                    }
                                }
                                _UnitOfWork.Commit();
                            }
                        }
                    }
                    _UnitOfWork.Commit();
                }
                else
                {
                    Survey survey = new Survey();
                    survey = _UnitOfWork.ISurveyRepository.GetAll().Where(s => s.SurveyName == model.SurveyName.Trim() && s.AdminID == model.AdminID && s.IsDeleted != true).SingleOrDefault();
                    if (survey != null)
                    {
                        model.Status = LAMPConstants.ERROR_CODE;
                        model.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_SURVEY_SAVE_FAILED);
                        return model;
                    }
                    survey = new Survey();
                    survey.SurveyName = model.SurveyName.Trim();
                    survey.Language = model.LanguageCode;
                    survey.AdminID = model.AdminID;
                    survey.CreatedOn = DateTime.UtcNow;
                    survey.EditedOn = DateTime.UtcNow;
                    survey.IsDeleted = false;
                    _UnitOfWork.ISurveyRepository.Add(survey);
                    _UnitOfWork.Commit();

                    long surveyId = _UnitOfWork.ISurveyRepository.GetAll().OrderByDescending(s => s.SurveyID).First().SurveyID;

                    if (model.QuestionText != null)
                    {
                        SurveyQuestion question = new SurveyQuestion();
                        question.QuestionText = model.QuestionText.Trim();
                        question.AnswerType = model.AnswerType;
                        question.SurveyID = surveyId;
                        question.IsDeleted = false;
                        _UnitOfWork.ISurveyQuestionRepository.Add(question);
                        _UnitOfWork.Commit();

                        if (model.Options != null && model.Options != "")
                        {
                            long questionId = _UnitOfWork.ISurveyQuestionRepository.GetAll().Where(q => q.SurveyID == surveyId).OrderByDescending(q => q.QuestionID).First().QuestionID;
                            if (model.Options.Contains(","))
                            {
                                string[] options = model.Options.Split(",".ToCharArray());
                                foreach (string item in options)
                                {
                                    string optionId = item.Split("_".ToCharArray())[0];
                                    string optionText = item.Split("_".ToCharArray())[1];

                                    SurveyQuestionOption option = new SurveyQuestionOption();
                                    option.QuestionID = questionId;
                                    option.OptionText = optionText;
                                    _UnitOfWork.ISurveyQuestionOptionRepository.Add(option);
                                }
                            }
                            _UnitOfWork.Commit();
                        }
                    }
                    model.SurveyID = surveyId;
                }
                model = GetSurveyBySurveyId(model.SurveyID);
                model.Status = LAMPConstants.SUCCESS_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_SURVEY_SAVED_SUCCESSFULLY);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model.Status = LAMPConstants.ERROR_CODE;
                model.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
            }
            return model;
        }

        /// <summary>
        /// To get survey details by the given SurveyId.
        /// </summary>
        /// <param name="SurveyId"></param>
        /// <returns></returns>
        public SurveyViewModel GetSurveyBySurveyId(long SurveyId)
        {
            SurveyViewModel model = new SurveyViewModel();
            try
            {
                model = (from survey in _UnitOfWork.ISurveyRepository.Find(s => s.SurveyID == SurveyId)
                         select new SurveyViewModel
                         {
                             SurveyID = survey.SurveyID,
                             SurveyName = survey.SurveyName,
                             LanguageCode = survey.Language == null ? LAMPConstants.DEFAULT_LANGUAGE : survey.Language,
                             Questions = (from questions in _UnitOfWork.ISurveyQuestionRepository.RetrieveAll().Where(q => q.SurveyID == survey.SurveyID && q.IsDeleted == false)
                                          select new SurveyQuestionViewModel
                                          {
                                              QuestionID = questions.QuestionID,
                                              QuestionText = questions.QuestionText,
                                              AnswerType = questions.AnswerType
                                          }).ToList()

                         }).SingleOrDefault();

                foreach (SurveyQuestionViewModel question in model.Questions)
                {
                    if (question.AnswerType == (int)SurveyQuestionTypes.ScrollWheels)
                    {
                        question.Options = (from options in _UnitOfWork.ISurveyQuestionOptionRepository.RetrieveAll().Where(o => o.QuestionID == question.QuestionID)
                                            select new SurveyQuestionOptionsViewModel
                                            {
                                                OptionID = options.OptionID,
                                                OptionText = options.OptionText
                                            }).ToList();
                    }
                }

                model.Status = 0;
                model.Message = "";
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model = new SurveyViewModel
                {
                    Status = (short)LAMPConstants.API_UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return model;
        }

        /// <summary>
        /// To delete a survey.
        /// </summary>
        /// <param name="SurveyId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public SurveyListViewModel DeleteSurvey(long SurveyId, long UserId)
        {
            SurveyListViewModel model = new SurveyListViewModel();
            model.UserId = UserId;
            try
            {
                Survey survey = _UnitOfWork.ISurveyRepository.GetById(SurveyId);
                if (survey != null)
                {
                    survey.IsDeleted = true;
                    survey.EditedOn = DateTime.UtcNow;
                    _UnitOfWork.ISurveyRepository.Update(survey);
                    //Deleting survey scheduled.
                    Admin_SurveySchedule Admin_SurveySchedule = _UnitOfWork.IAdminSurveyScheduleRepository.SingleOrDefault(s => s.SurveyID == SurveyId && s.AdminID == UserId && s.IsDeleted != true);
                    if (Admin_SurveySchedule != null)
                    {
                        Admin_SurveySchedule.IsDeleted = true;
                        Admin_SurveySchedule.EditedOn = DateTime.UtcNow;
                        _UnitOfWork.IAdminSurveyScheduleRepository.Update(Admin_SurveySchedule);
                    }

                    _UnitOfWork.Commit();
                    model = GetSurveys(model);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model = new SurveyListViewModel
                {
                    Status = (short)LAMPConstants.API_UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return model;
        }

        /// <summary>
        /// To delete a survey question.
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <returns></returns>
        public SurveyViewModel DeleteSurveyQuestion(long QuestionId)
        {
            SurveyViewModel model = new SurveyViewModel();
            try
            {
                SurveyQuestion question = _UnitOfWork.ISurveyQuestionRepository.GetById(QuestionId);
                if (question != null)
                {
                    Survey survey = _UnitOfWork.ISurveyRepository.GetById(question.SurveyID);
                    List<SurveyQuestion> questionsList = _UnitOfWork.ISurveyQuestionRepository.GetAll().Where(s => s.SurveyID == survey.SurveyID && s.IsDeleted == false).ToList();
                    if (questionsList != null)
                    {
                        if (questionsList.Count == 1)
                        {
                            model.Status = LAMPConstants.ERROR_CODE;
                            model.Message = ResourceHelper.GetStringResource(LAMPConstants.MSG_SURVEY_QUESTIONS_REQUIRED);
                            return model;
                        }
                    }

                    question.IsDeleted = true;
                    _UnitOfWork.ISurveyQuestionRepository.Update(question);
                    // Update Survey - EditedOn 
                    if (survey != null)
                    {
                        survey.EditedOn = DateTime.UtcNow;
                        _UnitOfWork.ISurveyRepository.Update(survey);
                    }
                    _UnitOfWork.Commit();
                    model = GetSurveyBySurveyId(question.SurveyID);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model = new SurveyViewModel
                {
                    Status = (short)LAMPConstants.API_UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return model;
        }

        /// <summary>
        /// Edits the survey question.
        /// </summary>
        /// <param name="QuestionId">The question identifier.</param>
        /// <returns></returns>
        public SurveyViewModel EditSurveyQuestion(long QuestionId)
        {
            SurveyViewModel model = new SurveyViewModel();
            try
            {
                SurveyQuestion question = _UnitOfWork.ISurveyQuestionRepository.GetById(QuestionId);
                if (question != null)
                {
                    if (question.SurveyQuestionOptions != null && question.SurveyQuestionOptions.Count > 0)
                    {
                        foreach (SurveyQuestionOption option in question.SurveyQuestionOptions)
                        {
                            model.Options += option.OptionID + "_" + option.OptionText + ",";
                        }
                        model.Options = model.Options.Substring(0, model.Options.Length - 1);
                    }
                    model.QuestionID = question.QuestionID;
                    model.QuestionText = question.QuestionText;
                    model.AnswerType = question.AnswerType;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                model = new SurveyViewModel
                {
                    Status = (short)LAMPConstants.API_UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return model;
        }

        #endregion

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Gets the slot time options.
        /// </summary>
        /// <param name="adminSurveySchID">The admin survey SCH identifier.</param>
        /// <returns></returns>
        private List<string> GetSlotTimeOptionsForSurvey(long adminSurveySchID)
        {
            List<string> options = new List<string>();
            var SlotTimeOptionsForSurveyList = (from questions in _UnitOfWork.IAdminSurveyScheduleCustomTimeRepository.GetAll().Where(d => d.AdminSurveySchID == adminSurveySchID)
                                                select new Admin_SurveyScheduleCustomTime
                                                {
                                                    Time = questions.Time
                                                }).ToList();
            foreach (var item in SlotTimeOptionsForSurveyList)
            {
                DateTime surveyDisplayTime = (DateTime)(item.Time);
                var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;                
                options.Add(Timestamp.ToString());
            }
            return options;
        }

        /// <summary>
        /// Gets the slot time options for game.
        /// </summary>
        /// <param name="adminCTestSchID">The admin c test SCH identifier.</param>
        /// <returns></returns>
        private List<string> GetSlotTimeOptionsForGame(long adminCTestSchID)
        {
            List<string> options = new List<string>();
            var SlotTimeOptionsForSurveyList = (from questions in _UnitOfWork.IAdminCTestScheduleCustomTimeRepository.GetAll().Where(d => d.AdminCTestSchID == adminCTestSchID)
                                                select new Admin_CTestScheduleCustomTime
                                                {
                                                    Time = questions.Time
                                                }).ToList();
            foreach (var item in SlotTimeOptionsForSurveyList)
            {
                DateTime surveyDisplayTime = (DateTime)(item.Time);
                var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                options.Add(Timestamp.ToString());
            }
            return options;

        }

        /// <summary>
        /// Emails the normal admin credentials.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="adminName">Name of the admin.</param>
        /// <param name="adminPassword">The admin password.</param>
        /// <returns></returns>
        private bool EmailNormalAdminCredentials(string email, string adminName, string adminPassword)
        {
            try
            {
                EmailData Edata = new EmailData();
                Edata.Email = email;
                Edata.Subject = "LAMP : Admin Login Details";
                Edata.TemplateName = LAMPConstants.HTML_RESOURCE_ADMIN_PWD;

                var lampHost = System.Configuration.ConfigurationManager.AppSettings["lampHost"].ToString();
                Edata.Data.Add(new replaceingData { Name = "LAMP_LOGO", Value = lampHost + LAMPConstants.LOGO_PATH });
                Edata.Data.Add(new replaceingData { Name = "ADMIN_NAME", Value = adminName });
                Edata.Data.Add(new replaceingData { Name = "EMAIL", Value = email });
                Edata.Data.Add(new replaceingData { Name = "PASSWORD", Value = "Password: " + adminPassword });
                Helper.SendEmail(Edata);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Send email when save user details 
        /// </summary>
        /// <param name="email">To Email</param>
        /// <param name="user_name">Name</param>
        /// <param name="studyId">Study Id</param>
        /// <param name="studyCode">Study Code</param>
        /// <param name="user_password">Password</param>
        /// <returns>Status</returns>
        private bool EmailUserStudyId_Pwd(string email, string user_name, string studyId, string studyCode, string user_password)
        {
            try
            {
                EmailData Edata = new EmailData();
                Edata.Email = email;
                Edata.Subject = "LAMP - Account Details";
                Edata.TemplateName = LAMPConstants.HTML_RESOURCE_USER_STUDYID_PWD;
                var lampHost = System.Configuration.ConfigurationManager.AppSettings["lampHost"].ToString();
                Edata.Data.Add(new replaceingData { Name = "LAMP_LOGO", Value = lampHost + LAMPConstants.LOGO_PATH });
                Edata.Data.Add(new replaceingData { Name = "USER_NAME", Value = user_name });
                Edata.Data.Add(new replaceingData { Name = "STUDY_ID", Value = studyId });
                Edata.Data.Add(new replaceingData { Name = "STUDY_CODE", Value = studyCode });
                Edata.Data.Add(new replaceingData { Name = "PASSWORD", Value = "Password: " + user_password });
                Helper.SendEmail(Edata);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return false;
            }

        }

        /// <summary>
        /// gets all users list data
        /// </summary>
        /// <param name="model">Sorting Object</param>
        /// <returns>User List</returns>
        private UserListViewModel GetAllUsers(UserListViewModel model)
        {
            IQueryable<AdminUser> enumUserList = null;
            var listUserResponse = new UserListViewModel();
            enumUserList = (from user in _UnitOfWork.IUserRepository.RetrieveAll()
                            join survey in _UnitOfWork.ISurveyResultRepository.RetrieveAll()
                            on user.UserID equals survey.UserID
                            into userSurvey
                            where user.IsDeleted == false
                            from survey in userSurvey.DefaultIfEmpty()
                            select new AdminUser
                            {
                                UserID = user.UserID,
                                StudyId = user.StudyId,
                                RegisteredOn = user.RegisteredOn,
                                Email = user.Email,
                                Phone = user.Phone,
                                Device = "device",
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Surveys = userSurvey.Count().ToString(),
                                IsActive = (user.Status == null || user.Status == false) ? false : true,
                                ClinicalProfileURL = user.ClinicalProfileURL,
                                AdminId = (long)user.AdminID,

                            }).Distinct();
            List<AdminUser> userList = enumUserList.ToList();
            if (model.LoggedInAdminId != 1) //if no super admin, filter the user list
            {
                userList = userList.Where(u => u.AdminId == model.LoggedInAdminId).ToList();
            }
            string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "StudyId" : model.SortPageOptions.SortField;
            string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
            switch (sortField)
            {
                case "StudyId":
                    if (sortDirection == "asc")
                        userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                    else
                        userList = userList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                    break;
                case "RegisteredOn":
                    if (sortDirection == "asc")
                        userList = userList.OrderBy(c => c.RegisteredOn).ToList();
                    else
                        userList = userList.OrderByDescending(c => c.RegisteredOn).ToList();
                    break;
                case "Email":
                    if (sortDirection == "asc")
                        userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.Email)).ToList();
                    else
                        userList = userList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.Email)).ToList();
                    break;
                default:
                    userList = userList.OrderBy(c => CryptoUtil.DecryptInfo(c.StudyId)).ToList();
                    break;
            }

            listUserResponse.UserList = userList;
            listUserResponse.TotalRows = userList.Count;
            return listUserResponse;
        }

        /// <summary>
        /// Create Game object and set Date and Status for last result
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <param name="gameType">Game Type</param>
        /// <param name="gameName">Game Name</param>
        /// <param name="lastRating">Last Rating</param>
        /// <param name="overAllRating">Over all Rating</param>
        /// <param name="totalGames">Total Games</param>
        /// <param name="lastDate">Last Date</param>
        /// <param name="lastResultDate">Last Result Date</param>
        /// <param name="lastResultStatus">Last Result Status</param>
        /// <returns>Game object</returns>
        private UserCognition CreateGame(long userID, Int16 gameType, string gameName, short lastRating, short overAllRating, short totalGames, DateTime? lastDate, ref DateTime lastResultDate, ref string lastResultStatus, decimal earnedPoint, bool? IsNotificationGame, string spinWheelScore)  //,long? adminBatchSchID
        {
            LogUtil.Error("CreateGame() gameName: " + gameName + " last date: " + lastResultDate.ToString() + " new date: " + lastDate.ToString());
            if (lastDate != null)
            {
                DateTime _lastResultDate = Convert.ToDateTime(lastDate);
                if (_lastResultDate > lastResultDate)
                {
                    lastResultDate = _lastResultDate;
                    lastResultStatus = Helper.GetRating(lastRating);
                }
            }
            LogUtil.Error("CreateGame() last date: " + lastResultDate.ToString());

            UserCognition game = new UserCognition();
            game.UserID = userID;
            game.CognitionType = gameType;
            game.CognitionName = gameName;
            game.RatingName = Helper.GetRating(lastRating);
            game.OverAllRating = Helper.GetRating(overAllRating);
            game.TotalGames = totalGames;
            game.EarnedPoints = (decimal)earnedPoint;
            game.IsNotificationGame = IsNotificationGame;
            if (spinWheelScore != null && spinWheelScore.Length > 0)
                game.SpinWheelScore = spinWheelScore + " SW Score";
            return game;
        }

        private UserCognition CreateBatchScheduleGame(long adminBatchSchID, long userID, Int16 gameType, string gameName, short lastRating, short overAllRating, short totalGames, DateTime? lastDate, ref DateTime lastResultDate, ref string lastResultStatus, decimal earnedPoint, bool? IsNotificationGame, string spinWheelScore)
        {
            Admin_BatchScheduleCTest _game = _UnitOfWork.IAdminBatchScheduleCTestRepository.RetrieveAll().Where(w => w.CTestID == gameType && w.AdminBatchSchID == adminBatchSchID).FirstOrDefault();
            LogUtil.Error("CreateGame() gameName: " + gameName + " last date: " + lastResultDate.ToString() + " new date: " + lastDate.ToString());
            if (lastDate != null)
            {
                DateTime _lastResultDate = Convert.ToDateTime(lastDate);
                if (_lastResultDate > lastResultDate)
                {
                    lastResultDate = _lastResultDate;
                    lastResultStatus = Helper.GetRating(lastRating);
                }
            }
            LogUtil.Error("CreateGame() last date: " + lastResultDate.ToString());

            UserCognition game = new UserCognition();
            game.UserID = userID;
            game.CognitionType = gameType;
            game.CognitionName = gameName;
            game.RatingName = Helper.GetRating(lastRating);
            game.OverAllRating = Helper.GetRating(overAllRating);
            game.TotalGames = totalGames;
            game.EarnedPoints = (decimal)earnedPoint;
            game.AdminBatchSchID = adminBatchSchID;
            game.IsNotificationGame = IsNotificationGame;
            game.Order = _game == null ? 0 : (int)_game.Order;
            if (spinWheelScore != null && spinWheelScore.Length > 0)
                game.SpinWheelScore = spinWheelScore + " SW Score";
            return game;
        }
        #endregion       

    }
}
