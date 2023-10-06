using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace App.Web.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;

        public TelegramService(IConfiguration configuration, HttpClient httpClient)
        {
            _botToken = configuration["AppSettings:TelegramBotToken"];
            _chatId = configuration["AppSettings:ChatId"];
            _httpClient = httpClient;
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                var apiUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";

                // Data you want to send in the API request (in key-value pairs)
                var requestData = new Dictionary<string, string>
                {
                    { "chat_id", _chatId },
                    { "text", message }
                };

                // Create FormUrlEncodedContent from the data dictionary
                HttpContent content = new FormUrlEncodedContent(requestData);

                // Send a POST request to the API endpoint with the form data
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

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
    }
}
