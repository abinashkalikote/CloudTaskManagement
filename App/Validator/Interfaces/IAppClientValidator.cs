using App.Model;

namespace App.Web.Validator.Interfaces;

public interface IAppClientValidator
{
    Task ValidateClientUniqueness(AppClient client, long? clientId = null);
}