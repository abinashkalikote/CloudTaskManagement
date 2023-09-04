using App.Web.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class TaskType
    {
        [Key]
        public int Id { get; set; }
        public string TaskTypeName { get; set; }
        public char? RecStatus { get; set; } = Status.Active;
        public DateTime RecDate { get; set; } = DateTime.Now;

        public virtual User RecBy { get; set; }
        public int RecById { get; set; }
    }
}
