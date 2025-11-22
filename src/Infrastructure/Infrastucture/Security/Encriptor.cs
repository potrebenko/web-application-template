using System.Security.Cryptography;

namespace Infrastructure.Security;

public class AesAesEncryptor: IAesEncryptor
{
    private const int IvSize = 16;
    private const int KeySize = 256;
    
    public string Encrypt(string plainText, string password)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Plain text cannot be empty", nameof(plainText));
            
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        var saltBytes = GenerateRandomBytes(32);
        var ivBytes = GenerateRandomBytes(IvSize);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        // Derive key from password
        using var key = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var keyBytes = key.GetBytes(KeySize / 8);  // Convert bits to bytes

        // Encrypt the data
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = 128;  // AES block size is always 128 bits
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = ivBytes;

        using var memoryStream = new MemoryStream();
        // Write the salt and IV to the beginning of the stream
        memoryStream.Write(saltBytes, 0, saltBytes.Length);
        memoryStream.Write(ivBytes, 0, ivBytes.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
        }

        var encryptedBytes = memoryStream.ToArray();
        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string encryptedText, string password)
    { 
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("Encrypted text cannot be empty", nameof(encryptedText));
            
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        var encryptedBytes = Convert.FromBase64String(encryptedText);
        
        if (encryptedBytes.Length < 32 + IvSize)
            throw new ArgumentException("Invalid encrypted text format", nameof(encryptedText));

        // Extract salt and IV
        var saltBytes = new byte[32];
        var ivBytes = new byte[IvSize];
        
        Buffer.BlockCopy(encryptedBytes, 0, saltBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(encryptedBytes, saltBytes.Length, ivBytes, 0, ivBytes.Length);

        // Derive key from password using same parameters
        using var key = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var keyBytes = key.GetBytes(KeySize / 8);

        // Setup decryption
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = ivBytes;

        var dataLength = encryptedBytes.Length - (saltBytes.Length + ivBytes.Length);
        var cipherTextBytes = new byte[dataLength];
        Buffer.BlockCopy(encryptedBytes, saltBytes.Length + ivBytes.Length, cipherTextBytes, 0, dataLength);

        // Decrypt the data
        using var memoryStream = new MemoryStream(cipherTextBytes);
        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream, Encoding.UTF8);
        
        return streamReader.ReadToEnd();
    }

    private byte[] GenerateRandomBytes(int length) => RandomNumberGenerator.GetBytes(length);
}