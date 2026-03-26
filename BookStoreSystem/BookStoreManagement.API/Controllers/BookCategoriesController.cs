using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Services.Interfaces;
using BookStoreManagement.API.DTOs.BookCategories;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreManagement.API.Controllers
{
    [Route("bookcategory")]
    [ApiController]
    [Authorize]
    public class BookCategoriesController : ControllerBase
    {
        private readonly IBookCategoryService _service;

        public BookCategoriesController(IBookCategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCategoryCreateDto dto)
        {
            var bc = new BookCategory
            {
                BookId = dto.BookId,
                CategoryId = dto.CategoryId
            };

            return Ok(await _service.Create(bc));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int bookId, int categoryId)
        {
            var deleted = await _service.Delete(bookId, categoryId);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}