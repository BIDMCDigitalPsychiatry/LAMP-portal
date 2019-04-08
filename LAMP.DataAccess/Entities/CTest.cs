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
    
    public partial class CTest
    {
        public CTest()
        {
            this.Admin_CTestSchedule = new HashSet<Admin_CTestSchedule>();
            this.Admin_CTestSurveySettings = new HashSet<Admin_CTestSurveySettings>();
        }
    
        public long CTestID { get; set; }
        public string CTestName { get; set; }
        public Nullable<bool> IsDistractionSurveyRequired { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> MaxVersions { get; set; }
    
        public virtual ICollection<Admin_CTestSchedule> Admin_CTestSchedule { get; set; }
        public virtual ICollection<Admin_CTestSurveySettings> Admin_CTestSurveySettings { get; set; }
    }
}