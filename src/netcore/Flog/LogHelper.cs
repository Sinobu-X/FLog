using System;
using System.Text;

namespace FLog
{
    public static class LogHelper
    {
        public static string BuildLog(LogFormatter formatter, string formatterString,
            LogRepository repository, LogData logData){
            if (formatter != null){
                return formatter(logData);
            }

            if (!string.IsNullOrEmpty(formatterString)){
                return BuildLogByString(formatterString, logData);
            }

            if (repository.Formatter != null){
                return repository.Formatter(logData);
            }

            if (!string.IsNullOrEmpty(repository.FormatterString)){
                return BuildLogByString(repository.FormatterString, logData);
            }

            if (LogManager.Formatter != null){
                return LogManager.Formatter(logData);
            }

            if (!string.IsNullOrEmpty(LogManager.FormatterString)){
                return BuildLogByString(LogManager.FormatterString, logData);
            }

            return BuildLogByString("%date [%thread] %level %logger - %message%newline", logData);
        }

        private static string BuildLogByString(string formatterString, LogData logData){
            formatterString = formatterString.Replace("%date", logData.Time.ToString("yyyy-MM-dd HH:mm:ss"));
            formatterString = formatterString.Replace("%thread", logData.ThreadId.ToString());
            formatterString = formatterString.Replace("%level", LevelToString(logData.Level));
            formatterString = formatterString.Replace("%logger", logData.LoggerName);

            var message = logData.Message;
            var ignoreErrorMessage = false;
            if (string.IsNullOrEmpty(message)){
                if (logData.Error != null){
                    message = logData.Error.Message;
                    ignoreErrorMessage = true;
                }
            }

            formatterString = formatterString.Replace("%message", message);
            formatterString = formatterString.Replace("%newline", Environment.NewLine);

            var sb = new StringBuilder();
            sb.Append(formatterString);

            if (logData.Error != null){
                if (!ignoreErrorMessage){
                    sb.AppendLine(logData.Error.Message);
                }

                sb.AppendLine(logData.Error.StackTrace);

                var innerError = logData.Error.InnerException;
                while (innerError != null){
                    sb.AppendLine(logData.Error.Message);
                    sb.AppendLine(logData.Error.StackTrace);
                    innerError = innerError.InnerException;
                }
            }

            return sb.ToString();
        }

        public static string LevelToString(LogLevel level){
            switch (level){
                case LogLevel.None:
                    return "All";
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Information:
                    return "Info";
                case LogLevel.Warning:
                    return "Warn";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Critical:
                    return "Critical";
                default:
                    return "All";
            }
        }

        public static LogLevel StringToLevel(string name){
            switch (name){
                case "All":
                    return LogLevel.None;
                case "Trace":
                    return LogLevel.Trace;
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Information;
                case "Warn":
                    return LogLevel.Warning;
                case "Error":
                    return LogLevel.Error;
                case "Critical":
                    return LogLevel.Critical;
                default:
                    return LogLevel.None;
            }
        }
    }
}