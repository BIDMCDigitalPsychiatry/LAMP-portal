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
    
    public partial class HealthKit_ParamValues
    {
        public long HKParamValueID { get; set; }
        public long UserID { get; set; }
        public long HKParamID { get; set; }
        public string Value { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
    
        public virtual HealthKit_Parameters HealthKit_Parameters { get; set; }
        public virtual User User { get; set; }
    }
}
