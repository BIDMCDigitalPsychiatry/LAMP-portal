using System;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class VisualAssociationGameRequest
    /// </summary>
    public class VisualAssociationGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalQuestions { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public int? Version { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }
    }
}

