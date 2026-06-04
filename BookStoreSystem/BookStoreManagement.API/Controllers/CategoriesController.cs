using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Services.Interfaces;
using BookStoreManagement.API.DTOs.Categories;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookStoreManagement.API.Hubs; 

namespace BookStoreManagement.API.Controllers
{
    [Route("category")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IHubContext<BookHub, IBookHubClient> _hubContext;

        public CategoriesController(ICategoryService service, IHubContext<BookHub, IBookHubClient> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
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
            var result = await _service.Create(c);
            await _hubContext.Clients.All.BookUpdated(0, new { message = "Category created" });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            var category = new Category { CategoryId = id, Name = dto.Name };
            var isSuccess = await _service.Update(id, category);

            if (!isSuccess) return BadRequest("Update failed.");

            await _hubContext.Clients.All.BookUpdated(id, new { message = "Category updated" });
            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isSuccess = await _service.Delete(id);
                if (!isSuccess) return NotFound("Category not found.");

                await _hubContext.Clients.All.BookUpdated(id, new { message = "Category deleted" });
                return Ok(new { message = "Category deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CategoryUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }
}