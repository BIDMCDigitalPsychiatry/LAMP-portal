using System;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class NBackGameRequest
    /// </summary>
    public class NBackGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalQuestions { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public int? Version { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }
    public class NBackNewGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalQuestions { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }

    public class GeneralGameScores
    {
        public long UserID { get; set; }
        public decimal Score { get; set; }
    }
    public class UserAverage
    {
        public long UserId { get; set; }
        public double Avg { get; set; }
        public double totalscore { get; set; }
    }
}
