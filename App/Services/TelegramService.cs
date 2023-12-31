﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types.Enums;
using App.Data;
using System.Text.Json;
using App.Web.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace App.Web.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _db;

        public TelegramService(
            IConfiguration configuration,
            HttpClient httpClient,
            AppDbContext db)
        {
            _botToken = configuration["AppSettings:TelegramBotToken"];
            _chatId = configuration["AppSettings:ChatId"];
            _httpClient = httpClient;
            _db = db;
        }

        public async Task SendMessageAsync(string message, int? TaskId = null)
        {
            try
            {
                HttpResponseMessage response = await SendToTelegram(message);

                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync().Result;
                    TelegramResponseVM data = JsonSerializer.Deserialize<TelegramResponseVM>(contents);
                    if (TaskId != null)
                    {
                        var task = await _db.CloudTasks.FirstOrDefaultAsync(e => e.Id == TaskId);
                        task.TelegramMessageId = data.result.message_id;

                        _db.CloudTasks.Update(task);
                        await _db.SaveChangesAsync();
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


        public async Task SendReplyMessageAsync(string message, int? ReplyToMessageId = null)
        {
            try
            {
                var response = await SendToTelegram(message, ReplyToMessageId);
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
        
        private async Task<HttpResponseMessage> SendToTelegram(string message, int? replyToId = null)
        {
            var apiUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";

            // Data you want to send in the API request (in key-value pairs)
            var requestData = new Dictionary<string, string>
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
