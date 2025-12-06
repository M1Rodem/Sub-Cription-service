using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IPlaceService
{
    Task<IEnumerable<PlaceDto>> GetAllPlacesAsync();
    Task<PlaceDto?> GetPlaceByIdAsync(Guid id);
    Task<PlaceDto> CreatePlaceAsync(CreatePlaceDto createDto);
    Task<bool> UpdatePlaceAsync(Guid id, UpdatePlaceDto updateDto);
    Task<bool> DeletePlaceAsync(Guid id);
}