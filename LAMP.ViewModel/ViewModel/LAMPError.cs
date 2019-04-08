
namespace LAMP.ViewModel
{    
    /// <summary>
    /// Class LAMPError
    /// </summary>
    public class LAMPError
    {
        public string Key;
        public string Message;

        public LAMPError(string Key, string Message)
        {
            this.Key = Key;
            this.Message = Message;
        }
    }
}
