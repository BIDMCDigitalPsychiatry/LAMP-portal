using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    public class CognitionScratchImageViewModel : ViewModelBase
    {
        public long UserID { get; set; }
        public DateTime LastCognitionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public List<CognitionScratchImageDetail> CTest_ScratchImageResultList { get; set; }
        public CognitionScratchImageViewModel()
        {
            CTest_ScratchImageResultList = new List<CognitionScratchImageDetail>();
        }
    }

    /// <summary>
    /// Class Cognition3DFigureDetails
    /// </summary>
    public class CognitionScratchImageDetails
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
    public class CognitionScratchImageDetail
    {
        public long GameNumber { get; set; }
        public String GameName { get; set; }
        public Int16 Points { get; set; }
        public List<CognitionScratchImageGroupDetail> CTest_CognitionScratchImageGroupDetailList { get; set; }
        public CognitionScratchImageDetail()
        {
            CTest_CognitionScratchImageGroupDetailList = new List<CognitionScratchImageGroupDetail>();
        }
    }
    /// <summary>
    /// Class Cognition3DFigureGroupDetail
    /// </summary>
    public class CognitionScratchImageGroupDetail
    {
        public long FigureResultID { get; set; }
        public String FileName { get; set; }
        public String DrawnFigFileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String DurationString { get; set; }
        public DateTime CreatedOn { get; set; }

    }

}
