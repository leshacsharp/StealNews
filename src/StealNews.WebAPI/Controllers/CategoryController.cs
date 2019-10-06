using Microsoft.AspNetCore.Mvc;
using StealNews.Core.Services.Abstraction;
using System.Threading.Tasks;

namespace StealNews.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly INewsService _newsService;

        public CategoryController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _newsService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}