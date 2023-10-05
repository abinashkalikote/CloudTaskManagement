using App.Model;

namespace App.Web.ViewModel
{
    public class CloudTaskLogVM
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string CloudTaskStatus { get; set; }
        public DateTime RecDate { get; set; }
    }
}
