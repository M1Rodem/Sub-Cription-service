using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Infrastructure.Data;

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
        services.AddOpenApi(); // В .NET 9 это всё что нужно
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }
}