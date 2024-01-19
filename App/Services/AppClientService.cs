using System.Transactions;
using App.Base.DataContext.Interface;
using App.Model;
using App.Web.Dto;
using App.Web.Services.Interfaces;
using App.Web.Validator.Interfaces;

namespace App.Web.Services;

public class AppClientService : IAppClientService
{
    private readonly IUow _uow;
    private readonly IAppClientValidator _appClientValidator;

    public AppClientService(IUow uow, IAppClientValidator appClientValidator)
    {
        _uow = uow;
        _appClientValidator = appClientValidator;
    }

    public async Task<AppClient> Create(AppClientDto appClientDto)
    {
        using var tx = new TransactionScope((TransactionScopeAsyncFlowOption.Enabled));
        var client = new AppClient();
        client.ClientName = appClientDto.ClientName;
        client.Link = NormalizeLink(appClientDto.Link);
        client.Location = appClientDto.Location;
        client.CurrentVersion = appClientDto.CurrentVersion;
        client.IsMobileBankingClient = appClientDto.IsMobileBankingClient;

        await _appClientValidator.ValidateClientUniqueness(client);

        await _uow.CreateAsync(client);
        await _uow.CommitAsync();
        tx.Complete();
        return client;
    }

    public async Task Update(AppClient client, AppClientDto appClientDto)
    {
        using var tx = new TransactionScope((TransactionScopeAsyncFlowOption.Enabled));
        client.ClientName = appClientDto.ClientName;
        client.Link = NormalizeLink(appClientDto.Link);
        client.Location = appClientDto.Location;
        client.CurrentVersion = appClientDto.CurrentVersion;
        client.IsMobileBankingClient = appClientDto.IsMobileBankingClient;

        await _appClientValidator.ValidateClientUniqueness(client, client.Id);

        _uow.Update(client);
        await _uow.CommitAsync();
        tx.Complete();
    }

    private static string NormalizeLink(string link)
    {
        if (!Uri.TryCreate(link, UriKind.Absolute, out var finalUrl)) throw new Exception("Invalid URL provided");
        var x = $"{finalUrl.Scheme}://{finalUrl.Host}";

        if (finalUrl.Port > 0 || !finalUrl.IsDefaultPort)
        {
            x = x + ":" + finalUrl.Port;
        }

        return x;
    }
}