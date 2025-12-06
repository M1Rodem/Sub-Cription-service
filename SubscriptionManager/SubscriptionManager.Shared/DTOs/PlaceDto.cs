namespace SubscriptionManager.Shared.DTOs;

public class PlaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreatePlaceDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdatePlaceDto
{
    public string? Name { get; set; }
}