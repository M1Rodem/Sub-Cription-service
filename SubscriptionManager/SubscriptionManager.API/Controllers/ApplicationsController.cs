using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // GET: api/applications
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAll()
    {
        var applications = await _applicationService.GetAllApplicationsAsync();
        return Ok(applications);
    }

    // GET: api/applications/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);

        if (application == null)
        {
            return NotFound();
        }

        return Ok(application);
    }

    // POST: api/applications
    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> Create(CreateApplicationDto createDto)
    {
        var createdApplication = await _applicationService.CreateApplicationAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdApplication.Id },
            createdApplication);
    }

    // PUT: api/applications/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateApplicationDto updateDto)
    {
        var result = await _applicationService.UpdateApplicationAsync(id, updateDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/applications/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _applicationService.DeleteApplicationAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}