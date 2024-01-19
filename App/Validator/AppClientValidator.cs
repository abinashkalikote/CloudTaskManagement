using App.Model;
using App.Web.Exceptions.AppClientExceptions;
using App.Web.Repository.Interfaces;
using App.Web.Validator.Interfaces;

namespace App.Web.Validator;

public class AppClientValidator : IAppClientValidator
{
    private readonly IAppClientRepo _clientRepo;

    public AppClientValidator(IAppClientRepo clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task ValidateClientUniqueness(AppClient client, long? clientId = null)
    {
        if (await _clientRepo.CheckIfExistAsync(x =>
                x.Id != clientId && x.ClientName.ToLower() == client.ClientName.ToLower()))
        {
            throw new DuplicateClientNameException(client.ClientName);
        }

        if (await _clientRepo.CheckIfExistAsync(x =>
                x.Id != clientId && x.Link.ToLower() == client.Link.ToLower()))
        {
            throw new DuplicateClientUrlException(client.Link);
        }
    }
}