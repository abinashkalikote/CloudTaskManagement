using App.Web.Models;
using App.Web.Repository.Interfaces;
using App.Web.Repository;
using App.Web.Services.Interfaces;
using App.Web.Services;
using App.Web.Validator.Interfaces;
using App.Web.Validator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Pioneer.Pagination;
using App.Base.Configuration;
using App.Base.Providers.Interfaces;
using App.Base.Services.Interfaces;
using App.CloudTask.Configuration;
using App.Web.Data;
using App.Web.Providers.Implementation;

namespace App.Web
{
    public static class DiConfig
    {
        public static void UseApp(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<DbContext, AppDbContext>();


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/Access/Login";
                    option.ExpireTimeSpan = TimeSpan.FromDays(1);
                    option.LogoutPath = "/Home/Logout";
                });

            builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromMinutes(20));

            builder.Services.UseBase();
            builder.Services.UseCloudTask();
            
            
            builder.Services.AddScoped<ILoginUserProvider, LoginUserProvider>()
                .AddScoped<IHostProvider, HostProvider>();
            builder.Services.AddTransient<IPaginatedMetaService, PaginatedMetaService>();

            builder.Services.AddScoped<IAppClientRepo, AppClientRepo>()
                .AddScoped<IAppClientValidator, AppClientValidator>()
                .AddScoped<IAppClientService, AppClientService>();


            builder.Services.AddScoped<ITelegramService, TelegramService>();
            builder.Services.AddHttpClient();
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            builder.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            builder.Services.AddControllersWithViews();
        }
    }
}
