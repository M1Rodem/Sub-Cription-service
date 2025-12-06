using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class AdminSubscriptionsController : ControllerBase
{
    private readonly IAdminSubscriptionService _adminSubscriptionService;

    public AdminSubscriptionsController(IAdminSubscriptionService adminSubscriptionService)
    {
        _adminSubscriptionService = adminSubscriptionService;
    }

    // GET: api/admin/adminsubscriptions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminSubscriptionDto>>> GetAll()
    {
        var subscriptions = await _adminSubscriptionService.GetAllSubscriptionsAdminAsync();
        return Ok(subscriptions);
    }

    // GET: api/admin/adminsubscriptions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AdminSubscriptionDto>> GetById(Guid id)
    {
        var subscription = await _adminSubscriptionService.GetSubscriptionByIdAdminAsync(id);

        if (subscription == null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }

    // POST: api/admin/adminsubscriptions/{id}/block
    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockSubscription(Guid id, BlockSubscriptionDto blockDto)
    {
        var result = await _adminSubscriptionService.BlockSubscriptionAsync(id, blockDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/admin/adminsubscriptions/{id}/unblock
    [HttpPost("{id}/unblock")]
    public async Task<IActionResult> UnblockSubscription(Guid id)
    {
        var result = await _adminSubscriptionService.UnblockSubscriptionAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // PATCH: api/admin/adminsubscriptions/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeSubscriptionStatusDto statusDto)
    {
        var result = await _adminSubscriptionService.ChangeSubscriptionStatusAsync(id, statusDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}