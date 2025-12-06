public class AuthUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // OAuth провайдеры
    public string? GoogleId { get; set; }
    public string? YandexId { get; set; }
    public string? DodoISId { get; set; }
    public string? DodoISUnitId { get; set; } // ID пиццерии в DodoIS
    public string? DodoISAccessToken { get; set; }
    public string? DodoISRefreshToken { get; set; }
    public DateTime? DodoISTokenExpires { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public string Role { get; set; } = "Customer"; // Customer, Admin, PizzaManager
}