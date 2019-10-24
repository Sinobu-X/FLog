using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace FLog.Extensions
{
    internal class FLogLogger : ILogger
    {
        private readonly FLog.Logger _log;
        private readonly Func<Exception, bool> _exceptionFilter;

        public FLogLogger(string name, Func<Exception, bool> exceptionFilter){
            _log = FLog.LogManager.GetLogger(name);
            _exceptionFilter = exceptionFilter;
        }

        public IDisposable BeginScope<TState>(TState state){
            return new NoopDisposable();
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose(){
            }
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel){
            return LogManager.HasHandlers(new LogData(){
                LoggerName = _log.Name,
                Level = ConvertLogLevel(logLevel)
            });
        }

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter){
            if (!IsEnabled(logLevel)){
                return;
            }

            if (_exceptionFilter != null && _exceptionFilter(exception)){
                return;
            }

            if (formatter == null){
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = $"{formatter(state, exception)} {exception}";

            switch (logLevel){
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    _log.Critical(message);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    _log.Debug(message);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    _log.Error(message);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.None:
                    _log.Error(message);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    _log.Info(message);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    _log.Warning(message);
                    break;
                default:
                    _log.Warning($"Encountered unknown log level {logLevel}, writing out as Info.");
                    _log.Info(message, exception);
                    break;
            }
        }

        private FLog.LogLevel ConvertLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel){
            switch (logLevel){
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return FLog.LogLevel.Critical;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return FLog.LogLevel.Error;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return FLog.LogLevel.Warning;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return FLog.LogLevel.Information;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return FLog.LogLevel.Debug;
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    return FLog.LogLevel.Trace;
                case Microsoft.Extensions.Logging.LogLevel.None:
                    return FLog.LogLevel.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

//        private bool AllowDiagnostics(){
//            if (!_skipDiagnosticLogs){
//                return true;
//            }
//
//            return !(_name.ToLower().StartsWith("microsoft")
//                     || _name == "IdentityServer4.AccessTokenValidation.Infrastructure.NopAuthenticationMiddleware");
//        }
    }
}