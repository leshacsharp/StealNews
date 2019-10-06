using Microsoft.AspNetCore.Mvc;
using StealNews.Core.Services.Abstraction;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetConfiguration()
        {
            var configuration = await _configurationService.GetAsync();
            return Ok(configuration);
        }
    }
}