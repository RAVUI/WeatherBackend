using System.Net;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using WeatherBackend.Service;
using WeatherBackend.Configurations;
using WeatherBackend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherBackend.Extensions;
using WeatherBackend.Middleware;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Versioning; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddScoped<UserHistoryService>();
builder.Services.AddScoped<FavoriteCityService>();
builder.Services.AddScoped<JwtService>();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.RegisterSupabaseClient(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    var apiSettings = builder.Configuration
        .GetSection("ApiSettings")
        .Get<ApiSettings>();

    options.DefaultApiVersion = ApiVersion.Parse(apiSettings.DefaultVersion);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // Use only URL path for versioning
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); 
})
.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddScoped<JwtService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["JWT:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection happens before authentication
app.UseHttpsRedirection();

// custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// CORS policy
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();

public class ApiSettings
{
    public string Version { get; set; }
    public string DefaultVersion { get; set; }
    public string[] SupportedVersions { get; set; }
}