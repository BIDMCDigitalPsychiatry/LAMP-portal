using System;

namespace LAMP.ViewModel
{
    public class ScratchImageGameRequest
    {
        public long UserID { get; set; }
        public long ScratchImageID { get; set; }
        public string GameName { get; set; }
        public string DrawnImage { get; set; }
        public string DrawnImageName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public bool IsNotificationGame { get; set; }
        public long AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }
}
