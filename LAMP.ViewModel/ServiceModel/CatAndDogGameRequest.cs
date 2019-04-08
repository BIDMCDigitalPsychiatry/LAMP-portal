using System;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CatAndDogGameRequest
    /// </summary>
    public class CatAndDogGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalQuestions { get; set; }
        public Int32 CorrectAnswers { get; set; }
        public Int32 WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }
}
