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
    
    public partial class Admin_SurveySchedule
    {
        public Admin_SurveySchedule()
        {
            this.Admin_SurveyScheduleCustomTime = new HashSet<Admin_SurveyScheduleCustomTime>();
        }
    
        public long AdminSurveySchID { get; set; }
        public Nullable<long> AdminID { get; set; }
        public Nullable<long> SurveyID { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public Nullable<long> SlotID { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public Nullable<long> RepeatID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Admin Admin { get; set; }
        public virtual Repeat Repeat { get; set; }
        public virtual Slot Slot { get; set; }
        public virtual Survey Survey { get; set; }
        public virtual ICollection<Admin_SurveyScheduleCustomTime> Admin_SurveyScheduleCustomTime { get; set; }
    }
}
