using App.Base.Entities;
using App.Base.GenericRepository;
using App.Web.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.Web.Repository;

public class AppClientRepo : GenericRepository<AppClient>, IAppClientRepo
{
    public AppClientRepo(DbContext context) : base(context)
    {
    }
}