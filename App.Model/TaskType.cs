using System.ComponentModel.DataAnnotations;

namespace App.Model
{
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
