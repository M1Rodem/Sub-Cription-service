namespace SubscriptionManager.Core.Interfaces;

public interface IInvoiceGenerationService
{
    Task GenerateMonthlyInvoicesAsync(DateTime forDate);
}