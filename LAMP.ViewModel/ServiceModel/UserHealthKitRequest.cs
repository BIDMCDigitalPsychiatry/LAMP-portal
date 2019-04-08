using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// User Health Kit Request
    /// </summary>
    public class UserHealthKitRequest
    {
        public long UserID { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string BloodType { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public string RespiratoryRate { get; set; }
        public string Sleep { get; set; }
        public string Steps { get; set; }
        public string FlightClimbed { get; set; }
        public string Segment { get; set; }
        public string Distance { get; set; }
    }

    public class HealthKitUserRequest
    {
        public Int64 UserID { get; set; }
        public string DateOfBirth { get; set; }
        /// <summary>
        /// M,F
        /// </summary>
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public List<HealthKitParam> HealthKitParams { get; set; }
    }

    public class HealthKitParam
    {
        public Int64 ParamID { get; set; }
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
    }

}
