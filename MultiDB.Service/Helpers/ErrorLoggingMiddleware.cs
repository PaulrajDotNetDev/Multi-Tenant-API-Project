using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
public class ErrorLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ErrorLoggingMiddleware> _logger;

  public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context); // Call the next middleware in the pipeline
    }
    catch (Exception ex)
    {
      // Log using ILogger
      _logger.LogError(ex, "An unexpected error occurred");

      // Log to a file directly
      LogErrorToFile(ex, context);

      // Set the response to indicate error
      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";

      // Return a generic error message to the client
      await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "An unexpected error occurred" }));
    }
  }

  private void LogErrorToFile(Exception ex, HttpContext context)
  {
    try
    {
      // Define the log file path
     

      string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"middleware-errors_{DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}.log");



      // Ensure the directory exists
      Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

      // Append error details to the file
      using (StreamWriter writer = new StreamWriter(logFilePath, true))
      {
        writer.WriteLine($"Timestamp: {DateTime.UtcNow}");
        writer.WriteLine($"Request Path: {context.Request.Path}");
        writer.WriteLine($"Query String: {context.Request.QueryString}");
        writer.WriteLine($"Exception Message: {ex.Message}");
        writer.WriteLine($"Stack Trace: {ex.StackTrace}");
        writer.WriteLine(new string('-', 80));
      }
    }
    catch (Exception fileEx)
    {
      // Log any errors during file logging (to avoid suppressing primary errors)
      _logger.LogError(fileEx, "Failed to log error to file");
    }
  }
}
