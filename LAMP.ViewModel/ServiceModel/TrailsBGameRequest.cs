using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class TrailsBGameRequest
    /// </summary>
    public class TrailsBGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }

        public List<RoutesList> RoutesList { get; set; }
    }
    public class TrailsBNewGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public int? Version { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
        public List<RoutesList> RoutesList { get; set; }
    }
    public class TrailsBDotTouchGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }

        public List<RoutesList> RoutesList { get; set; }
    }
    public class JewelsTrailsBGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public decimal? Score { get; set; }
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
        public List<RoutesList> RoutesList { get; set; }
    }
    public class JewelsTrailsAGameRequest
    {
        public long UserID { get; set; }
        public Int32 TotalAttempts { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public decimal? Score { get; set; }
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public string SpinWheelScore { get; set; }
        public long? AdminBatchSchID { get; set; }

        public List<RoutesList> RoutesList { get; set; }
    }

    public class Route
    {
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public Nullable<bool> Status { get; set; }       
    }

    public class RoutesList
    {
        public List<Route> Routes { get; set; }
    }
}
