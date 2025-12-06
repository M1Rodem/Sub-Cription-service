using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Core.Services;
using SubscriptionManager.Infrastructure.Data;
using SubscriptionManager.Infrastructure.Repositories;
using SubscriptionManager.Shared;
using System.Text;

namespace SubscriptionManager.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // Простейшая конфигурация Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Authentication:Jwt:Key"]!;
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Authentication:Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Authentication:Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        // Репозитории
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        // Сервисы
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IPlaceService, PlaceService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAdminSubscriptionService, AdminSubscriptionService>();
        services.AddScoped<IBffSubscriptionService, BffSubscriptionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        // Провайдеры
        services.AddSingleton<DateTimeProvider>(new DateTimeProvider());

        // Дополнительные сервисы
        services.AddScoped<IDodoISService, DodoISService>();
        services.AddScoped<IInvoiceGenerationService, InvoiceGenerationService>();
        services.AddScoped<IWorkingDaysService, WorkingDaysService>();

        // Регистрируем HttpClient для DodoIS API
        services.AddHttpClient<IDodoISService, DodoISService>((serviceProvider, client) =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Регистрируем Background Service
        //services.AddHostedService<MonthlyInvoiceBackgroundService>();
    }
}