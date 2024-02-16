using System.Transactions;
using App.Base.Providers.Interfaces;
using App.Base.Services.Interfaces;
using App.CloudTask.Dto;
using App.CloudTask.Managers.Interfaces;
using App.CloudTask.Service.Interfaces;
using NepDate;

namespace App.CloudTask.Managers;

public class CloudTaskManager : ICloudTaskManager
{
    private readonly ICloudTaskService _cloudTaskService;
    private readonly ITelegramService _telegramService;
    private readonly ILoginUserProvider _loginUserProvider;
    private readonly IHostProvider _hostProvider;

    public CloudTaskManager(
        ICloudTaskService cloudTaskService,
        ITelegramService telegramService,
        ILoginUserProvider loginUserProvider,
        IHostProvider hostProvider
        )
    {
        _cloudTaskService = cloudTaskService;
        _telegramService = telegramService;
        _loginUserProvider = loginUserProvider;
        _hostProvider = hostProvider;
    }
    public async Task<bool> Create(CloudTaskCreateDto dto)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var entity = await _cloudTaskService.Create(dto);
        await SendNewTaskMessageToTelegram(entity);
        scope.Complete();
        
        return true;
    }



    #region Private Methods

    private async Task SendNewTaskMessageToTelegram(CloudTask.Entity.CloudTask cloudTask)
        {
            var user = _loginUserProvider.GetUsername();


            string taskCreatedBy = user != null ? $"## Task Created by <b>{user}</b> ##" : "";

            string pri = "";
            pri = cloudTask.Priority == 'Y' ? $"<b>Priority :</b> Urgent\r\n" : "";

            var baseUrl = _hostProvider.GetHost() + "/Task/TaskDetails?TaskID=" + cloudTask.Id;

            string? message = cloudTask.TaskTypeId switch
            {
                1 => $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                     $"<b>Software Version :</b> <b>From</b> {cloudTask.SoftwareVersionFrom} <b>To</b> {cloudTask.SoftwareVersionTo}\r\n" +
                     pri + $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>Client :</b> {cloudTask.ClientId}\r\n" + $"<b>Cloud URL :</b> {cloudTask.Client?.Link}\r\n" +
                     $"<b>Issue :</b> {cloudTask.IssueOnPreviousSoftware} \r\n\r\n\r\n" +
                     $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" + taskCreatedBy,
                2 => $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>To do :</b> {cloudTask.TaskName}\r\n" + pri +
                     $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>Client :</b> {cloudTask.ClientId}\r\n" + $"<b>PAN No :</b> PANNo will be here\r\n" +
                     $"<b>License Date :</b> {cloudTask.LicDate}\r\n\r\n" +
                     $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" + taskCreatedBy,
                3 => $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>To do :</b> {cloudTask.TaskName}\r\n" + pri +
                     $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                     $"------------------------------------------------------\r\n" +
                     $"<b>Client :</b> {cloudTask.ClientId}\r\n" + $"<b>Cloud URL :</b> {cloudTask.Client?.Link}\r\n\r\n" +
                     $"<b>Task Link</b> : <a href='\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" + taskCreatedBy,
                _ => ""
            };

            await _telegramService.SendMessageAsync(message, cloudTask.Id);
        }

    #endregion
}