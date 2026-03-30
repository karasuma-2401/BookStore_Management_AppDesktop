using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Services.Interfaces;
using BookStoreManagement.API.DTOs.Categories;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreManagement.API.Controllers
{
    [Route("category")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }
        [HttpGet("string")]
        public async Task<IActionResult> GetAsString()
        {
            var categories = await _service.GetAll();
            return Ok(string.Join(", ", categories.Select(c => c.Name)));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var c = new Category { Name = dto.Name };
            return Ok(await _service.Create(c));
        }
    }
}