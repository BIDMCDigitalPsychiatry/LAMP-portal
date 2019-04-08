using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Get User Report Request
    /// </summary>
    public class GetUserReportRequest
    {
        public long UserId { get; set; }
    }
    public class JewelsTrialsAList
    {
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public decimal? Score { get; set; }
        public decimal ScoreAvg { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class JewelsTrialsBList
    {
        public int? TotalJewelsCollected { get; set; }
        public int? TotalBonusCollected { get; set; }
        public decimal? Score { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal ScoreAvg { get; set; }
    }
    public class UserReportResponse : APIResponseBase
    {
        public List<JewelsTrialsAList> JewelsTrialsAList { get; set; }
        public List<JewelsTrialsBList> JewelsTrialsBList { get; set; }
        public UserReportResponse()
        {
            JewelsTrialsAList = new List<JewelsTrialsAList>();
            JewelsTrialsBList = new List<JewelsTrialsBList>();
        }
    }
}
