//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using SubscriptionManager.Core.Interfaces;

//namespace SubscriptionManager.Infrastructure.BackgroundServices;

//public class MonthlyInvoiceBackgroundService : BackgroundService
//{
//    private readonly ILogger<MonthlyInvoiceBackgroundService> _logger;
//    private readonly IServiceProvider _serviceProvider;
//    private Timer? _timer;

//    public MonthlyInvoiceBackgroundService(
//        ILogger<MonthlyInvoiceBackgroundService> logger,
//        IServiceProvider serviceProvider)
//    {
//        _logger = logger;
//        _serviceProvider = serviceProvider;
//    }

//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Фоновый сервис для генерации счетов запущен");

//        // Планируем первый запуск через 1 минуту для тестирования
//        _timer = new Timer(RunInvoiceGeneration, null, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(-1));

//        return Task.CompletedTask;
//    }

//    private async void RunInvoiceGeneration(object? state)
//    {
//        try
//        {
//            _logger.LogInformation("💰 Начинаем тестовую генерацию счетов");

//            using var scope = _serviceProvider.CreateScope();
//            var invoiceGenerationService = scope.ServiceProvider.GetRequiredService<IInvoiceGenerationService>();

//            // Тестовая генерация за предыдущий месяц
//            var previousMonth = DateTime.UtcNow.AddMonths(-1);
//            await invoiceGenerationService.GenerateMonthlyInvoicesAsync(previousMonth);

//            _logger.LogInformation("Тестовая генерация счетов завершена");

//            // Планируем следующий запуск на 1-е число в 02:00
//            ScheduleMonthlyRun();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Ошибка при генерации счетов");
//        }
//    }

//    private void ScheduleMonthlyRun()
//    {
//        try
//        {
//            // Вычисляем время до следующего 1-го числа месяца в 02:00
//            var now = DateTime.Now;
//            var nextRun = new DateTime(now.Year, now.Month, 1, 2, 0, 0).AddMonths(1);

//            // Если сегодня уже после 02:00 1-го числа, запускаем в следующем месяце
//            if (now.Day == 1 && now.Hour >= 2)
//            {
//                nextRun = nextRun.AddMonths(-1);
//            }

//            var delay = nextRun - now;

//            _timer?.Dispose();
//            _timer = new Timer(_ => RunInvoiceGeneration(null), null, delay, TimeSpan.FromMilliseconds(-1));
//            _logger.LogInformation($"Следующий запуск генерации счетов запланирован на: {nextRun}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Ошибка при планировании следующего запуска");
//        }
//    }

//    public override Task StopAsync(CancellationToken stoppingToken)
//    {
//        _timer?.Dispose();
//        _logger.LogInformation("Фоновый сервис для генерации счетов остановлен");
//        return base.StopAsync(stoppingToken);
//    }
//}