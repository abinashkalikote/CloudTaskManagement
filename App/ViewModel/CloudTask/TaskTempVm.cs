﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModel.CloudTask
{
    public class TaskTempVm
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
        public string ClientName { get; set; }

        [DisplayName("Cloud URL")]
        public string? CloudURL { get; set; }

        [DisplayName("High Priority ?")]
        public string HighPriority { get; set; }

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


        public bool IsInPending { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCanceled { get; set; }

        public bool? IsShowPeekButton { get; set; } = true;
        public bool? IsShowDetailsButton { get; set; } = true;
        public bool? IsShowActionButton { get; set; } = true;



        // For Display Purpose
        public string? TaskTypeName { get; set; }
        public string? RecDate { get; set; }
        public string? ProccedTime { get; set; }
        public string? CompleteTime { get; set; }
        public string? RecBy { get; set; }
        public string? ProccedBy { get; set; }
        public string? CompletedBy { get; set; }
        public DateTime? TimeSpan { get; set; }

        public List<CloudTaskLogVm>? cloudTaskLogs { get; set; } = new List<CloudTaskLogVm>();
    }
}
