using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDBContext _context;

    public CustomerService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetCustomers()
    {
        return await _context.Customers
            .Where(c => !c.IsDeleted)
            .Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                Debt = c.Debt
            })
            .ToListAsync();
    }

    public async Task<CustomerResponseDto?> GetCustomerById(int id)
    {
        return await _context.Customers
            .Where(c => c.CustomerId == id && !c.IsDeleted)
            .Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                Debt = c.Debt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerResponseDto> CreateCustomer(CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Debt = 0 // mặc định
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerResponseDto
        {
            CustomerId = customer.CustomerId,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Debt = customer.Debt
        };
    }

    public async Task<bool> UpdateCustomer(int id, CustomerUpdateDto dto)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);

        if (customer == null)
            return false;

        customer.Name = dto.Name;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.Address = dto.Address;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCustomer(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);

        if (customer == null)
            return false;

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RestoreCustomer(int id)
    {
        var customer = await _context.Customers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null || !customer.IsDeleted)
            return false;

        customer.IsDeleted = false;
        customer.DeletedAt = null;

        await _context.SaveChangesAsync();

        return true;
    }
}