using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("customer")]
[ApiController]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET /customer
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await _customerService.GetCustomers();
        return Ok(result);
    }

    // GET /customer/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerById(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    // GET /customer/search?keyword=abc
    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string? keyword)
    {
        var result = await _customerService.SearchCustomers(keyword);
        return Ok(result);
    }

    // POST /customer
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CustomerCreateDto dto)
    {
        var customer = await _customerService.CreateCustomer(dto);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
    }

    // PUT /customer/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateDto dto)
    {
        var updated = await _customerService.UpdateCustomer(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    // DELETE /customer/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await _customerService.DeleteCustomer(id);
        
        if (!deleted)
            return NotFound();

        return NoContent();
    }
    // PATCH /customer/restore/1
    [HttpPatch("restore/{id}")]
    public async Task<IActionResult> RestoreCustomer(int id)
    {
        var restored = await _customerService.RestoreCustomer(id);

        if (!restored)
            return NotFound();

        return Ok(new { message = "Customer restored successfully" });
    }
}