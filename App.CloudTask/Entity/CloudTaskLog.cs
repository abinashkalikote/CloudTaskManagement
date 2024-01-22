using System.ComponentModel.DataAnnotations.Schema;
using App.Base.Entities;

namespace App.CloudTask.Entity
{
    [Table("CloudTasksLog")]
    public class CloudTaskLog
    {
        public int Id { get; set; }

        public virtual CloudTaskLog? CloudTask { get; set; }
        public int CloudTaskId { get; init; }

        public virtual User? User { get; set; }
        public int UserId { get; set; }

        public string CloudTaskStatus { get; set; }

        public DateTime RecDate { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
    }
}
