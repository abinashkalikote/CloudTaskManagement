using App.Model;
using App.Web.Constants;
using App.Web.ViewModel;
using CTM.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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
                using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
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
                        RecAuditLog = "Task Created by " + HttpContext.Session.GetString("FullName"),
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
                    TempData["error"] = "Error occured while creating Task !" + ex.Message;
                    return RedirectToAction("CreateTask", "Task");
                }
            }
            vm.taskTypes = await _db.TaskTypes.ToListAsync();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AuditTask()
        {
            var vm = new TaskReportVM();
            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).Where(e => e.ProccedById != null || (e.ProccedById != null && e.CompletedById != null)).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AuditTask(TaskReportVM vm)
        {
            var data = await GetTaskQueryable(vm).Where(e => e.ProccedById != null || (e.ProccedById != null && e.CompletedById != null)).ToListAsync();
            return RedirectToAction(nameof(AuditTask));
        }

        
        [HttpGet]
        public async Task<IActionResult> DeleteTask(int? TaskID)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var Task = await _db.CloudTasks.FirstOrDefaultAsync(e => e.Id == TaskID);
            try { 
                Task.RecStatus = Status.InActive;
                _db.CloudTasks.Update(Task);

                await _db.SaveChangesAsync();
                scope.Complete();
            }catch(Exception ex)
            {
                scope.Dispose();
                TempData["error"] = "Task Cannot be updated !";
                return RedirectToAction("AuditTask");
            }

            return RedirectToAction("AuditTask");
        }


        [HttpGet]
        //[Route("/Api/GetAllTasks")]
        public async Task<IActionResult> GetAllTask(TaskReportVM vm)
        {

            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }
        public async Task<IActionResult> GetAllPendingTask(TaskReportVM vm)
        {

            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).Where(e => (e.ProccedById == null && e.CompletedById == null)).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }
        public async Task<IActionResult> GetAllWorkingTask(TaskReportVM vm)
        {
            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).Where(e => (e.ProccedById != null && e.CompletedById == null)).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
        }
        public async Task<IActionResult> GetAllCompletedTask(TaskReportVM vm)
        {
            InitializeTaskVM(vm);
            var query = await GetTaskQueryable(vm).Where(e => (e.ProccedById != null && e.CompletedById != null)).ToListAsync();
            vm.Tasks = PrepareTaskVms(query);
            return View(vm);
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
                vm.RecBy = item.RecBy.Username;
                vm.ProccedBy = item.ProccedBy != null ? item.ProccedBy.Username : "-";
                vm.CompletedBy = item.CompletedBy != null ? item.CompletedBy.Username : "-";
                list.Add(vm);
            }
            return list;
        }
       
        private void InitializeTaskVM(TaskReportVM vm)
        {
            vm.taskTypes = _db.TaskTypes.ToList();
            vm.Users = _db.Users.Where(e => e.RecStatus == 'A').ToList();
        }
    }
}
