namespace SubscriptionManager.Shared.DTOs;

public class SubscriptionPeriodDto
{
    public DateOnly Start { get; set; }
    public DateOnly? End { get; set; }
    public int Days => End.HasValue
        ? (End.Value.DayNumber - Start.DayNumber)
        : (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - Start.DayNumber);
}

public class BffSubscriptionDto
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Status { get; set; }
    public Guid UserId { get; set; }

    // Информация о приложении
    public Guid ApplicationId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public decimal ApplicationPrice { get; set; } // Цена за день/месяц

    // Пиццерии в подписке
    public List<Guid> PlaceIds { get; set; } = new();
    public List<string> PlaceNames { get; set; } = new();

    // История периодов по каждой пиццерии
    public Dictionary<Guid, List<SubscriptionPeriodDto>> PlacePeriods { get; set; } = new();

    // Расчёт стоимости
    public decimal TotalPrice { get; set; }
    public decimal PricePerPeriod { get; set; } // Цена за период (например, день)

    // Информация для отображения
    public int ActivePlacesCount => PlaceIds.Count;
    public bool IsActive => Status == 0;
}