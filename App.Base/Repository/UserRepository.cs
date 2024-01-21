using App.Base.Entities;
using App.Base.GenericRepository;
using App.Base.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.Base.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}