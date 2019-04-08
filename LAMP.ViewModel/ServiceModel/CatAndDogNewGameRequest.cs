using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    public class CatAndDogNewGameRequest
    {
        public long UserID { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Point { get; set; }
        public int? Score { get; set; }
        public byte StatusType { get; set; }
        public bool? IsNotificationGame { get; set; }
        public long? AdminBatchSchID { get; set; }
        public string SpinWheelScore { get; set; }
        public List<CatAndDogNewGameLevelDetailRequest> GameLevelDetailList { get; set; }
    }

    public class CatAndDogNewGameLevelDetailRequest
    {
        public int CorrectAnswer { get; set; }
        public int WrongAnswer { get; set; }
        public string TimeTaken { get; set; }
    }
}
