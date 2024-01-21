using App.Base.Constants;
using App.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using App.Base.DataContext.Interface;
using App.CloudTask.Entity;
using App.CloudTask.Repository.Interfaces;
using App.Web.Data;
using App.Web.Providers.Interface;
using App.Web.Services;
using NepDate;
using App.Web.Repository.Interfaces;

namespace App.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IUserProvider _userProvider;
        private readonly TelegramService _telegramService;
        private readonly HttpContext _httpContext;
        public readonly IAppClientRepo _appClientRepo;
        private readonly IUow _uow;
        private readonly ICloudTaskRepo _cloudTaskRepo;

        public TaskController(
            AppDbContext db,
            IUserProvider userProvider,
            TelegramService telegramService,
            IHttpContextAccessor httpContextAccessor,
            IAppClientRepo appClientRepo,
            IUow uow,
            ICloudTaskRepo cloudTaskRepo
        )
        {
            _db = db;
            _userProvider = userProvider;
            _telegramService = telegramService;
            _appClientRepo = appClientRepo;
            _uow = uow;
            _cloudTaskRepo = cloudTaskRepo;
            _httpContext = httpContextAccessor.HttpContext;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateTask()
        {
            TaskVM vm = new()
            {
                taskTypes = _db.TaskTypes.ToList(),
                clients = await _appClientRepo.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(TaskVM vm)
        {
            if (ModelState.IsValid)
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var pr = 'N';
                    if (vm.HighPriority == "true")
                        pr = 'Y';

                    CloudTask.Entity.CloudTask cloudTask = new()
                    {
                        TaskName = vm.TaskTitle,
                        TaskTypeId = vm.TaskTypeId,
                        ClientId = vm.ClientId,
                        CloudUrl = vm.CloudURL ?? "",
                        Priority = pr,
                        TaskTime = vm.TaskTime,
                        SoftwareVersionFrom = vm.SoftwareVersionFrom ?? "Latest",
                        SoftwareVersionTo = vm.SoftwareVersionTo ?? "Latest",
                        IssueOnPreviousSoftware = vm.IssueOnPreviousSoftware ?? "",
                        Remarks = vm.Remarks ?? "",
                        LicDate = vm.LicDate ?? "",
                        RecAuditLog = "Task Created by " + _userProvider.GetUsername(),
                        RecById = Convert.ToInt32(_userProvider.GetUserId()),
                        TSKStatus = CloudTaskStatus.Pending
                    };

                    //Cloud Task Log a Log
                    CloudTaskLog cloudTaskLog = new()
                    {
                        Remarks = vm.Remarks ?? "",
                        CloudTaskStatus = CloudTaskStatus.Pending,
                        UserId = Convert.ToInt32(_userProvider.GetUserId())
                    };

                    cloudTask.CloudTaskLogs.Add(cloudTaskLog);

                    var result = await _uow.CreateAsync(cloudTask);
                    // var result = await _db.CloudTasks.AddAsync(cloudTask);
                    await _db.SaveChangesAsync();


                    //Sending a message to telegram
                    await SendNewTaskMessageToTelegram(cloudTask, result.Id);

                    scope.Complete();


                    TempData["success"] = "Task Added Successfully !";
                    return RedirectToAction("CreateTask", "Task");
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    TempData["error"] = "Error occured while creating Task !" + ex.Message;
                    return RedirectToAction("CreateTask", "Task");
                }
            }

            vm.taskTypes = await _db.TaskTypes.ToListAsync();
            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> EditTask(long? TaskID)
        {
            try
            {
                if (TaskID == null)
                    throw new Exception($"TaskID {TaskID} can't found !");


                var task = await _cloudTaskRepo.FindAsync(TaskID ?? 0);

                if (task == null)
                    throw new Exception($"Task not found !");

                if (task.TSKStatus == CloudTaskStatus.InProgress ||
                    task.TSKStatus == CloudTaskStatus.Completed)
                    throw new Exception($"Task Is Already in Progress or Completed, You can't Edit this task !");


                TaskVM vm = new()
                {
                    Id = task.Id,
                    TaskTitle = task.TaskName,
                    TaskTypeId = task.TaskTypeId,
                    ClientId = task.ClientId,
                    CloudURL = task.CloudUrl,
                    HighPriority = task.Priority == 'Y' ? "true" : "false",
                    TaskTime = task.TaskTime,
                    SoftwareVersionFrom = task.SoftwareVersionFrom,
                    SoftwareVersionTo = task.SoftwareVersionTo,
                    IssueOnPreviousSoftware = task.IssueOnPreviousSoftware,
                    Remarks = task.Remarks ?? "",
                    LicDate = task.LicDate ?? "",
                    taskTypes = _db.TaskTypes.ToList()
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(GetAllTask));
            }
        }


        [HttpGet]
        public async Task<IActionResult> AuditTask()
        {
            var vm = new TaskReportVM();
            InitializeTaskVm(vm);
            var query = await GetTaskQueryable(vm)
                .Where(e => e.TSKStatus == CloudTaskStatus.Pending)
                .ToListAsync();

            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AuditTask(TaskReportVM vm)
        {
            var data = await GetTaskQueryable(vm)
                .Where(e => e.TSKStatus == CloudTaskStatus.Pending)
                .ToListAsync();
            return RedirectToAction(nameof(AuditTask));
        }


        [HttpGet]
        public async Task<IActionResult> DeleteTask(long? TaskID)
        {
            if (TaskID == null)
            {
                return RedirectToAction(nameof(AuditTask));
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var task = await _cloudTaskRepo.FindAsync(TaskID ?? 0);
            try
            {
                if (task == null)
                {
                    throw new Exception("Task not found to update.");
                }

                task.TSKStatus = CloudTaskStatus.Canceled;
                //Adding a Log
                CloudTaskLog cloudTaskLog = new()
                {
                    Remarks = "",
                    CloudTaskStatus = CloudTaskStatus.Canceled,
                    UserId = Convert.ToInt32(_userProvider.GetUserId())
                };

                task.CloudTaskLogs.Add(cloudTaskLog);

                _uow.Update(task);
                await _db.SaveChangesAsync();

                //Sending Message To Telegram
                string? message = "### Deleted :  <b>" + CloudTaskStatus.Canceled + "</b> ###";
                await _telegramService.SendReplyMessageAsync(message, task.TelegramMessageId);
                scope.Complete();

                TempData["success"] = "Task Reversed successfully !";
                return RedirectToAction(nameof(AuditTask));
            }
            catch (Exception e)
            {
                scope.Dispose();
                TempData["error"] = e.Message;
                return RedirectToAction(nameof(AuditTask));
            }
        }


        #region TaskReportMethod

        [HttpGet]
        public async Task<IActionResult> GetAllTask(TaskReportVM vm)
        {
            InitializeTaskVm(vm);
            var query = await GetTaskQueryable(vm)
                .OrderByDescending(e => e.RecDate)
                .ThenBy(e => e.ProccedTime).ToListAsync();

            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> TaskDetails(long? TaskID)
        {
            if (TaskID == null)
                throw new Exception("Task Not Found !");

            var data = await _cloudTaskRepo.GetQueryable()
                .Include(e => e.TaskType)
                .Include(e => e.ClientId)
                .Include(e => e.RecBy)
                .Include(e => e.ProccedBy)
                .Include(e => e.CompletedBy)
                .Include(e => e.CloudTaskLogs)
                .ThenInclude(cl => cl.User)
                .Where(e => e.RecStatus == Status.Active && e.Id == TaskID)
                .FirstOrDefaultAsync() ?? throw new Exception("Task Not Found !");

            var vm = new TaskTempVM();

            vm.Id = data.Id;
            vm.TaskTitle = data.TaskName;
            vm.ClientId = data.ClientId;
            vm.CloudURL = data.CloudUrl;
            vm.TaskTypeId = data.TaskType.Id;
            vm.TaskTypeName = data.TaskType != null ? data.TaskType.TaskTypeName : "Not Declared";
            vm.TaskTime = data.TaskTime;
            vm.TimeSpan = data.RecDate;


            vm.LicDate = data.LicDate;


            vm.HighPriority = data.Priority == 'Y' ? "Urgent" : "";
            vm.SoftwareVersionFrom = data.SoftwareVersionFrom;
            vm.SoftwareVersionTo = data.SoftwareVersionTo;
            vm.IssueOnPreviousSoftware = data.IssueOnPreviousSoftware;
            vm.RecDate = data.RecDate.ToString("yyyy/MM/dd") + " " + data.RecDate.ToString("dddd");
            vm.RecBy = data.RecBy.FullName;
            vm.ProccedBy = data.ProccedBy != null ? data.ProccedBy.FullName : "-";
            vm.CompletedBy = data.CompletedBy != null ? data.CompletedBy.FullName : "-";
            vm.IsInPending = data.TSKStatus == CloudTaskStatus.Pending;
            vm.IsInProgress = data.TSKStatus == CloudTaskStatus.InProgress;
            vm.IsCompleted = data.TSKStatus == CloudTaskStatus.Completed;
            vm.IsCanceled = data.TSKStatus == CloudTaskStatus.Canceled;

            if (data.CloudTaskLogs.Count <= 0) return View(vm);
            
            foreach (var log in data.CloudTaskLogs)
            {
                CloudTaskLogVM cloudTaskLogVm = new()
                {
                    UserId = log.UserId,
                    UserName = log.User.FullName,
                    CloudTaskStatus = log.CloudTaskStatus,
                    RecDate = log.RecDate
                };
                vm.cloudTaskLogs?.Add(cloudTaskLogVm);
            }

            return View(vm);
        }

        #endregion TaskReportMethod


        public async Task<IActionResult> ProccedTask(long? TaskID)
        {
            if (TaskID == null)
            {
                TempData["error"] = "Something Went Wrong !";
                return RedirectToAction(nameof(GetAllTask));
            }


            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var pendingTask =
                    await _cloudTaskRepo.GetFirstOrDefaultAsync(e =>
                        e.Id == TaskID && e.TSKStatus == CloudTaskStatus.Pending);

                if (pendingTask == null)
                {
                    throw new Exception("Not a valid Task to Proceed !");
                }
                pendingTask.ProccedById = _userProvider.GetUserId();
                pendingTask.ProccedTime = DateTime.Now;
                pendingTask.TSKStatus = CloudTaskStatus.InProgress;


                //Adding a Log
                CloudTaskLog cloudTaskLog = new()
                {
                    Remarks = "",
                    CloudTaskStatus = CloudTaskStatus.InProgress,
                    UserId = Convert.ToInt32(_userProvider.GetUserId())
                };

                pendingTask.CloudTaskLogs.Add(cloudTaskLog);

                _uow.Update(pendingTask);
                await _db.SaveChangesAsync();

                //Sending Message To Telegram
                string? message = "### Action on it : <b>" + _userProvider.GetUsername() + "</b>  . ###";
                await _telegramService.SendReplyMessageAsync(message, pendingTask.TelegramMessageId);
                scope.Complete();


                TempData["success"] = "Task has been Procced Successfully !";
                return RedirectToAction("TaskDetails", new { TaskID = TaskID });
            }
            catch (Exception e)
            {
                scope.Dispose();
                TempData["error"] = e.Message;
                return RedirectToAction(nameof(GetAllTask));
            }
        }

        public async Task<IActionResult> CompletedTask(int? TaskID)
        {
            if (TaskID == null)
                return RedirectToAction(nameof(GetAllTask));


            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var workingTask = await _cloudTaskRepo.GetFirstOrDefaultAsync(e =>
                    e.Id == TaskID && e.TSKStatus == CloudTaskStatus.InProgress);

                if (workingTask == null)
                    throw new Exception("Not a valid Task to Proceed !");

                workingTask.CompletedById = _userProvider.GetUserId();
                workingTask.CompleteTime = DateTime.Now;
                workingTask.TSKStatus = CloudTaskStatus.Completed;


                //Adding a Log
                CloudTaskLog cloudTaskLog = new()
                {
                    Remarks = "",
                    CloudTaskStatus = CloudTaskStatus.Completed,
                    UserId = Convert.ToInt32(_userProvider.GetUserId())
                };

                workingTask.CloudTaskLogs.Add(cloudTaskLog);

                _uow.Update(workingTask);
                await _db.SaveChangesAsync();

                //Sending Message To Telegram
                string? message = "### ✅ Done :  <b>" + _userProvider.GetUsername() + "</b> ###";
                await _telegramService.SendReplyMessageAsync(message, workingTask.TelegramMessageId);

                scope.Complete();
                TempData["success"] = "Task has been Completed Successfully !";
                return RedirectToAction("TaskDetails", new { TaskID = TaskID });
            }
            catch (Exception e)
            {
                scope.Dispose();
                TempData["error"] = e.Message;
                return RedirectToAction(nameof(GetAllTask));
            }
        }


        #region private methods

        private IQueryable<CloudTask.Entity.CloudTask> GetTaskQueryable(TaskReportVM reportVM)
        {
            var data = _cloudTaskRepo.GetQueryable()
                .Include(e => e.TaskType)
                .Include(e => e.RecBy)
                .Include(e => e.ProccedBy)
                .Include(e => e.CompletedBy)
                .Where(e => e.RecStatus == Status.Active);


            //Filter a data as per the passed data by form
            if (reportVM.ClientName != null)
            {
                data = data.Where(e => e.ClientId == reportVM.ClientName);
            }

            if (!reportVM.TaskTitle.IsNullOrEmpty())
            {
                data = data.Where(e => e.TaskName.Contains(reportVM.TaskTitle));
            }

            if (reportVM.TaskTypeId != null)
            {
                data = data.Where(e => e.TaskTypeId == reportVM.TaskTypeId);
            }

            if (!reportVM.TaskTime.IsNullOrEmpty())
            {
                data = data.Where(e => e.TaskTime.Contains(reportVM.TaskTime));
            }

            if (reportVM.CreatedBy != null)
            {
                data = data.Where(e => e.RecById == reportVM.CreatedBy);
            }

            if (reportVM.TSKStatus != null)
            {
                data = data.Where(e => e.TSKStatus == reportVM.TSKStatus);
            }


            return data;
        }

        private List<TaskTempVM> PrepareTaskVms(List<CloudTask.Entity.CloudTask> data)
        {
            var list = new List<TaskTempVM>();
            foreach (var item in data)
            {
                var vm = new TaskTempVM();
                vm.Id = item.Id;
                vm.TaskTitle = item.TaskName;
                vm.ClientId = item.ClientId;
                vm.CloudURL = item.CloudUrl;
                vm.TaskTypeName = item.TaskType != null ? item.TaskType.TaskTypeName : "Not Declared";
                vm.TaskTime = item.TaskTime;
                vm.HighPriority = item.Priority == 'Y' ? "Yes" : "No";
                vm.SoftwareVersionFrom = item.SoftwareVersionFrom;
                vm.SoftwareVersionTo = item.SoftwareVersionTo;
                vm.IssueOnPreviousSoftware = item.IssueOnPreviousSoftware;
                vm.RecDate = item.RecDate.ToString("yyyy/MM/dd") + " " + item.RecDate.ToString("dddd");
                vm.TimeSpan = item.RecDate;

                vm.LicDate = item.LicDate;

                vm.RecBy = item.RecBy.FullName;
                vm.ProccedBy = item.ProccedBy != null ? item.ProccedBy.FullName : "-";
                vm.CompletedBy = item.CompletedBy != null ? item.CompletedBy.FullName : "-";
                vm.IsInPending = item.TSKStatus == CloudTaskStatus.Pending;
                vm.IsInProgress = item.TSKStatus == CloudTaskStatus.InProgress;
                vm.IsCompleted = item.TSKStatus == CloudTaskStatus.Completed;
                vm.IsCanceled = item.TSKStatus == CloudTaskStatus.Canceled;
                vm.IsShowPeekButton = true;
                list.Add(vm);
            }

            return list;
        }

        private void InitializeTaskVm(TaskReportVM vm)
        {
            vm.taskTypes = _db.TaskTypes.ToList();
            vm.Users = _db.Users.Where(e => e.RecStatus == 'A').ToList();
        }


        //Private Method to send Newly created task in Telegram
        private async Task SendNewTaskMessageToTelegram(CloudTask.Entity.CloudTask cloudTask, long TaskId)
        {
            var user = _userProvider.GetUsername();


            string TaskCreatedBy = "";
            if (user != null)
                TaskCreatedBy = $"## Task Created by <b>{user}</b> ##";
            else
                TaskCreatedBy = "";


            string pri = "";
            if (cloudTask.Priority == 'Y')
                pri = $"<b>Priority :</b> Urgent\r\n";
            else
                pri = "";

            // Get the current HttpContext
            var httpContext = HttpContext;
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host.Host;
            var port = httpContext.Request.Host.Port;
            var baseUrl = $"{scheme}://{host}:{port}/Task/TaskDetails?TaskID=" + TaskId;


            string? message = "";
            if (cloudTask.TaskTypeId == 1)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    $"<b>Software Version :</b> <b>From</b> {cloudTask.SoftwareVersionFrom} <b>To</b> {cloudTask.SoftwareVersionTo}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>Cloud URL :</b> {cloudTask.CloudUrl}\r\n" +
                    $"<b>Issue :</b> {cloudTask.IssueOnPreviousSoftware} \r\n\r\n\r\n" +
                    $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }
            else if (cloudTask.TaskTypeId == 2)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>PAN No :</b> PANNo will be here\r\n" +
                    $"<b>License Date :</b> {cloudTask.LicDate}\r\n\r\n" +
                    $"<b>Task Link</b> : <a href=\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }
            else if (cloudTask.TaskTypeId == 3)
            {
                message =
                    $"<b>Date:</b> {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>To do :</b> {cloudTask.TaskName}\r\n" +
                    pri +
                    $"<b>Update Time :</b> {cloudTask.TaskTime} \r\n" +
                    $"------------------------------------------------------\r\n" +
                    $"<b>Client :</b> {cloudTask.ClientId}\r\n" +
                    $"<b>Cloud URL :</b> {cloudTask.CloudUrl}\r\n\r\n" +
                    $"<b>Task Link</b> : <a href='\"{baseUrl}\">{baseUrl}</a> \r\n\r\n" +
                    TaskCreatedBy;
            }


            await _telegramService.SendMessageAsync(message, TaskId);
        }

        #endregion private methods


        #region TaskAPI

        [HttpGet]
        public async Task<IActionResult> GetTask(int Id)
        {
            try
            {
                var data = await _cloudTaskRepo.GetItemAsync(e => e.Id == Id && e.RecStatus == Status.Active);

                var finalData = new
                {
                    ClientName = data.ClientId,
                    TaskTitle = data.TaskName,
                    CloudURL = data.CloudUrl,
                    SoftwareVersionFrom = data.SoftwareVersionFrom ?? "",
                    SoftwareVersionTo = data.SoftwareVersionTo ?? "",
                    IssueOnPreviousVersion = data.IssueOnPreviousSoftware ?? "",
                    CreatedDate = data.RecDate.Date,
                    CreatedBy = data.RecBy.FullName,
                    data.Remarks
                };
                return Ok(new
                {
                    finalData
                });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        #endregion TaskAPI
    }
}