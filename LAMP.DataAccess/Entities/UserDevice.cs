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
    
    public partial class UserDevice
    {
        public long UserDeviceID { get; set; }
        public long UserID { get; set; }
        public byte DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceToken { get; set; }
        public Nullable<System.DateTime> LastLoginOn { get; set; }
    
        public virtual User User { get; set; }
    }
}
