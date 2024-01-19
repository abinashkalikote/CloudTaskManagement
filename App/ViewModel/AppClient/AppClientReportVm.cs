using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace App.Web.ViewModel.AppClient
{
    public class AppClientReportVm : BaseFilterVm
    {
        public string? CurrentVersion { get; set; } = string.Empty;

        [DisplayName("FinSmart Client ?")]
        public string? IsMobileBankingClient { get; set; } = string.Empty;
        
        public List<AppClientReport> AppClientReport { get; set; } = new List<AppClientReport>();



        public List<string> SoftwareVersions;
        public SelectList GetSoftwareVersionList() => new SelectList(SoftwareVersions, CurrentVersion);
    }

    public class AppClientReport
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public string Link { get; set; }
        public string Location { get; set; }
        public string CurrentVersion { get; set; }
        public bool IsMobileBankingClient { get; set; }
    }
}
