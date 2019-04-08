//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LAMP.DataAccess.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class CTest_JewelsTrailsAResult
    {
        public CTest_JewelsTrailsAResult()
        {
            this.CTest_JewelsTrailsAResultDtl = new HashSet<CTest_JewelsTrailsAResultDtl>();
        }
    
        public long JewelsTrailsAResultID { get; set; }
        public long UserID { get; set; }
        public Nullable<int> TotalAttempts { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<byte> Rating { get; set; }
        public Nullable<decimal> Point { get; set; }
        public string TotalJewelsCollected { get; set; }
        public string TotalBonusCollected { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string Score { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<bool> IsNotificationGame { get; set; }
        public Nullable<long> AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
    
        public virtual Admin_BatchSchedule Admin_BatchSchedule { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<CTest_JewelsTrailsAResultDtl> CTest_JewelsTrailsAResultDtl { get; set; }
    }
}
