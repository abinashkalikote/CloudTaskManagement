using App.Base.ValueObject;
using App.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.ViewModel
{
    public class TaskReportVM
    {

        /// <summary>
        /// For Filter
        /// </summary>
        public string? TaskTitle { get; set; }
        public int? TaskTypeId { get; set; }
        public string? TaskTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? ClientName { get; set; }


        //List of Task Send to show in Table
        public List<TaskTempVM>? Tasks { get; set; }



        /// <summary>
        /// This will be used to make a TaskTypes dropdown List in view
        /// </summary>
        public List<TaskType>? taskTypes { get; set; }
        public SelectList TaskTypesList() => new SelectList(taskTypes, nameof(TaskType.Id), nameof(TaskType.TaskTypeName), TaskTypeId);



        public SelectList TaskTimeList() => new SelectList(new List<TaskTimeVM>()
        {
            new TaskTimeVM(){TaskTimeName="After a Hours"},
            new TaskTimeVM(){TaskTimeName="Immediately"},
        }, nameof(TaskTimeVM.TaskTimeName), nameof(TaskTimeVM.TaskTimeName), TaskTime);


        public List<User>? Users { get; set; }
        public SelectList UserList() => new SelectList(Users, nameof(User.Id), nameof(User.Username), CreatedBy);

    }
}
