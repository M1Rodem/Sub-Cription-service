using Microsoft.Extensions.Logging;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class InvoiceGenerationService : IInvoiceGenerationService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IHistoryRepository _historyRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ILogger<InvoiceGenerationService> _logger;
    private readonly IWorkingDaysService _workingDaysService;

    public InvoiceGenerationService(
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        IHistoryRepository historyRepository,
        IApplicationRepository applicationRepository,
        ILogger<InvoiceGenerationService> logger,
        IWorkingDaysService workingDaysService)
    {
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _historyRepository = historyRepository;
        _applicationRepository = applicationRepository;
        _logger = logger;
        _workingDaysService = workingDaysService;
    }

    public async Task GenerateMonthlyInvoicesAsync(DateTime forDate)
    {
        _logger.LogInformation($"Начинаем генерацию счетов за {forDate:MMMM yyyy}");

        var year = forDate.Year;
        var month = forDate.Month;

        // Находим все активные подписки на конец месяца
        var subscriptions = await _subscriptionRepository.GetAllAsync();

        foreach (var subscription in subscriptions)
        {
            // Проверяем, был ли уже сгенерирован счёт за этот месяц
            var existingInvoice = await GetExistingInvoiceAsync(subscription.UserId, year, month);
            if (existingInvoice != null)
            {
                _logger.LogDebug($"Счёт для пользователя {subscription.UserId} за {month}/{year} уже существует");
                continue;
            }

            // Расчёт периода: конец месяца + 6 рабочих дней
            var periodEnd = CalculateBillingPeriodEnd(forDate);

            // Проверяем, действовала ли подписка в расчётном периоде
            if (!IsSubscriptionActiveInPeriod(subscription, forDate, periodEnd))
                continue;

            // Создаём счёт
            await GenerateInvoiceForSubscriptionAsync(subscription, year, month);
        }

        _logger.LogInformation($"Генерация счетов за {forDate:MMMM yyyy} завершена");
    }

    private async Task<Invoice?> GetExistingInvoiceAsync(Guid userId, int year, int month)
    {
        var userInvoices = await _invoiceRepository.GetByUserIdAsync(userId);
        return userInvoices.FirstOrDefault(i => i.Year == year && i.Month == month);
    }

    private DateOnly CalculateBillingPeriodEnd(DateTime forDate)
    {
        // Конец месяца
        var endOfMonth = new DateOnly(forDate.Year, forDate.Month,
            DateTime.DaysInMonth(forDate.Year, forDate.Month));

        // Добавляем 6 рабочих дней
        return _workingDaysService.AddWorkingDays(endOfMonth, 6);
    }

    private bool IsSubscriptionActiveInPeriod(Subscription subscription, DateTime periodStart, DateOnly periodEnd)
    {
        // Подписка должна быть активна в расчётном периоде
        if (subscription.Status != 0) // Не активна
            return false;

        // Проверяем пересечение периодов
        var subscriptionEndDate = DateOnly.FromDateTime(subscription.End);
        var periodStartDate = DateOnly.FromDateTime(periodStart);

        return subscriptionEndDate >= periodStartDate &&
               DateOnly.FromDateTime(subscription.Start) <= periodEnd;
    }

    private async Task GenerateInvoiceForSubscriptionAsync(Subscription subscription, int year, int month)
    {
        // Получаем историю активности пиццерий за месяц
        var startOfMonth = new DateOnly(year, month, 1);
        var endOfMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        var invoiceItems = new List<InvoiceItem>();
        var totalAmount = 0m;

        // Для каждой пиццерии в подписке
        foreach (var subscriptionItem in subscription.SubscriptionItems)
        {
            // Получаем периоды активности пиццерии в этом месяце
            var periods = await GetActivePeriodsInMonthAsync(
                subscription.Id, subscriptionItem.PlaceId, year, month);

            if (periods.Count == 0)
                continue;

            // Рассчитываем количество дней активности
            var activeDays = CalculateActiveDays(periods, startOfMonth, endOfMonth);

            // Получаем цену приложения
            var application = await _applicationRepository.GetByIdAsync(subscription.ApplicationId);
            if (application == null) continue;

            // Рассчитываем стоимость (цена за день * количество дней)
            var dailyPrice = application.Price / 30; // Предполагаем месячную цену
            var itemPrice = dailyPrice * activeDays;

            invoiceItems.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                ApplicationId = subscription.ApplicationId,
                PlaceId = subscriptionItem.PlaceId,
                Price = itemPrice
            });

            totalAmount += itemPrice;
        }

        if (invoiceItems.Count == 0)
        {
            _logger.LogDebug($"Нет активных периодов для подписки {subscription.Id} за {month}/{year}");
            return;
        }

        // Создаём счёт
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = subscription.UserId,
            CreateAt = DateTime.UtcNow,
            Year = year,
            Month = month,
            InvoiceItems = invoiceItems
        };

        await _invoiceRepository.AddAsync(invoice);
        _logger.LogInformation($"Создан счёт {invoice.Id} для пользователя {subscription.UserId} на сумму {totalAmount:C}");
    }

    private async Task<List<(DateOnly Start, DateOnly? End)>> GetActivePeriodsInMonthAsync(
    Guid subscriptionId, Guid placeId, int year, int month)
    {
        var startOfMonth = new DateOnly(year, month, 1);
        var endOfMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        // Получаем все периоды для этой пиццерии
        var histories = await _historyRepository.GetBySubscriptionAndPlaceAsync(subscriptionId, placeId);

        var periodsInMonth = new List<(DateOnly Start, DateOnly? End)>();

        foreach (var history in histories)
        {
            // Проверяем пересечение с месяцем
            if (history.Start <= endOfMonth && (!history.End.HasValue || history.End.Value >= startOfMonth))
            {
                var periodStart = history.Start > startOfMonth ? history.Start : startOfMonth;
                var periodEnd = history.End.HasValue
                    ? (history.End.Value < endOfMonth ? history.End.Value : endOfMonth)
                    : endOfMonth;

                periodsInMonth.Add((periodStart, periodEnd));
            }
        }

        return periodsInMonth;
    }

    private int CalculateActiveDays(List<(DateOnly Start, DateOnly? End)> periods, DateOnly monthStart, DateOnly monthEnd)
    {
        var totalDays = 0;

        foreach (var period in periods)
        {
            var start = period.Start > monthStart ? period.Start : monthStart;
            var end = period.End.HasValue
                ? (period.End.Value < monthEnd ? period.End.Value : monthEnd)
                : monthEnd;

            if (start <= end)
            {
                totalDays += (end.DayNumber - start.DayNumber) + 1;
            }
        }

        return totalDays;
    }
}