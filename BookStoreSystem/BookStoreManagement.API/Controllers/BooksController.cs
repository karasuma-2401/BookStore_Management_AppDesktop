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
        public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetBooks()
        {
            var books = await _bookService.GetBooks();

            var result = books.Select(book => new BookResponseDto
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name,
                Quantity = book.Quantity,
                ImagePath = book.ImagePath
            });

            return Ok(result);
        }

        // GET: api/books/id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            var result = new BookResponseDto
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name,
                Quantity = book.Quantity,
                ImagePath = book.ImagePath
            };

            return Ok(result);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookResponseDto>> PostBook(BookCreateDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                AuthorId = dto.AuthorId,
                Quantity = dto.Quantity,
                ImagePath = dto.ImagePath
            };

            var createdBook = await _bookService.CreateBook(book);

            var result = new BookResponseDto
            {
                BookId = createdBook.BookId,
                Title = createdBook.Title,
                AuthorId = createdBook.AuthorId,
                Quantity = createdBook.Quantity,
                ImagePath = createdBook.ImagePath
            };

            return CreatedAtAction(nameof(GetBook), new { id = result.BookId }, result);
        }

        // PUT: api/books/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto dto)
        {
            var book = new Book
            {
                BookId = id,
                Title = dto.Title,
                AuthorId = dto.AuthorId,
                Quantity = dto.Quantity,
                ImagePath = dto.ImagePath
            };

            var updated = await _bookService.UpdateBook(id, book);

            if (!updated)
                return NotFound();

            return NoContent();
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