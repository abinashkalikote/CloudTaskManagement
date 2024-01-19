namespace App.Web.Exceptions.AppClientExceptions;

public class DuplicateClientUrlException : Exception
{
    public DuplicateClientUrlException(string url, string? message = null) : base(GenerateMessage(url, message))
    {
    }

    private static string GenerateMessage(string url, string? message)
        => message ?? "Duplicate client URL: " + url;
}