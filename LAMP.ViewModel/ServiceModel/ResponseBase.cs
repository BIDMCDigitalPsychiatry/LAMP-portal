using System.Collections.Generic;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class ResponseBase
    /// </summary>
    public class ResponseBase
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }
        public string SuccessMessage { get; set; }
        public List<LAMPError> Errors = new List<LAMPError>();
    }
    /// <summary>
    /// Class APIResponseBase
    /// </summary>
    public class APIResponseBase
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
