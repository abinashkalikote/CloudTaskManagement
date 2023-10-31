using App.Model;
using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace App.Web.Controllers
{
    public class SeedingController : Controller
    {
        private readonly AppDbContext _db;

        public SeedingController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            //User Seeding
            if (!await _db.Users.AnyAsync())
            {
                await _db.Users.AddRangeAsync(UsersData);
                await _db.SaveChangesAsync();
            }

            //Task Types Seeding
            if (!await _db.TaskTypes.AnyAsync())
            {
                await _db.TaskTypes.AddRangeAsync(TaskTypesData);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }





        /// <summary>
        /// Seeding Data Declaration
        /// </summary>
        public List<TaskType> TaskTypesData = new()
        {
            new TaskType { TaskTypeName="Software Update", RecById = 1 },
            new TaskType { TaskTypeName="New Client", RecById = 1 },
            new TaskType { TaskTypeName="Local To Cloud Update", RecById = 1 }
        };

        public List<User> UsersData = new()
        {
            new User { FullName="Office", Email="info@prathamit.com", IsNewPassword='N', Password=BC.EnhancedHashPassword("test"), IsAdmin='Y', RecBy="AutoAdmin" }
        };
    }
}
