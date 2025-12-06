using Microsoft.Extensions.Logging;
using SubscriptionManager.Core.Interfaces;
using System.Text.Json;

namespace SubscriptionManager.Core.Services;

public class WorkingDaysService : IWorkingDaysService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkingDaysService> _logger;

    public WorkingDaysService(HttpClient httpClient, ILogger<WorkingDaysService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public DateOnly AddWorkingDays(DateOnly startDate, int workingDays)
    {
        var currentDate = startDate;
        var daysAdded = 0;

        while (daysAdded < workingDays)
        {
            currentDate = currentDate.AddDays(1);
            if (IsWorkingDay(currentDate))
                daysAdded++;
        }

        return currentDate;
    }

    public bool IsWorkingDay(DateOnly date)
    {
        // Проверяем выходные
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        // TODO: Проверка праздников через внешний API
        // Пока считаем все будни рабочими
        return true;
    }

    public int GetWorkingDaysBetween(DateOnly startDate, DateOnly endDate)
    {
        var days = 0;
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            if (IsWorkingDay(currentDate))
                days++;
            currentDate = currentDate.AddDays(1);
        }

        return days;
    }

    // Метод для проверки праздников через API (если нужно)
    private async Task<bool> IsHolidayAsync(DateOnly date)
    {
        try
        {
            // Пример: использование API для проверки праздников
            // var response = await _httpClient.GetAsync($"https://api.example.com/holidays/{date.Year}");
            // var holidays = await response.Content.ReadFromJsonAsync<List<DateOnly>>();
            // return holidays?.Contains(date) ?? false;

            return false; // Заглушка
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке праздников");
            return false;
        }
    }
}