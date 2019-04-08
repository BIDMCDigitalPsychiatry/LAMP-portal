using System;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class HelpCallRequest
    /// </summary>
    public class HelpCallRequest
    {
        public long UserID { get; set; }
        public string CalledNumber { get; set; }
        public DateTime CallDateTime { get; set; }
        public long CallDuraion { get; set; }
        public byte Type { get; set; }
    }

   
}
