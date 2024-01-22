namespace App.Base.Services.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(string? message, long? taskId = null);
    Task SendReplyMessageAsync(string? message, int? replyToMessageId = null);
}