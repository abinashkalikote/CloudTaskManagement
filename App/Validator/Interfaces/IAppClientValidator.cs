using App.Base.Entities;

namespace App.Web.Validator.Interfaces;

public interface IAppClientValidator
{
    Task ValidateClientUniqueness(AppClient client, long? clientId = null);
}