namespace SubscriptionManager.Shared.DTOs;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CreateApplicationDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class UpdateApplicationDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
}