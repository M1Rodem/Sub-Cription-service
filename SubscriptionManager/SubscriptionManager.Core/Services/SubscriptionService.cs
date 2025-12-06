using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared;
using SubscriptionManager.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Services;

public class SubscriptionService : ISubscriptionService
{    
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IHistoryRepository _historyRepository;
    private readonly IApplicationRepository _applicationRepository; // Добавляем
    private readonly IPlaceRepository _placeRepository; // Добавляем
    private readonly DateTimeProvider _dateTimeProvider;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IHistoryRepository historyRepository,
        IApplicationRepository applicationRepository, // Добавляем
        IPlaceRepository placeRepository, // Добавляем
        DateTimeProvider dateTimeProvider)
    {
        _subscriptionRepository = subscriptionRepository;
        _historyRepository = historyRepository;
        _applicationRepository = applicationRepository; // Добавляем
        _placeRepository = placeRepository; // Добавляем
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return subscriptions.Select(MapToDto);
    }

    public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        return subscription != null ? MapToDto(subscription) : null;
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto, Guid userId)
    {
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Start = _dateTimeProvider.UtcNow,
            End = _dateTimeProvider.UtcNow.AddMonths(1), // Пример: подписка на месяц
            Status = 0, // Активная
            UserId = userId,
            ApplicationId = createDto.ApplicationId,
            SubscriptionItems = createDto.PlaceIds.Select(placeId => new SubscriptionItem
            {
                Id = Guid.NewGuid(),
                PlaceId = placeId
            }).ToList()
        };

        var created = await _subscriptionRepository.AddAsync(subscription);
        return MapToDto(created);
    }

    public async Task<bool> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionDto updateDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null) return false;

        if (updateDto.Status.HasValue)
            subscription.Status = updateDto.Status.Value;

        // Здесь будет логика обновления PlaceIds (SubscriptionItems)
        // Пока оставим заглушку

        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    public async Task<bool> DeleteSubscriptionAsync(Guid id)
    {
        return await _subscriptionRepository.DeleteAsync(id);
    }

    private SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            Start = subscription.Start,
            End = subscription.End,
            Status = subscription.Status,
            UserId = subscription.UserId,
            ApplicationId = subscription.ApplicationId,
            PlaceIds = subscription.SubscriptionItems.Select(si => si.PlaceId).ToList()
        };
    }

    public async Task<bool> AddPlacesToSubscriptionAsync(Guid subscriptionId, AddPlacesToSubscriptionDto addDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
        if (subscription == null || subscription.Status != 0) // Не активная
            return false;

        // Проверяем что приложение существует
        var application = await _applicationRepository.GetByIdAsync(subscription.ApplicationId);
        if (application == null) return false;

        foreach (var placeId in addDto.PlaceIds)
        {
            // Проверяем что пиццерия существует
            var place = await _placeRepository.GetByIdAsync(placeId);
            if (place == null) continue; // Пропускаем несуществующие

            // Проверяем, нет ли уже этой пиццерии в подписке
            if (!subscription.SubscriptionItems.Any(si => si.PlaceId == placeId))
            {
                // Сначала добавляем SubscriptionItem
                subscription.SubscriptionItems.Add(new SubscriptionItem
                {
                    Id = Guid.NewGuid(),
                    PlaceId = placeId
                });

                // Потом создаём History запись
                var history = new History
                {
                    Id = Guid.NewGuid(),
                    PlaceId = placeId,
                    Start = DateOnly.FromDateTime(_dateTimeProvider.UtcNow),
                    End = null, // Активный период
                    ApplicationId = subscription.ApplicationId,
                    SubscriptionId = subscription.Id
                };

                // Сохраняем History
                await _historyRepository.AddAsync(history);
            }
        }

        // Сохраняем изменения в подписке
        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    // Новый метод: Удалить пиццерию из подписки
    public async Task<bool> RemovePlaceFromSubscriptionAsync(Guid subscriptionId, RemovePlaceFromSubscriptionDto removeDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
        if (subscription == null || subscription.Status != 0) // Не активная
            return false;

        // Находим и удаляем SubscriptionItem
        var subscriptionItem = subscription.SubscriptionItems
            .FirstOrDefault(si => si.PlaceId == removeDto.PlaceId);

        if (subscriptionItem == null) return false;

        subscription.SubscriptionItems.Remove(subscriptionItem);

        // Закрываем период в истории
        var activeHistory = await _historyRepository.GetActiveBySubscriptionAndPlaceAsync(
            subscriptionId, removeDto.PlaceId);

        if (activeHistory != null)
        {
            activeHistory.End = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
            await _historyRepository.UpdateAsync(activeHistory);
        }

        return await _subscriptionRepository.UpdateAsync(subscription);
    }

    // Новый метод: Восстановить отменённую подписку
    public async Task<bool> RestoreSubscriptionAsync(Guid subscriptionId, RestoreSubscriptionDto restoreDto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
        if (subscription == null || subscription.Status != 1) // Не отменённая
            return false;

        // Меняем статус на активный
        subscription.Status = 0;
        subscription.Start = _dateTimeProvider.UtcNow;
        subscription.End = _dateTimeProvider.UtcNow.AddMonths(1);

        // Очищаем старые пиццерии и добавляем новые
        subscription.SubscriptionItems.Clear();
        foreach (var placeId in restoreDto.PlaceIds)
        {
            subscription.SubscriptionItems.Add(new SubscriptionItem
            {
                Id = Guid.NewGuid(),
                PlaceId = placeId
            });

            // Записываем в историю новый период
            await _historyRepository.AddAsync(new History
            {
                Id = Guid.NewGuid(),
                PlaceId = placeId,
                Start = DateOnly.FromDateTime(_dateTimeProvider.UtcNow),
                End = null,
                ApplicationId = subscription.ApplicationId,
                SubscriptionId = subscription.Id
            });
        }

        return await _subscriptionRepository.UpdateAsync(subscription);
    }
}
