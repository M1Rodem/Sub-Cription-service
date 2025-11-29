namespace SubscriptionManager.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); // Это для .NET 9
        }
    }
}