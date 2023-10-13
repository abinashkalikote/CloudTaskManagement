using App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using App.Web.Providers.Interface;
using App.Web.Providers;
using Pioneer.Pagination;
using App.Web.Services;
using App.Web.Models;
using App.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<DbContext, AppDbContext>();



builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Access/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromMinutes(20));

builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddTransient<IPaginatedMetaService, PaginatedMetaService>();


builder.Services.AddScoped<TelegramService>();
builder.Services.AddHttpClient();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));



builder.Services.AddControllersWithViews();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
