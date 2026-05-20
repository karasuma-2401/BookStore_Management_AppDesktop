using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Book;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreManagement.API.Controllers
{
    [Route("book")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] int? categoryId,
            [FromQuery] int? authorId,
            [FromQuery] string? keyword,
            [FromQuery] string? sortBy = "price",
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _bookService.GetBooks(
                categoryId, authorId, keyword,
                sortBy, sortOrder,
                page, pageSize);

            return Ok(result);
        }

        // GET: api/books/id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookResponseDto>> PostBook(BookCreateDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                AuthorId = dto.AuthorId,
                ImagePath = dto.ImagePath,
                Description = dto.Description
            };

            var createdBook = await _bookService.CreateBook(book, dto.CategoryIds);

            var result = new BookResponseDto
            {
                BookId = createdBook.BookId,
                Title = createdBook.Title,
                AuthorId = createdBook.AuthorId,
                Quantity = createdBook.Quantity,
                Price = createdBook.Price,
                ImagePath = createdBook.ImagePath
            };

            return CreatedAtAction(nameof(GetBook), new { id = result.BookId }, result);
        }

        // PUT: api/books/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto dto)
        {
            var updated = await _bookService.UpdateBook(id, dto);

            if (!updated)
                return NotFound();

            return Ok(new { message = "Update successful" });
        }

        // DELETE: api/books/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBook(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}