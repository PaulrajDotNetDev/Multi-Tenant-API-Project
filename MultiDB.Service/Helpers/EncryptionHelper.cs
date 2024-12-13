using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
  private readonly byte[] _key;
  private readonly byte[] _iv;

  public EncryptionHelper()
  {
    _key = Convert.FromBase64String("aKIXV3Knz+9X90+8pN5LD5D1+8o76E1gW3HDi+kgSGE=");
    _iv = Convert.FromBase64String("fXpRT6dX6OYh2u1PuyzOoA==");
  }  
  public string Encrypt(string plainText)
  {
    if (string.IsNullOrEmpty(plainText))
      throw new ArgumentNullException(nameof(plainText), "The plain text cannot be null or empty.");

    using var aes = Aes.Create();
    aes.Key = _key;
    aes.IV = _iv;

    using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
    using var ms = new MemoryStream();
    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
    using (var sw = new StreamWriter(cs))
    {
      sw.Write(plainText);
    }
    return Convert.ToBase64String(ms.ToArray());
  }
  
  public string Decrypt(string cipherText)
  {
    if (string.IsNullOrEmpty(cipherText))
      throw new ArgumentNullException(nameof(cipherText), "The cipher text cannot be null or empty.");

    using var aes = Aes.Create();
    aes.Key = _key;
    aes.IV = _iv;

    using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
    using var sr = new StreamReader(cs);
    {
      return sr.ReadToEnd();
    }
  }
}
