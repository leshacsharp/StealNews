using Microsoft.Extensions.Logging;
using StealNews.Model.Exceptions;
using System;

namespace StealNews.Common.Logging
{
    public static class Logger 
    {
        private static ILoggerFactory _loggerFactory;

        public static void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
            _loggerFactory = loggerFactory;
        }

        public static ILogger GetLogger(Type type)
        {
            if(type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_loggerFactory == null)
            {
                throw new ObjectNotFoundException($"{nameof(ILoggerFactory)} is not found. Before create logger you should invoke Configure method!");
            }

            return _loggerFactory.CreateLogger(type);
        }
    }
}
