using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Base.Constants
{
    public static class CloudTaskStatus
    {

        public static string Pending { get; set; } = "Pending";
        public static string InProgress { get; set; } = "InProgress";
        public static string Completed { get; set; } = "Completed";
        public static string Canceled { get; set; } = "Canceled";

        public static List<TStatus> TaskStatusList = new()
        {
            new TStatus { TsStatus = "Pending"},
            new TStatus { TsStatus = "InProgress"},
            new TStatus { TsStatus = "Completed"},
            new TStatus { TsStatus = "Canceled"},
        };
    }

    public class TStatus
    {
        public string TsStatus { get; set; }
    }
}
