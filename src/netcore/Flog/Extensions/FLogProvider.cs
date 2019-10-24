using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace FLog.Extensions
{
    internal class FLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new ConcurrentDictionary<string, ILogger>();

        private readonly Func<Exception, bool> _exceptionFilter;

        public FLoggerProvider(){
            _exceptionFilter = null;
        }

        public FLoggerProvider(Func<Exception, bool> exceptionFilter){
            _exceptionFilter = exceptionFilter;
        }

        public ILogger CreateLogger(string categoryName){
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose(){
            _loggers.Clear();
        }

        private ILogger CreateLoggerImplementation(string name){
            return new FLogLogger(name, _exceptionFilter);
        }
    }
}