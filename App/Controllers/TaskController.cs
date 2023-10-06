using App.Model;
using App.Base.Constants;
using App.Web.ViewModel;
using CTM.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using App.Web.Providers.Interface;
using App.Base.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Threading.Tasks;
using App.Web.Services;
using NepDate;

namespace App.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly AppDbContext _db;

        private readonly IUserProvider _userProvider;
        private readonly TelegramService _telegramService;

        public TaskController(
            AppDbContext db,
            IUserProvider userProvider,
            TelegramService telegramService
            )
        {
            _db = db;
            _userProvider = userProvider;
            _telegramService = telegramService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            TaskVM vm = new()
            {
                taskTypes = _db.TaskTypes.ToList()
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

                    CloudTask cloudTask = new()
                    {
                        TaskName = vm.TaskTitle,
                        TaskTypeId = vm.TaskTypeId,
                        ClientName = vm.ClientName,
                        CloudUrl = vm.CloudURL,
                        Priority = pr,
                        TaskTime = vm.TaskTime,
                        SoftwareVersionFrom = vm.SoftwareVersionFrom ?? "Latest",
                        SoftwareVersionTo = vm.SoftwareVersionTo ?? "Latest",
                        IssueOnPreviousSoftware = vm.IssueOnPreviousSoftware ?? "",
                        Remarks = vm.Remarks ?? "",
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

                    await _db.CloudTasks.AddAsync(cloudTask);
                    await _db.SaveChangesAsync();
                    scope.Complete();

                    var pri = cloudTask.Priority == 'Y' ? "Urgent" : "";

                    string message = $"Date: {@Convert.ToDateTime(cloudTask.RecDate).ToNepaliDate()}\r\nTo do : {cloudTask.TaskName}\r\nPriority : { pri }\r\nClient : {cloudTask.ClientName}\r\nCloud URL : {cloudTask.CloudUrl}";
                    await _telegramService.SendMessageAsync(message);


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
        public IActionResult EditTask(int? TaskID)
        {
            if (TaskID == null)
                throw new Exception($"TaskID {TaskID} can't found !");


            var task = _db.CloudTasks.FirstOrDefault(e => e.Id == TaskID);

            if (task == null)
                throw new Exception($"Task not found !");

            if (task.TSKStatus == CloudTaskStatus.InProgress ||
                task.TSKStatus == CloudTaskStatus.Completed)
                throw new Exception($"Task Is Alreday in Progress or Completed, You can't Edit this task !");


            TaskVM vm = new()
            {
                Id = task.Id,
                TaskTitle = task.TaskName,
                TaskTypeId = task.TaskTypeId,
                ClientName = task.ClientName,
                CloudURL = task.CloudUrl,
                HighPriority = task.Priority == 'Y' ? "true" : "false",
                TaskTime = task.TaskTime,
                SoftwareVersionFrom = task.SoftwareVersionFrom,
                SoftwareVersionTo = task.SoftwareVersionTo,
                IssueOnPreviousSoftware = task.IssueOnPreviousSoftware,
                Remarks = task.Remarks ?? "",
                taskTypes = _db.TaskTypes.ToList()
            };
            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> AuditTask()
        {
            var vm = new TaskReportVM();
            InitializeTaskVM(vm);
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
        public async Task<IActionResult> DeleteTask(int? TaskID)
        {
            if (TaskID == null)
                return RedirectToAction(nameof(AuditTask));

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var task = await _db.CloudTasks.FirstOrDefaultAsync(e => e.Id == TaskID);
            try
            {
                task.TSKStatus = CloudTaskStatus.Canceled;


                //Adding a Log
                CloudTaskLog cloudTaskLog = new()
                {
                    Remarks = "",
                    CloudTaskStatus = CloudTaskStatus.Canceled,
                    UserId = Convert.ToInt32(_userProvider.GetUserId())
                };

                task.CloudTaskLogs.Add(cloudTaskLog);

                _db.CloudTasks.Update(task);
                await _db.SaveChangesAsync();
                scope.Complete();

                TempData["success"] = "Task Reversed successfully !";
                return RedirectToAction(nameof(AuditTask));
            }
            catch (Exception)
            {
                scope.Dispose();
                TempData["error"] = "Task Cannot be Delete !";
                return RedirectToAction(nameof(AuditTask));
            }
        }


        #region TaskReportMethod
        [HttpGet]
        public async Task<IActionResult> GetAllTask(TaskReportVM vm)
        {

            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }

        public async Task<IActionResult> TaskDetails(int? TaskID)
        {
            if (TaskID == null)
                throw new Exception("Task Not Found !");

            var data = await _db.CloudTasks.Include(e => e.TaskType)
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
            vm.ClientName = data.ClientName;
            vm.CloudURL = data.CloudUrl;
            vm.TaskTypeId = data.TaskType.Id;
            vm.TaskTypeName = data.TaskType != null ? data.TaskType.TaskTypeName : "Not Declared";
            vm.TaskTime = data.TaskTime;
            vm.TimeSpan = data.RecDate;
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

            if (data.CloudTaskLogs.Count > 0)
            {
                foreach (var log in data.CloudTaskLogs)
                {
                    CloudTaskLogVM cloudTaskLogVM = new()
                    {
                        UserId = log.UserId,
                        UserName = log.User.FullName,
                        CloudTaskStatus = log.CloudTaskStatus,
                        RecDate = log.RecDate
                    };
                    vm.cloudTaskLogs.Add(cloudTaskLogVM);
                }
            }

            return View(vm);
        }
        #endregion TaskReportMethod




        public async Task<IActionResult> ProccedTask(int? TaskID)
        {
            if (TaskID == null)
            {
                TempData["error"] = "Something Went Wrong !";
                return RedirectToAction(nameof(GetAllTask));
            }


            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {

                var pendingTask = await _db.CloudTasks.FirstOrDefaultAsync(e => e.Id == TaskID && e.TSKStatus == CloudTaskStatus.Pending);

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

                _db.CloudTasks.Update(pendingTask);
                await _db.SaveChangesAsync();

                scope.Complete();
                TempData["success"] = "Task has been Procced Successfully !";
                return RedirectToAction(nameof(GetAllTask));
            }
            catch (Exception)
            {
                scope.Dispose();
                TempData["error"] = "Something Went Wrong !";
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
                var workingTask = await _db.CloudTasks.FirstOrDefaultAsync(e => e.Id == TaskID && e.TSKStatus == CloudTaskStatus.InProgress);

                if (workingTask == null)
                    throw new Exception("Task Not Found !");

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

                _db.CloudTasks.Update(workingTask);
                await _db.SaveChangesAsync();

                scope.Complete();
                TempData["success"] = "Task has been Completed Successfully !";
                return RedirectToAction(nameof(GetAllTask));
            }
            catch (Exception)
            {
                scope.Dispose();
                TempData["error"] = "Something Went Wrong !";
                return RedirectToAction(nameof(GetAllTask));
            }
        }






        #region private methods

        private IQueryable<CloudTask> GetTaskQueryable(TaskReportVM reportVM)
        {
            var data = _db.CloudTasks.Include(e => e.TaskType)
                .Include(e => e.RecBy)
                .Include(e => e.ProccedBy)
                .Include(e => e.CompletedBy)
                .Where(e => e.RecStatus == Status.Active);


            //Filter a data as per the passed data by form
            if (!reportVM.ClientName.IsNullOrEmpty())
            {
                data = data.Where(e => e.ClientName.Contains(reportVM.ClientName));
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

        private List<TaskTempVM> PrepareTaskVms(List<CloudTask> data)
        {
            var list = new List<TaskTempVM>();
            foreach (var item in data)
            {
                var vm = new TaskTempVM();
                vm.Id = item.Id;
                vm.TaskTitle = item.TaskName;
                vm.ClientName = item.ClientName;
                vm.CloudURL = item.CloudUrl;
                vm.TaskTypeName = item.TaskType != null ? item.TaskType.TaskTypeName : "Not Declared";
                vm.TaskTime = item.TaskTime;
                vm.HighPriority = item.Priority == 'Y' ? "Yes" : "No";
                vm.SoftwareVersionFrom = item.SoftwareVersionFrom;
                vm.SoftwareVersionTo = item.SoftwareVersionTo;
                vm.IssueOnPreviousSoftware = item.IssueOnPreviousSoftware;
                vm.RecDate = item.RecDate.ToString("yyyy/MM/dd") + " " + item.RecDate.ToString("dddd");
                vm.TimeSpan = item.RecDate;
                vm.RecBy = item.RecBy.FullName;
                vm.ProccedBy = item.ProccedBy != null ? item.ProccedBy.FullName : "-";
                vm.CompletedBy = item.CompletedBy != null ? item.CompletedBy.FullName : "-";
                vm.IsInPending = item.TSKStatus == CloudTaskStatus.Pending;
                vm.IsInProgress = item.TSKStatus == CloudTaskStatus.InProgress;
                vm.IsCompleted = item.TSKStatus == CloudTaskStatus.Completed;
                vm.IsCanceled = item.TSKStatus == CloudTaskStatus.Canceled;
                vm.IsShowDetailsPeekButton = true;
                list.Add(vm);
            }
            return list;
        }

        private void InitializeTaskVM(TaskReportVM vm)
        {
            vm.taskTypes = _db.TaskTypes.ToList();
            vm.Users = _db.Users.Where(e => e.RecStatus == 'A').ToList();
        }

        #endregion private methods
    }
}
