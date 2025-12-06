using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class BffSubscriptionService : IBffSubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IHistoryRepository _historyRepository;
    private readonly DateTimeProvider _dateTimeProvider;

    public BffSubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IPlaceRepository placeRepository,
        IApplicationRepository applicationRepository,
        IHistoryRepository historyRepository,
        DateTimeProvider dateTimeProvider)
    {
        _subscriptionRepository = subscriptionRepository;
        _placeRepository = placeRepository;
        _applicationRepository = applicationRepository;
        _historyRepository = historyRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IEnumerable<BffSubscriptionDto>> GetBffSubscriptionsAsync(Guid userId)
    {
        var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
        var result = new List<BffSubscriptionDto>();

        foreach (var subscription in subscriptions)
        {
            var dto = await MapToBffDto(subscription);
            result.Add(dto);
        }

        return result;
    }

    public async Task<BffSubscriptionDto?> GetBffSubscriptionByIdAsync(Guid id, Guid userId)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);

        // Проверяем что подписка принадлежит пользователю
        if (subscription == null || subscription.UserId != userId)
            return null;

        return await MapToBffDto(subscription);
    }

    private async Task<BffSubscriptionDto> MapToBffDto(Subscription subscription)
    {
        // Получаем информацию о приложении
        var application = await _applicationRepository.GetByIdAsync(subscription.ApplicationId);

        // Получаем информацию о пиццериях
        var placeIds = subscription.SubscriptionItems.Select(si => si.PlaceId).ToList();
        var places = new List<Place>();
        foreach (var placeId in placeIds)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);
            if (place != null) places.Add(place);
        }

        // Получаем историю периодов для каждой пиццерии
        var placePeriods = new Dictionary<Guid, List<SubscriptionPeriodDto>>();
        foreach (var placeId in placeIds)
        {
            // Здесь нужно будет получить историю периодов для конкретной пиццерии
            // Пока создадим заглушку
            placePeriods[placeId] = new List<SubscriptionPeriodDto>
            {
                new SubscriptionPeriodDto
                {
                    Start = DateOnly.FromDateTime(subscription.Start),
                    End = subscription.Status == 1 ? DateOnly.FromDateTime(subscription.End) : null
                }
            };
        }

        // Расчёт стоимости (по твоей логике)
        // Цена за день = ApplicationPrice / 30 (если цена месячная)
        decimal pricePerDay = application?.Price / 30 ?? 0;

        // Расчёт дней использования по периодам
        int totalDays = 0;
        foreach (var periods in placePeriods.Values)
        {
            foreach (var period in periods)
            {
                if (period.End.HasValue)
                {
                    totalDays += period.Days;
                }
                else
                {
                    // Активный период - считаем до сегодня
                    totalDays += (DateOnly.FromDateTime(_dateTimeProvider.UtcNow).DayNumber - period.Start.DayNumber);
                }
            }
        }

        return new BffSubscriptionDto
        {
            Id = subscription.Id,
            Start = subscription.Start,
            End = subscription.End,
            Status = subscription.Status,
            UserId = subscription.UserId,
            ApplicationId = subscription.ApplicationId,
            ApplicationName = application?.Name ?? "Неизвестное приложение",
            ApplicationPrice = application?.Price ?? 0,
            PlaceIds = placeIds,
            PlaceNames = places.Select(p => p.Name).ToList(),
            PlacePeriods = placePeriods,
            PricePerPeriod = pricePerDay,
            TotalPrice = pricePerDay * totalDays
        };
    }
}