namespace SubscriptionManager.Shared.DTOs;

public class AdminSubscriptionDto : SubscriptionDto
{
    public string? UserEmail { get; set; } // Для отображения в админке
    public string? ApplicationName { get; set; }
    public List<string> PlaceNames { get; set; } = new();
}

public class BlockSubscriptionDto
{
    public string? Reason { get; set; }
}

public class ChangeSubscriptionStatusDto
{
    public int Status { get; set; } // 0-Активная, 1-Отмененная, 2-Заблокированная
    public string? Reason { get; set; }
}