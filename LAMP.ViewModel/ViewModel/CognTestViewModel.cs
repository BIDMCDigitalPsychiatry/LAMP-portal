using System;
using System.Collections.Generic;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class CognTestViewModel
    /// </summary>
    class CognTestViewModel
    {
    }
    /// <summary>
    /// Class CognTestRequest
    /// </summary>
    public class CognTestRequest
    {
       public long UserID { get; set; }
       public byte GameType { get; set; }
       public string TestName { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime EndTime { get; set; }
       public Int32 AnswerCount { get; set; }
       public string Rating { get; set; }
    }
    /// <summary>
    /// Class CognTestUpdateRequest
    /// </summary>
    public class CognTestUpdateRequest
    {
        public long CognTestResultID { get; set; }
        public long UserID { get; set; }
        public byte GameType { get; set; }
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Int32 AnswerCount { get; set; }
        public string Rating { get; set; }
    }
    /// <summary>
    /// Class CognTestGetRequest
    /// </summary>
    public class CognTestGetRequest
    {
        public long UserID { get; set; }
    }
    /// <summary>
    /// Class CognTestGetResponse
    /// </summary>
    public class CognTestGetResponse : APIResponseBase
    {
        public List<CognTestData> Data { get; set; }
        public CognTestGetResponse()
        {
            Data = new List<CognTestData>();
        }
    }
    /// <summary>
    /// Class CognTestData
    /// </summary>
    public class CognTestData
    {
       public long CognTestResultID { get; set; }
       public long UserID { get; set; }
       public byte GameType { get; set; }
       public string TestName { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime EndTime { get; set; }
       public Int32 AnswerCount { get; set; }
       public string Rating { get; set; }
       public DateTime CreatedOn { get; set; }
    }
}
