using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class JwtMiddleware
{
  private readonly RequestDelegate _next;
  private readonly string _secretKey;
  private readonly string _issuer;
  private readonly string _audience;

  public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
  {
    _next = next;
    _secretKey = configuration["Jwt:Key"];
    _issuer = configuration["Jwt:Issuer"];
    _audience = configuration["Jwt:Audience"];
  }

  public async Task Invoke(HttpContext context)
  {
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (!string.IsNullOrEmpty(token))
    {
      try
       {
        var claimsPrincipal = ValidateJwtToken(token);

        if (claimsPrincipal != null)
        {
          context.User = claimsPrincipal;
        }
      }
      catch (Exception ex)
      {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync($"Unauthorized: {ex.Message}");
        return;
      }
    }

    await _next(context);
  }

  private ClaimsPrincipal ValidateJwtToken(string token)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_secretKey);

    var tokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(key),
      ValidateIssuer = true,
      ValidIssuer = _issuer,
      ValidateAudience = true,
      ValidAudience = _audience,
      ValidateLifetime = true
    };

    SecurityToken validatedToken;
    return tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
  }
}
