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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register WeatherService
builder.Services.AddHttpClient<WeatherService>();

// MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

//  Supabase client as singleton
builder.Services.RegisterSupabaseClient(builder.Configuration); // Ensure correct namespace

// Register Services
builder.Services.AddScoped<UserHistoryService>();
builder.Services.AddScoped<FavoriteCityService>();

//Enable CORS 
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


//  custom services
builder.Services.AddScoped<JwtService>();

//  JWT Authentication
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

//  HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//  HTTPS redirection happens before authentication
app.UseHttpsRedirection();

//  custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

//  authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

//  CORS policy
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
