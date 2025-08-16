using System.Security.Cryptography;
using System.Text;

namespace Framework.Infrastructure.Persistence.Helpers;



public static class EncryptionHelper
{
    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using var aes = Aes.Create();
        aes.Key = GetAesKey();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        aes.GenerateIV();
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();

        ms.Write(iv, 0, iv.Length);
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        var encryptedBytes = ms.ToArray();
        return Convert.ToBase64String(encryptedBytes);
    }


    public static string Decrypt(string encryptedBase64)
    {
        if (string.IsNullOrEmpty(encryptedBase64))
            return encryptedBase64;

        var encryptedBytes = Convert.FromBase64String(encryptedBase64);

        using var aes = Aes.Create();
        aes.Key = GetAesKey();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        
        var iv = new byte[16];
        Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        using var ms = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);

        return sr.ReadToEnd();
    }

    
    private static byte[] GetAesKey()
    {
        var secretKey = EncryptionKeyHolder.SecretKey;

       
        var keyBytes = new byte[32];
        var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException());
        Array.Copy(secretKeyBytes, keyBytes,
            secretKeyBytes.Length >= keyBytes.Length ? keyBytes.Length : secretKeyBytes.Length);
        return keyBytes;
    }
}


public static class EncryptionKeyHolder
{
    public static string? SecretKey { get; private set; }

    public static void Initialize(string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey))
            throw new Exception("Secret key not provided");
        SecretKey = secretKey;
    }
}