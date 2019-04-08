using System;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Protocol Date Response
    /// </summary>
    public class ProtocolDateResponse : APIResponseBase
    {
        public DateTime? ProtocolDate { get; set; }
    }
}
