using Microsoft.AspNetCore.Mvc;
using StealNews.Core.Services.Abstraction;
using StealNews.Model.Models.Service.News;

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
        public IActionResult Search([FromQuery]NewsFindFilter filter)
        {
            var news = _newsService.Find(filter);
            return Ok(news);
        }   
    }
}
