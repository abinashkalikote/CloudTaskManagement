using System.ComponentModel.DataAnnotations.Schema;
using App.Base.Entities;

namespace App.CloudTask.Entity
{
    [Table("CloudTaskLog")]
    public class CloudTaskLog
    {
        public int Id { get; set; }

        public App.CloudTask.Entity.CloudTask CloudTask { get; set; }
        public int CloudTaskId { get; set; }

        public virtual User? User { get; set; }
        public int UserId { get; set; }

        public string CloudTaskStatus { get; set; }

        public DateTime RecDate { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
    }
}
