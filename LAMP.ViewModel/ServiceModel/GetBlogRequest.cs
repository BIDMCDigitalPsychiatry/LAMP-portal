
namespace LAMP.ViewModel
{
    /// <summary>
    /// Get Blog Request
    /// </summary>
    public class GetBlogRequest
    {
        public long UserId { get; set; }
    }

    public class GetTipsRequest
    {
        public long UserId { get; set; }
    }

    public class GetTipsandBlogsUpdateRequest
    {
        public long UserId { get; set; }
    }

    public class TipandBlogUpdateResponse : APIResponseBase 
    {
        public bool BlogsUpdate { get; set; }
        public bool TipsUpdate { get; set; }
    }

    public class GetAppHelpRequest
    {
        public long UserId { get; set; }
    }
}
