using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Temporal Order Game Request
    /// </summary>
    public class TemporalOrderGameRequest
    {
        public long UserID { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
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
}
