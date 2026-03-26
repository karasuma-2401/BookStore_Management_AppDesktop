using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Services.Interfaces;
using BookStoreManagement.API.DTOs.Authors;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreManagement.API.Controllers
{
    [Route("author")]
    [ApiController]
    //[Authorize]
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

        [HttpPost]
        public async Task<IActionResult> Create(AuthorCreateDto dto)
        {
            var author = new Author { Name = dto.Name };
            var result = await _service.Create(author);
            return Ok(result);
        }
    }
}