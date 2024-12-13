using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MultiDB.Service.Helpers
{
  public class JwtTokenService
  {
    // Method to generate a JWT token
    public static string GenerateToken(string clientId, IConfiguration configuration, int expiryMinutes = 60)
    {
      // Read keys from configuration
      var secretKey = configuration["Jwt:Key"];
      var issuer = configuration["Jwt:Issuer"];
      var audience = configuration["Jwt:Audience"];

      // Convert secret key to bytes
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Create claims
      var claims = new[]
      {
            new Claim("client_id", clientId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

      // Create the token
      var token = new JwtSecurityToken(
          issuer: issuer,
          audience: audience,
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
          signingCredentials: credentials
      );

      // Serialize the token to a string
      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Method to generate a secure random secret key
    public static byte[] GenerateSecretKey()
    {
      using (var rng = RandomNumberGenerator.Create())
      {
        var key = new byte[32]; // 256 bits
        rng.GetBytes(key);
        return key;
      }
    }
  }
}
