using System;
using System.Collections.Generic;
using PagedList;

namespace LAMP.ViewModel
{
    /// <summary>
    /// App Help ViewModel
    /// </summary>
    public class AppHelpViewModel:ViewModelBase
    {
        public long HelpID { get; set; }
        public string HelpTitle { get; set; }
        public string HelpText { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public long? AdminID { get; set; }
        public Admins Admin { get; set; }
        public string AppHelpExtension { get; set; }
        public bool IsSaved { get; set; }
        public string AdminFullName { get; set; }
        public string CreatedAdminFName { get; set; }
        public string CreatedAdminLName { get; set; }
    }
    public class AppHelpViewModelList : ViewModelBase
    {
        public long AdminLoggedInId { get; set; }
        public string SearchId { get; set; }
        public List<AppHelpViewModel> AppHelpList { get; set; }
        public StaticPagedList<AppHelpViewModel> PagedAppHelpList { get; set; }
        public string UnregisteredAppHelpMessage { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public bool? IsSuperAdminSaved { get; set; }
        public AppHelpViewModelList()
        {
            SortPageOptions = new SortPageOptions();
            AppHelpList = new List<AppHelpViewModel>();
        }
    }
}
