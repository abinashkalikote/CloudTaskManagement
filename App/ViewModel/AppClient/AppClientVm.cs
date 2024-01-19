using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModel.AppClient
{
    public class AppClientVm
    {
        [Required(ErrorMessage = "Client Name is required.")]
        public string ClientName { get; set; }

        [DisplayName("Cloud URL")]
        [Required(ErrorMessage = "Cloud URL is Required.")]
        public string Link { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Address is required.")]
        public string? Location { get; set; }

        [DisplayName("Current Version")]
        [Required(ErrorMessage = "Current Version is required.")]
        public string? CurrentVersion { get; set; }

        [DisplayName("Is FinSmart Client ?")]
        public bool IsMobileBankingClient { get; set; }
    }
}
