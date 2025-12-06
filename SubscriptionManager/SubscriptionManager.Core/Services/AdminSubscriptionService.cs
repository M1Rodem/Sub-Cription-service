using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class AdminSubscriptionService : IAdminSubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IApplicationRepository _applicationRepository;

    public AdminSubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IPlaceRepository placeRepository,
        IApplicationRepository applicationRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _placeRepository = placeRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<IEnumerable<AdminSubscriptionDto>> GetAllSubscriptionsAdminAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        var result = new List<AdminSubscriptionDto>();

        foreach (var subscription in subscriptions)
        {
            var dto = await MapToAdminDto(subscription);
            result.Add(dto);
        }

        return result;
    }

    public async Task<AdminSubscriptionDto?> GetSubscriptionByIdAdminAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        return subscription != null ? await MapToAdminDto(subscription) : null;
    }

    public async Task<bool> BlockSubscriptionAsync(Guid id, BlockSubscriptionDto blockDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null) return false;

        subscription.Status = 2; // Заблокированная
        // Можно добавить поле Reason в Subscription entity если нужно

        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    public async Task<bool> UnblockSubscriptionAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null) return false;

        subscription.Status = 0; // Активная

        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    public async Task<bool> ChangeSubscriptionStatusAsync(Guid id, ChangeSubscriptionStatusDto statusDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null) return false;

        subscription.Status = statusDto.Status;

        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    private async Task<AdminSubscriptionDto> MapToAdminDto(Subscription subscription)
    {
        var placeIds = subscription.SubscriptionItems.Select(si => si.PlaceId).ToList();
        var places = new List<Place>();

        foreach (var placeId in placeIds)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);
            if (place != null) places.Add(place);
        }

        var application = await _applicationRepository.GetByIdAsync(subscription.ApplicationId);

        return new AdminSubscriptionDto
        {
            Id = subscription.Id,
            Start = subscription.Start,
            End = subscription.End,
            Status = subscription.Status,
            UserId = subscription.UserId,
            ApplicationId = subscription.ApplicationId,
            PlaceIds = placeIds,
            ApplicationName = application?.Name,
            PlaceNames = places.Select(p => p.Name).ToList()
        };
    }
}