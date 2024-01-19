namespace App.Web.Dto;

public class AppClientDto
{
    public string ClientName { get; set; }

    public string Link { get; set; }

    public string? Location { get; set; }

    public string? CurrentVersion { get; set; }

    public bool IsMobileBankingClient { get; set; }
}