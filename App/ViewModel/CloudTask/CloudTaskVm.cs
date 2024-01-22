using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Base.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.ViewModel.CloudTask
{
    public class CloudTaskVm
    {
        public long? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Task Title")]
        public string TaskTitle { get; set; }


        [Required]
        [DisplayName("Task Type")]
        public int TaskTypeId { get; set; }

        [Required]
        [DisplayName("Client Name")]
        public long ClientId { get; set; }

        [DisplayName("Cloud URL")]
        public string? CloudURL { get; set; }

        [DisplayName("High Priority ?")]
        public bool HighPriority { get; set; }

        [DisplayName("Task Time")]
        public string TaskTime { get; set; }

        [DisplayName("Software Version From")]
        public string? SoftwareVersionFrom { get; set; }

        [DisplayName("Software Version To")]
        public string? SoftwareVersionTo { get; set; }

        [DisplayName("Issue On Previous Version")]
        public string? IssueOnPreviousSoftware { get; set; }

        [DisplayName("Remarks")]
        public string? Remarks { get; set; }



        [DisplayName("PAN No.")]
        public string? PANNo { get; set; }

        [DisplayName("License Date")]
        public string? LicDate { get; set; }

        [DisplayName("Address")]
        public string? ClientAddress { get; set; }


        // For Display Purpose
        public string? TaskTypeName { get; set; }
        public string? RecDate { get; set; }
        public string? RecBy { get; set; }
        public string? ProccedBy { get; set; }
        public string? CompletedBy { get; set; }



        /// <summary>
        /// This will be used to make a TaskTypes dropdown List in view
        /// </summary>
        public List<TaskType>? taskTypes { get; set; }
        public SelectList TaskTypesList() => new SelectList(taskTypes, nameof(TaskType.Id), nameof(TaskType.TaskTypeName), TaskTypeId);



        public List<Base.Entities.AppClient>? clients;
        public SelectList GetClientList() => new SelectList(clients, nameof(Base.Entities.AppClient.Id), nameof(Base.Entities.AppClient.ClientName), ClientId);


        public SelectList TaskTimeList() => new SelectList(new List<TaskTimeVm>()
        {
            new(){TaskTimeName="After Office hours"},
            new(){TaskTimeName="Immediately"},
        }, nameof(TaskTimeVm.TaskTimeName), nameof(TaskTimeVm.TaskTimeName), TaskTime);
    }
}
