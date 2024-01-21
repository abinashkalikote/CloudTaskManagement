using App.CloudTask.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.CloudTask.Configuration;

public static class EntityRegistrar
{
    public static ModelBuilder AddCloudTask(this ModelBuilder builder)
    {
        builder.Entity<Entity.CloudTask>();
        builder.Entity<CloudTaskLog>();
        return builder;
    }
}