using System;
using Microsoft.Extensions.Logging;

namespace FLog.Extensions
{
    public static class FLogExtensions
    {
        public static ILoggerFactory AddFLog(this ILoggerFactory factory){
            factory.AddProvider(new FLoggerProvider());
            return factory;
        }

        public static ILoggingBuilder AddFLog(this ILoggingBuilder loggingBuilder){
            loggingBuilder.AddProvider(new FLoggerProvider());
            return loggingBuilder;
        }

        public static ILoggingBuilder AddFLog(this ILoggingBuilder loggingBuilder, Func<Exception, bool> exceptionFilter){
            loggingBuilder.AddProvider(new FLoggerProvider(exceptionFilter));
            return loggingBuilder;
        }
    }
}