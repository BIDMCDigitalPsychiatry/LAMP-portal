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
    
    public partial class CTest_TrailsBDotTouchResultDtl
    {
        public long TrailsBDotTouchResultDtlID { get; set; }
        public long TrailsBDotTouchResultID { get; set; }
        public string Alphabet { get; set; }
        public string TimeTaken { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> Sequence { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual CTest_TrailsBDotTouchResult CTest_TrailsBDotTouchResult { get; set; }
    }
}
