namespace SubscriptionManager.Shared.DTOs;

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid PlaceId { get; set; }
    public decimal Price { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreateAt { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
}

public class CreateInvoiceDto
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<CreateInvoiceItemDto> InvoiceItems { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public Guid ApplicationId { get; set; }
    public Guid PlaceId { get; set; }
    public decimal Price { get; set; }
}

public class UpdateInvoiceDto
{
    public List<CreateInvoiceItemDto>? InvoiceItems { get; set; }
}