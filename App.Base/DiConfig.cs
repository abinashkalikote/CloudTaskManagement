using App.Base.DataContext;
using App.Base.DataContext.Interface;
using App.Base.GenericRepository;
using App.Base.GenericRepository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace App.Base
{
    public static class DiConfig
    {
        public static IServiceCollection UseBase( this IServiceCollection service) => 
            service.AddScoped<IUow, Uow>()
                   .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}
