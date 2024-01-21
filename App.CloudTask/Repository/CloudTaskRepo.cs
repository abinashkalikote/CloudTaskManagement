using App.Base.GenericRepository;
using App.CloudTask.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.CloudTask.Repository;

public class CloudTaskRepo : GenericRepository<Entity.CloudTask>, ICloudTaskRepo
{
    public CloudTaskRepo(DbContext context) : base(context)
    {
    }
}