using App.Base.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Base.Configuration;

public static class EntityRegistrar
{
    public static ModelBuilder AddBase(this ModelBuilder builder)
    {
        builder.Entity<AppClient>();
        builder.Entity<TaskType>();
        builder.Entity<User>();
        return builder;
    } 
}