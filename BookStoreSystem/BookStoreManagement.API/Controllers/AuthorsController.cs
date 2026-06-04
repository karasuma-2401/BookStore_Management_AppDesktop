using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Services.Interfaces;
using BookStoreManagement.API.DTOs.Authors;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace BookStoreManagement.API.Controllers
{
    [Route("author")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _service;

        public AuthorsController(IAuthorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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
            var author = new Author
            {
                AuthorId = id,
                Name = dto.Name
            };

            var isSuccess = await _service.Update(id, author);
            if (!isSuccess)
            {
                return BadRequest("Update failed. Author ID mismatch or not found.");
            }

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