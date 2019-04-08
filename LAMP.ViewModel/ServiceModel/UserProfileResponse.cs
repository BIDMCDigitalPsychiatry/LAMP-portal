namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserProfileResponse
    /// </summary>
    public class UserProfileResponse : APIResponseBase
    {
        public UserProfileData Data { get; set; }
        public UserProfileResponse()
        {
            Data = new UserProfileData();
        }
    }
    /// <summary>
    /// Class UserProfileData
    /// </summary>
    public class UserProfileData
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudyId { get; set; }
        
    }
}
