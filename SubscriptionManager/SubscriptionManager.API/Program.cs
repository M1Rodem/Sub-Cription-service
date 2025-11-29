using SubscriptionManager.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureCors();

builder.Services.AddControllers();

var app = builder.Build();

// Конфигурация пайплайна запросов
app.UseSwaggerDocumentation(); // Используем наш метод

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();