public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetCustomers();
    Task<CustomerResponseDto?> GetCustomerById(int id);
    Task<CustomerResponseDto> CreateCustomer(CustomerCreateDto dto);
    Task<IEnumerable<CustomerResponseDto>> SearchCustomers(string? keyword);
    Task<bool> UpdateCustomer(int id, CustomerUpdateDto dto);
    Task<bool> DeleteCustomer(int id);
    Task<bool>RestoreCustomer(int id);
}