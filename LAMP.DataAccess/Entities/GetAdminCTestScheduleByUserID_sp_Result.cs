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
    
    public partial class GetAdminCTestScheduleByUserID_sp_Result
    {
        public Nullable<long> GameScheduleID { get; set; }
        public Nullable<long> CTestId { get; set; }
        public string CTestName { get; set; }
        public Nullable<int> Version { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public Nullable<long> RepeatID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}