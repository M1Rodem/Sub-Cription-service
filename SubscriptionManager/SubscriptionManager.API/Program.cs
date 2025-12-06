using Microsoft.EntityFrameworkCore;
using SubscriptionManager.API.Extensions;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Конфигурация сервисов
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureCors();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureServices();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

// Конфигурация пайплайна запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscription Manager API v1");
        c.RoutePrefix = "swagger";
    });
}

//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Создаём тестового админа при первом запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Применяем миграции
    context.Database.Migrate();

    // Создаём тестового админа если пользователей нет
    if (!context.AuthUsers.Any())
    {
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

        // Создаём тестового админа
        var adminUser = new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@subscription.com",
            Name = "Администратор",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };

        context.AuthUsers.Add(adminUser);
        await context.SaveChangesAsync();

        var token = jwtService.GenerateToken(adminUser);

        Console.WriteLine("🔑 Тестовый администратор создан!");
        Console.WriteLine($"📧 Email: {adminUser.Email}");
        Console.WriteLine($"🔐 Пароль: не требуется (JWT аутентификация)");
        Console.WriteLine($"🪪 Токен для тестирования: {token}");
    }
}



app.Run();