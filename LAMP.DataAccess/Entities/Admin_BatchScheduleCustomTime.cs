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
    
    public partial class Admin_BatchScheduleCustomTime
    {
        public long AdminBatchSchCustTimID { get; set; }
        public Nullable<long> AdminBatchSchID { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
    
        public virtual Admin_BatchSchedule Admin_BatchSchedule { get; set; }
    }
}
