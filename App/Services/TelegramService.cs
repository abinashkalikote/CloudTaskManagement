namespace App.Web.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly HttpClient _httpClient;

        public TelegramService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _botToken = configuration["AppSettings:TelegramBotToken"];
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task SendMessageAsync(string chatId, string message)
        {
            var apiUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("chat_id", chatId),
            new KeyValuePair<string, string>("text", message)
        });

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Message sent successfully
            }
            else
            {
                // Handle error response
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or handle the error response
            }
        }
    }

}
