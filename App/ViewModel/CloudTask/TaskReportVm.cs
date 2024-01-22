using App.Base.Constants;
using App.Base.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.ViewModel.CloudTask
{
    public class TaskReportVm
    {

        /// <summary>
        /// For Filter
        /// </summary>
        public string? TaskTitle { get; set; }
        public int? TaskTypeId { get; set; }
        public string? TaskTime { get; set; }
        public int? CreatedBy { get; set; }
        public int? ClientName { get; set; }
        public string? TSKStatus { get; set; }





        //List of Task Send to show in Table
        public List<TaskTempVm>? Tasks { get; set; }






        /// <summary>
        /// This will be used to make a TaskTypes dropdown List in view
        /// </summary>
        public List<TaskType>? taskTypes { get; set; }
        public SelectList TaskTypesList() => new SelectList(taskTypes, nameof(TaskType.Id), nameof(TaskType.TaskTypeName), TaskTypeId);


        public SelectList TaskStatusList() => new SelectList(CloudTaskStatus.TaskStatusList, TSKStatus);


        public SelectList TaskTimeList() => new SelectList(new List<TaskTimeVm>()
        {
            new TaskTimeVm(){TaskTimeName="After Office Hour"},
            new TaskTimeVm(){TaskTimeName="Immediately"},
        }, nameof(TaskTimeVm.TaskTimeName), nameof(TaskTimeVm.TaskTimeName), TaskTime);


        public List<User>? Users { get; set; }
        public SelectList UserList() => new SelectList(Users, nameof(User.Id), nameof(User.FullName), CreatedBy);

    }
}
