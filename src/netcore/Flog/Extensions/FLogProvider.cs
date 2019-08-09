using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;

namespace FLog.Extensions
{
    internal class FLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new ConcurrentDictionary<string, ILogger>();

        public FLoggerProvider(){
        }

        public ILogger CreateLogger(string categoryName){
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose(){
            _loggers.Clear();
        }

        private ILogger CreateLoggerImplementation(string name){
            return new FLogLogger(name);
        }
    }
}