using System;
using Microsoft.Extensions.Logging;

namespace SharpMC.Log
{
    public static class LogManager
    {
        public static ILoggerFactory Factory;

        public static ILogger GetLogger(Type type)
        {
            return Factory.CreateLogger(type);
        }
    }
}