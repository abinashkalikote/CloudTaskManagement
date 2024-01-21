using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Base.Entities
{
    [Table("TaskTypes")]
    public class TaskType
    {
        [Key]
        public int Id { get; set; }
        public string TaskTypeName { get; set; }
        public char? RecStatus { get; set; } = 'A';
        public DateTime RecDate { get; set; } = DateTime.Now;

        public virtual User RecBy { get; set; }
        public int RecById { get; set; }
    }
}
