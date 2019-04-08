using System;
using System.Collections.Generic;


namespace LAMP.ViewModel
{
    public class BlogResponse : APIResponseBase
    {
        public List<BlogData> BlogList { get; set; }
        public BlogResponse()
        {
            BlogList = new List<BlogData>();
        }
    }

    public class BlogData
    {
        public string BlogTitle { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public string BlogText { get; set; }
    }

    public class BlogUpdations
    {
        public long BlogId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
    }
}
