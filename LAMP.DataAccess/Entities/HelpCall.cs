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
    
    public partial class HelpCall
    {
        public long HelpCallID { get; set; }
        public long UserID { get; set; }
        public string CalledNumber { get; set; }
        public Nullable<System.DateTime> CallDateTime { get; set; }
        public Nullable<long> CallDuraion { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> Type { get; set; }
    
        public virtual User User { get; set; }
    }
}