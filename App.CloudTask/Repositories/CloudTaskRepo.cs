using App.Base.GenericRepository;
using App.CloudTask.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.CloudTask.Repositories;

public class CloudTaskRepo : GenericRepository<Entity.CloudTask>, ICloudTaskRepo
{
    public CloudTaskRepo(DbContext context) : base(context)
    {
    }
}