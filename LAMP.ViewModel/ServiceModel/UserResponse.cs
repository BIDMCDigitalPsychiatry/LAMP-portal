using System;
using System.Collections.Generic;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserResponse
    /// </summary>
    public class UserResponse : APIResponseBase
    {
        public long UserId { get; set; }
        public string StudyId { get; set; }
        public string Email { get; set; }
        public byte Type { get; set; }
        public string SessionToken { get; set; }
        public UserSettingData Data { get; set; }
        public UserResponse()
        {
            Data = new UserSettingData();
        }
    }
    /// <summary>
    /// Class UserSessionToken
    /// </summary>
    public class UserSessionToken : APIResponseBase
    {
        public long UserId { get; set; }
        public string SessionToken { get; set; }
    }
    public class UserLoginResponse : APIResponseBase
    {
        public long UserId { get; set; }
        public string StudyId { get; set; }
        public string Email { get; set; }
        public byte Type { get; set; }
        public string SessionToken { get; set; }
        public UserSettingData Data { get; set; }
        public ActivityPoints ActivityPoints { get; set; }
        public JewelsPoints JewelsPoints { get; set; }
       
        public UserLoginResponse()
        {
            Data = new UserSettingData();
            ActivityPoints = new ActivityPoints();
            JewelsPoints = new JewelsPoints();
        }
    }
   
    public class ActivityPoints
    {
        public decimal SurveyPoint { get; set; }
        public decimal _3DFigurePoint { get; set; }
        public decimal CatAndDogPoint { get; set; }
        public decimal CatAndDogNewPoint { get; set; }
        public decimal DigitSpanForwardPoint { get; set; }
        public decimal DigitSpanBackwardPoint { get; set; }
        public decimal NBackPoint { get; set; }
        public decimal Serial7Point { get; set; }
        public decimal SimpleMemoryPoint { get; set; }
        public decimal SpatialForwardPoint { get; set; }
        public decimal SpatialBackwardPoint { get; set; }
        public decimal TrailsBPoint { get; set; }
        public decimal VisualAssociationPoint { get; set; }
        public decimal TemporalOrderPoint { get; set; }
        public decimal NBackNewPoint { get; set; }
        public decimal TrailsBNewPoint { get; set; }
        public decimal TrailsBDotTouchPoint { get; set; }
        public decimal JewelsTrailsAPoint { get; set; }
        public decimal JewelsTrailsBPoint { get; set; }
    }
    public class JewelsPoints
    {
        public int JewelsTrailsATotalBonus { get; set; }
        public int JewelsTrailsBTotalBonus { get; set; }
        public int JewelsTrailsATotalJewels { get; set; }
        public int JewelsTrailsBTotalJewels { get; set; }
    }


}
