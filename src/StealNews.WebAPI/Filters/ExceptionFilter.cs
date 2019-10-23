using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using StealNews.Common.Logging;

namespace StealNews.WebAPI.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(ExceptionFilterAttribute));

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
        }
    }
}
