using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookStoreManagement.API.Hubs;

namespace BookStoreManagement.API.Controllers
{
    [Route("book")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IHubContext<BookHub, IBookHubClient> _hubContext;

        public BooksController(
            IBookService bookService,
            IHubContext<BookHub, IBookHubClient> hubContext)
        {
            _bookService = bookService;
            _hubContext = hubContext;
        }

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

        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookResponseDto>> PostBook([FromBody] BookCreateDto dto)
        {
            try
            {
                var result = await _bookService.CreateBook(
                    new Models.Entities.Book
                    {
                        Title = dto.Title,
                        Description = dto.Description,
                        ImagePath = dto.ImagePath,
                        PublishYear = dto.PublishYear
                    },
                    dto.AuthorIds,
                    dto.CategoryIds
                );

                if (result == null)
                    return BadRequest("Could not create book.");

                await _hubContext.Clients.All.BookCreated(result);

                return CreatedAtAction(nameof(GetBook), new { id = result.BookId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] BookUpdateDto dto)
        {
            try
            {
                var updated = await _bookService.UpdateBook(id, dto);

                if (!updated)
                    return NotFound();

                var fresh = await _bookService.GetBookById(id);

                if (fresh != null)
                {
                    await _hubContext.Clients.All.BookUpdated(id, fresh);
                }

                return Ok(new { message = "Update successful" });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Update failed",
                    detail = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBook(id);

            if (!deleted)
                return NotFound();

            await _hubContext.Clients.All.BookDeleted(id);

            return NoContent();
        }
    }
}