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
    
    public partial class CTest_3DFigure
    {
        public CTest_3DFigure()
        {
            this.CTest_3DFigureResult = new HashSet<CTest_3DFigureResult>();
        }
    
        public long C3DFigureID { get; set; }
        public string FigureName { get; set; }
        public string FileName { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual ICollection<CTest_3DFigureResult> CTest_3DFigureResult { get; set; }
    }
}