using System;
using System.ComponentModel.DataAnnotations;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class UserViewModel 
    /// </summary>
    public class UserViewModel : ViewModelBase
    {
        public long UserID { get; set; }

        [Required(ErrorMessage = "Specify email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9~!@#$%^&*()?\/_]+[a-zA-Z0-9~!@#$%^&*()?\/_\s]*$", ErrorMessage = "Invalid password.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Specify first name.")]
        [RegularExpression(@"[^<>]*", ErrorMessage = "Invalid first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Specify last name.")]
        [RegularExpression(@"[^<>]*", ErrorMessage = "Invalid last name.")]
        public string LastName { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid zip.")]
        public string ZipCode { get; set; }
        [RegularExpression(@"[^<>]*", ErrorMessage = "Invalid city.")]
        public string City { get; set; }
        [RegularExpression(@"[^<>]*", ErrorMessage = "Invalid state.")]
        public string State { get; set; }
        public string Gender { get; set; }
        [Range(0, 120, ErrorMessage = "Invalid age.")]
        public byte? Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string ClinicalProfileURL { get; set; }
        public string ClinicalProfileExtension { get; set; }
        public bool IsGuestUser { get; set; }
        public string PhysicianFirstName { get; set; }
        public string PhysicianLastName { get; set; }
        public string StudyCode { get; set; }

        [Required(ErrorMessage = "Specify study id.")]
        public string StudyId { get; set; }
        public string FormattedStudyId { get; set; }
        [RegularExpression("^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)[0-9][0-9]$", ErrorMessage = "Invalid BirthDate.")]
        public string BirthDateString { get; set; }
        public bool IsSaved { get; set; }
        public bool IsError { get; set; }
        public bool IsDisabled { get; set; }
        public long AdminID { get; set; }
    }
}
