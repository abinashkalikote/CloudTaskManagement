using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModel
{
    public class UserVM
    {
        public int? Id { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        public string? Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password not matched !")]
        public string? ConfirmPassword { get; set; }

        [DisplayName("Is Admin?")]
        public string IsAdmin { get; set; }

        public string? Status { get; set; }
    }
}
