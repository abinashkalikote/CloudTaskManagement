using App.Web.ViewModel;
using CTM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace App.Web.Controllers
{
    
    public class TaskController : Controller
    {
        private readonly AppDbContext _db;

        public TaskController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            TaskVM taskVM = new()
            {
                taskTypes = _db.TaskTypes.ToList()
            };
            return View(taskVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(TaskVM taskVM)
        {
            if (ModelState.IsValid)
            {

            }
            taskVM.taskTypes = await _db.TaskTypes.ToListAsync();
            return View(taskVM);
        }
    }
}
