using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Base.Constants
{
    public static class CloudTaskStatus
    {

        public const string Pending = "Pending";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Canceled = "Canceled";

        public static List<string> TaskStatusList = new()
        {
            Pending,
            InProgress,
            Completed,
            Canceled
        };
    }
}
