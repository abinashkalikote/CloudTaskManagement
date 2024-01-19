using App.Base.GenericRepository;
using App.Data;
using App.Model;
using App.Web.Repository.Interfaces;

namespace App.Web.Repository;

public class AppClientRepo : GenericRepository<AppClient>, IAppClientRepo
{
    public AppClientRepo(AppDbContext context) : base(context)
    {
    }
}