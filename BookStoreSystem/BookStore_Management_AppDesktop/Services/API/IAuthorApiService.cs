using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IAuthorApiService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();

        Task<Author?> CreateAuthorAsync(AuthorCreateDto newAuthorDto);
    }
}
