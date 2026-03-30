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

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var c = new Category { Name = dto.Name };
            return Ok(await _service.Create(c));
        }
    }
}