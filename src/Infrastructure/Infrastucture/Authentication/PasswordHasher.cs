using System.Security.Cryptography;
using Application.Abstractions.Authentication;

namespace Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 5000000;
    private const string HashSeparator = "_";
    
    private static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA512;
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, AlgorithmName, HashSize);
            
        return $"{Convert.ToHexString(salt)}{HashSeparator}{Convert.ToHexString(hash)}";
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var parts = hashedPassword.Split(HashSeparator);
        var hash = Convert.FromHexString(parts[1]);
        var salt = Convert.FromHexString(parts[0]);
        var providedHash = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, Iterations, AlgorithmName, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, providedHash);
    }
}