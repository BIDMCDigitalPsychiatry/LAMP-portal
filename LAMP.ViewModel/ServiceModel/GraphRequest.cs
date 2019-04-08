
namespace LAMP.ViewModel
{
    /// <summary>
    /// Graph Request
    /// </summary>
    public class GraphRequest
    {
        public long UserID { get; set; }
    }

    public class GameGraphRequest
    {
        public long UserID { get; set; }
        public long GameID { get; set; }
    }

    public class GameTotalScoreRequest
    {
        public long UserID { get; set; }
        public string Date { get; set; }
    }

}
