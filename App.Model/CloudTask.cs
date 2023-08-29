using App.Base.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class CloudTask
    {
        [Key]
        public int Id { get; set; }
        public string TaskName { get; set; }
        public int TaskTypeId { get; set; }

        [ForeignKey("TaskTypeId")]
        public virtual TaskType TaskType { get; set; }

        public string ClientName { get; set; }
        public int? Priority { get; set; } = 0;
        public DateTime TaskTime { get; set; }
        public string? CloudUrl { get; set; }
        public string IssueOnPreviousSoftware { get; set; }
        public string SoftwareVersionFrom { get; set; }
        public string SoftwareVersionTo { get; set; }

        public string? Remarks { get; set; }

        public string RecAuditLog { get; set; } = "";
        public int RecVersion { get; set; } = 1;
        public char? RecStatus { get; set; } = Status.Active;
        public DateTime RecDate { get; set; } = DateTime.Now;


        public int RecBy { get; set; }
        [ForeignKey("RecBy")]
        public virtual User RecordedBy { get; set; }

        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; }

        public int? ProccedBy { get; set; }
        [ForeignKey("ProccedBy")]
        public virtual User ProcessedBy { get; set; }

        public int? CompletedBy { get; set; }
        [ForeignKey("CompletedBy")]
        public virtual User CompletedByUser { get; set; }

    }


}
