using App.Model;
using App.Web.Dto;

namespace App.Web.Services.Interfaces;

public interface IAppClientService
{
    Task<AppClient> Create(AppClientDto appClientDto);
    Task Update(AppClient client, AppClientDto appClientDto);
}