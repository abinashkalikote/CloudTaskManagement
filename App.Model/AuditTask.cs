﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class AuditTask
    {
        public int Id { get; set; }

        public virtual CloudTask? Task { get; set; }
        public int TaskId { get; set; }

        public string AuditBy { get; set; }
        public string Remarks { get; set; }
        public DateTime RecDate { get; set; }


        public virtual User RecBy { get; set; }
        public int RecById { get; set; }
    }
}
