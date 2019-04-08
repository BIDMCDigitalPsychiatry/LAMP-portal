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
    
    public partial class SurveyQuestion
    {
        public SurveyQuestion()
        {
            this.SurveyQuestionOptions = new HashSet<SurveyQuestionOption>();
        }
    
        public long QuestionID { get; set; }
        public long SurveyID { get; set; }
        public string QuestionText { get; set; }
        public Nullable<byte> AnswerType { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Survey Survey { get; set; }
        public virtual ICollection<SurveyQuestionOption> SurveyQuestionOptions { get; set; }
    }
}
