using BookStoreManagement.API.DTOs.Authors;
using BookStoreManagement.API.Hubs;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreManagement.API.Controllers
{
    [Route("author")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _service;
        private readonly IHubContext<BookHub, IBookHubClient> _hubContext;

        public AuthorsController(IAuthorService service, IHubContext<BookHub, IBookHubClient> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorResponseDto>>> GetAll()
        {
            var authors = await _service.GetAll();
            return Ok(authors);
        }

        [HttpGet("string")]
        public async Task<IActionResult> GetAsString()
        {
            var authors = await _service.GetAll();
            return Ok(string.Join(", ", authors.Select(a => a.Name)));
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorCreateDto dto)
        {
            var author = new Author { Name = dto.Name };
            var result = await _service.Create(author);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDto dto)
        {
            var author = new Author { AuthorId = id, Name = dto.Name };
            
            var isSuccess = await _service.Update(id, author);
            if (!isSuccess)
            {
                return BadRequest("Update failed. Author ID mismatch or not found.");
            }

            await _hubContext.Clients.All.BookUpdated(0, new { message = "Author name changed" });

            return Ok(new { message = "Author updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isSuccess = await _service.Delete(id);
                if (!isSuccess)
                {
                    return NotFound(new { message = "Delete failed. Author not found." });
                }
                return Ok(new { message = "Author deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class AuthorUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }
}