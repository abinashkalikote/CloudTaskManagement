namespace App.Web.Exceptions.AppClientExceptions;

public class DuplicateClientNameException : Exception
{
    public DuplicateClientNameException(string name, string? message = null) : base(GenerateMessage(name, message))
    {
    }

    private static string GenerateMessage(string name, string? message)
        => message ?? "Duplicate client name: " + name;
}