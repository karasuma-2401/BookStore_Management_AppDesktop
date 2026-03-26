using BookStore_Management_AppDesktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public class BookApiService : IBookApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7063/api/")
        };

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
            

        public BookApiService()
        {

        }

        // GET ALL BOOK: 
        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("books");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Book>>(json, _options) ?? new List<Book>();
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
                var json = JsonSerializer.Serialize(newBook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("books", content);

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
                var response = await _httpClient.DeleteAsync($"books/{id}");
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
                var json = JsonSerializer.Serialize(updatedBook); 
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"books/{id}", content);
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
