using App.CloudTask.Dto;

namespace App.CloudTask.Managers.Interfaces;

public interface ICloudTaskManager
{
    Task<bool> Create(CloudTaskCreateDto dto);
}