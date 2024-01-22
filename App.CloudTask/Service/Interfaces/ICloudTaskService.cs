using App.CloudTask.Dto;

namespace App.CloudTask.Service.Interfaces;

public interface ICloudTaskService
{
    Task<Entity.CloudTask> Create(CloudTaskCreateDto dto);
    void Update();
}