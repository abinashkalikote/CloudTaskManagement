using App.CloudTask.Repository;
using App.CloudTask.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.CloudTask.Configuration;

public static class DiConfig
{
    public static IServiceCollection UseCloudTask(this IServiceCollection services)
    {
        //Services
        
        //Repositories
        services.AddScoped<ICloudTaskRepo, CloudTaskRepo>();
        
        //Providers
        
        //Managers

        return services;
    }
}