using Apitransac.Common;
using Apitransac.Data;
using Apitransac.Middleware;
using Apitransac.Models.Configuration;
using Apitransac.Services.Auth;
using Apitransac.Services.Jwt;
using Apitransac.Services.Password;
using Apitransac.Services.RefreshTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy =
        JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings =
            builder.Configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()!;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                ClockSkew = TimeSpan.Zero
            };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                var response =
                    new ApiResponse<object>
                    {
                        IsSuccess = false,
                        StatusCode =
                            StatusCodes.Status401Unauthorized,
                        Data = null,
                        Errors =
                        [
                            new ApiError
                            {
                                Code = "UNAUTHORIZED",
                                Message =
                                    "Se requiere autenticación para acceder a este recurso."
                            }
                        ]
                    };

                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                     response);
            },

            OnForbidden = async context =>
            {
                var response =
                    new ApiResponse<object>
                    {
                        IsSuccess = false,
                        StatusCode =
                            StatusCodes.Status403Forbidden,
                        Data = null,
                        Errors =
                        [
                            new ApiError
                            {
                                Code = "FORBIDDEN",
                                Message =
                                    "No tienes permisos para acceder a este recurso."
                            }
                        ]
                    };

                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    response);
            }
        };
    });


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(error => new ApiError
            {
                Code = "VALIDATION_ERROR",
                Message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "El valor proporcionado no es válido."
                    : error.ErrorMessage
            })
            .ToList();

        var response = new ApiResponse<object>
        {
            IsSuccess = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Data = null,
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };
});


builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services.AddScoped<IAuthService, AuthService>();

// JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Refresh Token
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NewPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/*
migrations models to database
dotnet ef migrations add InitialMigration
dotnet ef database update

sqlcmd -S localhost\\SQLEXPRESS -E
USE NOMBRE_BASEDATA
go
 * 
 */