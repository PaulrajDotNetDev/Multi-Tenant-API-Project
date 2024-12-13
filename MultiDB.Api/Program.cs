using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MultiDB.Models.CommonModel;
using MultiDB.Service;
using MultiDB.Service.Helpers;
using MultiDB.Service.Interface;
using MultiDB.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
 builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Response>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClientServices, ClientServices>();
builder.Services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],       
    ValidAudience = builder.Configuration["Jwt:Audience"],   
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  
  };
});

builder.Services.AddAuthorization();

  var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
  
app.UseMiddleware<JwtMiddleware>();
// app.UseMiddleware<EncryptDecryptMiddleware>();
app.UseMiddleware<ErrorLoggingMiddleware>();


app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

  app.Run();
