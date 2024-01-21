using App.Base.DataContext;
using App.Base.DataContext.Interface;
using App.Base.GenericRepository;
using App.Base.GenericRepository.Interface;
using App.Base.Repository;
using App.Base.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.Base.Configuration
{
    public static class DiConfig
    {
        public static IServiceCollection UseBase( this IServiceCollection service) => 
            service.AddScoped<IUow, Uow>()
                   .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                   .AddScoped<IUserRepository, UserRepository>();
    }
}
