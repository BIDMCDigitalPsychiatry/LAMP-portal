using System;
namespace LAMP.ViewModel
{
    /// <summary>
    ///  Class _3DFigureGameRequest
    /// </summary>
    public class _3DFigureGameRequest
    {
        public long UserID { get; set; }
        public long C3DFigureID { get; set; }
        public string GameName { get; set; }
        public string DrawnFig { get; set; }
        public string DrawnFigFileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    }
}
