using System.ComponentModel.DataAnnotations;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Protocol View Model
    /// </summary>
    public class ProtocolViewModel : ViewModelBase
    {
         [Required(ErrorMessage = "Select a Protocol Date.")]
        public string DatePart { get; set; }
         [Required(ErrorMessage = "Select a Protocol Time.")]
         public string TimePart { get; set; }
         public long UserId { get; set; }
    }
}
