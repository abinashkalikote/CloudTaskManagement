using App.Base.Constants;
using App.Models;
using App.Web.Services;
using CTM.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _db;

        public TelegramService _telegramService { get; }

        public HomeController(
            ILogger<HomeController> logger, 
            AppDbContext db)
        {
            _db = db;
            _logger = logger;
        }



        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.TotalTaskCount = _db.CloudTasks.Count();
            ViewBag.TotalPendingTaskCount = _db.CloudTasks.Where(e => e.TSKStatus == CloudTaskStatus.Pending).Count();
            ViewBag.TotalWorkingTaskCount = _db.CloudTasks.Where(e => e.TSKStatus == CloudTaskStatus.InProgress).Count();
            ViewBag.TotalCompletedTaskCount = _db.CloudTasks.Where(e => e.TSKStatus == CloudTaskStatus.Completed).Count();
            ViewBag.TotalCanceledTaskCount = _db.CloudTasks.Where(e => e.TSKStatus == CloudTaskStatus.Canceled).Count();
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}