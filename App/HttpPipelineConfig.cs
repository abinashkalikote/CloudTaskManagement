using App.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Web
{
    public static class HttpPipelineConfig
    {
        public static void HttpPipelineConfiguration(this WebApplication app)
        {

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
        }
    }
}
