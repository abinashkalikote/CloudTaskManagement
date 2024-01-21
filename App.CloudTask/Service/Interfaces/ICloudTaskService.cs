namespace App.CloudTask.Service.Interfaces;

public interface ICloudTaskService
{
    Task<Entity.CloudTask> Create();
    void Update();
}