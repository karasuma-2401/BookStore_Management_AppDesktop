using BookStoreManagement.API.Models.DTOs;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface ISettingService
    {
        Task<IEnumerable<SettingResponseDTO>> GetAll();
        Task<SettingResponseDTO?> GetByName(string name);
        Task<SettingResponseDTO> Create(string name, string value);
        Task<SettingResponseDTO?> Update(string name, string value);
        Task<bool> Delete(string name);
        Task<int> GetInt(string key);
        Task<decimal> GetDecimal(string key);
    }
}