using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Spatial Span Game Request
    /// </summary>
    public class SpatialSpanGameRequest
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
        public List<BoxList> BoxList { get; set; }
    }

    public class Box
    {
        public byte? GameIndex { get; set; }
        public string TimeTaken { get; set; }
        public bool? Status { get; set; }
        public int? Level { get; set; }
    }

    public class BoxList
    {
        public List<Box> Boxes { get; set; }
    }
}
