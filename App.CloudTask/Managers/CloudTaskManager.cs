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


            string TaskCreatedBy = "";
            if (user != null)
                TaskCreatedBy = $"## Task Created by <b>{user}</b> ##";
            else
                TaskCreatedBy = "";


            string pri = "";
            if (cloudTask.Priority == 'Y')
                pri = $"<b>Priority :</b> Urgent\r\n";
            else
                pri = "";

            var baseUrl = _hostProvider.GetHost() + "/Task/TaskDetails?TaskID=" + cloudTask.Id;


            string? message = "";
            if (cloudTask.TaskTypeId == 1)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    $"<b>Software Version :</b> <b>From</b> {cloudTask.SoftwareVersionFrom} <b>To</b> {cloudTask.SoftwareVersionTo}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>Cloud URL :</b> {cloudTask.CloudUrl}\r\n" +
                    $"<b>Issue :</b> {cloudTask.IssueOnPreviousSoftware} \r\n\r\n\r\n" +
                    $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }
            else if (cloudTask.TaskTypeId == 2)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>PAN No :</b> PANNo will be here\r\n" +
                    $"<b>License Date :</b> {cloudTask.LicDate}\r\n\r\n" +
                    $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }
            else if (cloudTask.TaskTypeId == 3)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>Cloud URL :</b> {cloudTask.CloudUrl}\r\n\r\n" +
                    $"<b>Task Link</b> : <a href='\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }


            await _telegramService.SendMessageAsync(message, cloudTask.Id);
        }

    #endregion
}