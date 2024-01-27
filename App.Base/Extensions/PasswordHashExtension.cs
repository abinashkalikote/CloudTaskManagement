namespace App.Base.Extensions;

public static class PasswordHashExtension
{
    public static string Hash(this string text)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(text);
    }
}