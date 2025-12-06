namespace SubscriptionManager.Shared.DTOs;

public class AddPlacesToSubscriptionDto
{
    public List<Guid> PlaceIds { get; set; } = new();
}

public class RemovePlaceFromSubscriptionDto
{
    public Guid PlaceId { get; set; }
}

public class RestoreSubscriptionDto
{
    public List<Guid> PlaceIds { get; set; } = new(); // Пиццерии для восстановления
}