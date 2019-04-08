using System;

namespace LAMP.ViewModel
{
    public class SpinWheelGameRequest
    {
        public long UserID { get; set; }
        public DateTime StartTime { get; set; }
        public string CollectedStars { get; set; }
        public int DayStreak { get; set; }
        public byte StrakSpin { get; set; }
        public DateTime GameDate { get; set; }
    }
}
