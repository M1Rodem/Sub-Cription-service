using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BffSubscriptionsController : ControllerBase
{
    private readonly IBffSubscriptionService _bffSubscriptionService;

    public BffSubscriptionsController(IBffSubscriptionService bffSubscriptionService)
    {
        _bffSubscriptionService = bffSubscriptionService;
    }

    // GET: api/BffSubscriptions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BffSubscriptionDto>>> GetAll()
    {
        // TODO: Получать UserId из токена аутентификации
        // Пока используем временный ID для тестирования
        var temporaryUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var subscriptions = await _bffSubscriptionService.GetBffSubscriptionsAsync(temporaryUserId);
        return Ok(subscriptions);
    }

    // GET: api/BffSubscriptions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BffSubscriptionDto>> GetById(Guid id)
    {
        // TODO: Получать UserId из токена аутентификации
        var temporaryUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var subscription = await _bffSubscriptionService.GetBffSubscriptionByIdAsync(id, temporaryUserId);

        if (subscription == null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }
}