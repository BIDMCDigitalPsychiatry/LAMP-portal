namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserProfileRequest
    /// </summary>
     public class UserProfileRequest
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudyId { get; set; }
       
    }
     /// <summary>
     /// Class GetUserProfileRequest
     /// </summary>
     public class GetUserProfileRequest
     {
         public long UserID { get; set; }
     }
}
