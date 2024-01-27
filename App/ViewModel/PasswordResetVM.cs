using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModel
{
    public class PasswordResetVM
    {
        [Required]
        public long UserId { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Password must be minimum 4 characters !")]
        public string Password { get; set; } = string.Empty;
    }
}
