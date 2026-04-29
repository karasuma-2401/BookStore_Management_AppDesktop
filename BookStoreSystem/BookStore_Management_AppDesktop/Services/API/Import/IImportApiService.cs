using BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Services.API.Import
{
    public interface IImportApiService
    {
        Task<bool> CreateImportAsync(ImportCreateDTO dto);

        Task<List<ImportResponseDto>> GetAllImportsAsync();
    }
}
