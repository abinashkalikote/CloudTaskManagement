using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class CloudTaskLog
    {
        public int Id { get; set; }

        public CloudTask CloudTask { get; set; }
        public int CloudTaskId { get; set; }

        public virtual User? User { get; set; }
        public int UserId { get; set; }

        public string CloudTaskStatus { get; set; }

        public DateTime RecDate { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
    }
}
