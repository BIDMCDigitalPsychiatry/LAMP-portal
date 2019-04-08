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
    
    public partial class Admin
    {
        public Admin()
        {
            this.Admin_BatchSchedule = new HashSet<Admin_BatchSchedule>();
            this.Admin_CTestSchedule = new HashSet<Admin_CTestSchedule>();
            this.Admin_CTestSurveySettings = new HashSet<Admin_CTestSurveySettings>();
            this.Admin_JewelsTrailsASettings = new HashSet<Admin_JewelsTrailsASettings>();
            this.Admin_JewelsTrailsBSettings = new HashSet<Admin_JewelsTrailsBSettings>();
            this.Admin_Settings = new HashSet<Admin_Settings>();
            this.Admin_SurveySchedule = new HashSet<Admin_SurveySchedule>();
            this.AppHelps = new HashSet<AppHelp>();
            this.Blogs = new HashSet<Blog>();
            this.Tips = new HashSet<Tip>();
            this.Users = new HashSet<User>();
        }
    
        public long AdminID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<byte> AdminType { get; set; }
    
        public virtual ICollection<Admin_BatchSchedule> Admin_BatchSchedule { get; set; }
        public virtual ICollection<Admin_CTestSchedule> Admin_CTestSchedule { get; set; }
        public virtual ICollection<Admin_CTestSurveySettings> Admin_CTestSurveySettings { get; set; }
        public virtual ICollection<Admin_JewelsTrailsASettings> Admin_JewelsTrailsASettings { get; set; }
        public virtual ICollection<Admin_JewelsTrailsBSettings> Admin_JewelsTrailsBSettings { get; set; }
        public virtual ICollection<Admin_Settings> Admin_Settings { get; set; }
        public virtual ICollection<Admin_SurveySchedule> Admin_SurveySchedule { get; set; }
        public virtual ICollection<AppHelp> AppHelps { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Tip> Tips { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}