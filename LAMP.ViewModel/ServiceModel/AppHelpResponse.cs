using System;


namespace LAMP.ViewModel
{
    public class AppHelpResponse : APIResponseBase
    {
        public string HelpText { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
    }
}
