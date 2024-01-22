using App.Base.Constants;
using App.Models;
using App.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using App.CloudTask.Repositories.Interfaces;
using App.Web.Data;

namespace App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _db;
        private readonly ICloudTaskRepo _cloudTaskRepo;

        public TelegramService _telegramService { get; }

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext db,
            ICloudTaskRepo cloudTaskRepo)
        {
            _db = db;
            _cloudTaskRepo = cloudTaskRepo;
            _logger = logger;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalTaskCount = await _cloudTaskRepo.GetCountAsync(e => e.RecStatus == Status.Active);
            ViewBag.TotalPendingTaskCount = GetTaskCount(CloudTaskStatus.Pending);
            ViewBag.TotalWorkingTaskCount = GetTaskCount(CloudTaskStatus.InProgress);
            ViewBag.TotalCompletedTaskCount = GetTaskCount(CloudTaskStatus.Completed);
            ViewBag.TotalCanceledTaskCount = GetTaskCount(CloudTaskStatus.Canceled);
            
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


        #region DashboardChartAPI
        public IActionResult GetNoOfTask()
        {
            try
            {
                var pendingTask = _cloudTaskRepo.GetQueryable().Count(e => e.TSKStatus == CloudTaskStatus.Pending);
                var workingTask =_cloudTaskRepo.GetQueryable().Count(e => e.TSKStatus == CloudTaskStatus.InProgress);
                var completedTask = _cloudTaskRepo.GetQueryable().Count(e => e.TSKStatus == CloudTaskStatus.Completed);
                var canceledTask = _cloudTaskRepo.GetQueryable().Count(e => e.TSKStatus == CloudTaskStatus.Canceled);

                var data = new
                {
                    pendingTask, workingTask, completedTask, canceledTask
                };
                return Ok(new {data});
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion DashboardChartAPI

        #region privateMethods
        private int GetTaskCount(string taskStatus)
        {
            return _cloudTaskRepo.GetQueryable().Count(e => e.TSKStatus == taskStatus);
        }

        #endregion
    }
}