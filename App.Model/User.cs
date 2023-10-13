using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public char IsAdmin { get; set; } = 'N';
        public char IsNewPassword { get; set; } = 'N';
        public char? RecStatus { get; set; } = 'A';
        public DateTime RecDate { get; set; } = DateTime.Now;
        public string RecBy { get; set; }

    }
}
