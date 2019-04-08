using System;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class DigitSpanGameRequest
    /// </summary>
    public class DigitSpanGameRequest
    {
        public long UserID { get; set; }
        public byte Type { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }
}
