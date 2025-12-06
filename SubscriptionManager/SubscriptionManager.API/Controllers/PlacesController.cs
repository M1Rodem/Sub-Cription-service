using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacesController : ControllerBase
{
    private readonly IPlaceService _placeService;

    public PlacesController(IPlaceService placeService)
    {
        _placeService = placeService;
    }

    // GET: api/places
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetAll()
    {
        var places = await _placeService.GetAllPlacesAsync();
        return Ok(places);
    }

    // GET: api/places/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PlaceDto>> GetById(Guid id)
    {
        var place = await _placeService.GetPlaceByIdAsync(id);

        if (place == null)
        {
            return NotFound();
        }

        return Ok(place);
    }

    // POST: api/places
    [HttpPost]
    public async Task<ActionResult<PlaceDto>> Create(CreatePlaceDto createDto)
    {
        var createdPlace = await _placeService.CreatePlaceAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPlace.Id },
            createdPlace);
    }

    // PUT: api/places/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdatePlaceDto updateDto)
    {
        var result = await _placeService.UpdatePlaceAsync(id, updateDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/places/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _placeService.DeletePlaceAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}