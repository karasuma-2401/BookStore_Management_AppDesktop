using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Interfaces.Services;
namespace BookStoreManagement.API.Controllers
{
    [Route("import")]
    [Authorize]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _service;

        public ImportsController(IImportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ImportCreateDto dto)
        {
            var result = await _service.CreateImport(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetImports();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetImportById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}