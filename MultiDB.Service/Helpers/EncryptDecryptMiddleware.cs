using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Service.Helpers
{
  public class EncryptDecryptMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly EncryptionHelper _encryptionHelper;
    private readonly IConfiguration _configuration;

    public EncryptDecryptMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      _next = next;
      _configuration = configuration;
     // _encryptionHelper = new EncryptionHelper(_configuration);
      _encryptionHelper = new EncryptionHelper();
    }
    public async Task InvokeAsync(HttpContext context)
    {
      if (context.Request.Path.Value == "/api/Clients/ClientDetails")
      {
        await _next(context);
        return;
      }
      var originalBody = context.Response.Body;
      using (var newBody = new MemoryStream())
      {
        context.Response.Body = newBody;
        await _next(context);
        if (context.Response.StatusCode == 200)
        {

          // Check if the current request should be excluded

          newBody.Seek(0, SeekOrigin.Begin);
          var responseData = await new StreamReader(newBody).ReadToEndAsync();

          if (!string.IsNullOrEmpty(responseData))
          {
            var responseObj = JsonConvert.DeserializeObject<dynamic>(responseData);
            var message = responseObj?.message;
            var formattedResponse = new
            {
              Message = responseObj?.message,
              StatusCode = responseObj?.statusCode,
              data = responseObj?.data != null
                    ? _encryptionHelper.Encrypt(responseObj.data.ToString())
                    : ""
            };

            var encryptedResponseJson = JsonConvert.SerializeObject(formattedResponse);
            var encryptedResponseBytes = Encoding.UTF8.GetBytes(encryptedResponseJson);

            await originalBody.WriteAsync(encryptedResponseBytes, 0, encryptedResponseBytes.Length);
          }
          else
          {
            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);
          }
        }
        else
        {
          newBody.Seek(0, SeekOrigin.Begin);
          await newBody.CopyToAsync(originalBody);
        }
      }
    }

  }
}

