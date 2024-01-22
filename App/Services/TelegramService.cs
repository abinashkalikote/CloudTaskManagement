using System.Text.Json;
using App.Base.DataContext.Interface;
using App.Base.Services.Interfaces;
using App.CloudTask.Repositories.Interfaces;
using App.Web.Data;
using App.Web.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace App.Web.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly string? _botToken;
        private readonly string? _chatId;
        private readonly HttpClient _httpClient;
        private readonly ICloudTaskRepo _cloudTaskRepo;
        private readonly IUow _uow;

        public TelegramService(
            IConfiguration configuration,
            HttpClient httpClient,
            ICloudTaskRepo cloudTaskRepo,
            IUow uow)
        {
            _botToken = configuration["AppSettings:TelegramBotToken"];
            _chatId = configuration["AppSettings:ChatId"];
            _httpClient = httpClient;
            _cloudTaskRepo = cloudTaskRepo;
            _uow = uow;
        }

        public async Task SendMessageAsync(string? message, long? taskId = null)
        {
            try
            {
                HttpResponseMessage response = await SendToTelegram(message);

                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync().Result;
                    var responseData = JsonSerializer.Deserialize<TelegramResponseVM>(contents);
                    if (taskId != null)
                    {
                        var task = await _cloudTaskRepo.FindAsync(taskId ?? 0);
                        task.TelegramMessageId = responseData?.result.message_id;
                        
                        await _uow.CommitAsync();
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network errors)
                Console.WriteLine("HTTP Request Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("Exception occurred: " + ex.Message);
            }
        }


        public async Task SendReplyMessageAsync(string? message, int? replyToMessageId = null)
        {
            try
            {
                var response = await SendToTelegram(message, replyToMessageId);
                if (response.IsSuccessStatusCode)
                {

                }

            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network errors)
                Console.WriteLine("HTTP Request Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("Exception occurred: " + ex.Message);
            }
        }
        
        private async Task<HttpResponseMessage> SendToTelegram(string? message, int? replyToId = null)
        {
            var apiUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";

            // Data you want to send in the API request (in key-value pairs)
            var requestData = new Dictionary<string, string?>
            {
                    { "chat_id", _chatId },
                    { "text", message },
                    {"parse_mode", "HTML" }
                };
            if (replyToId.HasValue)
            {
                requestData.Add("reply_to_message_id", replyToId.Value.ToString());
            }

            // Create FormUrlEncodedContent from the data dictionary
            HttpContent content = new FormUrlEncodedContent(requestData);

            // Send a POST request to the API endpoint with the form data
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            return response;
        }
    }
}
