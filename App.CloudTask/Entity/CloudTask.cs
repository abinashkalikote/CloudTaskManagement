using System.ComponentModel.DataAnnotations.Schema;
using App.Base.Entities;

namespace App.CloudTask.Entity;

[Table("CloudTasks")]
public class CloudTask
{
    public long Id { get; set; }
    public string TaskName { get; set; }
    
    public virtual TaskType? TaskType { get; set; }
    public int TaskTypeId { get; set; }

    public long ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual AppClient? Client { get; set; }
    
    public char? Priority { get; set; } = 'Y';
    public string TaskTime { get; set; }
    public string? CloudUrl { get; set; }
    public string? IssueOnPreviousSoftware { get; set; }
    public string? SoftwareVersionFrom { get; set; }
    public string? SoftwareVersionTo { get; set; }

    public string? Remarks { get; set; }

    //For New client Setup on cloud
    public string? LicDate { get; set; }



    public string RecAuditLog { get; set; } = "";
    public int RecVersion { get; set; } = 1;
    public char? RecStatus { get; set; } = 'A';
    public DateTime RecDate { get; set; } = DateTime.Now;
    public string TSKStatus { get; set; }
    public DateTime? ProccedTime { get; set; }
    public DateTime? CompleteTime { get; set; }


    public virtual User RecBy { get; set; }
    public long RecById { get; set; }

    public virtual User? ProccedBy { get; set; }
    public long? ProccedById { get; set; }

    public virtual User? CompletedBy { get; set; }
    public long? CompletedById { get; set; }

    public int? TelegramMessageId { get; set; }



    public virtual List<CloudTaskLog> CloudTaskLogs { get; set; } = new();
}