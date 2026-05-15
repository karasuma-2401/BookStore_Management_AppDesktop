using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;

namespace BookStore_Management_AppDesktop.Services.API.CustomerServices
{
    public interface ICustomerApiService
    {
        Task<List<CustomerResponseDto>?> GetAllCustomersAsync();
        Task<CustomerResponseDto?> GetCustomerByIdAsync(int id);
        Task<CustomerResponseDto?> CreateCustomerAsync(CustomerCreateDto customer);
        Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDto customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
