using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;

    public ApplicationService(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync()
    {
        var applications = await _applicationRepository.GetAllAsync();
        return applications.Select(MapToDto);
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(Guid id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        return application != null ? MapToDto(application) : null;
    }

    public async Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto createDto)
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Price = createDto.Price
        };

        var created = await _applicationRepository.AddAsync(application);
        return MapToDto(created);
    }

    public async Task<bool> UpdateApplicationAsync(Guid id, UpdateApplicationDto updateDto)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) return false;

        if (!string.IsNullOrWhiteSpace(updateDto.Name))
            application.Name = updateDto.Name;

        if (updateDto.Price.HasValue)
            application.Price = updateDto.Price.Value;

        return await _applicationRepository.UpdateAsync(application);
    }

    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
        return await _applicationRepository.DeleteAsync(id);
    }

    private ApplicationDto MapToDto(Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            Name = application.Name,
            Price = application.Price
        };
    }
}