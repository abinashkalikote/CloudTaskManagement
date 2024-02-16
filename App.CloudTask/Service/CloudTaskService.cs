using System.Transactions;
using App.Base.Constants;
using App.Base.DataContext.Interface;
using App.Base.Providers.Interfaces;
using App.CloudTask.Dto;
using App.CloudTask.Entity;
using App.CloudTask.Service.Interfaces;

namespace App.CloudTask.Service;

public class CloudTaskService : ICloudTaskService
{
    private readonly ILoginUserProvider _loginUserProvider;
    private readonly IUow _uow;

    public CloudTaskService(
        ILoginUserProvider loginUserProvider,
        IUow uow
        )
    {
        _loginUserProvider = loginUserProvider;
        _uow = uow;
    }
    
    public async Task<Entity.CloudTask> Create(CloudTaskCreateDto dto)
    {
        var entity = MapCloudTaskCreateDtoToCloudTaskEntity(dto);
        
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _uow.CreateAsync(entity);
        await _uow.CommitAsync();
        
        scope.Complete();
        return entity;
    }

    public void Update()
    {
        throw new NotImplementedException();
    }


    #region Private Methods

    private Entity.CloudTask MapCloudTaskCreateDtoToCloudTaskEntity(CloudTaskCreateDto dto)
    {
        var cloudTask = new CloudTask.Entity.CloudTask
        {
            TaskName = dto.TaskName,
            TaskTypeId = dto.TaskTypeId,
            ClientId = dto.ClientId,
            Priority = dto.Priority,
            TaskTime = dto.TaskTime,
            IssueOnPreviousSoftware = dto.IssueOnPreviousSoftware,
            SoftwareVersionFrom = dto.SoftwareVersionFrom,
            SoftwareVersionTo = dto.SoftwareVersionTo,
            Remarks = dto.Remarks,
            LicDate = dto.LicDate,
            RecAuditLog = "Task Created by " + _loginUserProvider.GetUsername(),
            TSKStatus = dto.TSKStatus,
            RecById = dto.RecById
        };

        CloudTaskLog cloudTaskLog = new()
        {
            Remarks = dto.Remarks ?? "",
            CloudTaskStatus = CloudTaskStatus.Pending,
            UserId = Convert.ToInt32(_loginUserProvider.GetUserId())
        };
        cloudTask.CloudTaskLogs.Add(cloudTaskLog);
        return cloudTask;
    }

    #endregion
}