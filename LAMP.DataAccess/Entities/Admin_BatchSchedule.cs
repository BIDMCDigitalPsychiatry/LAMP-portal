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
    
    public partial class Admin_BatchSchedule
    {
        public Admin_BatchSchedule()
        {
            this.Admin_BatchScheduleCTest = new HashSet<Admin_BatchScheduleCTest>();
            this.Admin_BatchScheduleCustomTime = new HashSet<Admin_BatchScheduleCustomTime>();
            this.Admin_BatchScheduleSurvey = new HashSet<Admin_BatchScheduleSurvey>();
            this.CTest_3DFigureResult = new HashSet<CTest_3DFigureResult>();
            this.CTest_CatAndDogNewResult = new HashSet<CTest_CatAndDogNewResult>();
            this.CTest_CatAndDogResult = new HashSet<CTest_CatAndDogResult>();
            this.CTest_DigitSpanResult = new HashSet<CTest_DigitSpanResult>();
            this.CTest_JewelsTrailsAResult = new HashSet<CTest_JewelsTrailsAResult>();
            this.CTest_JewelsTrailsBResult = new HashSet<CTest_JewelsTrailsBResult>();
            this.CTest_NBackNewResult = new HashSet<CTest_NBackNewResult>();
            this.CTest_NBackResult = new HashSet<CTest_NBackResult>();
            this.CTest_ScratchImageResult = new HashSet<CTest_ScratchImageResult>();
            this.CTest_Serial7Result = new HashSet<CTest_Serial7Result>();
            this.CTest_SimpleMemoryResult = new HashSet<CTest_SimpleMemoryResult>();
            this.CTest_SpatialResult = new HashSet<CTest_SpatialResult>();
            this.SurveyResults = new HashSet<SurveyResult>();
            this.CTest_TemporalOrderResult = new HashSet<CTest_TemporalOrderResult>();
            this.CTest_TrailsBDotTouchResult = new HashSet<CTest_TrailsBDotTouchResult>();
            this.CTest_TrailsBNewResult = new HashSet<CTest_TrailsBNewResult>();
            this.CTest_TrailsBResult = new HashSet<CTest_TrailsBResult>();
            this.CTest_VisualAssociationResult = new HashSet<CTest_VisualAssociationResult>();
        }
    
        public long AdminBatchSchID { get; set; }
        public Nullable<long> AdminID { get; set; }
        public string BatchName { get; set; }
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
        public virtual ICollection<Admin_BatchScheduleCTest> Admin_BatchScheduleCTest { get; set; }
        public virtual ICollection<Admin_BatchScheduleCustomTime> Admin_BatchScheduleCustomTime { get; set; }
        public virtual ICollection<Admin_BatchScheduleSurvey> Admin_BatchScheduleSurvey { get; set; }
        public virtual ICollection<CTest_3DFigureResult> CTest_3DFigureResult { get; set; }
        public virtual ICollection<CTest_CatAndDogNewResult> CTest_CatAndDogNewResult { get; set; }
        public virtual ICollection<CTest_CatAndDogResult> CTest_CatAndDogResult { get; set; }
        public virtual ICollection<CTest_DigitSpanResult> CTest_DigitSpanResult { get; set; }
        public virtual ICollection<CTest_JewelsTrailsAResult> CTest_JewelsTrailsAResult { get; set; }
        public virtual ICollection<CTest_JewelsTrailsBResult> CTest_JewelsTrailsBResult { get; set; }
        public virtual ICollection<CTest_NBackNewResult> CTest_NBackNewResult { get; set; }
        public virtual ICollection<CTest_NBackResult> CTest_NBackResult { get; set; }
        public virtual ICollection<CTest_ScratchImageResult> CTest_ScratchImageResult { get; set; }
        public virtual ICollection<CTest_Serial7Result> CTest_Serial7Result { get; set; }
        public virtual ICollection<CTest_SimpleMemoryResult> CTest_SimpleMemoryResult { get; set; }
        public virtual ICollection<CTest_SpatialResult> CTest_SpatialResult { get; set; }
        public virtual ICollection<SurveyResult> SurveyResults { get; set; }
        public virtual ICollection<CTest_TemporalOrderResult> CTest_TemporalOrderResult { get; set; }
        public virtual ICollection<CTest_TrailsBDotTouchResult> CTest_TrailsBDotTouchResult { get; set; }
        public virtual ICollection<CTest_TrailsBNewResult> CTest_TrailsBNewResult { get; set; }
        public virtual ICollection<CTest_TrailsBResult> CTest_TrailsBResult { get; set; }
        public virtual ICollection<CTest_VisualAssociationResult> CTest_VisualAssociationResult { get; set; }
    }
}
