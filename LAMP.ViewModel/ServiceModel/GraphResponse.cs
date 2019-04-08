using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Graph Response
    /// </summary>
    public class GraphResponse : APIResponseBase
    {
        public List<GameScore> GameScoreList { get; set; }
        public GraphResponse()
        {
            GameScoreList = new List<GameScore>();
        }
    }

    public class GameScore
    {
        public string Game { get; set; }
        public int average { get; set; }
        public int totalAverage { get; set; }
    }

    public class GameGraphResponse : APIResponseBase
    {
        public Single HighScore { get; set; }
        public Single LowScore { get; set; }
        public Single[] DayTotalScore { get; set; }
    }

    public class GameDetailsForGraph
    {
        public long UserID { get; set; }
        public long GameID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Single Score { get; set; }
    }

    public class GameTotalScoreResponse : APIResponseBase
    {
        public Single TotalScore { get; set; }
        public Single CollectedStars { get; set; }
        public int? DayStreak { get; set; }
        public byte? StrakSpin { get; set; }
        public DateTime? GameDate { get; set; }
    }

    public class SpinWheelDayStreak_Date
    {
        public int? DayStreak { get; set; }
        public byte? StrakSpin { get; set; }
        public DateTime? GameDate { get; set; }
    }

}
