// HotelManagement.Infrastructure/Services/PasswordHasher.cs
using System.Security.Cryptography;
using System.Text;
using HotelManagement.Application.Abstractions;

namespace HotelManagement.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool Verify(string password, string hash)
    {
        var hashedPassword = Hash(password);
        return hashedPassword == hash;
    }
}