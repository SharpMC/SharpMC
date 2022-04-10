using System;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.LoggerFactory;

namespace SharpMC.Logging
{
    public static class LogManager
    {
        private static readonly ILoggerFactory Log;

        static LogManager()
        {
            Log = Create(f => f.AddConsole());
        }

        public static ILogger GetLogger(Type type)
        {
            return Log.CreateLogger(type);
        }
    }
}