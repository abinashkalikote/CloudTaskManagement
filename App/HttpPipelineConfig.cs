using App.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Web
{
    public static class HttpPipelineConfig
    {
        public static void HttpPipelineConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
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
        }
    }
}
