using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserListViewModel
    /// </summary>
    public class UserListViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public string SearchId { get; set; }
        public string UserTypeDropdownId { get; set; }
        public List<AdminUser> UserList { get; set; }
        public StaticPagedList<AdminUser> PagedUserList { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public string UnregisteredUserMessage { get; set; }
        public List<SelectListItem> UserTypeList { get; set; }
        public UserListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            UserList = new List<AdminUser>();
            UserTypeList = new List<SelectListItem>(){
                 new SelectListItem { Text = "User", Value = "1" },
                 new SelectListItem { Text = "Admin", Value = "2" },
                
            };
        }
    }
    /// <summary>
    /// Class AdminUser
    /// </summary>
    public class AdminUser
    {
        public long AdminId { get; set; }
        public long UserID { get; set; }
        public string StudyId { get; set; }
        public DateTime? RegisteredOn { get; set; }
        public string RegisteredOnString { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Device { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surveys { get; set; }
        public DateTime LastSurveyDate { get; set; }
        public string LastSurveyDateString { get; set; }
        public bool IsActive { get; set; }
        public string ClinicalProfileURL { get; set; }
    }

    /// <summary>
    /// Class SortPageOptions
    /// </summary>
    public class SortPageOptions : PagingBase
    {
        /// <summary>
        /// Current sort field
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// Current sort direction.
        /// </summary>
        public string SortOrder { get; set; }
    }

    public class Admins
    {
        public long AdminID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public byte? AdminType { get; set; }
    }

    /// <summary>
    /// Class AdminListViewModel
    /// </summary>
    public class AdminListViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public string SearchId { get; set; }
        public string userTypeDropdownId { get; set; }
        public List<Admins> AdminList { get; set; }
        public StaticPagedList<Admins> PagedAdminList { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public string UnregisteredUserMessage { get; set; }

        public AdminListViewModel()
        {
            SortPageOptions = new SortPageOptions();
            AdminList = new List<Admins>();
        }
    }
    public class AdminandUserListViewModel
    {
        public long LoggedInAdminId { get; set; }
        public string UserTypeDropdownId { get; set; }
        public string Command { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public AdminListViewModel AdminListViewModel { get; set; }
        public UserListViewModel UserListViewModel { get; set; }
        public AdminandUserListViewModel()
        {
            AdminListViewModel = new AdminListViewModel();
            UserListViewModel = new UserListViewModel();
        }
    }
}
