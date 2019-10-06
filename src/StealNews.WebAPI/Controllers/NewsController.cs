using Microsoft.AspNetCore.Mvc;
using StealNews.Core.Services.Abstraction;
using StealNews.Model.Models.Service;
using System.Threading.Tasks;

namespace StealNews.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Search([FromQuery]NewsFindFilter filter)
        {
            var news = await _newsService.FindAsync(filter);
            return Ok(news);
        }   
    }
}
