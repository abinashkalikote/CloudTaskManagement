using System.ComponentModel.DataAnnotations.Schema;

namespace App.Base.Entities;

[Table("app_client")]
public class AppClient
{
    public long Id { get; set; }

    public string ClientName { get; set; }

    public string Link { get; set; }

    public string? Location { get; set; }

    public string? CurrentVersion { get; set; }

    public bool IsMobileBankingClient { get; set; }
}