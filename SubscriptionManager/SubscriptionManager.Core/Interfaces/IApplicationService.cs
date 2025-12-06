using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IApplicationService
{
    Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync();
    Task<ApplicationDto?> GetApplicationByIdAsync(Guid id);
    Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto createDto);
    Task<bool> UpdateApplicationAsync(Guid id, UpdateApplicationDto updateDto);
    Task<bool> DeleteApplicationAsync(Guid id);
}