using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModel
{
    public class ChangePasswordVM
    {
        [Required]
        [MinLength(4)]
        [DisplayName("Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }


        [Required]
        [DisplayName("Confirm Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Password & Confirm Password Not Matched !")]
        public string ConfirmPassword { get; set; }
    }
}
