using System.Security.Policy;

namespace App.Web.ViewModel
{
    public class SessionUserVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public char IsAdmin { get; set; }
    }
}
