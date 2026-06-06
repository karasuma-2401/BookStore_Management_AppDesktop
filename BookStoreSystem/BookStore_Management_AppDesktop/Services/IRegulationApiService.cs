using BookStore_Management_AppDesktop.Models.DTOs.RegulationDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IRegulationApiService
    {
        Task<List<RegulationResponseDto>> GetAllAsync();
        Task<RegulationResponseDto> GetByNameAsync(string name);
        Task<bool> CreateAsync(RegulationCreateDto dto);
        Task<bool> UpdateAsync(string name, RegulationUpdateDto dto);
        Task<bool> DeleteAsync(string name);
    }
}
