using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.BookServices
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

        public async Task<PagedResponse<Book>> GetAllBooksAsync(BookQueryParameters queryParams, CancellationToken ct = default)
        {
            try
            {
                AddAuthorizationHeader();

                var query = new List<string>();
                if (queryParams.CategoryId.HasValue) query.Add($"categoryId={queryParams.CategoryId.Value}");
                if (queryParams.AuthorId.HasValue) query.Add($"authorId={queryParams.AuthorId.Value}");
                if (!string.IsNullOrWhiteSpace(queryParams.Keyword)) query.Add($"keyword={Uri.EscapeDataString(queryParams.Keyword)}");
                if (!string.IsNullOrWhiteSpace(queryParams.SortBy)) query.Add($"sortBy={queryParams.SortBy}");
                if (!string.IsNullOrWhiteSpace(queryParams.SortOrder)) query.Add($"sortOrder={queryParams.SortOrder}");
                if (queryParams.IncludeOutOfStock) query.Add($"includeOutOfStock={queryParams.IncludeOutOfStock}");

                query.Add($"page={queryParams.PageNumber}");
                query.Add($"pageSize={queryParams.PageSize}");

                string queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
                string finalUrl = $"book{queryString}";

                var response = await _httpClient.GetAsync(finalUrl, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);

                var pagedResult = JsonSerializer.Deserialize<PagedResponse<BookResponseDto>>(json, _options);

                if (pagedResult == null) return new PagedResponse<Book>();

                var books = pagedResult.Data.Select(dto => new Book
                {
                    BookId = dto.BookId,
                    Title = dto.Title,
                    AuthorIds = dto.AuthorIds ?? new List<int>(),
                    AuthorNames = dto.AuthorNames ?? new List<string>(),
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    ImagePath = dto.ImagePath,
                    Description = dto.Description,
                    PublishYear = dto.PublishYear,
                    CategoryIds = dto.CategoryIds ?? new List<int>(),
                    CategoryNames = dto.CategoryNames ?? new List<string>() 
                }).ToList();

                return new PagedResponse<Book>
                {
                    Page = pagedResult.Page,
                    PageSize = pagedResult.PageSize,
                    TotalItems = pagedResult.TotalItems,
                    TotalPages = pagedResult.TotalPages,
                    Data = books
                };
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("The old request has been cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllBooks Error: {ex.Message}");
                return new PagedResponse<Book>();
            }
        }

        public async Task<Book?> CreateBookAsync(Book newBook)
        {
            try
            {
                AddAuthorizationHeader();

                var createDto = new
                {
                    Title = newBook.Title ?? string.Empty,
                    AuthorIds = newBook.AuthorIds ?? new List<int>(),
                    PublishYear = newBook.PublishYear,
                    Description = newBook.Description ?? string.Empty,
                    ImagePath = newBook.ImagePath ?? string.Empty,
                    CategoryIds = newBook.CategoryIds ?? new List<int>()
                };

                var json = JsonSerializer.Serialize(createDto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("book", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var dto = JsonSerializer.Deserialize<BookResponseDto>(jsonResponse, _options);
                    if (dto != null)
                    {
                        return new Book
                        {
                            BookId = dto.BookId,
                            Title = dto.Title,
                            AuthorIds = dto.AuthorIds ?? new List<int>(),
                            AuthorNames = dto.AuthorNames ?? new List<string>(),
                            Price = dto.Price,
                            Quantity = dto.Quantity,
                            ImagePath = dto.ImagePath,
                            Description = dto.Description,
                            PublishYear = dto.PublishYear,
                            CategoryIds = dto.CategoryIds ?? new List<int>(),
                            CategoryNames = dto.CategoryNames ?? new List<string>()
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[API Fail] CreateBook Failed: {response.StatusCode} - {errorContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRASH] CreateBook Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync($"book/{id}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<BookResponseDto>(json, _options);

                if (dto == null) return null;

                return new Book
                {
                    BookId = dto.BookId,
                    Title = dto.Title,
                    AuthorIds = dto.AuthorIds ?? new List<int>(),
                    AuthorNames = dto.AuthorNames ?? new List<string>(),
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    ImagePath = dto.ImagePath,
                    Description = dto.Description,
                    PublishYear = dto.PublishYear,
                    CategoryIds = dto.CategoryIds ?? new List<int>(),
                    CategoryNames = dto.CategoryNames ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBookById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, Book updatedBook)
        {
            try
            {
                AddAuthorizationHeader();

                var updateDto = new
                {
                    Title = updatedBook.Title ?? string.Empty,
                    AuthorIds = updatedBook.AuthorIds ?? new List<int>(),
                    PublishYear = updatedBook.PublishYear,
                    Quantity = updatedBook.Quantity,
                    // BUG đã fix: bổ sung Price vào payload gửi lên server.
                    // Trước đây client không gửi field này => server không lưu được giá bán
                    // khi admin edit sách => dẫn đến "giá bán hiển thị nhầm bằng giá nhập" hoặc giữ giá cũ.
                    Price = updatedBook.Price,
                    Description = updatedBook.Description ?? string.Empty,
                    ImagePath = updatedBook.ImagePath ?? string.Empty,
                    CategoryIds = updatedBook.CategoryIds ?? new List<int>()
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
    }
}