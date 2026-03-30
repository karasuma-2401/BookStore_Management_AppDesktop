using BookStore_Management_AppDesktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BookStore_Management_AppDesktop.Models.DTOs;
using System.Linq;
using System.Net.Http.Headers;
namespace BookStore_Management_AppDesktop.Services.API
{
    public class BookApiService : IBookApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7063/")
        };  

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
            
        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // GET ALL BOOK: 
        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync("book");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var dtos = JsonSerializer.Deserialize<List<BookResponseDto>>(json, _options) ?? new List<BookResponseDto>();


                return dtos.Select(dto => new Book
                {
                    BookId = dto.BookId,
                    Title = dto.Title,
                    AuthorId = dto.AuthorId,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    ImagePath = dto.ImagePath
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllBooks Error: {ex.Message}");
                return new List<Book>();
            }
        }

        // CREATE  new BOOK: 
        public async Task<bool> CreateBookAsync(Book newBook)
        {
            try
            {
                AddAuthorizationHeader();
                var createDto = new BookCreateDto
                {
                    Title = newBook.Title,
                    AuthorId = newBook.AuthorId,
                    Price = newBook.Price,
                    Quantity = newBook.Quantity,
                    ImagePath = newBook.ImagePath
                };

                var json = JsonSerializer.Serialize(createDto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("book", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateBook Error: {ex.Message}");
                return false;
            }
        }

        // DELETE: 
        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"book/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteBook Error: {ex.Message}");
                return false;
            }
        }


        // UPDATE: 
        public async Task<bool> UpdateBookAsync(int id, Book updatedBook)
        {
            try
            {
                AddAuthorizationHeader();
                var updateDto = new BookUpdateDto
                {
                    BookId = id,
                    Title = updatedBook.Title,
                    AuthorId = updatedBook.AuthorId,
                    Price = updatedBook.Price,
                    Quantity = updatedBook.Quantity,
                    ImagePath = updatedBook.ImagePath
                };

                var json = JsonSerializer.Serialize(updateDto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"book/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateBook Error: {ex.Message}");
                return false;
            }
        }
    }
}
