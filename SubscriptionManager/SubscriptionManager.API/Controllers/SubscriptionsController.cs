using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    // GET: api/subscriptions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetAll()
    {
        var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
        return Ok(subscriptions);
    }

    // GET: api/subscriptions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionDto>> GetById(Guid id)
    {
        var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);

        if (subscription == null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }

    // POST: api/subscriptions
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionDto createDto)
    {
        // Получаем или создаём тестового пользователя
        var userId = await GetOrCreateTestUser();

        var createdSubscription = await _subscriptionService.CreateSubscriptionAsync(createDto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdSubscription.Id },
            createdSubscription);
    }

    private async Task<Guid> GetOrCreateTestUser()
    {
        // TODO: Заменить на реальную аутентификацию
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Здесь можно проверить/создать пользователя в БД
        return testUserId;
    }

    // PATCH: api/subscriptions/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSubscriptionDto updateDto)
    {
        var result = await _subscriptionService.UpdateSubscriptionAsync(id, updateDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/subscriptions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _subscriptionService.DeleteSubscriptionAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/subscriptions/{id}/places
    [HttpPost("{id}/places")]
    public async Task<IActionResult> AddPlaces(Guid id, AddPlacesToSubscriptionDto addDto)
    {
        var result = await _subscriptionService.AddPlacesToSubscriptionAsync(id, addDto);

        if (!result)
        {
            return BadRequest("Не удалось добавить пиццерии");
        }

        return NoContent();
    }

    // DELETE: api/subscriptions/{id}/places
    [HttpDelete("{id}/places")]
    public async Task<IActionResult> RemovePlace(Guid id, RemovePlaceFromSubscriptionDto removeDto)
    {
        var result = await _subscriptionService.RemovePlaceFromSubscriptionAsync(id, removeDto);

        if (!result)
        {
            return BadRequest("Не удалось удалить пиццерию");
        }

        return NoContent();
    }

    // POST: api/subscriptions/{id}/restore
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(Guid id, RestoreSubscriptionDto restoreDto)
    {
        var result = await _subscriptionService.RestoreSubscriptionAsync(id, restoreDto);

        if (!result)
        {
            return BadRequest("Не удалось восстановить подписку");
        }

        return NoContent();
    }
}