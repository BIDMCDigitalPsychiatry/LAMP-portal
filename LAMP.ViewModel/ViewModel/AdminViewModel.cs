using System;
using System.ComponentModel.DataAnnotations;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Admin View Model
    /// </summary>
    public class AdminViewModel : ViewModelBase
    {
        public long AdminID { get; set; }

        [Required(ErrorMessage = "Specify Email.")]
        [EmailAddress(ErrorMessage = "Specify a valid Email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Specify Password.")]
        [RegularExpression(@"^[a-zA-Z0-9~!@#$%^&*()?\/_]+[a-zA-Z0-9~!@#$%^&*()?\/_\s]*$", ErrorMessage = "Incorrect password.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Specify First Name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Specify Last Name.")]
        public string LastName { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<byte> AdminTypeId { get; set; }
        public string AdminType { get; set; }
        public bool IsSaved { get; set; }

    }
}
