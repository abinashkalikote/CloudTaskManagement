namespace App.Web.ViewModel
{
    public class TelegramResponseVM
    {
        public bool ok { get; set; }
        public Result result { get; set; }
    }
    // Define your view model class to match the JSON structure
    public class Result
    {
        public int message_id { get; set; }
        public SenderChat sender_chat { get; set; }
        public Chat chat { get; set; }
        public long date { get; set; }
        public string text { get; set; }
    }

    public class SenderChat
    {
        public long id { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }

    public class Chat
    {
        public long id { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }
}
