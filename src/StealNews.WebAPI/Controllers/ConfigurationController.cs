using Microsoft.AspNetCore.Mvc;
using StealNews.Core.Services.Abstraction;

namespace StealNews.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;
        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetConfiguration()
        {
            var configuration = _configurationService.Get();
            return Ok(configuration);
        }
    }
}