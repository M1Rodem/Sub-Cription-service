namespace SubscriptionManager.Core.Interfaces;

public interface IWorkingDaysService
{
    DateOnly AddWorkingDays(DateOnly startDate, int workingDays);
    bool IsWorkingDay(DateOnly date);
    int GetWorkingDaysBetween(DateOnly startDate, DateOnly endDate);
}