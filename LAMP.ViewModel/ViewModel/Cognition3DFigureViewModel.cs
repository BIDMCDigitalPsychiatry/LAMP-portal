using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class Cognition3DFigureViewModel
    /// </summary>
    public class Cognition3DFigureViewModel: ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public List<Cognition3DFigureDetail> CTest_3DFigureResultList { get; set; }
        public Cognition3DFigureViewModel()
        {
            CTest_3DFigureResultList = new List<Cognition3DFigureDetail>();
        }
      
    }
    /// <summary>
    /// Class Cognition3DFigureDetails
    /// </summary>
    public class Cognition3DFigureDetails
    {
        public long FigureResultID { get; set; }
        public String GameName { get; set; }
        public long FigureID { get; set; }
        public String DrawnFigFileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal Points { get; set; }
    }
    /// <summary>
    /// Class Cognition3DFigureDetail
    /// </summary>
    public class Cognition3DFigureDetail
    {
        public long GameNumber { get; set; }
        public String GameName { get; set; }
        public Int16 Points { get; set; }
        public List<Cognition3DFigureGroupDetail> CTest_Cognition3DFigureGroupDetailList { get; set; }
        public Cognition3DFigureDetail()
        {
            CTest_Cognition3DFigureGroupDetailList = new List<Cognition3DFigureGroupDetail>();
        }
    }
    /// <summary>
    /// Class Cognition3DFigureGroupDetail
    /// </summary>
    public class Cognition3DFigureGroupDetail
    {
        public long FigureResultID { get; set; }
        public String FileName { get; set; }
        public String DrawnFigFileName  { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public DateTime CreatedOn { get; set; }        
    }
    
}

