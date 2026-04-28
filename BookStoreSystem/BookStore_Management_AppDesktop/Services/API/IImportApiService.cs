using BookStore_Management_AppDesktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IImportApiService
    {
        Task<bool> CreateImportAsync(ImportCreateDTO dto);
    }
}
