using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class PlaceService : IPlaceService
{
    private readonly IPlaceRepository _placeRepository;

    public PlaceService(IPlaceRepository placeRepository)
    {
        _placeRepository = placeRepository;
    }

    public async Task<IEnumerable<PlaceDto>> GetAllPlacesAsync()
    {
        var places = await _placeRepository.GetAllAsync();
        return places.Select(MapToDto);
    }

    public async Task<PlaceDto?> GetPlaceByIdAsync(Guid id)
    {
        var place = await _placeRepository.GetByIdAsync(id);
        return place != null ? MapToDto(place) : null;
    }

    public async Task<PlaceDto> CreatePlaceAsync(CreatePlaceDto createDto)
    {
        var place = new Place
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name
        };

        var created = await _placeRepository.AddAsync(place);
        return MapToDto(created);
    }

    public async Task<bool> UpdatePlaceAsync(Guid id, UpdatePlaceDto updateDto)
    {
        var place = await _placeRepository.GetByIdAsync(id);
        if (place == null) return false;

        if (!string.IsNullOrWhiteSpace(updateDto.Name))
            place.Name = updateDto.Name;

        return await _placeRepository.UpdateAsync(place);
    }

    public async Task<bool> DeletePlaceAsync(Guid id)
    {
        return await _placeRepository.DeleteAsync(id);
    }

    private PlaceDto MapToDto(Place place)
    {
        return new PlaceDto
        {
            Id = place.Id,
            Name = place.Name
        };
    }
}