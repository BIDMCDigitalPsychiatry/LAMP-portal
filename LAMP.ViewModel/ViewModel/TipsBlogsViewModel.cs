using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PagedList;


namespace LAMP.ViewModel
{   /// <summary>
    /// Class TipsBlogsViewModel 
    /// </summary>
    public class TipsBlogsViewModel : ViewModelBase
    {
        public long LoggedInAdminId { get; set; }
        public TipsViewModel TipsViewModel { get; set; }
        public BlogsViewModel BlogsViewModel { get; set; }
        public List<BlogsViewModel> BlogList { get; set; }
        public StaticPagedList<BlogsViewModel> PagedBlogList { get; set; }
        public SortPageOptions SortPageOptions { get; set; }
        public long TotalRows { get; set; }
        public Int16 NoOfPages { get; set; }
        public TipsBlogsViewModel()
        {
            SortPageOptions = new SortPageOptions();
            BlogList = new List<BlogsViewModel>();
            TipsViewModel = new TipsViewModel();
            BlogsViewModel = new BlogsViewModel();
        }
    }
    public class TipsViewModel:ViewModelBase
    {
        public long TipID { get; set; }
        [Required(ErrorMessage = "Specify Tip.")]
        public string TipText { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime EditedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public bool IsSaved { get; set; }
        public long? AdminId { get; set; }
    }
    public class BlogsViewModel:ViewModelBase
    {
        public long BlogID { get; set; }
        [Required(ErrorMessage = "Specify Article Title.")]
        public string BlogTitle { get; set; }
        [Required(ErrorMessage = "Specify Article Content.")]
        public string Content { get; set; }
        [RegularExpression(@"^.+\.(([jJ][pP][gG])|([pP][nN][gG])|([jJ][pP][eE][gG])|([bB][mM][pP])|([tT][iI][fF])|([tT][iI][fF][fF])|([gG][iI][fF]))$", ErrorMessage = "Upload JPG, PNG, BMP or GIF image files only.")]
        public string ImageURL { get; set; }
        public string BlogExtension { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string CreatedAdminFName { get; set; }
        public string CreatedAdminLName { get; set; }
        public string CreatedAdminName { get; set; }
        public DateTime? EditedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public bool IsSaved { get; set; }
        public string BlogText { get; set; }
        public long? AdminId { get; set; }
    }

}
