using App.CloudTask.Managers;
using App.CloudTask.Managers.Interfaces;
using App.CloudTask.Repositories;
using App.CloudTask.Repositories.Interfaces;
using App.CloudTask.Service;
using App.CloudTask.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.CloudTask.Configuration;

public static class DiConfig
{
    public static IServiceCollection UseCloudTask(this IServiceCollection services)
    {
        //Services
        services.AddScoped<ICloudTaskService, CloudTaskService>();
        
        //Repositories
        services.AddScoped<ICloudTaskRepo, CloudTaskRepo>();
        
        //Providers
        
        //Managers
        services.AddScoped<ICloudTaskManager, CloudTaskManager>();

        return services;
    }
}