using App.Model;
using App.Web.Constants;
using App.Web.ViewModel;
using CTM.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Transactions;

namespace App.Web.Controllers
{
    [Authorize]
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
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var pr = 'N';
                        if (taskVM.HighPriority == "true")
                            pr = 'Y';

                        CloudTask cloudTask = new()
                        {
                            TaskName = taskVM.TaskTitle,
                            TaskTypeId = taskVM.TaskTypeId,
                            ClientName = taskVM.ClientName,
                            CloudUrl = taskVM.CloudURL,
                            Priority = pr,
                            TaskTime = taskVM.TaskTime,
                            SoftwareVersionFrom = taskVM.SoftwareVersionFrom ?? "Latest",
                            SoftwareVersionTo = taskVM.SoftwareVersionTo ?? "Latest",
                            IssueOnPreviousSoftware = taskVM.IssueOnPreviousSoftware ?? "",
                            Remarks = taskVM.Remarks ?? "",
                            RecAuditLog = "Task Created by "+HttpContext.Session.GetString("FullName"),
                            RecById = Convert.ToInt32(HttpContext.Session.GetString("UserID"))
                        };

                        await _db.CloudTasks.AddAsync(cloudTask);
                        await _db.SaveChangesAsync();
                        transactionScope.Complete();

                        TempData["success"] = "Task Added Successfully !";
                        return RedirectToAction("CreateTask", "Task");
                    }
                    catch (Exception ex)
                    {
                        transactionScope.Dispose();
                        TempData["error"] = "Error occured while creating Task !";
                        return RedirectToAction("CreateTask", "Task");
                    }
                }
            }
            taskVM.taskTypes = await _db.TaskTypes.ToListAsync();
            return View(taskVM);
        }

        [HttpGet]
        [Route("/Api/GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var data = await _db.CloudTasks.Include(e => e.TaskType)
                .Include(e => e.RecBy)
                .Include(e => e.ProccedBy)
                .Include(e => e.CompletedBy)
                .Where(e => e.RecStatus == Status.Active).ToListAsync();
            List<TaskVM> taskVM = new List<TaskVM>();
            foreach (var item in data)
            {
                var tempTVM = new TaskVM();
                tempTVM.Id = item.Id;
                tempTVM.TaskTitle = item.TaskName;
                tempTVM.ClientName = item.ClientName;
                tempTVM.CloudURL = item.CloudUrl;
                tempTVM.TaskTypeName = item.TaskType != null ? item.TaskType.TaskTypeName : "Not Declared";
                tempTVM.TaskTime = item.TaskTime;
                tempTVM.SoftwareVersionFrom = item.SoftwareVersionFrom;
                tempTVM.SoftwareVersionTo = item.SoftwareVersionTo;
                tempTVM.IssueOnPreviousSoftware = item.IssueOnPreviousSoftware;
                tempTVM.RecDate = item.RecDate.ToString("yyyy/MM/dd") +" "+ item.RecDate.ToString("dddd");
                tempTVM.RecBy = item.RecBy.Username;
                tempTVM.ProccedBy = item.ProccedBy != null ? item.ProccedBy.Username : "-- Not Assigned --";
                tempTVM.CompletedBy = item.CompletedBy != null ? item.CompletedBy.Username : "-- Not Assigned --";
                taskVM.Add(tempTVM);
            }

            return Ok(taskVM);
        }
    }
}
