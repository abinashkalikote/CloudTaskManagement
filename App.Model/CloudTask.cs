namespace App.Model
{
    public class CloudTask
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public virtual TaskType? TaskType { get; set; }
        public int TaskTypeId { get; set; }

        public int ClientName { get; set; }
        public char? Priority { get; set; } = 'Y';
        public string TaskTime { get; set; }
        public string? CloudUrl { get; set; }
        public string? IssueOnPreviousSoftware { get; set; }
        public string? SoftwareVersionFrom { get; set; }
        public string? SoftwareVersionTo { get; set; }

        public string? Remarks { get; set; }

        //For New client Setup on cloud
        public string? PANNo { get; set; }
        public string? LicDate { get; set; }
        public string? ClientAddress { get; set; }



        public string RecAuditLog { get; set; } = "";
        public int RecVersion { get; set; } = 1;
        public char? RecStatus { get; set; } = 'A';
        public DateTime RecDate { get; set; } = DateTime.Now;
        public string TSKStatus { get; set; }
        public DateTime? ProccedTime { get; set; }
        public DateTime? CompleteTime { get; set; }


        public virtual User RecBy { get; set; }
        public int RecById { get; set; }

        public virtual User? ProccedBy { get; set; }
        public int? ProccedById { get; set; }

        public virtual User? CompletedBy { get; set; }
        public int? CompletedById { get; set; }

        public int? TelegramMessageId { get; set; }



        public virtual List<CloudTaskLog> CloudTaskLogs { get; set; } = new List<CloudTaskLog>();

    }

}
