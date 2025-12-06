using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    // GET: api/invoices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        return Ok(invoices);
    }

    // GET: api/invoices/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

        if (invoice == null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }

    // GET: api/invoices/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByUserId(Guid userId)
    {
        var invoices = await _invoiceService.GetInvoicesByUserIdAsync(userId);
        return Ok(invoices);
    }

    // POST: api/invoices
    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceDto createDto)
    {
        var createdInvoice = await _invoiceService.CreateInvoiceAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdInvoice.Id },
            createdInvoice);
    }

    // PUT: api/invoices/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateInvoiceDto updateDto)
    {
        var result = await _invoiceService.UpdateInvoiceAsync(id, updateDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/invoices/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _invoiceService.DeleteInvoiceAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}